# Bind to a parent from inside a DataTemplate (AncestorBinding & ItemsControlBinding)

**Goal:** Read data or properties from an ancestor element when you're inside a `DataTemplate`.

## When to use
- You need the page/view model inside an item template.
- You need the parent control (e.g., the `ListView`) from an item.

## Dependencies
- `Uno.Toolkit.UI` (or `Uno.Toolkit.WinUI`)

## Bind to the nearest ItemsControl (shortest path)
```xml
<!-- Inside an ItemTemplate -->
<TextBlock Text="{utu:ItemsControlBinding Path=DataContext.SomeText}" />
```
- Use when you only need the parent `ItemsControl`'s data.

## Bind to a specific ancestor type
```xml
<!-- Get a property from the closest Page -->
<TextBlock Text="{utu:AncestorBinding AncestorType=Page, Path=Tag}" />
```
- Use when you need any ancestor (e.g., `Page`, `Grid`, `ListView`).

## Example
```xml
<!-- xaml namespace -->
xmlns:utu="using:Uno.Toolkit.UI"

<!-- Assuming DataContext = ExampleModel -->
<ListView ItemsSource="{Binding Items}" Tag="From ListView">
    <ListView.ItemTemplate>
        <DataTemplate>
            <StackPanel>
                <!-- item value -->
                <TextBlock Text="{Binding}" />
                <!-- parent view-model -->
                <TextBlock Text="{utu:ItemsControlBinding Path=DataContext.SomeText}" />
                <!-- page property -->
                <TextBlock Text="{utu:AncestorBinding AncestorType=Page, Path=Tag}" />
            </StackPanel>
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```
