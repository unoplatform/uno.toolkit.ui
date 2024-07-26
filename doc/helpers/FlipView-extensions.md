---
uid: Toolkit.Helpers.FlipViewExtensions
---
# FlipView Extensions

Provides additional features for `FlipView`.

## Attached Properties

| Property   | Type       | Description                                                   |
|------------|------------|---------------------------------------------------------------|
| `Next`     | `FlipView` | Sets the `FlipView` that should be moved to its next item     |
| `Previous` | `FlipView` | Sets the `FlipView` that should be moved to its previous item |

The `Next` and `Previous` properties will provide an easy hook to allow other controls, that derive from `ButtonBase`, to control the navigation between `FlipView.Items`, moving to the next or previous item, respectively.

## Usage

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

## Customization

The FlipView control for desktop targets will present arrow buttons to allow the user to click on them to navigate between the content inside the FlipView. If you don't want these arrows to be shown, you can use the NoArrowsFlipViewStyle as shown in the example below:

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<Grid>
    <FlipView Style="{StaticResource NoArrowsFlipViewStyle}" />
</Grid>
```
