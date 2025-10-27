---
uid: Toolkit.Helpers.FlipViewExtensions
---

# FlipView Extensions (Concise Reference)

## Summary

Provides additional features for `FlipView`.

## Properties

| Property   | Type       | Description                                                   |
|------------|------------|---------------------------------------------------------------|
| `Next`     | `FlipView` | Sets the `FlipView` that should be moved to its next item     |
| `Previous` | `FlipView` | Sets the `FlipView` that should be moved to its previous item |

## Usage Examples

```xml
<!-- Include the following XAML namespace to use the samples below -->
xmlns:utu="using:Uno.Toolkit.UI"
...
<Grid>
  <Grid.RowDefinitions>
    <RowDefinition Height="*" />
    <RowDefinition Height="Auto" />
  </Grid.RowDefinitions>

  <FlipView x:Name="flipView">
    <FlipView.Items>
      <Grid Background="Tomato">
        <TextBlock Text="Item 1"
                   HorizontalAlignment="Center"
                   VerticalAlignment="Center" />
      </Grid>
      <Grid Background="CornflowerBlue">
        <TextBlock Text="Item 2"
                   HorizontalAlignment="Center"
                   VerticalAlignment="Center" />
      </Grid>
      <Grid Background="Goldenrod">
        <TextBlock Text="Item 3"
                   HorizontalAlignment="Center"
                   VerticalAlignment="Center" />
      </Grid>
    </FlipView.Items>
  </FlipView>

  <StackPanel Orientation="Horizontal"
              HorizontalAlignment="Center"
              Grid.Row="1">
    <Button HorizontalAlignment="Left"
            utu:FlipViewExtensions.Previous="{Binding ElementName=flipView}"
            Content="Previous" />
    <Button HorizontalAlignment="Right"
            utu:FlipViewExtensions.Next="{Binding ElementName=flipView}"
            Content="Next" />
  </StackPanel>
</Grid>
```

---

**Note**: This is a concise reference. 
For complete documentation, see [FlipView-extensions.md](FlipView-extensions.md)