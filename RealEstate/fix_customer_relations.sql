-- Fix Customer Relations: Rename ClientID to CustomerId and set up FKs

-- 1. Drop old FK from Appointments to Clients
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointments_Clients_ClientID')
BEGIN
    ALTER TABLE [dbo].[Appointments] DROP CONSTRAINT [FK_Appointments_Clients_ClientID];
END

-- 2. Rename Customers.ClientID to CustomerId
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Customers' AND COLUMN_NAME = 'ClientID')
BEGIN
    EXEC sp_rename 'dbo.Customers.ClientID', 'CustomerId', 'COLUMN';
END

-- 3. Rename Appointments.ClientID to CustomerId
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'ClientID')
BEGIN
    EXEC sp_rename 'dbo.Appointments.ClientID', 'CustomerId', 'COLUMN';
END

-- 4. Rename index on Appointments if it exists
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_ClientID' AND object_id = OBJECT_ID('Appointments'))
BEGIN
    EXEC sp_rename 'dbo.Appointments.IX_Appointments_ClientID', 'IX_Appointments_CustomerId', 'INDEX';
END

-- 5. Add new FK from Appointments to Customers
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointments_Customers_CustomerId')
BEGIN
    ALTER TABLE [dbo].[Appointments] ADD CONSTRAINT [FK_Appointments_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([CustomerId]) ON DELETE CASCADE;
END

-- 6. Create ViewingAppointments table if it doesn't exist
IF OBJECT_ID(N'[dbo].[ViewingAppointments]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ViewingAppointments] (
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

    -- Create indexes
    CREATE INDEX [IX_ViewingAppointments_PropertyId] ON [dbo].[ViewingAppointments]([PropertyId]);
    CREATE INDEX [IX_ViewingAppointments_WhenUtc] ON [dbo].[ViewingAppointments]([WhenUtc]);
    CREATE INDEX [IX_ViewingAppointments_CustomerEmail] ON [dbo].[ViewingAppointments]([CustomerEmail]);
    CREATE INDEX [IX_ViewingAppointments_CustomerId] ON [dbo].[ViewingAppointments]([CustomerId]);

    -- Add FK to Customers
    ALTER TABLE [dbo].[ViewingAppointments] ADD CONSTRAINT [FK_ViewingAppointments_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([CustomerId]) ON DELETE SET NULL;
END

-- 7. Update migration history
IF NOT EXISTS (SELECT 1 FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = N'20260425183417_CustomerIdRenameAndRelations')
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260425183417_CustomerIdRenameAndRelations', N'9.0.4');
END
