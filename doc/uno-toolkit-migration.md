---
uid: Toolkit.Migration
---

# Upgrading Uno Toolkit

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

## Upgrading to Uno Toolkit 11.0

Uno Toolkit 11.0 requires Uno Platform 7.0, .NET 10 and Uno Themes 9.0. Apps using the Uno SDK get compatible versions by updating the SDK; apps pinning package versions manually should update their `Uno.WinUI` and Uno Themes references.

Uno Platform 7.0 renders with Skia on every platform, so the native mode of `NavigationBar` on iOS and Android has been removed and the control is rendered from its XAML template everywhere:

- `NativeFramePresenter` and `NativeNavigationBarPresenter` have been removed. Remove any `Frame` style based on `NativeDefaultToolkitFrame`; `Frame` no longer needs a custom style.
- The `NativeDefaultToolkitFrame`, `NativeNavigationBarTemplate`, `MaterialNativeNavigationBarTemplate` and `SimpleNativeNavigationBarTemplate` resources have been removed.

See [Migrating from the native mode](xref:Toolkit.Controls.NavigationBar#migrating-from-the-native-mode) for the behavior changes, and the [Uno Platform 7.0 migration guide](xref:Uno.Development.MigratingToUno7) for the platform changes.

## Upgrading to Uno Toolkit 10.0

Uno Toolkit 10.0 adopts the Uno Themes typography and spacing changes. Applications that override removed font resources or persist numeric density values need to update those usages:

- Replace `TypefacePlain`, `TypefaceBrand`, `SimpleFontFamily`, and Simple's per-weight font-family keys with `DefaultFontFamily`. See [Uno Themes typography](xref:Uno.Themes.DesignTokens#typography) for the resource changes.
- Use named `Density` members and migrate persisted numeric values: `Compact`, `Regular`, and `Comfy` changed from `3`, `4`, and `5` to `0`, `1`, and `2`. XAML using the names needs no change. See [Uno Themes density modes](xref:Uno.Themes.DesignTokens#density-modes).

`MaterialToolkitTheme` and `SimpleToolkitTheme` inherit the shared `BaseTheme` APIs. Apply the [Uno Themes customization examples](xref:Uno.Themes.DesignTokens#overriding-tokens) directly to the Toolkit theme; a separate underlying theme dictionary is unnecessary.

Toolkit's `NavigationBarFontFamily` and `DividerSubHeaderFontFamily` override keys remain available; `DefaultFontFamily` does not regenerate these aliases. Fixed Toolkit padding and margins also remain independent of the generated spacing scale. See [Material customization](material-getting-started.md#customization) and [Simple customization](simple-getting-started.md#customization) for Toolkit-specific considerations.

## Upgrading to Uno Toolkit 9.0

Uno Toolkit 9.0 takes a dependency on Uno Themes 7.0, which introduces seed-based color generation and a unified design-token system. It also drops UWP support and reshapes the public surface of the theme classes. Most apps that consume `<MaterialToolkitTheme/>` or `<SimpleToolkitTheme/>` via XAML will not need code changes, but several behaviors and defaults have shifted. Uno Toolkit 9.0 requires Uno.WinUI 6.5 or later; apps using the Uno SDK get compatible versions automatically, while apps pinning package versions manually should update their `Uno.WinUI` reference.

1. UWP target dropped

    The Uno Toolkit packages no longer target UWP and migrate to the `Uno.Sdk` single-project model. Apps must be on .NET 6+ / WinUI to upgrade. UWP-only consumers should remain on the 8.x line (the last UWP-flavored release is 8.4.2). The packages also no longer ship `net9.0-macos` / `net9.0-maccatalyst` assets — move these heads to the Skia Desktop target (for example `net10.0-desktop`) instead.

2. Generated resource paths renamed

    The `WinUI` / `UWP` package-name suffix has been removed from the toolkit's generated resource dictionary URIs.

    | Before                                                                       | After                                                                |
    |------------------------------------------------------------------------------|----------------------------------------------------------------------|
    | `ms-appx:///Uno.Toolkit.WinUI/Generated/mergedpages.WinUI.xaml`               | `ms-appx:///Uno.Toolkit.WinUI/Generated/mergedpages.xaml`             |
    | `ms-appx:///Uno.Toolkit.WinUI.Material/Generated/mergedpages.WinUI.v2.xaml`   | `ms-appx:///Uno.Toolkit.WinUI.Material/Generated/mergedpages.v2.xaml` |

    This breaks code that merged these dictionaries directly by URI. From 9.0 through 11.0 it also broke the deprecated `MaterialToolkitResources`, `MaterialToolkitResourcesV1` and `MaterialToolkitResourcesV2` dictionaries, which kept the old URIs internally and failed at startup with `Cannot locate resource from 'ms-appx:///Uno.Toolkit.WinUI.Material/Generated/mergedpages.WinUI.v2.xaml'`; they load again from 11.1. They remain deprecated, so replace them with `<MaterialToolkitTheme/>`. Apps already using `<MaterialToolkitTheme/>` or `<SimpleToolkitTheme/>` are unaffected.

3. `MaterialToolkitTheme` and new `SimpleToolkitTheme` inherit from their underlying theme

    Previously `MaterialToolkitTheme` derived from `ResourceDictionary` and instantiated `MaterialTheme` internally as a merged dictionary. It now derives from `MaterialTheme` directly, and the new `SimpleToolkitTheme` derives from `SimpleTheme`. The override dependency properties (`FontOverrideSource`, `ColorOverrideSource`, `FontOverrideDictionary`, `ColorOverrideDictionary`) are now inherited from `BaseTheme` in Uno Themes.

    Source-compatible for normal XAML and C# usage — no changes required if you set these properties via XAML attributes or the standard property accessors. Reflection-based code that asserts the inheritance chain (e.g. `typeof(MaterialToolkitTheme).BaseType == typeof(ResourceDictionary)`) will need to walk the chain instead.

    > [!IMPORTANT]
    > Because `MaterialToolkitTheme` is now also a `MaterialTheme`, do not initialize both `<MaterialTheme/>` and `<MaterialToolkitTheme/>` in the same `App.xaml` — that would cause duplicate theme initialization. The same applies to `<SimpleTheme/>` and `<SimpleToolkitTheme/>`. The toolkit theme already initializes the underlying theme.

4. New `Colors` property — recommended replacement for `ColorOverrideDictionary` / `ColorOverrideSource`

    The new `Colors` property is a `ThemeColors` object that bundles seed colors and override dictionaries. It is the recommended way to customize theme colors going forward. The legacy `ColorOverrideDictionary` and `ColorOverrideSource` properties still work, and are now routed internally through `Colors.OverrideDictionary`.

    ```xml
    <MaterialToolkitTheme xmlns="using:Uno.Toolkit.UI.Material"
                          xmlns:ut="using:Uno.Themes">
        <MaterialToolkitTheme.Colors>
            <ut:ThemeColors PrimarySeed="#2196F3"
                            OverrideSource="ms-appx:///Style/ColorPaletteOverride.xaml" />
        </MaterialToolkitTheme.Colors>
    </MaterialToolkitTheme>
    ```

5. Seed-based palette generation is opt-in

    Uno Themes 7.0 ships with seed-based palette generation, but it is opt-in: `MaterialToolkitTheme` keeps the same default Material Design 3 palette as 8.x unless you set `Colors.PrimarySeed` (or the other seed properties). `SimpleToolkitTheme` likewise keeps its default neutral palette unless a seed is set.

6. `ColorOverrideDictionary` precedence changed

    Previously, color overrides were merged as a base palette before any seed-derived palette. They are now applied as the highest-precedence layer (post-seed) via `Colors.OverrideDictionary`. This ensures explicit user overrides always win over seed-generated colors. If you relied on the prior pre-seed merge order, switch to the new `Colors` property and set `PrimarySeed`/`SecondarySeed`/`TertiarySeed` to drive the palette directly.

7. New `DefaultDensity` and `DefaultCornerRadius` properties

    Both toolkit themes now expose density and shape design tokens inherited from `BaseTheme`.

    | Property              | Type      | Default   | Description                                                                                                                                         |
    |-----------------------|-----------|-----------|-----------------------------------------------------------------------------------------------------------------------------------------------------|
    | `DefaultDensity`      | `Density` | `Regular` | Drives the base spacing unit used by `Space*` tokens. Accepted values: `Compact` (3 px), `Regular` (4 px), `Comfy` (5 px). Control heights and icon sizes are unchanged across densities. |
    | `DefaultCornerRadius` | `double`  | `4`       | Base corner radius unit (in pixels). All `Radius*` tokens are computed as multiples of this. `RadiusFull` always remains `9999`.                    |

    ```xml
    <MaterialToolkitTheme xmlns="using:Uno.Toolkit.UI.Material"
                          DefaultDensity="Comfy"
                          DefaultCornerRadius="6" />
    ```

## Upgrading to Uno Toolkit 8.0

1. Bump to Uno 6.0

    Version 8.0 of Uno Toolkit now requires `Uno 6.0`.

2. Switch from `ReturnType` to `InputReturnType`

    The `InputExtensions` helper has been updated to use the built-in `InputReturnType` enum (provided by Uno v6+) instead of the legacy `ReturnType` enum.

    ```csharp
    // No code changes required in XAML, but under the hood
    // the attached property now uses InputReturnType:
    public static DependencyProperty ReturnTypeProperty =
        DependencyProperty.RegisterAttached(
            "ReturnType",
            typeof(InputReturnType),
            typeof(InputExtensions),
            new PropertyMetadata(InputReturnType.Default, OnReturnTypeChanged));
    ```
