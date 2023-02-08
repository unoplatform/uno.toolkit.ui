using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Disposables;
using Uno.Toolkit.RuntimeTests.Extensions;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.RuntimeTests.Tests.TestPages;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Windows.System;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif


namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	internal partial class NavigationBarTests
	{
		[TestMethod]
		[DataRow(MainCommandMode.Back, DisplayName = nameof(MainCommandMode.Back))]
		[DataRow(MainCommandMode.Action, DisplayName = nameof(MainCommandMode.Action))]
		public async Task MainCommand_In_Popup_Without_Page(MainCommandMode mainCommandMode)
		{
			var shouldGoBack = mainCommandMode == MainCommandMode.Back;
			var navigationBar = new NavigationBar { Content = "Title", MainCommandMode = mainCommandMode };
			var popup = new Popup { Width = 100, Height = 100, HorizontalOffset = 100, VerticalOffset = 100, Child = new StackPanel { Children = { navigationBar } } };
			var content = new StackPanel { Children = { popup } };

			try
			{
				await UnitTestUIContentHelperEx.SetContentAndWait(content);

				popup.IsOpen = true;

				await UnitTestsUIContentHelper.WaitForIdle();
				await UnitTestsUIContentHelper.WaitForLoaded(popup);

				Assert.IsTrue(navigationBar.TryPerformMainCommand() == shouldGoBack, "Unexpected result from TryPerformMainCommand");

				await UnitTestsUIContentHelper.WaitForIdle();

				Assert.IsTrue(popup.IsOpen == !shouldGoBack, "Popup is in an incorrect state");
			}
			finally
			{
				popup.IsOpen = false;
			}
		}

		[TestMethod]
		[DataRow(MainCommandMode.Back, DisplayName = nameof(MainCommandMode.Back))]
		[DataRow(MainCommandMode.Action, DisplayName = nameof(MainCommandMode.Action))]
		public async Task MainCommand_In_Popup_With_Page(MainCommandMode mainCommandMode)
		{
			NavigationBar? firstPageNavBar = null;
			var shouldGoBack = mainCommandMode == MainCommandMode.Back;
			var popup = new Popup { Width = 100, Height = 100, HorizontalOffset = 100, VerticalOffset = 100 };
			var content = new Border { Width = 100, Height = 100, Child = popup };
			var frame = new Frame() { Width = 400, Height = 400 };

			popup.Child = frame;
			try
			{
				await UnitTestUIContentHelperEx.SetContentAndWait(content);

				popup.IsOpen = true;

				await UnitTestsUIContentHelper.WaitForIdle();

				frame.Navigate(typeof(NavBarFirstPage));

				await UnitTestsUIContentHelper.WaitForIdle();

				var firstPage = frame.Content as NavBarFirstPage;
				if (firstPage?.FindChild<NavigationBar>() is { } firstNavBar)
				{
					firstPageNavBar = firstNavBar;
					firstNavBar.MainCommandMode = mainCommandMode;
				}

				await UnitTestsUIContentHelper.WaitForLoaded(firstPageNavBar!);

				Assert.IsTrue(firstPageNavBar!.TryPerformMainCommand() == shouldGoBack, "Unexpected result from TryPerformMainCommand");

				await UnitTestsUIContentHelper.WaitForIdle();

				Assert.IsTrue(popup.IsOpen == !shouldGoBack, "Popup is in an incorrect state");
			}
			finally
			{
				popup.IsOpen = false;
			}
		}

		[TestMethod]
		[DataRow(MainCommandMode.Back, DisplayName = nameof(MainCommandMode.Back))]
		[DataRow(MainCommandMode.Action, DisplayName = nameof(MainCommandMode.Action))]
		public async Task MainCommand_In_Popup_With_Page_With_BackStack(MainCommandMode mainCommandMode)
		{
			NavigationBar? firstPageNavBar = null;
			NavigationBar? secondPageNavBar = null;

			var shouldGoBack = mainCommandMode == MainCommandMode.Back;
			var popup = new Popup { Width = 100, Height = 100, HorizontalOffset = 100, VerticalOffset = 100 };
			
			var content = new Border { Width = 100, Height = 100, Child = popup };
			var frame = new Frame() { Width = 400, Height = 400 };

			popup.Child = frame;

			try
			{
				await UnitTestUIContentHelperEx.SetContentAndWait(content);

				popup.IsOpen = true;

				await UnitTestsUIContentHelper.WaitForIdle();

				frame.Navigate(typeof(NavBarFirstPage));

				await UnitTestsUIContentHelper.WaitForIdle();

				var firstPage = frame.Content as NavBarFirstPage;
				if (firstPage?.FindChild<NavigationBar>() is { } firstNavBar)
				{
					firstPageNavBar = firstNavBar;
					firstNavBar.MainCommandMode = mainCommandMode;
				}

				frame.Navigate(typeof(NavBarSecondPage));

				await UnitTestsUIContentHelper.WaitForIdle();

				var secondPage = frame.Content as NavBarSecondPage;
				if (secondPage?.FindChild<NavigationBar>() is { } secondNavBar)
				{
					secondPageNavBar = secondNavBar;
					secondNavBar.MainCommandMode = mainCommandMode;
				}

				await UnitTestsUIContentHelper.WaitForLoaded(secondPageNavBar!);

				//Assert that the back was handled by the NavBar
				Assert.IsTrue(secondPageNavBar!.TryPerformMainCommand() == shouldGoBack, "Unexpected result from TryPerformMainCommand");

				if (mainCommandMode == MainCommandMode.Back)
				{
					await UnitTestsUIContentHelper.WaitForLoaded(firstPageNavBar!);
					Assert.IsTrue(frame.CurrentSourcePageType == typeof(NavBarFirstPage), "Expected to navigate back to NavBarFirstPage");
				}
				else
				{
					await UnitTestsUIContentHelper.WaitForIdle();
					Assert.IsTrue(frame.CurrentSourcePageType == typeof(NavBarSecondPage), "Expected to stay on NavBarSecondPage");
				}

				Assert.IsTrue(popup.IsOpen, "Expected Popup to remain open");

				// Now we try to GoBack again
				if (mainCommandMode == MainCommandMode.Back)
				{
					Assert.IsTrue(firstPageNavBar!.TryPerformMainCommand(), "Expected Back to be handled by NavigationBar");
				}
				else
				{
					Assert.IsFalse(secondPageNavBar!.TryPerformMainCommand(), "Expected Back to not be handled by NavigationBar");
				}
				await UnitTestsUIContentHelper.WaitForIdle();


				if (mainCommandMode == MainCommandMode.Back)
				{
					Assert.IsFalse(popup.IsOpen, "Expected Popup to be closed");
				}
				else
				{
					Assert.IsTrue(frame.CurrentSourcePageType == typeof(NavBarSecondPage), "Expected to stay on NavBarSecondPage");
				}

			}
			finally
			{
				popup.IsOpen = false;
			}
		}
	}
}

