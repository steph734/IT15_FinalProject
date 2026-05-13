# EstateFlow Database - Manager, Payroll & Audit Logs Tables

## Manager Table

| Field Names | Datatype | Length | Description |
|---|---|---|---|
| ManagerId-PK | Int-AI | 9 | Manager's ID Number |
| UserId-FK-Required | Int | 9 | Foreign Key to Users Table |
| BrokerId-FK-Required | Int | 9 | Foreign Key to Brokers Table |
| ManagerName | NVarChar | Max | Manager's Full Name |
| Email | NVarChar | 255 | Manager's Email Address |
| Phone | NVarChar | 20 | Manager's Phone Number |
| Department | NVarChar | 50 | Department (Sales/HR/Finance/Operations/Admin) |
| JobTitle | NVarChar | 100 | Job Title/Position |
| ReportsTo | Int | 9 | Manager ID of Direct Supervisor (if applicable) |
| ManagerType | NVarChar | 50 | Type (Regional/Branch/Department/Team Lead) |
| AreaOfResponsibility | NVarChar | Max | Area/Region/Department Managed |
| NumberOfTeamMembers | Int | 9 | Number of Staff Managed |
| CanApproveCommissions | Bit | 1 | Authority to Approve Commissions (1=Yes, 0=No) |
| CanApproveLeavals | Bit | 1 | Authority to Approve Leaves (1=Yes, 0=No) |
| CanApproveExpenses | Bit | 1 | Authority to Approve Expenses (1=Yes, 0=No) |
| CanApprovePayroll | Bit | 1 | Authority to Approve Payroll (1=Yes, 0=No) |
| CanApproveInvoices | Bit | 1 | Authority to Approve Invoices (1=Yes, 0=No) |
| ExpenseApprovalLimit | Decimal | 18,2 | Maximum Expense Approval Amount (PHP) |
| TargetSales | Decimal | 18,2 | Sales Target for Managed Team (PHP) |
| ActualSales | Decimal | 18,2 | Actual Sales Achieved (PHP) |
| TargetCommission | Decimal | 18,2 | Commission Target (PHP) |
| ActualCommission | Decimal | 18,2 | Actual Commission Earned (PHP) |
| Status | NVarChar | 50 | Status (Active/Inactive/On Leave/Suspended) |
| EmploymentType | NVarChar | 50 | Type (Full-Time/Part-Time/Contract) |
| JoinDate | DateTime2 | - | Date Manager Joined |
| EndDate | DateTime2 | - | Date Manager Left (if applicable) |
| Notes | NVarChar | Max | Additional Notes |
| CreatedDate | DateTime2 | - | Record Creation Date (Auto) |
| UpdatedDate | DateTime2 | - | Record Last Updated Date |

---

## Payroll Table

| Field Names | Datatype | Length | Description |
|---|---|---|---|
| PayrollId-PK | Int-AI | 9 | Payroll Record's ID Number |
| UserId-FK-Required | Int | 9 | Foreign Key to Users Table (Employee) |
| BrokerId-FK-Required | Int | 9 | Foreign Key to Brokers Table (Employer) |
| PayrollMonth-Required | Date | - | Month of Payroll (First day of month) |
| PayPeriodStart-Required | Date | - | Payroll Period Start Date |
| PayPeriodEnd-Required | Date | - | Payroll Period End Date |
| PaymentDate-Required | DateTime2 | - | Payment Disbursement Date |
| BaseSalary | Decimal | 18,2 | Base Salary (PHP) |
| CommissionEarned | Decimal | 18,2 | Commission Earned (PHP) |
| Bonuses | Decimal | 18,2 | Performance Bonuses (PHP) |
| OtherAllowances | Decimal | 18,2 | Other Allowances/Benefits (PHP) |
| GrossSalary-Required | Decimal | 18,2 | Gross Salary (PHP) |
| IncomeTax | Decimal | 18,2 | Income Tax Deduction (PHP) |
| SSS | Decimal | 18,2 | SSS Contribution (PHP) |
| PhilHealth | Decimal | 18,2 | PhilHealth Contribution (PHP) |
| PagIbig | Decimal | 18,2 | PAG-IBIG Contribution (PHP) |
| HealthInsurance | Decimal | 18,2 | Health Insurance Deduction (PHP) |
| Loans | Decimal | 18,2 | Loan Repayment Deduction (PHP) |
| OtherDeductions | Decimal | 18,2 | Other Deductions (PHP) |
| TotalDeductions-Required | Decimal | 18,2 | Total Deductions (PHP) |
| NetSalary-Required | Decimal | 18,2 | Net Salary After Deductions (PHP) |
| Status | NVarChar | 50 | Status (Draft/Pending/Approved/Paid/Cancelled) |
| ApprovedBy-FK | Int | 9 | User ID Who Approved (Manager/HR) |
| ApprovedDate | DateTime2 | - | Date Payroll Approved |
| PaymentStatus | NVarChar | 50 | Payment Status (Unpaid/Paid) |
| PaymentMethod | NVarChar | 50 | Payment Method (Bank Transfer/Cash/Check) |
| BankAccountNumber | NVarChar | 50 | Employee Bank Account (if Bank Transfer) |
| TransactionReferenceNumber | NVarChar | 100 | Bank Transaction Reference Number |
| Notes | NVarChar | Max | Payroll Notes/Comments |
| AttendanceRecordId | Int | 9 | Reference to Attendance Records (Optional) |
| CreatedDate | DateTime2 | - | Record Creation Date (Auto) |
| UpdatedDate | DateTime2 | - | Record Last Updated Date |

---

## Audit Logs Table

| Field Names | Datatype | Length | Description |
|---|---|---|---|
| AuditLogId-PK | Int-AI | 9 | Audit Log Entry ID Number |
| UserId-FK-Required | Int | 9 | Foreign Key to Users Table (User who made change) |
| TableName-Required | NVarChar | 100 | Name of Table Affected (e.g., Customers, Invoices) |
| RecordId-Required | Int | 9 | ID of Record that was changed |
| ActionType-Required | NVarChar | 50 | Type of Action (Create/Read/Update/Delete/Export) |
| **CHANGE DETAILS** | | | |
| FieldName | NVarChar | 100 | Name of Field Changed (if applicable) |
| OldValue | NVarChar | Max | Previous/Old Value Before Change |
| NewValue | NVarChar | Max | New Value After Change |
| **CONTEXT INFORMATION** | | | |
| BrokerId-FK | Int | 9 | Associated Broker ID |
| IpAddress | NVarChar | 50 | IP Address of User Making Change |
| UserAgent | NVarChar | Max | Browser/Device Information |
| SessionId | NVarChar | 100 | Session ID Token |
| **BUSINESS CONTEXT** | | | |
| Module | NVarChar | 50 | Module/Feature (Customers/Invoices/Payroll/Accounting) |
| Description | NVarChar | Max | Detailed Description of Change |
| Reason | NVarChar | 200 | Reason for Change (Optional) |
| **APPROVAL TRACKING** | | | |
| RequiresApproval | Bit | 1 | Whether change requires approval (1=Yes, 0=No) |
| ApprovedBy-FK | Int | 9 | User ID Who Approved the Change (if applicable) |
| ApprovedDate | DateTime2 | - | Date Change was Approved |
| ApprovalStatus | NVarChar | 50 | Approval Status (Pending/Approved/Rejected/Cancelled) |
| ApprovalNotes | NVarChar | Max | Notes from Approver |
| **SENSITIVE DATA HANDLING** | | | |
| IsSensitiveData | Bit | 1 | Whether record contains sensitive data (1=Yes, 0=No) |
| IsEncrypted | Bit | 1 | Whether data is encrypted (1=Yes, 0=No) |
| EncryptionMethod | NVarChar | 50 | Encryption method used (e.g., AES-256) |
| **STATUS & METADATA** | | | |
| Status | NVarChar | 50 | Status (Active/Archived/Deleted) |
| Severity | NVarChar | 50 | Severity Level (Info/Warning/Critical/Compliance) |
| Tags | NVarChar | Max | Tags for categorization (comma-separated) |
| CreatedDate | DateTime2 | - | Timestamp of Change (Auto - UTC) |

---

## Managers Table (Summary Format)

```
Managers Table

Field Names              | Datatype  | Length | Description
ManagerId-PK             | Int-AI    | 9      | Manager's ID Number
UserId-FK-Required       | Int       | 9      | Foreign Key to Users Table
BrokerId-FK-Required     | Int       | 9      | Foreign Key to Brokers Table
ManagerName              | NVarChar  | Max    | Manager's Full Name
Email                    | NVarChar  | 255    | Manager's Email Address
Phone                    | NVarChar  | 20     | Manager's Phone Number
Department               | NVarChar  | 50     | Sales/HR/Finance/Operations/Admin
JobTitle                 | NVarChar  | 100    | Job Title/Position
ReportsTo                | Int       | 9      | Direct Supervisor Manager ID
ManagerType              | NVarChar  | 50     | Regional/Branch/Department/Team Lead
AreaOfResponsibility     | NVarChar  | Max    | Area/Region/Department Managed
NumberOfTeamMembers      | Int       | 9      | Number of Staff Managed
CanApproveCommissions    | Bit       | 1      | Authority to Approve Commissions
CanApproveLeavals        | Bit       | 1      | Authority to Approve Leaves
CanApproveExpenses       | Bit       | 1      | Authority to Approve Expenses
CanApprovePayroll        | Bit       | 1      | Authority to Approve Payroll
CanApproveInvoices       | Bit       | 1      | Authority to Approve Invoices
ExpenseApprovalLimit     | Decimal   | 18,2   | Maximum Expense Approval Amount (PHP)
TargetSales              | Decimal   | 18,2   | Sales Target for Team (PHP)
ActualSales              | Decimal   | 18,2   | Actual Sales Achieved (PHP)
TargetCommission         | Decimal   | 18,2   | Commission Target (PHP)
ActualCommission         | Decimal   | 18,2   | Actual Commission Earned (PHP)
Status                   | NVarChar  | 50     | Active/Inactive/On Leave/Suspended
EmploymentType           | NVarChar  | 50     | Full-Time/Part-Time/Contract
JoinDate                 | DateTime2 | -      | Date Manager Joined
EndDate                  | DateTime2 | -      | Date Manager Left (if applicable)
Notes                    | NVarChar  | Max    | Additional Notes
CreatedDate              | DateTime2 | -      | Record Creation Date (Auto)
UpdatedDate              | DateTime2 | -      | Record Last Updated Date
```

---

## Payroll Table (Summary Format)

```
Payroll Table

Field Names                    | Datatype  | Length | Description
PayrollId-PK                   | Int-AI    | 9      | Payroll Record's ID Number
UserId-FK-Required             | Int       | 9      | Foreign Key to Users Table (Employee)
BrokerId-FK-Required           | Int       | 9      | Foreign Key to Brokers Table (Employer)
PayrollMonth-Required          | Date      | -      | Month of Payroll (First day of month)
PayPeriodStart-Required        | Date      | -      | Payroll Period Start Date
PayPeriodEnd-Required          | Date      | -      | Payroll Period End Date
PaymentDate-Required           | DateTime2 | -      | Payment Disbursement Date
BaseSalary                     | Decimal   | 18,2   | Base Salary (PHP)
CommissionEarned               | Decimal   | 18,2   | Commission Earned (PHP)
Bonuses                        | Decimal   | 18,2   | Performance Bonuses (PHP)
OtherAllowances                | Decimal   | 18,2   | Other Allowances/Benefits (PHP)
GrossSalary-Required           | Decimal   | 18,2   | Gross Salary (PHP)
IncomeTax                      | Decimal   | 18,2   | Income Tax Deduction (PHP)
SSS                            | Decimal   | 18,2   | SSS Contribution (PHP)
PhilHealth                     | Decimal   | 18,2   | PhilHealth Contribution (PHP)
PagIbig                        | Decimal   | 18,2   | PAG-IBIG Contribution (PHP)
HealthInsurance                | Decimal   | 18,2   | Health Insurance Deduction (PHP)
Loans                          | Decimal   | 18,2   | Loan Repayment Deduction (PHP)
OtherDeductions                | Decimal   | 18,2   | Other Deductions (PHP)
TotalDeductions-Required       | Decimal   | 18,2   | Total Deductions (PHP)
NetSalary-Required             | Decimal   | 18,2   | Net Salary After Deductions (PHP)
Status                         | NVarChar  | 50     | Draft/Pending/Approved/Paid/Cancelled
ApprovedBy-FK                  | Int       | 9      | User ID Who Approved (Manager/HR)
ApprovedDate                   | DateTime2 | -      | Date Payroll Approved
PaymentStatus                  | NVarChar  | 50     | Unpaid/Paid
PaymentMethod                  | NVarChar  | 50     | Bank Transfer/Cash/Check
BankAccountNumber              | NVarChar  | 50     | Employee Bank Account
TransactionReferenceNumber     | NVarChar  | 100    | Bank Transaction Reference Number
Notes                          | NVarChar  | Max    | Payroll Notes/Comments
AttendanceRecordId             | Int       | 9      | Reference to Attendance Records
CreatedDate                    | DateTime2 | -      | Record Creation Date (Auto)
UpdatedDate                    | DateTime2 | -      | Record Last Updated Date
```

---

## Audit Logs Table (Summary Format)

```
AuditLogs Table

Field Names                    | Datatype  | Length | Description
AuditLogId-PK                  | Int-AI    | 9      | Audit Log Entry ID Number
UserId-FK-Required             | Int       | 9      | User who made change (FK to Users)
TableName-Required             | NVarChar  | 100    | Name of Table Affected
RecordId-Required              | Int       | 9      | ID of Record that was changed
ActionType-Required            | NVarChar  | 50     | Create/Read/Update/Delete/Export
FieldName                      | NVarChar  | 100    | Name of Field Changed
OldValue                       | NVarChar  | Max    | Previous/Old Value Before Change
NewValue                       | NVarChar  | Max    | New Value After Change
BrokerId-FK                    | Int       | 9      | Associated Broker ID
IpAddress                      | NVarChar  | 50     | IP Address of User Making Change
UserAgent                      | NVarChar  | Max    | Browser/Device Information
SessionId                      | NVarChar  | 100    | Session ID Token
Module                         | NVarChar  | 50     | Customers/Invoices/Payroll/Accounting
Description                    | NVarChar  | Max    | Detailed Description of Change
Reason                         | NVarChar  | 200    | Reason for Change (Optional)
RequiresApproval               | Bit       | 1      | Whether change requires approval
ApprovedBy-FK                  | Int       | 9      | User ID Who Approved the Change
ApprovedDate                   | DateTime2 | -      | Date Change was Approved
ApprovalStatus                 | NVarChar  | 50     | Pending/Approved/Rejected/Cancelled
ApprovalNotes                  | NVarChar  | Max    | Notes from Approver
IsSensitiveData                | Bit       | 1      | Whether record contains sensitive data
IsEncrypted                    | Bit       | 1      | Whether data is encrypted
EncryptionMethod               | NVarChar  | 50     | Encryption method used (e.g., AES-256)
Status                         | NVarChar  | 50     | Active/Archived/Deleted
Severity                       | NVarChar  | 50     | Info/Warning/Critical/Compliance
Tags                           | NVarChar  | Max    | Tags for categorization
CreatedDate                    | DateTime2 | -      | Timestamp of Change (Auto - UTC)
```

---

## Key Relationships

### Managers Table Relationships
```
Users (1) ──→ (M) Managers (UserId-FK)
Brokers (1) ──→ (M) Managers (BrokerId-FK)
Managers (1) ──→ (M) Managers (ReportsTo - Self-referencing)
Managers (1) ──→ (M) Payroll (ApprovedBy FK)
Managers (1) ──→ (M) Commissions (ApprovedBy FK)
Managers (1) ──→ (M) Invoices (CreatedBy/ApprovedBy FK)
```

### Payroll Table Relationships
```
Users (1) ──→ (M) Payroll (UserId-FK)
Brokers (1) ──→ (M) Payroll (BrokerId-FK)
Managers (1) ──→ (M) Payroll (ApprovedBy-FK)
Payroll (1) ──→ (M) Accounting (PayrollId-FK)
Payroll (1) ──→ (M) AuditLogs (RecordId reference)
```

### Audit Logs Table Relationships
```
Users (1) ──→ (M) AuditLogs (UserId-FK)
Brokers (1) ──→ (M) AuditLogs (BrokerId-FK)
Managers (1) ──→ (M) AuditLogs (ApprovedBy-FK)
All Tables ──→ (M) AuditLogs (via TableName + RecordId)
```

---

## Business Logic

### Manager Hierarchy
```
Level 1: Super Admin (System Level)
├─ Level 2: Regional Manager
│  ├─ Level 3: Branch Manager
│  │  ├─ Level 4: Department Manager
│  │  │  └─ Level 5: Team Lead
│  │  └─ Individual Agents/Staff
│  └─ Other Regional Managers
```

### Payroll Calculations
```
GrossSalary = BaseSalary + CommissionEarned + Bonuses + OtherAllowances
TotalDeductions = IncomeTax + SSS + PhilHealth + PagIbig + HealthInsurance + Loans + OtherDeductions
NetSalary = GrossSalary - TotalDeductions
```

### Audit Log Tracking
```
Actions Tracked:
- Create: New record created
- Read: Record accessed/viewed
- Update: Record fields changed
- Delete: Record deleted
- Export: Data exported from system

Severity Levels:
- Info: Normal operations
- Warning: Significant changes
- Critical: High-impact changes
- Compliance: Regulatory/compliance events
```

---

## SQL Constraints & Rules

### Managers Table
```sql
CHECK (Status IN ('Active', 'Inactive', 'On Leave', 'Suspended'))
CHECK (ManagerType IN ('Regional', 'Branch', 'Department', 'Team Lead'))
CHECK (EmploymentType IN ('Full-Time', 'Part-Time', 'Contract'))
CHECK (Department IN ('Sales', 'HR', 'Finance', 'Operations', 'Admin'))
CHECK (NumberOfTeamMembers >= 0)
CHECK (ReportsTo IS NULL OR ReportsTo <> ManagerId)
```

### Payroll Table
```sql
CHECK (Status IN ('Draft', 'Pending', 'Approved', 'Paid', 'Cancelled'))
CHECK (PaymentStatus IN ('Unpaid', 'Paid'))
CHECK (BaseSalary >= 0)
CHECK (GrossSalary >= 0)
CHECK (TotalDeductions >= 0)
CHECK (NetSalary = (GrossSalary - TotalDeductions))
```

### Audit Logs Table
```sql
CHECK (ActionType IN ('Create', 'Read', 'Update', 'Delete', 'Export'))
CHECK (ApprovalStatus IN ('Pending', 'Approved', 'Rejected', 'Cancelled'))
CHECK (Severity IN ('Info', 'Warning', 'Critical', 'Compliance'))
```

---

## Example Data

### Managers Example
```
ManagerId | UserId | BrokerId | ManagerName   | Department | JobTitle         | Status | CanApprovePayroll
1         | 5      | 1        | Juan Santos   | Sales      | Regional Manager | Active | 1
2         | 6      | 1        | Maria Cruz    | Sales      | Branch Manager   | Active | 1
3         | 7      | 1        | Pedro Lopez   | Finance    | Finance Manager  | Active | 0
```

### Payroll Example
```
PayrollId | UserId | BrokerId | PayrollMonth | BaseSalary | CommissionEarned | GrossSalary | TotalDeductions | NetSalary | Status
1         | 8      | 1        | 2026-01-01   | 30000      | 50000            | 80000       | 12000           | 68000     | Paid
2         | 9      | 1        | 2026-01-01   | 25000      | 30000            | 55000       | 8250            | 46750     | Approved
```

### Audit Logs Example
```
AuditLogId | UserId | TableName  | RecordId | ActionType | FieldName  | OldValue | NewValue | Severity
1          | 5      | Customers  | 100      | Update     | Status     | Active   | Inactive | Info
2          | 6      | Payroll    | 50       | Create     | NULL       | NULL     | NULL     | Info
3          | 7      | Invoices   | 25       | Update     | TotalAmount| 50000    | 55000    | Warning
```

---

## Indexes Required

### Managers Table Indexes
```
CREATE INDEX [IX_Managers_UserId] ON [Managers]([UserId])
CREATE INDEX [IX_Managers_BrokerId] ON [Managers]([BrokerId])
CREATE INDEX [IX_Managers_Department] ON [Managers]([Department])
CREATE INDEX [IX_Managers_Status] ON [Managers]([Status])
CREATE INDEX [IX_Managers_ReportsTo] ON [Managers]([ReportsTo])
```

### Payroll Table Indexes
```
CREATE INDEX [IX_Payroll_UserId] ON [Payroll]([UserId])
CREATE INDEX [IX_Payroll_BrokerId] ON [Payroll]([BrokerId])
CREATE INDEX [IX_Payroll_PayrollMonth] ON [Payroll]([PayrollMonth])
CREATE INDEX [IX_Payroll_Status] ON [Payroll]([Status])
CREATE INDEX [IX_Payroll_PaymentStatus] ON [Payroll]([PaymentStatus])
```

### Audit Logs Table Indexes
```
CREATE INDEX [IX_AuditLogs_UserId] ON [AuditLogs]([UserId])
CREATE INDEX [IX_AuditLogs_TableName] ON [AuditLogs]([TableName])
CREATE INDEX [IX_AuditLogs_ActionType] ON [AuditLogs]([ActionType])
CREATE INDEX [IX_AuditLogs_CreatedDate] ON [AuditLogs]([CreatedDate])
CREATE INDEX [IX_AuditLogs_BrokerId] ON [AuditLogs]([BrokerId])
CREATE INDEX [IX_AuditLogs_Severity] ON [AuditLogs]([Severity])
```

---

**Status:** Production Ready  
**Total Tables:** 3 (Manager, Payroll, Audit Logs)  
**Total Fields:** 90+ fields across all tables  
**Foreign Keys:** 15+  
**Relationships:** Complex multi-table relationships  

