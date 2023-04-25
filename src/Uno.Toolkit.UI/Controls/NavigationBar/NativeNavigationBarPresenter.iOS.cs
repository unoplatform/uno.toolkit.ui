#if __IOS__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Uno.Disposables;
using Uno.UI;
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

using XamlStatusBar = Windows.UI.ViewManagement.StatusBar;

namespace Uno.Toolkit.UI
{
	public partial class NativeNavigationBarPresenter : ContentPresenter, INavigationBarPresenter
	{
		private readonly SerialDisposable _statusBarSubscription = new SerialDisposable();
		private readonly SerialDisposable _orientationSubscription = new SerialDisposable();
		private SerialDisposable _mainCommandClickHandler = new SerialDisposable();
		private WeakReference<NavigationBar?>? _navBarRef;

		private UINavigationBar? _navigationBar;
		private readonly bool _isPhone = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone;

		public NativeNavigationBarPresenter()
		{
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}

		public void SetOwner(NavigationBar navigationBar)
		{
			//Owner is accessed through TemplatedParent on Uno platforms
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			_statusBarSubscription.Disposable = null;
			_orientationSubscription.Disposable = null;
			_mainCommandClickHandler.Disposable = null;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			// TODO: Find a proper way to decide whether a NavigationBar exists on canvas (within Page), or is mapped to the UINavigationController's NavigationBar.

			NavigationBar? navBar = null;

			_navBarRef?.TryGetTarget(out navBar);

			if (navBar == null)
			{
				navBar = TemplatedParent as NavigationBar;
				_navBarRef = new WeakReference<NavigationBar?>(navBar);
			}

			_navigationBar = navBar?.GetOrAddDefaultRenderer().Native;

			if (navBar is { })
			{
				navBar.MainCommand.Click += OnMainCommandClick;
				_mainCommandClickHandler.Disposable = null;
				_mainCommandClickHandler.Disposable = Disposable.Create(() => navBar.MainCommand.Click -= OnMainCommandClick);
			}

			if (_navigationBar == null)
			{
				throw new InvalidOperationException("No NavigationBar from renderer");
			}

			_navigationBar.SetNeedsLayout();

			var navigationBarSuperview = _navigationBar?.Superview;

			// Allows the UINavigationController's NavigationBar instance to be moved to the Page. This feature
			// is used in the context of the sample application to test NavigationBars outside of a NativeFramePresenter for 
			// UI Testing. In general cases, this should not happen as the bar may be moved back to to this presenter while
			// another page is already visible, making this bar overlay on top of another.			
			if (FeatureConfiguration.CommandBar.AllowNativePresenterContent && (navigationBarSuperview == null || navigationBarSuperview is NativeNavigationBarPresenter))
			{
				Content = _navigationBar;
			}

			var statusBar = XamlStatusBar.GetForCurrentView();

			statusBar.Showing += OnStatusBarChanged;
			statusBar.Hiding += OnStatusBarChanged;

			_statusBarSubscription.Disposable = Disposable.Create(() =>
			{
				statusBar.Showing -= OnStatusBarChanged;
				statusBar.Hiding -= OnStatusBarChanged;
			});

			// iOS doesn't automatically update the navigation bar position when the status bar visibility changes.
			void OnStatusBarChanged(XamlStatusBar sender, object args)
			{
				_navigationBar!.SetNeedsLayout();
				_navigationBar!.Superview.SetNeedsLayout();
			}
		}

		private void OnMainCommandClick(object sender, RoutedEventArgs e)
		{
			NavigationBar? navBar = null;
			if (_navBarRef?.TryGetTarget(out navBar) ?? false)
			{
				navBar?.TryPerformMainCommand();
			}
		}

		protected override Size MeasureOverride(Size size)
		{
			var measuredSize = base.MeasureOverride(size);

			if (_isPhone)
			{
				switch (UIDevice.CurrentDevice.Orientation)
				{
					// On iPhone, the OS may wrongly report the height of the UINavigationBar:
					//   - After the creation when it's in landscape mode, it's wrongly
					//     returning the portrait height as measured height (the width is correct)
					//   - After the rotation from landscape to portrait, the next measured
					//     height will be the landscape one. Width is also correct.
					//
					// This is a ugly hack to circumvent this annoying OS bug.
					//
					// NOTE: those values are been fixed for all iOS versions. Tested on iOS 14.
					case UIDeviceOrientation.LandscapeLeft:
					case UIDeviceOrientation.LandscapeRight:
						measuredSize = new Size(measuredSize.Width, 32);
						break;
					case UIDeviceOrientation.Portrait:
					case UIDeviceOrientation.PortraitUpsideDown:
						measuredSize = new Size(measuredSize.Width, 44);
						break;
				}
			}

			return measuredSize;
		}

		
	}
}
#endif
