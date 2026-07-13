# How to detect and set system themes in Uno applications

## 1. Detect the current OS theme

Use this when you just want to know what the operating system is using (Light/Dark).

```csharp
using Uno.Toolkit.UI;
using Microsoft.UI.Xaml; // or Windows.UI.Xaml, depending on your target

var osTheme = SystemThemeHelper.GetCurrentOsTheme();

// osTheme is ApplicationTheme.Light or ApplicationTheme.Dark
```

**What it returns**

* `ApplicationTheme.Light`
* `ApplicationTheme.Dark`

**When to use**

* You want to align your page with the OS
* You want to log or telemeter current OS theme
* You don’t have a `XamlRoot` yet

---

## 2. Get the theme of the current view (with XamlRoot)

Use this when your app has multiple windows/surfaces and you need the theme **for that surface**.

```csharp
using Uno.Toolkit.UI;
using Microsoft.UI.Xaml; // or Windows.UI.Xaml, depending on your target

// any FrameworkElement you have on screen:
var xamlRoot = MyPage.XamlRoot;

var appTheme = SystemThemeHelper.GetRootTheme(xamlRoot);

// appTheme is ApplicationTheme.Light or ApplicationTheme.Dark
```

**Notes**

* If `xamlRoot` is `null`, the helper falls back to the OS theme.
* This reads the theme of `XamlRoot.Content` - the root visual of the `XamlRoot`. For the hosted-safe default, prefer the `Window`/`FrameworkElement` overloads (section 8).

---

## 3. Check if the current view is in dark mode

Use this when you want a boolean instead of an enum.

```csharp
using Uno.Toolkit.UI;

var isDark = SystemThemeHelper.IsRootInDarkMode(MyPage.XamlRoot);

if (isDark)
{
    // apply dark-specific resources
}
else
{
    // apply light-specific resources
}
```

**Why this is useful**

* You can short-circuit code paths
* You can pick images/icons conditionally
* You can log “dark vs light usage”

---

## 4. Force a view to Light or Dark

Use this when you want to **override** the OS/user setting for a specific root.

```csharp
using Uno.Toolkit.UI;
using Microsoft.UI.Xaml; // or Windows.UI.Xaml, depending on your target

var xamlRoot = MyPage.XamlRoot;

// Force to Light
SystemThemeHelper.SetApplicationTheme(xamlRoot, ElementTheme.Light);

// or force to Dark
// SystemThemeHelper.SetApplicationTheme(xamlRoot, ElementTheme.Dark);
```

**What it does**

* Sets the theme on the provided `XamlRoot`
* Affects that visual tree
* Doesn’t require changing the global app theme

**Use cases**

* In-app theme picker
* Screenshots/exports that must look the same
* “Preview in Light/Dark” buttons

> [!IMPORTANT]
> The `XamlRoot`-based overloads target `XamlRoot.Content` - the root visual of the `XamlRoot`. If your app's content is hosted under a `XamlRoot` it doesn't own (for example under Hot Design), that element is the **host's** root, not your app's. Prefer the `Window`/`FrameworkElement` overloads described in section 8 below - they behave identically in standalone apps and remain correct when hosted.

---

## 5. Force a view to Dark using a boolean (older style)

The helper also has an overload that takes a `bool` and a `XamlRoot`. Use this if you prefer a boolean flow.

```csharp
using Uno.Toolkit.UI;

var xamlRoot = MyPage.XamlRoot;

// true = dark, false = light
SystemThemeHelper.SetRootTheme(xamlRoot, darkMode: true);
```

**Pick this when**

* You already have a `bool isDark` in your viewmodel
* You’re toggling from a ToggleSwitch

---

## 6. Log or print the current themes

Sometimes you just want to know “what do I have right now?”

```csharp
using Uno.Toolkit.UI;
using Microsoft.UI.Xaml; // or Windows.UI.Xaml, depending on your target

var osTheme   = SystemThemeHelper.GetCurrentOsTheme();
var rootTheme = SystemThemeHelper.GetRootTheme(MyPage.XamlRoot);

System.Diagnostics.Debug.WriteLine($"OS: {osTheme}, Root: {rootTheme}");
```

This is handy for diagnostics on platforms where theme can change underneath.

---

## 7. Migrate away from obsolete methods

The original helper exposes some members marked **[Obsolete]**:

* `GetApplicationTheme()` – obsolete
* `IsAppInDarkMode()` – obsolete
* `SetApplicationTheme(bool)` – obsolete
* `ToggleApplicationTheme()` – obsolete

### 7.1 If you used `GetApplicationTheme()`

**Before (obsolete):**

```csharp
var theme = SystemThemeHelper.GetApplicationTheme(); // obsolete
```

**After (recommended):**

```csharp
var theme = SystemThemeHelper.GetRootTheme(appRoot); // the app's root element (see section 8)
// or, standalone-only: SystemThemeHelper.GetRootTheme(MyPage.XamlRoot);
```

Why: the new version is root-aware, and the element overload stays correct when hosted (section 8).

---

### 7.2 If you used `IsAppInDarkMode()`

**Before (obsolete):**

```csharp
var isDark = SystemThemeHelper.IsAppInDarkMode(); // obsolete
```

**After (recommended):**

```csharp
var isDark = SystemThemeHelper.IsRootInDarkMode(appRoot); // the app's root element (see section 8)
// or, standalone-only: SystemThemeHelper.IsRootInDarkMode(MyPage.XamlRoot);
```

Why: again, this works for the current visual root - and the element overload stays correct when hosted.

---

### 7.3 If you used `SetApplicationTheme(bool)`

**Before (obsolete):**

```csharp
SystemThemeHelper.SetApplicationTheme(true); // dark
```

**After (recommended):**

Option A – use the app's root element + enum (hosted-safe, see section 8):

```csharp
SystemThemeHelper.SetApplicationTheme(appRoot, ElementTheme.Dark);
```

Option B – use the app's root element + bool:

```csharp
SystemThemeHelper.SetRootTheme(appRoot, darkMode: true);
```

The `XamlRoot`-based equivalents remain available for standalone apps.

---

### 7.4 If you used `ToggleApplicationTheme()`

**Before (obsolete):**

```csharp
SystemThemeHelper.ToggleApplicationTheme(); // obsolete
```

**After (manual toggle):**

```csharp
var isDark = SystemThemeHelper.IsRootInDarkMode(appRoot); // the app's root element (see section 8)
SystemThemeHelper.SetRootTheme(appRoot, darkMode: !isDark);
```

This keeps the old behavior but on the correct root.

---

## 8. Theme the app via its own root element or window (hosted-safe)

All the `XamlRoot`-based members above resolve the target element through `XamlRoot.Content`. In a standalone app that element **is** your app's root, so they work fine. But when your app's content is hosted under a `XamlRoot` it doesn't own - for example under **Hot Design**, where the app's `Window.Content` is re-parented into a host's shared `XamlRoot` - `XamlRoot.Content` is the *host's* root visual. Reading through it reports the host's theme, and writing through it re-themes the host while your app stays unchanged.

For this, the helper offers overloads that take your app's own root element or `Window` directly:

```csharp
using Uno.Toolkit.UI;

// Option A - with your app's root element, captured once at startup
// (e.g. in App.xaml.cs, right after setting window.Content):
var appRoot = window.Content as FrameworkElement;
SystemThemeHelper.SetRootTheme(appRoot, darkMode: true);
var appTheme = SystemThemeHelper.GetRootTheme(appRoot);

// Option B - with your app's Window:
SystemThemeHelper.SetApplicationTheme(window, ElementTheme.Dark);
var isDark = SystemThemeHelper.IsRootInDarkMode(window);
```

**Notes**

* `RequestedTheme` cascades down the subtree: setting it on your app's root themes your whole app and nothing above it.
* Prefer capturing the root element once (Option A). The `Window` overloads read `window.Content` at call time; capture the root while you know the content is set.
* Pass your app's **root** element (typically `window.Content`), not an arbitrary page - elements outside the page's subtree would otherwise keep the previous theme.
* Content hosted in the popup root (flyouts, `ContentDialog`, tooltips) renders **outside** the app root's subtree and may not pick up the theme automatically - set `RequestedTheme` on such content explicitly if it doesn't.
* If your root element gets recreated (e.g. a hot-reload replacing the visual tree), re-apply the theme to the new root.
* In standalone apps these overloads behave exactly like the `XamlRoot` ones, so they are a safe default everywhere.
* If the resolved root is `null` (window content not set yet, or not a `FrameworkElement`), `Set*` is a no-op (a warning is logged when logging is enabled) and `Get*` falls back to the OS theme.

**Toggle example (hosted-safe):**

```csharp
var isDark = SystemThemeHelper.IsRootInDarkMode(appRoot);
SystemThemeHelper.SetRootTheme(appRoot, darkMode: !isDark);
```
