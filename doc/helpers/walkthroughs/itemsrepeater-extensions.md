# Add selection and incremental loading to ItemsRepeater

**Goal:** Get `ItemsRepeater` to support selection and auto-prefetching.

## Dependencies
- `Uno.Toolkit.UI` (or `Uno.Toolkit.WinUI`)

## Single selection
```xml
xmlns:utu="using:Uno.Toolkit.UI"
xmlns:muxc="using:Microsoft.UI.Xaml.Controls"

<muxc:ItemsRepeater ItemsSource="{Binding Items}"
                    utu:ItemsRepeaterExtensions.SelectionMode="Single"
                    utu:ItemsRepeaterExtensions.SelectedItem="{Binding Selected, Mode=TwoWay}"
                    utu:ItemsRepeaterExtensions.SelectedIndex="{Binding SelectedIndex, Mode=TwoWay}">
    <muxc:ItemsRepeater.ItemTemplate>
        <DataTemplate>
            <RadioButton Content="{Binding}" />
        </DataTemplate>
    </muxc:ItemsRepeater.ItemTemplate>
</muxc:ItemsRepeater>
```

## Multiple selection
```xml
<muxc:ItemsRepeater ItemsSource="{Binding Items}"
                    utu:ItemsRepeaterExtensions.SelectionMode="Multiple"
                    utu:ItemsRepeaterExtensions.SelectedItems="{Binding SelectedItems, Mode=TwoWay}"
                    utu:ItemsRepeaterExtensions.SelectedIndexes="{Binding SelectedIndexes, Mode=TwoWay}">
    <muxc:ItemsRepeater.ItemTemplate>
        <DataTemplate>
            <ListViewItem Content="{Binding}" />
            <!-- Or CheckBox/ToggleButton/Chip -->
        </DataTemplate>
    </muxc:ItemsRepeater.ItemTemplate>
</muxc:ItemsRepeater>
```

## Incremental loading (prefetch on scroll)
```xml
<Grid>
  <ScrollViewer>
    <muxc:ItemsRepeater ItemsSource="{Binding InfiniteItems}"
                        utu:ItemsRepeaterExtensions.SupportsIncrementalLoading="True"
                        utu:ItemsRepeaterExtensions.IsLoading="{Binding IsLoading, Mode=TwoWay}"
                        utu:ItemsRepeaterExtensions.DataFetchSize="5"
                        utu:ItemsRepeaterExtensions.IncrementalLoadingThreshold="2">
      <muxc:ItemsRepeater.ItemTemplate>
        <DataTemplate>
          <!-- item -->
        </DataTemplate>
      </muxc:ItemsRepeater.ItemTemplate>
    </muxc:ItemsRepeater>
  </ScrollViewer>
  <TextBlock Text="Loading more..." Visibility="{Binding IsLoading, Converter={StaticResource TrueToVisible}}" />
</Grid>
```
- `ItemsSource` must implement `ISupportIncrementalLoading`.
