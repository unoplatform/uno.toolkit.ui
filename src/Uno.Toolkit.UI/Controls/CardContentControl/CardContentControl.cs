using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endif

namespace Uno.Toolkit.UI.Controls
{
	public partial class CardContentControl : ContentControl
	{
#if __ANDROID__
		private static readonly Windows.UI.Color _defaultShadowColor = Colors.Black;
#else
		private static readonly Windows.UI.Color _defaultShadowColor = Windows.UI.Color.FromArgb(64, 0, 0, 0);
#endif

		public CardContentControl()
		{
			DefaultStyleKey = typeof(CardContentControl);
		}

		#region Elevation
		public double Elevation
		{
			get => (double)GetValue(ElevationProperty);
			set => SetValue(ElevationProperty, value);
		}

		public static readonly DependencyProperty ElevationProperty =
			DependencyProperty.Register("Elevation", typeof(double), typeof(CardContentControl), new PropertyMetadata(0));
		#endregion

		#region ShadowColor
		public Windows.UI.Color ShadowColor
		{
			get => (Windows.UI.Color)GetValue(ShadowColorProperty);
			set => SetValue(ShadowColorProperty, value);
		}

		public static readonly DependencyProperty ShadowColorProperty =
			DependencyProperty.Register("ShadowColor", typeof(Windows.UI.Color), typeof(CardContentControl), new PropertyMetadata(_defaultShadowColor));
		#endregion

		protected override void OnApplyTemplate()
		{
			if (IsEnabled)
			{
				VisualStateManager.GoToState(this, "Normal", true);
			}
			else
			{
				VisualStateManager.GoToState(this, "Disabled", true);
			}

			base.OnApplyTemplate();
		}

		protected override void OnPointerEntered(PointerRoutedEventArgs e)
		{
			VisualStateManager.GoToState(this, "PointerOver", true);

			base.OnPointerEntered(e);
		}

		protected override void OnPointerExited(PointerRoutedEventArgs e)
		{
			VisualStateManager.GoToState(this, "Normal", true);

			base.OnPointerExited(e);
		}

		protected override void OnPointerPressed(PointerRoutedEventArgs e)
		{
			VisualStateManager.GoToState(this, "Pressed", true);

			base.OnPointerPressed(e);
		}

		protected override void OnPointerReleased(PointerRoutedEventArgs e)
		{
			VisualStateManager.GoToState(this, "Normal", true);

			base.OnPointerReleased(e);
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			VisualStateManager.GoToState(this, "Focused", true);
			VisualStateManager.GoToState(this, "PointerFocused", true);

			base.OnGotFocus(e);
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			VisualStateManager.GoToState(this, "Unfocused", true);

			base.OnLostFocus(e);
		}
	}
}
