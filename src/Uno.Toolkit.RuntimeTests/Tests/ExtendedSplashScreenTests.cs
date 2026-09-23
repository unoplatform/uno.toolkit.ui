using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.Extensions;
using Uno.UI.RuntimeTests;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI;
using Windows.UI.Xaml;
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
		var host = await ExtendedSplashScreen.GetNativeSplashScreen().ConfigureAwait(false) ?? throw new Exception("Failed to load native splash screen");

		await AssertSplashImageLoads(host);
	}

	[TestMethod]
	public async Task When_Resizetizer_Definition_Embedded_Then_Image_Loads()
	{
		// The sample apps declare an UnoSplashScreen, so Uno.Resizetizer embeds its definition on every target.
		// This is the only splash screen source on Android and iOS, and the fallback everywhere else.
		var splash = ExtendedSplashScreen.LoadSplashScreenFromResizetizerDefinition() ?? throw new Exception("Failed to find the embedded splash screen definition");

		await AssertSplashImageLoads(ExtendedSplashScreen.BuildSplashScreen(splash));
	}

#if __ANDROID__ || __IOS__
	[TestMethod]
	public async Task When_Mobile_Then_Splash_Screen_Is_Built_Synchronously()
	{
		// The first frame dismisses the operating system splash screen, so the splash screen must not wait on manifest lookups.
		var loading = ExtendedSplashScreen.GetNativeSplashScreen();

		Assert.IsTrue(loading.IsCompleted, "The splash screen should be built from the embedded definition without awaiting.");

		await AssertSplashImageLoads(await loading ?? throw new Exception("Failed to load native splash screen"));
	}
#endif

	private static async Task AssertSplashImageLoads(FrameworkElement host)
	{
		var sut = host.GetFirstDescendant<Image>() ?? throw new Exception("Failed to find splash image control");
		var tcs = new TaskCompletionSource<(bool Success, string? Message)>();

		sut.ImageOpened += (s, e) => tcs.SetResult((Success: true, null));
		sut.ImageFailed += (s, e) => tcs.SetResult((Success: false, e.ErrorMessage));

		await UnitTestUIContentHelperEx.SetContentAndWait(host);

		if (await Task.WhenAny(tcs.Task, Task.Delay(2000)) != tcs.Task)
		{
			throw new TimeoutException("Timed out waiting on image to load");
		}

		if ((await tcs.Task) is { Success: false, Message: var message })
		{
			throw new Exception($"Failed to load image: {message}");
		}
	}

	[TestMethod]
	public void When_Platforms_Then_SplashIsEnabled_Follows_Current_Platform()
	{
		var currentPlatform =
#if WINDOWS_WINUI
			SplashScreenPlatform.Windows;
#elif __ANDROID__
			SplashScreenPlatform.Android;
#elif __IOS__
			SplashScreenPlatform.iOS;
#else
			OperatingSystem.IsBrowser() ? SplashScreenPlatform.WebAssembly : SplashScreenPlatform.Skia;
#endif

		Assert.IsTrue(new ExtendedSplashScreen { Platforms = currentPlatform }.SplashIsEnabled);
		Assert.IsFalse(new ExtendedSplashScreen { Platforms = SplashScreenPlatform.All & ~currentPlatform }.SplashIsEnabled);
	}

	[TestMethod]
	public void When_Resizetizer_Definition_Then_Image_Is_At_App_Root()
	{
		var splash = ExtendedSplashScreen.ParseResizetizerDefinition(
			@"File=..\Shared\Assets\Splash\splash_screen.svg;Link=;BaseSize=128,96;Resize=;TintColor=;Color=#FF0000;ForegroundScale=")
			?? throw new Exception("Failed to parse the splash screen definition");

		Assert.AreEqual(new Uri("ms-appx:///splash_screen.png"), splash.ImageUri);
		Assert.AreEqual(Colors.Red, splash.Background);
		Assert.AreEqual(new Size(128, 96), splash.LogoSize);
	}

	[TestMethod]
	public void When_Resizetizer_Definition_Has_Raster_Image_And_No_Optional_Values()
	{
		var splash = ExtendedSplashScreen.ParseResizetizerDefinition(
			"File=Assets/Splash/splash_screen.jpg;Link=;BaseSize=;Resize=;TintColor=;Color=;ForegroundScale=")
			?? throw new Exception("Failed to parse the splash screen definition");

		Assert.AreEqual(new Uri("ms-appx:///splash_screen.jpg"), splash.ImageUri);
		Assert.AreEqual(Colors.White, splash.Background);
		Assert.IsNull(splash.LogoSize);
	}

	[TestMethod]
	public void When_Resizetizer_Definition_Has_No_File()
	{
		Assert.IsNull(ExtendedSplashScreen.ParseResizetizerDefinition("Link=;BaseSize=128,128;Color=#FFFFFF"));
		Assert.IsNull(ExtendedSplashScreen.ParseResizetizerDefinition(null));
	}
}
