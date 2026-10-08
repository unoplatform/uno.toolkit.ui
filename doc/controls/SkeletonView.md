---
uid: Toolkit.Controls.SkeletonView
---

# SkeletonView

Skeleton placeholders mimic the shape of content that is still loading, improving perceived performance compared to a spinner. Uno Toolkit **generates the placeholders for you** from the real layout of your content: you never declare skeleton shapes.

Three building blocks cover the different ways content can be loading:

| Type | Use it when | How it works |
|------|-------------|--------------|
| [`SkeletonView`](#skeletonview) | The real content is already in the visual tree, bound to data that is still loading (or being refreshed). | Wraps the content, hides it while `IsLoading` is true, and overlays one placeholder per visual element at its actual position. |
| [`SkeletonPresenter`](#skeletonpresenter) | The real content is *not* in the tree while loading, e.g. inside a loading slot or a `ProgressTemplate`. | Instantiates a private, data-less copy of a `DataTemplate` and generates placeholders from it, stamping placeholder rows into empty lists. |
| [`Skeleton` attached properties](#skeleton-attached-properties) | You need to tune generation, or inject a skeleton into a control that follows the `ProgressTemplate`/`ValueTemplate` convention. | `Ignore`, `Shape`, `PlaceholderCount`, `PlaceholderTemplate`, `IsEnabled`. |

For MVUX's `FeedView`, see [FeedView](#feedview-uno-extensions).

```xml
xmlns:utu="using:Uno.Toolkit.UI"
```

## How placeholders are generated

Understanding the generation rules helps you get the skeleton you want with little or no tuning.

1. **The visual tree is walked from the content root.** Collapsed elements and elements marked with `utu:Skeleton.Ignore="True"` are skipped along with their subtree.
2. **Leaves get exactly one placeholder.** A leaf is a `TextBlock`, an `Image`, any `Shape` (`Ellipse`, `Rectangle`, `Path`, ...), or a button (`ButtonBase`). Leaves are not descended into, so a `Button` produces a single button-sized placeholder regardless of its content.
3. **Containers contribute their children, not themselves.** Panels and borders produce no placeholder of their own. For content controls (e.g. `ListViewItem`, `ContentControl`, cards), only the *content* is walked; the control's template chrome (backgrounds, selection and focus visuals) is ignored.
4. **Each placeholder matches the leaf's position and size** within the control.
5. **Leaves without a value yet fall back to their layout slot.** An empty data-bound `TextBlock` or a source-less `Image` measures as (almost) nothing, so the space the layout granted it is used instead:
   - width: the slot's width (a stretched empty `TextBlock` produces a full-width line); when the slot gives no width either, a `TextBlock` uses a default line width of 100;
   - height: for a `TextBlock`, one text line derived from its `FontSize`; otherwise the slot's height.
   Give such elements an explicit size, alignment or `MinWidth` when you want a more realistic placeholder.
6. **Shape:** an `Ellipse` produces a circle; every other leaf produces a rounded rectangle using `PlaceholderCornerRadius`. Force either with `utu:Skeleton.Shape`. A circle is sized to the smaller dimension and centered.

Placeholders are regenerated whenever the content or the control is resized, and when an appearance property changes.

While loading, the content stays in the tree, so layout and bindings keep working, but it is not hit-testable. **Only the elements covered by a placeholder are hidden**: everything else stays visible, including ignored elements and the backgrounds and borders of containers (for example a card keeps its frame, with placeholders inside). Covered elements are hidden by setting their `Opacity` to 0; their original `Opacity` (local value or binding) is restored once loaded.

## SkeletonView

Wraps live content and replaces it visually with generated placeholders while loading.

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsLoading` | `bool` | **`true`** | Whether the placeholders are shown instead of the content. Note the default: a `SkeletonView` starts in its loading state. Bind it to your busy flag, or set `Source`. |
| `Source` | `ILoadable` | `null` | An `ILoadable` (such as an async command) whose `IsExecuting` state drives `IsLoading`. When set, `IsLoading` follows `IsExecuting`. See [LoadingView](xref:Toolkit.Controls.LoadingView) for `ILoadable` implementations. |
| `EnableShimmer` | `bool` | `true` | Whether the shimmer animation plays while loading. Set to `false` for static placeholders (e.g. to reduce motion). |
| `ShimmerDuration` | `Duration` | `0:0:1.5` | Duration of one shimmer sweep across the control. Larger values produce a slower sweep. |
| `SkeletonBackground` | `Brush` | `SkeletonViewBackground` resource | Fill brush of the placeholders. |
| `ShimmerBrush` | `Brush` | `SkeletonViewShimmerBrush` resource | Brush of the highlight band swept across the placeholders. Typically a horizontal `LinearGradientBrush` fading in and out of transparency. Set to `null` to remove the band entirely. |
| `PlaceholderCornerRadius` | `CornerRadius` | `SkeletonViewCornerRadius` resource (`4`) | Corner radius of rectangular placeholders. Circles are unaffected. |

As a `ContentControl`, `SkeletonView` also accepts `Content`, `ContentTemplate`, `HorizontalContentAlignment`/`VerticalContentAlignment` (both `Stretch` by default), `Background`, `Padding`, etc.

### Basic usage

Write the content once, as regular XAML bound to your data, and wrap it:

```xml
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
		<Button Content="Some action" />
	</StackPanel>
</utu:SkeletonView>
```

While `IsBusy` is true, this shows a circle, two text lines, a full-width text line for the (still empty) description, and a button-shaped placeholder.

### Driven by an ILoadable

Instead of a flag, let any `ILoadable` (e.g. an async command) drive the loading state:

```xml
<Button Content="Reload" Command="{Binding LoadDataCommand}" />

<utu:SkeletonView Source="{Binding LoadDataCommand}">
	<TextBlock Text="{Binding Description}" TextWrapping="Wrap" />
</utu:SkeletonView>
```

### Refreshing loaded content

Because placeholders are generated from the live layout, toggling `IsLoading` back on over content that is already loaded produces a skeleton that matches it exactly: the same number of list items, and text lines matching the length of the current text. This makes `SkeletonView` well suited to *refresh* scenarios.

### Appearance

Every appearance property can be set per instance:

```xml
<utu:SkeletonView IsLoading="{Binding IsBusy}"
				  SkeletonBackground="#D7E3F4"
				  PlaceholderCornerRadius="12"
				  ShimmerDuration="0:0:3">
	<utu:SkeletonView.ShimmerBrush>
		<LinearGradientBrush StartPoint="0,0.5" EndPoint="1,0.5">
			<GradientStop Offset="0" Color="#00FFFFFF" />
			<GradientStop Offset="0.5" Color="#AAFFFFFF" />
			<GradientStop Offset="1" Color="#00FFFFFF" />
		</LinearGradientBrush>
	</utu:SkeletonView.ShimmerBrush>
	<!-- content -->
</utu:SkeletonView>

<!-- Static placeholders, no animation -->
<utu:SkeletonView IsLoading="{Binding IsBusy}" EnableShimmer="False">
	<!-- content -->
</utu:SkeletonView>
```

To change the look app-wide, override the [lightweight styling](#lightweight-styling) resources instead.

## SkeletonPresenter

A `SkeletonView` that generates its skeleton from a `DataTemplate` instead of live content. Use it wherever the real content does not exist yet while loading: a `LoadingView`'s `LoadingContent`, a hand-written `ProgressTemplate`, or a control template.

`SkeletonPresenter` derives from `SkeletonView`, so it inherits all of its [properties](#properties) and lightweight styling. `IsLoading` defaults to `true`, which is what you want for a loading slot; set it to `false` to stop the shimmer when the presenter stays in the tree but is hidden.

### Template resolution

The template the skeleton is derived from is, in order:

1. the presenter's own `ContentTemplate`;
2. otherwise, `utu:Skeleton.PlaceholderTemplate` of the nearest ancestor with `utu:Skeleton.IsEnabled="True"`;
3. otherwise, that ancestor's `ValueTemplate` (see [Automatic injection](#automatic-injection-skeletonisenabled)).

The presenter's `Content` (if any) becomes the template's data context. Leave it empty, or pass an object whose data is not loaded yet, so that bound values are empty and placeholders follow the [empty-value rules](#how-placeholders-are-generated).

### Placeholder rows for lists

A template typically contains a list whose items are not loaded yet. Every `ItemsControl` (including `ListView`/`GridView`) and `ItemsRepeater` in the instantiated template that has an `ItemTemplate` but no `ItemsSource` receives `utu:Skeleton.PlaceholderCount` dummy items, so the list realizes placeholder rows from its own `ItemTemplate`. This only ever affects the presenter's private copy of the template, never your live content.

`ItemTemplateSelector` is not used for placeholder rows: set `ItemTemplate`.

### Usage

```xml
<Page.Resources>
	<DataTemplate x:Key="PersonRowTemplate">
		<StackPanel Orientation="Horizontal" Spacing="12">
			<Ellipse Width="40" Height="40" Fill="{ThemeResource SystemControlHighlightAccentBrush}" />
			<StackPanel VerticalAlignment="Center" Spacing="4">
				<TextBlock Text="{Binding Name}" />
				<TextBlock Text="{Binding Title}" FontSize="12" />
			</StackPanel>
		</StackPanel>
	</DataTemplate>
</Page.Resources>

<utu:SkeletonPresenter utu:Skeleton.PlaceholderCount="3">
	<utu:SkeletonPresenter.ContentTemplate>
		<DataTemplate>
			<ListView ItemTemplate="{StaticResource PersonRowTemplate}" />
		</DataTemplate>
	</utu:SkeletonPresenter.ContentTemplate>
</utu:SkeletonPresenter>
```

### Inside a LoadingView

Reuse the same item template for both the loaded list and its loading skeleton:

```xml
<utu:LoadingView Source="{Binding LoadPeopleCommand}">
	<ListView ItemsSource="{Binding People}" ItemTemplate="{StaticResource PersonRowTemplate}" />

	<utu:LoadingView.LoadingContent>
		<utu:SkeletonPresenter utu:Skeleton.PlaceholderCount="3">
			<utu:SkeletonPresenter.ContentTemplate>
				<DataTemplate>
					<ListView ItemTemplate="{StaticResource PersonRowTemplate}" />
				</DataTemplate>
			</utu:SkeletonPresenter.ContentTemplate>
		</utu:SkeletonPresenter>
	</utu:LoadingView.LoadingContent>
</utu:LoadingView>
```

## Skeleton attached properties

The static `Skeleton` class provides attached properties that tune generation and enable automatic injection.

| Property | Type | Default | Set on | Description |
|----------|------|---------|--------|-------------|
| `Skeleton.Ignore` | `bool` | `false` | Any element inside the content | Excludes the element **and its subtree** from generation: no placeholder is produced for it, and it **stays visible** while loading. Use it for static parts that do not depend on the data being loaded, such as headers or labels. |
| `Skeleton.Shape` | `SkeletonShape` | `Auto` | Any element inside the content | Forces the placeholder shape. Any value other than `Auto` also makes the element a leaf: a single placeholder covers it and its subtree is not visited. Use it to collapse a complex element (e.g. an avatar made of an `Image` inside a `Border`) into one shape. |
| `Skeleton.PlaceholderCount` | `int` | `4` | A `SkeletonPresenter`, or any of its ancestors (typically the control using `Skeleton.IsEnabled`, or the control whose template hosts the presenter) | Number of placeholder rows stamped into empty lists of a template-derived skeleton. The value of the `Skeleton.IsEnabled` owner wins; otherwise the nearest element (the presenter itself first) that sets it. `0` disables stamping. |
| `Skeleton.PlaceholderTemplate` | `DataTemplate` | `null` | The control using `Skeleton.IsEnabled` | Template the injected skeleton is derived from, instead of the control's `ValueTemplate`. Use it when the value template is too complex, or when you want a simplified loading layout. |
| `Skeleton.IsEnabled` | `bool` | `false` | A control exposing a `ProgressTemplate` dependency property | Injects an auto-generated skeleton as the control's `ProgressTemplate`. See [Automatic injection](#automatic-injection-skeletonisenabled). |

### SkeletonShape

| Value | Placeholder |
|-------|-------------|
| `Auto` | Inferred: an `Ellipse` produces a circle, every other leaf a rounded rectangle. |
| `Rectangle` | A rectangle with `PlaceholderCornerRadius`, covering the element's bounds. |
| `Circle` | A circle whose diameter is the smaller of the element's width and height, centered in its bounds. |

### Overrides

```xml
<utu:SkeletonView IsLoading="{Binding IsBusy}">
	<StackPanel Spacing="12">
		<!-- static header: stays visible while loading -->
		<TextBlock utu:Skeleton.Ignore="True" Text="Profile" />

		<!-- one circle instead of the image inside -->
		<Border utu:Skeleton.Shape="Circle" Width="64" Height="64" CornerRadius="32">
			<Image Source="{Binding AvatarUrl}" Stretch="UniformToFill" />
		</Border>

		<!-- one block instead of every chip inside -->
		<ItemsControl utu:Skeleton.Shape="Rectangle" Height="32" ItemsSource="{Binding Tags}" />
	</StackPanel>
</utu:SkeletonView>
```

### Automatic injection (Skeleton.IsEnabled)

Some state-aware controls display a separate `ProgressTemplate` while loading and a `ValueTemplate` once loaded. Setting `utu:Skeleton.IsEnabled="True"` on such a control injects a `SkeletonPresenter` as its `ProgressTemplate`, so its loading state becomes a skeleton derived from its own `ValueTemplate`, with no skeleton markup at all:

```xml
<local:MyStateView Source="{Binding People}"
				   utu:Skeleton.IsEnabled="True"
				   utu:Skeleton.PlaceholderCount="5">
	<local:MyStateView.ValueTemplate>
		<DataTemplate>
			<ListView ItemsSource="{Binding Data}" ItemTemplate="{StaticResource PersonRowTemplate}" />
		</DataTemplate>
	</local:MyStateView.ValueTemplate>
</local:MyStateView>
```

To derive the skeleton from a different, simpler template:

```xml
<local:MyStateView utu:Skeleton.IsEnabled="True">
	<utu:Skeleton.PlaceholderTemplate>
		<DataTemplate>
			<StackPanel Spacing="8">
				<TextBlock Width="200" />
				<TextBlock Width="120" />
			</StackPanel>
		</DataTemplate>
	</utu:Skeleton.PlaceholderTemplate>
	<!-- ... -->
</local:MyStateView>
```

Requirements and behavior:

- The control must expose a public static `ProgressTemplateProperty` dependency property (field or property). `ValueTemplateProperty` is used as the default skeleton source. These are resolved by convention, using reflection, so the toolkit takes no dependency on the control's library.
- A `ProgressTemplate` you assign yourself is never overwritten, and setting `IsEnabled` back to `false` only removes the template it injected.
- The injected template is the `SkeletonDefaultProgressTemplate` resource from the Uno Toolkit resources, which must be merged in your application resources (as for any Uno Toolkit control).
- If the property cannot be found (unsupported control, or removed by trimming), nothing is injected and a warning is logged.
- Injection only replaces the loading template, so it covers the *initial* load. On MVUX's `FeedView` it works, but prefer [`SkeletonFeedViewStyle`](#feedview-uno-extensions), which also covers refreshes.

## FeedView (Uno Extensions)

MVUX's `FeedView` is supported through the `SkeletonFeedViewStyle` style provided by Uno.Extensions, which uses `SkeletonPresenter` for the initial load and `SkeletonView` for refreshes. See the [FeedView documentation](xref:Uno.Extensions.Mvux.FeedView#skeleton-loading) for how to apply it.

## Lightweight Styling

| Key                        | Type                  | Value                                                              |
|----------------------------|-----------------------|--------------------------------------------------------------------|
| `SkeletonViewBackground`   | `SolidColorBrush`     | `#E0E0E0` (Dark: `#3D3D3D`)                                        |
| `SkeletonViewShimmerBrush` | `LinearGradientBrush` | Horizontal transparent → `#F5F5F5` → transparent (Dark: `#4D4D4D`) |
| `SkeletonViewCornerRadius` | `CornerRadius`        | `4`                                                                |

These resources apply to both `SkeletonView` and `SkeletonPresenter`. Override them in your application resources to restyle every skeleton:

```xml
<ResourceDictionary.ThemeDictionaries>
	<ResourceDictionary x:Key="Light">
		<SolidColorBrush x:Key="SkeletonViewBackground" Color="#E8EEF6" />
	</ResourceDictionary>
	<ResourceDictionary x:Key="Dark">
		<SolidColorBrush x:Key="SkeletonViewBackground" Color="#2B3442" />
	</ResourceDictionary>
</ResourceDictionary.ThemeDictionaries>
```

## Limitations and troubleshooting

- **Nothing is shown while loading.** The content has no leaves yet, or they are collapsed: e.g. a `ListView` with no items inside a `SkeletonView`. A `SkeletonView` only mirrors what is laid out; use a `SkeletonPresenter` (template-derived, with placeholder rows) for content that does not exist yet.
- **A placeholder covers a whole area.** An element in the content is a `Shape` or button spanning that area (e.g. a decorative background `Rectangle`). Mark it with `utu:Skeleton.Ignore="True"` to keep it visible as-is instead.
- **Text placeholders are full width.** An empty, horizontally stretched `TextBlock` falls back to its layout slot. Set `HorizontalAlignment="Left"` with a `MinWidth`, or a `Width`, for a shorter line.
- **An element's `Opacity` is changed while loading.** Covered elements are hidden through `Opacity`, overriding values the app sets or animates on them during that time; the original value or binding is restored once loaded. Avoid driving `Opacity` of covered elements while loading.
- **The skeleton always shows.** `IsLoading` defaults to `true`; bind it or set `Source`.
- **Elements outside the control are not covered.** Placeholders are confined to the `SkeletonView`; wrap each area that loads.
- **Virtualized lists** only produce placeholders for realized items, which is usually what fills the viewport.

## Samples

See the `SkeletonView` page of the Uno Toolkit sample app for a live demo of `SkeletonView` and `SkeletonPresenter`, including generation overrides, appearance and `LoadingView` integration.
