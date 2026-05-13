-- =====================================================
-- EstateFlow Real Estate Database Schema
-- Created: 2026
-- Purpose: Complete database structure for EstateFlow
-- =====================================================

-- =====================================================
-- 1. ROLES TABLE
-- =====================================================
CREATE TABLE [Roles] (
    [RoleId] INT PRIMARY KEY IDENTITY(1,1),
    [RoleType] NVARCHAR(50) NOT NULL UNIQUE,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE()
);

-- =====================================================
-- 2. USERS TABLE
-- =====================================================
CREATE TABLE [Users] (
    [UserId] INT PRIMARY KEY IDENTITY(1,1),
    [FullName] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL UNIQUE,
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [RoleId] INT NOT NULL,
    [IsActive] BIT DEFAULT 1,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    FOREIGN KEY ([RoleId]) REFERENCES [Roles]([RoleId]) ON DELETE RESTRICT
);

-- Create indexes for Users
CREATE INDEX [IX_Users_Email] ON [Users]([Email]);
CREATE INDEX [IX_Users_RoleId] ON [Users]([RoleId]);
CREATE INDEX [IX_Users_IsActive] ON [Users]([IsActive]);

-- =====================================================
-- 3. CUSTOMERS TABLE (EXTENDED)
-- =====================================================
CREATE TABLE [Customers] (
    [ClientID] INT PRIMARY KEY IDENTITY(1,1),
    [BrokerId] INT NULL,
    
    -- Personal Information
    [FullName] NVARCHAR(MAX) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [Phone] NVARCHAR(20),
    [Address] NVARCHAR(MAX),
    [City] NVARCHAR(100),
    [State] NVARCHAR(100),
    [ZipCode] NVARCHAR(20),
    [Country] NVARCHAR(100),
    
    -- Property Information
    [PropertyType] NVARCHAR(50) DEFAULT 'Residential', -- Residential, Commercial, Industrial
    [InterestedProperties] NVARCHAR(MAX),
    [MinBudget] DECIMAL(18,2),
    [MaxBudget] DECIMAL(18,2),
    [Status] NVARCHAR(50) DEFAULT 'Interested', -- Interested, Follow-up, Under Review
    
    -- Payment Information
    [PaymentMethod] NVARCHAR(50) DEFAULT 'Mastercard', -- Mastercard, Visa, Paypal, Bank Transfer
    [CardholderName] NVARCHAR(100),
    [CardNumber] NVARCHAR(MAX),
    [ExpiryDate] NVARCHAR(10),
    [CVV] NVARCHAR(10),
    
    -- Metadata
    [Notes] NVARCHAR(MAX),
    [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
    [LastContactedDate] DATETIME2,
    [IsActive] BIT DEFAULT 1
);

-- Create indexes for Customers
CREATE INDEX [IX_Customers_Email] ON [Customers]([Email]);
CREATE INDEX [IX_Customers_Status] ON [Customers]([Status]);
CREATE INDEX [IX_Customers_PropertyType] ON [Customers]([PropertyType]);
CREATE INDEX [IX_Customers_BrokerId] ON [Customers]([BrokerId]);

-- =====================================================
-- 4. PAYMENT TRANSACTIONS TABLE
-- =====================================================
CREATE TABLE [PaymentTransactions] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [CustomerId] INT NOT NULL,
    [PayMongoPaymentIntentId] NVARCHAR(MAX),
    [PayMongoSourceId] NVARCHAR(MAX),
    [Amount] DECIMAL(18,2) NOT NULL,
    [Currency] NVARCHAR(10) DEFAULT 'PHP',
    [Status] NVARCHAR(50) DEFAULT 'pending', -- pending, succeeded, failed
    [PaymentMethod] NVARCHAR(50),
    [Description] NVARCHAR(MAX),
    [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedDate] DATETIME2,
    [WebhookResponse] NVARCHAR(MAX),
    [ErrorMessage] NVARCHAR(MAX),
    [IsProcessed] BIT DEFAULT 0,
    FOREIGN KEY ([CustomerId]) REFERENCES [Customers]([ClientID]) ON DELETE CASCADE
);

-- Create indexes for PaymentTransactions
CREATE INDEX [IX_PaymentTransactions_CustomerId] ON [PaymentTransactions]([CustomerId]);
CREATE INDEX [IX_PaymentTransactions_Status] ON [PaymentTransactions]([Status]);
CREATE INDEX [IX_PaymentTransactions_PayMongoPaymentIntentId] ON [PaymentTransactions]([PayMongoPaymentIntentId]);

-- =====================================================
-- 5. VIEWING APPOINTMENTS TABLE
-- =====================================================
CREATE TABLE [ViewingAppointments] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [PropertyId] INT,
    [ClientName] NVARCHAR(100) NOT NULL,
    [ClientEmail] NVARCHAR(255),
    [ClientPhone] NVARCHAR(20),
    [AppointmentDate] DATETIME2 NOT NULL,
    [Status] NVARCHAR(50) DEFAULT 'Pending', -- Pending, Confirmed, Cancelled, Completed
    [Notes] NVARCHAR(MAX),
    [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
    [CreatedBy] INT
);

-- Create indexes for ViewingAppointments
CREATE INDEX [IX_ViewingAppointments_AppointmentDate] ON [ViewingAppointments]([AppointmentDate]);
CREATE INDEX [IX_ViewingAppointments_Status] ON [ViewingAppointments]([Status]);

-- =====================================================
-- 6. OTP VERIFICATIONS TABLE
-- =====================================================
CREATE TABLE [OtpVerifications] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Email] NVARCHAR(255) NOT NULL,
    [OtpCode] NVARCHAR(10) NOT NULL,
    [ExpiresAt] DATETIME2 NOT NULL,
    [IsVerified] BIT DEFAULT 0,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [VerifiedAt] DATETIME2
);

-- Create indexes for OtpVerifications
CREATE INDEX [IX_OtpVerifications_Email] ON [OtpVerifications]([Email]);
CREATE INDEX [IX_OtpVerifications_ExpiresAt] ON [OtpVerifications]([ExpiresAt]);

-- =====================================================
-- 7. CLIENTS TABLE
-- =====================================================
CREATE TABLE [Clients] (
    [ClientId] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(255),
    [Phone] NVARCHAR(20),
    [Address] NVARCHAR(MAX),
    [City] NVARCHAR(100),
    [State] NVARCHAR(100),
    [Country] NVARCHAR(100),
    [ZipCode] NVARCHAR(20),
    [ClientType] NVARCHAR(50), -- Buyer, Seller, Investor
    [Status] NVARCHAR(50) DEFAULT 'Active',
    [Notes] NVARCHAR(MAX),
    [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedDate] DATETIME2
);

-- Create indexes for Clients
CREATE INDEX [IX_Clients_Email] ON [Clients]([Email]);
CREATE INDEX [IX_Clients_Status] ON [Clients]([Status]);

-- =====================================================
-- 8. APPOINTMENTS TABLE
-- =====================================================
CREATE TABLE [Appointments] (
    [AppointmentId] INT PRIMARY KEY IDENTITY(1,1),
    [ClientId] INT,
    [AgentId] INT,
    [AppointmentDate] DATETIME2 NOT NULL,
    [Duration] INT, -- Duration in minutes
    [Subject] NVARCHAR(MAX),
    [Description] NVARCHAR(MAX),
    [Status] NVARCHAR(50) DEFAULT 'Scheduled', -- Scheduled, Completed, Cancelled
    [Location] NVARCHAR(MAX),
    [Notes] NVARCHAR(MAX),
    [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
    [CreatedBy] INT,
    FOREIGN KEY ([ClientId]) REFERENCES [Clients]([ClientId]) ON DELETE CASCADE
);

-- Create indexes for Appointments
CREATE INDEX [IX_Appointments_AppointmentDate] ON [Appointments]([AppointmentDate]);
CREATE INDEX [IX_Appointments_Status] ON [Appointments]([Status]);
CREATE INDEX [IX_Appointments_ClientId] ON [Appointments]([ClientId]);

-- =====================================================
-- 9. SCHEDULES TABLE
-- =====================================================
CREATE TABLE [Schedules] (
    [ScheduleId] INT PRIMARY KEY IDENTITY(1,1),
    [UserId] INT,
    [ScheduleDate] DATE NOT NULL,
    [StartTime] TIME NOT NULL,
    [EndTime] TIME NOT NULL,
    [Title] NVARCHAR(MAX),
    [Description] NVARCHAR(MAX),
    [Color] NVARCHAR(50), -- For calendar display
    [IsRecurring] BIT DEFAULT 0,
    [RecurrencePattern] NVARCHAR(50), -- Daily, Weekly, Monthly
    [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE
);

-- Create indexes for Schedules
CREATE INDEX [IX_Schedules_UserId] ON [Schedules]([UserId]);
CREATE INDEX [IX_Schedules_ScheduleDate] ON [Schedules]([ScheduleDate]);

-- =====================================================
-- 10. PROPERTIES TABLE (Optional - for future use)
-- =====================================================
CREATE TABLE [Properties] (
    [PropertyId] INT PRIMARY KEY IDENTITY(1,1),
    [Title] NVARCHAR(MAX) NOT NULL,
    [Description] NVARCHAR(MAX),
    [Address] NVARCHAR(MAX) NOT NULL,
    [City] NVARCHAR(100),
    [State] NVARCHAR(100),
    [Country] NVARCHAR(100),
    [ZipCode] NVARCHAR(20),
    [Price] DECIMAL(18,2),
    [PropertyType] NVARCHAR(50), -- Residential, Commercial, Industrial
    [Bedrooms] INT,
    [Bathrooms] INT,
    [SquareFeet] DECIMAL(10,2),
    [Status] NVARCHAR(50) DEFAULT 'Available', -- Available, Sold, Rented
    [ListingDate] DATETIME2,
    [AgentId] INT,
    [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedDate] DATETIME2
);

-- Create indexes for Properties
CREATE INDEX [IX_Properties_PropertyType] ON [Properties]([PropertyType]);
CREATE INDEX [IX_Properties_Status] ON [Properties]([Status]);
CREATE INDEX [IX_Properties_City] ON [Properties]([City]);
CREATE INDEX [IX_Properties_Price] ON [Properties]([Price]);

-- =====================================================
-- 11. BROKERS TABLE (Optional - for future use)
-- =====================================================
CREATE TABLE [Brokers] (
    [BrokerId] INT PRIMARY KEY IDENTITY(1,1),
    [UserId] INT NOT NULL,
    [CompanyName] NVARCHAR(MAX),
    [LicenseNumber] NVARCHAR(50),
    [Phone] NVARCHAR(20),
    [Email] NVARCHAR(255),
    [Address] NVARCHAR(MAX),
    [City] NVARCHAR(100),
    [State] NVARCHAR(100),
    [Country] NVARCHAR(100),
    [ZipCode] NVARCHAR(20),
    [CommissionRate] DECIMAL(5,2),
    [IsActive] BIT DEFAULT 1,
    [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE
);

-- Create indexes for Brokers
CREATE INDEX [IX_Brokers_UserId] ON [Brokers]([UserId]);
CREATE INDEX [IX_Brokers_IsActive] ON [Brokers]([IsActive]);

-- =====================================================
-- 12. TRANSACTIONS TABLE (Optional - for tracking)
-- =====================================================
CREATE TABLE [Transactions] (
    [TransactionId] INT PRIMARY KEY IDENTITY(1,1),
    [PropertyId] INT,
    [BuyerId] INT,
    [SellerId] INT,
    [AgentId] INT,
    [TransactionDate] DATETIME2 NOT NULL,
    [SalePrice] DECIMAL(18,2),
    [Commission] DECIMAL(18,2),
    [Status] NVARCHAR(50) DEFAULT 'Pending', -- Pending, Completed, Cancelled
    [Notes] NVARCHAR(MAX),
    [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY ([PropertyId]) REFERENCES [Properties]([PropertyId]) ON DELETE SET NULL,
    FOREIGN KEY ([BuyerId]) REFERENCES [Clients]([ClientId]) ON DELETE SET NULL,
    FOREIGN KEY ([SellerId]) REFERENCES [Clients]([ClientId]) ON DELETE SET NULL
);

-- Create indexes for Transactions
CREATE INDEX [IX_Transactions_PropertyId] ON [Transactions]([PropertyId]);
CREATE INDEX [IX_Transactions_Status] ON [Transactions]([Status]);
CREATE INDEX [IX_Transactions_TransactionDate] ON [Transactions]([TransactionDate]);

-- =====================================================
-- 13. COMMISSIONS TABLE
-- =====================================================
CREATE TABLE [Commissions] (
    [CommissionId] INT PRIMARY KEY IDENTITY(1,1),
    [TransactionId] INT NOT NULL,
    [BrokerId] INT NOT NULL,
    [AgentId] INT,

    [SaleAmount] DECIMAL(18,2) NOT NULL,
    [CommissionRate] DECIMAL(5,2) NOT NULL,
    [CommissionAmount] DECIMAL(18,2) NOT NULL,

    [Status] NVARCHAR(50) DEFAULT 'Pending',
    [ApprovedDate] DATETIME2,
    [ApprovedBy] INT,
    [PaymentDate] DATETIME2,

    [Notes] NVARCHAR(MAX),
    [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedDate] DATETIME2,

    FOREIGN KEY ([TransactionId]) REFERENCES [Transactions]([TransactionId]) ON DELETE CASCADE,
    FOREIGN KEY ([BrokerId]) REFERENCES [Brokers]([BrokerId]) ON DELETE CASCADE,
    FOREIGN KEY ([AgentId]) REFERENCES [Users]([UserId]) ON DELETE SET NULL
);

-- Create indexes for Commissions
CREATE INDEX [IX_Commissions_TransactionId] ON [Commissions]([TransactionId]);
CREATE INDEX [IX_Commissions_BrokerId] ON [Commissions]([BrokerId]);
CREATE INDEX [IX_Commissions_AgentId] ON [Commissions]([AgentId]);
CREATE INDEX [IX_Commissions_Status] ON [Commissions]([Status]);
CREATE INDEX [IX_Commissions_PaymentDate] ON [Commissions]([PaymentDate]);

-- =====================================================
-- 14. INVOICES TABLE
-- =====================================================
CREATE TABLE [Invoices] (
    [InvoiceId] INT PRIMARY KEY IDENTITY(1,1),
    [InvoiceNumber] NVARCHAR(50) NOT NULL UNIQUE,

    [InvoiceType] NVARCHAR(50),
    [IssuedDate] DATETIME2 NOT NULL,
    [DueDate] DATETIME2,

    [BrokerId] INT,
    [ClientId] INT,
    [PropertyId] INT,
    [TransactionId] INT,

    [SubTotal] DECIMAL(18,2),
    [TaxAmount] DECIMAL(18,2),
    [DiscountAmount] DECIMAL(18,2),
    [TotalAmount] DECIMAL(18,2) NOT NULL,

    [Status] NVARCHAR(50) DEFAULT 'Draft',
    [PaymentStatus] NVARCHAR(50) DEFAULT 'Unpaid',
    [AmountPaid] DECIMAL(18,2) DEFAULT 0,
    [OutstandingAmount] DECIMAL(18,2),

    [PaymentMethod] NVARCHAR(50),
    [PaymentDate] DATETIME2,
    [PaymentReferenceNumber] NVARCHAR(100),

    [Description] NVARCHAR(MAX),
    [Notes] NVARCHAR(MAX),
    [CreatedBy] INT,
    [SentDate] DATETIME2,
    [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedDate] DATETIME2,

    FOREIGN KEY ([BrokerId]) REFERENCES [Brokers]([BrokerId]) ON DELETE SET NULL,
    FOREIGN KEY ([ClientId]) REFERENCES [Clients]([ClientId]) ON DELETE SET NULL,
    FOREIGN KEY ([PropertyId]) REFERENCES [Properties]([PropertyId]) ON DELETE SET NULL,
    FOREIGN KEY ([TransactionId]) REFERENCES [Transactions]([TransactionId]) ON DELETE SET NULL
);

-- Create indexes for Invoices
CREATE INDEX [IX_Invoices_InvoiceNumber] ON [Invoices]([InvoiceNumber]);
CREATE INDEX [IX_Invoices_BrokerId] ON [Invoices]([BrokerId]);
CREATE INDEX [IX_Invoices_ClientId] ON [Invoices]([ClientId]);
CREATE INDEX [IX_Invoices_Status] ON [Invoices]([Status]);
CREATE INDEX [IX_Invoices_PaymentStatus] ON [Invoices]([PaymentStatus]);
CREATE INDEX [IX_Invoices_IssuedDate] ON [Invoices]([IssuedDate]);
CREATE INDEX [IX_Invoices_DueDate] ON [Invoices]([DueDate]);

-- =====================================================
-- 15. PAYROLL TABLE
-- =====================================================
CREATE TABLE [Payroll] (
    [PayrollId] INT PRIMARY KEY IDENTITY(1,1),
    [UserId] INT NOT NULL,
    [BrokerId] INT NOT NULL,

    [PayrollMonth] DATE NOT NULL,
    [PayPeriodStart] DATE NOT NULL,
    [PayPeriodEnd] DATE NOT NULL,
    [PaymentDate] DATETIME2 NOT NULL,

    [BaseSalary] DECIMAL(18,2),
    [CommissionEarned] DECIMAL(18,2) DEFAULT 0,
    [Bonuses] DECIMAL(18,2) DEFAULT 0,
    [OtherAllowances] DECIMAL(18,2) DEFAULT 0,
    [GrossSalary] DECIMAL(18,2) NOT NULL,

    [IncomeTax] DECIMAL(18,2) DEFAULT 0,
    [SSS] DECIMAL(18,2) DEFAULT 0,
    [PhilHealth] DECIMAL(18,2) DEFAULT 0,
    [PagIbig] DECIMAL(18,2) DEFAULT 0,
    [HealthInsurance] DECIMAL(18,2) DEFAULT 0,
    [Loans] DECIMAL(18,2) DEFAULT 0,
    [OtherDeductions] DECIMAL(18,2) DEFAULT 0,
    [TotalDeductions] DECIMAL(18,2) NOT NULL,

    [NetSalary] DECIMAL(18,2) NOT NULL,

    [Status] NVARCHAR(50) DEFAULT 'Draft',
    [ApprovedBy] INT,
    [ApprovedDate] DATETIME2,
    [PaymentStatus] NVARCHAR(50) DEFAULT 'Unpaid',
    [PaymentMethod] NVARCHAR(50),
    [BankAccountNumber] NVARCHAR(50),

    [Notes] NVARCHAR(MAX),
    [AttendanceRecordId] INT,
    [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedDate] DATETIME2,

    FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE,
    FOREIGN KEY ([BrokerId]) REFERENCES [Brokers]([BrokerId]) ON DELETE CASCADE,
    FOREIGN KEY ([ApprovedBy]) REFERENCES [Users]([UserId]) ON DELETE SET NULL
);

-- Create indexes for Payroll
CREATE INDEX [IX_Payroll_UserId] ON [Payroll]([UserId]);
CREATE INDEX [IX_Payroll_BrokerId] ON [Payroll]([BrokerId]);
CREATE INDEX [IX_Payroll_PayrollMonth] ON [Payroll]([PayrollMonth]);
CREATE INDEX [IX_Payroll_Status] ON [Payroll]([Status]);
CREATE INDEX [IX_Payroll_PaymentStatus] ON [Payroll]([PaymentStatus]);
CREATE INDEX [IX_Payroll_PaymentDate] ON [Payroll]([PaymentDate]);

-- =====================================================
-- 16. INVESTORS TABLE
-- =====================================================
CREATE TABLE [Investors] (
    [InvestorId] INT PRIMARY KEY IDENTITY(1,1),
    [UserId] INT,

    -- Investor Information
    [InvestorName] NVARCHAR(MAX) NOT NULL,
    [Email] NVARCHAR(255),
    [Phone] NVARCHAR(20),

    -- Address Information
    [Address] NVARCHAR(MAX),
    [City] NVARCHAR(100),
    [State] NVARCHAR(100),
    [Country] NVARCHAR(100),
    [ZipCode] NVARCHAR(20),

    -- Investment Details
    [InvestorType] NVARCHAR(50), -- Individual, Corporate, Partnership, Trust
    [TaxId] NVARCHAR(50),
    [LicenseNumber] NVARCHAR(50),
    [InitialInvestment] DECIMAL(18,2),
    [TotalInvestment] DECIMAL(18,2),
    [AvailableFunds] DECIMAL(18,2),

    -- Portfolio Tracking
    [NumberOfProperties] INT DEFAULT 0,
    [TotalReturns] DECIMAL(18,2) DEFAULT 0,
    [AverageROI] DECIMAL(5,2) DEFAULT 0,

    -- Status & Compliance
    [Status] NVARCHAR(50) DEFAULT 'Active', -- Active, Inactive, Suspended, Under Review
    [VerificationStatus] NVARCHAR(50) DEFAULT 'Pending', -- Pending, Verified, Rejected
    [ApprovedBy] INT,
    [ApprovedDate] DATETIME2,
    [ComplianceNotes] NVARCHAR(MAX),

    -- Preferences
    [PreferredPropertyType] NVARCHAR(50),
    [MinimumInvestmentAmount] DECIMAL(18,2),
    [RiskProfile] NVARCHAR(50), -- Low, Medium, High

    -- Management
    [BrokerId] INT,
    [Notes] NVARCHAR(MAX),
    [LastActivityDate] DATETIME2,

    -- Audit Trail
    [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedDate] DATETIME2,

    FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE SET NULL,
    FOREIGN KEY ([BrokerId]) REFERENCES [Brokers]([BrokerId]) ON DELETE SET NULL,
    FOREIGN KEY ([ApprovedBy]) REFERENCES [Users]([UserId]) ON DELETE SET NULL
);

-- Create indexes for Investors
CREATE INDEX [IX_Investors_Email] ON [Investors]([Email]);
CREATE INDEX [IX_Investors_Status] ON [Investors]([Status]);
CREATE INDEX [IX_Investors_VerificationStatus] ON [Investors]([VerificationStatus]);
CREATE INDEX [IX_Investors_BrokerId] ON [Investors]([BrokerId]);
CREATE INDEX [IX_Investors_InvestorType] ON [Investors]([InvestorType]);

-- =====================================================
-- 17. MANAGERS TABLE
-- =====================================================
CREATE TABLE [Managers] (
    [ManagerId] INT PRIMARY KEY IDENTITY(1,1),
    [UserId] INT NOT NULL,
    [BrokerId] INT NOT NULL,

    -- Manager Information
    [ManagerName] NVARCHAR(MAX),
    [Email] NVARCHAR(255),
    [Phone] NVARCHAR(20),
    [Department] NVARCHAR(50), -- Sales, HR, Finance, Operations, Admin
    [JobTitle] NVARCHAR(100),

    -- Reporting Structure
    [ReportsTo] INT,
    [ManagerType] NVARCHAR(50), -- Regional, Branch, Department, Team Lead

    -- Responsibility Area
    [AreaOfResponsibility] NVARCHAR(MAX),
    [NumberOfTeamMembers] INT DEFAULT 0,

    -- Approval Authority
    [CanApproveCommissions] BIT DEFAULT 0,
    [CanApproveLeavals] BIT DEFAULT 0,
    [CanApproveExpenses] BIT DEFAULT 0,
    [CanApprovePayroll] BIT DEFAULT 0,
    [CanApproveInvoices] BIT DEFAULT 0,
    [ExpenseApprovalLimit] DECIMAL(18,2),

    -- Performance Metrics
    [TargetSales] DECIMAL(18,2),
    [ActualSales] DECIMAL(18,2),
    [TargetCommission] DECIMAL(18,2),
    [ActualCommission] DECIMAL(18,2),

    -- Status & Dates
    [Status] NVARCHAR(50) DEFAULT 'Active', -- Active, Inactive, On Leave, Suspended
    [EmploymentType] NVARCHAR(50), -- Full-Time, Part-Time, Contract
    [JoinDate] DATETIME2,
    [EndDate] DATETIME2,

    -- Metadata
    [Notes] NVARCHAR(MAX),

    -- Audit Trail
    [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedDate] DATETIME2,

    FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE,
    FOREIGN KEY ([BrokerId]) REFERENCES [Brokers]([BrokerId]) ON DELETE CASCADE,
    FOREIGN KEY ([ReportsTo]) REFERENCES [Managers]([ManagerId]) ON DELETE SET NULL
);

-- Create indexes for Managers
CREATE INDEX [IX_Managers_UserId] ON [Managers]([UserId]);
CREATE INDEX [IX_Managers_BrokerId] ON [Managers]([BrokerId]);
CREATE INDEX [IX_Managers_Department] ON [Managers]([Department]);
CREATE INDEX [IX_Managers_Status] ON [Managers]([Status]);
CREATE INDEX [IX_Managers_ReportsTo] ON [Managers]([ReportsTo]);
CREATE INDEX [IX_Managers_Email] ON [Managers]([Email]);

-- =====================================================
-- 18. ACCOUNTING TABLE
-- =====================================================
CREATE TABLE [Accounting] (
    [AccountingId] INT PRIMARY KEY IDENTITY(1,1),

    -- Transaction Details
    [TransactionDate] DATETIME2 NOT NULL,
    [GLAccountNumber] NVARCHAR(50),
    [Description] NVARCHAR(MAX) NOT NULL,

    -- Transaction Type
    [TransactionType] NVARCHAR(50), -- Income, Expense, Asset, Liability, Equity
    [Category] NVARCHAR(50), -- Commission, Salary, Tax, Rent, Utilities, Other

    -- Amounts
    [DebitAmount] DECIMAL(18,2),
    [CreditAmount] DECIMAL(18,2),
    [Amount] DECIMAL(18,2),

    -- Related Records (Foreign Keys)
    [InvoiceId] INT,
    [CommissionId] INT,
    [PayrollId] INT,
    [PaymentTransactionId] INT,
    [UserId] INT,
    [BrokerId] INT,

    -- Status & Reconciliation
    [Status] NVARCHAR(50) DEFAULT 'Draft', -- Draft, Posted, Reconciled, Reversed
    [IsReconciled] BIT DEFAULT 0,
    [ReconciliationDate] DATETIME2,

    -- Reference Information
    [Department] NVARCHAR(50),
    [Project] NVARCHAR(100),
    [Reference] NVARCHAR(100),

    -- Approvals
    [CreatedBy] INT,
    [ApprovedBy] INT,
    [ApprovedDate] DATETIME2,

    -- Metadata
    [Notes] NVARCHAR(MAX),
    [Attachment] NVARCHAR(MAX),

    -- Audit Trail
    [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedDate] DATETIME2,

    FOREIGN KEY ([InvoiceId]) REFERENCES [Invoices]([InvoiceId]) ON DELETE SET NULL,
    FOREIGN KEY ([CommissionId]) REFERENCES [Commissions]([CommissionId]) ON DELETE SET NULL,
    FOREIGN KEY ([PayrollId]) REFERENCES [Payroll]([PayrollId]) ON DELETE SET NULL,
    FOREIGN KEY ([PaymentTransactionId]) REFERENCES [PaymentTransactions]([Id]) ON DELETE SET NULL,
    FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE SET NULL,
    FOREIGN KEY ([BrokerId]) REFERENCES [Brokers]([BrokerId]) ON DELETE SET NULL,
    FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]) ON DELETE SET NULL,
    FOREIGN KEY ([ApprovedBy]) REFERENCES [Users]([UserId]) ON DELETE SET NULL
);

-- Create indexes for Accounting
CREATE INDEX [IX_Accounting_TransactionDate] ON [Accounting]([TransactionDate]);
CREATE INDEX [IX_Accounting_GLAccountNumber] ON [Accounting]([GLAccountNumber]);
CREATE INDEX [IX_Accounting_Status] ON [Accounting]([Status]);
CREATE INDEX [IX_Accounting_Category] ON [Accounting]([Category]);
CREATE INDEX [IX_Accounting_BrokerId] ON [Accounting]([BrokerId]);
CREATE INDEX [IX_Accounting_IsReconciled] ON [Accounting]([IsReconciled]);
CREATE INDEX [IX_Accounting_InvoiceId] ON [Accounting]([InvoiceId]);
CREATE INDEX [IX_Accounting_CommissionId] ON [Accounting]([CommissionId]);
CREATE INDEX [IX_Accounting_PayrollId] ON [Accounting]([PayrollId]);

-- =====================================================
-- 19. AUDIT LOGS TABLE
-- =====================================================
CREATE TABLE [AuditLogs] (
    [AuditLogId] INT PRIMARY KEY IDENTITY(1,1),
    [UserId] INT NOT NULL,
    [TableName] NVARCHAR(100) NOT NULL,
    [RecordId] INT NOT NULL,
    [ActionType] NVARCHAR(50) NOT NULL, -- Create, Read, Update, Delete, Export

    -- Change Details
    [FieldName] NVARCHAR(100),
    [OldValue] NVARCHAR(MAX),
    [NewValue] NVARCHAR(MAX),

    -- Context Information
    [BrokerId] INT,
    [IpAddress] NVARCHAR(50),
    [UserAgent] NVARCHAR(MAX),
    [SessionId] NVARCHAR(100),

    -- Business Context
    [Module] NVARCHAR(50), -- Customers, Invoices, Payroll, Accounting, etc.
    [Description] NVARCHAR(MAX),
    [Reason] NVARCHAR(200),

    -- Approval Tracking
    [RequiresApproval] BIT DEFAULT 0,
    [ApprovedBy] INT,
    [ApprovedDate] DATETIME2,
    [ApprovalStatus] NVARCHAR(50) DEFAULT 'Pending', -- Pending, Approved, Rejected, Cancelled
    [ApprovalNotes] NVARCHAR(MAX),

    -- Sensitive Data Handling
    [IsSensitiveData] BIT DEFAULT 0,
    [IsEncrypted] BIT DEFAULT 0,
    [EncryptionMethod] NVARCHAR(50),

    -- Status & Metadata
    [Status] NVARCHAR(50) DEFAULT 'Active', -- Active, Archived, Deleted
    [Severity] NVARCHAR(50) DEFAULT 'Info', -- Info, Warning, Critical, Compliance
    [Tags] NVARCHAR(MAX),

    -- Audit Trail
    [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),

    FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE,
    FOREIGN KEY ([BrokerId]) REFERENCES [Brokers]([BrokerId]) ON DELETE SET NULL,
    FOREIGN KEY ([ApprovedBy]) REFERENCES [Users]([UserId]) ON DELETE SET NULL
);

-- Create indexes for AuditLogs
CREATE INDEX [IX_AuditLogs_UserId] ON [AuditLogs]([UserId]);
CREATE INDEX [IX_AuditLogs_TableName] ON [AuditLogs]([TableName]);
CREATE INDEX [IX_AuditLogs_RecordId] ON [AuditLogs]([RecordId]);
CREATE INDEX [IX_AuditLogs_ActionType] ON [AuditLogs]([ActionType]);
CREATE INDEX [IX_AuditLogs_CreatedDate] ON [AuditLogs]([CreatedDate]);
CREATE INDEX [IX_AuditLogs_BrokerId] ON [AuditLogs]([BrokerId]);
CREATE INDEX [IX_AuditLogs_Severity] ON [AuditLogs]([Severity]);
CREATE INDEX [IX_AuditLogs_Module] ON [AuditLogs]([Module]);

-- =====================================================
-- SUMMARY OF TABLES
-- =====================================================
-- Total Tables: 19
--
-- CORE AUTHENTICATION & USERS:
-- 1. Roles - User roles (Admin, Broker, Agent, Investor)
-- 2. Users - User accounts
--
-- CUSTOMER MANAGEMENT:
-- 3. Customers - Customers with extended payment info
-- 4. Clients - Client information
-- 5. PaymentTransactions - Payment records via PayMongo
--
-- APPOINTMENTS & SCHEDULING:
-- 6. Appointments - Scheduled appointments
-- 7. ViewingAppointments - Property viewing appointments
-- 8. Schedules - Calendar schedules
--
-- PROPERTY MANAGEMENT:
-- 9. Properties - Property listings
-- 10. Brokers - Broker information
--
-- TRANSACTION TRACKING:
-- 11. Transactions - Property transactions
--
-- BUSINESS OPERATIONS:
-- 12. Commissions - Broker commission tracking
-- 13. Invoices - Invoice management
-- 14. Payroll - Employee payroll
--
-- INVESTMENT & MANAGEMENT:
-- 15. Investors - Investor management
-- 16. Managers - Manager/Supervisor information
--
-- FINANCIAL & ACCOUNTING:
-- 17. Accounting - General ledger and accounting entries
--
-- COMPLIANCE & AUDIT (NEW):
-- 18. AuditLogs - Complete audit trail of all changes
--
-- SECURITY:
-- 19. OtpVerifications - OTP records
--
-- =====================================================
-- KEY FEATURES:
-- =====================================================
-- ✅ Foreign Key Constraints for data integrity
-- ✅ Cascade Delete for related records
-- ✅ Indexes on frequently queried columns
-- ✅ DateTime2 for precise timestamps (UTC)
-- ✅ DECIMAL(18,2) for financial values
-- ✅ Default values for status fields
-- ✅ Comprehensive audit trail (CreatedDate, UpdatedDate)
--
-- =====================================================
-- IMPORTANT NOTES:
-- =====================================================
-- 1. Run these CREATE statements in SQL Server Management Studio
-- 2. Ensure they run in this order (respect foreign key relationships)
-- 3. After creating tables, run Entity Framework migrations
-- 4. Test on a development database first
-- 5. Back up existing data before applying to production
--
-- =====================================================
