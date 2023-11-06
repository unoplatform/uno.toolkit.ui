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
    <Setter Property="utu:DrawerFlyoutPresenter.LightDismissOverlayBackground" Value="#80808080" />
    <Setter Property="utu:DrawerFlyoutPresenter.IsGestureEnabled" Value="True" />
</Style>
<!-- and/or -->
<utu:DrawerFlyoutPresenter OpenDirection="Top"
                           LightDismissOverlayBackground="#80808080"
                           IsGestureEnabled="True" />
```

### Properties
todo@xy: add entry for DrawerDepth

Property|Type|Description
-|-|-
OpenDirection|DrawerOpenDirection|Gets or sets the direction in which the drawer opens toward.<br/>note: The position of drawer when opened is the opposite of this value.
LightDismissOverlayBackground|Brush|Gets or sets the brush used to paint the light dismiss overlay. The default value is `#80808080` (from the default style).
IsGestureEnabled|bool|Get or sets a value that indicates whether the user can interact with the control using gesture. The default value is `true`.

## Usage
todo@xy: add examples here and in sample page for
    - depth at different value: Auto, x*, 1*, x
    - [FAQ] corner radius with proper background

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
