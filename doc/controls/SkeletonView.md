---
uid: Toolkit.Controls.SkeletonView
---

# SkeletonView

Wraps content and, while loading, visually replaces it with auto-generated skeleton placeholders and an optional shimmer animation.

## Overview

`SkeletonView` improves perceived performance by showing placeholder UI that mimics the structure of the content being loaded. Unlike typical skeleton implementations, you do **not** declare any placeholder shapes: the control derives them from the actual layout of its content.

While `IsLoading` is true, the control walks the content's visual tree, generates one placeholder per *leaf* element (`TextBlock`, `Image`, any `Shape`, buttons), matching each element's position and size, and hides the real content. When loading completes, the placeholders are removed and the content is revealed. Because the placeholders come from the real layout, they never drift out of sync with it.

For elements that have no value yet (an empty data-bound `TextBlock`, an `Image` without a source), the placeholder falls back to the space the layout granted the element — a stretched empty `TextBlock` produces a full-width text line whose height is derived from its `FontSize`.

## Properties

| Property                  | Type           | Description                                                                                                       |
|---------------------------|----------------|-------------------------------------------------------------------------------------------------------------------|
| `IsLoading`               | `bool`         | Whether the skeleton placeholders are shown instead of the content. Default is `true`. Driven automatically when `Source` is set. |
| `Source`                  | `ILoadable`    | An `ILoadable` (e.g. an async command) whose `IsExecuting` state drives `IsLoading`. See [LoadingView](xref:Toolkit.Controls.LoadingView) for details on `ILoadable`. |
| `EnableShimmer`           | `bool`         | Whether the shimmer animation plays while loading; when `false` the placeholders are static. Default is `true`.   |
| `ShimmerDuration`         | `Duration`     | Duration of one shimmer sweep. Default is 1.5 seconds.                                                            |
| `SkeletonBackground`      | `Brush`        | Fill brush of the generated placeholder shapes.                                                                   |
| `ShimmerBrush`            | `Brush`        | Brush of the band swept across the placeholders; typically a horizontal `LinearGradientBrush` fading in and out of transparency. |
| `PlaceholderCornerRadius` | `CornerRadius` | Corner radius of the generated rectangular placeholders.                                                          |

### Attached properties

| Property               | Type            | Description                                                                                                        |
|------------------------|-----------------|--------------------------------------------------------------------------------------------------------------------|
| `SkeletonView.Ignore`  | `bool`          | Excludes an element and its subtree from placeholder generation (e.g. a static header that should stay visible in structure). |
| `SkeletonView.Shape`   | `SkeletonShape` | Forces the placeholder shape for an element: `Auto` (default), `Rectangle`, or `Circle`. A non-`Auto` value also makes the element a generation leaf — a single placeholder covers it and its subtree is not visited. |

With `SkeletonShape.Auto`, an `Ellipse` produces a circle and every other leaf produces a rounded rectangle.

## Usage

The content is written once, as regular XAML bound to your data — no skeleton markup:

```xml
xmlns:utu="using:Uno.Toolkit.UI"

<utu:SkeletonView IsLoading="{Binding IsBusy}">
	<StackPanel Spacing="12">
		<StackPanel Orientation="Horizontal" Spacing="12">
			<Ellipse Width="48" Height="48" Fill="{ThemeResource SystemControlHighlightAccentBrush}" />
			<StackPanel VerticalAlignment="Center" Spacing="4">
				<TextBlock Text="{Binding Title}" Style="{StaticResource SubtitleTextBlockStyle}" />
				<TextBlock Text="{Binding Subtitle}" Style="{StaticResource CaptionTextBlockStyle}" />
			</StackPanel>
		</StackPanel>
		<TextBlock Text="{Binding Description}" TextWrapping="Wrap" />
	</StackPanel>
</utu:SkeletonView>
```

### Driven by an ILoadable

Like `LoadingView`, the skeleton state can follow any `ILoadable` (such as an async command) instead of a manual flag:

```xml
<utu:SkeletonView Source="{Binding LoadDataCommand}">
	<TextBlock Text="{Binding Description}" TextWrapping="Wrap" />
</utu:SkeletonView>
```

### Overrides

```xml
<utu:SkeletonView IsLoading="{Binding IsBusy}">
	<StackPanel Spacing="12">
		<!-- excluded from generation -->
		<TextBlock utu:SkeletonView.Ignore="True" Text="Static header" />

		<!-- forced into a circular placeholder, subtree not visited -->
		<Border utu:SkeletonView.Shape="Circle" Width="64" Height="64">
			<Image Source="{Binding AvatarUrl}" />
		</Border>
	</StackPanel>
</utu:SkeletonView>
```

## Lightweight Styling

Key | Type | Value
--- | ---- | -----
`SkeletonViewBackground` | `SolidColorBrush` | `#E0E0E0` (Dark: `#3D3D3D`)
`SkeletonViewShimmerBrush` | `LinearGradientBrush` | Horizontal transparent → `#F5F5F5` → transparent (Dark: `#4D4D4D`)
`SkeletonViewCornerRadius` | `CornerRadius` | `4`
