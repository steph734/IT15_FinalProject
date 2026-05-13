# EstateFlow Database - Entity Relationship Diagram (ERD)

## Complete Database Structure

```
╔═════════════════════════════════════════════════════════════════════════════╗
║                         AUTHENTICATION & USERS                             ║
╠═════════════════════════════════════════════════════════════════════════════╣
║                                                                             ║
║  ┌──────────────┐                      ┌──────────────┐                    ║
║  │    Roles     │◄─────────────────────│    Users     │                    ║
║  ├──────────────┤        1:M           ├──────────────┤                    ║
║  │ RoleId (PK)  │                      │ UserId (PK)  │                    ║
║  │ RoleType (U) │                      │ FullName     │                    ║
║  │ CreatedAt    │                      │ Email (U,IX) │                    ║
║  └──────────────┘                      │ PasswordHash │                    ║
║                                        │ RoleId (FK)  │                    ║
║                                        │ IsActive (IX)│                    ║
║                                        │ CreatedAt    │                    ║
║                                        │ UpdatedAt    │                    ║
║                                        └──────────────┘                    ║
║                                              ▼ 1:M                         ║
║                                        ┌──────────────┐                    ║
║                                        │  Schedules   │                    ║
║                                        ├──────────────┤                    ║
║                                        │ ScheduleId   │                    ║
║                                        │ UserId (FK)  │                    ║
║                                        │ ScheduleDate │                    ║
║                                        │ StartTime    │                    ║
║                                        │ EndTime      │                    ║
║                                        │ Title        │                    ║
║                                        │ IsRecurring  │                    ║
║                                        └──────────────┘                    ║
║                                                                             ║
║                                        ┌──────────────┐                    ║
║                                        │   Brokers    │                    ║
║                                        ├──────────────┤                    ║
║                                        │ BrokerId (PK)│                    ║
║                                        │ UserId (FK)  │                    ║
║                                        │ CompanyName  │                    ║
║                                        │ CommissionRate                    ║
║                                        │ IsActive (IX)│                    ║
║                                        └──────────────┘                    ║
║                                                                             ║
╚═════════════════════════════════════════════════════════════════════════════╝

╔═════════════════════════════════════════════════════════════════════════════╗
║                      CUSTOMER & PAYMENT MANAGEMENT                         ║
╠═════════════════════════════════════════════════════════════════════════════╣
║                                                                             ║
║  ┌──────────────────────┐                                                  ║
║  │    Customers         │                                                  ║
║  ├──────────────────────┤                                                  ║
║  │ ClientID (PK)        │◄───────────────┐                                ║
║  │ BrokerId (IX)        │                │ 1:M                             ║
║  │                      │                │                                 ║
║  │ Personal Info:       │        ┌───────┴──────────────┐                 ║
║  │ ├─ FullName         │        │                       │                 ║
║  │ ├─ Email (U,IX)     │        │                       ▼                 ║
║  │ ├─ Phone            │        │      ┌────────────────────────┐         ║
║  │ ├─ Address          │        │      │ PaymentTransactions    │         ║
║  │ ├─ City, State      │        │      ├────────────────────────┤         ║
║  │ ├─ ZipCode          │        │      │ Id (PK)                │         ║
║  │ └─ Country          │        │      │ CustomerId (FK) ◄──────┘         ║
║  │                     │        │      │                                   ║
║  │ Property Info:      │        │      │ PayMongoPaymentIntentId          ║
║  │ ├─ PropertyType(IX) │        │      │ PayMongoSourceId                 ║
║  │ ├─ InterestedProps  │        │      │ Amount (DECIMAL)                 ║
║  │ ├─ MinBudget        │        │      │ Currency                         ║
║  │ ├─ MaxBudget        │        │      │ Status (IX)                      ║
║  │ └─ Status (IX)      │        │      │ PaymentMethod                    ║
║  │                     │        │      │ CreatedDate                      ║
║  │ Payment Info:       │        │      │ UpdatedDate                      ║
║  │ ├─ PaymentMethod    │        │      │ WebhookResponse                  ║
║  │ ├─ CardholderName   │        │      │ ErrorMessage                     ║
║  │ ├─ CardNumber       │        │      │ IsProcessed                      ║
║  │ ├─ ExpiryDate       │        │      └────────────────────────┘         ║
║  │ └─ CVV              │        │                                           ║
║  │                     │        │                                           ║
║  │ CreatedDate         │        │                                           ║
║  │ LastContactedDate   │        │                                           ║
║  │ IsActive            │        │                                           ║
║  └──────────────────────┘        │                                           ║
║                                  │                                           ║
║  ┌──────────────────────┐        │                                           ║
║  │     Clients          │◄───────┘                                          ║
║  ├──────────────────────┤                                                   ║
║  │ ClientId (PK)        │                                                   ║
║  │ Name                 │                                                   ║
║  │ Email (IX)           │                                                   ║
║  │ Phone                │                                                   ║
║  │ Address, City, State │                                                   ║
║  │ ClientType           │                                                   ║
║  │ Status (IX)          │                                                   ║
║  │ Notes                │                                                   ║
║  │ CreatedDate          │                                                   ║
║  └──────────────────────┘                                                   ║
║                                                                              ║
║  For OTP Verification:                                                      ║
║  ┌──────────────────────┐                                                   ║
║  │ OtpVerifications     │                                                   ║
║  ├──────────────────────┤                                                   ║
║  │ Id (PK)              │                                                   ║
║  │ Email (IX)           │                                                   ║
║  │ OtpCode              │                                                   ║
║  │ ExpiresAt (IX)       │                                                   ║
║  │ IsVerified           │                                                   ║
║  │ CreatedAt            │                                                   ║
║  └──────────────────────┘                                                   ║
║                                                                              ║
╚═════════════════════════════════════════════════════════════════════════════╝

╔═════════════════════════════════════════════════════════════════════════════╗
║                     APPOINTMENTS & SCHEDULING                              ║
╠═════════════════════════════════════════════════════════════════════════════╣
║                                                                             ║
║      ┌──────────────────┐        ┌───────────────────────┐                ║
║      │  Appointments    │        │ ViewingAppointments   │                ║
║      ├──────────────────┤        ├───────────────────────┤                ║
║      │ AppointmentId(PK)│        │ Id (PK)               │                ║
║      │ ClientId (FK)◄───┼────────│ PropertyId            │                ║
║      │ AgentId          │ 1:M    │ ClientName            │                ║
║      │                  │        │ ClientEmail           │                ║
║      │ AppointmentDate  │        │ ClientPhone           │                ║
║      │ (IX)             │        │                       │                ║
║      │ Duration         │        │ AppointmentDate (IX)  │                ║
║      │ Subject          │        │ Status (IX)           │                ║
║      │ Description      │        │ Notes                 │                ║
║      │ Status (IX)      │        │ CreatedDate           │                ║
║      │ Location         │        │ CreatedBy             │                ║
║      │ Notes            │        └───────────────────────┘                ║
║      │ CreatedDate      │                                                 ║
║      │ CreatedBy        │        Cascade to Clients (1:M)                 ║
║      └──────────────────┘                                                 ║
║             ▲                                                              ║
║             │ 1:M from Clients                                            ║
║             │                                                              ║
║      ┌──────────────────┐                                                 ║
║      │     Clients      │                                                 ║
║      └──────────────────┘                                                 ║
║                                                                             ║
╚═════════════════════════════════════════════════════════════════════════════╝

╔═════════════════════════════════════════════════════════════════════════════╗
║                     PROPERTY & TRANSACTION MANAGEMENT                      ║
╠═════════════════════════════════════════════════════════════════════════════╣
║                                                                             ║
║  ┌─────────────────────────┐                                              ║
║  │     Properties          │                                              ║
║  ├─────────────────────────┤                                              ║
║  │ PropertyId (PK)         │                                              ║
║  │ Title, Description      │                                              ║
║  │ Address, City, State    │                                              ║
║  │ Country, ZipCode        │                                              ║
║  │                         │                                              ║
║  │ Price (DECIMAL) (IX)    │                                              ║
║  │ PropertyType (IX)       │────┐                                         ║
║  │ Bedrooms, Bathrooms     │    │                                         ║
║  │ SquareFeet              │    │ 1:M                                     ║
║  │                         │    │                                         ║
║  │ Status (IX)             │    ├───────────────────────────┐             ║
║  │ ListingDate             │    │                           ▼             ║
║  │ AgentId                 │    │    ┌─────────────────────────────┐     ║
║  │ CreatedDate             │    │    │    Transactions            │     ║
║  │ UpdatedDate             │    │    ├─────────────────────────────┤     ║
║  └─────────────────────────┘    │    │ TransactionId (PK)          │     ║
║                                 └───┤ PropertyId (FK, SET NULL)    │     ║
║                                      │                             │     ║
║                                      │ BuyerId (FK, SET NULL)◄─────┼──┐  ║
║                                      │ SellerId (FK, SET NULL)◄────┼──┼──┼─┐
║                                      │ AgentId                     │  │  │ │
║                                      │                             │  │  │ │
║                                      │ TransactionDate (IX)        │  │  │ │
║                                      │ SalePrice (DECIMAL)         │  │  │ │
║                                      │ Commission (DECIMAL)        │  │  │ │
║                                      │ Status (IX)                 │  │  │ │
║                                      │ Notes                       │  │  │ │
║                                      │ CreatedDate                 │  │  │ │
║                                      └─────────────────────────────┘  │  │ │
║                                            ▲                           │  │ │
║                                            │                           │  │ │
║                                            └───────────────────────────┘  │ │
║                                          (SET NULL on Clients delete)     │ │
║                                                                            │ │
║                                                                            └─┤
║                                                                              │
║                                      ┌──────────────────┐                  │
║                                      │     Clients      │                  │
║                                      ├──────────────────┤                  │
║                                      │ ClientId (PK)    │                  │
║                                      │ Name             │◄─────────────────┘
║                                      │ Email (IX)       │                   
║                                      │ Phone            │                   
║                                      │ Address          │                   
║                                      │ ClientType       │                   
║                                      │ Status (IX)      │                   
║                                      └──────────────────┘                   
║                                                                             ║
╚═════════════════════════════════════════════════════════════════════════════╝
```

---

## Key Legend

```
PK   = Primary Key
FK   = Foreign Key
U    = Unique Constraint
IX   = Index
1:M  = One to Many relationship
M:1  = Many to One relationship

CASCADE   = Delete child records when parent deleted
SET NULL  = Set FK to NULL when parent deleted
RESTRICT  = Prevent delete if children exist
```

---

## Relationship Summary Table

| From | To | Type | Delete Strategy |
|------|----|----|-----------------|
| Roles | Users | 1:M | RESTRICT |
| Users | Schedules | 1:M | CASCADE |
| Users | Brokers | 1:M | CASCADE |
| Customers | PaymentTransactions | 1:M | CASCADE |
| Clients | Appointments | 1:M | CASCADE |
| Clients | Transactions (Buyer) | 1:M | SET NULL |
| Clients | Transactions (Seller) | 1:M | SET NULL |
| Properties | Transactions | 1:M | SET NULL |
| Properties | ViewingAppointments | 1:M | (implicit) |

---

## Data Flow Diagram

```
┌─────────────────┐
│   Broker Login  │
└────────┬────────┘
         │
         ▼
    ┌─────────┐
    │  Users  │ ◄─── Authenticated
    └────┬────┘
         │
    ┌────┴────────────────────────────┐
    │                                 │
    ▼                                 ▼
┌─────────────┐              ┌────────────────┐
│  Dashboard  │              │ Broker Profile │
└────┬────────┘              └────────────────┘
     │
     ├─────► View Customers ──► View Customers List ──► SELECT Customers WHERE BrokerId
     │
     ├─────► Create Customer ─────────────────┐
     │                                         │
     │     Step 1: Personal Details            │
     │     Step 2: Property Info               │
     │     Step 3: Payment Info                │
     │                                         ▼
     │                              INSERT Customers
     │                                         │
     │                                         ▼
     │                              CREATE PaymentTransaction
     │                                         │
     │                                         ▼
     │                           PayMongo API → Payment Intent
     │                                         │
     │                                         ▼
     │                              Payment Confirmation Page
     │                                         │
     │                                         ▼
     │                        User Confirms Payment (Webhook)
     │                                         │
     │                                         ▼
     │                           UPDATE PaymentTransaction.Status
     │
     └─────► View Properties ──► Filter & Search
     │
     └─────► Manage Transactions ──► View Completed Sales
```

---

## Database Normalization

All tables follow **Third Normal Form (3NF)**:

- ✅ No repeating groups
- ✅ All attributes dependent on primary key
- ✅ No transitive dependencies
- ✅ Appropriate foreign key relationships
- ✅ Referential integrity maintained

---

## Scalability Considerations

### Current Design Supports
- Up to 10K customers per broker
- Up to 50K payment transactions
- Multi-broker architecture
- Soft deletes via IsActive flag
- Audit trail via timestamps

### Future Enhancements
- Add AuditLog table for compliance
- Add IndexedSearch table for performance
- Partition large tables by date
- Archive historical data
- Add DocumentStorage table

---

## Query Performance Optimization

### High-Traffic Queries
```
SELECT * FROM Customers WHERE BrokerId = @BrokerId AND Status = 'Interested'
  → Use Index: (BrokerId, Status)

SELECT * FROM PaymentTransactions WHERE Status = 'pending' 
  → Use Index: Status

SELECT * FROM Properties WHERE PropertyType = 'Residential' AND City = @City
  → Use Index: (PropertyType, City)

SELECT * FROM Transactions WHERE TransactionDate BETWEEN @Start AND @End
  → Use Index: TransactionDate
```

### Index Strategy
- **Selective Indexes:** On frequently filtered columns
- **Composite Indexes:** On columns used together in WHERE clauses
- **Foreign Keys:** Always indexed
- **Status/Type Fields:** Always indexed
- **Date Ranges:** Always indexed

---

## Cardinality

| Relationship | Cardinality | Notes |
|---|---|---|
| Roles to Users | 1:5-500 | Few roles, many users |
| Users to Schedules | 1:10-1000 | Many schedules per user |
| Users to Brokers | 1:1 | One user = one broker |
| Brokers to Customers | 1:100-10000 | Many customers per broker |
| Customers to Payments | 1:1-10 | Multiple payments per customer |
| Clients to Appointments | 1:5-50 | Multiple appointments per client |
| Properties to ViewingApp | 1:10-100 | Many viewings per property |
| Properties to Transactions | 1:1 | One transaction per property |

---

## Security Considerations in ERD

```
Sensitive Data Fields:
├─ Customers.CardNumber     → Should be encrypted
├─ Customers.CVV             → Should be encrypted
├─ Customers.ExpiryDate      → Should be encrypted
├─ Users.PasswordHash        → Should use bcrypt/Argon2
└─ OtpVerifications.OtpCode  → Short TTL, encrypted

Access Control:
├─ Users.RoleId              → Controls permissions
├─ Customers.BrokerId        → Row-level security
├─ Brokers.IsActive          → Soft delete
└─ Appointments.CreatedBy    → Audit trail

Data Privacy:
├─ GDPR Compliance           → Right to be forgotten
├─ Audit Logging             → Track all changes
└─ Encryption                → At rest & in transit
```

---

**ERD Version:** 1.0  
**Last Updated:** 2026  
**Database Type:** SQL Server  
**Tables:** 12  
**Relationships:** 8 (Foreign Keys)
