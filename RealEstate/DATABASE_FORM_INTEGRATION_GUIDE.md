# Database Migration & Form Integration

## ViewingAppointments Table Schema

```sql
CREATE TABLE [ViewingAppointments] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [PropertyId] INT NOT NULL,
    [CustomerId] INT NULL,
    [CustomerName] NVARCHAR(100) NOT NULL,
    [CustomerEmail] NVARCHAR(255) NULL,
    [CustomerPhone] NVARCHAR(20) NULL,
    [CustomerPhotoUrl] NVARCHAR(500) NULL,
    [WhenUtc] DATETIME NOT NULL,
    [PreferredTime] NVARCHAR(20) NULL,
    [NumberOfVisitors] INT NOT NULL DEFAULT 1,
    [BuyerType] NVARCHAR(50) NULL,
    [FinancingStatus] NVARCHAR(50) NULL,
    [InformationSource] NVARCHAR(50) NULL,
    [Notes] NVARCHAR(1000) NULL,
    [CreatedAtUtc] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    [Status] INT NOT NULL DEFAULT 0, -- AppointmentStatus enum
    FOREIGN KEY ([PropertyId]) REFERENCES [Properties]([Id]),
    FOREIGN KEY ([CustomerId]) REFERENCES [Customers]([Id])
)
```

## Form Data Collection & Mapping

### Step 1: Personal Details Collection
```csharp
// ScheduleViewingViewModel (source)
public string Name { get; set; }              // → CustomerName
public string Email { get; set; }             // → CustomerEmail
public string? Phone { get; set; }            // → CustomerPhone
public int? CustomerId { get; set; }          // → CustomerId (optional)

// These are collected in RequestViewing.cshtml Step 1
// Database inserts these values into ViewingAppointments table
```

### Step 2: Viewing Details & Preferences Collection
```csharp
// Schedule Details (Step 2)
public DateTime PreferredDate { get; set; }   // Combined with time
public string? PreferredTime { get; set; }    // → PreferredTime
public int NumberOfVisitors { get; set; }     // → NumberOfVisitors
public string? BuyerType { get; set; }        // → BuyerType
public string? FinancingStatus { get; set; }  // → FinancingStatus
public string? InformationSource { get; set; } // → InformationSource
public string? Notes { get; set; }            // → Notes
```

### DateTime Handling
```csharp
// The WhenUtc field combines Date + Time
DateTime preferredDate = DateTime.Parse("2024-04-27");
string preferredTime = "02:00 PM";

// Combined creation
DateTime whenUtc = preferredDate.Add(TimeSpan.Parse("14:00"));
```

## Form Submission Process

### Step 1-2: Data Collection
- Form validates all required fields
- Data stored in ScheduleViewingViewModel object
- Progress bar updates to show completion

### Step 3: Review
- Display all collected data in receipt format
- Allow user to modify by clicking "Previous"
- Confirm all information is correct

### Step 4: Payment
- Collect booking fee (₱500.00)
- Process payment via PayMongo
- Create ViewingAppointment record with Status = Scheduled

### Step 5: Confirmation
- Display success message
- Show appointment details
- Email confirmation sent to CustomerEmail

## Enum Mapping

### AppointmentStatus Enum
```csharp
public enum AppointmentStatus
{
    Scheduled = 0,    // Initial state after form submission
    Completed = 1,    // After viewing is completed
    Cancelled = 2     // If appointment is cancelled
}
```

### Buyer Type Values
- "first-time" → FirstTimeBuyer
- "experienced" → ExperiencedBuyer
- "investor" → Investor
- "renter" → Renter

### Financing Status Values
- "preapproved" → PreApproved
- "need-financing" → NeedFinancing
- "cash" → PayingCash
- "undecided" → StillDeciding

### Information Source Values
- "website" → Website
- "social-media" → SocialMedia
- "referral" → Referral
- "agent" → RealEstateAgent
- "other" → Other

## Controller Action Example

```csharp
[HttpPost]
public async Task<IActionResult> CreateCheckout(ScheduleViewingViewModel model)
{
    // Validate form data
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    try
    {
        // 1. Process payment
        var paymentResult = await ProcessPaymentAsync(model);
        
        // 2. Create viewing appointment
        var appointment = new ViewingAppointment
        {
            PropertyId = model.PropertyId,
            CustomerId = model.CustomerId,
            CustomerName = model.Name,
            CustomerEmail = model.Email,
            CustomerPhone = model.Phone,
            WhenUtc = CombineDateTime(model.PreferredDate, model.PreferredTime),
            PreferredTime = model.PreferredTime,
            NumberOfVisitors = model.NumberOfVisitors,
            BuyerType = model.BuyerType,
            FinancingStatus = model.FinancingStatus,
            InformationSource = model.InformationSource,
            Notes = model.Notes,
            CreatedAtUtc = DateTime.UtcNow,
            Status = AppointmentStatus.Scheduled
        };

        _context.ViewingAppointments.Add(appointment);
        await _context.SaveChangesAsync();

        // 3. Send confirmation email
        await _emailService.SendViewingConfirmationAsync(
            appointment.CustomerEmail,
            appointment.CustomerName,
            model.PreferredDate,
            model.PreferredTime
        );

        return RedirectToAction("ConfirmationPage", new { id = appointment.Id });
    }
    catch (Exception ex)
    {
        TempData["ErrorMessage"] = "An error occurred. Please try again.";
        return RedirectToAction("RequestViewing", new { propertyId = model.PropertyId });
    }
}
```

## Data Validation Rules

| Field | Rule | Error Message |
|-------|------|---------------|
| CustomerName | Required, Length ≤ 100 | Name is required |
| CustomerEmail | Required, Valid Email | Email must be valid |
| CustomerPhone | Required, Phone Format | Phone must be valid |
| PropertyId | Required, Must Exist | Property not found |
| WhenUtc | Required, Future Date | Date must be in future |
| NumberOfVisitors | Required, ≥ 1 | Must specify visitors |
| BuyerType | Optional | - |
| Status | Default to Scheduled | - |
| CreatedAtUtc | Auto-set | - |

## Query for Viewing All Appointments

```sql
SELECT TOP (1000) 
    [Id],
    [PropertyId],
    [CustomerId],
    [CustomerName],
    [CustomerEmail],
    [CustomerPhone],
    [CustomerPhotoUrl],
    [WhenUtc],
    [PreferredTime],
    [NumberOfVisitors],
    [BuyerType],
    [FinancingStatus],
    [InformationSource],
    [Notes],
    [CreatedAtUtc],
    [Status]
FROM [db49649].[dbo].[ViewingAppointments]
ORDER BY [CreatedAtUtc] DESC;
```

## Progress Bar State vs Database State

| Step | Progress | Form State | Database State |
|------|----------|-----------|----------------|
| 1 | 20% | Collecting Personal Details | No record yet |
| 2 | 40% | Collecting Schedule Details | No record yet |
| 3 | 60% | Reviewing All Data | No record yet |
| 4 | 80% | Processing Payment | No record yet |
| 5 | 100% | Complete | Record created with Status=Scheduled |

## Email Notification Template

When Step 5 is complete, a confirmation email is sent with:
```
Subject: Viewing Appointment Confirmation - [Property Title]

Dear [CustomerName],

Your viewing appointment has been successfully scheduled!

Appointment Details:
- Property: [PropertyTitle]
- Address: [PropertyLocation]
- Date: [PreferredDate]
- Time: [PreferredTime]
- Number of Visitors: [NumberOfVisitors]

Our agent will meet you at the property 15 minutes before the scheduled time.

If you need to reschedule or cancel, please call us at +63 (2) 8812-4400.

Best regards,
EstateFlow - Real Estate Solutions
```
