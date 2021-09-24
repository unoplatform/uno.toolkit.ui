using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Disposables;
using Uno.UI.ToolkitLib.Extensions;
using Windows.Foundation.Collections;
using CommandBar = Microsoft.UI.Xaml.Controls.CommandBar;
using AppBarButton = Microsoft.UI.Xaml.Controls.AppBarButton;
using ICommandBarElement = Microsoft.UI.Xaml.Controls.ICommandBarElement;
using Uno.UI.DataBinding;

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

namespace Uno.UI.ToolkitLib
{
	public partial class NavigationBarPresenter : Control, INavigationBarPresenter
	{
		private const string XamlNavigationBarCommandBar = "XamlNavigationBarCommandBar";

		private CommandBar? _commandBar;
		private WeakReference<NavigationBar?>? _weakNavBar;
		private SerialDisposable _navBarCommandsChangedHandler = new SerialDisposable();
		private SerialDisposable _leftCommandClickedHandler = new SerialDisposable();

		public NavigationBarPresenter()
		{
			DefaultStyleKey = typeof(NavigationBarPresenter);
		}

		public void SetOwner(NavigationBar? owner)
		{
			InitializeCommandBar(owner);
		}

		private void InitializeCommandBar(NavigationBar? navigationBar)
		{
			if (_weakNavBar?.Target == navigationBar)
			{
				return;
			}

			UnregisterEvents();

			_commandBar = GetTemplateChild(XamlNavigationBarCommandBar) as CommandBar;
			_weakNavBar = new WeakReference<NavigationBar?>(navigationBar);
			
			SetBindings();

			RegisterEvents();
		}

		private void SetBindings()
		{
			if (_commandBar is { }
				&& _weakNavBar?.Target is NavigationBar navigationBar)
			{
				void setBinding(UIElement target, object source, DependencyProperty property, string path, BindingMode mode = BindingMode.TwoWay)
					=> target?.SetBinding(
						property,
						new Binding
						{
							Path = new PropertyPath(path),
							Source = source,
							Mode = mode
						});

				foreach (var command in navigationBar.PrimaryCommands)
				{
					_commandBar.PrimaryCommands.Add(command);
				}

				foreach (var command in navigationBar.SecondaryCommands)
				{
					_commandBar.SecondaryCommands.Add(command);
				}

				setBinding(_commandBar, navigationBar, CommandBar.ContentProperty, nameof(navigationBar.Content));
				setBinding(_commandBar, navigationBar, CommandBar.IsStickyProperty, nameof(navigationBar.IsSticky));
				setBinding(_commandBar, navigationBar, CommandBar.IsOpenProperty, nameof(navigationBar.IsOpen));
				setBinding(_commandBar, navigationBar, CommandBar.LightDismissOverlayModeProperty, nameof(navigationBar.LightDismissOverlayMode));
				setBinding(_commandBar, navigationBar, CommandBar.IsDynamicOverflowEnabledProperty, nameof(navigationBar.IsDynamicOverflowEnabled));
				setBinding(_commandBar, navigationBar, CommandBar.DefaultLabelPositionProperty, nameof(navigationBar.DefaultLabelPosition));
				setBinding(_commandBar, navigationBar, CommandBar.OverflowButtonVisibilityProperty, nameof(navigationBar.OverflowButtonVisibility));
				setBinding(_commandBar, navigationBar, CommandBar.ClosedDisplayModeProperty, nameof(navigationBar.ClosedDisplayMode));
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
				setBinding(_commandBar, navigationBar, CommandBarExtensions.LeftCommandProperty, nameof(navigationBar.LeftCommand));

				var leftCommand = CommandBarExtensions.GetLeftCommand(_commandBar);
				if (leftCommand != null)
				{
					setBinding(leftCommand, navigationBar, AppBarButton.StyleProperty, nameof(navigationBar.LeftCommandStyle));
					VisualStateManager.GoToState(leftCommand, "LabelOnRight", false);
				}
			}
		}

		private void OnCommandBarLeftCommandClicked(object sender, RoutedEventArgs e)
		{
			if (_weakNavBar?.Target is NavigationBar navigationBar)
			{
				navigationBar.PerformBack();
			}
		}

		private void RegisterEvents()
		{
			if (_weakNavBar?.Target is NavigationBar navigationBar)
			{
				UnregisterEvents();

				var disposables = new CompositeDisposable(2);
				navigationBar.PrimaryCommands.VectorChanged += OnNavBarPrimaryCommandsChanged;
				navigationBar.SecondaryCommands.VectorChanged += OnNavBarSecondaryCommandsChanged;

				disposables.Add(() => navigationBar.PrimaryCommands.VectorChanged -= OnNavBarPrimaryCommandsChanged);
				disposables.Add(() => navigationBar.SecondaryCommands.VectorChanged -= OnNavBarSecondaryCommandsChanged);

				_navBarCommandsChangedHandler.Disposable = disposables;
			}

			
			var commandBarLeftCommand = CommandBarExtensions.GetLeftCommand(_commandBar);
			if (commandBarLeftCommand != null)
			{
				commandBarLeftCommand.Click += OnCommandBarLeftCommandClicked;
				_leftCommandClickedHandler.Disposable = Disposable.Create(() => commandBarLeftCommand.Click -= OnCommandBarLeftCommandClicked);
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

		private void OnCommandBarDynamicOverflowItemsChanging(CommandBar sender, Microsoft.UI.Xaml.Controls.DynamicOverflowItemsChangingEventArgs args)
		{
			_weakNavBar?.Target?.RaiseDynamicOverflowItemsChanging(args);
		}

		private void OnCommandBarClosing(object sender, object e)
		{
			_weakNavBar?.Target?.RaiseClosingEvent(sender, e);
		}

		private void OnCommandBarClosed(object sender, object e)
		{
			_weakNavBar?.Target?.RaiseClosedEvent(sender, e);
		}

		private void OnCommandBarOpening(object sender, object e)
		{
			_weakNavBar?.Target?.RaiseOpeningEvent(sender, e);
		}

		private void OnCommandBarOpened(object sender, object e)
		{
			_weakNavBar?.Target?.RaiseOpenedEvent(sender, e);
		}

		private void UnregisterEvents()
		{
			if (_weakNavBar?.Target is { })
			{
				_navBarCommandsChangedHandler.Disposable = null;
			}

			if (CommandBarExtensions.GetLeftCommand(_commandBar) != null)
			{
				_leftCommandClickedHandler.Disposable = null;
			}
		}

		private void OnNavBarPrimaryCommandsChanged(IObservableVector<ICommandBarElement> sender, IVectorChangedEventArgs args)
			=> OnCommandsChanged(sender, args, CommandBar.PrimaryCommandsProperty);

		private void OnNavBarSecondaryCommandsChanged(IObservableVector<ICommandBarElement> sender, IVectorChangedEventArgs args) 
			=> OnCommandsChanged(sender, args, CommandBar.SecondaryCommandsProperty);

		private void OnCommandsChanged(IObservableVector<ICommandBarElement> sender, IVectorChangedEventArgs args, DependencyProperty prop)
		{
			if (_commandBar is { })
			{
				var change = args.CollectionChange;
				var changeIndex = args.Index;

				if (_commandBar.GetValue(prop) is IObservableVector<ICommandBarElement> commands)
				{
					if (change == CollectionChange.Reset)
					{
						commands.Clear();
					}
					else if (change == CollectionChange.ItemInserted ||
						change == CollectionChange.ItemChanged)
					{
						var element = sender[(int)changeIndex];
						if (element is { })
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
