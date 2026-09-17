#if __ANDROID__
using Android.OS;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Activity = Android.App.Activity;
using AndroidSplashScreen = AndroidX.Core.SplashScreen.SplashScreen;

namespace Uno.Toolkit.UI;

public partial class ExtendedSplashScreen
{
	/// <summary>
	/// Installs the AndroidX splash screen on the provided <see cref="Activity"/>.<br/>
	/// The operating system splash screen then stays visible until the app renders its first frame,
	/// at which point the <see cref="ExtendedSplashScreen"/> takes over.
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
	/// <remarks>
	/// This is equivalent to calling <c>AndroidX.Core.SplashScreen.SplashScreen.InstallSplashScreen</c>, except that it also
	/// works on Android 11 and earlier when the activity theme does not derive from <c>Theme.SplashScreen</c>.
	/// </remarks>
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

		AndroidSplashScreen.InstallSplashScreen(activity);
	}
}
#endif
