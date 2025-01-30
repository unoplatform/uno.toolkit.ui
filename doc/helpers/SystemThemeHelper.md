---
uid: Toolkit.Helpers.SystemThemeHelper
---

# SystemThemeHelper

Provides utilities for managing and retrieving the current theme of the operating system and the application.

## Theme Retrieval

### Methods

| Method                       | Return Type             | Description                                                                                                   |
|------------------------------|-------------------------|---------------------------------------------------------------------------------------------------------------|
| `GetCurrentOsTheme()`        | `ApplicationTheme`      | Retrieves the current theme of the operating system.                          |
| `GetApplicationTheme()`      | `ApplicationTheme`      | **[Obsolete]** Retrieves the current theme of the application. Will be removed in future versions.           |
| `GetRootTheme(XamlRoot?)`    | `ApplicationTheme`      | Gets the `ApplicationTheme` of the provided `XamlRoot`, or falls back to the operating system theme.          |
| `IsAppInDarkMode()`          | `bool`                 | **[Obsolete]** Returns `true` if the application is currently in dark mode, otherwise `false`.               |
| `IsRootInDarkMode(XamlRoot)` | `bool`                 | Determines if the provided `XamlRoot` is in dark mode.                                                       |

### Usage

Example of retrieving the current OS theme:

```csharp
var osTheme = SystemThemeHelper.GetCurrentOsTheme();
Console.WriteLine($"Current OS Theme: {osTheme}");
```

Retrieve the application theme:

```csharp
var xamlRoot = someElement.XamlRoot;
var appTheme = SystemThemeHelper.GetRootTheme(xamlRoot);
Console.WriteLine($"Current App Theme: {appTheme}");
```

## Theme Management

### Methods

| Method                              | Description                                                                                  |
|-------------------------------------|----------------------------------------------------------------------------------------------|
| `SetApplicationTheme(bool)`         | **[Obsolete]** Sets the application theme to dark mode if `darkMode` is `true`, otherwise light mode. Will be removed in future versions. |
| `SetRootTheme(XamlRoot?, bool)`     | Sets the theme for the provided `XamlRoot` based on the `darkMode` parameter.              |
| `SetApplicationTheme(XamlRoot?, ElementTheme)` | Sets the `ElementTheme` (`Light` or `Dark`) for the provided `XamlRoot`. |
| `ToggleApplicationTheme()`          | **[Obsolete]** Toggles the application theme between light and dark modes. Will be removed in future versions. |

### Usage

Set the theme for a specific `XamlRoot`:

```csharp
var xamlRoot = someElement.XamlRoot;
SystemThemeHelper.SetApplicationTheme(xamlRoot, ElementTheme.Light);
Console.WriteLine("XamlRoot theme set to Light Mode.");
```
