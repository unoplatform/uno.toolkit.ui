---
uid: Toolkit.Controls.ZoomContentControl
---

# ZoomContentControl (Concise Reference)

## Summary

> This guide covers details for the `ZoomContentControl`. If you are just getting started with the Uno Toolkit UI Library, please see our [general getting started](../getting-started.md) page to make sure you have the correct setup in place.

## Properties

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

---

**Note**: This is a concise reference. 
For complete documentation, see [ZoomContentControl.md](ZoomContentControl.md)