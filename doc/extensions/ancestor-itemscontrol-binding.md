---
uid: Toolkit.Helpers.Bindings
---

# AncestorBinding & ItemsControlBinding

These markup extensions provides relative binding based on ancestor type. If you are familiar with WPF, they are very similar to `{RelativeSource Mode=FindAncestor}`.

## Remarks

Both of the markup extensions are available for all **non-Windows** UWP platforms as well as all WinUI 3 platforms. 

## AncestorBinding

This markup extension provides a mean to bind to an ancestor of a specific type.

### Properties

Property|Type|Description
-|-|-
AncestorType|Type|Type of ancestor to bind from.
Path|string|Binding path from the ancestor.

### Usage

```xml
<!-- Include the following XAML namespace to use the samples below -->
xmlns:utu="using:Uno.Toolkit.UI"
...
<ListView ItemsSource="{Binding Items}">
    <ListView.ItemTemplate>
        <DataTemplate>
            <StackPanel>
                <TextBlock Text="{utu:AncestorBinding AncestorType=ListView, Path=HorizontalAlignment}" />
                <TextBlock Text="{utu:AncestorBinding AncestorType=ListView, Path=DataContext.Data.PropertyOnSameLevelAsItems}" />
            <StackPanel>
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```

## ItemsControlBinding

This markup extension provides a binding to the closest parent ItemsControl. This markup can be used to access the parent ItemsControl from inside of the ItemTemplate.

### Properties

Property|Type|Description
-|-|-
Path|string|Binding path from the ancestor.

### Usage

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...
<ListView ItemsSource="{Binding Items}">
    <ListView.ItemTemplate>
        <DataTemplate>
            <StackPanel>
                <TextBlock Text="{utu:AncestorBinding AncestorType=ListView, Path=HorizontalAlignment}" />
                <TextBlock Text="{utu:ItemsControlBinding Path=DataContext.Data.PropertyOnSameLevelAsItems}" />
            <StackPanel>
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```
