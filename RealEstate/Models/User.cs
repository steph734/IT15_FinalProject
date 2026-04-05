namespace RealEstate.Models;

public class Role
{
    public int RoleId { get; set; }
    public string RoleType { get; set; } = string.Empty;  // Manager, SuperAdmin, Investor, Broker, Accounting
    
    // Navigation property
    public ICollection<User> Users { get; set; } = new List<User>();
}

public class User
{
    public int UserId { get; set; }
    public int RoleId { get; set; }  // Foreign Key
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation property
    public Role? Role { get; set; }
}
