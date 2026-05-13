-- Seed SuperAdmin Account
-- Username: superadmin
-- Password: Superadmin@123
-- Email: superadmin@estateflow.com

DECLARE @PasswordHash NVARCHAR(MAX);
DECLARE @Salt NVARCHAR(MAX);

-- SHA256 hash of "Superadmin@123"
SET @PasswordHash = CONVERT(NVARCHAR(MAX), HASHBYTES('SHA256', 'Superadmin@123'), 2);

INSERT INTO Users (FullName, Email, Username, PasswordHash, Role, Status, IsActive, CreatedAt, PhoneNumber, UpdatedAt)
VALUES (
    'Super Administrator',
    'superadmin@estateflow.com',
    'superadmin',
    @PasswordHash,
    'SuperAdmin',
    'Active',
    1,
    GETUTCDATE(),
    '',
    NULL
);

PRINT 'SuperAdmin account created successfully!';
PRINT 'Username: superadmin';
PRINT 'Password: Superadmin@123';
PRINT 'Email: superadmin@estateflow.com';
