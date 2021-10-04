using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Disposables;
using Windows.Foundation;
using Uno.UI.ToolkitLib.Extensions;
using Uno.UI.ToolkitLib.Behaviors;



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

namespace Uno.UI.ToolkitLib
{
	/// <summary>
	/// A Panel that is used to contain and translate a view being used as a selection indicator for a <see cref="TabBar"/>
	/// </summary>
	public partial class TabBarSelectionIndicatorPresenter : Canvas
	{
		private readonly SerialDisposable _tabBarSelectionChangedRevoker = new SerialDisposable();
		private readonly SerialDisposable _tabBarItemSizeChangedRevoker = new SerialDisposable();
		private readonly SerialDisposable _offsetChangedRevoker = new SerialDisposable();
		private readonly Storyboard _indicatorSlideStoryboard = new Storyboard();

		/// <summary>
		/// Return the view within the Panel being used as the selection indicator
		/// </summary>
		/// <returns></returns>
		public UIElement? GetSelectionIndicator()
			=> Children?.FirstOrDefault();

		private bool _isSelectorPresent => TabBarSelectorBehavior.GetSelector(Owner) != null;

		public TabBarSelectionIndicatorPresenter()
		{
			SizeChanged += OnSizeChanged;
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs args)
		{
			var selectedItem = Owner?.SelectedItem as TabBarItem;
			MoveSelectionIndicator(selectedItem);
		}

		private void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
		{
			var property = args.Property;
			if (property == OwnerProperty)
			{
				UpdateOwner();
			}
		}

		private void UpdateOwner()
		{
			_tabBarSelectionChangedRevoker.Disposable = null;
			_offsetChangedRevoker.Disposable = null;

			if (Owner is TabBar tabBar)
			{
				var selectionOffsetSubscription = tabBar.RegisterPropertyChangedCallback(SelectorExtensions.SelectionOffsetProperty, OnSelectionOffsetChanged);
				_offsetChangedRevoker.Disposable = Disposable.Create(() => tabBar.UnregisterPropertyChangedCallback(SelectorExtensions.SelectionOffsetProperty, selectionOffsetSubscription));

				tabBar.SelectionChanged += OnTabBarSelectionChanged;
				_tabBarSelectionChangedRevoker.Disposable = Disposable.Create(() => tabBar.SelectionChanged -= OnTabBarSelectionChanged);

				var selectedItem = Owner?.SelectedItem as TabBarItem;
				MoveSelectionIndicator(selectedItem);
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
			var selectedItem = Owner?.SelectedItem as TabBarItem;
			MoveSelectionIndicator(selectedItem);
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
					Opacity = 1f;
					MoveSelectionIndicator(tabBarItem);
				}
			}
		}

		private void MoveSelectionIndicator(TabBarItem? selectedItem)
		{
			UIElement? child;

			if (selectedItem == null
				|| Owner == null
				|| (child = GetSelectionIndicator()) is null)
			{
				return;
			}

			var point = new Point(0, 0);
			double nextPos;

			var nextPosPoint = selectedItem.TransformToVisual(Owner)
				.TransformPoint(point);

			var currentPoint = child.TransformToVisual(Owner)
				.TransformPoint(new Point(0, 0));

			nextPos = nextPosPoint.X + (selectedItem.ActualWidth / 2);

			if (IndicatorTransitionMode == IndicatorTransitionMode.Snap)
			{
				child.RenderTransform = new TranslateTransform() { X = nextPos - (child.ActualSize.X / 2) };
			}
			else if (IndicatorTransitionMode == IndicatorTransitionMode.Slide)
			{
				_indicatorSlideStoryboard.Stop();
				_indicatorSlideStoryboard.Children.Clear();

				var easing = new CubicEase();

				var transform = child.RenderTransform as CompositeTransform;
				if (transform == null)
				{
					transform = new CompositeTransform();
					child.RenderTransform = transform;
				}
				child.RenderTransformOrigin = new Point(0, 0);

				var db = new DoubleAnimation
				{
					To = nextPos - (child.ActualSize.X / 2),
					From = currentPoint.X,
					EasingFunction = easing,
					Duration = TimeSpan.FromMilliseconds(400)
				};
				Storyboard.SetTarget(db, child);
				var axis = "X";	// X axis
				Storyboard.SetTargetProperty(db, $"(UIElement.RenderTransform).(CompositeTransform.Translate{axis})");

				_indicatorSlideStoryboard.BeginTime = TimeSpan.FromMilliseconds(0);

				_indicatorSlideStoryboard.Children.Add(db);
				_indicatorSlideStoryboard.Begin();
			}
		}
	}
}
