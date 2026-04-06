# Agent Access Control System

## Overview
This document explains the agent role-based access control system implemented in the EstateFlow application. Agents can now only access pages and functionalities that are appropriate for their role.

## Implementation Details

### 1. **Authorization Attributes** (`RealEstate/Attributes/AuthorizationAttributes.cs`)
- `RequireAuthenticationAttribute` - Requires user to be logged in
- `RequireRoleAttribute` - Restricts access to specific roles
- `RequireAgentAttribute` - Restricts access to agents only
- `RequireManagerAttribute` - Restricts access to managers and super admins
- `RequireInvestorAttribute` - Restricts access to investors only
- `RequireAdminAttribute` - Restricts access to super admins and managers

### 2. **Authorization Helper** (`RealEstate/Helpers/AuthorizationHelper.cs`)
Static helper class with methods to check:
- `IsUserAuthenticated()` - Check if user is logged in
- `GetUserRole()` - Get current user's role
- `GetUserId()` - Get current user's ID
- `HasRole()` - Check if user has specific role(s)
- `IsAgent()` - Check if user is an agent
- `IsManager()` - Check if user is a manager
- `IsInvestor()` - Check if user is an investor
- `IsAdmin()` - Check if user is admin (SuperAdmin or Manager)

### 3. **Agent Controller** (`RealEstate/Controllers/AgentController.cs`)
The main controller for agent functionality with the `RequireAgent` attribute on the class level.

**Routes:**
- `GET /agent/dashboard` - Agent dashboard
- `GET /agent/myproperties` - View agent's own properties only
- `GET /agent/inquiries` - View inquiries for agent's properties
- `GET /agent/clients` - View agent's clients
- `GET /agent/profile` - View/edit agent profile
- `POST /agent/logout` - Logout

### 4. **Agent Views**
All agent views are located in `RealEstate/Views/Agent/`:
- `Dashboard.cshtml` - Main dashboard with quick stats and navigation
- `MyProperties.cshtml` - Display only the agent's properties
- `Inquiries.cshtml` - Show inquiries for agent's properties only
- `Clients.cshtml` - Display agent's client relationships
- `Profile.cshtml` - Agent profile and performance metrics

## How Access Control Works

### Login Flow
1. User logs in via `/admin/login`
2. `AdminController.Login()` authenticates user and checks their role
3. Based on role, user is redirected to appropriate dashboard:
   - **Agent** → `/agent/dashboard`
   - **Manager** → `/manager/dashboard`
   - **Investor** → `/investor/dashboard`
   - **Accounting** → `/accounting/dashboard`
   - **Broker** → `/broker/dashboard`
   - **SuperAdmin** → `/superadmin/dashboard`

### Session-Based Authorization
The system uses ASP.NET Core Sessions to store:
- `UserId` - Current user's ID
- `UserEmail` - User's email
- `UserName` - User's full name
- `UserRole` - User's role type

### URL Protection
Each controller/action with the `[RequireAgent]` attribute:
1. Checks if user is authenticated
2. Checks if user has "Agent" role
3. If not authorized, redirects to `/admin/login`

## Agent Restrictions

### Agents can ONLY access:
- `/agent/*` routes
- `/Properties/Details` (public property details)
- `/Properties/Index` (search/browse properties)

### Agents CANNOT access:
- `/manager/*` - Manager dashboard and functions
- `/investor/*` - Investor dashboard and functions
- `/accounting/*` - Accounting dashboard and functions
- `/admin/dashboard` - Admin dashboard
- `/admin/*` - Admin functions (except login/logout)

### Data Filtering
In `MyProperties` action, properties are filtered by agent ID:
```csharp
var allProperties = _catalog.GetProperties()
    .Where(p => p.AgentId == agentId.Value)
    .ToList();
```

This ensures agents can only see and manage their own properties.

## Implementation in Controllers

### Using RequireAgent Attribute
```csharp
[Route("agent")]
[RequireAgent]
public class AgentController : Controller
{
    // All actions in this controller require Agent role
}
```

### Using Authorization Helper in Actions
```csharp
[HttpGet("myproperties")]
public IActionResult MyProperties()
{
    var agentId = AuthorizationHelper.GetUserId(HttpContext);
    if (!agentId.HasValue)
        return RedirectToAction("Login", "Admin");
        
    // Get only this agent's properties
    var properties = _catalog.GetProperties()
        .Where(p => p.AgentId == agentId.Value)
        .ToList();
    
    return View(properties);
}
```

## Logout
Agents can logout by posting to `/agent/logout`, which:
1. Clears the session
2. Redirects to `/admin/login`

## Future Enhancements

### Potential additions:
1. **Property Type Restrictions** - Limit agents to specific property types
2. **Geographic Restrictions** - Limit agents to specific areas/barangays
3. **Activity Logging** - Log agent actions for audit trail
4. **Permission Levels** - Create sub-roles (Senior Agent, Junior Agent, etc.)
5. **Time-based Restrictions** - Restrict access during certain hours
6. **IP Whitelisting** - Restrict access from specific IP addresses

## Testing Access Control

### To test agent access:
1. Create an agent user via `/admin/register` with role "Agent"
2. Login as agent with their credentials
3. Verify redirected to `/agent/dashboard`
4. Try to access `/manager/dashboard` - should get 403 Forbidden
5. Try to access `/investor/dashboard` - should get 403 Forbidden
6. Verify agent can only see their own properties in `/agent/myproperties`

## Troubleshooting

### Agent can still access all pages?
- Clear browser cache and cookies
- Verify role in database is exactly "Agent" (case-sensitive)
- Check that `RequireAgent` attribute is present on controller/action
- Verify session is being set correctly in `AdminController.Login()`

### Agents see other agents' properties?
- Check that filtering by `agentId` is implemented
- Verify `Property.AgentId` is set correctly when properties are created
- Check SQL database to confirm property assignments

### Can't login as agent?
- Verify user exists in database with "Agent" role
- Check password hash is correct
- Verify email/password combination in login form
