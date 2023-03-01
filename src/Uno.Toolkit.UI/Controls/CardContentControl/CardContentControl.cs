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

		private static readonly Windows.UI.Color DefaultShadowColor
#if __ANDROID__
			= Colors.Black;
#else
			= Windows.UI.Color.FromArgb(64, 0, 0, 0);
#endif
	}

	/// <summary>
	/// Represents a control used to visually group related child elements and information.
	/// </summary>
	public partial class CardContentControl : ContentControl
	{
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
			new PropertyMetadata(true));

		/// <summary>
		/// Gets or sets a value indicating whether the control will respond to pointer and focus events.
		/// </summary>
		public bool IsClickable
		{
			get => (bool)GetValue(IsClickableProperty);
			set => SetValue(IsClickableProperty, value);
		}

		#endregion

		public CardContentControl()
		{
			DefaultStyleKey = typeof(CardContentControl);
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
