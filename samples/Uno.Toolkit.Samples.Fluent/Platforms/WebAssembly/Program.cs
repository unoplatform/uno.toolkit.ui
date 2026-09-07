using Uno.UI.Hosting;

namespace Uno.Toolkit.Samples.Fluent;

public static class Program
{
	public static async Task Main(string[] args)
	{
		var host = UnoPlatformHostBuilder.Create()
			.App(() => new App())
			.UseWebAssembly()
			.Build();

		await host.RunAsync();
	}
}
