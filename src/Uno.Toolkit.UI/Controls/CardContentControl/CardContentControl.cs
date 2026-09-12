using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI
{
	public partial class CardContentControl
	{
		private static class CommonStates
		{
			public const string Normal = nameof(Normal);
			public const string PointerOver = nameof(PointerOver);
			public const string Pressed = nameof(Pressed);
			public const string Disabled = nameof(Disabled);
		}
		private static class FocusStates
		{
			public const string Unfocused = nameof(Unfocused);
			public const string Focused = nameof(Focused);
			public const string PointerFocused = nameof(PointerFocused);
		}

		private static readonly Windows.UI.Color DefaultShadowColor = 
			Windows.UI.Color.FromArgb(64, 0, 0, 0);
	}

	/// <summary>
	/// Represents a control used to visually group related child elements and information.
	/// </summary>
	public partial class CardContentControl : ContentControl
	{
		private const string ContentPresenterName = "ContentPresenter";

		private Windows.System.VirtualKey? _activationKey;

		#region DependencyProperty: Elevation

		public static DependencyProperty ElevationProperty { get; } = DependencyProperty.Register(
			nameof(Elevation),
			typeof(double),
			typeof(CardContentControl),
			new PropertyMetadata(default(double)));

		/// <summary>
		/// Gets or sets the elevation of the control.
		/// </summary>
		public
#if __ANDROID__
			new
#endif
			double Elevation
		{
			get => (double)GetValue(ElevationProperty);
			set => SetValue(ElevationProperty, value);
		}

		#endregion
		#region DependencyProperty: ShadowColor = DefaultShadowColor

		public static DependencyProperty ShadowColorProperty { get; } = DependencyProperty.Register(
			nameof(ShadowColor),
			typeof(Windows.UI.Color),
			typeof(CardContentControl),
			new PropertyMetadata(DefaultShadowColor));

		/// <summary>
		/// Gets or sets the color to use for the shadow of the control.
		/// </summary>
		public Windows.UI.Color ShadowColor
		{
			get => (Windows.UI.Color)GetValue(ShadowColorProperty);
			set => SetValue(ShadowColorProperty, value);
		}

		#endregion
		#region DependencyProperty: IsClickable = true

		public static DependencyProperty IsClickableProperty { get; } = DependencyProperty.Register(
			nameof(IsClickable),
			typeof(bool),
			typeof(CardContentControl),
			new PropertyMetadata(true, OnIsClickableChanged));

		/// <summary>
		/// Gets or sets a value indicating whether the control supports pointer, keyboard, and automation activation.
		/// </summary>
		public bool IsClickable
		{
			get => (bool)GetValue(IsClickableProperty);
			set => SetValue(IsClickableProperty, value);
		}

		private static void OnIsClickableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			if (sender is CardContentControl card)
			{
				card.UpdateIsClickable((bool)args.NewValue);
			}
		}

		#endregion

		/// <summary>
		/// Occurs when the clickable card is activated by pointer, keyboard, or automation.
		/// </summary>
		public
#if __ANDROID__
			new
#endif
			event RoutedEventHandler? Click;

		public CardContentControl()
		{
			DefaultStyleKey = typeof(CardContentControl);
			IsTabStop = IsClickable;
			Tapped += OnTapped;
		}

		/// <inheritdoc />
		protected override AutomationPeer OnCreateAutomationPeer() => new CardContentControlAutomationPeer(this);

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

		internal UIElement? GetContentTemplateRoot()
		{
			if (ContentTemplateRoot as UIElement is { } root)
			{
				return root;
			}

			if (GetTemplateChild(ContentPresenterName) is UIElement presenter)
			{
				return presenter;
			}

			return VisualTreeHelper.GetChildrenCount(this) > 0
				? VisualTreeHelper.GetChild(this, 0) as UIElement
				: null;
		}

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

		private void UpdateIsClickable(bool isClickable)
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
