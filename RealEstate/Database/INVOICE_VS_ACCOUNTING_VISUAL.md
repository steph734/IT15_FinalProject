# Invoice Table vs Accounting Table - Visual Comparison

## Side-by-Side Comparison

### INVOICES TABLE
```
PURPOSE: Customer Billing Documents
FOCUS: What do customers owe?

┌─────────────────────────────────────────┐
│ Invoice #INV-2026-001                   │
├─────────────────────────────────────────┤
│ Client: ABC Company                     │
│ Issue Date: 2026-01-15                  │
│ Due Date: 2026-02-15                    │
│                                         │
│ Subtotal:        PHP 100,000            │
│ Tax (12%):       PHP 12,000             │
│ Total:           PHP 112,000            │
│                                         │
│ Status: SENT                            │
│ Amount Paid: PHP 50,000                 │
│ Outstanding: PHP 62,000                 │
│ Payment Status: PARTIALLY PAID          │
└─────────────────────────────────────────┘

KEY FIELDS:
- InvoiceNumber (unique)
- ClientId, BrokerId
- SubTotal, TaxAmount, TotalAmount
- PaymentStatus, AmountPaid
- DueDate, PaymentDate
```

### ACCOUNTING TABLE
```
PURPOSE: Financial GL Entries (Double-Entry System)
FOCUS: What is our financial position?

ENTRY 1 - INVOICE CREATED:
┌─────────────────────────────────────────┐
│ GL Account: 3030                        │
│ Description: Accounts Receivable        │
│ DEBIT: PHP 112,000                      │
│ Date: 2026-01-15                        │
│ Reference: INV-2026-001                 │
└─────────────────────────────────────────┘

ENTRY 2 - INVOICE CREATED:
┌─────────────────────────────────────────┐
│ GL Account: 1020                        │
│ Description: Service Revenue           │
│ CREDIT: PHP 112,000                     │
│ Date: 2026-01-15                        │
│ Reference: INV-2026-001                 │
└─────────────────────────────────────────┘

ENTRY 3 - PAYMENT RECEIVED:
┌─────────────────────────────────────────┐
│ GL Account: 3010                        │
│ Description: Cash/Bank                  │
│ DEBIT: PHP 50,000                       │
│ Date: 2026-01-20                        │
│ Reference: BANK-TXN-12345               │
└─────────────────────────────────────────┘

ENTRY 4 - PAYMENT RECEIVED:
┌─────────────────────────────────────────┐
│ GL Account: 3030                        │
│ Description: Accounts Receivable        │
│ CREDIT: PHP 50,000                      │
│ Date: 2026-01-20                        │
│ Reference: BANK-TXN-12345               │
└─────────────────────────────────────────┘

KEY FIELDS:
- GLAccountNumber (1000-5999 range)
- TransactionDate
- DebitAmount, CreditAmount
- TransactionType (Income/Expense/Asset/Liability/Equity)
- Status (Draft/Posted/Reconciled)
```

---

## Flow Diagram

```
BILLING PROCESS                   ACCOUNTING PROCESS
─────────────────                 ──────────────────

1. Create Invoice
   ↓
   InvoiceId: 1
   InvoiceNumber: INV-2026-001
   ClientId: 20
   TotalAmount: 112,000
   ↓
   ├─→ CREATE 2 GL ENTRIES
   │   ├─ Debit Accounts Receivable (3030): 112,000
   │   └─ Credit Service Revenue (1020): 112,000
   ↓

2. Send to Customer
   Status: SENT
   ↓
   (No GL impact - just operational)
   ↓

3. Customer Pays 50,000
   ↓
   AmountPaid: 50,000
   OutstandingAmount: 62,000
   PaymentStatus: PARTIALLY PAID
   ↓
   ├─→ CREATE 2 MORE GL ENTRIES
   │   ├─ Debit Cash (3010): 50,000
   │   └─ Credit Accounts Receivable (3030): 50,000
   ↓

4. Full Payment Received
   ↓
   AmountPaid: 112,000
   OutstandingAmount: 0
   PaymentStatus: PAID
   Status: PAID
   ↓
   (Accounting entries already recorded)
```

---

## Table Structure Comparison

### INVOICES TABLE (1 Record)
```
InvoiceId              | 1
InvoiceNumber          | INV-2026-001        ← Unique identifier
InvoiceType            | Service
IssuedDate             | 2026-01-15
DueDate                | 2026-02-15
BrokerId               | 5
ClientId               | 20                  ← Who owes money
PropertyId             | NULL
TransactionId          | NULL
SubTotal               | 100,000
TaxAmount              | 12,000
DiscountAmount         | 0
TotalAmount            | 112,000             ← Invoice amount
Status                 | Paid
PaymentStatus          | Partially Paid      ← Collection status
AmountPaid             | 50,000              ← What they paid
OutstandingAmount      | 62,000              ← What they still owe
PaymentMethod          | Bank Transfer
PaymentDate            | 2026-01-20
PaymentReferenceNumber | BANK-12345
Description            | Service fees
Notes                  | NULL
CreatedBy              | 5
SentDate               | 2026-01-15
CreatedDate            | 2026-01-15
UpdatedDate            | 2026-01-20
```

### ACCOUNTING TABLE (4 Records)
```
Record 1 (Invoice Created - Debit):
AccountingId           | 1
TransactionDate        | 2026-01-15
GLAccountNumber        | 3030                ← Accounts Receivable
Description            | Invoice INV-2026-001
TransactionType        | Asset
Category               | Service
DebitAmount            | 112,000
CreditAmount           | NULL
Amount                 | 112,000
InvoiceId              | 1
CommissionId           | NULL
PayrollId              | NULL
PaymentTransactionId   | NULL
UserId                 | NULL
BrokerId               | 5
Status                 | Posted
IsReconciled           | 0
Department             | Sales
Reference              | INV-2026-001

Record 2 (Invoice Created - Credit):
AccountingId           | 2
TransactionDate        | 2026-01-15
GLAccountNumber        | 1020                ← Service Revenue
Description            | Invoice INV-2026-001
TransactionType        | Income
Category               | Service
DebitAmount            | NULL
CreditAmount           | 112,000
Amount                 | 112,000
InvoiceId              | 1
Status                 | Posted
IsReconciled           | 0
Reference              | INV-2026-001

Record 3 (Payment Received - Debit):
AccountingId           | 3
TransactionDate        | 2026-01-20
GLAccountNumber        | 3010                ← Cash/Bank
Description            | Payment for INV-2026-001
TransactionType        | Asset
Category               | Other
DebitAmount            | 50,000
CreditAmount           | NULL
Amount                 | 50,000
PaymentTransactionId   | 100
InvoiceId              | 1
Status                 | Posted

Record 4 (Payment Received - Credit):
AccountingId           | 4
TransactionDate        | 2026-01-20
GLAccountNumber        | 3030                ← Accounts Receivable
Description            | Payment for INV-2026-001
TransactionType        | Asset
Category               | Other
DebitAmount            | NULL
CreditAmount           | 50,000
Amount                 | 50,000
PaymentTransactionId   | 100
InvoiceId              | 1
Status                 | Posted
```

---

## Key Distinctions

### **INVOICES TABLE**
- ✅ User-facing document
- ✅ Tracks business relationships with customers
- ✅ Manages payment collection
- ✅ Single record per invoice
- ✅ Used by: Sales, Customer Service, Billing
- ✅ Reports: Invoice aging, collections, revenue by customer

### **ACCOUNTING TABLE**
- ✅ Internal financial records
- ✅ Double-entry bookkeeping
- ✅ Multiple records per transaction
- ✅ Links to multiple business operations
- ✅ Used by: Accountants, Finance, Auditors
- ✅ Reports: Balance sheet, Income statement, GL report

---

## Why Keep Both?

### **Separation of Concerns**
- Invoices: Operational tracking
- Accounting: Financial reporting

### **Different Access Control**
- Invoices: Sales team, Customer service, Customers
- Accounting: Finance team, CFO, External auditors

### **Different Reporting Needs**
- Invoices: "Which customers haven't paid?"
- Accounting: "What is our financial position?"

### **Compliance & Audit**
- Invoices: Customer communication, collections
- Accounting: Tax reporting, financial statements

### **Data Integrity**
- Invoices: Business transactions
- Accounting: Financial records (must balance)

---

## Summary

```
┌──────────────────────────────────────────────────────────────┐
│              INVOICES TABLE                                  │
│  Customer Billing Documents - What's owed                    │
│  1 Invoice = 1 Record                                        │
│  Status tracking, Payment tracking                           │
└──────────────────────────────────────────────────────────────┘
                              ↓
                        (Creates)
                              ↓
┌──────────────────────────────────────────────────────────────┐
│              ACCOUNTING TABLE                                │
│  Financial GL Entries - Company's Financial Position         │
│  1 Invoice = 2-4+ GL Records (Double-Entry)                 │
│  GL accounts, Debits/Credits, Reconciliation                │
└──────────────────────────────────────────────────────────────┘
```

**NOT THE SAME TABLE** - They work together to provide:
- Operational visibility (Invoices)
- Financial accuracy (Accounting)

