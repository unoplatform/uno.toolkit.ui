---
uid: Toolkit.Helpers.Bindings
---

# AncestorBinding & ItemsControlBinding

These markup extensions provide relative binding based on ancestor type. If you are familiar with WPF, they are very similar to `{RelativeSource Mode=FindAncestor}`.
They are typically used from inside a `DataTemplate` to access elements outside of said data-template which is not normally accessible (eg: through `ElementName` binding). The common usage is to access the parent data-context from inside the `ItemsControl.ItemTemplate`.

## Remarks

Both of the markup extensions are available for all **non-Windows** UWP platforms as well as all WinUI 3 platforms.

## ItemsControlBinding

This markup extension provides a binding to the closest parent ItemsControl. This markup can be used to access the parent ItemsControl from inside of the ItemTemplate.

> [!TIP]
> This is a shortcut for `{utu:AncestorBinding AncestorType=ItemsControl}`.

### Properties

> see the Properties section in AncestorBinding.

### Usage

```csharp
public class ExampleModel
{
    public int[] Items { get; } = new[] { 1, 2, 3 };
    public string SomeText { get; } = "Lorem Ipsum";
}
```

```xml
<!-- Include the following XAML namespace to use the samples below -->
xmlns:utu="using:Uno.Toolkit.UI"
...

<!-- Assuming the DataContext is set to an instance of ExampleModel -->
<ListView ItemsSource="{Binding Items}">
    <ListView.ItemTemplate>
        <DataTemplate>
            <StackPanel>
                <TextBlock>Current item DataContext: <Run Text="{Binding}" /></TextBlock>
                <TextBlock>Current parent(ExampleModel) DataContext:
                    <Run Text="{utu:ItemsControlBinding Path=DataContext.SomeText}" />
                </TextBlock>
            <StackPanel>
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```

## AncestorBinding

This markup extension provides a means to bind to an ancestor of a specific type.

### Properties

| Property           | Type            | Description                                                                                                                            |
| ------------------ | --------------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| `AncestorType`     | `Type`          | Type of ancestor to bind from.                                                                                                         |
| `Path`             | `string`        | Binding path from the ancestor.                                                                                                        |
| Converter          | IValueConverter | Converter object that is called by the binding engine to modify the data as it is passed between the source and target, or vice versa. |
| ConverterParameter | object          | Parameter that can be used in the Converter logic.                                                                                     |
| ConverterLanguage  | string          | value that names the language to pass to any converter specified by the Converter property.                                            |

### Usage

```csharp
public class ExampleModel
{
    public int[] Items { get; } = new[] { 1, 2, 3 };
    public string SomeText { get; } = "Lorem Ipsum";
}
```

```xml
<!-- Include the following XAML namespace to use the samples below -->
xmlns:utu="using:Uno.Toolkit.UI"
...

<!-- Assuming the DataContext is set to an instance of ExampleModel -->
<Page Tag="From Page">
    <StackPanel>
        <TextBlock Text="From DataContext:" />
        <TextBlock Text="{Binding SomeText}" />

        <ListView ItemsSource="{Binding Items}" Tag="From ListView">
            <ListView.ItemTemplate>
                <DataTemplate>
                    <StackPanel>
                        <TextBlock Text="Current item DataContext:" />
                        <TextBlock FontWeight="Bold" Text="{Binding}" />

                        <TextBlock Text="Current parent (ExampleModel) DataContext:" />
                        <TextBlock FontWeight="Bold" Text="{utu:ItemsControlBinding Path=DataContext.SomeText}" />

                        <TextBlock Text="Accessing property of a parent ListView:" />
                        <TextBlock FontWeight="Bold" Text="{utu:ItemsControlBinding Path=Tag}" />

                        <TextBlock Text="Accessing property of a Page:" />
                        <TextBlock FontWeight="Bold" Text="{utu:AncestorBinding AncestorType=Page, Path=Tag}" />
                    </StackPanel>
                </DataTemplate>
            </ListView.ItemTemplate>
        </ListView>
    </Grid>
</Page>
```
