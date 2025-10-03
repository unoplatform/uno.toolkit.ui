---
uid: Toolkit.Controls.ZoomContentControl
---

# ZoomContentControl

> [!TIP]
> This guide covers details for the `ZoomContentControl`. If you are just getting started with the Uno Toolkit UI Library, please see our [general getting started](../getting-started.md) page to make sure you have the correct setup in place.

## Summary

`ZoomContentControl` allows you to display content that can be zoomed in and out, as well as panned. It is especially useful for scenarios such as viewing large images, maps, or documents where users need control over zoom levels and panning.

### XAML

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<utu:ZoomContentControl ZoomLevel="1.5"
                        MinZoomLevel="0.5"
                        MaxZoomLevel="3.0"
                        IsZoomAllowed="True"
                        IsPanAllowed="True">
    <utu:ZoomContentControl.Content>
        <Image Source="ms-appx:///Assets/Media/LargeMedia.svg" Stretch="Uniform" />
    </utu:ZoomContentControl.Content>
</utu:ZoomContentControl>
```

### Properties

| Property            | Type                                | Description                                                                                                    |
| ------------------- | ----------------------------------- | -------------------------------------------------------------------------------------------------------------- |
| `ZoomLevel`         | `double`                            | Gets or sets the current zoom level for the content.                                                           |
| `MinZoomLevel`      | `double`                            | Gets or sets the minimum zoom level allowed for the content.                                                   |
| `MaxZoomLevel`      | `double`                            | Gets or sets the maximum zoom level allowed for the content.                                                   |
| `IsZoomAllowed`     | `bool`                              | Gets or sets a value indicating whether zooming is allowed.                                                    |
| `ScaleWheelRatio`   | `double`                            | Gets or sets the ratio for scaling zoom level when using a mouse wheel.                                        |
| `PanWheelRatio`     | `double`                            | Gets or sets the ratio for panning when using a mouse wheel.                                                   |
| `IsPanAllowed`      | `bool`                              | Gets or sets a value indicating whether panning is allowed.                                                    |
| `IsActive`          | `bool`                              | Gets or sets a value indicating whether the control is active.                                                 |
| `AutoFitToCanvas`   | `bool`                              | Gets or sets a value indicating whether the content should be automatically scaled to fit within the viewport. |
| `AutoCenterContent` | `bool`                              | Gets or sets a value indicating whether the content should be automatically centered within the viewport.      |
| `AllowFreePanning`  | `bool`                              | Gets or sets a value indicating whether content can be panned outside of the viewport.                         |
| `AdditionalMargin`  | `Thickness`                         | Gets or sets additional margins around the content.                                                            |
| `ScrollBarLayout`   | `ZoomContentControlScrollBarLayout` | Gets or sets the layout style of the scroll bars.                                                              |
| `ElementOnFocus`    | `FrameworkElement`                  | Gets or sets the focused element which auto-zoom and auto-fit will be centered around.                         |

### Methods

| Method            | Return Type | Description                                                         |
| ----------------- | ----------- | ------------------------------------------------------------------- |
| `CenterContent()` | `void`      | Centers the content within the viewport.                            |
| `FitToCanvas()`   | `void`      | Adjusts the ZoomLevel so that the content fits within the viewport. |
| `ResetViewport()` | `void`      | Resets the ZoomLevel to 1, and center the content.                  |
| `ResetZoom()`     | `void`      | Resets the ZoomLevel to 1.                                          |
| `ResetScroll()`   | `void`      | Resets the scroll position to (0,0).                                |

### Usage

Below are the built-in interactions for ZoomContentControl usage. For these interactions to work, ensure the control is:

1. Visible and loaded (`IsLoaded` is true).
2. Active (`IsActive` = true).
3. Zooming and/or panning is allowed (`IsZoomAllowed`/`IsPanAllowed` = true).

#### Zooming (Ctrl + Mouse Wheel)

Ctrl + Mouse Wheel: Zoom in/out around the current cursor position.
The `ScaleWheelRatio` property controls how quickly the zoom factor changes per mouse wheel tick.

#### Scrolling (Mouse Wheel)

Mouse Wheel by itself scrolls vertically.
Shift + Mouse Wheel scrolls horizontally.
The `PanWheelRatio` property determines how many pixels to move per mouse wheel tick.

#### Panning (Middle-Click + Drag)

Press and hold the middle button.
Drag to move the content.
Release the mouse button to stop panning.
