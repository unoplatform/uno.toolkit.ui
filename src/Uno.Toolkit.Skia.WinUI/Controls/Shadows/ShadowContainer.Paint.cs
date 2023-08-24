using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using Uno.Extensions;
using Uno.Logging;
using Windows.UI;

namespace Uno.Toolkit.UI;

public partial class ShadowContainer
{
	private static readonly ILogger _logger = typeof(ShadowContainer).Log();

	private ShadowPaintState? _lastPaintState;
	private bool _isShadowDirty;

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

		var pixelRatio = surfaceWidth / (float)_shadowHost.Width;
		var width = (float)_currentContent.ActualWidth;
		var height = (float)_currentContent.ActualHeight;

		var background = GetBackgroundColor(Background);
		if (background is { A: 0 })
		{
			// background step can be skipped with fully transparent background.
			// any color with 0-alpha will produce the same result, null'ing it here will also prevent cache miss.
			background = null;
		}

		var state = new ShadowPaintState(width, height, background, CornerRadius, pixelRatio, ShadowInfo.Snapshot(Shadows));
		_isShadowDirty = false;

		if (!NeedsPaint(state, out bool pixelRatioChanged))
		{
			return;
		}

		var canvas = surface.Canvas;

		if (state.ShadowInfos.Length > 0)
		{
			canvas.Clear(SKColors.Transparent);
			canvas.Save();
		}
		else
		{
			DrawContentBackground(state, canvas, background ?? Colors.Transparent);
			return;
		}

		var key =
			FormattableString.Invariant($"[{width}x{height},{background}]: ") +
			string.Join("; ", state.ShadowInfos.Select(x => x.ToKey()));
		if (Cache.TryGetValue(key, out var snapshot))
		{
			if (pixelRatioChanged)
			{
				// Monitor pixel density changed, need to remove cached image
				Cache.Remove(key);
			}
			else
			{
				canvas.DrawImage(snapshot, SKPoint.Empty);
				canvas.Restore();
				return;
			}
		}

		var childWidth = (float)width * pixelRatio;
		var childHeight = (float)height * pixelRatio;
		var diffWidthSurfaceChild = surfaceWidth - childWidth;
		var diffHeightSurfaceChild = surfaceHeight - childHeight;
		canvas.Translate(diffWidthSurfaceChild / 2, diffHeightSurfaceChild / 2);

		using var paint = new SKPaint() { IsAntialias = true };

		foreach (var shadow in state.GetDropShadows())
		{
			DrawDropShadow(state, canvas, paint, shadow);
		}

		var contentShape = new SKRoundRect(
			new SKRect(0, 0, childWidth, childHeight),
			(float)state.CornerRadius.BottomRight * pixelRatio);
		canvas.ClipRoundRect(contentShape, antialias: true);

		if (background is { } bg)
		{
			DrawContentBackground(state, canvas, bg);
		}

		foreach (var shadow in state.GetInsetShadows())
		{
			DrawInnerShadow(state, canvas, paint, shadow);
		}

		canvas.Restore();

		// If a property has changed dynamically during this paint method,
		// then we don't want to cache the updated shadows
		if (!_isShadowDirty)
		{
			Cache.AddOrUpdate(key, surface.Snapshot());
		}
	}

	private static Color? GetBackgroundColor(Brush background)
	{
		return background switch
		{
			SolidColorBrush scb => scb.Color with { A = (byte)(scb.Color.A * scb.Opacity) },

			null => default(Color?),
			_ => throw new NotSupportedException($"Invalid background brush type: {background.GetType().Name}")
		};
	}

	private static void DrawContentBackground(ShadowPaintState state, SKCanvas canvas, Color color)
	{
		var rect = new SKRect(0, 0, state.ContentWidth, state.ContentHeight).Scale(state.PixelRatio);
		var shape = new SKRoundRect(rect, (float)state.CornerRadius.BottomRight * state.PixelRatio);

		using var backgroundPaint = new SKPaint
		{
			Color = color.ToSkiaColor(),
			Style = SKPaintStyle.Fill,
		};

		if (_logger.IsEnabled(LogLevel.Trace))
		{
			_logger.Trace($"[ShadowContainer] DrawContentBackground => color: {backgroundPaint.Color}");
		}
		canvas.DrawRoundRect(shape, backgroundPaint);
	}

	private static void DrawDropShadow(ShadowPaintState state, SKCanvas canvas, SKPaint paint, ShadowInfo shadow)
	{
		var blurSigma = (float)shadow.GetBlurSigma(state.PixelRatio);

		paint.Style = SKPaintStyle.Fill;
		paint.Color = shadow.GetSKColor();
		paint.ImageFilter = SKImageFilter.CreateBlur(blurSigma, blurSigma);
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

		var rect = new SKRect(
			shadow.OffsetX,
			shadow.OffsetY,
			shadow.OffsetX + state.ContentWidth,
			shadow.OffsetY + state.ContentHeight
		).Scale(state.PixelRatio);
		var shape = new SKRoundRect(rect, (float)state.CornerRadius.BottomRight * state.PixelRatio);
		shape.Inflate(shadow.Spread * state.PixelRatio, shadow.Spread * state.PixelRatio);

		canvas.DrawRoundRect(shape, paint);

		if (_logger.IsEnabled(LogLevel.Trace))
		{
			_logger.Trace($"[ShadowContainer] DrawDropShadow => x: {shape.Rect.Left}, y: {shadow.OffsetY}, width: {rect.Width}, height: {rect.Height}");
		}
	}

	private static void DrawInnerShadow(ShadowPaintState state, SKCanvas canvas, SKPaint paint, ShadowInfo shadow)
	{
		var blurSigma = shadow.GetBlurSigma(state.PixelRatio);
		var strokeWidthX = Math.Abs(shadow.OffsetX) * state.PixelRatio;
		var strokeWidthY = Math.Abs(shadow.OffsetY) * state.PixelRatio;
		var strokeWidth = Math.Max(Math.Max(strokeWidthX, strokeWidthY), blurSigma) + 2 * (shadow.Spread * state.PixelRatio);
		var cornerRadius = (float)state.CornerRadius.BottomRight * state.PixelRatio;

		paint.Style = SKPaintStyle.Stroke;
		paint.Color = shadow.GetSKColor();
		paint.StrokeWidth = strokeWidth * 2;
		paint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, blurSigma);
		paint.ImageFilter = null;

		if (_logger.IsEnabled(LogLevel.Trace))
		{
			_logger.Trace($"[ShadowContainer] DrawInnerShadow => strokeWidth: {paint.StrokeWidth}, cornerRadius: {cornerRadius}, x: {shadow.OffsetX * state.PixelRatio}, y: {shadow.OffsetY * state.PixelRatio}, width: {state.ContentWidth * state.PixelRatio}, height: {state.ContentHeight * state.PixelRatio}");
		}

		var rect = new SKRect(
			0,
			0,
			state.ContentWidth * state.PixelRatio + paint.StrokeWidth,
			state.ContentHeight * state.PixelRatio + paint.StrokeWidth);
		var shadowShape = new SKRoundRect(rect, cornerRadius);
		shadowShape.Deflate(shadow.Spread, shadow.Spread);
		shadowShape.Offset(shadow.OffsetX * state.PixelRatio - strokeWidth, shadow.OffsetY * state.PixelRatio - strokeWidth);

		canvas.DrawRoundRect(shadowShape, paint);
	}

	/// <summary>
	/// Record of <see cref="Shadow"/> properties at one point in time.
	/// </summary>
	private readonly record struct ShadowInfo(bool Inset, float OffsetX, float OffsetY, float BlurRadius, float Spread, Color Color, float Opacity)
	{
		public static ShadowInfo Snapshot(Shadow x) => new(
			x.IsInner,
			(float)x.OffsetX, (float)x.OffsetY,
			(float)x.BlurRadius, (float)x.Spread,
			x.Color,
			(float)x.Opacity
		);

		public static ShadowInfo[] Snapshot(IEnumerable<Shadow> shadows) => shadows.Safe().Select(Snapshot).ToArray();

		public SKColor GetSKColor() => new SKColor(Color.R, Color.G, Color.B, (byte)(Color.A * Opacity));

		public float GetBlurSigma(float pixelRatio)
		{
			var radius = BlurRadius * pixelRatio;
			// Blur sigma conversion taken from flutter source code
			var sigma = radius > 0 ? radius * 0.57735f + 0.5f : 0f;

			return sigma;
		}

		public string ToKey() => string.Join(",", Inset, OffsetX, OffsetY, BlurRadius, Spread, Color, Opacity);
	}

	/// <summary>
	/// Used in comparison to determine if the shadow needs to be repainted.
	/// </summary>
	private sealed record ShadowPaintState(
		float ContentWidth, float ContentHeight,
		Color? Background,
		CornerRadius CornerRadius,
		float PixelRatio,
		ShadowInfo[] ShadowInfos)
	{
		public IEnumerable<ShadowInfo> GetDropShadows() => ShadowInfos.Safe().Where(x => !x.Inset);
		public IEnumerable<ShadowInfo> GetInsetShadows() => ShadowInfos.Safe().Where(x => x.Inset);

		// GetHashCode+Equals are needed here for ShadowInfos' sequential equality

		public override int GetHashCode()
		{
			var hash = 1214348419;

			hash = hash * -1521134295 + ContentWidth.GetHashCode();
			hash = hash * -1521134295 + ContentHeight.GetHashCode();
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
			ContentWidth == x.ContentWidth &&
			ContentHeight == x.ContentHeight &&
			PixelRatio == x.PixelRatio &&
			Background == x.Background &&
			CornerRadius == x.CornerRadius &&
			ShadowInfos.SequenceEqual(x.ShadowInfos);
	}
}
