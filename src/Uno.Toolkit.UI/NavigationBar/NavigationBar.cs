#if __IOS__ || __ANDROID__
#define HAS_NATIVE_NAVBAR
#endif

using System;
using System.Linq;
using Uno.Disposables;
using Windows.Foundation;

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

		private long _isEnabledChangedToken = 0L;

		public NavigationBar()
		{
			DefaultStyleKey = typeof(NavigationBar);
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

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

		private void RegisterEvents()
		{
			UnregisterEvents();

			var disposables = new CompositeDisposable(3);

			_isEnabledChangedToken = this.RegisterPropertyChangedCallback(IsEnabledProperty, (s, e) => UpdateCommonState());
			disposables.Add(() => this.UnregisterPropertyChangedCallback(IsEnabledProperty, _isEnabledChangedToken));

			if (_moreButton is { } moreButton)
			{
				moreButton.Click += OnMoreButtonClicked;

				disposables.Add(() => moreButton.Click -= OnMoreButtonClicked);
			}

			if (_overflowPopup is { } overflowPopup)
			{
				overflowPopup.Closed += OnOverflowPopupClosed;

				disposables.Add(() => overflowPopup.Closed -= OnOverflowPopupClosed);
			}

			_eventSubscriptions.Disposable = disposables;
		}

		private void UnregisterEvents() => _eventSubscriptions.Disposable = null;
		
		private void OnOverflowPopupClosed(object? sender, object e) => IsOpen = false;

		private void OnMoreButtonClicked(object sender, RoutedEventArgs e) => IsOpen = !IsOpen;

		private void UpdateButtonsIsCompact()
		{
			var allCommands = Enumerable.Concat(PrimaryCommands, SecondaryCommands).OfType<ICommandBarElement>();

			foreach (var command in allCommands)
			{
				command.IsCompact = !IsOpen;
			};
		}

		private void GetTemplatePart<T>(string name, out T? element) where T : class
		{
			element = GetTemplateChild(name) as T;
		}
	}
}
