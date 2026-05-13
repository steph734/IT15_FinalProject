# Schedule Viewing Modal - Quick Reference Guide

## 📋 What Was Added

**Profile Photo Upload Field** in Schedule Viewing Modal (Step 1)

## 🎯 Key Details

| Property | Value |
|----------|-------|
| **Database Column** | `CustomerPhotoUrl` (NVARCHAR(500)) |
| **Form Location** | Step 1: Personal Details |
| **Field Type** | File Input (Optional) |
| **Accepted Formats** | JPG, PNG, GIF |
| **Max File Size** | 2MB |
| **Required** | No |
| **Preview Size** | 100x100px (with remove button) |
| **Review Display** | 80x80px thumbnail |

## ✅ Form Fields Checklist

### Step 1: Personal Details
```
☑ Full Name (Required)
☑ Email Address (Required)
☑ Phone Number (Required)
☑ Profile Photo (Optional) ⭐ NEW
```

### Step 2: Schedule & Reason
```
☑ Preferred Date (Required)
☑ Preferred Time (Optional)
☑ Number of Visitors (Required)
☑ Buyer Type (Required)
☑ Financing Status (Optional)
☑ Information Source (Optional)
☑ Additional Notes (Optional)
```

## 📊 Database Coverage

```
Total Columns: 16
✅ User-Collected: 11
✅ Auto-Generated: 2 (Id, CreatedAtUtc)
✅ Backend-Set: 3 (CustomerId, Status, Property)

Coverage: 100%
```

## 🔧 Implementation

### ViewModel
```csharp
[Display(Name = "Profile Photo")]
[StringLength(500)]
public string? CustomerPhotoUrl { get; set; }
```

### HTML
```html
<input type="file" id="customerPhoto" 
       name="Schedule.CustomerPhotoUrl" 
       accept="image/*" />
```

### Validation
```javascript
// File Type: JPG, PNG, GIF
// File Size: ≤ 2MB
// Client-side validation with error messages
```

## 🎨 CSS Classes

```css
.photo-upload-container      /* Main wrapper */
.photo-preview               /* Image thumbnail */
.photo-preview-remove        /* Remove button */
.file-input-label            /* Upload button */
.photo-upload-hint           /* Helper text */
```

## 💻 JavaScript Functions

```javascript
handlePhotoChange(e)    // Validate & preview file
removePhoto()           // Clear selection
```

## 📁 Files Modified

1. **ScheduleViewingViewModel.cs**
   - Added CustomerPhotoUrl property

2. **RequestViewing.cshtml**
   - Added photo upload CSS
   - Added photo upload HTML
   - Added photo display in review
   - Added JavaScript functions
   - Updated form submission

## ⚙️ Backend Integration Required

```csharp
[HttpPost]
public async Task<IActionResult> CreateCheckout(
    ScheduleViewingViewModel model, 
    IFormFile photoFile)
{
    // 1. Validate file
    // 2. Upload file to storage
    // 3. Get file URL
    // 4. Update model.CustomerPhotoUrl = url
    // 5. Create ViewingAppointment record
    // 6. Save to database
}
```

## ✨ User Experience

### Upload Flow
```
1. User clicks "Choose Photo"
2. System shows file picker (images only)
3. User selects JPG/PNG/GIF (≤2MB)
4. System validates file
5. Preview thumbnail displays
6. User can click X to remove
7. Photo included in Step 3 review
8. Photo sent with form submission
```

## 📚 Documentation

- **PROFILE_PHOTO_UPLOAD_FIELD_GUIDE.md** - Full technical guide
- **VIEWING_APPOINTMENTS_COMPLETE_MAPPING.md** - Database mapping
- **SCHEDULE_VIEWING_MODAL_COMPLETE_DELIVERY.md** - Full delivery summary

## 🚀 Status

- ✅ Build: Successful
- ✅ Database Coverage: 100% (16/16 columns)
- ✅ Form Fields: 11 user inputs collected
- ✅ Validation: Client-side implemented
- ✅ Ready for Backend Integration

## 🔒 Security Notes

- Client-side file validation (type, size)
- Server-side validation required
- Store files outside web root
- Implement access control for agents/admins
- Use HTTPS for uploads

## 📱 Responsive

- ✅ Desktop: Full UI
- ✅ Tablet: Optimized layout
- ✅ Mobile: Touch-friendly buttons

## 🌐 Browser Support

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+
- Mobile browsers

---

**Quick Links**:
- Full Guide: `PROFILE_PHOTO_UPLOAD_FIELD_GUIDE.md`
- Schema Map: `VIEWING_APPOINTMENTS_COMPLETE_MAPPING.md`
- Complete Delivery: `SCHEDULE_VIEWING_MODAL_COMPLETE_DELIVERY.md`
