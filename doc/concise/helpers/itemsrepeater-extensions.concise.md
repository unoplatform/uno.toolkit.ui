---
uid: Toolkit.Helpers.ItemRepeaterExtensions
---

# ItemsRepeater Extensions (Concise Reference)

## Summary

Provides selection and incremental loading support for `ItemsRepeater`.

## Properties

| Property          | Type                 | Description                                                                                                                                                              |
|-------------------|----------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `SelectedItem`    | `object`             | Two-ways bindable property for the current/first(in Multiple mode) selected item.\*                                                                                      |
| `SelectedIndex`   | `int`                | Two-ways bindable property for the current/first(in Multiple mode) selected index.\*                                                                                     |
| `SelectedItems`   | `IList<object>`      | Two-ways bindable property for the current selected items.\*                                                                                                             |
| `SelectedIndexes` | `IList<int>`         | Two-ways bindable property for the current selected indexes.\*                                                                                                           |
| `SelectionMode`   | `ItemsSelectionMode` | Gets or sets the selection behavior: `None`, `SingleOrNone`, `Single`, `Multiple` <br/> note: Changing this value will cause the `Selected-`properties to be re-coerced. |

## Usage Examples

```csharp
private class ViewModel : INotifyPropertyChanged
{
    public int[] MultiItemsSource { get; set; }
    public object[] MultiSelectedItems { get; set; }
    public int[] MultiSelectedIndexes { get; set; }

    public int[] SingleItemsSource { get; set; }
    public int SingleSelectedItem { get; set; }
    public int SingleSelectedIndex { get; set; }
}
```

```xml
<!-- Include the following XAML namespaces to use the samples below -->
xmlns:utu="using:Uno.Toolkit.UI"
xmlns:muxc="using:Microsoft.UI.Xaml.Controls"
...

<!-- Multi-Selection -->
<muxc:ItemsRepeater ItemsSource="{Binding MultiItemsSource}"
                    utu:ItemsRepeaterExtensions.SelectionMode="Multiple"
                    utu:ItemsRepeaterExtensions.SelectedItems="{Binding MultiSelectedItems, Mode=TwoWay}"
                    utu:ItemsRepeaterExtensions.SelectedIndexes="{Binding MultiSelectedIndexes, Mode=TwoWay}">
    <muxc:ItemsRepeater.ItemTemplate>
        <DataTemplate>
            <!--
                The root element of the ItemTemplate needs to be one of these below,
                in order for the selection visual to work.
                No additional binding on IsChecked/IsSelected is needed; It is handled by this extension.
            -->
            <ListViewItem Content="{Binding}" />
            <!-- <CheckBox Content="{Binding}" /> -->
            <!-- <ToggleButton Content="{Binding}" /> -->
            <!-- <utu:Chip Content="{Binding}" /> -->
            <!-- <RadioButton Content="{Binding}" /> -->
        </DataTemplate>
    </muxc:ItemsRepeater.ItemTemplate>
</muxc:ItemsRepeater>

<!-- Single-Selection -->
<muxc:ItemsRepeater ItemsSource="{Binding SingleItemsSource}"
                    utu:ItemsRepeaterExtensions.SelectionMode="Single"
                    utu:ItemsRepeaterExtensions.SelectedItem="{Binding SingleSelectedItem, Mode=TwoWay}"
                    utu:ItemsRepeaterExtensions.SelectedIndex="{Binding SingleSelectedIndex, Mode=TwoWay}">
    <muxc:ItemsRepeater.ItemTemplate>
        <DataTemplate>
            <!--
                The root element of the ItemTemplate needs to be one of these below,
                in order for the selection visual to work.
                No additional binding on IsChecked/IsSelected is needed; It is handled by this extension.
            -->
            <!-- <ListViewItem Content="{Binding}" /> -->
            <!-- <CheckBox Content="{Binding}" /> -->
            <!-- <ToggleButton Content="{Binding}" /> -->
            <!-- <utu:Chip Content="{Binding}" /> -->
            <RadioButton Content="{Binding}" />
        </DataTemplate>
    </muxc:ItemsRepeater.ItemTemplate>
</muxc:ItemsRepeater>
```

---

**Note**: This is a concise reference. 
For complete documentation, see [itemsrepeater-extensions.md](itemsrepeater-extensions.md)