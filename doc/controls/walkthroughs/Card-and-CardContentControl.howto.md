---
uid: Toolkit.Controls.Card.HowTo
tags: [card, material, elevation, surface, cardcontentcontrol, container]
---

# Build a fully custom card layout (template-driven)

> Controls covered: `CardContentControl`.

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

**Do I need a design system package?**

For Material styles (`ElevatedCardStyle`, etc.), install `Uno.Toolkit.UI.Material`. Follow Uno Toolkit "Getting Started" to set up base + design system packages. ([Uno Platform][1])

**What styles exist?**

Material offers **Elevated**, **Filled**, **Outlined** for both `Card` and `CardContentControl`. ([Uno Platform][2])

**Is `Card` a `ContentControl`?**

`Card` derives from `Control` (with extra content properties). `CardContentControl` derives from `ContentControl`. ([Uno Platform][2])

---

## NuGet summary (add what you use)

* `Uno.Toolkit.UI` – base controls & helpers. ([Uno Platform][1])
* `Uno.Toolkit.UI.Material` – Material v2/MD3 styles including Card/CardContentControl styles. ([Uno Platform][3])

---

## References

* Official docs: **Card & CardContentControl** (properties, styles, usage). ([Uno Platform][2])
* Getting started with Uno Toolkit (packages and design systems). ([Uno Platform][1])
* Lightweight styling (resource key guide). ([Uno Platform][4])
* NuGet: `Uno.Toolkit.UI`. ([NuGet][5])

[1]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/getting-started.html?utm_source=chatgpt.com "Getting Started with Uno Toolkit"
[2]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/controls/CardAndCardContentControl.html "Card & CardContentControl "
[3]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/material-migration.html?utm_source=chatgpt.com "Upgrading Material Toolkit Version"
[4]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/lightweight-styling.html?utm_source=chatgpt.com "Lightweight Styling"
[5]: https://www.nuget.org/packages/Uno.Toolkit.UI/?utm_source=chatgpt.com "Uno.Toolkit.UI 8.2.4"
