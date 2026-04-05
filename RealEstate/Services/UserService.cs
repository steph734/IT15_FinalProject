using RealEstate.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace RealEstate.Services;

public class UserService
{
    private readonly ApplicationDBContext _context;

    public UserService(ApplicationDBContext context)
    {
        _context = context;
        SeedRoles();
    }

    private void SeedRoles()
    {
        // Check if roles exist
        if (_context.Roles.Any())
            return;

        // Add default roles
        _context.Roles.AddRange(
            new Role { RoleType = "SuperAdmin" },
            new Role { RoleType = "Manager" },
            new Role { RoleType = "Accounting" },
            new Role { RoleType = "Investor" },
            new Role { RoleType = "Broker" }
        );

        _context.SaveChanges();
    }

    // Get user by ID
    public User? GetUserById(int userId)
    {
        return _context.Users.Include(u => u.Role).FirstOrDefault(u => u.UserId == userId);
    }

    // Get user by email
    public User? GetUserByEmail(string email)
    {
        return _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Email == email);
    }

    // Get all users of specific role
    public IReadOnlyList<User> GetUsersByRole(string roleType)
    {
        return _context.Users
            .Include(u => u.Role)
            .Where(u => u.Role != null && u.Role.RoleType == roleType)
            .ToList()
            .AsReadOnly();
    }

    // Get all active users
    public IReadOnlyList<User> GetActiveUsers()
    {
        return _context.Users
            .Include(u => u.Role)
            .Where(u => u.IsActive)
            .ToList()
            .AsReadOnly();
    }

    // Get all users
    public IReadOnlyList<User> GetAllUsers()
    {
        return _context.Users
            .Include(u => u.Role)
            .ToList()
            .AsReadOnly();
    }

    // Register new user
    public User RegisterUser(string fullName, string email, string password, string roleType)
    {
        // Validate
        if (_context.Users.Any(u => u.Email == email))
            throw new InvalidOperationException("Email already registered");

        if (password.Length < 6)
            throw new InvalidOperationException("Password must be at least 6 characters");

        // Get role
        var role = _context.Roles.FirstOrDefault(r => r.RoleType == roleType);
        if (role == null)
            throw new InvalidOperationException($"Role '{roleType}' does not exist");

        // Create user
        var user = new User
        {
            FullName = fullName,
            Email = email,
            PasswordHash = HashPassword(password),
            RoleId = role.RoleId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return user;
    }

    // Login user
    public User? LoginUser(string email, string password)
    {
        var user = GetUserByEmail(email);

        if (user == null || !VerifyPassword(password, user.PasswordHash))
            return null;

        if (!user.IsActive)
            return null;

        // Update last login
        user.LastLogin = DateTime.UtcNow;
        _context.SaveChanges();

        return user;
    }

    // Update user
    public void UpdateUser(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    // Change password
    public bool ChangePassword(int userId, string oldPassword, string newPassword)
    {
        var user = GetUserById(userId);
        if (user == null)
            return false;

        if (!VerifyPassword(oldPassword, user.PasswordHash))
            return false;

        if (newPassword.Length < 6)
            return false;

        user.PasswordHash = HashPassword(newPassword);
        UpdateUser(user);

        return true;
    }

    // Activate user
    public void ActivateUser(int userId)
    {
        var user = GetUserById(userId);
        if (user != null)
        {
            user.IsActive = true;
            UpdateUser(user);
        }
    }

    // Deactivate user
    public void DeactivateUser(int userId)
    {
        var user = GetUserById(userId);
        if (user != null)
        {
            user.IsActive = false;
            UpdateUser(user);
        }
    }

    // Delete user
    public void DeleteUser(int userId)
    {
        var user = GetUserById(userId);
        if (user != null)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }

    // Get user count by role
    public int GetUserCountByRole(string roleType)
    {
        return _context.Users
            .Where(u => u.Role != null && u.Role.RoleType == roleType)
            .Count();
    }

    // Get total active users
    public int GetActiveUserCount()
    {
        return _context.Users.Count(u => u.IsActive);
    }

    // Get all roles
    public IReadOnlyList<Role> GetAllRoles()
    {
        return _context.Roles.ToList().AsReadOnly();
    }

    // Hash password using SHA256
    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    // Verify password
    private bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hash;
    }
}
