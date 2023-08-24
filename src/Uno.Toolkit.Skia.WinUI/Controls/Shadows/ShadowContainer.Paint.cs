using System;
using System.Linq;

using SkiaSharp;

using Windows.UI;

using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;
using System.Collections.Generic;
using Microsoft.UI;
using Microsoft.UI.Xaml;

#if IS_WINUI
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;
using SkiaSharp.Views.Windows;
#else
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using SkiaSharp.Views.UWP;
#endif

namespace Uno.Toolkit.UI;

public partial class ShadowContainer
{
	private static readonly ILogger _logger = typeof(ShadowContainer).Log();

	private ShadowPaintState? _lastPaintState;

	private bool NeedsPaint(ShadowPaintState state, out bool pixelRatioChanged)
	{
		var needsPaint = state != _lastPaintState;
		pixelRatioChanged = state.PixelRatio != _lastPaintState?.PixelRatio;

		_lastPaintState = state;
		return needsPaint;
	}

	private void OnSurfacePainted(object? sender, SKPaintSurfaceEventArgs e)
	{
		if (_shadowHost == null ||
			_currentContent is not { ActualHeight: > 0, ActualWidth: > 0 })
		{
			return;
		}

		var surface = e.Surface;
		var surfaceWidth = e.Info.Width;
		var surfaceHeight = e.Info.Height;

		float pixelRatio = surfaceWidth / (float)_shadowHost.Width;
		double width = _currentContent.ActualWidth;
		double height = _currentContent.ActualHeight;
		
		var background = GetBackgroundColor(Background);
		if (background is { A: 0 })
		{
			// background step can be skipped with fully transparent background.
			// any color with 0-alpha will produce the same result, null'ing it here will also prevent cache miss.
			background = null;
		}

		var state = new ShadowPaintState(width, height, background, CornerRadius, pixelRatio, ShadowInfo.Snapshot(Shadows));
		if (!NeedsPaint(state, out bool pixelRatioChanged))
		{
			return;
		}

		var canvas = surface.Canvas;
		canvas.Clear(SKColors.Transparent);
		canvas.Save();

		if (Shadows is not { Count: > 0 } shadows)
		{
			return;
		}

		var key = FormattableString.Invariant($"[{width}x{height},{background}]: {shadows.ToKey()}]");
		if (Cache.TryGetValue(key, out var shadowsImage))
		{
			if (pixelRatioChanged)
			{
				// Monitor pixel density changed, need to remove cached image
				Cache.Remove(key);
			}
			else
			{
				canvas.DrawImage(shadowsImage, SKPoint.Empty);
				canvas.Restore();
				return;
			}
		}

		float childWidth = (float)width * pixelRatio;
		float childHeight = (float)height * pixelRatio;

		float diffWidthSurfaceChild = surfaceWidth - childWidth;
		float diffHeightSurfaceChild = surfaceHeight - childHeight;
		canvas.Translate(diffWidthSurfaceChild / 2, diffHeightSurfaceChild / 2);

		using var paint = new SKPaint() { IsAntialias = true };

		float cornerRadius = (float)CornerRadius.BottomLeft * pixelRatio;

		foreach (var shadow in shadows.Where(s => !s.IsInner))
		{
			var skShadow = SKShadow.From(shadow, childWidth, childHeight, cornerRadius, pixelRatio);

			DrawDropShadow(canvas, paint, skShadow);
		}

		// Always draw inner shadows on top of the drop shadows
		if (shadows.Any(x => x.IsInner))
		{
			var contentShape = new SKRoundRect(new SKRect(0, 0, childWidth, childHeight), cornerRadius);
			canvas.ClipRoundRect(contentShape, antialias: true);

			// Draw the content background first
			if (background is not null)
			{
				DrawContentBackground(canvas, ToSkiaColor(background.Value), contentShape);
			}

			// Then we draw the inner shadows
			foreach (var shadow in shadows.Where(s => s.IsInner))
			{
				var skShadow = SKShadow.From(shadow, childWidth, childHeight, cornerRadius, pixelRatio);

				DrawInnerShadow(canvas, paint, skShadow);
			}
		}

		canvas.Restore();

		if (!_shadowPropertyChanged)
		{
			// If a property has changed dynamically, we don't want to cache the updated shadows
			Cache.AddOrUpdate(key, surface.Snapshot());
		}

		_shadowPropertyChanged = false;
	}

	private static Color? GetBackgroundColor(Brush background)
	{
		return background switch
		{
			SolidColorBrush scb => scb.Color,

			null => default(Color?),
			_ => throw new NotSupportedException($"Invalid background brush type: {background.GetType().Name}")
		};
	}

	private static void DrawContentBackground(SKCanvas canvas, SKColor contentBackgroundColor, SKRoundRect childShape)
	{
		using var backgroundPaint = new SKPaint
		{
			Color = contentBackgroundColor,
			Style = SKPaintStyle.Fill,
		};

		if (_logger.IsEnabled(LogLevel.Debug))
		{
			_logger.Debug(
				$"[ShadowContainer] DrawContentBackground => color: {backgroundPaint.Color}");
		}
		canvas.DrawRoundRect(childShape, backgroundPaint);
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
			_logger.Debug(
				$"[ShadowContainer] DrawDropShadow => x: {shadow.OffsetX}, y: {shadow.OffsetY}, width: {shadow.ContentWidth}, height: {shadow.ContentHeight}");
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
			_logger.Debug(
			$"[ShadowContainer] DrawInnerShadow => strokeWidth: {paint.StrokeWidth}, cornerRadius: {shadow.CornerRadius}, x: {shadow.OffsetX}, y: {shadow.OffsetY}, width: {shadow.ContentWidth}, height: {shadow.ContentHeight}");
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
			float blurRadius = (float)shadow.BlurRadius * pixelRatio;
			// Blur sigma conversion taken from flutter source code
			float blurSigma = blurRadius > 0 ? blurRadius * 0.57735f + 0.5f : 0f;

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
	
	/// <summary>
	/// Record of <see cref="Shadow"/> properties at one point in time.
	/// </summary>
	private readonly record struct ShadowInfo(double OffsetX, double OffsetY, double BlurRadius, double Spread, Color Color, double Opacity)
	{
		public static ShadowInfo Snapshot(Shadow x) => new(
			x.OffsetX, x.OffsetY,
			x.BlurRadius, x.Spread,
			x.Color,
			x.Opacity
		);

		public static ShadowInfo[] Snapshot(IEnumerable<Shadow> shadows) => shadows?.Select(Snapshot).ToArray() ?? Array.Empty<ShadowInfo>();
	}
	
	/// <summary>
	/// Used in comparison to determine if the shadow needs to be repainted.
	/// </summary>
	private sealed record ShadowPaintState(double Width, double Height, Color? Background, CornerRadius CornerRadius, double PixelRatio, ShadowInfo[] ShadowInfos)
	{
		// GetHashCode+Equals are needed here for ShadowInfos' sequential equality

		public override int GetHashCode()
		{
			var hash = 1214348419;
		
			hash = hash * -1521134295 + Width.GetHashCode();
			hash = hash * -1521134295 + Height.GetHashCode();
			hash = hash * -1521134295 + PixelRatio.GetHashCode();
			hash = hash * -1521134295 + Background.GetHashCode();
			hash = hash * -1521134295 + CornerRadius.GetHashCode();
			foreach (var item in ShadowInfos)
			{
				hash = hash * -1521134295 + item.GetHashCode();
			}
		
			return hash;
		}

		public bool Equals(ShadowPaintState? x) =>
			x is not null &&
			Width == x.Width &&
			Height == x.Height &&
			PixelRatio == x.PixelRatio &&
			Background == x.Background &&
			CornerRadius == x.CornerRadius &&
			ShadowInfos.SequenceEqual(x.ShadowInfos);
	}
}
