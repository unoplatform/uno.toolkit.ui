using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Uno.Extensions.Markup;
using Uno.Extensions.Markup.Internals;

namespace Uno.Toolkit.UI.Markup;

public static partial class Theme
{
	public static partial class TabBar
	{
		public static partial class Resources
		{
			public static partial class Bottom
			{
				public static partial class Background
				{
					[ResourceKeyDefinition(typeof(Brush), "BottomTabBarBackground")]
					public static ThemeResourceKey<Brush> Default => new("BottomTabBarBackground");
				}

				[ResourceKeyDefinition(typeof(double), "NavigationTabBarWidthOrHeight")]
				public static ThemeResourceKey<double> NavigationTabBarWidthOrHeight => new("NavigationTabBarWidthOrHeight");
			}

			public static partial class Top
			{
				public static partial class Background
				{
					[ResourceKeyDefinition(typeof(Brush), "TopTabBarBackground")]
					public static ThemeResourceKey<Brush> Default => new("TopTabBarBackground");
				}

				[ResourceKeyDefinition(typeof(double), "TopTabBarHeight")]
				public static ThemeResourceKey<double> Height => new("TopTabBarHeight");

				public static partial class Colored
				{
					public static partial class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarBackground")]
						public static ThemeResourceKey<Brush> Default => new("ColoredTopTabBarBackground");
					}
				}
			}

			public static partial class Vertical
			{
				public static partial class Background
				{
					[ResourceKeyDefinition(typeof(Brush), "VerticalTabBarBackground")]
					public static ThemeResourceKey<Brush> Default => new("VerticalTabBarBackground");
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

				public static partial class TintBrush
				{
					[ResourceKeyDefinition(typeof(Brush), "NavigationTabBarTintBrush")]
					public static ThemeResourceKey<Brush> Default => new("NavigationTabBarTintBrush");
				}

				[ResourceKeyDefinition(typeof(double), "NavigationTabBarWidthOrHeight")]
				public static ThemeResourceKey<double> WidthOrHeight => new("NavigationTabBarWidthOrHeight");

				[ResourceKeyDefinition(typeof(double), "NavigationTabBarTintOpacity")]
				public static ThemeResourceKey<double> TintOpacity => new("NavigationTabBarTintOpacity");
			}
		}

		public static partial class Styles
		{
			[ResourceKeyDefinition(typeof(Style), "VerticalTabBarStyle", TargetType = typeof(global::Uno.Toolkit.UI.TabBar))]
			public static StaticResourceKey<Style> Vertical => new("VerticalTabBarStyle");

			[ResourceKeyDefinition(typeof(Style), "BottomTabBarStyle", TargetType = typeof(global::Uno.Toolkit.UI.TabBar))]
			public static StaticResourceKey<Style> Bottom => new("BottomTabBarStyle");

			[ResourceKeyDefinition(typeof(Style), "TopTabBarStyle", TargetType = typeof(global::Uno.Toolkit.UI.TabBar))]
			public static StaticResourceKey<Style> Top => new("TopTabBarStyle");

			[ResourceKeyDefinition(typeof(Style), "ColoredTopTabBarStyle", TargetType = typeof(global::Uno.Toolkit.UI.TabBar))]
			public static StaticResourceKey<Style> ColoredTop => new("ColoredTopTabBarStyle");
		}
	}
}
