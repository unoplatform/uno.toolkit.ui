using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Uno.Extensions.Markup;
using Uno.Extensions.Markup.Internals;

namespace Uno.Toolkit.UI.Markup;

public static partial class Theme
{
	public static partial class Card
	{
		public static partial class Resources
		{
			public static partial class Avatar
			{
				public static partial class Elevated
				{
					public static partial class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "AvatarElevatedCardBackground")]
						public static ThemeResourceKey<Brush> Default => new("AvatarElevatedCardBackground");
					}

					public static partial class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "AvatarElevatedCardBorderBrush")]
						public static ThemeResourceKey<Brush> Default => new("AvatarElevatedCardBorderBrush");

						[ResourceKeyDefinition(typeof(Brush), "AvatarElevatedCardBorderBrushFocused")]
						public static ThemeResourceKey<Brush> Focused => new("AvatarElevatedCardBorderBrushFocused");

						[ResourceKeyDefinition(typeof(Brush), "AvatarElevatedCardBorderBrushPointerOver")]
						public static ThemeResourceKey<Brush> PointerOver => new("AvatarElevatedCardBorderBrushPointerOver");
					}
				}

				public static partial class Filled
				{
					public static partial class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "AvatarFilledCardBackground")]
						public static ThemeResourceKey<Brush> Default => new("AvatarFilledCardBackground");
					}

					public static partial class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "AvatarFilledCardBorderBrush")]
						public static ThemeResourceKey<Brush> Default => new("AvatarFilledCardBorderBrush");

						[ResourceKeyDefinition(typeof(Brush), "AvatarFilledCardBorderBrushFocused")]
						public static ThemeResourceKey<Brush> Focused => new("AvatarFilledCardBorderBrushFocused");

						[ResourceKeyDefinition(typeof(Brush), "AvatarFilledCardBorderBrushPointerOver")]
						public static ThemeResourceKey<Brush> PointerOver => new("AvatarFilledCardBorderBrushPointerOver");
					}
				}

				public static partial class Outlined
				{
					public static partial class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "AvatarOutlinedCardBackground")]
						public static ThemeResourceKey<Brush> Default => new("AvatarOutlinedCardBackground");
					}

					public static partial class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "AvatarOutlinedCardBorderBrush")]
						public static ThemeResourceKey<Brush> Default => new("AvatarOutlinedCardBorderBrush");
					}
				}
			}

			public static partial class Elevated
			{
				public static partial class Background
				{
					[ResourceKeyDefinition(typeof(Brush), "ElevatedCardBackground")]
					public static ThemeResourceKey<Brush> Default => new("ElevatedCardBackground");
				}

				public static partial class BorderBrush
				{
					[ResourceKeyDefinition(typeof(Brush), "ElevatedCardBorderBrush")]
					public static ThemeResourceKey<Brush> Default => new("ElevatedCardBorderBrush");

					[ResourceKeyDefinition(typeof(Brush), "ElevatedCardBorderBrushFocused")]
					public static ThemeResourceKey<Brush> Focused => new("ElevatedCardBorderBrushFocused");

					[ResourceKeyDefinition(typeof(Brush), "ElevatedCardBorderBrushPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("ElevatedCardBorderBrushPointerOver");
				}
			}

			public static partial class Filled
			{
				public static partial class Background
				{
					[ResourceKeyDefinition(typeof(Brush), "FilledCardBackground")]
					public static ThemeResourceKey<Brush> Default => new("FilledCardBackground");
				}

				public static partial class BorderBrush
				{
					[ResourceKeyDefinition(typeof(Brush), "FilledCardBorderBrush")]
					public static ThemeResourceKey<Brush> Default => new("FilledCardBorderBrush");

					[ResourceKeyDefinition(typeof(Brush), "FilledCardBorderBrushFocused")]
					public static ThemeResourceKey<Brush> Focused => new("FilledCardBorderBrushFocused");

					[ResourceKeyDefinition(typeof(Brush), "FilledCardBorderBrushPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("FilledCardBorderBrushPointerOver");
				}
			}

			public static partial class Outlined
			{
				public static partial class Background
				{
					[ResourceKeyDefinition(typeof(Brush), "OutlinedCardBackground")]
					public static ThemeResourceKey<Brush> Default => new("OutlinedCardBackground");
				}

				public static partial class BorderBrush
				{
					[ResourceKeyDefinition(typeof(Brush), "OutlinedCardBorderBrush")]
					public static ThemeResourceKey<Brush> Default => new("OutlinedCardBorderBrush");
				}
			}

			public static partial class SmallMedia
			{
				public static partial class Elevated
				{
					public static partial class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "SmallMediaElevatedCardBackground")]
						public static ThemeResourceKey<Brush> Default => new("SmallMediaElevatedCardBackground");
					}

					public static partial class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "SmallMediaElevatedCardBorderBrush")]
						public static ThemeResourceKey<Brush> Default => new("SmallMediaElevatedCardBorderBrush");

						[ResourceKeyDefinition(typeof(Brush), "SmallMediaElevatedCardBorderBrushFocused")]
						public static ThemeResourceKey<Brush> Focused => new("SmallMediaElevatedCardBorderBrushFocused");

						[ResourceKeyDefinition(typeof(Brush), "SmallMediaElevatedCardBorderBrushPointerOver")]
						public static ThemeResourceKey<Brush> PointerOver => new("SmallMediaElevatedCardBorderBrushPointerOver");
					}
				}

				public static partial class Filled
				{
					public static partial class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "SmallMediaFilledCardBackground")]
						public static ThemeResourceKey<Brush> Default => new("SmallMediaFilledCardBackground");
					}

					public static partial class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "SmallMediaFilledCardBorderBrush")]
						public static ThemeResourceKey<Brush> Default => new("SmallMediaFilledCardBorderBrush");

						[ResourceKeyDefinition(typeof(Brush), "SmallMediaFilledCardBorderBrushFocused")]
						public static ThemeResourceKey<Brush> Focused => new("SmallMediaFilledCardBorderBrushFocused");

						[ResourceKeyDefinition(typeof(Brush), "SmallMediaFilledCardBorderBrushPointerOver")]
						public static ThemeResourceKey<Brush> PointerOver => new("SmallMediaFilledCardBorderBrushPointerOver");
					}
				}

				public static partial class Outlined
				{
					public static partial class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "SmallMediaOutlinedCardBackground")]
						public static ThemeResourceKey<Brush> Default => new("SmallMediaOutlinedCardBackground");
					}

					public static partial class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "SmallMediaOutlinedCardBorderBrush")]
						public static ThemeResourceKey<Brush> Default => new("SmallMediaOutlinedCardBorderBrush");
					}
				}
			}
		}

		public static partial class Styles
		{
			[ResourceKeyDefinition(typeof(Style), "FilledCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> Filled => new("FilledCardStyle");

			[ResourceKeyDefinition(typeof(Style), "OutlinedCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> Outlined => new("OutlinedCardStyle");

			[ResourceKeyDefinition(typeof(Style), "ElevatedCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> Elevated => new("ElevatedCardStyle");

			[ResourceKeyDefinition(typeof(Style), "AvatarFilledCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> AvatarFilled => new("AvatarFilledCardStyle");

			[ResourceKeyDefinition(typeof(Style), "AvatarOutlinedCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> AvatarOutlined => new("AvatarOutlinedCardStyle");

			[ResourceKeyDefinition(typeof(Style), "AvatarElevatedCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> AvatarElevated => new("AvatarElevatedCardStyle");

			[ResourceKeyDefinition(typeof(Style), "SmallMediaFilledCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> SmallMediaFilled => new("SmallMediaFilledCardStyle");

			[ResourceKeyDefinition(typeof(Style), "SmallMediaOutlinedCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> SmallMediaOutlined => new("SmallMediaOutlinedCardStyle");

			[ResourceKeyDefinition(typeof(Style), "SmallMediaElevatedCardStyle", TargetType = typeof(global::Uno.Toolkit.UI.Card))]
			public static StaticResourceKey<Style> SmallMediaElevated => new("SmallMediaElevatedCardStyle");
		}
	}
}
