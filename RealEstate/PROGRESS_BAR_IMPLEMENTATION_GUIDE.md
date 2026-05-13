# Progress Bar Implementation Guide

## Overview
The Schedule Viewing modal now includes an enhanced multi-step progress bar that provides visual feedback to users as they complete the viewing request form. The implementation uses a 5-step wizard pattern that aligns perfectly with the `ViewingAppointments` database schema.

## Progress Bar Features

### Visual Components
1. **Progress Bar Fill** - Animated percentage fill that advances as users complete steps
2. **Step Circles** - Numbered indicators (1-5) showing:
   - **Inactive**: Gray (#E5E7EB)
   - **Active**: Orange (#FF9500) with scale effect
   - **Completed**: Green (#10B981) with checkmark appearance
3. **Step Labels** - Descriptive text for each step with color coding
4. **Step Counter** - Displays "Step X of 5" for clarity

### Animation Effects
- Smooth progress bar fill transition (0.5s cubic-bezier)
- Scale animation on active step circle (1.1x)
- Color transitions on state changes
- Hover effects on navigation buttons

## 5-Step Form Workflow

### Step 1: Personal Details
**Database Fields**: CustomerName, CustomerEmail, CustomerPhone
```
- Full Name (required)
- Email Address (required)
- Phone Number (required)
```

### Step 2: Schedule & Reason for Viewing
**Database Fields**: PreferredDate, PreferredTime, NumberOfVisitors, BuyerType, FinancingStatus, InformationSource, Notes
```
- Preferred Date (required)
- Preferred Time (optional)
- Number of Visitors
- Buyer Type
- Financing Status
- How Did You Hear About Us?
- Additional Notes (optional)
```

### Step 3: Review Details
**Database Fields**: All fields (review/confirmation display)
```
- Receipt-style display of all entered information
- Summary of personal, viewing, and additional details
- Verification before submission
```

### Step 4: Payment
**Database Fields**: WhenUtc (set upon payment)
```
- Booking fee display (₱500.00)
- Payment method selection
- Final confirmation before processing
```

### Step 5: Confirmation
**Database Fields**: CreatedAtUtc, Status (AppointmentStatus.Scheduled)
```
- Success message display
- Appointment confirmation details
- Next steps information
```

## Database Schema Alignment

The form collects data that maps directly to the `ViewingAppointments` table:

| Form Field | Database Field | Step | Type | Nullable |
|-----------|----------------|------|------|----------|
| Full Name | CustomerName | 1 | string(100) | No |
| Email | CustomerEmail | 1 | string(255) | Yes |
| Phone | CustomerPhone | 1 | string(20) | Yes |
| Preferred Date | WhenUtc | 2 | DateTime | No |
| Preferred Time | PreferredTime | 2 | string(20) | Yes |
| Number of Visitors | NumberOfVisitors | 2 | int | No (default=1) |
| Buyer Type | BuyerType | 2 | string(50) | Yes |
| Financing Status | FinancingStatus | 2 | string(50) | Yes |
| Information Source | InformationSource | 2 | string(50) | Yes |
| Additional Notes | Notes | 2 | string(1000) | Yes |
| Property ID | PropertyId | 1 | int | No |
| Customer ID | CustomerId | 1 | int | Yes |
| Photo URL | CustomerPhotoUrl | - | string(500) | Yes |
| Status | Status | 5 | AppointmentStatus | No (default=Scheduled) |

## JavaScript Functionality

### Progress Bar Updates
```javascript
// Progress percentage calculation
const progressPercent = (currentStep / totalSteps) * 100;
document.getElementById('progressBarFill').style.width = progressPercent + '%';

// Step counter display
document.getElementById('currentStepNum').textContent = currentStep;
```

### Step Navigation
- **Next Button**: Validates current step before advancing
- **Previous Button**: Allows backtracking (hidden on Step 1 and final step)
- **Validation**: Custom validation for each step
- **Review Update**: Auto-populates review section before Step 3

### State Management
- Tracks current step (1-5)
- Updates UI elements based on current step
- Manages button visibility and actions
- Applies CSS classes for visual feedback

## Responsive Design

The progress bar adapts to mobile screens:
- Step labels remain visible on all screen sizes
- Progress bar maintains visual clarity
- Navigation buttons stack properly on smaller screens
- Touch-friendly circle sizes

## Styling Classes

```css
.progress-bar-container      /* Main container with gradient background */
.progress-bar-label          /* "Step X of 5" text */
.progress-bar-fill           /* Gray background track */
.progress-bar-fill-inner     /* Orange animated fill */
.progress-steps              /* Container for step indicators */
.progress-step               /* Individual step container */
.step-circle                 /* Numbered circle indicator */
.step-circle.active          /* Active step styling */
.step-circle.completed       /* Completed step styling */
.step-label                  /* Step description text */
```

## Browser Support
- Modern browsers (Chrome, Firefox, Safari, Edge)
- CSS3 transitions and transforms
- Flexbox layout
- ES6+ JavaScript

## Notes
- All form fields are properly validated before advancing
- The form maintains state through all 5 steps
- Payment processing occurs at Step 4
- Final confirmation is displayed at Step 5
- Anti-forgery tokens are included for security
