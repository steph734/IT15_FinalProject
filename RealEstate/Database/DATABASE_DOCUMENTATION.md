# EstateFlow Database Schema Documentation

## Overview
Complete database schema for the EstateFlow Real Estate Management System with 12 tables covering authentication, customer management, properties, transactions, and scheduling.

---

## Table Directory

### 1. **Roles Table**
**Purpose:** Store user role types  
**Key Fields:**
- `RoleId` (INT, PK) - Primary Key
- `RoleType` (NVARCHAR(50), UNIQUE) - Role name (Admin, Broker, Agent, Investor)
- `CreatedAt` (DATETIME2) - Creation timestamp

**Records:** 4-5 typical roles
```
Admin, Broker, Agent, Investor, Manager
```

---

### 2. **Users Table**
**Purpose:** Store user account information and authentication data  
**Key Fields:**
- `UserId` (INT, PK)
- `FullName` (NVARCHAR(100))
- `Email` (NVARCHAR(255), UNIQUE) - Login credential
- `PasswordHash` (NVARCHAR(MAX)) - Hashed password
- `RoleId` (INT, FK) - References Roles
- `IsActive` (BIT) - Account status
- `CreatedAt`, `UpdatedAt` (DATETIME2)

**Indexes:** Email, RoleId, IsActive  
**Dependencies:** Roles table

---

### 3. **Customers Table** (EXTENDED)
**Purpose:** Store customer information with payment and property details  
**Key Fields:**
- `ClientID` (INT, PK)
- `BrokerId` (INT) - Associated broker
- **Personal Info:** FullName, Email, Phone, Address, City, State, ZipCode, Country
- **Property Info:** PropertyType, InterestedProperties, MinBudget, MaxBudget, Status
- **Payment Info:** PaymentMethod, CardholderName, CardNumber, ExpiryDate, CVV
- **Metadata:** Notes, CreatedDate, LastContactedDate, IsActive

**Indexes:** Email, Status, PropertyType, BrokerId  
**Data Types:**
- Budget fields: DECIMAL(18,2) for financial values
- Status: 'Interested', 'Follow-up', 'Under Review'
- PropertyType: 'Residential', 'Commercial', 'Industrial'
- PaymentMethod: 'Mastercard', 'Visa', 'Paypal', 'Bank Transfer'

---

### 4. **PaymentTransactions Table**
**Purpose:** Track all payment transactions via PayMongo  
**Key Fields:**
- `Id` (INT, PK)
- `CustomerId` (INT, FK) - References Customers
- `PayMongoPaymentIntentId` (NVARCHAR(MAX)) - PayMongo intent ID
- `PayMongoSourceId` (NVARCHAR(MAX)) - PayMongo source ID
- `Amount` (DECIMAL(18,2)) - Transaction amount
- `Currency` (NVARCHAR(10)) - Default: PHP
- `Status` (NVARCHAR(50)) - pending, succeeded, failed
- `PaymentMethod` (NVARCHAR(50))
- `Description` (NVARCHAR(MAX))
- `CreatedDate`, `UpdatedDate` (DATETIME2)
- `WebhookResponse` (NVARCHAR(MAX)) - PayMongo webhook data
- `ErrorMessage` (NVARCHAR(MAX)) - Error details if failed
- `IsProcessed` (BIT) - Payment confirmation status

**Indexes:** CustomerId, Status, PayMongoPaymentIntentId  
**Dependencies:** Customers table (CASCADE DELETE)

---

### 5. **ViewingAppointments Table**
**Purpose:** Track property viewing appointments  
**Key Fields:**
- `Id` (INT, PK)
- `PropertyId` (INT) - Property being viewed
- `ClientName`, `ClientEmail`, `ClientPhone`
- `AppointmentDate` (DATETIME2)
- `Status` (NVARCHAR(50)) - Pending, Confirmed, Cancelled, Completed
- `Notes` (NVARCHAR(MAX))
- `CreatedDate` (DATETIME2)
- `CreatedBy` (INT) - User who created appointment

**Indexes:** AppointmentDate, Status

---

### 6. **OtpVerifications Table**
**Purpose:** Store one-time password verification records  
**Key Fields:**
- `Id` (INT, PK)
- `Email` (NVARCHAR(255))
- `OtpCode` (NVARCHAR(10))
- `ExpiresAt` (DATETIME2)
- `IsVerified` (BIT)
- `CreatedAt`, `VerifiedAt` (DATETIME2)

**Indexes:** Email, ExpiresAt  
**Retention:** Automatic cleanup of expired OTPs recommended

---

### 7. **Clients Table**
**Purpose:** Store client/customer information (buyers, sellers, investors)  
**Key Fields:**
- `ClientId` (INT, PK)
- `Name`, `Email`, `Phone`
- `Address`, `City`, `State`, `Country`, `ZipCode`
- `ClientType` (NVARCHAR(50)) - Buyer, Seller, Investor
- `Status` (NVARCHAR(50)) - Active, Inactive, Prospect
- `Notes` (NVARCHAR(MAX))
- `CreatedDate`, `UpdatedDate` (DATETIME2)

**Indexes:** Email, Status  
**Relationship:** Separate from Customers table for flexibility

---

### 8. **Appointments Table**
**Purpose:** Track scheduled appointments (meetings, consultations)  
**Key Fields:**
- `AppointmentId` (INT, PK)
- `ClientId`, `AgentId` (INT, FK)
- `AppointmentDate` (DATETIME2)
- `Duration` (INT) - Minutes
- `Subject`, `Description` (NVARCHAR(MAX))
- `Status` (NVARCHAR(50)) - Scheduled, Completed, Cancelled
- `Location` (NVARCHAR(MAX))
- `Notes` (NVARCHAR(MAX))
- `CreatedDate`, `CreatedBy` (DATETIME2, INT)

**Indexes:** AppointmentDate, Status, ClientId  
**Dependencies:** Clients table (CASCADE DELETE)

---

### 9. **Schedules Table**
**Purpose:** Store user calendar schedules  
**Key Fields:**
- `ScheduleId` (INT, PK)
- `UserId` (INT, FK) - References Users
- `ScheduleDate` (DATE)
- `StartTime`, `EndTime` (TIME)
- `Title`, `Description` (NVARCHAR(MAX))
- `Color` (NVARCHAR(50)) - For calendar visualization
- `IsRecurring` (BIT)
- `RecurrencePattern` (NVARCHAR(50)) - Daily, Weekly, Monthly
- `CreatedDate` (DATETIME2)

**Indexes:** UserId, ScheduleDate  
**Dependencies:** Users table (CASCADE DELETE)

---

### 10. **Properties Table**
**Purpose:** Store property listings  
**Key Fields:**
- `PropertyId` (INT, PK)
- `Title`, `Description` (NVARCHAR(MAX))
- `Address`, `City`, `State`, `Country`, `ZipCode`
- `Price` (DECIMAL(18,2))
- `PropertyType` (NVARCHAR(50)) - Residential, Commercial, Industrial
- `Bedrooms`, `Bathrooms` (INT)
- `SquareFeet` (DECIMAL(10,2))
- `Status` (NVARCHAR(50)) - Available, Sold, Rented
- `ListingDate` (DATETIME2)
- `AgentId` (INT)
- `CreatedDate`, `UpdatedDate` (DATETIME2)

**Indexes:** PropertyType, Status, City, Price  
**Search Optimization:** Multiple indexes for filtering

---

### 11. **Brokers Table**
**Purpose:** Store broker information and commissions  
**Key Fields:**
- `BrokerId` (INT, PK)
- `UserId` (INT, FK) - References Users (CASCADE DELETE)
- `CompanyName` (NVARCHAR(MAX))
- `LicenseNumber` (NVARCHAR(50))
- `Phone`, `Email`
- `Address`, `City`, `State`, `Country`, `ZipCode`
- `CommissionRate` (DECIMAL(5,2)) - Percentage (e.g., 5.50%)
- `IsActive` (BIT)
- `CreatedDate` (DATETIME2)

**Indexes:** UserId, IsActive  
**Dependencies:** Users table (CASCADE DELETE)

---

### 12. **Transactions Table**
**Purpose:** Track property transactions and sales  
**Key Fields:**
- `TransactionId` (INT, PK)
- `PropertyId`, `BuyerId`, `SellerId`, `AgentId` (INT, FK)
- `TransactionDate` (DATETIME2)
- `SalePrice`, `Commission` (DECIMAL(18,2))
- `Status` (NVARCHAR(50)) - Pending, Completed, Cancelled
- `Notes` (NVARCHAR(MAX))
- `CreatedDate` (DATETIME2)

**Indexes:** PropertyId, Status, TransactionDate  
**Dependencies:** 
- Properties (SET NULL on delete)
- Clients - BuyerId (SET NULL on delete)
- Clients - SellerId (SET NULL on delete)

---

## Relationships Summary

```
Users (1) ──→ (M) Roles
Users (1) ──→ (M) Schedules
Users (1) ──→ (M) Brokers

Customers (M) ──→ (1) PaymentTransactions (FK: CustomerId)

Clients (1) ──→ (M) Appointments
Clients (1) ──→ (M) Transactions (BuyerId)
Clients (1) ──→ (M) Transactions (SellerId)

Properties (1) ──→ (M) Transactions

Brokers ──→ Customers (via BrokerId)
```

---

## Data Types Used

| Type | Usage | Examples |
|------|-------|----------|
| INT | Primary & Foreign Keys | IDs, Counts |
| NVARCHAR(MAX) | Long text | Descriptions, Notes, Addresses |
| NVARCHAR(255) | Standard text | Emails, Phone numbers |
| NVARCHAR(50) | Status/Type | Statuses, Role types |
| DECIMAL(18,2) | Financial values | Prices, Amounts, Commissions |
| DATETIME2 | Timestamps | Created/Updated dates |
| DATE | Date only | Schedule dates |
| TIME | Time only | Schedule times |
| BIT | Boolean | IsActive, IsVerified, IsRecurring |

---

## Indexes Summary

| Table | Indexes | Purpose |
|-------|---------|---------|
| Users | Email, RoleId, IsActive | Authentication, Role filtering |
| Customers | Email, Status, PropertyType, BrokerId | Customer search & filtering |
| PaymentTransactions | CustomerId, Status, PayMongoId | Payment tracking |
| ViewingAppointments | AppointmentDate, Status | Calendar & filtering |
| OtpVerifications | Email, ExpiresAt | Quick OTP lookup |
| Clients | Email, Status | Client search |
| Appointments | AppointmentDate, Status, ClientId | Calendar & filtering |
| Schedules | UserId, ScheduleDate | User calendar |
| Properties | PropertyType, Status, City, Price | Property search |
| Brokers | UserId, IsActive | Broker lookup |
| Transactions | PropertyId, Status, TransactionDate | Transaction tracking |

---

## Implementation Steps

### Phase 1: Create Base Tables
1. Roles
2. Users
3. OtpVerifications

### Phase 2: Create Customer & Payment Tables
4. Customers
5. PaymentTransactions

### Phase 3: Create Client & Appointment Tables
6. Clients
7. Appointments
8. ViewingAppointments
9. Schedules

### Phase 4: Create Property & Transaction Tables
10. Properties
11. Brokers
12. Transactions

---

## Recommendations

### Backup
- Back up existing database before running scripts
- Test in development/staging first

### Performance
- Monitor slow queries on indexed columns
- Consider partitioning Transactions/PaymentTransactions by date

### Security
- Encrypt CardNumber, CVV in Customers table
- Mask sensitive data in logs
- Implement row-level security for broker data

### Maintenance
- Clean up expired OTP records regularly
- Archive old transactions yearly
- Monitor indexes for fragmentation

### Future Enhancements
- Add audit log table for compliance
- Add document storage table for contracts
- Add email/notification log table
- Add analytics/reporting tables

---

## Quick Start SQL Commands

```sql
-- To view all tables
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'dbo';

-- To view table structure
EXEC sp_help 'Customers';

-- To check indexes
SELECT * FROM sys.indexes 
WHERE object_id = OBJECT_ID('Customers');

-- To check relationships
SELECT CONSTRAINT_NAME, TABLE_NAME, REFERENCED_TABLE_NAME
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE REFERENCED_TABLE_NAME IS NOT NULL;
```

---

## Total Statistics

- **Total Tables:** 12
- **Total Indexes:** 25+
- **Primary Keys:** 12
- **Foreign Keys:** 8
- **Default Values:** 15+
- **Unique Constraints:** 2 (Email, RoleType)

---

## File Location
The complete SQL script is available at:
`RealEstate/Database/EstateFlow_Database_Schema.sql`

To apply the schema:
1. Open SQL Server Management Studio
2. Open the SQL file
3. Review all table definitions
4. Execute the script on your development database
5. Run Entity Framework migrations if applicable
