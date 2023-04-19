#if __IOS__
using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Uno.Disposables;
using Uno.Extensions;
using Uno.Toolkit.UI.Helpers;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif

namespace Uno.Toolkit.UI
{
	internal partial class NavigationBarRenderer : Renderer<NavigationBar, UINavigationBar>
	{
		public NavigationBarRenderer(NavigationBar element) : base(element) { }

		protected override UINavigationBar CreateNativeInstance()
		{
			var navigationBar = new UINavigationBar();
			if (Element is { } element)
			{
				var navigationItem = element.GetRenderer(() => new NavigationBarNavigationItemRenderer(element)).Native;
				navigationBar.PushNavigationItem(navigationItem!, false);
			}

			return navigationBar;
		}

		protected override IEnumerable<IDisposable> Initialize()
		{
			if (Element is not { } element)
			{
				yield break;
			}

			yield return element.RegisterDisposableNestedPropertyChangedCallback(
				(s, e) => Invalidate(),
				new[] { NavigationBar.VisibilityProperty },
				new[] { NavigationBar.PrimaryCommandsProperty },
				new[] { NavigationBar.ContentProperty },
				new[] { NavigationBar.ForegroundProperty },
				new[] { NavigationBar.ForegroundProperty, SolidColorBrush.ColorProperty },
				new[] { NavigationBar.ForegroundProperty, SolidColorBrush.OpacityProperty },
				new[] { NavigationBar.BackgroundProperty },
				new[] { NavigationBar.BackgroundProperty, SolidColorBrush.ColorProperty },
				new[] { NavigationBar.BackgroundProperty, SolidColorBrush.OpacityProperty },
				new[] { NavigationBar.MainCommandProperty, AppBarButton.ForegroundProperty },
				new[] { NavigationBar.MainCommandProperty, AppBarButton.IconProperty }
			);
		}

		protected override void Render()
		{
			var native = Native ?? throw new ArgumentNullException(nameof(Native));
			var element = Element ?? throw new ArgumentNullException(nameof(Element));

			ApplyVisibility();
			var appearance = new UINavigationBarAppearance();

			// Background
			if (ColorHelper.TryGetColorWithOpacity(element.Background, out var backgroundColor))
			{
				switch (backgroundColor)
				{
					case { } opaqueColor when opaqueColor.A == byte.MaxValue:
						if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
						{
							appearance.ConfigureWithOpaqueBackground();
							appearance.BackgroundColor = opaqueColor;
						}
						else
						{
							// Prefer BarTintColor because it supports smooth transitions
							native.BarTintColor = opaqueColor;
							native.Translucent = false; //Make fully opaque for consistency with SetBackgroundImage
							native.SetBackgroundImage(null, UIBarMetrics.Default);
							native.ShadowImage = null;
						}
						break;
					case { } semiTransparentColor when semiTransparentColor.A > 0:
						if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
						{
							appearance.ConfigureWithDefaultBackground();
							appearance.BackgroundColor = semiTransparentColor;
						}
						else
						{
							native.BarTintColor = null;
							// Use SetBackgroundImage as hack to support semi-transparent background
							native.SetBackgroundImage(((UIColor)semiTransparentColor).ToUIImage(), UIBarMetrics.Default);
							native.Translucent = true;
							native.ShadowImage = null;
						}
						break;
					case { } transparent when transparent.A == 0:
						if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
						{
							appearance.ConfigureWithTransparentBackground();
							appearance.BackgroundColor = transparent;
						}
						else
						{
							native.BarTintColor = null;
							native.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
							// We make sure a transparent bar doesn't cast a shadow.
							native.ShadowImage = new UIImage(); // Removes the default 1px line
							native.Translucent = true;
						}
						break;
					default: //Background is null
						if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
						{
							appearance.ConfigureWithDefaultBackground();
							appearance.BackgroundColor = null;
						}
						else
						{
							native.BarTintColor = null;
							native.SetBackgroundImage(null, UIBarMetrics.Default); // Restores the default blurry background
							native.ShadowImage = null; // Restores the default 1px line
							native.Translucent = true;
						}
						break;
				}
			}
			else
			{
				if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
				{
					appearance.ConfigureWithDefaultBackground();
					appearance.BackgroundColor = null;
				}
				else
				{
					native.BarTintColor = null;
					native.SetBackgroundImage(null, UIBarMetrics.Default); // Restores the default blurry background
					native.ShadowImage = null; // Restores the default 1px line
					native.Translucent = true;
				}
			}

			// Foreground
			if (ColorHelper.TryGetColorWithOpacity(element.Foreground, out var foregroundColor))
			{
				if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
				{
					appearance.TitleTextAttributes = new UIStringAttributes
					{
						ForegroundColor = foregroundColor,
					};

					appearance.LargeTitleTextAttributes = new UIStringAttributes
					{
						ForegroundColor = foregroundColor,
					};
				}
				else
				{
					native.TitleTextAttributes = new UIStringAttributes
					{
						ForegroundColor = foregroundColor,
					};
				}
			}
			else
			{
				if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
				{
					appearance.TitleTextAttributes = new UIStringAttributes();
					appearance.LargeTitleTextAttributes = new UIStringAttributes();
				}
				else
				{
					native.TitleTextAttributes = null!;
				}
			}

			var mainCommand = element.GetValue(NavigationBar.MainCommandProperty) as AppBarButton;

			// MainCommand.Foreground
			Windows.UI.Color? mainForeground = null;

			if (mainCommand is { }
				&& ColorHelper.TryGetColorWithOpacity(mainCommand.Foreground, out var mainCommandForeground))
			{
				native.TintColor = mainForeground = mainCommandForeground;
			}

			// MainCommand.Icon
			var mainCommandIcon =
				mainCommand?.Icon is BitmapIcon bitmapIcon
				? ImageHelper.FromUri(bitmapIcon.UriSource)?.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal)
				: null;

			if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
			{
				var backButtonAppearance = new UIBarButtonItemAppearance(UIBarButtonItemStyle.Plain);

				if (mainForeground is { } foreground)
				{
					var titleTextAttributes = new UIStringAttributes
					{
						ForegroundColor = foreground
					};

					var attributes = titleTextAttributes.Dictionary.ToNSDictionary<NSString, NSObject>();

					backButtonAppearance.Normal.TitleTextAttributes = attributes;
					backButtonAppearance.Highlighted.TitleTextAttributes = attributes;
					
					if (mainCommandIcon is { } image)
					{
						var tintedImage = image.ApplyTintColor(foreground);
						appearance.SetBackIndicatorImage(tintedImage, tintedImage);
					}
					else if (appearance.BackIndicatorImage is { } backImage)
					{
						var tintedBack = backImage.ApplyTintColor(foreground);
						appearance.SetBackIndicatorImage(tintedBack, tintedBack);
					}
				}
				else
				{
					if (mainCommandIcon is { } image)
					{
						appearance.SetBackIndicatorImage(image, image);
					}
				}

				appearance.BackButtonAppearance = backButtonAppearance;
			}
			else
			{
				native.BackIndicatorImage = mainCommandIcon;
				native.BackIndicatorTransitionMaskImage = mainCommandIcon;
			}

			// Remove 1px "shadow" line from the bottom of UINavigationBar
			// The legacy customization (iOS12 and lower) to remove this shadow is done in the above switch for Background property
			if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
			{
				appearance.ShadowColor = UIColor.Clear;
			}

			native.CompactAppearance = appearance;
			native.StandardAppearance = appearance;
			native.ScrollEdgeAppearance = appearance;
		}

		private void ApplyVisibility()
		{
			var newHidden = Element?.Visibility == Visibility.Collapsed;
			var hasChanged = Native.Hidden != newHidden;
			Native.Hidden = newHidden;
			if (hasChanged)
			{
				// Re-layout UINavigationBar when visibility changes, this is important eg in the case that status bar was shown/hidden
				// while CommandBar was collapsed
				Native.SetNeedsLayout();
				Native.Superview?.SetNeedsLayout();
			}
		}
	}

}
#endif
