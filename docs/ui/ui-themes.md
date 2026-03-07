# UI Themes

## Overview
Demons and Dogs supports two visual themes selectable per campaign.
Theme is stored on the `CampaignResource` and applied at the app level
when a campaign is active. Default theme is Fantasy.

## Theme Implementation
Themes are implemented as CSS custom property sets applied to the `<body>` tag
via a `data-theme` attribute: `<body data-theme="fantasy">` or `<body data-theme="steampunk">`.

A `ThemeService` in `DemonsAndDogs` manages the active theme and persists it
to localStorage so it survives page refreshes.

## Fantasy Theme
**Concept:** Ancient tome, candlelit tavern, parchment and ink.

### Palette
```css
[data-theme="fantasy"] {
  --bg-primary: #1a1209;        /* near-black, warm */
  --bg-secondary: #2c1f0e;      /* dark brown */
  --bg-card: #f4e4c1;           /* aged parchment */
  --bg-card-dark: #2a1a0a;      /* dark card variant */
  --text-primary: #f4e4c1;      /* parchment text on dark */
  --text-on-card: #2c1a0a;      /* dark ink on parchment */
  --accent-primary: #c9973a;    /* antique gold */
  --accent-secondary: #8b2020;  /* deep crimson */
  --accent-glow: #e8b84b;       /* golden glow */
  --border-color: #8b6914;      /* aged gold border */
  --shadow-color: rgba(0,0,0,0.7);
}
```

### Typography
- **Display/headings:** `Cinzel` (Google Fonts) — Roman-inspired serif, regal
- **Body:** `Crimson Text` (Google Fonts) — elegant readable serif
- **Monospace/rolls:** `MedievalSharp` or fallback `Georgia`

### Visual Details
- Parchment texture on cards (CSS noise filter or SVG texture)
- Decorative corner flourishes on cards using CSS border-image or pseudo-elements
- Subtle vignette on page background
- Golden glow on hover states and active elements
- Flickering animation on accent elements (subtle, not distracting)
- Horizontal rules styled as ornamental dividers

### Key Classes
```css
.card-fantasy        /* parchment card with decorative border */
.btn-fantasy         /* aged metal button with gold accent */
.heading-fantasy     /* Cinzel font with letter-spacing */
.divider-fantasy     /* ornamental horizontal rule */
.glow-fantasy        /* golden glow effect */
```

## Steampunk Theme
**Concept:** Victorian inventor's workshop, brass and iron, gas lamps.

### Palette
```css
[data-theme="steampunk"] {
  --bg-primary: #0d0d0d;        /* near-black */
  --bg-secondary: #1a1208;      /* very dark brown-black */
  --bg-card: #1c1208;           /* dark leather card */
  --text-primary: #d4a853;      /* warm brass text */
  --text-on-card: #c8a96e;      /* aged brass on dark */
  --accent-primary: #b87333;    /* copper */
  --accent-secondary: #8b6914;  /* aged brass */
  --accent-glow: #ff6b1a;       /* hot ember orange */
  --border-color: #8b6914;      /* brass border */
  --shadow-color: rgba(0,0,0,0.8);
}
```

### Typography
- **Display/headings:** `Special Elite` (Google Fonts) — typewriter, mechanical
- **Body:** `Libre Baskerville` (Google Fonts) — Victorian-era serif
- **Monospace/rolls:** `Courier Prime`

### Visual Details
- Riveted metal texture on cards using CSS repeating gradients
- Gear/cog decorative elements on section headers (CSS or SVG)
- Amber/orange glow on hover (ember effect)
- Steam/smoke subtle animation on certain elements
- Horizontal rules styled as pipe fittings
- Copper and brass metallic gradients on buttons

### Key Classes
```css
.card-steampunk      /* riveted dark metal card */
.btn-steampunk       /* brass button with copper gradient */
.heading-steampunk   /* Special Elite with mechanical styling */
.divider-steampunk   /* pipe-fitting horizontal rule */
.glow-steampunk      /* ember orange glow effect */
```

## ThemeService
```csharp
// DemonsAndDogs/Services/ThemeService.cs
public class ThemeService
{
    public string CurrentTheme { get; private set; } = "fantasy";
    public event Action? OnThemeChanged;

    public void SetTheme(string theme)
    {
        CurrentTheme = theme;
        OnThemeChanged?.Invoke();
    }
}
```

Applied in `MainLayout.razor`:
```razor
<body data-theme="@ThemeService.CurrentTheme">
```

## Per-Campaign Theme
`CampaignResource` has a `Theme` property (`"fantasy"` or `"steampunk"`).
When a player enters a session, `ThemeService.SetTheme(campaign.Theme)` is called.
Theme reverts to default (`"fantasy"`) when leaving a session.

## Google Fonts to Import
```html
<!-- In index.html -->
<link href="https://fonts.googleapis.com/css2?family=Cinzel:wght@400;600;700&family=Crimson+Text:ital,wght@0,400;0,600;1,400&family=Special+Elite&family=Libre+Baskerville:ital,wght@0,400;0,700;1,400&display=swap" rel="stylesheet">
```
