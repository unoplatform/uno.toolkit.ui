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
	public SplashScreen? SplashScreen { get; set; }

	public
#if __IOS__ || __MACOS__ // hides UIView.Window and NSView.Window
	new
#endif
	Window? Window { get; set; }

	#region DependencyProperty: SplashScreenContent

	public static DependencyProperty SplashScreenContentProperty { get; } = DependencyProperty.Register(
		nameof(SplashScreenContent),
		typeof(object),
		typeof(ExtendedSplashScreen),
		new PropertyMetadata(default(object)));

	/// <summary>
	/// Gets or sets the native splash screen content to be displayed during loading/waiting.
	/// </summary>
	public object SplashScreenContent
	{
		get => (object)GetValue(SplashScreenContentProperty);
		set => SetValue(SplashScreenContentProperty, value);
	}

	#endregion

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


#if !__ANDROID__ && !__IOS__ && !(WINDOWS || WINDOWS_UWP)
	private static Task<FrameworkElement?> GetNativeSplashScreen(SplashScreen? splashScreen)
	{
		return Task.FromResult<FrameworkElement?>(null);
	}
#endif
}
