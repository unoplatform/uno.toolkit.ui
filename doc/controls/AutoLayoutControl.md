---
uid: Toolkit.Controls.AutoLayoutControl
---

# AutoLayout

> [!TIP] 
> This guide covers details for `AutoLayoutControl` specifically. If you are just getting started with the Uno Toolkit UI Library, please see our [general getting started](xref:Toolkit.GettingStarted) page to make sure you have the correct setup in place.

## Summary
`AutoLayoutControl` arranges child elements into a single line that can be oriented horizontally or vertically. It is the reflection in XAML of the [Autolayout frame in Figma](https://www.figma.com/widget-docs/api/component-AutoLayout).

### C#
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
Object &#8594; DependencyObject &#8594; UIElement &#8594; FrameworkElement &#8594; Control

### Constructors
| Constructor     | Description                                              |
|-----------------|----------------------------------------------------------|
| AutoLayout() | Initializes a new instance of the `AutoLayout` class. |

### Properties
Property|Type|Description
-|-|-
Orientation | Orientation | Defines constants that specify the different orientations that a control or layout can have. 
Spacing | double | Gets or sets the uniform distance (in pixels) between grid. Each grid of the AutoLayout will have the spacing set by this value depending the AautoLayout orientation.
Justify | AutoLayoutJustify | Either Stack, space between children behave like a `StackPanel`, or SpaceBetween, between each children a equal space is added assuring that all the `AutoLayout` space is occupy. Note : if a child has his `PrimaryAlignment` set to Stretch, it will default to Stack.
Children |  AutoLayoutChildren | List of FrameworkElement that are the AutoLayout children.
PrimaryAxisAlignment | AutoLayoutAlignment | Indicates where the content be displayed on the axis of the AutoLayout orientation.
IsReverseZIndex | bool | Reverse the Z-index of all the children.
Padding | Thickness | /!\ Padding for `AutoLayout` behave the same as it does within the Figma Plugin: The anchor points determine which sides of the Padding will taken into consideration. For example, items that are aligned to the Right and Top positions will only take the `Tickness.Right` and `Thickness.Top` values of `Padding` into consideration.

### Attached Properties
Property|Type|Description
-|-|-
PrimaryAlignment|AutoLayoutPrimaryAlignment|Either Auto, the child grid take only the place it need in the autolayout direction, or Stretch, the child grid take all the available place in the autolayout direction. Note if set to Strech AutoLayoutJustify will behave as if in Stack mode.
CounterAlignment|AutoLayoutAlignment|Indicates where an element should be displayed on the counter axis of the parent orientation relative to the allocated layout slot of the parent element.
PrimaryLength|double| Sets the height or width of the child depending on the `Orientation`. If height or width are already set they will have priority.
CounterLength|double|Sets the height or width of the child depending on the inverse of `Orientation`. If height or width are already set they will have priority.
IsIndependentLayout | bool | Make the child independent of the AutoLayout positioning. Should not be used with other Attached Properties. Reflect the Absolute Position option from Figma.