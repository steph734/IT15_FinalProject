-- Add Username column to Users table
-- Run this script directly on your database

USE db49649;
GO

-- Step 1: Add Username column if it doesn't exist
IF NOT EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Users') 
    AND name = 'Username'
)
BEGIN
    ALTER TABLE dbo.Users
    ADD Username NVARCHAR(100) NOT NULL DEFAULT '';
    
    PRINT '✓ Username column added successfully';
END
ELSE
BEGIN
    PRINT '✓ Username column already exists';
END

GO

-- Step 2: Create unique index on Username if it doesn't exist
IF NOT EXISTS (
    SELECT * 
    FROM sys.indexes 
    WHERE name = 'IX_Users_Username' 
    AND object_id = OBJECT_ID('dbo.Users')
)
BEGIN
    CREATE UNIQUE INDEX IX_Users_Username ON dbo.Users(Username);
    PRINT '✓ Unique index on Username created successfully';
END
ELSE
BEGIN
    PRINT '✓ Unique index on Username already exists';
END

GO

-- Step 3: Verify the table structure
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    CHARACTER_MAXIMUM_LENGTH, 
    IS_NULLABLE,
    ORDINAL_POSITION
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users'
AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;

GO

-- Step 4: Show sample data (first 10 users)
SELECT TOP 10 
    UserId,
    RoleId,
    FullName,
    Username,
    Email,
    IsActive,
    CreatedAt
FROM dbo.Users
ORDER BY UserId;

GO

PRINT '=========================================';
PRINT 'Username column setup completed!';
PRINT '=========================================';

GO
