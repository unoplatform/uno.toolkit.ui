using System.Globalization;

#if __IOS__
using Foundation;
#endif

namespace Uno.Toolkit.Samples;

public partial class App : Application
{
	private Shell _shell;
	public static App Instance => Current as App;

	public App()
	{
		SamplePageLayout.ActiveDesign = Design.Material;
		this.InitializeComponent();
	}

	public Window? MainWindow { get; private set; }

	protected override async void OnLaunched(LaunchActivatedEventArgs e)
	{
#if __IOS__ && USE_UITESTS && !MACCATALYST && HAS_TESTCLOUD_AGENT
			Xamarin.Calabash.Start();
#endif
			MainWindow = new Window();
#if DEBUG
			MainWindow.UseStudio();
#endif

			if (MainWindow.Content is null)
			{
				var loadable = new ManualLoadable { IsExecuting = true };
				var splash = new ExtendedSplashScreen
				{
					Window = MainWindow,
					Source = loadable
				};
				MainWindow.Content = splash;
				MainWindow.Activate();

				await Task.Yield();

				splash.Content = _shell = BuildShell();
				loadable.IsExecuting = false;
			}

		}

		private class ManualLoadable : ILoadable
		{
			private bool isExecuting;

			public bool IsExecuting
			{
				get => isExecuting;
				set
				{
					isExecuting = value;
					IsExecutingChanged?.Invoke(this, EventArgs.Empty);
				}
			}

			public event EventHandler IsExecutingChanged;
		}

	public static void InitializeLogging()
	{
#if DEBUG
		var factory = LoggerFactory.Create(builder =>
		{
#if __WASM__
			builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__ || __MACCATALYST__
			builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#else
			builder.AddConsole();
#endif

			builder.SetMinimumLevel(LogLevel.Information);

			builder.AddFilter("Uno", LogLevel.Warning);
			builder.AddFilter("Windows", LogLevel.Warning);
			builder.AddFilter("Microsoft", LogLevel.Warning);
		});

		global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
		global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
#endif
	}

#if USE_UITESTS
#pragma warning disable CA1416 // Validate platform compatibility

#if __WASM__
	[System.Runtime.InteropServices.JavaScript.JSExport]
#endif
	public static void NavBackFromNestedPage() => Shell.GetForCurrentView()?.BackNavigateFromNestedSample();
#if __WASM__
	[System.Runtime.InteropServices.JavaScript.JSExport]
#endif
	public static void ForceNavigation(string sampleName) => (Application.Current as App)?.ForceSampleNavigation(sampleName);
#if __WASM__
	[System.Runtime.InteropServices.JavaScript.JSExport]
#endif
	public static void ExitNestedSample() => Shell.GetForCurrentView()?.ExitNestedSample();
#if __WASM__
	[System.Runtime.InteropServices.JavaScript.JSExport]
#endif
	public static void NavigateToNestedSample(string pageName) => (Application.Current as App)?.NavigateToNestedSampleCore(pageName);
#if __WASM__

	[System.Runtime.InteropServices.JavaScript.JSExport]
#endif
	public static string GetDisplayScreenScaling(string value) => (DisplayInformation.GetForCurrentView().LogicalDpi * 100f / 96f).ToString(CultureInfo.InvariantCulture);
#pragma warning restore CA1416 // Validate platform compatibility


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
