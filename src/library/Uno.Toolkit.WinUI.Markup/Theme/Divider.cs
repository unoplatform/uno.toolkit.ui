using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Uno.Extensions.Markup;
using Uno.Extensions.Markup.Internals;

namespace Uno.Toolkit.UI.Markup;

public static partial class ToolkitTheme
{
	public static partial class Divider
	{
		public static partial class Resources
		{
			public static partial class Default
			{
				public static partial class Foreground
				{
					[ResourceKeyDefinition(typeof(Brush), "DividerForeground")]
					public static ThemeResourceKey<Brush> Default => new("DividerForeground");
				}

				[ResourceKeyDefinition(typeof(double), "DividerHeight")]
				public static ThemeResourceKey<double> Height => new("DividerHeight");

				public static partial class SubHeader
				{
					public static partial class Typography
					{
						[ResourceKeyDefinition(typeof(int), "DividerSubHeaderCharacterSpacing")]
						public static ThemeResourceKey<int> CharacterSpacing => new("DividerSubHeaderCharacterSpacing");

						[ResourceKeyDefinition(typeof(FontFamily), "DividerSubHeaderFontFamily")]
						public static ThemeResourceKey<FontFamily> FontFamily => new("DividerSubHeaderFontFamily");

						[ResourceKeyDefinition(typeof(double), "DividerSubHeaderFontSize")]
						public static ThemeResourceKey<double> FontSize => new("DividerSubHeaderFontSize");

						[ResourceKeyDefinition(typeof(string), "DividerSubHeaderFontWeight")]
						public static ThemeResourceKey<string> FontWeight => new("DividerSubHeaderFontWeight");
					}

					public static partial class Foreground
					{
						[ResourceKeyDefinition(typeof(Brush), "DividerSubHeaderForeground")]
						public static ThemeResourceKey<Brush> Default => new("DividerSubHeaderForeground");
					}

					[ResourceKeyDefinition(typeof(Thickness), "DividerSubHeaderMargin")]
					public static ThemeResourceKey<Thickness> Margin => new("DividerSubHeaderMargin");
				}
			}
		}

		public static partial class Styles
		{
			[ResourceKeyDefinition(typeof(Style), "DividerStyle", TargetType = typeof(global::Uno.Toolkit.UI.Divider))]
			public static StaticResourceKey<Style> Default => new("DividerStyle");
		}
	}
}
