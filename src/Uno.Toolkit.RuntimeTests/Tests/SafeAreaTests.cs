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
using Uno.UI;
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
using Uno.UI.Xaml;
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
			using var _ = SetupWindow();
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
			using var _ = SetupWindow();
			
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

			var blueWithOpaqueBars = blueGrid.TransformToVisual(redGrid).TransformBounds(new Windows.Foundation.Rect(0, 0, blueGrid.ActualWidth, blueGrid.ActualHeight));
			var redWithOpaqueBars = redGrid.TransformToVisual(redGrid).TransformBounds(new Windows.Foundation.Rect(0, 0, redGrid.ActualWidth, redGrid.ActualHeight));
			var visibleBoundsWithOpaqueBars = ApplicationView.GetForCurrentView().VisibleBounds;

			// before: redWithOpaqueBars should be at (0, [statusBarHeight]) and the same size as visibleBoundsWithOpaqueBars
			using var __ = UseTranslucentBars();
			// after: redWithTranslucentBars should be at (0, 0) and should differ from visibleBoundsWithTranslucentBars in height by [statusBarHeight] + [navAreaHeight]

			await UnitTestsUIContentHelper.WaitForIdle();

			var blueWithTranslucentBars = blueGrid.TransformToVisual(redGrid).TransformBounds(new Windows.Foundation.Rect(0, 0, blueGrid.ActualWidth, blueGrid.ActualHeight));
			var redWithTranslucentBars = redGrid.TransformToVisual(redGrid).TransformBounds(new Windows.Foundation.Rect(0, 0, redGrid.ActualWidth, redGrid.ActualHeight));
			var visibleBoundsWithTranslucentBars = ApplicationView.GetForCurrentView().VisibleBounds;

			var statusBarHeight = visibleBoundsWithTranslucentBars.Top - redWithTranslucentBars.Top;
			var navAreaHeight = redWithTranslucentBars.Bottom - visibleBoundsWithTranslucentBars.Bottom;

			Assert.AreEqual(blueWithTranslucentBars.Top, statusBarHeight, message: $"Blue rect top: {blueWithTranslucentBars.Top} should equal status bar height: {statusBarHeight}");
			Assert.AreEqual(blueWithTranslucentBars.Bottom, redWithTranslucentBars.Bottom - navAreaHeight, message: $"Blue rect bottom: {blueWithTranslucentBars.Bottom} should be offset by nav area height: {navAreaHeight}");
			Assert.AreEqual(redGrid.Padding.Top, statusBarHeight, message: $"Red rect padding top: {redGrid.Padding.Top} should be equal to status bar height: {statusBarHeight}");
			Assert.AreEqual(redGrid.Padding.Bottom, navAreaHeight, message: $"Red rect padding bottom: {redGrid.Padding.Bottom} should be equal to nav area height: {navAreaHeight}");
		}


		private static IDisposable UseTranslucentBars()
		{
			var activity = Uno.UI.ContextHelper.Current as Android.App.Activity;
			activity?.Window?.AddFlags(Android.Views.WindowManagerFlags.TranslucentNavigation | Android.Views.WindowManagerFlags.TranslucentStatus);

			return Disposable.Create(() =>
			{
				activity?.Window?.ClearFlags(Android.Views.WindowManagerFlags.TranslucentNavigation | Android.Views.WindowManagerFlags.TranslucentStatus);
			});
		}


		private static IDisposable SetupWindow()
		{
			var disposables = new CompositeDisposable();

			ApplicationView.GetForCurrentView().ExitFullScreenMode();

			// The [RequiresFullWindow] attribute sets the app to fullscreen on Android, hiding all system bars.
			// This method maintains the fullscreen state, but shows the system bars for tests that need them.
			UnitTestsUIContentHelper.UseActualWindowRoot = true;
			UnitTestsUIContentHelper.SaveOriginalContent();

			disposables.Add(Disposable.Create(() =>
			{
				UnitTestsUIContentHelper.RestoreOriginalContent();
				UnitTestsUIContentHelper.UseActualWindowRoot = false;
			}));

			// When runtime tests are initiated through the UI Tests, each test starts by entering fullscreen.
			// We need to exit fullscreen in order to test the status bar height.
			// Also, the emulator running on the CI has Window flags (WindowManagerFlags.LayoutInScreen | WindowManagerFlags.LayoutInsetDecor)
			// which causes issues when running tests on the UI thread while attempting to alter the window flags
			if (ContextHelper.Current is Android.App.Activity activity
				&& activity.Window is { } window
				&& window.Attributes is { } attr)
			{
				var flags = attr.Flags;
				window.ClearFlags(Android.Views.WindowManagerFlags.LayoutInScreen | Android.Views.WindowManagerFlags.LayoutInsetDecor);

				disposables.Add(Disposable.Create(() => window.SetFlags(flags, flags)));
			}

			return disposables;
		}
#endif
	}
}
