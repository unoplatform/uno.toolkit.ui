using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Uno.Toolkit.Samples.Droid;
[Activity(
    MainLauncher = true,
    ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
    WindowSoftInputMode = SoftInput.AdjustNothing | SoftInput.StateHidden
)]
public class MainActivity : Microsoft.UI.Xaml.ApplicationActivity
{
#if USE_UITESTS
    [Export("NavBackFromNestedPage")]
	public static void NavBackFromNestedPage(string unused) => App.NavBackFromNestedPage();
    [Export("ForceNavigation")]
	public static void ForceNavigation(string sampleName) => App.ForceNavigation(sampleName);
    [Export("ExitNestedSample")]
    public static void ExitNestedSample(string unused) => App.ExitNestedSample();
    [Export("NavigateToNestedSample")]
	public static void NavigateToNestedSample(string pageName) => App.NavigateToNestedSample(pageName);
    [Export("GetDisplayScreenScaling")]
	public static string GetDisplayScreenScaling(string value) => App.GetDisplayScreenScaling(value);
#endif
}
