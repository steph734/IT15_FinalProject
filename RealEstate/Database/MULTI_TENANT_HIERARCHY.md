# EstateFlow Database - Multi-Tenant User Hierarchy & Access Control

## User Hierarchy Structure

```
LEVEL 1: SUPER ADMIN
├─ System Administrator
├─ Full system access
├─ Can manage all users and roles
└─ Can access all tables and data

LEVEL 2: SYSTEM ADMIN / MODULE ADMIN
├─ Platform Administrator
├─ Can manage brokers and their data
├─ Can manage system-wide settings
└─ Can view reports and analytics

LEVEL 3: BROKER
├─ Real Estate Broker/Company Owner
├─ Can manage own customers
├─ Can manage own properties
├─ Can manage own transactions
└─ Limited to their broker account data

LEVEL 4: BROKER AGENT / STAFF
├─ Broker's Employee/Agent
├─ Can view broker's customers
├─ Can create and manage listings
├─ Can manage appointments
└─ Limited to assigned properties

LEVEL 5: CLIENT / INVESTOR
├─ Property Buyer/Seller
├─ Can view properties
├─ Can schedule viewings
├─ Can track own transactions
└─ Limited to own account data
```

---

## Database Tables with Multi-Tenant Access Hierarchy

### **TABLE 1: Roles Table**

**Access Hierarchy:**

| Level | Role | Create | Read | Update | Delete | Notes |
|-------|------|--------|------|--------|--------|-------|
| **1** | Super Admin | ✅ Full | ✅ Full | ✅ Full | ✅ Full | System-wide role management |
| **2** | System Admin | ❌ | ✅ View Only | ❌ | ❌ | View assigned roles only |
| **3** | Broker | ❌ | ❌ | ❌ | ❌ | No access |
| **4** | Agent | ❌ | ❌ | ❌ | ❌ | No access |
| **5** | Client | ❌ | ❌ | ❌ | ❌ | No access |

**Tenant Isolation:** Global (System-wide)

---

### **TABLE 2: Users Table**

**Access Hierarchy:**

| Level | Role | Create | Read | Update | Delete | Notes |
|-------|------|--------|------|--------|--------|-------|
| **1** | Super Admin | ✅ Full | ✅ All Users | ✅ Full | ✅ Full | Can manage all system users |
| **2** | System Admin | ✅ Limited | ✅ Assigned Brokers + Staff | ✅ Own Staff | ❌ | Can create broker staff only |
| **3** | Broker | ❌ | ✅ Own Staff Only | ✅ Own Staff | ❌ | Cannot delete users |
| **4** | Agent | ❌ | ✅ Own Profile Only | ✅ Own Profile | ❌ | View/Update own account only |
| **5** | Client | ❌ | ✅ Own Profile Only | ✅ Own Profile | ❌ | View/Update own account only |

**Tenant Isolation:** By BrokerId (Multi-tenant per broker)

---

### **TABLE 3: Customers Table (EXTENDED)**

**Access Hierarchy:**

| Level | Role | Create | Read | Update | Delete | Notes |
|-------|------|--------|------|--------|--------|-------|
| **1** | Super Admin | ✅ Full | ✅ All Customers | ✅ Full | ✅ Full | System-wide customer management |
| **2** | System Admin | ✅ Assigned Brokers | ✅ Assigned Brokers | ✅ Limited | ❌ | Can manage customer data for assigned brokers |
| **3** | Broker | ✅ Own Customers | ✅ Own Customers | ✅ Own Customers | ✅ Soft Delete | Full control of own customer base |
| **4** | Agent | ✅ Broker's List | ✅ Broker's Customers | ✅ Limited | ❌ | Can view and create; limited update |
| **5** | Client | ❌ | ✅ Own Profile Only | ✅ Own Profile | ❌ | Can only view/update own customer record |

**Tenant Isolation:** By BrokerId (Broker-specific customers)

**Tenant Column:** BrokerId

---

### **TABLE 4: PaymentTransactions Table**

**Access Hierarchy:**

| Level | Role | Create | Read | Update | Delete | Notes |
|-------|------|--------|------|--------|--------|-------|
| **1** | Super Admin | ✅ Full | ✅ All Transactions | ✅ Full | ✅ Full | System-wide payment management |
| **2** | System Admin | ❌ | ✅ Assigned Brokers | ✅ Reconciliation | ❌ | View and reconcile transactions |
| **3** | Broker | ❌ | ✅ Own Transactions | ❌ | ❌ | Read-only: View own transaction history |
| **4** | Agent | ❌ | ✅ Assigned Transactions | ❌ | ❌ | View transactions for own customers |
| **5** | Client | ❌ | ✅ Own Transactions | ❌ | ❌ | View own payment records only |

**Tenant Isolation:** By CustomerId → BrokerId (Broker-specific transactions)

**Tenant Column:** CustomerId (via Customers table)

---

### **TABLE 5: Clients Table**

**Access Hierarchy:**

| Level | Role | Create | Read | Update | Delete | Notes |
|-------|------|--------|------|--------|--------|-------|
| **1** | Super Admin | ✅ Full | ✅ All Clients | ✅ Full | ✅ Full | System-wide client management |
| **2** | System Admin | ✅ Assigned Brokers | ✅ Assigned Brokers | ✅ Limited | ❌ | Can manage clients for assigned brokers |
| **3** | Broker | ✅ Own Clients | ✅ Own Clients | ✅ Own Clients | ✅ Soft Delete | Full control of own client list |
| **4** | Agent | ✅ Broker's List | ✅ Broker's Clients | ✅ Limited | ❌ | Can create and view; limited update |
| **5** | Client | ✅ Self-Registration | ✅ Own Profile | ✅ Own Profile | ❌ | Can only manage own account |

**Tenant Isolation:** By Broker (Implicit through Appointments/Transactions)

---

### **TABLE 6: Appointments Table**

**Access Hierarchy:**

| Level | Role | Create | Read | Update | Delete | Notes |
|-------|------|--------|------|--------|--------|-------|
| **1** | Super Admin | ✅ Full | ✅ All Appointments | ✅ Full | ✅ Full | System-wide appointment management |
| **2** | System Admin | ✅ Assigned Brokers | ✅ Assigned Brokers | ✅ Limited | ❌ | View and manage assigned broker appointments |
| **3** | Broker | ✅ Own Appointments | ✅ Own Appointments | ✅ Own Appointments | ✅ Full | Full control of own appointments |
| **4** | Agent | ✅ Own Appointments | ✅ Assigned Appointments | ✅ Own Appointments | ✅ Full | Manage own assigned appointments |
| **5** | Client | ✅ Schedule Viewings | ✅ Own Appointments | ✅ Own Appointments | ❌ | Can schedule/view own appointments |

**Tenant Isolation:** By UserId (Creator) and ClientId (Broker)

**Tenant Column:** CreatedBy

---

### **TABLE 7: ViewingAppointments Table**

**Access Hierarchy:**

| Level | Role | Create | Read | Update | Delete | Notes |
|-------|------|--------|------|--------|--------|-------|
| **1** | Super Admin | ✅ Full | ✅ All Viewings | ✅ Full | ✅ Full | System-wide viewing management |
| **2** | System Admin | ✅ Assigned Brokers | ✅ Assigned Brokers | ✅ Limited | ❌ | Monitor and manage viewings |
| **3** | Broker | ✅ Own Properties | ✅ Own Properties | ✅ Own Viewings | ✅ Full | Full control of own property viewings |
| **4** | Agent | ✅ Assigned Properties | ✅ Assigned Properties | ✅ Own Viewings | ✅ Full | Manage viewings for assigned properties |
| **5** | Client | ✅ Schedule Viewing | ✅ Own Viewing | ✅ Own Viewing | ❌ | Schedule and view own appointments |

**Tenant Isolation:** By PropertyId → BrokerId

**Tenant Column:** PropertyId (via Properties table)

---

### **TABLE 8: Schedules Table**

**Access Hierarchy:**

| Level | Role | Create | Read | Update | Delete | Notes |
|-------|------|--------|------|--------|--------|-------|
| **1** | Super Admin | ✅ Full | ✅ All Schedules | ✅ Full | ✅ Full | System-wide schedule management |
| **2** | System Admin | ❌ | ✅ Assigned Users | ❌ | ❌ | View assigned staff schedules |
| **3** | Broker | ❌ | ✅ Staff Schedules | ❌ | ❌ | View own broker staff schedules |
| **4** | Agent | ✅ Own Schedule | ✅ Own Schedule | ✅ Own Schedule | ✅ Own Schedule | Full control of own schedule |
| **5** | Client | ❌ | ❌ | ❌ | ❌ | No access to schedules |

**Tenant Isolation:** By UserId

**Tenant Column:** UserId

---

### **TABLE 9: OtpVerifications Table**

**Access Hierarchy:**

| Level | Role | Create | Read | Update | Delete | Notes |
|-------|------|--------|------|--------|--------|-------|
| **1** | Super Admin | ✅ Full | ✅ Full | ✅ Full | ✅ Full | System-wide OTP management |
| **2** | System Admin | ✅ Staff Only | ✅ Staff Only | ✅ Limited | ❌ | Can generate OTP for staff |
| **3** | Broker | ✅ Staff Only | ✅ Staff Only | ✅ Limited | ❌ | Can generate OTP for own staff |
| **4** | Agent | ✅ Own OTP | ✅ Own OTP | ❌ | ❌ | Auto-generated for login |
| **5** | Client | ✅ Own OTP | ✅ Own OTP | ❌ | ❌ | Auto-generated for verification |

**Tenant Isolation:** By Email (Implicit through user)

**Tenant Column:** Email

---

### **TABLE 10: Properties Table**

**Access Hierarchy:**

| Level | Role | Create | Read | Update | Delete | Notes |
|-------|------|--------|------|--------|--------|-------|
| **1** | Super Admin | ✅ Full | ✅ All Properties | ✅ Full | ✅ Full | System-wide property management |
| **2** | System Admin | ✅ Assigned Brokers | ✅ Assigned Brokers | ✅ Limited | ❌ | Can manage assigned broker properties |
| **3** | Broker | ✅ Own Properties | ✅ Own Properties | ✅ Own Properties | ✅ Soft Delete | Full control of own listings |
| **4** | Agent | ✅ Assigned Properties | ✅ Assigned Properties | ✅ Assigned Properties | ❌ | Can manage assigned listings |
| **5** | Client | ❌ | ✅ Available Properties | ❌ | ❌ | Can only view available properties |

**Tenant Isolation:** By AgentId → BrokerId

**Tenant Column:** AgentId (Implicit via broker staff)

---

### **TABLE 11: Brokers Table**

**Access Hierarchy:**

| Level | Role | Create | Read | Update | Delete | Notes |
|-------|------|--------|------|--------|--------|-------|
| **1** | Super Admin | ✅ Full | ✅ All Brokers | ✅ Full | ✅ Full | System-wide broker management |
| **2** | System Admin | ✅ Create/Manage | ✅ Assigned Brokers | ✅ Manage | ✅ Deactivate | Can manage broker accounts |
| **3** | Broker | ❌ | ✅ Own Profile | ✅ Own Profile | ❌ | Can only manage own account |
| **4** | Agent | ❌ | ✅ Own Broker | ❌ | ❌ | View own broker info |
| **5** | Client | ❌ | ✅ Assigned Broker | ❌ | ❌ | View broker contact info |

**Tenant Isolation:** By BrokerId (Multi-tenant platform)

**Tenant Column:** BrokerId

---

### **TABLE 12: Transactions Table**

**Access Hierarchy:**

| Level | Role | Create | Read | Update | Delete | Notes |
|-------|------|--------|------|--------|--------|-------|
| **1** | Super Admin | ✅ Full | ✅ All Transactions | ✅ Full | ✅ Full | System-wide transaction management |
| **2** | System Admin | ✅ Assigned Brokers | ✅ Assigned Brokers | ✅ Limited | ❌ | Can manage assigned broker transactions |
| **3** | Broker | ✅ Own Transactions | ✅ Own Transactions | ✅ Limited | ❌ | Can view and manage own sales |
| **4** | Agent | ✅ Assigned Transactions | ✅ Assigned Transactions | ✅ Limited | ❌ | Can manage own sales records |
| **5** | Client | ❌ | ✅ Own Transactions | ❌ | ❌ | Can only view own transaction |

**Tenant Isolation:** By PropertyId → BrokerId / Buyer/Seller Client

**Tenant Column:** PropertyId, BuyerId, SellerId

---

## Access Control Legend

| Symbol | Meaning |
|--------|---------|
| ✅ Full | Full access (Create, Read, Update, Delete) |
| ✅ Limited | Restricted access (Specific rows/columns only) |
| ✅ Soft Delete | Can mark as inactive but not hard delete |
| ✅ View Only | Can read but not modify |
| ✅ Own Only | Can only access own records |
| ✅ Assigned | Can access assigned/managed records |
| ❌ | No access |

---

## Multi-Tenant Implementation Strategy

### **Data Isolation Levels:**

```
LEVEL 1: SUPER ADMIN
└─ Access: ALL DATA
   └─ Sees: All brokers, customers, transactions, users
   └─ Can manage: Everything

LEVEL 2: SYSTEM ADMIN
└─ Access: ASSIGNED BROKERS
   └─ Sees: Only assigned broker data
   └─ Can manage: Assigned brokers + their staff + their data
   └─ Filter Query: WHERE BrokerId IN (assigned_brokers)

LEVEL 3: BROKER
└─ Access: OWN BROKER DATA
   └─ Sees: Only own customers, properties, transactions
   └─ Can manage: Own broker account and all associated data
   └─ Filter Query: WHERE BrokerId = @CurrentBrokerId

LEVEL 4: AGENT/STAFF
└─ Access: ASSIGNED DATA
   └─ Sees: Customers assigned to them, properties assigned
   └─ Can manage: Assigned records only
   └─ Filter Query: WHERE AgentId = @CurrentUserId OR AssignedTo = @CurrentUserId

LEVEL 5: CLIENT/INVESTOR
└─ Access: OWN DATA ONLY
   └─ Sees: Own profile, own transactions, own appointments
   └─ Can manage: Own account only
   └─ Filter Query: WHERE ClientId = @CurrentUserId
```

---

## Row-Level Security (RLS) Implementation

### **SQL Server Row-Level Security Predicates:**

```sql
-- Example: Customers table RLS
CREATE FUNCTION fn_customerPredictor(@BrokerId INT)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN
    SELECT 1 AS is_accessible
    WHERE 
        -- Super Admin (UserId = 1)
        (SELECT RoleId FROM Users WHERE UserId = CONTEXT_PRINCIPAL_ID()) = 1
        OR 
        -- System Admin managing this broker
        @BrokerId IN (SELECT BrokerId FROM BrokerAdminAssignments 
                      WHERE AdminId = CONTEXT_PRINCIPAL_ID())
        OR 
        -- Broker accessing own customers
        @BrokerId = (SELECT BrokerId FROM Brokers 
                    WHERE UserId = CONTEXT_PRINCIPAL_ID());

-- Create RLS Policy
CREATE SECURITY POLICY CustomerRLS
ADD FILTER PREDICATE fn_customerPredictor(BrokerId)
ON dbo.Customers
WITH (STATE = ON);
```

---

## Tenant Column Mapping

| Table | Tenant Column | Isolation Level | Multi-Tenant |
|-------|---------------|-----------------|--------------|
| Roles | Global | System-wide | ❌ No |
| Users | N/A | By UserId/RoleId | ✅ Partial |
| Customers | BrokerId | Per Broker | ✅ Yes |
| PaymentTransactions | CustomerId→BrokerId | Per Broker | ✅ Yes |
| Clients | Implicit (Broker) | Per Broker | ✅ Implicit |
| Appointments | CreatedBy/ClientId | Per Broker | ✅ Yes |
| ViewingAppointments | PropertyId→BrokerId | Per Broker | ✅ Yes |
| Schedules | UserId | Per User | ✅ Yes |
| OtpVerifications | Email | Per User | ✅ Yes |
| Properties | AgentId→BrokerId | Per Broker | ✅ Yes |
| Brokers | BrokerId | Per Broker | ✅ Yes |
| Transactions | PropertyId→BrokerId | Per Broker | ✅ Yes |

---

## Authentication & Authorization Flow

```
USER LOGIN
   ↓
VERIFY CREDENTIALS (Users table)
   ↓
GET USER ROLE (Roles table via RoleId)
   ↓
DETERMINE ACCESS LEVEL (1-5)
   ↓
├─ LEVEL 1: Super Admin → GRANT FULL SYSTEM ACCESS
├─ LEVEL 2: System Admin → FILTER BY ASSIGNED BROKERS
├─ LEVEL 3: Broker → FILTER BY OWN BROKEID
├─ LEVEL 4: Agent → FILTER BY ASSIGNED DATA
└─ LEVEL 5: Client → FILTER BY OWN USERID
   ↓
APPLY ROW-LEVEL SECURITY FILTERS
   ↓
EXECUTE QUERY WITH TENANT FILTERS
   ↓
RETURN FILTERED RESULTS
```

---

## Database Query Examples with Tenant Filtering

### **Get Customers (with Tenant Isolation):**

```sql
-- Super Admin (Level 1)
SELECT * FROM Customers;

-- System Admin (Level 2) - Only assigned brokers
SELECT * FROM Customers 
WHERE BrokerId IN (
    SELECT BrokerId FROM BrokerAdminAssignments 
    WHERE AdminId = @CurrentUserId
);

-- Broker (Level 3) - Only own customers
SELECT * FROM Customers 
WHERE BrokerId = @CurrentBrokerId;

-- Agent (Level 4) - Only broker's customers
SELECT * FROM Customers 
WHERE BrokerId IN (
    SELECT BrokerId FROM Brokers 
    WHERE UserId IN (
        SELECT UserId FROM Users 
        WHERE UserId = @CurrentUserId
    )
);

-- Client (Level 5) - Only own profile
SELECT * FROM Customers 
WHERE ClientID = @CurrentUserId;
```

### **Get Properties (with Tenant Isolation):**

```sql
-- Broker (Level 3) - Only own properties
SELECT * FROM Properties 
WHERE AgentId IN (
    SELECT UserId FROM Users 
    WHERE UserId IN (
        SELECT UserId FROM Brokers 
        WHERE BrokerId = @CurrentBrokerId
    )
);

-- Client (Level 5) - Only available properties
SELECT * FROM Properties 
WHERE Status = 'Available'
AND PropertyId NOT IN (
    SELECT PropertyId FROM ViewingAppointments 
    WHERE Status IN ('Cancelled', 'Completed')
);
```

---

## Access Control Matrix - Full Summary

| Table | Super Admin | System Admin | Broker | Agent | Client |
|-------|-------------|--------------|--------|-------|--------|
| Roles | CRUD | R | - | - | - |
| Users | CRUD | CR (Staff) | R (Own) | R (Own) | R (Own) |
| Customers | CRUD | CRU (Assigned) | CRUD | CRU | RU (Own) |
| PaymentTransactions | CRUD | R | R | R | R (Own) |
| Clients | CRUD | CRU (Assigned) | CRUD | CR | RU (Own) |
| Appointments | CRUD | R | CRUD | CRUD | CRU (Own) |
| ViewingAppointments | CRUD | R | CRUD | CRUD | CRU (Own) |
| Schedules | CRUD | R (Staff) | R (Staff) | CRUD | - |
| OtpVerifications | CRUD | CR (Staff) | CR (Staff) | C (Auto) | C (Auto) |
| Properties | CRUD | CRU (Assigned) | CRUD | CRU | R |
| Brokers | CRUD | CRU | RU (Own) | R | R (Assigned) |
| Transactions | CRUD | CRU (Assigned) | CRU | CRU | R (Own) |

**Legend:** C=Create, R=Read, U=Update, D=Delete, CRU=Create/Read/Update, CRUD=Full Access, R=Read-Only, -=No Access

---

## Security Best Practices

### **1. Authentication:**
- Multi-factor authentication (OTP for sensitive operations)
- Password hashing with bcrypt/Argon2
- Session management with secure tokens
- Auto-logout after inactivity

### **2. Authorization:**
- Role-based access control (RBAC)
- Row-level security (RLS) in database
- Tenant filtering at API/application level
- Regular access reviews

### **3. Data Protection:**
- Encrypt sensitive fields (CardNumber, CVV, PasswordHash)
- TLS/HTTPS for data in transit
- Database encryption at rest
- Regular backups with encryption

### **4. Audit & Compliance:**
- Log all user actions
- Track data access per tenant
- Maintain audit trail with timestamps
- GDPR-compliant data retention

### **5. API Security:**
- JWT tokens with role claims
- Rate limiting per user/tenant
- Input validation on all endpoints
- Output encoding to prevent XSS

---

## Recommended Implementation Approach

### **Phase 1: Authentication (Week 1)**
- Implement user login with role verification
- Set up OTP verification system
- Configure session management

### **Phase 2: Authorization (Week 2)**
- Implement role-based access control
- Add tenant filtering to queries
- Set up row-level security

### **Phase 3: Tenant Isolation (Week 3)**
- Add BrokerId filtering to all queries
- Implement query interceptors
- Test data isolation

### **Phase 4: Security (Week 4)**
- Implement field encryption
- Set up audit logging
- Configure monitoring & alerts

---

## Testing Checklist

### **Access Control Testing:**
- [ ] Super Admin can access all data
- [ ] System Admin sees only assigned brokers
- [ ] Broker sees only own data
- [ ] Agent sees only assigned data
- [ ] Client sees only own data
- [ ] Cross-tenant access is prevented
- [ ] Deleted users cannot access data
- [ ] Deactivated brokers cannot access system

### **Data Isolation Testing:**
- [ ] Broker A cannot see Broker B's customers
- [ ] Agent A cannot see Agent B's appointments
- [ ] Client cannot access other clients' data
- [ ] No SQL injection exploits work
- [ ] RLS policies are enforced

---

**Multi-Tenant Architecture:** ✅ Complete  
**User Hierarchy Levels:** 5 levels defined  
**Tenant Isolation:** Per Broker (BrokerId)  
**Row-Level Security:** SQL RLS ready  
**Access Control:** Matrix defined  
**Status:** Ready for Implementation

