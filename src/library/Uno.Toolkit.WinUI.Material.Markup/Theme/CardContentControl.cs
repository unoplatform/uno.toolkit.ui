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
	public static partial class CardContentControl
	{
		public static partial class Resources
		{
			public static partial class Elevated
			{
				[ResourceKeyDefinition(typeof(Brush), "ElevatedCardContentBackground")]
				public static ThemeResourceKey<Brush> CardContentBackground => new("ElevatedCardContentBackground");

				[ResourceKeyDefinition(typeof(Brush), "ElevatedCardContentBorderBrushFocused")]
				public static ThemeResourceKey<Brush> CardContentBorderBrushFocused => new("ElevatedCardContentBorderBrushFocused");

				[ResourceKeyDefinition(typeof(Brush), "ElevatedCardContentBorderBrushPointerOver")]
				public static ThemeResourceKey<Brush> CardContentBorderBrushPointerOver => new("ElevatedCardContentBorderBrushPointerOver");

				[ResourceKeyDefinition(typeof(Brush), "ElevatedCardContentBorderBrushPressed")]
				public static ThemeResourceKey<Brush> CardContentBorderBrushPressed => new("ElevatedCardContentBorderBrushPressed");
			}

			public static partial class Filled
			{
				public static partial class CardContent
				{
					[ResourceKeyDefinition(typeof(Brush), "FilledCardContentBackground")]
					public static StaticResourceKey<Brush> Background => new("FilledCardContentBackground");

					[ResourceKeyDefinition(typeof(Brush), "FilledCardContentBorderBrush")]
					public static ThemeResourceKey<Brush> BorderBrush => new("FilledCardContentBorderBrush");
				}

				[ResourceKeyDefinition(typeof(Brush), "FilledCardContentBorderBrushFocused")]
				public static ThemeResourceKey<Brush> CardContentBorderBrushFocused => new("FilledCardContentBorderBrushFocused");

				[ResourceKeyDefinition(typeof(Brush), "FilledCardContentBorderBrushPointerOver")]
				public static ThemeResourceKey<Brush> CardContentBorderBrushPointerOver => new("FilledCardContentBorderBrushPointerOver");

				[ResourceKeyDefinition(typeof(Brush), "FilledCardContentBorderBrushPressed")]
				public static ThemeResourceKey<Brush> CardContentBorderBrushPressed => new("FilledCardContentBorderBrushPressed");
			}

			public static partial class Outlined
			{
				public static partial class CardContent
				{
					[ResourceKeyDefinition(typeof(Brush), "OutlinedCardContentBackground")]
					public static ThemeResourceKey<Brush> Background => new("OutlinedCardContentBackground");

					[ResourceKeyDefinition(typeof(Brush), "OutlinedCardContentBorderBrush")]
					public static ThemeResourceKey<Brush> BorderBrush => new("OutlinedCardContentBorderBrush");
				}
			}
		}

		public static partial class Styles
		{
			[ResourceKeyDefinition(typeof(Style), "MaterialFilledCardContentControlStyle", TargetType = typeof(global::Uno.Toolkit.UI.CardContentControl))]
			public static StaticResourceKey<Style> Filled => new("MaterialFilledCardContentControlStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialOutlinedCardContentControlStyle", TargetType = typeof(global::Uno.Toolkit.UI.CardContentControl))]
			public static StaticResourceKey<Style> Outlined => new("MaterialOutlinedCardContentControlStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialElevatedCardContentControlStyle", TargetType = typeof(global::Uno.Toolkit.UI.CardContentControl))]
			public static StaticResourceKey<Style> Elevated => new("MaterialElevatedCardContentControlStyle");
		}
	}
}
