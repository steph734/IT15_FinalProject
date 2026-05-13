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
    }

    // Get user by ID
    public ApplicationUser? GetUserById(int userId)
    {
        return _context.Users.FirstOrDefault(u => u.UserId == userId);
    }

    // Get user by email
    public ApplicationUser? GetUserByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email);
    }

    // Get user by username
    public ApplicationUser? GetUserByUsername(string username)
    {
        return _context.Users.FirstOrDefault(u => u.Username == username);
    }

    // Get all users of specific role
    public IReadOnlyList<ApplicationUser> GetUsersByRole(string roleType)
    {
        return _context.Users
            .Where(u => u.Role == roleType)
            .ToList()
            .AsReadOnly();
    }

    // Get all active users
    public IReadOnlyList<ApplicationUser> GetActiveUsers()
    {
        return _context.Users
            .Where(u => u.IsActive)
            .ToList()
            .AsReadOnly();
    }

    // Get all users
    public IReadOnlyList<ApplicationUser> GetAllUsers()
    {
        return _context.Users
            .ToList()
            .AsReadOnly();
    }

    // Register new user
    public ApplicationUser RegisterUser(string fullName, string username, string email, string password, string roleType)
    {
        // Validate username
        if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            throw new InvalidOperationException("Username must be at least 3 characters");
        
        if (_context.Users.Any(u => u.Username == username))
            throw new InvalidOperationException("Username already taken");
        
        // Validate email
        if (_context.Users.Any(u => u.Email == email))
            throw new InvalidOperationException("Email already registered");

        if (password.Length < 6)
            throw new InvalidOperationException("Password must be at least 6 characters");

        // Create user as inactive — activated after OTP verification
        var user = new ApplicationUser
        {
            FullName = fullName,
            Username = username,
            Email = email,
            PasswordHash = HashPassword(password),
            Role = roleType,
            CreatedAt = DateTime.UtcNow,
            IsActive = false   // activated in VerifyOtp after email confirmed
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        if (roleType == "Agent")
        {
            var names = fullName.Split(' ', 2);
            var agent = new Agent
            {
                UserId = user.UserId,
                FirstName = names[0],
                LastName = names.Length > 1 ? names[1] : "",
                Email = email,
                Name = fullName,
                IsArchived = false,
                CreatedAtUtc = DateTime.UtcNow
            };
            _context.Agents.Add(agent);
            _context.SaveChanges();
        }

        return user;
    }

    // Login user with username
    public ApplicationUser? LoginUser(string usernameOrEmail, string password)
    {
        // Try to find user by username or email
        var user = _context.Users
            .FirstOrDefault(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);

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
    public void UpdateUser(ApplicationUser user)
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
            .Where(u => u.Role == roleType)
            .Count();
    }

    // Get total active users
    public int GetActiveUserCount()
    {
        return _context.Users.Count(u => u.IsActive);
    }

    // Get all roles available for registration
    public IReadOnlyList<string> GetAllRoles()
    {
        return new List<string>
        {
            "Manager",
            "Broker",
            "Seller",
            "Accounting",
            "Agent"
        }.AsReadOnly();
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

    // Verify password for a specific user by ID
    public bool VerifyPasswordForUser(int userId, string password)
    {
        var user = GetUserById(userId);
        if (user == null) return false;
        return VerifyPassword(password, user.PasswordHash);
    }
}
