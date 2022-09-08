# SafeArea

`SafeArea` is a specialized control that overrides the `Padding` or `Margin` properties of its child/attached control to ensure that its inner content is always within the [`ApplicationView.VisibleBounds`](https://docs.microsoft.com/en-us/uwp/api/windows.ui.viewmanagement.applicationview.visiblebounds?view=winrt-22621) rectangle.

The `ApplicationView.VisibleBounds` is the rectangular area of the screen which is completely unobscured by any window decoration, such as the status bar, rounded screen corners, or any type of screen notch.

The `SafeArea` can also be used to ensure that the specified control will never be hidden by any sort of soft-input panel, such as the on-screen keyboard. This is done by observing the state of the keyboard and treating the area that it occupies when open as part of the "unsafe" area of the screen.

In some cases, it is acceptable for visible content to be partially obscured (a page background for example) and it should extend to fill the entire window. Other types of content should be restricted to the visible bounds (for instance: readable text, or interactive controls). `SafeArea` enables this kind of fine-grained control over responsiveness to the safe and "unsafe" areas of the screen.

## Properties

### Remarks

All of the `SafeArea` properties can be used both as a dependency property or as an attached property, much like the `ScrollViewer` properties:

```xml
xmlns:utu="using:Uno.Toolkit.UI"

<Grid utu:SafeArea.Insets="Left,Top,Right,Bottom">
 <!-- Content -->
</Grid>

<!-- and/or -->

<SafeArea Insets="Left,Top,Right,Bottom">
 <!-- Content -->
</SafeArea>
```

Property|Type|Description
-|-|-
Insets|`InsetMask`|Gets or sets the specific bound(s) of the "safe" area that you want to be considered when `SafeArea` attempts to apply the Padding or Margin. Defaults to `InsetMask.None`.
Mode|`InsetMode`|Gets or sets whether the `SafeArea` insets will be applied to the control's `Margin` or its `Padding`. Defaults to `InsetMode.Padding`.

## Usage

### Using `SafeArea.Insets`

The `InsetMask` enum can represent a single bound or it can be combined with multiple values as shown in the above example XAML. `InsetMask` has the following available values:

 - `Left`
 - `Top`
 - `Right`
 - `Bottom`
 - `SoftInput`
 - `VisibleBounds = Left | Top | Right | Bottom`
 - `All = VisibleBounds | SoftInput`

### Using `InsetMask.SoftInput` for on-screen keyboards

The `InsetMask.SoftInput` value is used to ensure that the specified control will never be obstructed by any sort of soft-input panel that may appear, such as the on-screen keyboard on touch devices. Currently, `SafeArea` is built with the assumption that the soft input panel would appear at the bottom of the screen, therefore, the soft input inset will be applied to the Bottom value of the control's `Margin` or `Padding` property.

> [!WARNING]
> Special care must be taken when using `InsetMask.SoftInput` for Android applications. Combining `SafeArea`'s `SoftInput` logic within an Activity whose [`WindowSoftInputMode`](https://developer.android.com/guide/topics/manifest/activity-element#wsoft) is set to `adjustResize` or `adjustPan` may result in undesired behavior, especially when working with text entry controls such as `TextBox` or `PasswordBox`. It is possible to set the `WindowSoftInputMode` to `adjustNothing`. More information on Android specific keyboard behaviors can be found [here](https://developer.android.com/develop/ui/views/touch-and-input/keyboard-input/visibility).

### Using `InsetMode.Padding` versus `InsetMode.Margin`

The default `Mode` for `SafeArea` is set to `InsetMode.Padding`. Using the Padding property as the `SafeArea` inset ensures that your control's content will never be obscured by the "unsafe" area but still allows things like the control's Background color to "bleed" into the unsafe area. To highlight this feature, refer to the example below.

#### Example
Here we are using the Toolkit's [`TabBar`](TabBarAndTabBarItem.md) with both the `TopTabBarStyle` and the `BottomTabBarStyle`. Both controls have their `Background`s set to `Purple`. Note the differences within the unsafe areas of the screen between the `Padding` mode and the `Margin` mode.

> [!NOTE]
> The `BottomTabBarStyle` uses `SafeArea` and has the `Insets` property set to `Bottom` by default. This is removed from the style for the purpose of the demontration below.

Given the following XAML, we can see what SafeArea is doing and what the differences are between an `InsetMode` of `Padding` versus `Margin`.

```xml
<Page xmlns:utu="using:Uno.Toolkit.UI" ...>
	<Grid>
		<Grid.RowDefinitions>
			<RowDefinition Height="Auto" />
			<RowDefinition Height="*" />
			<RowDefinition Height="Auto" />
		</Grid.RowDefinitions>
		<utu:TabBar Background="Purple">
			<utu:TabBar.Items>
				<utu:TabBarItem Foreground="White"
								Content="Home" />
				<utu:TabBarItem Foreground="White"
								Content="Search" />
				<utu:TabBarItem Foreground="White"
								Content="Support" />
				<utu:TabBarItem Foreground="White"
								Content="About" />
			</utu:TabBar.Items>
		</utu:TabBar>

		<TextBlock Text="Page Content"
				   FontSize="30"
				   Grid.Row="1"
				   VerticalAlignment="Center"
				   HorizontalAlignment="Center" />

		<utu:TabBar Grid.Row="2"
					Background="Purple">
			<utu:TabBar.Items>
				<utu:TabBarItem Foreground="White"
								Content="Home">
					<utu:TabBarItem.Icon>
						<FontIcon Foreground="White"
								  Glyph="&#xE80F;" />
					</utu:TabBarItem.Icon>
				</utu:TabBarItem>
				<utu:TabBarItem Foreground="White"
								Content="Search">
					<utu:TabBarItem.Icon>
						<FontIcon Foreground="White"
								  Glyph="&#xe721;" />
					</utu:TabBarItem.Icon>
				</utu:TabBarItem>
				<utu:TabBarItem Foreground="White"
								Content="Support">
					<utu:TabBarItem.Icon>
						<FontIcon Foreground="White"
								  Glyph="&#xE8F2;" />
					</utu:TabBarItem.Icon>
				</utu:TabBarItem>
				<utu:TabBarItem Foreground="White"
								Content="About">
					<utu:TabBarItem.Icon>
						<FontIcon Foreground="White"
								  Glyph="&#xE946;" />
					</utu:TabBarItem.Icon>
				</utu:TabBarItem>
			</utu:TabBar.Items>
		</utu:TabBar>
	</Grid>
</Page>
```

# [**Without SafeArea**](#tab/none)

![safearea_without_padding_alpha](../assets/safearea_without_padding.png)

# [**Padding (default)**](#tab/padding)

Top TabBar:

```diff
<utu:TabBar Background="Purple"
+           utu:SafeArea.Insets="Top">
```

Bottom TabBar:

```diff
<utu:TabBar Grid.Row="2"
+           utu:SafeArea.Insets="Bottom"
            Background="Purple">
```

![safearea_with_padding_alpha](../assets/safearea_with_padding.png)

# [**Margin**](#tab/margin)

Top TabBar:

```diff
<utu:TabBar Background="Purple"
+           utu:SafeArea.Insets="Top"
+           utu:SafeArea.Mode="Margin">
```

Bottom TabBar:

```diff
<utu:TabBar Grid.Row="2"
+           utu:SafeArea.Insets="Bottom"
+           utu:SafeArea.Mode="Margin"
            Background="Purple">
```

![safearea_with_margin_alpha](../assets/safearea_with_margin.png)

***

## Notes

- `SafeArea` is able to adapt to views that are only _partially_ obscured by applying the minimum amount of `Padding`/`Margin` needed until the content is fully inside the visible bounds.
- When a control already has a non-zero `Padding`/`Margin`, `SafeArea` takes those values into consideration when calculating the minimum amount of pixels needed for the view to be within the safe area.
- `SafeArea` on WinAppSDK/Desktop does not have any effect. It is present to allow for same-XAML across platforms.