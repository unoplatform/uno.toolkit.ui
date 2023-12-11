---
uid: Toolkit.Helpers.Bindings
---

# AncestorBinding & ItemsControlBinding

These markup extensions provides relative binding based on ancestor type. If you are familiar with WPF, they are very similar to `{RelativeSource Mode=FindAncestor}`.
They are typically used from inside a `DataTemplate` to access element outside of said data-template which is not normally accessible (eg: through `ElementName` binding). The common usage is to access the parent data-context from inside the `ItemsControl.ItemTemplate`.

## Remarks

Both of the markup extensions are available for all **non-Windows** UWP platforms as well as all WinUI 3 platforms.

## ItemsControlBinding

This markup extension provides a binding to the closest parent ItemsControl. This markup can be used to access the parent ItemsControl from inside of the ItemTemplate.

> [!TIP]
> This is a shortcut for `{utu:AncestorBinding AncestorType=ItemsControl}`.

### Properties

Property|Type|Description
-|-|-
Path|string|Binding path from the ancestor.

### Usage
```cs
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

<!-- assuming the data-context is set to an instance of ExampleModel -->
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

This markup extension provides a mean to bind to an ancestor of a specific type.

### Properties

Property|Type|Description
-|-|-
AncestorType|Type|Type of ancestor to bind from.
Path|string|Binding path from the ancestor.

### Usage

```cs
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

<!-- assuming the data-context is set to an instance of ExampleModel -->
<Border Tag="From Border">
    <ListView ItemsSource="{Binding Items}" Tag="From ListView">
        <ListView.ItemTemplate>
            <DataTemplate>
                <StackPanel>
                    <TextBlock>Current item DataContext: <Run Text="{Binding}" /></TextBlock>
                    <TextBlock>Current parent(ExampleModel) DataContext:
                        <Run Text="{utu:AncestorBinding AncestorType=ListView, Path=DataContext.SomeText}" />
                    </TextBlock>

                    <TextBlock>Accessing property of a parent ListView:
                        <Run Text="{utu:AncestorBinding AncestorType=ListView, Path=Tag}" />
                    </TextBlock>
                    <TextBlock>Accessing property of a parent Border:
                        <Run Text="{utu:AncestorBinding AncestorType=Border, Path=Tag}" />
                    </TextBlock>
                <StackPanel>
            </DataTemplate>
        </ListView.ItemTemplate>
    </ListView>
</Border>
```
