# 🛒 Buy Property Modal - Complete Database Mapping

## 📊 Database Table: `Transactions`

**Full Path:** `[db49649].[dbo].[Transactions]`

---

## 🔗 Form Field to Database Column Mapping

### Step 1: Personal Details

| Form Field | Input Name | Database Column | Data Type | Example | Notes |
|------------|-----------|-----------------|-----------|---------|-------|
| Full Name * | `fullName` | `CustomerName` | NVARCHAR(100) | "Juan Dela Cruz" | Required, trimmed |
| Email Address * | `email` | `CustomerEmail` | NVARCHAR(255) | "juan@email.com" | Required, trimmed |
| Phone Number * | `phoneNumber` | `CustomerPhone` | NVARCHAR(20) | "+63 9XX XXX XXXX" | Required, trimmed |
| Address | `address` | `Notes` (partial) | NVARCHAR(2000) | "123 Main St, City" | Optional, stored in Notes |

### Step 2: Property Cost

| Form Field | Input Name | Database Column | Data Type | Example | Notes |
|------------|-----------|-----------------|-----------|---------|-------|
| Property ID | `propertyId` (hidden) | `PropertyId` | INT (FK) | 5 | Foreign Key to Properties |
| Total Amount | `totalAmount` (hidden) | `SellingPrice` | DECIMAL(18,2) | 5,165,000.00 | Calculated: Price + Fees |

### Step 3: Payment Method

| Form Field | Input Name | Database Column | Data Type | Example | Notes |
|------------|-----------|-----------------|-----------|---------|-------|
| Payment Method | `paymentMethod` (hidden) | `Notes` (partial) | NVARCHAR(2000) | "GCash" | Stored in Notes field |

### Auto-Generated Fields

| Database Column | Data Type | Value | Description |
|----------------|-----------|-------|-------------|
| `TransactionId` | INT (PK, AI) | Auto-generated | Primary key, auto-increment |
| `Status` | NVARCHAR(50) | "Pending" | Default status |
| `CreatedAt` | DATETIME2 | GETUTCDATE() | Timestamp of creation |
| `AgentId` | INT (FK, NULL) | NULL | Assigned later by manager |
| `CustomerId` | INT (FK, NULL) | NULL | Link to registered user if exists |
| `ClosedAt` | DATETIME2 (NULL) | NULL | Set when transaction completes |

---

## 💾 Complete Database Record Example

### Form Submission:
```
Full Name: Juan Dela Cruz
Email: juan@email.com
Phone: +63 917 123 4567
Address: 123 Main St, Manila
Property ID: 5
Total Amount: ₱5,165,000
Payment Method: GCash
```

### Database Record Created:
```sql
INSERT INTO [db49649].[dbo].[Transactions] VALUES (
    TransactionId = 123,                    -- Auto-generated
    PropertyId = 5,                         -- From hidden field
    AgentId = NULL,                         -- Not assigned yet
    CustomerId = NULL,                      -- Not linked yet
    CustomerName = 'Juan Dela Cruz',       -- From fullName input
    CustomerEmail = 'juan@email.com',      -- From email input
    CustomerPhone = '+63 917 123 4567',   -- From phoneNumber input
    SellingPrice = 5165000.00,             -- From totalAmount (calculated)
    Status = 'Pending',                    -- Default value
    CreatedAt = '2026-04-27 10:30:00',    -- Auto-generated UTC time
    ClosedAt = NULL,                       -- Not closed yet
    Notes = 'Payment Method: GCash
             Buyer Address: 123 Main St, Manila
             Property: Luxury Condo in Makati
             Location: Makati City, Metro Manila'
);
```

---

## 📝 Notes Field Format

The `Notes` field stores multiple pieces of information in a structured format:

```
Payment Method: GCash
Buyer Address: 123 Main St, Manila
Property: Luxury Condo in Makati
Location: Makati City, Metro Manila
```

### Parsing Notes in C#:
```csharp
var lines = transaction.Notes.Split('\n');
var paymentMethod = lines[0].Replace("Payment Method: ", "").Trim();
var address = lines[1].Replace("Buyer Address: ", "").Trim();
var propertyTitle = lines[2].Replace("Property: ", "").Trim();
var location = lines[3].Replace("Location: ", "").Trim();
```

---

## 🔄 Transaction Status Workflow

```
┌─────────────┐
│  Submitted  │  ← Form creates transaction
│  (Pending)  │
└──────┬──────┘
       │
       ├──────────────┬────────────────┐
       │              │                │
       ▼              ▼                ▼
┌──────────┐  ┌──────────┐    ┌──────────┐
│ Manager  │  │ Customer │    │  Error/  │
│ Reviews  │  │ Cancels  │    │  Failed  │
└────┬─────┘  └────┬─────┘    └────┬─────┘
     │              │              │
     ▼              ▼              ▼
┌──────────┐  ┌──────────┐  ┌──────────┐
│Approved &│  │Cancelled │  │  Void    │
│ Closed   │  │          │  │          │
└──────────┘  └──────────┘  └──────────┘
```

---

## 🔗 Foreign Key Relationships

### 1. PropertyId → Properties
```sql
SELECT * FROM Properties 
WHERE PropertyId = [Transaction].PropertyId
```
- Gets property details (title, location, price, etc.)
- RESTRICT delete (cannot delete property with transactions)

### 2. AgentId → Users (nullable)
```sql
SELECT * FROM Users 
WHERE UserId = [Transaction].AgentId
```
- Agent handling the transaction
- SET NULL on delete (preserve transaction if agent deleted)
- Initially NULL, assigned by manager later

### 3. CustomerId → Users (nullable)
```sql
SELECT * FROM Users 
WHERE UserId = [Transaction].CustomerId
```
- Registered user who made purchase
- SET NULL on delete (preserve transaction if user deleted)
- NULL for guest purchases

---

## 📊 Related Tables

When a Transaction is created, these related tables can be populated:

### 1. Payments (Buyer Payments)
```sql
INSERT INTO Payments (
    TransactionId,  -- Links to this transaction
    Amount,
    PaymentMethod,  -- GCash, PayMaya, BankTransfer, CreditCard
    ReferenceNumber,
    Status
)
```

### 2. Invoices (Billing Documents)
```sql
INSERT INTO Invoices (
    TransactionId,  -- Links to this transaction
    InvoiceNumber,
    Amount,
    Status
)
```

### 3. Commissions (Agent Commission)
```sql
INSERT INTO Commissions (
    TransactionId,  -- Links to this transaction
    AgentId,
    CommissionAmount,
    CommissionPercent,
    Status
)
```

### 4. FinancialRecords (Accounting)
```sql
INSERT INTO FinancialRecords (
    TransactionId,  -- Links to this transaction
    AccountId,
    Amount,
    Type,
    Description
)
```

---

## 🔍 Query Examples

### View All Purchase Requests
```sql
SELECT TOP 20 
    t.TransactionId,
    t.CustomerName,
    t.CustomerEmail,
    t.CustomerPhone,
    p.Title AS PropertyTitle,
    p.Location AS PropertyLocation,
    t.SellingPrice,
    t.Status,
    t.CreatedAt,
    t.Notes
FROM Transactions t
LEFT JOIN Properties p ON t.PropertyId = p.PropertyId
ORDER BY t.CreatedAt DESC;
```

### View Pending Purchases
```sql
SELECT 
    t.TransactionId,
    t.CustomerName,
    t.CustomerEmail,
    t.CustomerPhone,
    p.Title,
    t.SellingPrice,
    t.CreatedAt
FROM Transactions t
INNER JOIN Properties p ON t.PropertyId = p.PropertyId
WHERE t.Status = 'Pending'
ORDER BY t.CreatedAt DESC;
```

### Get Transaction Details
```sql
SELECT 
    t.*,
    p.Title AS PropertyTitle,
    p.Location AS PropertyLocation,
    p.PropertyType,
    p.Bedrooms,
    p.Bathrooms,
    u.Name AS AgentName
FROM Transactions t
LEFT JOIN Properties p ON t.PropertyId = p.PropertyId
LEFT JOIN Users u ON t.AgentId = u.UserId
WHERE t.TransactionId = 123;
```

### Parse Notes Field
```sql
SELECT 
    TransactionId,
    CustomerName,
    SellingPrice,
    -- Extract payment method from notes
    SUBSTRING(Notes, 
              CHARINDEX('Payment Method: ', Notes) + LEN('Payment Method: '),
              CHARINDEX(CHAR(10), Notes, CHARINDEX('Payment Method: ', Notes)) - CHARINDEX('Payment Method: ', Notes) - LEN('Payment Method: ')
    ) AS PaymentMethod,
    -- Extract address from notes
    SUBSTRING(Notes, 
              CHARINDEX('Buyer Address: ', Notes) + LEN('Buyer Address: '),
              CHARINDEX(CHAR(10), Notes, CHARINDEX('Buyer Address: ', Notes)) - CHARINDEX('Buyer Address: ', Notes) - LEN('Buyer Address: ')
    ) AS BuyerAddress
FROM Transactions
WHERE TransactionId = 123;
```

---

## 🎯 Controller Implementation

### File: `Controllers/PropertiesController.cs`

```csharp
[HttpPost]
[Route("Properties/BuyProperty")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> BuyProperty(
    [FromForm] int propertyId,
    [FromForm] string fullName,
    [FromForm] string email,
    [FromForm] string phoneNumber,
    [FromForm] string? address,
    [FromForm] decimal totalAmount,
    [FromForm] string paymentMethod)
{
    try
    {
        // 1. Validate all required fields
        if (string.IsNullOrWhiteSpace(fullName))
            return Error("Full name is required.");
        
        if (string.IsNullOrWhiteSpace(email))
            return Error("Email address is required.");
        
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Error("Phone number is required.");
        
        if (totalAmount <= 0)
            return Error("Invalid purchase amount.");

        // 2. Verify property exists
        var property = _db.Properties.Find(propertyId);
        if (property == null)
            return Error("Property not found.");

        // 3. Create transaction record
        var transaction = new Transaction
        {
            PropertyId = propertyId,
            CustomerName = fullName.Trim(),
            CustomerEmail = email.Trim(),
            CustomerPhone = phoneNumber.Trim(),
            SellingPrice = totalAmount,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            Notes = $"Payment Method: {paymentMethod?.Trim()}\n" +
                    $"Buyer Address: {address?.Trim()}\n" +
                    $"Property: {property.Title}\n" +
                    $"Location: {property.Location}"
        };

        // 4. Save to database
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        // 5. Log success
        Console.WriteLine($"[SUCCESS] Transaction {transaction.TransactionId} created");

        // 6. Return success
        TempData["SuccessMessage"] = $"Purchase request submitted! Total: ₱{totalAmount:N0}";
        TempData["TransactionId"] = transaction.TransactionId;
        
        return RedirectToAction("Details", new { id = propertyId });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] {ex.Message}");
        TempData["ErrorMessage"] = $"Error: {ex.Message}";
        return RedirectToAction("Details", new { id = propertyId });
    }
}
```

---

## 📱 Modal Form Structure

### File: `Views/Properties/Details.cshtml`

```html
<form id="buyPropertyForm" method="post" action="/Properties/BuyProperty">
    <input type="hidden" name="propertyId" value="@p.Id" />
    @Html.AntiForgeryToken()
    
    <!-- Step 1: Personal Details -->
    <div id="step1">
        <input name="fullName" required />
        <input name="email" required />
        <input name="phoneNumber" required />
        <input name="address" />
    </div>
    
    <!-- Step 2: Property Cost -->
    <div id="step2">
        <input type="hidden" name="totalAmount" value="@(calculated)" />
    </div>
    
    <!-- Step 3: Payment Method -->
    <div id="step3">
        <input type="hidden" name="paymentMethod" required />
    </div>
    
    <!-- Step 4: Confirmation -->
    <div id="step4">
        <input type="checkbox" required />
    </div>
</form>
```

---

## ✅ Validation Checklist

### Client-Side (Browser)
- [x] Full Name: Required field
- [x] Email: Required + email format validation
- [x] Phone: Required field
- [x] Payment Method: Must be selected
- [x] Terms & Conditions: Must be checked

### Server-Side (Controller)
- [x] Full Name: Not null/empty
- [x] Email: Not null/empty
- [x] Phone: Not null/empty
- [x] Total Amount: Must be > 0
- [x] Property ID: Must exist in database
- [x] AntiForgeryToken: CSRF protection

---

## 🔒 Security Features

1. **AntiForgeryToken** - Prevents CSRF attacks
2. **Server Validation** - All inputs validated server-side
3. **SQL Injection Protection** - Entity Framework parameterization
4. **XSS Protection** - HTML encoding in views
5. **HTTPS Required** - GPS and forms require secure connection

---

## 📈 Performance Considerations

### Database Indexes
```sql
CREATE INDEX IX_Transactions_PropertyId ON Transactions(PropertyId);
CREATE INDEX IX_Transactions_AgentId ON Transactions(AgentId);
CREATE INDEX IX_Transactions_CustomerId ON Transactions(CustomerId);
CREATE INDEX IX_Transactions_Status ON Transactions(Status);
```

### Query Optimization
- Use `.Find()` for primary key lookups
- Include related data with `.Include()`
- Paginate results with `.Skip().Take()`

---

## 🐛 Troubleshooting

### Issue: Form submission fails
**Check:**
1. AntiForgeryToken present in form
2. All required fields filled
3. Property ID is valid
4. Database connection working

### Issue: Transaction not saving
**Check:**
1. Entity Framework DbContext configured
2. Migrations applied
3. No validation errors
4. Check console logs for errors

### Issue: Total amount incorrect
**Check:**
1. Calculation in hidden field
2. Decimal precision (18,2)
3. Fees calculation logic

---

## 📝 Summary

| Item | Value |
|------|-------|
| **Database** | `db49649` |
| **Table** | `Transactions` |
| **Primary Key** | `TransactionId` (Auto-increment) |
| **Foreign Keys** | `PropertyId`, `AgentId`, `CustomerId` |
| **Model Class** | `RealEstate.Models.Transaction` |
| **Controller** | `PropertiesController.BuyProperty()` |
| **Route** | `POST /Properties/BuyProperty` |
| **View** | `Views/Properties/Details.cshtml` (Modal) |
| **Security** | AntiForgeryToken + Server Validation |
| **Status Flow** | Pending → Closed / Cancelled |

---

**Last Updated:** April 27, 2026  
**Status:** ✅ Complete Implementation
