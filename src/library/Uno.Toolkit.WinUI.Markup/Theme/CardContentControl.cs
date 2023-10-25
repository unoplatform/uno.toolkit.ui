using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Uno.Extensions.Markup;
using Uno.Extensions.Markup.Internals;

namespace Uno.Toolkit.UI.Markup;

public static partial class Theme
{
	public static partial class CardContentControl
	{
		public static partial class Resources
		{
			public static partial class Elevated
			{
				[ResourceKeyDefinition(typeof(Brush), "ElevatedCardContentBackground")]
				public static ThemeResourceKey<Brush> Background => new("ElevatedCardContentBackground");

				public static partial class BorderBrush
				{
					[ResourceKeyDefinition(typeof(Brush), "ElevatedCardContentBorderBrush")]
					public static ThemeResourceKey<Brush> Default => new("ElevatedCardContentBorderBrush");

					[ResourceKeyDefinition(typeof(Brush), "ElevatedCardContentBorderBrushFocused")]
					public static ThemeResourceKey<Brush> Focused => new("ElevatedCardContentBorderBrushFocused");

					[ResourceKeyDefinition(typeof(Brush), "ElevatedCardContentBorderBrushPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("ElevatedCardContentBorderBrushPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "ElevatedCardContentBorderBrushPressed")]
					public static ThemeResourceKey<Brush> Pressed => new("ElevatedCardContentBorderBrushPressed");
				}
			}

			public static partial class Filled
			{
				[ResourceKeyDefinition(typeof(Brush), "FilledCardContentBackground")]
				public static StaticResourceKey<Brush> Background => new("FilledCardContentBackground");

				public static partial class BorderBrush
				{
					[ResourceKeyDefinition(typeof(Brush), "FilledCardContentBorderBrush")]
					public static ThemeResourceKey<Brush> Default => new("FilledCardContentBorderBrush");

					[ResourceKeyDefinition(typeof(Brush), "FilledCardContentBorderBrushFocused")]
					public static ThemeResourceKey<Brush> Focused => new("FilledCardContentBorderBrushFocused");

					[ResourceKeyDefinition(typeof(Brush), "FilledCardContentBorderBrushPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("FilledCardContentBorderBrushPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "FilledCardContentBorderBrushPressed")]
					public static ThemeResourceKey<Brush> Pressed => new("FilledCardContentBorderBrushPressed");
				}
			}

			public static partial class Outlined
			{
				[ResourceKeyDefinition(typeof(Brush), "OutlinedCardContentBackground")]
				public static ThemeResourceKey<Brush> Background => new("OutlinedCardContentBackground");

				[ResourceKeyDefinition(typeof(Brush), "OutlinedCardContentBorderBrush")]
				public static ThemeResourceKey<Brush> BorderBrush => new("OutlinedCardContentBorderBrush");
			}
		}

		public static partial class Styles
		{
			[ResourceKeyDefinition(typeof(Style), "FilledCardContentControlStyle", TargetType = typeof(global::Uno.Toolkit.UI.CardContentControl))]
			public static StaticResourceKey<Style> Filled => new("FilledCardContentControlStyle");

			[ResourceKeyDefinition(typeof(Style), "OutlinedCardContentControlStyle", TargetType = typeof(global::Uno.Toolkit.UI.CardContentControl))]
			public static StaticResourceKey<Style> Outlined => new("OutlinedCardContentControlStyle");

			[ResourceKeyDefinition(typeof(Style), "ElevatedCardContentControlStyle", TargetType = typeof(global::Uno.Toolkit.UI.CardContentControl))]
			public static StaticResourceKey<Style> Elevated => new("ElevatedCardContentControlStyle");
		}
	}
}
