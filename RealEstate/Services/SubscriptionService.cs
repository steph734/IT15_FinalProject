using RealEstate.Models;

namespace RealEstate.Services;

public enum SubscriptionTier { Free, Plus, Premium }

public class SubscriptionService
{
    // In-memory subscription store for demo; keyed by a session id or username
    private readonly Dictionary<string, SubscriptionTier> _subscriptions = new();
    private readonly Dictionary<int, DateTime> _trialEnds = new();

    public void SetSubscription(string key, SubscriptionTier tier)
    {
        if (string.IsNullOrEmpty(key)) return;
        _subscriptions[key] = tier;
        if (tier != SubscriptionTier.Free)
        {
            // set a 14-day trial end for demo users
            _trialEnds[key.GetHashCode()] = DateTime.UtcNow.AddDays(14);
        }
    }

    public SubscriptionTier GetSubscription(string key)
    {
        if (string.IsNullOrEmpty(key)) return SubscriptionTier.Free;
        return _subscriptions.TryGetValue(key, out var t) ? t : SubscriptionTier.Free;
    }

    public bool IsNoAds(string key) => GetSubscription(key) != SubscriptionTier.Free;
    public bool HasPriorityNotifications(string key) => GetSubscription(key) != SubscriptionTier.Free;
    public bool HasPremiumMapData(string key) => GetSubscription(key) == SubscriptionTier.Premium;
    public bool CanMessageOwners(string key) => GetSubscription(key) == SubscriptionTier.Premium;

    public DateTime? GetTrialEnd(string key)
    {
        if (_trialEnds.TryGetValue(key.GetHashCode(), out var dt)) return dt;
        return null;
    }
}
