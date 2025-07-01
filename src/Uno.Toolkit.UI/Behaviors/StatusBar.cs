using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.ViewManagement;
using Uno.Disposables;
using System.Diagnostics.CodeAnalysis;
using Windows.Foundation.Metadata;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;

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

#if HAS_UNO
using XamlStatusBar = Windows.UI.ViewManagement.StatusBar;
#endif

using XamlColor = Windows.UI.Color;

namespace Uno.Toolkit.UI
{
	public static partial class StatusBar
	{
		private static readonly HashSet<Frame> _frames = new();
		private static readonly List<Page> _pages = new();
		private static IDisposable? _colorValueChangedSubscription;
		private static UISettings _uiSettings = new();
		private static Page? _lastActivePage;
		private static ElementTheme? _lastAppliedTheme;

		private static readonly ILogger _logger = typeof(Uno.Toolkit.UI.StatusBar).Log();

		#region DependencyProperty: Foreground

		public static DependencyProperty ForegroundProperty { [DynamicDependency(nameof(GetForeground))] get; } = DependencyProperty.RegisterAttached(
			"Foreground",
			typeof(StatusBarForegroundTheme),
			typeof(StatusBar),
			new PropertyMetadata(default(StatusBarForegroundTheme), OnForegroundChanged));

		[DynamicDependency(nameof(SetForeground))]
		public static StatusBarForegroundTheme GetForeground(Page obj) => (StatusBarForegroundTheme)obj.GetValue(ForegroundProperty);
		/// <summary>
		/// Sets the foreground color for the text and icons on the status bar.
		/// </summary>
		[DynamicDependency(nameof(GetForeground))]
		public static void SetForeground(Page obj, StatusBarForegroundTheme value) => obj.SetValue(ForegroundProperty, value);

		#endregion
		#region DependencyProperty: Background

		public static DependencyProperty BackgroundProperty { [DynamicDependency(nameof(GetBackground))] get; } = DependencyProperty.RegisterAttached(
			"Background",
			typeof(Brush),
			typeof(StatusBar),
			new PropertyMetadata(default(Brush), OnBackgroundChanged));

		[DynamicDependency(nameof(SetBackground))]
		public static Brush GetBackground(Page obj) => (Brush)obj.GetValue(BackgroundProperty);
		/// <summary>
		/// Sets the background color for the status bar.
		/// </summary>
		/// <remarks>
		/// Due to platform limitations, only <see cref="SolidColorBrush"/>es are accepted.
		/// </remarks>
		[DynamicDependency(nameof(GetBackground))]
		public static void SetBackground(Page obj, Brush value) => obj.SetValue(BackgroundProperty, value);

		#endregion
		#region DependencyProperty: Subscription (private)

		private static DependencyProperty SubscriptionProperty { [DynamicDependency(nameof(GetSubscription))] get; } = DependencyProperty.RegisterAttached(
			"Subscription",
			typeof(IDisposable),
			typeof(StatusBar),
			new PropertyMetadata(default(IDisposable?)));

		[DynamicDependency(nameof(SetSubscription))]
		private static IDisposable? GetSubscription(Page obj) => (IDisposable?)obj.GetValue(SubscriptionProperty);
		[DynamicDependency(nameof(GetSubscription))]
		private static void SetSubscription(Page obj, IDisposable? value) => obj.SetValue(SubscriptionProperty, value);

		#endregion

		private static void OnForegroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => OnForeOrBackgroundChanged(sender, e);
		private static void OnBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => OnForeOrBackgroundChanged(sender, e);
		private static void OnForeOrBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not Page page) return;

			if ((GetForeground(page) is { } theme && theme != StatusBarForegroundTheme.None) ||
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
			if (GetForeground(page) is { } theme && theme != StatusBarForegroundTheme.None) SetForegroundCore(GetForegroundValue(page, theme));
			if (GetBackground(page) is SolidColorBrush brush) SetBackgroundCore(brush.Color);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetForeground(StatusBarForegroundTheme value)
		{
			var color = value switch
			{
				StatusBarForegroundTheme.Light => Colors.White,
				StatusBarForegroundTheme.Dark => Colors.Black,

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

		private static void SetForegroundCore(XamlColor value)
		{
			if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
			{
#if HAS_UNO
				XamlStatusBar.GetForCurrentView().ForegroundColor = value;
#endif
				return;
			}

			_logger.WarnIfEnabled(() => $"SetForeground: {value}; Not supported on this platform");
		}

		private static void SetBackgroundCore(XamlColor value)
		{

			if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
			{
#if HAS_UNO
				XamlStatusBar.GetForCurrentView().BackgroundColor = value;
#endif
				return;
			}

			_logger.WarnIfEnabled(() => $"SetBackground: {value}; Not supported on this platform");
		}

		private static XamlColor GetForegroundValue(Page page, StatusBarForegroundTheme theme)
		{
			var pageTheme = page.ActualTheme == ElementTheme.Default
				? GetSystemTheme()
				: page.ActualTheme;
			var light = (pageTheme, theme) switch
			{
				(_, StatusBarForegroundTheme.Light) => true,
				(_, StatusBarForegroundTheme.Dark) => false,
				(ElementTheme.Light, StatusBarForegroundTheme.Auto) => false,
				(ElementTheme.Light, StatusBarForegroundTheme.AutoInverse) => true,
				(ElementTheme.Dark, StatusBarForegroundTheme.Auto) => true,
				(ElementTheme.Dark, StatusBarForegroundTheme.AutoInverse) => false,

				_ => false,
			};

			return light ? Colors.White : Colors.Black;
		}

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
#if !HAS_UNO
				page.GetDispatcherCompat().Schedule(() => {
#endif
					if (GetSubscription(page) != null &&
						GetForeground(page) is StatusBarForegroundTheme.Auto or StatusBarForegroundTheme.AutoInverse &&
						GetSystemTheme() is var theme && theme != _lastAppliedTheme)
						{
							// this will prevent deadlock, as setting the XamlStatusBar.Foreground will trigger ColorValuesChanged
							_lastAppliedTheme = theme;

							UpdateStatusBar(page);
						}
#if !HAS_UNO
				});
#endif
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
