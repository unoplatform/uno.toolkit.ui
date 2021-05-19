using System;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using XamlLaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;
using XamlWindow = Microsoft.UI.Xaml.Window;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XamlLaunchActivatedEventArgs = Windows.ApplicationModel.Activation.LaunchActivatedEventArgs;
using XamlWindow = Windows.UI.Xaml.Window;
#endif
using MUXC = Microsoft.UI.Xaml.Controls;


namespace Uno.Toolkit.Samples
{
	public sealed partial class Shell : UserControl
	{
		public Shell()
		{
			this.InitializeComponent();

			InitializeSafeArea();
		}

		public static Shell GetForCurrentView() => (Shell)XamlWindow.Current.Content;

		public MUXC.NavigationView NavigationView => NavigationViewControl;

		public string CurrentSampleBackdoor
		{
			get { return (string)GetValue(CurrentSampleBackdoorProperty); }
			set { SetValue(CurrentSampleBackdoorProperty, value); }
		}

		public static DependencyProperty CurrentSampleBackdoorProperty { get; } =
			DependencyProperty.Register(nameof(CurrentSampleBackdoor), typeof(string), typeof(Shell), new PropertyMetadata(null));


		/// <summary>
		/// This method handles the top padding for phones like iPhone X.
		/// </summary>
		private void InitializeSafeArea()
		{
			var full = XamlWindow.Current.Bounds;
			var bounds = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBounds;

			var topPadding = Math.Abs(full.Top - bounds.Top);

			if (topPadding > 0)
			{
				TopPaddingRow.Height = new GridLength(topPadding);
			}
		}

		private void NavViewToggleButton_Click(object sender, RoutedEventArgs e)
		{
			if (NavigationViewControl.PaneDisplayMode == MUXC.NavigationViewPaneDisplayMode.LeftMinimal)
			{
				NavigationViewControl.IsPaneOpen = !NavigationViewControl.IsPaneOpen;
			}
			else if (NavigationViewControl.PaneDisplayMode == MUXC.NavigationViewPaneDisplayMode.Left)
			{
				NavigationViewControl.IsPaneVisible = !NavigationViewControl.IsPaneVisible;
				NavigationViewControl.IsPaneOpen = NavigationViewControl.IsPaneVisible;
			}
		}

		private void NavigationViewControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			// This could be done using VisualState with Adaptive triggers, but an issue prevents this currently - https://github.com/unoplatform/uno/issues/5168
			var desktopWidth = (double)Application.Current.Resources["DesktopAdaptiveThresholdWidth"];
			if (e.NewSize.Width >= desktopWidth && NavigationViewControl.PaneDisplayMode != MUXC.NavigationViewPaneDisplayMode.Left)
			{
				NavigationViewControl.PaneDisplayMode = MUXC.NavigationViewPaneDisplayMode.Left;
				NavigationViewControl.IsPaneOpen = true;
			}
			else if (e.NewSize.Width < desktopWidth && NavigationViewControl.PaneDisplayMode != MUXC.NavigationViewPaneDisplayMode.LeftMinimal)
			{
				NavigationViewControl.IsPaneVisible = true;
				NavigationViewControl.PaneDisplayMode = MUXC.NavigationViewPaneDisplayMode.LeftMinimal;
			}
		}
	}
}
