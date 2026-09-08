---
uid: Toolkit.Migration
---

# Upgrading Uno Toolkit

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

## Upgrading to Uno Toolkit 10.0

Uno Toolkit 10.0 adopts the Uno Themes typography and spacing changes in `8.0.0-dev.15`. Apps that override font resource keys or persist density values should make the following changes.

### Replace removed font resources

The `TypefacePlain` and `TypefaceBrand` root tokens have been replaced by `DefaultFontFamily`. Simple also removes `SimpleFontFamily`, `SimpleRegularFontFamily`, `SimpleMediumFontFamily`, `SimpleSemiBoldFontFamily`, and `SimpleBoldFontFamily`. Replace these keys in app resource overrides with `DefaultFontFamily`; per-scale weights are controlled by the existing `*FontWeight` tokens.

Both toolkit themes inherit a `DefaultFontFamily` property that generates the root and derived type-scale font families:

```xml
<SimpleToolkitTheme xmlns="using:Uno.Toolkit.UI.Simple"
                    DefaultFontFamily="ms-appx:///Fonts/MyFont.ttf#MyFont" />
```

Use a variable font or a font manifest to resolve the required weights from a single family. `FontOverrideSource` and `FontOverrideDictionary` still support individual overrides, and their explicit keys take precedence over corresponding generated keys. Toolkit aliases such as `NavigationBarFontFamily` and `DividerSubHeaderFontFamily` retain their own override keys; the root property does not regenerate them. Material's legacy `MaterialLightFontFamily`, `MaterialMediumFontFamily`, and `MaterialRegularFontFamily` keys remain available.

Changing `DefaultFontFamily` at runtime regenerates its resources. Existing text reading those resources through `ThemeResource` re-resolves after a theme-change pass or root-content recreation. Unstyled text keeps the framework's font default. See [font customization](simple-getting-started.md#customize-fonts) for details.

### Separate spacing from density

`DefaultSpacing` is the base spacing unit in pixels (default `4`). `DefaultDensity` now scales that unit: `Compact` ×0.75, `Regular` ×1, or `Comfy` ×1.25. Defaults still produce the previous 3 / 4 / 5 pixel spacing bases.

```xml
<MaterialToolkitTheme xmlns="using:Uno.Toolkit.UI.Material"
                      DefaultSpacing="6"
                      DefaultDensity="Comfy" />
```

This produces an effective base of 7.5 pixels (`Space100=7.5`, `Space200=15`). Only styles that consume the spacing tokens follow this scale; fixed Toolkit padding, control heights, and icon sizes are unchanged.

The numeric values of `Density.Compact`, `Density.Regular`, and `Density.Comfy` changed from `3`, `4`, and `5` to `0`, `1`, and `2`. Replace casts from spacing values with named enum members, and migrate any persisted numeric density values. XAML using the names needs no change.

For runtime resource refresh behavior and precedence, see [Uno Themes design tokens](xref:Uno.Themes.DesignTokens).

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

    This breaks code that merged these dictionaries directly by URI, as well as the deprecated `MaterialToolkitResourcesV1` / `MaterialToolkitResourcesV2` dictionaries, which reference the old URIs internally and now fail at startup with `Cannot locate resource from 'ms-appx:///Uno.Toolkit.WinUI.Material/Generated/mergedpages.WinUI.v2.xaml'`. Replace them with `<MaterialToolkitTheme/>`. Apps already using `<MaterialToolkitTheme/>` or `<SimpleToolkitTheme/>` are unaffected.

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
