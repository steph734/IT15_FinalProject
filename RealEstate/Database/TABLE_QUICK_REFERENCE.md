# EstateFlow Database - Table Quick Reference

## 📋 All 12 Tables at a Glance

### AUTHENTICATION & AUTHORIZATION (2 tables)

#### 1. **Roles**
```
┌─────────────────────────────────────────┐
│              Roles                      │
├─────────────────────────────────────────┤
│ PK │ RoleId (INT)                       │
│ U  │ RoleType (NVARCHAR(50))            │
│    │ CreatedAt (DATETIME2)              │
└─────────────────────────────────────────┘

Sample Data:
- Admin
- Broker
- Agent
- Investor
- Manager
```

#### 2. **Users**
```
┌──────────────────────────────────────────────┐
│              Users                           │
├──────────────────────────────────────────────┤
│ PK │ UserId (INT)                            │
│ FK │ RoleId (INT) → Roles                    │
│    │ FullName (NVARCHAR(100))                │
│ U  │ Email (NVARCHAR(255))                   │
│    │ PasswordHash (NVARCHAR(MAX))            │
│    │ IsActive (BIT)                          │
│    │ CreatedAt, UpdatedAt (DATETIME2)        │
└──────────────────────────────────────────────┘
```

---

### CUSTOMER MANAGEMENT (3 tables)

#### 3. **Customers** (EXTENDED)
```
┌─────────────────────────────────────────────────────┐
│              Customers                              │
├─────────────────────────────────────────────────────┤
│ PK │ ClientID (INT)                                  │
│    │ BrokerId (INT)                                  │
│    │                                                 │
│    │ PERSONAL INFO:                                  │
│    │ - FullName, Email, Phone                        │
│    │ - Address, City, State, ZipCode, Country        │
│    │                                                 │
│    │ PROPERTY INFO:                                  │
│    │ - PropertyType (Residential/Commercial/Ind.)    │
│    │ - InterestedProperties (NVARCHAR(MAX))          │
│    │ - MinBudget, MaxBudget (DECIMAL(18,2))          │
│    │ - Status (Interested/Follow-up/Under Review)    │
│    │                                                 │
│    │ PAYMENT INFO:                                   │
│    │ - PaymentMethod (Card/Paypal/Transfer)          │
│    │ - CardholderName                                │
│    │ - CardNumber, ExpiryDate, CVV (encrypted)       │
│    │                                                 │
│    │ METADATA:                                       │
│    │ - Notes                                         │
│    │ - CreatedDate, LastContactedDate                │
│    │ - IsActive (BIT)                                │
└─────────────────────────────────────────────────────┘

Indexes: Email, Status, PropertyType, BrokerId
Status Values: "Interested", "Follow-up", "Under Review"
```

#### 4. **PaymentTransactions**
```
┌────────────────────────────────────────────────┐
│          PaymentTransactions                   │
├────────────────────────────────────────────────┤
│ PK │ Id (INT)                                   │
│ FK │ CustomerId (INT) → Customers (CASCADE)     │
│    │                                            │
│    │ PayMongo Integration:                      │
│    │ - PayMongoPaymentIntentId (NVARCHAR)       │
│    │ - PayMongoSourceId (NVARCHAR)              │
│    │                                            │
│    │ Amount (DECIMAL(18,2))                     │
│    │ Currency (NVARCHAR(10)) - Default: PHP     │
│    │ Status (pending/succeeded/failed)          │
│    │ PaymentMethod (NVARCHAR(50))               │
│    │ Description (NVARCHAR(MAX))                │
│    │                                            │
│    │ CreatedDate, UpdatedDate (DATETIME2)       │
│    │ WebhookResponse (NVARCHAR(MAX))            │
│    │ ErrorMessage (NVARCHAR(MAX))               │
│    │ IsProcessed (BIT)                          │
└────────────────────────────────────────────────┘

Indexes: CustomerId, Status, PayMongoPaymentIntentId
```

#### 5. **Clients**
```
┌──────────────────────────────────────────┐
│              Clients                     │
├──────────────────────────────────────────┤
│ PK │ ClientId (INT)                      │
│    │ Name (NVARCHAR(100))                │
│    │ Email, Phone                        │
│    │ Address, City, State, Country       │
│    │ ZipCode                             │
│    │ ClientType (Buyer/Seller/Investor)  │
│    │ Status (Active/Inactive/Prospect)   │
│    │ Notes (NVARCHAR(MAX))               │
│    │ CreatedDate, UpdatedDate            │
└──────────────────────────────────────────┘

Indexes: Email, Status
```

---

### APPOINTMENTS & SCHEDULING (3 tables)

#### 6. **Appointments**
```
┌────────────────────────────────────────────┐
│           Appointments                     │
├────────────────────────────────────────────┤
│ PK │ AppointmentId (INT)                    │
│ FK │ ClientId → Clients (CASCADE)           │
│ FK │ AgentId (INT)                          │
│    │                                        │
│    │ AppointmentDate (DATETIME2)            │
│    │ Duration (INT) - in minutes            │
│    │ Subject (NVARCHAR(MAX))                │
│    │ Description (NVARCHAR(MAX))            │
│    │ Status (Scheduled/Completed/Cancelled)│
│    │ Location (NVARCHAR(MAX))               │
│    │ Notes (NVARCHAR(MAX))                  │
│    │                                        │
│    │ CreatedDate, CreatedBy                 │
└────────────────────────────────────────────┘

Indexes: AppointmentDate, Status, ClientId
```

#### 7. **ViewingAppointments**
```
┌─────────────────────────────────────────────┐
│        ViewingAppointments                  │
├─────────────────────────────────────────────┤
│ PK │ Id (INT)                               │
│    │ PropertyId (INT)                       │
│    │                                        │
│    │ ClientName (NVARCHAR(100))             │
│    │ ClientEmail (NVARCHAR(255))            │
│    │ ClientPhone (NVARCHAR(20))             │
│    │                                        │
│    │ AppointmentDate (DATETIME2)            │
│    │ Status (Pending/Confirmed/Cancelled)   │
│    │ Status (Completed)                     │
│    │                                        │
│    │ Notes (NVARCHAR(MAX))                  │
│    │ CreatedDate (DATETIME2)                │
│    │ CreatedBy (INT)                        │
└─────────────────────────────────────────────┘

Indexes: AppointmentDate, Status
```

#### 8. **Schedules**
```
┌──────────────────────────────────────────────┐
│            Schedules                         │
├──────────────────────────────────────────────┤
│ PK │ ScheduleId (INT)                        │
│ FK │ UserId → Users (CASCADE)                │
│    │                                         │
│    │ ScheduleDate (DATE)                     │
│    │ StartTime, EndTime (TIME)               │
│    │ Title (NVARCHAR(MAX))                   │
│    │ Description (NVARCHAR(MAX))             │
│    │                                         │
│    │ Color (NVARCHAR(50)) - For display      │
│    │ IsRecurring (BIT)                       │
│    │ RecurrencePattern (Daily/Weekly/Month)  │
│    │                                         │
│    │ CreatedDate (DATETIME2)                 │
└──────────────────────────────────────────────┘

Indexes: UserId, ScheduleDate
```

---

### VERIFICATION & SECURITY (1 table)

#### 9. **OtpVerifications**
```
┌────────────────────────────────────┐
│      OtpVerifications              │
├────────────────────────────────────┤
│ PK │ Id (INT)                       │
│    │ Email (NVARCHAR(255))          │
│    │ OtpCode (NVARCHAR(10))         │
│    │ ExpiresAt (DATETIME2)          │
│    │ IsVerified (BIT)               │
│    │ CreatedAt (DATETIME2)          │
│    │ VerifiedAt (DATETIME2)         │
└────────────────────────────────────┘

Indexes: Email, ExpiresAt
Purpose: 2FA/Email verification
Retention: Auto-cleanup of expired records
```

---

### PROPERTY MANAGEMENT (2 tables)

#### 10. **Properties**
```
┌────────────────────────────────────────────────┐
│            Properties                          │
├────────────────────────────────────────────────┤
│ PK │ PropertyId (INT)                          │
│    │                                           │
│    │ Title, Description (NVARCHAR(MAX))        │
│    │ Address, City, State, Country, ZipCode    │
│    │ Price (DECIMAL(18,2))                     │
│    │                                           │
│    │ PropertyType (Residential/Commercial/Ind.)│
│    │ Bedrooms, Bathrooms (INT)                 │
│    │ SquareFeet (DECIMAL(10,2))                │
│    │                                           │
│    │ Status (Available/Sold/Rented)            │
│    │ ListingDate (DATETIME2)                   │
│    │ AgentId (INT)                             │
│    │                                           │
│    │ CreatedDate, UpdatedDate                  │
└────────────────────────────────────────────────┘

Indexes: PropertyType, Status, City, Price
Primary Search Columns: Status, City, Price Range, PropertyType
```

#### 11. **Brokers**
```
┌──────────────────────────────────────────────┐
│            Brokers                           │
├──────────────────────────────────────────────┤
│ PK │ BrokerId (INT)                          │
│ FK │ UserId → Users (CASCADE)                │
│    │                                         │
│    │ CompanyName (NVARCHAR(MAX))             │
│    │ LicenseNumber (NVARCHAR(50))            │
│    │ Phone, Email                            │
│    │ Address, City, State, Country, ZipCode  │
│    │                                         │
│    │ CommissionRate (DECIMAL(5,2)) - %       │
│    │ IsActive (BIT)                          │
│    │                                         │
│    │ CreatedDate (DATETIME2)                 │
└──────────────────────────────────────────────┘

Indexes: UserId, IsActive
Relationship: One broker per user account
```

---

### TRANSACTIONS & SALES (1 table)

#### 12. **Transactions**
```
┌────────────────────────────────────────────────┐
│           Transactions                         │
├────────────────────────────────────────────────┤
│ PK │ TransactionId (INT)                       │
│ FK │ PropertyId → Properties (SET NULL)        │
│ FK │ BuyerId → Clients (SET NULL)              │
│ FK │ SellerId → Clients (SET NULL)             │
│ FK │ AgentId (INT)                             │
│    │                                           │
│    │ TransactionDate (DATETIME2)               │
│    │ SalePrice (DECIMAL(18,2))                 │
│    │ Commission (DECIMAL(18,2))                │
│    │                                           │
│    │ Status (Pending/Completed/Cancelled)      │
│    │ Notes (NVARCHAR(MAX))                     │
│    │ CreatedDate (DATETIME2)                   │
└────────────────────────────────────────────────┘

Indexes: PropertyId, Status, TransactionDate
Delete Strategy: SET NULL (preserve transaction history)
```

---

## 🔄 Relationship Map

```
┌─────────────────────────────────────────────────────────┐
│                    CORE USERS                           │
├─────────────────────────────────────────────────────────┤
│  Roles ──1:M─→ Users ──1:M─→ Schedules                 │
│                       ┌────→ Brokers                    │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│                CUSTOMER & PAYMENTS                      │
├─────────────────────────────────────────────────────────┤
│  Customers ←───1:M─── PaymentTransactions              │
│       ├─ Has PropertyType                               │
│       ├─ Has Status                                     │
│       └─ Associated with Broker                         │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│               CLIENT MANAGEMENT                         │
├─────────────────────────────────────────────────────────┤
│  Clients ──1:M─→ Appointments                           │
│          ──1:M─→ Transactions (as Buyer)                │
│          ──1:M─→ Transactions (as Seller)               │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│            PROPERTY & TRANSACTIONS                      │
├─────────────────────────────────────────────────────────┤
│  Properties ──1:M─→ Transactions                        │
│            ──1:M─→ ViewingAppointments                  │
└─────────────────────────────────────────────────────────┘
```

---

## 📊 Data Storage Estimates

| Table | Typical Records | Estimated Storage | Growth Rate |
|-------|-----------------|-------------------|------------|
| Roles | 5 | < 1 KB | Static |
| Users | 50-500 | 50-500 KB | Slow |
| Customers | 1K-10K | 5-50 MB | Medium |
| PaymentTransactions | 5K-50K | 10-100 MB | Fast |
| Clients | 500-5K | 2-20 MB | Slow |
| Appointments | 2K-20K | 5-50 MB | Medium |
| ViewingAppointments | 1K-10K | 2-20 MB | Medium |
| Schedules | 5K-50K | 5-50 MB | Medium |
| OtpVerifications | Auto-cleanup | < 1 MB | Static |
| Properties | 500-5K | 5-50 MB | Slow |
| Brokers | 50-500 | 100 KB-1 MB | Slow |
| Transactions | 1K-10K | 5-50 MB | Slow |

**Total Estimated:** 50-400 MB for typical operation

---

## 🔐 Security Considerations

### Sensitive Data
- **CardNumber, CVV** in Customers: Encrypt before storing
- **PasswordHash** in Users: Use bcrypt/Argon2
- **Email** fields: Consider data masking in logs

### Access Control
- Implement role-based access (Roles table)
- Broker can only see their own customers
- Admin access to all tables

### Audit Trail
- CreatedDate/UpdatedDate on all tables
- Consider adding UpdatedBy columns
- Keep transaction history (don't delete)

---

## 📈 Performance Optimization

### Query Patterns
```sql
-- Frequently used queries
SELECT * FROM Customers WHERE Status = 'Interested' AND BrokerId = @BrokerId
SELECT * FROM PaymentTransactions WHERE Status = 'succeeded' ORDER BY CreatedDate DESC
SELECT * FROM Properties WHERE PropertyType = 'Residential' AND City = @City AND Price BETWEEN @Min AND @Max
SELECT * FROM Appointments WHERE AppointmentDate BETWEEN @Start AND @End
```

### Index Strategy
- All foreign keys indexed
- Status fields indexed (common filter)
- Email indexed (unique + search)
- Date fields indexed (range queries)
- Composite index on (PropertyType, Status) for Properties

---

## 🚀 Implementation Checklist

- [ ] Create Roles table
- [ ] Create Users table
- [ ] Create Customers table
- [ ] Create PaymentTransactions table
- [ ] Create Clients table
- [ ] Create Appointments table
- [ ] Create ViewingAppointments table
- [ ] Create Schedules table
- [ ] Create OtpVerifications table
- [ ] Create Properties table
- [ ] Create Brokers table
- [ ] Create Transactions table
- [ ] Create all indexes
- [ ] Create foreign key relationships
- [ ] Test data integrity
- [ ] Set up backup strategy
- [ ] Configure maintenance jobs
- [ ] Document custom procedures (if any)
- [ ] Setup monitoring & alerts

---

## 📞 Support Tables for Future Features

Consider adding these tables for enhanced functionality:

- **AuditLog** - Track all data changes
- **Documents** - Store contracts and files
- **EmailLogs** - Track sent communications
- **Reports** - Save generated reports
- **Analytics** - Performance metrics
- **Notifications** - User notifications queue
- **WebhookLogs** - PayMongo webhook history
- **APIKeys** - Third-party integrations

---

**Last Updated:** 2026  
**Total Tables:** 12  
**Total Indexes:** 25+  
**Total Foreign Keys:** 8
