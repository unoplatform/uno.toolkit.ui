---
uid: Toolkit.Controls.ShadowContainer.HowTo
tags: [shadow, drop-shadow, inner-shadow, elevation, box-shadow, skia]
---

# Display custom shadows around content

> NuGet needed for all how-tos: **Uno.Toolkit.Skia.WinUI**. ([Uno Platform][1])
>
> What it does: adds one or many shadows around your content; it mimics the content's size/corner radius (not complex alpha shapes yet). ([Uno Platform][1])

## Add a soft drop shadow to a button

```xml
<Page
  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
  xmlns:utu="using:Uno.Toolkit.UI">
  <utu:ShadowContainer>
    <utu:ShadowContainer.Shadows>
      <utu:ShadowCollection>
        <utu:Shadow BlurRadius="15" OffsetY="8" Opacity="0.5" Color="#7a67f8"/>
      </utu:ShadowCollection>
    </utu:ShadowContainer.Shadows>

    <Button Content="Add Shadow" Foreground="White" Background="#7a67f8"/>
  </utu:ShadowContainer>
</Page>
```

Use `BlurRadius`, `OffsetX|OffsetY`, `Opacity`, `Color`. ([Uno Platform][1])

---

## Reuse the same shadow across many controls

```xml
<Page.Resources>
  <utu:ShadowCollection x:Key="ButtonShadows">
    <utu:Shadow BlurRadius="15" OffsetY="8" Opacity="0.5" Color="#7a67f8"/>
  </utu:ShadowCollection>
</Page.Resources>

<StackPanel Orientation="Horizontal" Spacing="16">
  <utu:ShadowContainer Shadows="{StaticResource ButtonShadows}">
    <Button Content="One"/>
  </utu:ShadowContainer>
  <utu:ShadowContainer Shadows="{StaticResource ButtonShadows}">
    <Button Content="Two"/>
  </utu:ShadowContainer>
</StackPanel>
```

`Shadows` accepts a `ShadowCollection`; you can store it in resources for consistency. ([Uno Platform][1])

---

## Layer two colored shadows (dual-tone depth)

```xml
<utu:ShadowContainer>
  <utu:ShadowContainer.Shadows>
    <utu:ShadowCollection>
      <utu:Shadow BlurRadius="20" OffsetX="10"  OffsetY="10"  Opacity="0.5" Spread="-5" Color="#7a67f8"/>
      <utu:Shadow BlurRadius="20" OffsetX="-10" OffsetY="-10" Opacity="0.5" Spread="-5" Color="#f85977"/>
    </utu:ShadowCollection>
  </utu:ShadowContainer.Shadows>

  <StackPanel Width="300" Padding="16" CornerRadius="20" BorderThickness="1">
    <TextBlock Text="Add many shadows"/>
  </StackPanel>
</utu:ShadowContainer>
```

Any number of `<utu:Shadow/>` entries can be stacked for richer effects. ([Uno Platform][1])

---

## Control which shadow sits on top (order matters)

```xml
<Page.Resources>
  <utu:ShadowCollection x:Key="InnerShadows">
    <!-- first -->
    <utu:Shadow BlurRadius="30" OffsetX="-50" OffsetY="-50" Opacity="1" Spread="-5" Color="Green" IsInner="True"/>
    <!-- last → appears on top -->
    <utu:Shadow BlurRadius="30" OffsetX="50"  OffsetY="50"  Opacity="1" Spread="-5" Color="Red"   IsInner="True"/>
  </utu:ShadowCollection>
</Page.Resources>

<utu:ShadowContainer Background="LightGray" Shadows="{StaticResource InnerShadows}">
  <Grid Width="100" Height="100" CornerRadius="50"/>
</utu:ShadowContainer>
```

Later shadows can overlap earlier ones; reorder to change the final look. ([Uno Platform][1])

---

## Add an **inner** shadow (inset)

```xml
<utu:ShadowContainer>
  <utu:ShadowContainer.Shadows>
    <utu:ShadowCollection>
      <utu:Shadow IsInner="True" BlurRadius="10" OffsetX="5" OffsetY="5" Opacity="1" Color="#000000"/>
    </utu:ShadowCollection>
  </utu:ShadowContainer.Shadows>

  <Grid Width="120" Height="60" CornerRadius="12"/>
</utu:ShadowContainer>
```

`IsInner="True"` renders an inset (CSS-like `box-shadow: inset`). ([Uno Platform][1])

---

## Prevent inner shadow from being hidden by child backgrounds

**Do this (put background on the container, not the child):**

```xml
<utu:ShadowContainer Background="#222" Shadows="{StaticResource MyInnerShadows}">
  <Grid/> <!-- no Background here -->
</utu:ShadowContainer>
```

**Don't do this:**

```xml
<utu:ShadowContainer Shadows="{StaticResource MyInnerShadows}">
  <Grid Background="#222"/> <!-- hides inner shadow -->
</utu:ShadowContainer>
```

Inner shadows paint on top only if the child has no `Background/Fill` (or it's transparent). Set the color on the **ShadowContainer**. ([Uno Platform][1])

---

## Know the limits (shape mimic)

The container follows the child's **size** and **corner radius**. Complex alpha silhouettes (e.g., text, detailed PNG cutouts) aren't supported yet. ([Uno Platform][1])

---

## Shadow property reference (quick)

* `IsInner` (bool) — inset vs drop.
* `OffsetX|OffsetY` (double) — shadow position.
* `Color` + `Opacity` — final color = color × opacity.
* `BlurRadius` (0..100) — softness.
* `Spread` — inflate/deflate before blur (negative = smaller).

Use a `ShadowCollection` in `ShadowContainer.Shadows`. Don't confuse with the unrelated `UIElement.Shadow` property. ([Uno Platform][1])

---

### FAQ

**How do I keep inner shadows visible over backgrounds?**

Put the background on the **ShadowContainer**, not on the child. ([Uno Platform][1])

**Can I define shadows once and reuse?**

Yes—create a `ShadowCollection` in resources and bind via `Shadows="{StaticResource ...}"`. ([Uno Platform][1])

**Does order affect layered shadows?**

Yes—later entries render over earlier ones; reorder for the look you want. ([Uno Platform][1])

---

[1]: https://platform.uno/
