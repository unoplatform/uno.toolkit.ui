---
uid: Toolkit.Helpers.ItemRepeaterExtensions
---

# ItemsRepeater Extensions

Provides selection support for `ItemsRepeater`.

## Attached Properties

Property|Type|Description
-|-|-
SelectedItem|object|Two-ways bindable property for the current/first(in Multiple mode) selected item.\*
SelectedIndex|int|Two-ways bindable property for the current/first(in Multiple mode) selected index.\*
SelectedItems|IList\<object>|Two-ways bindable property for the current selected items.\*
SelectedIndexes|IList\<int>|Two-ways bindable property for the current selected indexes.\*
SelectionMode|ItemsSelectionMode|Gets or sets the selection behavior: `None`, `SingleOrNone`, `Single`, `Multiple` <br/> note: Changing this value will cause the `Selected-`properties to be re-coerced.

### Remarks

- `Selected-`properties only takes effect when `SelectionMode` is set to a valid value that is not `None`.
- `ItemsSelectionMode`: Defines constants that specify the selection behavior.
  > Different numbers of selected items are guaranteed: None=0, SingleOrNone=0 or 1, Single=1, Multiple=0 or many.
  - `None`: Selection is disabled.
  - `SingleOrNone`: Up to one item can be selected at a time. The current item can be deselected.
  - `Single`: One item is selected at any time. The current item cannot be deselected.
  - `Multiple`: The current item cannot be deselected.

## Usage

View-Model:

```cs
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

Xaml:

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

> [!NOTE]
> While the `SelectedItems` property **needs** to be bound to a property of type `object[]`,
> the `SelectedItem` property can be bound to a property of type `object` or the item type of the `ItemsSource` collection.

### Remarks

- The selection feature from this extension supports ItemTemplate whose root element is a `SelectorItem` or `ToggleButton`(which includes `Chip`).
- `RadioButton`: Multiple mode is not supported due to control limitation.
