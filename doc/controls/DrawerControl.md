---
uid: Toolkit.Controls.DrawerControl
---
# DrawerControl

> [!TIP]
> This guide covers details for `DrawerControl` specifically. If you are just getting started with the Uno Material Toolkit Library, please see our [general getting started](../getting-started.md) page to make sure you have the correct setup in place.

## Summary

`DrawerControl` is a container with two views; one view for the main content, and another view that can be revealed with a swipe gesture.

## Remarks

Due to the lack of clipping, this control should be used as a full window-sized control or, at least, the side where the drawer opens from should be placed on the edge of the screen.

### C\#

```csharp
public partial class DrawerControl : ContentControl
```

### XAML

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<utu:DrawerControl>
    <utu:DrawerControl.Content>
        content
    </utu:DrawerControl.Content>
    <utu:DrawerControl.DrawerContent>
        drawerContent
    </utu:DrawerControl.DrawerContent>
</utu:DrawerControl>
```

### Inheritance

`Object` &#8594; `DependencyObject` &#8594; `UIElement` &#8594; `FrameworkElement` &#8594; `Control` &#8594; `ContentControl` &#8594; `DrawerControl`

### Constructors

| Constructor       | Description                                              |
|-------------------|----------------------------------------------------------|
| `DrawerControl()` | Initializes a new instance of the `DrawerControl` class. |

### Properties

| Property                        | Type                          | Description                                                                                                                                                                                                                                                                                |
|---------------------------------|-------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `DrawerBackground`              | `Brush`                       | Gets or sets the background of the drawer.                                                                                                                                                                                                                                                 |
| `DrawerContent`                 | `object`                      | Gets or sets the drawer content.                                                                                                                                                                                                                                                           |
| `DrawerDepth`                   | `double?`                     | Get or sets the depth (width or height depending on the `OpenDirection`) of the drawer.<br/>note: The default value is null which enables fully stretched or fit the content (see: `FitToDrawerContent`). Alternatively, a concrete value can be set for a fixed depth.                    |
| `EdgeSwipeDetectionLength`      | `double?`                     | Gets or sets the length (width or height depending on the `OpenDirection`) of the area allowed for opening swipe gesture. <br/>note: By default, this value is null allowing the drawer to be swiped open from anywhere. Setting a positive value will enforce the edge swipe for opening. |
| `FitToDrawerContent`            | `bool`=`true`                 | Get or sets a value that indicates whether the drawer will fit to content and aligned to the edge or stretch to fill the control when `DrawerDepth` is null.                                                                                                                               |
| `IsGestureEnabled`              | `bool`=`true`                 | Get or sets a value that indicates whether the user can interact with the control using gesture.                                                                                                                                                                                           |
| `IsLightDismissEnabled`         | `bool`=`true`                 | Gets or sets a value that indicates whether the drawer can be light-dismissed.                                                                                                                                                                                                             |
| `IsOpen`                        | `bool`                        | Gets or sets a value that specifies whether the drawer is open.                                                                                                                                                                                                                            |
| `LightDismissOverlayBackground` | `Brush`                       | Gets or sets the brush used to paint the light dismiss overlay.                                                                                                                                                                                                                            |
| `OpenDirection`                 | `DrawerOpenDirection`=`Right` | Gets or sets the direction in which the drawer opens toward. <br/>note: The position of the drawer when opened is the opposite of this value.                                                                                                                                              |

### Usage

#### Basic usage

```xml
xmlns:toolkit="using:Uno.UI.Toolkit"
xmlns:utu="using:Uno.Toolkit.UI"
...

<utu:DrawerControl>
    <utu:DrawerControl.Content>
        <!-- Main Content ... -->
        <Frame x:Name="ContentFrame"/>
    </utu:DrawerControl.Content>
    <utu:DrawerControl.DrawerContent>
        <Grid toolkit:VisibleBoundsPadding.PaddingMask="All"
              Padding="16">
              <!-- Drawer Content... -->
              <TextBlock Text="Drawer" />
        </Grid>
    </utu:DrawerControl.DrawerContent>
</utu:DrawerControl>
```

#### NavigationView with drawer

The `DrawerControl` can also be used to enhance [`NavigationView` (muxc)](https://learn.microsoft.com/windows/winui/api/microsoft.ui.xaml.controls.navigationview) with gesture support, using the `DrawerNavigationViewStyle`:

```xml
xmlns:utu="using:Uno.Toolkit.UI"
xmlns:muxc="using:Microsoft.UI.Xaml.Controls"
...

<muxc:NavigationView PaneTitle="Gesture NavView"
                     OpenPaneLength="320"
                     utu:DrawerControlBehavior.FitToDrawerContent="False"
                     Style="{StaticResource DrawerNavigationViewStyle}">
    <muxc:NavigationView.MenuItems>
        <muxc:NavigationViewItem Content="Home"/>
        <muxc:NavigationViewItem Content="Page 1"/>
        <muxc:NavigationViewItem Content="Page 2"/>
        <muxc:NavigationViewItem Content="Page 3"/>
    </muxc:NavigationView.MenuItems>
    <muxc:NavigationView.Content>
        <Frame x:Name="contentFrame"/>
    </muxc:NavigationView.Content>
</muxc:NavigationView>
```

From the `NavigationView`, the properties of the `DrawerControl` can be accessed through the same/similarly named properties or via attached properties:

| DrawerControl                   | NavigationView                                            |
|---------------------------------|-----------------------------------------------------------|
| `DrawerBackground`              | `utu:DrawerControlBehavior.DrawerBackground`              |
| `DrawerDepth`                   | `OpenPaneLength`                                          |
| `EdgeSwipeDetectionLength`      | `utu:DrawerControlBehavior.EdgeSwipeDetectionLength`      |
| `FitToDrawerContent`            | `utu:DrawerControlBehavior.FitToDrawerContent`            |
| `IsGestureEnabled`              | `utu:DrawerControlBehavior.IsGestureEnabled`              |
| `IsLightDismissEnabled`         | `utu:DrawerControlBehavior.IsLightDismissEnabled`         |
| `IsOpen`                        | `IsPaneOpen`                                              |
| `LightDismissOverlayBackground` | `utu:DrawerControlBehavior.LightDismissOverlayBackground` |
| `OpenDirection`                 | `utu:DrawerControlBehavior.OpenDirection`                 |

> Content and DrawerContent are populated with those of NavigationView.
