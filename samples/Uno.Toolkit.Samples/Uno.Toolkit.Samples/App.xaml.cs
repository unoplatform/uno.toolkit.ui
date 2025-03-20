using System;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Uno.Resizetizer;

#if __IOS__
using Foundation;
#endif

namespace Uno.Toolkit.Samples;

public partial class App : Application
{
	private Window _window;
	private Shell _shell;
	public static App Instance => Current as App;
	public Window Window => _window;

	/// <summary>
	/// Initializes the singleton application object. This is the first line of authored code
	/// executed, and as such is the logical equivalent of main() or WinMain().
	/// </summary>
	public App()
	{
		this.InitializeComponent();

#if HAS_UNO
			FeatureConfiguration.Style.SetUWPDefaultStylesOverride<Frame>(false);
#endif
#if __ANDROID__ && USE_UITESTS
			FeatureConfiguration.NativeFramePresenter.AndroidUnloadInactivePages = true;
#endif
	}

	protected Window? MainWindow { get; private set; }

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
#if __IOS__ && USE_UITESTS && !MACCATALYST
		Xamarin.Calabash.Start();
#endif
		MainWindow = new Window();
#if DEBUG
		MainWindow.UseStudio();
#endif


		// Do not repeat app initialization when the Window already has content,
		// just ensure that the window is active
		if (MainWindow.Content is not Frame rootFrame)
		{
			// Create a Frame to act as the navigation context and navigate to the first page
			rootFrame = new Frame();

			// Place the frame in the current Window
			MainWindow.Content = rootFrame;

			rootFrame.NavigationFailed += OnNavigationFailed;
		}

		if (rootFrame.Content == null)
		{
			// When the navigation stack isn't restored navigate to the first page,
			// configuring the new page by passing required information as a navigation
			// parameter
			rootFrame.Navigate(typeof(MainPage), args.Arguments);
		}

		MainWindow.SetWindowIcon();
		// Ensure the current window is active
		MainWindow.Activate();
	}

	/// <summary>
	/// Configures global Uno Platform logging
	/// </summary>
	public static void InitializeLogging()
	{
#if DEBUG
		// Logging is disabled by default for release builds, as it incurs a significant
		// initialization cost from Microsoft.Extensions.Logging setup. If startup performance
		// is a concern for your application, keep this disabled. If you're running on the web or
		// desktop targets, you can use URL or command line parameters to enable it.
		//
		// For more performance documentation: https://platform.uno/docs/articles/Uno-UI-Performance.html

		var factory = LoggerFactory.Create(builder =>
		{
#if __WASM__
			builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__ || __MACCATALYST__
			builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
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
			// builder.AddFilter("Microsoft.UI.Xaml", LogLevel.Debug );
			// builder.AddFilter("Microsoft.UI.Xaml.VisualStateGroup", LogLevel.Debug );
			// builder.AddFilter("Microsoft.UI.Xaml.StateTriggerBase", LogLevel.Debug );
			// builder.AddFilter("Microsoft.UI.Xaml.UIElement", LogLevel.Debug );
			// builder.AddFilter("Microsoft.UI.Xaml.FrameworkElement", LogLevel.Trace );

			// Layouter specific messages
			// builder.AddFilter("Microsoft.UI.Xaml.Controls", LogLevel.Debug );
			// builder.AddFilter("Microsoft.UI.Xaml.Controls.Layouter", LogLevel.Debug );
			// builder.AddFilter("Microsoft.UI.Xaml.Controls.Panel", LogLevel.Debug );

			// builder.AddFilter("Windows.Storage", LogLevel.Debug );

			// Binding related messages
			// builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug );
			// builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug );

			// Binder memory references tracking
			// builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

			// DevServer and HotReload related
			// builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

			// Debug JS interop
			// builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
		});

		global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
		global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
#endif
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
