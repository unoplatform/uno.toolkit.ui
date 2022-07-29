using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using static Uno.UI.Toolkit.VisibleBoundsPadding;

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

namespace Uno.Toolkit.UI.Controls
{
	public partial class SafeArea : ContentControl
	{
		[Flags]
		public enum InsetMask
		{
			All = Left | Right | Top | Bottom,
			AllWithSoftInput = All | SoftInput,
			None = 0,
			Top = 1,
			Bottom = 2,
			Left = 4,
			Right = 8,
			SoftInput = 10
		}

		public enum InsetMode
		{
			Padding,
			Margin
		}

		/// <summary>
		/// The padding of the safe area relative to the entire window.
		/// </summary>
		/// <remarks>This will be 0 if the entire window is 'safe' for content.</remarks>
		public static Thickness WindowPadding
		{
			get
			{
#if WINUI
				return new();
#else
				var visibleBounds = ApplicationView.GetForCurrentView().VisibleBounds;
				var bounds = Window.Current?.Bounds ?? Rect.Empty;
				var result = new Thickness {
					Left = visibleBounds.Left - bounds.Left,
					Top = visibleBounds.Top - bounds.Top,
					Right = bounds.Right - visibleBounds.Right,
					Bottom = bounds.Bottom - visibleBounds.Bottom
				};

#if HAS_UNO
				if (_log.IsEnabled(LogLevel.Debug))
				{
					_log.LogDebug($"WindowPadding={result} bounds={bounds} visibleBounds={visibleBounds}");
				}
#endif

				return result;
#endif
			}
		}

		/// <summary>
		/// VisibleBounds offset to the reference frame of the window Bounds.
		/// </summary>
		private static Rect OffsetVisibleBounds
		{
			get
			{
#if WINUI
				return new();
#else
				var visibleBounds = ApplicationView.GetForCurrentView().VisibleBounds;

				if (Window.Current is Window window)
				{
					var bounds = window.Bounds;
					visibleBounds.X -= bounds.X;
					visibleBounds.Y -= bounds.Y;
				}

				return visibleBounds;
#endif
			}
		}

		private static void OnInsetsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
		{
#if WINUI
			// VisibleBoundsPadding is disabled for WinUI 3 and up as there's available API for bounds.
#else
			if (dependencyObject is FrameworkElement fe)
			{
				VisibleBoundsDetails.GetInstance(fe).OnInsetsChanged((InsetMask)args.OldValue, (InsetMask)args.NewValue);
			}
			else
			{
#if HAS_UNO // Is building using Uno.UI
				if (dependencyObject.Log().IsEnabled(LogLevel.Debug))
				{
					dependencyObject.Log().LogDebug($"PaddingMask is only supported on FrameworkElement (Found {dependencyObject?.GetType()})");
				}
#endif
			}
#endif
		}

		public class VisibleBoundsDetails
		{
			private static readonly ConditionalWeakTable<FrameworkElement, VisibleBoundsDetails> _instances =
				new ConditionalWeakTable<FrameworkElement, VisibleBoundsDetails>();
			private readonly WeakReference _owner;
			private readonly TypedEventHandler<global::Windows.UI.ViewManagement.ApplicationView, object> _visibleBoundsChanged;
			private InsetMask _insetMask;
			private InsetMode _insetMode = InsetMode.Padding;
			private readonly Thickness _originalInsets;

			internal VisibleBoundsDetails(FrameworkElement owner)
			{
				_owner = new WeakReference(owner);

				_originalInsets = _insetMode switch
				{
					InsetMode.Margin => owner.Margin,
					_ or InsetMode.Padding => owner.GetPadding(),
				};

				_visibleBoundsChanged = (s2, e2) => UpdatePadding();

#if __IOS__
				// For iOS, it's required to react on SizeChanged to prevent weird alignment
				// problems with Text using the LayoutManager (NSTextContainer).
				// https://github.com/unoplatform/uno/issues/2836
				owner.SizeChanged += (s, e) => UpdatePadding();
#endif
				owner.LayoutUpdated += (s, e) => UpdatePadding();

				owner.Loaded += (s, e) =>
				{
					UpdatePadding();
					ApplicationView.GetForCurrentView().VisibleBoundsChanged += _visibleBoundsChanged;
				};
				owner.Unloaded += (s, e) => ApplicationView.GetForCurrentView().VisibleBoundsChanged -= _visibleBoundsChanged;
			}

			private FrameworkElement? Owner => _owner.Target as FrameworkElement;

			private void UpdatePadding()
			{
				if (Window.Current?.Content == null)
				{
					return;
				}

				if (!AreBoundsAspectRatiosConsistent)
				{
					return;
				}

				if (Owner is null || !Owner.IsLoaded)
				{
					return;
				}

				Thickness visibilityPadding;
				
				if (WindowPadding.Left != 0
					|| WindowPadding.Right != 0
					|| WindowPadding.Top != 0
					|| WindowPadding.Bottom != 0)
				{
					var scrollAncestor = GetScrollAncestor();

					// If the owner view is scrollable, the visibility of interest is that of the scroll viewport.
					var fixedControl = scrollAncestor ?? Owner;

					var controlBounds = GetRelativeBounds(fixedControl, Window.Current.Content);

					visibilityPadding = CalculateVisibilityPadding(OffsetVisibleBounds, controlBounds);

					if (scrollAncestor != null)
					{
						visibilityPadding = AdjustScrollablePadding(visibilityPadding, scrollAncestor);
					}
				}
				else
				{
					visibilityPadding = default(Thickness);
				}

				var padding = CalculateAppliedInsets(_insetMask, visibilityPadding);

				ApplyPadding(padding);
			}

			/// <summary>
			/// Calculate the padding required to keep the view entirely within the 'safe' visible bounds of the window.
			/// </summary>
			/// <param name="visibleBounds">The safe visible bounds of the window.</param>
			/// <param name="controlBounds">The bounds of the control, in the window's coordinates.</param>
			private Thickness CalculateVisibilityPadding(Rect visibleBounds, Rect controlBounds)
			{
				var windowPadding = WindowPadding;

				var left = Math.Min(visibleBounds.Left - controlBounds.Left, windowPadding.Left);
				var top = Math.Min(visibleBounds.Top - controlBounds.Top, windowPadding.Top);
				var right = Math.Min(controlBounds.Right - visibleBounds.Right, windowPadding.Right);
				var bottom = Math.Min(controlBounds.Bottom - visibleBounds.Bottom, windowPadding.Bottom);

				return new Thickness {
					Left = left,
					Top = top,
					Right = right,
					Bottom = bottom
				};
			}

			/// <summary>
			/// Apply adjustments when target view is inside of a ScrollViewer.
			/// </summary>
			private Thickness AdjustScrollablePadding(Thickness visibilityPadding, ScrollViewer scrollAncestor)
			{
				var scrollableRoot = scrollAncestor.Content as FrameworkElement;
#if XAMARIN
				if (scrollableRoot is ItemsPresenter)
				{
					// This implies we're probably inside a ListView, in which case the reasoning breaks down in Uno (because ItemsPresenter 
					// is *outside* the scrollable region); we skip the adjustment and hope for the best.
					scrollableRoot = null;
				}
#endif
				if (scrollableRoot != null && Owner is { })
				{
					// Get the spacing already provided by the alignment of the child relative to it ancestor at the root of the scrollable hierarchy.
					var controlBounds = GetRelativeBounds(Owner, scrollableRoot);
					var rootBounds = new Rect(0, 0, scrollableRoot.ActualWidth, scrollableRoot.ActualHeight);

					// Adjust for existing spacing
					visibilityPadding.Left -= controlBounds.Left - rootBounds.Left;
					visibilityPadding.Top -= controlBounds.Top - rootBounds.Top;
					visibilityPadding.Right -= rootBounds.Right - controlBounds.Right;
					visibilityPadding.Bottom -= rootBounds.Bottom - controlBounds.Bottom;
				}

				return visibilityPadding;
			}

			/// <summary>
			/// Calculate the padding to apply to the view, based on the selected PaddingMask.
			/// </summary>
			/// <param name="mask">The PaddingMask settings.</param>
			/// <param name="visibilityPadding">The padding required to keep the view entirely within the 'safe' visible bounds of the window.</param>
			/// <returns>The padding that will actually be set on the view.</returns>
			private Thickness CalculateAppliedInsets(InsetMask mask, Thickness visibilityPadding)
			{
				// Apply left padding if the PaddingMask is "left" or "all"
				var left = mask.HasFlag(PaddingMask.Left)
					? Math.Max(_originalInsets.Left, visibilityPadding.Left)
					: _originalInsets.Left;
				// Apply top padding if the PaddingMask is "top" or "all"
				var top = mask.HasFlag(PaddingMask.Top)
					? Math.Max(_originalInsets.Top, visibilityPadding.Top)
					: _originalInsets.Top;
				// Apply right padding if the PaddingMask is "right" or "all"
				var right = mask.HasFlag(PaddingMask.Right)
					? Math.Max(_originalInsets.Right, visibilityPadding.Right)
					: _originalInsets.Right;
				// Apply bottom padding if the PaddingMask is "bottom" or "all"
				var bottom = mask.HasFlag(PaddingMask.Bottom)
					? Math.Max(_originalInsets.Bottom, visibilityPadding.Bottom)
					: _originalInsets.Bottom;

				return new Thickness {
					Left = left,
					Top = top,
					Right = right,
					Bottom = bottom
				};
			}

			private void ApplyPadding(Thickness padding)
			{
#if HAS_UNO // Is building using Uno.UI
				if (Owner is { } owner
					&& owner.SetPadding(padding)
					&& _log.IsEnabled(LogLevel.Debug))
				{
					_log.LogDebug($"ApplyPadding={padding}");
				}
#endif
			}

			internal static VisibleBoundsDetails GetInstance(FrameworkElement element)
				=> _instances.GetValue(element, e => new VisibleBoundsDetails(e));

			internal void OnInsetsChanged(InsetMask oldValue, InsetMask newValue)
			{
				_insetMask = newValue;

				UpdatePadding();
			}

			private ScrollViewer? GetScrollAncestor()
			{
				return Owner?.FindFirstParent<ScrollViewer>();
			}

			private static Rect GetRelativeBounds(FrameworkElement boundsOf, UIElement relativeTo)
			{
				return boundsOf
					.TransformToVisual(relativeTo)
					.TransformBounds(new Rect(0, 0, boundsOf.ActualWidth, boundsOf.ActualHeight));
			}
		}

	}
}
