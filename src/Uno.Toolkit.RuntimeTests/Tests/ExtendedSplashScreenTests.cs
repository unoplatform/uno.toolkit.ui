// FindFirstDescendent is not available in UWP

#if !IS_UWP
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
#if __ANDROID__ || __IOS__
	[Ignore]
#endif
	public async Task Smoke_Test()
	{
		var host = await ExtendedSplashScreen.GetNativeSplashScreen().ConfigureAwait(false) ?? throw new Exception("Failed to load native splash screen");

#if !__MOBILE__ // ignore native platforms impl: ios,droid,macos
		var sut = host.FindFirstDescendant<Image>() ?? throw new Exception("Failed to find splash image control");
		var tcs = new TaskCompletionSource<(bool Success, string? Message)>();
 
		sut.ImageOpened += (s, e) => tcs.SetResult((Success: true, null));
		sut.ImageFailed += (s, e) => tcs.SetResult((Success: false, e.ErrorMessage));
#endif

		await UnitTestUIContentHelperEx.SetContentAndWait(host);

#if !__MOBILE__
		if (await Task.WhenAny(tcs.Task, Task.Delay(2000)) != tcs.Task)
			throw new TimeoutException("Timed out waiting on image to load");

		if ((await tcs.Task) is { Success: false, Message: var message })
			throw new Exception($"Failed to load image: {message}");
#endif
	}
}
#endif
