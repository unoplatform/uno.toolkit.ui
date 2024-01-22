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
    <FlipView>
        <FlipView.Items>
            <Grid Background="Azure">
                <Button
                    HorizontalAlignment="Left"
                    utu:FlipViewExtensions.Previous="{Binding ElementName=flipView}"
                    Content="Previous" />
                <Button
                    HorizontalAlignment="Right"
                    utu:FlipViewExtensions.Next="{Binding ElementName=flipView}"
                    Content="Next" />
            </Grid>
            <Grid Background="Blue">
                <Button
                    HorizontalAlignment="Left"
                    utu:FlipViewExtensions.Previous="{Binding ElementName=flipView}"
                    Content="Previous" />
                <Button
                    HorizontalAlignment="Right"
                    utu:FlipViewExtensions.Next="{Binding ElementName=flipView}"
                    Content="Next" />
            </Grid>
        </FlipView.Items>
    </FlipView>
```

## Customization

The `FlipView` control for desktop targets will present arrow buttons to allow the user to click on the to navigate between the content inside the `FlipView`, if you don't want these arrows to be shown, you use the `NoArrowsFlipViewStyle` as shown in the example below:

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<Grid>
    <FlipView Style="{StaticResource NoArrowsFlipViewStyle}" />
</Grid>
```
