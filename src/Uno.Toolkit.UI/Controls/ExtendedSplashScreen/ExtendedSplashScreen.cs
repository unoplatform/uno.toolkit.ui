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
[TemplatePart(Name = SplashScreenPresenterPartName, Type = typeof(ContentPresenter))]
public partial class ExtendedSplashScreen : LoadingView
{
	private const string SplashScreenPresenterPartName = "SplashScreenPresenter";

	public SplashScreen? SplashScreen { get; set; }

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		if(SplashScreen is null)
		{
			this.Log().LogWarning("SplashScreen property has not been set (typically done in the OnLaunched Application method)");
			return;
		}

		if (GetTemplateChild(SplashScreenPresenterPartName) is ContentPresenter splashScreenPresenter)
		{
			splashScreenPresenter.Content = GetNativeSplashScreen(SplashScreen);
		}
		else
		{
			this.Log().LogWarning($"Template for {nameof(ExtendedSplashScreen)} doesn't contain {nameof(ContentPresenter)} with Name set to {SplashScreenPresenterPartName}");
		}

	}


#if !__ANDROID__ && !__IOS__ && !(WINDOWS || WINDOWS_UWP)
	private FrameworkElement? GetNativeSplashScreen(SplashScreen splashScreen)
	{
		// ExtendedSplashscreen is not implemented on WASM.
		return default;
	}
#endif
}
