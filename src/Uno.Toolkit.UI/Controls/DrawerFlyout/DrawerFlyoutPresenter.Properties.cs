using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI
{
	public partial class DrawerFlyoutPresenter
	{
		internal static class DefaultValues
		{
			public const DrawerOpenDirection OpenDirection = DrawerOpenDirection.Up;
			public const bool IsGestureEnabled = true;
		}

		#region DependencyProperty: [Private] IsOpen

		private static DependencyProperty IsOpenProperty { get; } = DependencyProperty.Register(
			nameof(IsOpen),
			typeof(bool),
			typeof(DrawerFlyoutPresenter),
			new PropertyMetadata(default(bool), OnIsOpenChanged));

		/// <summary>
		/// Gets or sets a value that specifies whether the drawer is open.
		/// </summary>
		private bool IsOpen
		{
			get => (bool)GetValue(IsOpenProperty);
			set => SetValue(IsOpenProperty, value);
		}

		#endregion

		// note: These properties below can function both as a direct DP (from the owner) and as an attached DP (from any dependency object),
		// just like the ScrollViewer properties.

		#region AttachedProperty: OpenDirection = Top

		public static DependencyProperty OpenDirectionProperty { get; } = DependencyProperty.RegisterAttached(
			nameof(OpenDirection),
			typeof(DrawerOpenDirection),
			typeof(DrawerFlyoutPresenter),
			new PropertyMetadata(DefaultValues.OpenDirection, OnOpenDirectionChanged));

		/// <summary>
		/// Gets or sets the direction in which the drawer opens toward.
		/// </summary>
		/// <remarks>
		/// The position of drawer when opened is the opposite of this value.
		/// </remarks>
		public DrawerOpenDirection OpenDirection
		{
			get => (DrawerOpenDirection)GetValue(OpenDirectionProperty);
			set => SetValue(OpenDirectionProperty, value);
		}

		public static DrawerOpenDirection GetOpenDirection(DependencyObject obj) => (DrawerOpenDirection)obj.GetValue(OpenDirectionProperty);
		public static void SetOpenDirection(DependencyObject obj, DrawerOpenDirection value) => obj.SetValue(OpenDirectionProperty, value);

		#endregion
		#region AttachedProperty: LightDismissOverlayBackground

		public static DependencyProperty LightDismissOverlayBackgroundProperty { get; } = DependencyProperty.RegisterAttached(
			nameof(LightDismissOverlayBackground),
			typeof(Brush),
			typeof(DrawerFlyoutPresenter),
			new PropertyMetadata(default(Brush)));

		/// <summary>
		/// Gets or sets the brush used to paint the light dismiss overlay.
		/// </summary>
		public Brush LightDismissOverlayBackground
		{
			get => (Brush)GetValue(LightDismissOverlayBackgroundProperty);
			set => SetValue(LightDismissOverlayBackgroundProperty, value);
		}

		public static Brush GetLightDismissOverlayBackground(DependencyObject obj) => (Brush)obj.GetValue(LightDismissOverlayBackgroundProperty);
		public static void SetLightDismissOverlayBackground(DependencyObject obj, Brush value) => obj.SetValue(LightDismissOverlayBackgroundProperty, value);

		#endregion
		#region AttachedProperty: IsGestureEnabled = true

		public static DependencyProperty IsGestureEnabledProperty { get; } = DependencyProperty.RegisterAttached(
			nameof(IsGestureEnabled),
			typeof(bool),
			typeof(DrawerFlyoutPresenter),
			new PropertyMetadata(DefaultValues.IsGestureEnabled));

		/// <summary>
		/// Get or sets a value that indicates whether the user can interact with the control using gesture.
		/// </summary>
		public bool IsGestureEnabled
		{
			get => (bool)GetValue(IsGestureEnabledProperty);
			set => SetValue(IsGestureEnabledProperty, value);
		}

		public static bool GetIsGestureEnabled(DependencyObject obj) => (bool)obj.GetValue(IsGestureEnabledProperty);
		public static void SetIsGestureEnabled(DependencyObject obj, bool value) => obj.SetValue(IsGestureEnabledProperty, value);

		#endregion

		private static void OnOpenDirectionChanged(DependencyObject control, DependencyPropertyChangedEventArgs e) => (control as DrawerFlyoutPresenter)?.OnOpenDirectionChanged(e);
		private static void OnIsOpenChanged(DependencyObject control, DependencyPropertyChangedEventArgs e) => (control as DrawerFlyoutPresenter)?.OnIsOpenChanged(e);
	}
}
