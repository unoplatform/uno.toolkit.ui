---
uid: Toolkit.DesignTokens
---
# Design Tokens

The Uno Toolkit uses the **design token system** defined in [Uno.Themes](xref:uno.themes.designtokens) to control spacing, shape (corner radius), and density across all controls. Instead of hardcoding pixel values, toolkit controls reference semantic token keys that automatically adjust when the density or corner-radius base changes.

> [!TIP]
> For the full list of token keys and how they are generated, see the [Uno.Themes Design Tokens](xref:uno.themes.designtokens) documentation.

## Token Categories Used by Toolkit Controls

### Spacing (`Space*`)

Spacing tokens control padding, margins, and layout gaps. They are computed as multiples of a base unit set by `DefaultDensity`.

| Token | Regular (4 px) | Compact (3 px) | Comfy (5 px) |
|-------|:--------------:|:--------------:|:------------:|
| `Space100` | 4 | 3 | 5 |
| `Space150` | 6 | 4.5 | 7.5 |
| `Space200` | 8 | 6 | 10 |
| `Space300` | 12 | 9 | 15 |
| `Space400` | 16 | 12 | 20 |
| `Space500` | 20 | 15 | 25 |
| `Space600` | 24 | 18 | 30 |
| `Space1600` | 64 | 48 | 80 |

Each `Space*` key also has directional `Thickness` variants: `Space200Thickness`, `Space200HorizontalThickness`, `Space200LeftThickness`, `Space200TopThickness`, `Space200BottomThickness`, etc.

### Shape (`Radius*`)

Corner radius tokens are multiples of a base value set by `DefaultCornerRadius` (default: 4).

| Token | Default (4 px) |
|-------|:--------------:|
| `Radius100` | 4 |
| `Radius200` | 8 |
| `Radius300` | 12 |
| `Radius400` | 16 |

Each also has a `CornerRadius` variant: `Radius200CornerRadius`, etc.

### Fixed Tokens (Density-Invariant)

These values remain constant regardless of density:

| Token | Value (px) | Usage |
|-------|:----------:|-------|
| `ControlHeightSmall` | 32 | Chip height |
| `ControlHeightMedium` | 40 | Avatar dimensions |
| `ControlHeightMediumLarge` | 44 | iOS NavigationBar presenter |
| `ControlHeightLarge` | 48 | Top TabBar height |
| `IconSizeSmall` | 16 | AppBar button icons, FAB icon |
| `IconSizeMedium` | 24 | Main command icons |

## Toolkit Controls Using Design Tokens

The following table maps toolkit controls to the tokens they reference. Values shown are for **Regular** density (base = 4 px) and default corner radius (base = 4 px).

### Card

| Resource Key | Token | Regular Value |
|---|---|:---:|
| `CardPadding` | `Space400Thickness` | 16 |
| `CardCornerRadius` | `Radius300CornerRadius` | 12 |
| `CardElevationMargin` | `Space150Thickness` | 6 |

### Chip

| Resource Key | Token | Regular Value |
|---|---|:---:|
| `ChipHeight` | `ControlHeightSmall` | 32 |
| `ChipSize` | `Space300` | 12 |
| `ChipCornerRadius` | `Radius200CornerRadius` | 8 |
| `ChipContentMargin` | `Space200HorizontalThickness` | 8,0 |
| `ChipPadding` | `Space200HorizontalThickness` | 8,0 |
| `ChipElevationMargin` | `Space100Thickness` | 4 |
| `ChipContentMinHeight` | `Space500` | 20 |

### Divider

| Resource Key | Token | Regular Value |
|---|---|:---:|
| `DividerSubHeaderMargin` | `Space100TopThickness` | 0,4,0,0 |

### NavigationBar

| Resource Key | Token | Regular Value |
|---|---|:---:|
| `MaterialXamlNavigationBarHeight` | `Space1600` | 64 |
| `MaterialNavigationBarHeight` | `Space1600` | 64 |
| `MaterialNavigationBarContentMargin` | `Space400HorizontalThickness` | 16,0,16,0 |
| `NavigationBarPadding` | `Space100LeftThickness` | 4,0,0,0 |
| `NavBarAppBarButtonContentHeight` | `IconSizeSmall` | 16 |
| `NavBarMainCommandAppBarButtonContentHeight` | `IconSizeMedium` | 24 |
| `NavBarAppBarThemeCompactHeight` | `Space1600` | 64 |

### TabBar

| Resource Key | Token | Regular Value |
|---|---|:---:|
| `TopTabBarHeight` | `ControlHeightLarge` | 48 |
| `NavigationTabBarItemActiveIndicatorCornerRadius` | `Radius400CornerRadius` | 16 |
| `TopTabBarItemIconHeight` | `Space500` | 20 |
| `TopTabBarItemContentMargin` | `Space200LeftThickness` | 8,0,0,0 |
| `FabTabBarItemContentWidthOrHeight` | `IconSizeSmall` | 16 |
| `FabTabBarItemIconTextPadding` | `Space300` | 12 |
| `FabTabBarItemCornerRadius` | `Radius400CornerRadius` | 16 |
| `FabTabBarItemPadding` | `Space500Thickness` | 20 |
| `NavigationTabBarItemSmallBadgeHeight` | `Space150` | 6 |
| `NavigationTabBarItemSmallBadgeWidth` | `Space150` | 6 |
| `NavigationTabBarItemLargeBadgeHeight` | `Space400` | 16 |
| `NavigationTabBarItemLargeBadgeMinWidth` | `Space400` | 16 |
| `NavigationTabBarItemLargeBadgePadding` | `Space100HorizontalThickness` | 4,0 |
| `NavigationTabBarItemLargeBadgeCornerRadius` | `Radius200CornerRadius` | 8 |

## Customizing Density

Set the `DefaultDensity` property on your theme to adjust all spacing-based tokens at once:

```xml
<!-- Compact — base spacing = 3 px -->
<MaterialToolkitTheme xmlns="using:Uno.Toolkit.UI.Material"
                      DefaultDensity="Compact" />

<!-- Regular (default) — base spacing = 4 px -->
<MaterialToolkitTheme xmlns="using:Uno.Toolkit.UI.Material" />

<!-- Comfy — base spacing = 5 px -->
<MaterialToolkitTheme xmlns="using:Uno.Toolkit.UI.Material"
                      DefaultDensity="Comfy" />
```

The same property is available on `SimpleToolkitTheme`:

```xml
<SimpleToolkitTheme xmlns="using:Uno.Toolkit.UI.Simple"
                    DefaultDensity="Compact" />
```

## Customizing Corner Radius

Set `DefaultCornerRadius` to change the base unit for all shape tokens:

```xml
<MaterialToolkitTheme xmlns="using:Uno.Toolkit.UI.Material"
                      DefaultCornerRadius="6" />
```

With `DefaultCornerRadius="6"`, `Radius200CornerRadius` becomes `12` instead of `8`.

## Overriding Individual Tokens

You can override any token at any scope using standard XAML resource overrides:

```xml
<!-- Override globally in App.xaml -->
<x:Double x:Key="Space400">20</x:Double>
<Thickness x:Key="Space400Thickness">20</Thickness>

<!-- Override at page scope -->
<Page.Resources>
    <CornerRadius x:Key="Radius200CornerRadius">10</CornerRadius>
</Page.Resources>

<!-- Override per control via lightweight styling -->
<utu:Card>
    <utu:Card.Resources>
        <Thickness x:Key="CardPadding">24</Thickness>
    </utu:Card.Resources>
</utu:Card>
```

## Further Reading

- [Uno.Themes Design Tokens](xref:uno.themes.designtokens) — full token key reference and generation logic
- [Lightweight Styling](xref:Toolkit.LightweightStyling) — per-control resource key overrides
