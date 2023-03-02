using Windows.ApplicationModel.Store;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endif

namespace Uno.Toolkit.UI
{
	public partial class Card : Control
	{
		public Card()
		{
			DefaultStyleKey = typeof(Card);
		}

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
			if (IsClickable)
			{
				VisualStateManager.GoToState(this, FocusStates.Unfocused, true);

				base.OnLostFocus(e);
			}
		}
	}
}
