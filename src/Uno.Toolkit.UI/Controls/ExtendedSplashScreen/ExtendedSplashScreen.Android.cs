#if __ANDROID__
using Android.App;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Uno.Extensions;
using Uno.Logging;
using Uno.UI;
using Windows.Foundation;
using System.Runtime.InteropServices;
using Android.OS;
using Android.Graphics;
using View = Android.Views.View;
using Color = Android.Graphics.Color;
using Android.Widget;
using AndroidX.Core.View;
using Android.Views;
using Android.Util;
using Size = Windows.Foundation.Size;
using Activity = Android.App.Activity;
using AndroidSplashScreen = AndroidX.Core.SplashScreen.SplashScreen;
using Android;
using Windows.Devices.Sensors;
using Android.Content.Res;

#if IS_WINUI
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Canvas = Microsoft.UI.Xaml.Controls.Canvas;
using Colors = Microsoft.UI.Colors;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Canvas = Windows.UI.Xaml.Controls.Canvas;
using Colors = Windows.UI.Colors;
#endif

namespace Uno.Toolkit.UI;

public partial class ExtendedSplashScreen
{
	private static AndroidSplashScreen? NativeSplashScreen;
	private static Size SplashSize = SizeExtensions.Zero;
	private static Bitmap? SplashBitmap;
	private static SystemUiConfig? OriginalSystemUi;

	public bool SplashIsEnabled => (Platforms & SplashScreenPlatform.Android) != 0;

	/// <summary>
	/// Initializes the native Android SplashScreen.<br/>
	/// Setting this changes the behavior of the ExtendedSplashScreen to use the AndroidX SplashScreen APIs (instead of the windowBackground).
	/// <example>
	/// <code>
	/// protected override void OnCreate(Bundle bundle)
	/// { 
	///	    // Handle the splash screen transition.
	///	    Uno.Toolkit.UI.ExtendedSplashScreen.Init(this);
	/// 
	///	    base.OnCreate(bundle);
	///	}
	///	</code>
	/// </example>
	/// </summary>
	public static void Init(Activity activity)
	{
		// Below API 31, the AndroidX SplashScreen implementation relies on a theme attribute (splashScreenIconSize) that is only part of the pre-built "Theme.SplashScreen" themes.
		// If the current Activity theme is not set to a SplashScreen theme, we apply it here to ensure the SplashScreen APIs work as expected
		// ref: https://developer.android.com/develop/ui/views/launch/splash-screen/migrate#migrate
		if (Build.VERSION.SdkInt < BuildVersionCodes.S)
		{
			// Apply the workaround SplashScreen theme attributes to the current Activity's theme,
			// skipping conflicting attributes that are already set (force = false)
			activity.Theme?.ApplyStyle(Resource.Style.ExtendedSplashScreenTheme, force: false);

			typeof(ExtendedSplashScreen).Log().LogDebug("Applying ExtendedSplashScreenTheme workaround");
		}

		var splashScreen = AndroidSplashScreen.InstallSplashScreen(activity);
		splashScreen.ExitAnimation += OnExitAnimation;

		NativeSplashScreen = splashScreen;

		SaveSystemUi();
		HideSystemUi();
	}

	partial void InitPartial()
	{
		if (NativeSplashScreen is not null)
		{
			Instance?.RegisterDisposablePropertyChangedCallback(SourceProperty, OnSourceChanged);
		}
	}

	private void OnSourceChanged(DependencyObject sender, DependencyProperty dp)
	{
		if (sender is ExtendedSplashScreen extended)
		{
			// We only care about IsExecuting transitioning from true -> false. We don't care if it was originally false or true so don't propagate initial value
			extended.Source?.BindIsExecuting(OnIsExecutingChanged, propagateInitialValue: false);
		}
	}

	private void OnIsExecutingChanged(bool isExecuting)
	{
		if (!isExecuting)
		{
			RestoreSystemUi();
		}
	}

	private async static void OnExitAnimation(object? sender, AndroidSplashScreen.ExitAnimationEventArgs e)
	{
		typeof(ExtendedSplashScreen).Log().LogInformation("Exiting the native Android SplashScreen, transitioning to the ExtendedSplashScreen.");

		var view = e.SplashScreenViewProvider.View;
		var splashSize = new Size(view.Width, view.Height).PhysicalToLogicalPixels();

		// It seems like the view can sometimes be invalid according to https://github.com/nventive/ExtendedSplashScreen/issues/76.
		if (splashSize.IsNotZero())
		{
			SplashSize = splashSize;
			SplashBitmap = GetBitmapFromView(view);
		}

		if (Instance is not null)
		{
			Instance.SplashScreenContent = await GetNativeSplashScreen() ?? new Border();
		}

		// Remove the native splash now that we've set the extended one.
		e.SplashScreenViewProvider.Remove();
	}


	private static void HideSystemUi()
	{
		// In order to avoid a flicker/jitter and visual content offset when transitioning from native to extended,
		// we ensure the layout of the ExtendedSplashScreen does not respect the window insets
		if (GetActivity()?.Window is { } window)
		{
			WindowCompat.SetDecorFitsSystemWindows(window, false);

			// On API 31+, the native splash displays as full-screen and underneath any display cutouts
			// Following code ensures the extended splash screen is displayed in the same way
			if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
			{
				if (window.Attributes is { } attr)
				{
					attr.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.Always;
					window.Attributes = attr;
				}

				var controller = WindowCompat.GetInsetsController(window, window.DecorView);
				controller.Hide(WindowInsetsCompat.Type.SystemBars() | WindowInsetsCompat.Type.DisplayCutout());
				controller.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
			}
		}
	}

	private static void RestoreSystemUi()
	{
		if (GetActivity()?.Window is { } window && OriginalSystemUi is { } systemUi)
		{
			WindowCompat.SetDecorFitsSystemWindows(window, systemUi.DecorFitsSystemWindows);

			if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
			{
				var controller = WindowCompat.GetInsetsController(window, window.DecorView);
				controller.Show(WindowInsetsCompat.Type.SystemBars());

				window.Attributes = systemUi.WindowAttributes;
				window.SetNavigationBarColor(systemUi.NavigationBarColor);
				window.SetStatusBarColor(systemUi.StatusBarColor);
			}
		}
	}

	private static void SaveSystemUi()
	{
		if (GetActivity()?.Window is { } window)
		{
			OriginalSystemUi = new
			(
				StatusBarColor: new Color(window.StatusBarColor),
				NavigationBarColor: new Color(window.NavigationBarColor),
				WindowAttributes: window.Attributes,
				DecorFitsSystemWindows: window.DecorView.FitsSystemWindows
			);
		}
	}

	private static Bitmap? GetBitmapFromView(View view)
	{
		var returnedBitmap = Bitmap.CreateBitmap(view.Width, view.Height, Bitmap.Config.Argb8888!);
		if (returnedBitmap is not null)
		{
			var canvas = new Android.Graphics.Canvas(returnedBitmap);

			var bgDrawable = view.Background;
			if (bgDrawable != null)
			{
				//has background drawable, then draw it on the canvas
				bgDrawable.Draw(canvas);
			}
			else
			{
				//does not have background drawable, then draw white background on the canvas
				canvas.DrawColor(Color.White);
			}

			view.Draw(canvas);
		}

		return returnedBitmap;
	}

	internal static Task<FrameworkElement?> GetNativeSplashScreen()
	{
		try
		{
			if (NativeSplashScreen is not null && SplashBitmap is null)
			{
				typeof(ExtendedSplashScreen).Log().LogInformation("NativeSplashScreen is not null. Waiting for native splashscreen animation exit to get the native view.");
				//SaveSystemUi();

				return Task.FromResult<FrameworkElement?>(null);
			}

			if (GetActivity() is not { } activity)
			{
				return Task.FromResult<FrameworkElement?>(null);
			}

			View? splashView = null;
			var splashScreenSize = GetSplashScreenSize(activity);

			if (SplashBitmap is not null)
			{
				var imageView = new ImageView(activity);
				imageView.SetImageBitmap(SplashBitmap);

				splashView = imageView;
			}

			// If we aren't building the extended splash view using the Android 12+ SplashScreen APIs, fallback to legacy windowBackground method
			if (splashView is null)
			{
				// AndroidX SplashScreen provides back compat until API 21
				if (NativeSplashScreen is null && Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
				{
					typeof(ExtendedSplashScreen).Log().LogWarning("Using obsolete implementation of ExtendedSplashScreen. It is now recommended to call Uno.Toolkit.UI.ExtendedSplashScreen.Init(); in the Android Activity.OnCreate override.");
				}

				// Get the theme's windowBackground (which we use as splash screen)
				var attribute = new TypedValue();
				activity?.Theme?.ResolveAttribute(Android.Resource.Attribute.WindowBackground, attribute, true);
				var windowBackgroundResourceId = attribute.ResourceId;

				// Create the splash screen surface
				splashView = new View(activity);
				splashView.SetBackgroundResource(attribute.ResourceId);
			}

			// We use a Canvas to ensure it's clipped but not resized (important when device has soft-keys)
			var element = new Canvas
			{
				// We set a background to prevent touches from going through
				Background = new SolidColorBrush(Colors.Transparent),
				// We use a Border to ensure proper layout
				Children =
				{
					new Border()
					{
						Width = splashScreenSize.Width,
						Height = splashScreenSize.Height,
						Child = VisualTreeHelper.AdaptNative(splashView),
					}
				},
			};

			return Task.FromResult<FrameworkElement?>(element);
		}
		catch (Exception e)
		{
			typeof(ExtendedSplashScreen).Log().LogError(0, e, "Error while getting native splash screen.");

			return Task.FromResult<FrameworkElement?>(null);
		}
	}

	private static Activity? GetActivity() => ContextHelper.Current as Activity;

	private static Size GetSplashScreenSize(Activity activity)
	{
		if (SplashSize.IsNotZero())
		{
			return SplashSize;
		}

		var physicalDisplaySize = new Android.Graphics.Point();
#pragma warning disable CS0618, CA1422 // Type or member is obsolete
		if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
		{
			// The windowBackground takes the size of the screen (only when using Theme.AppCompat.*)
			activity.WindowManager?.DefaultDisplay?.GetRealSize(physicalDisplaySize);
		}
		else
		{
			// The windowBackground takes the size of the screen minus the bottom navigation bar
			activity.WindowManager?.DefaultDisplay?.GetSize(physicalDisplaySize);
		}
#pragma warning restore CS0618, CA1422 // Type or member is obsolete

		return new Size(
			ViewHelper.PhysicalToLogicalPixels(physicalDisplaySize.X),
			ViewHelper.PhysicalToLogicalPixels(physicalDisplaySize.Y)
		);
	}

	record SystemUiConfig(Color StatusBarColor, Color NavigationBarColor, WindowManagerLayoutParams? WindowAttributes, bool DecorFitsSystemWindows) { }
}
#endif
