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
		public async Task When_HostedUnderForeignXamlRoot_ThemesOwnRoot_NotHost(bool appDark)
		{
			// Hosted-app topology: the app root is a nested child, XamlRoot.Content is the host's root.
			var hostTheme = appDark ? ElementTheme.Light : ElementTheme.Dark;
			var appRoot = new Grid();
			// explicit size: WaitForLoaded requires non-zero ActualWidth/Height
			var hostRoot = new Grid { Width = 200, Height = 200, RequestedTheme = hostTheme, Children = { appRoot } };

			await UnitTestUIContentHelperEx.SetContentAndWait(hostRoot);

			Assert.IsNotNull(appRoot.XamlRoot);
			Assert.AreNotEqual((object)appRoot, appRoot.XamlRoot!.Content, "test precondition: the XamlRoot content must not be the app root");

			var xamlRootContent = appRoot.XamlRoot.Content as FrameworkElement;
			Assert.IsNotNull(xamlRootContent);
			var xamlRootContentTheme = xamlRootContent!.RequestedTheme;

			SystemThemeHelper.SetRootTheme(appRoot, darkMode: appDark);
			await UnitTestsUIContentHelper.WaitForIdle();

			var expectedAppTheme = appDark ? ElementTheme.Dark : ElementTheme.Light;
			Assert.AreEqual(expectedAppTheme, appRoot.RequestedTheme);
			Assert.AreEqual(expectedAppTheme, appRoot.ActualTheme);
			Assert.AreEqual(appDark, SystemThemeHelper.IsRootInDarkMode(appRoot));
			Assert.AreEqual(appDark ? ApplicationTheme.Dark : ApplicationTheme.Light, SystemThemeHelper.GetRootTheme(appRoot));

			Assert.AreEqual(hostTheme, hostRoot.RequestedTheme);
			Assert.AreEqual(hostTheme, hostRoot.ActualTheme);
			Assert.AreEqual(xamlRootContentTheme, xamlRootContent.RequestedTheme);
		}

		[TestMethod]
		public void When_NullArguments_NoThrow()
		{
			SystemThemeHelper.SetApplicationTheme((FrameworkElement?)null, ElementTheme.Dark);
			SystemThemeHelper.SetApplicationTheme((Window?)null, ElementTheme.Dark);
			SystemThemeHelper.SetRootTheme((FrameworkElement?)null, darkMode: true);

			Assert.AreEqual(SystemThemeHelper.GetCurrentOsTheme(), SystemThemeHelper.GetRootTheme((FrameworkElement?)null));
			Assert.AreEqual(SystemThemeHelper.GetCurrentOsTheme(), SystemThemeHelper.GetRootTheme((Window?)null));
		}

		[TestMethod]
		public async Task When_SetApplicationTheme_OnElement_SubtreeInherits()
		{
			var child = new Border();
			var appRoot = new Grid { Children = { child } };
			var hostRoot = new Grid { Width = 200, Height = 200, RequestedTheme = ElementTheme.Light, Children = { appRoot } };

			await UnitTestUIContentHelperEx.SetContentAndWait(hostRoot);

			SystemThemeHelper.SetApplicationTheme(appRoot, ElementTheme.Dark);
			await UnitTestsUIContentHelper.WaitForIdle();

			Assert.AreEqual(ElementTheme.Dark, child.ActualTheme);
			Assert.AreEqual(ElementTheme.Light, hostRoot.ActualTheme);
		}

		[TestMethod]
		public async Task When_WindowOverload_TargetsWindowContent()
		{
			var window = UnitTestsUIContentHelper.CurrentTestWindow;
			Assert.IsNotNull(window);
			var windowRoot = window!.Content as FrameworkElement;
			Assert.IsNotNull(windowRoot);

			var initialRequested = windowRoot!.RequestedTheme;
			var initialIsDark = SystemThemeHelper.IsRootInDarkMode(window);
			try
			{
				SystemThemeHelper.SetRootTheme(window, darkMode: !initialIsDark);
				await UnitTestsUIContentHelper.WaitForIdle();

				Assert.AreEqual(!initialIsDark ? ElementTheme.Dark : ElementTheme.Light, windowRoot.RequestedTheme);
				Assert.AreEqual(!initialIsDark, SystemThemeHelper.IsRootInDarkMode(window));
				Assert.AreEqual(SystemThemeHelper.GetRootTheme(windowRoot), SystemThemeHelper.GetRootTheme(window));
			}
			finally
			{
				windowRoot.RequestedTheme = initialRequested;
				await UnitTestsUIContentHelper.WaitForIdle();
			}
		}
	}
}
