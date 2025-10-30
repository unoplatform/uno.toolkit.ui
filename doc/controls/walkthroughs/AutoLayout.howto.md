---
uid: Toolkit.Controls.AutoLayout.HowTo
tags: [autolayout, figma, layout, flexbox, stack, spacing, alignment, figma-plugin]
---

# Layout content in one row or one column and mirror Figma's Auto-Layout behavior (direction, spacing, packing, alignment, padding, per-child overrides)

---

## Add AutoLayout to a page

**Why:** Use a single container to stack and align children like Figma Auto-Layout.

**Dependencies**

* NuGet: `Uno.Toolkit.UI` (XAML prefix `utu`) ([Uno Platform][1])

```xml
<Page
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:utu="using:Uno.Toolkit.UI">

    <utu:AutoLayout>
        <!-- children -->
    </utu:AutoLayout>
</Page>
```

---

## Stack items horizontally with fixed gaps

**Outcome:** Left-to-right row with uniform spacing.

```xml
<utu:AutoLayout Orientation="Horizontal" Spacing="12">
    <Button Content="One"/>
    <Button Content="Two"/>
    <Button Content="Three"/>
</utu:AutoLayout>
```

* `Orientation` chooses row vs column.
* `Spacing` is pixels along the orientation axis. ([Uno Platform][1])

---

## Stack items vertically with "space between"

**Outcome:** First and last items pin to edges; remaining space spreads between them.

```xml
<utu:AutoLayout Orientation="Vertical"
                Justify="SpaceBetween"
                Height="240">
    <TextBlock Text="Top"/>
    <TextBlock Text="Middle"/>
    <TextBlock Text="Bottom"/>
</utu:AutoLayout>
```

* `Justify`: `Stack` (packed) or `SpaceBetween`. With `SpaceBetween`, items stretch apart along the primary axis.
* If a child sets `PrimaryAlignment="Stretch"`, it behaves as `Stack`. ([Uno Platform][1])

---

## Center children as a group

**Outcome:** Pack items at the center of the primary axis.

```xml
<utu:AutoLayout Orientation="Horizontal"
                PrimaryAxisAlignment="Center"
                Spacing="8"
                Width="300">
    <Ellipse Width="24" Height="24"/>
    <TextBlock Text="Centered group"/>
</utu:AutoLayout>
```

* `PrimaryAxisAlignment`: `Start|Center|End|Stretch`.
* Applies along the orientation axis. ([Uno Platform][1])

---

## Center children cross-axis and stretch to container width/height

**Outcome:** Controls align and size on the counter axis.

```xml
<utu:AutoLayout Orientation="Vertical"
                CounterAxisAlignment="Stretch"
                Padding="16,24">
    <TextBlock Text="Title"/>
    <TextBlock Text="Subtitle" />
</utu:AutoLayout>
```

* `CounterAxisAlignment` targets the axis **opposite** `Orientation` and can `Start|Center|End|Stretch`.
* Padding follows Figma-style anchoring (see next recipe). ([Uno Platform][1])

---

## Use padding like Figma (anchored sides only)

**Outcome:** Apply per-side padding that respects the group's anchors.

```xml
<utu:AutoLayout Orientation="Horizontal"
                PrimaryAxisAlignment="End"
                CounterAxisAlignment="Start"
                Padding="24,12,8,4">
    <!-- Only Right + Top padding apply given these anchors -->
    <TextBlock Text="Anchored like Figma"/>
</utu:AutoLayout>
```

* Padding behaves like the Figma plugin: only the anchored sides are honored (e.g., Right + Top). ([Uno Platform][1])

---

## Make one child fill remaining space (primary axis)

**Outcome:** A specific child expands along the primary axis.

```xml
<utu:AutoLayout Orientation="Horizontal" Spacing="8">
    <TextBlock Text="Label"/>
    <TextBox Text="Grows" utu:AutoLayout.PrimaryAlignment="Stretch"/>
    <Button Content="Action"/>
</utu:AutoLayout>
```

* Per-child `utu:AutoLayout.PrimaryAlignment="Stretch"` makes that child fill on the primary axis. ([Uno Platform][1])

---

## Give a child an explicit size on the primary axis

**Outcome:** Fix width (row) or height (column) of a child without touching `Width`/`Height`.

```xml
<utu:AutoLayout Orientation="Horizontal" Spacing="8">
    <Button Content="Fixed 120"
            utu:AutoLayout.PrimaryLength="120"/>
    <Button Content="Auto"/>
</utu:AutoLayout>
```

* `PrimaryLength` sets size along the primary axis (overridden by `Width`/`Height` if set). ([Uno Platform][1])

---

## Give a child an explicit size on the counter axis

**Outcome:** Fix height (row) or width (column) of a child on the **other** axis.

```xml
<utu:AutoLayout Orientation="Horizontal" Spacing="8"
                CounterAxisAlignment="Center">
    <Border Background="LightGray"
            utu:AutoLayout.CounterLength="48">
        <TextBlock Text="48 high in a row"/>
    </Border>
</utu:AutoLayout>
```

* `CounterLength` sizes along the opposite axis (overridden by `Width`/`Height` if set). ([Uno Platform][1])

---

## Align one child differently from the group (counter axis)

**Outcome:** Override cross-axis alignment per child.

```xml
<utu:AutoLayout Orientation="Horizontal" CounterAxisAlignment="Center" Spacing="8">
    <Border Width="64" Height="64" Background="LightGray"/>
    <Border Width="64" Height="64" Background="LightGray"
            utu:AutoLayout.CounterAlignment="End"/>
    <Border Width="64" Height="64" Background="LightGray"/>
</utu:AutoLayout>
```

* `CounterAlignment` (`Start|Center|End|Stretch`) overrides the container's `CounterAxisAlignment` for that child. ([Uno Platform][1])

---

## Absolutely position a child inside the layout

**Outcome:** Place a child independent of the stacking flow (like Figma "Absolute position").

```xml
<Grid>
    <utu:AutoLayout Orientation="Vertical" Spacing="12">
        <Rectangle Height="120" Fill="LightGray"/>
        <Rectangle Height="120" Fill="Gainsboro"/>
    </utu:AutoLayout>

    <Border Background="#AAFFCC00"
            utu:AutoLayout.IsIndependentLayout="True"
            HorizontalAlignment="Right"
            VerticalAlignment="Top"
            Margin="12">
        <TextBlock Text="Badge"/>
    </Border>
</Grid>
```

* `IsIndependentLayout="True"` detaches the element from AutoLayout positioning; position it with normal alignments. (Avoid mixing with other attached props.) ([Uno Platform][1])

---

## Control z-overlap order of children

**Outcome:** Flip which stacked child appears on top.

```xml
<utu:AutoLayout Orientation="Vertical" IsReverseZIndex="True">
    <!-- Later children render below earlier ones -->
    <Border Height="40" Background="#330000FF"/>
    <Border Height="40" Background="#3300FF00"/>
</utu:AutoLayout>
```

* `IsReverseZIndex` reverses the children's Z-ordering; does **not** change layout order. ([Uno Platform][1])

---

## Quick reference: key properties & attachments

* **Container:** `Orientation`, `Spacing`, `Justify` (`Stack|SpaceBetween`),
  `PrimaryAxisAlignment` (`Start|Center|End|Stretch`),
  `CounterAxisAlignment` (`Start|Center|End|Stretch`),
  `Padding` (Figma-anchored), `IsReverseZIndex`.
* **Per-child (attached):** `PrimaryAlignment` (`Auto|Stretch`),
  `CounterAlignment` (`Start|Center|End|Stretch`),
  `PrimaryLength`, `CounterLength`, `IsIndependentLayout`. ([Uno Platform][1])

---

## Notes & gotchas

* **Spacing vs margins:** AutoLayout's spacing is along the primary axis; margins may interact differently than Grid/StackPanel in some edge cases. Test when porting complex margins. ([GitHub][2])
* **Figma parity:** AutoLayout is designed to match Figma Auto-Layout behavior used by the Uno Figma plugin. ([Uno Platform][1])

---

[1]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/controls/AutoLayoutControl.html "AutoLayout "
[2]: https://github.com/unoplatform/uno.toolkit.ui/issues/1279?utm_source=chatgpt.com "[AutoLayout] Child Controls with Margins Arrange ..."
