-- ============================================================
-- Seed Predefined Users for EstateFlow
-- Roles: Manager, Broker, Seller, Accounting
-- Passwords are SHA-256 hashed (same as UserService.HashPassword)
-- ============================================================

-- Manager
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Username] = 'manager' OR [Email] = 'manager@estateflow.com')
BEGIN
    INSERT INTO [dbo].[Users] ([FullName], [Email], [Username], [Password], [PasswordHash], [Role], [Status], [IsActive], [CreatedAt])
    VALUES ('System Manager', 'manager@estateflow.com', 'manager', '', '6DkpJamMnCJ5XR/F0N/uW5ppQ/a3aOxaKgwHfl7RGc8=', 'Manager', 'Active', 1, GETUTCDATE())
END

-- Broker
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Username] = 'broker' OR [Email] = 'broker@estateflow.com')
BEGIN
    INSERT INTO [dbo].[Users] ([FullName], [Email], [Username], [Password], [PasswordHash], [Role], [Status], [IsActive], [CreatedAt])
    VALUES ('System Broker', 'broker@estateflow.com', 'broker', '', 'NeOfeT2z9jLkREUUY4sBMesXU74l4dikshrkVNRZa2k=', 'Broker', 'Active', 1, GETUTCDATE())
END

-- Seller
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Username] = 'seller' OR [Email] = 'seller@estateflow.com')
BEGIN
    INSERT INTO [dbo].[Users] ([FullName], [Email], [Username], [Password], [PasswordHash], [Role], [Status], [IsActive], [CreatedAt])
    VALUES ('System Seller', 'seller@estateflow.com', 'seller', '', 'vSjJSADCvgVbMyn43WOj1aQTfA3vJRe/T86F6xHmKFM=', 'Seller', 'Active', 1, GETUTCDATE())
END

-- Accounting
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Username] = 'accounting' OR [Email] = 'accounting@estateflow.com')
BEGIN
    INSERT INTO [dbo].[Users] ([FullName], [Email], [Username], [Password], [PasswordHash], [Role], [Status], [IsActive], [CreatedAt])
    VALUES ('System Accounting', 'accounting@estateflow.com', 'accounting', '', 'bCMltjf8GcJwr6TS1uB8k8y8NyYZzwsjGwgpAR0+zzI=', 'Accounting', 'Active', 1, GETUTCDATE())
END

-- Verify inserted rows
SELECT [UserId], [FullName], [Email], [Username], [Role], [Status], [IsActive], [CreatedAt]
FROM [dbo].[Users]
ORDER BY [UserId];
