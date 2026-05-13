# EstateFlow Database - Manager Table

## 18. MANAGER TABLE

| Field Names | Datatype | Length | Description |
|---|---|---|---|
| ManagerId-PK | Int-AI | 9 | Manager's ID Number |
| UserId-FK-Required | Int | 9 | Foreign Key to Users Table |
| BrokerId-FK-Required | Int | 9 | Foreign Key to Brokers Table (Broker/Company Manager) |
| **MANAGER INFORMATION** | | | |
| ManagerName | NVarChar | Max | Manager's Full Name |
| Email | NVarChar | 255 | Manager's Email Address |
| Phone | NVarChar | 20 | Manager's Phone Number |
| Department | NVarChar | 50 | Department (Sales/HR/Finance/Operations/Admin) |
| JobTitle | NVarChar | 100 | Job Title/Position |
| **REPORTING STRUCTURE** | | | |
| ReportsTo | Int | 9 | Manager ID of Direct Supervisor (if applicable) |
| ManagerType | NVarChar | 50 | Type (Regional/Branch/Department/Team Lead) |
| **RESPONSIBILITY AREA** | | | |
| AreaOfResponsibility | NVarChar | Max | Area/Region/Department Managed |
| NumberOfTeamMembers | Int | 9 | Number of Staff Managed |
| **APPROVAL AUTHORITY** | | | |
| CanApproveCommissions | Bit | 1 | Authority to Approve Commissions (1=Yes, 0=No) |
| CanApproveLeavals | Bit | 1 | Authority to Approve Leaves (1=Yes, 0=No) |
| CanApproveExpenses | Bit | 1 | Authority to Approve Expenses (1=Yes, 0=No) |
| CanApprovePayroll | Bit | 1 | Authority to Approve Payroll (1=Yes, 0=No) |
| CanApproveInvoices | Bit | 1 | Authority to Approve Invoices (1=Yes, 0=No) |
| ExpenseApprovalLimit | Decimal | 18,2 | Maximum Expense Approval Amount (PHP) |
| **PERFORMANCE METRICS** | | | |
| TargetSales | Decimal | 18,2 | Sales Target for Managed Team (PHP) |
| ActualSales | Decimal | 18,2 | Actual Sales Achieved (PHP) |
| TargetCommission | Decimal | 18,2 | Commission Target (PHP) |
| ActualCommission | Decimal | 18,2 | Actual Commission Earned (PHP) |
| **STATUS & DATES** | | | |
| Status | NVarChar | 50 | Status (Active/Inactive/On Leave/Suspended) |
| EmploymentType | NVarChar | 50 | Type (Full-Time/Part-Time/Contract) |
| JoinDate | DateTime2 | - | Date Manager Joined |
| EndDate | DateTime2 | - | Date Manager Left (if applicable) |
| **METADATA** | | | |
| Notes | NVarChar | Max | Additional Notes |
| CreatedDate | DateTime2 | - | Record Creation Date (Auto) |
| UpdatedDate | DateTime2 | - | Record Last Updated Date |

---

## Manager Table Summary Format

```
Managers Table

Field Names                | Datatype  | Length | Description
ManagerId-PK               | Int-AI    | 9      | Manager's ID Number
UserId-FK-Required         | Int       | 9      | Foreign Key to Users Table
BrokerId-FK-Required       | Int       | 9      | Foreign Key to Brokers Table
ManagerName                | NVarChar  | Max    | Manager's Full Name
Email                      | NVarChar  | 255    | Manager's Email Address
Phone                      | NVarChar  | 20     | Manager's Phone Number
Department                 | NVarChar  | 50     | Department (Sales/HR/Finance/Operations/Admin)
JobTitle                   | NVarChar  | 100    | Job Title/Position
ReportsTo                  | Int       | 9      | Manager ID of Direct Supervisor
ManagerType                | NVarChar  | 50     | Type (Regional/Branch/Department/Team Lead)
AreaOfResponsibility       | NVarChar  | Max    | Area/Region/Department Managed
NumberOfTeamMembers        | Int       | 9      | Number of Staff Managed
CanApproveCommissions      | Bit       | 1      | Authority to Approve Commissions
CanApproveLeavals          | Bit       | 1      | Authority to Approve Leaves
CanApproveExpenses         | Bit       | 1      | Authority to Approve Expenses
CanApprovePayroll          | Bit       | 1      | Authority to Approve Payroll
CanApproveInvoices         | Bit       | 1      | Authority to Approve Invoices
ExpenseApprovalLimit       | Decimal   | 18,2   | Maximum Expense Approval Amount (PHP)
TargetSales                | Decimal   | 18,2   | Sales Target for Managed Team (PHP)
ActualSales                | Decimal   | 18,2   | Actual Sales Achieved (PHP)
TargetCommission           | Decimal   | 18,2   | Commission Target (PHP)
ActualCommission           | Decimal   | 18,2   | Actual Commission Earned (PHP)
Status                     | NVarChar  | 50     | Active/Inactive/On Leave/Suspended
EmploymentType             | NVarChar  | 50     | Full-Time/Part-Time/Contract
JoinDate                   | DateTime2 | -      | Date Manager Joined
EndDate                    | DateTime2 | -      | Date Manager Left (if applicable)
Notes                      | NVarChar  | Max    | Additional Notes
CreatedDate                | DateTime2 | -      | Record Creation Date (Auto)
UpdatedDate                | DateTime2 | -      | Record Last Updated Date
```

---

## Database Relationships

### Manager Table Relationships
```
Users (1) ──→ (M) Managers (UserId-FK)
└─ Each user can be a manager

Brokers (1) ──→ (M) Managers (BrokerId-FK)
└─ Each broker can have multiple managers

Managers (1) ──→ (M) Managers (ReportsTo)
└─ Managers can report to other managers (hierarchical structure)

Managers (1) ──→ (M) Payroll (ManagerId to ApprovedBy)
└─ Managers can approve payroll records

Managers (1) ──→ (M) Commissions (ManagerId to ApprovedBy)
└─ Managers can approve commissions

Managers (1) ──→ (M) Invoices (ManagerId to CreatedBy/ApprovedBy)
└─ Managers can create and approve invoices
```

---

## Business Logic

### Manager Hierarchy
```
Organization Structure:
├─ Super Admin (Top Level)
├─ Regional Manager
│  ├─ Branch Manager
│  │  ├─ Sales Manager
│  │  ├─ HR Manager
│  │  └─ Finance Manager
│  └─ Team Lead
└─ Individual Contributors (Agents)
```

### Approval Authority Levels
```
Level 1 - Super Admin: Unlimited authority for all approvals
Level 2 - Regional Manager: Approve up to PHP 500,000
Level 3 - Branch Manager: Approve up to PHP 250,000
Level 4 - Department Manager: Approve up to PHP 100,000
Level 5 - Team Lead: Approve up to PHP 50,000

Each manager can approve:
- Commissions (if CanApproveCommissions = 1)
- Expenses (up to their limit)
- Invoices (if CanApproveInvoices = 1)
- Payroll (if CanApprovePayroll = 1)
- Leaves (if CanApproveLeavals = 1)
```

### Performance Tracking
```
Sales Performance:
- TargetSales = Goal for manager's team
- ActualSales = Sum of team's sales
- Achievement % = (ActualSales / TargetSales) * 100

Commission Performance:
- TargetCommission = Goal for manager's team
- ActualCommission = Sum of team's commissions
- Commission Achievement % = (ActualCommission / TargetCommission) * 100
```

---

## SQL Constraints & Rules

```sql
-- Manager status
CHECK (Status IN ('Active', 'Inactive', 'On Leave', 'Suspended'))

-- Manager type
CHECK (ManagerType IN ('Regional', 'Branch', 'Department', 'Team Lead'))

-- Employment type
CHECK (EmploymentType IN ('Full-Time', 'Part-Time', 'Contract'))

-- Department
CHECK (Department IN ('Sales', 'HR', 'Finance', 'Operations', 'Admin'))

-- Cannot manage negative number of team members
CHECK (NumberOfTeamMembers >= 0)

-- Cannot have negative targets or actuals
CHECK (TargetSales >= 0)
CHECK (ActualSales >= 0)
CHECK (TargetCommission >= 0)
CHECK (ActualCommission >= 0)
CHECK (ExpenseApprovalLimit >= 0)

-- Cannot report to self
CHECK (ReportsTo IS NULL OR ReportsTo <> ManagerId)
```

---

## Example Data

### Managers Example
```
ManagerId | UserId | BrokerId | ManagerName    | Department | JobTitle          | Status  | CanApproveCommissions
1         | 5      | 1        | Juan Santos    | Sales      | Regional Manager  | Active  | 1
2         | 6      | 1        | Maria Cruz     | Sales      | Branch Manager    | Active  | 1
3         | 7      | 1        | Pedro Lopez    | Finance    | Finance Manager   | Active  | 0
4         | 8      | 2        | Ana Rodriguez  | Sales      | Sales Manager     | Active  | 1
```

### Manager Hierarchy Example
```
Super Admin (System Level)
  └─ Juan Santos (Regional Manager) - Reports: None
     ├─ Maria Cruz (Branch Manager) - Reports To: Juan Santos
     │  ├─ Pedro Lopez (Finance Manager) - Reports To: Maria Cruz
     │  └─ Sales Team Leads
     └─ Other Regional Managers
```

---

## Access Control

### Managers Table Access by Role
- **Level 1 (Super Admin):** ✅ Full CRUD
- **Level 2 (System Admin):** 🟡 Limited (assigned brokers)
- **Level 3 (Broker):** 🟡 Limited (own managers only)
- **Level 4 (Agent):** 🔵 Read-only (view manager info)
- **Level 5 (Client):** ❌ No access

---

## Reports & Queries

### Manager Performance Report
```sql
SELECT 
    m.ManagerName,
    m.Department,
    m.NumberOfTeamMembers,
    m.TargetSales,
    m.ActualSales,
    (m.ActualSales / NULLIF(m.TargetSales, 0)) * 100 as SalesAchievementPercent,
    m.TargetCommission,
    m.ActualCommission,
    (m.ActualCommission / NULLIF(m.TargetCommission, 0)) * 100 as CommissionAchievementPercent
FROM Managers m
WHERE m.Status = 'Active'
ORDER BY m.Department, m.ManagerName;
```

### Manager Approval Authority Report
```sql
SELECT 
    m.ManagerName,
    m.JobTitle,
    m.Department,
    m.CanApproveCommissions,
    m.CanApprovePayroll,
    m.CanApproveInvoices,
    m.ExpenseApprovalLimit,
    COUNT(DISTINCT u.UserId) as TeamMemberCount
FROM Managers m
LEFT JOIN Users u ON u.UserId = m.UserId
WHERE m.Status = 'Active'
GROUP BY m.ManagerId, m.ManagerName, m.JobTitle, m.Department, 
         m.CanApproveCommissions, m.CanApprovePayroll, 
         m.CanApproveInvoices, m.ExpenseApprovalLimit;
```

### Manager Hierarchy Report
```sql
WITH ManagerHierarchy AS (
    SELECT 
        ManagerId,
        ManagerName,
        ReportsTo,
        Department,
        1 as Level
    FROM Managers
    WHERE ReportsTo IS NULL
    
    UNION ALL
    
    SELECT 
        m.ManagerId,
        m.ManagerName,
        m.ReportsTo,
        m.Department,
        mh.Level + 1
    FROM Managers m
    INNER JOIN ManagerHierarchy mh ON m.ReportsTo = mh.ManagerId
)
SELECT * FROM ManagerHierarchy
ORDER BY Level, ManagerName;
```

---

## Constraints & Validations

### Business Rules
1. Each manager must have a UserId linking to Users table
2. Each manager must belong to a Broker
3. A manager cannot report to themselves
4. Approval limits must be positive values
5. Target sales and commissions must be positive
6. Team member count cannot be negative
7. Join date must be before end date (if end date exists)

### Data Validation
- Email must be unique per broker
- Phone number format validation
- Department must be from predefined list
- Status must be from predefined list
- Employment type must be from predefined list

---

## Integration Points

### With Other Tables
- **Users:** Manager is created when a User with manager role is added
- **Payroll:** Manager can approve payroll records for their team
- **Commissions:** Manager can approve commissions based on approval authority
- **Invoices:** Manager can create/approve invoices within limits
- **Appointments:** Manager can be assigned as supervisor for appointments
- **Accounting:** Manager's approvals create accounting entries

---

## Features

✅ **Hierarchical Manager Structure** - Managers can report to other managers
✅ **Approval Authority** - Granular control over what each manager can approve
✅ **Expense Limits** - Set maximum approval amounts per manager
✅ **Performance Tracking** - Sales and commission targets vs actuals
✅ **Department Management** - Organize by department (Sales, HR, Finance, etc.)
✅ **Team Management** - Track number of team members
✅ **Employment Type** - Full-time, Part-time, Contract employees
✅ **Status Tracking** - Active, Inactive, On Leave, Suspended
✅ **Audit Trail** - Created/Updated dates for compliance

---

**Table Number:** 18  
**Status:** Production Ready  
**Integration:** Full support for approval workflows  
**Relationships:** 6+ foreign key relationships

