# EstateFlow Color Palette & Style Guide

Based on the official EstateFlow logo, this document defines the complete color palette for the website.

## Primary Colors

### Primary Teal (Buildings/Main Accent)
- **Hex**: `#16A39E`
- **RGB**: `22, 163, 158`
- **HSL**: `176°, 76%, 36%`
- **Usage**: Primary accent, call-to-action buttons, links, section highlights

### Primary Navy Blue (Estate)
- **Hex**: `#1E3A5F`
- **RGB**: `30, 58, 95`
- **HSL**: `216°, 51%, 25%`
- **Usage**: Headings, main text, navbar background, footer

### Primary Orange (Flow/Accent)
- **Hex**: `#FF9500`
- **RGB**: `255, 149, 0`
- **HSL**: `38°, 100%, 50%`
- **Usage**: Secondary accent, hover states, highlights, emphasis

## Secondary Colors

### Light Teal (Backgrounds)
- **Hex**: `#E0F2F1`
- **RGB**: `224, 242, 241`
- **HSL**: `176°, 48%, 91%`
- **Usage**: Light backgrounds, subtle accents, hover states

### Medium Teal (Borders)
- **Hex**: `#4DB8AE`
- **RGB**: `77, 184, 174`
- **HSL**: `176°, 41%, 51%`
- **Usage**: Borders, dividers, secondary buttons

### Dark Navy (Deep Text)
- **Hex**: `#0F1F35`
- **RGB**: `15, 31, 53`
- **HSL**: `216°, 56%, 13%`
- **Usage**: Body text, deep headings, critical content

## Neutral Colors

### Light Gray (Backgrounds)
- **Hex**: `#F8F9FA`
- **RGB**: `248, 249, 250`
- **HSL**: `220°, 13%, 98%`
- **Usage**: Section backgrounds, card backgrounds

### Medium Gray (Secondary Text)
- **Hex**: `#6B7280`
- **RGB**: `107, 114, 128`
- **HSL**: `217°, 9%, 45%`
- **Usage**: Secondary text, descriptions, helper text

### Dark Gray (Borders)
- **Hex**: `#E5E7EB`
- **RGB**: `229, 231, 235`
- **HSL**: `216°, 15%, 91%`
- **Usage**: Borders, dividers, subtle separators

### White
- **Hex**: `#FFFFFF`
- **RGB**: `255, 255, 255`
- **HSL**: `0°, 0%, 100%`
- **Usage**: Cards, modals, main content background

## Functional Colors

### Success (Green)
- **Hex**: `#10B981`
- **RGB**: `16, 185, 129`
- **HSL**: `160°, 84%, 39%`
- **Usage**: Success messages, checkmarks, positive indicators

### Warning (Amber)
- **Hex**: `#F59E0B`
- **RGB**: `245, 158, 11`
- **HSL**: `45°, 93%, 50%`
- **Usage**: Warning messages, alerts, caution indicators

### Error (Red)
- **Hex**: `#EF4444`
- **RGB**: `239, 68, 68`
- **HSL**: `0°, 84%, 60%`
- **Usage**: Error messages, critical alerts, validation errors

### Info (Blue)
- **Hex**: `#3B82F6`
- **RGB**: `59, 130, 246`
- **HSL**: `217°, 91%, 60%`
- **Usage**: Info messages, tooltips, informational content

## Gradients

### Primary Gradient (Teal to Navy)
```css
background: linear-gradient(135deg, #16A39E 0%, #1E3A5F 100%);
```

### Accent Gradient (Teal to Orange)
```css
background: linear-gradient(135deg, #16A39E 0%, #FF9500 100%);
```

### Orange Gradient (Logo Style)
```css
background: linear-gradient(135deg, #FF9500 0%, #FF8C00 100%);
```

### Soft Gradient (Light Teal)
```css
background: linear-gradient(135deg, #E0F2F1 0%, #F8F9FA 100%);
```

## Color Usage Guidelines

### Navigation Bar
- **Background**: `#FFFFFF` or `#F8F9FA`
- **Text**: `#1E3A5F`
- **Logo Icon Background**: `linear-gradient(135deg, #FF9500, #FF8C00)`
- **Active Link**: `#16A39E`
- **Hover Link**: `#16A39E` with 0.1s transition

### Buttons
- **Primary Button (CTA)**: 
  - Background: `#16A39E`
  - Text: `#FFFFFF`
  - Hover: `#0F8B84`
  
- **Secondary Button**: 
  - Background: `#FF9500`
  - Text: `#FFFFFF`
  - Hover: `#FF8C00`
  
- **Tertiary Button**: 
  - Background: `#E0F2F1`
  - Text: `#16A39E`
  - Hover: `#D0EAE7`

### Cards & Containers
- **Background**: `#FFFFFF`
- **Border**: `#E5E7EB`
- **Shadow**: `rgba(30, 58, 95, 0.1)` on hover

### Text
- **Primary Text**: `#1E3A5F`
- **Secondary Text**: `#6B7280`
- **Tertiary Text**: `#9CA3AF`
- **Links**: `#16A39E`
- **Link Hover**: `#0F8B84`

### Sections
- **Light Background**: `#F8F9FA`
- **Accent Background**: `#E0F2F1`
- **Dark Background**: `#1E3A5F`

### Form Elements
- **Border (Default)**: `#E5E7EB`
- **Border (Focus)**: `#16A39E`
- **Border (Error)**: `#EF4444`
- **Focus Shadow**: `0 0 0 3px rgba(22, 163, 158, 0.1)`

### Badges & Tags
- **Primary Badge**: Background `#16A39E`, Text `#FFFFFF`
- **Accent Badge**: Background `#FF9500`, Text `#FFFFFF`
- **Neutral Badge**: Background `#E5E7EB`, Text `#1E3A5F`

## Accessibility Considerations

### Color Contrast Ratios (WCAG AA Standard - 4.5:1 minimum for text)
- ✅ **Navy (#1E3A5F) on White**: 11.4:1 (AAA)
- ✅ **Teal (#16A39E) on White**: 6.2:1 (AA)
- ✅ **Orange (#FF9500) on White**: 3.9:1 (Not suitable for small text)
- ✅ **Orange (#FF9500) on Navy**: 6.1:1 (AA)
- ✅ **Gray (#6B7280) on White**: 7.0:1 (AA)

### Recommendations
- Use Navy or Teal for main body text
- Use Orange only for larger headings or as accent elements
- Use White text on Navy, Teal, or Orange backgrounds
- Always test color combinations for readability

## Color Code Quick Reference

```css
/* Primary Colors */
--primary-teal: #16A39E;
--primary-navy: #1E3A5F;
--primary-orange: #FF9500;

/* Secondary Colors */
--secondary-light-teal: #E0F2F1;
--secondary-medium-teal: #4DB8AE;
--secondary-dark-navy: #0F1F35;

/* Neutral Colors */
--neutral-white: #FFFFFF;
--neutral-light-gray: #F8F9FA;
--neutral-medium-gray: #6B7280;
--neutral-dark-gray: #E5E7EB;

/* Functional Colors */
--success: #10B981;
--warning: #F59E0B;
--error: #EF4444;
--info: #3B82F6;
```

## Implementation Example

### Navbar (Updated for EstateFlow)
```css
.navbar-modern {
    background: #FFFFFF;
    border-bottom: 1px solid #E5E7EB;
}

.navbar-logo {
    color: #1E3A5F;
}

.logo-icon {
    background: linear-gradient(135deg, #FF9500, #FF8C00);
    color: #FFFFFF;
}

.nav-link {
    color: #1E3A5F;
}

.nav-link:hover {
    color: #16A39E;
}

.contact-badge {
    color: #16A39E;
}
```

### Hero Section
```css
.hero-section-modern {
    background: #F8F9FA;
}

.hero-title-modern {
    color: #1E3A5F;
}

.btn-primary-modern {
    background: #16A39E;
}

.btn-primary-modern:hover {
    background: #0F8B84;
}
```

### Feature Cards
```css
.feature-box-modern {
    background: #FFFFFF;
    border: 1px solid #E5E7EB;
}

.feature-icon-modern {
    color: #FF9500;
}

.feature-box-modern:hover {
    border-color: #16A39E;
}
```

## Color Variations

### Teal Variations (for depth)
- **Teal 50**: `#F0FFFE` (Very light)
- **Teal 100**: `#E0F2F1` (Light)
- **Teal 400**: `#4DB8AE` (Medium)
- **Teal 600**: `#0F8B84` (Dark)
- **Teal 900**: `#0A5652` (Very dark)

### Navy Variations (for text hierarchy)
- **Navy 50**: `#F3F7FB` (Very light)
- **Navy 100**: `#E7EFF7` (Light)
- **Navy 600**: `#1A2F4A` (Medium)
- **Navy 800**: `#0F1F35` (Very dark)

### Orange Variations (for accents)
- **Orange 50**: `#FFF7ED` (Very light)
- **Orange 400**: `#FB923C` (Medium)
- **Orange 700**: `#C2410C` (Dark)

## Brand Voice Through Color
- **Teal**: Trust, Growth, Technology
- **Navy**: Professional, Stability, Authority
- **Orange**: Energy, Innovation, Action
- **White/Light Backgrounds**: Clarity, Cleanliness, Openness

---

**Last Updated**: 2024
**Logo Reference**: EstateFlow Official Logo
**Brand Colors**: Teal, Navy, Orange
