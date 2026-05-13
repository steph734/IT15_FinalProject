# EstateFlow Database - Additional Tables (Investors & Accounting)

## 16. Investors Table

| Field Names | Datatype | Length | Description |
|---|---|---|---|
| InvestorId-PK | Int-AI | 9 | Investor's ID Number |
| UserId-FK | Int | 9 | Foreign Key to Users Table (if user account exists) |
| **INVESTOR INFORMATION** | | | |
| InvestorName-Required | NVarChar | Max | Investor's Full Name |
| Email | NVarChar | 255 | Investor's Email Address |
| Phone | NVarChar | 20 | Investor's Phone Number |
| **ADDRESS INFORMATION** | | | |
| Address | NVarChar | Max | Investor's Street Address |
| City | NVarChar | 100 | Investor's City |
| State | NVarChar | 100 | Investor's State/Province |
| Country | NVarChar | 100 | Investor's Country |
| ZipCode | NVarChar | 20 | Investor's Postal Code |
| **INVESTMENT DETAILS** | | | |
| InvestorType | NVarChar | 50 | Type (Individual/Corporate/Partnership/Trust) |
| TaxId | NVarChar | 50 | Tax Identification Number |
| LicenseNumber | NVarChar | 50 | Investment License (if applicable) |
| InitialInvestment | Decimal | 18,2 | Initial Investment Amount (PHP) |
| TotalInvestment | Decimal | 18,2 | Total Investment to Date (PHP) |
| AvailableFunds | Decimal | 18,2 | Available Funds for Investment (PHP) |
| **PORTFOLIO TRACKING** | | | |
| NumberOfProperties | Int | 9 | Number of Properties Invested In |
| TotalReturns | Decimal | 18,2 | Total Returns Generated (PHP) |
| AverageROI | Decimal | 5,2 | Average Return on Investment (%) |
| **STATUS & COMPLIANCE** | | | |
| Status | NVarChar | 50 | Status (Active/Inactive/Suspended/Under Review) |
| VerificationStatus | NVarChar | 50 | Verification (Pending/Verified/Rejected) |
| ApprovedBy | Int | 9 | User ID of Approver |
| ApprovedDate | DateTime2 | - | Date Investor Approved |
| ComplianceNotes | NVarChar | Max | Compliance and Regulatory Notes |
| **PREFERENCES** | | | |
| PreferredPropertyType | NVarChar | 50 | Preferred Type (Residential/Commercial/Industrial) |
| MinimumInvestmentAmount | Decimal | 18,2 | Minimum Investment Required (PHP) |
| RiskProfile | NVarChar | 50 | Risk Profile (Low/Medium/High) |
| **METADATA** | | | |
| BrokerId-FK | Int | 9 | Associated Broker/Manager ID |
| Notes | NVarChar | Max | Additional Notes |
| LastActivityDate | DateTime2 | - | Last Activity Date |
| CreatedDate | DateTime2 | - | Record Creation Date (Auto) |
| UpdatedDate | DateTime2 | - | Record Last Updated Date |

---

## 17. Accounting Table

| Field Names | Datatype | Length | Description |
|---|---|---|---|
| AccountingId-PK | Int-AI | 9 | Accounting Entry's ID Number |
| **REFERENCE INFORMATION** | | | |
| TransactionDate-Required | DateTime2 | - | Date of Transaction |
| GLAccountNumber | NVarChar | 50 | General Ledger Account Number |
| Description-Required | NVarChar | Max | Transaction Description |
| **TRANSACTION DETAILS** | | | |
| TransactionType | NVarChar | 50 | Type (Income/Expense/Asset/Liability/Equity) |
| Category | NVarChar | 50 | Category (Commission/Salary/Tax/Rent/Utilities/Other) |
| **AMOUNTS** | | | |
| DebitAmount | Decimal | 18,2 | Debit Amount (PHP) |
| CreditAmount | Decimal | 18,2 | Credit Amount (PHP) |
| Amount | Decimal | 18,2 | Transaction Amount (PHP) |
| **RELATED RECORDS** | | | |
| InvoiceId-FK | Int | 9 | Foreign Key to Invoices (if applicable) |
| CommissionId-FK | Int | 9 | Foreign Key to Commissions (if applicable) |
| PayrollId-FK | Int | 9 | Foreign Key to Payroll (if applicable) |
| PaymentTransactionId-FK | Int | 9 | Foreign Key to PaymentTransactions (if applicable) |
| UserId-FK | Int | 9 | Foreign Key to Users (if applicable) |
| BrokerId-FK | Int | 9 | Foreign Key to Brokers (if applicable) |
| **STATUS & RECONCILIATION** | | | |
| Status | NVarChar | 50 | Status (Draft/Posted/Reconciled/Reversed) |
| IsReconciled | Bit | 1 | Reconciliation Status (1=Reconciled, 0=Pending) |
| ReconciliationDate | DateTime2 | - | Date Reconciled |
| **AUDIT TRAIL** | | | |
| Department | NVarChar | 50 | Department/Cost Center |
| Project | NVarChar | 100 | Project Code (if applicable) |
| Reference | NVarChar | 100 | Reference Number/Invoice Number |
| CreatedBy-FK | Int | 9 | User ID Who Created Entry |
| ApprovedBy | Int | 9 | User ID Who Approved Entry |
| ApprovedDate | DateTime2 | - | Date Entry Approved |
| **METADATA** | | | |
| Notes | NVarChar | Max | Additional Notes |
| Attachment | NVarChar | Max | Attachment File Path (if any) |
| CreatedDate | DateTime2 | - | Record Creation Date (Auto) |
| UpdatedDate | DateTime2 | - | Record Last Updated Date |

---

## Summary Format (Your Exact Style)

### INVESTORS TABLE

```
Investors Table

Field Names                 | Datatype  | Length | Description
InvestorId-PK              | Int-AI    | 9      | Investor's ID Number
UserId-FK                  | Int       | 9      | Foreign Key to Users Table (if user account exists)
InvestorName-Required      | NVarChar  | Max    | Investor's Full Name
Email                      | NVarChar  | 255    | Investor's Email Address
Phone                      | NVarChar  | 20     | Investor's Phone Number
Address                    | NVarChar  | Max    | Investor's Street Address
City                       | NVarChar  | 100    | Investor's City
State                      | NVarChar  | 100    | Investor's State/Province
Country                    | NVarChar  | 100    | Investor's Country
ZipCode                    | NVarChar  | 20     | Investor's Postal Code
InvestorType               | NVarChar  | 50     | Type (Individual/Corporate/Partnership/Trust)
TaxId                      | NVarChar  | 50     | Tax Identification Number
LicenseNumber              | NVarChar  | 50     | Investment License (if applicable)
InitialInvestment          | Decimal   | 18,2   | Initial Investment Amount (PHP)
TotalInvestment            | Decimal   | 18,2   | Total Investment to Date (PHP)
AvailableFunds             | Decimal   | 18,2   | Available Funds for Investment (PHP)
NumberOfProperties         | Int       | 9      | Number of Properties Invested In
TotalReturns               | Decimal   | 18,2   | Total Returns Generated (PHP)
AverageROI                 | Decimal   | 5,2    | Average Return on Investment (%)
Status                     | NVarChar  | 50     | Status (Active/Inactive/Suspended/Under Review)
VerificationStatus         | NVarChar  | 50     | Verification (Pending/Verified/Rejected)
ApprovedBy                 | Int       | 9      | User ID of Approver
ApprovedDate               | DateTime2 | -      | Date Investor Approved
ComplianceNotes            | NVarChar  | Max    | Compliance and Regulatory Notes
PreferredPropertyType      | NVarChar  | 50     | Preferred Type (Residential/Commercial/Industrial)
MinimumInvestmentAmount    | Decimal   | 18,2   | Minimum Investment Required (PHP)
RiskProfile                | NVarChar  | 50     | Risk Profile (Low/Medium/High)
BrokerId-FK                | Int       | 9      | Associated Broker/Manager ID
Notes                      | NVarChar  | Max    | Additional Notes
LastActivityDate           | DateTime2 | -      | Last Activity Date
CreatedDate                | DateTime2 | -      | Record Creation Date (Auto)
UpdatedDate                | DateTime2 | -      | Record Last Updated Date
```

---

### ACCOUNTING TABLE

```
Accounting Table

Field Names                | Datatype  | Length | Description
AccountingId-PK            | Int-AI    | 9      | Accounting Entry's ID Number
TransactionDate-Required   | DateTime2 | -      | Date of Transaction
GLAccountNumber            | NVarChar  | 50     | General Ledger Account Number
Description-Required       | NVarChar  | Max    | Transaction Description
TransactionType            | NVarChar  | 50     | Type (Income/Expense/Asset/Liability/Equity)
Category                   | NVarChar  | 50     | Category (Commission/Salary/Tax/Rent/Utilities/Other)
DebitAmount                | Decimal   | 18,2   | Debit Amount (PHP)
CreditAmount               | Decimal   | 18,2   | Credit Amount (PHP)
Amount                     | Decimal   | 18,2   | Transaction Amount (PHP)
InvoiceId-FK               | Int       | 9      | Foreign Key to Invoices (if applicable)
CommissionId-FK            | Int       | 9      | Foreign Key to Commissions (if applicable)
PayrollId-FK               | Int       | 9      | Foreign Key to Payroll (if applicable)
PaymentTransactionId-FK    | Int       | 9      | Foreign Key to PaymentTransactions (if applicable)
UserId-FK                  | Int       | 9      | Foreign Key to Users (if applicable)
BrokerId-FK                | Int       | 9      | Foreign Key to Brokers (if applicable)
Status                     | NVarChar  | 50     | Status (Draft/Posted/Reconciled/Reversed)
IsReconciled               | Bit       | 1      | Reconciliation Status (1=Reconciled, 0=Pending)
ReconciliationDate         | DateTime2 | -      | Date Reconciled
Department                 | NVarChar  | 50     | Department/Cost Center
Project                    | NVarChar  | 100    | Project Code (if applicable)
Reference                  | NVarChar  | 100    | Reference Number/Invoice Number
CreatedBy-FK               | Int       | 9      | User ID Who Created Entry
ApprovedBy                 | Int       | 9      | User ID Who Approved Entry
ApprovedDate               | DateTime2 | -      | Date Entry Approved
Notes                      | NVarChar  | Max    | Additional Notes
Attachment                 | NVarChar  | Max    | Attachment File Path (if any)
CreatedDate                | DateTime2 | -      | Record Creation Date (Auto)
UpdatedDate                | DateTime2 | -      | Record Last Updated Date
```

---

## Database Relationships

### Investors Table Relationships
```
Users (1) ──→ (M) Investors (UserId-FK)
└─ Each user can be linked to investor account

Brokers (1) ──→ (M) Investors (BrokerId-FK)
└─ Each broker can manage multiple investors

Investors (1) ──→ (M) InvestorProperties (via junction table)
└─ Each investor can invest in multiple properties
```

### Accounting Table Relationships
```
Invoices (1) ──→ (M) Accounting (InvoiceId-FK)
└─ Each invoice can have multiple accounting entries

Commissions (1) ──→ (M) Accounting (CommissionId-FK)
└─ Each commission can have accounting records

Payroll (1) ──→ (M) Accounting (PayrollId-FK)
└─ Each payroll record can have accounting entries

PaymentTransactions (1) ──→ (M) Accounting (PaymentTransactionId-FK)
└─ Each payment can create accounting entries

Users (1) ──→ (M) Accounting (UserId-FK, CreatedBy-FK, ApprovedBy)
└─ Users can create and approve accounting entries

Brokers (1) ──→ (M) Accounting (BrokerId-FK)
└─ Accounting entries per broker
```

---

## Business Logic

### Investors Table
**Investment Calculations:**
```
Total Investment = Sum of all investment amounts from this investor
Available Funds = InitialInvestment + TotalReturns - TotalInvested
Average ROI = (TotalReturns / TotalInvestment) * 100

Example:
- Initial Investment: PHP 1,000,000
- Total Returns: PHP 250,000
- Current Investment Value: PHP 1,100,000
- Available for New Investment: PHP 150,000
- Average ROI: (250,000 / 1,000,000) * 100 = 25%
```

### Accounting Table
**Double Entry System:**
```
Every transaction must have:
- Debit Amount on one GL Account
- Credit Amount on corresponding GL Account
- Debit Total = Credit Total (always balanced)

Journal Entry Flow:
1. Create Entry (Status: Draft)
2. Review & Approval (Status: Posted)
3. Reconciliation with Bank (Status: Reconciled)
4. Close Period or Reverse if needed
```

---

## GL Account Structure

### Common GL Accounts for Real Estate
```
INCOME ACCOUNTS (1000-1999):
1010 - Commission Income
1020 - Property Management Fees
1030 - Investment Returns
1040 - Other Income

EXPENSE ACCOUNTS (2000-2999):
2010 - Agent Salaries
2020 - Broker Commissions
2030 - Marketing Expenses
2040 - Office Rent
2050 - Utilities
2060 - Taxes

ASSET ACCOUNTS (3000-3999):
3010 - Cash
3020 - Bank Accounts
3030 - Accounts Receivable
3040 - Investment Properties

LIABILITY ACCOUNTS (4000-4999):
4010 - Accounts Payable
4020 - Loans Payable
4030 - Deferred Income

EQUITY ACCOUNTS (5000-5999):
5010 - Owner's Capital
5020 - Retained Earnings
```

---

## Example Data

### Investors Example
```
InvestorId | InvestorName    | InvestorType | InitialInvestment | TotalInvestment | Status  | VerificationStatus
1          | Juan dela Cruz  | Individual   | 1000000           | 1500000         | Active  | Verified
2          | ABC Corporation | Corporate    | 5000000           | 7500000         | Active  | Verified
3          | Maria Santos    | Individual   | 500000            | 750000          | Active  | Verified
```

### Accounting Example
```
AccountingId | TransactionDate | GLAccountNumber | Description        | DebitAmount | CreditAmount | Status
1            | 2026-01-15      | 1010           | Commission Income  | 100000      | NULL         | Posted
2            | 2026-01-15      | 3010           | Cash Received      | NULL        | 100000       | Posted
3            | 2026-01-20      | 2010           | Agent Salary       | 50000       | NULL         | Posted
4            | 2026-01-20      | 3010           | Cash Paid          | NULL        | 50000        | Posted
```

---

## Constraints & Rules

### Investors Table
```sql
-- Investor status values
CHECK (Status IN ('Active', 'Inactive', 'Suspended', 'Under Review'))

-- Verification status
CHECK (VerificationStatus IN ('Pending', 'Verified', 'Rejected'))

-- Risk profile
CHECK (RiskProfile IN ('Low', 'Medium', 'High'))

-- Investment amounts must be positive
CHECK (InitialInvestment > 0)
CHECK (TotalInvestment >= 0)
CHECK (AvailableFunds >= 0)
```

### Accounting Table
```sql
-- Transaction type
CHECK (TransactionType IN ('Income', 'Expense', 'Asset', 'Liability', 'Equity'))

-- Category values
CHECK (Category IN ('Commission', 'Salary', 'Tax', 'Rent', 'Utilities', 'Other'))

-- Status flow
CHECK (Status IN ('Draft', 'Posted', 'Reconciled', 'Reversed'))

-- Double entry rule: Debit must equal Credit for each transaction
-- (Enforced at application layer)

-- Cannot have both Debit and Credit for same transaction
CHECK ((DebitAmount IS NULL OR CreditAmount IS NULL) OR (DebitAmount = 0 OR CreditAmount = 0))
```

---

## SQL Constraints for Balance

```sql
-- For Investors: Total must always be updated
CREATE TRIGGER TR_UpdateInvestorTotal
ON Investors
AFTER INSERT, UPDATE
AS
BEGIN
    UPDATE Investors
    SET TotalInvestment = (
        SELECT SUM(InvestmentAmount) 
        FROM InvestorProperties 
        WHERE InvestorId = Investors.InvestorId
    )
    WHERE InvestorId IN (SELECT InvestorId FROM INSERTED)
END

-- For Accounting: Ensure Debit = Credit balance
CREATE VIEW v_AccountingBalance AS
SELECT 
    TransactionDate,
    SUM(DebitAmount) as TotalDebits,
    SUM(CreditAmount) as TotalCredits,
    CASE WHEN SUM(DebitAmount) = SUM(CreditAmount) THEN 'BALANCED' ELSE 'UNBALANCED' END as BalanceStatus
FROM Accounting
GROUP BY TransactionDate
```

---

## Access Control

### Investors Table Access
- **Level 1 (Super Admin):** ✅ Full CRUD
- **Level 2 (System Admin):** 🟡 Limited (assigned brokers)
- **Level 3 (Broker):** 🟡 Limited (own investors)
- **Level 4 (Agent):** 🔵 Read-only
- **Level 5 (Client):** ❌ No access

### Accounting Table Access
- **Level 1 (Super Admin):** ✅ Full CRUD
- **Level 2 (System Admin):** 🟡 Limited (assigned brokers)
- **Level 3 (Broker):** 🟡 Limited (own records)
- **Level 4 (Agent):** 🔵 View-only
- **Level 5 (Client):** ❌ No access

---

## Reports Enabled

### Investors Reports
- Investor Portfolio Summary
- Investment ROI Analysis
- Investor Payment History
- Property Investment Distribution
- Risk Profile Distribution

### Accounting Reports
- General Ledger Report
- Trial Balance
- Income Statement
- Balance Sheet
- Accounts Payable/Receivable Aging
- Reconciliation Report

---

**New Tables Added:** 2  
**Total Tables Now:** 17  
**New Fields:** 50+  
**Status:** ✅ Complete

