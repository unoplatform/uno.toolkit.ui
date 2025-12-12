# How to sync a selector with a pipspager

This how-to shows how to make a `Selector` control (for example `FlipView`, `ListViewBase`, `ListView`, `GridView`) move together with a `PipsPager`.

> [!IMPORTANT]
> The `SelectorExtensions.PipsPager` attached property must be set on the **Selector control** (e.g., `FlipView`, `ListView`), NOT on the `PipsPager`.
>
> `<FlipView utu:SelectorExtensions.PipsPager="{Binding ElementName=pager}" />`

**Outcome:** when the user changes page with the `PipsPager`, the selector changes its selected item; when the selector changes selection, the `PipsPager` updates too.

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

## 1. Add namespaces

```xml
<Page
    ...
    xmlns:utu="using:Uno.Toolkit.UI"
    xmlns:muxc="using:Microsoft.UI.Xaml.Controls">
```

* `utu:` is for Uno Toolkit attached properties
* `muxc:` is for `PipsPager`

## 2. Declare the pager

```xml
<muxc:PipsPager
    x:Name="Pager"
    HorizontalAlignment="Center"
    MaxVisiblePips="10" />
```

* `x:Name="Pager"` is important — we will bind to it.

## 3. Bind the selector to the pager

> [!IMPORTANT]
> Attach the property to the **FlipView**, NOT to the PipsPager.

Example with `FlipView`:

```xml
<FlipView
    utu:SelectorExtensions.PipsPager="{Binding ElementName=Pager}">
    <!-- your FlipView items here -->
</FlipView>
```

**What happens:**

* `SelectorExtensions.PipsPager` is the only property you need.
* It attaches **to the selector (FlipView)**, not the pager.
* When set, the extension:

  * updates the pager `NumberOfPages` based on the selector items
  * keeps the pager `SelectedIndex` in sync with the selector `SelectedIndex`

---

## How to page a FlipView with pips

**Goal:** show content in a `FlipView` and let users jump to any item using a row of pips.

```xml
<Grid
    xmlns:utu="using:Uno.Toolkit.UI"
    xmlns:muxc="using:Microsoft.UI.Xaml.Controls">

    <Grid.RowDefinitions>
        <RowDefinition Height="*" />
        <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>

    <!-- Content to page through -->
    <FlipView
        Grid.Row="0"
        utu:SelectorExtensions.PipsPager="{Binding ElementName=FlipPips}">
        <FlipViewItem Content="Page 1" />
        <FlipViewItem Content="Page 2" />
        <FlipViewItem Content="Page 3" />
    </FlipView>

    <!-- Pager -->
    <muxc:PipsPager
        x:Name="FlipPips"
        Grid.Row="1"
        HorizontalAlignment="Center"
        MaxVisiblePips="5" />
</Grid>
```

**Notes:**

* You do **not** set `NumberOfPages` yourself; the extension does it.
* You do **not** handle `SelectionChanged`; the extension does it.

---

## How to page a ListView with pips

You can use any selector-like control that exposes `SelectedIndex`. The extension will still sync it.

```xml
<Grid
    xmlns:utu="using:Uno.Toolkit.UI"
    xmlns:muxc="using:Microsoft.UI.Xaml.Controls">

    <Grid.RowDefinitions>
        <RowDefinition Height="*" />
        <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>

    <ListView
        Grid.Row="0"
        utu:SelectorExtensions.PipsPager="{Binding ElementName=ListPips}">
        <x:String>Alpha</x:String>
        <x:String>Beta</x:String>
        <x:String>Gamma</x:String>
        <x:String>Delta</x:String>
    </ListView>

    <muxc:PipsPager
        x:Name="ListPips"
        Grid.Row="1"
        HorizontalAlignment="Center" />
</Grid>
```

**Outcome:** selecting an item moves the pips; clicking a pip selects the item.

---

## How to page dynamic items

If items are added/removed, the extension updates the pager automatically, because it tracks the selector’s items.

```xml
<ListView
    ItemsSource="{Binding People}"
    utu:SelectorExtensions.PipsPager="{Binding ElementName=PeoplePips}" />

<muxc:PipsPager
    x:Name="PeoplePips"
    MaxVisiblePips="7" />
```

* When `People` grows, `NumberOfPages` in the pager grows.
* When `People` shrinks, `NumberOfPages` shrinks.

---

## API notes (flat for RAG)

* **Attached property:** `utu:SelectorExtensions.PipsPager`

  * **Type:** `Microsoft.UI.Xaml.Controls.PipsPager`
  * **Usage:** attach to any `Selector`-derived control
  * **Effect:** keeps `SelectedIndex` and `NumberOfPages` in sync between selector and pager
  * **Direction:** two-way (selector → pager, pager → selector)
  * **Manual code-behind:** **not required** in normal scenarios
