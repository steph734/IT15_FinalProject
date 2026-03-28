namespace RealEstate.Models.Admin;

public class AdminAuthOptions
{
    public const string SectionName = "AdminAuth";

    /// <summary>Optional multi-account list (brokers, admins).</summary>
    public List<AdminCredential> Accounts { get; set; } = new();

    /// <summary>Legacy single-account binding when Accounts is empty.</summary>
    public string? Username { get; set; }

    public string? Password { get; set; }

    public bool TryValidate(string username, string password, out string? normalizedUsername)
    {
        normalizedUsername = null;
        var u = username.Trim();

        if (Accounts.Count > 0)
        {
            foreach (var a in Accounts)
            {
                if (string.IsNullOrWhiteSpace(a.Username))
                    continue;
                if (!string.Equals(u, a.Username.Trim(), StringComparison.OrdinalIgnoreCase))
                    continue;
                if (!string.Equals(password, a.Password, StringComparison.Ordinal))
                    continue;
                normalizedUsername = a.Username.Trim();
                return true;
            }
        }

        if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
        {
            if (string.Equals(u, Username.Trim(), StringComparison.OrdinalIgnoreCase) &&
                string.Equals(password, Password, StringComparison.Ordinal))
            {
                normalizedUsername = Username.Trim();
                return true;
            }
        }

        return false;
    }
}
