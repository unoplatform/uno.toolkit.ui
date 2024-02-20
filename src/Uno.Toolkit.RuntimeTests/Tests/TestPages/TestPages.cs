using System;
using Uno.Toolkit.UI;
using Uno.Disposables;


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

namespace Uno.Toolkit.RuntimeTests.Tests.TestPages
{
	public sealed partial class FirstPage : Page
	{
		public FirstPage()
		{
			Content = new NavigationBar
			{
				Content = "First Page"
			};
		}
	}

	public sealed partial class LabelTitlePage : Page
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
	public sealed partial class ContentTitlePage : Page
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

	public partial class NavBarTestPage : Page
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

	public sealed partial class FontIconPage : Page
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

	public sealed partial class SymbolIconPage : Page
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

	public sealed partial class PathIconPage : Page
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

	public sealed partial class RedNavBarPage : Page
	{
		public RedNavBarPage()
		{
			Content = new NavigationBar
			{
				Background = new SolidColorBrush(Colors.Red),
			};
		}
	}
}
