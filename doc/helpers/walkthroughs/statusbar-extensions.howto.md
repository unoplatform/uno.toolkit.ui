# How to customize the status bar appearance in Uno applications

The following how-tos show how to control the appearance of the platform **status bar** (system top bar) in Uno applications using attached properties.

```xml
xmlns:utu="using:Uno.Toolkit.UI"
```

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

> [!IMPORTANT]
> **Always set StatusBar properties on every page**
>
> Every Page in your app should include `utu:StatusBar.Foreground` and optionally `utu:StatusBar.Background` to ensure proper appearance on iOS and Android.

---

## Default pattern for all pages

**Goal:** Set up proper status bar styling on every page you create.

**XAML**

```xml
<Page
    x:Class="MyApp.MyPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:utu="using:Uno.Toolkit.UI"
    utu:StatusBar.Foreground="Auto"
    utu:StatusBar.Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
    
    <Grid>
        <!-- Page content -->
    </Grid>
</Page>
```

**Why this is essential:**

- Without StatusBar settings, the system status bar may have poor contrast with your page
- Using `Auto` for Foreground automatically adapts to light/dark themes
- Matching Background to your page theme creates a seamless, branded appearance
- This pattern should be applied to every page in your app, not just the main page

**Quick rules:**

- White/light pages → `Foreground="Dark"`
- Dark pages → `Foreground="Light"`
- Theme-adaptive pages → `Foreground="Auto"`

---

## Change the status bar background color

Use this to match the system status bar with your page color.

```xml
<Page
    x:Class="MyApp.HomePage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:utu="using:Uno.Toolkit.UI"
    utu:StatusBar.Background="SteelBlue">
    
    <Grid>
        <!-- Page content -->
    </Grid>
</Page>
```

**Why**
Gives your app a seamless, branded look across mobile devices.

---

## Use light icons on dark backgrounds

```xml
<Page
    ...
    xmlns:utu="using:Uno.Toolkit.UI"
    utu:StatusBar.Background="#003366"
    utu:StatusBar.Foreground="Light">
    
    <Grid />
</Page>
```

**Why**
When your app’s top area is dark, use `Light` so the system icons (battery, clock, etc.) stay readable.

---

## Use dark icons on light backgrounds

```xml
<Page
    ...
    xmlns:utu="using:Uno.Toolkit.UI"
    utu:StatusBar.Background="White"
    utu:StatusBar.Foreground="Dark">
    
    <Grid />
</Page>
```

**Why**
Makes icons visible on bright or white surfaces.

---

## Let the system choose automatically

```xml
<Page
    ...
    xmlns:utu="using:Uno.Toolkit.UI"
    utu:StatusBar.Background="{ThemeResource SystemControlBackgroundAccentBrush}"
    utu:StatusBar.Foreground="Auto">
    
    <Grid />
</Page>
```

**Why**
Lets Uno determine the best icon color based on the system theme or background brightness.

---

## Force inverse icon color automatically

```xml
<Page
    ...
    xmlns:utu="using:Uno.Toolkit.UI"
    utu:StatusBar.Background="#FF6200EE"
    utu:StatusBar.Foreground="AutoInverse">
    
    <Grid />
</Page>
```

**Why**
When `Auto` doesn’t give enough contrast, `AutoInverse` flips the theme logic for better readability.

---

## Update the status bar in code

```csharp
using Uno.Toolkit.UI;
using Windows.UI;

public sealed partial class DetailsPage : Page
{
    public DetailsPage()
    {
        this.InitializeComponent();
    }

    private void OnThemeChanged(object sender, RoutedEventArgs e)
    {
        // Set background color and foreground style programmatically
        StatusBar.SetBackground(this, Colors.Black);
        StatusBar.SetForeground(this, StatusBarForegroundTheme.Light);
    }
}
```

**Why**
Allows you to react to runtime changes, like user theme switching or media playback mode.

---

## Foreground property values

| Value         | Description                              |
| ------------- | ---------------------------------------- |
| `None`        | Leaves current icon/text color unchanged |
| `Light`       | Uses bright icons for dark backgrounds   |
| `Dark`        | Uses dark icons for light backgrounds    |
| `Auto`        | Chooses based on theme or luminance      |
| `AutoInverse` | Inverts automatic choice for contrast    |

---

## Platform notes

- **Supported platforms:** Android, iOS, Windows.
- **No-op platforms:** WebAssembly, Skia (desktop).
- These attached properties only affect the visual style, not visibility or animation.

---

## FAQ

**Q: Do I need to add StatusBar settings to every Page?**

Yes, for mobile apps. Each Page should have at least `utu:StatusBar.Foreground="Auto"` to ensure proper appearance across light/dark themes and different page backgrounds.

**Q: What's the safest default for StatusBar.Foreground?**

Use `Auto` - it automatically chooses light/white in dark mode and dark/black in light mode, adapting to theme changes.

**Q: Should I set StatusBar.Background on every page?**

Yes, recommended. Match it to your page's top background color for a seamless look. Use theme resources like `{ThemeResource ApplicationPageBackgroundThemeBrush}` for consistency.

**Q: What if I forget to set StatusBar on a page?**

The status bar will keep the settings from the previous page, which may cause poor contrast or branding inconsistency when navigating between pages.

**Q: Can I use the same StatusBar settings on all pages?**

If all your pages have the same background color and theme, yes. But it's best practice to set it explicitly on each page for clarity and to handle per-page customization.

**Q: What StatusBar settings should I use for a login page?**

Match your page's theme:

```xml
<!-- Light themed login -->
<Page utu:StatusBar.Foreground="Dark"
      utu:StatusBar.Background="White" />

<!-- Dark themed login -->
<Page utu:StatusBar.Foreground="Light"
      utu:StatusBar.Background="#1E1E1E" />

<!-- Theme-adaptive login -->
<Page utu:StatusBar.Foreground="Auto"
      utu:StatusBar.Background="{ThemeResource ApplicationPageBackgroundThemeBrush}" />
```
