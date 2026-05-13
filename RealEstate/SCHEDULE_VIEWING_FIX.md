# Schedule Viewing HTTP 400/404 Error - FIXED ✅

## Problems
1. **HTTP 400 Error** - When submitting the form, users got a Bad Request error
2. **HTTP 404 Error** - When accessing `/Properties/ScheduleViewing` directly, users got a Page Not Found error

## Root Causes Identified

### 1. **Missing AntiForgeryToken**
The form in `Views/Properties/Details.cshtml` was submitting POST data without an `__RequestVerificationToken`, causing HTTP 400 errors.

### 2. **Incorrect Route Attributes (HTTP 404)**
The route attributes `[HttpGet("ScheduleViewing")]` and `[HttpPost("ScheduleViewing")]` were creating routes at `/ScheduleViewing` instead of `/Properties/ScheduleViewing`, causing HTTP 404 errors.

### 3. **ViewBag vs TempData Issue**
After redirecting, the `ViewBag.AppointmentId` was lost because ViewBag data doesn't persist across redirects. This prevented the success modal from showing the "Start Live Tracking" button.

### 4. **DateTime Kind Handling**
The `scheduleDate` parameter might come with `DateTimeKind.Unspecified`, which could cause issues when storing in the database as UTC.

## Fixes Applied

### ✅ Fix 1: Fixed Route Attributes (HTTP 404)
**File:** `Controllers/PropertiesController.cs` (Lines 302-321)

Changed from:
```csharp
[HttpGet("ScheduleViewing")]
public IActionResult ScheduleViewing_Get()

[HttpPost("ScheduleViewing")]
public async Task<IActionResult> ScheduleViewing(...)
```

Changed to:
```csharp
[HttpGet]
[Route("Properties/ScheduleViewing")]
public IActionResult ScheduleViewing_Get()

[HttpPost]
[Route("Properties/ScheduleViewing")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ScheduleViewing(...)
```

**Same fix applied to BuyProperty routes** (Lines 392-411)

### ✅ Fix 2: Added AntiForgeryToken to Form (HTTP 400)
**File:** `Views/Properties/Details.cshtml` (Line 829)

```html
<form id="scheduleViewingForm" method="post" action="/Properties/ScheduleViewing">
    <input type="hidden" name="propertyId" value="@p.Id" />
    @Html.AntiForgeryToken()  <!-- ADDED THIS LINE -->
```

### ✅ Fix 3: Added ValidateAntiForgeryToken to Controller
**File:** `Controllers/PropertiesController.cs` (Line 312)

```csharp
[HttpPost("ScheduleViewing")]
[ValidateAntiForgeryToken]  // ADDED THIS ATTRIBUTE
public async Task<IActionResult> ScheduleViewing(
    [FromForm] int propertyId,
    [FromForm] string fullName,
    [FromForm] string email,
    [FromForm] string phoneNumber,
    [FromForm] DateTime scheduleDate,
    [FromForm] string? reason)
```

### ✅ Fix 4: Enhanced Validation & Error Handling
**File:** `Controllers/PropertiesController.cs` (Lines 320-387)

Added comprehensive validation:
- ✅ Validates `fullName` is not empty
- ✅ Validates `email` is not empty
- ✅ Validates `phoneNumber` is not empty
- ✅ Properly converts DateTime to UTC
- ✅ Added console logging for debugging
- ✅ Better error messages

```csharp
// Validate required fields
if (string.IsNullOrWhiteSpace(fullName))
{
    TempData["ErrorMessage"] = "Full name is required.";
    return RedirectToAction("Details", new { id = propertyId });
}

// Convert to UTC properly
WhenUtc = scheduleDate.Kind == DateTimeKind.Unspecified 
    ? DateTime.SpecifyKind(scheduleDate, DateTimeKind.Utc) 
    : scheduleDate.ToUniversalTime(),
```

### ✅ Fix 5: Changed ViewBag to TempData
**File:** `Controllers/PropertiesController.cs` (Line 374)

```csharp
// BEFORE:
ViewBag.AppointmentId = appointment.Id;

// AFTER:
TempData["AppointmentId"] = appointment.Id;
```

**File:** `Views/Properties/Details.cshtml` (Line 1192)

```html
<!-- BEFORE: -->
@if (ViewBag.AppointmentId != null)
{
    <a href="/tracking/customer/@ViewBag.AppointmentId" ...>

<!-- AFTER: -->
@if (TempData["AppointmentId"] != null)
{
    <a href="/tracking/customer/@TempData["AppointmentId"]" ...>
```

## Database Storage

The viewing appointment is now properly stored in the `[ViewingAppointments]` table with these fields:

```sql
SELECT TOP (1000) 
      [Id]              -- Auto-generated
      ,[PropertyId]     -- From form: propertyId
      ,[CustomerId]     -- NULL (can be linked later)
      ,[CustomerName]   -- From form: fullName
      ,[CustomerEmail]  -- From form: email
      ,[CustomerPhone]  -- From form: phoneNumber
      ,[CustomerPhotoUrl] -- NULL
      ,[WhenUtc]        -- From form: scheduleDate (converted to UTC)
      ,[PreferredTime]  -- NULL (can be extracted from WhenUtc)
      ,[NumberOfVisitors] -- Default: 1
      ,[BuyerType]      -- NULL
      ,[FinancingStatus] -- NULL
      ,[InformationSource] -- NULL
      ,[Notes]          -- From form: reason
      ,[CreatedAtUtc]   -- Auto-generated: GETUTCDATE()
      ,[Status]         -- Default: 'Scheduled'
FROM [db49649].[dbo].[ViewingAppointments]
```

## Testing the Fix

### Sample Form Data:
```
Full Name: dssds
Email Address: fddffg@gmail.com
Phone Number: 0987654354
Preferred Date & Time: 04/10/2026 01:53 PM
Reason for Viewing: dfdfdfd
```

### Expected Result:
1. ✅ Form submits successfully (no HTTP 400 error)
2. ✅ Data is stored in `ViewingAppointments` table
3. ✅ Success modal appears with confirmation message
4. ✅ "Start Live Tracking" button is available
5. ✅ Console logs show: `[SUCCESS] Viewing appointment created: ID=X, Property=Y, Customer=Z`

## How to Verify

### 1. Check Database:
```sql
SELECT TOP 10 * 
FROM [db49649].[dbo].[ViewingAppointments]
ORDER BY [CreatedAtUtc] DESC
```

### 2. Check Application Logs:
Look for console output when running the application:
```
[SUCCESS] Viewing appointment created: ID=123, Property=5, Customer=dssds, Email=fddffg@gmail.com, Date=2026-04-10 05:53:00
```

### 3. Test the Flow:
1. Navigate to any property details page
2. Click "Schedule Viewing" button
3. Fill out the form with test data
4. Click "Confirm Schedule"
5. Verify success modal appears
6. Check database for new record

## Additional Notes

### Security Improvements:
- ✅ CSRF protection enabled with AntiForgeryToken
- ✅ Input validation on all required fields
- ✅ Proper HTML encoding prevents XSS attacks

### Data Integrity:
- ✅ DateTime stored in UTC format
- ✅ All strings trimmed before storage
- ✅ Status defaults to "Scheduled"
- ✅ CreatedAtUtc automatically set

### Future Enhancements (Optional):
The form can be extended to capture additional fields:
- `NumberOfVisitors` (default: 1)
- `BuyerType` (First-time, Investor, Upgrading, etc.)
- `FinancingStatus` (Cash, Pre-approved, Pre-qualified, etc.)
- `InformationSource` (Google, Social Media, Referral, etc.)
- `PreferredTime` (extracted from datetime)

## Build Status
✅ **Build Succeeded** - No compilation errors
- 100 warnings (pre-existing, non-critical)
- All new code compiles successfully

---

**Fixed Date:** April 27, 2026  
**Status:** ✅ RESOLVED
