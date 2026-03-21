#if __IOS__ || __ANDROID__
#define HAS_NATIVE_NAVBAR
#endif
#if !IS_WINUI || HAS_UNO
#define SYS_NAV_MGR_SUPPORTED
#endif

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Core;
using Uno.Disposables;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Navigation;
using XamlWindow = Microsoft.UI.Xaml.Window;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using XamlWindow = Windows.UI.Xaml.Window;
#endif

#if HAS_UNO
using Uno.UI;
using Uno.UI.Helpers;
#endif

#if __IOS__
using UIKit;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Represents a specialized app bar that provides layout for AppBarButton and navigation logic.
	/// </summary>
	[ContentProperty(Name = nameof(PrimaryCommands))]
	[TemplatePart(Name = NavigationBarPresenter, Type = typeof(FrameworkElement))]
	public partial class NavigationBar : ContentControl
	{
		public event EventHandler<object>? Closed;
		public event EventHandler<object>? Opened;
		public event EventHandler<object>? Closing;
		public event EventHandler<object>? Opening;
		public event TypedEventHandler<NavigationBar, DynamicOverflowItemsChangingEventArgs?>? DynamicOverflowItemsChanging;

		private const string NavigationBarPresenter = "NavigationBarPresenter";
		private const double DefaultCollapsedHeight = 64.0;

		private Popup? _popupHost;
		private ContentControl? _skiaHeaderContentControl;

		private INavigationBarPresenter? _presenter;
		private SerialDisposable _backRequestedHandler = new();
		private SerialDisposable _frameBackStackChangedHandler = new();
		private SerialDisposable _commandBarLoadedRevoker = new();
		private SerialDisposable _scrollViewerSubscription = new();
		private ScrollViewer? _scrollViewer;
		private WeakReference<Page?>? _pageRef;

		public NavigationBar()
		{
			DefaultStyleKey = typeof(NavigationBar);

			TemplateSettings = new NavigationBarTemplateSettings(this);

			MainCommand ??= new AppBarButton();
			PrimaryCommands ??= new NavigationBarElementCollection();
			SecondaryCommands ??= new NavigationBarElementCollection();

			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			RegisterPropertyChangedCallback(ContentControl.HorizontalContentAlignmentProperty, OnHorizontalContentAlignmentChanged);
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			GetTemplatePart(NavigationBarPresenter, out _presenter);

			_presenter?.SetOwner(this);

			SubscribeToCommandBarIfNeeded();
		}

		private void SubscribeToCommandBarIfNeeded()
		{
			if (HasXamlPresenter())
			{
				_commandBarLoadedRevoker.Disposable = (_presenter as NavigationBarPresenter)?.SubscribeToNestedElements(
				[
					x => x?.GetFirstDescendant<CommandBar>(),
					// fixme@xy: impl xpath-like query to properly isolate the header content control
					// atm, this works because the header presenter currently is defined before navigation command and primary command presenters.
					// with depth-first search, this is very prune to failure without a more robust way to uniquely identify the header presenter.
					x => x?.GetFirstDescendant<ContentControl>(cc => cc.Name == "ContentControl"),
				],
				x =>
				{
					_skiaHeaderContentControl = x as ContentControl;
					UpdateHeaderContentAlignment();
				});
			}
			else
			{
				_commandBarLoadedRevoker.Disposable = null;
			}
		}

		internal bool TryPerformMainCommand()
		{
			if (MainCommandMode != MainCommandMode.Back)
			{
				return false;
			}

			if (GetPage() is { } page)
			{
				if (page.Frame is { Visibility: Visibility.Visible } frame
					&& frame.CurrentSourcePageType == page.GetType())
				{

					if (frame.CanGoBack == false && _popupHost is { })
					{
						// If we are within a Page that is hosted within a Popup and the BackStack is empty:
						// close the Popup
						_popupHost.IsOpen = false;
						return true;
					}
					else if (frame.CanGoBack)
					{
						frame.GoBack();
						return true;
					}
				}
			}
			else if (_popupHost is { })
			{
				// If we are not hosted within a Page but we are still within a Popup:
				// close the Popup
				_popupHost.IsOpen = false;
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

		internal void RaiseDynamicOverflowItemsChanging(DynamicOverflowItemsChangingEventArgs args)
			=> DynamicOverflowItemsChanging?.Invoke(this, args);
		#endregion

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			_backRequestedHandler.Disposable = null;
			_frameBackStackChangedHandler.Disposable = null;
			TeardownExpandableBehavior();
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			_pageRef = new WeakReference<Page?>(FindPage());

			_popupHost = Uno.Toolkit.UI.DependencyObjectExtensions.FindFirstParent<Popup>(this);

#if SYS_NAV_MGR_SUPPORTED
			SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
			_backRequestedHandler.Disposable = Disposable.Create(() => SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested);
#endif

#if !HAS_NATIVE_NAVBAR
			if (GetPage() is { } page)
			{
				var frame = page?.Frame;
				if (frame?.BackStack is ObservableCollection<PageStackEntry> backStack)
				{
					backStack.CollectionChanged += OnBackStackChanged;
					_frameBackStackChangedHandler.Disposable = Disposable.Create(() => backStack.CollectionChanged -= OnBackStackChanged);
				}
			}
#endif
			UpdateMainCommandVisibility();
			SetupExpandableBehavior();
		}

		private Page? FindPage()
		{
			return VisualTreeHelperEx.GetFirstAncestor<Page>(this,
				hierarchyPredicate: x => x is not Popup, // prevent looking past the popup host
				predicate: _ => true);
		}


#if !HAS_NATIVE_NAVBAR
		private void OnBackStackChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			UpdateMainCommandVisibility();
		}
#endif

#if SYS_NAV_MGR_SUPPORTED
		private void OnBackRequested(object? sender, BackRequestedEventArgs e)
		{
			if (!e.Handled)
			{
				e.Handled = TryPerformMainCommand();
			}
		}
#endif

		private void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
		{
			// central entry point for all locally defined dependency property changes

			if (args.Property == MainCommandProperty)
			{
				UpdateMainCommandVisibility();
			}
			else if (args.Property == MainCommandModeProperty)
			{
				UpdateMainCommandVisibility();
			}
			else if (args.Property == MainCommandStyleProperty)
			{
				if (MainCommand is { } mainCommand)
				{
					mainCommand.Style = args.NewValue as Style;
				}
			}
			else if (args.Property == IsExpandableProperty)
			{
				if ((bool)args.NewValue)
				{
					SetupExpandableBehavior();
				}
				else
				{
					TeardownExpandableBehavior();
				}
			}
			else if (args.Property == ExpandedHeightProperty)
			{
				UpdateExpandableState();
			}
		}

		private void OnHorizontalContentAlignmentChanged(DependencyObject sender, DependencyProperty dp)
		{
			UpdateHeaderContentAlignment();
		}

		internal void UpdateMainCommandVisibility()
		{
			if (MainCommand is null) return;

			MainCommand.Visibility = MainCommandMode switch
			{
				MainCommandMode.Back => (GetPage()?.Frame?.CanGoBack is true) || _popupHost is { },
				MainCommandMode.Action => true,

				_ => false
			} ? Visibility.Visible : Visibility.Collapsed;
		}

		private void UpdateHeaderContentAlignment()
		{
			// native impl has centering built-in
			if (!HasXamlPresenter()) return;

			if (_skiaHeaderContentControl != null)
			{
				var inline = HorizontalContentAlignment is not HorizontalAlignment.Center;

				Grid.SetColumn(_skiaHeaderContentControl, inline ? 1 : 0);
				Grid.SetColumnSpan(_skiaHeaderContentControl, inline ? 1 : 4);
			}
		}

		private void GetTemplatePart<T>(string name, out T? element) where T : class
		{
			element = GetTemplateChild(name) as T;
		}

		private Page? GetPage()
		{
			Page? page = null;
			if ((_pageRef?.TryGetTarget(out page) ?? false))
			{
				return page;
			}

			return null;
		}

		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "#if-conditioned implementation")]
		private bool HasNativePresenter()
		{
#if HAS_NATIVE_NAVBAR
			return _presenter is NativeNavigationBarPresenter;
#else
			return false;
#endif
		}

		private bool HasXamlPresenter() => _presenter is NavigationBarPresenter;

		#region Expandable Behavior

		private void SetupExpandableBehavior()
		{
			TeardownExpandableBehavior();

			if (!IsExpandable || !HasXamlPresenter())
			{
				return;
			}

			var page = GetPage();
			if (page == null)
			{
				return;
			}

			var scrollViewer = page.GetFirstDescendant<ScrollViewer>();
			if (scrollViewer != null)
			{
				AttachToScrollViewer(scrollViewer);
			}
			else
			{
				// ScrollViewer may not be in the visual tree yet; retry after one layout pass.
				DispatcherQueue.TryEnqueue(() =>
				{
					if (!IsExpandable || _scrollViewer != null) return;

					var sv = GetPage()?.GetFirstDescendant<ScrollViewer>();
					if (sv != null)
					{
						AttachToScrollViewer(sv);
					}
					else
					{
						// No ScrollViewer found; start fully expanded.
						UpdateExpandableState(0);
					}
				});
			}

			// Set initial expanded state
			UpdateExpandableState(0);
		}

		private void AttachToScrollViewer(ScrollViewer scrollViewer)
		{
			_scrollViewer = scrollViewer;
			scrollViewer.ViewChanged += OnScrollViewerViewChanged;
			_scrollViewerSubscription.Disposable = Disposable.Create(() =>
			{
				scrollViewer.ViewChanged -= OnScrollViewerViewChanged;
			});

			// Apply current scroll position
			UpdateExpandableState(scrollViewer.VerticalOffset);
		}

		private void TeardownExpandableBehavior()
		{
			_scrollViewerSubscription.Disposable = null;
			_scrollViewer = null;
			UpdateInlineContentOpacity(1.0);
		}

		private void OnScrollViewerViewChanged(object? sender, ScrollViewerViewChangedEventArgs e)
		{
			if (sender is ScrollViewer sv)
			{
				UpdateExpandableState(sv.VerticalOffset);
			}
		}

		private void UpdateExpandableState()
		{
			UpdateExpandableState(_scrollViewer?.VerticalOffset ?? 0);
		}

		private void UpdateExpandableState(double verticalOffset)
		{
			if (!IsExpandable) return;

			var expandedHeight = ExpandedHeight;
			var collapsedHeight = DefaultCollapsedHeight;
			var expandRange = expandedHeight - collapsedHeight;

			if (expandRange <= 0)
			{
				TemplateSettings.CurrentExpandedAreaHeight = 0;
				TemplateSettings.ExpandProgress = 0;
				TemplateSettings.ExpandedContentScale = 1.0;
				UpdateInlineContentOpacity(1.0);
				return;
			}

			// Clamp offset to [0, expandRange]
			var clampedOffset = Math.Max(0, Math.Min(verticalOffset, expandRange));

			// Progress: 1.0 = fully expanded (offset=0, top of content), 0.0 = collapsed (scrolled down)
			var progress = 1.0 - (clampedOffset / expandRange);

			TemplateSettings.CurrentExpandedAreaHeight = expandRange * progress;
			TemplateSettings.ExpandProgress = progress;

			// Scale: interpolate from collapsed ratio (~0.78, TitleLarge/HeadlineMedium) to 1.0
			const double minScale = 0.78;
			TemplateSettings.ExpandedContentScale = minScale + ((1.0 - minScale) * progress);

			// Inline title: hidden when expanded, visible when collapsed
			UpdateInlineContentOpacity(1.0 - progress);
		}

		private void UpdateInlineContentOpacity(double opacity)
		{
			if (_skiaHeaderContentControl != null)
			{
				_skiaHeaderContentControl.Opacity = opacity;
			}
		}

		#endregion
	}
}
