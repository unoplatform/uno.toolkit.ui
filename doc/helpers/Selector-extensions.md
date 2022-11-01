# SelectorExtensions Attached Properties

Provides an easy integration between `PipsPager` and `Selector` controls.

## Properties
Property|Type|Description
-|-|-
PipsPagerProperty|PipsPager| Backing property for the `PipsPager` that will interact with the `Selector` control.

When the `SelectorExtensions.PipsPager` is set, the control will take care of updating the number of items and the `SelectedIndex` automatically.

## Usage
```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<Grid>
    <FlipView
        utu:SelectorExtensions.PipsPager="{Binding ElementName=pipsPager}"
        Background="Blue">
    <msui:PipsPager
        x:Name="pipsPager"
        MinHeight="100"
        HorizontalAlignment="Center"
        MaxVisiblePips="10" />
```