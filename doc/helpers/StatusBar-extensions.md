---
uid: Toolkit.Helpers.StatusBarExtensions
---

# StatusBar Extensions

Provides two attached properties on `Page` to control the visuals of the status bar on mobile platforms.

> [!Video https://www.youtube-nocookie.com/embed/Mef71b6978s]

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
