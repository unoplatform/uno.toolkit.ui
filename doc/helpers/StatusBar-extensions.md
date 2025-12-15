---
uid: Toolkit.Helpers.StatusBarExtensions
---

# StatusBar Extensions

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

Provides two attached properties on `Page` to control the visuals of the status bar on mobile platforms.

> [!Video https://www.youtube-nocookie.com/embed/Mef71b6978s]

> [!IMPORTANT]
> **Always set StatusBar properties on every page**
>
> On mobile platforms (iOS, Android), you should set `utu:StatusBar.Foreground` and optionally `utu:StatusBar.Background` on every page to ensure consistent appearance.
>
> **Recommended pattern for all pages:**
> ```xml
> <Page xmlns:utu="using:Uno.Toolkit.UI"
>       utu:StatusBar.Foreground="Auto"
>       utu:StatusBar.Background="{ThemeResource SystemControlBackgroundChromeMediumLowBrush}">
>     <!-- Page content -->
> </Page>
> ```
>
> **Common scenarios:**
> - Light pages: `utu:StatusBar.Foreground="Dark"` with light background
> - Dark pages: `utu:StatusBar.Foreground="Light"` with dark background
> - Theme-aware: `utu:StatusBar.Foreground="Auto"` to adapt automatically

## Remarks

The attached properties do nothing on platforms other than iOS and Android.
For iOS, `UIViewControllerBasedStatusBarAppearance` should be set to false in `info.plist`.

## Attached Properties

| Property     | Type                         | Description                                                                                                                        |
|--------------|------------------------------|------------------------------------------------------------------------------------------------------------------------------------|
| `Foreground` | `StatusBarForegroundTheme`\* | Sets the foreground color for the text and icons on the status bar. Possible values are: `None, Light, Dark, Auto or AutoInverse`. |
| `Background` | `Brush`                      | Sets the background color for the status bar. <br/> note: Due to platform limitations, only `SolidColorBrush`es are accepted.      |

`StatusBarForegroundTheme`\*: `Auto` and `AutoInverse` will set the foreground in accordance to the theme, and update itself when the system theme or the app theme changes:

- `Auto`: light/white in the dark mode, and dark/black in the light mode
- `AutoInverse`: dark/black in the dark mode, and light/white in the light mode

## Usage

```xml
<Page ...
      xmlns:utu="using:Uno.Toolkit.UI"
      utu:StatusBar.Foreground="Dark"
      utu:StatusBar.Background="SkyBlue" />
```
