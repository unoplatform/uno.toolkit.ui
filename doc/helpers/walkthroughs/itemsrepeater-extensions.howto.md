# ItemsRepeater selection & incremental loading how-tos

## 1. Select one item in an ItemsRepeater

**Goal:** user taps an item → item becomes selected → viewmodel is updated.

**Dependencies:**

* `Uno.Toolkit.UI` (the package that brings `ItemsRepeaterExtensions`)

**XAML**

```xml
<Page
    x:Class="MyApp.Views.PeoplePage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:muxc="using:Microsoft.UI.Xaml.Controls"
    xmlns:utu="using:Uno.Toolkit.UI">

    <muxc:ItemsRepeater
        ItemsSource="{Binding People}"
        utu:ItemsRepeaterExtensions.SelectionMode="Single"
        utu:ItemsRepeaterExtensions.SelectedItem="{Binding SelectedPerson, Mode=TwoWay}">
        <muxc:ItemsRepeater.ItemTemplate>
            <DataTemplate>
                <!-- Any SelectorItem or ToggleButton root works -->
                <ListViewItem Content="{Binding Name}" />
            </DataTemplate>
        </muxc:ItemsRepeater.ItemTemplate>
    </muxc:ItemsRepeater>
</Page>
```

**What matters**

* `SelectionMode="Single"` turns on selection. Without it, selection attached properties do nothing. ([Uno Platform][1])
* `SelectedItem` is two-way, so your VM gets the current item.
* Use a selectable root (`ListViewItem`, `SelectorItem`, `ToggleButton`, or a template that behaves like one) — the extension was built to work with that. ([Uno Platform][2])

---

## 2. Allow “tap again to unselect” (single or none)

**Goal:** user can select **or** go back to “nothing selected”.

**XAML**

```xml
<muxc:ItemsRepeater
    ItemsSource="{Binding Filters}"
    xmlns:muxc="using:Microsoft.UI.Xaml.Controls"
    xmlns:utu="using:Uno.Toolkit.UI"
    utu:ItemsRepeaterExtensions.SelectionMode="SingleOrNone"
    utu:ItemsRepeaterExtensions.SelectedItem="{Binding ActiveFilter, Mode=TwoWay}">
    <muxc:ItemsRepeater.ItemTemplate>
        <DataTemplate>
            <ListViewItem Content="{Binding Label}" />
        </DataTemplate>
    </muxc:ItemsRepeater.ItemTemplate>
</muxc:ItemsRepeater>
```

**Notes**

* `SingleOrNone` guarantees either **0 or 1** selected items. ([Uno Platform][1])
* When the user deselects, the bound `ActiveFilter` becomes `null`.

---

## 3. Let users pick many items

**Goal:** user can select multiple things and you get the list.

**XAML**

```xml
<muxc:ItemsRepeater
    ItemsSource="{Binding Tags}"
    xmlns:muxc="using:Microsoft.UI.Xaml.Controls"
    xmlns:utu="using:Uno.Toolkit.UI"
    utu:ItemsRepeaterExtensions.SelectionMode="Multiple"
    utu:ItemsRepeaterExtensions.SelectedItems="{Binding SelectedTags, Mode=TwoWay}">
    <muxc:ItemsRepeater.ItemTemplate>
        <DataTemplate>
            <!-- ToggleButton works great for multiple -->
            <ToggleButton Content="{Binding Name}" />
        </DataTemplate>
    </muxc:ItemsRepeater.ItemTemplate>
</muxc:ItemsRepeater>
```

**What you get**

* `SelectedItems` is an `IList<object>` mirrored to your VM. ([Uno Platform][1])
* In Multiple mode, the first selected item is also exposed through `SelectedItem` (if you bind it), but prefer the list.

---

## 4. Bind by index instead of item

**Goal:** you only care about the index (maybe you virtualize items or you map to an enum).

**XAML**

```xml
<muxc:ItemsRepeater
    ItemsSource="{Binding Options}"
    xmlns:muxc="using:Microsoft.UI.Xaml.Controls"
    xmlns:utu="using:Uno.Toolkit.UI"
    utu:ItemsRepeaterExtensions.SelectionMode="Single"
    utu:ItemsRepeaterExtensions.SelectedIndex="{Binding SelectedOptionIndex, Mode=TwoWay}">
    <muxc:ItemsRepeater.ItemTemplate>
        <DataTemplate>
            <ListViewItem Content="{Binding}" />
        </DataTemplate>
    </muxc:ItemsRepeater.ItemTemplate>
</muxc:ItemsRepeater>
```

**Details**

* `SelectedIndex` is two-way.
* Works only when `SelectionMode != None`. If you forget the mode, the extension re-coerces selection and your index won’t stick. ([Uno Platform][1])

---

## 5. Get several indexes at once

**Goal:** multiple selection, but you want integers, not objects.

**XAML**

```xml
<muxc:ItemsRepeater
    ItemsSource="{Binding Files}"
    xmlns:muxc="using:Microsoft.UI.Xaml.Controls"
    xmlns:utu="using:Uno.Toolkit.UI"
    utu:ItemsRepeaterExtensions.SelectionMode="Multiple"
    utu:ItemsRepeaterExtensions.SelectedIndexes="{Binding SelectedFileIndexes, Mode=TwoWay}">
    <muxc:ItemsRepeater.ItemTemplate>
        <DataTemplate>
            <ToggleButton Content="{Binding Name}" />
        </DataTemplate>
    </muxc:ItemsRepeater.ItemTemplate>
</muxc:ItemsRepeater>
```

**Why**

* `SelectedIndexes` → `IList<int>` of current selections.
* Best when your VM already has the item list and just needs to know “which ones”.
* As with the other `Selected-*` properties, changing the selection mode re-applies the selection. ([Uno Platform][1])

---

## 6. Turn on incremental loading

**Goal:** ItemsRepeater shows more data as the user scrolls.

**Requirements**

1. Your `ItemsSource` **must** implement `ISupportIncrementalLoading`.
2. You must tell the extension to use it.

**XAML**

```xml
<muxc:ItemsRepeater
    ItemsSource="{Binding PaginatedItems}"
    xmlns:muxc="using:Microsoft.UI.Xaml.Controls"
    xmlns:utu="using:Uno.Toolkit.UI"
    utu:ItemsRepeaterExtensions.SupportsIncrementalLoading="True">
    <muxc:ItemsRepeater.ItemTemplate>
        <DataTemplate>
            <ListViewItem Content="{Binding Title}" />
        </DataTemplate>
    </muxc:ItemsRepeater.ItemTemplate>
</muxc:ItemsRepeater>
```

**How it works**

* When the repeater reaches the end, it calls into your source to load more.
* The docs explicitly say incremental loading only kicks in when the source supports it. ([Uno Platform][1])
* Uno mentions this is designed to play nicely with **MVUX Pagination APIs**. ([Uno Platform][1])

**Gotcha (from issue tracker)**

* There was a scenario where setting `SupportsIncrementalLoading="True"` caused items to appear only after interaction on Android; wrapping with `AutoLayout` was a workaround. If you hit that, test on mobile targets. ([GitHub][3])

---

## 7. Use with MVUX pagination

**Goal:** MVUX provides pages of data; ItemsRepeater shows them; scrolling asks MVUX for more.

**XAML (conceptual)**

```xml
<muxc:ItemsRepeater
    ItemsSource="{Binding MyPagedFeed}"          <!-- exposes ISupportIncrementalLoading -->
    xmlns:muxc="using:Microsoft.UI.Xaml.Controls"
    xmlns:utu="using:Uno.Toolkit.UI"
    utu:ItemsRepeaterExtensions.SupportsIncrementalLoading="True">
    <muxc:ItemsRepeater.ItemTemplate>
        <DataTemplate>
            <ListViewItem Content="{Binding Name}" />
        </DataTemplate>
    </muxc:ItemsRepeater.ItemTemplate>
</muxc:ItemsRepeater>
```

**Why this is a separate how-to**

* The original page only drops a note about MVUX; here we make it explicit so RAG can match “MVUX + ItemsRepeater incremental”. ([Uno Platform][1])

---

## 8. Show nothing but keep selection infrastructure

**Goal:** sometimes you start with an empty list but you already want the repeater to support selection once data arrives.

**XAML**

```xml
<muxc:ItemsRepeater
    ItemsSource="{Binding Results}"
    xmlns:muxc="using:Microsoft.UI.Xaml.Controls"
    xmlns:utu="using:Uno.Toolkit.UI"
    utu:ItemsRepeaterExtensions.SelectionMode="Single"
    utu:ItemsRepeaterExtensions.SelectedItem="{Binding SelectedResult, Mode=TwoWay}">
    <muxc:ItemsRepeater.ItemTemplate>
        <DataTemplate>
            <ListViewItem Content="{Binding Title}" />
        </DataTemplate>
    </muxc:ItemsRepeater.ItemTemplate>
</muxc:ItemsRepeater>
```

**Note**

* Even if the list is empty at first, defining selection up front avoids coerced values later when items arrive (because changing `SelectionMode` re-coerces `Selected-*`). ([Uno Platform][1])

---

[1]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/helpers/itemsrepeater-extensions.html "ItemsRepeater Extensions"
[2]: https://platform.uno/blog/uno-platform-4-8-new-startup-experience-design-system-package-importing-resizetizer-and-more/ "App Template Wizard, OpenGL Acceleration, .NET 8, DSP ..."
[3]: https://github.com/unoplatform/uno.toolkit.ui/issues/1280 "Issue #1280 · unoplatform/uno.toolkit.ui"
