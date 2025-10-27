---
uid: Toolkit.Controls.DrawerControl
---

# DrawerControl (Concise Reference)

## Summary

> This guide covers details for `DrawerControl` specifically. If you are just getting started with the Uno Material Toolkit Library, please see our [general getting started](../getting-started.md) page to make sure you have the correct setup in place.

## Constructors

| Constructor       | Description                                              |
|-------------------|----------------------------------------------------------|
| `DrawerControl()` | Initializes a new instance of the `DrawerControl` class. |

## Properties

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

## Usage Examples

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

---

**Note**: This is a concise reference. 
For complete documentation, see [DrawerControl.md](DrawerControl.md)