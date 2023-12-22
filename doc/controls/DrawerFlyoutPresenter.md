---
uid: Toolkit.Controls.DrawerFlyoutPresenter
---
# DrawerFlyoutPresenter
## Summary
`DrawerFlyoutPresenter` is a special `ContentPresenter` to be used in the template of a `FlyoutPresenter` to enable gesture support.

## Properties
### Remarks
All of the properties below can be used both as a dependency property or as an attached property, much like the `ScrollViewer` properties:
# [**XAML**](#tab/xaml)
```xml
xmlns:utu="using:Uno.Toolkit.UI"

<Style x:Key="CustomDrawerFlyoutPresenterStyle"
       BasedOn="{StaticResource DrawerFlyoutPresenterStyle}"
       TargetType="FlyoutPresenter">
    <Setter Property="utu:DrawerFlyoutPresenter.OpenDirection" Value="Top" />
    <Setter Property="utu:DrawerFlyoutPresenter.DrawerLength" Value="0.66*" />
    <Setter Property="utu:DrawerFlyoutPresenter.LightDismissOverlayBackground" Value="#80808080" />
    <Setter Property="utu:DrawerFlyoutPresenter.IsGestureEnabled" Value="True" />
</Style>
<!-- and/or -->
<utu:DrawerFlyoutPresenter OpenDirection="Top"
                           DrawerLength="0.66*"
                           LightDismissOverlayBackground="#80808080"
                           IsGestureEnabled="True" />
```
# [**C#**](#tab/csharp)
```cs
new Style<FlyoutPresenter>()
    .BasedOn("DrawerFlyoutPresenterStyle")
    .Setters(s => s.DrawerFlyoutPresenter(x => x.OpenDirection(DrawerOpenDirection.Up)))
    .Setters(s => s.DrawerFlyoutPresenter(x => x.LightDismissOverlayBackground("#80808080")))
    .Setters(s => s.DrawerFlyoutPresenter(x => x.IsGestureEnabled(true)))
// and/or
new DrawerFlyoutPresenter()
    .OpenDirection(DrawerOpenDirection.Up)
    .LightDismissOverlayBackground("#80808080")
    .IsGestureEnabled(true)
```
> [!IMPORTANT]
> `DrawerLength` is currently not supported in C# Markup.
***

> [!IMPORTANT]
> There is currently a bug on windows that prevents the usage of attached property style setters. The workaround is to add the following code in your application:

```xml
<!--
    microsoft/microsoft-ui-xaml#6388 (winui, and on windows only):
    If you define attached property setter on a style with BasedOn another style with Template defined from a separate class library
    it will throw when the template is materialized:
    > Failed to assign to property 'Uno.Toolkit.UI.DrawerFlyoutPresenter.OpenDirection'.
    ^ It will mention the first attached property used in the template, regardless of which attached property setter that triggered it.

    The workaround here is to define the template in the consuming assembly again.
    It doesn't matter if it is used or not.
-->
<win:Style x:Key="MUX6388_Workaround_ForDefinitionOnly" TargetType="FlyoutPresenter">
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="FlyoutPresenter">
                <!-- This is not the full template, we are just making explicit reference to these definitions below -->
                <utu:DrawerFlyoutPresenter OpenDirection="{TemplateBinding utu:DrawerFlyoutPresenter.OpenDirection}"
                                            DrawerLength="{TemplateBinding utu:DrawerFlyoutPresenter.DrawerLength}"
                                            LightDismissOverlayBackground="{TemplateBinding utu:DrawerFlyoutPresenter.LightDismissOverlayBackground}"
                                            IsGestureEnabled="{TemplateBinding utu:DrawerFlyoutPresenter.IsGestureEnabled}" />
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</win:Style>
```

### Properties
Property|Type|Description
-|-|-
OpenDirection|DrawerOpenDirection=`Up`|Gets or sets the direction in which the drawer opens toward.<br/>note: The position of drawer when opened is the opposite of this value.
DrawerLength|GridLength=`0.66*`|Get or sets the length (width or height depending on the `OpenDirection`) of the drawer.\*
LightDismissOverlayBackground|Brush|Gets or sets the brush used to paint the light dismiss overlay. The default value is `#80808080` (from the default style).
IsGestureEnabled|bool=`true`|Get or sets a value that indicates whether the user can interact with the control using gesture.

notes:
- For DrawerLength, this value has 3 mode based on `GridUnitType`:
    ```xml
    <Style TargetType="FlyoutPresenter">
        <Setter Property="utu:DrawerFlyoutPresenter.DrawerLength" Value="Auto" />
        <Setter Property="utu:DrawerFlyoutPresenter.DrawerLength" Value="0.66*" />
        <Setter Property="utu:DrawerFlyoutPresenter.DrawerLength" Value="150" />
    <!-- and/or -->
    <DrawerFlyoutPresenter DrawerLength="Auto" />
    <DrawerFlyoutPresenter DrawerLength="0.66*" />
    <DrawerFlyoutPresenter DrawerLength="150" />
    ```
    - `GridUnitType.Auto`: Fit to flyout content.
    - `GridUnitType.Star`: At given ratio of screen/flyout width or height. Valid range is between 0* and 1*, excluding 0* itself.
    - `GridUnitType.Pixel`: Fixed at the given value.

## Usage
To use this, simply use a `Flyout` with `Placement="Full"` and one of the followings as the `FlyoutPresenterStyle`:
> note: The name prefix here indicates the initial position of the drawer (where it opens from). The open animation direction (`OpenDirection`) is the opposite.
- `LeftDrawerFlyoutPresenterStyle` (OpenDirection=Right)
- `TopDrawerFlyoutPresenterStyle` (OpenDirection=Down)
- `RightDrawerFlyoutPresenterStyle` (OpenDirection=Left)
- `BottomDrawerFlyoutPresenterStyle` (OpenDirection=Up)

Example:
# [**XAML**](#tab/xaml)
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
# [**C#**](#tab/csharp)
```cs
new Button()
    .Flyout(
        new Flyout()
            .Placement(FlyoutPlacementMode.Full)
            //TODO couldn't find BottomDrawer style
            .FlyoutPresenterStyle(Theme.FlyoutPresenter.Styles.BottomDrawer)
            .Content(
                new StackPanel()
                    .VisibleBoundsPadding(PaddingMask.All)
                    .Background(Colors.SkyBlue)
                    .MinHeight(200)
                    .Children(
                        new TextBlock()
                            .Text("text"),
                        new Button()
                            .Content("button")
                    )
            )
    )
```
***
> [!NOTE]
> Here `VisibleBoundsPadding.PaddingMask` is used to prevent the content from being placed outside of the user-interactable area on mobile devices.

### Extended Use Cases
- Rounded Corner
    # [**XAML**](#tab/xaml)
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
    # [**C#**](#tab/csharp)
    ```cs
    new Flyout()
        .Placement(FlyoutPlacementMode.Full)
        .FlyoutPresenterStyle(
            new Style<FlyoutPresenter>()
                .BasedOn("BottomDrawerFlyoutPresenterStyle")
                .Setters(s => s.CornerRadius(new CornerRadius(16, 16, 0, 0)))
        )
        .Content(
            new Border()
                .VisibleBoundsPadding(PaddingMask.All)
                .Padding(new Thickness(16, 16, 0, 0))
        )
    ```
    ***

    > remarks: `Padding` is used on the flyout content to avoid content being clipped.

- Custom background
    # [**XAML**](#tab/xaml)
    ```xml
    <Flyout Placement="Full">
        <Flyout.FlyoutPresenterStyle>
            <Style BasedOn="{StaticResource BottomDrawerFlyoutPresenterStyle}" TargetType="FlyoutPresenter">
                <Setter Property="Background" Value="SkyBlue" />
            </Style>
        </Flyout.FlyoutPresenterStyle>
        <Border toolkit:VisibleBoundsPadding.PaddingMask="All" >
            <!-- flyout content -->
        </Border>
    </Flyout>
    ```
    # [**C#**](#tab/csharp)
    ```cs
    new Flyout()
        .Placement(FlyoutPlacementMode.Full)
        .FlyoutPresenterStyle(
            new Style<FlyoutPresenter>()
                .BasedOn("BottomDrawerFlyoutPresenterStyle")
                .Setters(s => s.Background(Colors.SkyBlue))
        )
        .Content(
            new Border()
                .VisibleBoundsPadding(PaddingMask.All)
                .Padding(new Thickness(16, 16, 0, 0))
        )
    ```
    ***
    > remarks: Avoid setting `Background` directly on the flyout content:
    > ```xml
    > <Border toolkit:VisibleBoundsPadding.PaddingMask="All" Background="SkyBlue">
    > ```
    > ```cs
    > new Border()
    >     .Placement(FlyoutPlacementMode.Full)
    >     .Background(Colors.SkyBlue)
    > ```
    > Instead, `Background` should be set from style setter to avoid edge bleeding on certain platforms, and to avoid default background being painted on the rounded corners.
