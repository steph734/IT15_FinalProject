-- Verify and fix Username column in Users table
-- This script ensures the Username column exists and is properly configured

USE db49649;
GO

-- Check if Username column exists
IF NOT EXISTS (
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Users' 
    AND COLUMN_NAME = 'Username'
)
BEGIN
    -- Add Username column if it doesn't exist
    ALTER TABLE Users
    ADD Username NVARCHAR(100) NOT NULL DEFAULT '';
    
    PRINT 'Username column added successfully';
END
ELSE
BEGIN
    PRINT 'Username column already exists';
END

-- Add unique index on Username if it doesn't exist
IF NOT EXISTS (
    SELECT * 
    FROM sys.indexes 
    WHERE name = 'IX_Users_Username' 
    AND object_id = OBJECT_ID('Users')
)
BEGIN
    CREATE UNIQUE INDEX IX_Users_Username ON Users(Username);
    PRINT 'Unique index on Username created successfully';
END
ELSE
BEGIN
    PRINT 'Unique index on Username already exists';
END

-- Verify the column
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users'
AND COLUMN_NAME IN ('UserId', 'FullName', 'Username', 'Email', 'PasswordHash', 'RoleId', 'IsActive', 'LastLogin', 'CreatedAt')
ORDER BY ORDINAL_POSITION;

GO
