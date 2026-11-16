---
uid: Toolkit.Controls.ZoomContentControl.HowTo
tags: [zoom, pan, pinch-zoom, zoom-control, image-zoom, map-zoom]
---

# Display content in a zoom-able and pannable view

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

## Pan & zoom large content

Place any visual inside `ZoomContentControl` to enable zooming and panning (Ctrl + mouse wheel zooms, the wheel scrolls, middle-click-drag pans).

```xml
<Page
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:utu="using:Uno.Toolkit.UI">
    <utu:ZoomContentControl MinZoomLevel="0.5" MaxZoomLevel="4" x:Name="Zoomer">
        <Image Source="/Assets/large-map.png" Stretch="Uniform"/>
    </utu:ZoomContentControl>
</Page>
```

## Drive the zoom from code

`ZoomLevel` is a plain `double` dependency property — set it (or bind it) directly, and use the built-in helpers for the common actions:

```csharp
// x:Name="Zoomer"
Zoomer.ZoomLevel = 2.0;   // zoom to 200%; also bindable from a view-model
Zoomer.FitToCanvas();     // pick the ZoomLevel that fits the content to the viewport
Zoomer.ResetViewport();   // back to 100%, re-centered (ResetZoom + CenterContent)
```
