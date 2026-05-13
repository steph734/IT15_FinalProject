# Invoice Table vs Accounting Table - Key Differences

## Quick Comparison

| Aspect | Invoice Table | Accounting Table |
|--------|---|---|
| **Purpose** | Billing/Customer Invoices | General Ledger/Financial Records |
| **Who Uses** | Billing Department, Customers | Accounting, Finance, Auditors |
| **Primary Goal** | Track what customers owe | Track financial position |
| **Records** | Individual invoices to customers | Double-entry GL entries |
| **Frequency** | One record per invoice | Multiple records per transaction |

---

## Invoice Table Purpose

**What it does:**
- Tracks **billing documents** sent to customers
- Records what amount customers **owe** (Accounts Receivable)
- Tracks payment status and collection
- Manages invoicing workflow

**Example:**
```
Invoice #INV-2026-001
Client: ABC Company
Amount: PHP 100,000
Status: Sent
AmountPaid: PHP 50,000
OutstandingAmount: PHP 50,000
```

**Fields Focus:**
- InvoiceNumber (unique identifier)
- BrokerId, ClientId (who is involved)
- SubTotal, TaxAmount, TotalAmount (amounts)
- PaymentStatus, AmountPaid, OutstandingAmount
- IssuedDate, DueDate, PaymentDate

---

## Accounting Table Purpose

**What it does:**
- Records **financial transactions** using double-entry bookkeeping
- Creates GL entries for financial reporting
- Links to multiple business operations (Invoice, Commission, Payroll)
- Tracks financial position and profitability

**Example:**
```
When Invoice #INV-2026-001 is created:

Entry 1 (Debit):
GL Account: 3030 (Accounts Receivable)
Amount: PHP 100,000
Description: Invoice INV-2026-001 created

Entry 2 (Credit):
GL Account: 1020 (Service Revenue)
Amount: PHP 100,000
Description: Invoice INV-2026-001 created
```

**Fields Focus:**
- TransactionDate (when it happened)
- GLAccountNumber (which GL account affected)
- DebitAmount, CreditAmount (double-entry system)
- TransactionType (Income/Expense/Asset/Liability/Equity)
- Status (Draft/Posted/Reconciled/Reversed)

---

## Real-World Example

### Scenario: A broker issues invoice to client for PHP 100,000

**INVOICES TABLE - Records:**
```
InvoiceId: 1
InvoiceNumber: INV-2026-001
BrokerId: 5
ClientId: 20
TotalAmount: 100,000
Status: Sent
PaymentStatus: Unpaid
AmountPaid: 0
OutstandingAmount: 100,000
CreatedDate: 2026-01-15
```

**ACCOUNTING TABLE - Records TWO entries:**

Entry 1 - Debit:
```
AccountingId: 1
GLAccountNumber: 3030 (Accounts Receivable)
Description: Invoice INV-2026-001 from Broker 5
DebitAmount: 100,000
CreditAmount: NULL
InvoiceId: 1
TransactionType: Asset
Status: Posted
```

Entry 2 - Credit:
```
AccountingId: 2
GLAccountNumber: 1020 (Service Revenue)
Description: Invoice INV-2026-001 from Broker 5
DebitAmount: NULL
CreditAmount: 100,000
InvoiceId: 1
TransactionType: Income
Status: Posted
```

---

### Later: Client pays PHP 50,000

**INVOICES TABLE - Updated:**
```
InvoiceId: 1
AmountPaid: 50,000          ← Updated
OutstandingAmount: 50,000   ← Updated
PaymentStatus: Partially Paid
PaymentDate: 2026-01-20
PaymentReferenceNumber: BANK-TXN-12345
```

**ACCOUNTING TABLE - Records TWO MORE entries:**

Entry 3 - Debit:
```
AccountingId: 3
GLAccountNumber: 3010 (Cash/Bank)
DebitAmount: 50,000
CreditAmount: NULL
Description: Payment received for INV-2026-001
PaymentTransactionId: 100
```

Entry 4 - Credit:
```
AccountingId: 4
GLAccountNumber: 3030 (Accounts Receivable)
DebitAmount: NULL
CreditAmount: 50,000
Description: Payment received for INV-2026-001
PaymentTransactionId: 100
```

---

## Key Differences Explained

### 1. **One Invoice = Multiple GL Entries**
- **1 Invoice Record** in Invoices table
- **Multiple GL Entries** in Accounting table (at least 2, often more)

### 2. **Purpose Difference**
- **Invoices Table**: "How much does this customer owe us?"
- **Accounting Table**: "What is our financial position?"

### 3. **Users**
- **Invoices**: Used by billing, sales, customer service
- **Accounting**: Used by accountants, auditors, finance team

### 4. **Reporting**
- **Invoices**: Invoice aging report, collections report, revenue by customer
- **Accounting**: Balance sheet, income statement, trial balance, GL report

### 5. **Status Tracking**
- **Invoices**: Draft → Sent → Viewed → Paid/Overdue
- **Accounting**: Draft → Posted → Reconciled (with bank)

---

## Data Relationship

```
Invoices Table (1)
    ↓
    └──→ Accounting Table (M - Usually 2-4 GL entries)
```

**One invoice creates multiple GL entries:**
- When invoice is created: 2 GL entries (Debit Receivable, Credit Revenue)
- When payment received: 2-3 GL entries (Debit Cash, Credit Receivable, possibly more)

---

## Which Table to Query For?

### Use **Invoices Table** when you need:
- List of outstanding invoices
- Which customers haven't paid
- Total revenue by customer
- Invoice aging analysis
- Payment history with specific clients

### Use **Accounting Table** when you need:
- Balance sheet
- Income statement
- Cash flow analysis
- GL account balance
- Financial position at a date
- Audit trail for auditors
- Tax reporting
- Reconciliation with bank

---

## Example Queries

### From INVOICES Table:
```sql
-- Outstanding invoices
SELECT InvoiceNumber, ClientId, OutstandingAmount
FROM Invoices
WHERE PaymentStatus IN ('Unpaid', 'Partially Paid')
ORDER BY DueDate;

-- Total revenue by customer
SELECT ClientId, SUM(TotalAmount) as TotalRevenue
FROM Invoices
WHERE Status = 'Paid'
GROUP BY ClientId;
```

### From ACCOUNTING Table:
```sql
-- Balance sheet (Assets)
SELECT GLAccountNumber, SUM(DebitAmount - CreditAmount) as Balance
FROM Accounting
WHERE TransactionType = 'Asset'
AND Status = 'Posted'
GROUP BY GLAccountNumber;

-- Monthly profit
SELECT 
    MONTH(TransactionDate) as Month,
    SUM(CASE WHEN TransactionType = 'Income' THEN CreditAmount ELSE 0 END) as Income,
    SUM(CASE WHEN TransactionType = 'Expense' THEN DebitAmount ELSE 0 END) as Expenses
FROM Accounting
WHERE Status = 'Posted'
GROUP BY MONTH(TransactionDate);
```

---

## Why Both Tables?

### **Invoices Table is for:**
- Operational tracking (billing)
- Customer management
- Payment collection

### **Accounting Table is for:**
- Financial reporting
- Tax compliance
- Audit trail
- Financial analysis
- Reconciliation

**They work together:**
- Invoices create GL entries
- GL entries roll up to financial statements
- Both needed for complete financial picture

---

## Summary

✅ **INVOICES:** Customer billing document tracking  
✅ **ACCOUNTING:** Financial GL entries for financial reporting  
❌ **NOT THE SAME** - Different purposes, different users, different data

**Think of it this way:**
- **Invoice** = Customer receipt ("You owe me PHP 100,000")
- **Accounting** = Company's financial records ("Our receivables are PHP 500,000")

---

**Recommendation:** Keep both tables separate for:
- Clear separation of concerns
- Accurate financial reporting
- Audit compliance
- Different user access rights

