---
uid: Toolkit.Controls.DrawerFlyoutPresenter
---

# DrawerFlyoutPresenter (Concise Reference)

## Properties

| Property                        | Type                       | Description                                                                                                                              |
|---------------------------------|----------------------------|------------------------------------------------------------------------------------------------------------------------------------------|
| `OpenDirection`                 | `DrawerOpenDirection`=`Up` | Gets or sets the direction in which the drawer opens toward.<br/>note: The position of drawer when opened is the opposite of this value. |
| `DrawerLength`                  | `GridLength`=`0.66*`       | Get or sets the length (width or height depending on the `OpenDirection`) of the drawer.\*                                               |
| `LightDismissOverlayBackground` | `Brush`                    | Gets or sets the brush used to paint the light dismiss overlay. The default value is `#80808080` (from the default style).               |
| `IsGestureEnabled`              | `bool`=`true`              | Get or sets a value that indicates whether the user can interact with the control using gesture.                                         |
| `IsLightDismissEnabled`         | `bool`=`true`              | Gets or sets a value that indicates whether the drawer flyout can be light-dismissed.                                                    |

## Usage Examples

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

---

**Note**: This is a concise reference. 
For complete documentation, see [DrawerFlyoutPresenter.md](DrawerFlyoutPresenter.md)