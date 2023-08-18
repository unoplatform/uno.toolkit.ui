#if false
// We keep that as a reference cause it would be better to use the hardware-accelerated version
#define ANDROID_REFERENTIAL_IMPL
#endif

using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;
using Windows.UI;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using Uno.Extensions;
using Uno.Logging;

#if __ANDROID__ && ANDROID_REFERENTIAL_IMPL
using _SKXamlCanvas = SkiaSharp.Views.Windows.SKSwapChainPanel;
using _SKPaintSurfaceEventArgs = SkiaSharp.Views.Windows.SKPaintGLSurfaceEventArgs;
#else
using _SKXamlCanvas = SkiaSharp.Views.Windows.SKXamlCanvas;
using _SKPaintSurfaceEventArgs = SkiaSharp.Views.Windows.SKPaintSurfaceEventArgs;
#endif

namespace Uno.Toolkit.UI;

public partial class ShadowContainer
{
	private static readonly ILogger _logger = typeof(ShadowContainer).Log();

	private static readonly ShadowsCache Cache = new ShadowsCache();

	private record ShadowInfos(double Width, double Height, bool IsInner, double BlurRadius, double Spread, double X, double Y, Color color)
	{
		public static readonly ShadowInfos Empty = new ShadowInfos(0, 0, false, 0, 0, 0, 0, new Color());

		public static ShadowInfos From(Shadow shadow, double width, double height)
		{
			return new ShadowInfos(
				width,
				height,
				shadow.IsInner,
				shadow.BlurRadius,
				shadow.Spread,
				shadow.OffsetX,
				shadow.OffsetY,
				Color.FromArgb((byte)(shadow.Color.A * shadow.Opacity), shadow.Color.R, shadow.Color.G, shadow.Color.B));
		}
	}

	private readonly record struct SKShadow(
		bool IsInner,
		float OffsetX,
		float OffsetY,
		float BlurSigma,
		SKColor Color,
		float Spread,
		float ContentWidth,
		float ContentHeight,
		float CornerRadius)
	{
		public static SKShadow From(Shadow shadow, float width, float height, float cornerRadius, float pixelRatio)
		{
			var blurRadius = (float)shadow.BlurRadius * pixelRatio;
			// Blur sigma conversion taken from flutter source code
			var blurSigma = blurRadius > 0 ? blurRadius * 0.57735f + 0.5f : 0f;

			// Can't use ToSKColor() or we end up with a weird compilation error asking us to reference System.Drawing
			Color windowsUiColor = shadow.Color;
			var color = ToSkiaColor(windowsUiColor);
			color = color.WithAlpha((byte)(color.Alpha * shadow.Opacity));

			return new SKShadow(
				shadow.IsInner,
				(float)shadow.OffsetX * pixelRatio,
				(float)shadow.OffsetY * pixelRatio,
				blurSigma,
				color,
				(float)shadow.Spread * pixelRatio,
				width,
				height,
				cornerRadius);
		}
	}

	private static bool NeedsPaint(ShadowPaintContext context, Shadow[] shadows, double width, double height, float pixelRatio, out bool pixelRatioChanged)
	{
		var states = new ShadowPaintState(width, height, pixelRatio, ShadowInfo.Snapshot(shadows));
		var needsPaint = states != context.LastPaintState;
		pixelRatioChanged = states.PixelRatio != context.LastPaintState?.PixelRatio;

		context.LastPaintState = states;
		return needsPaint;
	}

	private void OnPaintSurface(object? sender, _SKPaintSurfaceEventArgs e)
	{
		var context = sender switch
		{
			_SKXamlCanvas x when x == _backgroundPaintContext.ShadowHost => _backgroundPaintContext,
			_SKXamlCanvas x when x == _foregroundPaintContext.ShadowHost => _foregroundPaintContext,

			_ => throw new InvalidOperationException(),
		};

#if __ANDROID__ && ANDROID_REFERENTIAL_IMPL
		if (!context.IsOpacitySet && (context.ShadowHost as Android.Views.ViewGroup)?.GetChildAt(0) is TextureView openGlTexture)
		{
			openGlTexture.SetOpaque(false);
			context.IsOpacitySet = true;
		}
#endif

		if (context.ShadowHost == null ||
			_currentContent is not { ActualHeight: > 0, ActualWidth: > 0 })
		{
			return;
		}

		var surface = e.Surface;
		var surfaceWidth = e.Info.Width;
		var surfaceHeight = e.Info.Height;

		var pixelRatio = (float)(surfaceWidth / context.ShadowHost.Width);
		var width = _currentContent.ActualWidth;
		var height = _currentContent.ActualHeight;

		var shadows = Shadows?.Where(x => x.IsInner == context.IsInner).ToArray() ?? Array.Empty<Shadow>();
		if (!NeedsPaint(context, shadows, width, height, pixelRatio, out bool pixelRatioChanged)) // todo@xy: split path
		{
			return;
		}

		var canvas = surface.Canvas;
		canvas.Clear(SKColors.Transparent);
		canvas.Save();

		if (shadows.Length == 0)
		{
			return;
		}

		var key =
			FormattableString.Invariant($"w{width},h{height}") +
			string.Join('/', shadows.Select(x => x.ToKey()));
		if (pixelRatioChanged)
		{
			// Pixel density changed, invalidate cached image
			Cache.Remove(key);
		}
		else if (Cache.TryGetValue(key, out var shadowsImage))
		{
			canvas.DrawImage(shadowsImage, SKPoint.Empty);
			canvas.Restore();
			return;
		}

		var childWidth = (float)width * pixelRatio;
		var childHeight = (float)height * pixelRatio;
		var diffWidthSurfaceChild = surfaceWidth - childWidth;
		var diffHeightSurfaceChild = surfaceHeight - childHeight;

		canvas.Translate(diffWidthSurfaceChild / 2, diffHeightSurfaceChild / 2);

		using var paint = new SKPaint() { IsAntialias = true };
		var cornerRadius = (float)_cornerRadius.BottomRight * pixelRatio;

		if (context.IsBackground)
		{
			foreach (var shadow in shadows)
			{
				var skShadow = SKShadow.From(shadow, childWidth, childHeight, cornerRadius, pixelRatio);

				DrawDropShadow(canvas, paint, skShadow);
			}
		}
		else
		{
			var contentShape = new SKRoundRect(new SKRect(0, 0, childWidth, childHeight), cornerRadius);
			canvas.ClipRoundRect(contentShape, antialias: true);

			foreach (var shadow in shadows)
			{
				var skShadow = SKShadow.From(shadow, childWidth, childHeight, cornerRadius, pixelRatio);

				DrawInnerShadow(canvas, paint, skShadow);
			}
		}

		canvas.Restore();


		// If a property has changed dynamically, we don't want to cache the updated shadows
		if (!context.IsDirty)
		{
			Cache.AddOrUpdate(key, surface.Snapshot());
		}

		context.IsDirty = false;
	}

	private static void DrawDropShadow(SKCanvas canvas, SKPaint paint, SKShadow shadow)
	{
		paint.Style = SKPaintStyle.Fill;
		paint.Color = shadow.Color;
		paint.ImageFilter = SKImageFilter.CreateBlur(shadow.BlurSigma, shadow.BlurSigma);
		paint.MaskFilter = null;
		paint.StrokeWidth = 0;

		// Two other ways to create shadows
		// 1. Mask filter
		// x = 0;
		// y = 0;
		// paint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, blurSigma);
		// 2. DropShadow
		// paint.ImageFilter = SKImageFilter.CreateDropShadowOnly(
		//     0,
		//     0,
		//     blurSigma,
		//     blurSigma,
		//     color);

		var shadowShape = new SKRoundRect(
			new SKRect(
				shadow.OffsetX,
				shadow.OffsetY,
				shadow.OffsetX + shadow.ContentWidth,
				shadow.OffsetY + shadow.ContentHeight),
			shadow.CornerRadius);
		shadowShape.Inflate(shadow.Spread, shadow.Spread);
		canvas.DrawRoundRect(shadowShape, paint);

		if (_logger.IsEnabled(LogLevel.Debug))
		{
			_logger.Debug($"[ShadowContainer] DrawDropShadow => x: {shadow.OffsetX}, y: {shadow.OffsetY}, width: {shadow.ContentWidth}, height: {shadow.ContentHeight}");
		}
	}

	private static void DrawInnerShadow(SKCanvas canvas, SKPaint paint, SKShadow shadow)
	{
		float strokeWidthX = Math.Abs(shadow.OffsetX);
		float strokeWidthY = Math.Abs(shadow.OffsetY);
		float strokeWidth = Math.Max(Math.Max(strokeWidthX, strokeWidthY), shadow.BlurSigma) + shadow.Spread * 2;

		paint.Style = SKPaintStyle.Stroke;
		paint.Color = shadow.Color;
		paint.StrokeWidth = strokeWidth * 2;
		paint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, shadow.BlurSigma);
		paint.ImageFilter = null;

		if (_logger.IsEnabled(LogLevel.Debug))
		{
			_logger.Debug($"[ShadowContainer] DrawInnerShadow => strokeWidth: {paint.StrokeWidth}, cornerRadius: {shadow.CornerRadius}, x: {shadow.OffsetX}, y: {shadow.OffsetY}, width: {shadow.ContentWidth}, height: {shadow.ContentHeight}");
		}

		var shadowShape = new SKRoundRect(
			new SKRect(
				0,
				0,
				shadow.ContentWidth + (paint.StrokeWidth),
				shadow.ContentHeight + (paint.StrokeWidth)),
			shadow.CornerRadius);

		shadowShape.Deflate(shadow.Spread, shadow.Spread);

		shadowShape.Offset(shadow.OffsetX - strokeWidth, shadow.OffsetY - strokeWidth);
		canvas.DrawRoundRect(shadowShape, paint);
	}

	private static SKColor ToSkiaColor(Color windowsUiColor)
	{
		return new SKColor(windowsUiColor.R, windowsUiColor.G, windowsUiColor.B, windowsUiColor.A);
	}

	/// <summary>
	/// Record of <see cref="Shadow"/> properties at one point in time.
	/// </summary>
	private record ShadowInfo(double OffsetX, double OffsetY, double BlurRadius, double Spread, Color Color, double Opacity)
	{
		public static ShadowInfo Snapshot(Shadow x) => new(
			x.OffsetX, x.OffsetY,
			x.BlurRadius, x.Spread,
			x.Color,
			x.Opacity
		);

		public static ShadowInfo[] Snapshot(Shadow[] shadows) => shadows.Select(Snapshot).ToArray();
	}

	/// <summary>
	/// Used in comparison to determine if the shadow needs to be repainted.
	/// </summary>
	private record ShadowPaintState(double Width, double Height, double PixelRatio, ShadowInfo[] ShadowInfos);

	private class ShadowPaintContext
	{
		public _SKXamlCanvas? ShadowHost { get; set; }

		public ShadowPaintState? LastPaintState { get; set; }

		public bool IsBackground { get; init; }
#if __ANDROID__ && ANDROID_REFERENTIAL_IMPL
		public bool IsOpacitySet { get; set; }
#endif
		public bool IsDirty { get; set; }

		// Inner shadows are drawn on top of content.
		// In the code here, they are also refered as "foreground" shadows.
		public bool IsInner => !IsBackground;
	}
}
