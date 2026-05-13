# EstateFlow Database - Complete Table Listing

## SUPER ADMIN

---

### 1. Roles Table
| Field Names | Datatype | Length | Description |
|---|---|---|---|
| RoleId-PK | Int-AI | 9 | Role's ID Number |
| RoleType-U | NVarChar | 50 | Role Type (Admin, Broker, Agent, Investor, Manager) |
| CreatedAt | DateTime2 | - | Role Creation Timestamp |

---

### 2. Users Table
| Field Names | Datatype | Length | Description |
|---|---|---|---|
| UserId-PK | Int-AI | 9 | User's ID Number |
| FullName-Required | NVarChar | 100 | User's Full Name |
| Email-U-Required | NVarChar | 255 | User's Email Address (Unique) |
| PasswordHash-Required | NVarChar | Max | User's Password Hash (Encrypted) |
| RoleId-FK | Int | 9 | Foreign Key to Roles Table |
| IsActive | Bit | 1 | User Account Status (1=Active, 0=Inactive) |
| CreatedAt | DateTime2 | - | Account Creation Timestamp |
| UpdatedAt | DateTime2 | - | Account Last Updated Timestamp |

---

## CUSTOMER MANAGEMENT

---

### 3. Customers Table (EXTENDED)
| Field Names | Datatype | Length | Description |
|---|---|---|---|
| ClientID-PK | Int-AI | 9 | Customer's ID Number |
| BrokerId-FK | Int | 9 | Associated Broker ID |
| **PERSONAL INFORMATION** | | | |
| FullName-Required | NVarChar | Max | Customer's Full Name |
| Email-Required-U | NVarChar | 255 | Customer's Email Address (Unique) |
| Phone | NVarChar | 20 | Customer's Phone Number |
| Address | NVarChar | Max | Customer's Street Address |
| City | NVarChar | 100 | Customer's City |
| State | NVarChar | 100 | Customer's State/Province |
| ZipCode | NVarChar | 20 | Customer's Zip/Postal Code |
| Country | NVarChar | 100 | Customer's Country |
| **PROPERTY INFORMATION** | | | |
| PropertyType | NVarChar | 50 | Property Type (Residential/Commercial/Industrial) |
| InterestedProperties | NVarChar | Max | List of Interested Properties |
| MinBudget | Decimal | 18,2 | Minimum Budget (PHP) |
| MaxBudget | Decimal | 18,2 | Maximum Budget (PHP) |
| Status | NVarChar | 50 | Customer Status (Interested/Follow-up/Under Review) |
| **PAYMENT INFORMATION** | | | |
| PaymentMethod | NVarChar | 50 | Payment Method (Mastercard/Visa/PayPal/Bank Transfer) |
| CardholderName | NVarChar | 100 | Cardholder's Full Name |
| CardNumber | NVarChar | Max | Card Number (Encrypted) |
| ExpiryDate | NVarChar | 10 | Card Expiry Date (MM/YY) |
| CVV | NVarChar | 10 | Card CVV (Encrypted) |
| **METADATA** | | | |
| Notes | NVarChar | Max | Additional Customer Notes |
| CreatedDate | DateTime2 | - | Record Creation Date |
| LastContactedDate | DateTime2 | - | Last Contact Date |
| IsActive | Bit | 1 | Customer Status (1=Active, 0=Inactive) |

---

### 4. PaymentTransactions Table
| Field Names | Datatype | Length | Description |
|---|---|---|---|
| Id-PK | Int-AI | 9 | Transaction's ID Number |
| CustomerId-FK-Required | Int | 9 | Foreign Key to Customers Table |
| PayMongoPaymentIntentId | NVarChar | Max | PayMongo Payment Intent ID |
| PayMongoSourceId | NVarChar | Max | PayMongo Card Source ID |
| Amount-Required | Decimal | 18,2 | Transaction Amount (PHP) |
| Currency | NVarChar | 10 | Currency Code (Default: PHP) |
| Status | NVarChar | 50 | Payment Status (pending/succeeded/failed) |
| PaymentMethod | NVarChar | 50 | Payment Method Type |
| Description | NVarChar | Max | Transaction Description |
| CreatedDate | DateTime2 | - | Transaction Creation Date |
| UpdatedDate | DateTime2 | - | Transaction Last Updated Date |
| WebhookResponse | NVarChar | Max | PayMongo Webhook Response Data |
| ErrorMessage | NVarChar | Max | Error Message if Payment Failed |
| IsProcessed | Bit | 1 | Payment Processed Status (1=Yes, 0=No) |

---

### 5. Clients Table
| Field Names | Datatype | Length | Description |
|---|---|---|---|
| ClientId-PK | Int-AI | 9 | Client's ID Number |
| Name-Required | NVarChar | 100 | Client's Full Name |
| Email | NVarChar | 255 | Client's Email Address |
| Phone | NVarChar | 20 | Client's Phone Number |
| Address | NVarChar | Max | Client's Street Address |
| City | NVarChar | 100 | Client's City |
| State | NVarChar | 100 | Client's State/Province |
| Country | NVarChar | 100 | Client's Country |
| ZipCode | NVarChar | 20 | Client's Zip/Postal Code |
| ClientType | NVarChar | 50 | Client Type (Buyer/Seller/Investor) |
| Status | NVarChar | 50 | Client Status (Active/Inactive/Prospect) |
| Notes | NVarChar | Max | Additional Client Notes |
| CreatedDate | DateTime2 | - | Record Creation Date |
| UpdatedDate | DateTime2 | - | Record Last Updated Date |

---

## APPOINTMENTS & SCHEDULING

---

### 6. Appointments Table
| Field Names | Datatype | Length | Description |
|---|---|---|---|
| AppointmentId-PK | Int-AI | 9 | Appointment's ID Number |
| ClientId-FK | Int | 9 | Foreign Key to Clients Table |
| AgentId | Int | 9 | Agent ID (Associated Agent) |
| AppointmentDate-Required | DateTime2 | - | Scheduled Appointment Date/Time |
| Duration | Int | 9 | Appointment Duration (in minutes) |
| Subject | NVarChar | Max | Appointment Subject/Title |
| Description | NVarChar | Max | Appointment Description/Details |
| Status | NVarChar | 50 | Appointment Status (Scheduled/Completed/Cancelled) |
| Location | NVarChar | Max | Meeting Location |
| Notes | NVarChar | Max | Additional Appointment Notes |
| CreatedDate | DateTime2 | - | Record Creation Date |
| CreatedBy | Int | 9 | User ID Who Created This Record |

---

### 7. ViewingAppointments Table
| Field Names | Datatype | Length | Description |
|---|---|---|---|
| Id-PK | Int-AI | 9 | Viewing Appointment's ID Number |
| PropertyId | Int | 9 | Property ID Being Viewed |
| ClientName-Required | NVarChar | 100 | Prospective Client's Name |
| ClientEmail | NVarChar | 255 | Prospective Client's Email |
| ClientPhone | NVarChar | 20 | Prospective Client's Phone |
| AppointmentDate-Required | DateTime2 | - | Scheduled Viewing Date/Time |
| Status | NVarChar | 50 | Viewing Status (Pending/Confirmed/Cancelled/Completed) |
| Notes | NVarChar | Max | Additional Viewing Notes |
| CreatedDate | DateTime2 | - | Record Creation Date |
| CreatedBy | Int | 9 | User ID Who Created This Record |

---

### 8. Schedules Table
| Field Names | Datatype | Length | Description |
|---|---|---|---|
| ScheduleId-PK | Int-AI | 9 | Schedule's ID Number |
| UserId-FK-Required | Int | 9 | Foreign Key to Users Table |
| ScheduleDate-Required | Date | - | Date of Scheduled Event |
| StartTime-Required | Time | - | Event Start Time |
| EndTime-Required | Time | - | Event End Time |
| Title | NVarChar | Max | Schedule Event Title |
| Description | NVarChar | Max | Schedule Event Description |
| Color | NVarChar | 50 | Color Code for Calendar Display |
| IsRecurring | Bit | 1 | Recurring Event Flag (1=Yes, 0=No) |
| RecurrencePattern | NVarChar | 50 | Recurrence Pattern (Daily/Weekly/Monthly) |
| CreatedDate | DateTime2 | - | Record Creation Date |

---

### 9. OtpVerifications Table
| Field Names | Datatype | Length | Description |
|---|---|---|---|
| Id-PK | Int-AI | 9 | OTP Verification's ID Number |
| Email-Required | NVarChar | 255 | Email Address to Verify |
| OtpCode-Required | NVarChar | 10 | One-Time Password Code |
| ExpiresAt-Required | DateTime2 | - | OTP Expiration Date/Time |
| IsVerified | Bit | 1 | Verification Status (1=Verified, 0=Pending) |
| CreatedAt | DateTime2 | - | OTP Creation Timestamp |
| VerifiedAt | DateTime2 | - | OTP Verification Timestamp |

---

## PROPERTY MANAGEMENT

---

### 10. Properties Table
| Field Names | Datatype | Length | Description |
|---|---|---|---|
| PropertyId-PK | Int-AI | 9 | Property's ID Number |
| Title-Required | NVarChar | Max | Property Title/Name |
| Description | NVarChar | Max | Property Description/Details |
| Address-Required | NVarChar | Max | Property Street Address |
| City | NVarChar | 100 | Property City |
| State | NVarChar | 100 | Property State/Province |
| Country | NVarChar | 100 | Property Country |
| ZipCode | NVarChar | 20 | Property Zip/Postal Code |
| Price | Decimal | 18,2 | Property Price (PHP) |
| PropertyType | NVarChar | 50 | Property Type (Residential/Commercial/Industrial) |
| Bedrooms | Int | 9 | Number of Bedrooms |
| Bathrooms | Int | 9 | Number of Bathrooms |
| SquareFeet | Decimal | 10,2 | Property Size (Square Feet) |
| Status | NVarChar | 50 | Listing Status (Available/Sold/Rented) |
| ListingDate | DateTime2 | - | Date Property Listed |
| AgentId | Int | 9 | Agent ID (Listing Agent) |
| CreatedDate | DateTime2 | - | Record Creation Date |
| UpdatedDate | DateTime2 | - | Record Last Updated Date |

---

### 11. Brokers Table
| Field Names | Datatype | Length | Description |
|---|---|---|---|
| BrokerId-PK | Int-AI | 9 | Broker's ID Number |
| UserId-FK-Required | Int | 9 | Foreign Key to Users Table (CASCADE) |
| CompanyName | NVarChar | Max | Broker's Company Name |
| LicenseNumber | NVarChar | 50 | Broker License Number |
| Phone | NVarChar | 20 | Broker's Phone Number |
| Email | NVarChar | 255 | Broker's Email Address |
| Address | NVarChar | Max | Broker's Office Address |
| City | NVarChar | 100 | Broker's Office City |
| State | NVarChar | 100 | Broker's Office State/Province |
| Country | NVarChar | 100 | Broker's Office Country |
| ZipCode | NVarChar | 20 | Broker's Office Zip/Postal Code |
| CommissionRate | Decimal | 5,2 | Broker Commission Rate (Percentage) |
| IsActive | Bit | 1 | Broker Status (1=Active, 0=Inactive) |
| CreatedDate | DateTime2 | - | Record Creation Date |

---

## TRANSACTION TRACKING

---

### 12. Transactions Table
| Field Names | Datatype | Length | Description |
|---|---|---|---|
| TransactionId-PK | Int-AI | 9 | Transaction's ID Number |
| PropertyId-FK | Int | 9 | Foreign Key to Properties Table (SET NULL) |
| BuyerId-FK | Int | 9 | Foreign Key to Clients Table - Buyer (SET NULL) |
| SellerId-FK | Int | 9 | Foreign Key to Clients Table - Seller (SET NULL) |
| AgentId | Int | 9 | Agent ID Handling Transaction |
| TransactionDate-Required | DateTime2 | - | Transaction Date |
| SalePrice | Decimal | 18,2 | Sale Price (PHP) |
| Commission | Decimal | 18,2 | Commission Amount (PHP) |
| Status | NVarChar | 50 | Transaction Status (Pending/Completed/Cancelled) |
| Notes | NVarChar | Max | Additional Transaction Notes |
| CreatedDate | DateTime2 | - | Record Creation Date |

---

## LEGEND

### Field Name Notation
- **-PK** = Primary Key
- **-FK** = Foreign Key
- **-U** = Unique Constraint
- **-AI** = Auto Increment
- **-Required** = NOT NULL constraint

### Data Types
- **Int** = Integer (whole numbers)
- **Int-AI** = Auto-Incrementing Integer
- **NVarChar(n)** = Variable-length Unicode string (n = max length)
- **NVarChar(Max)** = Variable-length Unicode string (up to 8000+ characters)
- **Decimal(p,s)** = Decimal number (p=total digits, s=decimal places)
- **DateTime2** = Date and Time with millisecond precision
- **Date** = Date only
- **Time** = Time only
- **Bit** = Boolean (1 or 0)

### Constraints
- **CASCADE** = Delete child records when parent deleted
- **SET NULL** = Set FK to NULL when parent deleted
- **RESTRICT** = Prevent deletion if children exist
- **UNIQUE** = No duplicate values allowed

---

## SUMMARY STATISTICS

| Metric | Value |
|--------|-------|
| **Total Tables** | 12 |
| **Total Fields** | 150+ |
| **Primary Keys** | 12 |
| **Foreign Keys** | 8 |
| **Unique Constraints** | 2 |
| **Indexes** | 25+ |
| **Auto-Increment Fields** | 12 |

---

## FIELD TYPES SUMMARY

| Type | Count | Purpose |
|------|-------|---------|
| **Int-AI (PK)** | 12 | Primary Key Auto-Increment |
| **NVarChar** | 80+ | Text fields |
| **Decimal** | 10+ | Financial values |
| **DateTime2** | 15+ | Timestamps |
| **Date/Time** | 5+ | Date/Time specific |
| **Bit** | 5+ | Boolean flags |

---

## RELATIONSHIPS SUMMARY

| From Table | To Table | Type | Strategy |
|---|---|---|---|
| Roles | Users | 1:M | One role to many users |
| Users | Schedules | 1:M | CASCADE |
| Users | Brokers | 1:M | CASCADE |
| Customers | PaymentTransactions | 1:M | CASCADE |
| Clients | Appointments | 1:M | CASCADE |
| Clients | Transactions | 1:M | SET NULL |
| Properties | Transactions | 1:M | SET NULL |
| Properties | ViewingAppointments | 1:M | Implicit |

---

**Database Name:** DB_Real_Estate  
**Total Tables:** 12  
**Total Fields:** 150+  
**Status:** Production Ready  
**Last Updated:** 2026

