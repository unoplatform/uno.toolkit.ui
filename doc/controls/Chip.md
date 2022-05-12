# Chip

## Summary

`Chip` is a compact `ToggleButton` can be used for selection, filters or for a list of action to trigger. `ChipGroup` can be used to display a list of `Chip`. 

## Features

### Chip

| Properties         | Type         | Description                                                | Supported       |
|--------------------|--------------|------------------------------------------------------------|-----------------|
| IsCheckable        | bool         | Whether the chip can be checked. note: When used inside a ChipGroup, this property will be overwritten by ChipGroup's SelectionMode. | All platforms   |
| Icon          | object       | Icon to display on the chip.                          | All platforms   |
| IconTemplate  | DataTemplate | Template to display as the chip icon.                 | All platforms   |
| CanRemove          | bool         | Whether there's a remove icon on the chip.                 | All platforms   |
| RemoveCommand      | TODO         | TODO                                                       | Not implemented |
| RemoveEvent        | TODO         | TODO                                                       | Not implemented |

### ChipGroup

| Properties         | Type              | Description                                                   | Supported       |
|--------------------|-------------------|---------------------------------------------------------------|-----------------|
| SelectionMode      | ChipSelectionMode | Gets or sets the selection behavior. (None, Single, Multiple) | All platforms   |
| SelectedItem       | object            | Current selected item. (SelectionMode = Single)               | All platforms   |
| SelectedItems      | IList             | Current selected items. (SelectionMode = Multiple)            | All platforms   |
| IconTemplate  | DataTemplate      | IconTemplate to use for each `Chip`.                     | All platforms   |
| CanRemove          | bool              | Whether we display a remove icon for each `Chip`              | All platforms   |
| RemoveCommand      | TODO              | TODO                                                          | Not implemented |
| RemoveEvent        | TODO              | TODO                                                          | Not implemented |

## Usage

### Chip

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...
<!-- Filled Input Material chip -->
<utu:Chip Content="Chip"
            CanRemove="True"
			Style="{StaticResource MaterialFilledInputChipStyle}"/>

<!-- Filled Input Material chip chip with icon-->
<utu:Chip Content="Chip"
			   Style="{StaticResource MaterialFilledInputChipStyle}">
	<utu:Chip.Icon>
		<!-- Icon -->
	</utu:Chip.Icon>
</utu:Chip>

```

### ChipGroup

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...
<!-- Filled Input Material ChipGroup with static items -->
<utu:ChipGroup Style="{StaticResource MaterialFilledInputChipGroupStyle}">
    <utu:Chip Content="Chip" />
    <utu:Chip Content="Chip"
                   IsChecked="True" />
    <utu:Chip Content="Chip" />
</utu:ChipGroup>

<!-- Filled Choice Material ChipGroup with dynamic items -->
<utu:ChipGroup ItemsSource="{Binding Items}"
                 Style="{StaticResource MaterialFilledChoiceChipGroupStyle}">

<!-- Outlined Input ChipGroup with custom thumbnail template -->
<utu:ChipGroup ItemsSource="{Binding Items}"
                 Style="{StaticResource MaterialOutlinedInputChipGroupStyle}">
                 
    <utu:ChipGroup.IconTemplate>
        <DataTemplate>
             <!-- IconTemplate -->
        </DataTemplate>
    </utu:ChipGroup.IconTemplate>
</utu:ChipGroup>
```
