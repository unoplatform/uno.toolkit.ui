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
	public static partial class Card
	{
		public static partial class Resources
		{
			public static partial class AvatarElevated
			{
				[ResourceKeyDefinition(typeof(Brush), "AvatarElevatedCardBackground")]
				public static ThemeResourceKey<Brush> Background => new("AvatarElevatedCardBackground");

				[ResourceKeyDefinition(typeof(Brush), "AvatarElevatedCardBorderBrush")]
				public static ThemeResourceKey<Brush> BorderBrush => new("AvatarElevatedCardBorderBrush");

				[ResourceKeyDefinition(typeof(Brush), "AvatarElevatedCardBorderBrushFocused")]
				public static ThemeResourceKey<Brush> BorderBrushFocused => new("AvatarElevatedCardBorderBrushFocused");

				[ResourceKeyDefinition(typeof(Brush), "AvatarElevatedCardBorderBrushPointerOver")]
				public static ThemeResourceKey<Brush> BorderBrushPointerOver => new("AvatarElevatedCardBorderBrushPointerOver");

				[ResourceKeyDefinition(typeof(CornerRadius), "CardCornerRadius")]
				public static StaticResourceKey<CornerRadius> CornerRadius => new("CardCornerRadius");

				[ResourceKeyDefinition(typeof(double), "CardElevation")]
				public static StaticResourceKey<double> Elevation => new("CardElevation");
			}

			public static partial class AvatarFilled
			{
				[ResourceKeyDefinition(typeof(Brush), "AvatarFilledCardBackground")]
				public static ThemeResourceKey<Brush> Background => new("AvatarFilledCardBackground");

				[ResourceKeyDefinition(typeof(Brush), "AvatarFilledCardBorderBrush")]
				public static ThemeResourceKey<Brush> BorderBrush => new("AvatarFilledCardBorderBrush");

				[ResourceKeyDefinition(typeof(Brush), "AvatarFilledCardBorderBrushFocused")]
				public static ThemeResourceKey<Brush> BorderBrushFocused => new("AvatarFilledCardBorderBrushFocused");

				[ResourceKeyDefinition(typeof(Brush), "AvatarFilledCardBorderBrushPointerOver")]
				public static ThemeResourceKey<Brush> BorderBrushPointerOver => new("AvatarFilledCardBorderBrushPointerOver");

				[ResourceKeyDefinition(typeof(CornerRadius), "CardCornerRadius")]
				public static StaticResourceKey<CornerRadius> CornerRadius => new("CardCornerRadius");
			}

			public static partial class AvatarOutlined
			{
				[ResourceKeyDefinition(typeof(Brush), "AvatarOutlinedCardBackground")]
				public static ThemeResourceKey<Brush> Background => new("AvatarOutlinedCardBackground");

				[ResourceKeyDefinition(typeof(Brush), "AvatarOutlinedCardBorderBrush")]
				public static ThemeResourceKey<Brush> BorderBrush => new("AvatarOutlinedCardBorderBrush");

				[ResourceKeyDefinition(typeof(Thickness), "CardBorderThickness")]
				public static StaticResourceKey<Thickness> BorderThickness => new("CardBorderThickness");
			}

			public static partial class Card
			{
				[ResourceKeyDefinition(typeof(CornerRadius), "CardCornerRadius")]
				public static StaticResourceKey<CornerRadius> CornerRadius => new("CardCornerRadius");

				[ResourceKeyDefinition(typeof(double), "CardMaxWidth")]
				public static StaticResourceKey<double> MaxWidth => new("CardMaxWidth");

				[ResourceKeyDefinition(typeof(double), "CardMinHeight")]
				public static StaticResourceKey<double> MinHeight => new("CardMinHeight");

				[ResourceKeyDefinition(typeof(Thickness), "CardPadding")]
				public static StaticResourceKey<Thickness> Padding => new("CardPadding");
			}

			public static partial class Elevated
			{
				[ResourceKeyDefinition(typeof(Brush), "ElevatedCardBackground")]
				public static ThemeResourceKey<Brush> Background => new("ElevatedCardBackground");

				[ResourceKeyDefinition(typeof(Brush), "ElevatedCardBorderBrush")]
				public static ThemeResourceKey<Brush> BorderBrush => new("ElevatedCardBorderBrush");

				[ResourceKeyDefinition(typeof(Brush), "ElevatedCardBorderBrushFocused")]
				public static ThemeResourceKey<Brush> BorderBrushFocused => new("ElevatedCardBorderBrushFocused");

				[ResourceKeyDefinition(typeof(Brush), "ElevatedCardBorderBrushPointerOver")]
				public static ThemeResourceKey<Brush> BorderBrushPointerOver => new("ElevatedCardBorderBrushPointerOver");

				[ResourceKeyDefinition(typeof(CornerRadius), "CardCornerRadius")]
				public static StaticResourceKey<CornerRadius> CornerRadius => new("CardCornerRadius");

				[ResourceKeyDefinition(typeof(double), "CardElevation")]
				public static StaticResourceKey<double> Elevation => new("CardElevation");
			}

			public static partial class Filled
			{
				[ResourceKeyDefinition(typeof(Brush), "FilledCardBackground")]
				public static ThemeResourceKey<Brush> Background => new("FilledCardBackground");

				[ResourceKeyDefinition(typeof(Brush), "FilledCardBorderBrush")]
				public static ThemeResourceKey<Brush> BorderBrush => new("FilledCardBorderBrush");

				[ResourceKeyDefinition(typeof(Brush), "FilledCardBorderBrushFocused")]
				public static ThemeResourceKey<Brush> BorderBrushFocused => new("FilledCardBorderBrushFocused");

				[ResourceKeyDefinition(typeof(Brush), "FilledCardBorderBrushPointerOver")]
				public static ThemeResourceKey<Brush> BorderBrushPointerOver => new("FilledCardBorderBrushPointerOver");

				[ResourceKeyDefinition(typeof(CornerRadius), "CardCornerRadius")]
				public static StaticResourceKey<CornerRadius> CornerRadius => new("CardCornerRadius");
			}

			public static partial class Outlined
			{
				[ResourceKeyDefinition(typeof(Brush), "OutlinedCardBackground")]
				public static ThemeResourceKey<Brush> Background => new("OutlinedCardBackground");

				[ResourceKeyDefinition(typeof(Brush), "OutlinedCardBorderBrush")]
				public static ThemeResourceKey<Brush> BorderBrush => new("OutlinedCardBorderBrush");

				[ResourceKeyDefinition(typeof(Thickness), "CardBorderThickness")]
				public static StaticResourceKey<Thickness> BorderThickness => new("CardBorderThickness");
			}

			public static partial class SmallMediaElevated
			{
				[ResourceKeyDefinition(typeof(Brush), "SmallMediaElevatedCardBackground")]
				public static ThemeResourceKey<Brush> Background => new("SmallMediaElevatedCardBackground");

				[ResourceKeyDefinition(typeof(Brush), "SmallMediaElevatedCardBorderBrush")]
				public static ThemeResourceKey<Brush> BorderBrush => new("SmallMediaElevatedCardBorderBrush");

				[ResourceKeyDefinition(typeof(Brush), "SmallMediaElevatedCardBorderBrushFocused")]
				public static ThemeResourceKey<Brush> BorderBrushFocused => new("SmallMediaElevatedCardBorderBrushFocused");

				[ResourceKeyDefinition(typeof(Brush), "SmallMediaElevatedCardBorderBrushPointerOver")]
				public static ThemeResourceKey<Brush> BorderBrushPointerOver => new("SmallMediaElevatedCardBorderBrushPointerOver");

				[ResourceKeyDefinition(typeof(CornerRadius), "CardCornerRadius")]
				public static StaticResourceKey<CornerRadius> CornerRadius => new("CardCornerRadius");

				[ResourceKeyDefinition(typeof(double), "CardElevation")]
				public static StaticResourceKey<double> Elevation => new("CardElevation");
			}

			public static partial class SmallMediaFilled
			{
				[ResourceKeyDefinition(typeof(Brush), "SmallMediaFilledCardBackground")]
				public static ThemeResourceKey<Brush> Background => new("SmallMediaFilledCardBackground");

				[ResourceKeyDefinition(typeof(Brush), "SmallMediaFilledCardBorderBrush")]
				public static ThemeResourceKey<Brush> BorderBrush => new("SmallMediaFilledCardBorderBrush");

				[ResourceKeyDefinition(typeof(Brush), "SmallMediaFilledCardBorderBrushFocused")]
				public static ThemeResourceKey<Brush> BorderBrushFocused => new("SmallMediaFilledCardBorderBrushFocused");

				[ResourceKeyDefinition(typeof(Brush), "SmallMediaFilledCardBorderBrushPointerOver")]
				public static ThemeResourceKey<Brush> BorderBrushPointerOver => new("SmallMediaFilledCardBorderBrushPointerOver");

				[ResourceKeyDefinition(typeof(CornerRadius), "CardCornerRadius")]
				public static StaticResourceKey<CornerRadius> CornerRadius => new("CardCornerRadius");
			}

			public static partial class SmallMediaOutlined
			{
				[ResourceKeyDefinition(typeof(Brush), "SmallMediaOutlinedCardBackground")]
				public static ThemeResourceKey<Brush> Background => new("SmallMediaOutlinedCardBackground");

				[ResourceKeyDefinition(typeof(Brush), "SmallMediaOutlinedCardBorderBrush")]
				public static ThemeResourceKey<Brush> BorderBrush => new("SmallMediaOutlinedCardBorderBrush");

				[ResourceKeyDefinition(typeof(Thickness), "CardBorderThickness")]
				public static StaticResourceKey<Thickness> BorderThickness => new("CardBorderThickness");
			}

			[ResourceKeyDefinition(typeof(Thickness), "CardElevationMargin")]
			public static StaticResourceKey<Thickness> CardElevationMargin => new("CardElevationMargin");
		}

		public static partial class Styles
		{
			[ResourceKeyDefinition(typeof(Style), "MaterialAvatarBaseCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> AvatarBase => new("MaterialAvatarBaseCardStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialSmallMediaBaseCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> SmallMediaBase => new("MaterialSmallMediaBaseCardStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialFilledCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> Filled => new("MaterialFilledCardStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialOutlinedCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> Outlined => new("MaterialOutlinedCardStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialElevatedCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> Elevated => new("MaterialElevatedCardStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialAvatarFilledCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> AvatarFilled => new("MaterialAvatarFilledCardStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialAvatarOutlinedCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> AvatarOutlined => new("MaterialAvatarOutlinedCardStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialAvatarElevatedCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> AvatarElevated => new("MaterialAvatarElevatedCardStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialSmallMediaFilledCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> SmallMediaFilled => new("MaterialSmallMediaFilledCardStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialSmallMediaOutlinedCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> SmallMediaOutlined => new("MaterialSmallMediaOutlinedCardStyle");

			[ResourceKeyDefinition(typeof(Style), "MaterialSmallMediaElevatedCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> SmallMediaElevated => new("MaterialSmallMediaElevatedCardStyle");
		}
	}
}
