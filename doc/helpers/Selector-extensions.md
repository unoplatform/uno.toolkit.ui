---
uid: Toolkit.Helpers.SelectorExtensions
---

# Selector Extensions

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

Provides an easy integration between `PipsPager` and `Selector` controls.

> [!IMPORTANT]
> The `SelectorExtensions.PipsPager` attached property must be set on the **Selector control** (e.g., `FlipView`, `ListView`), NOT on the `PipsPager`.
>
> `<FlipView utu:SelectorExtensions.PipsPager="{Binding ElementName=pager}" />`

## Attached Properties

| Property            | Type        | Description                                                                          |
|---------------------|-------------|--------------------------------------------------------------------------------------|
| `PipsPagerProperty` | `PipsPager` | Backing property for the `PipsPager` that will interact with the `Selector` control. |

When the `SelectorExtensions.PipsPager` is set, the control will take care of updating the `NumberOfPages` and the `SelectedIndex` properties automatically.

## Usage

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
