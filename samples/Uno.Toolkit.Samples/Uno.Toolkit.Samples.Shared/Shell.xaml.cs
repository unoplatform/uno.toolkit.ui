#if !IS_WINUI || HAS_UNO
#define SYS_NAV_MGR_SUPPORTED
#endif

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using Uno.Extensions;
using Uno.Toolkit.Samples.Content;
using Uno.Toolkit.Samples.Content.Controls;
using Uno.Toolkit.Samples.Content.NestedSamples;
using Uno.Toolkit.Samples.Helpers;
using Uno.Toolkit.UI;

#if __IOS__
using Foundation;
#endif

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

			this.Loaded += OnLoaded;

			NestedSampleFrame.RegisterPropertyChangedCallback(ContentControl.ContentProperty, OnNestedSampleFrameChanged);

#if SYS_NAV_MGR_SUPPORTED
			SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) => e.Handled = BackNavigateFromNestedSample();
#endif
		}

		public static Shell GetForCurrentView() => (Shell)(App.Instance.Window.Content as ExtendedSplashScreen)!.Content;

		public MUXC.NavigationView NavigationView => NavigationViewControl;

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
#if DEBUG
			this.FindName("DebugPanel"); // materialized x:Load=false element
#endif
			SetDarkLightToggleInitialState();
		}

		private void SetDarkLightToggleInitialState()
		{
#if !IS_WINUI
			// Initialize the toggle to the current theme.
			DarkModeToggle.IsChecked = SystemThemeHelper.IsAppInDarkMode();
#endif
		}

		private void ToggleButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.XamlRoot.Content is FrameworkElement root)
			{
				switch (root.ActualTheme)
				{
					case ElementTheme.Default:
						if (SystemThemeHelper.GetCurrentOsTheme() == ApplicationTheme.Dark)
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

#if SYS_NAV_MGR_SUPPORTED
			// toggle built-in back button for wasm (from browser) and uwp (on title bar)
			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = isInsideNestedSample
				? AppViewBackButtonVisibility.Visible
				: AppViewBackButtonVisibility.Collapsed;
#endif
		}

		public void ShowNestedSample(Type pageType, bool? clearStack = null)
		{
			var wasFrameEmpty = NestedSampleFrame.Content == null;
			if (NestedSampleFrame.Navigate(pageType) && (clearStack ?? wasFrameEmpty))
			{
				NestedSampleFrame.BackStack.Clear();
			}
		}

		public void ShowNestedSample<TPage>(bool? clearStack = null) where TPage : Page
		{
			ShowNestedSample(typeof(TPage), clearStack);
		}

		public bool BackNavigateFromNestedSample()
		{
			if (NestedSampleFrame.Content == null)
			{
				return false;
			}


			if (NestedSampleFrame.CanGoBack)
			{
				//Let the NavigationBar within the nested page handle the back nav logic
				return false;
			}
			else
			{
				ExitNestedSample();
			}

			return true;
		}

		public void ExitNestedSample()
		{
			var sender = NestedSampleFrame.Content;
			NestedSampleFrame.Content = null;

#if __IOS__
			// This will force reset the UINavigationController, to prevent the back button from appearing when the stack is supposely empty.
			// note: Merely setting the Frame.Content to null, doesnt fully reset the stack.
			// When revisiting the page1 again, the previous page1 is still in the UINavigationController stack
			// causing a back button to appear that takes us back to the previous page1
			NestedSampleFrame.BackStack.Add(default);
			NestedSampleFrame.BackStack.Clear();
#endif
#if __ANDROID__
			NestedSampleFrame.BackStack.Clear();
#endif

			if (NavigationView.Content is IExitNestedSampleHandler handler)
			{
				// Allows the page to be able to handle back navigation that exited nested sample.
				// There is no other ways of knowing this, since this "navigation" effectively
				// just changes the visibility of the nested frame that overlays everything.
				handler.OnExitedFromNestedSample(sender);
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

		private void DebugVT(object sender, RoutedEventArgs e)
		{
			object FindViewOfInterest()
			{
				// the view of interest is:
				// - in RuntimeTestRunner, the "UnitTestsUIContentHelper.Content"
				// - for any page, the active section of SamplePageLayout if present OR the content of that page
				// ...
				// todo: add support for [RequiresFullWindow], flyout, nested navigation sample

				var content = NavigationViewControl.Content;
				if (content is RuntimeTestRunner runner)
				{
					return runner.GetFirstDescendant<ContentControl>(x => x.Name == "unitTestContentRoot")?.Content;
				}
				else if (content is Page page)
				{
					if (page.GetFirstDescendant<SamplePageLayout>() is { } layout &&
						layout.GetActivePresenter() is { } presenter)
					{
						// presenter.Content is the optional [SampleAttribute].DataType instance
						return presenter.GetChildren().FirstOrDefault();
					}
					else
					{
						return page.Content;
					}
				}

				return null;
			}

			// for the best viewing experience, paste the tree in VSCode (you can collapse node) with `ini` syntax highlighting
			var tree = this.TreeGraph();
			var target = FindViewOfInterest();
			var targetTree = (target as DependencyObject)?.TreeGraph();

			// note: you can also tag element with unique x:Name to inspect here
			//var sut = this.GetFirstDescendant<Chip>(x => x.Name == "SUT");
			//var tree = sut?.TreeGraph();

#if WINDOWS || WINDOWS_UWP
			var data = new DataPackage();
			data.SetText(targetTree ?? tree);

			Clipboard.SetContent(data);
#elif __WASM__
			Console.WriteLine(targetTree ?? tree);
#endif

			// note: insert a breakpoint around here or uncomment the next line to debug.
			//if (Debugger.IsAttached) Debugger.Break();
		}

		private async void DebugVTAsync(object sender, RoutedEventArgs e)
		{
			// leave some time to perform action like: opening combo/flyout or navigation
			await Task.Delay(5000);

			DebugVT(sender, e);
		}
	}
}
