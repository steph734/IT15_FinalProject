-- =====================================================
-- ADD USERNAME COLUMN TO USERS TABLE
-- Migration for Login/Register Changes
-- =====================================================

USE [DB_Real_Estate]
GO

PRINT '========================================';
PRINT 'ADDING USERNAME COLUMN TO USERS TABLE';
PRINT '========================================';
PRINT '';

-- Check if Username column already exists
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[Users]') 
    AND name = 'Username'
)
BEGIN
    PRINT 'Adding Username column to Users table...';
    
    -- Add Username column
    ALTER TABLE [dbo].[Users]
    ADD [Username] NVARCHAR(50) NOT NULL DEFAULT '';
    
    PRINT '✓ Username column added successfully!';
    PRINT '';
    
    -- Create unique index on Username
    PRINT 'Creating unique index on Username...';
    CREATE UNIQUE INDEX [IX_Users_Username] ON [dbo].[Users] ([Username]);
    PRINT '✓ Unique index created!';
    PRINT '';
    
    -- Update existing users with username based on email
    PRINT 'Updating existing users with usernames based on email...';
    
    UPDATE [dbo].[Users]
    SET [Username] = LOWER(REPLACE(LEFT([Email], CHARINDEX('@', [Email]) - 1), '.', '_'))
    WHERE [Username] = '' OR [Username] IS NULL;
    
    PRINT '✓ Existing users updated with usernames!';
    PRINT '';
    
    -- Show updated users
    PRINT 'Current users with new usernames:';
    SELECT 
        UserId,
        FullName,
        Username,
        Email,
        [Role].[RoleType]
    FROM [dbo].[Users]
    INNER JOIN [dbo].[Roles] AS [Role] ON [Users].[RoleId] = [Role].[RoleId]
    ORDER BY UserId;
END
ELSE
BEGIN
    PRINT '⚠ Username column already exists!';
    PRINT '';
    
    -- Check if any users don't have usernames
    DECLARE @EmptyUsernameCount INT;
    SELECT @EmptyUsernameCount = COUNT(*) 
    FROM [dbo].[Users] 
    WHERE [Username] = '' OR [Username] IS NULL;
    
    IF @EmptyUsernameCount > 0
    BEGIN
        PRINT 'Found ' + CAST(@EmptyUsernameCount AS VARCHAR) + ' users without usernames. Updating...';
        
        UPDATE [dbo].[Users]
        SET [Username] = LOWER(REPLACE(LEFT([Email], CHARINDEX('@', [Email]) - 1), '.', '_'))
        WHERE [Username] = '' OR [Username] IS NULL;
        
        PRINT '✓ Users updated!';
    END
    ELSE
    BEGIN
        PRINT '✓ All users have usernames.';
    END
END

PRINT '';
PRINT '========================================';
PRINT 'MIGRATION COMPLETE';
PRINT '========================================';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Restart your application';
PRINT '2. Test login with username instead of email';
PRINT '3. Test registration with new username field';
PRINT '4. Verify OTP is sent after registration';
PRINT '';
GO
