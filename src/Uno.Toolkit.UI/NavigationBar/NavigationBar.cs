#if __IOS__ || __ANDROID__
#define HAS_NATIVE_NAVBAR
#endif
#if __IOS__
using UIKit;
#endif
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using Uno.Disposables;
using Windows.Foundation;
using Windows.UI.Core;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif

namespace Uno.UI.ToolkitLib
{
	[ContentProperty(Name = nameof(PrimaryCommands))]
	[TemplatePart(Name = NavigationBarPresenter, Type = typeof(FrameworkElement))]
	public partial class NavigationBar : ContentControl
	{
		public event EventHandler<object>? Closed;
		public event EventHandler<object>? Opened;
		public event EventHandler<object>? Closing;
		public event EventHandler<object>? Opening;
		public event TypedEventHandler<NavigationBar, Microsoft.UI.Xaml.Controls.DynamicOverflowItemsChangingEventArgs?>? DynamicOverflowItemsChanging;

		private const string MoreButton = "MoreButton";
		private const string PrimaryItemsControl = "PrimaryItemsControl";
		private const string SecondaryItemsControl = "SecondaryItemsControl";
		private const string NavigationBarPresenter = "NavigationBarPresenter";


#if HAS_NATIVE_NAVBAR
		private bool _isNativeTemplate;
#endif

		private INavigationBarPresenter? _presenter;
		private SerialDisposable _backRequestedHandler = new SerialDisposable();
		private SerialDisposable _frameBackStackChangedHandler = new SerialDisposable();

		public NavigationBar()
		{
			LeftCommand ??= new Microsoft.UI.Xaml.Controls.AppBarButton() { Visibility = Visibility.Collapsed, Icon = new SymbolIcon(Symbol.Back) };
			PrimaryCommands ??= new NavigationBarElementCollection();
			SecondaryCommands ??= new NavigationBarElementCollection();

			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			DefaultStyleKey = typeof(NavigationBar);
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			GetTemplatePart(NavigationBarPresenter, out _presenter);

			_presenter?.SetOwner(this);

#if HAS_NATIVE_NAVBAR
			_isNativeTemplate = _presenter is NativeNavigationBarPresenter;
#endif
		}

		protected override Size MeasureOverride(Size availableSize)
		{
#if HAS_NATIVE_NAVBAR
			if (_isNativeTemplate)
			{
				var size = base.MeasureOverride(availableSize);
				return size;
			}
#endif
			return base.MeasureOverride(availableSize);
		}

		internal bool PerformBack()
		{
			var page = this.FindFirstParent<Page>();

			if (page?.Frame is { Visibility: Visibility.Visible } frame
				&& frame.CurrentSourcePageType == page.GetType()
				&& frame.CanGoBack)
			{
				frame.GoBack();
				return true;
			}

			return false;
		}

		#region Event Raising
		internal void RaiseClosingEvent(object e) 
			=> Closing?.Invoke(this, e);

		internal void RaiseClosedEvent(object e) 
			=> Closed?.Invoke(this, e);
		
		internal void RaiseOpeningEvent(object e)
			=> Opening?.Invoke(this, e);
		
		internal void RaiseOpenedEvent(object e)
			=> Opened?.Invoke(this, e);

		internal void RaiseDynamicOverflowItemsChanging(Microsoft.UI.Xaml.Controls.DynamicOverflowItemsChangingEventArgs args)
			=> DynamicOverflowItemsChanging?.Invoke(this, args);
		#endregion

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			_backRequestedHandler.Disposable = null;
			_frameBackStackChangedHandler.Disposable = null;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
			_backRequestedHandler.Disposable = Disposable.Create(() => SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested);

			var frame = this.FindFirstParent<Frame>();
			if (frame?.BackStack is ObservableCollection<PageStackEntry> backStack)
			{
				backStack.CollectionChanged += OnBackStackChanged;
				_frameBackStackChangedHandler.Disposable = Disposable.Create(() => backStack.CollectionChanged -= OnBackStackChanged);
			}
		}

		private void OnBackStackChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdateLeftCommandVisibility();
		}

		private void UpdateLeftCommandVisibility()
		{
			var frame = this.FindFirstParent<Frame>();
			var buttonVisibility = (frame?.CanGoBack ?? false)
				? Visibility.Visible
				: Visibility.Collapsed;

			LeftCommand.Visibility = buttonVisibility;
		}

		private void OnBackRequested(object sender, BackRequestedEventArgs e)
		{
			e.Handled = PerformBack();
		}

		private void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
		{
			if (args.Property == LeftCommandProperty)
			{
				UpdateLeftCommandVisibility();
			}
		}

		private void GetTemplatePart<T>(string name, out T? element) where T : class
		{
			element = GetTemplateChild(name) as T;
		}
	}
}
