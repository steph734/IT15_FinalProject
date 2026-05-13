# EstateFlow Database - ERD (Entity Relationship Diagram) & Connections

## Complete Table Relationships Map

```
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                          ESTATEFLOW DATABASE RELATIONSHIPS                              │
└─────────────────────────────────────────────────────────────────────────────────────────┘

TIER 1: AUTHENTICATION LAYER
────────────────────────────
┌──────────┐
│  Roles   │
│  --------│
│ RoleID-PK│
└────┬─────┘
     │ 1:M
     │ (RoleID-FK)
     │
┌────▼─────────┐
│    Users     │
│   ---------  │
│  UserID-PK   │
│  RoleID-FK ──┤
└────┬─────────┘
     │ 1:M (Used in multiple tables)
     │
     ├──────────────┬──────────────┬──────────────┬──────────────┐
     │              │              │              │              │
     │              │              │              │              │


TIER 2: CORE MANAGEMENT LAYER
──────────────────────────────
┌──────────────┐         ┌──────────────┐
│   Branch     │         │    Broker    │
│ ----------   │         │ ----------   │
│ BranchID-PK  │         │ BrokerID-PK  │
└──────────────┘         └────┬─────────┘
                               │ 1:M
                               │ (BrokerID-FK)
                               │
                    ┌──────────┼──────────┬──────────┐
                    │          │          │          │
                    ▼          ▼          ▼          ▼


TIER 3: CUSTOMER & PROPERTY LAYER
──────────────────────────────────
┌──────────────┐         ┌──────────────┐         ┌──────────────┐
│   Client     │         │   Property   │         │Accommodation │
│ ----------   │         │ ----------   │         │ ----------   │
│ ClientID-PK  │         │PropertyID-PK │         │AccomID-PK    │
│ BrokerID-FK──┼────────▶│ BrokerID-FK  │◀─────┬──│(Links to     │
│ PaymentID-FK │         │ AccomID-FK ──┼──────┘  │ Property)    │
└────┬─────────┘         │ InvestorID-FK│         └──────────────┘
     │                   └──────────────┘
     │ 1:M                     │
     │ (ClientID-FK)          │ 1:M
     │                        │
     ▼                        ▼


TIER 4: TRANSACTION & FINANCIAL LAYER
──────────────────────────────────────
┌──────────────┐         ┌──────────────┐         ┌──────────────┐
│   Payment    │         │  Commission  │         │    Invoice   │
│ ----------   │         │ ----------   │         │ ----------   │
│ PaymentID-PK │         │CommissionID  │         │ InvoiceID-PK │
│ ClientID-FK──┼────────▶│ BrokerID-FK  │◀────┬──│ BrokerID-FK  │
└──────────────┘         │ ManagerID-FK │     │  │ ClientID-FK  │
                         │TransactionID │     │  │TransactionID │
                         └──────────────┘     │  └──────────────┘
                                              │


TIER 5: SCHEDULING & TRACKING LAYER
────────────────────────────────────
┌──────────────┐         ┌──────────────┐         ┌──────────────┐
│ Appointment  │         │OtpVerification│        │  AuditLogs   │
│ ----------   │         │ ----------    │        │ ----------   │
│AppointmentID │         │  OtpID-PK     │        │  LogsID-PK   │
│ ClientID-FK──┼────────▶│  Email        │        │ UserID-FK    │
│ BrokerID-FK  │         │  OtpCode      │        │ Action       │
└──────────────┘         └───────────────┘        │ Timestamp    │
                                                  │ IP Address   │
                                                  └──────────────┘
```

---

## Detailed Relationship Matrix

### 1. **ROLES ↔ USERS (1:M - One-to-Many)**

```
Relationship Type: Parent-Child
Cardinality: 1 Role : M Users
Foreign Key: Users.RoleID → Roles.RoleID

┌─────────────────────────┐
│     ROLES (1)           │
├─────────────────────────┤
│ RoleID (PK)             │
│ RoleType (Admin,        │
│           Broker,       │
│           Agent,        │
│           Investor,     │
│           Manager,      │
│           Accounting)   │
│ CreatedAt               │
└────────────┬────────────┘
             │ 1:M
             │ (One Role → Many Users)
             │
┌────────────▼──────────────────┐
│     USERS (M)                 │
├───────────────────────────────┤
│ UserID (PK)                   │
│ RoleID (FK) ─────┐            │
│ FullName         │ References│
│ Email            │ Roles     │
│ Password         │           │
│ ConfirmPassword  │           │
└──────────────────┘           │
```

**SQL Relationship:**
```sql
ALTER TABLE Users
ADD CONSTRAINT FK_Users_Roles
FOREIGN KEY (RoleID) REFERENCES Roles(RoleID) ON DELETE RESTRICT;
```

---

### 2. **USERS ↔ MULTIPLE TABLES (1:M)**

```
Users is Referenced By:
├─ Broker (UserID-FK) → Broker Profile
├─ AuditLogs (UserID-FK) → Who made the action
├─ Manager (UserID-FK) → Manager Profile
├─ Commission (ManagerID-FK) → Commission Manager
└─ And Others

Relationship Pattern:
One User can be:
├─ One Broker
├─ One Manager
├─ Multiple Actions logged in AuditLogs
└─ Multiple Commission records
```

---

### 3. **BROKER ↔ CLIENT (1:M)**

```
Relationship Type: Parent-Child
Cardinality: 1 Broker : M Clients
Foreign Key: Client.BrokerID → Broker.BrokerID

┌──────────────────────────┐
│     BROKER (1)           │
├──────────────────────────┤
│ BrokerID (PK)            │
│ UserID (FK)              │
│ BrokerFullName           │
│ LicenseNumber            │
│ Email                    │
│ Address                  │
│ Phone                    │
└────────────┬─────────────┘
             │ 1:M
             │ (One Broker → Many Clients)
             │
┌────────────▼──────────────────┐
│     CLIENT (M)                │
├───────────────────────────────┤
│ ClientID (PK)                 │
│ BrokerID (FK) ────┐           │
│ FullName          │ References│
│ Email             │ Broker    │
│ Phone             │           │
│ Address           │           │
│ PropertyType      │           │
│ Status            │           │
└───────────────────┘           │
```

**SQL Relationship:**
```sql
ALTER TABLE Clients
ADD CONSTRAINT FK_Clients_Broker
FOREIGN KEY (BrokerID) REFERENCES Brokers(BrokerID) ON DELETE SET NULL;
```

---

### 4. **CLIENT ↔ PAYMENT (1:M)**

```
Relationship Type: Parent-Child
Cardinality: 1 Client : M Payments
Foreign Key: Payment.ClientID → Client.ClientID

┌──────────────────────────┐
│     CLIENT (1)           │
├──────────────────────────┤
│ ClientID (PK)            │
│ BrokerID (FK)            │
│ FullName                 │
│ Email                    │
│ Phone                    │
│ Status                   │
└────────────┬─────────────┘
             │ 1:M
             │ (One Client → Many Payments)
             │
┌────────────▼────────────────────┐
│     PAYMENT (M)                 │
├─────────────────────────────────┤
│ PaymentID (PK)                  │
│ ClientID (FK) ──┐               │
│ Amount          │ References    │
│ PaymentMongoID  │ Client        │
│ Status          │               │
│ PaymentMethod   │               │
│ CreatedAt       │               │
└─────────────────┘               │
```

---

### 5. **CLIENT ↔ APPOINTMENT (1:M)**

```
Relationship Type: Parent-Child
Cardinality: 1 Client : M Appointments
Foreign Key: Appointment.ClientID → Client.ClientID

┌──────────────────────────┐
│     CLIENT (1)           │
├──────────────────────────┤
│ ClientID (PK)            │
└────────────┬─────────────┘
             │ 1:M
             │
┌────────────▼────────────────────┐
│     APPOINTMENT (M)             │
├─────────────────────────────────┤
│ AppointmentID (PK)              │
│ ClientID (FK) ──────────────┐   │
│ BrokerID (FK)              │   │
│ Date                       │Refs│
│ Status                     │Cli │
│ Description                │ent │
│ CreatedAt                  │   │
└─────────────────────────────┘   │
```

---

### 6. **BROKER ↔ APPOINTMENT (1:M)**

```
Relationship Type: Parent-Child
Cardinality: 1 Broker : M Appointments
Foreign Key: Appointment.BrokerID → Broker.BrokerID

Represents: Which broker is managing which appointments
```

---

### 7. **BROKER ↔ PROPERTY (1:M)**

```
Relationship Type: Parent-Child
Cardinality: 1 Broker : M Properties
Foreign Key: Property.BrokerID → Broker.BrokerID

┌──────────────────────────┐
│     BROKER (1)           │
├──────────────────────────┤
│ BrokerID (PK)            │
└────────────┬─────────────┘
             │ 1:M
             │
┌────────────▼──────────────────┐
│     PROPERTY (M)              │
├───────────────────────────────┤
│ PropertyID (PK)               │
│ BrokerID (FK) ─┐              │
│ AccomID (FK)   │ References   │
│ InvestorID     │ Broker       │
│ PropertyName   │              │
│ PropertyType   │              │
│ Price          │              │
│ Status         │              │
└───────────────────────────────┘
```

---

### 8. **PROPERTY ↔ ACCOMMODATION (1:1)**

```
Relationship Type: One-to-One
Cardinality: 1 Property : 1 Accommodation
Foreign Key: Property.AccomID → Accommodation.AccomID

┌──────────────────────────┐
│     PROPERTY (1)         │
├──────────────────────────┤
│ PropertyID (PK)          │
│ AccomID (FK) ────────┐   │
│ PropertyName        │1:1 │
│ PropertyType        │Rel │
│ Price               │ationship
└──────────────────────────┘
                      │
┌──────────────────────────┐
│     ACCOMMODATION (1)    │
├──────────────────────────┤
│ AccomID (PK)             │
│ BedroomCount             │
│ BathroomCount            │
│ MaxGuests                │
│ Status                   │
│ CreatedAt                │
└──────────────────────────┘

Meaning: Each Property has ONE Accommodation record with details
```

---

### 9. **PROPERTY ↔ INVESTOR (M:M or 1:M)**

```
Relationship: Properties can be invested by Investors
Foreign Key: Property.InvestorID → Investor.InvestorID (if M:1)
OR Junction Table needed if M:M

Current Implementation (M:1):
One Investor can invest in Many Properties
One Property is invested by One/Many Investors
```

---

### 10. **BROKER ↔ COMMISSION (1:M)**

```
Relationship Type: Parent-Child
Cardinality: 1 Broker : M Commissions
Foreign Key: Commission.BrokerID → Broker.BrokerID

┌──────────────────────────┐
│     BROKER (1)           │
├──────────────────────────┤
│ BrokerID (PK)            │
└────────────┬─────────────┘
             │ 1:M
             │
┌────────────▼──────────────────┐
│     COMMISSION (M)            │
├───────────────────────────────┤
│ CommissionID (PK)             │
│ TransactionID (FK)            │
│ BrokerID (FK) ────────────┐   │
│ ManagerID (FK)           │   │
│ Status                   │Refs│
│ ApprovedDate             │Bro│
│ TargetSales              │ker│
│ Amount                   │   │
│ Rate                     │   │
│ CreatedAt                │   │
└───────────────────────────┘   │
```

---

### 11. **MANAGER ↔ COMMISSION (1:M)**

```
Relationship Type: Parent-Child
Cardinality: 1 Manager : M Commissions
Foreign Key: Commission.ManagerID → Manager.ManagerID

Represents: Managers approve/manage commissions
```

---

### 12. **BROKER ↔ INVOICE (1:M)**

```
Relationship Type: Parent-Child
Cardinality: 1 Broker : M Invoices
Foreign Key: Invoice.BrokerID → Broker.BrokerID

┌──────────────────────────┐
│     BROKER (1)           │
├──────────────────────────┤
│ BrokerID (PK)            │
└────────────┬─────────────┘
             │ 1:M
             │
┌────────────▼──────────────────┐
│     INVOICE (M)               │
├───────────────────────────────┤
│ InvoiceID (PK)                │
│ InvoiceType                   │
│ IssueDate                     │
│ DueDate                       │
│ BrokerID (FK) ─┐              │
│ ClientID (FK)  │ References   │
│ TransactionID  │ Broker       │
│ SubTotal       │              │
│ TaxAmount      │              │
│ CreatedAt      │              │
└───────────────────────────────┘
```

---

### 13. **CLIENT ↔ INVOICE (1:M)**

```
Relationship Type: Parent-Child
Cardinality: 1 Client : M Invoices
Foreign Key: Invoice.ClientID → Client.ClientID

Represents: Each client receives multiple invoices
```

---

### 14. **TRANSACTION ↔ COMMISSION (1:M)**

```
Relationship Type: Parent-Child
Cardinality: 1 Transaction : M Commissions
Foreign Key: Commission.TransactionID → Transaction.TransactionID

Represents: One transaction can generate one or more commissions
```

---

### 15. **TRANSACTION ↔ INVOICE (1:M)**

```
Relationship Type: Parent-Child
Cardinality: 1 Transaction : M Invoices
Foreign Key: Invoice.TransactionID → Transaction.TransactionID

Represents: One transaction creates one or more invoices
```

---

### 16. **USER ↔ AUDITLOGS (1:M)**

```
Relationship Type: Parent-Child
Cardinality: 1 User : M AuditLogs
Foreign Key: AuditLogs.UserID → User.UserID

┌──────────────────────────┐
│     USERS (1)            │
├──────────────────────────┤
│ UserID (PK)              │
└────────────┬─────────────┘
             │ 1:M
             │
┌────────────▼──────────────────┐
│     AUDITLOGS (M)             │
├───────────────────────────────┤
│ LogsID (PK)                   │
│ UserID (FK) ────────┐         │
│ Action              │ Who made│
│ Timestamp           │ the     │
│ IPAddress           │ action  │
└───────────────────────────────┘

Represents: Every action is tracked to a user
```

---

## Complete Relationship Summary Table

| Parent Table | Child Table | Relationship | Foreign Key | Cardinality |
|---|---|---|---|---|
| Roles | Users | User belongs to a role | Users.RoleID | 1:M |
| Users | AuditLogs | User performs actions logged | AuditLogs.UserID | 1:M |
| Broker | Client | Broker manages clients | Client.BrokerID | 1:M |
| Broker | Property | Broker owns properties | Property.BrokerID | 1:M |
| Broker | Commission | Broker earns commissions | Commission.BrokerID | 1:M |
| Broker | Invoice | Broker issues invoices | Invoice.BrokerID | 1:M |
| Client | Payment | Client makes payments | Payment.ClientID | 1:M |
| Client | Appointment | Client has appointments | Appointment.ClientID | 1:M |
| Client | Invoice | Client receives invoices | Invoice.ClientID | 1:M |
| Property | Accommodation | Property has one accommodation | Property.AccomID | 1:1 |
| Manager | Commission | Manager approves commissions | Commission.ManagerID | 1:M |
| Transaction | Commission | Transaction generates commissions | Commission.TransactionID | 1:M |
| Transaction | Invoice | Transaction creates invoices | Invoice.TransactionID | 1:M |
| Appointment | Broker | Appointment with broker | Appointment.BrokerID | M:1 |

---

## SQL Relationship Statements

```sql
-- ROLE → USER (1:M)
ALTER TABLE Users
ADD CONSTRAINT FK_Users_Roles
FOREIGN KEY (RoleID) REFERENCES Roles(RoleID) ON DELETE RESTRICT;

-- BROKER → CLIENT (1:M)
ALTER TABLE Clients
ADD CONSTRAINT FK_Clients_Broker
FOREIGN KEY (BrokerID) REFERENCES Brokers(BrokerID) ON DELETE SET NULL;

-- CLIENT → PAYMENT (1:M)
ALTER TABLE Payments
ADD CONSTRAINT FK_Payment_Client
FOREIGN KEY (ClientID) REFERENCES Clients(ClientID) ON DELETE CASCADE;

-- CLIENT → APPOINTMENT (1:M)
ALTER TABLE Appointments
ADD CONSTRAINT FK_Appointment_Client
FOREIGN KEY (ClientID) REFERENCES Clients(ClientID) ON DELETE CASCADE;

-- BROKER → APPOINTMENT (1:M)
ALTER TABLE Appointments
ADD CONSTRAINT FK_Appointment_Broker
FOREIGN KEY (BrokerID) REFERENCES Brokers(BrokerID) ON DELETE CASCADE;

-- BROKER → PROPERTY (1:M)
ALTER TABLE Properties
ADD CONSTRAINT FK_Property_Broker
FOREIGN KEY (BrokerID) REFERENCES Brokers(BrokerID) ON DELETE CASCADE;

-- PROPERTY ↔ ACCOMMODATION (1:1)
ALTER TABLE Properties
ADD CONSTRAINT FK_Property_Accommodation
FOREIGN KEY (AccomID) REFERENCES Accommodations(AccomID) ON DELETE SET NULL;

-- BROKER → COMMISSION (1:M)
ALTER TABLE Commissions
ADD CONSTRAINT FK_Commission_Broker
FOREIGN KEY (BrokerID) REFERENCES Brokers(BrokerID) ON DELETE CASCADE;

-- MANAGER → COMMISSION (1:M)
ALTER TABLE Commissions
ADD CONSTRAINT FK_Commission_Manager
FOREIGN KEY (ManagerID) REFERENCES Managers(ManagerID) ON DELETE SET NULL;

-- TRANSACTION → COMMISSION (1:M)
ALTER TABLE Commissions
ADD CONSTRAINT FK_Commission_Transaction
FOREIGN KEY (TransactionID) REFERENCES Transactions(TransactionID) ON DELETE CASCADE;

-- BROKER → INVOICE (1:M)
ALTER TABLE Invoices
ADD CONSTRAINT FK_Invoice_Broker
FOREIGN KEY (BrokerID) REFERENCES Brokers(BrokerID) ON DELETE SET NULL;

-- CLIENT → INVOICE (1:M)
ALTER TABLE Invoices
ADD CONSTRAINT FK_Invoice_Client
FOREIGN KEY (ClientID) REFERENCES Clients(ClientID) ON DELETE SET NULL;

-- TRANSACTION → INVOICE (1:M)
ALTER TABLE Invoices
ADD CONSTRAINT FK_Invoice_Transaction
FOREIGN KEY (TransactionID) REFERENCES Transactions(TransactionID) ON DELETE SET NULL;

-- USER → AUDITLOGS (1:M)
ALTER TABLE AuditLogs
ADD CONSTRAINT FK_AuditLogs_User
FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE;
```

---

## Relationship Legend

| Symbol | Meaning |
|---|---|
| **1:M** | One-to-Many (One parent, many children) |
| **1:1** | One-to-One (One parent, one child) |
| **M:M** | Many-to-Many (Requires junction table) |
| **PK** | Primary Key (Unique identifier) |
| **FK** | Foreign Key (References another table) |
| **CASCADE** | Delete children when parent is deleted |
| **SET NULL** | Set foreign key to NULL when parent is deleted |
| **RESTRICT** | Prevent deletion if children exist |

---

## Data Flow Illustration

```
USER REGISTRATION & AUTHENTICATION
┌─────────────┐
│  Role Type  │
└─────┬───────┘
      │
      ▼
┌─────────────────────┐
│ User Creates Account│
│ • Assign Role       │
│ • Set Permissions   │
└─────┬───────────────┘
      │
      ▼
┌──────────────────────────────────┐
│   User Actions Logged            │
│   ├─ Login                       │
│   ├─ Create Client               │
│   ├─ Create Invoice              │
│   └─ Approve Commission          │
└──────────────────────────────────┘
      │
      ▼ (Tracked in)
┌──────────────────────────────────┐
│       AUDIT LOGS                 │
│   • UserID                       │
│   • Action                       │
│   • Timestamp                    │
│   • IP Address                   │
└──────────────────────────────────┘


BUSINESS TRANSACTION FLOW
┌─────────────────┐
│  Broker Posts   │
│   Property      │
└────────┬────────┘
         │
         ▼
┌──────────────────────────┐
│  Client Views Property   │
│  (With Accommodation)    │
└────────┬─────────────────┘
         │
         ▼
┌─────────────────────────────────┐
│  Client Books Appointment       │
│  (Appointment Created)          │
└────────┬────────────────────────┘
         │
         ▼
┌─────────────────────────────────┐
│  Transaction Completed          │
└────────┬────────────────────────┘
         │
         ├─────────────────┬───────────────┐
         │                 │               │
         ▼                 ▼               ▼
    ┌─────────┐     ┌──────────┐   ┌──────────────┐
    │Commission│     │ Invoice  │   │  Payment     │
    │Created  │     │Created   │   │Processed     │
    └─────────┘     └──────────┘   └──────────────┘
         │                 │               │
         └─────────┬───────┴───────────────┘
                   │
                   ▼
         ┌──────────────────────┐
         │  AUDIT LOG ENTRY     │
         │  All actions tracked │
         └──────────────────────┘
```

---

**ERD Status:** ✅ Complete  
**Total Relationships:** 16+  
**Cardinality Types:** 1:M, 1:1, M:M Support  
**Referential Integrity:** Full FK constraints  

