using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	internal class SystemThemeHelperTests
	{
		[TestMethod]
		[DataRow(true, DisplayName = "App goes dark, host stays light")]
		[DataRow(false, DisplayName = "App goes light, host stays dark")]
		public async Task When_AppRootOverride_ThemesAppRoot_NotHost(bool appDark)
		{
			// Hosted-app topology (e.g. Hot Design): the app root is a nested child, XamlRoot.Content is the host's root.
			var hostTheme = appDark ? ElementTheme.Light : ElementTheme.Dark;
			var appRoot = new Grid();
			// explicit size: WaitForLoaded requires non-zero ActualWidth/Height
			var hostRoot = new Grid { Width = 200, Height = 200, RequestedTheme = hostTheme, Children = { appRoot } };

			await UnitTestUIContentHelperEx.SetContentAndWait(hostRoot);

			Assert.IsNotNull(appRoot.XamlRoot);
			var xamlRoot = appRoot.XamlRoot!;
			Assert.AreNotEqual((object)appRoot, xamlRoot.Content, "test precondition: the XamlRoot content must not be the app root");

			var xamlRootContent = xamlRoot.Content as FrameworkElement;
			Assert.IsNotNull(xamlRootContent);
			var xamlRootContentTheme = xamlRootContent!.RequestedTheme;

			SystemThemeHelper.SetAppRootOverride(xamlRoot, appRoot);
			try
			{
				SystemThemeHelper.SetRootTheme(xamlRoot, darkMode: appDark);
				await UnitTestsUIContentHelper.WaitForIdle();

				var expectedAppTheme = appDark ? ElementTheme.Dark : ElementTheme.Light;
				Assert.AreEqual(expectedAppTheme, appRoot.RequestedTheme);
				Assert.AreEqual(expectedAppTheme, appRoot.ActualTheme);
				Assert.AreEqual(appDark, SystemThemeHelper.IsRootInDarkMode(xamlRoot));
				Assert.AreEqual(appDark ? ApplicationTheme.Dark : ApplicationTheme.Light, SystemThemeHelper.GetRootTheme(xamlRoot));

				Assert.AreEqual(hostTheme, hostRoot.RequestedTheme);
				Assert.AreEqual(hostTheme, hostRoot.ActualTheme);
				Assert.AreEqual(xamlRootContentTheme, xamlRootContent.RequestedTheme);
			}
			finally
			{
				SystemThemeHelper.SetAppRootOverride(xamlRoot, null);
			}
		}

		[TestMethod]
		public async Task When_NoOverride_TargetsXamlRootContent()
		{
			var content = new Grid { Width = 100, Height = 100 };
			await UnitTestUIContentHelperEx.SetContentAndWait(content);

			var xamlRoot = content.XamlRoot!;
			var target = xamlRoot.Content as FrameworkElement;
			Assert.IsNotNull(target);

			var initialRequested = target!.RequestedTheme;
			var initialIsDark = SystemThemeHelper.IsRootInDarkMode(xamlRoot);
			try
			{
				SystemThemeHelper.SetRootTheme(xamlRoot, darkMode: !initialIsDark);
				await UnitTestsUIContentHelper.WaitForIdle();

				Assert.AreEqual(!initialIsDark ? ElementTheme.Dark : ElementTheme.Light, target.RequestedTheme);
				Assert.AreEqual(!initialIsDark, SystemThemeHelper.IsRootInDarkMode(xamlRoot));
			}
			finally
			{
				target.RequestedTheme = initialRequested;
				await UnitTestsUIContentHelper.WaitForIdle();
			}
		}

		[TestMethod]
		public async Task When_OverrideNotAttached_FallsBackToXamlRootContent()
		{
			var content = new Grid { Width = 100, Height = 100 };
			await UnitTestUIContentHelperEx.SetContentAndWait(content);

			var xamlRoot = content.XamlRoot!;
			var detached = new Grid(); // never attached: XamlRoot is null, so the override must be ignored
			SystemThemeHelper.SetAppRootOverride(xamlRoot, detached);
			try
			{
				var target = xamlRoot.Content as FrameworkElement;
				Assert.IsNotNull(target);
				var initialRequested = target!.RequestedTheme;
				var initialIsDark = SystemThemeHelper.IsRootInDarkMode(xamlRoot);
				try
				{
					SystemThemeHelper.SetRootTheme(xamlRoot, darkMode: !initialIsDark);
					await UnitTestsUIContentHelper.WaitForIdle();

					Assert.AreEqual(!initialIsDark ? ElementTheme.Dark : ElementTheme.Light, target.RequestedTheme);
					Assert.AreEqual(ElementTheme.Default, detached.RequestedTheme);
				}
				finally
				{
					target.RequestedTheme = initialRequested;
					await UnitTestsUIContentHelper.WaitForIdle();
				}
			}
			finally
			{
				SystemThemeHelper.SetAppRootOverride(xamlRoot, null);
			}
		}

		[TestMethod]
		public void When_NullXamlRoot_NoThrow()
		{
			SystemThemeHelper.SetApplicationTheme(null, ElementTheme.Dark);
			SystemThemeHelper.SetRootTheme(null, darkMode: true);

			// null root falls back to the OS theme
			Assert.AreEqual(SystemThemeHelper.GetCurrentOsTheme(), SystemThemeHelper.GetRootTheme(null));
		}
	}
}
