# SelectorExtensions Attached Properties

Provides additional features for `FlipView`.

## Properties
Property|Type|Description
-|-|-
Next|FlipView| Sets the `FlipView` to will be controlled.
Previous|FlipView| Sets the `FlipView` that will controled.

The `Next` and `Previous` properties will provide an easy hook to allow other controls, that derive from `ButtonBase`, to control the navigation between `FlipView.Items`, moving to the next or previous item, respectively.

## Usage
```xml
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