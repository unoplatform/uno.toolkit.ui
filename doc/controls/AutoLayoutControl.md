---
uid: Toolkit.Controls.AutoLayoutControl
---

# AutoLayout

> [!TIP]
> This guide covers details for the `AutoLayout` control. If you are just getting started with the Uno Toolkit UI Library, please see our [general getting started](../getting-started.md) page to make sure you have the correct setup in place.

## Summary

`AutoLayout` arranges child elements into a single row or column, depending on the `Orientation` property. The control is intended to reflect the same behaviors as the [AutoLayout Frame component in Figma](https://www.figma.com/widget-docs/api/component-AutoLayout).

### C\#

```csharp
public partial class AutoLayout : Control
```

### XAML

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<utu:AutoLayout>
    contents
</utu:AutoLayout>
```

### Inheritance

`Object` &#8594; `DependencyObject` &#8594; `UIElement` &#8594; `FrameworkElement` &#8594; `Control` &#8594; `AutoLayout`

### Constructors

| Constructor    | Description                                           |
|----------------|-------------------------------------------------------|
| `AutoLayout()` | Initializes a new instance of the `AutoLayout` class. |

### Properties

| Property               | Type                  | Description                                                                                                                                                                                                                                                                                                                                            |
|------------------------|-----------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Orientation`          | `Orientation`         | Gets or sets the dimension by which the items are stacked.                                                                                                                                                                                                                                                                                             |
| `Spacing`              | `double`              | Gets or sets a uniform distance (in pixels) between stacked items. It is applied in the direction of the `AutoLayout`'s Orientation.                                                                                                                                                                                                                   |
| `Justify`              | `AutoLayoutJustify`   | Gets or sets the value to determine how items are justified within the container. Options are `Stack` or `SpaceBetween`. Note: If a child has its `PrimaryAlignment` set to `Stretch`, it will default to `Stack`.                                                                                                                                     |
| `PrimaryAxisAlignment` | `AutoLayoutAlignment` | Gets or sets the alignment characteristics that are applied to the content, based on the value of the `Orientation` property.  Options are `Start`, `Center`, `End` and `Stretch`. The default is `Start`.                                                                                                                                             |
| `CounterAxisAlignment` | `AutoLayoutAlignment` | Gets or sets the alignment characteristics that are applied to the content, based on the inverse value of the `Orientation` property. Options are `Start`, `Center`, `End`, and `Stretch`. The default is `Stretch`. If already set in `CounterAlignment`, CounterAlignment will have priority.                                                        |
| `IsReverseZIndex`      | `bool`                | Gets or sets whether or not the ZIndex of the children should be reversed. The default is `false`.                                                                                                                                                                                                                                                     |
| `Padding`              | `Thickness`           | **WARNING:** Padding for `AutoLayout` behaves the same as it does within the Figma Plugin: The anchor points determine which sides of the Padding will be taken into consideration. For example, items that are aligned to the Right and Top positions will only take the `Tickness.Right` and `Thickness.Top` values of `Padding` into consideration. |

### Attached Properties

| Property              | Type                         | Description                                                                                                                                                                                                                                                                                                                                              |
|-----------------------|------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `PrimaryAlignment`    | `AutoLayoutPrimaryAlignment` | Gets or sets the alignment characteristics that are applied to this element when it is composed in an `AutoLayout` parent, based on the value of the `Orientation` property. Options are `Auto` and `Stretch`. The default is `Auto`.                                                                                                                    |
| `CounterAlignment`    | `AutoLayoutAlignment`        | Gets or sets the counter-alignment characteristics that are applied to this element when it is composed in an `AutoLayout` parent, based on the inverse value of the `Orientation` property. Options are `Start`, `Center`, `End`, and `Stretch`. The default is `Start`. If already set in `CounterAxisAlignment`, CounterAlignment will have priority. |
| `PrimaryLength`       | `double`                     | Gets or sets the height or width of the child depending on the `Orientation`. If `Height` or `Width` are already set they will have priority.                                                                                                                                                                                                            |
| `CounterLength`       | `double`                     | Gets or sets the height or width of the child depending on the inverse of `Orientation`. If `Height` or `Width` are already set they will have priority.                                                                                                                                                                                                 |
| `IsIndependentLayout` | `bool`                       | **WARNING:** Should not be used with other Attached Properties. Gets or sets whether the element is independent of the `AutoLayout` positioning. Reflects the `Absolute Position` option from Figma.                                                                                                                                                     |
