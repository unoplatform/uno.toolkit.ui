#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif

namespace Uno.Toolkit.UI.Controls
{
	public partial class NativeNavigationBarPresenter : ContentPresenter, INavigationBarPresenter
	{
		public NativeNavigationBarPresenter()
		{
			Loaded += OnLoaded;
		}

		public void SetOwner(NavigationBar navigationBar)
		{
			//Owner is accessed through TemplatedParent on Uno platforms
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			var navBar = TemplatedParent as NavigationBar;
			Content = navBar?.GetRenderer(() => new NavigationBarRenderer(navBar)).Native;
		}
	}
}
#endif