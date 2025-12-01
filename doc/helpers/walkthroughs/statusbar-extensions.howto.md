# How to customize the status bar appearance in Uno applications

The following how-tos show how to control the appearance of the platform **status bar** (system top bar) in Uno applications using attached properties.

```xml
xmlns:utu="using:Uno.Toolkit.UI"
```

**UnoFeature:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

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

* **Supported platforms:** Android, iOS, Windows.
* **No-op platforms:** WebAssembly, Skia (desktop).
* These attached properties only affect the visual style, not visibility or animation.
