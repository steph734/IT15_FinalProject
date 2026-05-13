# Properties Page Visual Reference

## ✅ REDESIGN COMPLETE - CityScape Style

---

## 🎨 Page Layout Visual

```
┌─────────────────────────────────────────────────────────────────────┐
│                         HEADER SECTION                              │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│  [Status ▼] [Type ▼] [Location .......] [Advanced Filter]          │  TOP FILTERS
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────┬───────────────────────────┐
│           MAIN CONTENT (66%)            │   RIGHT SIDEBAR (33%)    │
├─────────────────────────────────────────┤                           │
│ Showing XX   [Grid][List] Sort: [New▼] │ PROPERTY TYPE             │
├─────────────────────────────────────────┤ ─────────────────────     │
│                                         │ ☐ House                  │
│  ┌──────────────┬──────────────────┐   │ ☐ Single Family           │
│  │              │ Title            │   │ ☐ Apartment               │
│  │   IMAGE      │ Features         │   │ ☐ Office Villa            │
│  │   280x200    │ Location         │   │ ☐ Luxury Home             │
│  │              │ ₱Price/mo        │   │ ☐ Studio                  │
│  │              │ [VIEW DETAILS]   │   │                           │
│  └──────────────┴──────────────────┘   │ PRICE RANGE               │
│                                         │ ─────────────────────     │
│  ┌──────────────┬──────────────────┐   │ ☐ Low Budget              │
│  │              │ Title            │   │ ☐ Medium Range            │
│  │   IMAGE      │ Features         │   │ ☐ Premium                 │
│  │   280x200    │ Location         │   │                           │
│  │              │ ₱Price/mo        │   │ LISTING TYPE              │
│  │              │ [VIEW DETAILS]   │   │ ─────────────────────     │
│  └──────────────┴──────────────────┘   │ ☐ For Sale                │
│                                         │ ☐ For Rent                │
│  ┌──────────────┬──────────────────┐   │                           │
│  │              │ Title            │   │                           │
│  │   IMAGE      │ Features         │   │                           │
│  │   280x200    │ Location         │   │                           │
│  │              │ ₱Price/mo        │   │                           │
│  │              │ [VIEW DETAILS]   │   │                           │
│  └──────────────┴──────────────────┘   │                           │
│                                         │                           │
│        [1] [2] [3] [4] Pagination      │                           │
└─────────────────────────────────────────┴───────────────────────────┘
```

---

## 📱 Property Card Breakdown

### Full Width View
```
┌──────────────────────────────────────────────────────┐
│  ┌─────────────┬──────────────────────────────────┐  │
│  │             │ Turning Dreams into Addresses    │  │
│  │   IMAGE     │ Home State                       │  │
│  │   280×200   │ [BADGE: 2000m²]                │  │
│  │   px        │                                  │  │
│  │   [HOVER:   │ 🏠 3 Beds  |  🚿 2 Baths         │  │
│  │   ZOOM]     │                                  │  │
│  │             │ 📍 66 Brooklyst, New York       │  │
│  │             │                                  │  │
│  │             │ ₱456,000 /per day                │  │
│  │             │                                  │  │
│  │             │ [BOOK NOW →]                     │  │
│  └─────────────┴──────────────────────────────────┘  │
└──────────────────────────────────────────────────────┘
```

### Color Annotations
```
Text: #1E3A5F (Navy)
      ↓
      "Turning Dreams into Addresses Home State"

Features: #6B7280 (Gray)
         ↓
         🏠 3 Beds  |  🚿 2 Baths

Icons: #FF9500 (Orange)
      ↓
      [BADGE: 2000m²]  [MAP PIN]  [BED ICON]  [BATH ICON]

Price: #1E3A5F (Navy) + #9CA3AF (Light Gray for unit)
      ↓
      ₱456,000 /per day

Button: #FF9500 (Orange) background, White text
       ↓
       [BOOK NOW →]
```

---

## 🎯 Filter Sidebar Sections

### Property Type Section
```
┌────────────────────────────┐
│ Property Type              │  ← Dark text, bold
├────────────────────────────┤
│ ☐ House                    │
│ ☐ Single Family            │
│ ☐ Apartment                │
│ ☐ Office Villa             │
│ ☐ Luxury Home              │
│ ☐ Studio                   │
└────────────────────────────┘
```

### Price Range Section
```
┌────────────────────────────┐
│ Price Range                │
├────────────────────────────┤
│ ☐ Low Budget               │
│ ☐ Medium Range             │
│ ☐ Premium                  │
└────────────────────────────┘
```

### Listing Type Section
```
┌────────────────────────────┐
│ Listing Type               │
├────────────────────────────┤
│ ☐ For Sale                 │
│ ☐ For Rent                 │
└────────────────────────────┘
```

---

## 🎨 Color Palette Reference

### Primary Colors
```
Orange:  #FF9500  ████████████ Primary accent, buttons, active states
Navy:    #1E3A5F  ████████████ Primary text, headings
Gray:    #6B7280  ████████████ Secondary text, labels
```

### Supporting Colors
```
White:   #FFFFFF  ████████████ Card backgrounds
LtGray:  #F8F9FA  ████████████ Page background
Border:  #E5E7EB  ████████████ Borders, separators
```

---

## 📊 Top Filter Bar

```
┌─────────────────────────────────────────────────────────┐
│  [Status ▼]  [Type ▼]  [Location .......]  [Advanced]  │
└─────────────────────────────────────────────────────────┘
    ↑            ↑            ↑                   ↑
    Icons for each filter type
    All filters responsive
    Dropdowns/inputs with rounded borders
```

---

## 📈 Sorting & View Controls

```
Showing 1-10 of 23     [Grid] [List]    Sort by: Newest ▼

  ↑ Result count        ↑ View toggle    ↑ Sort dropdown
  (Gray text)           (Orange=active)   (Dropdown)
```

---

## 🔢 Pagination Style

```
[1]  [2]  [3]  [4]

Active (Current page):
┌───┐
│ 1 │ ← Orange background, white text
└───┘

Inactive:
┌───┐
│ 2 │ ← White background, gray text, hover = orange text
└───┘
```

---

## 📱 Responsive Behavior

### Desktop (≥992px)
```
Full sidebar visible    [Grid - 280px + content] Layout
8/12 + 4/12 split      Sticky sidebar position
```

### Tablet (768px-991px)
```
Sidebar moves below     [Cards - full width]
Single column content   Mobile-optimized
```

### Mobile (<768px)
```
No filter icons        [Cards - full width, single column]
Compact controls       Simplified layout
Touch-friendly buttons
```

---

## 🎬 Animations & Interactions

### Card Hover
```
BEFORE:
┌──────────────────────────┐
│ [Image] [Content]        │
│ Shadow: 0px drop         │
└──────────────────────────┘

AFTER (Hover):
┌──────────────────────────┐
│ [Image+ZOOM] [Content]   │
│ Shadow: 8px drop         │
│ Image scales 1.05x       │
└──────────────────────────┘
```

### Button Hover
```
BEFORE:
[VIEW DETAILS →]  Background: #FF9500  Gap: 8px

AFTER (Hover):
[VIEW DETAILS    →]  Background: #FF8C00  Gap: 12px
                     (darker orange)     (arrow moves right)
```

---

## 📋 Key Measurements

```
Top Filters:          20px padding, 15px gaps
Filter Bar Padding:   Border-bottom 1px #E5E7EB
Property Card:        280px (image) + flexible (content)
Card Image Height:    200px
Card Padding:         20px on all sides
Sidebar Width:        Col-lg-4 (33.333%)
Main Content:         Col-lg-8 (66.666%)
Gap Between Cards:    20px (15px on mobile)
Filter Badge:         6px × 12px padding
Pagination Gap:       12px between dots
Border Radius:        4-8px (cards), 6px (inputs)
```

---

## 🎯 Typography

### Headings
```
Property Title:        1.1rem, 700 weight, #1E3A5F (Navy)
Filter Section Title:  0.95rem, 700 weight, #1E3A5F (Navy)
```

### Body Text
```
Location/Features:     0.85rem, 400 weight, #6B7280 (Gray)
Price:                 1.3rem, 800 weight, #1E3A5F (Navy)
Price Unit:            0.85rem, 400 weight, #9CA3AF (Light Gray)
Result Count:          0.95rem, 400 weight, #6B7280 (Gray)
Labels:                0.9rem, 400 weight, #6B7280 (Gray)
```

### Buttons & Links
```
CTA Button:            0.85rem, 600 weight, uppercase, letter-spacing
View/Sort:             0.9rem, 400-600 weight
Pagination:            0.875rem, 600 weight
```

---

## 🔗 Component Relationships

```
TOP FILTERS
    ↓
MAIN CONTENT (66%)
    ├─ SORTING CONTROLS
    ├─ PROPERTY GRID
    │   ├─ Card 1
    │   ├─ Card 2
    │   └─ Card N
    └─ PAGINATION

RIGHT SIDEBAR (33%)
    ├─ Property Type Filter
    ├─ Price Range Filter
    └─ Listing Type Filter
```

---

## ✨ User Flow

1. **Enter Page** → View featured listings + filters
2. **Use Top Filters** → Search by status/type/location
3. **Browse Sidebar** → Refine by type/price/listing
4. **View Cards** → See property details at glance
5. **Click Card/Details** → Navigate to full property view
6. **Paginate** → Load more results
7. **Re-filter** → Adjust criteria and search again

---

## 🚀 Performance Features

✅ Single CSS block (inline styles in view)
✅ No external CSS files needed
✅ Optimized flex/grid layouts
✅ Minimal animations (0.3s smooth)
✅ Responsive images (object-fit-cover)
✅ Fast pagination
✅ Efficient data binding

---

## 🔐 Accessibility Features

✅ Semantic HTML5
✅ Proper heading hierarchy
✅ ARIA labels where needed
✅ Color contrast (WCAG AA)
✅ Keyboard navigation support
✅ Focus indicators
✅ Alternative text for images
✅ Form labels properly associated

---

## 📊 Layout Dimensions Reference

```
DESKTOP VIEW:
Total Width: 1400px (max-width: container-fluid)
Main Col:    933px (66.666%)
Sidebar:     467px (33.333%)
Gutter:      30px (g-4 = 1.5rem × 2)

TABLET VIEW:
Full Width: 100%
Single Column Layout
Sidebar Below Content

MOBILE VIEW:
Full Width: 100%
Single Column Layout
Reduced padding/margins
```

---

## 🎓 Design System Applied

**Color System:**
- Primary: Orange (#FF9500)
- Text: Navy (#1E3A5F)
- Secondary: Gray (#6B7280)
- Surfaces: White (#FFFFFF)

**Spacing Scale:**
- xs: 8px
- sm: 12px
- md: 15px
- lg: 20px
- xl: 25px
- 2xl: 30px

**Border Radius:**
- Input/Button: 6px
- Card/Container: 8px
- Badge: 4px

**Shadows:**
- Light: 0 1px 2px rgba(0,0,0,0.05)
- Medium: 0 8px 20px rgba(0,0,0,0.1)

---

## 📞 Quick Reference

**Property Card:**
- Image Width: 280px
- Image Height: 200px
- Content Padding: 20px
- Badge: Top-left, Orange, 6px × 12px

**Sidebar:**
- Width: 33% on desktop
- Position: Sticky at 80px top
- Sections: 3 main filter groups

**Buttons:**
- Orange (#FF9500) primary color
- White text
- Hover: Darker orange (#FF8C00)

**Pagination:**
- Dot style (not arrow style)
- 4 pages max shown
- Orange active state

---

**Status:** ✅ PRODUCTION READY
**Version:** 1.0
**Last Updated:** March 2024

