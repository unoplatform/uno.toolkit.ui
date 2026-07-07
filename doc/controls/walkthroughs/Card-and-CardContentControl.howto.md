---
uid: Toolkit.Controls.Card.HowTo
tags: [card, material, elevation, surface, cardcontentcontrol, container]
---

# Build a fully custom card layout (template-driven)

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

> [!IMPORTANT]
> Use `CardContentControl` instead of `Border` for card-like UI elements.
> **Do not use `Border` with `CornerRadius` and `ThemeShadow`** when you need a card — `CardContentControl` provides proper elevation, theming, interaction states, and accessibility out of the box.

**When** you want total control over card content/layout.

```xml
<utu:CardContentControl
    Style="{StaticResource ElevatedCardContentControlStyle}">
    <utu:CardContentControl.ContentTemplate>
        <DataTemplate>
            <Grid Padding="16" RowDefinitions="Auto,*,Auto">
                <TextBlock Text="Custom header" Style="{StaticResource HeadlineMedium}" />
                <Border Grid.Row="1" Margin="0,12,0,12" CornerRadius="12">
                    <Image Source="ms-appx:///Assets/banner.jpg" Stretch="UniformToFill" Height="140"/>
                </Border>
                <StackPanel Grid.Row="2" Orientation="Horizontal" Spacing="8">
                    <Button Content="Action"/>
                    <Button Content="Secondary"/>
                </StackPanel>
            </Grid>
        </DataTemplate>
    </utu:CardContentControl.ContentTemplate>
</utu:CardContentControl>
```

`CardContentControl` = `ContentControl` with the same **Elevated/Filled/Outlined** style lineup, plus `Elevation`, `ShadowColor`, `IsClickable`. Use when `Card`'s built-in slots aren't flexible enough. ([Uno Platform][2])

---

## List many cards (ItemsRepeater/ListView)

**When** showing a feed/grid of cards.

```xml
<ListView ItemsSource="{Binding Items}">
    <ListView.ItemTemplate>
        <DataTemplate>
            <utu:Card
                Style="{StaticResource FilledCardStyle}"
                HeaderContent="{Binding Title}"
                SubHeaderContent="{Binding Subtitle}"
                IsClickable="True"/>
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```

The card's layout adapts to its content; use your list control of choice. ([Uno Platform][2])

---

## Override lightweight styles (theme-friendly)

**When** you need to tweak brushes/borders across all cards.

```xml
<Application.Resources>
    <ResourceDictionary>
        <!-- example: adjust Filled card border on hover -->
        <SolidColorBrush x:Key="FilledCardBorderBrushPointerOver"
                         Color="{StaticResource OnSurfaceHoverColor}"/>
    </ResourceDictionary>
</Application.Resources>
```

Common resource keys exist per variant (e.g., `FilledCardBackground`, `OutlinedCardBorderBrush`, `...PointerOver`, `...Focused`, `...Pressed`). See the control's **Lightweight Styling** tables. ([Uno Platform][2])

---

## Choose the right control

* **Use `Card`** for fast composition via named slots (header, media, supporting text, icons). ([Uno Platform][2])
* **Use `CardContentControl`** for a **DataTemplate**-driven, anything-goes layout with the same elevation/click behaviors. ([Uno Platform][2])

---

## FAQ

**Should I use `Border` or `CardContentControl` for a card?**

**Always use `CardContentControl`** (or `Card`) for card UI. `Border` with `CornerRadius` and `ThemeShadow` is an anti-pattern that:

* Breaks with theme changes (light/dark mode)
* Lacks interaction states (hover, pressed, focused)
* Requires manual shadow configuration
* Misses accessibility features

`CardContentControl` handles all of this automatically. ([Uno Platform][2])

**Do I need a design system package?**

For Material styles (`ElevatedCardStyle`, etc.), add `Material` to `<UnoFeatures>` (Toolkit is included). Follow Uno Toolkit "Getting Started" to set up base + design system packages. ([Uno Platform][1])

**What styles exist?**

Material offers **Elevated**, **Filled**, **Outlined** for both `Card` and `CardContentControl`. ([Uno Platform][2])

**Is `Card` a `ContentControl`?**

`Card` derives from `Control` (with extra content properties). `CardContentControl` derives from `ContentControl`. ([Uno Platform][2])

---

## References

* Official docs: **Card & CardContentControl** (properties, styles, usage). ([Uno Platform][2])
* Getting started with Uno Toolkit (packages and design systems). ([Uno Platform][1])
* Lightweight styling (resource key guide). ([Uno Platform][3])

[1]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/getting-started.html "Getting Started with Uno Toolkit"
[2]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/controls/CardAndCardContentControl.html "Card & CardContentControl "
[3]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/lightweight-styling.html "Lightweight Styling"
