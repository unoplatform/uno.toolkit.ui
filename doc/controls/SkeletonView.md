---
uid: Toolkit.Controls.SkeletonView
---

# SkeletonView

Auto-generated skeleton placeholders with an optional shimmer animation, in three flavors:

- `SkeletonView` wraps live content and visually replaces it with placeholders while loading.
- `SkeletonPresenter` derives a skeleton from a `DataTemplate` — for loading templates where the real content isn't in the tree yet.
- The `Skeleton.IsEnabled` attached property injects a `SkeletonPresenter` as the `ProgressTemplate` of a state-aware control (such as `FeedView` from Uno.Extensions) with no skeleton markup at all.

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

### Skeleton attached properties

| Property                       | Type            | Description                                                                                                        |
|--------------------------------|-----------------|--------------------------------------------------------------------------------------------------------------------|
| `Skeleton.Ignore`              | `bool`          | Excludes an element and its subtree from placeholder generation (e.g. a static header that should stay visible in structure). |
| `Skeleton.Shape`               | `SkeletonShape` | Forces the placeholder shape for an element: `Auto` (default), `Rectangle`, or `Circle`. A non-`Auto` value also makes the element a generation leaf — a single placeholder covers it and its subtree is not visited. |
| `Skeleton.IsEnabled`           | `bool`          | Set on a control exposing a `ProgressTemplate` (e.g. `FeedView`) to inject an auto-generated skeleton as that template. An explicitly assigned `ProgressTemplate` is never overwritten. |
| `Skeleton.PlaceholderTemplate` | `DataTemplate`  | Overrides the template the injected skeleton is derived from (default: the target's `ValueTemplate`).              |
| `Skeleton.PlaceholderCount`    | `int`           | How many placeholder rows are stamped into empty list controls of a derived skeleton. Default is `4`.              |

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
		<TextBlock utu:Skeleton.Ignore="True" Text="Static header" />

		<!-- forced into a circular placeholder, subtree not visited -->
		<Border utu:Skeleton.Shape="Circle" Width="64" Height="64">
			<Image Source="{Binding AvatarUrl}" />
		</Border>
	</StackPanel>
</utu:SkeletonView>
```

### FeedView (automatic injection)

For controls whose loading state is a *separate template* — like `FeedView` from Uno.Extensions, which shows its `ProgressTemplate` while the feed loads — a single attached property injects the whole skeleton:

```xml
<mvux:FeedView Source="{Binding People}"
			   utu:Skeleton.IsEnabled="True">
	<DataTemplate>
		<ListView ItemsSource="{Binding Data}"
				  ItemTemplate="{StaticResource PersonItem}" />
	</DataTemplate>
</mvux:FeedView>
```

While the feed is loading, an auto-generated skeleton is shown, derived from the `ValueTemplate` itself: empty list controls inside it are stamped with `Skeleton.PlaceholderCount` placeholder rows of their own `ItemTemplate`, so the skeleton can never drift from the real layout. Notes:

- The target is resolved by convention (`ProgressTemplate`/`ValueTemplate` dependency properties, found reflectively), so the toolkit takes no dependency on Uno.Extensions — and the same convention works on any control that follows it.
- A `ProgressTemplate` you assign yourself always wins; `Skeleton.IsEnabled` never overwrites it.
- Set `Skeleton.PlaceholderTemplate` on the same element to derive the skeleton from a different template than the `ValueTemplate`.

### SkeletonPresenter (template-driven, manual)

`SkeletonPresenter` is what the injection drops in — an always-loading `SkeletonView` that derives its skeleton from a template instead of live content. It can also be used directly inside any loading slot (e.g. `LoadingView.LoadingContent`, or a hand-written `ProgressTemplate`):

```xml
<utu:SkeletonPresenter utu:Skeleton.PlaceholderCount="3">
	<utu:SkeletonPresenter.ContentTemplate>
		<DataTemplate>
			<ListView ItemTemplate="{StaticResource PersonItem}" />
		</DataTemplate>
	</utu:SkeletonPresenter.ContentTemplate>
</utu:SkeletonPresenter>
```

It inherits all of `SkeletonView`'s appearance properties and theme resources.

## Lightweight Styling

| Key                        | Type                  | Value                                                              |
|----------------------------|-----------------------|--------------------------------------------------------------------|
| `SkeletonViewBackground`   | `SolidColorBrush`     | `#E0E0E0` (Dark: `#3D3D3D`)                                        |
| `SkeletonViewShimmerBrush` | `LinearGradientBrush` | Horizontal transparent → `#F5F5F5` → transparent (Dark: `#4D4D4D`) |
| `SkeletonViewCornerRadius` | `CornerRadius`        | `4`                                                                |
