---
uid: Toolkit.Helpers.SelectorExtensions
---

# Selector Extensions (Concise Reference)

## Summary

Provides an easy integration between `PipsPager` and `Selector` controls.

## Properties

| Property            | Type        | Description                                                                          |
|---------------------|-------------|--------------------------------------------------------------------------------------|
| `PipsPagerProperty` | `PipsPager` | Backing property for the `PipsPager` that will interact with the `Selector` control. |

## Usage Examples

```xml
<!-- Include the following XAML namespace to use the samples below -->
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

---

**Note**: This is a concise reference. 
For complete documentation, see [Selector-extensions.md](Selector-extensions.md)