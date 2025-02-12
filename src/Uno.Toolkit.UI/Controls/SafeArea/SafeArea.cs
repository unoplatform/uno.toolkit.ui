// SafeArea is disabled for WinUI 3 and up as there's no available API for VisibleBounds.
#if IS_WINUI && !HAS_UNO
#define VISIBLEBOUNDS_API_NOT_SUPPORTED
#endif

using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;
using Uno.Disposables;
using Uno.Extensions;
using Uno.Logging;
using Windows.Foundation;
using Windows.UI.ViewManagement;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using XamlWindow = Microsoft.UI.Xaml.Window;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using XamlWindow = Windows.UI.Xaml.Window;
#endif


namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Ensures its content are always within the <see cref="ApplicationView.VisibleBounds"/> and never blocked by the onscreen keyboard, by applying the appropriate margin or padding.
	/// </summary>
	/// <remarks>
	/// SafeArea can also be used as an attached property on any <see cref="FrameworkElement"/>.
	/// </remarks>
	public partial class SafeArea : ContentControl
	{
		private const string SoftInputVisualStateName = "SoftInput";
		private const string NoSoftInputVisualStateName = "NoSoftInput";

#if !VISIBLEBOUNDS_API_NOT_SUPPORTED
		private bool _isTemplateApplied;
#endif

		[Flags]
		public enum InsetMask
		{
			None = 0,
			Top = 1,
			Bottom = 2,
			Left = 4,
			Right = 8,
			SoftInput = 16,
			VisibleBounds = Left | Right | Top | Bottom,
			All = VisibleBounds | SoftInput
		}

		public enum InsetMode
		{
			Padding,
			Margin
		}

		private static readonly ILogger _log = typeof(SafeArea).Log();

		public SafeArea()
		{
			DefaultStyleKey = typeof(SafeArea);
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			UpdateInsetsVisualState();

#if !VISIBLEBOUNDS_API_NOT_SUPPORTED
			_isTemplateApplied = true;
#endif
		}

		private void UpdateInsetsVisualState()
		{
			var state = GetInsets(this).HasFlag(InsetMask.SoftInput) ? SoftInputVisualStateName : NoSoftInputVisualStateName;
			VisualStateManager.GoToState(this, state, false);
		}

		/// <summary>
		/// The insets of the safe area relative to the entire XamlWindow.
		/// </summary>
		/// <remarks>This will be 0 if the entire window is 'safe' for content.</remarks>
		public static Thickness WindowInsets => GetWindowInsets();

		private static void OnInsetsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
		{
#if VISIBLEBOUNDS_API_NOT_SUPPORTED
			if (_log.IsEnabled(LogLevel.Warning))
			{
				_log.LogWarning("SafeArea is not supported on this platform.");
			}
#else
			if (dependencyObject is FrameworkElement fe)
			{
				if (fe is SafeArea safeArea && safeArea._isTemplateApplied)
				{
					safeArea.UpdateInsetsVisualState();
				}

				SafeAreaDetails.GetInstance(fe).OnInsetsChanged((InsetMask)args.OldValue, (InsetMask)args.NewValue);
			}
			else
			{
				if (dependencyObject.Log().IsEnabled(LogLevel.Debug))
				{
					dependencyObject.Log().LogDebug($"Insets is only supported on FrameworkElement (Found {dependencyObject?.GetType()})");
				}
			}
#endif
		}

		private static void OnSafeAreaOverrideChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
		{
#if VISIBLEBOUNDS_API_NOT_SUPPORTED
			if (_log.IsEnabled(LogLevel.Warning))
			{
				_log.LogWarning("SafeArea is not supported on this platform.");
			}
#else
			if (dependencyObject is FrameworkElement fe)
			{
				SafeAreaDetails.GetInstance(fe).OnSafeAreaOverrideChanged((Thickness?)args.OldValue, (Thickness?)args.NewValue);
			}
			else
			{
				if (dependencyObject.Log().IsEnabled(LogLevel.Debug))
				{
					dependencyObject.Log().LogDebug($"SafeAreaOverride is only supported on FrameworkElement (Found {dependencyObject?.GetType()})");
				}
			}
#endif
		}

		private static void OnInsetModeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
		{
#if VISIBLEBOUNDS_API_NOT_SUPPORTED
			if (_log.IsEnabled(LogLevel.Warning))
			{
				_log.LogWarning("SafeArea is not supported on this platform.");
			}
#else
			if (dependencyObject is FrameworkElement fe)
			{
				SafeAreaDetails.GetInstance(fe).OnInsetModeChanged((InsetMode)args.OldValue, (InsetMode)args.NewValue);
			}
			else
			{
				if (dependencyObject.Log().IsEnabled(LogLevel.Debug))
				{
					dependencyObject.Log().LogDebug($"Mode is only supported on FrameworkElement (Found {dependencyObject?.GetType()})");
				}
			}
#endif
		}

		/// <summary>
		/// If false, ApplicationView.VisibleBounds and XamlWindow.Current.Bounds have different aspect ratios (eg portrait vs landscape) which
		/// might arise transiently when the screen orientation changes.
		/// </summary>
		private static bool AreBoundsAspectRatiosConsistent
		{
			get
			{
#if HAS_UNO && !WINUI
				return ApplicationView.GetForCurrentView().VisibleBounds.GetOrientation() == XamlWindow.Current?.Bounds.GetOrientation();
#else
				return true;
#endif
			}
		}

		private static Thickness GetWindowInsets(Rect? safeAreaOverride = null, bool withSoftInput = false)
		{
#if VISIBLEBOUNDS_API_NOT_SUPPORTED
			if (_log.IsEnabled(LogLevel.Warning))
			{
				_log.LogWarning("SafeArea is not supported on this platform.");
			}
			return new();
#else
			var visibleBounds = GetVisibleBounds(safeAreaOverride, withSoftInput);
			var bounds = XamlWindow.Current?.Bounds ?? Rect.Empty;

			var result = new Thickness
			{
				Left = visibleBounds.Left - bounds.Left,
				Top = visibleBounds.Top - bounds.Top,
				Right = bounds.Right - visibleBounds.Right,
				Bottom = bounds.Bottom - visibleBounds.Bottom,
			};

			if (_log.IsEnabled(LogLevel.Debug))
			{
				_log.LogDebug($"WindowInsets={result} bounds={bounds} visibleBounds={visibleBounds}");
			}

			return result;
#endif
		}


		private static Rect GetVisibleBounds(Rect? safeAreaOverride = null, bool withSoftInput = false)
		{
#if VISIBLEBOUNDS_API_NOT_SUPPORTED
			if (_log.IsEnabled(LogLevel.Warning))
			{
				_log.LogWarning("SafeArea is not supported on this platform.");
			}
			return new();
#else
			var visibleBounds = safeAreaOverride?.IsEmptyOrZero() ?? true
				? ApplicationView.GetForCurrentView().VisibleBounds
				: safeAreaOverride.GetValueOrDefault();
#if __ANDROID__
			var statusBarOffset = 0d;
			if ((int)Android.OS.Build.VERSION.SdkInt < 35 && !XamlWindow.Current!.IsStatusBarTranslucent())
			{
				statusBarOffset = Windows.UI.ViewManagement.StatusBar.GetForCurrentView()?.OccludedRect.Height ?? 0d;
			}
#endif
			if (withSoftInput)
			{
				var inputRect = InputPane.GetForCurrentView()?.OccludedRect ?? Rect.Empty;
				if (inputRect.Top != 0 && inputRect.Top < visibleBounds.Bottom)
				{

					var windowBottom = XamlWindow.Current!.Bounds.Bottom;
#if __ANDROID__
					var totalOffset = Math.Max(0, inputRect.Bottom - windowBottom);

					// On Android, the InputPane OccludedRect includes the space needed for the system-level NavigationBar
					// (either the 3-/2-button navigation area or the gesture navigation "pill"), as well as the height of the system status bar.
					// If either of these areas are not translucent, the VisibleBounds does not include them in its Rect and we will have to offset
					// the InputRect to align with the VisibleBounds Rect.
					if (totalOffset > 0)
					{
						var navBarOffset = (totalOffset - statusBarOffset);

						inputRect.Height -= navBarOffset;
					}

#endif
					var newBottom = windowBottom - inputRect.Height;
					visibleBounds.Height = newBottom - visibleBounds.Y;

				}
			}
#if __ANDROID__
			else
			{
				visibleBounds.Y += statusBarOffset;
				visibleBounds.Height -= statusBarOffset;
			}
#endif
			return visibleBounds;
#endif
		}

		internal class SafeAreaDetails
		{
			private static readonly ConditionalWeakTable<FrameworkElement, SafeAreaDetails> _instances =
				new ConditionalWeakTable<FrameworkElement, SafeAreaDetails>();
			private readonly WeakReference _owner;
			private Rect? _overriddenVisibleBounds;
			private InsetMask _insetMask;
			private InsetMode _insetMode = InsetMode.Padding;
			private readonly Thickness _originalPadding;
			private readonly Thickness _originalMargin;
			private Thickness _appliedPadding = new Thickness(0);
			private Thickness _appliedMargin = new Thickness(0);
			private readonly CompositeDisposable _subscriptions = new();
			private ScrollViewer? _scrollAncestor;

			internal SafeAreaDetails(FrameworkElement owner)
			{
				_owner = new WeakReference(owner);

				_originalMargin = owner.Margin;
				_originalPadding = PaddingHelper.GetPadding(owner);
#if __IOS__
				// For iOS, it's required to react on SizeChanged to prevent weird alignment
				// problems with Text using the LayoutManager (NSTextContainer).
				// https://github.com/unoplatform/uno/issues/2836

				owner.SizeChanged += OnInsetUpdateRequired;
				_subscriptions.Add(() => owner.SizeChanged -= OnInsetUpdateRequired);
#endif
				owner.LayoutUpdated += OnInsetUpdateRequired;
				_subscriptions.Add(() => owner.LayoutUpdated -= OnInsetUpdateRequired);

				if (!owner.IsLoaded)
				{
					owner.Loaded += OnOwnerLoaded;
					_subscriptions.Add(() => owner.Loaded -= OnOwnerLoaded);
				}
				else
				{
					RegisterEvents();
				}

				owner.Unloaded += OnOwnerUnloaded;
				_subscriptions.Add(() => owner.Unloaded -= OnOwnerUnloaded);
			}

			private void OnOwnerUnloaded(object sender, RoutedEventArgs e)
			{
				Unregister();
			}

			private void OnOwnerLoaded(object sender, RoutedEventArgs e)
			{
				UpdateInsets();
				RegisterEvents();
			}

			private void OnInsetUpdateRequired(object? sender, object? args)
			{
				UpdateInsets();
			}

			private void RegisterEvents()
			{
				ApplicationView.GetForCurrentView().VisibleBoundsChanged += OnInsetUpdateRequired;
				_subscriptions.Add(() => ApplicationView.GetForCurrentView().VisibleBoundsChanged -= OnInsetUpdateRequired);


				if (InputPane.GetForCurrentView() is { } inputPane)
				{
					inputPane.Showing += OnInputPaneChanged;
					inputPane.Hiding += OnInputPaneChanged;

					_subscriptions.Add(() =>
					{
						inputPane.Showing -= OnInputPaneChanged;
						inputPane.Hiding -= OnInputPaneChanged;
					});
				}

				if (GetScrollAncestor() is { } scrollAncestor)
				{
					_scrollAncestor = scrollAncestor;

					_scrollAncestor.SizeChanged += OnInsetUpdateRequired;
					_subscriptions.Add(() => _scrollAncestor.SizeChanged -= OnInsetUpdateRequired);
				}
			}

			private void Unregister()
			{
				_subscriptions.Dispose();
			}

			private void OnInputPaneChanged(InputPane sender, InputPaneVisibilityEventArgs args)
			{
				if (HasSoftInput())
				{
					UpdateInsets();
				}
			}

			private FrameworkElement? Owner => _owner.Target as FrameworkElement;

			/// <summary>
			/// VisibleBounds offset to the reference frame of the window Bounds.
			/// </summary>
#pragma warning disable CA1822 // Mark members as static
			private Rect OffsetVisibleBounds
#pragma warning disable CA1822
			{
				get
				{
#if VISIBLEBOUNDS_API_NOT_SUPPORTED
					if (_log.IsEnabled(LogLevel.Warning))
					{
						_log.LogWarning("SafeArea is not supported on this platform.");
					}
					return new();
#else
					var visibleBounds = GetVisibleBounds(_overriddenVisibleBounds, HasSoftInput());
					if (XamlWindow.Current is XamlWindow window)
					{
						var bounds = window.Bounds;
						visibleBounds.X -= bounds.X;
						visibleBounds.Y -= bounds.Y;
					}

					return visibleBounds;
#endif
				}
			}

			private bool HasSoftInput() => _insetMask.HasFlag(InsetMask.SoftInput);

			private void UpdateInsets()
			{
				if (XamlWindow.Current?.Content == null)
				{
					return;
				}

				if (!AreBoundsAspectRatiosConsistent)
				{
					return;
				}

				if (Owner is null || !Owner.IsLoaded || (Owner.ActualWidth == 0d && Owner.ActualHeight == 0d))
				{
					return;
				}

				Thickness visibilityPadding;

				UpdateSafeAreaOverride();

				var windowPadding = GetWindowInsets(_overriddenVisibleBounds, HasSoftInput());

				if (windowPadding != default)
				{
					// If the owner view is scrollable, the visibility of interest is that of the scroll viewport.
					var fixedControl = _scrollAncestor ?? Owner;

					// Using relativeTo: null instead of Window.Current.Content since there are cases when the current UIElement
					// may be outside the bounds of the current Window content, for example, when the element is hosted in a modal window.
					var controlBounds = GetRelativeBounds(fixedControl, relativeTo: null);

					visibilityPadding = CalculateVisibilityInsets(OffsetVisibleBounds, controlBounds);

					if (_scrollAncestor is { } scrollViewer)
					{
						visibilityPadding = AdjustScrollableInsets(visibilityPadding, scrollViewer);
					}
				}
				else
				{
					visibilityPadding = default(Thickness);
				}

				var padding = CalculateAppliedInsets(_insetMask, visibilityPadding);

				ApplyInsets(padding);
			}

			/// <summary>
			/// Calculate the padding required to keep the view entirely within the 'safe' visible bounds of the XamlWindow.
			/// </summary>
			/// <param name="visibleBounds">The safe visible bounds of the XamlWindow.</param>
			/// <param name="controlBounds">The bounds of the control, in the window's coordinates.</param>
			private Thickness CalculateVisibilityInsets(Rect visibleBounds, Rect controlBounds)
			{
				var windowPadding = GetWindowInsets(_overriddenVisibleBounds, HasSoftInput());
				var appliedMargin = _insetMode == InsetMode.Margin ? _appliedMargin : new Thickness(0);

				var left = Math.Min(visibleBounds.Left - (controlBounds.Left - appliedMargin.Left), windowPadding.Left);
				var top = Math.Min(visibleBounds.Top - (controlBounds.Top - appliedMargin.Top), windowPadding.Top);
				var right = Math.Min((controlBounds.Right + appliedMargin.Right) - visibleBounds.Right, windowPadding.Right);
				var bottom = Math.Min((controlBounds.Bottom + appliedMargin.Bottom) - visibleBounds.Bottom, windowPadding.Bottom);

				return new Thickness
				{
					Left = left,
					Top = top,
					Right = right,
					Bottom = bottom
				};
			}

			/// <summary>
			/// Apply adjustments when target view is inside of a ScrollViewer.
			/// </summary>
			private Thickness AdjustScrollableInsets(Thickness visibilityInsets, ScrollViewer scrollAncestor)
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
					var appliedMargin = _insetMode == InsetMode.Margin ? _appliedMargin : new Thickness(0);

					if (rootBounds.Equals(controlBounds))
					{
						return visibilityInsets;
					}

					// Adjust for existing spacing
					visibilityInsets.Left -= Math.Max(controlBounds.Left - appliedMargin.Left, 0) - rootBounds.Left;
					visibilityInsets.Top -= Math.Max(controlBounds.Top - appliedMargin.Top, 0) - rootBounds.Top;
					visibilityInsets.Right -= rootBounds.Right - (controlBounds.Right + appliedMargin.Right);
					visibilityInsets.Bottom -= rootBounds.Bottom - (controlBounds.Bottom + appliedMargin.Bottom);
				}

				return visibilityInsets;
			}

			/// <summary>
			/// Calculate the insets to apply to the view, based on the selected InsetMask.
			/// </summary>
			/// <param name="mask">The InsetMask settings.</param>
			/// <param name="visibilityInsets">The insets required to keep the view entirely within the 'safe' visible bounds of the XamlWindow.</param>
			/// <returns>The insets that will actually be set on the view.</returns>
			private Thickness CalculateAppliedInsets(InsetMask mask, Thickness visibilityInsets)
			{
				var originalInsets = GetOriginalInsets();
				var hasSoftInput = HasSoftInput();
				var inputRect = hasSoftInput ? InputPane.GetForCurrentView()?.OccludedRect ?? Rect.Empty : Rect.Empty;

				// Apply left inset if the InsetMask is "left" or "all"
				var left = mask.HasFlag(InsetMask.Left)
					? Math.Max(originalInsets.Left, visibilityInsets.Left)
					: originalInsets.Left;
				// Apply top inset if the InsetMask is "top" or "all"
				var top = mask.HasFlag(InsetMask.Top)
					? Math.Max(originalInsets.Top, visibilityInsets.Top)
					: originalInsets.Top;
				// Apply right inset if the InsetMask is "right" or "all"
				var right = mask.HasFlag(InsetMask.Right)
					? Math.Max(originalInsets.Right, visibilityInsets.Right)
					: originalInsets.Right;
				// Apply bottom inset if the InsetMask is "bottom" or "all"

				var bottom = mask.HasFlag(InsetMask.Bottom) || (hasSoftInput && inputRect.Height > 0)
					? Math.Max(originalInsets.Bottom, visibilityInsets.Bottom)
					: originalInsets.Bottom;

				return new Thickness
				{
					Left = left,
					Top = top,
					Right = right,
					Bottom = bottom
				};
			}

			private Thickness GetOriginalInsets()
			{
				return _insetMode switch
				{
					InsetMode.Margin => _originalMargin,
					InsetMode.Padding or _ => _originalPadding
				};
			}

			private void ApplyInsets(Thickness insets)
			{
				if (Owner is not { } owner)
				{
					return;
				}
				if (_insetMode == InsetMode.Padding &&
					owner.TryUpdatePadding(insets))
				{
					_appliedPadding = insets;
					OnInsetsApplied(owner, insets);
				}
				else if (_insetMode == InsetMode.Margin)
				{
					if (!owner.Margin.Equals(insets))
					{
						_appliedMargin = insets;
						owner.Margin = insets;
						OnInsetsApplied(owner, insets);
					}
				}
			}

			private void OnInsetsApplied(FrameworkElement owner, Thickness newInsets)
			{
#if __ANDROID__
				// Dispatching on Android prevents issues where layout/render changes occurring
				// during the initial loading of the view are not always properly picked up by the layouting/rendering engine.
				owner.GetDispatcherCompat().Schedule(()=> owner.InvalidateMeasure());
#endif
				if (_log.IsEnabled(LogLevel.Debug))
				{
					_log.LogDebug($"ApplyInsets={newInsets}, Mode={_insetMode}");
				}
			}

			internal static SafeAreaDetails GetInstance(FrameworkElement element)
				=> _instances.GetValue(element, (ConditionalWeakTable<FrameworkElement, SafeAreaDetails>.CreateValueCallback)(e => (SafeAreaDetails)new SafeArea.SafeAreaDetails(e)));

			internal void OnInsetsChanged(InsetMask oldValue, InsetMask newValue)
			{
				_insetMask = newValue;

				VerifySoftInputUsage();

				UpdateInsets();
			}

			private void VerifySoftInputUsage()
			{
				if (HasSoftInput() && Owner is { } owner && (owner is not SafeArea or ScrollViewer))
				{
					_log.WarnIfEnabled(() => $"The '{nameof(InsetMask.SoftInput)}' mask is only supported on {nameof(SafeArea)} or ScrollViewer.");
				}
			}

			private ScrollViewer? GetScrollAncestor()
			{
				return Owner?.FindFirstParent<ScrollViewer>();
			}

			private static Rect GetRelativeBounds(FrameworkElement boundsOf, UIElement? relativeTo)
			{
				return boundsOf
					.TransformToVisual(relativeTo)
					.TransformBounds(new Rect(0, 0, boundsOf.ActualWidth, boundsOf.ActualHeight));
			}

			internal void OnSafeAreaOverrideChanged(Thickness? oldValue, Thickness? newValue)
			{
				UpdateInsets();
			}

			internal void UpdateSafeAreaOverride()
			{
				if (Owner is { } owner)
				{
					var safeAreaOverride = GetSafeAreaOverride(owner);

					if (safeAreaOverride is not { } unsafeArea)
					{
						_overriddenVisibleBounds = null;
						return;
					}

					var fullBounds = XamlWindow.Current!.Bounds;
					var height = fullBounds.Height - unsafeArea.Top - unsafeArea.Bottom;
					var width = fullBounds.Width - unsafeArea.Left - unsafeArea.Right;

					_overriddenVisibleBounds = new Rect(fullBounds.X + unsafeArea.Left, fullBounds.Y + unsafeArea.Top, width, height);
				}
			}

			internal void OnInsetModeChanged(InsetMode oldValue, InsetMode newValue)
			{
				_insetMode = newValue;

				if (Owner is { } owner)
				{
						if (oldValue == InsetMode.Margin)
						{
							_appliedMargin = new Thickness(0);
							owner.Margin = _originalMargin;
							OnInsetsApplied(owner, _originalMargin);
						}
						else if (oldValue == InsetMode.Padding)
						{
							_appliedPadding = new Thickness(0);
							PaddingHelper.SetPadding(owner, _originalPadding);
							OnInsetsApplied(owner, _originalPadding);
						}
				}
				UpdateInsets();
			}
		}
	}
}
