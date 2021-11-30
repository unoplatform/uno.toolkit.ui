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

namespace Uno.Toolkit.UI
{
	public partial class DrawerControl
	{
		/* DrawerContent: ...
		 * DrawerDepth: The depth (width or height depending on the OpenDirection) of the drawer. 
		 *		The default value is null which enables fully stretched or fit to content (see FitToDrawerContent).
		 *		Alternatively, a concrete value can be set for a fixed depth.
		 * DrawerBackground: ...
		 * OpenDirection: The direction in which the drawer opens toward. The position of drawer when opened is the opposite of this value.
		 * IsOpen: ...
		 * FitToDrawerContent: When DrawerDepth is null, this value dictates if the drawer should:
		 *	- fit to content and aligned to the edge, or
		 *	- stretch to fill the control.
		 * LightDismissOverlayBackground: ...
		 * EdgeSwipeDetectionLength: The length (width or height depending on the OpenDirection) of the area allowed for opening swipe gesture.
		 * IsGestureEnabled: Used to en/disable swipe gesture.
		 */

		internal static class DefaultValues
		{
			public const bool IsGestureEnabled = true;
			public const bool FitToDrawerContent = true;
		}

		#region DependencyProperty: DrawerContent

		public static DependencyProperty DrawerContentProperty { get; } = DependencyProperty.Register(
			nameof(DrawerContent),
			typeof(object),
			typeof(DrawerControl),
			new PropertyMetadata(default(object)));

		/// <summary>
		/// Gets or sets the drawer content.
		/// </summary>
		public object DrawerContent
		{
			get => (object)GetValue(DrawerContentProperty);
			set => SetValue(DrawerContentProperty, value);
		}

		#endregion
		#region DependencyProperty: DrawerDepth

		public static DependencyProperty DrawerDepthProperty { get; } = DependencyProperty.Register(
			nameof(DrawerDepth),
			typeof(double?),
			typeof(DrawerControl),
			new PropertyMetadata(default(double?), OnDrawerDepthChanged));

		/// <summary>
		/// Get or sets the depth (width or height depending on the <see cref="OpenDirection"/>) of the drawer.
		/// </summary>
		/// <remarks>
		/// The default value is null which enables fully stretched or fit to content (see: <seealso cref="FitToDrawerContent"/>).
		/// Alternatively, a concrete value can be set for a fixed depth.
		/// </remarks>
		public double? DrawerDepth
		{
			get => (double?)GetValue(DrawerDepthProperty);
			set => SetValue(DrawerDepthProperty, value);
		}

		#endregion
		#region DependencyProperty: DrawerBackground

		public static DependencyProperty DrawerBackgroundProperty { get; } = DependencyProperty.Register(
			nameof(DrawerBackground),
			typeof(Brush),
			typeof(DrawerControl),
			new PropertyMetadata(default(Brush)));

		/// <summary>
		/// Gets or sets the background of the drawer.
		/// </summary>
		public Brush DrawerBackground
		{
			get => (Brush)GetValue(DrawerBackgroundProperty);
			set => SetValue(DrawerBackgroundProperty, value);
		}

		#endregion
		#region DependencyProperty: OpenDirection

		public static DependencyProperty OpenDirectionProperty { get; } = DependencyProperty.Register(
			nameof(OpenDirection),
			typeof(DrawerOpenDirection),
			typeof(DrawerControl),
			new PropertyMetadata(default(DrawerOpenDirection), OnOpenDirectionChanged));

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

		#endregion
		#region DependencyProperty: IsOpen

		public static DependencyProperty IsOpenProperty { get; } = DependencyProperty.Register(
			nameof(IsOpen),
			typeof(bool),
			typeof(DrawerControl),
			new PropertyMetadata(default(bool), OnIsOpenChanged));

		/// <summary>
		/// Gets or sets a value that specifies whether the drawer is open.
		/// </summary>
		public bool IsOpen
		{
			get => (bool)GetValue(IsOpenProperty);
			set => SetValue(IsOpenProperty, value);
		}

		#endregion
		#region DependencyProperty: FitToDrawerContent = true

		public static DependencyProperty FitToDrawerContentProperty { get; } = DependencyProperty.Register(
			nameof(FitToDrawerContent),
			typeof(bool),
			typeof(DrawerControl),
			new PropertyMetadata(DefaultValues.FitToDrawerContent, OnFitToDrawerContentChanged));

		/// <summary>
		/// Get or sets a value that indicates whether the drawer will fit to content and aligned to the edge
		/// or stretch to fill the control when <see cref="DrawerDepth"/> is null.
		/// </summary>
		public bool FitToDrawerContent
		{
			get => (bool)GetValue(FitToDrawerContentProperty);
			set => SetValue(FitToDrawerContentProperty, value);
		}

		#endregion
		#region DependencyProperty: LightDismissOverlayBackground

		public static DependencyProperty LightDismissOverlayBackgroundProperty { get; } = DependencyProperty.Register(
			nameof(LightDismissOverlayBackground),
			typeof(Brush),
			typeof(DrawerControl),
			new PropertyMetadata(default(Brush)));

		/// <summary>
		/// Gets or sets the brush used to paint the light dismiss overlay.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the length (width or height depending on the <see cref="OpenDirection"/>) of the area allowed for opening swipe gesture.
		/// </summary>
		/// <remarks>
		/// By default, this value is null allowing the drawer to be swiped opened from anywhere. Setting a positive value will enforce the edge swipe for openning.
		/// </remarks>
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

		/// <summary>
		/// Get or sets a value that indicates whether the user can interact with the control using gesture.
		/// </summary>
		public bool IsGestureEnabled
		{
			get => (bool)GetValue(IsGestureEnabledProperty);
			set => SetValue(IsGestureEnabledProperty, value);
		}

		#endregion

		private static void OnDrawerDepthChanged(DependencyObject control, DependencyPropertyChangedEventArgs e) => ((DrawerControl)control).OnDrawerDepthChanged(e);
		private static void OnOpenDirectionChanged(DependencyObject control, DependencyPropertyChangedEventArgs e) => ((DrawerControl)control).OnOpenDirectionChanged(e);
		private static void OnIsOpenChanged(DependencyObject control, DependencyPropertyChangedEventArgs e) => ((DrawerControl)control).OnIsOpenChanged(e);
		private static void OnFitToDrawerContentChanged(DependencyObject control, DependencyPropertyChangedEventArgs e) => ((DrawerControl)control).OnFitToDrawerContentChanged(e);
	}
}
