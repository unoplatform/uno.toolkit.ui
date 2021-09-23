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
	public partial class NavigationBarPresenter : Control
	{
		private CommandBar? _commandBar;
		private WeakReference<NavigationBar?>? _weakNavBar;
		private SerialDisposable _navBarCommandsChangedRevoker = new SerialDisposable();

		public NavigationBarPresenter()
		{
			DefaultStyleKey = typeof(NavigationBarPresenter);
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

			UnregisterCommandsChanged();

			_weakNavBar = new WeakReference<NavigationBar?>(navigationBar);
			
			SetBindings();

			RegisterCommandsChanged();
		}

		private void SetBindings()
		{
			if (_commandBar is { }
				&& _weakNavBar?.Target is NavigationBar navigationBar)
			{
				void setBinding(DependencyProperty property, string path)
					=> _commandBar?.SetBinding(
						property,
						new Binding
						{
							Path = new PropertyPath(path),
							Source = this,
							Mode = BindingMode.TwoWay,
							RelativeSource = RelativeSource.TemplatedParent
						});

				setBinding(CommandBar.PrimaryCommandsProperty, nameof(navigationBar.PrimaryCommands));
				setBinding(CommandBar.SecondaryCommandsProperty, nameof(navigationBar.SecondaryCommands));
				setBinding(CommandBar.IsStickyProperty, nameof(navigationBar.IsSticky));
				setBinding(CommandBar.IsOpenProperty, nameof(navigationBar.IsOpen));
				setBinding(CommandBar.LightDismissOverlayModeProperty, nameof(navigationBar.LightDismissOverlayMode));
				setBinding(CommandBar.IsDynamicOverflowEnabledProperty, nameof(navigationBar.IsDynamicOverflowEnabled));
				setBinding(CommandBar.DefaultLabelPositionProperty, nameof(navigationBar.DefaultLabelPosition));
				setBinding(CommandBar.OverflowButtonVisibilityProperty, nameof(navigationBar.OverflowButtonVisibility));
				setBinding(CommandBar.ClosedDisplayModeProperty, nameof(navigationBar.ClosedDisplayMode));
				setBinding(CommandBar.ForegroundProperty, nameof(navigationBar.Foreground));
				setBinding(CommandBar.BackgroundProperty, nameof(navigationBar.Background));
				setBinding(CommandBar.BorderThicknessProperty, nameof(navigationBar.BorderThickness));
				setBinding(CommandBar.PaddingProperty, nameof(navigationBar.Padding));
				setBinding(CommandBar.HorizontalAlignmentProperty, nameof(navigationBar.HorizontalAlignment));
				setBinding(CommandBar.HorizontalContentAlignmentProperty, nameof(navigationBar.HorizontalContentAlignment));
				setBinding(CommandBar.VerticalAlignmentProperty, nameof(navigationBar.VerticalAlignment));
				setBinding(CommandBar.VerticalContentAlignmentProperty, nameof(navigationBar.VerticalContentAlignment));
				setBinding(CommandBar.FontFamilyProperty, nameof(navigationBar.FontFamily));
				setBinding(CommandBar.FontSizeProperty, nameof(navigationBar.FontSize));
				setBinding(CommandBar.WidthProperty, nameof(navigationBar.Width));
				setBinding(CommandBar.UseSystemFocusVisualsProperty, nameof(navigationBar.UseSystemFocusVisuals));
			}
		}

		private void RegisterCommandsChanged()
		{
			
			if (_weakNavBar?.Target is NavigationBar navigationBar)
			{
				UnregisterCommandsChanged();

				var disposables = new CompositeDisposable(2);
				navigationBar.PrimaryCommands.VectorChanged += OnNavBarPrimaryCommandsChanged;
				navigationBar.SecondaryCommands.VectorChanged += OnNavBarSecondaryCommandsChanged;

				disposables.Add(() => navigationBar.PrimaryCommands.VectorChanged -= OnNavBarPrimaryCommandsChanged);
				disposables.Add(() => navigationBar.SecondaryCommands.VectorChanged -= OnNavBarSecondaryCommandsChanged);

				_navBarCommandsChangedRevoker.Disposable = disposables;
			}
		}
		private void UnregisterCommandsChanged()
		{
			if (_weakNavBar?.Target is { })
			{
				_navBarCommandsChangedRevoker.Disposable = null;
			}
		}

		private void OnNavBarPrimaryCommandsChanged(IObservableVector<AppBarButton> sender, IVectorChangedEventArgs @event)
		{
			if (_commandBar is { })
			{
				_commandBar.PrimaryCommands.Clear();
				foreach (var appBarButton in sender)
				{
					_commandBar.PrimaryCommands.Append(appBarButton);
				}
			}
		}

		private void OnNavBarSecondaryCommandsChanged(IObservableVector<AppBarButton> sender, IVectorChangedEventArgs @event)
		{
			if (_commandBar is { })
			{
				_commandBar.SecondaryCommands.Clear();
				foreach (var appBarButton in sender)
				{
					_commandBar.SecondaryCommands.Append(appBarButton);
				}
			}
		}
	}
}
