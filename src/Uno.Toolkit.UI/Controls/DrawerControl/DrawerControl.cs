#if HAS_UNO
//#define STORYBOARD_RETARGET_ISSUE // https://github.com/unoplatform/uno/issues/6960
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
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
#endif
using Windows.System;

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Represents a container with two views; one view for the main content,
	/// and another view that can be revealed with swipe gesture.
	/// </summary>
	[TemplatePart(Name = TemplateParts.MainContentPresenterName, Type = typeof(ContentPresenter))]
	[TemplatePart(Name = TemplateParts.DrawerContentControlName, Type = typeof(ContentControl))]
	[TemplatePart(Name = TemplateParts.LightDismissOverlayName, Type = typeof(Border))]
	[TemplatePart(Name = TemplateParts.GestureInterceptorName, Type = typeof(Border))]
	public partial class DrawerControl : ContentControl
	{
		public static class TemplateParts
		{
			public const string MainContentPresenterName = "MainContentPresenter";
			public const string DrawerContentControlName = "DrawerContentControl";
			public const string LightDismissOverlayName = "LightDismissOverlay";
			public const string GestureInterceptorName = "GestureInterceptor";
		}

		private const double DragToggleThresholdRatio = 1.0 / 3; // only accept gesture open/close if 1/3 of distance is completed
		private const double AnimateSnappingThresholdRatio = 0.05; // skip animation at if 5% of distance from done
		private const double OpennessTolerance = 0.0001;
		private static readonly TimeSpan AnimationDuration = TimeSpan.FromMilliseconds(150);

		// template parts
		private ContentPresenter? _mainContentPresenter;
		private ContentControl? _drawerContentControl;
		private Border? _lightDismissOverlay;
		private Border? _gestureInterceptor;

		// references
		private DispatcherCompat _dispatcher;
		private Storyboard _storyboard = new Storyboard();
		private DoubleAnimation? _translateAnimation, _opacityAnimation;
		private TranslateTransform? _drawerContentPresenterTransform;

		// states
		private bool _isReady;
		private bool _isGestureCaptured;
		private double _startingTranslateOffset;
		private bool _suppressIsOpenHandler;
		private WeakReference<Control>? _previouslyFocusedElement;
		private FocusState _previousFocusState;

		// test backdoor
		internal Storyboard AnimationStoryboard => _storyboard;
		internal ContentControl? DrawerContentPart => _drawerContentControl;
		internal Border? LightDismissPart => _lightDismissOverlay;

		/// <summary>
		/// Walks up the visual tree to find the owning <see cref="DrawerControl"/>.
		/// </summary>
		/// <remarks>
		/// FrameworkElement.TemplatedParent is not public API on WinUI, so it
		/// cannot be used to climb out of a template on every target platform.
		/// </remarks>
		internal static DrawerControl? FindOwner(DependencyObject? element)
		{
			for (var current = element; current is not null;
				 current = VisualTreeHelper.GetParent(current))
			{
				if (current is DrawerControl drawer)
				{
					return drawer;
				}
			}

			return null;
		}

		public DrawerControl()
		{
			DefaultStyleKey = typeof(DrawerControl);

			_dispatcher = this.GetDispatcherCompat();
		}

		/// <inheritdoc />
		protected override AutomationPeer OnCreateAutomationPeer() => new DrawerControlAutomationPeer(this);

		protected override void OnApplyTemplate()
		{
			StopRunningAnimation();
			ResetPreviousTemplate();

			base.OnApplyTemplate();

			_mainContentPresenter = GetTemplateChild(TemplateParts.MainContentPresenterName) as ContentPresenter;
			_drawerContentControl = GetTemplateChild(TemplateParts.DrawerContentControlName) as ContentControl;
			_lightDismissOverlay = GetTemplateChild(TemplateParts.LightDismissOverlayName) as Border;
			_gestureInterceptor = GetTemplateChild(TemplateParts.GestureInterceptorName) as Border;

			if (_drawerContentControl != null)
			{
				if (_drawerContentControl is DrawerPane drawerPane)
				{
					drawerPane.DrawerOwner = this;
				}

				UpdateSwipeContentPresenterSize();
				UpdateSwipeContentPresenterLayout();
				_drawerContentControl.RenderTransform = _drawerContentPresenterTransform = new TranslateTransform();

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

			_storyboard.Completed += (_, _) =>
			{
				if (!IsOpen)
				{
					UpdateAccessibilityState(isOpen: false);
				}
			};

			if (_gestureInterceptor != null)
			{
				UpdateGestureInterceptorSize();
				UpdateGestureInterceptorLayout();
			}

			if (DrawerDepth != 0)
			{
				UpdateIsOpen(IsOpen, shouldAnimate: false);
			}
			else
			{
				UpdateInteractionState(IsOpen);
			}
			_isReady = _drawerContentControl != null;

			void ResetPreviousTemplate()
			{
				_isReady = false;

				_storyboard.Children.Clear();
				_storyboard = new Storyboard();

				_drawerContentPresenterTransform = null;
				_translateAnimation = null;
				_opacityAnimation = null;

				_isGestureCaptured = false;
				_startingTranslateOffset = 0;
				_suppressIsOpenHandler = false;
			}
		}

		protected override void OnKeyDown(KeyRoutedEventArgs e)
		{
			base.OnKeyDown(e);

			if (!e.Handled && TryHandleDismissKey(e.Key))
			{
				e.Handled = true;
			}
		}

		internal bool TryHandleDismissKey(VirtualKey key)
		{
			if (key != VirtualKey.Escape || !IsOpen || !IsLightDismissEnabled)
			{
				return false;
			}

			Dismiss();
			return true;
		}

		private void OnIsOpenChanged(DependencyPropertyChangedEventArgs e)
		{
			if (!_isReady) return;
			if (_suppressIsOpenHandler) return;

			var isOpen = (bool)e.NewValue;
			if (!_dispatcher.HasThreadAccess)
			{
				_dispatcher.Invoke(() =>
				{
					if (_isReady && IsOpen == isOpen)
					{
						StopRunningAnimation();
						UpdateIsOpen(isOpen, shouldAnimate: true);
					}
				});
				return;
			}

			StopRunningAnimation();
			UpdateIsOpen(isOpen, shouldAnimate: true);
		}

		private void OnDrawerDepthChanged(DependencyPropertyChangedEventArgs e)
		{
			if (!_isReady) return;

			UpdateSwipeContentPresenterSize();
			UpdateIsOpen(IsOpen, shouldAnimate: false);
		}

		private void OnOpenDirectionChanged(DependencyPropertyChangedEventArgs e)
		{
			if (!_isReady) return;

			StopRunningAnimation();
			UpdateSwipeContentPresenterSize();
			UpdateSwipeContentPresenterLayout();
			UpdateGestureInterceptorSize();
			UpdateGestureInterceptorLayout();
			UpdateManipulationMode();
#if !STORYBOARD_RETARGET_ISSUE
			UpdateTranslateAnimationTargetProperty();
#else
			RebuildTranslateAnimation();
#endif
			ResetOtherAxisTranslateOffset();
			UpdateIsOpen(IsOpen, shouldAnimate: false);
		}

		private void OnEdgeSwipeDetectionLengthChanged(DependencyPropertyChangedEventArgs e)
		{
			UpdateGestureInterceptorSize();
			UpdateGestureInterceptorLayout();
		}

		private void OnFitToDrawerContentChanged(DependencyPropertyChangedEventArgs e)
		{
			UpdateSwipeContentPresenterSize();
			UpdateSwipeContentPresenterLayout();

			_drawerContentControl?.UpdateLayout();
			UpdateIsOpen(IsOpen, shouldAnimate: false);
		}

		private void OnIsGestureEnabledChanged(DependencyPropertyChangedEventArgs e)
		{
			if (_gestureInterceptor is not null)
			{
				_gestureInterceptor.IsHitTestVisible = IsGestureEnabled && !IsOpen;
			}
		}

		private void OnIsLightDismissEnabledChanged(DependencyPropertyChangedEventArgs e)
		{
			UpdateLightDismissAccessibilityState();
		}

		private void OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
		{
			if (!ShouldHandleManipulationFrom(e.OriginalSource)) return;
			if (!IsGestureEnabled) return;

			var position =
#if MANIPULATION_ABSOLUTE_COORD_ISSUE
				this.TransformToVisual(null).Inverse.TransformPoint(e.Position);
#else
				e.Position;
#endif

			_isGestureCaptured = IsOpen ? true : IsInRangeForOpeningEdgeSwipe(position);
			if (_isGestureCaptured)
			{
				StopRunningAnimation();
				if (!IsOpen && PrepareDrawerForOpening())
				{
					TranslateOffset = GetVectoredLength();
				}

				_startingTranslateOffset = TranslateOffset;

				e.Handled = true;
			}
			else
			{
				e.Complete();
			}
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

			UpdateIsOpen(IsOpen ^ (isInCorrectDirection && isPastThresholdRatio), shouldAnimate: true);
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateSwipeContentPresenterSize();
			UpdateIsOpen(IsOpen, shouldAnimate: false);
		}

		private void OnLightDismissOverlayTapped(object sender, TappedRoutedEventArgs e)
		{
			Dismiss();
		}

		internal void Dismiss()
		{
			if (!IsOpen || !IsLightDismissEnabled)
			{
				return;
			}

			StopRunningAnimation();
			UpdateIsOpen(false, shouldAnimate: true);
		}

		private void UpdateIsOpen(bool willBeOpen, bool shouldAnimate = true)
		{
			var wasHidden = willBeOpen && PrepareDrawerForOpening();

			var length = GetActualDrawerDepth();
			if (wasHidden)
			{
				TranslateOffset = UseNegativeTranslation() ? -length : length;
			}

			var currentOffset = TranslateOffset;
			var targetOffset = GetSnappingOffsetFor(willBeOpen);
			var relativeDistanceRatio = Math.Abs(Math.Abs(currentOffset) - Math.Abs(targetOffset)) / length;

			var willAnimate =
				shouldAnimate &&
				IsLoaded &&
				_dispatcher.HasThreadAccess &&
				length > 0 && // skip animation if we have nothing to animate (either from not being ready, or no valid content)
				relativeDistanceRatio >= AnimateSnappingThresholdRatio; // skip animation if we are less than 5% from done

			if (willAnimate)
			{
				UpdateIsOpenWithSuppress(willBeOpen);
				PlayAnimation(willBeOpen);
			}
			else
			{
				UpdateOpenness(willBeOpen ? 0 : 1);
				UpdateIsOpenWithSuppress(willBeOpen);
			}

			UpdateInteractionState(willBeOpen, keepPaneVisible: willAnimate && !willBeOpen);

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
			var isClosed = Math.Abs(ratio - 1) < OpennessTolerance;
			if (_lightDismissOverlay != null)
			{
				_lightDismissOverlay.Opacity = 1 - ratio;
				_lightDismissOverlay.IsHitTestVisible = !isClosed;
				_lightDismissOverlay.Visibility = isClosed ? Visibility.Collapsed : Visibility.Visible;
			}
			if (_gestureInterceptor != null)
			{
				_gestureInterceptor.IsHitTestVisible = IsGestureEnabled && isClosed;
			}
		}

		private void PlayAnimation(bool willBeOpen)
		{
			if (_storyboard == null) return;

			var vectoredLength = GetVectoredLength();
			if (Math.Abs(vectoredLength) > 0)
			{
				var fromRatio = TranslateOffset / vectoredLength;
				var toRatio = willBeOpen ? 0 : 1;
				if (_translateAnimation != null)
				{
					_translateAnimation.From = fromRatio * vectoredLength;
					_translateAnimation.To = toRatio * vectoredLength;
				}
				if (_opacityAnimation != null)
				{
					_opacityAnimation.From = 1 - fromRatio;
					_opacityAnimation.To = 1 - toRatio;
				}

				_storyboard.Begin();
			}

			if (_lightDismissOverlay != null)
			{
				_lightDismissOverlay.IsHitTestVisible = willBeOpen;
				_lightDismissOverlay.Visibility = Visibility.Visible;
			}
			if (_gestureInterceptor != null)
			{
				_gestureInterceptor.IsHitTestVisible = IsGestureEnabled && !willBeOpen;
			}
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
			if (_drawerContentControl == null) return;

			switch (OpenDirection)
			{
				case DrawerOpenDirection.Left:
					_drawerContentControl.HorizontalAlignment = HorizontalAlignment.Right;
					_drawerContentControl.VerticalAlignment = VerticalAlignment.Stretch;
					_drawerContentControl.HorizontalContentAlignment = FitToDrawerContent ? HorizontalAlignment.Right : HorizontalAlignment.Stretch;
					_drawerContentControl.VerticalContentAlignment = VerticalAlignment.Stretch;
					break;

				case DrawerOpenDirection.Down:
					_drawerContentControl.HorizontalAlignment = HorizontalAlignment.Stretch;
					_drawerContentControl.VerticalAlignment = VerticalAlignment.Top;
					_drawerContentControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
					_drawerContentControl.VerticalContentAlignment = FitToDrawerContent ? VerticalAlignment.Top : VerticalAlignment.Stretch;
					break;

				case DrawerOpenDirection.Up:
					_drawerContentControl.HorizontalAlignment = HorizontalAlignment.Stretch;
					_drawerContentControl.VerticalAlignment = VerticalAlignment.Bottom;
					_drawerContentControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
					_drawerContentControl.VerticalContentAlignment = FitToDrawerContent ? VerticalAlignment.Bottom : VerticalAlignment.Stretch;
					break;

				case DrawerOpenDirection.Right:
				default:
					_drawerContentControl.HorizontalAlignment = HorizontalAlignment.Left;
					_drawerContentControl.VerticalAlignment = VerticalAlignment.Stretch;
					_drawerContentControl.HorizontalContentAlignment = FitToDrawerContent ? HorizontalAlignment.Left : HorizontalAlignment.Stretch;
					_drawerContentControl.VerticalContentAlignment = VerticalAlignment.Stretch;
					break;
			}
		}

		private void UpdateGestureInterceptorLayout()
		{
			if (_gestureInterceptor == null) return;

			switch (OpenDirection)
			{
				case DrawerOpenDirection.Left:
					_gestureInterceptor.HorizontalAlignment = EdgeSwipeDetectionLength.HasValue ? HorizontalAlignment.Right : HorizontalAlignment.Stretch;
					_gestureInterceptor.VerticalAlignment = VerticalAlignment.Stretch;
					break;

				case DrawerOpenDirection.Down:
					_gestureInterceptor.HorizontalAlignment = HorizontalAlignment.Stretch;
					_gestureInterceptor.VerticalAlignment = EdgeSwipeDetectionLength.HasValue ? VerticalAlignment.Top : VerticalAlignment.Stretch;
					break;

				case DrawerOpenDirection.Up:
					_gestureInterceptor.HorizontalAlignment = HorizontalAlignment.Stretch;
					_gestureInterceptor.VerticalAlignment = EdgeSwipeDetectionLength.HasValue ? VerticalAlignment.Bottom : VerticalAlignment.Stretch;
					break;

				case DrawerOpenDirection.Right:
				default:
					_gestureInterceptor.HorizontalAlignment = EdgeSwipeDetectionLength.HasValue ? HorizontalAlignment.Left : HorizontalAlignment.Stretch;
					_gestureInterceptor.VerticalAlignment = VerticalAlignment.Stretch;
					break;
			}
		}

		private void UpdateSwipeContentPresenterSize()
		{
			if (_drawerContentControl == null) return;

			if (IsOpenDirectionHorizontal())
			{
				_drawerContentControl.Height = double.NaN;
				_drawerContentControl.Width = DrawerDepth ?? (FitToDrawerContent ? double.NaN : ActualWidth);
			}
			else
			{
				_drawerContentControl.Height = DrawerDepth ?? (FitToDrawerContent ? double.NaN : ActualHeight);
				_drawerContentControl.Width = double.NaN;
			}
		}

		private void UpdateGestureInterceptorSize()
		{
			if (_gestureInterceptor == null) return;

			if (IsOpenDirectionHorizontal())
			{
				_gestureInterceptor.Height = double.NaN;
				_gestureInterceptor.Width = EdgeSwipeDetectionLength ?? double.NaN;
			}
			else
			{
				_gestureInterceptor.Height = EdgeSwipeDetectionLength ?? double.NaN;
				_gestureInterceptor.Width = double.NaN;
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
			// TODO: Revise null suppressions.
			if (IsOpenDirectionHorizontal())
			{
				_drawerContentPresenterTransform!.Y = 0;
			}
			else
			{
				_drawerContentPresenterTransform!.X = 0;
			}
		}

		// helpers
		private double TranslateOffset
		{
			// TODO: Revise null suppressions.
			get => IsOpenDirectionHorizontal() ? _drawerContentPresenterTransform!.X : _drawerContentPresenterTransform!.Y;
			set
			{
				if (IsOpenDirectionHorizontal()) _drawerContentPresenterTransform!.X = value;
				else _drawerContentPresenterTransform!.Y = value;
			}
		}

		private bool ShouldHandleManipulationFrom(object source)
		{
#pragma warning disable CS0252 // CS0252: Possible unintended reference comparison
			return source == this
				|| source == _gestureInterceptor
				|| source == _lightDismissOverlay;
#pragma warning restore CS0252
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
			else // anywhere is fine if null
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
			if (_drawerContentControl == null) throw new InvalidOperationException($"{nameof(_drawerContentControl)} is null");

			return DrawerDepth ?? (IsOpenDirectionHorizontal()
				? GetDrawerContentSizeReferenceElement().ActualWidth
				: GetDrawerContentSizeReferenceElement().ActualHeight
			);

			FrameworkElement GetDrawerContentSizeReferenceElement() => FitToDrawerContent
				? (GetDrawerContentElement() ?? _drawerContentControl)
				: _drawerContentControl;
			FrameworkElement? GetDrawerContentElement() =>
				_drawerContentControl.Content as FrameworkElement ??
				(VisualTreeHelper.GetChildrenCount(_drawerContentControl) > 0
					? VisualTreeHelper.GetChild(_drawerContentControl, 0) as FrameworkElement
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

		private void MoveFocusIntoDrawer()
		{
			if (_drawerContentControl is null)
			{
				return;
			}

			var focusedControl = XamlRoot is { } xamlRoot
				? FocusManager.GetFocusedElement(xamlRoot) as Control
				: null;
			if (focusedControl is not null && IsAncestorOf(_drawerContentControl, focusedControl))
			{
				return;
			}

			if (focusedControl is not null && _previouslyFocusedElement is null)
			{
				_previouslyFocusedElement = new WeakReference<Control>(focusedControl);
				_previousFocusState = focusedControl.FocusState == FocusState.Unfocused
					? FocusState.Programmatic
					: focusedControl.FocusState;
			}

			if (FocusManager.FindFirstFocusableElement(_drawerContentControl) is Control firstFocusable)
			{
				firstFocusable.Focus(FocusState.Programmatic);
			}
			else
			{
				_drawerContentControl.Focus(FocusState.Programmatic);
			}
		}

		private void RestoreFocus()
		{
			if (_previouslyFocusedElement?.TryGetTarget(out var previouslyFocusedElement) == true)
			{
				previouslyFocusedElement.Focus(
					_previousFocusState == FocusState.Unfocused ? FocusState.Programmatic : _previousFocusState);
			}

			_previouslyFocusedElement = null;
			_previousFocusState = FocusState.Unfocused;
		}

		private static bool IsAncestorOf(DependencyObject ancestor, DependencyObject descendant)
		{
			for (var current = descendant; current is not null; current = VisualTreeHelper.GetParent(current))
			{
				if (ReferenceEquals(current, ancestor))
				{
					return true;
				}
			}

			return false;
		}

		private void UpdateAccessibilityState(bool isOpen, bool keepPaneVisible = false)
		{
			if (!_dispatcher.HasThreadAccess)
			{
				_dispatcher.Invoke(() => UpdateAccessibilityState(isOpen, keepPaneVisible));
				return;
			}

			if (_lightDismissOverlay is not null)
			{
				_lightDismissOverlay.Visibility = isOpen || keepPaneVisible
					? Visibility.Visible
					: Visibility.Collapsed;
				AutomationProperties.SetAccessibilityView(
					_lightDismissOverlay,
					isOpen && IsLightDismissEnabled ? AccessibilityView.Control : AccessibilityView.Raw);
			}

			if (_drawerContentControl is not null)
			{
				_drawerContentControl.Visibility = isOpen || keepPaneVisible
					? Visibility.Visible
					: Visibility.Collapsed;
				_drawerContentControl.IsEnabled = isOpen;
				_drawerContentControl.IsHitTestVisible = isOpen;
				_drawerContentControl.TabFocusNavigation = isOpen
					? KeyboardNavigationMode.Cycle
					: KeyboardNavigationMode.Local;
				AutomationProperties.SetAccessibilityView(
					_drawerContentControl,
					isOpen ? AccessibilityView.Control : AccessibilityView.Raw);
			}
		}

		private void UpdateInteractionState(bool isOpen, bool keepPaneVisible = false)
		{
			if (!_dispatcher.HasThreadAccess)
			{
				_dispatcher.Invoke(() => UpdateInteractionState(isOpen, keepPaneVisible));
				return;
			}

			UpdateAccessibilityState(isOpen, keepPaneVisible);
			if (isOpen)
			{
				MoveFocusIntoDrawer();
			}
			else
			{
				RestoreFocus();
			}
		}

		private void UpdateLightDismissAccessibilityState()
		{
			if (!_dispatcher.HasThreadAccess)
			{
				_dispatcher.Invoke(UpdateLightDismissAccessibilityState);
				return;
			}

			if (_lightDismissOverlay is not null)
			{
				AutomationProperties.SetAccessibilityView(
					_lightDismissOverlay,
					IsOpen && IsLightDismissEnabled ? AccessibilityView.Control : AccessibilityView.Raw);
			}
		}

		private bool PrepareDrawerForOpening()
		{
			if (_drawerContentControl is null || _drawerContentControl.Visibility == Visibility.Visible)
			{
				return false;
			}

			_drawerContentControl.Visibility = Visibility.Visible;
			_drawerContentControl.UpdateLayout();
			return true;
		}
	}
}
