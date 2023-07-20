using Microsoft.UI.Xaml.Media;
using Uno.Extensions.Markup.Internals;
using Uno.Toolkit.Markup;

namespace Uno.Toolkit.Markup
{
	public static partial class Theme
	{
		public static class Card
		{
			public static class Resources
			{
				public static class ContentTemplate
				{
					public static class Foreground
					{
						[ResourceKeyDefinition(typeof(Brush), "ContentTemplateForeground")]
						public static ResourceValue<Brush> Default => new("ContentTemplateForeground", true);
					}

					public static class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "ContentTemplateBorderBrush")]
						public static ResourceValue<Brush> Default => new("ContentTemplateBorderBrush", true);
					}
				}

				public static class Elevated
				{
					public static class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "ElevatedCardBackground")]
						public static ResourceValue<Brush> Default => new("ElevatedCardBackground", true);
					}

					public static class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "ElevatedCardBorderBrush")]
						public static ResourceValue<Brush> Default => new("ElevatedCardBorderBrush", true);

						[ResourceKeyDefinition(typeof(Brush), "ElevatedCardBorderBrushPointerOver")]
						public static ResourceValue<Brush> PointerOver => new("ElevatedCardBorderBrushPointerOver", true);

						[ResourceKeyDefinition(typeof(Brush), "ElevatedCardBorderBrushFocused")]
						public static ResourceValue<Brush> Focused => new("ElevatedCardBorderBrushFocused", true);
					}
				}

				public static class AvatarElevated
				{
					public static class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "AvatarElevatedCardBackground")]
						public static ResourceValue<Brush> Default => new("AvatarElevatedCardBackground", true);
					}

					public static class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "AvatarElevatedCardBorderBrush")]
						public static ResourceValue<Brush> Default => new("AvatarElevatedCardBorderBrush", true);

						[ResourceKeyDefinition(typeof(Brush), "AvatarElevatedCardBorderBrushPointerOver")]
						public static ResourceValue<Brush> PointerOver => new("AvatarElevatedCardBorderBrushPointerOver", true);

						[ResourceKeyDefinition(typeof(Brush), "AvatarElevatedCardBorderBrushFocused")]
						public static ResourceValue<Brush> Focused => new("AvatarElevatedCardBorderBrushFocused", true);
					}
				}

				public static class SmallMediaElevated
				{
					public static class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "SmallMediaElevatedCardBackground")]
						public static ResourceValue<Brush> Default => new("SmallMediaElevatedCardBackground", true);
					}

					public static class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "SmallMediaElevatedCardBorderBrush")]
						public static ResourceValue<Brush> Default => new("SmallMediaElevatedCardBorderBrush", true);

						[ResourceKeyDefinition(typeof(Brush), "SmallMediaElevatedCardBorderBrushPointerOver")]
						public static ResourceValue<Brush> PointerOver => new("SmallMediaElevatedCardBorderBrushPointerOver", true);

						[ResourceKeyDefinition(typeof(Brush), "SmallMediaElevatedCardBorderBrushFocused")]
						public static ResourceValue<Brush> Focused => new("SmallMediaElevatedCardBorderBrushFocused", true);
					}
				}

				public static class Outlined
				{
					public static class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "OutlinedCardBackground")]
						public static ResourceValue<Brush> Default => new("OutlinedCardBackground", true);
					}

					public static class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "OutlinedCardBorderBrush")]
						public static ResourceValue<Brush> Default => new("OutlinedCardBorderBrush", true);
					}
				}

				public static class AvatarOutlined
				{
					public static class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "AvatarOutlinedCardBackground")]
						public static ResourceValue<Brush> Default => new("AvatarOutlinedCardBackground", true);
					}

					public static class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "AvatarOutlinedCardBorderBrush")]
						public static ResourceValue<Brush> Default => new("AvatarOutlinedCardBorderBrush", true);
					}
				}

				public static class SmallMediaOutlined
				{
					public static class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "SmallMediaOutlinedCardBackground")]
						public static ResourceValue<Brush> Default => new("SmallMediaOutlinedCardBackground", true);
					}

					public static class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "SmallMediaOutlinedCardBorderBrush")]
						public static ResourceValue<Brush> Default => new("SmallMediaOutlinedCardBorderBrush", true);
					}
				}

				public static class Filled
				{
					public static class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "FilledCardBackground")]
						public static ResourceValue<Brush> Default => new("FilledCardBackground", true);
					}

					public static class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "FilledCardBorderBrush")]
						public static ResourceValue<Brush> Default => new("FilledCardBorderBrush", true);

						[ResourceKeyDefinition(typeof(Brush), "FilledCardBorderBrushPointerOver")]
						public static ResourceValue<Brush> PointerOver => new("FilledCardBorderBrushPointerOver", true);

						[ResourceKeyDefinition(typeof(Brush), "FilledCardBorderBrushFocused")]
						public static ResourceValue<Brush> Focused => new("FilledCardBorderBrushFocused", true);
					}
				}

				public static class AvatarFilled
				{
					public static class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "AvatarFilledCardBackground")]
						public static ResourceValue<Brush> Default => new("AvatarFilledCardBackground", true);
					}

					public static class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "AvatarFilledCardBorderBrush")]
						public static ResourceValue<Brush> Default => new("AvatarFilledCardBorderBrush", true);

						[ResourceKeyDefinition(typeof(Brush), "AvatarFilledCardBorderBrushPointerOver")]
						public static ResourceValue<Brush> PointerOver => new("AvatarFilledCardBorderBrushPointerOver", true);

						[ResourceKeyDefinition(typeof(Brush), "AvatarFilledCardBorderBrushFocused")]
						public static ResourceValue<Brush> Focused => new("AvatarFilledCardBorderBrushFocused", true);
					}
				}

				public static class SmallMediaFilled
				{
					public static class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "SmallMediaFilledCardBackground")]
						public static ResourceValue<Brush> Default => new("SmallMediaFilledCardBackground", true);
					}

					public static class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "SmallMediaFilledCardBorderBrush")]
						public static ResourceValue<Brush> Default => new("SmallMediaFilledCardBorderBrush", true);

						[ResourceKeyDefinition(typeof(Brush), "SmallMediaFilledCardBorderBrushPointerOver")]
						public static ResourceValue<Brush> PointerOver => new("SmallMediaFilledCardBorderBrushPointerOver", true);

						[ResourceKeyDefinition(typeof(Brush), "SmallMediaFilledCardBorderBrushFocused")]
						public static ResourceValue<Brush> Focused => new("SmallMediaFilledCardBorderBrushFocused", true);
					}
				}
			}
		}
	}
}
