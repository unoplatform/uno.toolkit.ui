using System;
using Windows.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Uno.Extensions.Markup;
using Uno.Extensions.Markup.Internals;

namespace Uno.Toolkit.Markup;

public static partial class Theme
{
	public static partial class TabBar
	{
		public static partial class Resources
		{
			public static partial class Bottom
			{
				[ResourceKeyDefinition(typeof(Brush), "BottomTabBarBackground")]
				public static ThemeResourceKey<Brush> Background => new("BottomTabBarBackground");

				[ResourceKeyDefinition(typeof(double), "NavigationTabBarWidthOrHeight")]
				public static ThemeResourceKey<double> NavigationTabBarWidthOrHeight => new("NavigationTabBarWidthOrHeight");
			}

			public static partial class ColoredTop
			{
				[ResourceKeyDefinition(typeof(Brush), "ColoredTopTabBarBackground")]
				public static ThemeResourceKey<Brush> Background => new("ColoredTopTabBarBackground");
			}

			public static partial class Top
			{
				[ResourceKeyDefinition(typeof(Brush), "TopTabBarBackground")]
				public static ThemeResourceKey<Brush> Background => new("TopTabBarBackground");

				[ResourceKeyDefinition(typeof(double), "TopTabBarHeight")]
				public static ThemeResourceKey<double> Height => new("TopTabBarHeight");
			}

			public static partial class Vertical
			{
				[ResourceKeyDefinition(typeof(Brush), "VerticalTabBarBackground")]
				public static ThemeResourceKey<Brush> Background => new("VerticalTabBarBackground");

				[ResourceKeyDefinition(typeof(double), "NavigationTabBarWidthOrHeight")]
				public static ThemeResourceKey<double> NavigationTabBarWidthOrHeight => new("NavigationTabBarWidthOrHeight");
			}

		}

		public static partial class Styles
		{
			[ResourceKeyDefinition(typeof(Style), "WorkaroundTabBarStyle", TargetType = typeof(global::Uno.Toolkit.UI.TabBar))]
			public static StaticResourceKey<Style> Workaround => new("WorkaroundTabBarStyle");

			[ResourceKeyDefinition(typeof(Style), "BaseMaterialTabBarStyle", TargetType = typeof(global::Uno.Toolkit.UI.TabBar))]
			public static StaticResourceKey<Style> BaseMaterial => new("BaseMaterialTabBarStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialVerticalTabBarStyle", TargetType = typeof(global::Uno.Toolkit.UI.TabBar))]
			public static StaticResourceKey<Style> Vertical => new("MaterialVerticalTabBarStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialBottomTabBarStyle", TargetType = typeof(global::Uno.Toolkit.UI.TabBar))]
			public static StaticResourceKey<Style> Bottom => new("MaterialBottomTabBarStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialTopTabBarStyle", TargetType = typeof(global::Uno.Toolkit.UI.TabBar))]
			public static StaticResourceKey<Style> Top => new("MaterialTopTabBarStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialColoredTopTabBarStyle", TargetType = typeof(global::Uno.Toolkit.UI.TabBar))]
			public static StaticResourceKey<Style> ColoredTop => new("MaterialColoredTopTabBarStyle");
		}
	}
}
