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

```xml
<!-- Include the following XAML namespaces to use the samples below -->
xmlns:utu="using:Uno.Toolkit.UI"
xmlns:muxc="using:Microsoft.UI.Xaml.Controls"
...

<muxc:ItemsRepeater ItemsSource="{Binding ...}"
                    utu:ItemsRepeaterExtensions.SelectionMode="Single">
    <muxc:ItemsRepeater.ItemTemplate>
        <DataTemplate>
            <!-- pick one: -->
            <ListViewItem Content="{Binding}" />
            <!-- <CheckBox Content="{Binding}" /> -->
            <!-- <RadioButton Content="{Binding}" /> -->
            <!-- <ToggleButton Content="{Binding}" /> -->
            <!-- <utu:Chip Content="{Binding}" /> -->
        </DataTemplate>
    </muxc:ItemsRepeater.ItemTemplate>
</muxc:ItemsRepeater>
```

### Remarks

- The selection feature from this extensions support ItemTemplate whose the root element is a `SelectorItem` or `ToggleButton`(which includes `Chip`).
- `RadioButton`: Multiple mode is not supported due to control limitation.
