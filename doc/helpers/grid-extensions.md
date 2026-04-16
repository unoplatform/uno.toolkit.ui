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
| `Direction` | `Orientation` | Fill direction. `Horizontal` fills left-to-right (columns first); `Vertical` fills top-to-bottom (rows first). Default: `Horizontal`. |

## Fill Behavior

The placement logic depends on which definitions are present:

| ColumnDefinitions | RowDefinitions | Placement |
|---|---|---|
| Empty | Empty | All children placed at Row 0, Column 0. |
| Defined | Empty | Children fill along columns only (`row=0`, `col = index % cols`). |
| Empty | Defined | Children fill along rows only (`col=0`, `row = index % rows`). |
| Defined | Defined | Normal two-axis fill (see `Direction`). Overflow wraps back modulo `rows × cols`. |

When both axes are defined, `Direction` determines the fill order:

- **Horizontal**: `row = cell / cols`, `col = cell % cols` (left-to-right, wraps to next row)
- **Vertical**: `col = cell / rows`, `row = cell % rows` (top-to-bottom, wraps to next column)

> [!NOTE]
> `GridExtensions` does **not** add or remove `RowDefinition`/`ColumnDefinition` entries. You are responsible for defining the grid dimensions. If children overflow the available cells, placement wraps back to cell 0.

## Usage

```xml
xmlns:utu="using:Uno.Toolkit.UI"
```

### Horizontal fill (3 columns, auto rows)

```xml
<Grid utu:GridExtensions.Auto="True"
      utu:GridExtensions.Direction="Horizontal">
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

### Vertical fill (3 rows, auto columns)

```xml
<Grid utu:GridExtensions.Auto="True"
      utu:GridExtensions.Direction="Vertical">
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
