---
uid: Toolkit.Helpers.ItemRepeaterExtensions
---

# ItemsRepeater Extensions

Provides selection and incremental loading support for `ItemsRepeater`.

## Selection

### Attached Properties

| Property          | Type                 | Description                                                                                                                                                              |
|-------------------|----------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `SelectedItem`    | `object`             | Two-ways bindable property for the current/first(in Multiple mode) selected item.\*                                                                                      |
| `SelectedIndex`   | `int`                | Two-ways bindable property for the current/first(in Multiple mode) selected index.\*                                                                                     |
| `SelectedItems`   | `IList<object>`      | Two-ways bindable property for the current selected items.\*                                                                                                             |
| `SelectedIndexes` | `IList<int>`         | Two-ways bindable property for the current selected indexes.\*                                                                                                           |
| `SelectionMode`   | `ItemsSelectionMode` | Gets or sets the selection behavior: `None`, `SingleOrNone`, `Single`, `Multiple` <br/> note: Changing this value will cause the `Selected-`properties to be re-coerced. |

#### Remarks

- `Selected-`properties only take effect when `SelectionMode` is set to a valid value that is not `None`.
- `ItemsSelectionMode`: Defines constants that specify the selection behavior.
  > Different numbers of selected items are guaranteed: None=0, SingleOrNone=0 or 1, Single=1, Multiple=0 or many.
  - `None`: Selection is disabled.
  - `SingleOrNone`: Up to one item can be selected at a time. The current item can be deselected.
  - `Single`: One item is selected at any time. The current item cannot be deselected.
  - `Multiple`: The current item cannot be deselected.

### Usage

View-Model:

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

XAML:

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

#### Remarks

- The selection feature from this extension supports ItemTemplate whose root element is a `SelectorItem` or `ToggleButton`(which includes `Chip`).
- `RadioButton`: Multiple mode is not supported due to control limitations.

## Incremental Loading

Incremental loading is only supported when the `ItemsSource` of the `ItemsRepeater` implements `ISupportIncrementalLoading` and the `SupportsIncrementalLoading` attached property is set to `True`.

> [!NOTE]
> These attached properties work together with [MVUX Pagination APIs](xref:Uno.Extensions.Mvux.Advanced.Pagination) to provide a seamless experience for incremental loading.

### Attached Properties

| Property                        | Type                 | Description                                                                                                                                                                                                                                                                                                                                                        |
|---------------------------------|----------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `SupportsIncrementalLoading`    | `bool`               | Gets or sets whether the ItemsRepeaterExtensions should automatically prefetch more items when the user scrolls to the end of the list. <br/> note: The `ItemsSource` of the `ItemsRepeater` must implement [`ISupportIncrementalLoading`](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.data.isupportincrementalloading). |
| `IsLoading`                     | `bool`               | Gets whether there is currently a prefetch operation in progress.                                                                                                                                                                                                                                                                                                  |
| `DataFetchSize`                 | `double`             | Gets or sets the amount of data to fetch for prefetch operations, in pages*. Defaults to 3.                                                                                                                                                                                                                                                                        |
| `IncrementalLoadingThreshold`   | `double`             | Gets or sets the loading threshold, in terms of pages*, that governs when the ItemsRepeaterExtensions will begin to prefetch more items. Defaults to 0.                                                                                                                                                                                                            |

> [!TIP]
> \* A _page_ is defined as the estimated number of items that fit in the ItemsRepeater's [EffectiveViewport](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.frameworkelement.effectiveviewportchanged).

View-Model:

```csharp
private class ViewModel : INotifyPropertyChanged
{
    public bool IsLoading { get; set; }
    public InfiniteSource<int> InfiniteItemsSource { get; set; }
}

...

public class InfiniteSource<T> : ISupportIncrementalLoading
{
    public bool HasMoreItems { get; set; } = true;
    public Task<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
    {
        // Load more items
    }
}

```

XAML:

```xml
<!-- Include the following XAML namespaces to use the samples below -->
xmlns:utu="using:Uno.Toolkit.UI"
xmlns:muxc="using:Microsoft.UI.Xaml.Controls"
...

<!-- Incremental Loading -->
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="*" />
        <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>
    <ScrollViewer>
        <muxc:ItemsRepeater ItemsSource="{Binding InfiniteItemsSource}"
                            utu:ItemsRepeaterExtensions.SupportsIncrementalLoading="True"
                            utu:ItemsRepeaterExtensions.IsLoading="{Binding IsLoading, Mode=TwoWay}"
                            utu:ItemsRepeaterExtensions.DataFetchSize="5"
                            utu:ItemsRepeaterExtensions.IncrementalLoadingThreshold="2">
            <muxc:ItemsRepeater.ItemTemplate>
                <DataTemplate>
                    <!-- ... -->
                </DataTemplate>
            </muxc:ItemsRepeater.ItemTemplate>
        </muxc:ItemsRepeater>
    </ScrollViewer>
    <TextBlock Grid.Row="1" 
               Text="Loading more..." 
               HorizontalAlignment="Center"
               Visibility="{Binding IsLoading, Converter={StaticResource TrueToVisible}}" />
</Grid>
```

#### Remarks

- The above XAML will automatically prefetch enough items to fill the viewport with 5 pages worth of items when the user scrolls to within the last 2 pages of the list.
