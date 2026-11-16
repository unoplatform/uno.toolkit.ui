using System;

namespace Uno.Toolkit.UI;

/// <summary>
/// Platforms on which the <see cref="ExtendedSplashScreen"/> displays the application's splash screen.
/// </summary>
/// <remarks>
/// Every target renders with Skia on Uno Platform 7: the values name the platform the app runs on, not its renderer.
/// </remarks>
[Flags]
public enum SplashScreenPlatform
{
	None,
	/// <summary>Android apps.</summary>
	Android = 1,
	/// <summary>iOS apps.</summary>
	iOS = Android << 1,
	/// <summary>Windows App SDK apps.</summary>
	Windows = iOS << 1,
	/// <summary>WebAssembly apps.</summary>
	WebAssembly = Windows << 1,
	/// <summary>Desktop apps (Windows, macOS and Linux).</summary>
	Skia = WebAssembly << 1,
	All = Android | iOS | Windows | WebAssembly | Skia
}
