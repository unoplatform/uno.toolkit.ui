---
uid: Toolkit.Migration.Material2.0
---

# Upgrading Material Toolkit Version

## Upgrading to Material Toolkit v6

Material Toolkit v6 contains a dependency on [Uno Material](Uno.Themes.Material.GetStarted) which, as of its v5 release, introduces breaking changes that can affect applications using the Material Toolkit. Refer to the [Uno Material v5 upgrade documentation](xref:Uno.Themes.Material.Migration#upgrading-to-uno-themes-v50) for further information.

### Opacity and brushes

The opacity values of certain brushes have been adjusted in Uno Material v5. The following table shows the changes in opacity values:

| Opacity variant | Old Value | New Value |
|-----------------|-----------|-----------|
| Medium          | 0.54      | 0.64      |
| Disabled        | 0.38      | 0.12      |
| DisabledLow     | 0.12      | *removed* |

Existing explicit references to `-DisabledLow` resources have been updated to use `-Disabled`.

### Markup Theme class name

The `Theme` class in the `Uno.Toolkit.UI.Markup` has been renamed to `ToolkitTheme` in order to no longer clash with the `Theme` class from `Uno.Themes.Markup`.

This affects all references to the `Theme` class in the `Uno.Toolkit.UI.Markup` namespace, which are outlined in the table below:

| Class                  | Old                          | New                                                               |
|------------------------|------------------------------|-------------------------------------------------------------------|
| `Card`                 | `Theme.Card.*`               | `ToolkitTheme.Card.*`                                             |
| `CardContentControl`   | `Theme.CardContentControl.*` | `ToolkitTheme.CardContentControl.*`                               |
| `Chip`                 | `Theme.Chip.*`               | `ToolkitTheme.Chip.*`                                             |
| `ChipGroup`            | `Theme.ChipGroup.*`          | `ToolkitTheme.ChipGroup.*`                                        |
| `Divider`              | `Theme.Divider.*`            | `ToolkitTheme.Divider.*`                                          |
| `NavigationBar`        | `Theme.NavigationBar.*`      | `ToolkitTheme.NavigationBar.*`                                    |
| `TabBar`               | `Theme.TabBar.*`             | `ToolkitTheme.TabBar.*`                                           |
| `TabBar`               | `Theme.TabBarItem.*`         | `ToolkitTheme.TabBarItem.*`                                       |

## Upgrading to Material Toolkit v4

Material Toolkit v4 introduces support for [Lightweight Styling](lightweight-styling.md) and, as a result, many resource keys have been added as well as renamed. For a list of all the new resource keys, please refer to the [Lightweight Styling documentation](lightweight-styling.md#resource-keys).

Along with the above list of new resource keys, below is a list of the resource keys that have been removed or renamed.

> [!NOTE]
> Most resources, including those that have been added or renamed, have now been placed inside of a `ThemeDictionary`. This means that the resources should now be referenced using the `ThemeResource` markup extension instead of `StaticResource`. For more information on theme resources, please refer to the [XAML theme resources documentation](https://learn.microsoft.com/en-us/windows/apps/design/style/xaml-theme-resources).

### Chip

| Old Key                                       | New Key                         | Value                                                             |
|-----------------------------------------------|---------------------------------|-------------------------------------------------------------------|
| `MaterialChipElevationMargin`                 | `ChipElevationMargin`           | 4                                                                 |
| `MaterialChipCornerRadius`                    | `ChipCornerRadius`              | 8                                                                 |
| `MaterialChipIconSize`                        | `ChipIconSize`                  | 18                                                                |
| `MaterialChipElevation`                       | `ChipElevation`                 | 4                                                                 |
| `M3MateriaChipCheckGlyphSize`                 | ***REMOVED***                   | 20                                                                |
| `MaterialChipBorderThickness`                 | `ChipBorderThickness`           | 1                                                                 |
| `MaterialChipDeleteIconLength`                | `ChipDeleteIconLength`          | 11                                                                |
| `MaterialChipDeleteIconContainerLength`       | `ChipDeleteIconContainerLength` | 18                                                                |
| `MaterialChipSize`                            | `ChipSize`                      | 12                                                                |
| `MaterialChipContentMinHeight`                | `ChipContentMinHeight`          | 20                                                                |
| `MaterialChipHeight`                          | `ChipHeight`                    | 32                                                                |
| `MaterialChipDisabledBorderBrush`             | `ChipBorderBrushDisabled`       | `OnSurfaceVariantDisabledLowBrush`                                |
| `MaterialChipDisabledBackground`              | `ChipBackgroundDisabled`        | `OnSurfaceDisabledLowBrush`                                       |
| `MaterialChipDisabledForeground`              | `ChipForegroundDisabled`        | `OnSurfaceDisabledBrush`                                          |
| `MaterialChipIconDisabledForeground`          | `ChipIconForegroundDisabled`    | `OnSurfaceDisabledBrush`                                          |
| `MaterialChipIconForeground`                  | `ChipIconForeground`            | `PrimaryBrush`                                                    |
| `MaterialChipSelectedPressedForeground`       | ***REMOVED***                   | `OnSecondaryContainerBrush`                                       |
| `MaterialChipSelectedFocusedForeground`       | ***REMOVED***                   | `OnSecondaryContainerBrush`                                       |
| `MaterialChipSelectedPointerOverForeground`   | ***REMOVED***                   | `OnSecondaryContainerBrush`                                       |
| `MaterialChipSelectedForeground`              | ***REMOVED***                   | `OnSecondaryContainerBrush`                                       |
| `MaterialChipPressedForeground`               | `ChipForegroundPressed`         | `OnSurfaceVariantBrush`                                           |
| `MaterialChipFocusedForeground`               | `ChipForegroundFocused`         | `OnSurfaceVariantBrush` -> `SystemControlTransparentBrush`        |
| `MaterialChipPointerOverForeground`           | `ChipForegroundPointerOver`     | `OnSurfaceVariantBrush`                                           |
| `MaterialChipForeground`                      | `ChipForeground`                | `OnSurfaceVariantBrush`                                           |
| `MaterialChipSelectedPressedStateOverlay`     | ***REMOVED***                   | `OnSecondaryContainerSelectedBrush`                               |
| `MaterialChipSelectedFocusedStateOverlay`     | ***REMOVED***                   | `OnSecondaryContainerFocusedBrush`                                |
| `MaterialChipSelectedPointerOverStateOverlay` | ***REMOVED***                   | `OnSecondaryContainerHoverBrush`                                  |
| `MaterialChipPressedStateOverlay`             | `ChipStateOverlayPressed`       | `OnSurfaceVariantPressedBrush`                                    |
| `MaterialChipFocusedStateOverlay`             | `ChipStateOverlayFocused`       | `OnSurfaceVariantFocusedBrush` -> `SystemControlTransparentBrush` |
| `MaterialChipPointerOverStateOverlay`         | `ChipStateOverlayPointerOver`   | `OnSurfaceVariantHoverBrush`                                      |
| `MaterialChipSelectedBackground`              | ***REMOVED***                   | `SecondaryContainerBrush`                                         |
| `MaterialChipBackground`                      | `ChipBackground`                | `SystemControlTransparentBrush`                                   |

### Divider

| Old Key                 | New Key         | Value |
|-------------------------|-----------------|-------|
| `MaterialDividerHeight` | `DividerHeight` | 1     |

### TabBar

| Old Key                                                   | New Key                                           | Value     |
|-----------------------------------------------------------|---------------------------------------------------|-----------|
| `MaterialNavigationTabBarItemLargeBadgeCornerRadius`      | `NavigationTabBarItemLargeBadgeCornerRadius`      | 8         |
| `MaterialNavigationTabBarItemLargeBadgePadding`           | `NavigationTabBarItemLargeBadgePadding`           | 4,0       |
| `MaterialNavigationTabBarItemLargeBadgeMargin`            | `NavigationTabBarItemLargeBadgeMargin`            | 32,2,0,0  |
| `MaterialNavigationTabBarItemLargeBadgeMinWidth`          | `NavigationTabBarItemLargeBadgeMinWidth`          | 16        |
| `MaterialNavigationTabBarItemLargeBadgeHeight`            | `NavigationTabBarItemLargeBadgeHeight`            | 16        |
| `MaterialNavigationTabBarItemSmallBadgeMargin`            | `NavigationTabBarItemSmallBadgeMargin`            | 0,4,20,0  |
| `MaterialNavigationTabBarItemSmallBadgeWidth`             | `NavigationTabBarItemSmallBadgeWidth`             | 6         |
| `MaterialNavigationTabBarItemSmallBadgeHeight`            | `NavigationTabBarItemSmallBadgeHeight`            | 6         |
| `MaterialFabTabBarItemPadding`                            | `FabTabBarItemPadding`                            | 20        |
| `MaterialFabTabBarItemCornerRadius`                       | `FabTabBarItemCornerRadius`                       | 16        |
| `MaterialFabTabBarItemIconTextPadding`                    | `FabTabBarItemIconTextPadding`                    | 12        |
| `MaterialFabTabBarItemContentWidthOrHeight`               | `FabTabBarItemContentWidthOrHeight`               | 16        |
| `MaterialFabTabBarItemOffset`                             | `FabTabBarItemOffset`                             | -32       |
| `MaterialTopTabBarItemContentMargin`                      | `TopTabBarItemContentMargin`                      | 0         |
| `MaterialTopTabBarItemIconHeight`                         | `TopTabBarItemIconHeight`                         | 20        |
| `MaterialTopTabBarHeight`                                 | `TopTabBarHeight`                                 | 48        |
| `MaterialNavigationTabBarItemActiveIndicatorCornerRadius` | `NavigationTabBarItemActiveIndicatorCornerRadius` | 16        |
| `MaterialNavigationTabBarItemPadding`                     | `NavigationTabBarItemPadding`                     | 0,12,0,16 |
| `MaterialNavigationTabBarItemActiveIndicatorHeight`       | `NavigationTabBarItemActiveIndicatorHeight`       | 32        |
| `MaterialNavigationTabBarItemActiveIndicatorWidth`        | `NavigationTabBarItemActiveIndicatorWidth`        | 64        |
| `MaterialNavigationTabBarItemIconHeight`                  | `NavigationTabBarItemIconHeight`                  | 18        |
| `MaterialNavigationTabBarWidthOrHeight`                   | `NavigationTabBarWidthOrHeight`                   | 80        |

## Upgrading Material Toolkit to Uno Material v2

Starting with version 2.0.0 of the [Uno.Toolkit.UI.Material](https://www.nuget.org/packages/Uno.Toolkit.UI.Material/2.0.0) and [Uno.Toolkit.WinUI.Material](https://www.nuget.org/packages/Uno.Toolkit.WinUI.Material/2.0.0) packages, users can now take advantage of the new [Material Design 3](https://m3.material.io/) design system from Google.
Along with the new Material Design 3 styles, our Material Toolkit NuGet packages will continue to support the previous Material Design 2 styles. In order to achieve this backward compatibility, we have had to make some changes to the way the Material toolkit is initialized within your `App.xaml`.

> [!NOTE]
> In order to avoid any confusion going forward in this article, we will be referring to the new collection of Material Design 3 compatible styles as the "v2" styles and the previous collection of Material Design 2 styles will be referred to as the "v1" styles.

 There are two available paths once you have updated to the new Material Toolkit v2 package:

- Continue to keep using the v1 styles; or,
- Migrate app to reference the new v2 styles

### Continue Using v1 Styles

> [!WARNING]
> In order to continue using the v1 styles, some changes are required in your `App.xaml`.

<!-- TODO: Use xref link. For some reason, it currently doesn't work. -->
Since the Material Toolkit has a dependency on the Uno Material library, it is required to first follow the steps in the ***Continue Using v1 Styles*** section of the [Uno Material v2 migration documentation](https://platform.uno/docs/articles/external/uno.themes/doc/material-migration.html).

The Material Toolkit v2 NuGet package contains both sets of v1 and v2 styles. Within your `App.xaml`, you will need to replace the reference to `MaterialToolkitResources` with `MaterialToolkitResourcesV1`.

### Migrating to v2 Styles

`MaterialToolkitResources` will now always initialize the latest version of the Material-compatible Toolkit styles. It is also possible to directly reference the `MaterialToolkitResourcesV2` ResourceDictionary if needed.

The v2 styles introduce a new naming system for its resource keys. Refer to the [Material Toolkit Styles Matrix](controls-styles.md) for the updated style keys.
