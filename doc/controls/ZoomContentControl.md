---
uid: Toolkit.Controls.ZoomContentControl
---

# ZoomContentControl

> [!TIP]
> This guide covers details for the `ZoomContentControl`. If you are just getting started with the Uno Toolkit UI Library, please see our [general getting started](../getting-started.md) page to make sure you have the correct setup in place.

## Summary

`ZoomContentControl` allows you to display content that can be zoomed in and out, as well as panned. It is especially useful for scenarios such as viewing large images, maps, or documents where users need control over zoom levels and panning.

### C\#

```csharp
public partial class ZoomContentControl : Control
```

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
        <Image Source="ms-appx:///Assets/Media/LargeMedia.svg"
               Stretch="Uniform" />
    </utu:ZoomContentControl.Content>
</utu:ZoomContentControl>
```

### Inheritance

`Object` &#8594; `DependencyObject` &#8594; `UIElement` &#8594; `FrameworkElement` &#8594; `Control` &#8594; `ContentControl` &#8594; `ZoomContentControl`

### Constructors

| Constructor            | Description                                                   |
| ---------------------- | ------------------------------------------------------------- |
| `ZoomContentControl()` | Initializes a new instance of the `ZoomContentControl` class. |

### Properties

| Property           | Type        | Description                                                                                            |
| ------------------ | ----------- | ------------------------------------------------------------------------------------------------------ |
| `ZoomLevel`        | `double`    | Gets or sets the current zoom level for the content.                                                   |
| `MinZoomLevel`     | `double`    | Gets or sets the minimum zoom level allowed for the content.                                           |
| `MaxZoomLevel`     | `double`    | Gets or sets the maximum zoom level allowed for the content.                                           |
| `IsZoomAllowed`    | `bool`      | Gets or sets a value indicating whether zooming is allowed.                                            |
| `ScaleWheelRatio`  | `double`    | Gets or sets the ratio for scaling zoom level when using a mouse wheel.                                |
| `PanWheelRatio`    | `double`    | Gets or sets the ratio for panning when using a mouse wheel.                                           |
| `IsPanAllowed`     | `bool`      | Gets or sets a value indicating whether panning is allowed.                                            |
| `IsActive`         | `bool`      | Gets or sets a value indicating whether the control is active.                                         |
| `AutoFitToCanvas`  | `bool`      | Determines if the content should automatically fit into the available canvas when the control resizes. |
| `AdditionalMargin` | `Thickness` | Gets or sets additional margins around the content.                                                    |

### Methods

| Method            | Return Type | Description                                                                               |
| ----------------- | ----------- | ----------------------------------------------------------------------------------------- |
| `FitToCanvas()`   | `void`      | Adjust the zoom level so that the content fits within the available space.                |
| `ResetViewport()` | `void`      | Resets the zoom level and panning offset to their default values and centers the content. |

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

#### Programmatic Control

Beyond user interactions, you can control zoom and pan directly:

`FitToCanvas()`: Automatically sizes the content so it fits the entire available space.  
`ResetViewport()`: Resets both zoom level and offset to their defaults (zoom = 1, scroll offsets = 0).

### Advanced Usage: Overriding Pointer Methods

ZoomContentControl does not subscribe to pointer events directly. Instead, it overrides the base OnPointer methods. This lets you derive from ZoomContentControl and fully customize pointer handling in your subclass. For example:

```csharp
public class MyCustomZoomControl : ZoomContentControl
{
    protected override void OnPointerPressed(PointerRoutedEventArgs e)
    {
        // Skip or extend default behavior:
        // base.OnPointerPressed(e);

        // Implement your own pointer logic here...
    }
}
```

If you do not call `base.OnPointerPressed(e)`, you bypass the built-in pan/zoom logic entirely. This approach lets you disable or alter parts of the default pointer behavior without modifying the original code.
