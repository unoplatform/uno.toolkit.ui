#if __ANDROID__ || __IOS__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Disposables;

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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif

namespace Uno.Toolkit.UI
{
	partial class NativeNavigationBarPresenter : ContentPresenter, INavigationBarPresenter
	{
		private WeakReference<NavigationBar?>? _weakNavBar;

		public void SetOwner(NavigationBar? navigationBar)
		{
			if (GetNavBar() == navigationBar)
			{
				return;
			}
			_weakNavBar = new WeakReference<NavigationBar?>(navigationBar);

			OnOwnerChanged();
		}

		private NavigationBar? GetNavBar()
		{
			if (_weakNavBar == null)
			{
				return null;
			}

			NavigationBar? targetNavBar = null;
			if (_weakNavBar.TryGetTarget(out targetNavBar))
			{
				return targetNavBar;
			}

			return null;
		}

		partial void OnOwnerChanged();
	}
}
#endif
