---
uid: Toolkit.Controls.DrawerFlyoutPresenter
---
# DrawerFlyoutPresenter
## Summary
`DrawerFlyoutPresenter` is a special `ContentPresenter` to be used in the template of a `FlyoutPresenter` to enable gesture support.


## Properties
### Remarks
All of the properties below can be used both as a dependency property or as an attached property, much like the `ScrollViewer` properties:
```xml
xmlns:utu="using:Uno.Toolkit.UI"

<Style x:Key="CustomDrawerFlyoutPresenterStyle"
       BasedOn="{StaticResource DrawerFlyoutPresenterStyle}"
       TargetType="FlyoutPresenter">
    <Setter Property="utu:DrawerFlyoutPresenter.OpenDirection" Value="Top" />
    <Setter Property="utu:DrawerFlyoutPresenter.DrawerDepth" Value="0.66*" />
    <Setter Property="utu:DrawerFlyoutPresenter.LightDismissOverlayBackground" Value="#80808080" />
    <Setter Property="utu:DrawerFlyoutPresenter.IsGestureEnabled" Value="True" />
</Style>
<!-- and/or -->
<utu:DrawerFlyoutPresenter OpenDirection="Top"
                           DrawerDepth="0.66*"
                           LightDismissOverlayBackground="#80808080"
                           IsGestureEnabled="True" />
```

### Properties
Property|Type|Description
-|-|-
OpenDirection|DrawerOpenDirection|Gets or sets the direction in which the drawer opens toward.<br/>note: The position of drawer when opened is the opposite of this value.
DrawerDepth|GridLength|Get or sets the depth (width or height depending on the `OpenDirection`) of the drawer.\*
LightDismissOverlayBackground|Brush|Gets or sets the brush used to paint the light dismiss overlay. The default value is `#80808080` (from the default style).
IsGestureEnabled|bool|Get or sets a value that indicates whether the user can interact with the control using gesture. The default value is `true`.

## Usage
- For DrawerDepth, this value has 3 mode based on `GridUnitType`:
    ```xml
    <Style TargetType="FlyoutPresenter">
        <Setter Property="utu:DrawerFlyoutPresenter.DrawerDepth" Value="Auto" />
        <Setter Property="utu:DrawerFlyoutPresenter.DrawerDepth" Value="0.66*" />
        <Setter Property="utu:DrawerFlyoutPresenter.DrawerDepth" Value="150" />
    <!-- and/or -->
    <DrawerFlyoutPresenter DrawerDepth="Auto" />
    <DrawerFlyoutPresenter DrawerDepth="0.66*" />
    <DrawerFlyoutPresenter DrawerDepth="150" />
    ```
    - `GridUnitType.Auto`: Fit to flyout content.
    - `GridUnitType.Star`: At given ratio of screen/flyout width or height. Valid range is between 0* and 1*, excluding 0* itself.
    - `GridUnitType.Pixel`: Fixed at the given value.

To use this, simply use a `Flyout` with `Placement="Full"` and one of the followings as the `FlyoutPresenterStyle`:
> note: The direction here indicates the initial position of the drawer. The open direction is the opposite.
- `LeftDrawerFlyoutPresenterStyle`
- `TopDrawerFlyoutPresenterStyle`
- `RightDrawerFlyoutPresenterStyle`
- `BottomDrawerFlyoutPresenterStyle`

Example:
```xml
<Button Content="Bottom Drawer"
        xmlns:toolkit="using:Uno.UI.Toolkit">
    <Button.Flyout>
        <Flyout Placement="Full" FlyoutPresenterStyle="{StaticResource BottomDrawerFlyoutPresenterStyle}">
            <StackPanel toolkit:VisibleBoundsPadding.PaddingMask="All"
                        Background="SkyBlue"
                        MinHeight="200">
                <TextBlock Text="text" />
                <Button Content="button" />
            </StackPanel>
        </Flyout>
    </Button.Flyout>
</Button>
```
> note: Here `VisibleBoundsPadding.PaddingMask` is used to prevent the content from being placed outside of the user-interactable area on mobile devices.

### Extended Use Cases
- Rounded Corner
    ```xml
    <Flyout Placement="Full">
        <Flyout.FlyoutPresenterStyle>
            <Style BasedOn="{StaticResource BottomDrawerFlyoutPresenterStyle}" TargetType="FlyoutPresenter">
                <Setter Property="CornerRadius" Value="16,16,0,0" />
            </Style>
        </Flyout.FlyoutPresenterStyle>
        <Border toolkit:VisibleBoundsPadding.PaddingMask="All" Padding="16,16,0,0">
            <!-- flyout content -->
        </Border>
    </Flyout>
    ```
    > remarks: `Padding` is used on the flyout content to avoid content being clipped.
- Custom background
    ```xml
    <Flyout Placement="Full">
        <Flyout.FlyoutPresenterStyle>
            <Style BasedOn="{StaticResource BottomDrawerFlyoutPresenterStyle}" TargetType="FlyoutPresenter">
                <Setter Property="Background" Value="SkyBlue" />
            </Style>
        </Flyout.FlyoutPresenterStyle>
        <Border toolkit:VisibleBoundsPadding.PaddingMask="All" >
        <!--<Border toolkit:VisibleBoundsPadding.PaddingMask="All" Background="SkyBlue">-->
            <!-- flyout content -->
        </Border>
    </Flyout>
    ```
    > remarks: Do not set `Background` directly from the flyout content. Instead, `Background` should be set from style setter to avoid edge bleeding on certain platforms, and to avoid background being painted on the rounded corners.
