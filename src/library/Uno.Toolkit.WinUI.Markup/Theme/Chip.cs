using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Uno.Extensions.Markup;
using Uno.Extensions.Markup.Internals;

namespace Uno.Toolkit.UI.Markup;

public static partial class Theme
{
	public static partial class Chip
	{
		public static partial class Resources
		{
			public static partial class Assist
			{
				[ResourceKeyDefinition(typeof(Brush), "ChipBorderBrush")]
				public static ThemeResourceKey<Brush> BorderBrush => new("ChipBorderBrush");

				[ResourceKeyDefinition(typeof(Thickness), "ChipBorderThickness")]
				public static ThemeResourceKey<Thickness> BorderThickness => new("ChipBorderThickness");
			}

			public static partial class Default
			{
				public static partial class Background
				{
					[ResourceKeyDefinition(typeof(Brush), "ChipBackground")]
					public static ThemeResourceKey<Brush> Default => new("ChipBackground");

					[ResourceKeyDefinition(typeof(Brush), "ChipBackgroundPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("ChipBackgroundPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "ChipBackgroundFocused")]
					public static ThemeResourceKey<Brush> Focused => new("ChipBackgroundFocused");

					[ResourceKeyDefinition(typeof(Brush), "ChipBackgroundPressed")]
					public static ThemeResourceKey<Brush> Pressed => new("ChipBackgroundPressed");

					[ResourceKeyDefinition(typeof(Brush), "ChipBackgroundDisabled")]
					public static ThemeResourceKey<Brush> Disabled => new("ChipBackgroundDisabled");

					[ResourceKeyDefinition(typeof(Brush), "ChipBackgroundChecked")]
					public static ThemeResourceKey<Brush> Checked => new("ChipBackgroundChecked");

					[ResourceKeyDefinition(typeof(Brush), "ChipBackgroundCheckedPointerOver")]
					public static ThemeResourceKey<Brush> CheckedPointerOver => new("ChipBackgroundCheckedPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "ChipBackgroundCheckedFocused")]
					public static ThemeResourceKey<Brush> CheckedFocused => new("ChipBackgroundCheckedFocused");

					[ResourceKeyDefinition(typeof(Brush), "ChipBackgroundCheckedPressed")]
					public static ThemeResourceKey<Brush> CheckedPressed => new("ChipBackgroundCheckedPressed");

					[ResourceKeyDefinition(typeof(Brush), "ChipBackgroundCheckedDisabled")]
					public static ThemeResourceKey<Brush> CheckedDisabled => new("ChipBackgroundCheckedDisabled");
				}

				public static partial class BorderBrush
				{
					[ResourceKeyDefinition(typeof(Brush), "ChipBorderBrush")]
					public static ThemeResourceKey<Brush> Default => new("ChipBorderBrush");

					[ResourceKeyDefinition(typeof(Brush), "ChipBorderBrushPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("ChipBorderBrushPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundFocused")]
					public static ThemeResourceKey<Brush> Focused => new("ChipIconForegroundFocused");

					[ResourceKeyDefinition(typeof(Brush), "ChipBorderBrushPressed")]
					public static ThemeResourceKey<Brush> Pressed => new("ChipBorderBrushPressed");

					[ResourceKeyDefinition(typeof(Brush), "ChipBorderBrushDisabled")]
					public static ThemeResourceKey<Brush> Disabled => new("ChipBorderBrushDisabled");

					[ResourceKeyDefinition(typeof(Brush), "ChipBorderBrushChecked")]
					public static ThemeResourceKey<Brush> Checked => new("ChipBorderBrushChecked");

					[ResourceKeyDefinition(typeof(Brush), "ChipBorderBrushCheckedPointerOver")]
					public static ThemeResourceKey<Brush> CheckedPointerOver => new("ChipBorderBrushCheckedPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundCheckedFocused")]
					public static ThemeResourceKey<Brush> CheckedFocused => new("ChipIconForegroundCheckedFocused");

					[ResourceKeyDefinition(typeof(Brush), "ChipBorderBrushCheckedPressed")]
					public static ThemeResourceKey<Brush> CheckedPressed => new("ChipBorderBrushCheckedPressed");

					[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundCheckedDisabled")]
					public static ThemeResourceKey<Brush> CheckedDisabled => new("ChipIconForegroundCheckedDisabled");
				}

				public static partial class Foreground
				{
					[ResourceKeyDefinition(typeof(Brush), "ChipForeground")]
					public static ThemeResourceKey<Brush> Default => new("ChipForeground");

					[ResourceKeyDefinition(typeof(Brush), "ChipForegroundPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("ChipForegroundPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "ChipForegroundFocused")]
					public static ThemeResourceKey<Brush> Focused => new("ChipForegroundFocused");

					[ResourceKeyDefinition(typeof(Brush), "ChipForegroundPressed")]
					public static ThemeResourceKey<Brush> Pressed => new("ChipForegroundPressed");

					[ResourceKeyDefinition(typeof(Brush), "ChipForegroundDisabled")]
					public static ThemeResourceKey<Brush> Disabled => new("ChipForegroundDisabled");

					[ResourceKeyDefinition(typeof(Brush), "ChipForegroundChecked")]
					public static ThemeResourceKey<Brush> Checked => new("ChipForegroundChecked");

					[ResourceKeyDefinition(typeof(Brush), "ChipForegroundCheckedPointerOver")]
					public static ThemeResourceKey<Brush> CheckedPointerOver => new("ChipForegroundCheckedPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "ChipForegroundCheckedFocused")]
					public static ThemeResourceKey<Brush> CheckedFocused => new("ChipForegroundCheckedFocused");

					[ResourceKeyDefinition(typeof(Brush), "ChipForegroundCheckedPressed")]
					public static ThemeResourceKey<Brush> CheckedPressed => new("ChipForegroundCheckedPressed");

					[ResourceKeyDefinition(typeof(Brush), "ChipForegroundCheckedDisabled")]
					public static ThemeResourceKey<Brush> CheckedDisabled => new("ChipForegroundCheckedDisabled");
				}

				public static partial class CheckGlyph
				{
					[ResourceKeyDefinition(typeof(Thickness), "ChipCheckGlyphPadding")]
					public static ThemeResourceKey<Thickness> Padding => new("ChipCheckGlyphPadding");

					[ResourceKeyDefinition(typeof(double), "ChipCheckGlyphSize")]
					public static ThemeResourceKey<double> Size => new("ChipCheckGlyphSize");
				}

				public static partial class Icon
				{
					public static partial class Foreground
					{
						[ResourceKeyDefinition(typeof(Brush), "ChipIconForeground")]
						public static ThemeResourceKey<Brush> Default => new("ChipIconForeground");

						[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundPointerOver")]
						public static ThemeResourceKey<Brush> PointerOver => new("ChipIconForegroundPointerOver");

						[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundFocused")]
						public static ThemeResourceKey<Brush> Focused => new("ChipIconForegroundFocused");

						[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundPressed")]
						public static ThemeResourceKey<Brush> Pressed => new("ChipIconForegroundPressed");

						[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundDisabled")]
						public static ThemeResourceKey<Brush> Disabled => new("ChipIconForegroundDisabled");

						[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundChecked")]
						public static ThemeResourceKey<Brush> Checked => new("ChipIconForegroundChecked");

						[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundCheckedPointerOver")]
						public static ThemeResourceKey<Brush> CheckedPointerOver => new("ChipIconForegroundCheckedPointerOver");

						[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundCheckedFocused")]
						public static ThemeResourceKey<Brush> CheckedFocused => new("ChipIconForegroundCheckedFocused");

						[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundCheckedPressed")]
						public static ThemeResourceKey<Brush> CheckedPressed => new("ChipIconForegroundCheckedPressed");

						[ResourceKeyDefinition(typeof(Brush), "ChipIconForegroundCheckedDisabled")]
						public static ThemeResourceKey<Brush> CheckedDisabled => new("ChipIconForegroundCheckedDisabled");
					}

					[ResourceKeyDefinition(typeof(double), "ChipIconSize")]
					public static ThemeResourceKey<double> Size => new("ChipIconSize");
				}

				public static partial class DeleteIcon
				{
					[ResourceKeyDefinition(typeof(Brush), "ChipDeleteIconBackground")]
					public static ThemeResourceKey<Brush> Background => new("ChipDeleteIconBackground");

					[ResourceKeyDefinition(typeof(Brush), "ChipDeleteIconForeground")]
					public static ThemeResourceKey<Brush> Foreground => new("ChipDeleteIconForeground");

					[ResourceKeyDefinition(typeof(double), "ChipDeleteIconContainerLength")]
					public static ThemeResourceKey<double> ContainerLength => new("ChipDeleteIconContainerLength");

					[ResourceKeyDefinition(typeof(double), "ChipDeleteIconLength")]
					public static ThemeResourceKey<double> Length => new("ChipDeleteIconLength");
				}

				public static partial class StateOverlay
				{
					[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlay")]
					public static ThemeResourceKey<Brush> Default => new("ChipStateOverlay");

					[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlayPointerOver")]
					public static ThemeResourceKey<Brush> PointerOver => new("ChipStateOverlayPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlayFocused")]
					public static ThemeResourceKey<Brush> Focused => new("ChipStateOverlayFocused");

					[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlayPressed")]
					public static ThemeResourceKey<Brush> Pressed => new("ChipStateOverlayPressed");

					[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlayDisabled")]
					public static ThemeResourceKey<Brush> Disabled => new("ChipStateOverlayDisabled");

					[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlayChecked")]
					public static ThemeResourceKey<Brush> Checked => new("ChipStateOverlayChecked");

					[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlayCheckedPointerOver")]
					public static ThemeResourceKey<Brush> CheckedPointerOver => new("ChipStateOverlayCheckedPointerOver");

					[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlayCheckedFocused")]
					public static ThemeResourceKey<Brush> CheckedFocused => new("ChipStateOverlayCheckedFocused");

					[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlayCheckedPressed")]
					public static ThemeResourceKey<Brush> CheckedPressed => new("ChipStateOverlayCheckedPressed");

					[ResourceKeyDefinition(typeof(Brush), "ChipStateOverlayCheckedDisabled")]
					public static ThemeResourceKey<Brush> CheckedDisabled => new("ChipStateOverlayCheckedDisabled");
				}

				public static partial class Elevation
				{
					[ResourceKeyDefinition(typeof(double), "ChipElevation")]
					public static ThemeResourceKey<double> Default => new("ChipElevation");

					[ResourceKeyDefinition(typeof(double), "ChipElevationDisabled")]
					public static ThemeResourceKey<double> Disabled => new("ChipElevationDisabled");
				}


				[ResourceKeyDefinition(typeof(Thickness), "ChipBorderThickness")]
				public static ThemeResourceKey<Thickness> BorderThickness => new("ChipBorderThickness");

				[ResourceKeyDefinition(typeof(CornerRadius), "ChipCornerRadius")]
				public static ThemeResourceKey<CornerRadius> CornerRadius => new("ChipCornerRadius");

				[ResourceKeyDefinition(typeof(double), "ChipSize")]
				public static ThemeResourceKey<double> Size => new("ChipSize");

				[ResourceKeyDefinition(typeof(double), "ChipHeight")]
				public static ThemeResourceKey<double> Height => new("ChipHeight");

				[ResourceKeyDefinition(typeof(double), "ChipContentMinHeight")]
				public static ThemeResourceKey<double> ContentMinHeight => new("ChipContentMinHeight");

				[ResourceKeyDefinition(typeof(Thickness), "ChipPadding")]
				public static ThemeResourceKey<Thickness> Padding => new("ChipPadding");

				[ResourceKeyDefinition(typeof(Thickness), "ChipContentMargin")]
				public static ThemeResourceKey<Thickness> ContentMargin => new("ChipContentMargin");

				[ResourceKeyDefinition(typeof(Thickness), "ChipElevationMargin")]
				public static ThemeResourceKey<Thickness> ElevationMargin => new("ChipElevationMargin");
			}
		}

		public static partial class Styles
		{
			[ResourceKeyDefinition(typeof(Style), "ChipStyle", TargetType = typeof(global::Uno.Toolkit.UI.Chip))]
			public static StaticResourceKey<Style> Default => new("ChipStyle");

			[ResourceKeyDefinition(typeof(Style), "AssistChipStyle", TargetType = typeof(global::Uno.Toolkit.UI.Chip))]
			public static StaticResourceKey<Style> Assist => new("AssistChipStyle");

			[ResourceKeyDefinition(typeof(Style), "ElevatedAssistChipStyle", TargetType = typeof(global::Uno.Toolkit.UI.Chip))]
			public static StaticResourceKey<Style> ElevatedAssist => new("ElevatedAssistChipStyle");

			[ResourceKeyDefinition(typeof(Style), "InputChipStyle", TargetType = typeof(global::Uno.Toolkit.UI.Chip))]
			public static StaticResourceKey<Style> Input => new("InputChipStyle");

			[ResourceKeyDefinition(typeof(Style), "FilterChipStyle", TargetType = typeof(global::Uno.Toolkit.UI.Chip))]
			public static StaticResourceKey<Style> Filter => new("FilterChipStyle");

			[ResourceKeyDefinition(typeof(Style), "ElevatedFilterChipStyle", TargetType = typeof(global::Uno.Toolkit.UI.Chip))]
			public static StaticResourceKey<Style> ElevatedFilter => new("ElevatedFilterChipStyle");

			[ResourceKeyDefinition(typeof(Style), "SuggestionChipStyle", TargetType = typeof(global::Uno.Toolkit.UI.Chip))]
			public static StaticResourceKey<Style> Suggestion => new("SuggestionChipStyle");

			[ResourceKeyDefinition(typeof(Style), "ElevatedSuggestionChipStyle", TargetType = typeof(global::Uno.Toolkit.UI.Chip))]
			public static StaticResourceKey<Style> ElevatedSuggestion => new("ElevatedSuggestionChipStyle");
		}
	}
}
