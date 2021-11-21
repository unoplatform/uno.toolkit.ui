#if __IOS__
using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Uno.Disposables;
using Uno.Extensions;
using Uno.Toolkit.UI.Extensions;
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

namespace Uno.Toolkit.UI.Controls
{
	internal partial class NavigationBarRenderer : Renderer<NavigationBar, UINavigationBar>
	{
		public NavigationBarRenderer(NavigationBar element) : base(element) { }

		protected override UINavigationBar CreateNativeInstance()
		{
			var navigationBar = new UINavigationBar();
			if (Element is { })
			{
				var navigationItem = Element.GetRenderer(() => new NavigationBarNavigationItemRenderer(Element)).Native;
				navigationBar.PushNavigationItem(navigationItem, false);
			}

			return navigationBar;
		}

		protected override IEnumerable<IDisposable> Initialize()
		{
			if (Element == null)
			{
				yield break;
			}

			yield return Element.RegisterDisposableNestedPropertyChangedCallback(
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
			if (Element is null)
			{
				return;
			}

			ApplyVisibility();

			// Foreground
			if (ColorHelper.TryGetColorWithOpacity(Element.Foreground, out var foregroundColor))
			{
				Native.TitleTextAttributes = new UIStringAttributes
				{
					ForegroundColor = foregroundColor,
				};
			}
			else
			{
				Native.TitleTextAttributes = null!;
			}

			// Background
			ColorHelper.TryGetColorWithOpacity(Element.Background, out var backgroundColor);
			switch (backgroundColor)
			{
				case { } opaqueColor when opaqueColor.A == byte.MaxValue:
					// Prefer BarTintColor because it supports smooth transitions
					Native.BarTintColor = opaqueColor;
					Native.Translucent = false; //Make fully opaque for consistency with SetBackgroundImage
					Native.SetBackgroundImage(null, UIBarMetrics.Default);
					Native.ShadowImage = null;
					break;
				case { } semiTransparentColor when semiTransparentColor.A > 0:
					Native.BarTintColor = null;
					// Use SetBackgroundImage as hack to support semi-transparent background
					Native.SetBackgroundImage(((UIColor)semiTransparentColor).ToUIImage(), UIBarMetrics.Default);
					Native.Translucent = true;
					Native.ShadowImage = null;
					break;
				case { } transparent when transparent.A == 0:
					Native.BarTintColor = null;
					Native.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
					// We make sure a transparent bar doesn't cast a shadow.
					Native.ShadowImage = new UIImage(); // Removes the default 1px line
					Native.Translucent = true;
					break;
				default: //Background is null
					Native.BarTintColor = null;
					Native.SetBackgroundImage(null, UIBarMetrics.Default); // Restores the default blurry background
					Native.ShadowImage = null; // Restores the default 1px line
					Native.Translucent = true;
					break;
			}

			var mainCommand = Element.GetValue(NavigationBar.MainCommandProperty) as AppBarButton;

			// CommandBarExtensions.BackButtonForeground
			ColorHelper.TryGetColorWithOpacity(mainCommand?.Foreground, out var backButtonForeground);
			Native.TintColor = backButtonForeground;
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