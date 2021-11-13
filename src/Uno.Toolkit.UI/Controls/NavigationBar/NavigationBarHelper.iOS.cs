#if __IOS__
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using UIKit;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI.Controls
{
	internal static class NavigationBarHelper
	{
		internal static void SetNavigationBar(NavigationBar NavigationBar, UIKit.UINavigationBar? navigationBar)
		{
			NavigationBar.GetRenderer(() => new NavigationBarRenderer(NavigationBar)).Native = navigationBar;
		}

		internal static void SetNavigationItem(NavigationBar NavigationBar, UIKit.UINavigationItem? navigationItem)
		{
			NavigationBar.GetRenderer(() => new NavigationBarNavigationItemRenderer(NavigationBar)).Native = navigationItem;
		}

		/// <summary>
		/// Finds and configures the <see cref="NavigationBar" /> for a given page <see cref="UIViewController" />.
		/// </summary>
		/// <param name="pageController">The controller of the page</param>
		public static void PageCreated(UIViewController pageController)
		{
			var topNavigationBar = pageController.FindTopNavigationBar();
			if (topNavigationBar == null)
			{
				// The default NavigationBar style contains information that might be relevant to all pages, including those without a NavigationBar.
				// For example the Uno.UI.Toolkit.NavigationBarExtensions.BackButtonTitle attached property is often set globally to "" through
				// a default NavigationBar style in order to remove the back button text throughout an entire application.
				// In order to leverage this information, we create a new NavigationBar instance that only exists to "render" the NavigationItem.
				// Since Uno 3.0 objects which are not part of the Visualtree does not get the Global Styles applied. Hence the fact we are manually applying it here.
				topNavigationBar = new NavigationBar
				{
					Style = Application.Current.Resources[typeof(NavigationBar)] as Style
				};
			}

			// Hook NavigationBar to NavigationItem
			SetNavigationItem(topNavigationBar, pageController.NavigationItem);
		}

		/// <summary>
		/// Cleanups the <see cref="NavigationBar" /> of a page <see cref="UIViewController" />.
		/// </summary>
		/// <param name="pageController">The controller of the page</param>
		public static void PageDestroyed(UIViewController pageController)
		{
			if (pageController.FindTopNavigationBar() is { } topNavigationBar)
			{
				SetNavigationItem(topNavigationBar, null);
			}
		}

		/// <summary>
		/// When a page <see cref="UIViewController" /> will appear, connects the <see cref="NavigationBar" /> to the navigation controller.
		/// </summary>
		/// <param name="pageController">The controller of the page</param>
		public static void PageWillAppear(UIViewController pageController)
		{
			var topNavigationBar = pageController.FindTopNavigationBar();
			if (topNavigationBar != null)
			{
				if (topNavigationBar.Visibility == Visibility.Visible)
				{
					SetNavigationBar(topNavigationBar, pageController.NavigationController.NavigationBar);

					// When the NavigationBar is visible, we need to call SetNavigationBarHidden
					// AFTER it has been rendered. Otherwise, it causes a bug introduced
					// in iOS 11 in which the BackButtonIcon is not rendered properly.
					pageController.NavigationController.SetNavigationBarHidden(hidden: false, animated: true);
				}
				else
				{
					// Even if the NavigationBar should technically be collapsed,
					// we don't hide it using the NavigationController because it
					// automatically disables the back gesture.
					// In order to visually hide it, the NavigationBarRenderer
					// will hide the native view using the UIView.Hidden property.
					pageController.NavigationController.SetNavigationBarHidden(hidden: false, animated: true);

					SetNavigationBar(topNavigationBar, pageController.NavigationController.NavigationBar);
				}
			}
			else // No NavigationBar
			{
				pageController.NavigationController.SetNavigationBarHidden(true, true);
			}
		}

		/// <summary>
		/// When a page <see cref="UIViewController" /> did disappear, disconnects the <see cref="NavigationBar" /> from the navigation controller.
		/// </summary>
		/// <param name="pageController">The controller of the page</param>
		public static void PageDidDisappear(UIViewController pageController)
		{
			if (pageController.FindTopNavigationBar() is { } topNavigationBar)
			{
				// Set the native navigation bar to null so it does not render when the page is not visible
				SetNavigationBar(topNavigationBar, null);
			}
		}

		public static void PageWillDisappear(UIViewController pageController)
		{
			if (pageController.FindTopNavigationBar() is { } topNavigationBar)
			{
				// Set the native navigation bar to null so it does not render when the page is not visible
				SetNavigationBar(topNavigationBar, null);
			}
		}

		private static NavigationBar? FindTopNavigationBar(this UIViewController controller)
		{
			return controller.View.FindFirstChild<NavigationBar?>();
		}
	}
}
#endif