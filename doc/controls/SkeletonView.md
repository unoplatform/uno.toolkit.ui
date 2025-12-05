---
uid: Toolkit.Controls.SkeletonView
---

# SkeletonView

Represents a skeleton loading placeholder control that displays animated placeholder shapes while content is loading. The shimmer animation provides visual feedback that content is being loaded.

## Overview

`SkeletonView` is designed to improve perceived performance by showing placeholder UI that mimics the structure of the actual content being loaded. It can be used:

- As a standalone loading indicator with its own `IsActive` property
- As `LoadingContent` within a [LoadingView](xref:Toolkit.Controls.LoadingView)

The control supports an optional shimmer animation that creates a sweeping highlight effect across the placeholder content.

## Properties

| Property             | Type       | Description                                                                                              |
|----------------------|------------|----------------------------------------------------------------------------------------------------------|
| `IsActive`           | `bool`     | Gets or sets whether the skeleton view is active (showing placeholders). Default is `true`.              |
| `EnableShimmer`      | `bool`     | Gets or sets whether the shimmer animation is enabled. Default is `true`.                                |
| `ShimmerDuration`    | `Duration` | Gets or sets the duration of one shimmer animation cycle. Default is 1.5 seconds.                        |
| `SkeletonBackground` | `Brush`    | Gets or sets the background brush for skeleton placeholder shapes.                                       |
| `ShimmerBrush`       | `Brush`    | Gets or sets the brush used for the shimmer highlight effect.                                            |

## Skeleton Element

The `Skeleton` control represents a single placeholder shape that can be composed within a `SkeletonView`. Multiple `Skeleton` elements can be arranged to match the layout of the content being loaded.

### Skeleton Properties

| Property             | Type            | Description                                                                           |
|----------------------|-----------------|---------------------------------------------------------------------------------------|
| `Shape`              | `SkeletonShape` | Gets or sets the shape type (Rectangle, Circle, Text). Default is `Rectangle`.        |
| `SkeletonBackground` | `Brush`         | Gets or sets the background brush for this element. Defaults to theme resource.       |

### Skeleton Shapes

| Shape       | Description                                  |
|-------------|----------------------------------------------|
| `Rectangle` | A rectangular shape with optional corner radius |
| `Circle`    | A circular/elliptical shape                  |
| `Text`      | A text line placeholder                      |

## Pre-built Skeleton Styles

The toolkit provides several pre-built styles for common skeleton patterns:

| Style Name          | Description                                |
|---------------------|--------------------------------------------|
| `TextSkeleton`      | A single line of text placeholder (16px)   |
| `TitleSkeleton`     | A title/header placeholder (24px, 200px wide) |
| `SubtitleSkeleton`  | A subtitle placeholder (20px, 150px wide)  |
| `CircleSkeleton`    | A circular shape (48x48px)                 |
| `AvatarSkeleton`    | A small avatar placeholder (40x40px)       |
| `LargeAvatarSkeleton` | A large avatar placeholder (64x64px)     |
| `ImageSkeleton`     | An image/card placeholder (120px height)   |
| `ButtonSkeleton`    | A button placeholder (100x36px)            |

## Lightweight Styling Resources

| Resource Key                  | Type     | Description                                      |
|-------------------------------|----------|--------------------------------------------------|
| `SkeletonViewBackground`      | `Brush`  | Default background color for skeleton shapes     |
| `SkeletonViewShimmerBrush`    | `Brush`  | Color of the shimmer highlight effect            |
| `SkeletonViewShimmerDuration` | `String` | Duration of shimmer animation (TimeSpan format)  |

## Usage

### Standalone Usage

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<utu:SkeletonView IsActive="{Binding IsLoading}">
    <StackPanel Spacing="12">
        <!-- Profile header skeleton -->
        <StackPanel Orientation="Horizontal" Spacing="12">
            <utu:Skeleton Style="{StaticResource AvatarSkeleton}" />
            <StackPanel Spacing="8" VerticalAlignment="Center">
                <utu:Skeleton Style="{StaticResource TitleSkeleton}" />
                <utu:Skeleton Style="{StaticResource SubtitleSkeleton}" Width="100" />
            </StackPanel>
        </StackPanel>

        <!-- Content skeleton -->
        <utu:Skeleton Style="{StaticResource ImageSkeleton}" />
        
        <!-- Text lines skeleton -->
        <utu:Skeleton Style="{StaticResource TextSkeleton}" />
        <utu:Skeleton Style="{StaticResource TextSkeleton}" Width="80%" />
        <utu:Skeleton Style="{StaticResource TextSkeleton}" Width="60%" />
    </StackPanel>
</utu:SkeletonView>
```

### With LoadingView

The `SkeletonView` integrates seamlessly with `LoadingView`:

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<utu:LoadingView Source="{Binding FetchDataCommand}">
    <!-- Actual content -->
    <StackPanel>
        <PersonView DataContext="{Binding Person}" />
        <ListView ItemsSource="{Binding Items}" />
    </StackPanel>

    <!-- Skeleton placeholder as LoadingContent -->
    <utu:LoadingView.LoadingContent>
        <utu:SkeletonView>
            <StackPanel Spacing="16" Padding="16">
                <!-- Person skeleton -->
                <StackPanel Orientation="Horizontal" Spacing="12">
                    <utu:Skeleton Style="{StaticResource LargeAvatarSkeleton}" />
                    <StackPanel Spacing="8" VerticalAlignment="Center">
                        <utu:Skeleton Style="{StaticResource TitleSkeleton}" />
                        <utu:Skeleton Style="{StaticResource TextSkeleton}" Width="120" />
                    </StackPanel>
                </StackPanel>

                <!-- List items skeleton -->
                <utu:Skeleton Style="{StaticResource ImageSkeleton}" Height="80" />
                <utu:Skeleton Style="{StaticResource ImageSkeleton}" Height="80" />
                <utu:Skeleton Style="{StaticResource ImageSkeleton}" Height="80" />
            </StackPanel>
        </utu:SkeletonView>
    </utu:LoadingView.LoadingContent>
</utu:LoadingView>
```

### Without Shimmer Animation

```xml
<utu:SkeletonView IsActive="{Binding IsLoading}" EnableShimmer="False">
    <StackPanel Spacing="8">
        <utu:Skeleton Style="{StaticResource TextSkeleton}" />
        <utu:Skeleton Style="{StaticResource TextSkeleton}" />
    </StackPanel>
</utu:SkeletonView>
```

### Custom Shimmer Duration

```xml
<utu:SkeletonView IsActive="{Binding IsLoading}" ShimmerDuration="0:0:2">
    <StackPanel Spacing="8">
        <utu:Skeleton Style="{StaticResource TextSkeleton}" />
        <utu:Skeleton Style="{StaticResource TextSkeleton}" />
    </StackPanel>
</utu:SkeletonView>
```

### Custom Skeleton Shapes

Create custom skeleton layouts by combining `Skeleton` elements:

```xml
<utu:SkeletonView IsActive="{Binding IsLoading}">
    <!-- Card skeleton -->
    <Border CornerRadius="12" Padding="16" BorderThickness="1"
            BorderBrush="{ThemeResource SystemControlForegroundBaseLowBrush}">
        <StackPanel Spacing="12">
            <utu:Skeleton Height="180" CornerRadius="8" />
            <utu:Skeleton Style="{StaticResource TitleSkeleton}" />
            <StackPanel Spacing="4">
                <utu:Skeleton Style="{StaticResource TextSkeleton}" />
                <utu:Skeleton Style="{StaticResource TextSkeleton}" Width="70%" />
            </StackPanel>
            <StackPanel Orientation="Horizontal" Spacing="8">
                <utu:Skeleton Style="{StaticResource ButtonSkeleton}" />
                <utu:Skeleton Style="{StaticResource ButtonSkeleton}" />
            </StackPanel>
        </StackPanel>
    </Border>
</utu:SkeletonView>
```

## Theming

The skeleton colors automatically adapt to light and dark themes:

| Theme | Background | Shimmer Highlight |
|-------|------------|-------------------|
| Light | `#E0E0E0`  | `#F5F5F5`         |
| Dark  | `#3D3D3D`  | `#4D4D4D`         |

Override theme colors by defining these resources in your app:

```xml
<ResourceDictionary.ThemeDictionaries>
    <ResourceDictionary x:Key="Light">
        <SolidColorBrush x:Key="SkeletonViewBackground" Color="#E8E8E8" />
        <SolidColorBrush x:Key="SkeletonViewShimmerBrush" Color="#FFFFFF" />
    </ResourceDictionary>
    <ResourceDictionary x:Key="Dark">
        <SolidColorBrush x:Key="SkeletonViewBackground" Color="#404040" />
        <SolidColorBrush x:Key="SkeletonViewShimmerBrush" Color="#505050" />
    </ResourceDictionary>
</ResourceDictionary.ThemeDictionaries>
```

## Best Practices

1. **Match the actual layout**: Design skeleton layouts that closely match the structure of the content being loaded to minimize visual shift when content appears.

2. **Use appropriate shapes**: Use `CircleSkeleton` for avatars, `TextSkeleton` for text lines, and `ImageSkeleton` for media content.

3. **Vary widths for text**: When creating multiple text line skeletons, vary their widths (100%, 80%, 60%) to create a more natural appearance.

4. **Consider performance**: For complex layouts with many skeleton elements, consider disabling shimmer animation (`EnableShimmer="False"`) to reduce rendering overhead.

5. **Combine with LoadingView**: Use `SkeletonView` as `LoadingContent` within `LoadingView` for automatic state management based on your data loading operations.

## See Also

- [LoadingView](xref:Toolkit.Controls.LoadingView)
