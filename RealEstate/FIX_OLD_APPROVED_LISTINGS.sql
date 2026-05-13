-- =====================================================
-- FIX: Update Old "Approved" Listings to New Workflow
-- =====================================================

USE [DB_Real_Estate]
GO

PRINT '========================================';
PRINT 'FINDING LISTINGS WITH OLD "APPROVED" STATUS';
PRINT '========================================';
PRINT '';

-- Show all listings with Approved status (old workflow)
SELECT 
    Id,
    Title,
    Status,
    'Approved (Old)' AS CurrentStatus,
    SellerName,
    SuggestedPrice,
    FinalPrice,
    CreatedAtUtc
FROM SellerListings
WHERE Status = 1  -- Old "Approved" status
ORDER BY Id;

PRINT '';
PRINT '========================================';
PRINT 'FIXING: Converting to New Payment Workflow';
PRINT '========================================';
PRINT '';

-- Update all old "Approved" listings to "AwaitingManagerPayment"
UPDATE SellerListings 
SET Status = 4,  -- Change from Approved (1) to AwaitingManagerPayment (4)
    UpdatedAtUtc = GETUTCDATE()
WHERE Status = 1;

PRINT '✓ Updated all "Approved" listings to "AwaitingManagerPayment"';
PRINT '';

-- Verify the update
SELECT 
    Id,
    Title,
    Status,
    CASE Status
        WHEN 4 THEN 'AwaitingManagerPayment ✅ (New workflow - payment button will show)'
        ELSE 'Other'
    END AS NewStatus,
    SellerName,
    FinalPrice
FROM SellerListings
WHERE Status = 4
ORDER BY Id;

PRINT '';
PRINT '========================================';
PRINT 'NEXT STEPS';
PRINT '========================================';
PRINT '';
PRINT '1. Login as Manager';
PRINT '2. Go to the listing detail page';
PRINT '3. You will now see the "Complete Payment" button!';
PRINT '4. Click it to complete payment to seller';
PRINT '';
PRINT '========================================';
PRINT 'COMPLETE';
PRINT '========================================';
GO
