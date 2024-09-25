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

| Constructor| Description|
|----------------|-------------------------------------------------------|
| `ZoomContentControl()`|Initializes a new instance of the `ZoomContentControl` class.|

### Properties

| Property| Type| Description|
|------------------------|-----------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `ZoomLevel`| `double`| Gets or sets the current zoom level for the content.|
| `MinZoomLevel`| `double`| Gets or sets the minimum zoom level allowed for the content.|
| `MaxZoomLevel`| `double`| Gets or sets the maximum zoom level allowed for the content.|
| `isZoomAllowed`| `bool`| Gets or sets a value indicating whether zooming is allowed.|
| `IsPanAllowed`| `bool`| Gets or sets a value indicating whether panning is allowed.|
| `HorizontalOffest`| `double`| Gets or sets the horizontal offset for panning.|
| `VerticalOffset`| `double`| Gets or sets the vertical offset for panning.|
| `HorizontalZoomCenter`| `double`| Gets or sets the horizontal center point of the zooming operation.|
| `VerticalZoomCenter`| `double`| Gets or sets the vertical center point of the zooming operation.|
| `ScaleWheelRatio`| `double`| Gets or sets the ratio for scaling zoom level when using a mouse wheel.|
| `PanWheelRatio`| `double`| Gets or sets the ratio for panning when using a mouse wheel.|
| `ResetWhenNotActive`| `bool`| Gets or sets a value indicating whether the zoom and pan should reset when the control is not active.|

### Methods

| Method| Return Type| Description|
|-----------------------|------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `ResetZoom()`| `void`| Resets the zoom level to its default value.|
| `ResetOffset()`| `void`| Resets the horizontal and vertical offset to its default value. |
| `ZoomToCanvas()`| `void`| Adjust the zoom level so that the content fits within the available space.|
| `Centralize()`| `void`| Centers the content within the available space.|
