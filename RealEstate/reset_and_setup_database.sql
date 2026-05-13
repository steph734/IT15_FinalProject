-- Complete Database Reset and Setup Script
-- This script will:
-- 1. Clear all existing data from Users table
-- 2. Ensure Username column exists
-- 3. Create predefined accounts for Broker, Manager, Seller, Accounting
-- 4. Reset identity counters

USE db49649;
GO

PRINT '=========================================';
PRINT 'Starting Database Reset and Setup...';
PRINT '=========================================';
PRINT '';

-- Step 1: Clear existing data (except predefined accounts we'll create)
PRINT 'Step 1: Clearing existing user data...';

-- Delete related data first to avoid foreign key conflicts
-- Delete commission rules that reference users
IF OBJECT_ID('dbo.CommissionRules', 'U') IS NOT NULL
BEGIN
    DELETE FROM CommissionRules;
    PRINT '  ✓ Cleared CommissionRules table';
END

-- Delete OTP verifications for existing users
IF OBJECT_ID('dbo.OtpVerifications', 'U') IS NOT NULL
BEGIN
    DELETE FROM OtpVerifications;
    PRINT '  ✓ Cleared OtpVerifications table';
END

-- Delete audit logs
IF OBJECT_ID('dbo.AuditLogs', 'U') IS NOT NULL
BEGIN
    DELETE FROM AuditLogs;
    PRINT '  ✓ Cleared AuditLogs table';
END

-- Delete notifications
IF OBJECT_ID('dbo.Notifications', 'U') IS NOT NULL
BEGIN
    DELETE FROM Notifications;
    PRINT '  ✓ Cleared Notifications table';
END

-- Delete commission deals
IF OBJECT_ID('dbo.CommissionDeals', 'U') IS NOT NULL
BEGIN
    DELETE FROM CommissionDeals;
    PRINT '  ✓ Cleared CommissionDeals table';
END

-- Delete commission batches
IF OBJECT_ID('dbo.CommissionBatches', 'U') IS NOT NULL
BEGIN
    DELETE FROM CommissionBatches;
    PRINT '  ✓ Cleared CommissionBatches table';
END

-- Delete agents
IF OBJECT_ID('dbo.Agents', 'U') IS NOT NULL
BEGIN
    DELETE FROM Agents;
    PRINT '  ✓ Cleared Agents table';
END

-- Delete seller listings
IF OBJECT_ID('dbo.SellerListings', 'U') IS NOT NULL
BEGIN
    DELETE FROM SellerListings;
    PRINT '  ✓ Cleared SellerListings table';
END

-- Now delete all users
DELETE FROM Users;
PRINT '  ✓ Cleared Users table';

-- Reset identity counter
DBCC CHECKIDENT ('Users', RESEED, 0);
PRINT '  ✓ Reset Users identity counter';

PRINT '';

-- Step 2: Ensure Username column exists
PRINT 'Step 2: Checking Username column...';

IF NOT EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Users') 
    AND name = 'Username'
)
BEGIN
    ALTER TABLE dbo.Users
    ADD Username NVARCHAR(100) NOT NULL DEFAULT '';
    PRINT '  ✓ Username column added successfully';
END
ELSE
BEGIN
    PRINT '  ✓ Username column already exists';
END

-- Fix any empty or null usernames before creating unique index
UPDATE Users 
SET Username = 'user_' + CAST(UserId AS NVARCHAR(20)) 
WHERE Username IS NULL OR Username = '' OR LEN(Username) = 0;
PRINT '  ✓ Fixed empty usernames';

-- Create unique index on Username if it doesn't exist
IF NOT EXISTS (
    SELECT * 
    FROM sys.indexes 
    WHERE name = 'IX_Users_Username' 
    AND object_id = OBJECT_ID('dbo.Users')
)
BEGIN
    CREATE UNIQUE INDEX IX_Users_Username ON dbo.Users(Username);
    PRINT '  ✓ Unique index on Username created';
END
ELSE
BEGIN
    PRINT '  ✓ Unique index on Username already exists';
END

PRINT '';

-- Step 3: Ensure all required roles exist
PRINT 'Step 3: Setting up roles...';

-- Insert roles if they don't exist
IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleType = 'SuperAdmin')
BEGIN
    INSERT INTO Roles (RoleType) VALUES ('SuperAdmin');
    PRINT '  ✓ Created SuperAdmin role';
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleType = 'Broker')
BEGIN
    INSERT INTO Roles (RoleType) VALUES ('Broker');
    PRINT '  ✓ Created Broker role';
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleType = 'Manager')
BEGIN
    INSERT INTO Roles (RoleType) VALUES ('Manager');
    PRINT '  ✓ Created Manager role';
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleType = 'Seller')
BEGIN
    INSERT INTO Roles (RoleType) VALUES ('Seller');
    PRINT '  ✓ Created Seller role';
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleType = 'Accounting')
BEGIN
    INSERT INTO Roles (RoleType) VALUES ('Accounting');
    PRINT '  ✓ Created Accounting role';
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleType = 'Investor')
BEGIN
    INSERT INTO Roles (RoleType) VALUES ('Investor');
    PRINT '  ✓ Created Investor role';
END

PRINT '';

-- Step 4: Create predefined accounts
PRINT 'Step 4: Creating predefined accounts...';

-- Broker Account
INSERT INTO Users (RoleId, FullName, Username, Email, PasswordHash, CreatedAt, IsActive)
VALUES (
    (SELECT RoleId FROM Roles WHERE RoleType = 'Broker'),
    'Broker User',
    'broker123',
    'broker@estateflow.com',
    'zYv6smZFbvsuRbxLmquLl0hUku3wzfvfUFv8Fz5skLs=',
    GETUTCDATE(),
    1
);
PRINT '  ✓ Created Broker account (username: broker123, password: broker@123456)';

-- Manager Account
INSERT INTO Users (RoleId, FullName, Username, Email, PasswordHash, CreatedAt, IsActive)
VALUES (
    (SELECT RoleId FROM Roles WHERE RoleType = 'Manager'),
    'Manager User',
    'manager123',
    'manager@estateflow.com',
    '4vxbsmqB3b//MlfgaoAlRQd5oW7Ah8E8qY5dZAupPhc=',
    GETUTCDATE(),
    1
);
PRINT '  ✓ Created Manager account (username: manager123, password: manager@123456)';

-- Seller Account
INSERT INTO Users (RoleId, FullName, Username, Email, PasswordHash, CreatedAt, IsActive)
VALUES (
    (SELECT RoleId FROM Roles WHERE RoleType = 'Seller'),
    'Seller User',
    'seller123',
    'seller@estateflow.com',
    'htLAB7DHxRLnKXD8zlwI6XOH9fThtGzMe+ycWeasiJo=',
    GETUTCDATE(),
    1
);
PRINT '  ✓ Created Seller account (username: seller123, password: seller@123456)';

-- Accounting Account
INSERT INTO Users (RoleId, FullName, Username, Email, PasswordHash, CreatedAt, IsActive)
VALUES (
    (SELECT RoleId FROM Roles WHERE RoleType = 'Accounting'),
    'Accounting User',
    'accounting123',
    'accounting@estateflow.com',
    '4jx5ztwSAKekq6ydvQds0lsj/uTyUETm2/Vb1XI2XKM=',
    GETUTCDATE(),
    1
);
PRINT '  ✓ Created Accounting account (username: accounting123, password: accounting@123456)';

PRINT '';

-- Step 5: Verify the setup
PRINT 'Step 5: Verifying setup...';
PRINT '';

-- Show all users
SELECT 
    u.UserId,
    r.RoleType,
    u.FullName,
    u.Username,
    u.Email,
    u.IsActive,
    u.CreatedAt
FROM Users u
INNER JOIN Roles r ON u.RoleId = r.RoleId
ORDER BY u.UserId;

PRINT '';
PRINT '=========================================';
PRINT 'Database Reset and Setup Completed!';
PRINT '=========================================';
PRINT '';
PRINT 'Predefined Account Credentials:';
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';
PRINT 'Broker:     username: broker123     | password: broker@123456';
PRINT 'Manager:    username: manager123    | password: manager@123456';
PRINT 'Seller:     username: seller123     | password: seller@123456';
PRINT 'Accounting: username: accounting123 | password: accounting@123456';
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';
PRINT '';
PRINT 'You can now login with any of these accounts!';
PRINT '=========================================';

GO
