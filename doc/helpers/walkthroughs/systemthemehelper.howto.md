# How to detect and set system themes in Uno applications

## 1. Detect the current OS theme

Use this when you just want to know what the operating system is using (Light/Dark).

```csharp
using Uno.Toolkit.UI;
using Windows.UI.Xaml;

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
using Windows.UI.Xaml;

// any FrameworkElement you have on screen:
var xamlRoot = MyPage.XamlRoot;

var appTheme = SystemThemeHelper.GetRootTheme(xamlRoot);

// appTheme is ApplicationTheme.Light or ApplicationTheme.Dark
```

**Notes**

* If `xamlRoot` is `null`, the helper falls back to the OS theme.
* This is the recommended way when you have a specific visual root.

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
using Windows.UI.Xaml;

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
var theme = SystemThemeHelper.GetRootTheme(MyPage.XamlRoot);
```

Why: the new version is root-aware.

---

### 7.2 If you used `IsAppInDarkMode()`

**Before (obsolete):**

```csharp
var isDark = SystemThemeHelper.IsAppInDarkMode(); // obsolete
```

**After (recommended):**

```csharp
var isDark = SystemThemeHelper.IsRootInDarkMode(MyPage.XamlRoot);
```

Why: again, this works for the current visual root.

---

### 7.3 If you used `SetApplicationTheme(bool)`

**Before (obsolete):**

```csharp
SystemThemeHelper.SetApplicationTheme(true); // dark
```

**After (recommended):**

Option A – use `XamlRoot` + enum:

```csharp
SystemThemeHelper.SetApplicationTheme(MyPage.XamlRoot, ElementTheme.Dark);
```

Option B – use `XamlRoot` + bool:

```csharp
SystemThemeHelper.SetRootTheme(MyPage.XamlRoot, darkMode: true);
```

---

### 7.4 If you used `ToggleApplicationTheme()`

**Before (obsolete):**

```csharp
SystemThemeHelper.ToggleApplicationTheme(); // obsolete
```

**After (manual toggle):**

```csharp
var isDark = SystemThemeHelper.IsRootInDarkMode(MyPage.XamlRoot);
SystemThemeHelper.SetRootTheme(MyPage.XamlRoot, darkMode: !isDark);
```

This keeps the old behavior but on the correct root.
