using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Uno.UI.ToolkitLib.Behaviors
{
	public static class DrawerControlBehavior
	{
		#region DependencyProperty: OpenDirection

		public static DependencyProperty OpenDirectionProperty { get; } = DependencyProperty.RegisterAttached(
			"OpenDirection",
			typeof(DrawerOpenDirection),
			typeof(DrawerControlBehavior),
			new PropertyMetadata(default(DrawerOpenDirection)));

		public static DrawerOpenDirection GetOpenDirection(DependencyObject obj) => (DrawerOpenDirection)obj.GetValue(OpenDirectionProperty);
		public static void SetOpenDirection(DependencyObject obj, DrawerOpenDirection value) => obj.SetValue(OpenDirectionProperty, value);

		#endregion
		#region DependencyProperty: LightDismissOverlayBackground

		public static DependencyProperty LightDismissOverlayBackgroundProperty { get; } = DependencyProperty.RegisterAttached(
			"LightDismissOverlayBackground",
			typeof(Brush),
			typeof(DrawerControlBehavior),
			new PropertyMetadata(default(Brush)));

		public static Brush GetLightDismissOverlayBackground(DependencyObject obj) => (Brush)obj.GetValue(LightDismissOverlayBackgroundProperty);
		public static void SetLightDismissOverlayBackground(DependencyObject obj, Brush value) => obj.SetValue(LightDismissOverlayBackgroundProperty, value);

		#endregion
		#region DependencyProperty: EdgeSwipeDetectionLength

		public static DependencyProperty EdgeSwipeDetectionLengthProperty { get; } = DependencyProperty.RegisterAttached(
			"EdgeSwipeDetectionLength",
			typeof(double?),
			typeof(DrawerControlBehavior),
			new PropertyMetadata(default(double?)));

		public static double? GetEdgeSwipeDetectionLength(DependencyObject obj) => (double?)obj.GetValue(EdgeSwipeDetectionLengthProperty);
		public static void SetEdgeSwipeDetectionLength(DependencyObject obj, double? value) => obj.SetValue(EdgeSwipeDetectionLengthProperty, value);

		#endregion
		#region DependencyProperty: IsGestureEnabled = true

		public static DependencyProperty IsGestureEnabledProperty { get; } = DependencyProperty.RegisterAttached(
			"IsGestureEnabled",
			typeof(bool),
			typeof(DrawerControlBehavior),
			new PropertyMetadata(DrawerControl.DefaultValues.IsGestureEnabled));

		public static bool GetIsGestureEnabled(DependencyObject obj) => (bool)obj.GetValue(IsGestureEnabledProperty);
		public static void SetIsGestureEnabled(DependencyObject obj, bool value) => obj.SetValue(IsGestureEnabledProperty, value);

		#endregion
	}
}
