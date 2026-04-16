---
uid: Toolkit.Helpers.AutoGridExtensions
---

# AutoGrid Extensions

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

Provides the `AutoGrid.Mode` attached property that automatically assigns `Grid.Row` and `Grid.Column` to the children of a `Grid` based on their order in the `Children` collection. Whenever the children or definitions change, the assignments are recalculated.

## Attached Properties

| Property | Type | Description |
|---|---|---|
| `Mode` | `AutoGridMode` | Enables auto-placement and sets the fill direction. Default: `None`. |

## AutoGridMode Enum

| Value | Description |
|---|---|
| `None` (0) | Auto-placement is off. |
| `Row` (1) | Fill top-to-bottom, wrapping to the next column. Row count is taken from `RowDefinitions`. |
| `Column` (2) | Fill left-to-right, wrapping to the next row. Column count is taken from `ColumnDefinitions`. |

## Fill Behavior

The placement logic depends on which definitions are present:

| ColumnDefinitions | RowDefinitions | Placement |
|---|---|---|
| Empty | Empty | All children placed at Row 0, Column 0. |
| Defined | Empty | Children fill along columns only (`row=0`, `col = index % cols`). |
| Empty | Defined | Children fill along rows only (`col=0`, `row = index % rows`). |
| Defined | Defined | Normal two-axis fill (see mode). Overflow wraps back modulo `rows × cols`. |

When both axes are defined, the `Mode` determines the fill direction:

- **Column**: `row = cell / cols`, `col = cell % cols`
- **Row**: `col = cell / rows`, `row = cell % rows`

> [!NOTE]
> `AutoGrid` does **not** add or remove `RowDefinition`/`ColumnDefinition` entries. You are responsible for defining the grid dimensions. If children overflow the available cells, placement wraps back to cell 0.

## Usage

```xml
xmlns:utu="using:Uno.Toolkit.UI"
```

### Column fill (3 columns, auto rows)

```xml
<Grid utu:AutoGrid.Mode="Column">
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

### Row fill (3 rows, auto columns)

```xml
<Grid utu:AutoGrid.Mode="Row">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="Auto" />
        <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
        <ColumnDefinition />
        <ColumnDefinition />
    </Grid.ColumnDefinitions>

    <TextBlock Text="(0,0)" />  <!-- Row=0 Col=0 -->
    <TextBlock Text="(1,0)" />  <!-- Row=1 Col=0 -->
    <TextBlock Text="(2,0)" />  <!-- Row=2 Col=0 -->
    <TextBlock Text="(0,1)" />  <!-- Row=0 Col=1 -->
</Grid>
```
