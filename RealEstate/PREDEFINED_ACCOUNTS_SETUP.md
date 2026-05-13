# Predefined Accounts Setup Guide

## Overview
The registration system has been updated to remove role selection. Now:
- **Regular users** who register through the form automatically get the **"Investor"** role
- **Predefined accounts** exist for Broker, Manager, Seller, and Accounting roles

## Predefined Account Credentials

| Role | Username | Password | Email |
|------|----------|----------|-------|
| **Broker** | `broker123` | `broker@123456` | broker@estateflow.com |
| **Manager** | `manager123` | `manager@123456` | manager@estateflow.com |
| **Seller** | `seller123` | `seller@123456` | seller@estateflow.com |
| **Accounting** | `accounting123` | `accounting@123456` | accounting@estateflow.com |

## Setup Instructions

### Step 1: Add Username Column to Database
Run this SQL script first:
```
File: add_username_to_users.sql
```

This will:
- Add the `Username` column to the Users table
- Create a unique index on Username
- Verify the column was added successfully

### Step 2: Create Predefined Accounts
Run this SQL script:
```
File: create_predefined_accounts.sql
```

This will:
- Ensure all required roles exist (Broker, Manager, Seller, Accounting)
- Create the 4 predefined accounts with proper SHA256 password hashes
- Display the created accounts for verification

### Step 3: Test the Accounts
After running both scripts, you can login with any of the predefined accounts:

1. Go to `/admin/login`
2. Use the credentials from the table above
3. You will be redirected to the appropriate dashboard based on your role

## How It Works

### Registration Flow
1. User visits `/admin/register`
2. Fills in: Full Name, Username, Email, Password
3. **No role selection** - role is automatically set to "Investor"
4. User receives OTP email verification
5. After verification, user can login and access Investor dashboard

### Login Flow
1. User enters username/email and password
2. System looks up user by username OR email
3. System checks password hash (SHA256)
4. System identifies role from the Users table
5. User is redirected to role-specific dashboard:
   - Broker → `/broker/dashboard`
   - Manager → `/manager/dashboard`
   - Seller → `/seller/dashboard`
   - Accounting → `/accounting/dashboard`
   - Investor → `/investor/dashboard`

### Role-Based Access
Each controller has role-based authorization:
- `[AuthorizeRole("Broker")]` - Only Broker role can access
- `[AuthorizeRole("Manager")]` - Only Manager role can access
- etc.

The system checks the user's role from the session and database.

## Password Hashing

Passwords are hashed using **SHA256** algorithm:

```csharp
private string HashPassword(string password)
{
    using (var sha256 = SHA256.Create())
    {
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
```

The predefined accounts use these SHA256 hashes:
- `broker@123456` → `zYv6smZFbvsuRbxLmquLl0hUku3wzfvfUFv8Fz5skLs=`
- `manager@123456` → `4vxbsmqB3b//MlfgaoAlRQd5oW7Ah8E8qY5dZAupPhc=`
- `seller@123456` → `htLAB7DHxRLnKXD8zlwI6XOH9fThtGzMe+ycWeasiJo=`
- `accounting@123456` → `4jx5ztwSAKekq6ydvQds0lsj/uTyUETm2/Vb1XI2XKM=`

## Files Modified

1. **Views/Admin/Register.cshtml**
   - Removed role selection dropdown
   - Simplified form to: Full Name, Username, Email, Password

2. **Controllers/AdminController.cs**
   - Removed `roleType` parameter from Register action
   - Automatically assigns "Investor" role to new registrations

3. **ApplicationDBContext.cs**
   - Added Username property configuration
   - Added unique index on Username

4. **Services/UserService.cs**
   - Updated LoginUser to search by username OR email
   - Supports dual login method

## SQL Scripts Created

1. **add_username_to_users.sql**
   - Adds Username column to Users table
   - Creates unique index

2. **create_predefined_accounts.sql**
   - Creates predefined accounts with proper passwords
   - Ensures roles exist

3. **verify_username_column.sql**
   - Verification script to check column exists

## Security Notes

⚠️ **Important**: 
- The predefined passwords should be changed in production
- Consider implementing password reset functionality
- Enable OTP verification for all logins (already implemented)
- Monitor login attempts for security

## Troubleshooting

### Issue: "Username column doesn't exist"
**Solution**: Run `add_username_to_users.sql` script

### Issue: "Invalid username or password"
**Solution**: 
1. Verify the account exists: `SELECT * FROM Users WHERE Username = 'broker123'`
2. Check if the password hash matches
3. Ensure the account is active: `IsActive = 1`

### Issue: "Login redirects to wrong dashboard"
**Solution**: 
1. Check the user's RoleId in the Users table
2. Verify the role exists in the Roles table
3. Clear browser session/cookies and try again

## Next Steps

After setup:
1. ✅ Test all 4 predefined accounts
2. ✅ Test regular user registration (should get Investor role)
3. ✅ Verify OTP email verification works
4. ✅ Check role-based access control
5. ✅ Change default passwords if needed
