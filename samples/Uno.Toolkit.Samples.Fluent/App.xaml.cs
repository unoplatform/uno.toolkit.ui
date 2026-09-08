using Uno.Extensions;
using Uno.Toolkit.Samples.RuntimeTesting;

namespace Uno.Toolkit.Samples;

public partial class App : Application
{
	// BuildShell initializes this before shared navigation handles any events.
	private Shell _shell = null!;
	public static App Instance => (App)Current;

	public App()
	{
		SamplePageLayout.ActiveDesign = Design.Fluent;
		InitializeComponent();
	}

	public Window? MainWindow { get; private set; }

	protected override async void OnLaunched(LaunchActivatedEventArgs args)
	{
		try
		{
			MainWindow = new Window();
			var runtimeTests = RuntimeTestModeDetector.IsRuntimeTestMode(args.Arguments, Environment.GetCommandLineArgs())
				|| !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("UNO_RUNTIME_TESTS_RUN_TESTS"));
			if (runtimeTests)
			{
				// The hosted runner discovers test assemblies already loaded in the process.
				GC.KeepAlive(typeof(global::Uno.Toolkit.RuntimeTests.Tests.Given_FluentToolkitTheme).Assembly);
				MainWindow.Content = new RuntimeTestRunner();
				MainWindow.Activate();
				return;
			}

			var loadable = new ManualLoadable { IsExecuting = true };
			var splash = new ExtendedSplashScreen
			{
				Window = MainWindow,
				Source = loadable,
			};
			MainWindow.Content = splash;
			MainWindow.Activate();
			await Task.Yield();
			splash.Content = _shell = BuildShell();
			loadable.IsExecuting = false;
		}
		catch (Exception exception)
		{
			this.Log().LogError(exception, "Failed to launch the Fluent Toolkit sample app.");
			throw;
		}
	}

	private sealed class ManualLoadable : ILoadable
	{
		private bool _isExecuting;

		public bool IsExecuting
		{
			get => _isExecuting;
			set
			{
				_isExecuting = value;
				IsExecutingChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public event EventHandler? IsExecutingChanged;
	}
}
