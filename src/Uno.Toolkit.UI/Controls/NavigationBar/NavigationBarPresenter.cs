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
	public partial class NavigationBarPresenter : Control, INavigationBarPresenter
	{
		private const string XamlNavigationBarCommandBar = "XamlNavigationBarCommandBar";

		private CommandBar? _commandBar;
		private WeakReference<NavigationBar?>? _weakNavBar;
		private SerialDisposable _navBarCommandsChangedHandler = new SerialDisposable();
		private SerialDisposable _MainCommandClickedHandler = new SerialDisposable();
		private SerialDisposable _MainCommandPropertyChangedHandler = new SerialDisposable();

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

			SetBindings();
			RegisterEvents();
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
			if (_commandBar != null
				&& navigationBar != null)
			{
				void setBinding(UIElement target, object source, DependencyProperty property, string path, BindingMode mode = BindingMode.TwoWay)
				{
					var binding = new Binding
					{
						Path = new PropertyPath(path),
						Source = source,
						Mode = mode
					};

					BindingOperations.SetBinding(target, property, binding);
				}

				foreach (var command in navigationBar.PrimaryCommands)
				{
					_commandBar.PrimaryCommands.Add(command);
				}

				foreach (var command in navigationBar.SecondaryCommands)
				{
					_commandBar.SecondaryCommands.Add(command);
				}

				setBinding(_commandBar, navigationBar, CommandBar.IsStickyProperty, nameof(navigationBar.IsSticky));
				setBinding(_commandBar, navigationBar, CommandBar.IsOpenProperty, nameof(navigationBar.IsOpen));
				setBinding(_commandBar, navigationBar, CommandBar.LightDismissOverlayModeProperty, nameof(navigationBar.LightDismissOverlayMode));
				setBinding(_commandBar, navigationBar, CommandBar.IsDynamicOverflowEnabledProperty, nameof(navigationBar.IsDynamicOverflowEnabled));
				setBinding(_commandBar, navigationBar, CommandBar.ForegroundProperty, nameof(navigationBar.Foreground));
				setBinding(_commandBar, navigationBar, CommandBar.BackgroundProperty, nameof(navigationBar.Background));
				setBinding(_commandBar, navigationBar, CommandBar.BorderThicknessProperty, nameof(navigationBar.BorderThickness));
				setBinding(_commandBar, navigationBar, CommandBar.PaddingProperty, nameof(navigationBar.Padding));
				setBinding(_commandBar, navigationBar, CommandBar.HorizontalAlignmentProperty, nameof(navigationBar.HorizontalAlignment));
				setBinding(_commandBar, navigationBar, CommandBar.HorizontalContentAlignmentProperty, nameof(navigationBar.HorizontalContentAlignment));
				setBinding(_commandBar, navigationBar, CommandBar.VerticalAlignmentProperty, nameof(navigationBar.VerticalAlignment));
				setBinding(_commandBar, navigationBar, CommandBar.VerticalContentAlignmentProperty, nameof(navigationBar.VerticalContentAlignment));
				setBinding(_commandBar, navigationBar, CommandBar.FontFamilyProperty, nameof(navigationBar.FontFamily));
				setBinding(_commandBar, navigationBar, CommandBar.FontSizeProperty, nameof(navigationBar.FontSize));
				setBinding(_commandBar, navigationBar, CommandBar.WidthProperty, nameof(navigationBar.Width));
				setBinding(_commandBar, navigationBar, CommandBar.UseSystemFocusVisualsProperty, nameof(navigationBar.UseSystemFocusVisuals));
				setBinding(_commandBar, navigationBar, CommandBarExtensions.MainCommandProperty, nameof(navigationBar.MainCommand));

				var MainCommand = CommandBarExtensions.GetMainCommand(_commandBar);
				if (MainCommand != null)
				{
					setBinding(MainCommand, navigationBar, AppBarButton.StyleProperty, nameof(navigationBar.MainCommandStyle));
				}
			}
		}


		private void OnCommandBarMainCommandClicked(object sender, RoutedEventArgs e)
		{
			GetNavBar()?.TryPerformMainCommand();
		}

		private void OnCommandBarMainCommandChanged(DependencyObject sender, DependencyProperty dp)
		{
			// When the MainCommand changes on the CommandBar, apply the MainCommandStyle from the NavigationBar
			var navigationBar = GetNavBar();
			var mainCommand = CommandBarExtensions.GetMainCommand(_commandBar);
			
			if (navigationBar != null && mainCommand != null)
			{
				if (navigationBar.MainCommandStyle is Style mainCommandStyle)
				{
					mainCommand.Style = mainCommandStyle;
				}
				
				// Re-register the click handler for the new MainCommand
				mainCommand.Click += OnCommandBarMainCommandClicked;
				_MainCommandClickedHandler.Disposable = Disposable.Create(() => mainCommand.Click -= OnCommandBarMainCommandClicked);
			}
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

			// Register for MainCommand property changes on the CommandBar
			if (_commandBar != null)
			{
				var token = _commandBar.RegisterPropertyChangedCallback(CommandBarExtensions.MainCommandProperty, OnCommandBarMainCommandChanged);
				_MainCommandPropertyChangedHandler.Disposable = Disposable.Create(() => _commandBar.UnregisterPropertyChangedCallback(CommandBarExtensions.MainCommandProperty, token));
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
			
			_MainCommandPropertyChangedHandler.Disposable = null;
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
			if (_commandBar == null) return;
			var change = args.CollectionChange;
			var changeIndex = args.Index;
			if (_commandBar.GetValue(prop) is not IObservableVector<ICommandBarElement> commands) return;
			if (change == CollectionChange.Reset)
			{
				commands.Clear();
			}
			else if (change == CollectionChange.ItemInserted)
			{
				var element = sender[(int)changeIndex];
				if (element != null)
				{
					commands.Insert((int)changeIndex, element);
				}
			}
			else if (change == CollectionChange.ItemChanged)
			{
				if (changeIndex < commands.Count)
				{
					var element = sender[(int)changeIndex];
					if (element != null)
					{
						commands[(int)changeIndex] = element;
					}
				}
			}
			else if (change == CollectionChange.ItemRemoved)
			{
				if (changeIndex < commands.Count)
				{
					commands.RemoveAt((int)changeIndex);
				}
			}
		}
	}
}
