---
uid: Toolkit.Controls.FlexPanel
---

# FlexPanel

> [!TIP]
> This guide covers details for the `FlexPanel` control. If you are just getting started with the Uno Toolkit UI Library, please see our [general getting started](../getting-started.md) page to make sure you have the correct setup in place.

## Summary

`FlexPanel` arranges its children using [CSS Flexbox](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_flexible_box_layout) semantics. Layout is computed by a vendored C# port of [Meta's Yoga](https://github.com/facebook/yoga) engine, so sizing, wrapping and distribution follow the CSS specification rather than an approximation of it. That port — and the `Panel` adapter this control is adapted from — comes from Microsoft's [Reactor](https://github.com/microsoft/microsoft-ui-reactor) project. Both are MIT; see [THIRD-PARTY-NOTICES.md](../../THIRD-PARTY-NOTICES.md).

For task-oriented recipes, see the [FlexPanel walkthrough](walkthroughs/FlexPanel.howto.md).

### C\#

```csharp
public partial class FlexPanel : Panel
```

### XAML

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<utu:FlexPanel Direction="Row"
               Wrap="Wrap"
               AlignItems="Center"
               ColumnGap="8"
               RowGap="8"
               Padding="16,8">
    <TextBlock Text="MyApp" utu:FlexPanel.Shrink="0" />
    <Border utu:FlexPanel.Grow="1" />
    <Button Content="Settings" />
</utu:FlexPanel>
```

## FlexPanel vs AutoLayout: which do I want?

Both arrange children along an axis, and they are independent controls — neither replaces the other.

| Use | When |
|---|---|
| `FlexPanel` | You are thinking in CSS terms: `flex-grow` / `flex-shrink` / `flex-basis` distribution, wrapping onto multiple lines, `justify-content` / `align-items`, gaps that act as a floor. Also when porting a web layout, where matching flexbox exactly matters. |
| [`AutoLayout`](AutoLayoutControl.md) | You are reproducing a [Figma auto-layout frame](https://www.figma.com/widget-docs/api/component-AutoLayout) and want its semantics — including Figma-specific concepts such as independent (absolutely positioned) children, reverse Z-index, and negative spacing. |

Two concrete capability differences worth knowing before you choose:

- `FlexPanel` wraps onto multiple lines; `AutoLayout` does not.
- `AutoLayout` supports negative `Spacing` (for deliberately overlapping items such as stacked avatars). `FlexPanel` cannot: the engine clamps a negative gap to `0`. Use a negative child `Margin` instead, which is supported.

## Properties

### Container properties

| Property | Type | Default | CSS equivalent |
|---|---|---|---|
| `Direction` | `FlexDirection` | `Row` | `flex-direction` |
| `Wrap` | `FlexWrap` | `NoWrap` | `flex-wrap` |
| `JustifyContent` | `FlexJustify` | `FlexStart` | `justify-content` |
| `AlignItems` | `FlexAlign` | `Stretch` | `align-items` |
| `AlignContent` | `FlexAlign` | `FlexStart` | `align-content` |
| `ColumnGap` | `double` | `0` | `column-gap` |
| `RowGap` | `double` | `0` | `row-gap` |
| `Padding` | `Thickness` | `0` | `padding` |

### Attached properties

Set these on the *children* of a `FlexPanel`.

| Property | Type | Default | CSS equivalent |
|---|---|---|---|
| `FlexPanel.Grow` | `double` | `0` | `flex-grow` |
| `FlexPanel.Shrink` | `double` | `1` | `flex-shrink` |
| `FlexPanel.Basis` | `double` | `NaN` (= `auto`) | `flex-basis` (points only) |
| `FlexPanel.FlexMinWidth` | `double` | `NaN` (= no floor) | `min-width` |
| `FlexPanel.FlexMinHeight` | `double` | `NaN` (= no floor) | `min-height` |
| `FlexPanel.AlignSelf` | `FlexAlign` | `Auto` | `align-self` |
| `FlexPanel.Position` | `FlexPositionType` | `Relative` | `position` |
| `FlexPanel.Left` / `.Top` / `.Right` / `.Bottom` | `double` | `NaN` | inset properties |

`FlexMinWidth` and `FlexMinHeight` carry the `Flex` prefix deliberately. They would otherwise sit next to `FrameworkElement.MinWidth` / `MinHeight` on the same element while meaning something different: the framework properties force `Measure` to return at least *X*, whereas these clamp the flex-resolved slot to at least *X*.

### Properties that participate without an attached property

A child's `Margin`, `Width`, `Height` and `Visibility` are read directly:

| Child property | Effect |
|---|---|
| `Margin` | Mapped to the item's margin edges. |
| `Width` / `Height` | Mapped to a definite item size; `NaN` means `auto`. |
| `Visibility="Collapsed"` | Mapped to `display: none` — the child contributes no size **and no gap slot**. |

### Which `FlexAlign` members apply where

As in CSS, `AlignItems`, `AlignContent` and `AlignSelf` share one keyword set (`FlexAlign`), so not every member is meaningful on every property. Every member of `FlexDirection`, `FlexWrap`, `FlexJustify` and `FlexPositionType` has an effect.

| Property | Meaningful members |
|---|---|
| `JustifyContent` | `FlexStart`, `Center`, `FlexEnd`, `SpaceBetween`, `SpaceAround`, `SpaceEvenly` |
| `AlignItems` | `Stretch`, `FlexStart`, `Center`, `FlexEnd`, `Baseline` |
| `AlignContent` | `FlexStart`, `Center`, `FlexEnd`, `Stretch`, `SpaceBetween`, `SpaceAround`, `SpaceEvenly` |
| `AlignSelf` | `Auto` plus everything valid for `AlignItems` |

## `Basis` vs `Width`

This is the most common surprise when coming from WinUI. On the main axis, `Basis` is the flex base size and `Width` is not:

```xml
<!-- Arranges 400 wide, not 200: Grow resolves the slot and Width is not the flex base size. -->
<utu:FlexPanel Width="400">
    <Border Width="200" utu:FlexPanel.Basis="100" utu:FlexPanel.Grow="1" />
</utu:FlexPanel>
```

### Equal columns need `Basis="0"`

`Grow="1"` alone distributes only the **leftover** space, so items with different content stay different sizes. Equal columns are the CSS `flex: 1 1 0` shorthand — both `Grow` and `Basis`:

```xml
<!-- Three columns of exactly the same width, whatever their content. -->
<utu:FlexPanel ColumnGap="8">
    <Border utu:FlexPanel.Grow="1" utu:FlexPanel.Basis="0" />
    <Border utu:FlexPanel.Grow="1" utu:FlexPanel.Basis="0" />
    <Border utu:FlexPanel.Grow="1" utu:FlexPanel.Basis="0" />
</utu:FlexPanel>
```

## Minimum size

By default a child can shrink all the way to `0`, as in Yoga and React Native. This is a deliberate difference from CSS, where [§4.5](https://www.w3.org/TR/css-flexbox-1/#min-size-auto) floors a flex item at its min-content size (for text, its longest word). WinUI has no way to ask an element for its min-content size, so that floor cannot be computed.

To keep a child from being crushed:

- set `FlexPanel.Shrink="0"` to keep it at its natural size, or
- set `FlexPanel.FlexMinWidth` / `FlexPanel.FlexMinHeight` to an explicit floor.

`FlexMinWidth` always clamps the width and `FlexMinHeight` always clamps the height, whatever the `Direction`.

## Right-to-left

Set `FlowDirection="RightToLeft"` (inherited from `FrameworkElement`), as with any other panel. The engine always lays out left-to-right and the platform mirrors the whole panel, so the main axis starts on the right.

Because the mirroring is visual, the `Left` / `Right` insets of an absolutely positioned child swap sides too, the same way `Canvas.Left` does. CSS treats `left` / `right` as physical edges instead.

## Layout rounding

`UseLayoutRounding` (inherited from `UIElement`) controls whether the engine snaps results to the pixel grid:

- `true` (default) — the engine rounds using the current `XamlRoot.RasterizationScale`.
- `false` — the engine emits unrounded values and only the platform's own rounding applies.

Set it to `false` if you see sub-pixel drift from double-rounding on a particular target.

## Limitations

- **No border or corner radius.** `FlexPanel` derives from `Panel`, which exposes only `Background`. Wrap it in a `Border` if you need one.
- **No `order`.** CSS `order` is not supported and is not planned: the underlying engine does not implement it. Reorder the children instead.
- **Negative gaps clamp to `0`.** Use a negative child `Margin` for overlap.
- **`Basis` is points only.** Percentage values are not supported.
- **Not yet surfaced:** `MaxWidth`/`MaxHeight`, `AspectRatio`, `Overflow`, `BoxSizing`, the `flex` shorthand, and CSS Grid. Each is additive and may be added later.

## Samples

The **FlexPanel** page under *Controls* in the sample app walks through each property with a labelled example. Its **Open the interactive playground** button opens a full-screen surface where every container and per-child property is driven live, including a `UseLayoutRounding` toggle, a `FlowDirection` toggle, a 200-child stress case, and a readout of each child's arranged rectangle.
