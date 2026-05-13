-- SQL Seeder Script for EstateFlow
-- Run this against your database to seed initial data
-- Note: This is only needed if EF Core migrations don't work
-- The migration-based seeding (in OnModelCreating) is preferred

USE [RealEstate];
GO

-- Check if users already exist to avoid duplicates
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'manager')
BEGIN
    SET IDENTITY_INSERT Users ON;

    -- Seed Users (Role-specific passwords)
    -- manager / Manager@123
    -- broker / Broker@123  
    -- seller / Seller@123
    -- accounting / Accounting@123
    -- superadmin / Admin@123
    INSERT INTO Users (UserId, FullName, Username, Email, PasswordHash, Role, Status, IsActive, CreatedAt, PhoneNumber)
    VALUES 
        (1, 'System Manager', 'manager', 'manager@estateflow.com', 
         'd6g+mYT/D9IwRlBAn9o0xIWY8AK1wLQ+P18CTc2M1p0=', 'Manager', 'Active', 1, '2023-01-01', '09123456789'),
        
        (2, 'System Broker', 'broker', 'broker@estateflow.com',
         'Ygh57lrWRjQGOjFH/r0aL8h6UcM6O1s6SXGLpts4uMg=', 'Broker', 'Active', 1, '2023-01-01', '09223456789'),
        
        (3, 'System Seller', 'seller', 'seller@estateflow.com',
         '5Dq11+6d4/mXus0hHGvIWoXAjpuy5XyMT/OaYbpxLrY=', 'Seller', 'Active', 1, '2023-01-01', '09323456789'),
        
        (4, 'System Accounting', 'accounting', 'accounting@estateflow.com',
         '6t/z2aTP6DXwyy/qjSvx1mWgkq1TaOrPMb3ghkNVQ3k=', 'Accounting', 'Active', 1, '2023-01-01', '09423456789'),
        
        (5, 'Super Administrator', 'superadmin', 'superadmin@estateflow.com',
         'GoFeFN+z3PYs0mOONJNnWm4xjS10y4y5a6g8zOVeJOE=', 'SuperAdmin', 'Active', 1, '2023-01-01', '09523456789');

    SET IDENTITY_INSERT Users OFF;

    -- Seed Commission Rules
    SET IDENTITY_INSERT CommissionRules ON;
    INSERT INTO CommissionRules (RuleId, ManagerId, CommissionPercent, CompanySharePercent, IsActive, CreatedAt)
    VALUES (1, 1, 3.0, 2.0, 1, '2023-01-01');
    SET IDENTITY_INSERT CommissionRules OFF;

    -- Seed Audit Log
    SET IDENTITY_INSERT AuditLogs ON;
    INSERT INTO AuditLogs (LogId, UserId, UserRole, Action, EntityType, Description, IPAddress, UserAgent, CreatedAt)
    VALUES (1, 1, 'System', 'System Initialization', 'System', 'Database seeded with predefined accounts', '127.0.0.1', 'SQL Seeder', GETUTCDATE());
    SET IDENTITY_INSERT AuditLogs OFF;

    PRINT 'Database seeded successfully!';
END
ELSE
BEGIN
    PRINT 'Users already exist. Skipping seed to avoid duplicates.';
END
GO

-- Verify seed
SELECT UserId, FullName, Username, Email, Role, Status, IsActive 
FROM Users 
ORDER BY UserId;
