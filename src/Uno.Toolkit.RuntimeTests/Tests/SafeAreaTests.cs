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
using XamlWindow = Microsoft.UI.Xaml.Window;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.UI.ViewManagement;
using XamlWindow = Windows.UI.Xaml.Window;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	internal partial class SafeAreaTests
	{
		[TestMethod]
		[RequiresFullWindow]
		[Ignore("Failing as of bump to net9. issue#1467")]
		public async Task NestedSafeArea_ApplyCount()
		{
			var setup = XamlHelper.LoadXaml<Grid>("""
				<Grid BorderBrush="Red" BorderThickness="5" utu:SafeArea.Insets="VisibleBounds">
					<Grid utu:SafeArea.Insets="VisibleBounds">
						<Border Background="SkyBlue" />
					</Grid>
				</Grid>
			""");

			var grid0 = setup;
			var grid1 = (Grid)setup.Children[0];

			// internal dp are not settable by XamlReader
			var customBounds = new Thickness(0, 123, 0, 0);
			SafeArea.SetSafeAreaOverride(grid0, customBounds);
			SafeArea.SetSafeAreaOverride(grid1, customBounds);

			var details0 = SafeArea.SafeAreaDetails.FindInstance(grid0) ?? throw new InvalidOperationException("SafeAreaDetails not found for outer grid");
			var details1 = SafeArea.SafeAreaDetails.FindInstance(grid1) ?? throw new InvalidOperationException("SafeAreaDetails not found for inner grid");

			var effectiveUpdates = new List<(string, Thickness)>();
			details0.EffectiveInsetsApplied += (s, e) => effectiveUpdates.Add(("grid0", e));
			details1.EffectiveInsetsApplied += (s, e) => effectiveUpdates.Add(("grid1", e));

#if DEBUG
			var updates = new List<(string, Thickness)>();
			details0.InsetsApplied += (s, e) => updates.Add(("grid0", e));
			details1.InsetsApplied += (s, e) => updates.Add(("grid1", e));
#endif

			await UnitTestUIContentHelperEx.SetContentAndWait(setup);
			await UnitTestUIContentHelperEx.WaitForIdle();

#if DEBUG
			Assert.AreEqual(3, updates.Count);
#endif
			Assert.AreEqual(1, effectiveUpdates.Count);
			Assert.AreEqual((nameof(grid0), customBounds), effectiveUpdates[0]);
		}

#if DEBUG && !__ANDROID__ && !WINDOWS_WINUI
		[TestMethod]
		[RequiresFullWindow]
		public async Task BoundsTransitionGuard_NotActive_OnNonAndroid()
		{
			// Regression test for the iPad CameraCaptureUI live-lock (fix/iPadOS.transitions).
			//
			// PR #1554 added a Bounds/VisibleBounds-mismatch guard in UpdateInsets() that
			// self-reschedules on the dispatcher when VisibleBounds changes ahead of Window.Bounds.
			// On iPad, after a UIImagePickerController-style FormSheet/OverFullScreen modal
			// dismisses, VisibleBounds updates while Window.Bounds stays stable — Bounds NEVER
			// catches up because the host view stayed attached. Without the Android-only gate,
			// the guard reschedules indefinitely and starves the managed DispatcherQueue,
			// freezing taps/buttons/back-nav while UIKit-routed scroll keeps working.
			//
			// This test seeds the trap state (prior Bounds matches current; prior VisibleBounds
			// differs from current) and asserts that on non-Android the guard never engages.
			var grid = new Grid { Background = new SolidColorBrush(Colors.Red), Width = 200, Height = 200 };
			SafeArea.SetInsets(grid, SafeArea.InsetMask.VisibleBounds);

			await UnitTestUIContentHelperEx.SetContentAndWait(grid);
			await UnitTestUIContentHelperEx.WaitForIdle();

			var details = SafeArea.SafeAreaDetails.FindInstance(grid)
				?? throw new InvalidOperationException("SafeAreaDetails not found");

			var savedBounds = SafeArea.SafeAreaDetails.TestHook_LastKnownBounds;
			var savedVisibleBounds = SafeArea.SafeAreaDetails.TestHook_LastKnownVisibleBounds;
			var savedPending = SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending;

			try
			{
				var currentBounds = XamlWindow.Current?.Bounds ?? default;
				SafeArea.SafeAreaDetails.TestHook_LastKnownBounds = currentBounds;
				SafeArea.SafeAreaDetails.TestHook_LastKnownVisibleBounds = new Windows.Foundation.Rect(0, 0, 1, 1);
				SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending = false;

				details.TestHook_InvokeUpdateInsets(forceUpdate: false);
				await UnitTestUIContentHelperEx.WaitForIdle();

				Assert.IsFalse(
					SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending,
					"Bounds-transition guard must stay gated off on non-Android — otherwise iPad live-locks the managed dispatcher after a FormSheet modal dismisses (see fix/iPadOS.transitions).");
			}
			finally
			{
				SafeArea.SafeAreaDetails.TestHook_LastKnownBounds = savedBounds;
				SafeArea.SafeAreaDetails.TestHook_LastKnownVisibleBounds = savedVisibleBounds;
				SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending = savedPending;
			}
		}
#endif

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


		[TestMethod]
		public async Task BottomInset_NotInflated_WhenTransitioningToTranslucentBars()
		{
			// Regression test: When transitioning to translucent system bars (e.g., setting StatusBar.Background),
			// VisibleBounds updates before Window.Bounds, causing the SafeArea to temporarily calculate inflated
			// bottom insets. This transient inflation can permanently stretch controls in Auto-sized grid rows.
			using var _ = SetupWindow();

			var content = new Grid { Background = new SolidColorBrush(Colors.Blue) };
			var tabBarGrid = new Grid
			{
				Background = new SolidColorBrush(Colors.Red),
				MinHeight = 80,
			};

			var parentGrid = new Grid
			{
				RowDefinitions =
				{
					new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
					new RowDefinition { Height = GridLength.Auto },
				},
			};

			Grid.SetRow(content, 0);
			Grid.SetRow(tabBarGrid, 1);
			parentGrid.Children.Add(content);
			parentGrid.Children.Add(tabBarGrid);

			// Apply SafeArea.Insets="Bottom" on the bottom element (mimics MaterialBottomTabBarStyle)
			SafeArea.SetInsets(tabBarGrid, SafeArea.InsetMask.Bottom);

			await UnitTestUIContentHelperEx.SetContentAndWait(parentGrid);
			await UnitTestsUIContentHelper.WaitForIdle();

			var heightBeforeTransition = tabBarGrid.ActualHeight;
			var paddingBeforeTransition = tabBarGrid.Padding.Bottom;

			// Now transition to translucent bars (simulates StatusBar.Background being set)
			using var __ = UseTranslucentBars();

			// Wait for the layout to stabilize after the bar transition
			var lastHeight = tabBarGrid.ActualHeight;
			await UnitTestUIContentHelperEx.WaitFor(() =>
			{
				var current = tabBarGrid.ActualHeight;
				var stable = Math.Abs(current - lastHeight) < 0.1;
				lastHeight = current;
				return stable;
			}, timeoutMS: 2000, message: "TabBar height did not stabilize after bar transition");

			var heightAfterTransition = tabBarGrid.ActualHeight;
			var paddingAfterTransition = tabBarGrid.Padding.Bottom;

			// The bottom padding should not have increased beyond what it was before the transition.
			// During the race condition, it would temporarily spike (e.g., from 24 to 75.8) and
			// the control would get stuck at the inflated height.
			Assert.IsTrue(
				heightAfterTransition <= heightBeforeTransition + 1,
				$"TabBar height should not inflate during bar transition. Before: {heightBeforeTransition}, After: {heightAfterTransition}");
			Assert.IsTrue(
				paddingAfterTransition <= paddingBeforeTransition + 1,
				$"TabBar bottom padding should not inflate during bar transition. Before: {paddingBeforeTransition}, After: {paddingAfterTransition}");
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
