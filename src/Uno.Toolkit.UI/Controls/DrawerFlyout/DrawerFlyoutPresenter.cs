#if HAS_UNO
//#define STORYBOARD_RETARGET_ISSUE // PATCHED https://github.com/unoplatform/uno/issues/6960
#define MANIPULATION_ABSOLUTE_COORD_ISSUE // https://github.com/unoplatform/uno/issues/6964
#define ITEMSREPEATER_OFFSCREEN_ITEMS_OPTIMIZATION_WORKAROUND // https://github.com/unoplatform/uno/issues/21377
#endif

#nullable disable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

using XamlWindow = Microsoft.UI.Xaml.Window;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

using XamlWindow = Windows.UI.Xaml.Window;
#endif

namespace Uno.Toolkit.UI
{
	public partial class DrawerFlyoutPresenter
	{
		public static class TemplateParts
		{
			public const string LightDismissOverlay = nameof(LightDismissOverlay);
			public const string DrawerContentPresenter = nameof(DrawerContentPresenter);
		}

		private const double DragToggleThresholdRatio = 1.0 / 3;
		private const double AnimateSnappingThresholdRatio = 0.95;
		private static readonly TimeSpan AnimationDuration = TimeSpan.FromMilliseconds(250);
	}

	[TemplatePart(Name = TemplateParts.LightDismissOverlay, Type = typeof(Border))]
	[TemplatePart(Name = TemplateParts.DrawerContentPresenter, Type = typeof(ContentPresenter))]
	public partial class DrawerFlyoutPresenter : ContentControl
	{
		// template parts
		private Border _lightDismissOverlay;
		private ContentPresenter _drawerContentPresenter;

		// references
		private readonly DispatcherCompat _dispatcher;
		private TranslateTransform _drawerContentPresenterTransform;
		private Storyboard _storyboard = new Storyboard();
		private DoubleAnimation _translateAnimation, _opacityAnimation;
		private Popup _popup;

		// states
		private bool _isReady;
		private bool _isGestureCaptured;
		private bool _initOnceOnLoaded = true;
		private double _startingTranslateOffset;
		private bool _suppressIsOpenHandler;
		private double? _lastSetOpenness;

		private Size? _lastMeasuredFlyoutContentSize;

		public DrawerFlyoutPresenter()
		{
			DefaultStyleKey = typeof(DrawerFlyoutPresenter);

			_dispatcher = this.GetDispatcherCompat();
		}

		protected override void OnApplyTemplate()
		{
			if (_isReady) throw new Exception("unexpected: Template is being re-applied.");

			base.OnApplyTemplate();

			T FindTemplatePart<T>(string name) where T : class =>
				(GetTemplateChild(name) ?? throw new Exception($"Expected template part not found: {name}"))
				as T ?? throw new Exception($"Expected template part '{name}' to be of type: {typeof(T)}");

			_lightDismissOverlay = FindTemplatePart<Border>(TemplateParts.LightDismissOverlay);
			_drawerContentPresenter = FindTemplatePart<ContentPresenter>(TemplateParts.DrawerContentPresenter);

			_drawerContentPresenter.RenderTransform = _drawerContentPresenterTransform = new TranslateTransform();
			_translateAnimation = new DoubleAnimation { Duration = new Duration(AnimationDuration) };
			Storyboard.SetTarget(_translateAnimation, _drawerContentPresenterTransform);
			UpdateTranslateAnimationTargetProperty();
			_storyboard.Children.Add(_translateAnimation);

			// no point updating size here, as we lack the flyout size that is unknown until SizeChanged
			UpdateSwipeContentPresenterLayout();
			//UpdateSwipeContentPresenterSize();

			UpdateManipulationMode();
			ManipulationStarted += OnManipulationStarted;
			ManipulationDelta += OnManipulationDelta;
			ManipulationCompleted += OnManipulationCompleted;

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

#if HAS_UNO // uno: the visual tree parent is not set, until Loaded.
			Loaded += (s, e) =>
#endif
			{
				if (_initOnceOnLoaded)
				{
					_initOnceOnLoaded = false;

					_popup = FindHostPopup() ?? throw new Exception("Unable to find host popup.");
					_popup.Opened += OnPopupOpened;
					_storyboard.Completed += (s, e) =>
					{
						if (!IsOpen)
						{
							_popup.IsOpen = false;
						}
#if ITEMSREPEATER_OFFSCREEN_ITEMS_OPTIMIZATION_WORKAROUND
						else
						{
							UpdateSwipeContentPresenterSize();
						}
#endif
					};
					_drawerContentPresenter.SizeChanged += DrawerContentPresenterSizeChanged;
					UpdateOpenness(false);
				}
			};

			// note: by the time we got here, the popup would be already opened, thus we will miss the first opened event.
			// in order to catch it, we use LayoutUpdated; Loaded event cannot be used here, as the _drawerContentPresenter
			// still don't have its Actual(Width|Height) set which are needed for changing the position.
			SizeChanged += OnSizeChanged;

			_isReady = true;
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (!HasConcreteActualSize()) return;

			UpdateSwipeContentPresenterLayout();
			UpdateSwipeContentPresenterSize();
		}

		private void DrawerContentPresenterSizeChanged(object sender, SizeChangedEventArgs e)
		{
			_lastMeasuredFlyoutContentSize = e.NewSize;

			// For native renderer specifically, on first opening, there are two size changed on the DrawerContentPresenter.
			// First one contains the minimal size, and the second one contains the stabilized size. We need to re-adjust the translate offset
			// with the values from the second one. Otherwise, we may observe a single frame "jump" by UpdateOpenness dispatched from OnPopupOpened.
			if (_lastSetOpenness is { } value)
			{
<<<<<<< HEAD
				// When the control is not loaded, we should only be in the closed(openness=0) position.
				// So we can, later, start animate from closed to opened.
				UpdateOpenness(IsLoaded ? value : 0);
=======
				UpdateOpenness(value);
>>>>>>> 1ff6e5c (fix(drawerflyout): missing initial open animation)

				// For the first open animation, we attempt to delay StartOpenAnimation to run after DrawerContentPresenterSizeChanged, by re-dispatching it.
				// On native, it may still be too early. In that case, we should start the animation again, as we now have the required size to proceed.
				var previousLength = IsOpenDirectionHorizontal() ? e.PreviousSize.Width : e.PreviousSize.Height;
				if (previousLength is 0 && HasConcreteDrawerActualSize() &&
<<<<<<< HEAD
					IsLoaded && IsOpen && _popup is { IsOpen: true })
=======
					IsOpen && _popup is { IsOpen: true })
>>>>>>> 1ff6e5c (fix(drawerflyout): missing initial open animation)
				{
					StartOpenAnimation();
				}
			}
		}

		private void OnPopupOpened(object sender, object e)
		{
			if (!HasConcreteActualSize())
			{
				_dispatcher.Invoke(() =>
				{
					StartOpenAnimation();
				});
			}
			else
			{
				StartOpenAnimation();
			}
		}

		private void OnDrawerLengthChanged(DependencyPropertyChangedEventArgs e)
		{
			if (!_isReady) return;

			StopRunningAnimation();

			UpdateSwipeContentPresenterLayout();
			UpdateSwipeContentPresenterSize();
			UpdateIsOpen(IsOpen, animate: false);
		}

		private void OnIsOpenChanged(DependencyPropertyChangedEventArgs e)
		{
			if (!_isReady) return;
			if (_suppressIsOpenHandler) return;

			StopRunningAnimation();
			UpdateIsOpen((bool)e.NewValue, animate: IsLoaded);
		}

		private void OnOpenDirectionChanged(DependencyPropertyChangedEventArgs e)
		{
			if (!_isReady) return;

			StopRunningAnimation();
			UpdateSwipeContentPresenterLayout();
			UpdateSwipeContentPresenterSize();
			UpdateManipulationMode();
			UpdateTranslateAnimationTargetProperty();
			ResetOtherAxisTranslateOffset();
			UpdateIsOpen(IsOpen, animate: false);
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

			_isGestureCaptured = true;
			StopRunningAnimation();
			_startingTranslateOffset = TranslateOffset;

			e.Handled = true;
		}

		private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
		{
			if (!_isGestureCaptured || !IsGestureEnabled) return;
			e.Handled = true;

			var length = GetActualDrawerLength();
			var cumulative = IsOpenDirectionHorizontal() ? e.Cumulative.Translation.X : e.Cumulative.Translation.Y;
			var currentOffset = UseNegativeTranslation()
				? Clamp(-length, _startingTranslateOffset + cumulative, 0)
				: Clamp(0, _startingTranslateOffset + cumulative, length);
			var ratio = Math.Abs(currentOffset) / length;

			UpdateOpenness((1 - ratio));
		}

		private void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
		{
			if (!_isGestureCaptured || !IsGestureEnabled) return;
			_isGestureCaptured = false;
			e.Handled = true;

			StopRunningAnimation();
			var length = GetActualDrawerLength();
			var cumulative = IsOpenDirectionHorizontal() ? e.Cumulative.Translation.X : e.Cumulative.Translation.Y;

			var isInCorrectDirection = Math.Sign(cumulative) == (IsOpen ^ UseNegativeTranslation() ? 1 : -1);
			var isPastThresholdRatio = Math.Abs(cumulative / length) >= DragToggleThresholdRatio;

			UpdateIsOpen(IsOpen ^ (isInCorrectDirection && isPastThresholdRatio));
		}

		private void OnLightDismissOverlayTapped(object sender, TappedRoutedEventArgs e)
		{
			if (!IsLightDismissEnabled) return;

			StopRunningAnimation();
			UpdateIsOpen(false);
		}

		private void UpdateIsOpen(bool willBeOpen, bool animate = true)
		{
			var length = GetActualDrawerLength();
			var currentOffset = TranslateOffset;
			var targetOffset = GetSnappingOffsetFor(willBeOpen);
			var relativeDistanceRatio = Math.Abs(Math.Abs(currentOffset) - Math.Abs(targetOffset)) / length;

			var shouldSkipAnimation = !animate || ((1 - relativeDistanceRatio) >= AnimateSnappingThresholdRatio);
			if (shouldSkipAnimation)
			{
				UpdateOpenness(willBeOpen);
				UpdateIsOpenWithSuppress(willBeOpen);
				if (!willBeOpen && _popup != null)
				{
					_popup.IsOpen = false;
				}
			}
			else
			{
				UpdateIsOpenWithSuppress(willBeOpen);
				PlayAnimation(currentOffset / GetVectoredLength(), willBeOpen);
				// note: the popup will be closed on Storyboard.Completed
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

		private void UpdateOpenness(bool isOpen) => UpdateOpenness(isOpen ? 1 : 0);

		private void UpdateOpenness(double ratio)
		{
			_lastSetOpenness = ratio;

			TranslateOffset = (1 - ratio) * GetVectoredLength();
			if (_lightDismissOverlay != null)
			{
				_lightDismissOverlay.Opacity = ratio;
				_lightDismissOverlay.IsHitTestVisible = ratio == 1;
			}
		}

		private void StartOpenAnimation()
		{
<<<<<<< HEAD
#if ITEMSREPEATER_OFFSCREEN_ITEMS_OPTIMIZATION_WORKAROUND
			// ItemsRepeater when materializing will skip offscreen/clipped items to optimize performance.
			// This breaks IR inside the drawer, so no items loads, as they all initially start offscreen.
			// In windows, when animated in, IR would still materialize them, but in uno don't...
			// As a workaround, we lower the size by 1 here, and restore it back in Storyboard.Completed forcing IR to re-measure.
			_drawerContentPresenter.Height -= 1;
			_drawerContentPresenter.Width -= 1;
#endif

			// normally not needed, but if this was last closed via focus lost, we will need it
			// because TranslateOffset would reset to last hold-end value (be 0), when any value is assigned...
			StopRunningAnimation();

=======
>>>>>>> 1ff6e5c (fix(drawerflyout): missing initial open animation)
			// reset to close position, and animate to open position
			UpdateOpenness(false);
			UpdateIsOpen(true, animate: true);
		}

		private void PlayAnimation(double fromRatio, bool willBeOpen)
		{
			if (_storyboard == null) return;
			if (!HasConcreteDrawerActualSize()) return;

			if (double.IsNaN(fromRatio))
			{
				fromRatio = willBeOpen ? 1 : 0;
			}
<<<<<<< HEAD
			var toRatio = (double)(willBeOpen ? 0 : 1);
=======
			var toRatio = willBeOpen ? 0 : 1;

>>>>>>> 1ff6e5c (fix(drawerflyout): missing initial open animation)

			if (_translateAnimation != null)
			{
				var vectoredLength = GetVectoredLength();

				// windows: _drawerContentPresenter is not measured on reopening
				// in such case, all numerical values here are invalid
				if (_drawerContentPresenter.ActualSize == default)
				{
					// attempt to recover with last measured size,
					// which normally shouldn't change in common scenario...
					if (_lastMeasuredFlyoutContentSize is { } lastMeasured)
					{
						// note: despite having the right values here to play the animation,
						// the drawer still blink into existence without the slide-in effect
						vectoredLength = IsOpenDirectionHorizontal() ? lastMeasured.Width : lastMeasured.Height;
						fromRatio = TranslateOffset / vectoredLength;
					}
					else
					{
						// the assumption is false, the control is opening for the first time
						// this is unsalvageable
						return;
					}
				}

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

			_lastSetOpenness = willBeOpen ? 1 : 0;
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

			// align the presenter to guarantee the right corner sticks to the edge.
			(_drawerContentPresenter.HorizontalAlignment, _drawerContentPresenter.VerticalAlignment) = OpenDirection switch
			{
				DrawerOpenDirection.Left => (HorizontalAlignment.Right, VerticalAlignment.Stretch),
				DrawerOpenDirection.Down => (HorizontalAlignment.Stretch, VerticalAlignment.Top),
				DrawerOpenDirection.Up => (HorizontalAlignment.Stretch, VerticalAlignment.Bottom),
				_ => (HorizontalAlignment.Left, VerticalAlignment.Stretch),
			};

			// but, align its content, so that it can stretch.
			_drawerContentPresenter.HorizontalContentAlignment = HorizontalAlignment.Stretch;
			_drawerContentPresenter.VerticalContentAlignment = VerticalAlignment.Stretch;
		}

		private void UpdateSwipeContentPresenterSize()
		{
			if (_drawerContentPresenter == null) return;

			var length = DrawerLength;
			var available = IsOpenDirectionHorizontal() ? ActualWidth : ActualHeight;
			var value = length.GridUnitType switch
			{
				GridUnitType.Auto => double.NaN,
				GridUnitType.Star => 0 < length.Value && length.Value <= 1
					? available * GetSafeStarValue(length.Value)
					: available,
				GridUnitType.Pixel => available != 0 && length.Value >= available
					? double.NaN
					: Math.Min(available, length.Value),

				_ => double.NaN,
			};

			if (IsOpenDirectionHorizontal())
			{
				_drawerContentPresenter.Height = double.NaN;
				_drawerContentPresenter.Width = value;
			}
			else
			{
				_drawerContentPresenter.Height = value;
				_drawerContentPresenter.Width = double.NaN;
			}

			double GetSafeStarValue(double starValue) => 0 < starValue && starValue <= 1 ? starValue : 0.66;
		}

		private void UpdateTranslateAnimationTargetProperty()
		{
			if (_translateAnimation == null) return;

			var property = IsOpenDirectionHorizontal() ? nameof(_drawerContentPresenterTransform.X) : nameof(_drawerContentPresenterTransform.Y);
			Storyboard.SetTargetProperty(_translateAnimation, property);
		}

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
	}

	public partial class DrawerFlyoutPresenter // helpers
	{
		internal double TranslateOffset
		{
			get => IsOpenDirectionHorizontal() ? _drawerContentPresenterTransform.X : _drawerContentPresenterTransform.Y;
			set
			{
				if (IsOpenDirectionHorizontal()) _drawerContentPresenterTransform.X = value;
				else _drawerContentPresenterTransform.Y = value;
			}
		}

		private bool ShouldHandleManipulationFrom(object source)
		{
			// only the content area should respond to gesture

			if (ReferenceEquals(source, _lightDismissOverlay)) return false;

			// note: on uwp, we cant distinguish the origin of the event, as it would always be from this DrawerFlyoutPresenter.
			return ReferenceEquals(source, this)
				|| (source is DependencyObject sourceAsDO && VisualTreeHelperEx.GetAncestors(sourceAsDO).Any(x => x == this));
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

		private bool HasConcreteActualSize() => ActualWidth > 0 && ActualHeight > 0;

		private bool HasConcreteDrawerActualSize() => _drawerContentPresenter?.ActualWidth > 0 && _drawerContentPresenter?.ActualHeight > 0;

		private double GetActualDrawerLength()
		{
			if (_drawerContentPresenter == null) throw new InvalidOperationException($"{nameof(_drawerContentPresenter)} is null");

			return IsOpenDirectionHorizontal()
				? _drawerContentPresenter.ActualWidth
				: _drawerContentPresenter.ActualHeight;
		}

		private double GetVectoredLength()
		{
			return UseNegativeTranslation() ? -GetActualDrawerLength() : GetActualDrawerLength();
		}

		private Popup FindHostPopup()
		{
			if (this.FindFirstParent<FlyoutPresenter>() is FlyoutPresenter flyoutPresenter)
			{
				return VisualTreeHelper.GetOpenPopupsForXamlRoot(XamlRoot).FirstOrDefault(x => x.Child == flyoutPresenter);
			}

			return default;
		}

		private static double Clamp(double min, double value, double max)
		{
			return Math.Max(Math.Min(value, max), min);
		}
	}

#if DEBUG
	[DebuggerTypeProxy(typeof(DebugProxy))]
	public partial class DrawerFlyoutPresenter
	{
		public class DebugProxy(DrawerFlyoutPresenter owner)
		{
			public double? _lastSetOpenness => owner._lastSetOpenness;
			public Size? _lastMeasuredFlyoutContentSize => owner._lastMeasuredFlyoutContentSize;
			public double? ActualDrawerLength => owner.GetActualDrawerLength();
			public double TranslateOffset => owner.TranslateOffset;

			public DrawerOpenDirection OpenDirection => owner.OpenDirection;
			public ClockState? StoryboardState => owner._storyboard?.GetCurrentState();
		}
	}
#endif
}
