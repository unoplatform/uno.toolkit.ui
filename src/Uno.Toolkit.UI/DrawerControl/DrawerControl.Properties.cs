using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.UI.ToolkitLib
{
	public partial class DrawerControl
	{
		internal static class DefaultValues
		{
			public const bool IsGestureEnabled = true;
		}

		#region DependencyProperty: DrawerContent

		public static DependencyProperty DrawerContentProperty { get; } = DependencyProperty.Register(
			nameof(DrawerContent),
			typeof(object),
			typeof(DrawerControl),
			new PropertyMetadata(default(object)));

		public object DrawerContent
		{
			get => (object)GetValue(DrawerContentProperty);
			set => SetValue(DrawerContentProperty, value);
		}

		#endregion
		#region DependencyProperty: DrawerDepth

		public static DependencyProperty DrawerDepthProperty { get; } = DependencyProperty.Register(
			nameof(DrawerDepth),
			typeof(double),
			typeof(DrawerControl),
			new PropertyMetadata(default(double), OnDrawerDepthChanged));

		public double DrawerDepth
		{
			get => (double)GetValue(DrawerDepthProperty);
			set => SetValue(DrawerDepthProperty, value);
		}

		#endregion
		#region DependencyProperty: OpenDirection

		public static DependencyProperty OpenDirectionProperty { get; } = DependencyProperty.Register(
			nameof(OpenDirection),
			typeof(DrawerOpenDirection),
			typeof(DrawerControl),
			new PropertyMetadata(default(DrawerOpenDirection), OnOpenDirectionChanged));

		public DrawerOpenDirection OpenDirection
		{
			get => (DrawerOpenDirection)GetValue(OpenDirectionProperty);
			set => SetValue(OpenDirectionProperty, value);
		}

		#endregion
		#region DependencyProperty: IsOpen

		public static DependencyProperty IsOpenProperty { get; } = DependencyProperty.Register(
			nameof(IsOpen),
			typeof(bool),
			typeof(DrawerControl),
			new PropertyMetadata(default(bool), OnIsOpenChanged));

		public bool IsOpen
		{
			get => (bool)GetValue(IsOpenProperty);
			set => SetValue(IsOpenProperty, value);
		}

		#endregion
		#region DependencyProperty: LightDismissOverlayBackground

		public static DependencyProperty LightDismissOverlayBackgroundProperty { get; } = DependencyProperty.Register(
			nameof(LightDismissOverlayBackground),
			typeof(Brush),
			typeof(DrawerControl),
			new PropertyMetadata(default(Brush)));

		public Brush LightDismissOverlayBackground
		{
			get => (Brush)GetValue(LightDismissOverlayBackgroundProperty);
			set => SetValue(LightDismissOverlayBackgroundProperty, value);
		}

		#endregion
		#region DependencyProperty: EdgeSwipeDetectionLength

		public static DependencyProperty EdgeSwipeDetectionLengthProperty { get; } = DependencyProperty.Register(
			nameof(EdgeSwipeDetectionLength),
			typeof(double?),
			typeof(DrawerControl),
			new PropertyMetadata(default(double?)));

		public double? EdgeSwipeDetectionLength
		{
			get => (double?)GetValue(EdgeSwipeDetectionLengthProperty);
			set => SetValue(EdgeSwipeDetectionLengthProperty, value);
		}

		#endregion
		#region DependencyProperty: IsGestureEnabled = true

		public static DependencyProperty IsGestureEnabledProperty { get; } = DependencyProperty.Register(
			nameof(IsGestureEnabled),
			typeof(bool),
			typeof(DrawerControl),
			new PropertyMetadata(DefaultValues.IsGestureEnabled));

		public bool IsGestureEnabled
		{
			get => (bool)GetValue(IsGestureEnabledProperty);
			set => SetValue(IsGestureEnabledProperty, value);
		}

		#endregion

		private static void OnDrawerDepthChanged(DependencyObject control, DependencyPropertyChangedEventArgs e) => ((DrawerControl)control).OnDrawerDepthChanged(e);
		private static void OnOpenDirectionChanged(DependencyObject control, DependencyPropertyChangedEventArgs e) => ((DrawerControl)control).OnOpenDirectionChanged(e);
		private static void OnIsOpenChanged(DependencyObject control, DependencyPropertyChangedEventArgs e) => ((DrawerControl)control).OnIsOpenChanged(e);
	}
}
