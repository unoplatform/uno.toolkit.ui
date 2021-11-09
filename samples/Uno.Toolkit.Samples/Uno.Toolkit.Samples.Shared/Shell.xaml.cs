using System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Uno.Toolkit.Samples.Helpers;

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
#if __ANDROID__
			EnableDuoNavViewSupport();
#endif
			this.Loaded += OnLoaded;

			NestedSampleFrame.RegisterPropertyChangedCallback(ContentControl.ContentProperty, OnNestedSampleFrameChanged);
			SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) => e.Handled = BackNavigateFromNestedSample();
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

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			SetDarkLightToggleInitialState();
		}

		private void SetDarkLightToggleInitialState()
		{
			// Initialize the toggle to the current theme.
			var root = XamlWindow.Current.Content as FrameworkElement;

			switch (root.ActualTheme)
			{
				case ElementTheme.Default:
					DarkLightModeToggle.IsChecked = SystemThemeHelper.GetSystemApplicationTheme() == ApplicationTheme.Dark;
					break;
				case ElementTheme.Light:
					DarkLightModeToggle.IsChecked = false;
					break;
				case ElementTheme.Dark:
					DarkLightModeToggle.IsChecked = true;
					break;
			}
		}

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

		private void ToggleButton_Click(object sender, RoutedEventArgs e)
		{
			// Set theme for window root.
			if (XamlWindow.Current.Content is FrameworkElement root)
			{
				switch (root.ActualTheme)
				{
					case ElementTheme.Default:
						if (SystemThemeHelper.GetSystemApplicationTheme() == ApplicationTheme.Dark)
						{
							root.RequestedTheme = ElementTheme.Light;
						}
						else
						{
							root.RequestedTheme = ElementTheme.Dark;
						}
						break;
					case ElementTheme.Light:
						root.RequestedTheme = ElementTheme.Dark;
						break;
					case ElementTheme.Dark:
						root.RequestedTheme = ElementTheme.Light;
						break;
				}

				if (NavigationViewControl.PaneDisplayMode == MUXC.NavigationViewPaneDisplayMode.LeftMinimal)
				{
					// Close navigation view when changing the theme
					// to allow the user to see the difference between the themes.
					NavigationViewControl.IsPaneOpen = false;
				}
			}
		}

		private void OnNestedSampleFrameChanged(DependencyObject sender, DependencyProperty dp)
		{
			var isInsideNestedSample = NestedSampleFrame.Content != null;

			NavViewToggleButton.Visibility = isInsideNestedSample
				? Visibility.Collapsed
				: Visibility.Visible;

			// prevent empty frame from blocking the content (nav-view) behind it
			NestedSampleFrame.Visibility = isInsideNestedSample
				? Visibility.Visible
				: Visibility.Collapsed;

			// toggle built-in back button for wasm (from browser) and uwp (on title bar)
			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = isInsideNestedSample
				? AppViewBackButtonVisibility.Visible
				: AppViewBackButtonVisibility.Collapsed;
		}

		public void ShowNestedSample<TPage>(bool? clearStack = null) where TPage : Page
		{
			var wasFrameEmpty = NestedSampleFrame.Content == null;
			if (NestedSampleFrame.Navigate(typeof(TPage)) && (clearStack ?? wasFrameEmpty))
			{
				NestedSampleFrame.BackStack.Clear();
			}
		}

		public bool BackNavigateFromNestedSample()
		{
			if (NestedSampleFrame.Content == null)
			{
				return false;
			}

			if (NestedSampleFrame.CanGoBack)
			{
				NestedSampleFrame.GoBack();
			}
			else
			{
				NestedSampleFrame.Content = null;

#if __IOS__
				// This will force reset the UINavigationController, to prevent the back button from appearing when the stack is supposely empty.
				// note: Merely setting the Frame.Content to null, doesnt fully reset the stack.
				// When revisiting the page1 again, the previous page1 is still in the UINavigationController stack
				// causing a back button to appear that takes us back to the previous page1
				NestedSampleFrame.BackStack.Add(default);
				NestedSampleFrame.BackStack.Clear();
#endif
			}

			return true;
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
				NavigationViewControl.IsPaneOpen = !NavigationViewControl.IsPaneVisible;
			}
		}

		private void NavigationViewControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
#if __IOS__ || __ANDROID__
			// disabling the code below, as the drawer nav-view style will be used for these platforms
			// which only supports only the overlay pane mode for now
			// so, there is no needs to respond to size changes
#else
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
#endif
		}

#if __ANDROID__
		private void EnableDuoNavViewSupport()
		{
			// check for duo-screen capability
			if (!ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.Spanning)) return;

			// replacing the RootSplitView implementation (via its style template) of the nav-view
			// will allow us to inject or remove the support for duo-screen
			if (NavigationView.GetTemplateChild("RootSplitView") is SplitView rootSP)
			{
				var twoPaneSPStyle = Application.Current.Resources["TwoPaneSplitViewStyle"] as Style ?? throw new Exception("Missing resource: TwoPaneSplitViewStyle");
				var oldStyle = rootSP.Style;

				XamlWindow.Current.SizeChanged += (s, e) =>
				{
					var targetStyle = ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.Spanning
						? twoPaneSPStyle : oldStyle;
					if (rootSP.Style != targetStyle)
					{
						rootSP.Style = targetStyle;
					}
				};
			}
		}
#endif
	}
}
