using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif
namespace Uno.Toolkit.UI
{
	public static class ContentPresenterExtensions
	{
		#region SplashScreen Attached Property
		public static bool GetSplashScreen(DependencyObject obj)
		{
			return (bool)obj.GetValue(SplashScreenProperty);
		}

		public static void SetSplashScreen(DependencyObject obj, bool value)
		{
			obj.SetValue(SplashScreenProperty, value);
		}

		/// <summary>
		/// Property that can be used to observe the position of the currently selected item within a <see cref="Selector"/>
		/// </summary>
		public static DependencyProperty SplashScreenProperty { get; } =
			DependencyProperty.RegisterAttached("SplashScreen", typeof(bool), typeof(ContentPresenterExtensions), new PropertyMetadata(false,SplashScreenChanged));

		private static void SplashScreenChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
		{
			if (args.NewValue is bool splashEnabled && splashEnabled && dependencyObject is ContentPresenter splashPresenter)
			{
				ApplySplashScreen(splashPresenter);
			}
		}

		private static void ApplySplashScreen(ContentPresenter splashPresenter)
		{
			if(!splashPresenter.IsLoaded)
			{
				splashPresenter.Loaded+=(s,e)=>ApplySplashScreen(splashPresenter);
				return;
			}
			var splashScreen = splashPresenter.FindFirstParent<LoadingView>(false)?.SplashScreen;

			var splash = ExtendedSplashScreen.GetNativeSplashScreen(splashScreen);
			splashPresenter.Content = splash;
		}
		#endregion
	}
}
