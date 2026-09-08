using Microsoft.Extensions.Logging;
using Uno.UI.Hosting;

namespace Uno.Toolkit.Samples;

public static class Program
{
	[STAThread]
	public static async Task Main(string[] args)
	{
		using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
		global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = loggerFactory;
		global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();

		var host = UnoPlatformHostBuilder.Create()
			.App(() => new App())
			.UseX11()
			.UseLinuxFrameBuffer()
			.UseMacOS()
			.UseWin32()
			.Build();

		await host.RunAsync();
	}
}
