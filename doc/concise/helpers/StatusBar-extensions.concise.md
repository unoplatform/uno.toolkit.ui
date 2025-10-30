---
uid: Toolkit.Helpers.StatusBarExtensions
---

# StatusBar Extensions (Concise Reference)

## Summary

Provides two attached properties on `Page` to control the visuals of the status bar on mobile platforms.

## Properties

| Property     | Type                         | Description                                                                                                                        |
|--------------|------------------------------|------------------------------------------------------------------------------------------------------------------------------------|
| `Foreground` | `StatusBarForegroundTheme`\* | Sets the foreground color for the text and icons on the status bar. Possible values are: `None, Light, Dark, Auto or AutoInverse`. |
| `Background` | `Brush`                      | Sets the background color for the status bar. <br/> note: Due to platform limitations, only `SolidColorBrush`es are accepted.      |

## Usage Examples

```xml
<Page ...
      xmlns:utu="using:Uno.Toolkit.UI"
      utu:StatusBar.Foreground="Dark"
      utu:StatusBar.Background="SkyBlue" />
```

---

**Note**: This is a concise reference. 
For complete documentation, see [StatusBar-extensions.md](StatusBar-extensions.md)