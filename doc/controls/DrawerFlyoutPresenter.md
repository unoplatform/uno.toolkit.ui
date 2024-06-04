---
uid: Toolkit.Controls.DrawerFlyoutPresenter
---
# DrawerFlyoutPresenter

## Summary

`DrawerFlyoutPresenter` is a special `ContentPresenter` to be used in the template of a `FlyoutPresenter` to enable gesture support.

## Remarks

All of the properties below can be used both as a dependency property or as an attached property, much like the `ScrollViewer` properties:

```xml
xmlns:utu="using:Uno.Toolkit.UI"

<Style x:Key="CustomDrawerFlyoutPresenterStyle"
       BasedOn="{StaticResource DrawerFlyoutPresenterStyle}"
       TargetType="FlyoutPresenter">
    <Setter Property="utu:DrawerFlyoutPresenter.OpenDirection" Value="Up" />
    <Setter Property="utu:DrawerFlyoutPresenter.DrawerLength" Value="0.66*" />
    <Setter Property="utu:DrawerFlyoutPresenter.LightDismissOverlayBackground" Value="#80808080" />
    <Setter Property="utu:DrawerFlyoutPresenter.IsGestureEnabled" Value="True" />
    <Setter Property="utu:DrawerFlyoutPresenter.IsLightDismissEnabled" Value="True" />
</Style>
<!-- and/or -->
<utu:DrawerFlyoutPresenter OpenDirection="Up"
                           DrawerLength="0.66*"
                           LightDismissOverlayBackground="#80808080"
                           IsGestureEnabled="True"
                           IsLightDismissEnabled="True" />
```

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
                                            IsGestureEnabled="{TemplateBinding utu:DrawerFlyoutPresenter.IsGestureEnabled}"
                                            IsLightDismissEnabled="{TemplateBinding utu:DrawerFlyoutPresenter.IsLightDismissEnabled}" />
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</win:Style>
```

## Properties

| Property                        | Type                       | Description                                                                                                                              |
|---------------------------------|----------------------------|------------------------------------------------------------------------------------------------------------------------------------------|
| `OpenDirection`                 | `DrawerOpenDirection`=`Up` | Gets or sets the direction in which the drawer opens toward.<br/>note: The position of drawer when opened is the opposite of this value. |
| `DrawerLength`                  | `GridLength`=`0.66*`       | Get or sets the length (width or height depending on the `OpenDirection`) of the drawer.\*                                               |
| `LightDismissOverlayBackground` | `Brush`                    | Gets or sets the brush used to paint the light dismiss overlay. The default value is `#80808080` (from the default style).               |
| `IsGestureEnabled`              | `bool`=`true`              | Get or sets a value that indicates whether the user can interact with the control using gesture.                                         |
| `IsLightDismissEnabled`         | `bool`=`true`              | Gets or sets a value that indicates whether the drawer flyout can be light-dismissed.                                                    |

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
  - `GridUnitType.Star`: At given ratio of screen/flyout width or height. Valid range is between 0\* and 1\*, excluding 0\* itself.
  - `GridUnitType.Pixel`: Fixed at the given value.

### Usage

> [!Video https://www.youtube-nocookie.com/embed/JWTZxnVsd_A]

To use this, simply use a `Flyout` with `Placement="Full"` and one of the followings as the `FlyoutPresenterStyle`.

> [!NOTE]
> The name prefix here indicates the initial position of the drawer (where it opens from). The open animation direction (`OpenDirection`) is the opposite.
>
> - `LeftDrawerFlyoutPresenterStyle` (OpenDirection=Right)
> - `TopDrawerFlyoutPresenterStyle` (OpenDirection=Down)
> - `RightDrawerFlyoutPresenterStyle` (OpenDirection=Left)
> - `BottomDrawerFlyoutPresenterStyle` (OpenDirection=Up)

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

> [!NOTE]
> Here `VisibleBoundsPadding.PaddingMask` is used to prevent the content from being placed outside of the user-interactable area on mobile devices.

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

    > [!NOTE]
    > `Padding` is used on the flyout content to avoid content being clipped.
- Custom background

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

    > [!WARNING]
    > Avoid setting `Background` directly on the flyout content:
    >
    > ```xml
    > <Border toolkit:VisibleBoundsPadding.PaddingMask="All" Background="SkyBlue">
    > ```
    >
    > Instead, `Background` should be set from style setter to avoid edge bleeding on certain platforms, and to avoid default background being painted on the rounded corners.
