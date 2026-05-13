# EstateFlow Database - Detailed Table Specifications

## Commissions Table

| Field Names | Datatype | Length | Description |
|---|---|---|---|
| CommissionId-PK | Int-AI | 9 | Commission's ID Number |
| TransactionId-FK | Int | 9 | Foreign Key to Transactions Table |
| BrokerId-FK | Int | 9 | Foreign Key to Brokers Table |
| AgentId-FK | Int | 9 | Foreign Key to Users Table (Agent) |
| SaleAmount | Decimal | 18,2 | Original Sale Amount in PHP |
| CommissionRate | Decimal | 5,2 | Commission Rate Percentage (0-100%) |
| CommissionAmount | Decimal | 18,2 | Calculated Commission Amount in PHP |
| Status | NVarChar | 50 | Commission Status (Pending/Approved/Paid/Rejected) |
| ApprovedDate | DateTime2 | - | Date Commission Was Approved |
| ApprovedBy-FK | Int | 9 | User ID of Approver (Manager/Admin) |
| PaymentDate | DateTime2 | - | Date Commission Payment Was Made |
| Notes | NVarChar | Max | Additional Comments or Remarks |
| CreatedDate | DateTime2 | - | Record Creation Date (Auto) |
| UpdatedDate | DateTime2 | - | Record Last Updated Date |

---

## Invoices Table

| Field Names | Datatype | Length | Description |
|---|---|---|---|
| InvoiceId-PK | Int-AI | 9 | Invoice's ID Number |
| InvoiceNumber-U | NVarChar | 50 | Unique Invoice Number (INV-2026-001) |
| InvoiceType | NVarChar | 50 | Type of Invoice (Service/Property/Commission/Rental) |
| IssuedDate | DateTime2 | - | Date Invoice Was Issued |
| DueDate | DateTime2 | - | Payment Due Date |
| BrokerId-FK | Int | 9 | Foreign Key to Brokers Table (Issuer) |
| ClientId-FK | Int | 9 | Foreign Key to Clients Table (Recipient) |
| PropertyId-FK | Int | 9 | Foreign Key to Properties Table (If Applicable) |
| TransactionId-FK | Int | 9 | Foreign Key to Transactions Table (If Applicable) |
| SubTotal | Decimal | 18,2 | Subtotal Amount Before Tax & Discount (PHP) |
| TaxAmount | Decimal | 18,2 | Tax/VAT Amount (12% of SubTotal in PHP) |
| DiscountAmount | Decimal | 18,2 | Discount Amount Applied (PHP) |
| TotalAmount | Decimal | 18,2 | Total Invoice Amount (SubTotal + Tax - Discount) |
| Status | NVarChar | 50 | Invoice Status (Draft/Sent/Viewed/Paid/Overdue/Cancelled) |
| PaymentStatus | NVarChar | 50 | Payment Status (Unpaid/Partially Paid/Paid) |
| AmountPaid | Decimal | 18,2 | Amount Already Paid by Client (PHP) |
| OutstandingAmount | Decimal | 18,2 | Remaining Balance to be Paid (PHP) |
| PaymentMethod | NVarChar | 50 | Method of Payment (Cash/Check/Bank Transfer/PayMongo/Credit Card) |
| PaymentDate | DateTime2 | - | Date Payment Was Received |
| PaymentReferenceNumber | NVarChar | 100 | Payment Reference or Check Number |
| Description | NVarChar | Max | Invoice Description and Line Items |
| Notes | NVarChar | Max | Additional Invoice Notes or Comments |
| CreatedBy-FK | Int | 9 | User ID Who Created the Invoice |
| SentDate | DateTime2 | - | Date Invoice Was Sent to Client |
| CreatedDate | DateTime2 | - | Record Creation Date (Auto) |
| UpdatedDate | DateTime2 | - | Record Last Updated Date |

---

## Brokers Table (Branch)

| Field Names | Datatype | Length | Description |
|---|---|---|---|
| BrokerId-PK | Int-AI | 9 | Broker/Branch ID Number |
| UserId-FK | Int | 9 | Foreign Key to Users Table (Broker Manager) |
| CompanyName | NVarChar | Max | Broker Company Name |
| LicenseNumber | NVarChar | 50 | Broker License Number for Compliance |
| **CONTACT INFORMATION** | | | |
| Phone | NVarChar | 20 | Broker's Contact Phone Number |
| Email | NVarChar | 255 | Broker's Email Address |
| **ADDRESS INFORMATION** | | | |
| Address | NVarChar | Max | Broker's Office Street Address |
| City | NVarChar | 100 | Broker's Office City |
| State | NVarChar | 100 | Broker's Office State/Province |
| Country | NVarChar | 100 | Broker's Office Country |
| ZipCode | NVarChar | 20 | Broker's Office Postal Code |
| **BUSINESS DETAILS** | | | |
| CommissionRate | Decimal | 5,2 | Default Commission Rate Percentage (e.g., 2.5%) |
| IsActive | Bit | 1 | Broker Status (1=Active, 0=Inactive) |
| **AUDIT TRAIL** | | | |
| CreatedDate | DateTime2 | - | Record Creation Date (Auto) |
| UpdatedDate | DateTime2 | - | Record Last Updated Date |

---

## Summary Format Comparison

### Original Format (Users Table - Your Example)
```
Users Table
Field Names        | Datatype  | Length | Description
UserID-PK          | Int-AI    | 9      | User's ID Number
RoleID-FK          | Int       | 9      | Foreign Key to Roles Table
Fullname           | NVarChar  | 100    | User's Full Name
Email              | NVarChar  | 255    | User's Email Address (Unique)
Password           | NVarChar  | 30     | User's Password
ConfirmPassword    | Text      | 20     | Confirm Password
```

### Same Format Applied to New Tables

---

## Complete Table Specifications

### COMMISSIONS TABLE

```
Commissions Table

Field Names        | Datatype  | Length | Description
CommissionId-PK    | Int-AI    | 9      | Commission's ID Number
TransactionId-FK   | Int       | 9      | Foreign Key to Transactions Table
BrokerId-FK        | Int       | 9      | Foreign Key to Brokers Table
AgentId-FK         | Int       | 9      | Foreign Key to Users Table (Agent)
SaleAmount         | Decimal   | 18,2   | Original Sale Amount in PHP
CommissionRate     | Decimal   | 5,2    | Commission Rate Percentage (0-100%)
CommissionAmount   | Decimal   | 18,2   | Calculated Commission Amount in PHP
Status             | NVarChar  | 50     | Commission Status (Pending/Approved/Paid/Rejected)
ApprovedDate       | DateTime2 | -      | Date Commission Was Approved
ApprovedBy-FK      | Int       | 9      | User ID of Approver (Manager/Admin)
PaymentDate        | DateTime2 | -      | Date Commission Payment Was Made
Notes              | NVarChar  | Max    | Additional Comments or Remarks
CreatedDate        | DateTime2 | -      | Record Creation Date (Auto)
UpdatedDate        | DateTime2 | -      | Record Last Updated Date
```

---

### INVOICES TABLE

```
Invoices Table

Field Names                | Datatype  | Length | Description
InvoiceId-PK               | Int-AI    | 9      | Invoice's ID Number
InvoiceNumber-U            | NVarChar  | 50     | Unique Invoice Number (INV-2026-001)
InvoiceType                | NVarChar  | 50     | Type of Invoice (Service/Property/Commission/Rental)
IssuedDate                 | DateTime2 | -      | Date Invoice Was Issued
DueDate                    | DateTime2 | -      | Payment Due Date
BrokerId-FK                | Int       | 9      | Foreign Key to Brokers Table (Issuer)
ClientId-FK                | Int       | 9      | Foreign Key to Clients Table (Recipient)
PropertyId-FK              | Int       | 9      | Foreign Key to Properties Table (If Applicable)
TransactionId-FK           | Int       | 9      | Foreign Key to Transactions Table (If Applicable)
SubTotal                   | Decimal   | 18,2   | Subtotal Amount Before Tax & Discount (PHP)
TaxAmount                  | Decimal   | 18,2   | Tax/VAT Amount (12% of SubTotal in PHP)
DiscountAmount             | Decimal   | 18,2   | Discount Amount Applied (PHP)
TotalAmount                | Decimal   | 18,2   | Total Invoice Amount (SubTotal + Tax - Discount)
Status                     | NVarChar  | 50     | Invoice Status (Draft/Sent/Viewed/Paid/Overdue/Cancelled)
PaymentStatus              | NVarChar  | 50     | Payment Status (Unpaid/Partially Paid/Paid)
AmountPaid                 | Decimal   | 18,2   | Amount Already Paid by Client (PHP)
OutstandingAmount          | Decimal   | 18,2   | Remaining Balance to be Paid (PHP)
PaymentMethod              | NVarChar  | 50     | Method of Payment (Cash/Check/Bank Transfer/PayMongo/Credit Card)
PaymentDate                | DateTime2 | -      | Date Payment Was Received
PaymentReferenceNumber     | NVarChar  | 100    | Payment Reference or Check Number
Description                | NVarChar  | Max    | Invoice Description and Line Items
Notes                      | NVarChar  | Max    | Additional Invoice Notes or Comments
CreatedBy-FK               | Int       | 9      | User ID Who Created the Invoice
SentDate                   | DateTime2 | -      | Date Invoice Was Sent to Client
CreatedDate                | DateTime2 | -      | Record Creation Date (Auto)
UpdatedDate                | DateTime2 | -      | Record Last Updated Date
```

---

### BROKERS TABLE (BRANCH)

```
Brokers Table

Field Names        | Datatype  | Length | Description
BrokerId-PK        | Int-AI    | 9      | Broker/Branch ID Number
UserId-FK          | Int       | 9      | Foreign Key to Users Table (Broker Manager)
CompanyName        | NVarChar  | Max    | Broker Company Name
LicenseNumber      | NVarChar  | 50     | Broker License Number for Compliance
Phone              | NVarChar  | 20     | Broker's Contact Phone Number
Email              | NVarChar  | 255    | Broker's Email Address
Address            | NVarChar  | Max    | Broker's Office Street Address
City               | NVarChar  | 100    | Broker's Office City
State              | NVarChar  | 100    | Broker's Office State/Province
Country            | NVarChar  | 100    | Broker's Office Country
ZipCode            | NVarChar  | 20     | Broker's Office Postal Code
CommissionRate     | Decimal   | 5,2    | Default Commission Rate Percentage (e.g., 2.5%)
IsActive           | Bit       | 1      | Broker Status (1=Active, 0=Inactive)
CreatedDate        | DateTime2 | -      | Record Creation Date (Auto)
UpdatedDate        | DateTime2 | -      | Record Last Updated Date
```

---

## Additional Notes

### Field Name Notation
- **-PK** = Primary Key (Unique Identifier)
- **-FK** = Foreign Key (References Another Table)
- **-U** = Unique Constraint (No Duplicate Values)
- **-AI** = Auto Increment (Automatically Generated)

### Data Type Details
- **Int-AI** = Auto-Incrementing Integer (1, 2, 3, ...)
- **NVarChar(n)** = Variable Unicode Text (n = max characters)
- **NVarChar(Max)** = Unlimited Unicode Text (up to 8000+ characters)
- **Decimal(p,s)** = Decimal Number (p=total digits, s=decimal places)
  - Example: Decimal(18,2) = 999,999,999,999,999.99
- **DateTime2** = Date and Time (e.g., 2026-01-15 14:30:45.123)
- **Date** = Date Only (e.g., 2026-01-15)
- **Bit** = Boolean (0 or 1, False or True)

### Constraints & Rules

**Commissions Table:**
- CommissionRate must be between 0-100%
- CommissionAmount = (SaleAmount * CommissionRate) / 100
- Status values: Pending → Approved → Paid
- Cannot have duplicate (TransactionId, AgentId) combination

**Invoices Table:**
- InvoiceNumber must be unique across all invoices
- TotalAmount = SubTotal + TaxAmount - DiscountAmount
- OutstandingAmount = TotalAmount - AmountPaid
- Status flow: Draft → Sent → Viewed → Paid/Overdue
- AmountPaid cannot exceed TotalAmount

**Brokers Table:**
- CommissionRate default is 2.5% (can be customized)
- Each UserId can only have one Broker record
- IsActive flag for soft delete (0 = inactive, 1 = active)
- Email must be unique per broker

---

## Example Data

### Commissions Example
```
CommissionId | TransactionId | BrokerId | AgentId | SaleAmount | CommissionRate | CommissionAmount | Status   | PaymentDate
1            | 100           | 5        | 12      | 5000000    | 2.5            | 125000           | Paid     | 2026-01-20
2            | 101           | 5        | 13      | 3500000    | 2.5            | 87500            | Approved | NULL
3            | 102           | 6        | 14      | 8000000    | 3.0            | 240000           | Pending  | NULL
```

### Invoices Example
```
InvoiceId | InvoiceNumber | InvoiceType | BrokerId | ClientId | TotalAmount | Status | PaymentStatus | AmountPaid | OutstandingAmount
1         | INV-2026-001  | Property    | 5        | 20       | 5662000     | Sent   | Partially Paid| 2831000    | 2831000
2         | INV-2026-002  | Commission  | 5        | 15       | 125000      | Paid   | Paid          | 125000     | 0
3         | INV-2026-003  | Service     | 6        | 21       | 50000       | Draft  | Unpaid        | 0          | 50000
```

### Brokers Example
```
BrokerId | UserId | CompanyName        | LicenseNumber | Email              | City        | CommissionRate | IsActive
1        | 5      | Prime Realty Group | LIC-2025-001  | info@prime.com     | Manila      | 2.5            | 1
2        | 6      | BGC Properties     | LIC-2025-002  | hello@bgc.com      | Makati      | 3.0            | 1
3        | 7      | Cebu Real Estate   | LIC-2025-003  | contact@cebu.com   | Cebu City   | 2.0            | 1
```

---

## Database Relationships

```
Transactions (1) ──→ (M) Commissions
└─ Each transaction can have multiple commissions (broker + agent)

Brokers (1) ──→ (M) Commissions
└─ Each broker can earn multiple commissions

Users (1) ──→ (M) Commissions (as Agent)
└─ Each agent can earn multiple commissions

Clients (1) ──→ (M) Invoices
└─ Each client can receive multiple invoices

Brokers (1) ──→ (M) Invoices
└─ Each broker can issue multiple invoices

Properties (1) ──→ (M) Invoices
└─ Each property can have multiple invoices

Transactions (1) ──→ (M) Invoices
└─ Each transaction can have multiple invoices
```

---

## SQL Constraints

```sql
-- Commissions Constraints
CHECK (CommissionRate BETWEEN 0 AND 100)
CHECK (CommissionAmount >= 0)
UNIQUE (TransactionId, AgentId)

-- Invoices Constraints
UNIQUE (InvoiceNumber)
CHECK (TotalAmount = SubTotal + TaxAmount - DiscountAmount)
CHECK (AmountPaid <= TotalAmount)
CHECK (OutstandingAmount = TotalAmount - AmountPaid)

-- Brokers Constraints
CHECK (CommissionRate BETWEEN 0 AND 100)
UNIQUE (UserId)
```

---

## Status Workflows

### Commission Status Flow
```
Pending → Approved (by Manager) → Paid (Payment Recorded)
   ↓
Rejected (if not approved)
```

### Invoice Status Flow
```
Draft → Sent (to Client) → Viewed → Paid/Overdue
              ↓
           Cancelled (if needed)
```

---

**Format:** Complete Table Specifications  
**Tables:** 3 (Commissions, Invoices, Brokers)  
**Total Fields:** 42 fields across 3 tables  
**Status:** Production Ready  
**Last Updated:** 2026

