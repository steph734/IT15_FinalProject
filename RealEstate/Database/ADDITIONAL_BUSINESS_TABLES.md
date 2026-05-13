# EstateFlow Database - Additional Business Tables

## 3 Missing Critical Tables

### **TABLE 13: Commissions Table**

**Purpose:** Track broker commissions from transactions

| Field Names | Datatype | Length | Description |
|---|---|---|---|
| CommissionId-PK | Int-AI | 9 | Commission's ID Number |
| TransactionId-FK-Required | Int | 9 | Foreign Key to Transactions Table |
| BrokerId-FK-Required | Int | 9 | Foreign Key to Brokers Table |
| AgentId-FK | Int | 9 | Foreign Key to Users Table (Agent) |
| SaleAmount | Decimal | 18,2 | Original Sale Amount (PHP) |
| CommissionRate | Decimal | 5,2 | Commission Rate (Percentage) |
| CommissionAmount | Decimal | 18,2 | Calculated Commission Amount (PHP) |
| **STATUS FIELDS** | | | |
| Status | NVarChar | 50 | Commission Status (Pending/Approved/Paid/Rejected) |
| ApprovedDate | DateTime2 | - | Date Commission Approved |
| ApprovedBy | Int | 9 | User ID Who Approved |
| PaymentDate | DateTime2 | - | Date Commission Paid |
| **METADATA** | | | |
| Notes | NVarChar | Max | Commission Notes/Remarks |
| CreatedDate | DateTime2 | - | Record Creation Date |
| UpdatedDate | DateTime2 | - | Record Last Updated Date |

**Indexes:** TransactionId, BrokerId, AgentId, Status, PaymentDate

**Relationships:**
- FK to Transactions (TransactionId)
- FK to Brokers (BrokerId)
- FK to Users (AgentId)

---

### **TABLE 14: Invoices Table**

**Purpose:** Track all invoices (Customer invoices, Broker invoices, Service invoices)

| Field Names | Datatype | Length | Description |
|---|---|---|---|
| InvoiceId-PK | Int-AI | 9 | Invoice's ID Number |
| InvoiceNumber-U | NVarChar | 50 | Unique Invoice Number (INV-2026-001) |
| **INVOICE DETAILS** | | | |
| InvoiceType | NVarChar | 50 | Invoice Type (Service/Property/Commission/Rental) |
| IssuedDate-Required | DateTime2 | - | Invoice Issue Date |
| DueDate | DateTime2 | - | Invoice Due Date |
| **RELATED ENTITIES** | | | |
| BrokerId-FK | Int | 9 | Foreign Key to Brokers (Issuer) |
| ClientId-FK | Int | 9 | Foreign Key to Clients Table (Recipient) |
| PropertyId-FK | Int | 9 | Foreign Key to Properties Table (If applicable) |
| TransactionId-FK | Int | 9 | Foreign Key to Transactions (If applicable) |
| **AMOUNTS** | | | |
| SubTotal | Decimal | 18,2 | Subtotal Amount (PHP) |
| TaxAmount | Decimal | 18,2 | Tax/VAT Amount (PHP) |
| DiscountAmount | Decimal | 18,2 | Discount Amount (PHP) |
| TotalAmount-Required | Decimal | 18,2 | Total Invoice Amount (PHP) |
| **PAYMENT TRACKING** | | | |
| Status | NVarChar | 50 | Invoice Status (Draft/Sent/Viewed/Paid/Overdue/Cancelled) |
| PaymentStatus | NVarChar | 50 | Payment Status (Unpaid/Partially Paid/Paid) |
| AmountPaid | Decimal | 18,2 | Amount Already Paid (PHP) |
| OutstandingAmount | Decimal | 18,2 | Remaining Balance (PHP) |
| **PAYMENT DETAILS** | | | |
| PaymentMethod | NVarChar | 50 | Payment Method (Cash/Check/Bank Transfer/PayMongo/Credit Card) |
| PaymentDate | DateTime2 | - | Date Payment Received |
| PaymentReferenceNumber | NVarChar | 100 | Payment Reference/Check Number |
| **METADATA** | | | |
| Description | NVarChar | Max | Invoice Description/Items |
| Notes | NVarChar | Max | Additional Notes |
| CreatedBy | Int | 9 | User ID Who Created Invoice |
| SentDate | DateTime2 | - | Date Invoice Sent to Client |
| CreatedDate | DateTime2 | - | Record Creation Date |
| UpdatedDate | DateTime2 | - | Record Last Updated Date |

**Indexes:** InvoiceNumber, BrokerId, ClientId, Status, PaymentStatus, IssuedDate, DueDate

**Relationships:**
- FK to Brokers (BrokerId)
- FK to Clients (ClientId)
- FK to Properties (PropertyId)
- FK to Transactions (TransactionId)

---

### **TABLE 15: Payroll Table**

**Purpose:** Track employee payroll, salaries, and deductions

| Field Names | Datatype | Length | Description |
|---|---|---|---|
| PayrollId-PK | Int-AI | 9 | Payroll Record's ID Number |
| UserId-FK-Required | Int | 9 | Foreign Key to Users Table (Employee) |
| BrokerId-FK-Required | Int | 9 | Foreign Key to Brokers Table (Employer) |
| **PAYROLL PERIOD** | | | |
| PayrollMonth | Date | - | Month of Payroll (First day of month) |
| PayPeriodStart | Date | - | Payroll Period Start Date |
| PayPeriodEnd | Date | - | Payroll Period End Date |
| PaymentDate-Required | DateTime2 | - | Payment Disbursement Date |
| **SALARY DETAILS** | | | |
| BaseSalary | Decimal | 18,2 | Base Salary (PHP) |
| CommissionEarned | Decimal | 18,2 | Commission Earned (PHP) |
| Bonuses | Decimal | 18,2 | Performance Bonuses (PHP) |
| OtherAllowances | Decimal | 18,2 | Other Allowances/Benefits (PHP) |
| GrossSalary | Decimal | 18,2 | Gross Salary (PHP) |
| **DEDUCTIONS** | | | |
| IncomeTax | Decimal | 18,2 | Income Tax Deduction (PHP) |
| SSS | Decimal | 18,2 | SSS Contribution (PHP) |
| PhilHealth | Decimal | 18,2 | PhilHealth Contribution (PHP) |
| PagIbig | Decimal | 18,2 | PAG-IBIG Contribution (PHP) |
| HealthInsurance | Decimal | 18,2 | Health Insurance Deduction (PHP) |
| Loans | Decimal | 18,2 | Loan Repayment Deduction (PHP) |
| OtherDeductions | Decimal | 18,2 | Other Deductions (PHP) |
| TotalDeductions | Decimal | 18,2 | Total Deductions (PHP) |
| **NET PAY** | | | |
| NetSalary | Decimal | 18,2 | Net Salary After Deductions (PHP) |
| **STATUS & PAYMENT** | | | |
| Status | NVarChar | 50 | Payroll Status (Draft/Pending/Approved/Paid/Cancelled) |
| ApprovedBy | Int | 9 | User ID Who Approved (Manager/HR) |
| ApprovedDate | DateTime2 | - | Date Payroll Approved |
| PaymentStatus | NVarChar | 50 | Payment Status (Unpaid/Paid) |
| PaymentMethod | NVarChar | 50 | Payment Method (Bank Transfer/Cash/Check) |
| BankAccountNumber | NVarChar | 50 | Employee Bank Account (if Bank Transfer) |
| **METADATA** | | | |
| Notes | NVarChar | Max | Payroll Notes/Comments |
| AttendanceRecordId | Int | 9 | Reference to Attendance Records (Optional) |
| CreatedDate | DateTime2 | - | Record Creation Date |
| UpdatedDate | DateTime2 | - | Record Last Updated Date |

**Indexes:** UserId, BrokerId, PayrollMonth, Status, PaymentStatus, PaymentDate

**Relationships:**
- FK to Users (UserId)
- FK to Brokers (BrokerId)

---

## Updated Database Statistics

| Metric | Updated Value |
|--------|----------------|
| **Total Tables** | 15 (was 12) |
| **Total Fields** | 200+ (was 150+) |
| **Primary Keys** | 15 |
| **Foreign Keys** | 11 (was 8) |
| **Indexes** | 35+ (was 25+) |

---

## Complete Table List (15 Tables)

### **TIER 1: AUTHENTICATION (2 tables)**
1. Roles
2. Users

### **TIER 2: CUSTOMER MANAGEMENT (3 tables)**
3. Customers
4. PaymentTransactions
5. Clients

### **TIER 3: APPOINTMENTS & SCHEDULING (3 tables)**
6. Appointments
7. ViewingAppointments
8. Schedules

### **TIER 4: PROPERTIES & TRANSACTIONS (3 tables)**
9. Properties
10. Brokers
11. Transactions

### **TIER 5: BUSINESS OPERATIONS (3 tables)** ✅ NEW
12. **Commissions** (NEW)
13. **Invoices** (NEW)
14. **Payroll** (NEW)

### **TIER 6: SECURITY (1 table)**
15. OtpVerifications

---

## Multi-Tenant Hierarchy Access for New Tables

### **Commission Table Access**

| Level | Role | Create | Read | Update | Delete | Notes |
|-------|------|--------|------|--------|--------|-------|
| **1** | Super Admin | ✅ Full | ✅ All | ✅ Full | ✅ Full | System-wide commission management |
| **2** | System Admin | ❌ | ✅ Assigned | ✅ Limited | ❌ | View assigned broker commissions |
| **3** | Broker | ❌ | ✅ Own Only | ❌ | ❌ | Read-only: View own commissions |
| **4** | Agent | ❌ | ✅ Own Only | ❌ | ❌ | View own earned commissions |
| **5** | Client | ❌ | ❌ | ❌ | ❌ | No access |

**Tenant Isolation:** By BrokerId

---

### **Invoice Table Access**

| Level | Role | Create | Read | Update | Delete | Notes |
|-------|------|--------|------|--------|--------|-------|
| **1** | Super Admin | ✅ Full | ✅ All | ✅ Full | ✅ Full | System-wide invoice management |
| **2** | System Admin | ✅ Assigned | ✅ Assigned | ✅ Limited | ❌ | Create invoices for assigned brokers |
| **3** | Broker | ✅ Own | ✅ Own | ✅ Own | ✅ Soft Delete | Full control of own invoices |
| **4** | Agent | ✅ Limited | ✅ Own | ✅ Limited | ❌ | Create/View assigned invoices |
| **5** | Client | ❌ | ✅ Own Only | ❌ | ❌ | View own invoices received |

**Tenant Isolation:** By BrokerId

---

### **Payroll Table Access**

| Level | Role | Create | Read | Update | Delete | Notes |
|-------|------|--------|------|--------|--------|-------|
| **1** | Super Admin | ✅ Full | ✅ All | ✅ Full | ✅ Full | System-wide payroll management |
| **2** | System Admin | ✅ Assigned | ✅ Assigned | ✅ Full | ❌ | Manage assigned broker payroll |
| **3** | Broker | ✅ Own Staff | ✅ Own | ✅ Own | ❌ | Can create own payroll |
| **4** | Agent | ❌ | ✅ Own Only | ❌ | ❌ | View own payroll records |
| **5** | Client | ❌ | ❌ | ❌ | ❌ | No access |

**Tenant Isolation:** By BrokerId (UserId)

---

## Business Logic & Formulas

### **Commissions Calculation**
```
CommissionAmount = (SaleAmount * CommissionRate) / 100

Example:
- Sale Amount: PHP 5,000,000
- Commission Rate: 3.5%
- Commission Amount = (5,000,000 * 3.5) / 100 = PHP 175,000
```

### **Invoice Calculations**
```
SubTotal = Sum of all line items
TaxAmount = SubTotal * TaxRate (12% for VAT)
DiscountAmount = User-defined discount
TotalAmount = SubTotal + TaxAmount - DiscountAmount
OutstandingAmount = TotalAmount - AmountPaid
```

### **Payroll Calculations**
```
GrossSalary = BaseSalary + CommissionEarned + Bonuses + OtherAllowances
TotalDeductions = IncomeTax + SSS + PhilHealth + PagIbig + HealthInsurance + Loans + OtherDeductions
NetSalary = GrossSalary - TotalDeductions
```

---

## Sample Data Scenarios

### **Commission Example**
```
Transaction: Property sold for PHP 10,000,000
Broker Commission Rate: 2.5%
Agent Commission Rate: 1.5%

Commission Records Created:
1. Broker Commission: (10,000,000 * 2.5%) = PHP 250,000
2. Agent Commission: (10,000,000 * 1.5%) = PHP 150,000

Status Flow: Pending → Approved → Paid
```

### **Invoice Example**
```
Property Sale Invoice:
- Property: BGC Condo Unit
- Sale Amount: PHP 5,000,000
- Commission (2%): PHP 100,000
- VAT (12%): PHP 72,000
- Total: PHP 5,172,000

Payment Status: Pending → Partially Paid (PHP 2,586,000) → Paid
```

### **Payroll Example**
```
Employee: John Doe (Real Estate Agent)
Payroll Period: January 2026

Income:
- Base Salary: PHP 30,000
- Commissions: PHP 150,000
- Bonus: PHP 10,000
- Gross: PHP 190,000

Deductions:
- Income Tax: PHP 18,500
- SSS: PHP 1,650
- PhilHealth: PHP 1,625
- PAG-IBIG: PHP 100
- Total Deductions: PHP 21,875

Net Salary: PHP 168,125
```

---

## Implementation Order

### **Phase 1: Database Creation**
1. Create base tables (Roles, Users, Customers, etc.)
2. Create Commissions table
3. Create Invoices table
4. Create Payroll table

### **Phase 2: Business Logic**
1. Implement commission calculations
2. Implement invoice management
3. Implement payroll processing

### **Phase 3: Reporting**
1. Commission reports (by broker, by agent, by period)
2. Invoice reports (aged receivables, payment tracking)
3. Payroll reports (salary summary, deductions summary)

### **Phase 4: Integration**
1. Link commissions to transactions
2. Link invoices to transactions/commissions
3. Link payroll to employee records

---

## SQL Constraints & Rules

### **Commission Table**
```sql
-- Commission status flow: Pending → Approved → Paid
ALTER TABLE Commissions
ADD CONSTRAINT CK_Commission_Status
CHECK (Status IN ('Pending', 'Approved', 'Paid', 'Rejected'));

-- Commission amount must be positive
ALTER TABLE Commissions
ADD CONSTRAINT CK_Commission_Amount
CHECK (CommissionAmount >= 0);

-- Commission rate between 0-100%
ALTER TABLE Commissions
ADD CONSTRAINT CK_Commission_Rate
CHECK (CommissionRate BETWEEN 0 AND 100);
```

### **Invoice Table**
```sql
-- Invoice status progression
ALTER TABLE Invoices
ADD CONSTRAINT CK_Invoice_Status
CHECK (Status IN ('Draft', 'Sent', 'Viewed', 'Paid', 'Overdue', 'Cancelled'));

-- Payment status tracking
ALTER TABLE Invoices
ADD CONSTRAINT CK_Payment_Status
CHECK (PaymentStatus IN ('Unpaid', 'Partially Paid', 'Paid'));

-- Outstanding amount cannot exceed total
ALTER TABLE Invoices
ADD CONSTRAINT CK_Outstanding_Amount
CHECK (OutstandingAmount >= 0 AND OutstandingAmount <= TotalAmount);
```

### **Payroll Table**
```sql
-- Payroll status progression
ALTER TABLE Payroll
ADD CONSTRAINT CK_Payroll_Status
CHECK (Status IN ('Draft', 'Pending', 'Approved', 'Paid', 'Cancelled'));

-- Net salary must be calculated correctly
ALTER TABLE Payroll
ADD CONSTRAINT CK_Net_Salary
CHECK (NetSalary = (GrossSalary - TotalDeductions));

-- All amounts must be positive
ALTER TABLE Payroll
ADD CONSTRAINT CK_Payroll_Amounts
CHECK (GrossSalary >= 0 AND TotalDeductions >= 0 AND NetSalary >= 0);
```

---

## Updated SQL Schema

The updated schema includes:
- 15 tables (up from 12)
- 200+ fields (up from 150+)
- 11 foreign keys (up from 8)
- 35+ indexes (up from 25+)

---

## Excel Integration

These tables support exporting to Excel for:
- **Commission Report:** Commission details by broker/agent/period
- **Invoice Report:** Outstanding invoices, payment status
- **Payroll Report:** Salary summary, deduction breakdown

---

**New Tables Added:** 3  
**Total Tables Now:** 15  
**New Fields:** 50+  
**Status:** ✅ Complete with business operations

