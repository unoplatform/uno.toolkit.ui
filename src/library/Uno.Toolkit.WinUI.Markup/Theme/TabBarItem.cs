using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Uno.Extensions.Markup;
using Uno.Extensions.Markup.Internals;

namespace Uno.Toolkit.UI.Markup;

public static partial class Theme
{
	public static partial class TabBarItem
	{
		public static partial class Resources
		{
			public static partial class Default
			{
				public static partial class Background
				{
					[ResourceKeyDefinition(typeof(Brush), "TabBarItemBackground")]
					public static ThemeResourceKey<Brush> Default => new("TabBarItemBackground");

					[ResourceKeyDefinition(typeof(Brush), "TabBarItemBackgroundPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("TabBarItemBackgroundPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "TabBarItemBackgroundPressed")]
					public static ThemeResourceKey<Brush> Pressed => new("TabBarItemBackgroundPressed");

					[ResourceKeyDefinition(typeof(Brush), "TabBarItemBackgroundSelected")]
					public static ThemeResourceKey<Brush> Selected => new("TabBarItemBackgroundSelected");

					[ResourceKeyDefinition(typeof(Brush), "TabBarItemBackgroundSelectedPointerOver")]
					public static ThemeResourceKey<Brush> SelectedPointerOver => new("TabBarItemBackgroundSelectedPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "TabBarItemBackgroundSelectedPressed")]
					public static ThemeResourceKey<Brush> SelectedPressed => new("TabBarItemBackgroundSelectedPressed");
				}

				public static partial class Foreground
				{
					[ResourceKeyDefinition(typeof(Brush), "TabBarItemForeground")]
					public static ThemeResourceKey<Brush> Default => new("TabBarItemForeground");

					[ResourceKeyDefinition(typeof(Brush), "TabBarItemForegroundPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("TabBarItemForegroundPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "TabBarItemForegroundPressed")]
					public static ThemeResourceKey<Brush> Pressed => new("TabBarItemForegroundPressed");

					[ResourceKeyDefinition(typeof(Brush), "TabBarItemForegroundDisabled")]
					public static ThemeResourceKey<Brush> Disabled => new("TabBarItemForegroundDisabled");

					[ResourceKeyDefinition(typeof(Brush), "TabBarItemForegroundSelected")]
					public static ThemeResourceKey<Brush> Selected => new("TabBarItemForegroundSelected");

					[ResourceKeyDefinition(typeof(Brush), "TabBarItemForegroundSelectedPointerOver")]
					public static ThemeResourceKey<Brush> SelectedPointerOver => new("TabBarItemForegroundSelectedPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "TabBarItemForegroundSelectedPressed")]
					public static ThemeResourceKey<Brush> SelectedPressed => new("TabBarItemForegroundSelectedPressed");
				}

				public static partial class BorderBrush
				{
					[ResourceKeyDefinition(typeof(Brush), "TabBarItemBorderBrush")]
					public static ThemeResourceKey<Brush> Default => new("TabBarItemBorderBrush");
				}
			}

			public static partial class Navigation
			{
				public static partial class Background
				{
					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarBackground")]
					public static ThemeResourceKey<Brush> Default => new("NavigationTabBarBackground");

					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarBackgroundPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("NavigationTabBarBackgroundPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarBackgroundFocused")]
					public static ThemeResourceKey<Brush> Focused => new("NavigationTabBarBackgroundFocused");

					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarBackgroundPressed")]
					public static ThemeResourceKey<Brush> Pressed => new("NavigationTabBarBackgroundPressed");

					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarBackgroundSelected")]
					public static ThemeResourceKey<Brush> Selected => new("NavigationTabBarBackgroundSelected");

					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarBackgroundSelectedPointerOver")]
					public static ThemeResourceKey<Brush> SelectedPointerOver => new("NavigationTabBarBackgroundSelectedPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarBackgroundSelectedPressed")]
					public static ThemeResourceKey<Brush> SelectedPressed => new("NavigationTabBarBackgroundSelectedPressed");
				}

				public static partial class Foreground
				{
					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarForeground")]
					public static ThemeResourceKey<Brush> Default => new("NavigationTabBarForeground");

					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarForegroundPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("NavigationTabBarForegroundPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarForegroundPressed")]
					public static ThemeResourceKey<Brush> Focused => new("NavigationTabBarForegroundPressed");

					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarForegroundDisabled")]
					public static ThemeResourceKey<Brush> Pressed => new("NavigationTabBarForegroundDisabled");

					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarForegroundSelected")]
					public static ThemeResourceKey<Brush> Selected => new("NavigationTabBarForegroundSelected");

					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarForegroundSelectedPointerOver")]
					public static ThemeResourceKey<Brush> SelectedPointerOver => new("NavigationTabBarForegroundSelectedPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarForegroundSelectedPressed")]
					public static ThemeResourceKey<Brush> SelectedPressed => new("NavigationTabBarForegroundSelectedPressed");
				}

				public static partial class BorderBrush
				{
					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarBorderBrush")]
					public static ThemeResourceKey<Brush> Default => new("NavigationTabBarBorderBrush");
				}

				public static partial class IconForeground
				{
					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarItemIconForeground")]
					public static ThemeResourceKey<Brush> Default => new("NavigationTabBarItemIconForeground");

					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarItemIconForegroundPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("NavigationTabBarItemIconForegroundPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarItemIconForegroundPressed")]
					public static ThemeResourceKey<Brush> Pressed => new("NavigationTabBarItemIconForegroundPressed");

					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarItemIconForegroundDisabled")]
					public static ThemeResourceKey<Brush> Disabled => new("NavigationTabBarItemIconForegroundDisabled");

					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarItemIconForegroundSelected")]
					public static ThemeResourceKey<Brush> Selected => new("NavigationTabBarItemIconForegroundSelected");

					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarItemIconForegroundSelectedPointerOver")]
					public static ThemeResourceKey<Brush> SelectedPointerOver => new("NavigationTabBarItemIconForegroundSelectedPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarItemIconForegroundSelectedPressed")]
					public static ThemeResourceKey<Brush> SelectedPressed => new("NavigationTabBarItemIconForegroundSelectedPressed");
				}

				public static partial class ActiveIndicator
				{
					public static partial class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarItemActiveIndicatorBackground")]
						public static ThemeResourceKey<Brush> Default => new("NavigationTabBarItemActiveIndicatorBackground");

						[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarItemActiveIndicatorBackgroundPointerOver")]
						public static ThemeResourceKey<Brush> PointerOver => new("NavigationTabBarItemActiveIndicatorBackgroundPointerOver");

						[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarItemActiveIndicatorBackgroundPressed")]
						public static ThemeResourceKey<Brush> Pressed => new("NavigationTabBarItemActiveIndicatorBackgroundPressed");

						[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarItemActiveIndicatorBackgroundDisabled")]
						public static ThemeResourceKey<Brush> Disabled => new("NavigationTabBarItemActiveIndicatorBackgroundDisabled");

						[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarItemActiveIndicatorBackgroundSelected")]
						public static ThemeResourceKey<Brush> Selected => new("NavigationTabBarItemActiveIndicatorBackgroundSelected");

						[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarItemActiveIndicatorBackgroundSelectedPointerOver")]
						public static ThemeResourceKey<Brush> SelectedPointerOver => new("NavigationTabBarItemActiveIndicatorBackgroundSelectedPointerOver");

						[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarItemActiveIndicatorBackgroundSelectedPressed")]
						public static ThemeResourceKey<Brush> SelectedPressed => new("NavigationTabBarItemActiveIndicatorBackgroundSelectedPressed");
					}

					[ResourceKeyDefinition(typeof(double), "NavigationTabBarItemActiveIndicatorWidth")]
					public static ThemeResourceKey<double> Width => new("NavigationTabBarItemActiveIndicatorWidth");

					[ResourceKeyDefinition(typeof(double), "NavigationTabBarItemActiveIndicatorHeight")]
					public static ThemeResourceKey<double> Height => new("NavigationTabBarItemActiveIndicatorHeight");

					[ResourceKeyDefinition(typeof(double), "NavigationTabBarItemActiveIndicatorCornerRadius")]
					public static ThemeResourceKey<double> CornerRadius => new("NavigationTabBarItemActiveIndicatorCornerRadius");
				}

				public static partial class RippleFeedback
				{
					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarItemRippleFeedback")]
					public static ThemeResourceKey<Brush> Default => new("NavigationTabBarItemRippleFeedback");
				}

				[ResourceKeyDefinition(typeof(double), "NavigationTabBarItemIconHeight")]
				public static ThemeResourceKey<double> IconHeight => new("NavigationTabBarItemIconHeight");

				[ResourceKeyDefinition(typeof(Thickness), "NavigationTabBarItemPadding")]
				public static ThemeResourceKey<Thickness> Padding => new("NavigationTabBarItemPadding");
			}

			public static partial class Top
			{
				public static partial class Background
				{
					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemBackground")]
					public static ThemeResourceKey<Brush> Default => new("TopTabBarItemBackground");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemBackgroundPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("TopTabBarItemBackgroundPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemBackgroundFocused")]
					public static ThemeResourceKey<Brush> Focused => new("TopTabBarItemBackgroundFocused");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemBackgroundPressed")]
					public static ThemeResourceKey<Brush> Pressed => new("TopTabBarItemBackgroundPressed");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemBackgroundDisabled")]
					public static ThemeResourceKey<Brush> Disabled => new("TopTabBarItemBackgroundDisabled");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemBackgroundSelected")]
					public static ThemeResourceKey<Brush> Selected => new("TopTabBarItemBackgroundSelected");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemBackgroundSelectedPointerOver")]
					public static ThemeResourceKey<Brush> SelectedPointerOver => new("TopTabBarItemBackgroundSelectedPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemBackgroundSelectedPressed")]
					public static ThemeResourceKey<Brush> SelectedPressed => new("TopTabBarItemBackgroundSelectedPressed");
				}

				public static partial class Foreground
				{
					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemForeground")]
					public static ThemeResourceKey<Brush> Default => new("TopTabBarItemForeground");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemForegroundPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("TopTabBarItemForegroundPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemForegroundFocused")]
					public static ThemeResourceKey<Brush> Focused => new("TopTabBarItemForegroundFocused");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemForegroundPressed")]
					public static ThemeResourceKey<Brush> Pressed => new("TopTabBarItemForegroundPressed");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemForegroundDisabled")]
					public static ThemeResourceKey<Brush> Disabled => new("TopTabBarItemForegroundDisabled");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemForegroundSelected")]
					public static ThemeResourceKey<Brush> Selected => new("TopTabBarItemForegroundSelected");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemForegroundSelectedPointerOver")]
					public static ThemeResourceKey<Brush> SelectedPointerOver => new("TopTabBarItemForegroundSelectedPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemForegroundSelectedPressed")]
					public static ThemeResourceKey<Brush> SelectedPressed => new("TopTabBarItemForegroundSelectedPressed");
				}

				public static partial class IconForeground
				{
					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemIconForeground")]
					public static ThemeResourceKey<Brush> Default => new("TopTabBarItemIconForeground");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemIconForegroundPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("TopTabBarItemIconForegroundPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemIconForegroundFocused")]
					public static ThemeResourceKey<Brush> Focused => new("TopTabBarItemIconForegroundFocused");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemIconForegroundPressed")]
					public static ThemeResourceKey<Brush> Pressed => new("TopTabBarItemIconForegroundPressed");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemIconForegroundDisabled")]
					public static ThemeResourceKey<Brush> Disabled => new("TopTabBarItemIconForegroundDisabled");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemIconForegroundSelected")]
					public static ThemeResourceKey<Brush> Selected => new("TopTabBarItemIconForegroundSelected");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemIconForegroundSelectedPointerOver")]
					public static ThemeResourceKey<Brush> SelectedPointerOver => new("TopTabBarItemIconForegroundSelectedPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemIconForegroundSelectedPressed")]
					public static ThemeResourceKey<Brush> SelectedPressed => new("TopTabBarItemIconForegroundSelectedPressed");
				}

				public static partial class PointerFill
				{
					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemPointerFillBrush")]
					public static ThemeResourceKey<Brush> Default => new("TopTabBarItemPointerFillBrush");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemPointerFillBrushPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("TopTabBarItemPointerFillBrushPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemPointerFillBrushFocused")]
					public static ThemeResourceKey<Brush> Focused => new("TopTabBarItemPointerFillBrushFocused");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemPointerFillBrushPressed")]
					public static ThemeResourceKey<Brush> Pressed => new("TopTabBarItemPointerFillBrushPressed");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemPointerFillBrushDisabled")]
					public static ThemeResourceKey<Brush> Disabled => new("TopTabBarItemPointerFillBrushDisabled");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemPointerFillBrushSelected")]
					public static ThemeResourceKey<Brush> Selected => new("TopTabBarItemPointerFillBrushSelected");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemPointerFillBrushSelectedPointerOver")]
					public static ThemeResourceKey<Brush> SelectedPointerOver => new("TopTabBarItemPointerFillBrushSelectedPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemPointerFillBrushSelectedPressed")]
					public static ThemeResourceKey<Brush> SelectedPressed => new("TopTabBarItemPointerFillBrushSelectedPressed");
				}

				public static partial class BorderBrush
				{
					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemBorderBrush")]
					public static ThemeResourceKey<Brush> Default => new("TopTabBarItemBorderBrush");
				}

				public static partial class RippleFeedback
				{
					[ResourceKeyDefinition(typeof(Brush), "TopTabBarItemRippleFeedback")]
					public static ThemeResourceKey<Brush> Default => new("TopTabBarItemRippleFeedback");
				}

				public static partial class Colored
				{
					public static partial class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemBackground")]
						public static ThemeResourceKey<Brush> Default => new("ColoredTopTabBarItemBackground");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemBackgroundPointerOver")]
						public static ThemeResourceKey<Brush> PointerOver => new("ColoredTopTabBarItemBackgroundPointerOver");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemBackgroundFocused")]
						public static ThemeResourceKey<Brush> Focused => new("ColoredTopTabBarItemBackgroundFocused");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemBackgroundPressed")]
						public static ThemeResourceKey<Brush> Pressed => new("ColoredTopTabBarItemBackgroundPressed");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemBackgroundDisabled")]
						public static ThemeResourceKey<Brush> Disabled => new("ColoredTopTabBarItemBackgroundDisabled");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemBackgroundSelected")]
						public static ThemeResourceKey<Brush> Selected => new("ColoredTopTabBarItemBackgroundSelected");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemBackgroundSelectedPointerOver")]
						public static ThemeResourceKey<Brush> SelectedPointerOver => new("ColoredTopTabBarItemBackgroundSelectedPointerOver");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemBackgroundSelectedPressed")]
						public static ThemeResourceKey<Brush> SelectedPressed => new("ColoredTopTabBarItemBackgroundSelectedPressed");
					}

					public static partial class Foreground
					{
						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemForeground")]
						public static ThemeResourceKey<Brush> Default => new("ColoredTopTabBarItemForeground");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemForegroundPointerOver")]
						public static ThemeResourceKey<Brush> PointerOver => new("ColoredTopTabBarItemForegroundPointerOver");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemForegroundFocused")]
						public static ThemeResourceKey<Brush> Focused => new("ColoredTopTabBarItemForegroundFocused");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemForegroundPressed")]
						public static ThemeResourceKey<Brush> Pressed => new("ColoredTopTabBarItemForegroundPressed");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemForegroundDisabled")]
						public static ThemeResourceKey<Brush> Disabled => new("ColoredTopTabBarItemForegroundDisabled");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemForegroundSelected")]
						public static ThemeResourceKey<Brush> Selected => new("ColoredTopTabBarItemForegroundSelected");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemForegroundSelectedPointerOver")]
						public static ThemeResourceKey<Brush> SelectedPointerOver => new("ColoredTopTabBarItemForegroundSelectedPointerOver");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemForegroundSelectedPressed")]
						public static ThemeResourceKey<Brush> SelectedPressed => new("ColoredTopTabBarItemForegroundSelectedPressed");
					}

					public static partial class IconForeground
					{
						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemIconForeground")]
						public static ThemeResourceKey<Brush> Default => new("ColoredTopTabBarItemIconForeground");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemIconForegroundPointerOver")]
						public static ThemeResourceKey<Brush> PointerOver => new("ColoredTopTabBarItemIconForegroundPointerOver");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemIconForegroundFocused")]
						public static ThemeResourceKey<Brush> Focused => new("ColoredTopTabBarItemIconForegroundFocused");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemIconForegroundPressed")]
						public static ThemeResourceKey<Brush> Pressed => new("ColoredTopTabBarItemIconForegroundPressed");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemIconForegroundDisabled")]
						public static ThemeResourceKey<Brush> Disabled => new("ColoredTopTabBarItemIconForegroundDisabled");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemIconForegroundSelected")]
						public static ThemeResourceKey<Brush> Selected => new("ColoredTopTabBarItemIconForegroundSelected");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemIconForegroundSelectedPointerOver")]
						public static ThemeResourceKey<Brush> SelectedPointerOver => new("ColoredTopTabBarItemIconForegroundSelectedPointerOver");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemIconForegroundSelectedPressed")]
						public static ThemeResourceKey<Brush> SelectedPressed => new("ColoredTopTabBarItemIconForegroundSelectedPressed");
					}

					public static partial class PointerFill
					{
						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemPointerFillBrush")]
						public static ThemeResourceKey<Brush> Default => new("ColoredTopTabBarItemPointerFillBrush");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemPointerFillBrushPointerOver")]
						public static ThemeResourceKey<Brush> PointerOver => new("ColoredTopTabBarItemPointerFillBrushPointerOver");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemPointerFillBrushFocused")]
						public static ThemeResourceKey<Brush> Focused => new("ColoredTopTabBarItemPointerFillBrushFocused");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemPointerFillBrushPressed")]
						public static ThemeResourceKey<Brush> Pressed => new("ColoredTopTabBarItemPointerFillBrushPressed");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemPointerFillBrushDisabled")]
						public static ThemeResourceKey<Brush> Disabled => new("ColoredTopTabBarItemPointerFillBrushDisabled");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemPointerFillBrushSelected")]
						public static ThemeResourceKey<Brush> Selected => new("ColoredTopTabBarItemPointerFillBrushSelected");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemPointerFillBrushSelectedPointerOver")]
						public static ThemeResourceKey<Brush> SelectedPointerOver => new("ColoredTopTabBarItemPointerFillBrushSelectedPointerOver");

						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemPointerFillBrushSelectedPressed")]
						public static ThemeResourceKey<Brush> SelectedPressed => new("ColoredTopTabBarItemPointerFillBrushSelectedPressed");
					}

					public static partial class RippleFeedback
					{
						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarItemRippleFeedback")]
						public static ThemeResourceKey<Brush> Default => new("ColoredTopTabBarItemRippleFeedback");
					}
				}

				[ResourceKeyDefinition(typeof(double), "TopTabBarItemIconHeight")]
				public static ThemeResourceKey<double> IconHeight => new("TopTabBarItemIconHeight");

				[ResourceKeyDefinition(typeof(Thickness), "TopTabBarItemContentMargin")]
				public static ThemeResourceKey<Thickness> ContentMargin => new("TopTabBarItemContentMargin");
			}

			public static partial class Fab
			{
				public static partial class Background
				{
					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemBackground")]
					public static ThemeResourceKey<Brush> Default => new("FabTabBarItemBackground");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemBackgroundPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("FabTabBarItemBackgroundPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemBackgroundFocused")]
					public static ThemeResourceKey<Brush> Focused => new("FabTabBarItemBackgroundFocused");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemBackgroundPressed")]
					public static ThemeResourceKey<Brush> Pressed => new("FabTabBarItemBackgroundPressed");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemBackgroundDisabled")]
					public static ThemeResourceKey<Brush> Disabled => new("FabTabBarItemBackgroundDisabled");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemBackgroundSelected")]
					public static ThemeResourceKey<Brush> Selected => new("FabTabBarItemBackgroundSelected");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemBackgroundSelectedPointerOver")]
					public static ThemeResourceKey<Brush> SelectedPointerOver => new("FabTabBarItemBackgroundSelectedPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemBackgroundSelectedPressed")]
					public static ThemeResourceKey<Brush> SelectedPressed => new("FabTabBarItemBackgroundSelectedPressed");
				}

				public static partial class Foreground
				{
					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemForeground")]
					public static ThemeResourceKey<Brush> Default => new("FabTabBarItemForeground");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemForegroundPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("FabTabBarItemForegroundPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemForegroundFocused")]
					public static ThemeResourceKey<Brush> Focused => new("FabTabBarItemForegroundFocused");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemForegroundPressed")]
					public static ThemeResourceKey<Brush> Pressed => new("FabTabBarItemForegroundPressed");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemForegroundDisabled")]
					public static ThemeResourceKey<Brush> Disabled => new("FabTabBarItemForegroundDisabled");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemForegroundSelected")]
					public static ThemeResourceKey<Brush> Selected => new("FabTabBarItemForegroundSelected");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemForegroundSelectedPointerOver")]
					public static ThemeResourceKey<Brush> SelectedPointerOver => new("FabTabBarItemForegroundSelectedPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemForegroundSelectedPressed")]
					public static ThemeResourceKey<Brush> SelectedPressed => new("FabTabBarItemForegroundSelectedPressed");
				}

				public static partial class IconForeground
				{
					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemIconForeground")]
					public static ThemeResourceKey<Brush> Default => new("FabTabBarItemIconForeground");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemIconForegroundPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("FabTabBarItemIconForegroundPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemIconForegroundFocused")]
					public static ThemeResourceKey<Brush> Focused => new("FabTabBarItemIconForegroundFocused");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemIconForegroundPressed")]
					public static ThemeResourceKey<Brush> Pressed => new("FabTabBarItemIconForegroundPressed");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemIconForegroundDisabled")]
					public static ThemeResourceKey<Brush> Disabled => new("FabTabBarItemIconForegroundDisabled");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemIconForegroundSelected")]
					public static ThemeResourceKey<Brush> Selected => new("FabTabBarItemIconForegroundSelected");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemIconForegroundSelectedPointerOver")]
					public static ThemeResourceKey<Brush> SelectedPointerOver => new("FabTabBarItemIconForegroundSelectedPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "FabTabBarItemIconForegroundSelectedPressed")]
					public static ThemeResourceKey<Brush> SelectedPressed => new("FabTabBarItemIconForegroundSelectedPressed");
				}

				[ResourceKeyDefinition(typeof(double), "FabTabBarItemOffset")]
				public static ThemeResourceKey<double> Offset => new("FabTabBarItemOffset");

				[ResourceKeyDefinition(typeof(double), "FabTabBarItemContentWidthOrHeight")]
				public static ThemeResourceKey<double> ContentWidthOrHeight => new("FabTabBarItemContentWidthOrHeight");

				[ResourceKeyDefinition(typeof(double), "FabTabBarItemIconTextPadding")]
				public static ThemeResourceKey<double> IconTextPadding => new("FabTabBarItemIconTextPadding");

				[ResourceKeyDefinition(typeof(CornerRadius), "FabTabBarItemCornerRadius")]
				public static ThemeResourceKey<CornerRadius> CornerRadius => new("FabTabBarItemCornerRadius");

				[ResourceKeyDefinition(typeof(Thickness), "FabTabBarItemPadding")]
				public static ThemeResourceKey<Thickness> Padding => new("FabTabBarItemPadding");
			}
		}

		public static partial class Styles
		{
			[ResourceKeyDefinition(typeof(Style), "VerticalTabBarItemStyle", TargetType = typeof(global::Uno.Toolkit.UI.TabBarItem))]
			public static StaticResourceKey<Style> Vertical => new("VerticalTabBarItemStyle");

			[ResourceKeyDefinition(typeof(Style), "BottomTabBarItemStyle", TargetType = typeof(global::Uno.Toolkit.UI.TabBarItem))]
			public static StaticResourceKey<Style> Bottom => new("BottomTabBarItemStyle");

			[ResourceKeyDefinition(typeof(Style), "TopTabBarItemStyle", TargetType = typeof(global::Uno.Toolkit.UI.TabBarItem))]
			public static StaticResourceKey<Style> Top => new("TopTabBarItemStyle");

			[ResourceKeyDefinition(typeof(Style), "ColoredTopTabBarItemStyle", TargetType = typeof(global::Uno.Toolkit.UI.TabBarItem))]
			public static StaticResourceKey<Style> ColoredTop => new("ColoredTopTabBarItemStyle");

			[ResourceKeyDefinition(typeof(Style), "FabTabBarItemStyle", TargetType = typeof(global::Uno.Toolkit.UI.TabBarItem))]
			public static StaticResourceKey<Style> Fab => new("FabTabBarItemStyle");

			[ResourceKeyDefinition(typeof(Style), "VerticalFabTabBarItemStyle", TargetType = typeof(global::Uno.Toolkit.UI.TabBarItem))]
			public static StaticResourceKey<Style> VerticalFab => new("VerticalFabTabBarItemStyle");

			[ResourceKeyDefinition(typeof(Style), "BottomFabTabBarItemStyle", TargetType = typeof(global::Uno.Toolkit.UI.TabBarItem))]
			public static StaticResourceKey<Style> BottomFab => new("BottomFabTabBarItemStyle");

			[ResourceKeyDefinition(typeof(Style), "NavigationTabBarItemStyle", TargetType = typeof(global::Uno.Toolkit.UI.TabBarItem))]
			public static StaticResourceKey<Style> Navigation => new("NavigationTabBarItemStyle");
		}
	}
}
