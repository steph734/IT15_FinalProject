-- Create predefined accounts for Broker, Manager, Seller, and Accounting
-- This script creates specific login accounts with predefined usernames and passwords

USE db49649;
GO

-- First, let's check if the roles exist
DECLARE @BrokerRoleId INT, @ManagerRoleId INT, @SellerRoleId INT, @AccountingRoleId INT;

SELECT @BrokerRoleId = RoleId FROM Roles WHERE RoleType = 'Broker';
SELECT @ManagerRoleId = RoleId FROM Roles WHERE RoleType = 'Manager';
SELECT @SellerRoleId = RoleId FROM Roles WHERE RoleType = 'Seller';
SELECT @AccountingRoleId = RoleId FROM Roles WHERE RoleType = 'Accounting';

-- If roles don't exist, create them
IF @BrokerRoleId IS NULL
BEGIN
    INSERT INTO Roles (RoleType) VALUES ('Broker');
    SET @BrokerRoleId = SCOPE_IDENTITY();
    PRINT 'Created Broker role';
END

IF @ManagerRoleId IS NULL
BEGIN
    INSERT INTO Roles (RoleType) VALUES ('Manager');
    SET @ManagerRoleId = SCOPE_IDENTITY();
    PRINT 'Created Manager role';
END

IF @SellerRoleId IS NULL
BEGIN
    INSERT INTO Roles (RoleType) VALUES ('Seller');
    SET @SellerRoleId = SCOPE_IDENTITY();
    PRINT 'Created Seller role';
END

IF @AccountingRoleId IS NULL
BEGIN
    INSERT INTO Roles (RoleType) VALUES ('Accounting');
    SET @AccountingRoleId = SCOPE_IDENTITY();
    PRINT 'Created Accounting role';
END

GO

-- Now create the predefined accounts
-- Note: Passwords are hashed using BCrypt or similar. 
-- For now, we'll use a simple hash. You should update these with proper password hashes.

-- Broker Account
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'broker123')
BEGIN
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
    PRINT '✓ Created Broker account (username: broker123, password: broker@123456)';
END
ELSE
BEGIN
    PRINT '✓ Broker account already exists';
END

-- Manager Account
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'manager123')
BEGIN
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
    PRINT '✓ Created Manager account (username: manager123, password: manager@123456)';
END
ELSE
BEGIN
    PRINT '✓ Manager account already exists';
END

-- Seller Account
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'seller123')
BEGIN
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
    PRINT '✓ Created Seller account (username: seller123, password: seller@123456)';
END
ELSE
BEGIN
    PRINT '✓ Seller account already exists';
END

-- Accounting Account
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'accounting123')
BEGIN
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
    PRINT '✓ Created Accounting account (username: accounting123, password: accounting@123456)';
END
ELSE
BEGIN
    PRINT '✓ Accounting account already exists';
END

GO

-- Display all predefined accounts
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
WHERE u.Username IN ('broker123', 'manager123', 'seller123', 'accounting123')
ORDER BY r.RoleType;

GO

PRINT '=========================================';
PRINT 'Predefined accounts setup completed!';
PRINT '=========================================';
PRINT '';
PRINT 'Account Credentials:';
PRINT 'Broker:     username: broker123     password: broker@123456';
PRINT 'Manager:    username: manager123    password: manager@123456';
PRINT 'Seller:     username: seller123     password: seller@123456';
PRINT 'Accounting: username: accounting123 password: accounting@123456';
PRINT '';
PRINT 'IMPORTANT: You need to update the PasswordHash values';
PRINT 'with proper BCrypt hashes generated from the actual passwords.';
PRINT '=========================================';

GO
