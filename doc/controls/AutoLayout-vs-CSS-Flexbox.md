---
uid: Toolkit.Controls.AutoLayout.FlexboxComparison
---

# AutoLayout vs CSS Flexbox Comparison

This document provides a comprehensive comparison between the Uno Toolkit's `AutoLayout` control and CSS Flexbox. The `AutoLayout` control is an implementation of [Figma's AutoLayout](https://www.figma.com/widget-docs/api/component-AutoLayout), which shares many concepts with CSS Flexbox but has some intentional differences to align with Figma's design paradigm.

## Overview

| Aspect | CSS Flexbox | AutoLayout |
|--------|-------------|------------|
| Origin | Web standard (CSS) | Figma design tool concept |
| Primary Use | Web layout | Cross-platform XAML layout |
| Wrapping Support | Yes (`flex-wrap`) | No (single row/column only) |
| Direction Options | 4 (`row`, `row-reverse`, `column`, `column-reverse`) | 2 (`Horizontal`, `Vertical`) |
| Z-Index Control | Via `z-index` CSS property | Via `IsReverseZIndex` property |

## Container Properties Comparison

### Direction / Orientation

**CSS Flexbox:**
```css
.container {
    display: flex;
    flex-direction: row | row-reverse | column | column-reverse;
}
```

**AutoLayout:**
```xml
<utu:AutoLayout Orientation="Horizontal" /> <!-- row -->
<utu:AutoLayout Orientation="Vertical" />   <!-- column -->
```

| CSS Flexbox | AutoLayout | Notes |
|-------------|------------|-------|
| `flex-direction: row` | `Orientation="Horizontal"` | Items flow left to right |
| `flex-direction: column` | `Orientation="Vertical"` | Items flow top to bottom |
| `flex-direction: row-reverse` | Not directly supported | See note below |
| `flex-direction: column-reverse` | Not directly supported | See note below |

> [!NOTE]
> **Understanding `IsReverseZIndex`**: AutoLayout's `IsReverseZIndex` property only affects the stacking order (z-index) of children—determining which element appears on top when elements overlap. It does **not** reverse the visual layout order of items.
>
> CSS `row-reverse` and `column-reverse` visually reverse item order (last item appears first). To achieve this in AutoLayout, you would need to either:
> - Reverse the order of children in XAML/code
> - Use data binding with a reversed collection
>
> **Example of what `IsReverseZIndex` does:**
> ```xml
> <!-- Items are still laid out left-to-right, but Item3 has highest z-index (appears on top if overlapping) -->
> <utu:AutoLayout Orientation="Horizontal" IsReverseZIndex="True">
>     <Border x:Name="Item1" /> <!-- Z-Index: 2 (lowest, behind others) -->
>     <Border x:Name="Item2" /> <!-- Z-Index: 1 -->
>     <Border x:Name="Item3" /> <!-- Z-Index: 0 (highest, in front) -->
> </utu:AutoLayout>
> ```

### Gap / Spacing

**CSS Flexbox:**
```css
.container {
    display: flex;
    gap: 10px;           /* Both row and column gap */
    row-gap: 10px;       /* Gap between rows (when wrapped) */
    column-gap: 20px;    /* Gap between columns */
}
```

**AutoLayout:**
```xml
<utu:AutoLayout Spacing="10" />
```

| CSS Flexbox | AutoLayout | Notes |
|-------------|------------|-------|
| `gap` | `Spacing` | Uniform gap between all items |
| `row-gap` | N/A | Not applicable (no wrapping support) |
| `column-gap` | N/A | Not applicable (no wrapping support) |

> [!IMPORTANT]
> AutoLayout only supports a single `Spacing` value applied in the direction of the `Orientation`. Since AutoLayout doesn't support wrapping, `row-gap` and `column-gap` distinctions are not needed.

### Justify Content (Main Axis Alignment)

**CSS Flexbox:**
```css
.container {
    display: flex;
    justify-content: flex-start | flex-end | center | space-between | space-around | space-evenly;
}
```

**AutoLayout:**
```xml
<utu:AutoLayout PrimaryAxisAlignment="Start" Justify="Stack" />
<utu:AutoLayout PrimaryAxisAlignment="Center" />
<utu:AutoLayout PrimaryAxisAlignment="End" />
<utu:AutoLayout Justify="SpaceBetween" />
```

| CSS Flexbox | AutoLayout | Notes |
|-------------|------------|-------|
| `justify-content: flex-start` | `PrimaryAxisAlignment="Start"` | Items packed at the start |
| `justify-content: flex-end` | `PrimaryAxisAlignment="End"` | Items packed at the end |
| `justify-content: center` | `PrimaryAxisAlignment="Center"` | Items centered |
| `justify-content: space-between` | `Justify="SpaceBetween"` | Equal space between items, no space at edges |
| `justify-content: space-around` | Not directly supported | Each item has equal space around it |
| `justify-content: space-evenly` | Not directly supported | Equal space between all items including edges |

> [!TIP]
> To approximate `space-around` or `space-evenly` in AutoLayout, you can use `Justify="SpaceBetween"` combined with appropriate `Padding` values.

**Example - Achieving space-evenly-like behavior:**
```xml
<!-- Approximate space-evenly with SpaceBetween + Padding -->
<utu:AutoLayout Justify="SpaceBetween" Padding="20,0,20,0" Orientation="Horizontal">
    <Border Width="50" Height="50" />
    <Border Width="50" Height="50" />
    <Border Width="50" Height="50" />
</utu:AutoLayout>
```

### Align Items (Cross Axis Alignment)

**CSS Flexbox:**
```css
.container {
    display: flex;
    align-items: flex-start | flex-end | center | stretch | baseline;
}
```

**AutoLayout:**
```xml
<utu:AutoLayout CounterAxisAlignment="Start" />
<utu:AutoLayout CounterAxisAlignment="Center" />
<utu:AutoLayout CounterAxisAlignment="End" />
<utu:AutoLayout CounterAxisAlignment="Stretch" /> <!-- Default -->
```

| CSS Flexbox | AutoLayout | Notes |
|-------------|------------|-------|
| `align-items: flex-start` | `CounterAxisAlignment="Start"` | Items aligned at cross-axis start |
| `align-items: flex-end` | `CounterAxisAlignment="End"` | Items aligned at cross-axis end |
| `align-items: center` | `CounterAxisAlignment="Center"` | Items centered on cross-axis |
| `align-items: stretch` | `CounterAxisAlignment="Stretch"` | Items stretch to fill (default) |
| `align-items: baseline` | Not supported | Align items by their text baseline |

### Align Content (Multi-line Cross Axis Alignment)

**CSS Flexbox:**
```css
.container {
    display: flex;
    flex-wrap: wrap;
    align-content: flex-start | flex-end | center | stretch | space-between | space-around;
}
```

**AutoLayout:**
Not applicable - AutoLayout does not support wrapping, so `align-content` has no equivalent.

### Flex Wrap

**CSS Flexbox:**
```css
.container {
    display: flex;
    flex-wrap: nowrap | wrap | wrap-reverse;
}
```

**AutoLayout:**
Not supported - AutoLayout always behaves like `flex-wrap: nowrap`. Items are laid out in a single row or column.

> [!WARNING]
> If you need wrapping behavior, consider using a `WrapPanel` or other layout controls in combination with AutoLayout.

## Item Properties Comparison

### Flex Grow / Stretch

**CSS Flexbox:**
```css
.item {
    flex-grow: 1;
}
```

**AutoLayout:**
```xml
<Border utu:AutoLayout.PrimaryAlignment="Stretch" />
```

| CSS Flexbox | AutoLayout | Notes |
|-------------|------------|-------|
| `flex-grow: 0` (default) | `PrimaryAlignment="Auto"` | Item uses its natural size |
| `flex-grow: 1` | `PrimaryAlignment="Stretch"` | Item expands to fill available space |
| `flex-grow: 2` (proportional) | Not directly supported | All stretched items share space equally |

> [!NOTE]
> In CSS Flexbox, you can give items different `flex-grow` values for proportional sizing (e.g., `flex-grow: 2` gets twice the extra space). AutoLayout's `PrimaryAlignment="Stretch"` distributes remaining space equally among all stretched children.

**Workaround for proportional sizing:**
```xml
<!-- To achieve 2:1 ratio, nest AutoLayouts -->
<utu:AutoLayout Orientation="Horizontal">
    <utu:AutoLayout utu:AutoLayout.PrimaryAlignment="Stretch">
        <!-- This takes 2 parts -->
        <Border utu:AutoLayout.PrimaryAlignment="Stretch" />
        <Border utu:AutoLayout.PrimaryAlignment="Stretch" />
    </utu:AutoLayout>
    <Border Width="100" /> <!-- This takes 1 part with fixed size -->
</utu:AutoLayout>
```

### Flex Shrink

**CSS Flexbox:**
```css
.item {
    flex-shrink: 1; /* Default - item can shrink */
    flex-shrink: 0; /* Item won't shrink */
}
```

**AutoLayout:**
Use `MinWidth`/`MinHeight` on child elements to prevent shrinking below a certain size.

```xml
<Border MinWidth="100" MinHeight="50" />
```

### Flex Basis

**CSS Flexbox:**
```css
.item {
    flex-basis: 200px;
    flex-basis: auto;
    flex-basis: content;
}
```

**AutoLayout:**
```xml
<!-- Using Width/Height directly -->
<Border Width="200" />

<!-- Or using attached properties -->
<Border utu:AutoLayout.PrimaryLength="200" />
<Border utu:AutoLayout.CounterLength="100" />
```

| CSS Flexbox | AutoLayout | Notes |
|-------------|------------|-------|
| `flex-basis: auto` | `Width`/`Height` not set | Uses content size |
| `flex-basis: 200px` | `Width="200"` or `PrimaryLength="200"` | Fixed size |
| `flex-basis: 0` | Not directly equivalent | Use `PrimaryAlignment="Stretch"` for flexible sizing |

### Align Self (Item-level Cross Axis Override)

**CSS Flexbox:**
```css
.item {
    align-self: auto | flex-start | flex-end | center | stretch | baseline;
}
```

**AutoLayout:**
```xml
<Border utu:AutoLayout.CounterAlignment="Start" />
<Border utu:AutoLayout.CounterAlignment="Center" />
<Border utu:AutoLayout.CounterAlignment="End" />
<Border utu:AutoLayout.CounterAlignment="Stretch" />
```

| CSS Flexbox | AutoLayout | Notes |
|-------------|------------|-------|
| `align-self: auto` | Not set (uses `CounterAxisAlignment`) | Inherits from container |
| `align-self: flex-start` | `CounterAlignment="Start"` | Override to start alignment |
| `align-self: flex-end` | `CounterAlignment="End"` | Override to end alignment |
| `align-self: center` | `CounterAlignment="Center"` | Override to center alignment |
| `align-self: stretch` | `CounterAlignment="Stretch"` | Override to stretch |
| `align-self: baseline` | Not supported | - |

### Order

**CSS Flexbox:**
```css
.item {
    order: 2;
}
```

**AutoLayout:**
Not directly supported. Visual order is determined by the order of children in the XAML.

> [!TIP]
> To change display order dynamically, you would need to reorder the Children collection programmatically or use data binding with a sorted collection.

### Position: Absolute (Independent Layout)

**CSS Flexbox:**
```css
.container {
    position: relative;
}
.item {
    position: absolute;
    top: 10px;
    left: 20px;
}
```

**AutoLayout:**
```xml
<utu:AutoLayout>
    <Border Width="100" Height="100" /> <!-- Normal flow -->
    <Border Width="50" Height="50" 
            utu:AutoLayout.IsIndependentLayout="True"
            VerticalAlignment="Top"
            HorizontalAlignment="Left"
            Margin="20,10,0,0" /> <!-- Absolute positioned -->
</utu:AutoLayout>
```

This mirrors Figma's "Absolute Position" feature, where an item can be positioned independently of the AutoLayout flow.

## Padding Behavior

**CSS Flexbox:**
```css
.container {
    padding: 20px;
}
```

**AutoLayout:**
```xml
<utu:AutoLayout Padding="20" />
```

> [!WARNING]
> AutoLayout's `Padding` behavior matches Figma's implementation: the padding values that apply depend on the alignment settings.

**Example - Padding behavior based on alignment:**
```xml
<!-- When PrimaryAxisAlignment="Start" (default), only start padding applies -->
<utu:AutoLayout Padding="20,10,20,10" PrimaryAxisAlignment="Start">
    <Border /> <!-- Gets left padding (20px) in horizontal, top padding (10px) in vertical -->
</utu:AutoLayout>

<!-- When using Stretch or SpaceBetween, both start and end padding apply -->
<utu:AutoLayout Padding="20,10,20,10" Justify="SpaceBetween">
    <Border /> <!-- Gets both left (20px) and right (20px) padding in horizontal -->
</utu:AutoLayout>

<!-- When PrimaryAxisAlignment="End", only end padding applies -->
<utu:AutoLayout Padding="20,10,20,10" PrimaryAxisAlignment="End">
    <Border /> <!-- Gets right padding (20px) in horizontal, bottom padding (10px) in vertical -->
</utu:AutoLayout>
```

This differs from CSS Flexbox where padding always applies to all sides regardless of content alignment.

## Common Scenarios

### Scenario 1: Horizontal Navigation Bar

**CSS Flexbox:**
```css
.navbar {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 10px 20px;
}
```

**AutoLayout:**
```xml
<utu:AutoLayout Orientation="Horizontal" 
                Justify="SpaceBetween" 
                CounterAxisAlignment="Center"
                Padding="20,10,20,10">
    <TextBlock Text="Logo" />
    <StackPanel Orientation="Horizontal">
        <Button Content="Home" />
        <Button Content="About" />
        <Button Content="Contact" />
    </StackPanel>
</utu:AutoLayout>
```

### Scenario 2: Card with Stretched Content

**CSS Flexbox:**
```css
.card {
    display: flex;
    flex-direction: column;
    height: 300px;
}
.card-content {
    flex-grow: 1;
}
```

**AutoLayout:**
```xml
<utu:AutoLayout Orientation="Vertical" Height="300">
    <TextBlock Text="Card Title" />
    <Border utu:AutoLayout.PrimaryAlignment="Stretch">
        <!-- Content that fills remaining space -->
    </Border>
    <Button Content="Action" />
</utu:AutoLayout>
```

### Scenario 3: Centered Content

**CSS Flexbox:**
```css
.container {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100vh;
}
```

**AutoLayout:**
```xml
<utu:AutoLayout Orientation="Vertical"
                PrimaryAxisAlignment="Center"
                CounterAxisAlignment="Center"
                Height="400">
    <TextBlock Text="Centered Content" />
</utu:AutoLayout>
```

### Scenario 4: Equal Width Items

**CSS Flexbox:**
```css
.container {
    display: flex;
}
.item {
    flex: 1;
}
```

**AutoLayout:**
```xml
<utu:AutoLayout Orientation="Horizontal">
    <Border utu:AutoLayout.PrimaryAlignment="Stretch" />
    <Border utu:AutoLayout.PrimaryAlignment="Stretch" />
    <Border utu:AutoLayout.PrimaryAlignment="Stretch" />
</utu:AutoLayout>
```

### Scenario 5: Fixed + Flexible Layout

**CSS Flexbox:**
```css
.container {
    display: flex;
}
.sidebar {
    width: 200px;
}
.content {
    flex: 1;
}
```

**AutoLayout:**
```xml
<utu:AutoLayout Orientation="Horizontal">
    <Border Width="200" /> <!-- Fixed sidebar -->
    <Border utu:AutoLayout.PrimaryAlignment="Stretch" /> <!-- Flexible content -->
</utu:AutoLayout>
```

## Feature Comparison Summary

| Feature | CSS Flexbox | AutoLayout | Status |
|---------|-------------|------------|--------|
| Single-axis layout | ✅ | ✅ | Full support |
| Direction (row/column) | ✅ | ✅ | Full support |
| Reverse direction | ✅ | ⚠️ | Partial (Z-order only) |
| Gap/Spacing | ✅ | ✅ | Full support |
| Main axis alignment | ✅ | ✅ | Full support |
| Cross axis alignment | ✅ | ✅ | Full support (no baseline) |
| Wrapping | ✅ | ❌ | Not supported |
| Flex grow (stretch) | ✅ | ✅ | Equal distribution only |
| Flex shrink | ✅ | ⚠️ | Via Min constraints |
| Proportional flex-grow | ✅ | ❌ | Not supported |
| Order property | ✅ | ❌ | Not supported |
| Baseline alignment | ✅ | ❌ | Not supported |
| Absolute positioning | ✅ | ✅ | Via IsIndependentLayout |
| space-around | ✅ | ❌ | Not supported |
| space-evenly | ✅ | ❌ | Not supported |
| align-content | ✅ | ❌ | N/A (no wrapping) |

## Key Differences Summary

1. **No Wrapping**: AutoLayout does not support `flex-wrap`. Items always remain in a single row or column.

2. **Equal Distribution Only**: Unlike CSS Flexbox's proportional `flex-grow` values, AutoLayout distributes remaining space equally among all stretched children.

3. **No Baseline Alignment**: AutoLayout doesn't support aligning items by their text baseline.

4. **Limited justify-content Options**: AutoLayout supports `Stack` (start) and `SpaceBetween`, but not `space-around` or `space-evenly`.

5. **Figma-aligned Padding**: AutoLayout's padding behavior follows Figma's model, where applicable padding depends on alignment settings.

6. **IsReverseZIndex vs Direction Reverse**: The `IsReverseZIndex` property controls stacking order (z-index), not visual layout order like CSS `row-reverse`/`column-reverse`.

7. **Independent Layout**: AutoLayout provides `IsIndependentLayout` for absolute positioning, similar to CSS `position: absolute` but designed to work within the AutoLayout context (matching Figma's behavior).

## When to Use AutoLayout vs Other Controls

**Use AutoLayout when:**
- Building layouts that originated from Figma designs
- You need simple row or column layouts with consistent spacing
- You want Figma AutoLayout parity in your XAML
- Working with designs that use Figma's alignment and spacing paradigms

**Consider alternatives when:**
- You need wrapping behavior (use `WrapPanel` or `ItemsWrapGrid`)
- You need proportional flex-grow (use `Grid` with star sizing)
- You need complex multi-line layouts (use `Grid` or nested layouts)
- You need baseline text alignment (use `Grid` or custom panels)

## See Also

- [AutoLayout Control Documentation](AutoLayoutControl.md)
- [Figma AutoLayout Documentation](https://www.figma.com/widget-docs/api/component-AutoLayout)
- [CSS Flexbox Guide](https://css-tricks.com/snippets/css/a-guide-to-flexbox/)
- [.NET MAUI FlexLayout](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/layouts/flexlayout) (CSS Flexbox-based alternative)
