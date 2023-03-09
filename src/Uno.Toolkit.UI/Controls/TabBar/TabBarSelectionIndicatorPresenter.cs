using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Disposables;
using Windows.Foundation;
using System.Reflection;
using Uno.Extensions;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media.Animation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Displays the the selection indicator of a <see cref="TabBar"/> and animates it with any change in the selection.
	/// </summary>
	[TemplatePart(Name = TemplateParts.ContentPresenterName, Type = typeof(ContentPresenter))]
	[TemplatePart(Name = TemplateParts.RootName, Type = typeof(Grid))]
	public partial class TabBarSelectionIndicatorPresenter : ContentControl
	{
		public static class TemplateParts
		{
			public const string ContentPresenterName = "IndicatorPresenter";
			public const string RootName = "Root";
			public const string VerticalStoryboardName = "IndicatorTransitionVerticalStoryboard";
			public const string HorizontalStoryboardName = "IndicatorTransitionHorizontalStoryboard";
		}

		private ContentPresenter? _contentPresenter;
		private Grid? _root;
		private Storyboard? _verticalStoryboard;
		private Storyboard? _horizontalStoryboard;
		private bool _isTemplateApplied;
		private bool _isLoaded;

		private readonly SerialDisposable _indicatorSubscriptions = new SerialDisposable();
		private readonly SerialDisposable _ownerSubscriptions = new SerialDisposable();
		private readonly SerialDisposable _tabBarItemSizeChangedRevoker = new SerialDisposable();

		/// <summary>
		/// Returns the view within the presenter being used as the selection indicator
		/// </summary>
		/// <returns></returns>
		public FrameworkElement? GetSelectionIndicator() => _contentPresenter as FrameworkElement;

		private bool _isSelectorPresent => TabBarSelectorBehavior.GetSelector(Owner) != null;

		public TabBarSelectionIndicatorPresenter()
		{
			DefaultStyleKey = typeof(TabBarSelectionIndicatorPresenter);
			TemplateSettings = new TabBarSelectionIndicatorPresenterTemplateSettings();
			Unloaded += OnUnloaded;
			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			_isLoaded = true;

			if (IsReady)
			{
				SetupOwner();
				SetupIndicator();

				UpdateIndicator();
			}
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			_isLoaded = false;

			ResetOwner();
			ResetIndicator();
			_tabBarItemSizeChangedRevoker.Disposable = null;
		}

		protected override void OnApplyTemplate()
		{
			ResetIndicator();

			base.OnApplyTemplate();

			_root = GetTemplateChild(TemplateParts.RootName) as Grid;
			_contentPresenter = GetTemplateChild(TemplateParts.ContentPresenterName) as ContentPresenter;
			_verticalStoryboard = GetTemplateChild(TemplateParts.VerticalStoryboardName) as Storyboard;
			_horizontalStoryboard = GetTemplateChild(TemplateParts.HorizontalStoryboardName) as Storyboard;

			SizeChanged += OnSizeChanged;

			_isTemplateApplied = true;

			if (IsReady && _ownerSubscriptions.Disposable == null)
			{
				SetupOwner();
				SetupIndicator();

				UpdateIndicator();
			}
		}

		private void SetupIndicator()
		{
			if (!IsReady || _indicatorSubscriptions.Disposable != null)
			{
				return;
			}

			if (_contentPresenter is { } selectionIndicator)
			{
				selectionIndicator.SizeChanged += OnSizeChanged;
				_indicatorSubscriptions.Disposable = Disposable.Create(() => selectionIndicator.SizeChanged -= OnSizeChanged);
			}
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs args)
		{
			UpdateIndicator();
		}

		private void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
		{
			var property = args.Property;
			if (property == OwnerProperty)
			{
				SetupOwner(forceRewire: true);
				UpdateIndicator();
			}
		}

		private void ResetIndicator()
		{
			StopStoryboards();
			_indicatorSubscriptions.Disposable = null;
		}

		private void ResetOwner()
		{
			_ownerSubscriptions.Disposable = null;
		}

		private void SetupOwner(bool forceRewire = false)
		{
			if (!IsReady || (_ownerSubscriptions.Disposable != null && !forceRewire))
			{
				return;
			}

			var disposables = new CompositeDisposable();
			_ownerSubscriptions.Disposable = disposables;

			if (Owner is TabBar tabBar)
			{
				tabBar.RegisterDisposablePropertyChangedCallback(TabBar.OrientationProperty, OnTabBarOrientationChanged)
					.DisposeWith(disposables);

				tabBar.RegisterDisposablePropertyChangedCallback(SelectorExtensions.SelectionOffsetProperty, OnSelectionOffsetChanged)
					.DisposeWith(disposables);

				tabBar.SelectionChanged += OnTabBarSelectionChanged;
				Disposable
					.Create(() => tabBar.SelectionChanged -= OnTabBarSelectionChanged)
					.DisposeWith(disposables);

				tabBar.SizeChanged += OnTabBarSizeChanged;
				Disposable
					.Create(() => tabBar.SizeChanged -= OnTabBarSizeChanged)
					.DisposeWith(disposables);

				tabBar.Items.VectorChanged += OnTabBarItemsChanged;
				Disposable
					.Create(() => tabBar.Items.VectorChanged -= OnTabBarItemsChanged)
					.DisposeWith(disposables);
			}
		}

		private void OnTabBarItemsChanged(Windows.Foundation.Collections.IObservableVector<object> sender, Windows.Foundation.Collections.IVectorChangedEventArgs @event)
		{
			UpdateIndicator();
		}

		private void OnTabBarSizeChanged(object sender, SizeChangedEventArgs args)
		{
			UpdateIndicator();
		}

		private void UpdateIndicator()
		{
			if (!IsReady)
			{
				return;
			}

			SynchronizeSelection();
			UpdateSelectionIndicatorMaxSize();
			UpdateSelectionIndicatorPosition();
		}

		private void UpdateSelectionIndicatorMaxSize()
		{
			if (Owner is { } tabBar
				&& GetSelectionIndicator() is { } indicator)
			{
				var tabBarItems = tabBar.GetItemContainers<TabBarItem>().Where(tbi => tbi.Visibility == Visibility.Visible);
				if (tabBarItems.None())
				{
					return;
				}

				var maxSize = new Size();
				var numItems = tabBarItems.Count();

				if (tabBar.Orientation == Orientation.Vertical)
				{
					maxSize.Height = tabBar.ActualHeight / numItems;
					maxSize.Width = tabBar.ActualWidth;
				}
				else
				{
					maxSize.Width = tabBar.ActualWidth / numItems;
					maxSize.Height = tabBar.ActualHeight;
				}

				TemplateSettings.IndicatorMaxSize = maxSize;
			}
		}

		private void OnTabBarOrientationChanged(DependencyObject sender, DependencyProperty dp)
		{
			if (Owner is { } tabBar)
			{
				VisualStateManager.GoToState(this, tabBar.Orientation == Orientation.Vertical ? "Vertical" : "Horizontal", false);
			}
		}

		private void OnSelectionOffsetChanged(DependencyObject sender, DependencyProperty dp)
		{
			if (sender is TabBar tabBar
				&& GetSelectionIndicator() is { } selectionIndicator
				&& _isSelectorPresent
				&& IndicatorTransitionMode == IndicatorTransitionMode.Slide)
			{
				selectionIndicator.RenderTransform = new TranslateTransform
				{
					X = SelectorExtensions.GetSelectionOffset(tabBar)
				};
			}
		}

		private void OnSelectedTabBarItemSizeChanged(object sender, object e)
		{
			UpdateIndicator();
		}

		private void OnTabBarSelectionChanged(TabBar sender, TabBarSelectionChangedEventArgs args)
		{
			UpdateIndicator();
		}

		private void SynchronizeSelection()
		{
			_tabBarItemSizeChangedRevoker.Disposable = null;

			if (Owner is not { } tabBar ||
				tabBar.ContainerFromIndex(tabBar.SelectedIndex) is not TabBarItem newSelectedItem)
			{
				Opacity = 0f;
				return;
			}
		
			newSelectedItem.SizeChanged += OnSelectedTabBarItemSizeChanged;
			_tabBarItemSizeChangedRevoker.Disposable = Disposable.Create(() => newSelectedItem.SizeChanged -= OnSelectedTabBarItemSizeChanged);

			Opacity = (Content ?? ContentTemplate) is { } ? 1f : 0f;
		}

		private Storyboard? GetStoryboardForCurrentOrientation()
		{
			return Owner?.Orientation switch
			{
				Orientation.Horizontal => _horizontalStoryboard,
				Orientation.Vertical => _verticalStoryboard,
				_ => throw new ArgumentOutOfRangeException(nameof(Owner.Orientation))
			};
		}

		private void StopStoryboards()
		{
			_horizontalStoryboard?.Stop();
			_verticalStoryboard?.Stop();
		}

		private void UpdateSelectionIndicatorPosition(TabBarItem? destination = null)
		{
			destination ??= Owner?.SelectedItem as TabBarItem;

			if (destination is null ||
				Owner is not { } tabBar ||
				GetSelectionIndicator() is not { } indicator ||
				(indicator.ActualHeight == 0d || indicator.ActualWidth == 0d) ||
				GetStoryboardForCurrentOrientation() is not { } storyboard ||
				TemplateSettings is not { } templateSettings)
			{
				return;
			}

			StopStoryboards();

			var nextPosPoint = destination.TransformToVisual(tabBar).TransformPoint(default);

			templateSettings.IndicatorTransitionFrom = templateSettings.IndicatorTransitionTo;
			templateSettings.IndicatorTransitionTo = nextPosPoint;

			storyboard.BeginTime = TimeSpan.FromMilliseconds(0);

			storyboard.Begin();
			if (IndicatorTransitionMode == IndicatorTransitionMode.Snap)
			{
				storyboard.SkipToFill();
			}
		}

		private bool IsReady => _isLoaded && _isTemplateApplied;
	}
}
