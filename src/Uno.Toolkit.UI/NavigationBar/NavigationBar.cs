#if __IOS__ || __ANDROID__
#define HAS_NATIVE_NAVBAR
#endif

using System;
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
	[TemplatePart(Name = MoreButton, Type = typeof(Button))]
	[TemplatePart(Name = PrimaryItemsControl, Type = typeof(ItemsControl))]
	[TemplatePart(Name = SecondaryItemsControl, Type = typeof(ItemsControl))]
	[TemplatePart(Name = NavigationBarPresenter, Type = typeof(ContentPresenter))]
	public partial class NavigationBar : ContentControl
	{
		public event EventHandler<object>? Closed;
		public event EventHandler<object>? Opened;
		public event EventHandler<object>? Closing;
		public event EventHandler<object>? Opening;
		public event TypedEventHandler<NavigationBar, DynamicOverflowItemsChangingEventArgs?>? DynamicOverflowItemsChanging;

		private const string MoreButton = "MoreButton";
		private const string OverflowPopup = "OverflowPopup";
		private const string PrimaryItemsControl = "PrimaryItemsControl";
		private const string SecondaryItemsControl = "SecondaryItemsControl";
		private const string NavigationBarPresenter = "NavigationBarPresenter";


#if HAS_NATIVE_NAVBAR
		private bool _isNativeTemplate;
#endif

		private ContentPresenter? _presenter;
		private SerialDisposable _backRequestedRevoker = new SerialDisposable();
		private long _isEnabledChangedToken = 0L;

		public NavigationBar()
		{
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			DefaultStyleKey = typeof(NavigationBar);
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			_backRequestedRevoker.Disposable = null;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
			_backRequestedRevoker.Disposable = Disposable.Create(() => SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested);
		}

		private void OnBackRequested(object sender, BackRequestedEventArgs e)
		{
			var page = this.FindFirstParent<Page>();

			if (page?.Frame is { Visibility: Visibility.Visible } frame
				&& frame.CurrentSourcePageType == page.GetType())
			{
				frame.GoBack();
			}
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			GetTemplatePart("NavigationBarPresenter", out _presenter);

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

		private void GetTemplatePart<T>(string name, out T? element) where T : class
		{
			element = GetTemplateChild(name) as T;
		}
	}
}
