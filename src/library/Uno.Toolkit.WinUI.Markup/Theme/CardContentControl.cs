using Microsoft.UI.Xaml.Media;
using Uno.Extensions.Markup.Internals;

namespace Uno.Toolkit.Markup
{
	public static partial class Theme
	{
		public static class CardContentControl
		{
			public static class Resources
			{
				public static class Elevated
				{
					public static class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "ElevatedCardContentBackground")]
						public static ResourceValue<Brush> Default => new("ElevatedCardContentBackground", true);
					}

					public static class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "ElevatedCardContentBorderBrush")]
						public static ResourceValue<Brush> Default => new("ElevatedCardContentBorderBrush", true);

						[ResourceKeyDefinition(typeof(Brush), "ElevatedCardContentBorderBrushPointerOver")]
						public static ResourceValue<Brush> PointerOver => new("ElevatedCardContentBorderBrushPointerOver", true);

						[ResourceKeyDefinition(typeof(Brush), "ElevatedCardContentBorderBrushFocused")]
						public static ResourceValue<Brush> Focused => new("ElevatedCardContentBorderBrushFocused", true);

						[ResourceKeyDefinition(typeof(Brush), "ElevatedCardContentBorderBrushPressed")]
						public static ResourceValue<Brush> Pressed => new("ElevatedCardContentBorderBrushPressed", true);
					}
				}

				public static class Outlined
				{
					public static class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "OutlinedCardContentBackground")]
						public static ResourceValue<Brush> Default => new("OutlinedCardContentBackground", true);
					}

					public static class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "OutlinedCardContentBorderBrush")]
						public static ResourceValue<Brush> Default => new("OutlinedCardContentBorderBrush", true);
					}
				}

				public static class Filled
				{
					public static class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "FilledCardContentBackground")]
						public static ResourceValue<Brush> Default => new("FilledCardContentBackground", true);
					}

					public static class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "FilledCardContentBorderBrush")]
						public static ResourceValue<Brush> Default => new("FilledCardContentBorderBrush", true);

						[ResourceKeyDefinition(typeof(Brush), "FilledCardContentBorderBrushPointerOver")]
						public static ResourceValue<Brush> PointerOver => new("FilledCardContentBorderBrushPointerOver", true);

						[ResourceKeyDefinition(typeof(Brush), "FilledCardContentBorderBrushFocused")]
						public static ResourceValue<Brush> Focused => new("FilledCardContentBorderBrushFocused", true);

						[ResourceKeyDefinition(typeof(Brush), "FilledCardContentBorderBrushPressed")]
						public static ResourceValue<Brush> Pressed => new("FilledCardContentBorderBrushPressed", true);
					}
				}
			}
		}
	}
}
