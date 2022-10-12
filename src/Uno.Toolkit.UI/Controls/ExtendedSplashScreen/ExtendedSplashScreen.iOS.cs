#if __IOS__
using Foundation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uno.Extensions;
using Uno.Logging;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Graphics.Display;
#if IS_WINUI
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endif

namespace Uno.Toolkit.UI
{
	public partial class ExtendedSplashScreen
	{
		private async Task<FrameworkElement?> GetNativeSplashScreen(SplashScreen? splashScreen)
		{
			try
			{
				var infoPlistPath = NSBundle.MainBundle.PathForResource("Info", "plist");
				if (infoPlistPath is null)
				{
					return default;
				}
				var infoPlistDictionary = new NSDictionary(infoPlistPath);
				var storyboardName = infoPlistDictionary["UILaunchStoryboardName"].ToString();
				var storyboard = UIKit.UIStoryboard.FromName(storyboardName, null);
				var launchScreenViewController = storyboard.InstantiateInitialViewController();
				var launchScreenView = launchScreenViewController.View;

				// We use a Border to ensure proper layout
				var element = new Border
				{
					Child = VisualTreeHelper.AdaptNative(launchScreenView),

					// We set a background to prevent touches from going through
					Background = SolidColorBrushHelper.Transparent
				};

				return element;
			}
			catch (Exception e)
			{
				this.Log().LogError(0, e, "Error while getting native splash screen.");

				return default;
			}
		}
	}
}
#endif
