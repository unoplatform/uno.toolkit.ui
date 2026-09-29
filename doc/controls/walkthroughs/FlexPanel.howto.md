---
uid: Toolkit.Controls.FlexPanel.HowTo
tags: [flexpanel, flexbox, css, layout, yoga, wrap, grow, shrink, basis, gap, justify-content, align-items, align-self, responsive]
---

# Lay out content with CSS Flexbox rules (direction, wrapping, grow/shrink distribution, gaps, per-item alignment)

---

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

---

## Add FlexPanel to a page

**Why:** Use a single container that follows the CSS Flexbox specification rather than an approximation of it.

```xml
<Page
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:utu="using:Uno.Toolkit.UI">

    <utu:FlexPanel>
        <!-- children -->
    </utu:FlexPanel>
</Page>
```

* A bare `<utu:FlexPanel>` is a single-line row: `Direction` defaults to `Row` and `Wrap` to `NoWrap`.
* Layout is computed by a vendored port of [Meta's Yoga](https://github.com/facebook/yoga) engine, so results match CSS.

> [!TIP]
> If you are reproducing a Figma auto-layout frame rather than a CSS layout, use [`AutoLayout`](../AutoLayoutControl.md) instead. See [FlexPanel vs AutoLayout](../FlexPanel.md) for the full comparison.

---

## Size a child with `Basis`, not `Width`

**Outcome:** The child is arranged from the flex resolution, not from its `Width`.

```xml
<utu:FlexPanel Width="400">
    <!-- Arranges 400 wide, not 200. -->
    <Border Width="200"
            utu:FlexPanel.Basis="100"
            utu:FlexPanel.Grow="1" />
</utu:FlexPanel>
```

* This is the most common surprise when coming from WinUI. On the main axis, `Basis` is the flex base size and `Width` is not.
* `Grow` resolves the final slot; `Width` only seeds the measure.
* `Basis` accepts points only — percentages are not supported.

---

## Make columns exactly equal width

**Outcome:** Every column is the same width regardless of its content.

```xml
<utu:FlexPanel Width="300" ColumnGap="8">
    <Border utu:FlexPanel.Grow="1" utu:FlexPanel.Basis="0" />
    <Border utu:FlexPanel.Grow="1" utu:FlexPanel.Basis="0" />
</utu:FlexPanel>
```

* `Grow="1"` **alone** distributes only the leftover space, so items with different content stay different sizes.
* Equal columns are the CSS `flex: 1 1 0` shorthand — you need **both** `Grow` and `Basis="0"`. Setting `Basis="0"` takes the content size out of the equation entirely.

---

## Split leftover space by ratio

**Outcome:** Two children share the free space 1:2.

```xml
<utu:FlexPanel Width="300">
    <!-- 100 wide -->
    <Border utu:FlexPanel.Grow="1" utu:FlexPanel.Basis="0" />
    <!-- 200 wide -->
    <Border utu:FlexPanel.Grow="2" utu:FlexPanel.Basis="0" />
</utu:FlexPanel>
```

* `Grow` is a ratio, not a length in pixels.
* `Shrink` distributes a *deficit* by the same ratio rule: a child with `Shrink="1"` beside a child with `Shrink="0"` absorbs the whole overflow. Nothing stops a child at its content width — see [Keep a child from shrinking too far](#keep-a-child-from-shrinking-too-far).

---

## Stop a child from shrinking

**Outcome:** The children keep their natural size and the row overflows instead of compressing.

```xml
<utu:FlexPanel Width="200">
    <Border Width="150" utu:FlexPanel.Shrink="0" />
    <Border Width="150" utu:FlexPanel.Shrink="0" />
</utu:FlexPanel>
```

* `Shrink` defaults to `1`, so children compress to fit by default. `Shrink="0"` opts out.
* With every child at `Shrink="0"`, flexbox **overflows** rather than fitting the content to the panel. This is a deliberate difference from `AutoLayout`, which would fit them.

---

## Keep a child from shrinking too far

**Outcome:** A child compresses with its siblings, but never below a floor you choose.

```xml
<utu:FlexPanel Width="100">
    <!-- Arranges 80 wide; the sibling takes the remaining 20. -->
    <Border Width="100" utu:FlexPanel.FlexMinWidth="80" />
    <Border Width="100" />
</utu:FlexPanel>
```

* By default a child can shrink all the way to `0`, as in Yoga and React Native. Unlike CSS ([Flexbox 4.5](https://www.w3.org/TR/css-flexbox-1/#min-size-auto)), there is no automatic min-content floor, because WinUI cannot report an element's min-content size.
* `FlexMinWidth` clamps the width and `FlexMinHeight` the height, whatever the `Direction`.
* To keep a child at its natural size instead, use `Shrink="0"`.

> [!NOTE]
> `FlexMinWidth` / `FlexMinHeight` are **not** `FrameworkElement.MinWidth` / `MinHeight`. The framework properties force `Measure` to return at least *X*; these clamp the flex-resolved slot to at least *X*.

---

## Wrap onto multiple lines

**Outcome:** Children that do not fit start a new line.

```xml
<utu:FlexPanel Width="250"
               Wrap="Wrap"
               RowGap="20"
               AlignContent="FlexStart">
    <Border Width="100" Height="40" />
    <Border Width="100" Height="40" />
    <!-- Wraps to a second line, 40 + 20 below the first. -->
    <Border Width="100" Height="40" />
</utu:FlexPanel>
```

* `Wrap` is the capability `AutoLayout` does not have at all.
* `RowGap` separates the lines in a row layout; `ColumnGap` separates them in a column layout.
* `AlignContent` positions the **lines** as a group on the cross axis, and only does anything when the content actually wraps.

---

## Switch between a row and a column

**Outcome:** The same children stack horizontally or vertically.

```xml
<utu:FlexPanel Direction="Column" RowGap="8">
    <TextBlock Text="Title" />
    <TextBlock Text="Subtitle" />
</utu:FlexPanel>
```

* `Direction`: `Row` (default), `Column`, `RowReverse`, `ColumnReverse`.
* Changing `Direction` swaps which axis is the main axis — `JustifyContent` always applies to the main axis and `AlignItems` to the cross axis.

> [!NOTE]
> `Direction` defaults to `Row`, which is not the zero value of the `FlexDirection` enum — the enum preserves the underlying engine numbering, in which `Column = 0`.

---

## Space children with gaps, and hide one without leaving a hole

**Outcome:** Uniform separation that collapses cleanly when a child is hidden.

```xml
<utu:FlexPanel Width="400" ColumnGap="10">
    <Border Width="50" Height="50" />
    <Border Width="50" Height="50" Visibility="Collapsed" />
    <!-- Starts at x=60: 50 wide plus a single 10 gap. -->
    <Border Width="50" Height="50" />
</utu:FlexPanel>
```

* `Visibility="Collapsed"` maps to CSS `display: none`: the child contributes no size **and no gap slot**.
* A negative gap is clamped to `0`. Use a negative child `Margin` if you need items to overlap.

---

## Distribute children along the main axis

**Outcome:** Items pack, center, or spread along the main axis.

```xml
<utu:FlexPanel Width="400"
               ColumnGap="10"
               JustifyContent="SpaceBetween">
    <Border Width="50" Height="50" />
    <!-- Pushed to x=350. -->
    <Border Width="50" Height="50" />
</utu:FlexPanel>
```

* `JustifyContent`: `FlexStart` (default), `Center`, `FlexEnd`, `SpaceBetween`, `SpaceAround`, `SpaceEvenly`.
* A gap is a **floor**, not the final separation — `SpaceBetween` distributes whatever is left over on top of it.

---

## Align children on the cross axis

**Outcome:** Items align across the axis opposite `Direction`, with per-item overrides.

```xml
<utu:FlexPanel Height="200" AlignItems="Center" ColumnGap="8">
    <Border Width="64" Height="64" />
    <Border Width="64" Height="64" utu:FlexPanel.AlignSelf="FlexEnd" />
</utu:FlexPanel>
```

* `AlignItems`: `Stretch` (default), `FlexStart`, `Center`, `FlexEnd`, `Baseline`.
* `AlignSelf` on a child overrides the container for that child; `Auto` (its default) means inherit `AlignItems`.
* `AlignItems="Baseline"` aligns text baselines rather than boxes — with mixed font sizes, the smaller text is offset down so both sit on the same baseline.

---

## Take a child out of the flow

**Outcome:** One child is positioned by insets and does not displace its siblings.

```xml
<utu:FlexPanel Width="400" Height="200">
    <!-- Still arranged at x=0. -->
    <Border Width="100" Height="50" />

    <Border Width="60" Height="30"
            utu:FlexPanel.Position="Absolute"
            utu:FlexPanel.Left="200"
            utu:FlexPanel.Top="100" />
</utu:FlexPanel>
```

* `Position="Absolute"` removes the child from the flex line; `Left`, `Top`, `Right` and `Bottom` place it against the panel.
* Flowed siblings are laid out as if the absolute child were not there.
* Insets are measured from the panel's own edges, so the panel needs a resolved size — either set `Width` / `Height` on it as above, or let it fill a host that has one. In an auto-sized panel the insets resolve against a box that the flowed children determine.

---

## Mirror the layout for right-to-left

**Outcome:** The main axis runs right-to-left.

```xml
<utu:FlexPanel FlowDirection="RightToLeft" ColumnGap="8">
    <Border Width="60" Height="40" />
    <Border Width="60" Height="40" />
</utu:FlexPanel>
```

* `FlowDirection` works as it does on any other panel: the platform mirrors the whole panel, and the engine itself always lays out left-to-right.
* The mirroring is visual, so the `Left` / `Right` insets of an absolute child swap sides too, as `Canvas.Left` does.

---

## Quick reference: key properties & attachments

* **Container:** `Direction` (`Row|Column|RowReverse|ColumnReverse`), `Wrap` (`NoWrap|Wrap|WrapReverse`),
  `JustifyContent`, `AlignItems`, `AlignContent`, `ColumnGap`, `RowGap`, `Padding`.
  Right-to-left comes from the inherited `FlowDirection`.
* **Per-child (attached):** `Grow`, `Shrink`, `Basis`, `FlexMinWidth`, `FlexMinHeight`,
  `AlignSelf`, `Position`, `Left`, `Top`, `Right`, `Bottom`.
* **Read directly off the child:** `Margin`, `Width`, `Height`, `Visibility`.

See [FlexPanel](../FlexPanel.md) for types, defaults and the CSS equivalent of each.

---

## Notes & gotchas

* **`Basis` beats `Width` on the main axis.** If a child arranges at an unexpected size, check whether `Grow` or `Basis` is resolving the slot.
* **`Grow="1"` is not `flex: 1 1 0`.** Add `Basis="0"` whenever you want equal shares rather than equal *leftovers*.
* **No min-content floor.** Children shrink to `0` by default; set `FlexMinWidth` / `FlexMinHeight`, or `Shrink="0"`, to protect a label.
* **`FlexAlign` is shared.** As in CSS, `AlignItems`, `AlignContent` and `AlignSelf` use one keyword set, so `Auto` only means something on `AlignSelf` and the `Space*` members only on `AlignContent`.
* **No border or corner radius.** `FlexPanel` derives from `Panel`, which exposes only `Background`. Wrap it in a `Border` if you need one.
* **No CSS `order`.** Reorder the children instead.
* **Pixel snapping** follows `UseLayoutRounding`. Set it to `false` if you see sub-pixel drift from double-rounding on a particular target.
