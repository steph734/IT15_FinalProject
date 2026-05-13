# Landing Page Structure & Layout Guide

## Page Flow (Top to Bottom)

```
┌─────────────────────────────────────────────┐
│         NAVBAR (_LandingNavbar)             │
│  ┌────────────────────────────────────────┐ │
│  │ [Phone] [Email] [Login/Register]       │ │
│  └────────────────────────────────────────┘ │
│  ┌────────────────────────────────────────┐ │
│  │ [Logo] Menu Items... [GET STARTED BTN] │ │
│  └────────────────────────────────────────┘ │
└─────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────┐
│         HERO SECTION                        │
│  ┌────────────────────────────────────────┐ │
│  │  Background Image with Overlay         │ │
│  │                                        │ │
│  │    "WELCOME TO REALTOR"                │ │
│  │                                        │ │
│  │  ┌──────────────────────────────────┐ │ │
│  │  │   SEARCH FORM (2 rows)           │ │ │
│  │  │   City | Location | Status | Type│ │ │
│  │  │   BR | BA | Price Range | SEARCH│ │ │
│  │  └──────────────────────────────────┘ │ │
│  └────────────────────────────────────────┘ │
└─────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────┐
│     STATISTICS SECTION                      │
│  ┌──────┐  ┌──────┐  ┌──────┐  ┌──────┐    │
│  │ 25K+ │  │ 45K+ │  │ 50+  │  │ 800+ │    │
│  │Cust. │  │Props │  │Cities│  │Agents│    │
│  └──────┘  └──────┘  └──────┘  └──────┘    │
└─────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────┐
│     CAROUSEL SECTION (Dark Background)      │
│  [◀]                                    [▶] │
│  ┌─────┐  ┌─────┐  ┌─────┐  ┌─────┐       │
│  │ P1  │  │ P2  │  │ P3  │  │ P4  │       │
│  │Image│  │Image│  │Image│  │Image│       │
│  │ Info│  │ Info│  │ Info│  │ Info│       │
│  └─────┘  └─────┘  └─────┘  └─────┘       │
└─────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────┐
│   WHY CHOOSE US SECTION (NEW)               │
│  ┌───────┐ ┌───────┐ ┌───────┐             │
│  │ Easy  │ │Secure │ │ 24/7  │             │
│  │Search │ │Trans. │ │Support│             │
│  └───────┘ └───────┘ └───────┘             │
│  ┌───────┐ ┌───────┐ ┌───────┐             │
│  │ Top   │ │ Quick │ │ Wide  │             │
│  │Agents │ │Process│ │Cover. │             │
│  └───────┘ └───────┘ └───────┘             │
└─────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────┐
│    FEATURED LISTINGS SECTION                │
│  "Featured Listings"                        │
│  "Explore our latest properties"            │
│  ┌──────┐  ┌──────┐  ┌──────┐              │
│  │Card 1│  │Card 2│  │Card 3│              │
│  │      │  │      │  │      │              │
│  │Details│  │Details│  │Details│              │
│  └──────┘  └──────┘  └──────┘              │
└─────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────┐
│    CONTACT WITH AGENTS SECTION              │
│  [Agent Feature Card] | [4 Agent Cards]     │
└─────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────┐
│         JOURNEY SECTION                     │
│      [1] → [2] → [3] → [4]                  │
└─────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────┐
│         TRUST SECTION                       │
│   [Image] | [Content]                       │
└─────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────┐
│      TESTIMONIALS SECTION                   │
│   [Testimonial 1] [Large] [Testimonial 3]   │
└─────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────┐
│         FOOTER                              │
│  Newsletter | Company Links | Social        │
└─────────────────────────────────────────────┘
```

---

## Navbar Structure

### Top Bar
```
┌────────────────────────────────────────┐
│ [Call Icon] Call Us Now : +60 123-456  │
│ [Email Icon] info@realtor.com          │
│                    [Login/Register]    │
└────────────────────────────────────────┘
```

### Navigation Bar
```
┌────────────────────────────────────────────────┐
│ [House Icon] REALTOR | HOME | PROPERTIES |    │
│ SERVICES | ABOUT | TEAM | BLOG | CONTACT |    │
│                              [GET STARTED] (Green)
└────────────────────────────────────────────────┘
```

---

## Hero Section - Search Form Grid

### Desktop Layout (4 columns, 2 rows)
```
┌────────────┬────────────┬────────────┬────────────┐
│   City     │  Location  │   Status   │    Type    │
├────────────┼────────────┼────────────┼────────────┤
│ Bedrooms   │ Bathrooms  │ Price Range│   SEARCH   │
└────────────┴────────────┴────────────┴────────────┘
```

### Tablet Layout (2 columns, 3 rows)
```
┌────────────┬────────────┐
│   City     │  Location  │
├────────────┼────────────┤
│   Status   │    Type    │
├────────────┼────────────┤
│ Bedrooms   │ Bathrooms  │
├────────────┴────────────┤
│ Price Range │  SEARCH    │
└────────────┴────────────┘
```

### Mobile Layout (1 column)
```
┌────────────┐
│   City     │
├────────────┤
│ Location   │
├────────────┤
│   Status   │
├────────────┤
│    Type    │
├────────────┤
│ Bedrooms   │
├────────────┤
│ Bathrooms  │
├────────────┤
│Price Range │
├────────────┤
│   SEARCH   │
└────────────┘
```

---

## Statistics Cards

### Design
```
┌─────────────────────────┐
│ ┌──────────────────┐    │
│ │    [Icon Box]    │    │
│ │   Green Grad.    │    │
│ └──────────────────┘    │
│                         │
│    25,300+              │
│ Happy Customers         │
└─────────────────────────┘
```

### Colors
- Icon Background: Linear gradient (#4CAF50 → #45a049)
- Number: #333 (dark)
- Label: #666 (medium)
- Card Background: #f9f9f9
- Hover: Light green tint

---

## Why Choose Us - Feature Cards (3x2 Grid)

### Single Card
```
┌────────────────────────┐
│                        │
│    [Icon Box - 70px]   │
│      Green Gradient    │
│                        │
│    Card Title          │
│                        │
│   Card Description     │
│   (Multiple lines)     │
│                        │
└────────────────────────┘
```

### Grid Layout
```
Desktop (3 columns):
┌─────┐ ┌─────┐ ┌─────┐
│ 1   │ │ 2   │ │ 3   │
└─────┘ └─────┘ └─────┘
┌─────┐ ┌─────┐ ┌─────┐
│ 4   │ │ 5   │ │ 6   │
└─────┘ └─────┘ └─────┘

Tablet (2 columns):
┌─────┐ ┌─────┐
│ 1   │ │ 2   │
└─────┘ └─────┘
┌─────┐ ┌─────┐
│ 3   │ │ 4   │
└─────┘ └─────┘
┌─────┐ ┌─────┐
│ 5   │ │ 6   │
└─────┘ └─────┘

Mobile (1 column):
┌─────┐
│ 1   │
└─────┘
┌─────┐
│ 2   │
└─────┘
... etc
```

---

## Carousel Section

### Dark Theme Container
```
Background: #3a3a3a (Dark Gray)
Padding: 60px 20px

Structure:
[◀ Button] [Carousel Scroll Area] [▶ Button]

Item Size: 280px (Desktop) / 240px (Tablet) / 200px (Mobile)
```

### Property Card
```
┌──────────────────┐
│     Image        │
│  (180px height)  │
│                  │
│  [Green Badge]   │
├──────────────────┤
│ ₱PRICE (Green)   │
│ 4 BED ROOM       │
│ @ LOCATION       │
├──────────────────┤
│ [Avatar] Name    │
│ (Green Text)     │
└──────────────────┘
```

---

## Color Palette

### Primary Colors
- **Main Green**: `#4CAF50` - Buttons, icons, accents
- **Dark Green**: `#45a049` - Hover states
- **Dark Gray**: `#3a3a3a` - Carousel background
- **Very Dark**: `#2c2c2c` - Top bar

### Text Colors
- **Primary**: `#333` - Main text
- **Secondary**: `#666` - Descriptions
- **Light**: `#999` - Subtle text
- **Muted**: `#ccc` - Disabled/inactive

### Background Colors
- **White**: `#fff` - Cards, main sections
- **Light Gray**: `#f9f9f9` - Card backgrounds
- **Lighter Gray**: `#f0f0f0` - Hover states
- **Pale Green**: `#f0f8f5` - Green hover tint

### Borders
- **Light**: `#eee` - Main borders
- **Medium**: `#ddd` - Input borders
- **Dark**: `#444` - Dark theme borders

---

## Typography

### Headings
- **Hero Title**: 3.5rem (responsive down to 1.4rem)
- **Section Title**: clamp(1.9rem, 3vw, 2.8rem)
- **Main Section Title**: 2.5rem
- **Feature Title**: 1.2rem
- **Card Titles**: 0.9rem - 1.1rem

### Body Text
- **Regular**: 0.95rem - 1rem
- **Small**: 0.85rem - 0.9rem
- **Tiny**: 0.75rem - 0.8rem

### Font Weights
- **Regular**: 400
- **Medium**: 500
- **Semibold**: 600
- **Bold**: 700

---

## Spacing Standards

### Padding
- Section: 50-60px vertical
- Card: 25-35px
- Small Gap: 8-12px
- Medium Gap: 15-20px
- Large Gap: 20-30px

### Margins
- Section Title: 10-40px bottom
- Card Grid Gap: 16-24px

---

## Effects & Animations

### Hover Effects
- **Cards**: Lift up 5-10px with shadow increase
- **Buttons**: Background color change (0.3s)
- **Links**: Color change to green (0.3s)

### Transitions
- Duration: 0.2s - 0.3s
- Easing: ease / ease-in-out
- Properties: transform, background-color, box-shadow, color

---

## Responsive Behavior

### Mobile Priority
- Single column by default
- Stack elements vertically
- Full width inputs
- Larger touch targets (44px minimum)

### Tablet Optimization
- 2-3 column grids
- Balanced spacing
- Optimized typography

### Desktop Enhancement
- Full feature display
- 4-6 column grids
- Enhanced spacing and effects

---

## Accessibility Considerations

✅ Semantic HTML structure
✅ ARIA labels on interactive elements
✅ Color contrast ratios met (WCAG AA)
✅ Keyboard navigation support
✅ Focus states on all interactive elements
✅ Responsive text sizing
✅ Mobile-friendly touch targets

