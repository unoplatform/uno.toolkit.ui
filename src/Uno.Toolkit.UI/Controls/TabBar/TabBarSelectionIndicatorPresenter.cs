using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Disposables;
using Windows.Foundation;
using System.Reflection;

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
	/// Presenter that displays the selection indicator and translates it based on selection from the <see cref="TabBar"/>
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

		private readonly SerialDisposable _tabBarSelectionChangedRevoker = new SerialDisposable();
		private readonly SerialDisposable _tabBarItemSizeChangedRevoker = new SerialDisposable();
		private readonly SerialDisposable _indicatorSizeChangedRevoker = new SerialDisposable();
		private readonly SerialDisposable _offsetChangedRevoker = new SerialDisposable();
		private readonly SerialDisposable _tabBarOrientationChangedRevoker = new SerialDisposable();
		private readonly SerialDisposable _tabBarSizeChangedRevoker = new SerialDisposable();
		private readonly SerialDisposable _tabBarItemsChangedRevoker = new SerialDisposable();

		/// <summary>
		/// Returns the view within the <see cref="TabBarSelectionIndicatorPresenter"/> being used as the selection indicator
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
			UpdateOwner();
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			ResetOwner();
			ResetIndicator();
		}

		protected override void OnApplyTemplate()
		{
			ResetIndicator();

			base.OnApplyTemplate();

			_root = GetTemplateChild(TemplateParts.RootName) as Grid;
			_contentPresenter = GetTemplateChild(TemplateParts.ContentPresenterName) as ContentPresenter;
			_verticalStoryboard = GetTemplateChild(TemplateParts.VerticalStoryboardName) as Storyboard;
			_horizontalStoryboard = GetTemplateChild(TemplateParts.HorizontalStoryboardName) as Storyboard;

			if (_contentPresenter is { } selectionIndicator)
			{
				selectionIndicator.SizeChanged += OnSizeChanged;
				_indicatorSizeChangedRevoker.Disposable = Disposable.Create(() => selectionIndicator.SizeChanged -= OnSizeChanged);
			}

			SizeChanged += OnSizeChanged;

			_isTemplateApplied = true;

			UpdateOwner();
			UpdateIndicator();
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
				UpdateOwner();
			}
		}

		private void ResetIndicator()
		{
			StopStoryboards();
			_indicatorSizeChangedRevoker.Disposable = null;
			_contentPresenter = null;
		}

		private void ResetOwner()
		{
			_tabBarSelectionChangedRevoker.Disposable = null;
			_offsetChangedRevoker.Disposable = null;
			_tabBarItemSizeChangedRevoker.Disposable = null;
			_tabBarSizeChangedRevoker.Disposable = null;
			_tabBarItemsChangedRevoker.Disposable = null;
		}

		private void UpdateOwner()
		{
			ResetOwner();

			if (Owner is TabBar tabBar)
			{
				var selectionOrientationSubscription = tabBar.RegisterPropertyChangedCallback(TabBar.OrientationProperty, OnTabBarOrientationChanged);
				_tabBarOrientationChangedRevoker.Disposable = Disposable.Create(() => tabBar.UnregisterPropertyChangedCallback(TabBar.OrientationProperty, selectionOrientationSubscription));

				var selectionOffsetSubscription = tabBar.RegisterPropertyChangedCallback(SelectorExtensions.SelectionOffsetProperty, OnSelectionOffsetChanged);
				_offsetChangedRevoker.Disposable = Disposable.Create(() => tabBar.UnregisterPropertyChangedCallback(SelectorExtensions.SelectionOffsetProperty, selectionOffsetSubscription));

				tabBar.SelectionChanged += OnTabBarSelectionChanged;
				_tabBarSelectionChangedRevoker.Disposable = Disposable.Create(() => tabBar.SelectionChanged -= OnTabBarSelectionChanged);

				tabBar.SizeChanged += OnTabBarSizeChanged;
				_tabBarSizeChangedRevoker.Disposable = Disposable.Create(() => tabBar.SizeChanged -= OnTabBarSizeChanged);

				tabBar.Items.VectorChanged += OnTabBarItemsChanged;
				_tabBarItemsChangedRevoker.Disposable = Disposable.Create(() => tabBar.Items.VectorChanged -= OnTabBarItemsChanged);

				UpdateIndicator();
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
			if (!_isTemplateApplied)
			{
				return;
			}

			UpdateSelectionIndicatorMaxSize();
			UpdateSelectionIndicatorPosition();
		}

		private void UpdateSelectionIndicatorMaxSize()
		{
			if (Owner is { } tabBar
				&& GetSelectionIndicator() is { } indicator)
			{
				var numItems = tabBar.Items.Count;
				if (numItems < 1)
				{
					return;
				}

				var maxSize = new Size();

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
				&& _isSelectorPresent)
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
			_tabBarItemSizeChangedRevoker.Disposable = null;

			if (args.NewItem is TabBarItem tabBarItem)
			{
				tabBarItem.SizeChanged += OnSelectedTabBarItemSizeChanged;
				_tabBarItemSizeChangedRevoker.Disposable = Disposable.Create(() => tabBarItem.SizeChanged -= OnSelectedTabBarItemSizeChanged);

				//If a selection is being made for the first time, start showing the indicator
				if (!_isSelectorPresent
					|| args.OldItem == null)
				{
					Visibility = (Content ?? ContentTemplate) is { } ? Visibility.Visible : Visibility.Collapsed;
					UpdateSelectionIndicatorPosition(tabBarItem);
				}
			}
		}

		private Storyboard? GetActiveStoryboard()
		{
			return Owner?.Orientation switch
			{
				Orientation.Horizontal => _horizontalStoryboard,
				Orientation.Vertical => _verticalStoryboard,
				_ => null
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

			if (destination is null 
				|| Owner is not { } tabBar
				|| GetSelectionIndicator() is not { } indicator
				|| (indicator.ActualHeight == 0d || indicator.ActualWidth == 0d)
				|| GetActiveStoryboard() is not { } storyboard
				|| TemplateSettings is not { } templateSettings)
			{
				return;
			}

			StopStoryboards();

			var point = new Point(0, 0);
			var nextPosPoint = destination.TransformToVisual(tabBar).TransformPoint(point);

			templateSettings.IndicatorTransitionFrom = templateSettings.IndicatorTransitionTo;
			templateSettings.IndicatorTransitionTo = nextPosPoint;

			storyboard.BeginTime = TimeSpan.FromMilliseconds(0);

			storyboard.Begin();
			if (IndicatorTransitionMode == IndicatorTransitionMode.Snap)
			{
				storyboard.SkipToFill();
			}
		}
	}
}
