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

| Property   | Type     | Description                     |
|------------|----------|---------------------------------|
| `Path`     | `string` | Binding path from the ancestor. |

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

| Property       | Type     | Description                     |
|----------------|----------|---------------------------------|
| `AncestorType` | `Type`   | Type of ancestor to bind from.  |
| `Path`         | `string` | Binding path from the ancestor. |

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
<Page x:Class="AncestorBindingSample.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:local="using:AncestorBindingSample"
      xmlns:utu="using:Uno.Toolkit.UI"
      Background="{ThemeResource ApplicationPageBackgroundThemeBrush}"
      Tag="From Page">
    <Grid RowSpacing="20"
          HorizontalAlignment="Center">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>

        <TextBlock Text="From DataContext:"
                   FontSize="30"
                   FontWeight="Bold">
            <TextBlock.Inlines>
                <Run Text="{Binding SomeText}" />
            </TextBlock.Inlines>
        </TextBlock>
        <ListView ItemsSource="{Binding Items}"
                  Grid.Row="1"
                  Tag="From ListView">
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
