using System;

namespace Uno.Toolkit.UI;

[Flags]
public enum SplashScreenPlatform
{
	None,
	Android = 1,
	iOS = Android << 1,
	Windows = iOS << 1,
	WebAssembly = Windows << 1,
	Skia = WebAssembly << 1,
	All = Android | iOS | Windows | WebAssembly | Skia
}
