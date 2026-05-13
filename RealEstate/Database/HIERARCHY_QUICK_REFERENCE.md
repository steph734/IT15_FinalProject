# EstateFlow Database - User Hierarchy Quick Reference

## 5-Level User Hierarchy Structure

```
┌─────────────────────────────────────────────────────────────────────┐
│                     LEVEL 1: SUPER ADMIN                            │
│                    (System Administrator)                           │
│  Access: 100% of all data | Tables: All 12 | Operations: CRUD      │
│  ├─ Manage all roles and users                                      │
│  ├─ Manage all brokers (create, edit, delete, deactivate)          │
│  ├─ View all customers, transactions, properties                   │
│  ├─ View all appointments and schedules                            │
│  ├─ Generate system reports and analytics                          │
│  └─ Configure system settings and security                         │
└─────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────┐
│                  LEVEL 2: SYSTEM ADMIN                              │
│              (Platform Administrator)                              │
│ Access: Assigned brokers only | Operations: CRU (Limited)          │
│  ├─ Manage assigned broker accounts                                │
│  ├─ View assigned broker's data (customers, properties, trans.)   │
│  ├─ Create broker staff accounts                                  │
│  ├─ Monitor assigned broker transactions                          │
│  ├─ Generate broker-specific reports                              │
│  └─ Cannot delete brokers (soft-delete only)                      │
└─────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────┐
│                      LEVEL 3: BROKER                                │
│              (Real Estate Company Owner)                           │
│  Access: Own broker data only | Operations: CRUD                   │
│  ├─ Manage own company/broker profile                              │
│  ├─ Manage own customer list (create, edit, soft-delete)          │
│  ├─ Create and manage property listings                            │
│  ├─ View own transactions and sales                                │
│  ├─ Manage own broker staff (agents)                               │
│  ├─ View own payment transactions                                  │
│  └─ Cannot access other brokers' data                              │
└─────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────┐
│                   LEVEL 4: BROKER AGENT                             │
│              (Broker's Employee/Sales Agent)                       │
│  Access: Assigned data only | Operations: CRU (Limited)            │
│  ├─ View broker's customer list                                    │
│  ├─ Create customers (from broker's list)                          │
│  ├─ Manage assigned property listings                              │
│  ├─ Create and manage own appointments                             │
│  ├─ Schedule property viewings                                     │
│  ├─ View assigned transactions                                     │
│  ├─ Manage own schedule/calendar                                   │
│  └─ Cannot delete/modify other agents' data                        │
└─────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────┐
│                   LEVEL 5: CLIENT/INVESTOR                          │
│              (Property Buyer/Seller/Investor)                      │
│   Access: Own data only | Operations: RU (Read/Update Own)         │
│  ├─ View own profile information                                   │
│  ├─ Schedule property viewings                                     │
│  ├─ View available properties                                      │
│  ├─ Track own transactions                                         │
│  ├─ Manage own appointments                                        │
│  ├─ Self-register as client                                        │
│  └─ Cannot access other clients' data                              │
└─────────────────────────────────────────────────────────────────────┘
```

---

## Access Matrix by Table

### **Color Legend:**
- 🟢 **FULL** = Full CRUD access
- 🟡 **LIMITED** = Restricted access (specific rows/columns)
- 🔵 **READ-ONLY** = Read access only
- ⚪ **OWN ONLY** = Can only access own records
- ⚫ **NONE** = No access

---

## Table Access Grid

```
TABLE LEVEL ANALYSIS:

┌────────────────────────────────────────────────────────────────────┐
│ Roles Table                                                         │
├─────────────────────────────────────────────────────────────────┬──┤
│ LEVEL 1: Super Admin      │ 🟢 FULL  (Create/Read/Update/Delete) │
│ LEVEL 2: System Admin     │ 🔵 READ  (View roles only)           │
│ LEVEL 3: Broker           │ ⚫ NONE                               │
│ LEVEL 4: Agent            │ ⚫ NONE                               │
│ LEVEL 5: Client           │ ⚫ NONE                               │
└────────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────────┐
│ Users Table                                                         │
├─────────────────────────────────────────────────────────────────┬──┤
│ LEVEL 1: Super Admin      │ 🟢 FULL  (All system users)          │
│ LEVEL 2: System Admin     │ 🟡 LIMITED (Assigned brokers' staff) │
│ LEVEL 3: Broker           │ ⚪ OWN   (Own staff only)            │
│ LEVEL 4: Agent            │ ⚪ OWN   (Own profile only)          │
│ LEVEL 5: Client           │ ⚪ OWN   (Own profile only)          │
└────────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────────┐
│ Customers Table (EXTENDED with Payment Info)                       │
├─────────────────────────────────────────────────────────────────┬──┤
│ LEVEL 1: Super Admin      │ 🟢 FULL  (All customers)             │
│ LEVEL 2: System Admin     │ 🟡 LIMITED (Assigned brokers)        │
│ LEVEL 3: Broker           │ 🟢 FULL  (Own customers only)        │
│ LEVEL 4: Agent            │ 🟡 LIMITED (View + Limited update)   │
│ LEVEL 5: Client           │ ⚪ OWN   (Own profile only)          │
└────────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────────┐
│ PaymentTransactions Table                                           │
├─────────────────────────────────────────────────────────────────┬──┤
│ LEVEL 1: Super Admin      │ 🟢 FULL  (All transactions)          │
│ LEVEL 2: System Admin     │ 🔵 READ  (Assigned brokers only)     │
│ LEVEL 3: Broker           │ 🔵 READ  (Own transactions only)     │
│ LEVEL 4: Agent            │ 🔵 READ  (Assigned transactions)     │
│ LEVEL 5: Client           │ 🔵 READ  (Own transactions only)     │
└────────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────────┐
│ Clients Table                                                       │
├─────────────────────────────────────────────────────────────────┬──┤
│ LEVEL 1: Super Admin      │ 🟢 FULL  (All clients)               │
│ LEVEL 2: System Admin     │ 🟡 LIMITED (Assigned brokers)        │
│ LEVEL 3: Broker           │ 🟢 FULL  (Own clients only)          │
│ LEVEL 4: Agent            │ 🟡 LIMITED (View + Create)           │
│ LEVEL 5: Client           │ ⚪ OWN   (Own profile + self-reg)    │
└────────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────────┐
│ Appointments Table                                                  │
├─────────────────────────────────────────────────────────────────┬──┤
│ LEVEL 1: Super Admin      │ 🟢 FULL  (All appointments)          │
│ LEVEL 2: System Admin     │ 🟡 LIMITED (Assigned brokers)        │
│ LEVEL 3: Broker           │ 🟢 FULL  (Own appointments)          │
│ LEVEL 4: Agent            │ 🟢 FULL  (Assigned appointments)     │
│ LEVEL 5: Client           │ ⚪ OWN   (Own appointments)          │
└────────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────────┐
│ ViewingAppointments Table                                           │
├─────────────────────────────────────────────────────────────────┬──┤
│ LEVEL 1: Super Admin      │ 🟢 FULL  (All viewings)              │
│ LEVEL 2: System Admin     │ 🟡 LIMITED (Assigned brokers)        │
│ LEVEL 3: Broker           │ 🟢 FULL  (Own property viewings)     │
│ LEVEL 4: Agent            │ 🟢 FULL  (Assigned property viewings)│
│ LEVEL 5: Client           │ ⚪ OWN   (Own viewings only)         │
└────────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────────┐
│ Schedules Table                                                     │
├─────────────────────────────────────────────────────────────────┬──┤
│ LEVEL 1: Super Admin      │ 🟢 FULL  (All schedules)             │
│ LEVEL 2: System Admin     │ 🔵 READ  (Assigned staff schedules)  │
│ LEVEL 3: Broker           │ 🔵 READ  (Own staff schedules)       │
│ LEVEL 4: Agent            │ ⚪ OWN   (Own schedule only)         │
│ LEVEL 5: Client           │ ⚫ NONE                               │
└────────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────────┐
│ OtpVerifications Table                                              │
├─────────────────────────────────────────────────────────────────┬──┤
│ LEVEL 1: Super Admin      │ 🟢 FULL  (All OTP records)           │
│ LEVEL 2: System Admin     │ 🟡 LIMITED (Generate for staff)      │
│ LEVEL 3: Broker           │ 🟡 LIMITED (Generate for staff)      │
│ LEVEL 4: Agent            │ ⚪ OWN   (Auto-generated on login)   │
│ LEVEL 5: Client           │ ⚪ OWN   (Auto-generated on verify)  │
└────────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────────┐
│ Properties Table                                                    │
├─────────────────────────────────────────────────────────────────┬──┤
│ LEVEL 1: Super Admin      │ 🟢 FULL  (All properties)            │
│ LEVEL 2: System Admin     │ 🟡 LIMITED (Assigned brokers)        │
│ LEVEL 3: Broker           │ 🟢 FULL  (Own listings only)         │
│ LEVEL 4: Agent            │ 🟡 LIMITED (Assigned properties)     │
│ LEVEL 5: Client           │ 🔵 READ  (Available properties only) │
└────────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────────┐
│ Brokers Table                                                       │
├─────────────────────────────────────────────────────────────────┬──┤
│ LEVEL 1: Super Admin      │ 🟢 FULL  (Create/Edit/Delete)        │
│ LEVEL 2: System Admin     │ 🟡 LIMITED (Create/Edit assigned)    │
│ LEVEL 3: Broker           │ ⚪ OWN   (Edit own profile only)     │
│ LEVEL 4: Agent            │ 🔵 READ  (View own broker info)      │
│ LEVEL 5: Client           │ 🔵 READ  (View assigned broker only) │
└────────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────────┐
│ Transactions Table                                                  │
├─────────────────────────────────────────────────────────────────┬──┤
│ LEVEL 1: Super Admin      │ 🟢 FULL  (All transactions)          │
│ LEVEL 2: System Admin     │ 🟡 LIMITED (Assigned brokers)        │
│ LEVEL 3: Broker           │ 🟡 LIMITED (Own transactions)        │
│ LEVEL 4: Agent            │ 🟡 LIMITED (Assigned transactions)   │
│ LEVEL 5: Client           │ 🔵 READ  (Own transaction only)      │
└────────────────────────────────────────────────────────────────────┘
```

---

## Tenant Isolation Strategy

```
┌─────────────────────────────────────────────────┐
│      MULTI-TENANT DATA ISOLATION MODEL          │
└─────────────────────────────────────────────────┘

LEVEL 1: Super Admin (No Filtering)
├─ Can access all data across all brokers
├─ No WHERE clause restrictions
└─ Full database visibility

LEVEL 2: System Admin (Filtered by BrokerId)
├─ Can access only assigned brokers' data
├─ WHERE BrokerId IN (assigned_brokers)
└─ Limited to managed brokers

LEVEL 3: Broker (Filtered by Own BrokerId)
├─ Can access only own broker data
├─ WHERE BrokerId = @CurrentBrokerId
└─ Complete isolation from other brokers

LEVEL 4: Agent/Staff (Filtered by UserId + BrokerId)
├─ Can access own broker's data
├─ WHERE BrokerId = @CurrentBrokerId AND (AgentId = @UserId OR CreatedBy = @UserId)
└─ Assigned records only

LEVEL 5: Client/Investor (Filtered by UserId/ClientId)
├─ Can access only own profile/records
├─ WHERE ClientId = @CurrentUserId OR CreatedBy = @CurrentUserId
└─ Complete privacy isolation

TENANT ISOLATION ENFORCEMENT:
├─ Row-Level Security (RLS) in Database
├─ Application-level filtering in API
├─ Query parameter validation
└─ Audit logging of all access
```

---

## Quick Access Reference

| User Level | Tables Accessible | Primary Operation | Filtering |
|------------|------------------|-------------------|-----------|
| **1** | 12/12 (100%) | System Admin | None |
| **2** | 10/12 (83%) | Broker Admin | By BrokerId |
| **3** | 8/12 (67%) | Business Owner | By own BrokerId |
| **4** | 7/12 (58%) | Sales Agent | By Assignment |
| **5** | 5/12 (42%) | End User | By own ID |

---

## Data Visibility Example

### **Scenario: 3 Brokers (A, B, C) with Customers**

```
Broker A: Customers [C1, C2, C3, C4, C5]
Broker B: Customers [C6, C7, C8, C9, C10]
Broker C: Customers [C11, C12, C13, C14, C15]

LEVEL 1: Super Admin sees → [C1-C15] ✅
LEVEL 2: System Admin (manages A,B) sees → [C1-C10] ✅
LEVEL 2: System Admin (manages C) sees → [C11-C15] ✅
LEVEL 3: Broker A sees → [C1-C5] ✅
LEVEL 3: Broker B sees → [C6-C10] ✅
LEVEL 3: Broker C sees → [C11-C15] ✅
LEVEL 4: Agent A1 (from Broker A) sees → [C1-C5] ✅
LEVEL 4: Agent B1 (from Broker B) sees → [C6-C10] ✅
LEVEL 5: Client C1 sees → [Only C1 profile] ✅
```

---

## Security Rules

```
1. AUTHENTICATION
   ├─ User must log in with username/password
   ├─ OTP verification for sensitive operations
   ├─ Multi-factor authentication enabled
   └─ Session timeout after 30 minutes

2. AUTHORIZATION
   ├─ Role determines access level
   ├─ BrokerId filters data access
   ├─ Row-level security enforced
   └─ Cross-tenant access prevented

3. DATA ENCRYPTION
   ├─ Sensitive fields encrypted (CardNumber, CVV, PasswordHash)
   ├─ All passwords hashed with bcrypt
   ├─ Payment data encrypted at rest
   └─ TLS/HTTPS for data in transit

4. AUDIT LOGGING
   ├─ All user actions logged
   ├─ Data access tracked
   ├─ Changes recorded with timestamps
   └─ Admin actions highlighted

5. COMPLIANCE
   ├─ GDPR data retention policies
   ├─ Right to be forgotten implemented
   ├─ Data breach notification procedures
   └─ Regular security audits
```

---

## Implementation Checklist

### **Phase 1: Authentication**
- [ ] Set up user login system
- [ ] Implement password hashing
- [ ] Configure OTP verification
- [ ] Set up session management

### **Phase 2: Authorization**
- [ ] Implement role-based access control
- [ ] Add BrokerId filtering to all queries
- [ ] Set up row-level security (RLS)
- [ ] Test access restrictions

### **Phase 3: Multi-Tenancy**
- [ ] Verify data isolation per broker
- [ ] Test cross-tenant access prevention
- [ ] Implement query interceptors
- [ ] Add tenant context to all operations

### **Phase 4: Security**
- [ ] Encrypt sensitive fields
- [ ] Set up audit logging
- [ ] Configure monitoring & alerts
- [ ] Perform security penetration testing

---

**Hierarchy Levels:** 5 (Super Admin → Client)  
**Tables Covered:** 12  
**Tenant Isolation:** Per Broker (BrokerId)  
**Security Model:** RBAC + RLS  
**Status:** ✅ Ready for Implementation

