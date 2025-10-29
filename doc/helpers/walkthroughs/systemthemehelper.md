# Toggle app theme at runtime

**Goal:** Read current theme and switch Light/Dark from code.

## Dependencies
- `Uno.Toolkit.UI` (or `Uno.Toolkit.WinUI`)
- Optional: `CommunityToolkit.WinUI.UI` for `ThemeListener` (to observe OS changes)

## Detect current theme for a XamlRoot
```csharp
using Uno.Toolkit.UI;

bool isDark = SystemThemeHelper.IsRootInDarkMode(this.XamlRoot);
```

## Change the app theme for a XamlRoot
```csharp
using Uno.Toolkit.UI;

SystemThemeHelper.SetApplicationTheme(this.XamlRoot, ElementTheme.Dark);
// or ElementTheme.Light / ElementTheme.Default
```

## React to OS theme changes (optional)
```csharp
using CommunityToolkit.WinUI.UI.Helpers;
using Uno.Toolkit.UI;

ThemeListener.Current.ThemeChanged += _ =>
{
    var theme = SystemThemeHelper.IsRootInDarkMode(this.XamlRoot)
        ? ElementTheme.Light
        : ElementTheme.Dark;

    SystemThemeHelper.SetApplicationTheme(this.XamlRoot, theme);
};
```
