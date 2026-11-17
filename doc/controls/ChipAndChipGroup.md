---
uid: Toolkit.Controls.Chip
---
# Chip & ChipGroup

> [!TIP]
> This guide covers details for `Chip` and `ChipGroup` specifically. If you are just getting started with the Uno Toolkit, please see our [general getting started](../getting-started.md) page to make sure you have the correct setup in place.

## Summary

`Chip` is a control that can be used for selection, filtering, or performing an action from a list.
`ChipGroup` is a container that can house a collection of `Chip`s.

## Chip

`Chip` is derived from `ToggleButton`, a control that an user can select (check) or deselect (uncheck).

### C\#

```csharp
public partial class Chip : ToggleButton
```

### XAML

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<utu:Chip .../>
```

### Inheritance

`Object` &#8594; `DependencyObject` &#8594; `UIElement` &#8594; `FrameworkElement` &#8594; `Control` &#8594; `ContentControl` &#8594; `ButtonBase` &#8594; `ToggleButton` &#8594; `Chip`

### Constructors

| Constructor | Description                                     |
|-------------|-------------------------------------------------|
| `Chip()`    | Initializes a new instance of the `Chip` class. |

### Properties

| Property                  | Type           | Description                                                                    |
|---------------------------|----------------|--------------------------------------------------------------------------------|
| `CanRemove`               | `bool`         | Gets or sets whether the remove button is visible.                             |
| `Elevation`               | `double`       | Gets or sets the elevation of the `Chip`.                                      |
| `Icon`                    | `object`       | Gets or sets the icon of the `Chip`.                                           |
| `IconTemplate`            | `DataTemplate` | Gets or sets the data template that is used to display the icon of the `Chip`. |
| `RemovedCommand`          | `ICommand`     | Gets or sets the command to invoke when the remove button is pressed.          |
| `RemovedCommandParameter` | `object`       | Gets or sets the parameter to pass to the RemovedCommand property.             |

### Events

| Event      | Type                       | Description                                                                                         |
|------------|----------------------------|-----------------------------------------------------------------------------------------------------|
| `Removed`  | `RoutedEventHandler`       | Occurs when the remove button is pressed.                                                           |
| `Removing` | `ChipRemovingEventHandler` | Occurs when the remove button is pressed, but before the `Removed` event allowing for cancellation. |

> [!NOTE]
> When used outside of a `ChipGroup`, the `Removed` event does not cause itself to be removed from the view.

```csharp
delegate void ChipRemovingEventHandler(object sender, ChipRemovingEventArgs e);
sealed class ChipRemovingEventArgs : EventArgs
{
    // Gets or sets whether the Chip.Removed" event should be canceled.
    public bool Cancel { get; set; }
}
```

### Usage

Without a design-system package, `Chip` uses its default Fluent style (`DefaultChipStyle`): a pill-shaped toggle that follows the WinUI `ToggleButton` fills and text colors, with an optional leading icon and remove button.

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<utu:Chip Content="Chip" />
<utu:Chip Content="Selected" IsChecked="True" />
<utu:Chip Content="Removable" CanRemove="True" />

<!-- with icon -->
<utu:Chip Content="Chip">
    <utu:Chip.Icon>
        <SymbolIcon Symbol="Favorite" />
    </utu:Chip.Icon>
</utu:Chip>
```

> [!NOTE]
> The default style does not draw a shadow, so `Elevation` has no effect on it.

With the Material or Simple Toolkit libraries, the following style keys are also available:

```xml
<utu:Chip Content="Assist Chip" Style="{StaticResource AssistChipStyle}" />
<utu:Chip Content="Input Chip" IsChecked="True" Style="{StaticResource InputChipStyle}" />
<utu:Chip Content="Filter Chip" IsChecked="True" Style="{StaticResource FilterChipStyle}" />
<utu:Chip Content="Suggestion Chip" IsChecked="True" Style="{StaticResource SuggestionChipStyle}" />

<!-- with icon -->
<utu:Chip Content="Chip" Style="{StaticResource MaterialChipStyle}">
    <utu:Chip.Icon>
        <Image Source="ms-appx:///Assets/Avatar.png" />
    </utu:Chip.Icon>
</utu:Chip>
```

## Lightweight Styling

The default, Material, and Simple styles share the `Chip*` key names, so an override applies to whichever style is active. The default values differ per design system.

### Default (Fluent)

The default style follows the Pill in the [Windows UI Kit](https://www.figma.com/community/file/1440832812269040007). Fills and text use the WinUI `ToggleButton` resources of the same state; the border is a flat stroke that disappears when the chip is checked. In High Contrast, every brush is the system brush that the `ToggleButton` uses there, so the values in the table below apply to the Light and Dark themes.

| Key                                   | Type           | Value                                    |
|---------------------------------------|----------------|------------------------------------------|
| `ChipHeight`                          | `Double`       | 32                                       |
| `ChipIconSize`                        | `Double`       | 16                                       |
| `ChipDeleteIconContainerLength`       | `Double`       | 20                                       |
| `ChipDeleteIconLength`                | `Double`       | 8                                        |
| `ChipCornerRadius`                    | `CornerRadius` | 16                                       |
| `ChipDeleteIconCornerRadius`          | `CornerRadius` | 10                                       |
| `ChipBorderThickness`                 | `Thickness`    | 1                                        |
| `ChipPadding`                         | `Thickness`    | 11,0,5,0                                 |
| `ChipContentMargin`                   | `Thickness`    | 0,0,6,0                                  |
| `ChipIconMargin`                      | `Thickness`    | 0,0,8,0                                  |
| `ChipBackground`                      | `Brush`        | `ToggleButtonBackground`                 |
| `ChipBackgroundPointerOver`           | `Brush`        | `ToggleButtonBackgroundPointerOver`      |
| `ChipBackgroundPressed`               | `Brush`        | `ToggleButtonBackgroundPressed`          |
| `ChipBackgroundDisabled`              | `Brush`        | `ToggleButtonBackgroundDisabled`         |
| `ChipBackgroundChecked`               | `Brush`        | `ToggleButtonBackgroundChecked`          |
| `ChipBackgroundCheckedPointerOver`    | `Brush`        | `ToggleButtonBackgroundCheckedPointerOver` |
| `ChipBackgroundCheckedPressed`        | `Brush`        | `ToggleButtonBackgroundCheckedPressed`   |
| `ChipBackgroundCheckedDisabled`       | `Brush`        | `ToggleButtonBackgroundCheckedDisabled`  |
| `ChipForeground`                      | `Brush`        | `ToggleButtonForeground`                 |
| `ChipForegroundPointerOver`           | `Brush`        | `ToggleButtonForegroundPointerOver`      |
| `ChipForegroundPressed`               | `Brush`        | `ToggleButtonForegroundPressed`          |
| `ChipForegroundDisabled`              | `Brush`        | `ToggleButtonForegroundDisabled`         |
| `ChipForegroundChecked`               | `Brush`        | `ToggleButtonForegroundChecked`          |
| `ChipForegroundCheckedPointerOver`    | `Brush`        | `ToggleButtonForegroundCheckedPointerOver` |
| `ChipForegroundCheckedPressed`        | `Brush`        | `ToggleButtonForegroundCheckedPressed`   |
| `ChipForegroundCheckedDisabled`       | `Brush`        | `TextOnAccentFillColorDisabledBrush`     |
| `ChipIconForeground`                  | `Brush`        | `ToggleButtonForeground`                 |
| `ChipIconForegroundPointerOver`       | `Brush`        | `ToggleButtonForegroundPointerOver`      |
| `ChipIconForegroundPressed`           | `Brush`        | `ToggleButtonForegroundPressed`          |
| `ChipIconForegroundDisabled`          | `Brush`        | `ToggleButtonForegroundDisabled`         |
| `ChipIconForegroundChecked`           | `Brush`        | `ToggleButtonForegroundChecked`          |
| `ChipIconForegroundCheckedPointerOver`| `Brush`        | `ToggleButtonForegroundCheckedPointerOver` |
| `ChipIconForegroundCheckedPressed`    | `Brush`        | `ToggleButtonForegroundCheckedPressed`   |
| `ChipIconForegroundCheckedDisabled`   | `Brush`        | `TextOnAccentFillColorDisabledBrush`     |
| `ChipBorderBrush` | `Brush` | `ControlStrokeColorDefaultBrush` |
| `ChipBorderBrushPointerOver` | `Brush` | `ControlStrokeColorDefaultBrush` |
| `ChipBorderBrushPressed` | `Brush` | `ControlStrokeColorDefaultBrush` |
| `ChipBorderBrushDisabled` | `Brush` | `ControlStrokeColorDefaultBrush` |
| `ChipBorderBrushChecked` | `Brush` | `ControlFillColorTransparentBrush` |
| `ChipBorderBrushCheckedPointerOver` | `Brush` | `ControlFillColorTransparentBrush` |
| `ChipBorderBrushCheckedPressed` | `Brush` | `ControlFillColorTransparentBrush` |
| `ChipBorderBrushCheckedDisabled` | `Brush` | `ControlFillColorTransparentBrush` |
| `ChipDeleteIconBackground`            | `Brush`        | `SubtleFillColorTransparentBrush`        |
| `ChipDeleteIconBackgroundPointerOver` | `Brush`        | `SubtleFillColorSecondaryBrush`          |
| `ChipDeleteIconBackgroundPressed`     | `Brush`        | `SubtleFillColorTertiaryBrush`           |
| `ChipDeleteIconForeground`            | `Brush`        | `ToggleButtonForeground`                 |

The right inset of the text is `ChipPadding` plus `ChipContentMargin`, which lets the remove button sit closer to the edge than the text does.

The default style has no `Chip*Focused`, `ChipStateOverlay*`, or `ChipElevation*` resources: focus uses the system focus visual, and there is no overlay or shadow layer.

### Material

| Key                                    | Type              | Value                               |
|----------------------------------------|-------------------|-------------------------------------|
| `ChipContentMinHeight`                 | `Double`          | 20                                  |
| `ChipDeleteIconContainerLength`        | `Double`          | 18                                  |
| `ChipDeleteIconLength`                 | `Double`          | 11                                  |
| `ChipElevation`                        | `Double`          | 4                                   |
| `ChipElevationDisabled`                | `Double`          | 0                                   |
| `ChipHeight`                           | `Double`          | 32                                  |
| `ChipIconSize`                         | `Double`          | 18                                  |
| `ChipSize`                             | `Double`          | 12                                  |
| `ChipCornerRadius`                     | `CornerRadius`    | 8                                   |
| `ChipBorderThickness`                  | `Thickness`       | 1                                   |
| `ChipContentMargin`                    | `Thickness`       | 8,0                                 |
| `ChipElevationBorderThickness`         | `Thickness`       | 0                                   |
| `ChipElevationMargin`                  | `Thickness`       | 4                                   |
| `ChipPadding`                          | `Thickness`       | 8,0                                 |
| `ChipBackground`                       | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipBackgroundPointerOver`            | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipBackgroundFocused`                | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipBackgroundPressed`                | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipBackgroundDisabled`               | `SolidColorBrush` | `OnSurfaceDisabledBrush`            |
| `ChipBackgroundChecked`                | `SolidColorBrush` | `SecondaryContainerBrush`           |
| `ChipBackgroundCheckedPointerOver`     | `SolidColorBrush` | `SecondaryContainerBrush`           |
| `ChipBackgroundCheckedFocused`         | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipBackgroundCheckedPressed`         | `SolidColorBrush` | `SecondaryContainerBrush`           |
| `ChipBackgroundCheckedDisabled`        | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipForeground`                       | `SolidColorBrush` | `OnSurfaceVariantBrush`             |
| `ChipForegroundPointerOver`            | `SolidColorBrush` | `OnSurfaceVariantBrush`             |
| `ChipForegroundFocused`                | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipForegroundPressed`                | `SolidColorBrush` | `OnSurfaceVariantBrush`             |
| `ChipForegroundDisabled`               | `SolidColorBrush` | `OnSurfaceDisabledBrush`            |
| `ChipForegroundChecked`                | `SolidColorBrush` | `OnSecondaryContainerBrush`         |
| `ChipForegroundCheckedPointerOver`     | `SolidColorBrush` | `OnSecondaryContainerBrush`         |
| `ChipForegroundCheckedFocused`         | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipForegroundCheckedPressed`         | `SolidColorBrush` | `OnSecondaryContainerBrush`         |
| `ChipForegroundCheckedDisabled`        | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipDeleteIconBackground`             | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipDeleteIconForeground`             | `SolidColorBrush` | `OnSurfaceVariantBrush`             |
| `ChipIconForeground`                   | `SolidColorBrush` | `PrimaryBrush`                      |
| `ChipIconForegroundPointerOver`        | `SolidColorBrush` | `PrimaryBrush`                      |
| `ChipIconForegroundFocused`            | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipIconForegroundPressed`            | `SolidColorBrush` | `PrimaryBrush`                      |
| `ChipIconForegroundDisabled`           | `SolidColorBrush` | `OnSurfaceDisabledBrush`            |
| `ChipIconForegroundChecked`            | `SolidColorBrush` | `PrimaryBrush`                      |
| `ChipIconForegroundCheckedPointerOver` | `SolidColorBrush` | `PrimaryBrush`                      |
| `ChipIconForegroundCheckedFocused`     | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipIconForegroundCheckedPressed`     | `SolidColorBrush` | `PrimaryBrush`                      |
| `ChipIconForegroundCheckedDisabled`    | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipBorderBrush`                      | `SolidColorBrush` | `OutlineBrush`                      |
| `ChipBorderBrushPointerOver`           | `SolidColorBrush` | `OutlineBrush`                      |
| `ChipBorderBrushFocused`               | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipBorderBrushPressed`               | `SolidColorBrush` | `OutlineBrush`                      |
| `ChipBorderBrushDisabled`              | `SolidColorBrush` | `OnSurfaceVariantDisabledBrush`     |
| `ChipBorderBrushChecked`               | `SolidColorBrush` | `OutlineBrush`                      |
| `ChipBorderBrushCheckedPointerOver`    | `SolidColorBrush` | `OutlineBrush`                      |
| `ChipBorderBrushCheckedFocused`        | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipBorderBrushCheckedPressed`        | `SolidColorBrush` | `OutlineBrush`                      |
| `ChipBorderBrushCheckedDisabled`       | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipStateOverlay`                     | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipStateOverlayPointerOver`          | `SolidColorBrush` | `OnSurfaceVariantHoverBrush`        |
| `ChipStateOverlayFocused`              | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipStateOverlayPressed`              | `SolidColorBrush` | `OnSurfaceVariantPressedBrush`      |
| `ChipStateOverlayDisabled`             | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipStateOverlayChecked`              | `SolidColorBrush` | `SecondaryContainerBrush`           |
| `ChipStateOverlayCheckedPointerOver`   | `SolidColorBrush` | `OnSecondaryContainerHoverBrush`    |
| `ChipStateOverlayCheckedFocused`       | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipStateOverlayCheckedPressed`       | `SolidColorBrush` | `OnSecondaryContainerSelectedBrush` |
| `ChipStateOverlayCheckedDisabled`      | `SolidColorBrush` | `SystemControlTransparentBrush`     |
| `ChipRippleFeedback`                   | `SolidColorBrush` | `OnSurfaceFocusedBrush`             |
| `ElevatedChipBackground`               | `SolidColorBrush` | `SurfaceBrush`                      |

## ChipGroup

`ChipGroup` is a specialized `ItemsControl` used to present a collection of `Chip`s.

### C\#

```csharp
public partial class ChipGroup : ItemsControl
```

### XAML

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<utu:ChipGroup .../>
-or-
<utu:ChipGroup>
    oneOrMoreItems
</utu:ChipGroup>
```

### Inheritance

`Object` &#8594; `DependencyObject` &#8594; `UIElement` &#8594; `FrameworkElement` &#8594; `Control` &#8594; `ItemsControl` &#8594; `ChipGroup`

### Constructors

| Constructor   | Description                                          |
|---------------|------------------------------------------------------|
| `ChipGroup()` | Initializes a new instance of the `ChipGroup` class. |

### Properties

| Property              | Type                  | Description                                                                                                                                                                       |
|-----------------------|-----------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `CanRemove`           | `bool`                | Gets or sets the value of each `Chip.CanRemove`.                                                                                                                                  |
| `IconTemplate`        | `DataTemplate`        | Gets or sets the value of each `Chip.IconTemplate`.                                                                                                                               |
| `SelectedItem`        | `object`              | Gets or sets the selected item. <br/> note: This property only works for `ChipSelectionMode.Single` or `SingleOrNone`.                                                            |
| `SelectedItems`       | `IList`               | Gets or sets the selected items. <br/> note: The value will be null if the selection is empty. This property only works for `ChipSelectionMode.Multiple`.                         |
| `SelectionMemberPath` | `string`              | Gets or sets the path which each `Chip.IsChecked` is data-bind to.                                                                                                                |
| `SelectionMode`       | `ChipSelectionMode`\* | Gets or sets the selection behavior: `None`, `SingleOrNone`, `Single`, `Multiple` <br/> note: Changing this value will cause `SelectedItem` and `SelectedItems` to be re-coerced. |

#### Remarks

- `ChipSelectionMode`: Defines constants that specify the selection behavior for a ChipGroup.
  > Different numbers of selected items are guaranteed: None=0, SingleOrNone=0 or 1, Single=1, Multiple=0 or many.
  - `None`: Selection is disabled.
  - `SingleOrNone`: Up to one item can be selected at a time. The current item can be deselected.
  - `Single`: One item is selected at any time. The current item cannot be deselected.
  - `Multiple`: The current item cannot be deselected.

### Events

All events below are forwarded from the nested `Chip`s:

| Event           | Type                           | Description                                       |
|-----------------|--------------------------------|---------------------------------------------------|
| `ItemClick`     | `ChipItemEventHandler`         | Occurs when a `Chip` item is pressed.             |
| `ItemChecked`   | `ChipItemEventHandler`         | Occurs when a `Chip` item is checked.             |
| `ItemUnchecked` | `ChipItemEventHandler`         | Occurs when a `Chip` item is unchecked.           |
| `ItemRemoved`   | `ChipItemEventHandler`         | Occurs when a `Chip` is removed.                  |
| `ItemRemoving`  | `ChipItemRemovingEventHandler` | Occurs when a `Chip` item is about to be removed. |

```csharp
delegate void ChipItemEventHandler(object sender, ChipItemEventArgs e);
delegate void ChipItemRemovingEventHandler(object sender, ChipItemRemovingEventHandler e);

class ChipItemEventArgs : EventArgs
{
    // Gets the item associated with the event
    object Item { get; }
}
class ChipItemRemovingEventHandler : ChipItemEventHandler
{
    // Gets or sets whether the ItemRemoved event should be canceled.
    bool Cancel { get; set; }
}
```

### Usage

Without a design-system package, `ChipGroup` uses `DefaultChipGroupStyle`, which lays its chips out in a horizontal row. The `Style` values below come from the Material and Simple Toolkit libraries; omit them to use the default style.

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...
<!-- example with event -->
<utu:ChipGroup ItemClick="ChipGroup_ItemClick" Style="{StaticResource InputChipGroupStyle}">
    <utu:Chip Content="Chip" />
    <utu:Chip Content="Chip" IsChecked="True" />
    <utu:Chip Content="Chip" />
</utu:ChipGroup>

<!-- example with binding -->
<utu:ChipGroup ItemsSource="{Binding Items}" Style="{StaticResource SuggestionChipGroupStyle}">
    <utu:ChipGroup.ItemTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Name}" />
        </DataTemplate>
    </utu:ChipGroup.ItemTemplate>
</utu:ChipGroup>

<!-- single selection with binding -->
<utu:ChipGroup ItemsSource="{Binding Items}"
               SelectedItem="{Binding SelectedItem, Mode=TwoWay}"
               SelectionMode="Single"
               Style="{StaticResource SuggestionChipGroupStyle}" />

<!-- multi-selection with binding -->
<utu:ChipGroup ItemsSource="{Binding Items}"
               SelectedItems="{Binding SelectedItems, Mode=TwoWay}"
               SelectionMode="Multiple"
               Style="{StaticResource SuggestionChipGroupStyle}" />
```
