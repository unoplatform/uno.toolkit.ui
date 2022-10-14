using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Uno.Toolkit.UI;
using Uno.UI;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Graphics.Display;

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
using Page = Microsoft.UI.Xaml.Controls.Page;
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
using Page = Windows.UI.Xaml.Controls.Page;
#endif

namespace Uno.Toolkit.Samples
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public sealed partial class App : Application
	{
		private XamlWindow _window;

		private Shell _shell;
		public static App Instance => Current as App;
		public XamlWindow Window => _window;

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			InitializeLogging();

			this.InitializeComponent();

#if HAS_UNO || NETFX_CORE
			this.Suspending += OnSuspending;
#endif

#if HAS_UNO
			FeatureConfiguration.Style.SetUWPDefaultStylesOverride<Frame>(false);
#endif
#if __ANDROID__ && USE_UITESTS
			FeatureConfiguration.NativeFramePresenter.AndroidUnloadInactivePages = true;
#endif
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="e">Details about the launch request and process.</param>
		protected override async void OnLaunched(XamlLaunchActivatedEventArgs e)
		{
#if __IOS__ && !NET6_0 && USE_UITESTS
			Xamarin.Calabash.Start();
#endif

#if WINDOWS_UWP
			Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 568)); // (size of the iPhone SE)
#endif

#if NET5_0_OR_GREATER && WINDOWS && !HAS_UNO
            _window = new XamlWindow();
            _window.Activate();
#else
			_window = XamlWindow.Current;
#endif

			if(_window.Content is null)
			{
				var loadable = new ManualLoadable { IsExecuting=true};
				var splash = new ExtendedSplashScreen {
					Window= _window,
#if IS_WINUI
					SplashScreen = e.UWPLaunchActivatedEventArgs.SplashScreen,
#else
					SplashScreen = e.SplashScreen,
#endif
					Source = loadable };
				_window.Content = splash;
				// Ensure the current window is active
				_window.Activate();

				// Uncomment this line to see the splashscreen for longer
				// await Task.Delay(10000);

				// This is to allow the splash/loading view to start to render, before we finishing building shell etc
				await Task.Yield();

				splash.Content = _shell = BuildShell();
				loadable.IsExecuting = false;
			}

		}

		private class ManualLoadable : ILoadable
		{
			private bool isExecuting;

			public bool IsExecuting {
				get => isExecuting;
				set { isExecuting = value;
					IsExecutingChanged?.Invoke(this, EventArgs.Empty);
				}
			}

			public event EventHandler IsExecutingChanged;
		}

		/// <summary>
		/// Invoked when application execution is being suspended.  Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private void OnSuspending(object sender, SuspendingEventArgs e)
		{
			var deferral = e.SuspendingOperation.GetDeferral();
			//TODO: Save application state and stop any background activity
			deferral.Complete();
		}

		/// <summary>
		/// Configures global Uno Platform logging
		/// </summary>
		private static void InitializeLogging()
		{
			var factory = LoggerFactory.Create(builder =>
			{
#if __WASM__
				builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
				builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
				builder.AddDebug();
#else
				builder.AddConsole();
#endif

				// Exclude logs below this level
				builder.SetMinimumLevel(LogLevel.Information);

				// Default filters for Uno Platform namespaces
				builder.AddFilter("Uno", LogLevel.Warning);
				builder.AddFilter("Windows", LogLevel.Warning);
				builder.AddFilter("Microsoft", LogLevel.Warning);

				// Generic Xaml events
				// builder.AddFilter("Windows.UI.Xaml", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.UIElement", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.FrameworkElement", LogLevel.Trace );

				// Layouter specific messages
				// builder.AddFilter("Windows.UI.Xaml.Controls", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Controls.Panel", LogLevel.Debug );

				// builder.AddFilter("Windows.Storage", LogLevel.Debug );

				// Binding related messages
				// builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );

				// Binder memory references tracking
				// builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

				// RemoteControl and HotReload related
				// builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

				// Debug JS interop
				// builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
			});

			global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;
		}

#if USE_UITESTS
		public static void NavBackFromNestedPage() => Shell.GetForCurrentView()?.BackNavigateFromNestedSample();
		public static void ForceNavigation(string sampleName) => (Application.Current as App)?.ForceSampleNavigation(sampleName);
		public static void ExitNestedSample() => Shell.GetForCurrentView()?.ExitNestedSample();
		public static void NavigateToNestedSample(string pageName) => (Application.Current as App)?.NavigateToNestedSampleCore(pageName);
		public static string GetDisplayScreenScaling(string value) => (DisplayInformation.GetForCurrentView().LogicalDpi * 100f / 96f).ToString(CultureInfo.InvariantCulture);

#if __IOS__
		[Export("navBackFromNestedPage:")]
		public void NavBackFromNestedPageBackdoor(NSString value) => NavBackFromNestedPage();

		[Export("forceNavigation:")]
		public void ForceNavigationBackdoor(NSString value) => ForceNavigation(value);

		[Export("exitNestedSample:")]
		public void ExitNestedSampleBackdoor(NSString value) => ExitNestedSample();

		[Export("navigateToNestedSample:")]
		public void NavigateToNestedSampleBackdoor(NSString value) => NavigateToNestedSample(value);

		[Export("getDisplayScreenScaling:")]
		public NSString GetDisplayScreenScalingBackdoor(NSString value) => new NSString(GetDisplayScreenScaling(value));
#endif
#endif
	}
}
