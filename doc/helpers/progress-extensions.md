---
uid: Toolkit.Helpers.ProgressExtensions
---

# Progress Extensions

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

Provides attached properties to easily change the behavior of progress controls (ie. `ProgressBar`, `ProgressRing` and the shimmer of a [`SkeletonView`](xref:Toolkit.Controls.SkeletonView)), when nested in a complex visual tree.

## Remarks

This attached property is useful to indicate that some operation is ongoing for an entire section of a view, without having to manually set the `IsActive` property of each nested progress control.

## Attached Properties

| Property            | Type        | Description                                                                                   |
|---------------------|-------------|-----------------------------------------------------------------------------------------------|
| `IsActive`          | `bool`      | Once applied to a `FrameworkElement`, this property value sets whether its visual descendant progress controls display a loading indicator. |

Setting `ProgressExtensions.IsActive` to `true` will recursively enable the loading animation on all `ProgressBar` and `ProgressRing` controls in the visual tree, and resume the shimmer of every `SkeletonView`; `false` disables them. Progress controls only receive the value when it changes; a `SkeletonView` also reads the value of the nearest ancestor setting it when it loads, so one materialized in an already inactive host (e.g. the loading slot of a `LoadingView` that has finished loading) starts suspended.

## Usage

```xml
<!-- Include the following XAML namespace to use the samples below -->
xmlns:utu="using:Uno.Toolkit.UI"
...

<Grid>
    <ContentPresenter 
        x:Name="WeeklyPromotionsPresenter"
        Content="{TemplateBinding WeeklyPromotionsContent}"
        utu:ProgressExtensions.IsActive="True"
...
```
