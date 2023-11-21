using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
		// purely cosmetic class, and to emphasis the presence of default values
		internal static class DefaultValues
		{
			public static readonly GridLength DrawerLength = new GridLength(0.66, GridUnitType.Star);
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
		// just like the ScrollViewer properties. This is to allow the properties to be used both on the DrawerFlyoutPresenter directly and
		// as attached property on the Flyout.FlyoutPresenterStyle.

		#region AttachedProperty: OpenDirection = Up

		public static DependencyProperty OpenDirectionProperty { [DynamicDependency(nameof(GetOpenDirection))] get; } = DependencyProperty.RegisterAttached(
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

		[DynamicDependency(nameof(SetOpenDirection))]
		public static DrawerOpenDirection GetOpenDirection(DependencyObject obj) => (DrawerOpenDirection)obj.GetValue(OpenDirectionProperty);
		[DynamicDependency(nameof(GetOpenDirection))]
		public static void SetOpenDirection(DependencyObject obj, DrawerOpenDirection value) => obj.SetValue(OpenDirectionProperty, value);

		#endregion
		#region AttachedProperty: DrawerLength = new GridLength(0.66, GridUnitType.Star)

		/// <summary>
		/// Get or sets the length (width or height depending on the <see cref="OpenDirection"/>) of the drawer.
		/// </summary>
		/// <remarks>
		/// This value has 3 mode based on <seealso cref="GridUnitType"/>:
		/// <list type="bullet">
		/// <item><see cref="GridUnitType.Auto"/>: Fit to flyout content.</item>
		/// <item><see cref="GridUnitType.Star"/>: At given ratio of screen/flyout width or height. Valid range is between 0* and 1*, excluding 0* itself.</item>
		/// <item><see cref="GridUnitType.Pixel"/>: Fixed at the given value.</item>
		/// </list>
		/// </remarks>
		public static DependencyProperty DrawerLengthProperty { [DynamicDependency(nameof(GetDrawerLength))] get; } = DependencyProperty.RegisterAttached(
			nameof(DrawerLength),
			typeof(GridLength),
			typeof(DrawerFlyoutPresenter),
			new PropertyMetadata(DefaultValues.DrawerLength, OnDrawerLengthChanged));

		public GridLength DrawerLength
		{
			get => (GridLength)GetValue(DrawerLengthProperty);
			set => SetValue(DrawerLengthProperty, value);
		}

		[DynamicDependency(nameof(SetDrawerLength))]
		public static GridLength GetDrawerLength(DependencyObject obj) => (GridLength)obj.GetValue(DrawerLengthProperty);
		[DynamicDependency(nameof(GetDrawerLength))]
		public static void SetDrawerLength(DependencyObject obj, GridLength value) => obj.SetValue(DrawerLengthProperty, value);

		#endregion
		#region AttachedProperty: LightDismissOverlayBackground

		public static DependencyProperty LightDismissOverlayBackgroundProperty { [DynamicDependency(nameof(GetLightDismissOverlayBackground))] get; } = DependencyProperty.RegisterAttached(
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

		[DynamicDependency(nameof(SetLightDismissOverlayBackground))]
		public static Brush GetLightDismissOverlayBackground(DependencyObject obj) => (Brush)obj.GetValue(LightDismissOverlayBackgroundProperty);
		[DynamicDependency(nameof(GetLightDismissOverlayBackground))]
		public static void SetLightDismissOverlayBackground(DependencyObject obj, Brush value) => obj.SetValue(LightDismissOverlayBackgroundProperty, value);

		#endregion
		#region AttachedProperty: IsGestureEnabled = true

		public static DependencyProperty IsGestureEnabledProperty { [DynamicDependency(nameof(GetIsGestureEnabled))] get; } = DependencyProperty.RegisterAttached(
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

		[DynamicDependency(nameof(SetIsGestureEnabled))]
		public static bool GetIsGestureEnabled(DependencyObject obj) => (bool)obj.GetValue(IsGestureEnabledProperty);
		[DynamicDependency(nameof(GetIsGestureEnabled))]
		public static void SetIsGestureEnabled(DependencyObject obj, bool value) => obj.SetValue(IsGestureEnabledProperty, value);

		#endregion

		private static void OnDrawerLengthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as DrawerFlyoutPresenter)?.OnDrawerLengthChanged(e);
		private static void OnOpenDirectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as DrawerFlyoutPresenter)?.OnOpenDirectionChanged(e);
		private static void OnIsOpenChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as DrawerFlyoutPresenter)?.OnIsOpenChanged(e);
	}
}
