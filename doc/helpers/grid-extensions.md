---
uid: Toolkit.Helpers.GridExtensions
---

# GridExtensions

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

Provides attached properties that automatically assign `Grid.Row` and `Grid.Column` to the children of a `Grid` based on their order in the `Children` collection. Whenever the children or definitions change, the assignments are recalculated.

## Attached Properties

| Property | Type | Description |
|---|---|---|
| `Auto` | `bool` | Enables or disables auto-placement. Default: `false`. |

## Fill Behavior

The placement logic depends on which definitions are present:

| ColumnDefinitions | RowDefinitions | Placement |
|---|---|---|
| Empty | Empty | All children placed at Row 0, Column 0. |
| Defined | Empty | Children fill along columns only (`row=0`, `col = index % cols`). |
| Empty | Defined | Children fill along rows only (`col=0`, `row = index % rows`). |
| Defined | Defined | Normal two-axis fill (left-to-right, wraps to next row). Overflow wraps back modulo `rows × cols`. |

Fill order is implied by which definitions are present; there is no explicit orientation property.

## Limitations

> [!NOTE]
> `GridExtensions` does **not** add or remove `RowDefinition`/`ColumnDefinition` entries. You are responsible for defining the grid dimensions. If children overflow the available cells, placement wraps back to cell 0.

- **Spans are not accounted for.** `Grid.RowSpan`/`Grid.ColumnSpan` set on a child are left untouched, but placement treats every child as 1×1. A child with `Grid.ColumnSpan="2"` will therefore be overlapped by the next child, which takes the cell the span covers.
- **Author-set positions are overwritten.** While `Auto="True"`, any `Grid.Row`/`Grid.Column` set on a child is replaced by its index-derived position. There is no per-child opt-out.
- **Placement is recalculated on layout.** The `Auto` handler is scoped to the `Grid` being loaded, so children added while the `Grid` is detached from the visual tree are placed when it is loaded again.

## Usage

```xml
xmlns:utu="using:Uno.Toolkit.UI"
```

### Horizontal fill (3 columns, auto rows)

```xml
<Grid utu:GridExtensions.Auto="True">
    <Grid.ColumnDefinitions>
        <ColumnDefinition />
        <ColumnDefinition />
        <ColumnDefinition />
    </Grid.ColumnDefinitions>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>

    <TextBlock Text="(0,0)" />  <!-- Row=0 Col=0 -->
    <TextBlock Text="(0,1)" />  <!-- Row=0 Col=1 -->
    <TextBlock Text="(0,2)" />  <!-- Row=0 Col=2 -->
    <TextBlock Text="(1,0)" />  <!-- Row=1 Col=0 -->
</Grid>
```
