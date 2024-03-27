#if __ANDROID__
using Android.Views;
using AndroidX.AppCompat.Widget;
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
	internal class NavigationAppBarButtonRenderer : Renderer<AppBarButton, Toolbar>
	{
		private MainCommandMode? _mode;
		public MainCommandMode? Mode
		{
			get => _mode;
			set
			{
				_mode = value;
				Invalidate();
			}
		}

		public NavigationAppBarButtonRenderer(AppBarButton element) : base(element) { }

		protected override Toolbar CreateNativeInstance() => throw new NotSupportedException("The Native instance must be provided.");

		protected override IEnumerable<IDisposable> Initialize()
		{
			if (Element is { })
			{
				yield return Element.RegisterDisposableNestedPropertyChangedCallback(
					(s, e) => Invalidate(),
					new[] { AppBarButton.IconProperty },
					new[] { AppBarButton.IconProperty, BitmapIcon.UriSourceProperty },
					new[] { AppBarButton.VisibilityProperty },
					new[] { AppBarButton.OpacityProperty },
					new[] { AppBarButton.ForegroundProperty },
					new[] { AppBarButton.ForegroundProperty, SolidColorBrush.ColorProperty },
					new[] { AppBarButton.ForegroundProperty, SolidColorBrush.OpacityProperty },
					new[] { AppBarButton.LabelProperty }
				);
			}
		}

		protected override void Render()
		{
			var native = Native;
			var element = Element ?? throw new InvalidOperationException("Element is null.");

			if (native == null)
			{
				return;
			}

			// Foreground
			var hasIconColor = element.TryGetIconColor(out var foregroundColor);
			var foregroundOpacity = hasIconColor ? foregroundColor.A / 255d : 1.0;

			// Visibility
			if (element.Visibility == Visibility.Visible)
			{
				// Icon
				var iconUri = (element.Icon as BitmapIcon)?.UriSource;
				if (!TrySetIcon() && _mode == MainCommandMode.Back)
				{
					var navIcon = new AndroidX.AppCompat.Graphics.Drawable.DrawerArrowDrawable(ContextHelper.Current)
					{
						// 0 = menu icon
						// 1 = back icon
						Progress = 1,
					};

					if (hasIconColor)
					{
						navIcon.Color = (Android.Graphics.Color)foregroundColor;
					}

					native.NavigationIcon = navIcon;
				}

				// Label
				native.NavigationContentDescription = element.Label;

				// Opacity
				var opacity = element.Opacity;
				var finalOpacity = foregroundOpacity * opacity;
				var alpha = (int)(finalOpacity * 255);
				native.NavigationIcon?.SetAlpha(alpha);
			}
			else
			{
				native.NavigationIcon = null;
				native.NavigationContentDescription = null;
			}

			bool TrySetIcon()
			{
				if (element.Icon is not { } icon)
				{
					return false;
				}

				if (icon is BitmapIcon bitmap)
				{
					if (bitmap.UriSource is { } uriSource)
					{
						native.NavigationIcon = DrawableHelper.FromUri(uriSource);
						if (hasIconColor && native.NavigationIcon is { })
						{
							DrawableCompat.SetTint(iconDrawable, (Android.Graphics.Color)foregroundColor);
						}
					}

					return true;
				}
				else
				{
					this.Log().WarnIfEnabled(() => $"{icon.GetType().Name ?? "FontIcon, PathIcon and SymbolIcon"} are not supported. Use BitmapIcon instead with UriSource.");
				}

				return false;
			}
		}
	}
}
#endif
