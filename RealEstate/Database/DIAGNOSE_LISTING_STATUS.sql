-- =====================================================
-- DIAGNOSE AND FIX SELLER LISTING APPROVAL ISSUE
-- =====================================================

USE [DB_Real_Estate]
GO

-- STEP 1: Check current status of all listings
PRINT '========================================';
PRINT 'CURRENT SELLER LISTINGS STATUS';
PRINT '========================================';
PRINT '';

SELECT 
    Id,
    Title,
    Status,
    CASE Status
        WHEN 0 THEN 'PendingReview'
        WHEN 1 THEN 'Approved'
        WHEN 2 THEN 'Rejected'
        WHEN 3 THEN 'Sold'
        WHEN 4 THEN 'AwaitingManagerPayment'
        WHEN 5 THEN 'PaymentCompleted'
        ELSE 'UNKNOWN (' + CAST(Status AS VARCHAR) + ')'
    END AS StatusName,
    SuggestedPrice,
    FinalPrice,
    SellerName,
    CreatedAtUtc
FROM SellerListings
ORDER BY Id DESC;

PRINT '';
PRINT '========================================';
PRINT 'STATUS DISTRIBUTION';
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
    COUNT(*) AS Count
FROM SellerListings
GROUP BY Status
ORDER BY Status;

PRINT '';
PRINT '========================================';
PRINT 'CHECKING FOR INVALID STATUSES';
PRINT '========================================';
PRINT '';

DECLARE @InvalidCount INT;
SELECT @InvalidCount = COUNT(*) 
FROM SellerListings 
WHERE Status NOT IN (0, 1, 2, 3, 4, 5);

IF @InvalidCount > 0
BEGIN
    PRINT '⚠ Found ' + CAST(@InvalidCount AS VARCHAR) + ' listings with INVALID status values!';
    PRINT '';
    PRINT 'Invalid listings:';
    SELECT Id, Title, Status FROM SellerListings WHERE Status NOT IN (0, 1, 2, 3, 4, 5);
    PRINT '';
    PRINT 'Fixing: Resetting invalid statuses to PendingReview (0)...';
    
    UPDATE SellerListings 
    SET Status = 0 
    WHERE Status NOT IN (0, 1, 2, 3, 4, 5);
    
    PRINT '✓ Fixed!';
END
ELSE
BEGIN
    PRINT '✓ All listings have valid status values.';
END

PRINT '';
PRINT '========================================';
PRINT 'PENDING LISTINGS (CAN BE APPROVED)';
PRINT '========================================';
PRINT '';

DECLARE @PendingCount INT;
SELECT @PendingCount = COUNT(*) FROM SellerListings WHERE Status = 0;

IF @PendingCount > 0
BEGIN
    PRINT 'Found ' + CAST(@PendingCount AS VARCHAR) + ' pending listing(s) ready for approval:';
    PRINT '';
    
    SELECT 
        Id,
        Title,
        SellerName,
        SuggestedPrice,
        Location,
        PropertyType,
        CreatedAtUtc
    FROM SellerListings
    WHERE Status = 0
    ORDER BY CreatedAtUtc DESC;
END
ELSE
BEGIN
    PRINT '⚠ NO PENDING LISTINGS FOUND!';
    PRINT '';
    PRINT 'This is why the manager cannot approve properties.';
    PRINT '';
    PRINT 'To fix this, you need to:';
    PRINT '1. Login as Seller and submit a new property listing';
    PRINT '   OR';
    PRINT '2. Update an existing listing to PendingReview status:';
    PRINT '';
    PRINT '   Example - Update listing #3 to pending:';
    PRINT '   UPDATE SellerListings SET Status = 0 WHERE Id = 3;';
    PRINT '';
    PRINT '   Example - Update ALL rejected listings to pending:';
    PRINT '   UPDATE SellerListings SET Status = 0 WHERE Status = 2;';
END

PRINT '';
PRINT '========================================';
PRINT 'QUICK FIX OPTIONS';
PRINT '========================================';
PRINT '';
PRINT 'Option 1: Update specific listing to Pending';
PRINT '   UPDATE SellerListings SET Status = 0 WHERE Id = YOUR_LISTING_ID;';
PRINT '';
PRINT 'Option 2: Update ALL rejected listings to Pending';
PRINT '   UPDATE SellerListings SET Status = 0 WHERE Status = 2;';
PRINT '';
PRINT 'Option 3: Update ALL listings to Pending (for testing)';
PRINT '   UPDATE SellerListings SET Status = 0;';
PRINT '';

PRINT '========================================';
PRINT 'DIAGNOSTIC COMPLETE';
PRINT '========================================';
GO
