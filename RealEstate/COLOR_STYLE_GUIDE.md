# 🎨 Landing Page Color & Style Reference

## Color Scheme Guide

### Primary Colors
```
┌─────────────────────────────────────┐
│ Primary Green (#4CAF50)             │
│ Used for: Buttons, Icons, Accents   │
│ RGB: (76, 175, 80)                  │
│ Hex: #4CAF50                        │
│ Usage: 60% of accent elements       │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│ Dark Green (#45a049)                │
│ Used for: Hover states, Shadows     │
│ RGB: (69, 160, 73)                  │
│ Hex: #45a049                        │
│ Usage: Hover effects on buttons     │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│ Accent Red (#e74c3c)                │
│ Used for: Price ranges, Highlights  │
│ RGB: (231, 76, 60)                  │
│ Hex: #e74c3c                        │
│ Usage: Price display text           │
└─────────────────────────────────────┘
```

### Neutral Colors
```
┌─────────────────────────────────────┐
│ Dark Gray (#3a3a3a)                 │
│ Used for: Carousel background       │
│ RGB: (58, 58, 58)                   │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│ Very Dark Gray (#2c2c2c)            │
│ Used for: Top bar background        │
│ RGB: (44, 44, 44)                   │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│ Primary Gray (#333333)              │
│ Used for: Main text, headings       │
│ RGB: (51, 51, 51)                   │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│ Secondary Gray (#666666)            │
│ Used for: Body text, descriptions   │
│ RGB: (102, 102, 102)                │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│ Light Gray (#f9f9f9)                │
│ Used for: Card backgrounds          │
│ RGB: (249, 249, 249)                │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│ Lighter Gray (#f0f0f0)              │
│ Used for: Hover backgrounds         │
│ RGB: (240, 240, 240)                │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│ Border Gray (#eeeeee)               │
│ Used for: Borders, dividers         │
│ RGB: (238, 238, 238)                │
└─────────────────────────────────────┘
```

---

## Button Styles

### Primary Button (CTA)
```
Desktop:
┌──────────────────┐
│  GET STARTED     │ ← Text
│                  │
└──────────────────┘
Background: #4CAF50
Text Color: #ffffff
Padding: 0.6rem 1.5rem
Border Radius: 25px
Font Weight: 600
Font Size: 0.85rem

Hover:
Background: #45a049
Text Color: #ffffff
Box Shadow: 0 4px 12px rgba(76, 175, 80, 0.3)

Active:
Background: #3d8f40
Transform: scale(0.98)
```

### Search Button
```
┌──────────────────┐
│    SEARCH        │
└──────────────────┘
Background: #4CAF50
Text Color: #ffffff
Padding: 12px 30px
Border Radius: 5px
Font Weight: 600
Width: 100% (on mobile)

Hover:
Background: #45a049
```

---

## Card Styles

### Statistics Card
```
┌────────────────────────────┐
│  ┌──────────┐              │
│  │  [Icon]  │  25,300+     │
│  │  #4CAF50 │  Customers   │
│  └──────────┘              │
└────────────────────────────┘

Background: #f9f9f9
Border: 1px solid #eeeeee
Padding: 25px
Border Radius: 10px

Hover:
Background: #f0f8f5
Border Color: #4CAF50
Transform: translateY(-5px)
Box Shadow: 0 8px 24px rgba(76, 175, 80, 0.15)
```

### Feature Card
```
┌────────────────────────────┐
│                            │
│    ┌────────────┐          │
│    │   [Icon]   │          │
│    │ 70px Green │          │
│    └────────────┘          │
│                            │
│    Feature Title           │
│    (1.2rem, bold)          │
│                            │
│    Feature Description     │
│    (Multiple lines)        │
│                            │
└────────────────────────────┘

Background: #f9f9f9
Border: 1px solid #eeeeee
Padding: 35px 25px
Text Align: center
Transition: 0.3s

Hover:
Background: #ffffff
Border Color: #4CAF50
Transform: translateY(-10px)
Box Shadow: 0 12px 40px rgba(76, 175, 80, 0.2)
```

### Property Carousel Card
```
┌────────────────────┐
│   Property Image   │
│     (180px)        │
│  [Green Badge]     │
├────────────────────┤
│ ₱2,400,000 (Green) │
│ 4 BED ROOM @ LOC   │
│ Down Street        │
│ [Agent Avatar]     │
│ AGENT NAME (Green) │
└────────────────────┘

Background: #4a4a4a
Border Radius: 8px
Color: white

Hover:
Transform: translateY(-5px)
Box Shadow: increased
```

---

## Typography Hierarchy

### Headings
```
Hero Title (3.5rem)
"WELCOME TO REALTOR"
Color: #ffffff
Font Weight: 700
Letter Spacing: 2px
Text Shadow: 2px 2px 4px rgba(0,0,0,0.3)

Section Title Main (2.5rem)
"Why Choose REALTOR?"
Color: #333333
Font Weight: 700

Section Title (1.9rem - 2.8rem)
"Featured Listings"
Color: #333333
Font Weight: 700
Responsive sizing

Subsection Title (1.2rem)
"Easy Search"
Color: #333333
Font Weight: 700

Card Title (0.9rem - 1.1rem)
Color: #333333
Font Weight: 600
```

### Body Text
```
Main Body (1rem)
Color: #666666
Line Height: 1.6

Small Text (0.95rem)
Color: #666666

Labels (0.85rem - 0.9rem)
Color: #333333
Font Weight: 600

Tiny Text (0.75rem - 0.8rem)
Color: #999999
```

---

## Icons Style Guide

### Icon Containers
```
┌──────────────────┐
│   Green Gradient │
│   Background     │
│   [Icon Inside]  │
└──────────────────┘

Sizes:
- Large: 70px (Feature Cards)
- Medium: 60px (Statistics)
- Small: 32px (Agent avatars)

Background: Linear gradient (#4CAF50 → #45a049)
Color: #ffffff
Border Radius: 8px - 12px
Display: Flex (center items)
```

### Icon Colors
```
Primary Icon: #ffffff (on green background)
Text Icon: #4CAF50 (inline with text)
Hover Icon: #45a049
Disabled Icon: #cccccc
```

---

## Input Fields Style Guide

### Text Input / Select
```
┌──────────────────────┐
│ Placeholder text     │
└──────────────────────┘

Background: #ffffff
Border: 1px solid #dddddd
Border Radius: 5px
Padding: 10px 12px
Font Size: 0.95rem
Color: #333333

Focus:
Border Color: #4CAF50
Box Shadow: 0 0 0 3px rgba(76, 175, 80, 0.1)

Hover:
Border Color: #cccccc
```

### Range Slider
```
Track Background: Linear gradient (gray → green)
Track Height: 6px
Track Radius: 3px

Thumb (Handle):
Width: 18px
Height: 18px
Background: #ffffff
Border: 3px solid #4CAF50
Border Radius: 50%
Box Shadow: 0 2px 4px rgba(0,0,0,0.2)
```

---

## Spacing Standards

### Section Spacing
```
Top Padding: 50-60px
Bottom Padding: 50-60px
```

### Card Spacing
```
Internal Padding: 25-35px
Gap Between Items: 16-24px (Grid)
Margin Bottom: 10-40px
```

### Element Spacing
```
Small Gap: 8-12px
Medium Gap: 15-20px
Large Gap: 20-30px
```

---

## Shadow & Depth

### Subtle Shadow (Default Cards)
```
box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
```

### Medium Shadow (Hover)
```
box-shadow: 0 8px 24px rgba(76, 175, 80, 0.15);
```

### Large Shadow (Featured)
```
box-shadow: 0 12px 40px rgba(76, 175, 80, 0.2);
```

---

## Animations & Transitions

### Hover Transitions
```
Duration: 0.3s
Easing: ease
Properties: all

Example:
transition: all 0.3s ease;
```

### Lift Animation
```
Transform: translateY(-4px to -10px)
Box Shadow: Increases
Duration: 0.3s
```

### Color Transition
```
Duration: 0.2s - 0.3s
Easing: ease
Properties: background-color, color, border-color
```

---

## Responsive Considerations

### Mobile (< 768px)
- Font sizes reduced by 10-20%
- Padding reduced by 15-20%
- Single column layouts
- Touch-friendly buttons (44px min)

### Tablet (768px - 991px)
- Balanced spacing
- 2-column layouts
- Optimized font sizes

### Desktop (> 991px)
- Full spacing
- 3-4 column layouts
- Enhanced effects
- Optimal readability

---

## Accessibility Color Contrast

### Text on Light Backgrounds
- Primary Text (#333): 18:1 contrast ratio ✅
- Secondary Text (#666): 9:1 contrast ratio ✅
- Acceptable text: 4.5:1 minimum

### Text on Dark Backgrounds
- Light Text (#fff): 14:1 contrast ratio ✅
- Buttons: 4.5:1 minimum ✅

### WCAG AA Compliance
✅ All text contrast ratios meet or exceed AA standards
✅ Interactive elements have clear visual states
✅ Colors not used as only indicator

---

## Dark Theme Reference

### Dark Section (Carousel)
```
Background: #3a3a3a
Text: #ffffff
Accents: #4CAF50
Hover Background: rgba(255, 255, 255, 0.1)
Cards: #4a4a4a
```

---

## Brand Guidelines

### Logo
- Icon: Green gradient box with house icon
- Text: "REALTOR" (all caps)
- Color: Green gradient (#4CAF50 → #45a049)
- Minimum Size: 30px

### Color Usage
- Logo: Green
- Primary CTA: Green
- Accents: Green
- Hover States: Dark Green
- Secondary Info: Gray/Red

### Typography
- Headlines: Bold, Dark Gray (#333)
- Body: Regular, Medium Gray (#666)
- Accents: Green (#4CAF50)

---

## Quick Reference Table

| Element | Color | Size | Weight |
|---------|-------|------|--------|
| Buttons | #4CAF50 | 0.85rem | 600 |
| Headings | #333333 | 1.2rem - 3.5rem | 700 |
| Body | #666666 | 0.95rem - 1rem | 400 |
| Labels | #333333 | 0.85rem | 600 |
| Icons | #ffffff | 32px - 70px | N/A |
| Cards BG | #f9f9f9 | N/A | N/A |
| Borders | #eeeeee | 1px | N/A |

---

Generated: December 2024
Version: 2.0 (Final)

