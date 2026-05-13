-- =====================================================
-- FIX: Cannot Approve Seller Listings
-- This script will show current status and fix listings
-- =====================================================

USE [DB_Real_Estate]
GO

PRINT '========================================';
PRINT 'CURRENT SELLER LISTINGS';
PRINT '========================================';
PRINT '';

-- Show all listings with status names
SELECT 
    Id,
    Title,
    Status,
    CASE Status
        WHEN 0 THEN 'PendingReview ✅ CAN APPROVE'
        WHEN 1 THEN 'Approved ❌ Already approved'
        WHEN 2 THEN 'Rejected ❌ Cannot approve'
        WHEN 3 THEN 'Sold ❌ Already sold'
        WHEN 4 THEN 'AwaitingManagerPayment ❌ Old workflow'
        WHEN 5 THEN 'PaymentCompleted ❌ Old workflow'
        ELSE 'UNKNOWN ❌ Invalid status'
    END AS StatusInfo,
    SellerName,
    SuggestedPrice,
    FinalPrice,
    CreatedAtUtc
FROM SellerListings
ORDER BY Id DESC;

PRINT '';
PRINT '========================================';
PRINT 'STATUS COUNT';
PRINT '========================================';
PRINT '';

SELECT 
    Status,
    CASE Status
        WHEN 0 THEN 'PendingReview'
        WHEN 1 THEN 'Approved'
        WHEN 2 THEN 'Rejected'
        WHEN 3 THEN 'Sold'
        WHEN 4 THEN 'AwaitingManagerPayment'
        WHEN 5 THEN 'PaymentCompleted'
        ELSE 'UNKNOWN'
    END AS StatusName,
    COUNT(*) AS TotalListings
FROM SellerListings
GROUP BY Status
ORDER BY Status;

PRINT '';
PRINT '========================================';
PRINT 'LISTINGS THAT CAN BE APPROVED';
PRINT '========================================';
PRINT '';

DECLARE @PendingCount INT;
SELECT @PendingCount = COUNT(*) FROM SellerListings WHERE Status = 0;

IF @PendingCount > 0
BEGIN
    PRINT 'Found ' + CAST(@PendingCount AS VARCHAR) + ' pending listing(s):';
    PRINT '';
    
    SELECT Id, Title, SellerName, SuggestedPrice 
    FROM SellerListings 
    WHERE Status = 0;
END
ELSE
BEGIN
    PRINT '❌ NO PENDING LISTINGS FOUND!';
    PRINT '';
    PRINT 'This is why you cannot approve listings.';
    PRINT '';
    PRINT 'Choose ONE of the fixes below:';
    PRINT '';
    PRINT '--- FIX OPTION 1: Reset Specific Listing ---';
    PRINT 'UPDATE SellerListings SET Status = 0 WHERE Id = 3;';
    PRINT '';
    PRINT '--- FIX OPTION 2: Reset ALL Rejected to Pending ---';
    PRINT 'UPDATE SellerListings SET Status = 0 WHERE Status = 2;';
    PRINT '';
    PRINT '--- FIX OPTION 3: Reset ALL to Pending (Testing) ---';
    PRINT 'UPDATE SellerListings SET Status = 0;';
END

PRINT '';
PRINT '========================================';
PRINT 'AUTOMATIC FIX: Reset Listing #3 to Pending';
PRINT '========================================';
PRINT '';

-- Check if listing #3 exists
IF EXISTS (SELECT 1 FROM SellerListings WHERE Id = 3)
BEGIN
    DECLARE @CurrentStatus INT;
    SELECT @CurrentStatus = Status FROM SellerListings WHERE Id = 3;
    
    PRINT 'Listing #3 current status: ' + CAST(@CurrentStatus AS VARCHAR);
    
    IF @CurrentStatus = 0
    BEGIN
        PRINT '✅ Already in PendingReview status!';
        PRINT 'The issue might be something else.';
    END
    ELSE
    BEGIN
        PRINT 'Fixing: Resetting listing #3 to PendingReview...';
        
        UPDATE SellerListings 
        SET Status = 0,                    -- PendingReview
            ManagerNotes = NULL,           -- Clear rejection notes
            ReviewedByManagerId = NULL,    -- Clear reviewer
            ReviewedAtUtc = NULL,          -- Clear review date
            FinalPrice = NULL,             -- Clear price
            MarkupPercent = NULL,          -- Clear markup
            UpdatedAtUtc = GETUTCDATE()
        WHERE Id = 3;
        
        PRINT '✅ Fixed! Listing #3 is now PendingReview';
        PRINT '';
        PRINT 'You can now approve it as Manager!';
    END
END
ELSE
BEGIN
    PRINT '❌ Listing #3 does not exist!';
    PRINT 'Create a new listing as Seller first.';
END

PRINT '';
PRINT '========================================';
PRINT 'VERIFY FIX';
PRINT '========================================';
PRINT '';

SELECT 
    Id,
    Title,
    Status,
    CASE Status
        WHEN 0 THEN '✅ PendingReview - CAN APPROVE!'
        ELSE '❌ Still not pending'
    END AS ReadyToApprove
FROM SellerListings 
WHERE Id = 3;

PRINT '';
PRINT '========================================';
PRINT 'COMPLETE';
PRINT '========================================';
GO
