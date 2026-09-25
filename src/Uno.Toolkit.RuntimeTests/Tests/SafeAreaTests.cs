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
	public partial class SafeAreaTests
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
			// Guard spec: SafeArea.cs → "Bounds-transition guard" block, gated by
			//   `!HasSoftInput() && OperatingSystem.IsAndroid()`.
			// Full R&D recap: specs/safearea-bounds-guard/recap.md
			//
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

				// The guard sets s_boundsTransitionPending=true synchronously before scheduling
				// its reschedule, so we assert immediately. Awaiting WaitForIdle() here would hang
				// the test if the guard ever regresses — the reschedule loop saturates the
				// dispatcher and idle is never reached (the very symptom this test guards against).
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

		[TestMethod]
		[RequiresFullWindow]
		public async Task BoundsTransitionGuard_IgnoredForAllStates_OnNonAndroid()
		{
			// Guard spec: SafeArea.cs → "Bounds-transition guard" block, gated by
			//   `!HasSoftInput() && OperatingSystem.IsAndroid()`.
			// Full R&D recap: specs/safearea-bounds-guard/recap.md
			//
			// Complementary to BoundsTransitionGuard_NotActive_OnNonAndroid:
			// Even if s_boundsTransitionPending is already true (e.g. stale state from a
			// previous Android-gated execution), the guard block is entirely skipped on
			// non-Android. The pending flag must remain unchanged — it is never read or
			// cleared outside the Android guard.
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

				// Seed a "pending" state that would be handled on Android but must be ignored here.
				SafeArea.SafeAreaDetails.TestHook_LastKnownBounds = currentBounds;
				SafeArea.SafeAreaDetails.TestHook_LastKnownVisibleBounds = new Windows.Foundation.Rect(0, 0, 1, 1);
				SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending = true;

				details.TestHook_InvokeUpdateInsets(forceUpdate: false);

				// On non-Android, the guard block is skipped entirely, so the pending flag
				// stays in whatever state it was seeded with.
				Assert.IsTrue(
					SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending,
					"On non-Android the guard block is entirely skipped — pending flag must stay unchanged.");
			}
			finally
			{
				SafeArea.SafeAreaDetails.TestHook_LastKnownBounds = savedBounds;
				SafeArea.SafeAreaDetails.TestHook_LastKnownVisibleBounds = savedVisibleBounds;
				SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending = savedPending;
			}
		}
#endif

#if DEBUG && __ANDROID__
		[TestMethod]
		[RequiresFullWindow]
		public async Task BoundsTransition_DefersOnce_WhenVisibleBoundsChangeAheadOfWindowBounds()
		{
			// Guard spec: SafeArea.cs → Branch 2 — defer once.
			// Introduced in: https://github.com/unoplatform/uno.toolkit.ui/pull/1554
			// Full R&D recap: specs/safearea-bounds-guard/recap.md
			//
			// Branch 2: VisibleBounds changed but Window.Bounds hasn't yet — the guard
			// must set s_boundsTransitionPending = true and schedule one deferred callback.
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

				// Seed: Bounds match current (no change), VisibleBounds differ (changed).
				SafeArea.SafeAreaDetails.TestHook_LastKnownBounds = currentBounds;
				SafeArea.SafeAreaDetails.TestHook_LastKnownVisibleBounds = new Windows.Foundation.Rect(0, 0, 1, 1);
				SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending = false;

				details.TestHook_InvokeUpdateInsets(forceUpdate: false);

				// Branch 2 fires synchronously: pending flag must be true.
				Assert.IsTrue(
					SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending,
					"Guard must set pending=true when VisibleBounds changed ahead of Window.Bounds.");
			}
			finally
			{
				SafeArea.SafeAreaDetails.TestHook_LastKnownBounds = savedBounds;
				SafeArea.SafeAreaDetails.TestHook_LastKnownVisibleBounds = savedVisibleBounds;
				SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending = savedPending;
			}
		}

		[TestMethod]
		[RequiresFullWindow]
		public async Task BoundsTransition_AcceptsCurrent_AfterSingleDeferral()
		{
			// Guard spec: SafeArea.cs → Branch 1 — accept-and-proceed (one-shot).
			// Fixed in: https://github.com/unoplatform/uno.toolkit.ui/pull/1593
			// Full R&D recap: specs/safearea-bounds-guard/recap.md
			//
			// Branch 1 — THE KEY FIX: After one deferral, if Bounds still hasn't caught up,
			// the guard must accept current values and clear pending instead of re-deferring
			// indefinitely (which would starve DispatcherQueue Low-priority items).
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

				// Seed: pending=true (already deferred once), Bounds match current (no change).
				SafeArea.SafeAreaDetails.TestHook_LastKnownBounds = currentBounds;
				SafeArea.SafeAreaDetails.TestHook_LastKnownVisibleBounds = new Windows.Foundation.Rect(0, 0, 1, 1);
				SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending = true;

				details.TestHook_InvokeUpdateInsets(forceUpdate: false);

				// Branch 1 must accept and clear pending — no infinite re-deferral.
				Assert.IsFalse(
					SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending,
					"After one deferral, guard must accept current values and clear pending to prevent dispatcher starvation.");

				// Cached values must be updated to the current state.
				Assert.AreEqual(currentBounds, SafeArea.SafeAreaDetails.TestHook_LastKnownBounds,
					"LastKnownBounds must be updated to current Window.Bounds after acceptance.");

				var currentVB = ApplicationView.GetForCurrentView().VisibleBounds;
				Assert.AreEqual(currentVB, SafeArea.SafeAreaDetails.TestHook_LastKnownVisibleBounds,
					"LastKnownVisibleBounds must be updated to current VisibleBounds after acceptance.");
			}
			finally
			{
				SafeArea.SafeAreaDetails.TestHook_LastKnownBounds = savedBounds;
				SafeArea.SafeAreaDetails.TestHook_LastKnownVisibleBounds = savedVisibleBounds;
				SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending = savedPending;
			}
		}

		[TestMethod]
		[RequiresFullWindow]
		public async Task BoundsTransition_ClearsGuard_WhenBoundsCatchUp()
		{
			// Guard spec: SafeArea.cs → Branch 3 — normal path.
			// Full R&D recap: specs/safearea-bounds-guard/recap.md
			//
			// Branch 3: When Bounds change (i.e. they caught up to VisibleBounds), the guard
			// must clear pending and update cached values — normal flow resumes.
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

				// Seed: pending=true, but lastKnownBounds differ from current → boundsChanged=true.
				SafeArea.SafeAreaDetails.TestHook_LastKnownBounds = new Windows.Foundation.Rect(0, 0, 1, 1);
				SafeArea.SafeAreaDetails.TestHook_LastKnownVisibleBounds = new Windows.Foundation.Rect(0, 0, 1, 1);
				SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending = true;

				details.TestHook_InvokeUpdateInsets(forceUpdate: false);

				// Branch 3: bounds caught up — pending must be cleared and caches updated.
				Assert.IsFalse(
					SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending,
					"Guard must clear pending when Window.Bounds have caught up.");
				Assert.AreEqual(currentBounds, SafeArea.SafeAreaDetails.TestHook_LastKnownBounds,
					"LastKnownBounds must be updated when bounds catch up.");
			}
			finally
			{
				SafeArea.SafeAreaDetails.TestHook_LastKnownBounds = savedBounds;
				SafeArea.SafeAreaDetails.TestHook_LastKnownVisibleBounds = savedVisibleBounds;
				SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending = savedPending;
			}
		}

		[TestMethod]
		[RequiresFullWindow]
		public async Task BoundsTransition_NoDeferral_WhenLastKnownBoundsIsDefault()
		{
			// Guard spec: SafeArea.cs → Branch 2 edge case (s_lastKnownBounds == default).
			// Full R&D recap: specs/safearea-bounds-guard/recap.md
			//
			// Edge case for Branch 2 guard: s_lastKnownBounds == default means this is the
			// first ever UpdateInsets call. Even if VisibleBounds changed, no deferral should
			// occur because there is no "prior state" to compare against.
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
				// Seed: lastKnownBounds = default (first run), VB differs from current.
				SafeArea.SafeAreaDetails.TestHook_LastKnownBounds = default;
				SafeArea.SafeAreaDetails.TestHook_LastKnownVisibleBounds = new Windows.Foundation.Rect(0, 0, 1, 1);
				SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending = false;

				details.TestHook_InvokeUpdateInsets(forceUpdate: false);

				// Branch 2 condition requires s_lastKnownBounds != default — since it IS default,
				// we must fall through to Branch 3 (normal update), so no deferral.
				Assert.IsFalse(
					SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending,
					"No deferral should occur on the first-ever UpdateInsets call (lastKnownBounds is default).");
			}
			finally
			{
				SafeArea.SafeAreaDetails.TestHook_LastKnownBounds = savedBounds;
				SafeArea.SafeAreaDetails.TestHook_LastKnownVisibleBounds = savedVisibleBounds;
				SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending = savedPending;
			}
		}

		[TestMethod]
		[RequiresFullWindow]
		public async Task BoundsTransition_FullCycle_DefersOnceThenAccepts()
		{
			// Guard spec: SafeArea.cs → Branch 2 → Branch 1 full cycle.
			// Full R&D recap: specs/safearea-bounds-guard/recap.md
			//
			// Integration test: walks through the complete cycle that the fix enables.
			// Step 1 (Branch 2): VB changed, Bounds didn't → defer once.
			// Step 2 (Branch 1): Pending, Bounds still unchanged → accept and clear.
			// This proves the infinite loop is impossible.
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

				// Step 1: Simulate VB changing while Bounds stays the same.
				SafeArea.SafeAreaDetails.TestHook_LastKnownBounds = currentBounds;
				SafeArea.SafeAreaDetails.TestHook_LastKnownVisibleBounds = new Windows.Foundation.Rect(0, 0, 1, 1);
				SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending = false;

				details.TestHook_InvokeUpdateInsets(forceUpdate: false);
				Assert.IsTrue(
					SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending,
					"Step 1: Guard must defer once when VB changed ahead of Bounds.");

				// Step 2: Simulate the deferred callback firing — Bounds still haven't changed.
				// (In real code, the deferred Schedule() calls UpdateInsets again.)
				details.TestHook_InvokeUpdateInsets(forceUpdate: true);
				Assert.IsFalse(
					SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending,
					"Step 2: Guard must accept and clear pending after one deferral — no infinite loop.");
			}
			finally
			{
				SafeArea.SafeAreaDetails.TestHook_LastKnownBounds = savedBounds;
				SafeArea.SafeAreaDetails.TestHook_LastKnownVisibleBounds = savedVisibleBounds;
				SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending = savedPending;
			}
		}

		[TestMethod]
		[RequiresFullWindow]
		public async Task BoundsTransition_FullCycle_DefersOnceThenBoundsCatchUp()
		{
			// Guard spec: SafeArea.cs → Branch 2 → Branch 3 happy-path cycle.
			// Full R&D recap: specs/safearea-bounds-guard/recap.md
			//
			// Happy-path full cycle (the PR #1554 intended flow):
			// Step 1 (Branch 2): VB changed, Bounds didn't → defer once.
			// Step 2 (Branch 3): Bounds caught up → clear pending, update caches, proceed.
			// This is the status bar background transition, where Uno raises VisibleBoundsChanged
			// before it updates Window.Bounds.
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

				// Step 1: Simulate VB changing while Bounds stays the same.
				SafeArea.SafeAreaDetails.TestHook_LastKnownBounds = currentBounds;
				SafeArea.SafeAreaDetails.TestHook_LastKnownVisibleBounds = new Windows.Foundation.Rect(0, 0, 1, 1);
				SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending = false;

				details.TestHook_InvokeUpdateInsets(forceUpdate: false);
				Assert.IsTrue(
					SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending,
					"Step 1: Guard must defer once when VB changed ahead of Bounds.");

				// Step 2: Simulate Bounds catching up between deferred callbacks.
				// In reality, Window.Bounds changes → currentBounds differs from cached.
				// We simulate this by changing the cached value so the comparison yields
				// boundsChanged=true when XamlWindow.Current?.Bounds is read.
				SafeArea.SafeAreaDetails.TestHook_LastKnownBounds = new Windows.Foundation.Rect(0, 0, 1, 1);

				details.TestHook_InvokeUpdateInsets(forceUpdate: true);
				Assert.IsFalse(
					SafeArea.SafeAreaDetails.TestHook_BoundsTransitionPending,
					"Step 2: Guard must clear pending when Bounds have caught up.");
				Assert.AreEqual(currentBounds, SafeArea.SafeAreaDetails.TestHook_LastKnownBounds,
					"Step 2: LastKnownBounds must be updated to the new Bounds value.");
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
		// Android apps render edge-to-edge on Uno Platform 7: the system bars always overlap the window, and SafeArea
		// pads content by the insets reported through VisibleBounds.
		[TestMethod]
		public async Task SystemBars_Insets_Applied()
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
			await WaitForStatusBarInset(redGrid);

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
		public async Task SystemBars_Insets_Updated_WhenBarsHidden()
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
			await WaitForStatusBarInset(redGrid);

			var visibleBoundsWithBars = ApplicationView.GetForCurrentView().VisibleBounds;

			using var __ = UseFullScreen();
			await WaitForVisibleBoundsChange(visibleBoundsWithBars);
			await UnitTestsUIContentHelper.WaitForIdle();

			var blueWithoutBars = blueGrid.TransformToVisual(redGrid).TransformBounds(new Windows.Foundation.Rect(0, 0, blueGrid.ActualWidth, blueGrid.ActualHeight));
			var redWithoutBars = redGrid.TransformToVisual(redGrid).TransformBounds(new Windows.Foundation.Rect(0, 0, redGrid.ActualWidth, redGrid.ActualHeight));
			var visibleBoundsWithoutBars = ApplicationView.GetForCurrentView().VisibleBounds;

			var topInset = visibleBoundsWithoutBars.Top - redWithoutBars.Top;
			var bottomInset = redWithoutBars.Bottom - visibleBoundsWithoutBars.Bottom;

			Assert.AreEqual(blueWithoutBars.Top, topInset, message: $"Blue rect top: {blueWithoutBars.Top} should equal the top inset: {topInset}");
			Assert.AreEqual(blueWithoutBars.Bottom, redWithoutBars.Bottom - bottomInset, message: $"Blue rect bottom: {blueWithoutBars.Bottom} should be offset by the bottom inset: {bottomInset}");
			Assert.AreEqual(redGrid.Padding.Top, topInset, message: $"Red rect padding top: {redGrid.Padding.Top} should be equal to the top inset: {topInset}");
			Assert.AreEqual(redGrid.Padding.Bottom, bottomInset, message: $"Red rect padding bottom: {redGrid.Padding.Bottom} should be equal to the bottom inset: {bottomInset}");
		}

		[TestMethod]
		public async Task BottomInset_NotInflated_WhenStatusBarBackgroundSet()
		{
			// Guard spec: SafeArea.cs → Branch 2 defers to prevent this inflation.
			// Original fix: https://github.com/unoplatform/uno.toolkit.ui/pull/1554
			// Full R&D recap: specs/safearea-bounds-guard/recap.md
			//
			// Regression test: from API 35, a status bar background moves the window below the status bar, which changes both
			// Window.Bounds and VisibleBounds. Uno raises VisibleBoundsChanged before it updates Window.Bounds, so an inset
			// computed against the stale, taller Bounds adds the status bar height to the bottom inset.
			// Before API 35, the background does not resize the window, and the bottom inset must simply stay put.
			using var _ = SetupWindow();

			// Start from a window that spans the status bar.
			using var __ = UseStatusBarBackground(null);

			var (root, tabBar, tabBarContent) = CreateBottomTabBarLayout();

			await UnitTestUIContentHelperEx.SetContentAndWait(root);
			await UnitTestUIContentHelperEx.WaitFor(
				() => ApplicationView.GetForCurrentView().VisibleBounds.Top > 0,
				timeoutMS: 2000,
				message: "The window is expected to span the status bar when there is no status bar background.");
			await UnitTestsUIContentHelper.WaitForIdle();

			var details = SafeArea.SafeAreaDetails.FindInstance(tabBar)
				?? throw new InvalidOperationException("SafeAreaDetails not found");
			// The stale-bounds inset is transient, as the next layout pass corrects it, so record every applied inset.
			var appliedBottomInsets = new List<double>();
			details.EffectiveInsetsApplied += (s, insets) => appliedBottomInsets.Add(insets.Bottom);

			var boundsBefore = XamlWindow.Current?.Bounds ?? default;
			var paddingBefore = tabBar.Padding.Bottom;

			using var ___ = UseStatusBarBackground(Colors.Red);
			if (OperatingSystem.IsAndroidVersionAtLeast(35))
			{
				// Without this transition, the test cannot reproduce the race.
				await UnitTestUIContentHelperEx.WaitFor(
					() => (XamlWindow.Current?.Bounds ?? default) != boundsBefore,
					timeoutMS: 2000,
					message: "Window.Bounds did not change after setting the status bar background.");
			}

			await UnitTestsUIContentHelper.WaitForIdle();

			var paddingAfter = tabBar.Padding.Bottom;
			var settledPadding = Math.Max(paddingBefore, paddingAfter);

			Assert.IsTrue(
				appliedBottomInsets.All(inset => inset <= settledPadding + 1),
				$"The bottom inset should not exceed its settled value during the transition. Before: {paddingBefore}, After: {paddingAfter}, Applied: [{string.Join(", ", appliedBottomInsets)}]");
			AssertAutoRowFits(tabBar, tabBarContent);
		}

		[TestMethod]
		public async Task BottomInset_AutoRowShrinks_WhenSystemBarsHidden()
		{
			// Hiding the system bars removes the navigation bar inset, and the Auto row hosting the SafeArea must shrink with it.
			using var _ = SetupWindow();

			var (root, tabBar, tabBarContent) = CreateBottomTabBarLayout();

			await UnitTestUIContentHelperEx.SetContentAndWait(root);
			await UnitTestUIContentHelperEx.WaitFor(
				() => tabBar.Padding.Bottom > 0,
				timeoutMS: 2000,
				message: "SafeArea did not apply the navigation bar inset; the system bars are expected to be visible.");
			await UnitTestsUIContentHelper.WaitForIdle();

			var paddingWithBars = tabBar.Padding.Bottom;
			var heightWithBars = tabBar.ActualHeight;
			var visibleBoundsWithBars = ApplicationView.GetForCurrentView().VisibleBounds;

			using var __ = UseFullScreen();
			await WaitForVisibleBoundsChange(visibleBoundsWithBars);
			await UnitTestsUIContentHelper.WaitForIdle();

			Assert.IsTrue(
				tabBar.Padding.Bottom < paddingWithBars,
				$"The bottom inset should decrease when the system bars are hidden. With bars: {paddingWithBars}, Without: {tabBar.Padding.Bottom}");
			Assert.IsTrue(
				tabBar.ActualHeight < heightWithBars,
				$"The Auto row should shrink when the bottom inset decreases. With bars: {heightWithBars}, Without: {tabBar.ActualHeight}");
			AssertAutoRowFits(tabBar, tabBarContent);
		}

		private static (Grid Root, Grid TabBar, Border TabBarContent) CreateBottomTabBarLayout()
		{
			var tabBarContent = new Border { Height = 56 };
			var tabBar = new Grid
			{
				Background = new SolidColorBrush(Colors.Red),
				Children = { tabBarContent },
			};

			var root = new Grid
			{
				RowDefinitions =
				{
					new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
					new RowDefinition { Height = GridLength.Auto },
				},
			};

			Grid.SetRow(tabBar, 1);
			root.Children.Add(new Grid { Background = new SolidColorBrush(Colors.Blue) });
			root.Children.Add(tabBar);

			// Apply SafeArea.Insets="Bottom" on the bottom element (mimics MaterialBottomTabBarStyle)
			SafeArea.SetInsets(tabBar, SafeArea.InsetMask.Bottom);

			return (root, tabBar, tabBarContent);
		}

		private static void AssertAutoRowFits(Grid tabBar, FrameworkElement tabBarContent) =>
			Assert.AreEqual(
				tabBarContent.ActualHeight + tabBar.Padding.Bottom,
				tabBar.ActualHeight,
				1d,
				$"The Auto row should fit the tab bar content and its bottom inset: {tabBar.Padding.Bottom}");

		private static async Task WaitForStatusBarInset(Grid safeAreaGrid) =>
			await UnitTestUIContentHelperEx.WaitFor(
				() => safeAreaGrid.Padding.Top > 0,
				timeoutMS: 2000,
				message: "SafeArea did not apply the status bar inset; the system bars are expected to be visible.");

		private static async Task WaitForVisibleBoundsChange(Windows.Foundation.Rect previousVisibleBounds) =>
			await UnitTestUIContentHelperEx.WaitFor(
				() => ApplicationView.GetForCurrentView().VisibleBounds != previousVisibleBounds,
				timeoutMS: 2000,
				message: "VisibleBounds did not change after hiding the system bars.");

		private static IDisposable UseFullScreen()
		{
			var applicationView = ApplicationView.GetForCurrentView();
			Assert.IsTrue(applicationView.TryEnterFullScreenMode(), "Could not enter full screen mode to hide the system bars.");

			return Disposable.Create(applicationView.ExitFullScreenMode);
		}

		private static IDisposable UseStatusBarBackground(Windows.UI.Color? color)
		{
			// Qualified, as Uno.Toolkit.UI declares a StatusBar too.
			var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
			var originalColor = statusBar.BackgroundColor;
			statusBar.BackgroundColor = color;

			return Disposable.Create(() => statusBar.BackgroundColor = originalColor);
		}

		private static IDisposable SetupWindow()
		{
			// When runtime tests are initiated through the UI Tests, each test starts in full screen, which hides the system bars.
			ApplicationView.GetForCurrentView().ExitFullScreenMode();

			// Host the content in the actual window root, so that it spans the whole window, under the system bars.
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
