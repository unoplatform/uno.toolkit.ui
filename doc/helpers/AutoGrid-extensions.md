# AutoGrid Extensions

This document describes the extensions available for the `AutoGrid` control in the Uno.Toolkit library.

## Overview

The `AutoGrid` control provides a flexible way to automatically generate grid rows and columns based on its content. The extensions described here enhance its functionality and simplify common layout scenarios.

## Extensions

### 1. AutoGrid.SetRowCount

Sets the number of rows in the `AutoGrid`.

**Usage:**
```xml
<toolkit:AutoGrid toolkit:AutoGridExtensions.RowCount="3">
    <!-- Children -->
</toolkit:AutoGrid>
```

**Type:** `int`

### 2. AutoGrid.SetColumnCount

Sets the number of columns in the `AutoGrid`.

**Usage:**
```xml
<toolkit:AutoGrid toolkit:AutoGridExtensions.ColumnCount="2">
    <!-- Children -->
</toolkit:AutoGrid>
```

**Type:** `int`

### 3. AutoGrid.SetRowSpacing

Specifies the spacing between rows.

**Usage:**
```xml
<toolkit:AutoGrid toolkit:AutoGridExtensions.RowSpacing="8">
    <!-- Children -->
</toolkit:AutoGrid>
```

**Type:** `double`

### 4. AutoGrid.SetColumnSpacing

Specifies the spacing between columns.

**Usage:**
```xml
<toolkit:AutoGrid toolkit:AutoGridExtensions.ColumnSpacing="8">
    <!-- Children -->
</toolkit:AutoGrid>
```

**Type:** `double`

## Example

```xml
<toolkit:AutoGrid
    toolkit:AutoGridExtensions.RowCount="2"
    toolkit:AutoGridExtensions.ColumnCount="3"
    toolkit:AutoGridExtensions.RowSpacing="10"
    toolkit:AutoGridExtensions.ColumnSpacing="10">
    <Button Content="A"/>
    <Button Content="B"/>
    <Button Content="C"/>
    <Button Content="D"/>
    <Button Content="E"/>
    <Button Content="F"/>
</toolkit:AutoGrid>
```

## Remarks

- If row or column count is not specified, the control will auto-calculate based on the number of children.

## See Also

- [AutoGrid Control](AutoGrid.md)
- [Uno.Toolkit Documentation](../README.md)
