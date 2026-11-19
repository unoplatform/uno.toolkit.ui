#if IOS27_1_OR_GREATER
using System;
using System.Linq;
using UIKit;
using Windows.UI.ViewManagement;
#endif

namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	/// <summary>
	/// Shows the TabBar in the iOS 27.1 vertical bar, following the TabBar how-to.
	/// </summary>
	public sealed partial class M3MaterialIOSVerticalBarSampleNestedPage : Page
	{
		public M3MaterialIOSVerticalBarSampleNestedPage()
		{
			this.InitializeComponent();
#if IOS27_1_OR_GREATER
			Loaded += (_, _) =>
			{
				ApplicationView.GetForCurrentView().VisibleBoundsChanged += OnVisibleBoundsChanged;
				UpdateTabBarPlacement();
			};
			Unloaded += (_, _) => ApplicationView.GetForCurrentView().VisibleBoundsChanged -= OnVisibleBoundsChanged;
#endif
		}

#if IOS27_1_OR_GREATER
		// The insets change on rotation, and when the vertical bar appears after launch.
		private void OnVisibleBoundsChanged(ApplicationView sender, object args) => UpdateTabBarPlacement();

		private void UpdateTabBarPlacement()
		{
			// iOS reports the strip as a left or right safe-area inset.
			var insets = SafeArea.WindowInsets;
			var stripWidth = HasVerticalBar() ? Math.Max(insets.Left, insets.Right) : 0;
			var useStrip = stripWidth > 0;

			BottomTabBar.Visibility = useStrip ? Visibility.Collapsed : Visibility.Visible;
			VerticalBarHost.Visibility = useStrip ? Visibility.Visible : Visibility.Collapsed;
			if (useStrip)
			{
				VerticalBarHost.Width = stripWidth;
				VerticalBarHost.HorizontalAlignment = insets.Left > insets.Right ? HorizontalAlignment.Left : HorizontalAlignment.Right;
			}
		}

		private static bool HasVerticalBar()
		{
			// VerticalBarEdge is iOS-only; IsIOSVersionAtLeast also allows Mac Catalyst, which CA1416 flags.
			if (!OperatingSystem.IsOSPlatformVersionAtLeast("ios", 27, 1))
			{
				return false;
			}

			var scene = UIApplication.SharedApplication.ConnectedScenes.OfType<UIWindowScene>().FirstOrDefault();
			return scene is not null && scene.TraitCollection.VerticalBarEdge != UIVerticalBarEdge.Unspecified;
		}
#endif

		private void NavigateBack(object sender, RoutedEventArgs e)
		{
			// Normally we would've just called `Frame.GoBack();` if we only have a single frame.
			// However, a nested frame is used to show-case fullscreen sample, so we need some
			// custom handling to hide the nested frame on back navigation when the stack is empty.
			Shell.GetForCurrentView()?.BackNavigateFromNestedSample();
		}
	}
}
