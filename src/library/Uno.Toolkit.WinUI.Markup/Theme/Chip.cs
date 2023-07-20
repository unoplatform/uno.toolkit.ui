using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Uno.Extensions.Markup.Internals;

namespace Uno.Toolkit.Markup
{
	public static partial class Theme
	{
		public static class Chip
		{
			public static class Resources
			{
				public static class Default
				{
					public static class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "ChipBackground")]
						public static ResourceValue<Brush> Default => new("ChipBackground", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipBackgroundPointerOver")]
						public static ResourceValue<Brush> PointerOver => new("ChipBackgroundPointerOver", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipBackgroundFocused")]
						public static ResourceValue<Brush> Focused => new("ChipBackgroundFocused", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipBackgroundPressed")]
						public static ResourceValue<Brush> Pressed => new("ChipBackgroundPressed", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipBackgroundDisabled")]
						public static ResourceValue<Brush> Disabled => new("ChipBackgroundDisabled", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipBackgroundChecked")]
						public static ResourceValue<Brush> Checked => new("ChipBackgroundChecked", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipBackgroundCheckedPointerOver")]
						public static ResourceValue<Brush> CheckedPointerOver => new("ChipBackgroundCheckedPointerOver", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipBackgroundCheckedFocused")]
						public static ResourceValue<Brush> CheckedFocused => new("ChipBackgroundCheckedFocused", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipBackgroundCheckedPressed")]
						public static ResourceValue<Brush> CheckedPressed => new("ChipBackgroundCheckedPressed", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipBackgroundCheckedDisabled")]
						public static ResourceValue<Brush> CheckedDisabled => new("ChipBackgroundCheckedDisabled", true);
					}

					public static class Foreground
					{
						[ResourceKeyDefinition(typeof(Brush), "ChipForeground")]
						public static ResourceValue<Brush> Default => new("ChipForeground", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipForegroundPointerOver")]
						public static ResourceValue<Brush> PointerOver => new("ChipForegroundPointerOver", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipForegroundFocused")]
						public static ResourceValue<Brush> Focused => new("ChipForegroundFocused", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipForegroundPressed")]
						public static ResourceValue<Brush> Pressed => new("ChipForegroundPressed", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipForegroundDisabled")]
						public static ResourceValue<Brush> Disabled => new("ChipForegroundDisabled", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipForegroundChecked")]
						public static ResourceValue<Brush> Checked => new("ChipForegroundChecked", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipForegroundCheckedPointerOver")]
						public static ResourceValue<Brush> CheckedPointerOver => new("ChipForegroundCheckedPointerOver", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipForegroundCheckedFocused")]
						public static ResourceValue<Brush> CheckedFocused => new("ChipForegroundCheckedFocused", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipForegroundCheckedPressed")]
						public static ResourceValue<Brush> CheckedPressed => new("ChipForegroundCheckedPressed", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipForegroundCheckedDisabled")]
						public static ResourceValue<Brush> CheckedDisabled => new("ChipForegroundCheckedDisabled", true);
					}

					public static class BorderBrush
					{
						[ResourceKeyDefinition(typeof(Brush), "ChipBorderBrush")]
						public static ResourceValue<Brush> Default => new("ChipBorderBrush", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipBorderBrushPointerOver")]
						public static ResourceValue<Brush> PointerOver => new("ChipBorderBrushPointerOver", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipBorderBrushFocused")]
						public static ResourceValue<Brush> Focused => new("ChipBorderBrushFocused", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipBorderBrushPressed")]
						public static ResourceValue<Brush> Pressed => new("ChipBorderBrushPressed", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipBorderBrushDisabled")]
						public static ResourceValue<Brush> Disabled => new("ChipBorderBrushDisabled", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipBorderBrushChecked")]
						public static ResourceValue<Brush> Checked => new("ChipBorderBrushChecked", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipBorderBrushCheckedPointerOver")]
						public static ResourceValue<Brush> CheckedPointerOver => new("ChipBorderBrushCheckedPointerOver", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipBorderBrushCheckedFocused")]
						public static ResourceValue<Brush> CheckedFocused => new("ChipBorderBrushCheckedFocused", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipBorderBrushCheckedPressed")]
						public static ResourceValue<Brush> CheckedPressed => new("ChipBorderBrushCheckedPressed", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipBorderBrushCheckedDisabled")]
						public static ResourceValue<Brush> CheckedDisabled => new("ChipBorderBrushCheckedDisabled", true);
					}

					public static class BorderThickness
					{
						[ResourceKeyDefinition(typeof(Thickness), "ChipBorderThickness")]
						public static ResourceValue<Thickness> Default => new("ChipBorderThickness", true);
					}

					public static class CheckGlyph
					{
						public static class Padding
						{
							[ResourceKeyDefinition(typeof(Thickness), "ChipCheckGlyphPadding")]
							public static ResourceValue<Thickness> Default => new("ChipCheckGlyphPadding", true);
						}

						public static class Size
						{
							[ResourceKeyDefinition(typeof(double), "ChipCheckGlyphSize")]
							public static ResourceValue<double> Default => new("ChipCheckGlyphSize", true);
						}
					}

					public static class Content
					{
						public static class Margin
						{
							[ResourceKeyDefinition(typeof(Thickness), "ChipContentMargin")]
							public static ResourceValue<Thickness> Default => new("ChipContentMargin", true);
						}
						
						public static class MinHeight
						{
							[ResourceKeyDefinition(typeof(double), "ChipContentMinHeight")]
							public static ResourceValue<double> Default => new("ChipContentMinHeight", true);
						}
					}

					public static class CornerRadiuses
					{
						[ResourceKeyDefinition(typeof(CornerRadius), "ChipCornerRadius")]
						public static ResourceValue<CornerRadius> Default => new("ChipCornerRadius", true);
					}

					public static class DeleteIcon
					{
						public static class Background
						{
							[ResourceKeyDefinition(typeof(Brush), "ChipDeleteIconBackground")]
							public static ResourceValue<Brush> Default => new("ChipDeleteIconBackground", true);
						}

						public static class Foreground
						{
							[ResourceKeyDefinition(typeof(Brush), "ChipDeleteIconForeground")]
							public static ResourceValue<Brush> Default => new("ChipDeleteIconForeground", true);
						}

						public static class Container
						{
							public static class Length
							{
								[ResourceKeyDefinition(typeof(double), "ChipDeleteIconContainerLength")]
								public static ResourceValue<double> Default => new("ChipDeleteIconContainerLength", true);
							}
						}

						public static class Length
						{
							[ResourceKeyDefinition(typeof(double), "ChipDeleteIconLength")]
							public static ResourceValue<double> Default => new("ChipDeleteIconLength", true);
						}
					}

					public static class Elevation
					{
						[ResourceKeyDefinition(typeof(double), "ChipElevation")]
						public static ResourceValue<double> Default => new("ChipElevation", true);

						[ResourceKeyDefinition(typeof(double), "ChipElevationDisabled")]
						public static ResourceValue<double> Disabled => new("ChipElevationDisabled", true);

						public static class BorderThickness
						{
							[ResourceKeyDefinition(typeof(Thickness), "ChipElevationBorderThickness")]
							public static ResourceValue<Thickness> Default => new("ChipElevationBorderThickness", true);
						}

						public static class Margin
						{
							[ResourceKeyDefinition(typeof(Thickness), "ChipElevationMargin")]
							public static ResourceValue<Thickness> Default => new("ChipElevationMargin", true);
						}
					}

					public static class Height
					{
						[ResourceKeyDefinition(typeof(double), "ChipHeight")]
						public static ResourceValue<double> Default => new("ChipHeight", true);
					}

					public static class Icon
					{
						public static class Foreground
						{
							[ResourceKeyDefinition(typeof(Brush), "ChipIconForeground")]
							public static ResourceValue<Brush> Default => new("ChipIconForeground", true);

							[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundPointerOver")]
							public static ResourceValue<Brush> PointerOver => new("ChipIconForegroundPointerOver", true);

							[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundFocused")]
							public static ResourceValue<Brush> Focused => new("ChipIconForegroundFocused", true);

							[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundPressed")]
							public static ResourceValue<Brush> Pressed => new("ChipIconForegroundPressed", true);

							[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundDisabled")]
							public static ResourceValue<Brush> Disabled => new("ChipIconForegroundDisabled", true);

							[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundChecked")]
							public static ResourceValue<Brush> Checked => new("ChipIconForegroundChecked", true);

							[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundCheckedPointerOver")]
							public static ResourceValue<Brush> CheckedPointerOver => new("ChipIconForegroundCheckedPointerOver", true);

							[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundCheckedFocused")]
							public static ResourceValue<Brush> CheckedFocused => new("ChipIconForegroundCheckedFocused", true);

							[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundCheckedPressed")]
							public static ResourceValue<Brush> CheckedPressed => new("ChipIconForegroundCheckedPressed", true);

							[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundCheckedDisabled")]
							public static ResourceValue<Brush> CheckedDisabled => new("ChipIconForegroundCheckedDisabled", true);
						}

						public static class Size
						{
							[ResourceKeyDefinition(typeof(double), "ChipIconSize")]
							public static ResourceValue<double> Default => new("ChipIconSize", true);
						}
					}

					public static class Padding
					{
						[ResourceKeyDefinition(typeof(Thickness), "ChipPadding")]
						public static ResourceValue<Thickness> Default => new("ChipPadding", true);
					}

					public static class RippleFeedback
					{
						[ResourceKeyDefinition(typeof(Brush), "ChipRippleFeedback")]
						public static ResourceValue<Brush> Default => new("ChipRippleFeedback", true);
					}

					public static class Size
					{
						[ResourceKeyDefinition(typeof(double), "ChipSize")]
						public static ResourceValue<double> Default => new("ChipSize", true);
					}

					public static class StateOverlay
					{
						[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlay")]
						public static ResourceValue<Brush> Default => new("ChipStateOverlay", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlayPointerOver")]
						public static ResourceValue<Brush> PointerOver => new("ChipStateOverlayPointerOver", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlayFocused")]
						public static ResourceValue<Brush> Focused => new("ChipStateOverlayFocused", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlayPressed")]
						public static ResourceValue<Brush> Pressed => new("ChipStateOverlayPressed", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlayDisabled")]
						public static ResourceValue<Brush> Disabled => new("ChipStateOverlayDisabled", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlayChecked")]
						public static ResourceValue<Brush> Checked => new("ChipStateOverlayChecked", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlayCheckedPointerOver")]
						public static ResourceValue<Brush> CheckedPointerOver => new("ChipStateOverlayCheckedPointerOver", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlayCheckedFocused")]
						public static ResourceValue<Brush> CheckedFocused => new("ChipStateOverlayCheckedFocused", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlayCheckedPressed")]
						public static ResourceValue<Brush> CheckedPressed => new("ChipStateOverlayCheckedPressed", true);

						[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlayCheckedDisabled")]
						public static ResourceValue<Brush> CheckedDisabled => new("ChipStateOverlayCheckedDisabled", true);
					}
				}

				public static class Elevated
				{
					public static class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "ElevatedChipBackground")]
						public static ResourceValue<Brush> Default => new("ElevatedChipBackground", true);
					}
				}
			}
		}
	}
}
