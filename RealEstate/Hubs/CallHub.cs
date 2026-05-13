using Microsoft.AspNetCore.SignalR;

namespace RealEstate.Hubs
{
    /// <summary>
    /// SignalR Hub for managing WebRTC video calls between customers and support agents
    /// </summary>
    public class CallHub : Hub
    {
        // Static dictionaries to track available agents and active calls
        // In production, use Redis or a Database for scalability
        private static readonly Dictionary<string, AgentInfo> AvailableAgents = new();
        private static readonly Dictionary<string, string> ActiveCalls = new(); // CustomerConnectionId -> AgentConnectionId
        private static readonly Dictionary<string, string> UserRoles = new(); // ConnectionId -> Role ("agent" or "customer")

        /// <summary>
        /// Information about an available agent
        /// </summary>
        public class AgentInfo
        {
            public string PeerId { get; set; } = string.Empty;
            public string Name { get; set; } = "Support Agent";
            public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        }

        /// <summary>
        /// Register a support agent with their PeerJS ID
        /// </summary>
        public async Task RegisterAsAgent(string peerId, string agentName = "Support Agent")
        {
            AvailableAgents[Context.ConnectionId] = new AgentInfo
            {
                PeerId = peerId,
                Name = agentName,
                RegisteredAt = DateTime.UtcNow
            };
            
            UserRoles[Context.ConnectionId] = "agent";
            
            await Groups.AddToGroupAsync(Context.ConnectionId, "Agents");
            await Clients.Caller.SendAsync("AgentRegistered", peerId);
            
            Console.WriteLine($"Agent registered: {peerId} (Connection: {Context.ConnectionId})");
        }

        /// <summary>
        /// Customer requests an available agent
        /// </summary>
        public async Task RequestAgent()
        {
            UserRoles[Context.ConnectionId] = "customer";
            
            var agent = AvailableAgents.FirstOrDefault();
            if (agent.Value != null)
            {
                var agentId = agent.Key;
                var agentInfo = agent.Value;
                
                // Track this active call
                ActiveCalls[Context.ConnectionId] = agentId;
                
                // Remove agent from available list
                AvailableAgents.Remove(agentId);
                
                // Notify customer with agent's PeerID
                await Clients.Caller.SendAsync("ReceiveAgentId", agentInfo.PeerId, agentInfo.Name);
                
                // Notify agent about incoming call
                await Clients.Client(agentId).SendAsync("IncomingCall", Context.ConnectionId);
                
                Console.WriteLine($"Call established: Customer {Context.ConnectionId} -> Agent {agentId}");
            }
            else
            {
                await Clients.Caller.SendAsync("NoAgentsAvailable", "All agents are currently busy. Please try again later or send us an email.");
            }
        }

        /// <summary>
        /// Agent accepts the incoming call
        /// </summary>
        public async Task AcceptCall(string customerConnectionId)
        {
            await Clients.Client(customerConnectionId).SendAsync("CallAccepted", Context.ConnectionId);
            Console.WriteLine($"Call accepted by agent {Context.ConnectionId} for customer {customerConnectionId}");
        }

        /// <summary>
        /// Agent rejects the incoming call
        /// </summary>
        public async Task RejectCall(string customerConnectionId, string reason = "Agent unavailable")
        {
            await Clients.Client(customerConnectionId).SendAsync("CallRejected", reason);
            
            // Put agent back in available list
            if (ActiveCalls.ContainsKey(customerConnectionId))
            {
                var agentId = ActiveCalls[customerConnectionId];
                if (AvailableAgents.ContainsKey(agentId))
                {
                    // Agent is still available, make them available again
                    // This shouldn't happen in normal flow, but handle edge case
                }
                ActiveCalls.Remove(customerConnectionId);
            }
            
            Console.WriteLine($"Call rejected by agent {Context.ConnectionId} for customer {customerConnectionId}");
        }

        /// <summary>
        /// End an active call
        /// </summary>
        public async Task EndCall(string targetConnectionId)
        {
            await Clients.Client(targetConnectionId).SendAsync("CallEnded", "The call has ended.");
            
            // Clean up active call tracking
            var customerEntry = ActiveCalls.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (!string.IsNullOrEmpty(customerEntry.Key))
            {
                ActiveCalls.Remove(customerEntry.Key);
            }
            
            Console.WriteLine($"Call ended between {Context.ConnectionId} and {targetConnectionId}");
        }

        /// <summary>
        /// Send signaling data (ICE candidates, session descriptions)
        /// </summary>
        public async Task SendSignal(string targetConnectionId, string signalData)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveSignal", Context.ConnectionId, signalData);
        }

        /// <summary>
        /// Customer sends chat message during call
        /// </summary>
        public async Task SendChatMessage(string targetConnectionId, string message)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveChatMessage", message, DateTime.UtcNow.ToString("HH:mm"));
        }

        /// <summary>
        /// Get count of available agents
        /// </summary>
        public async Task GetAvailableAgentCount()
        {
            var count = AvailableAgents.Count;
            await Clients.Caller.SendAsync("AgentCountUpdate", count);
        }

        /// <summary>
        /// Handle disconnection - clean up
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            
            // Remove from available agents if they were an agent
            if (AvailableAgents.ContainsKey(connectionId))
            {
                AvailableAgents.Remove(connectionId);
                Console.WriteLine($"Agent disconnected: {connectionId}");
            }
            
            // Handle active call disconnection
            if (ActiveCalls.ContainsKey(connectionId))
            {
                // This was a customer who disconnected
                var agentId = ActiveCalls[connectionId];
                await Clients.Client(agentId).SendAsync("CallEnded", "Customer disconnected.");
                ActiveCalls.Remove(connectionId);
            }
            else
            {
                // Check if this was an agent in an active call
                var customerEntry = ActiveCalls.FirstOrDefault(x => x.Value == connectionId);
                if (!string.IsNullOrEmpty(customerEntry.Key))
                {
                    await Clients.Client(customerEntry.Key).SendAsync("CallEnded", "Agent disconnected.");
                    ActiveCalls.Remove(customerEntry.Key);
                }
            }
            
            // Clean up role tracking
            UserRoles.Remove(connectionId);
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}
