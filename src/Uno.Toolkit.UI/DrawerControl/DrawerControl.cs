#if HAS_UNO
#define STORYBOARD_RETARGET_ISSUE // https://github.com/unoplatform/uno/issues/6960
#define MANIPULATION_ABSOLUTE_COORD_ISSUE // https://github.com/unoplatform/uno/issues/6964
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Uno.UI.ToolkitLib
{
	/// <summary>
	/// Represents a container with two views; one view for the main content, 
	/// and another view that can be revealed with swipe gesture.
	/// </summary>
	[TemplatePart(Name = TemplateParts.MainContentPresenterName, Type = typeof(ContentPresenter))]
	[TemplatePart(Name = TemplateParts.DrawerContentPresenterName, Type = typeof(ContentPresenter))]
	[TemplatePart(Name = TemplateParts.LightDismissOverlayName, Type = typeof(Border))]
	public partial class DrawerControl : ContentControl
	{
		public static class TemplateParts
		{
			public const string MainContentPresenterName = "MainContentPresenter";
			public const string DrawerContentPresenterName = "DrawerContentPresenter";
			public const string LightDismissOverlayName = "LightDismissOverlay";
		}

		private const double DragToggleThresholdRatio = 1.0 / 3;
		private const double AnimateSnappingThresholdRatio = 0.95;
		private static readonly TimeSpan AnimationDuration = TimeSpan.FromMilliseconds(150);

		// template parts
		private ContentPresenter? _mainContentPresenter;
		private ContentPresenter? _drawerContentPresenter;
		private Border? _lightDismissOverlay;

		// references
		private TranslateTransform? _drawerContentPresenterTransform;
		private Storyboard _storyboard = new Storyboard();
		private DoubleAnimation? _translateAnimation, _opacityAnimation;

		// states
		private bool _isReady = false;
		private bool _isGestureCaptured = false;
		private double _startingTranslateOffset = 0;
		private bool _suppressIsOpenHandler = false;

		public DrawerControl()
		{
			DefaultStyleKey = typeof(DrawerControl);
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_mainContentPresenter = GetTemplateChild(TemplateParts.MainContentPresenterName) as ContentPresenter;
			_drawerContentPresenter = GetTemplateChild(TemplateParts.DrawerContentPresenterName) as ContentPresenter;
			_lightDismissOverlay = GetTemplateChild(TemplateParts.LightDismissOverlayName) as Border;

			if (_drawerContentPresenter != null)
			{
				UpdateSwipeContentPresenterSize();
				UpdateSwipeContentPresenterLayout();
				_drawerContentPresenter.RenderTransform = _drawerContentPresenterTransform = new TranslateTransform();

#if !STORYBOARD_RETARGET_ISSUE
				_translateAnimation = new DoubleAnimation
				{
					Duration = new Duration(AnimationDuration),
				};
				Storyboard.SetTarget(_translateAnimation, _drawerContentPresenterTransform);
				UpdateTranslateAnimationTargetProperty();
				_storyboard.Children.Add(_translateAnimation);
#else
				RebuildTranslateAnimation();
#endif

				UpdateManipulationMode();
				ManipulationStarted += OnManipulationStarted;
				ManipulationDelta += OnManipulationDelta;
				ManipulationCompleted += OnManipulationCompleted;
				SizeChanged += OnSizeChanged;
			}
			if (_lightDismissOverlay != null)
			{
				_opacityAnimation = new DoubleAnimation()
				{
					Duration = new Duration(AnimationDuration),
				};
				Storyboard.SetTarget(_opacityAnimation, _lightDismissOverlay);
				Storyboard.SetTargetProperty(_opacityAnimation, nameof(_lightDismissOverlay.Opacity));
				_storyboard.Children.Add(_opacityAnimation);

				_lightDismissOverlay.Tapped += OnLightDismissOverlayTapped;
			}

			if (DrawerDepth != 0)
			{
				UpdateIsOpen(IsOpen, animate: false);
			}
			_isReady = _drawerContentPresenter != null;
		}

		private void OnIsOpenChanged(DependencyPropertyChangedEventArgs e)
		{
			if (!_isReady) return;
			if (_suppressIsOpenHandler) return;

			StopRunningAnimation();
			UpdateIsOpen((bool)e.NewValue, animate: IsLoaded);
		}

		private void OnDrawerDepthChanged(DependencyPropertyChangedEventArgs e)
		{
			if (!_isReady) return;

			UpdateSwipeContentPresenterSize();
			UpdateIsOpen(IsOpen, animate: false);
		}

		private void OnOpenDirectionChanged(DependencyPropertyChangedEventArgs e)
		{
			if (!_isReady) return;

			StopRunningAnimation();
			UpdateSwipeContentPresenterSize();
			UpdateSwipeContentPresenterLayout();
			UpdateManipulationMode();
#if !STORYBOARD_RETARGET_ISSUE
			UpdateTranslateAnimationTargetProperty();
#else
			RebuildTranslateAnimation();
#endif
			ResetOtherAxisTranslateOffset();
			UpdateIsOpen(IsOpen, animate: false);
		}

		private void OnFitToDrawerContentChanged(DependencyPropertyChangedEventArgs e)
		{
			UpdateSwipeContentPresenterSize();
			UpdateSwipeContentPresenterLayout();

			_drawerContentPresenter?.UpdateLayout();
			UpdateIsOpen(IsOpen, animate: false);
		}

		private void OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
		{
			var position =
#if MANIPULATION_ABSOLUTE_COORD_ISSUE
				this.TransformToVisual(null).Inverse.TransformPoint(e.Position);
#else
				e.Position;
#endif

			_isGestureCaptured = IsOpen ? true : IsInRangeForOpeningEdgeSwipe(position);
			if (!_isGestureCaptured || !IsGestureEnabled) return;
			e.Handled = true;

			StopRunningAnimation();
			_startingTranslateOffset = TranslateOffset;
		}

		private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
		{
			if (!_isGestureCaptured || !IsGestureEnabled) return;
			e.Handled = true;

			var length = GetActualDrawerDepth();
			var cumulative = IsOpenDirectionHorizontal() ? e.Cumulative.Translation.X : e.Cumulative.Translation.Y;
			var currentOffset = UseNegativeTranslation()
				? Clamp(-length, _startingTranslateOffset + cumulative, 0)
				: Clamp(0, _startingTranslateOffset + cumulative, length);
			var ratio = Math.Abs(currentOffset) / length;

			UpdateOpenness(ratio);
		}

		private void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
		{
			if (!_isGestureCaptured || !IsGestureEnabled) return;
			_isGestureCaptured = false;
			e.Handled = true;

			StopRunningAnimation();
			var length = GetActualDrawerDepth();
			var cumulative = IsOpenDirectionHorizontal() ? e.Cumulative.Translation.X : e.Cumulative.Translation.Y;

			var isInCorrectDirection = Math.Sign(cumulative) == (IsOpen ^ UseNegativeTranslation() ? 1 : -1);
			var isPastThresholdRatio = Math.Abs(cumulative / length) >= DragToggleThresholdRatio;

			UpdateIsOpen(IsOpen ^ (isInCorrectDirection && isPastThresholdRatio));
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateSwipeContentPresenterSize();
			UpdateIsOpen(IsOpen, animate: false);
		}

		private void OnLightDismissOverlayTapped(object sender, TappedRoutedEventArgs e)
		{
			StopRunningAnimation();
			UpdateIsOpen(false);
		}

		private void UpdateIsOpen(bool willBeOpen, bool animate = true)
		{
			var length = GetActualDrawerDepth();
			var currentOffset = TranslateOffset;
			var targetOffset = GetSnappingOffsetFor(willBeOpen);
			var relativeDistanceRatio = Math.Abs(Math.Abs(currentOffset) - Math.Abs(targetOffset)) / length;

			if (!animate || ((1 - relativeDistanceRatio) >= AnimateSnappingThresholdRatio))
			{
				UpdateOpenness(willBeOpen ? 0 : 1);
				UpdateIsOpenWithSuppress(willBeOpen);
			}
			else
			{
				UpdateIsOpenWithSuppress(willBeOpen);
				PlayAnimation(currentOffset / GetVectoredLength(), willBeOpen);
			}

			void UpdateIsOpenWithSuppress(bool value)
			{
				try
				{
					_suppressIsOpenHandler = true;
					IsOpen = value;
				}
				finally
				{
					_suppressIsOpenHandler = false;
				}
			}
		}

		private void UpdateOpenness(double ratio)
		{
			TranslateOffset = ratio * GetVectoredLength();
			if (_lightDismissOverlay != null)
			{
				_lightDismissOverlay.Opacity = 1 - ratio;
				_lightDismissOverlay.IsHitTestVisible = ratio != 1;
			}
		}

		private void PlayAnimation(double fromRatio, bool willBeOpen)
		{
			if (_storyboard == null) return;

			var toRatio = willBeOpen ? 0 : 1;
			if (_translateAnimation != null)
			{
				var vectoredLength = GetVectoredLength();

				_translateAnimation.From = fromRatio * vectoredLength;
				_translateAnimation.To = toRatio * vectoredLength;
			}
			if (_opacityAnimation != null)
			{
				_opacityAnimation.From = 1 - fromRatio;
				_opacityAnimation.To = 1 - toRatio;
			}
			if (_lightDismissOverlay != null)
			{
				_lightDismissOverlay.IsHitTestVisible = willBeOpen;
			}

			_storyboard.Begin();
		}

		private void StopRunningAnimation()
		{
			if (_storyboard != null && _storyboard.GetCurrentState() != ClockState.Stopped)
			{
				// we want to Pause() the animation midway to avoid the jarring feeling
				// but since paused state will still yield ClockState.Active
				// we have to actually use Stop() in order to differentiate

				// pause & snapshot the animated values in the middle of animation
				_storyboard.Pause();
				var offset = TranslateOffset;
				var opacity = _lightDismissOverlay?.Opacity ?? default;

				// restore the values after stopping it
				_storyboard.Stop();
				TranslateOffset = offset;
				if (_lightDismissOverlay != null) _lightDismissOverlay.Opacity = opacity;
			}
		}

		private void UpdateManipulationMode()
		{
			ManipulationMode = IsOpenDirectionHorizontal() ? ManipulationModes.TranslateX : ManipulationModes.TranslateY;
		}

		private void UpdateSwipeContentPresenterLayout()
		{
			if (_drawerContentPresenter == null) return;

			switch (OpenDirection)
			{
				case DrawerOpenDirection.Left:
					_drawerContentPresenter.HorizontalAlignment = HorizontalAlignment.Right;
					_drawerContentPresenter.VerticalAlignment = VerticalAlignment.Stretch;
					_drawerContentPresenter.HorizontalContentAlignment = FitToDrawerContent ? HorizontalAlignment.Right : HorizontalAlignment.Stretch;
					_drawerContentPresenter.VerticalContentAlignment = VerticalAlignment.Stretch;
					break;

				case DrawerOpenDirection.Down:
					_drawerContentPresenter.HorizontalAlignment = HorizontalAlignment.Stretch;
					_drawerContentPresenter.VerticalAlignment = VerticalAlignment.Top;
					_drawerContentPresenter.HorizontalContentAlignment = HorizontalAlignment.Stretch;
					_drawerContentPresenter.VerticalContentAlignment = FitToDrawerContent ? VerticalAlignment.Top : VerticalAlignment.Stretch;
					break;

				case DrawerOpenDirection.Up:
					_drawerContentPresenter.HorizontalAlignment = HorizontalAlignment.Stretch;
					_drawerContentPresenter.VerticalAlignment = VerticalAlignment.Bottom;
					_drawerContentPresenter.HorizontalContentAlignment = HorizontalAlignment.Stretch;
					_drawerContentPresenter.VerticalContentAlignment = FitToDrawerContent ? VerticalAlignment.Bottom : VerticalAlignment.Stretch;
					break;

				case DrawerOpenDirection.Right:
				default:
					_drawerContentPresenter.HorizontalAlignment = HorizontalAlignment.Left;
					_drawerContentPresenter.VerticalAlignment = VerticalAlignment.Stretch;
					_drawerContentPresenter.HorizontalContentAlignment = FitToDrawerContent ? HorizontalAlignment.Left : HorizontalAlignment.Stretch;
					_drawerContentPresenter.VerticalContentAlignment = VerticalAlignment.Stretch;
					break;
			}


		}

		private void UpdateSwipeContentPresenterSize()
		{
			if (_drawerContentPresenter == null) return;

			if (IsOpenDirectionHorizontal())
			{
				_drawerContentPresenter.Height = double.NaN;
				_drawerContentPresenter.Width = DrawerDepth ?? (FitToDrawerContent ? double.NaN : ActualWidth);
			}
			else
			{
				_drawerContentPresenter.Height = DrawerDepth ?? (FitToDrawerContent ? double.NaN : ActualHeight);
				_drawerContentPresenter.Width = double.NaN;
			}
		}

#if !STORYBOARD_RETARGET_ISSUE
		private void UpdateTranslateAnimationTargetProperty()
		{
			if (_translateAnimation == null) return;

			var property = IsOpenDirectionHorizontal() ? nameof(_drawerContentPresenterTransform.X) : nameof(_drawerContentPresenterTransform.Y);
			Storyboard.SetTargetProperty(_translateAnimation, property);
		}
#else
		private void RebuildTranslateAnimation()
		{
			if (_translateAnimation != null)
			{
				_storyboard.Children.Remove(_translateAnimation);
				_translateAnimation = null;
			}

			_translateAnimation = new DoubleAnimation
			{
				Duration = new Duration(AnimationDuration),
			};
			var property = IsOpenDirectionHorizontal() ? nameof(TranslateTransform.X) : nameof(TranslateTransform.Y);
			Storyboard.SetTarget(_translateAnimation, _drawerContentPresenterTransform);
			Storyboard.SetTargetProperty(_translateAnimation, property);

			_storyboard.Children.Add(_translateAnimation);
		}
#endif

		private void ResetOtherAxisTranslateOffset()
		{
			if (IsOpenDirectionHorizontal())
			{
				_drawerContentPresenterTransform.Y = 0;
			}
			else
			{
				_drawerContentPresenterTransform.X = 0;
			}
		}

		// helpers
		private double TranslateOffset
		{
			get => IsOpenDirectionHorizontal() ? _drawerContentPresenterTransform.X : _drawerContentPresenterTransform.Y;
			set
			{
				if (IsOpenDirectionHorizontal()) _drawerContentPresenterTransform.X = value;
				else _drawerContentPresenterTransform.Y = value;
			}
		}

		private bool IsInRangeForOpeningEdgeSwipe(Point p)
		{
			if (EdgeSwipeDetectionLength is double limit)
			{
				var position = IsOpenDirectionHorizontal() ? p.X : p.Y;
				var edgePosition = UseNegativeTranslation()
					? 0
					: IsOpenDirectionHorizontal() ? ActualWidth : ActualHeight;
				var distanceToEdge = Math.Abs(position - edgePosition);

				return distanceToEdge <= limit;
			}
			else  // anywhere is fine if null
			{
				return true;
			}
		}

		private double GetSnappingOffsetFor(bool isOpen)
		{
			return isOpen ? 0 : GetVectoredLength();
		}

		private bool IsOpenDirectionHorizontal()
		{
			return OpenDirection switch
			{
				DrawerOpenDirection.Down => false,
				DrawerOpenDirection.Up => false,

				_ => true,
			};
		}

		private bool UseNegativeTranslation()
		{
			return OpenDirection switch
			{
				DrawerOpenDirection.Left => false,
				DrawerOpenDirection.Up => false,

				_ => true,
			};
		}

		private double GetActualDrawerDepth()
		{
			if (_drawerContentPresenter == null) throw new InvalidOperationException($"{nameof(_drawerContentPresenter)} is null");

			return DrawerDepth ?? (IsOpenDirectionHorizontal()
				? GetDrawerContentSizeReferenceElement().ActualWidth
				: GetDrawerContentSizeReferenceElement().ActualHeight
			);

			FrameworkElement GetDrawerContentSizeReferenceElement() => FitToDrawerContent
				? (GetDrawerContentElement() ?? _drawerContentPresenter)
				: _drawerContentPresenter;
			FrameworkElement? GetDrawerContentElement() =>
				_drawerContentPresenter.Content as FrameworkElement ??
				(VisualTreeHelper.GetChildrenCount(_drawerContentPresenter) > 0
					? VisualTreeHelper.GetChild(_drawerContentPresenter, 0) as FrameworkElement
					: default);
		}

		private double GetVectoredLength()
		{
			return UseNegativeTranslation() ? -GetActualDrawerDepth() : GetActualDrawerDepth();
		}

		private static double Clamp(double min, double value, double max)
		{
			return Math.Max(Math.Min(value, max), min);
		}
	}
}
