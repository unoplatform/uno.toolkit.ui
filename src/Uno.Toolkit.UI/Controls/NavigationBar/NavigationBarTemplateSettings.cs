using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif

namespace Uno.Toolkit.UI.Controls
{
	public partial class NavigationBarTemplateSettings : DependencyObject
	{
		private readonly NavigationBar _navBar;

		public NavigationBarTemplateSettings(NavigationBar navBar)
		{
			_navBar = navBar;
		}

		#region ClipRect
		public Rect ClipRect
		{
			get => (Rect)GetValue(ClipRectProperty);
			internal set => SetValue(ClipRectProperty, value);
		}

		internal static DependencyProperty ClipRectProperty { get; } =
			DependencyProperty.Register(nameof(ClipRect), typeof(Rect), typeof(NavigationBarTemplateSettings), new PropertyMetadata(new Rect()));
		#endregion

		#region CompactRootMargin
		public Thickness CompactRootMargin
		{
			get => (Thickness)GetValue(CompactRootMarginProperty);
			internal set => SetValue(CompactRootMarginProperty, value);
		}

		internal static DependencyProperty CompactRootMarginProperty { get; } =
			DependencyProperty.Register(nameof(CompactRootMargin), typeof(Thickness), typeof(NavigationBarTemplateSettings), new PropertyMetadata(new Thickness(0)));
		#endregion

		#region CompactVerticalDelta
		public double CompactVerticalDelta
		{
			get => (double)GetValue(CompactVerticalDeltaProperty);
			internal set => SetValue(CompactVerticalDeltaProperty, value);
		}

		internal static DependencyProperty CompactVerticalDeltaProperty { get; } =
			DependencyProperty.Register("CompactVerticalDelta", typeof(double), typeof(NavigationBarTemplateSettings), new PropertyMetadata(0.0));
		#endregion

		#region HiddenRootMargin
		public Thickness HiddenRootMargin
		{
			get => (Thickness)GetValue(HiddenRootMarginProperty);
			internal set => SetValue(HiddenRootMarginProperty, value);
		}

		public static DependencyProperty HiddenRootMarginProperty { get; } =
			DependencyProperty.Register("HiddenRootMargin", typeof(Thickness), typeof(NavigationBarTemplateSettings), new PropertyMetadata(new Thickness(0)));
		#endregion

		#region HiddenVerticalDelta
		public double HiddenVerticalDelta
		{
			get => (double)GetValue(HiddenVerticalDeltaProperty);
			internal set => SetValue(HiddenVerticalDeltaProperty, value);
		}

		internal static DependencyProperty HiddenVerticalDeltaProperty { get; } =
			DependencyProperty.Register("HiddenVerticalDelta", typeof(double), typeof(NavigationBarTemplateSettings), new PropertyMetadata(0.0));
		#endregion

		#region MinimalRootMargin
		public Thickness MinimalRootMargin
		{
			get => (Thickness)GetValue(MinimalRootMarginProperty);
			internal set => SetValue(MinimalRootMarginProperty, value);
		}

		internal static DependencyProperty MinimalRootMarginProperty { get; } =
			DependencyProperty.Register("MinimalRootMargin", typeof(Thickness), typeof(NavigationBarTemplateSettings), new PropertyMetadata(new Thickness(0)));
		#endregion

		#region MinimalVerticalDelta
		public double MinimalVerticalDelta
		{
			get => (double)GetValue(MinimalVerticalDeltaProperty);
			internal set => SetValue(MinimalVerticalDeltaProperty, value);
		}

		internal static DependencyProperty MinimalVerticalDeltaProperty { get; } =
			DependencyProperty.Register("MinimalVerticalDelta", typeof(double), typeof(NavigationBarTemplateSettings), new PropertyMetadata(0.0));
		#endregion


		#region NegativeCompactVerticalDelta
		public double NegativeCompactVerticalDelta
		{
			get => (double)GetValue(NegativeCompactVerticalDeltaProperty);
			internal set => SetValue(NegativeCompactVerticalDeltaProperty, value);
		}

		internal static DependencyProperty NegativeCompactVerticalDeltaProperty { get; } =
			DependencyProperty.Register("NegativeCompactVerticalDelta", typeof(double), typeof(NavigationBarTemplateSettings), new PropertyMetadata(0.0));
		#endregion

		#region NegativeMinimalVerticalDelta
		public double NegativeMinimalVerticalDelta
		{
			get => (double)GetValue(NegativeMinimalVerticalDeltaProperty);
			internal set => SetValue(NegativeMinimalVerticalDeltaProperty, value);
		}

		internal static DependencyProperty NegativeMinimalVerticalDeltaProperty { get; } =
			DependencyProperty.Register("NegativeMinimalVerticalDelta", typeof(double), typeof(NavigationBarTemplateSettings), new PropertyMetadata(0.0));
		#endregion

		#region NegativeHiddenVerticalDelta
		public double NegativeHiddenVerticalDelta
		{
			get => (double)GetValue(NegativeHiddenVerticalDeltaProperty);
			internal set => SetValue(NegativeHiddenVerticalDeltaProperty, value);
		}

		internal static DependencyProperty NegativeHiddenVerticalDeltaProperty { get; } =
			DependencyProperty.Register("NegativeHiddenVerticalDelta", typeof(double), typeof(NavigationBarTemplateSettings), new PropertyMetadata(0.0));
		#endregion

		#region ContentHeight
		public double ContentHeight
		{
			get { return (double)GetValue(ContentHeightProperty); }
			set { SetValue(ContentHeightProperty, value); }
		}

		public static DependencyProperty ContentHeightProperty { get; } =
			DependencyProperty.Register("ContentHeight", typeof(double), typeof(NavigationBarTemplateSettings), new PropertyMetadata(0.0));

		#endregion

		#region NegativeOverflowContentHeight
		public double NegativeOverflowContentHeight
		{
			get { return (double)GetValue(NegativeOverflowContentHeightProperty); }
			internal set { SetValue(NegativeOverflowContentHeightProperty, value); }
		}

		internal static DependencyProperty NegativeOverflowContentHeightProperty { get; } =
			DependencyProperty.Register("NegativeOverflowContentHeight", typeof(double), typeof(NavigationBarTemplateSettings), new PropertyMetadata(0.0));
		#endregion

		#region OverflowContentClipRect
		public Rect OverflowContentClipRect
		{
			get { return (Rect)GetValue(OverflowContentClipRectProperty); }
			internal set { SetValue(OverflowContentClipRectProperty, value); }
		}

		internal static DependencyProperty OverflowContentClipRectProperty { get; } =
			DependencyProperty.Register("OverflowContentClipRect", typeof(Rect), typeof(NavigationBarTemplateSettings), new PropertyMetadata(default(Rect)));
		#endregion


		#region OverflowContentHeight
		public double OverflowContentHeight
		{
			get { return (double)GetValue(OverflowContentHeightProperty); }
			internal set { SetValue(OverflowContentHeightProperty, value); }
		}

		internal static DependencyProperty OverflowContentHeightProperty { get; } =
			DependencyProperty.Register("OverflowContentHeight", typeof(double), typeof(NavigationBarTemplateSettings), new PropertyMetadata(0.0));
		#endregion


		#region OverflowContentHorizontalOffset
		public double OverflowContentHorizontalOffset
		{
			get { return (double)GetValue(OverflowContentHorizontalOffsetProperty); }
			internal set { SetValue(OverflowContentHorizontalOffsetProperty, value); }
		}

		internal static DependencyProperty OverflowContentHorizontalOffsetProperty { get; } =
			DependencyProperty.Register("OverflowContentHorizontalOffset", typeof(double), typeof(NavigationBarTemplateSettings), new PropertyMetadata(0.0));
		#endregion


		#region OverflowContentMaxHeight
		public double OverflowContentMaxHeight
		{
			get { return (double)GetValue(OverflowContentMaxHeightProperty); }
			internal set { SetValue(OverflowContentMaxHeightProperty, value); }
		}

		internal static DependencyProperty OverflowContentMaxHeightProperty { get; } =
			DependencyProperty.Register("OverflowContentMaxHeight", typeof(double), typeof(NavigationBarTemplateSettings), new PropertyMetadata(0.0));
		#endregion


		#region OverflowContentMinWidth
		public double OverflowContentMinWidth
		{
			get { return (double)GetValue(OverflowContentMinWidthProperty); }
			internal set { SetValue(OverflowContentMinWidthProperty, value); }
		}

		internal static DependencyProperty OverflowContentMinWidthProperty { get; } =
			DependencyProperty.Register("OverflowContentMinWidth", typeof(double), typeof(NavigationBarTemplateSettings), new PropertyMetadata(0.0));
		#endregion


		#region OverflowContentMaxWidth
		public double OverflowContentMaxWidth
		{
			get { return (double)GetValue(OverflowContentMaxWidthProperty); }
			internal set { SetValue(OverflowContentMaxWidthProperty, value); }
		}

		internal static DependencyProperty OverflowContentMaxWidthProperty { get; } =
			DependencyProperty.Register("OverflowContentMaxWidth", typeof(double), typeof(NavigationBarTemplateSettings), new PropertyMetadata(0.0));
		#endregion


		#region EffectiveOverflowButtonVisibility
		public Visibility EffectiveOverflowButtonVisibility
		{
			get { return (Visibility)GetValue(EffectiveOverflowButtonVisibilityProperty); }
			internal set { SetValue(EffectiveOverflowButtonVisibilityProperty, value); }
		}

		internal static DependencyProperty EffectiveOverflowButtonVisibilityProperty { get; } =
			DependencyProperty.Register("EffectiveOverflowButtonVisibility", typeof(Visibility), typeof(NavigationBarTemplateSettings), new PropertyMetadata(default(Visibility)));
		#endregion

	}
}
