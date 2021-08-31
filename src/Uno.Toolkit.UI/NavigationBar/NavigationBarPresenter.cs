using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Disposables;
using Uno.UI.ToolkitLib.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommandBar = Microsoft.UI.Xaml.Controls.CommandBar;

namespace Uno.UI.ToolkitLib
{
	public partial class NavigationBarPresenter : ContentPresenter
	{
		private CommandBar? _commandBar;
		private WeakReference<NavigationBar?>? _weakNavBar;
		private SerialDisposable _navBarPropertyChangedRevoker = new SerialDisposable();
		private SerialDisposable _commandBarPropertyChangedRevoker = new SerialDisposable();

		public NavigationBarPresenter()
		{
			_commandBar = new CommandBar();
		}

		protected override void OnTemplatedParentChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnTemplatedParentChanged(e);
			InitializeCommandBar(TemplatedParent as NavigationBar);
		}

		private void InitializeCommandBar(NavigationBar? navigationBar)
		{
			if (_weakNavBar?.Target == navigationBar)
			{
				return;
			}

			UnregisterPropertyChanges();

			_weakNavBar = new WeakReference<NavigationBar?>(navigationBar);
			_commandBar = new CommandBar();

			SynchronizeCommandBar();
			RegisterNavBarPropertyChanges();


			Content = _commandBar;
		}

		private void UnregisterPropertyChanges()
		{
			_navBarPropertyChangedRevoker.Disposable = null;
			_commandBarPropertyChangedRevoker.Disposable = null;
		}

		private void SynchronizeCommandBar()
		{
			if (_commandBar is { }
				&& _weakNavBar?.Target is NavigationBar navigationBar)
			{
				_commandBar.SetValue(CommandBar.PrimaryCommandsProperty, navigationBar.PrimaryCommands);
				_commandBar.SetValue(CommandBar.SecondaryCommandsProperty, navigationBar.SecondaryCommands);
				_commandBar.SetValue(CommandBar.IsStickyProperty, navigationBar.IsSticky);
				_commandBar.SetValue(CommandBar.IsOpenProperty, navigationBar.IsOpen);
				_commandBar.SetValue(CommandBar.LightDismissOverlayModeProperty, navigationBar.LightDismissOverlayMode);
				_commandBar.SetValue(CommandBar.IsDynamicOverflowEnabledProperty, navigationBar.IsDynamicOverflowEnabled);
				_commandBar.SetValue(CommandBar.DefaultLabelPositionProperty, navigationBar.DefaultLabelPosition);
				_commandBar.SetValue(CommandBar.OverflowButtonVisibilityProperty, navigationBar.OverflowButtonVisibility);
				_commandBar.SetValue(CommandBar.ClosedDisplayModeProperty, navigationBar.ClosedDisplayMode);
			}
		}

		private void RegisterNavBarPropertyChanges()
		{
			if (_weakNavBar?.Target is NavigationBar navigationBar)
			{
				var disposables = new CompositeDisposable(9);
				disposables.Add(navigationBar.RegisterDisposablePropertyChangedCallback(NavigationBar.PrimaryCommandsProperty, OnNavBarPrimaryCommandsChanged));
				disposables.Add(navigationBar.RegisterDisposablePropertyChangedCallback(NavigationBar.SecondaryCommandsProperty, OnNavBarSecondaryCommandsChanged));
				disposables.Add(navigationBar.RegisterDisposablePropertyChangedCallback(NavigationBar.IsStickyProperty, OnNavBarIsStickyChanged));
				disposables.Add(navigationBar.RegisterDisposablePropertyChangedCallback(NavigationBar.IsOpenProperty, OnNavBarIsOpenChanged));
				disposables.Add(navigationBar.RegisterDisposablePropertyChangedCallback(NavigationBar.LightDismissOverlayModeProperty, OnNavBarLightDismissOverlayModeChanged));
				disposables.Add(navigationBar.RegisterDisposablePropertyChangedCallback(NavigationBar.IsDynamicOverflowEnabledProperty, OnNavBarIsDynamicOverflowEnabledChanged));
				disposables.Add(navigationBar.RegisterDisposablePropertyChangedCallback(NavigationBar.DefaultLabelPositionProperty, OnNavBarDefaultLabelPositionChanged));
				disposables.Add(navigationBar.RegisterDisposablePropertyChangedCallback(NavigationBar.OverflowButtonVisibilityProperty, OnNavBarOverflowButtonVisibilityChanged));
				disposables.Add(navigationBar.RegisterDisposablePropertyChangedCallback(NavigationBar.ClosedDisplayModeProperty, OnNavBarClosedDisplayModePropertyChanged));

				_navBarPropertyChangedRevoker.Disposable = disposables;
			}
		}

		private void OnNavBarClosedDisplayModePropertyChanged(DependencyObject sender, DependencyProperty dp)
		{
			if (_weakNavBar?.Target is NavigationBar navBar)
			{
				_commandBar?.SetValue(CommandBar.ClosedDisplayModeProperty, navBar.ClosedDisplayMode);
			}
		}

		private void OnNavBarOverflowButtonVisibilityChanged(DependencyObject sender, DependencyProperty dp)
		{
			if (_weakNavBar?.Target is NavigationBar navBar)
			{
				_commandBar?.SetValue(CommandBar.OverflowButtonVisibilityProperty, navBar.OverflowButtonVisibility);
			}
		}

		private void OnNavBarDefaultLabelPositionChanged(DependencyObject sender, DependencyProperty dp)
		{
			if (_weakNavBar?.Target is NavigationBar navBar)
			{
				_commandBar?.SetValue(CommandBar.DefaultLabelPositionProperty, navBar.DefaultLabelPosition);
			}
		}

		private void OnNavBarIsDynamicOverflowEnabledChanged(DependencyObject sender, DependencyProperty dp)
		{
			if (_weakNavBar?.Target is NavigationBar navBar)
			{
				_commandBar?.SetValue(CommandBar.IsDynamicOverflowEnabledProperty, navBar.IsDynamicOverflowEnabled);
			}
		}

		private void OnNavBarLightDismissOverlayModeChanged(DependencyObject sender, DependencyProperty dp)
		{
			if (_weakNavBar?.Target is NavigationBar navBar)
			{
				_commandBar?.SetValue(CommandBar.LightDismissOverlayModeProperty, navBar.LightDismissOverlayMode);
			}
		}

		private void OnNavBarIsOpenChanged(DependencyObject sender, DependencyProperty dp)
		{
			if (_weakNavBar?.Target is NavigationBar navBar)
			{
				_commandBar?.SetValue(CommandBar.IsOpenProperty, navBar.IsOpen);
			}
		}

		private void OnNavBarIsStickyChanged(DependencyObject sender, DependencyProperty dp)
		{
			if (_weakNavBar?.Target is NavigationBar navBar)
			{
				_commandBar?.SetValue(CommandBar.IsStickyProperty, navBar.IsSticky);
			}
		}

		private void OnNavBarPrimaryCommandsChanged(DependencyObject sender, DependencyProperty dp)
		{
			if (_weakNavBar?.Target is NavigationBar navBar)
			{
				_commandBar?.SetValue(CommandBar.PrimaryCommandsProperty, navBar.PrimaryCommands);
			}
		}

		private void OnNavBarSecondaryCommandsChanged(DependencyObject sender, DependencyProperty dp)
		{
			if (_weakNavBar?.Target is NavigationBar navBar)
			{
				_commandBar?.SetValue(CommandBar.SecondaryCommandsProperty, navBar.SecondaryCommands);
			}
		}
	}
}
