﻿#if __ANDROID__
using Android.Text;
using Android.Text.Style;
using Android.Views;
using AndroidX.Core.Graphics.Drawable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Disposables;
using Uno.Extensions;
using Uno.Helpers;
using Uno.Logging;
using Uno.UI;
using Windows.Foundation;
using DrawableHelper = Uno.UI.DrawableHelper;
using static Uno.Toolkit.UI.VisualTreeHelperEx;
#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Automation.Peers;

#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif

namespace Uno.Toolkit.UI
{
	internal class AppBarButtonRenderer : Renderer<AppBarButton, IMenuItem>
	{
		private AppBarButtonWrapper? _appBarButtonWrapper;
		private bool _isInOverflow;
		private DependencyObject? _elementParent;

		public bool IsInOverflow
		{
			get => _isInOverflow;
			set
			{
				_isInOverflow = value;
				Invalidate();
			}
		}

		public AppBarButtonRenderer(AppBarButton element) : base(element)
		{
			element.ViewAttachedToWindow += OnElementAttachedToWindow;
		}

		private void OnElementAttachedToWindow(object? sender, View.ViewAttachedToWindowEventArgs e)
		{
			if (Element is { } element && element.Parent == _appBarButtonWrapper)
			{
				// if the new Parent is the wrapper, restore it to
				// its original value.
				if (_elementParent != null)
				{
					element.SetParent(_elementParent);
				}
			}
		}

		protected override IMenuItem CreateNativeInstance() => throw new NotSupportedException("Cannot create instance of IMenuItem.");

		protected override IEnumerable<IDisposable> Initialize()
		{
			// Content
			_appBarButtonWrapper = new AppBarButtonWrapper();
			if (Element?.Content is FrameworkElement content && content.Visibility == Visibility.Visible)
			{
				var elementsParent = Element.Parent;
				_appBarButtonWrapper.SetParent(elementsParent);

				yield return Disposable.Create(() =>
				{
					Element.ViewAttachedToWindow -= OnElementAttachedToWindow;
				});
			}

			yield return Disposable.Create(() => _appBarButtonWrapper = null);

			if (Element is { } element)
			{
				yield return element.RegisterDisposableNestedPropertyChangedCallback(
					(s, e) => Invalidate(),
					new[] { AppBarButton.LabelProperty },
					new[] { AppBarButton.IconProperty },
					new[] { AppBarButton.IconProperty, BitmapIcon.UriSourceProperty },
					new[] { AppBarButton.ContentProperty },
					new[] { AppBarButton.ContentProperty, FrameworkElement.VisibilityProperty },
					new[] { AppBarButton.OpacityProperty },
					new[] { AppBarButton.ForegroundProperty },
					new[] { AppBarButton.ForegroundProperty, SolidColorBrush.ColorProperty },
					new[] { AppBarButton.ForegroundProperty, SolidColorBrush.OpacityProperty },
					new[] { AppBarButton.VisibilityProperty },
					new[] { AppBarButton.IsEnabledProperty },
					new[] { AppBarButton.IsInOverflowProperty }
				);
			}
		}

		protected override void Render()
		{
			// NavigationBar::PrimaryCommands -> !IsInOverflow -> AsAction.Never -> displayed directly on navbar
			// NavigationBar::SecondaryCommands -> IsInOverflow -> AsAction.Awalys -> (displayed as flyout menu items under [...])

			if (Native is not { } native)
			{
				return;
			}

			var element = Element ?? throw new InvalidOperationException("Element is null.");

			// IsInOverflow
			var showAsAction = _isInOverflow
				? ShowAsAction.Never
				: ShowAsAction.Always;
			native.SetShowAsAction(showAsAction);

			string? titleText = element.Label;

			native.SetTitle(null);
			native.SetActionView(null);
			native.SetIcon(null);

			// (Icon ?? Content) and Label
			if (!TrySetIcon() && element.Content is { } content)
			{
				if (content is string text)
				{
					titleText = text;
				}
				else if (content is FrameworkElement { Visibility: Visibility.Visible } fe &&
					_appBarButtonWrapper is { } wrapper)
				{
					_elementParent = element.Parent;
					wrapper.Child = element;
					element.SetParent(_elementParent);
					native.SetActionView(wrapper);
				}
			}

			// IsEnabled
			native.SetEnabled(element.IsEnabled);
			// According to the Material Design guidelines, the opacity inactive icons should be:
			// - Light background: 38%
			// - Dark background: 50%
			// Source: https://material.io/guidelines/style/icons.html
			// For lack of a reliable way to identify whether the background is light or dark, 
			// we'll go with 50% opacity until this no longer satisfies projects requirements.
			var isEnabledOpacity = (element.IsEnabled ? 1.0 : 0.5);

			// Visibility
			native.SetVisible(element.Visibility == Visibility.Visible);

			// Foreground
			var foreground = element.Foreground as SolidColorBrush;
			var foregroundColor = foreground?.Color;
			var foregroundOpacity = 0d;

			if (native.Icon != null)
			{
				if (element.TryGetIconColor(out var iconColor))
				{
					foregroundOpacity = iconColor.A / 255d;
					DrawableCompat.SetTint(native.Icon, (Android.Graphics.Color)iconColor);
				}
				else
				{
					DrawableCompat.SetTintList(native.Icon, null);
				}
			}

			if (titleText != null)
			{
				if (foregroundColor != null)
				{
					var s = new SpannableString(titleText);
					s.SetSpan(new ForegroundColorSpan((Android.Graphics.Color)foregroundColor), 0, titleText.Length, 0);
					native.SetTitle(s);
				}
				else
				{
					native.SetTitle(titleText);
				}
			}

			// Background
			if (ColorHelper.TryGetColorWithOpacity(element.Background, out var backgroundColor))
			{
				_appBarButtonWrapper?.SetBackgroundColor((Android.Graphics.Color)backgroundColor);
			}

			// Opacity
			if (element != null)
			{
				var opacity = element.Opacity;
				var finalOpacity = isEnabledOpacity * foregroundOpacity * opacity;
				var alpha = (int)(finalOpacity * 255);
				native.Icon?.SetAlpha(alpha);
			}

			bool TrySetIcon()
			{
				if (element?.Icon is { } icon)
				{
					if (icon is BitmapIcon bitmap)
					{
						if (bitmap.UriSource is { } uriSource)
						{
							var drawable = DrawableHelper.FromUri(uriSource);
							native.SetIcon(drawable);
						}

						return true;
					}
					else
					{
						this.Log().WarnIfEnabled(() => $"{icon.GetType().Name ?? "FontIcon, PathIcon and SymbolIcon"} are not supported. Use BitmapIcon instead with UriSource.");
						native.SetIcon(null);
					}
				}

				return false;
			}
		}
		
	}

	internal partial class AppBarButtonWrapper : Border
	{
		// By default, the custom view of a MenuItem fills up the whole available area unless you 
		// explicitly collapse it by calling Native.CollapseActionView or calling SetShowAsAction with the extra flag
		// ShowAsAction.CollapseActionView. This is for instance the case of the search view used in a lot of scenarios.
		// To avoid this use case, we must explicitly set the size of the action view based on the real size of its content.
		// That being said, at some point in the future, we will need to support advanced scenarios where the AppBarButton needs to be expandable.
		private Size _measuredLogicalSize;

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

			var realSize = _measuredLogicalSize.LogicalToPhysicalPixels();

			this.SetMeasuredDimension((int)realSize.Width, (int)realSize.Height);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			_measuredLogicalSize = base.MeasureOverride(availableSize);

			return _measuredLogicalSize;
		}
	}
}
#endif
