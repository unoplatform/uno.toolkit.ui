---
uid: Toolkit.Helpers.SystemThemeHelper
---

# SystemThemeHelper

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

Provides utilities for managing and retrieving the current theme of the operating system and the application.

## Theme Retrieval

### Methods

| Method                       | Return Type             | Description                                                                                                   |
|------------------------------|-------------------------|---------------------------------------------------------------------------------------------------------------|
| `GetCurrentOsTheme()`        | `ApplicationTheme`      | Retrieves the current theme of the operating system.                          |
| `GetApplicationTheme()`      | `ApplicationTheme`      | **[Obsolete]** Retrieves the current theme of the application. Will be removed in future versions.           |
| `GetRootTheme(XamlRoot?)`    | `ApplicationTheme`      | Gets the `ApplicationTheme` of the provided `XamlRoot`, or falls back to the operating system theme.          |
| `GetRootTheme(FrameworkElement?)` | `ApplicationTheme` | Gets the `ApplicationTheme` of the provided root element, or falls back to the operating system theme. **Preferred**: reliable even when the app is hosted under a `XamlRoot` it doesn't own (e.g. Hot Design). |
| `GetRootTheme(Window?)`      | `ApplicationTheme`      | Gets the `ApplicationTheme` of the provided window's content, or falls back to the operating system theme.   |
| `IsAppInDarkMode()`          | `bool`                 | **[Obsolete]** Returns `true` if the application is currently in dark mode, otherwise `false`.               |
| `IsRootInDarkMode(XamlRoot)` | `bool`                 | Determines if the provided `XamlRoot` is in dark mode.                                                       |
| `IsRootInDarkMode(FrameworkElement)` | `bool`        | Determines if the provided root element is in dark mode.                                                     |
| `IsRootInDarkMode(Window)`   | `bool`                 | Determines if the provided window's content is in dark mode.                                                 |

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
| `SetRootTheme(FrameworkElement?, bool)` | Sets the theme for the provided root element (and its subtree) based on the `darkMode` parameter. **Preferred**: reliable even when the app is hosted under a `XamlRoot` it doesn't own (e.g. Hot Design). |
| `SetRootTheme(Window?, bool)`       | Sets the theme for the provided window's content based on the `darkMode` parameter.        |
| `SetApplicationTheme(XamlRoot?, ElementTheme)` | Sets the `ElementTheme` (`Light` or `Dark`) for the provided `XamlRoot`. |
| `SetApplicationTheme(FrameworkElement?, ElementTheme)` | Sets the `ElementTheme` (`Light` or `Dark`) on the provided root element, theming its whole subtree. |
| `SetApplicationTheme(Window?, ElementTheme)` | Sets the `ElementTheme` (`Light` or `Dark`) on the provided window's content. |
| `ToggleApplicationTheme()`          | **[Obsolete]** Toggles the application theme between light and dark modes. Will be removed in future versions. |

### Usage

Set the theme for the app, using its root element (preferred):

```csharp
// Capture the app's root element once (e.g. in App.xaml.cs, right after setting window.Content):
var appRoot = window.Content as FrameworkElement;
SystemThemeHelper.SetApplicationTheme(appRoot, ElementTheme.Dark);

// Or, using the app's Window directly:
SystemThemeHelper.SetApplicationTheme(window, ElementTheme.Dark);
```

If the resolved root element is `null` (for example, the window content is not set yet, or is not a `FrameworkElement`), the `Set*` overloads are a no-op (a warning is logged when logging is enabled), and the `Get*` overloads fall back to the operating system theme.

Set the theme for a specific `XamlRoot`:

```csharp
var xamlRoot = someElement.XamlRoot;
SystemThemeHelper.SetApplicationTheme(xamlRoot, ElementTheme.Light);
Console.WriteLine("XamlRoot theme set to Light Mode.");
```

> [!IMPORTANT]
> The `XamlRoot`-based overloads read and write the theme of `XamlRoot.Content` - the root visual of the `XamlRoot`. When the app's content is hosted under a `XamlRoot` it doesn't own (for example, when running under Hot Design, where the app's content is re-parented into a host's shared `XamlRoot`), `XamlRoot.Content` is the **host's** root visual: theming it re-themes the host instead of the app. In that scenario, use the `Window`- or `FrameworkElement`-based overloads with the app's own window or root element (typically `window.Content`), which behave identically in standalone apps and remain correct when hosted.
