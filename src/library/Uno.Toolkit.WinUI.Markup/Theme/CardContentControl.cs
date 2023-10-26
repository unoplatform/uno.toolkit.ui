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
				public static partial class Background
				{
					[ResourceKeyDefinition(typeof(Brush), "ElevatedCardContentBackground")]
					public static ThemeResourceKey<Brush> Default => new("ElevatedCardContentBackground");
				}

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
				public static partial class Background
				{
					[ResourceKeyDefinition(typeof(Brush), "FilledCardContentBackground")]
					public static ThemeResourceKey<Brush> Default => new("FilledCardContentBackground");
				}

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
				public static partial class Background
				{
					[ResourceKeyDefinition(typeof(Brush), "OutlinedCardContentBackground")]
					public static ThemeResourceKey<Brush> Default => new("OutlinedCardContentBackground");
				}

				public static partial class BorderBrush
				{
					[ResourceKeyDefinition(typeof(Brush), "OutlinedCardContentBorderBrush")]
					public static ThemeResourceKey<Brush> Default => new("OutlinedCardContentBorderBrush");
				}
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
