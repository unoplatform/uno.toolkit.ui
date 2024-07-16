#if __ANDROID__
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
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif

namespace Uno.Toolkit.UI
{
	public partial class NativeNavigationBarPresenter : ContentPresenter, INavigationBarPresenter
	{
		private SerialDisposable _mainCommandClickHandler = new SerialDisposable();

		public NativeNavigationBarPresenter()
		{
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			_mainCommandClickHandler.Disposable = null;
		}

		public void SetOwner(NavigationBar navigationBar)
		{
			//Owner is accessed through TemplatedParent on Uno platforms
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			var navBar = TemplatedParent as NavigationBar;
			if (navBar is { })
			{
				Content = navBar.GetOrAddDefaultRenderer().Native;
				ContentTemplate = null; // normally, the ContentTemplate is inherited from the NavigationBar, but in this case, we don't want it to. We want to use the renderer directly as the child.
				navBar.MainCommand.Click += OnMainCommandClicked;
				_mainCommandClickHandler.Disposable = null;
				_mainCommandClickHandler.Disposable = Disposable.Create(() => navBar.MainCommand.Click -= OnMainCommandClicked);
			}
		}

		private void OnMainCommandClicked(object sender, RoutedEventArgs e)
		{
			var navBar = TemplatedParent as NavigationBar;
			navBar?.TryPerformMainCommand();
		}
	}
}
#endif
