using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Disposables;
using Windows.Foundation.Collections;

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
	public partial class NavigationBarPresenter : ContentControl, INavigationBarPresenter
	{
		private const string XamlNavigationBarCommandBar = "XamlNavigationBarCommandBar";

		private CommandBar? _commandBar;
		private WeakReference<NavigationBar?>? _weakNavBar;
		private SerialDisposable _navBarCommandsChangedHandler = new SerialDisposable();
		private SerialDisposable _MainCommandClickedHandler = new SerialDisposable();

		public NavigationBarPresenter()
		{
			DefaultStyleKey = typeof(NavigationBarPresenter);
		}

		public void SetOwner(NavigationBar? owner)
		{
			if (GetNavBar() == owner)
			{
				return;
			}

			_weakNavBar = new WeakReference<NavigationBar?>(owner);	
		}

		private void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
		{
			if (args.Property == MainCommandProperty || args.Property == MainCommandStyleProperty)
			{
				ApplyMainCommandStyle();
			}
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			InitializeCommandBar();
		}

		private void InitializeCommandBar()
		{
			UnregisterEvents();

			_commandBar = GetTemplateChild(XamlNavigationBarCommandBar) as CommandBar;
			SetBindings();

			RegisterEvents();
		}

		private void SetBindings()
		{
			var navigationBar = GetNavBar(); 
			if (_commandBar != null && navigationBar != null)
			{
				foreach (var command in navigationBar.PrimaryCommands)
				{
					_commandBar.PrimaryCommands.Add(command);
				}

				foreach (var command in navigationBar.SecondaryCommands)
				{
					_commandBar.SecondaryCommands.Add(command);
				}

				ApplyMainCommandStyle();
			}
		}

		private void ApplyMainCommandStyle()
		{
			if (MainCommandStyle is { } mainCommandStyle && MainCommand is { } mainCommand)
			{
				mainCommand.Style = mainCommandStyle;
			}
		}

		private void OnCommandBarMainCommandClicked(object sender, RoutedEventArgs e)
		{
			GetNavBar()?.TryPerformMainCommand();
		}

		private void RegisterEvents()
		{
			var navigationBar = GetNavBar();
			if (navigationBar != null)
			{
				UnregisterEvents();

				var disposables = new CompositeDisposable(2);
				navigationBar.PrimaryCommands.VectorChanged += OnNavBarPrimaryCommandsChanged;
				navigationBar.SecondaryCommands.VectorChanged += OnNavBarSecondaryCommandsChanged;

				disposables.Add(() => navigationBar.PrimaryCommands.VectorChanged -= OnNavBarPrimaryCommandsChanged);
				disposables.Add(() => navigationBar.SecondaryCommands.VectorChanged -= OnNavBarSecondaryCommandsChanged);

				_navBarCommandsChangedHandler.Disposable = disposables;
			}

			
			var commandBarMainCommand = CommandBarExtensions.GetMainCommand(_commandBar);
			if (commandBarMainCommand != null)
			{
				commandBarMainCommand.Click += OnCommandBarMainCommandClicked;
				_MainCommandClickedHandler.Disposable = Disposable.Create(() => commandBarMainCommand.Click -= OnCommandBarMainCommandClicked);
			}

			if (_commandBar != null)
			{
				_commandBar.Opened += OnCommandBarOpened;
				_commandBar.Opening += OnCommandBarOpening;
				_commandBar.Closed += OnCommandBarClosed;
				_commandBar.Closing += OnCommandBarClosing;
				_commandBar.DynamicOverflowItemsChanging += OnCommandBarDynamicOverflowItemsChanging;
			}
		}

		private void OnCommandBarDynamicOverflowItemsChanging(CommandBar sender, DynamicOverflowItemsChangingEventArgs args)
		{
			GetNavBar()?.RaiseDynamicOverflowItemsChanging(args);
		}

		private void OnCommandBarClosing(object? sender, object e)
		{
			GetNavBar()?.RaiseClosingEvent(e);
		}

		private void OnCommandBarClosed(object? sender, object e)
		{
			GetNavBar()?.RaiseClosedEvent(e);
		}

		private void OnCommandBarOpening(object? sender, object e)
		{
			GetNavBar()?.RaiseOpeningEvent(e);
		}

		private void OnCommandBarOpened(object? sender, object e)
		{
			GetNavBar()?.RaiseOpenedEvent(e);
		}

		private void UnregisterEvents()
		{
			if (GetNavBar() != null)
			{
				_navBarCommandsChangedHandler.Disposable = null;
			}

			if (CommandBarExtensions.GetMainCommand(_commandBar) != null)
			{
				_MainCommandClickedHandler.Disposable = null;
			}
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

		private void OnNavBarPrimaryCommandsChanged(IObservableVector<ICommandBarElement> sender, IVectorChangedEventArgs args)
			=> OnCommandsChanged(sender, args, CommandBar.PrimaryCommandsProperty);

		private void OnNavBarSecondaryCommandsChanged(IObservableVector<ICommandBarElement> sender, IVectorChangedEventArgs args) 
			=> OnCommandsChanged(sender, args, CommandBar.SecondaryCommandsProperty);

		private void OnCommandsChanged(IObservableVector<ICommandBarElement> sender, IVectorChangedEventArgs args, DependencyProperty prop)
		{
			if (_commandBar != null)
			{
				var change = args.CollectionChange;
				var changeIndex = args.Index;
				var commands = _commandBar.GetValue(prop) as IObservableVector<ICommandBarElement>;
				if (commands != null)
				{
					if (change == CollectionChange.Reset)
					{
						commands.Clear();
					}
					else if (change == CollectionChange.ItemInserted ||
						change == CollectionChange.ItemChanged)
					{
						var element = sender[(int)changeIndex];
						if (element != null)
						{
							commands[(int)changeIndex] = element;
						}
					}
					else if (change == CollectionChange.ItemRemoved)
					{
						commands.RemoveAt((int)changeIndex);
					}
				}
			}
		}
	}
}
