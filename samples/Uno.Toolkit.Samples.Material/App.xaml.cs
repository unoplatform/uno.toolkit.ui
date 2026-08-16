using Uno.Toolkit.Samples.RuntimeTesting;

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
			MainWindow = new Window();
#if DEBUG
			MainWindow.UseStudio();
#endif

			if (TryStartRuntimeTests(e))
			{
				return;
			}

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

	private bool TryStartRuntimeTests(LaunchActivatedEventArgs args)
	{
		var isRuntimeTestMode = RuntimeTestModeDetector.IsRuntimeTestMode(
			args?.Arguments,
			Environment.GetCommandLineArgs());

		if (!isRuntimeTestMode)
		{
			return false;
		}

		MainWindow.Content = new RuntimeTestRunner();
		MainWindow.Activate();
		return true;
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
			builder.AddFilter("Uno.UI.RuntimeTests", LogLevel.Trace);
			builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Trace);
			builder.AddFilter("Windows", LogLevel.Warning);
			builder.AddFilter("Microsoft", LogLevel.Warning);
		});

		global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
		global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
#endif
	}

}
