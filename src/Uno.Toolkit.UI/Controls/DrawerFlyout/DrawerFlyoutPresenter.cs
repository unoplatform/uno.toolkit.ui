#if HAS_UNO
//#define STORYBOARD_RETARGET_ISSUE // PATCHED https://github.com/unoplatform/uno/issues/6960
#define MANIPULATION_ABSOLUTE_COORD_ISSUE // https://github.com/unoplatform/uno/issues/6964
#endif

#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

namespace Uno.Toolkit.UI.Controls
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
		private TranslateTransform _drawerContentPresenterTransform;
		private Storyboard _storyboard = new Storyboard();
		private DoubleAnimation _translateAnimation;
		private Popup _popup;

		// states
		private bool _isReady = false;
		private bool _isGestureCaptured = false;
		private bool _initOnceOnLoaded = true;
		private bool _initOnceOnLayoutUpdated = true;
		private double _startingTranslateOffset = 0;
		private bool _suppressIsOpenHandler = false;

		public DrawerFlyoutPresenter()
		{
			DefaultStyleKey = typeof(DrawerFlyoutPresenter);
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

			UpdateSwipeContentPresenterLayout();
			UpdateManipulationMode();
			ManipulationStarted += OnManipulationStarted;
			ManipulationDelta += OnManipulationDelta;
			ManipulationCompleted += OnManipulationCompleted;

			_lightDismissOverlay.Tapped += OnLightDismissOverlayTapped;

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
					};
				}
			};

			// note: by the time we got here, the popup would be already opened, thus we will miss the first opened event.
			// in order to catch it, we use LayoutUpdated; Loaded event cannot be used here, as the _drawerContentPresenter
			// still don't have its Actual(Width|Height) set which are needed for changing the position.
			LayoutUpdated += OnLayoutUpdated;

			_isReady = true;
		}

		private void OnLayoutUpdated(object sender, object e)
		{
			if (_initOnceOnLayoutUpdated)
			{
				_initOnceOnLayoutUpdated = false;

				// reset to close position, and animate to open position
				UpdateOpenness(false);
				UpdateIsOpen(true, animate: true);
			}
		}

		private void OnPopupOpened(object sender, object e)
		{
			// reset to close position, and animate to open position
			UpdateOpenness(false);
			UpdateIsOpen(true, animate: true);
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

			var length = GetActualDrawerDepth();
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
			var length = GetActualDrawerDepth();
			var cumulative = IsOpenDirectionHorizontal() ? e.Cumulative.Translation.X : e.Cumulative.Translation.Y;

			var isInCorrectDirection = Math.Sign(cumulative) == (IsOpen ^ UseNegativeTranslation() ? 1 : -1);
			var isPastThresholdRatio = Math.Abs(cumulative / length) >= DragToggleThresholdRatio;

			UpdateIsOpen(IsOpen ^ (isInCorrectDirection && isPastThresholdRatio));
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
			TranslateOffset = (1 - ratio) * GetVectoredLength();
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

				// restore the values after stopping it
				_storyboard.Stop();
				TranslateOffset = offset;
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
					_drawerContentPresenter.HorizontalContentAlignment = HorizontalAlignment.Right;
					_drawerContentPresenter.VerticalContentAlignment = VerticalAlignment.Stretch;
					break;

				case DrawerOpenDirection.Down:
					_drawerContentPresenter.HorizontalAlignment = HorizontalAlignment.Stretch;
					_drawerContentPresenter.VerticalAlignment = VerticalAlignment.Top;
					_drawerContentPresenter.HorizontalContentAlignment = HorizontalAlignment.Stretch;
					_drawerContentPresenter.VerticalContentAlignment = VerticalAlignment.Top;
					break;

				case DrawerOpenDirection.Up:
					_drawerContentPresenter.HorizontalAlignment = HorizontalAlignment.Stretch;
					_drawerContentPresenter.VerticalAlignment = VerticalAlignment.Bottom;
					_drawerContentPresenter.HorizontalContentAlignment = HorizontalAlignment.Stretch;
					_drawerContentPresenter.VerticalContentAlignment = VerticalAlignment.Bottom;
					break;

				case DrawerOpenDirection.Right:
				default:
					_drawerContentPresenter.HorizontalAlignment = HorizontalAlignment.Left;
					_drawerContentPresenter.VerticalAlignment = VerticalAlignment.Stretch;
					_drawerContentPresenter.HorizontalContentAlignment = HorizontalAlignment.Left;
					_drawerContentPresenter.VerticalContentAlignment = VerticalAlignment.Stretch;
					break;
			}
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
		private double TranslateOffset
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
			if (source == _lightDismissOverlay) return false;

			// note: on uwp, we cant distinguish the origin of the event, as it would always be from this DrawerFlyoutPresenter.
			return source == this
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

		private double GetActualDrawerDepth()
		{
			if (_drawerContentPresenter == null) throw new InvalidOperationException($"{nameof(_drawerContentPresenter)} is null");

			return IsOpenDirectionHorizontal()
				? _drawerContentPresenter.ActualWidth
				: _drawerContentPresenter.ActualHeight;
		}

		private double GetVectoredLength()
		{
			return UseNegativeTranslation() ? -GetActualDrawerDepth() : GetActualDrawerDepth();
		}

		private Popup FindHostPopup()
		{
			if (VisualTreeHelper.GetParent(this) is FlyoutPresenter parent)
			{
				return VisualTreeHelper
#if IS_WINUI
					.GetOpenPopupsForXamlRoot(XamlRoot)
#else
					.GetOpenPopups(XamlWindow.Current)
#endif
					.FirstOrDefault(x => x.Child == parent);
			}

			return default;
		}

		private static double Clamp(double min, double value, double max)
		{
			return Math.Max(Math.Min(value, max), min);
		}
	}
}
