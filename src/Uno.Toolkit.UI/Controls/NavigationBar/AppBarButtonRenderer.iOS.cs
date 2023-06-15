#if __IOS__
using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Uno.Disposables;
using Uno.Extensions;
using Uno.Logging;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using System.Runtime.InteropServices;

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
	internal class AppBarButtonRenderer : Renderer<AppBarButton, UIBarButtonItem>
	{
		private AppBarButtonWrapper? _appBarButtonWrapper;

		public AppBarButtonRenderer(AppBarButton element) : base(element)
		{
		}

		private bool HasContent => Element?.Content is FrameworkElement;

		protected override UIBarButtonItem CreateNativeInstance() => new UIBarButtonItem();

		protected override IEnumerable<IDisposable> Initialize()
		{
			// Content
			_appBarButtonWrapper = new AppBarButtonWrapper();
			var element = Element;
			if (element == null)
			{
				yield break;
			}

			if (element.Content is FrameworkElement content && content.Visibility == Visibility.Visible)
			{
				_appBarButtonWrapper.SetParent(element.Parent);
				_appBarButtonWrapper.ParentChanged += OnAppBarButtonWrapperParentChanged;

				yield return Disposable.Create(() =>
				{
					_appBarButtonWrapper.ParentChanged -= OnAppBarButtonWrapperParentChanged;
				});
			}

			yield return Disposable.Create(() =>
			{
				_appBarButtonWrapper = null;
			});

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

			Native.Clicked += OnNativeClicked;
			yield return Disposable.Create(() => { Native.Clicked -= OnNativeClicked; });
		}

		private void OnAppBarButtonWrapperParentChanged(object? sender, EventArgs e)
		{
			var element = Element ?? throw new InvalidOperationException("Element is null.");
			// Even though we set the NavigationBar as the parent of the _appBarButtonWrapper,
			// it will change to the native control when the view is added.
			// This control is the visual parent but is not a DependencyObject and will not propagate the DataContext.
			// In order to ensure the DataContext is propagated properly, we restore the NavigationBar
			// parent that can propagate the DataContext.
			if (!ReferenceEquals(_appBarButtonWrapper?.Parent, element.Parent))
			{
				_appBarButtonWrapper?.SetParent(element.Parent);
			}
		}

		protected override void Render()
		{
			if (_appBarButtonWrapper == null)
			{
				return; // not initialized
			}
			// Icon & Content
			var native = Native;
			var element = Element ?? throw new InvalidOperationException("Element is null.");

			native.Image = null;
			native.ClearCustomView();
			native.Title = element.Content is string content ? content : element.Label;
			if (element.Icon != null)
			{
				switch (element.Icon)
				{
					case BitmapIcon bitmap:
						native.Image = ImageHelper.FromUri(bitmap.UriSource);
						native.ClearCustomView();
						break;

					case FontIcon font: // not supported
					case PathIcon path: // not supported
					case SymbolIcon symbol: // not supported
					default:
						this.Log().WarnIfEnabled(() => $"{GetType().Name ?? "FontIcon, PathIcon and SymbolIcon"} are not supported. Use BitmapIcon instead with UriSource.");
						native.Image = null;
						native.ClearCustomView();
						// iOS doesn't add the UIBarButtonItem to the native logical tree unless it has an Image or Title set. 
						// We default to an empty string to ensure it is added.
						native.Title ??= string.Empty;
						break;
				}
			}
			else if (element.Content is FrameworkElement fe)
			{
				var currentParent = element.Parent;
				_appBarButtonWrapper.Child = element;

				//Restore the original parent if any, as we
				//want the DataContext to flow properly from the
				//NavigationBar.
				element.SetParent(currentParent);
				native.Image = null;
				native.CustomView = fe.Visibility == Visibility.Visible ? _appBarButtonWrapper : null;
				// iOS doesn't add the UIBarButtonItem to the native logical tree unless it has an Image or Title set.
				// We default to an empty string to ensure it is added, in order to support late-bound Content.
				native.Title ??= string.Empty;
			}

			// Foreground
			if (ColorHelper.TryGetColorWithOpacity(element.Foreground, out var foreground))
			{
				var color = (UIColor)foreground;
				native.TintColor = color.ColorWithAlpha((NFloat)element.Opacity);
			}
			else
			{
				native.TintColor = default(UIColor); // TODO .Clear;
			}

			// IsEnabled
			native.Enabled = element.IsEnabled;

			// Background
			if (ColorHelper.TryGetColorWithOpacity(element.Background, out var backgroundColor))
			{
				if (HasContent)
				{
					// Setup the background color when a custom content is set.
					_appBarButtonWrapper.BackgroundColor = backgroundColor;
				}
				else
				{
					var backgroundImage = ColorHelper.IsTransparent(backgroundColor)
						? new UIImage() // Clears the background
						: ((UIColor)backgroundColor).ToUIImage(); // Applies the solid color;

					// We're using SetBackgroundImage instead of SetBackgroundColor 
					// because it extends all the way up under the status bar.
					native.SetBackgroundImage(backgroundImage, UIControlState.Normal, UIBarMetrics.Default);
				}
			}
		}

		private void OnNativeClicked(object? sender, EventArgs e)
		{
			Element?.RaiseClick();
		}
	}

	/// <summary>
	/// Used to correctly lay out the AppBarButton in the UIBarButtonItem
	/// </summary>
	internal partial class AppBarButtonWrapper : Border
	{
		internal event EventHandler? ParentChanged;

		public AppBarButtonWrapper()
		{
		}

		// Even though we set the Navigation as the parent of the AppBarButtonWrapper,
		// it will change to the native control when the view is added (once MovedToSuperview is called).
		// This native control is the visual parent but is not a DependencyObject and will not propagate the DataContext.
		// In order to ensure the DataContext is propagated properly, we need to notify the renderer that this change has occured
		// so we can restore the NavigationBar parent that can propagate the DataContext.
		public override void MovedToSuperview()
		{
			base.MovedToSuperview();

			ParentChanged?.Invoke(this, EventArgs.Empty);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			// Giving it full available space so that its child can properly be measured and positioned after
			availableSize = new Size(double.PositiveInfinity, 44);

			// The frame needs to be explicitly set in order to render the CustomView of the UIBarButtonItem.
			var childSize = base.MeasureOverride(availableSize);

			if (!double.IsNaN(childSize.Width)
				&& !double.IsNaN(childSize.Height)
				&& childSize != default(Size)
				&& (Frame.Width != childSize.Width
					|| Frame.Height != childSize.Height)
				)
			{
				Frame = new CGRect(Frame.X, Frame.Y, childSize.Width, childSize.Height);
				// Request layout since previous (enormous) Frame may have squeezed out navigation bar title
				SetNeedsLayout();
			}

			return childSize;
		}
	}
}
#endif
