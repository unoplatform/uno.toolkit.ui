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
using Windows.Foundation;
#if __IOS__
using UIKit;
#endif

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using Microsoft.UI;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI;
using Windows.UI.Xaml;
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
#if !(__ANDROID__ || __IOS__)
		[TestMethod]
		public async Task NavigationBar_Renders_MainCommand()
		{
			var mainCommand = new AppBarButton();
			var navigationBar = new NavigationBar { Content = "Title", MainCommandMode = MainCommandMode.Action, MainCommand = mainCommand };
			var content = new Grid { Children = { navigationBar } };

			await UnitTestUIContentHelperEx.SetContentAndWait(content);


			Assert.IsTrue(mainCommand.ActualHeight > 0, "MainCommand.ActualHeight is not greater than 0");
			Assert.IsTrue(mainCommand.ActualWidth > 0, "MainCommand.ActualWidth is not greater than 0");
		}

		[TestMethod]
		public async Task MainCommand_Works_From_Code_Init()
		{
			bool success = false;
			var frame = new Frame() { Width = 400, Height = 400 };
			frame.Navigated += (s, e) =>
			{
				success = e.NavigationMode == NavigationMode.Back && e.SourcePageType == typeof(FirstPage);
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(frame);


			frame.Navigate(typeof(FirstPage));
			frame.Navigate(typeof(LabelTitlePage));

			await UnitTestsUIContentHelper.WaitForIdle();

			var page = frame.Content as LabelTitlePage;

#if HAS_UNO
			page?.FindChild<NavigationBar>()?.MainCommand.RaiseClick();
#endif

			Assert.IsTrue(success);
		}
#endif

		[TestMethod]
		[DataRow(MainCommandMode.Back, DisplayName = nameof(MainCommandMode.Back))]
		[DataRow(MainCommandMode.Action, DisplayName = nameof(MainCommandMode.Action))]
		public async Task MainCommand_In_Popup_Without_Page(MainCommandMode mainCommandMode)
		{
			var shouldGoBack = mainCommandMode == MainCommandMode.Back;
			var navigationBar = new NavigationBar { Content = "Title", MainCommandMode = mainCommandMode };
			var popup = new Popup { Width = 100, Height = 100, HorizontalOffset = 100, VerticalOffset = 100, Child = new StackPanel { Children = { navigationBar } } };
			var content = new StackPanel { Children = { popup } };

			EventHandler<object> popupOpened = async (s, e) => 
			{
				Assert.IsTrue(navigationBar.TryPerformMainCommand() == shouldGoBack, "Unexpected result from TryPerformMainCommand");

				await UnitTestsUIContentHelper.WaitForIdle();

				Assert.IsTrue(popup.IsOpen == !shouldGoBack, "Popup is in an incorrect state");
			};

			try
			{
				await UnitTestUIContentHelperEx.SetContentAndWait(content);

				popup.Opened += popupOpened;
				popup.IsOpen = true;
			}
			finally
			{
				popup.Opened -= popupOpened;
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

		[TestMethod]
		public async Task MainCommand_Hidden_Mode_Hides_Button()
		{
			var mainCommand = new AppBarButton();
			var navigationBar = new NavigationBar 
			{ 
				Content = "Title", 
				MainCommandMode = MainCommandMode.Hidden, 
				MainCommand = mainCommand 
			};
			var content = new Grid { Children = { navigationBar } };

			await UnitTestUIContentHelperEx.SetContentAndWait(content);

			Assert.AreEqual(Visibility.Collapsed, mainCommand.Visibility, "MainCommand should be collapsed when MainCommandMode is Hidden");
			Assert.IsFalse(navigationBar.TryPerformMainCommand(), "TryPerformMainCommand should return false when MainCommandMode is Hidden");
		}

		[TestMethod]
		public async Task MainCommand_Hidden_Mode_In_Frame_Stays_Hidden()
		{
			var frame = new Frame() { Width = 400, Height = 400 };
			await UnitTestUIContentHelperEx.SetContentAndWait(frame);

			frame.Navigate(typeof(NavBarFirstPage));
			await UnitTestsUIContentHelper.WaitForIdle();

			var firstPage = frame.Content as NavBarFirstPage;
			var firstNavBar = firstPage?.FindChild<NavigationBar>();
			
			if (firstNavBar != null)
			{
				firstNavBar.MainCommandMode = MainCommandMode.Hidden;
				await UnitTestsUIContentHelper.WaitForIdle();
				
				// Navigate to a second page to create backstack
				frame.Navigate(typeof(NavBarSecondPage));
				await UnitTestsUIContentHelper.WaitForIdle();

				var secondPage = frame.Content as NavBarSecondPage;
				var secondNavBar = secondPage?.FindChild<NavigationBar>();
				
				if (secondNavBar != null)
				{
					secondNavBar.MainCommandMode = MainCommandMode.Hidden;
					await UnitTestsUIContentHelper.WaitForIdle();

					// Even with backstack, Hidden mode should keep button collapsed
					Assert.AreEqual(Visibility.Collapsed, secondNavBar.MainCommand?.Visibility, 
						"MainCommand should remain collapsed in Hidden mode even with backstack");
					Assert.IsFalse(secondNavBar.TryPerformMainCommand(), 
						"TryPerformMainCommand should return false in Hidden mode");
				}
			}
		}

#if __ANDROID__ || __IOS__
		[TestMethod]
		public async Task NavigationBar_Dynamic_Background()
		{
			var frame = new Frame() { Width = 400, Height = 400 };
			await UnitTestUIContentHelperEx.SetContentAndWait(frame);
			await UnitTestsUIContentHelper.WaitForIdle();

			var navBar = await frame.NavigateAndGetNavBar<RedNavBarPage>();
			navBar!.Content = "Hello";
			Assert.IsTrue(navBar!.Background is SolidColorBrush redBrush && redBrush.Color == Colors.Red);
			await UnitTestsUIContentHelper.WaitForIdle();

			try
			{
				navBar!.Background = new SolidColorBrush(Colors.Green);
				navBar!.Padding = new Thickness(20);
			}
			catch (Exception ex)
			{
				Assert.Fail("Expected no exception, but got: " + ex.Message);
			}

			Assert.IsTrue(navBar!.Background is SolidColorBrush greenBrush && greenBrush.Color == Colors.Green);
		}

		private sealed partial class RedNavBarPage : Page
		{
			public RedNavBarPage()
			{
				Content = new NavigationBar
				{
					Background = new SolidColorBrush(Colors.Red),
				};
			}
		}

		[TestMethod]
		[RequiresFullWindow]
		[DataRow(typeof(FontIconPage), DisplayName = nameof(FontIconPage))]
		[DataRow(typeof(PathIconPage), DisplayName = nameof(PathIconPage))]
		[DataRow(typeof(SymbolIconPage), DisplayName = nameof(SymbolIconPage))]
		public async Task NavigationBar_Renders_With_Invalid_AppBarButton_IconElement(Type pageType)
		{
			var frame = new Frame { Width = 200, Height = 200 };

			await UnitTestUIContentHelperEx.SetContentAndWait(frame);

			var navBar = await frame.NavigateAndGetNavBar(pageType);
			AssertNavigationBar(frame);
		}

		[TestMethod]
		[RequiresFullWindow]
		public async Task Can_Navigate_Forward_And_Backwards()
		{
			var frame = new Frame() { Width = 400, Height = 400 };
			var content = new Grid { Children = { frame } };
			
			await UnitTestUIContentHelperEx.SetContentAndWait(content);

			await UnitTestsUIContentHelper.WaitForIdle();

			var firstNavBar = await frame.NavigateAndGetNavBar<NavBarFirstPage>();

			await UnitTestsUIContentHelper.WaitForLoaded(firstNavBar!);

			var secondNavBar = await frame.NavigateAndGetNavBar<NavBarSecondPage>();
			
			await UnitTestsUIContentHelper.WaitForLoaded(secondNavBar!);

			await Task.Delay(1000);

			frame.GoBack();

			await UnitTestsUIContentHelper.WaitForLoaded(firstNavBar!);
		}


#if __ANDROID__
		private static void AssertNavigationBar(Frame frame)
		{
			var page = frame.Content as Page;
			var navBar = page?.FindChild<NavigationBar>();

			var renderedNativeNavBar = navBar.GetNativeNavBar();

			Assert.IsNotNull(renderedNativeNavBar);

			Assert.IsTrue(renderedNativeNavBar!.Height > 0, "Native toolbar height is not greater than 0");
			Assert.IsTrue(renderedNativeNavBar!.Width > 0, "Native toolbar width is not greater than 0");
		}
#endif

#if __IOS__
		[TestMethod]
		public async Task Can_Set_MainCommand_Label_Or_Content()
		{
			Frame frame = new Frame() { Width = 400, Height = 400 };
			await UnitTestUIContentHelperEx.SetContentAndWait(frame);
			await UnitTestsUIContentHelper.WaitForIdle();

			// FirstPage
			var firstNavBar = await frame.NavigateAndGetNavBar<FirstPage>();
			Assert.IsNull(firstNavBar?.GetNativeNavBar()?.BackItem);

			// LabelTitlePage
			var labelTitleNavBar = await frame.NavigateAndGetNavBar<LabelTitlePage>();
			Assert.AreEqual("Label Title", labelTitleNavBar?.GetNativeNavBar()?.BackItem?.BackButtonTitle);

			// ContentTitlePage 
			var contentTitleNavBar = await frame.NavigateAndGetNavBar<ContentTitlePage>();
			Assert.AreEqual("Content Title", contentTitleNavBar?.GetNativeNavBar()?.BackItem?.BackButtonTitle);
		}

		[TestMethod]
		public async Task MainCommand_Use_LeftBarButtonItem_When_No_BackStack()
		{
			NavigationBar? firstPageNavBar = null;
			NavigationBar? secondPageNavBar = null;

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
				firstPageNavBar = firstPage?.FindChild<NavigationBar>();

				var renderedNativeNavItem = firstPageNavBar.GetNativeNavItem();

				Assert.IsNotNull(renderedNativeNavItem?.LeftBarButtonItem);

				frame.Navigate(typeof(NavBarSecondPage));

				await UnitTestsUIContentHelper.WaitForIdle();

				var secondPage = frame.Content as NavBarSecondPage;
				secondPageNavBar = secondPage?.FindChild<NavigationBar>();

				renderedNativeNavItem = secondPageNavBar.GetNativeNavItem();

				Assert.IsNull(renderedNativeNavItem?.LeftBarButtonItem);
			}
			finally
			{
				popup.IsOpen = false;
			}
		}

		[TestMethod]
		public async Task NavigationBar_Does_Render()
		{
			var frame = new Frame { Width = 200, Height = 200 }; ;
			await UnitTestUIContentHelperEx.SetContentAndWait(frame);

			frame.Navigate(typeof(NavBarSimplePage));

			await UnitTestsUIContentHelper.WaitForIdle();

			AssertNavigationBar(frame);
		}

		[TestMethod]
		public async Task NavigationBar_Does_Render_Within_AutoLayout()
		{
			var frame = new Frame { Width = 200, Height = 200 };
			await UnitTestUIContentHelperEx.SetContentAndWait(frame);

			frame.Navigate(typeof(NavBarAutoLayoutPage));

			await UnitTestsUIContentHelper.WaitForIdle();

			AssertNavigationBar(frame);
		}

		[TestMethod]
		public async Task NavigationBar_Renders_BackItem_Within_AutoLayout()
		{
			var frame = new Frame { Width = 600, Height = 200 };
			await UnitTestUIContentHelperEx.SetContentAndWait(frame);

			var firstNavBar = await frame.NavigateAndGetNavBar<NavBarAutoLayoutPage>();

			await UnitTestsUIContentHelper.WaitForIdle();

			frame.Navigate(typeof(NavBarAutoLayoutPage2));
			await UnitTestsUIContentHelper.WaitForIdle();

			await UnitTestUIContentHelperEx.WaitFor(() => firstNavBar?.GetNativeNavItem()?.BackButtonTitle == "Hello");
		}

		private static void AssertNavigationBar(Frame frame)
		{
			var page = frame.Content as Page;
			var presenter = frame.FindChild<NativeFramePresenter>();
			var navBar = page?.FindChild<NavigationBar>();

			var renderedNativeNavItem = navBar.GetNativeNavItem();
			var renderedNativeNavBar = navBar.GetNativeNavBar();

			Assert.IsNotNull(presenter);
			Assert.IsFalse(presenter!.NavigationController.NavigationBarHidden);

			Assert.AreSame(renderedNativeNavItem, presenter.NavigationController.TopViewController.NavigationItem);
			Assert.AreSame(renderedNativeNavBar, presenter.NavigationController.NavigationBar);

			Assert.IsTrue(renderedNativeNavBar!.Bounds.Height > 0, "Native toolbar height is not greater than 0");
			Assert.IsTrue(renderedNativeNavBar!.Bounds.Width > 0, "Native toolbar width is not greater than 0");
		}
#endif
#endif

		private sealed partial class FirstPage : Page
		{
			public FirstPage()
			{
				Content = new NavigationBar
				{
					Content = "First Page"
				};
			}
		}

		private sealed partial class LabelTitlePage : Page
		{
			public LabelTitlePage()
			{
				Content = new NavigationBar
				{
					MainCommand = new AppBarButton
					{
						Label = "Label Title"
					}
				};
			}
		}
		private sealed partial class ContentTitlePage : Page
		{
			public ContentTitlePage()
			{
				Content = new NavigationBar
				{
					MainCommand = new AppBarButton
					{
						Content = "Content Title"
					}
				};
			}
		}

		private partial class NavBarTestPage : Page
		{
			protected static FrameworkElement? PageContent;
			public static IDisposable SetPageContent(FrameworkElement pageContent)
			{
				PageContent = pageContent;
				return Disposable.Create(() => PageContent = null);
			}

			public NavBarTestPage()
			{
				Content = PageContent;
			}
		}

		private sealed partial class FontIconPage : Page
		{
			public FontIconPage()
			{
				var navBar = new NavigationBar
				{
					Content = "FontIconPage"
				};

				navBar.PrimaryCommands.Add(
					new AppBarButton
					{
						Icon = new FontIcon
						{
							Glyph = "&#xE113;",
						}
					}
				);

				Content = navBar;
			}
		}

		private sealed partial class SymbolIconPage : Page
		{
			public SymbolIconPage()
			{
				var navBar = new NavigationBar
				{
					Content = "SymbolIconPage"
				};

				navBar.PrimaryCommands.Add(
					new AppBarButton
					{
						Icon = new SymbolIcon
						{
							Symbol = Symbol.Home,
						}
					}
				);

				Content = navBar;
			}
		}

		private sealed partial class PathIconPage : Page
		{
			public PathIconPage()
			{
				var navBar = new NavigationBar
				{
					Content = "PathIconPage"
				};

				navBar.PrimaryCommands.Add(
					new AppBarButton
					{
						Icon = new PathIcon(),
					}
				);

				Content = navBar;
			}
		}
	}

#if __IOS__ || __ANDROID__
	public static class NavigationBarTestHelper
	{
#if __IOS__
		public static UINavigationBar? GetNativeNavBar(this NavigationBar? navBar) => navBar
			?.TryGetNative<NavigationBar, NavigationBarRenderer, UINavigationBar>(out var native) ?? false ? native : null;

		public static UINavigationItem? GetNativeNavItem(this NavigationBar? navBar) => navBar
			?.TryGetNative<NavigationBar, NavigationBarNavigationItemRenderer, UINavigationItem>(out var native) ?? false ? native : null;

#elif __ANDROID__
		public static AndroidX.AppCompat.Widget.Toolbar? GetNativeNavBar(this NavigationBar? navBar) => navBar
			?.TryGetNative<NavigationBar, NavigationBarRenderer, AndroidX.AppCompat.Widget.Toolbar>(out var native) ?? false ? native : null;
#endif
		public static Task<NavigationBar?> NavigateAndGetNavBar<TPage>(this Frame frame) where TPage : Page
		{
			return frame.NavigateAndGetNavBar(typeof(TPage));
		}

		public static async Task<NavigationBar?> NavigateAndGetNavBar(this Frame frame, Type pageType)
		{
			frame.Navigate(pageType);
			await UnitTestsUIContentHelper.WaitForIdle();

			var page = frame.Content as Page;
			await UnitTestsUIContentHelper.WaitForLoaded(page!);
			return page?.FindChild<NavigationBar>();
		}
	}
#endif
}
