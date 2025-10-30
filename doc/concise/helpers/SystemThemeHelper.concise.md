---
uid: Toolkit.Helpers.SystemThemeHelper
---

# SystemThemeHelper (Concise Reference)

## Summary

Provides utilities for managing and retrieving the current theme of the operating system and the application.

## Usage Examples

```csharp
var osTheme = SystemThemeHelper.GetCurrentOsTheme();
Console.WriteLine($"Current OS Theme: {osTheme}");
```

```csharp
var xamlRoot = someElement.XamlRoot;
var appTheme = SystemThemeHelper.GetRootTheme(xamlRoot);
Console.WriteLine($"Current App Theme: {appTheme}");
```

```csharp
var xamlRoot = someElement.XamlRoot;
SystemThemeHelper.SetApplicationTheme(xamlRoot, ElementTheme.Light);
Console.WriteLine("XamlRoot theme set to Light Mode.");
```

---

**Note**: This is a concise reference. 
For complete documentation, see [SystemThemeHelper.md](SystemThemeHelper.md)