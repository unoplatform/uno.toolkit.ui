using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Uno.Extensions.Markup;
using Uno.Extensions.Markup.Internals;

namespace Uno.Toolkit.Markup;

public static partial class Theme
{
	public static class TabBarItem
	{
		public static class Resources
		{
			public static class Default
			{
				public static class Background
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

				public static class Foreground
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

				public static class BorderBrush
				{
					[ResourceKeyDefinition(typeof(Brush), "TabBarItemBorderBrush")]
					public static ThemeResourceKey<Brush> Default => new("TabBarItemBorderBrush");
				}
			}
		}
	}

	public static class TabBar
	{
		public static class Resources
		{
			public static class Default
			{
			}
		}
	}
}
