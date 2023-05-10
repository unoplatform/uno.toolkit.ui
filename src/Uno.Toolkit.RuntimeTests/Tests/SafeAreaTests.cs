using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Disposables;
using Uno.Toolkit.RuntimeTests.Extensions;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.RuntimeTests.Tests.TestPages;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Windows.System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if __IOS__
using UIKit;
#endif

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Windows.UI.ViewManagement;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.UI.ViewManagement;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	internal partial class SafeAreaTests
	{
#if __ANDROID__
		[TestMethod]
		public async Task Translucent_SystemBars()
		{
			using var _ = UseFullWindow();
			using var __ = UseTranslucentBars();

			var redGrid = new Grid
			{
				Background = new SolidColorBrush(Colors.Red),
			};

			var blueGrid = new Grid
			{
				Background = new SolidColorBrush(Colors.Blue),
			};

			redGrid.Children.Add(blueGrid);

			SafeArea.SetInsets(redGrid, SafeArea.InsetMask.VisibleBounds);

			await UnitTestUIContentHelperEx.SetContentAndWait(redGrid);

			var visibleBounds = ApplicationView.GetForCurrentView().VisibleBounds;
			var blueRect = blueGrid.TransformToVisual(null).TransformBounds(new Windows.Foundation.Rect(0, 0, blueGrid.ActualWidth, blueGrid.ActualHeight));
			var redRect = redGrid.TransformToVisual(null).TransformBounds(new Windows.Foundation.Rect(0, 0, redGrid.ActualWidth, redGrid.ActualHeight));

			var statusBarHeight = visibleBounds.Top - redRect.Top;
			var navAreaHeight = redRect.Bottom - visibleBounds.Bottom;

			Assert.AreEqual(blueRect.Top, statusBarHeight);
			Assert.AreEqual(blueRect.Bottom, redRect.Bottom - navAreaHeight);
			Assert.AreEqual(redGrid.Padding.Top, statusBarHeight);
			Assert.AreEqual(redGrid.Padding.Bottom, navAreaHeight);
		}

		[TestMethod]
		public async Task Translucent_SystemBars_Dynamic()
		{
			using var _ = UseFullWindow();
			
			var redGrid = new Grid
			{
				Background = new SolidColorBrush(Colors.Red),
			};

			var blueGrid = new Grid
			{
				Background = new SolidColorBrush(Colors.Blue),
			};

			redGrid.Children.Add(blueGrid);
			SafeArea.SetInsets(redGrid, SafeArea.InsetMask.VisibleBounds);

			await UnitTestUIContentHelperEx.SetContentAndWait(redGrid);

			var blueWithOpaqueBars = blueGrid.TransformToVisual(null).TransformBounds(new Windows.Foundation.Rect(0, 0, blueGrid.ActualWidth, blueGrid.ActualHeight));
			var windowWithOpaqueBars = redGrid.TransformToVisual(null).TransformBounds(new Windows.Foundation.Rect(0, 0, redGrid.ActualWidth, redGrid.ActualHeight));
			var visibleBoundsWithOpaqueBars = ApplicationView.GetForCurrentView().VisibleBounds;

			// before: windowWithOpaqueBars should be at (0, [statusBarHeight]) and the same size as visibleBoundsWithOpaqueBars
			using var __ = UseTranslucentBars();
			// after: windowWithTranslucentBars should be at (0, 0) and should differ from visibleBoundsWithTranslucentBars in height by [statusBarHeight] + [navAreaHeight]

			await UnitTestsUIContentHelper.WaitForIdle();

			var blueWithTranslucentBars = blueGrid.TransformToVisual(null).TransformBounds(new Windows.Foundation.Rect(0, 0, blueGrid.ActualWidth, blueGrid.ActualHeight));
			var windowWithTranslucentBars = redGrid.TransformToVisual(null).TransformBounds(new Windows.Foundation.Rect(0, 0, redGrid.ActualWidth, redGrid.ActualHeight));
			var visibleBoundsWithTranslucentBars = ApplicationView.GetForCurrentView().VisibleBounds;

			var statusBarHeight = windowWithOpaqueBars.Top - windowWithTranslucentBars.Top;
			var navAreaHeight = windowWithTranslucentBars.Bottom - windowWithOpaqueBars.Bottom;

			Assert.AreEqual(blueWithOpaqueBars, blueWithTranslucentBars);
			Assert.AreEqual(blueWithTranslucentBars.Top, statusBarHeight);
			Assert.AreEqual(blueWithTranslucentBars.Bottom, windowWithTranslucentBars.Bottom - navAreaHeight);
			Assert.AreEqual(redGrid.Padding.Top, statusBarHeight);
			Assert.AreEqual(redGrid.Padding.Bottom, navAreaHeight);
		}


		private IDisposable UseTranslucentBars()
		{
			var activity = Uno.UI.ContextHelper.Current as Android.App.Activity;
			activity?.Window?.AddFlags(Android.Views.WindowManagerFlags.TranslucentNavigation | Android.Views.WindowManagerFlags.TranslucentStatus);

			return Disposable.Create(() =>
			{
				activity?.Window?.ClearFlags(Android.Views.WindowManagerFlags.TranslucentNavigation | Android.Views.WindowManagerFlags.TranslucentStatus);
			});
		}

		// The [RequiresFullWindow] attribute sets the app to fullscreen on Android, hiding all system bars.
		// This method maintains the fullscreen state, but shows the system bars for tests that need them.
		private IDisposable UseFullWindow()
		{
			UnitTestsUIContentHelper.UseActualWindowRoot = true;
			UnitTestsUIContentHelper.SaveOriginalContent();

			return Disposable.Create(() =>
			{
				UnitTestsUIContentHelper.RestoreOriginalContent();
				UnitTestsUIContentHelper.UseActualWindowRoot = false;
			});
		}
#endif
	}
}
