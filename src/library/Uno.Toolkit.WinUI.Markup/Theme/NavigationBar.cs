using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Uno.Extensions.Markup;
using Uno.Extensions.Markup.Internals;

namespace Uno.Toolkit.UI.Markup;

public static partial class Theme
{
	public static partial class NavigationBar
	{
		public static partial class Resources
		{
			public static partial class Default
			{
				public static partial class Foreground
				{
					[ResourceKeyDefinition(typeof(Brush), "NavigationBarForeground")]
					public static ThemeResourceKey<Brush> Default => new("NavigationBarForeground");
				}

				public static partial class Background
				{
					[ResourceKeyDefinition(typeof(Brush), "NavigationBarBackground")]
					public static ThemeResourceKey<Brush> Default => new("NavigationBarBackground");
				}

				public static partial class MainCommand
				{
					public static partial class Foreground
					{
						[ResourceKeyDefinition(typeof(Brush), "NavigationBarMainCommandForeground")]
						public static ThemeResourceKey<Brush> Default => new("NavigationBarMainCommandForeground");
					}
				}

				public static partial class Typography
				{
					[ResourceKeyDefinition(typeof(FontFamily), "NavigationBarFontFamily")]
					public static ThemeResourceKey<FontFamily> FontFamily => new("NavigationBarFontFamily");

					[ResourceKeyDefinition(typeof(double), "NavigationBarFontSize")]
					public static ThemeResourceKey<double> FontSize => new("NavigationBarFontSize");

					[ResourceKeyDefinition(typeof(FontWeights), "NavigationBarFontWeight")]
					public static ThemeResourceKey<FontWeights> FontWeight => new("NavigationBarFontWeight");
				}
			}
		}

		public static partial class Styles
		{
			[ResourceKeyDefinition(typeof(Style), "NavigationBarStyle", TargetType = typeof(global::Uno.Toolkit.UI.NavigationBar))]
			public static StaticResourceKey<Style> Default => new("NavigationBarStyle");

			[ResourceKeyDefinition(typeof(Style), "ModalNavigationBarStyle", TargetType = typeof(global::Uno.Toolkit.UI.NavigationBar))]
			public static StaticResourceKey<Style> Modal => new("ModalNavigationBarStyle");

			[ResourceKeyDefinition(typeof(Style), "PrimaryNavigationBarStyle", TargetType = typeof(global::Uno.Toolkit.UI.NavigationBar))]
			public static StaticResourceKey<Style> Primary => new("PrimaryNavigationBarStyle");
		}
	}
}
