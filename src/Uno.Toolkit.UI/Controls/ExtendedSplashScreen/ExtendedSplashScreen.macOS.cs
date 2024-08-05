#if !__ANDROID__ && !__IOS__ && !(WINDOWS || WINDOWS_UWP) && !__SKIA_OR_WASM__

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif
using System.Threading.Tasks;

namespace Uno.Toolkit.UI;

public partial class ExtendedSplashScreen
{
	internal static Task<FrameworkElement?> GetNativeSplashScreen()
	{
		return Task.FromResult<FrameworkElement?>(null);
	}

	public bool SplashIsEnabled => (Platforms & SplashScreenPlatform.All) != 0;
}
#endif
