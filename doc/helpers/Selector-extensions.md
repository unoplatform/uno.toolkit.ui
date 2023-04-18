---
uid: Toolkit.Helpers.SelectorExtensions
---
# SelectorExtensions Attached Properties

Provides an easy integration between `PipsPager` and `Selector` controls.

## Properties
Property|Type|Description
-|-|-
PipsPagerProperty|PipsPager| Backing property for the `PipsPager` that will interact with the `Selector` control.

When the `SelectorExtensions.PipsPager` is set, the control will take care of updating the `NumberOfPages` and the `SelectedIndex` properties automatically.

## Usage
```xml
xmlns:utu="using:Uno.Toolkit.UI"
xmlns:muxc="using:Microsoft.UI.Xaml.Controls"
...

<Grid>
    <FlipView
        utu:SelectorExtensions.PipsPager="{Binding ElementName=pipsPager}"
        Background="Blue">
    <muxc:PipsPager
        x:Name="pipsPager"
        MinHeight="100"
        HorizontalAlignment="Center"
        MaxVisiblePips="10" />
```