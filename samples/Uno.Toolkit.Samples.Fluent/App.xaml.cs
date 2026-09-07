using Microsoft.UI.Xaml;
using Uno.Toolkit.Samples.RuntimeTesting;
using Uno.UI.RuntimeTests;

namespace Uno.Toolkit.Samples.Fluent;

public sealed partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	public Window? MainWindow { get; private set; }

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		var runtimeTests = RuntimeTestModeDetector.IsRuntimeTestMode(args.Arguments, Environment.GetCommandLineArgs())
			|| !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("UNO_RUNTIME_TESTS_RUN_TESTS"));
		if (runtimeTests)
		{
			// The hosted runner discovers test assemblies already loaded in the process.
			GC.KeepAlive(typeof(global::Uno.Toolkit.RuntimeTests.Tests.Given_FluentToolkitTheme).Assembly);
		}
		MainWindow = new Window
		{
			Content = runtimeTests ? new UnitTestsControl() : new MainPage(),
		};
		MainWindow.Activate();
	}
}
