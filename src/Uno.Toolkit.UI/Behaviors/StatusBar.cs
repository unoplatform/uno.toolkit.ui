using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Windows.UI.ViewManagement;
using Uno.Disposables;

#if IS_WINUI
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif

using XamlColor = Windows.UI.Color;

namespace Uno.Toolkit.UI
{
	public enum StatusBarTheme { None, Light, Dark, Auto, AutoInverse }

	public static partial class StatusBar
	{
		#region DependencyProperty: ForegroundTheme

		public static DependencyProperty ForegroundThemeProperty { get; } = DependencyProperty.RegisterAttached(
			"ForegroundTheme",
			typeof(StatusBarTheme),
			typeof(StatusBar),
			new PropertyMetadata(default(StatusBarTheme), OnForegroundThemeChanged));

		public static StatusBarTheme GetForegroundTheme(Page obj) => (StatusBarTheme)obj.GetValue(ForegroundThemeProperty);
		public static void SetForegroundTheme(Page obj, StatusBarTheme value) => obj.SetValue(ForegroundThemeProperty, value);

		#endregion
		#region DependencyProperty: Background

		public static DependencyProperty BackgroundProperty { get; } = DependencyProperty.RegisterAttached(
			"Background",
			typeof(Brush),
			typeof(StatusBar),
			new PropertyMetadata(default(Brush), OnBackgroundChanged));

		public static Brush GetBackground(Page obj) => (Brush)obj.GetValue(BackgroundProperty);
		public static void SetBackground(Page obj, Brush value) => obj.SetValue(BackgroundProperty, value);

		#endregion
		#region DependencyProperty: Subscription (private)

		private static DependencyProperty SubscriptionProperty { get; } = DependencyProperty.RegisterAttached(
			"Subscription",
			typeof(IDisposable),
			typeof(StatusBar),
			new PropertyMetadata(default(IDisposable?)));

		private static IDisposable? GetSubscription(Page obj) => (IDisposable?)obj.GetValue(SubscriptionProperty);
		private static void SetSubscription(Page obj, IDisposable? value) => obj.SetValue(SubscriptionProperty, value);

		#endregion

		private static void OnForegroundThemeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => OnForeOrBackgroundChanged(sender, e);
		private static void OnBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => OnForeOrBackgroundChanged(sender, e);
		private static void OnForeOrBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not Page page) return;

			if ((GetForegroundTheme(page) is { } theme && theme != StatusBarTheme.None) ||
				(GetBackground(page) is SolidColorBrush brush))
			{
				// if either of fore/background is applicable, subscribe if not already.
				if (GetSubscription(page) is null)
				{
					SubscribeToSystemThemeIfNeeded();
					_pages.Add(page);
					page.Loaded += OnPageLoaded;
					page.ActualThemeChanged += OnPageThemeChanged;

					SetSubscription(page, Disposable.Create(() =>
					{
						_pages.Remove(page);
						page.Loaded -= OnPageLoaded;
						page.ActualThemeChanged -= OnPageThemeChanged;
					}));
				}
			}
			else
			{
				// if neither is, unsubscribe.
				GetSubscription(page)?.Dispose();
				SetSubscription(page, null);
			}

			if (page.IsLoaded)
			{
				_lastActivePage = page;
				UpdateStatusBar(page);
			}
		}

		private static void OnPageLoaded(object sender, RoutedEventArgs e)
		{
			if (sender is not Page page) return;

			_lastActivePage = page;
			UpdateStatusBar(page);

			if (page.Frame is { } frame && _frames.Add(frame))
			{
				frame.Navigated += OnFrameNavigated;
			}
		}

		private static void OnPageThemeChanged(FrameworkElement sender, object args)
		{
			if (sender == _lastActivePage)
			{
				UpdateStatusBar(_lastActivePage);
			}
		}

		private static void OnFrameNavigated(object sender, NavigationEventArgs e)
		{
			if (e.Content is not Page page) return;
			if (GetSubscription(page) is null) return;
			
			_lastActivePage = page;
			UpdateStatusBar(page);
		}

		private static void UpdateStatusBar(Page page)
		{
			if (GetForegroundTheme(page) is { } theme && theme != StatusBarTheme.None) SetForegroundCore(GetForegroundValue(page, theme));
			if (GetBackground(page) is SolidColorBrush brush) SetBackgroundCore(brush.Color);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetForegroundTheme(StatusBarTheme value)
		{
			var color = value switch
			{
				StatusBarTheme.Light => Colors.White,
				StatusBarTheme.Dark => Colors.Black,

				_ => (XamlColor?)null,
			};
			if (color is { } c)
			{
				SetForegroundCore(c);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetBackground(XamlColor value)
		{
			SetBackgroundCore(value);
		}

		static partial void SetForegroundCore(XamlColor value);
		static partial void SetBackgroundCore(XamlColor value);

		private static XamlColor GetForegroundValue(Page page, StatusBarTheme theme)
		{
			var pageTheme = page.ActualTheme == ElementTheme.Default
				? GetSystemTheme()
				: page.ActualTheme;
			var light = (pageTheme, theme) switch
			{
				(_, StatusBarTheme.Light) => true,
				(_, StatusBarTheme.Dark) => false,
				(ElementTheme.Light, StatusBarTheme.Auto) => false,
				(ElementTheme.Light, StatusBarTheme.AutoInverse) => true,
				(ElementTheme.Dark, StatusBarTheme.Auto) => true,
				(ElementTheme.Dark, StatusBarTheme.AutoInverse) => false,

				_ => false,
			};

			return light ? Colors.White : Colors.Black;
		}
	}

	public static partial class StatusBar
	{
		private static readonly HashSet<Frame> _frames = new();
		private static readonly List<Page> _pages = new();
		private static IDisposable? _colorValueChangedSubscription;
		private static UISettings _uiSettings = new();
		private static Page? _lastActivePage;
		private static ElementTheme? _lastAppliedTheme;

		private static void SubscribeToSystemThemeIfNeeded()
		{
			_colorValueChangedSubscription ??= Subscribe();

			IDisposable Subscribe()
			{
				_uiSettings.ColorValuesChanged += OnUISettingsColorValuesChanged;
				return Disposable.Create(() =>
					_uiSettings.ColorValuesChanged -= OnUISettingsColorValuesChanged
				);
			}
		}

		private static void OnUISettingsColorValuesChanged(UISettings sender, object args)
		{
			if (_lastActivePage is { } page)
			{
				if (GetSubscription(page) != null &&
					GetForegroundTheme(page) is StatusBarTheme.Auto or StatusBarTheme.AutoInverse &&
					GetSystemTheme() is var theme && theme != _lastAppliedTheme)
				{
					// this will prevent deadlock, as setting the XamlStatusBar.Foreground will trigger ColorValuesChanged
					_lastAppliedTheme = theme;

					UpdateStatusBar(page);
				}
			}
		}

		private static ElementTheme GetSystemTheme()
		{
			var background = _uiSettings.GetColorValue(UIColorType.Background);
			var result = background == Colors.Black ? ElementTheme.Dark : ElementTheme.Light;
			
			return result;
		}
	}
}
