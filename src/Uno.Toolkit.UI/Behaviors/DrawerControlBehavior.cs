using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// This class contains the attached properties that can be used to access/modify the same named dependency properties on <see cref="DrawerControl"/>
	/// when it is nested inside another control like <see cref="SplitView"/> or <see cref="NavigationView"/>.
	/// </summary>
	/// <example>
	/// Usage example:
	/// <code>
	/// &lt;muxc:NavigationView
	///   behaviors:DrawerControlBehavior.LightDismissOverlayBackground=&quot;SkyBlue&quot;
	///   behaviors:DrawerControlBehavior.EdgeSwipeDetectionLength=&quot;123&quot;
	///   Style=&quot;{StaticResource DrawerNavigationViewStyle}&quot;&gt;
	/// </code>
	/// </example>
	public static class DrawerControlBehavior
	{
		#region DependencyProperty: DrawerBackground

		public static DependencyProperty DrawerBackgroundProperty { [DynamicDependency(nameof(GetDrawerBackground))] get; } = DependencyProperty.RegisterAttached(
			"DrawerBackground",
			typeof(Brush),
			typeof(DrawerControlBehavior),
			new PropertyMetadata(default(Brush)));

		[DynamicDependency(nameof(SetDrawerBackground))]
		public static Brush GetDrawerBackground(DrawerControl obj) => (Brush)obj.GetValue(DrawerBackgroundProperty);

		[DynamicDependency(nameof(GetDrawerBackground))]
		public static void SetDrawerBackground(DrawerControl obj, Brush value) => obj.SetValue(DrawerBackgroundProperty, value);

		#endregion
		#region DependencyProperty: OpenDirection

		public static DependencyProperty OpenDirectionProperty { [DynamicDependency(nameof(GetOpenDirection))] get; } = DependencyProperty.RegisterAttached(
			"OpenDirection",
			typeof(DrawerOpenDirection),
			typeof(DrawerControlBehavior),
			new PropertyMetadata(default(DrawerOpenDirection)));

		[DynamicDependency(nameof(SetOpenDirection))]
		public static DrawerOpenDirection GetOpenDirection(DependencyObject obj) => (DrawerOpenDirection)obj.GetValue(OpenDirectionProperty);

		[DynamicDependency(nameof(GetOpenDirection))]
		public static void SetOpenDirection(DependencyObject obj, DrawerOpenDirection value) => obj.SetValue(OpenDirectionProperty, value);

		#endregion
		#region DependencyProperty: LightDismissOverlayBackground

		public static DependencyProperty LightDismissOverlayBackgroundProperty { [DynamicDependency(nameof(GetLightDismissOverlayBackground))] get; } = DependencyProperty.RegisterAttached(
			"LightDismissOverlayBackground",
			typeof(Brush),
			typeof(DrawerControlBehavior),
			new PropertyMetadata(default(Brush)));

		[DynamicDependency(nameof(SetLightDismissOverlayBackground))]
		public static Brush GetLightDismissOverlayBackground(DependencyObject obj) => (Brush)obj.GetValue(LightDismissOverlayBackgroundProperty);
		[DynamicDependency(nameof(GetLightDismissOverlayBackground))]
		public static void SetLightDismissOverlayBackground(DependencyObject obj, Brush value) => obj.SetValue(LightDismissOverlayBackgroundProperty, value);

		#endregion
		#region DependencyProperty: EdgeSwipeDetectionLength

		public static DependencyProperty EdgeSwipeDetectionLengthProperty { [DynamicDependency(nameof(GetEdgeSwipeDetectionLength))] get; } = DependencyProperty.RegisterAttached(
			"EdgeSwipeDetectionLength",
			typeof(double?),
			typeof(DrawerControlBehavior),
			new PropertyMetadata(default(double?)));

		[DynamicDependency(nameof(SetEdgeSwipeDetectionLength))]
		public static double? GetEdgeSwipeDetectionLength(DependencyObject obj) => (double?)obj.GetValue(EdgeSwipeDetectionLengthProperty);
		[DynamicDependency(nameof(GetEdgeSwipeDetectionLength))]
		public static void SetEdgeSwipeDetectionLength(DependencyObject obj, double? value) => obj.SetValue(EdgeSwipeDetectionLengthProperty, value);

		#endregion
		#region DependencyProperty: IsGestureEnabled = true

		public static DependencyProperty IsGestureEnabledProperty { [DynamicDependency(nameof(GetIsGestureEnabled))] get; } = DependencyProperty.RegisterAttached(
			"IsGestureEnabled",
			typeof(bool),
			typeof(DrawerControlBehavior),
			new PropertyMetadata(DrawerControl.DefaultValues.IsGestureEnabled));

		[DynamicDependency(nameof(SetIsGestureEnabled))]
		public static bool GetIsGestureEnabled(DependencyObject obj) => (bool)obj.GetValue(IsGestureEnabledProperty);
		[DynamicDependency(nameof(GetIsGestureEnabled))]
		public static void SetIsGestureEnabled(DependencyObject obj, bool value) => obj.SetValue(IsGestureEnabledProperty, value);

		#endregion
		#region DependencyProperty: IsLightDismissEnabled = true

		public static DependencyProperty IsLightDismissEnabledProperty { [DynamicDependency(nameof(GetIsLightDismissEnabled))] get; } = DependencyProperty.RegisterAttached(
			"IsLightDismissEnabled",
			typeof(bool),
			typeof(DrawerControlBehavior),
			new PropertyMetadata(DrawerControl.DefaultValues.IsLightDismissEnabled));

		[DynamicDependency(nameof(SetIsLightDismissEnabled))]
		public static bool GetIsLightDismissEnabled(DependencyObject obj) => (bool)obj.GetValue(IsLightDismissEnabledProperty);
		[DynamicDependency(nameof(GetIsLightDismissEnabled))]
		public static void SetIsLightDismissEnabled(DependencyObject obj, bool value) => obj.SetValue(IsLightDismissEnabledProperty, value);

		#endregion
		#region DependencyProperty: FitToDrawerContent = true

		public static DependencyProperty FitToDrawerContentProperty { [DynamicDependency(nameof(GetFitToDrawerContent))] get; } = DependencyProperty.RegisterAttached(
			"FitToDrawerContent",
			typeof(bool),
			typeof(DrawerControlBehavior),
			new PropertyMetadata(DrawerControl.DefaultValues.FitToDrawerContent));

		[DynamicDependency(nameof(SetFitToDrawerContent))]
		public static bool GetFitToDrawerContent(DrawerControl obj) => (bool)obj.GetValue(FitToDrawerContentProperty);
		[DynamicDependency(nameof(GetFitToDrawerContent))]
		public static void SetFitToDrawerContent(DrawerControl obj, bool value) => obj.SetValue(FitToDrawerContentProperty, value);

		#endregion
	}
}
