using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Default styles for the controls in the Uno.Toolkit.UI library.
	/// </summary>
	public sealed class ToolkitResources : ResourceDictionary
	{
		private const string PackageName =
#if IS_WINUI
			"Uno.Toolkit.WinUI";
#else
			"Uno.Toolkit.UI";
#endif

		public ToolkitResources()
		{
			var dicts = new string[]
			{
				"Controls/AutoLayout/AutoLayout.xaml",
				"Controls/DrawerControl/DrawerControl.xaml",
				"Controls/DrawerControl/DrawerControl.Enhanced.xaml",
				"Controls/DrawerFlyout/DrawerFlyoutPresenter.xaml",
				"Controls/LoadingView/LoadingView.xaml",
				"Controls/ExtendedSplashScreen/ExtendedSplashScreen.xaml",
#if __IOS__ || __ANDROID__
				"Controls/NavigationBar/NavigationBar.Native.xaml",
				"Behaviors/FlipView.Mobile.xaml",
#else
				"Controls/NavigationBar/NavigationBar.xaml",
				"Behaviors/FlipView.xaml",
#endif
				"Controls/TabBar/TabBar.xaml",
			};
			foreach (var dict in dicts)
			{
				MergedDictionaries.Add(new ResourceDictionary
				{
					Source = new Uri($"ms-appx:///{PackageName}/{dict}")
				});
			}
		}
	}
}
