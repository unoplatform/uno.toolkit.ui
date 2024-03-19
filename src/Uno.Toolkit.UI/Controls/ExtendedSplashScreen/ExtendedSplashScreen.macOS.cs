#if !__ANDROID__ && !__IOS__ && !(WINDOWS || WINDOWS_UWP) && !UNO_REFERENCE_API
using System.Threading.Tasks;

namespace Uno.Toolkit.UI;

public partial class ExtendedSplashScreen
{
	private static Task<FrameworkElement?> GetNativeSplashScreen()
	{
		return Task.FromResult<FrameworkElement?>(null);
	}

	public bool SplashIsEnabled => (Platforms & SplashScreenPlatform.All) != 0;
}
#endif
