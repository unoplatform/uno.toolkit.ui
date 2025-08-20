using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.Extensions;
using Uno.UI.RuntimeTests;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
public class ExtendedSplashScreenTests
{
	[TestMethod]
	public async Task Smoke_Test()
	{
		var host = await ExtendedSplashScreen.GetSplashScreen().ConfigureAwait(false) ?? throw new Exception("Failed to load native splash screen");

		var sut = host.GetFirstDescendant<Image>() ?? throw new Exception("Failed to find splash image control");
		var tcs = new TaskCompletionSource<(bool Success, string? Message)>();

		sut.ImageOpened += (s, e) => tcs.SetResult((Success: true, null));
		sut.ImageFailed += (s, e) => tcs.SetResult((Success: false, e.ErrorMessage));

		await UnitTestUIContentHelperEx.SetContentAndWait(host);

		if (await Task.WhenAny(tcs.Task, Task.Delay(2000)) != tcs.Task)
			throw new TimeoutException("Timed out waiting on image to load");

		if ((await tcs.Task) is { Success: false, Message: var message })
			throw new Exception($"Failed to load image: {message}");
	}
}
