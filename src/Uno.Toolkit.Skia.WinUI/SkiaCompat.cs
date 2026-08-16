#nullable enable

using System;
using System.Runtime.CompilerServices;
using SkiaSharp;
using static SkiaSharp.SKImageFilter;

namespace Uno.Toolkit.Skia.WinUI;

internal static class SkiaCompat
{
	private static readonly bool _isSkiaSharp3OrLater = typeof(SKPaint).Assembly.GetName().Version?.Major >= 3;

	internal static SKImageFilter SKImageFilter_CreateBlur(float sigmaX, float sigmaY)
	{
		if (!_isSkiaSharp3OrLater)
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			SKImageFilter Legacy() => SKImageFilter.CreateBlur(sigmaX, sigmaY);
			return Legacy();
		}

		return SKImageFilter_CreateBlur(null, sigmaX, sigmaY);
	}

	[UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "CreateBlur")]
	private static extern SKImageFilter SKImageFilter_CreateBlur(SKImageFilter? _, float sigmaX, float sigmaY);
}
