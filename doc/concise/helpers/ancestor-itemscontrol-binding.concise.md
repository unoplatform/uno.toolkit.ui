---
uid: Toolkit.Helpers.Bindings
---

# AncestorBinding & ItemsControlBinding (Concise Reference)

## Summary

These markup extensions provide relative binding based on ancestor type. If you are familiar with WPF, they are very similar to `{RelativeSource Mode=FindAncestor}`.
They are typically used from inside a `DataTemplate` to access elements outside of said data-template which is not normally accessible (eg: through `ElementName` binding). The common usage is to access the parent data-context from inside the `ItemsControl.ItemTemplate`.

## Usage Examples

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

```csharp
public class ExampleModel
{
    public int[] Items { get; } = new[] { 1, 2, 3 };
    public string SomeText { get; } = "Lorem Ipsum";
}
```

---

**Note**: This is a concise reference. 
For complete documentation, see [ancestor-itemscontrol-binding.md](ancestor-itemscontrol-binding.md)