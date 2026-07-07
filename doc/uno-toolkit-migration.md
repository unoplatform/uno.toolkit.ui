---
uid: Toolkit.Migration
---

# Upgrading Uno Toolkit

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

## Upgrading to Uno Toolkit 9.0

Uno Toolkit 9.0 takes a dependency on Uno Themes 7.0, which introduces seed-based color generation and a unified design-token system. It also drops UWP support and reshapes the public surface of the theme classes. Most apps that consume `<MaterialToolkitTheme/>` or `<SimpleToolkitTheme/>` via XAML will not need code changes, but several behaviors and defaults have shifted.

1. UWP target dropped

    The Uno Toolkit packages no longer target UWP and migrate to the `Uno.Sdk` single-project model. Apps must be on .NET 6+ / WinUI to upgrade. UWP-only consumers should remain on the 8.x line.

2. Generated resource paths renamed

    The `WinUI` / `UWP` package-name suffix has been removed from the toolkit's generated resource dictionary URIs.

    | Before                                                                       | After                                                                |
    |------------------------------------------------------------------------------|----------------------------------------------------------------------|
    | `ms-appx:///Uno.Toolkit.WinUI/Generated/mergedpages.WinUI.xaml`               | `ms-appx:///Uno.Toolkit.WinUI/Generated/mergedpages.xaml`             |
    | `ms-appx:///Uno.Toolkit.WinUI.Material/Generated/mergedpages.WinUI.v2.xaml`   | `ms-appx:///Uno.Toolkit.WinUI.Material/Generated/mergedpages.v2.xaml` |

    This only affects code that merged these dictionaries directly by URI. Apps using `<MaterialToolkitTheme/>` or `<SimpleToolkitTheme/>` are unaffected.

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

5. Default Material palette is now seed-generated

    `MaterialToolkitTheme` now uses a default primary seed color (`#5946D2`) to algorithmically generate the Material Design 3 tonal palette. This produces a different visual result than 8.x's static palette. To preserve the previous look, set `Colors.PrimarySeed` to your desired anchor color or use `Colors.OverrideDictionary` to override individual color resources. `SimpleToolkitTheme` still defaults to the standard neutral Simple palette; if you do not specify any `Colors` seeds or overrides, apps keep the default neutral appearance. In current documentation this default Simple palette may be described as being produced from the built-in neutral gray seed (`#808080`), rather than from an app-specified seed.

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
