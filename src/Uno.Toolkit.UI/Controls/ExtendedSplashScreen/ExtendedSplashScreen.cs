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
	public Window? Window { get; set; }

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

		_ = LoadNativeSplashScreen();

	}

	private async Task LoadNativeSplashScreen()
	{
		var splashScreenContent = await GetNativeSplashScreen(SplashScreen);

		// Return a non-visible element to make sure some content is set on the ContentPresenter
		// Setting null on WinUI throws an exception
		SplashScreenContent = splashScreenContent ?? new TextBlock { Text = "" };
	}


#if !__ANDROID__ && !__IOS__ && !(WINDOWS || WINDOWS_UWP)
	private async Task<FrameworkElement?> GetNativeSplashScreen(SplashScreen? splashScreen)
	{
		return default;
	}
#endif
}
