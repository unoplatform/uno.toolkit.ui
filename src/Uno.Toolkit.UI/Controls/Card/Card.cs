#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI
{
	public partial class Card : Control
	{
		private const string HeaderContentPresenterName = "HeaderContentPresenter";
		private const string SubHeaderContentPresenterName = "SubHeaderContentPresenter";
		private const string SupportingContentPresenterName = "SupportingContentPresenter";

		private Windows.System.VirtualKey? _activationKey;

		/// <summary>
		/// Occurs when the clickable card is activated by pointer, keyboard, or automation.
		/// </summary>
		public
#if __ANDROID__
			new
#endif
			event RoutedEventHandler? Click;

		public Card()
		{
			DefaultStyleKey = typeof(Card);
			IsTabStop = IsClickable;
			Tapped += OnTapped;
		}

		/// <inheritdoc />
		protected override AutomationPeer OnCreateAutomationPeer() => new CardAutomationPeer(this);

		protected override void OnApplyTemplate()
		{
			VisualStateManager.GoToState(this, IsEnabled ? CommonStates.Normal : CommonStates.Disabled, true);

			base.OnApplyTemplate();
		}

		protected override void OnPointerEntered(PointerRoutedEventArgs e)
		{
			if (IsClickable)
			{
				VisualStateManager.GoToState(this, CommonStates.PointerOver, true);

				base.OnPointerEntered(e);
			}
		}

		protected override void OnPointerExited(PointerRoutedEventArgs e)
		{
			if (IsClickable)
			{
				VisualStateManager.GoToState(this, CommonStates.Normal, true);

				base.OnPointerExited(e);
			}
		}

		protected override void OnPointerPressed(PointerRoutedEventArgs e)
		{
			if (IsClickable)
			{
				VisualStateManager.GoToState(this, CommonStates.Pressed, true);

				base.OnPointerPressed(e);
			}
		}

		protected override void OnPointerReleased(PointerRoutedEventArgs e)
		{
			if (IsClickable)
			{
				VisualStateManager.GoToState(this, CommonStates.Normal, true);

				base.OnPointerReleased(e);
			}
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			if (IsClickable)
			{
				VisualStateManager.GoToState(this, FocusStates.Focused, true);
				VisualStateManager.GoToState(this, FocusStates.PointerFocused, true);

				base.OnGotFocus(e);
			}
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			if (_activationKey is not null)
			{
				VisualStateManager.GoToState(this, IsEnabled ? CommonStates.Normal : CommonStates.Disabled, true);
			}

			_activationKey = null;

			if (IsClickable)
			{
				VisualStateManager.GoToState(this, FocusStates.Unfocused, true);
			}

			base.OnLostFocus(e);
		}

		protected override void OnKeyDown(KeyRoutedEventArgs e)
		{
			base.OnKeyDown(e);

			if (!e.Handled &&
				IsClickable &&
				IsEnabled &&
				FocusState != FocusState.Unfocused &&
				IsActivationKey(e.OriginalKey))
			{
				e.Handled = true;

				if (_activationKey is null)
				{
					_activationKey = e.OriginalKey;
					VisualStateManager.GoToState(this, CommonStates.Pressed, true);
				}
			}
		}

		protected override void OnKeyUp(KeyRoutedEventArgs e)
		{
			base.OnKeyUp(e);

			if (!e.Handled &&
				_activationKey == e.OriginalKey &&
				IsActivationKey(e.OriginalKey))
			{
				_activationKey = null;
				e.Handled = true;
				VisualStateManager.GoToState(this, IsEnabled ? CommonStates.Normal : CommonStates.Disabled, true);
				InvokeClick();
			}
		}

		internal UIElement? GetHeaderContentPresenter() => GetTemplateChild(HeaderContentPresenterName) as UIElement;

		internal UIElement? GetSubHeaderContentPresenter() => GetTemplateChild(SubHeaderContentPresenterName) as UIElement;

		internal UIElement? GetSupportingContentPresenter() => GetTemplateChild(SupportingContentPresenterName) as UIElement;

		internal UIElement? GetTemplateVisualRoot() =>
			VisualTreeHelper.GetChildrenCount(this) > 0
				? VisualTreeHelper.GetChild(this, 0) as UIElement
				: null;

		internal void InvokeClick()
		{
			if (!IsClickable || !IsEnabled)
			{
				return;
			}

			if (AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked) &&
				FrameworkElementAutomationPeer.CreatePeerForElement(this) is { } peer)
			{
				peer.RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
			}

			Click?.Invoke(this, new RoutedEventArgs());
		}

		internal void UpdateIsClickable(bool isClickable)
		{
			IsTabStop = isClickable;

			if (!isClickable)
			{
				_activationKey = null;
				VisualStateManager.GoToState(this, IsEnabled ? CommonStates.Normal : CommonStates.Disabled, true);
			}
		}

		private static bool IsActivationKey(Windows.System.VirtualKey key) =>
			key is Windows.System.VirtualKey.Enter or Windows.System.VirtualKey.Space;

		private void OnTapped(object sender, TappedRoutedEventArgs e)
		{
			InvokeClick();
		}
	}
}
