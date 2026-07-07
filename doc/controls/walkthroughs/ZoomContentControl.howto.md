---
uid: Toolkit.Controls.ZoomContentControl.HowTo
tags: [zoom, pan, pinch-zoom, zoom-control, image-zoom, map-zoom]
---

# Display content in a zoom-able and pannable view

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

## Pan & zoom large content

Place any visual inside `ZoomContentControl` to enable pinch‑zoom and panning.

```xml
<Page
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:utu="using:Uno.Toolkit.UI">
    <utu:ZoomContentControl MinZoomLevel="0.5" MaxZoomLevel="4" x:Name="Zoomer">
        <Image Source="/Assets/large-map.png" Stretch="Uniform"/>
    </utu:ZoomContentControl>
</Page>
```

## Reset zoom programmatically

Call methods from code‑behind/view‑model.

```csharp
// x:Name="Zoomer"
Zoomer.ZoomTo(1.0f);          // 100%
Zoomer.ZoomToRect(new Rect(0,0,500,300));
```
