using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Disposables;
using Uno.Extensions;
using Windows.ApplicationModel.Activation;
#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI;

/// <summary>
/// Displays a view that replicates the look and behavior of the native splash screen
/// </summary>
public partial class ExtendedSplashScreen : LoadingView
{
	#region DependencyProperty: Platforms
	public static DependencyProperty PlatformsProperty { get; } = DependencyProperty.Register(
		nameof(Platforms),
		typeof(SplashScreenPlatform),
		typeof(ExtendedSplashScreen),
		new PropertyMetadata(SplashScreenPlatform.All));

	/// <summary>
	/// Gets or sets the list of platforms where extended splash screen should be used.
	/// </summary>
	public SplashScreenPlatform Platforms
	{
		get => (SplashScreenPlatform)GetValue(PlatformsProperty);
		set => SetValue(PlatformsProperty, value);
	}
	#endregion

	public SplashScreen? SplashScreen { get; set; }

	#region DependencyProperty: SplashScreenContent
	internal static DependencyProperty SplashScreenContentProperty { get; } = DependencyProperty.Register(
		nameof(SplashScreenContent),
		typeof(object),
		typeof(ExtendedSplashScreen),
		new PropertyMetadata(default(object)));

	/// <summary>
	/// Gets or sets the native splash screen content to be displayed during loading/waiting.
	/// </summary>
	internal object SplashScreenContent
	{
		get => (object)GetValue(SplashScreenContentProperty);
		set => SetValue(SplashScreenContentProperty, value);
	}
	#endregion

	public
#if __IOS__ || __MACOS__ // hides UIView.Window and NSView.Window
	new
#endif
Window? Window
	{ get; set; }


	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		SplashScreenContent = new Border();

		_ = LoadNativeSplashScreen();
	}

	private async Task LoadNativeSplashScreen()
	{
		var splashScreenContent = await GetNativeSplashScreen(SplashScreen);

		if (splashScreenContent is not null)
		{
			// Return a non-visible element to make sure some content is set on the ContentPresenter
			// Setting null on WinUI throws an exception
			SplashScreenContent = splashScreenContent;
		}
	}


#if !__ANDROID__ && !__IOS__ && !(WINDOWS || WINDOWS_UWP) && !NETSTANDARD2_0
	private static Task<FrameworkElement?> GetNativeSplashScreen(SplashScreen? splashScreen)
	{
		return Task.FromResult<FrameworkElement?>(null);
	}
#endif
}
