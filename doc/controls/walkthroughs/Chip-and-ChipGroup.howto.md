---
uid: Toolkit.Controls.Chip.HowTo
tags: [chip, chipgroup, material, filter, selection, toggle, removable, badge]
---

# Display chips with Material styles

> Dependencies (for Material-styled chips):
>
> * NuGet: `Uno.Toolkit.WinUI` (controls), `Uno.Toolkit.WinUI.Material` (Material styles)
> * Add `<MaterialToolkitTheme .../>` to `App.xaml` resources. ([Uno Platform][1])

---

## Show chips with Material styles

```xml
xmlns:utu="using:Uno.Toolkit.UI"

<StackPanel Orientation="Horizontal" Spacing="8">
    <utu:Chip Content="Assist"     Style="{StaticResource AssistChipStyle}" />
    <utu:Chip Content="Input"      IsChecked="True" Style="{StaticResource InputChipStyle}" />
    <utu:Chip Content="Filter"     IsChecked="True" Style="{StaticResource FilterChipStyle}" />
    <utu:Chip Content="Suggestion" IsChecked="True" Style="{StaticResource SuggestionChipStyle}" />
</StackPanel>
```

Chips are toggleable; the control derives from `ToggleButton`. ([Uno Platform][2])

---

## Add an icon or avatar to a chip

```xml
<utu:Chip Content="Profile" Style="{StaticResource MaterialChipStyle}">
    <utu:Chip.Icon>
        <Image Source="ms-appx:///Assets/Avatar.png" />
    </utu:Chip.Icon>
</utu:Chip>
```

Use `Chip.Icon` (optionally templated via `IconTemplate`) to show images or glyphs. ([Uno Platform][2])

---

## Make a chip removable (shows an "x" button)

```xml
<utu:Chip Content="Mail" CanRemove="True"
          Removed="Chip_Removed" Removing="Chip_Removing" />
```

* `CanRemove` shows the remove button.
* Handle `Removing` to cancel, and `Removed` after it happens.

> Note: outside a `ChipGroup`, `Removed` doesn't remove the chip from the UI by itself. ([Uno Platform][2])

---

## Run logic when a chip is removed (with command)

```xml
<utu:Chip Content="Tag"
          CanRemove="True"
          RemovedCommand="{Binding RemoveTagCommand}"
          RemovedCommandParameter="{Binding}" />
```

Use `RemovedCommand`(+`RemovedCommandParameter`) for MVVM-friendly removal handling. ([Uno Platform][2])

---

## Show a single-select set (radio-like) with ChipGroup

```xml
<utu:ChipGroup SelectionMode="Single"
               SelectedItem="{Binding SelectedCategory, Mode=TwoWay}"
               Style="{StaticResource SuggestionChipGroupStyle}">
    <utu:Chip Content="News" />
    <utu:Chip Content="Sports" />
    <utu:Chip Content="Tech" />
</utu:ChipGroup>
```

`ChipGroup` manages selection and exposes `SelectedItem`. Use `SelectionMode="Single"` for exactly one selected. ([Uno Platform][2])

---

## Allow "none selected" in a single-select set

```xml
<utu:ChipGroup SelectionMode="SingleOrNone"
               SelectedItem="{Binding SelectedFilter, Mode=TwoWay}" />
```

`SingleOrNone` lets users clear the current choice. ([Uno Platform][2])

---

## Show a multi-select set (checkbox-like)

```xml
<utu:ChipGroup SelectionMode="Multiple"
               SelectedItems="{Binding SelectedTags, Mode=TwoWay}"
               Style="{StaticResource FilterChipGroupStyle}">
    <utu:Chip Content="C#" />
    <utu:Chip Content="XAML" />
    <utu:Chip Content=".NET" />
</utu:ChipGroup>
```

Use `SelectedItems` for multiple selections. (Empty selection can yield `null`.) ([Uno Platform][2])

---

## Bind chips to a collection

```xml
<utu:ChipGroup ItemsSource="{Binding People}"
               Style="{StaticResource SuggestionChipGroupStyle}">
    <utu:ChipGroup.ItemTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Name}" />
        </DataTemplate>
    </utu:ChipGroup.ItemTemplate>
</utu:ChipGroup>
```

`ChipGroup` is an `ItemsControl`; bind `ItemsSource` and set an `ItemTemplate`. ([Uno Platform][2])

---

## Bind selection to a bool on each item

```xml
<utu:ChipGroup ItemsSource="{Binding Filters}"
               SelectionMemberPath="IsOn"
               Style="{StaticResource FilterChipGroupStyle}" />
```

`SelectionMemberPath` binds each chip's `IsChecked` to a property (e.g., `IsOn`) on your item. ([Uno Platform][2])

---

## React to user actions (click / check / uncheck)

```xml
<utu:ChipGroup ItemClick="OnItemClick"
               ItemChecked="OnItemChecked"
               ItemUnchecked="OnItemUnchecked">
    <utu:Chip Content="Alpha" />
    <utu:Chip Content="Beta" />
</utu:ChipGroup>
```

`ChipGroup` forwards chip events: `ItemClick`, `ItemChecked`, `ItemUnchecked`, plus removal events. ([Uno Platform][2])

---

## Remove chips from a bound list automatically

```xml
<utu:ChipGroup ItemsSource="{Binding Tags}"
               Style="{StaticResource InputChipGroupStyle}">
    <!-- Make each chip removable -->
    <utu:ChipGroup.ItemTemplate>
        <DataTemplate>
            <utu:Chip Content="{Binding}" CanRemove="True" />
        </DataTemplate>
    </utu:ChipGroup.ItemTemplate>
</utu:ChipGroup>
```

In a `ChipGroup`, pressing a chip's remove button raises `ItemRemoving`/`ItemRemoved`; handle/cancel if needed. Use an `ObservableCollection<T>` for `ItemsSource` so UI updates when items are removed. ([Uno Platform][2])

---

## Use a single standalone filter chip

```xml
<utu:Chip Content="On sale" IsChecked="{Binding IsOnSale, Mode=TwoWay}"
          Style="{StaticResource FilterChipStyle}" />
```

A lone `Chip` behaves like a toggle; bind `IsChecked` directly. ([Uno Platform][2])

---

## Tweak size/elevation with lightweight tokens

Override keys to adjust visuals globally:

```xml
<ResourceDictionary>
    <x:Double x:Key="ChipHeight">28</x:Double>
    <x:Double x:Key="ChipElevation">2</x:Double>
    <CornerRadius x:Key="ChipCornerRadius">12</CornerRadius>
</ResourceDictionary>
```

Common tokens include `ChipHeight`, `ChipElevation`, `ChipCornerRadius`, `ChipPadding`, and brushes like `ChipBackgroundChecked`. ([Uno Platform][2])

---

## References

* Chip & ChipGroup API, properties, events, and usage. ([Uno Platform][2])
* Material Toolkit setup (packages, theme resource). ([Uno Platform][1])

[1]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/material-getting-started.html "Uno Material Toolkit Library "
[2]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/controls/ChipAndChipGroup.html "Chip & ChipGroup "
