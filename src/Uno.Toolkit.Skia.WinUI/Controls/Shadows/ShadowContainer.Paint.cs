using System;
using System.Linq;
<<<<<<< HEAD
using Microsoft.Extensions.Logging;
using Windows.UI;
using SkiaSharp;
=======

using SkiaSharp;

using Windows.UI;

using Microsoft.Extensions.Logging;
>>>>>>> 9d4be7f (fix(shadows): revert double shadow impl)
using Uno.Extensions;
using Uno.Logging;

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

	private ShadowInfos[] _shadowInfoArray = Array.Empty<ShadowInfos>();
	private float _currentPixelRatio;
	private Color? _currentContentBackgroundColor;

	private bool NeedsPaint(double width, double height, float pixelRatio, out bool pixelRatioChanged)
	{
		var shadows = Shadows ?? new ShadowCollection();
		var newShadowInfos = shadows.Select(s => ShadowInfos.From(s, width, height)).ToArray();

		pixelRatioChanged = false;

		bool needsPaint = !newShadowInfos.SequenceEqual(_shadowInfoArray);
		_shadowInfoArray = newShadowInfos;

		if (pixelRatio != _currentPixelRatio)
		{
			_currentPixelRatio = pixelRatio;
			pixelRatioChanged = needsPaint = true;
		}

		return needsPaint;
	}

#if false // ANDROID  (see comment in ShadowContainer.cs)
	private void OnSurfacePainted(object? sender, SKPaintGLSurfaceEventArgs e)
	{
		if (!_notOpaqueSet && ((ViewGroup)_shadowHost).GetChildAt(0) is TextureView openGlTexture)
		{
			openGlTexture.SetOpaque(false);
			_notOpaqueSet = true;
		}
#else
	private void OnSurfacePainted(object? sender, SKPaintSurfaceEventArgs e)
	{
#endif
		if (_shadowHost == null || _currentContent is not { ActualHeight: > 0, ActualWidth: > 0 })
		{
			return;
		}

		var surface = e.Surface;
		var surfaceWidth = e.Info.Width;
		var surfaceHeight = e.Info.Height;

		float pixelRatio = surfaceWidth / (float)_shadowHost.Width;
		double width = _currentContent.ActualWidth;
		double height = _currentContent.ActualHeight;

		if (!NeedsPaint(width, height, pixelRatio, out bool pixelRatioChanged))
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

<<<<<<< HEAD
		var key =
			FormattableString.Invariant($"w{width},h{height}") +
#if !NETSTANDARD
			string.Join('/', shadows.Select(x => x.ToKey()));
#else
			string.Join("/", shadows.Select(x => x.ToKey()));
#endif
		if (pixelRatioChanged)
=======
		// If there is any inner shadow, we need to:
		// 1. Get the background color from the content
		// 2. Set the content background to transparent
		// 3. Draw the content background with skia underneath inner shadows
		bool hasInnerShadow = shadows.HasInnerShadow();
		if (hasInnerShadow)
>>>>>>> 9d4be7f (fix(shadows): revert double shadow impl)
		{
			// Will set the content background to transparent if needed
			if (_currentContentBackgroundColor == null && ProcessContentBackgroundIfNeeded(out var contentBackgroundWinUIColor))
			{
				_currentContentBackgroundColor = contentBackgroundWinUIColor;
			}
		}
		else if (_currentContentBackgroundColor.HasValue)
		{
			// Means that there were inner shadows, and they have been removed: restore content background
			TrySetContentBackground(new SolidColorBrush(_currentContentBackgroundColor.Value));
			_currentContentBackgroundColor = null;
		}

		string shadowsKey = shadows.ToKey(width, height, _currentContentBackgroundColor);
		if (Cache.TryGetValue(shadowsKey, out var shadowsImage))
		{
			if (pixelRatioChanged)
			{
				// Monitor pixel density changed, need to remove cached image
				Cache.Remove(shadowsKey);
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

		using var paint = new SKPaint();
		paint.IsAntialias = true;

		float cornerRadius = (float)_cornerRadius.BottomRight * pixelRatio;

		foreach (var shadow in shadows.Where(s => !s.IsInner))
		{
			var skShadow = SKShadow.From(shadow, childWidth, childHeight, cornerRadius, pixelRatio);

			DrawDropShadow(canvas, paint, skShadow);
		}

		// Always draw inner shadows on top of the drop shadows
		if (hasInnerShadow)
		{
			var contentShape = new SKRoundRect(new SKRect(0, 0, childWidth, childHeight), cornerRadius);
			canvas.ClipRoundRect(contentShape, antialias: true);

			// Draw the content background first
			if (_currentContentBackgroundColor.HasValue)
			{
				var contentBackgroundColor = ToSkiaColor(_currentContentBackgroundColor.Value);
				DrawContentBackground(canvas, contentBackgroundColor, contentShape);
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
			Cache.AddOrUpdate(shadowsKey, surface.Snapshot());
		}

		_shadowPropertyChanged = false;
	}

	private bool ProcessContentBackgroundIfNeeded(out Color? contentBackgroundColor)
	{
		contentBackgroundColor = null;
		if (TryGetContentBackground(out var background))
		{
			if (background is not SolidColorBrush backgroundColorBrush)
			{
				throw new NotSupportedException("[ShadowContainer] Unsupported Background brush: when using inner shadows the only supported brush type for the Background property is SolidBrushColor");
			}

			if (backgroundColorBrush.Color != Color.FromArgb(0, 0, 0, 0))
			{
				contentBackgroundColor = backgroundColorBrush.Color;
			}

			TrySetContentBackground(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)));
			return true;
		}

		return false;
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

	private bool TryGetContentBackground(out Brush? background)
	{
		if (_currentContent == null)
		{
			background = null;
			return false;
		}

		background = _currentContent switch
		{
			Control control => control.Background,
			Panel panel => panel.Background,
			Border border => border.Background,
			_ => null,
		};

		return background != null;
	}

	private bool TrySetContentBackground(SolidColorBrush background)
	{
		switch (_currentContent)
		{
			case Control control:
				control.Background = background;
				break;
			case Panel panel:
				panel.Background = background;
				break;
			case Border border:
				border.Background = background;
				break;
			default:
				return false;
		}

		return true;
	}

	private static SKColor ToSkiaColor(Color windowsUiColor)
	{
		return new SKColor(windowsUiColor.R, windowsUiColor.G, windowsUiColor.B, windowsUiColor.A);
	}
}
