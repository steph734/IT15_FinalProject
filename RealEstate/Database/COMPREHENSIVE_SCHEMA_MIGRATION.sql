-- ═══════════════════════════════════════════════════════════════
-- COMPREHENSIVE DATABASE SCHEMA MIGRATION
-- EstateFlow Real Estate Management System
-- ═══════════════════════════════════════════════════════════════

-- This script creates all new tables for the comprehensive database schema
-- Run this AFTER ensuring the application builds successfully

USE [DB_Real_Estate]; -- Replace with your actual database name
GO

-- ═══════════════════════════════════════════════════════════════
-- 1. PROPERTY MANAGEMENT TABLES
-- ═══════════════════════════════════════════════════════════════

-- Properties Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Properties]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Properties] (
        [PropertyId] INT IDENTITY(1,1) NOT NULL,
        [SellerId] INT NOT NULL,
        [Title] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(2000) NULL,
        [PropertyType] NVARCHAR(50) NOT NULL,
        [Location] NVARCHAR(300) NOT NULL,
        [BasePrice] DECIMAL(18,2) NOT NULL,
        [FinalPrice] DECIMAL(18,2) NULL,
        [Status] NVARCHAR(50) NOT NULL CONSTRAINT [DF_Properties_Status] DEFAULT 'Pending',
        [ApprovedBy] INT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Properties_CreatedAt] DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [Sqft] INT NOT NULL DEFAULT 0,
        [Bedrooms] INT NOT NULL DEFAULT 1,
        [Bathrooms] INT NOT NULL DEFAULT 1,
        [ParkingSlots] INT NOT NULL DEFAULT 0,
        
        CONSTRAINT [PK_Properties] PRIMARY KEY CLUSTERED ([PropertyId] ASC)
    );
    
    -- Foreign Keys
    ALTER TABLE [dbo].[Properties] 
    ADD CONSTRAINT [FK_Properties_Users_Seller] 
    FOREIGN KEY ([SellerId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE NO ACTION;
    
    ALTER TABLE [dbo].[Properties] 
    ADD CONSTRAINT [FK_Properties_Users_Approver] 
    FOREIGN KEY ([ApprovedBy]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE SET NULL;
    
    -- Indexes
    CREATE NONCLUSTERED INDEX [IX_Properties_SellerId] ON [dbo].[Properties] ([SellerId]);
    CREATE NONCLUSTERED INDEX [IX_Properties_Status] ON [dbo].[Properties] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Properties_PropertyType] ON [dbo].[Properties] ([PropertyType]);
    
    PRINT '✓ Properties table created';
END
GO

-- PropertyImages Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PropertyImages]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PropertyImages] (
        [ImageId] INT IDENTITY(1,1) NOT NULL,
        [PropertyId] INT NOT NULL,
        [ImageUrl] NVARCHAR(500) NOT NULL,
        [IsPrimary] BIT NOT NULL DEFAULT 0,
        [UploadedAt] DATETIME2 NOT NULL CONSTRAINT [DF_PropertyImages_UploadedAt] DEFAULT GETUTCDATE(),
        
        CONSTRAINT [PK_PropertyImages] PRIMARY KEY CLUSTERED ([ImageId] ASC)
    );
    
    ALTER TABLE [dbo].[PropertyImages] 
    ADD CONSTRAINT [FK_PropertyImages_Properties] 
    FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Properties] ([PropertyId]) ON DELETE CASCADE;
    
    CREATE NONCLUSTERED INDEX [IX_PropertyImages_PropertyId] ON [dbo].[PropertyImages] ([PropertyId]);
    
    PRINT '✓ PropertyImages table created';
END
GO

-- PropertyDocuments Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PropertyDocuments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PropertyDocuments] (
        [DocumentId] INT IDENTITY(1,1) NOT NULL,
        [PropertyId] INT NOT NULL,
        [FilePath] NVARCHAR(500) NOT NULL,
        [DocumentType] NVARCHAR(50) NOT NULL,
        [UploadedAt] DATETIME2 NOT NULL CONSTRAINT [DF_PropertyDocuments_UploadedAt] DEFAULT GETUTCDATE(),
        
        CONSTRAINT [PK_PropertyDocuments] PRIMARY KEY CLUSTERED ([DocumentId] ASC)
    );
    
    ALTER TABLE [dbo].[PropertyDocuments] 
    ADD CONSTRAINT [FK_PropertyDocuments_Properties] 
    FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Properties] ([PropertyId]) ON DELETE CASCADE;
    
    CREATE NONCLUSTERED INDEX [IX_PropertyDocuments_PropertyId] ON [dbo].[PropertyDocuments] ([PropertyId]);
    
    PRINT '✓ PropertyDocuments table created';
END
GO

-- ═══════════════════════════════════════════════════════════════
-- 2. PRICING & MANAGER CONTROL TABLES
-- ═══════════════════════════════════════════════════════════════

-- PropertyPricing Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PropertyPricings]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PropertyPricings] (
        [PricingId] INT IDENTITY(1,1) NOT NULL,
        [PropertyId] INT NOT NULL,
        [BasePrice] DECIMAL(18,2) NOT NULL,
        [MarkupAmount] DECIMAL(18,2) NOT NULL,
        [FinalPrice] DECIMAL(18,2) NOT NULL,
        [SetBy] INT NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_PropertyPricings_CreatedAt] DEFAULT GETUTCDATE(),
        [Notes] NVARCHAR(500) NULL,
        
        CONSTRAINT [PK_PropertyPricings] PRIMARY KEY CLUSTERED ([PricingId] ASC)
    );
    
    ALTER TABLE [dbo].[PropertyPricings] 
    ADD CONSTRAINT [FK_PropertyPricings_Properties] 
    FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Properties] ([PropertyId]) ON DELETE CASCADE;
    
    ALTER TABLE [dbo].[PropertyPricings] 
    ADD CONSTRAINT [FK_PropertyPricings_Users_Manager] 
    FOREIGN KEY ([SetBy]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE NO ACTION;
    
    CREATE NONCLUSTERED INDEX [IX_PropertyPricings_PropertyId] ON [dbo].[PropertyPricings] ([PropertyId]);
    CREATE NONCLUSTERED INDEX [IX_PropertyPricings_SetBy] ON [dbo].[PropertyPricings] ([SetBy]);
    
    PRINT '✓ PropertyPricings table created';
END
GO

-- CommissionRules Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CommissionRules]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CommissionRules] (
        [RuleId] INT IDENTITY(1,1) NOT NULL,
        [ManagerId] INT NOT NULL,
        [CommissionPercent] DECIMAL(5,2) NOT NULL,
        [CompanySharePercent] DECIMAL(5,2) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_CommissionRules_CreatedAt] DEFAULT GETUTCDATE(),
        [IsActive] BIT NOT NULL DEFAULT 1,
        
        CONSTRAINT [PK_CommissionRules] PRIMARY KEY CLUSTERED ([RuleId] ASC)
    );
    
    ALTER TABLE [dbo].[CommissionRules] 
    ADD CONSTRAINT [FK_CommissionRules_Users_Manager] 
    FOREIGN KEY ([ManagerId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE NO ACTION;
    
    CREATE NONCLUSTERED INDEX [IX_CommissionRules_ManagerId] ON [dbo].[CommissionRules] ([ManagerId]);
    CREATE NONCLUSTERED INDEX [IX_CommissionRules_IsActive] ON [dbo].[CommissionRules] ([IsActive]);
    
    PRINT '✓ CommissionRules table created';
END
GO

-- ═══════════════════════════════════════════════════════════════
-- 3. CRM / LEADS / INQUIRIES TABLES
-- ═══════════════════════════════════════════════════════════════

-- Inquiries Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Inquiries]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Inquiries] (
        [InquiryId] INT IDENTITY(1,1) NOT NULL,
        [PropertyId] INT NOT NULL,
        [CustomerId] INT NULL,
        [AgentId] INT NULL,
        [CustomerName] NVARCHAR(100) NOT NULL,
        [CustomerEmail] NVARCHAR(255) NULL,
        [CustomerPhone] NVARCHAR(20) NULL,
        [Message] NVARCHAR(2000) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL CONSTRAINT [DF_Inquiries_Status] DEFAULT 'Pending',
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Inquiries_CreatedAt] DEFAULT GETUTCDATE(),
        [RespondedAt] DATETIME2 NULL,
        [Response] NVARCHAR(2000) NULL,
        
        CONSTRAINT [PK_Inquiries] PRIMARY KEY CLUSTERED ([InquiryId] ASC)
    );
    
    ALTER TABLE [dbo].[Inquiries] 
    ADD CONSTRAINT [FK_Inquiries_Properties] 
    FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Properties] ([PropertyId]) ON DELETE CASCADE;
    
    ALTER TABLE [dbo].[Inquiries] 
    ADD CONSTRAINT [FK_Inquiries_Users_Customer] 
    FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE NO ACTION;
    
    ALTER TABLE [dbo].[Inquiries] 
    ADD CONSTRAINT [FK_Inquiries_Users_Agent] 
    FOREIGN KEY ([AgentId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE NO ACTION;
    
    CREATE NONCLUSTERED INDEX [IX_Inquiries_PropertyId] ON [dbo].[Inquiries] ([PropertyId]);
    CREATE NONCLUSTERED INDEX [IX_Inquiries_CustomerId] ON [dbo].[Inquiries] ([CustomerId]);
    CREATE NONCLUSTERED INDEX [IX_Inquiries_Status] ON [dbo].[Inquiries] ([Status]);
    
    PRINT '✓ Inquiries table created';
END
GO

-- ═══════════════════════════════════════════════════════════════
-- 4. TRANSACTIONS (CORE BUSINESS) TABLES
-- ═══════════════════════════════════════════════════════════════

-- Transactions Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Transactions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Transactions] (
        [TransactionId] INT IDENTITY(1,1) NOT NULL,
        [PropertyId] INT NOT NULL,
        [AgentId] INT NULL,
        [CustomerId] INT NULL,
        [CustomerName] NVARCHAR(100) NOT NULL,
        [CustomerEmail] NVARCHAR(255) NULL,
        [CustomerPhone] NVARCHAR(20) NULL,
        [SellingPrice] DECIMAL(18,2) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL CONSTRAINT [DF_Transactions_Status] DEFAULT 'Pending',
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Transactions_CreatedAt] DEFAULT GETUTCDATE(),
        [ClosedAt] DATETIME2 NULL,
        [Notes] NVARCHAR(2000) NULL,
        
        CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED ([TransactionId] ASC)
    );
    
    ALTER TABLE [dbo].[Transactions] 
    ADD CONSTRAINT [FK_Transactions_Properties] 
    FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Properties] ([PropertyId]) ON DELETE NO ACTION;
    
    ALTER TABLE [dbo].[Transactions] 
    ADD CONSTRAINT [FK_Transactions_Users_Agent] 
    FOREIGN KEY ([AgentId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE NO ACTION;
    
    ALTER TABLE [dbo].[Transactions] 
    ADD CONSTRAINT [FK_Transactions_Users_Customer] 
    FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE NO ACTION;
    
    CREATE NONCLUSTERED INDEX [IX_Transactions_PropertyId] ON [dbo].[Transactions] ([PropertyId]);
    CREATE NONCLUSTERED INDEX [IX_Transactions_AgentId] ON [dbo].[Transactions] ([AgentId]);
    CREATE NONCLUSTERED INDEX [IX_Transactions_CustomerId] ON [dbo].[Transactions] ([CustomerId]);
    CREATE NONCLUSTERED INDEX [IX_Transactions_Status] ON [dbo].[Transactions] ([Status]);
    
    PRINT '✓ Transactions table created';
END
GO

-- Payments Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Payments] (
        [PaymentId] INT IDENTITY(1,1) NOT NULL,
        [TransactionId] INT NOT NULL,
        [Amount] DECIMAL(18,2) NOT NULL,
        [PaymentMethod] NVARCHAR(50) NOT NULL,
        [ReferenceNumber] NVARCHAR(100) NULL,
        [Status] NVARCHAR(50) NOT NULL CONSTRAINT [DF_Payments_Status] DEFAULT 'Pending',
        [RecordedBy] INT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Payments_CreatedAt] DEFAULT GETUTCDATE(),
        [CompletedAt] DATETIME2 NULL,
        [Notes] NVARCHAR(500) NULL,
        
        CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED ([PaymentId] ASC)
    );
    
    ALTER TABLE [dbo].[Payments] 
    ADD CONSTRAINT [FK_Payments_Transactions] 
    FOREIGN KEY ([TransactionId]) REFERENCES [dbo].[Transactions] ([TransactionId]) ON DELETE CASCADE;
    
    ALTER TABLE [dbo].[Payments] 
    ADD CONSTRAINT [FK_Payments_Users_RecordedBy] 
    FOREIGN KEY ([RecordedBy]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE SET NULL;
    
    CREATE NONCLUSTERED INDEX [IX_Payments_TransactionId] ON [dbo].[Payments] ([TransactionId]);
    CREATE NONCLUSTERED INDEX [IX_Payments_Status] ON [dbo].[Payments] ([Status]);
    
    PRINT '✓ Payments table created';
END
GO

-- Invoices Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Invoices]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Invoices] (
        [InvoiceId] INT IDENTITY(1,1) NOT NULL,
        [TransactionId] INT NOT NULL,
        [InvoiceNumber] NVARCHAR(50) NOT NULL,
        [Amount] DECIMAL(18,2) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL CONSTRAINT [DF_Invoices_Status] DEFAULT 'Pending',
        [IssuedDate] DATETIME2 NOT NULL CONSTRAINT [DF_Invoices_IssuedDate] DEFAULT GETUTCDATE(),
        [DueDate] DATETIME2 NULL,
        [PaidDate] DATETIME2 NULL,
        
        CONSTRAINT [PK_Invoices] PRIMARY KEY CLUSTERED ([InvoiceId] ASC)
    );
    
    ALTER TABLE [dbo].[Invoices] 
    ADD CONSTRAINT [FK_Invoices_Transactions] 
    FOREIGN KEY ([TransactionId]) REFERENCES [dbo].[Transactions] ([TransactionId]) ON DELETE CASCADE;
    
    CREATE NONCLUSTERED INDEX [IX_Invoices_TransactionId] ON [dbo].[Invoices] ([TransactionId]);
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Invoices_InvoiceNumber] ON [dbo].[Invoices] ([InvoiceNumber]);
    CREATE NONCLUSTERED INDEX [IX_Invoices_Status] ON [dbo].[Invoices] ([Status]);
    
    PRINT '✓ Invoices table created';
END
GO

-- ═══════════════════════════════════════════════════════════════
-- 5. COMMISSION SYSTEM TABLES
-- ═══════════════════════════════════════════════════════════════

-- Commissions Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Commissions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Commissions] (
        [CommissionId] INT IDENTITY(1,1) NOT NULL,
        [TransactionId] INT NOT NULL,
        [AgentId] INT NOT NULL,
        [CommissionAmount] DECIMAL(18,2) NOT NULL,
        [CommissionPercent] DECIMAL(5,2) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL CONSTRAINT [DF_Commissions_Status] DEFAULT 'Pending',
        [ApprovedBy] INT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Commissions_CreatedAt] DEFAULT GETUTCDATE(),
        [ApprovedAt] DATETIME2 NULL,
        [PaidAt] DATETIME2 NULL,
        [Notes] NVARCHAR(500) NULL,
        
        CONSTRAINT [PK_Commissions] PRIMARY KEY CLUSTERED ([CommissionId] ASC)
    );
    
    ALTER TABLE [dbo].[Commissions] 
    ADD CONSTRAINT [FK_Commissions_Transactions] 
    FOREIGN KEY ([TransactionId]) REFERENCES [dbo].[Transactions] ([TransactionId]) ON DELETE CASCADE;
    
    ALTER TABLE [dbo].[Commissions] 
    ADD CONSTRAINT [FK_Commissions_Users_Agent] 
    FOREIGN KEY ([AgentId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE NO ACTION;
    
    ALTER TABLE [dbo].[Commissions] 
    ADD CONSTRAINT [FK_Commissions_Users_Approver] 
    FOREIGN KEY ([ApprovedBy]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE SET NULL;
    
    CREATE NONCLUSTERED INDEX [IX_Commissions_TransactionId] ON [dbo].[Commissions] ([TransactionId]);
    CREATE NONCLUSTERED INDEX [IX_Commissions_AgentId] ON [dbo].[Commissions] ([AgentId]);
    CREATE NONCLUSTERED INDEX [IX_Commissions_Status] ON [dbo].[Commissions] ([Status]);
    
    PRINT '✓ Commissions table created';
END
GO

-- Payouts Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payouts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Payouts] (
        [PayoutId] INT IDENTITY(1,1) NOT NULL,
        [CommissionId] INT NOT NULL,
        [Amount] DECIMAL(18,2) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL CONSTRAINT [DF_Payouts_Status] DEFAULT 'Pending',
        [AuthorizedBy] INT NULL,
        [ProcessedBy] INT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Payouts_CreatedAt] DEFAULT GETUTCDATE(),
        [AuthorizedAt] DATETIME2 NULL,
        [ProcessedAt] DATETIME2 NULL,
        [PaidDate] DATETIME2 NULL,
        [Notes] NVARCHAR(500) NULL,
        
        CONSTRAINT [PK_Payouts] PRIMARY KEY CLUSTERED ([PayoutId] ASC)
    );
    
    ALTER TABLE [dbo].[Payouts] 
    ADD CONSTRAINT [FK_Payouts_Commissions] 
    FOREIGN KEY ([CommissionId]) REFERENCES [dbo].[Commissions] ([CommissionId]) ON DELETE CASCADE;
    
    ALTER TABLE [dbo].[Payouts] 
    ADD CONSTRAINT [FK_Payouts_Users_Authorizer] 
    FOREIGN KEY ([AuthorizedBy]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE NO ACTION;
    
    ALTER TABLE [dbo].[Payouts] 
    ADD CONSTRAINT [FK_Payouts_Users_Processor] 
    FOREIGN KEY ([ProcessedBy]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE NO ACTION;
    
    CREATE NONCLUSTERED INDEX [IX_Payouts_CommissionId] ON [dbo].[Payouts] ([CommissionId]);
    CREATE NONCLUSTERED INDEX [IX_Payouts_Status] ON [dbo].[Payouts] ([Status]);
    
    PRINT '✓ Payouts table created';
END
GO

-- ═══════════════════════════════════════════════════════════════
-- 6. FINANCIAL RECORDS & AUDIT LOGS TABLES
-- ═══════════════════════════════════════════════════════════════

-- FinancialRecords Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FinancialRecords]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FinancialRecords] (
        [RecordId] INT IDENTITY(1,1) NOT NULL,
        [TransactionId] INT NULL,
        [Type] NVARCHAR(50) NOT NULL,
        [Amount] DECIMAL(18,2) NOT NULL,
        [Category] NVARCHAR(100) NULL,
        [Description] NVARCHAR(1000) NULL,
        [RecordedBy] INT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_FinancialRecords_CreatedAt] DEFAULT GETUTCDATE(),
        [ReferenceNumber] NVARCHAR(100) NULL,
        
        CONSTRAINT [PK_FinancialRecords] PRIMARY KEY CLUSTERED ([RecordId] ASC)
    );
    
    ALTER TABLE [dbo].[FinancialRecords] 
    ADD CONSTRAINT [FK_FinancialRecords_Transactions] 
    FOREIGN KEY ([TransactionId]) REFERENCES [dbo].[Transactions] ([TransactionId]) ON DELETE SET NULL;
    
    ALTER TABLE [dbo].[FinancialRecords] 
    ADD CONSTRAINT [FK_FinancialRecords_Users_RecordedBy] 
    FOREIGN KEY ([RecordedBy]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE SET NULL;
    
    CREATE NONCLUSTERED INDEX [IX_FinancialRecords_Type] ON [dbo].[FinancialRecords] ([Type]);
    CREATE NONCLUSTERED INDEX [IX_FinancialRecords_CreatedAt] ON [dbo].[FinancialRecords] ([CreatedAt]);
    
    PRINT '✓ FinancialRecords table created';
END
GO

-- AuditLogs Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AuditLogs] (
        [LogId] INT IDENTITY(1,1) NOT NULL,
        [UserId] INT NULL,
        [UserRole] NVARCHAR(50) NULL,
        [Action] NVARCHAR(100) NOT NULL,
        [EntityType] NVARCHAR(100) NOT NULL,
        [EntityId] INT NULL,
        [Description] NVARCHAR(2000) NULL,
        [IPAddress] NVARCHAR(50) NULL,
        [UserAgent] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_AuditLogs_CreatedAt] DEFAULT GETUTCDATE(),
        [OldValues] NVARCHAR(4000) NULL,
        [NewValues] NVARCHAR(4000) NULL,
        
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY CLUSTERED ([LogId] ASC)
    );
    
    ALTER TABLE [dbo].[AuditLogs] 
    ADD CONSTRAINT [FK_AuditLogs_Users] 
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE SET NULL;
    
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_UserId] ON [dbo].[AuditLogs] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_EntityType] ON [dbo].[AuditLogs] ([EntityType]);
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_EntityId] ON [dbo].[AuditLogs] ([EntityId]);
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_CreatedAt] ON [dbo].[AuditLogs] ([CreatedAt]);
    
    PRINT '✓ AuditLogs table created';
END
GO

-- ═══════════════════════════════════════════════════════════════
-- SEED DATA
-- ═══════════════════════════════════════════════════════════════

-- Seed default commission rules for existing managers
IF NOT EXISTS (SELECT 1 FROM [CommissionRules])
BEGIN
    INSERT INTO [CommissionRules] ([ManagerId], [CommissionPercent], [CompanySharePercent], [CreatedAt], [IsActive])
    SELECT 
        u.[UserId],
        3.00,  -- 3% agent commission
        2.00,  -- 2% company share
        GETUTCDATE(),
        1
    FROM [Users] u
    INNER JOIN [Roles] r ON u.[RoleId] = r.[RoleId]
    WHERE r.[RoleType] = 'Manager'
    AND NOT EXISTS (
        SELECT 1 FROM [CommissionRules] cr WHERE cr.[ManagerId] = u.[UserId]
    );
    
    PRINT '✓ Commission rules seeded for managers';
END
GO

-- ═══════════════════════════════════════════════════════════════
-- VERIFICATION
-- ═══════════════════════════════════════════════════════════════

PRINT '';
PRINT '═══════════════════════════════════════════════════════════';
PRINT 'Migration Complete! Tables Created:';
PRINT '═══════════════════════════════════════════════════════════';

SELECT 
    t.name AS TableName,
    SUM(p.rows) AS RowCount
FROM sys.tables t
INNER JOIN sys.partitions p ON t.object_id = p.object_id AND p.index_id IN (0, 1)
WHERE t.name IN (
    'Properties', 'PropertyImages', 'PropertyDocuments',
    'PropertyPricings', 'CommissionRules',
    'Inquiries',
    'Transactions', 'Payments', 'Invoices',
    'Commissions', 'Payouts',
    'FinancialRecords', 'AuditLogs'
)
GROUP BY t.name
ORDER BY t.name;

PRINT '';
PRINT '✓ All tables created successfully!';
PRINT '═══════════════════════════════════════════════════════════';
GO
