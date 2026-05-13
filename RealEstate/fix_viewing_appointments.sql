-- Create ViewingAppointments table with all columns (original + new)
IF OBJECT_ID(N'[ViewingAppointments]', N'U') IS NULL
BEGIN
    CREATE TABLE [ViewingAppointments] (
        [Id] int NOT NULL IDENTITY,
        [PropertyId] int NOT NULL,
        [CustomerId] int NULL,
        [CustomerName] nvarchar(100) NOT NULL,
        [CustomerEmail] nvarchar(255) NULL,
        [CustomerPhone] nvarchar(20) NULL,
        [CustomerPhotoUrl] nvarchar(500) NULL,
        [WhenUtc] datetime2 NOT NULL,
        [PreferredTime] nvarchar(20) NULL,
        [NumberOfVisitors] int NOT NULL DEFAULT 1,
        [BuyerType] nvarchar(50) NULL,
        [FinancingStatus] nvarchar(50) NULL,
        [InformationSource] nvarchar(50) NULL,
        [Notes] nvarchar(1000) NULL,
        [CreatedAtUtc] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [Status] nvarchar(50) NOT NULL DEFAULT N'Scheduled',
        CONSTRAINT [PK_ViewingAppointments] PRIMARY KEY ([Id])
    );
END
ELSE
BEGIN
    -- Table exists but may be missing columns - add them safely
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'CustomerEmail' AND Object_ID = Object_ID(N'ViewingAppointments'))
        ALTER TABLE [ViewingAppointments] ADD [CustomerEmail] nvarchar(255) NULL;
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'PreferredTime' AND Object_ID = Object_ID(N'ViewingAppointments'))
        ALTER TABLE [ViewingAppointments] ADD [PreferredTime] nvarchar(20) NULL;
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'NumberOfVisitors' AND Object_ID = Object_ID(N'ViewingAppointments'))
        ALTER TABLE [ViewingAppointments] ADD [NumberOfVisitors] int NOT NULL DEFAULT 1;
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'BuyerType' AND Object_ID = Object_ID(N'ViewingAppointments'))
        ALTER TABLE [ViewingAppointments] ADD [BuyerType] nvarchar(50) NULL;
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'FinancingStatus' AND Object_ID = Object_ID(N'ViewingAppointments'))
        ALTER TABLE [ViewingAppointments] ADD [FinancingStatus] nvarchar(50) NULL;
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'InformationSource' AND Object_ID = Object_ID(N'ViewingAppointments'))
        ALTER TABLE [ViewingAppointments] ADD [InformationSource] nvarchar(50) NULL;
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'Notes' AND Object_ID = Object_ID(N'ViewingAppointments'))
        ALTER TABLE [ViewingAppointments] ADD [Notes] nvarchar(1000) NULL;
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'CreatedAtUtc' AND Object_ID = Object_ID(N'ViewingAppointments'))
        ALTER TABLE [ViewingAppointments] ADD [CreatedAtUtc] datetime2 NOT NULL DEFAULT GETUTCDATE();
END

-- Create indexes
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IX_ViewingAppointments_PropertyId' AND object_id = OBJECT_ID('ViewingAppointments'))
    CREATE INDEX [IX_ViewingAppointments_PropertyId] ON [ViewingAppointments] ([PropertyId]);

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IX_ViewingAppointments_WhenUtc' AND object_id = OBJECT_ID('ViewingAppointments'))
    CREATE INDEX [IX_ViewingAppointments_WhenUtc] ON [ViewingAppointments] ([WhenUtc]);

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IX_ViewingAppointments_CustomerEmail' AND object_id = OBJECT_ID('ViewingAppointments'))
    CREATE INDEX [IX_ViewingAppointments_CustomerEmail] ON [ViewingAppointments] ([CustomerEmail]);

-- Mark migration as applied if not already
IF NOT EXISTS(SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260425173933_AddViewingAppointmentDetails')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260425173933_AddViewingAppointmentDetails', N'10.0.5');
GO
