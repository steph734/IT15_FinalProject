-- =====================================================
-- NOTIFICATION SYSTEM & SELLER LISTING STATUS UPDATE
-- =====================================================
-- This script:
-- 1. Creates Notifications table
-- 2. Updates SellerListingStatus enum (adds AwaitingManagerPayment, PaymentCompleted)
-- =====================================================

USE [DB_Real_Estate]
GO

-- =====================================================
-- 1. CREATE NOTIFICATIONS TABLE
-- =====================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Notifications')
BEGIN
    PRINT 'Creating Notifications table...';
    
    CREATE TABLE [dbo].[Notifications] (
        [NotificationId] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NOT NULL,
        [NotificationType] NVARCHAR(50) NOT NULL,
        [Title] NVARCHAR(200) NOT NULL,
        [Message] NVARCHAR(1000) NULL,
        [RelatedEntityId] INT NULL,
        [RelatedEntityType] NVARCHAR(50) NULL,
        [IsRead] BIT NOT NULL DEFAULT 0,
        [ReadAtUtc] DATETIME2 NULL,
        [CreatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [DataJson] NVARCHAR(2000) NULL,
        
        CONSTRAINT [FK_Notifications_Users] 
            FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) 
            ON DELETE CASCADE
    );
    
    -- Create indexes for performance
    CREATE INDEX [IX_Notifications_UserId] ON [dbo].[Notifications] ([UserId]);
    CREATE INDEX [IX_Notifications_IsRead] ON [dbo].[Notifications] ([IsRead]);
    CREATE INDEX [IX_Notifications_CreatedAtUtc] ON [dbo].[Notifications] ([CreatedAtUtc] DESC);
    
    PRINT '✓ Notifications table created successfully!';
END
ELSE
BEGIN
    PRINT '⚠ Notifications table already exists - skipping creation.';
END
GO

-- =====================================================
-- 2. UPDATE SELLER LISTING STATUS ENUM VALUES
-- =====================================================
-- Note: SQL Server doesn't have native enum support
-- The enum values are stored as INT in the Status column
-- We just need to ensure existing data is valid
-- =====================================================

PRINT 'Verifying SellerListings Status values...';

-- Check for any invalid status values
DECLARE @InvalidStatusCount INT;
SELECT @InvalidStatusCount = COUNT(*) 
FROM [dbo].[SellerListings] 
WHERE [Status] NOT IN (0, 1, 2, 3, 4, 5);

IF @InvalidStatusCount > 0
BEGIN
    PRINT '⚠ Found ' + CAST(@InvalidStatusCount AS NVARCHAR) + ' listings with invalid status values.';
    PRINT '  Valid values: 0=PendingReview, 1=Approved, 2=Rejected, 3=Sold, 4=AwaitingManagerPayment, 5=PaymentCompleted';
    
    -- Reset invalid statuses to PendingReview (0)
    UPDATE [dbo].[SellerListings] 
    SET [Status] = 0 
    WHERE [Status] NOT IN (0, 1, 2, 3, 4, 5);
    
    PRINT '✓ Invalid statuses have been reset to PendingReview (0).';
END
ELSE
BEGIN
    PRINT '✓ All SellerListings have valid status values.';
END
GO

-- =====================================================
-- 3. VERIFICATION
-- =====================================================

PRINT '';
PRINT '========================================';
PRINT 'Migration Verification';
PRINT '========================================';

-- Check Notifications table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Notifications')
BEGIN
    DECLARE @NotificationCount INT;
    SELECT @NotificationCount = COUNT(*) FROM [dbo].[Notifications];
    PRINT '✓ Notifications table exists (' + CAST(@NotificationCount AS NVARCHAR) + ' records)';
    
    -- Show table structure
    PRINT '';
    PRINT 'Notifications table columns:';
    SELECT 
        COLUMN_NAME,
        DATA_TYPE,
        IS_NULLABLE,
        COLUMN_DEFAULT
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Notifications'
    ORDER BY ORDINAL_POSITION;
END
ELSE
BEGIN
    PRINT '✗ Notifications table was NOT created!';
END

PRINT '';
PRINT '========================================';
PRINT 'Migration Complete!';
PRINT '========================================';
PRINT '';
PRINT 'New Status Values:';
PRINT '  0 = PendingReview';
PRINT '  1 = Approved';
PRINT '  2 = Rejected';
PRINT '  3 = Sold';
PRINT '  4 = AwaitingManagerPayment (NEW)';
PRINT '  5 = PaymentCompleted (NEW)';
PRINT '';
GO
