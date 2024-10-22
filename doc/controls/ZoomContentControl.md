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

| Property| Type | Description |
|-|-|-|
| `IsZoomAllowed` | `bool` | Gets or sets a value indicating whether zooming is allowed. |
| `ScaleWheelRatio` | `double` | Gets or sets the ratio for scaling zoom level when using a mouse wheel. |
| `PanWheelRatio` | `double` | Gets or sets the ratio for panning when using a mouse wheel. |
| `IsPanAllowed` | `bool` | Gets or sets a value indicating whether panning is allowed. |
| `ViewportWidth` | `bool` | Gets or sets the width of the content's viewport. |
| `ViewportHeight` | `bool` | Gets or sets the height of the content's viewport. |
| `IsActive` | `bool` | Gets or sets a value indicating whether the control is active. |
| `AutoFitToCanvas` | `bool` | Determines if the content should automatically fit into the available canvas when the control resizes. |
| `AdditionalMargin` | `Thickness` | Gets or sets additional margins around the content. |

### Methods

| Method| Return Type| Description|
|-----------------------|------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `FitToCanvas()`| `void`| Adjust the zoom level so that the content fits within the available space.|
| `ResetViewport()`| `void`| Resets the zoom level and panning offset to their default values and centers the content.|
