#if HAS_UNO
using System;
using System.Collections.Generic;
using Uno.Disposables;

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

namespace Uno.Toolkit.UI
{
	internal static class PropertyChangedExtensions
	{
		public static IDisposable SubscribeNestedPropertyChangedCallback(this AppBarButton? appBarButton, PropertyChangedCallback callback)
		{
			if (appBarButton == null)
			{
				return Disposable.Empty;
			}

			var disposable = new CompositeDisposable();

			appBarButton
				.RegisterDisposableNestedPropertyChangedCallback(
					callback,
					new[] { AppBarButton.LabelProperty },
					new[] { AppBarButton.IconProperty },
					new[] { AppBarButton.ContentProperty },
					new[] { AppBarButton.ContentProperty, FrameworkElement.VisibilityProperty },
					new[] { AppBarButton.OpacityProperty },
					new[] { AppBarButton.ForegroundProperty },
					new[] { AppBarButton.VisibilityProperty },
					new[] { AppBarButton.IsEnabledProperty },
					new[] { AppBarButton.IsInOverflowProperty }
			   ).DisposeWith(disposable);

			(appBarButton.Icon as BitmapIcon)
				.SubscribeNestedPropertyChangedCallback(callback)
				.DisposeWith(disposable);

			(appBarButton.Foreground as SolidColorBrush)
				.SubscribeNestedPropertyChangedCallback(callback)
				.DisposeWith(disposable);

			return disposable;
		}

		public static IDisposable SubscribeNestedPropertyChangedCallback(this SolidColorBrush? scb, PropertyChangedCallback callback)
		{
			if (scb == null)
			{
				return Disposable.Empty;
			}

			var disposable = new CompositeDisposable();

			scb.RegisterDisposableNestedPropertyChangedCallback(
				callback,
				new[] { SolidColorBrush.ColorProperty },
				new[] { SolidColorBrush.OpacityProperty }
			).DisposeWith(disposable);

			return disposable;
		}

		public static IDisposable SubscribeNestedPropertyChangedCallback(this BitmapIcon? bmp, PropertyChangedCallback callback)
		{
			if (bmp == null)
			{
				return Disposable.Empty;
			}

			var disposable = new CompositeDisposable();

			bmp.RegisterDisposableNestedPropertyChangedCallback(
				callback,
				new[] { BitmapIcon.UriSourceProperty },
				new[] { BitmapIcon.ForegroundProperty },
				new[] { BitmapIcon.ShowAsMonochromeProperty }
			).DisposeWith(disposable);

			return disposable;
		}
	}
}
#endif
