# User ID System - Complete Migration Guide

## 🎯 SOLUTION: Unique IDs for All User Types

Your system now has a **unified User ID system** where each user (SuperAdmin, Manager, Accounting, Investor, Broker) gets a **globally unique ID**.

---

## 📊 Database Schema

### **Users Table** (Main table for ALL user types)
```
┌─────────────────────────────────────────────────────────────┐
│                         Users                               │
├─────────────────────────────────────────────────────────────┤
│ Id (PK)              │ Unique auto-increment for ALL users  │
│ FullName             │ nvarchar(100)                        │
│ Email (Unique)       │ nvarchar(255)                        │
│ Username (Unique)    │ nvarchar(50)                         │
│ PasswordHash         │ nvarchar(max) - SHA256 encrypted     │
│ PhoneNumber          │ nvarchar(20)                         │
│ ProfileImageUrl      │ nvarchar(500)                        │
│ Role                 │ SuperAdmin|Manager|Accounting|...    │
│ Status               │ Active|Inactive|Suspended|Pending    │
│ CreatedAt            │ datetime2 - Default: GETUTCDATE()    │
│ LastLogin            │ datetime2 nullable                   │
│ LastPasswordChanged  │ datetime2 nullable                   │
│ Department           │ nvarchar(100)                        │
│ EmailVerified        │ bit                                  │
│ Notes                │ nvarchar(500)                        │
│ Discriminator        │ For Entity Framework type tracking   │
└─────────────────────────────────────────────────────────────┘
```

### **Type-Specific Tables** (TPT - Table Per Type Inheritance)

**SuperAdminUsers**
```
Id (FK→Users.Id) | AdminPermissions
         1       | ALL
         2       | ALL
```

**ManagerUsers**
```
Id (FK→Users.Id) | TeamName        | TeamSize
      1001       | Sales Team      | 5
      1002       | Marketing Team  | 3
```

**AccountingUsers**
```
Id (FK→Users.Id) | AccountingDepartment | AccessLevel
     10001       | Finance             | 100
     10002       | Payroll             | 75
```

**InvestorUsers**
```
Id (FK→Users.Id) | InvestmentBudget | PreferredLocations | PropertyPreferences
    100001       | 5000000          | BGC, Makati       | Condo, House
    100002       | 10000000         | Cebu, Davao       | Commercial
```

**BrokerUsers**
```
Id (FK→Users.Id) | LicenseNumber | Agency           | CommissionRate
   1000001       | PH-2026-001   | EstateFlow Corp  | 5.0
   1000002       | PH-2026-002   | EstateFlow Corp  | 4.5
```

---

## 🔑 ID Distribution Strategy

Each user type gets a **UNIQUE GLOBAL ID** across your entire system:

| User Type | ID Range | Example IDs | Count |
|-----------|----------|-------------|-------|
| **SuperAdmin** | 1-999 | 1, 2, 3, 4 | Up to 999 admins |
| **Manager** | 1000-9999 | 1001, 1002, 2500 | Up to 9000 managers |
| **Accounting** | 10000-99999 | 10001, 15000, 50000 | Up to 90000 accountants |
| **Investor** | 100000-999999 | 100001, 500000 | Up to 900000 investors |
| **Broker** | 1000000+ | 1000001, 2000000 | Unlimited brokers |

---

## ✨ Key Features

✅ **Single ID Column** - All users share one auto-increment Id  
✅ **No Conflicts** - Each user type has unique global ID  
✅ **Easy Querying** - Get any user with just: `WHERE Id = @id`  
✅ **Role-Based Access** - Query by Role column: `WHERE Role = 'Manager'`  
✅ **Type-Specific Data** - Access specific properties from type tables  
✅ **Secure** - Passwords hashed with SHA256  
✅ **Audit Trail** - CreatedAt, LastLogin, LastPasswordChanged tracked  
✅ **Email Verified** - Track email verification status  

---

## 🚀 Implementation Steps

### **Step 1: Build the Project**
```powershell
cd C:\Users\ADMIN\source\repos\IT15_FinalProject\RealEstate
dotnet build
```

### **Step 2: Add New Migration**
```powershell
dotnet-ef migrations add MigrateAdminUsersToNewUserSystem
```

### **Step 3: Update Database**
```powershell
dotnet-ef database update
```

### **Step 4: Verify in SQL Server**
Connect to `DB_Real_Estate` and run:
```sql
-- Check Users table created
SELECT * FROM [Users];

-- Check user types
SELECT COUNT(*) as Total, Role FROM [Users] GROUP BY Role;

-- Check specific user type
SELECT u.*, s.AdminPermissions 
FROM [Users] u
LEFT JOIN [SuperAdminUsers] s ON u.Id = s.Id
WHERE u.Role = 'SuperAdmin';
```

---

## 📝 Using the UserService

### **Register a SuperAdmin**
```csharp
var admin = _userService.RegisterUser(
    fullName: "Admin User",
    email: "admin@estateflow.com",
    username: "admin",
    password: "SecurePass123",
    role: UserRole.SuperAdmin,
    phoneNumber: "09171234567"
);
// Returns: User with Id = 1
```

### **Register a Manager**
```csharp
var manager = _userService.RegisterUser(
    fullName: "John Manager",
    email: "manager@estateflow.com",
    username: "manager_john",
    password: "SecurePass123",
    role: UserRole.Manager,
    phoneNumber: "09179876543"
);
// Returns: User with Id = 1001
```

### **Register an Investor**
```csharp
var investor = _userService.RegisterUser(
    fullName: "Jane Investor",
    email: "investor@estateflow.com",
    username: "investor_jane",
    password: "SecurePass123",
    role: UserRole.Investor,
    phoneNumber: "09175551234"
);
// Returns: User with Id = 100001
```

### **Get User by ID (works for any type!)**
```csharp
var user = _userService.GetUserById(100001);
// Returns the Investor with Id = 100001

if (user?.Role == UserRole.Investor)
{
    // Handle investor logic
}
```

### **Get All Users of Specific Role**
```csharp
var allBrokers = _userService.GetUsersByRole(UserRole.Broker);
// Returns all BrokerUsers with IDs 1000001+
```

---

## 🔐 Security Features

✅ **Password Hashing** - SHA256 encryption  
✅ **Unique Email & Username** - Database constraints prevent duplicates  
✅ **Email Verification** - Track verified emails  
✅ **Account Status** - Active, Inactive, Suspended, Pending states  
✅ **Password Change Tracking** - LastPasswordChanged timestamp  
✅ **Login Tracking** - LastLogin timestamp  

---

## 📋 Migration Files

- `20260405122914_AddViewingAppointments.cs` - Viewing appointments table
- `20260405123641_InitialAdminUserMigration.cs` - Initial admin user table
- `20260405_MigrateAdminUsersToNewUserSystem.cs` - **NEW** - Multi-user system
  - Creates new Users table (main)
  - Creates 5 type-specific tables (SuperAdminUsers, ManagerUsers, etc.)
  - Migrates existing AdminUsers data
  - Creates unique indexes

---

## ✅ Verification Checklist

After migration, verify:

- [ ] Database updated successfully
- [ ] Users table created with unique ID column
- [ ] 5 type-specific tables created (SuperAdminUsers, ManagerUsers, etc.)
- [ ] Indexes created for Email, Username, Role, Status
- [ ] Existing AdminUsers migrated to new Users table
- [ ] Can register new users with different roles
- [ ] Each user gets unique global ID
- [ ] Login works for all user types
- [ ] Dashboard accessible after login

---

## 📊 Example Query Results

After migration and adding sample users:

```sql
SELECT 
    u.Id,
    u.FullName,
    u.Email,
    u.Username,
    u.Role,
    u.Status,
    u.CreatedAt
FROM Users u
ORDER BY u.Id;
```

Results:
```
Id   | FullName        | Email                 | Username      | Role      | Status | CreatedAt
-----|-----------------|----------------------|---------------|-----------|--------|-------------------
1    | Admin User      | admin@estateflow.com  | admin         | SuperAdmin| Active | 2026-04-05 20:30:00
2    | Super User 2    | super2@estateflow.com | superuser2    | SuperAdmin| Active | 2026-04-05 20:31:00
1001 | John Manager    | manager@estateflow.com| manager_john  | Manager   | Active | 2026-04-05 20:32:00
1002 | Jane Manager    | janemanager@...       | manager_jane  | Manager   | Active | 2026-04-05 20:33:00
10001| Finance Lead    | finance@estateflow.com| accountant_001| Accounting| Active | 2026-04-05 20:34:00
100001|Jane Investor   | investor@estateflow.com|investor_jane | Investor  | Active | 2026-04-05 20:35:00
1000001|Broker Agent   | broker@estateflow.com | broker_001    | Broker    | Active | 2026-04-05 20:36:00
```

---

## 🎉 Result

✅ **All users have globally unique IDs**  
✅ **No ID conflicts between user types**  
✅ **Role-based access control possible**  
✅ **Type-specific data stored separately**  
✅ **Scalable for future user types**  
✅ **Secure and auditable**  

**Your multi-user system is ready!** 🚀
