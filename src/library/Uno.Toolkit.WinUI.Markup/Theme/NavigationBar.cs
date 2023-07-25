using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Uno.Extensions.Markup.Internals;

namespace Uno.Toolkit.Markup
{
	public static partial class Theme
	{
		public static class AppBar
		{
			public static class EllipsisButton
			{
				public static class Margin
				{
					public static class InnerBorder
					{
						[ResourceKeyDefinition(typeof(Thickness), "MaterialAppBarEllipsisButtonInnerBorderMargin")]
						public static ResourceValue<Thickness> Default => new("MaterialAppBarEllipsisButtonInnerBorderMargin", true);
					}
				}
			}
		}

		public static class NavigationBar
		{
			public static class Resources
			{
				public static class Default
				{
					public static class Foreground
					{
						[ResourceKeyDefinition(typeof(Brush), "NavigationBarForeground")]
						public static ResourceValue<Brush> Default => new("NavigationBarForeground", true);
					}

					public static class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "NavigationBarBackground")]
						public static ResourceValue<Brush> Default => new("NavigationBarBackground", true);
					}

					public static class Content
					{
						public static class Margin
						{
							[ResourceKeyDefinition(typeof(Thickness), "MaterialNavigationBarContentMargin")]
							public static ResourceValue<Thickness> Default => new("MaterialNavigationBarContentMargin", true);
						}
					}

					public static class Elevation
					{
						[ResourceKeyDefinition(typeof(double), "MaterialNavigationBarElevation")]
						public static ResourceValue<double> Default => new("MaterialNavigationBarElevation", true);
					}

					public static class Height
					{
						[ResourceKeyDefinition(typeof(double), "MaterialNavigationBarHeight")]
						public static ResourceValue<double> Default => new("MaterialNavigationBarHeight", true);
					}

					public static class Padding
					{
						[ResourceKeyDefinition(typeof(Thickness), "NavigationBarPadding")]
						public static ResourceValue<Thickness> Default => new("NavigationBarPadding", true);
					}

					public static class AppBar
					{
						public static class Theme
						{
							public static class Height
							{
								[ResourceKeyDefinition(typeof(double), "NavBarAppBarThemeCompactHeight")]
								public static ResourceValue<double> Compact => new("NavBarAppBarThemeCompactHeight", true);
							}
						}

						public static class Button
						{
							public static class Content
							{
								public static class Height
								{
									[ResourceKeyDefinition(typeof(double), "NavBarAppBarButtonContentHeight")]
									public static ResourceValue<double> Default => new("NavBarAppBarButtonContentHeight", true);
								}
							}

							public static class Padding
							{
								[ResourceKeyDefinition(typeof(Thickness), "NavBarAppBarButtonPadding")]
								public static ResourceValue<Thickness> Default => new("NavBarAppBarButtonPadding", true);
							}

							public static class ChevronVisibility
							{
								[ResourceKeyDefinition(typeof(Visibility), "NavBarAppBarButtonHasFlyoutChevronVisibility")]
								public static ResourceValue<Visibility> Default => new("NavBarAppBarButtonHasFlyoutChevronVisibility", true);
							}
						}
					}

					public static class Typography
					{
						[ResourceKeyDefinition(typeof(FontFamily), "NavigationBarFontFamily")]
						public static ResourceValue<FontFamily> FontFamily => new("NavigationBarFontFamily", true);

						[ResourceKeyDefinition(typeof(FontWeights), "NavigationBarFontWeight")]
						public static ResourceValue<FontWeights> FontWeight => new("NavigationBarFontWeight", true);

						[ResourceKeyDefinition(typeof(double), "NavigationBarFontSize")]
						public static ResourceValue<double> FontSize => new("NavigationBarFontSize", true);
					}

					public static class EllipsisButton
					{
						public static class Foreground
						{
							[ResourceKeyDefinition(typeof(Brush), "NavigationBarEllipsisButtonForeground")]
							public static ResourceValue<Brush> Default => new("NavigationBarEllipsisButtonForeground", true);
						}

						public static class Background
						{
							[ResourceKeyDefinition(typeof(Brush), "NavigationBarEllipsisButtonBackground")]
							public static ResourceValue<Brush> Default => new("NavigationBarEllipsisButtonBackground", true);
						}

						public static class Typography
						{
							[ResourceKeyDefinition(typeof(FontFamily), "NavigationBarMaterialEllipsisButtonFontFamily")]
							public static ResourceValue<FontFamily> FontFamily => new("NavigationBarMaterialEllipsisButtonFontFamily", true);

							[ResourceKeyDefinition(typeof(FontWeights), "NavigationBarMaterialEllipsisButtonFontWeight")]
							public static ResourceValue<FontWeights> FontWeight => new("NavigationBarMaterialEllipsisButtonFontWeight", true);

							[ResourceKeyDefinition(typeof(double), "NavigationBarMaterialEllipsisButtonFontSize")]
							public static ResourceValue<double> FontSize => new("NavigationBarMaterialEllipsisButtonFontSize", true);
						}

						public static class Width
						{
							[ResourceKeyDefinition(typeof(double), "NavigationBarMaterialEllipsisButtonWidth")]
							public static ResourceValue<double> Default => new("NavigationBarMaterialEllipsisButtonWidth", true);
						}
					}

					public static class OverflowAppBar
					{
						public static class Button
						{
							public static class Foreground
							{
								[ResourceKeyDefinition(typeof(Brush), "NavigationBarOverflowAppBarButtonForeground")]
								public static ResourceValue<Brush> Default => new("NavigationBarOverflowAppBarButtonForeground", true);
							}

							public static class Background
							{
								[ResourceKeyDefinition(typeof(Brush), "NavigationBarOverflowAppBarButtonBackground")]
								public static ResourceValue<Brush> Default => new("NavigationBarOverflowAppBarButtonBackground", true);
							}
						}
					}

					public static class CommandBar
					{
						public static class Background
						{
							[ResourceKeyDefinition(typeof(Brush), "NavigationBarCommandBarBackgroundCompactOpenUp")]
							public static ResourceValue<Brush> CompactOpenUp => new("NavigationBarCommandBarBackgroundCompactOpenUp", true);

							[ResourceKeyDefinition(typeof(Brush), "NavigationBarCommandBarBackgroundCompactOpenDown")]
							public static ResourceValue<Brush> CompactOpenDown => new("NavigationBarCommandBarBackgroundCompactOpenDown", true);
						}

						public static class EllipsisIcon
						{
							public static class Foreground
							{
								[ResourceKeyDefinition(typeof(Brush), "NavigationBarCommandBarEllipsisIconForegroundDisabled")]
								public static ResourceValue<Brush> Disabled => new("NavigationBarCommandBarEllipsisIconForegroundDisabled", true);
							}
						}
					}

					public static class MainCommand
					{
						public static class Foreground
						{
							[ResourceKeyDefinition(typeof(Brush), "NavigationBarMainCommandForeground")]
							public static ResourceValue<Brush> Default => new("NavigationBarMainCommandForeground", true);
						}

						public static class AppBar
						{
							public static class Button
							{
								public static class Content
								{
									public static class Height
									{
										[ResourceKeyDefinition(typeof(double), "NavBarMainCommandAppBarButtonContentHeight")]
										public static ResourceValue<double> Default => new("NavBarMainCommandAppBarButtonContentHeight", true);
									}
								}
							}
						}
					}
				}

				public static class Modal
				{
					public static class Foreground
					{
						[ResourceKeyDefinition(typeof(Brush), "MaterialModalNavigationBarForeground")]
						public static ResourceValue<Brush> Default => new("MaterialModalNavigationBarForeground", true);
					}

					public static class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "MaterialModalNavigationBarBackground")]
						public static ResourceValue<Brush> Default => new("MaterialModalNavigationBarBackground", true);
					}

					public static class MainCommand
					{
						public static class Foreground
						{
							[ResourceKeyDefinition(typeof(Brush), "MaterialModalNavigationBarMainCommandForeground")]
							public static ResourceValue<Brush> Default => new("MaterialModalNavigationBarMainCommandForeground", true);
						}
					}
				}

				public static class Primary
				{
					public static class Foreground
					{
						[ResourceKeyDefinition(typeof(Brush), "MaterialPrimaryNavigationBarForeground")]
						public static ResourceValue<Brush> Default => new("MaterialPrimaryNavigationBarForeground", true);
					}

					public static class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "MaterialPrimaryNavigationBarBackground")]
						public static ResourceValue<Brush> Default => new("MaterialPrimaryNavigationBarBackground", true);
					}

					public static class AppBar
					{
						public static class Button
						{
							public static class Foreground
							{
								[ResourceKeyDefinition(typeof(Brush), "MaterialPrimaryAppBarButtonForeground")]
								public static ResourceValue<Brush> Default => new("MaterialPrimaryAppBarButtonForeground", true);
							}
						}
					}

					public static class CommandBar
					{
						public static class Background
						{
							[ResourceKeyDefinition(typeof(Brush), "MaterialPrimaryNavigationBarCommandBarBackgroundCompactOpenUp")]
							public static ResourceValue<Brush> CompactOpenUp => new("MaterialPrimaryNavigationBarCommandBarBackgroundCompactOpenUp", true);

							[ResourceKeyDefinition(typeof(Brush), "MaterialPrimaryNavigationBarCommandBarBackgroundCompactOpenDown")]
							public static ResourceValue<Brush> CompactOpenDown => new("MaterialPrimaryNavigationBarCommandBarBackgroundCompactOpenDown", true);
						}

						public static class EllipsisIcon
						{
							public static class Foreground
							{
								[ResourceKeyDefinition(typeof(Brush), "MaterialPrimaryNavigationBarCommandBarEllipsisIconForegroundDisabled")]
								public static ResourceValue<Brush> Disabled => new("MaterialPrimaryNavigationBarCommandBarEllipsisIconForegroundDisabled", true);
							}
						}
					}

					public static class MainCommand
					{
						public static class Foreground
						{
							[ResourceKeyDefinition(typeof(Brush), "MaterialPrimaryNavigationBarMainCommandForeground")]
							public static ResourceValue<Brush> Default => new("MaterialPrimaryNavigationBarMainCommandForeground", true);
						}
					}
				}

				public static class PrimaryModal
				{
					public static class Foreground
					{
						[ResourceKeyDefinition(typeof(Brush), "MaterialPrimaryModalNavigationBarForeground")]
						public static ResourceValue<Brush> Default => new("MaterialPrimaryModalNavigationBarForeground", true);
					}

					public static class Background
					{
						[ResourceKeyDefinition(typeof(Brush), "MaterialPrimaryModalNavigationBarBackground")]
						public static ResourceValue<Brush> Default => new("MaterialPrimaryModalNavigationBarBackground", true);
					}

					public static class MainCommand
					{
						public static class Foreground
						{
							[ResourceKeyDefinition(typeof(Brush), "MaterialPrimaryModalNavigationBarMainCommandForeground")]
							public static ResourceValue<Brush> Default => new("MaterialPrimaryModalNavigationBarMainCommandForeground", true);
						}
					}
				}

				public static class Xaml
				{
					public static class Height
					{
						[ResourceKeyDefinition(typeof(double), "MaterialXamlNavigationBarHeight")]
						public static ResourceValue<double> Default => new("MaterialXamlNavigationBarHeight", true);
					}
				}
			}
		}
	}
}
