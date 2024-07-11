using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using Uno.Extensions;
using Uno.Logging;
using Windows.UI;

namespace Uno.Toolkit.UI;

public partial class ShadowContainer
{
	private static readonly ILogger _logger = typeof(ShadowContainer).Log();

	// note: There "may" be a difference between windows vs skia in measurement; This ratio is recorded as the `PixelRatio` where: WindowsValue * PixelRatio = SkiaValue.
	// To ease comprehension, values in `double` type is in windows unit, and values in `float` is in skia unit or windows unit scaled to skia.

#if DEBUG
	private bool _isShadowDirty;
#endif
	private int _paintCount;
	private float _lastPixelRatio;

	internal event EventHandler<SurfacePaintCompletedEventArgs>? SurfacePaintCompleted;

	internal static void ClearCache()
	{
		Cache.Clear();
	}

	private void OnSurfacePaintCompleted(bool createdNewCanvas)
	{
		SurfacePaintCompleted?.Invoke(this, new SurfacePaintCompletedEventArgs(createdNewCanvas));
	}

	private void OnSurfacePainted(object? sender, SKPaintSurfaceEventArgs e)
	{
		if (_shadowHost == null ||
			Content is not FrameworkElement { ActualHeight: > 0, ActualWidth: > 0 } contentAsFE)
		{
			return;
		}

		var background = GetBackgroundColor(Background);
		if (background is { A: 0 })
		{
			// background step can be skipped with fully transparent background.
			// any color with 0-alpha will produce the same result, null'ing it here will also prevent cache miss.
			background = null;
		}

		var shape = GetShadowShapeContext(Content);
		var pixelRatio = (float)(e.Info.Width / _shadowHost.ActualWidth);
		var state = new ShadowPaintState(shape, background, pixelRatio, ShadowInfo.Snapshot(Shadows));
#if DEBUG
		_isShadowDirty = false;
#endif

		var canvas = e.Surface.Canvas;
		canvas.Clear(SKColors.Transparent);

		if (state.Shadows.Length == 0)
		{
			shape.DrawContentBackground(state, canvas, background ?? Colors.Transparent);
			return;
		}

		if (_logger.IsEnabled(LogLevel.Trace))
		{
			_logger.Trace($"[ShadowContainer] Painting shadows (x{++_paintCount}) for content {Content.GetType().Name}, actualSize: {contentAsFE.ActualWidth}x{contentAsFE.ActualHeight}");
		}

		using var _ = canvas.SnapshotState();

		var key =
			FormattableString.Invariant($"[{contentAsFE.ActualWidth}x{contentAsFE.ActualHeight},{_shadowHost.ActualWidth}x{_shadowHost.ActualHeight},{background},{shape.ToString()}]: ") +
			string.Join("; ", state.Shadows.Select(x => x.ToKey()));
		if (Cache.TryGetValue(key, out var snapshot))
		{
			if (pixelRatio != _lastPixelRatio)
			{
				// Monitor pixel density changed, need to remove cached image
				Cache.Remove(key);
				_lastPixelRatio = state.PixelRatio;
			}
			else
			{
				canvas.DrawImage(snapshot, SKPoint.Empty);
				OnSurfacePaintCompleted(createdNewCanvas: false);
				return;
			}
		}

		// relative to the SKCanvas, the entire content is padded to leave room for drop shadow
		// here, we need to re-calibrate the coord system to zero on the top-left corner of the content
		var deltaWidth = e.Info.Width - ((float)contentAsFE.ActualWidth * pixelRatio);
		var deltaHeight = e.Info.Height - ((float)contentAsFE.ActualHeight * pixelRatio);
		canvas.Translate(deltaWidth / 2, deltaHeight / 2);

		using var paint = new SKPaint() { IsAntialias = true };

		foreach (var shadow in state.GetDropShadows())
		{
			shape.DrawDropShadow(state, canvas, paint, shadow);
		}

		// clip background and inner shadow to content
		state.Shape.ClipToContent(state, canvas);

		if (background is { } bg)
		{
			shape.DrawContentBackground(state, canvas, bg);
		}

		foreach (var shadow in state.GetInsetShadows())
		{
			shape.DrawInnerShadow(state, canvas, paint, shadow);
		}

		// No property should have changed during painting.
#if DEBUG
		global::System.Diagnostics.Debug.Assert(!_isShadowDirty);
#endif

		Cache.AddOrUpdate(key, e.Surface.Snapshot());

		OnSurfacePaintCompleted(createdNewCanvas: true);
	}

	private static Color? GetBackgroundColor(Brush? background)
	{
		return background switch
		{
			SolidColorBrush scb => scb.Color with { A = (byte)(scb.Color.A * scb.Opacity) },

			null => default(Color?),
			_ => throw new NotSupportedException($"Invalid background brush type: {background.GetType().Name}"),
		};
	}

	private ShadowShapeContext GetShadowShapeContext(object content)
	{
		return content switch
		{
			// any dp used here, beside width/height that is covered by SizeChanged, needs to register for dp changed in: BindToPaintingProperties\BindToContent()
			Ellipse ellipse => new RadiusXYRectShadowShapeContext(ellipse.ActualWidth, ellipse.ActualHeight, ellipse.ActualWidth / 2, ellipse.ActualHeight / 2),
			Rectangle rect => new RadiusXYRectShadowShapeContext(rect.ActualWidth, rect.ActualHeight, rect.RadiusX, rect.RadiusY),
			FrameworkElement fe => new CornerRadiusRectShadowShapeContext(fe.ActualWidth, fe.ActualHeight, GetCornerRadiusFor(Content) ?? default),

			_ => throw new NotSupportedException($"Unsupported content type: {content.GetType().Name}"),
		};
	}

	/// <summary>
	/// Serves both as a record of states relevant to shadow shape (not <see cref="Shape"/>, but in the broad sense), and the implementations for painting the shadows.
	/// We can't use an interface here or we will lose the ability to compare instances of this class (thanks to generated Equals).
	/// </summary>
	private abstract record ShadowShapeContext
	{
		public abstract void ClipToContent(ShadowPaintState state, SKCanvas canvas);

		public abstract void DrawContentBackground(ShadowPaintState state, SKCanvas canvas, Color color);

		public abstract void DrawDropShadow(ShadowPaintState state, SKCanvas canvas, SKPaint paint, ShadowInfo shadow);

		public abstract void DrawInnerShadow(ShadowPaintState state, SKCanvas canvas, SKPaint paint, ShadowInfo shadow);
	}

	private abstract record RoundRectShadowShapeContext(double Width, double Height) : ShadowShapeContext
	{
		protected abstract SKRoundRect GetContentShape(ShadowPaintState state);

		public override void ClipToContent(ShadowPaintState state, SKCanvas canvas)
		{
			canvas.ClipRoundRect(GetContentShape(state), antialias: true);
		}

		public override void DrawContentBackground(ShadowPaintState state, SKCanvas canvas, Color color)
		{
			var shape = GetContentShape(state);
			using var backgroundPaint = new SKPaint
			{
				Color = color.ToSkiaColor(),
				Style = SKPaintStyle.Fill,
			};

			if (_logger.IsEnabled(LogLevel.Trace))
			{
				_logger.Trace($"[ShadowContainer] DrawContentBackground => color: {color}");
			}
			canvas.DrawRoundRect(shape, backgroundPaint);
		}

		public override void DrawDropShadow(ShadowPaintState state, SKCanvas canvas, SKPaint paint, ShadowInfo shadow)
		{
			var spread = (float)shadow.Spread * state.PixelRatio;
			var offsetX = (float)shadow.OffsetX * state.PixelRatio;
			var offsetY = (float)shadow.OffsetY * state.PixelRatio;

			var blurSigma = shadow.GetBlurSigma(state.PixelRatio);

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

			var shape = GetContentShape(state);
			shape.Offset(offsetX, offsetY);
			shape.Inflate(spread, spread);

			if (_logger.IsEnabled(LogLevel.Trace))
			{
				_logger.Trace($"[ShadowContainer] DrawDropShadow => x: {shape.Rect.Left}, y: {shape.Rect.Top}, width: {shape.Rect.Width}, height: {shape.Rect.Height}");
			}
			canvas.DrawRoundRect(shape, paint);
		}

		public override void DrawInnerShadow(ShadowPaintState state, SKCanvas canvas, SKPaint paint, ShadowInfo shadow)
		{
			var spread = (float)shadow.Spread * state.PixelRatio;
			var offsetX = (float)shadow.OffsetX * state.PixelRatio;
			var offsetY = (float)shadow.OffsetY * state.PixelRatio;

			var blurSigma = shadow.GetBlurSigma(state.PixelRatio);
			var strokeWidthX = Math.Abs(offsetX);
			var strokeWidthY = Math.Abs(offsetY);
			var strokeWidth = Math.Max(Math.Max(strokeWidthX, strokeWidthY), blurSigma) + spread * 2;

			paint.Style = SKPaintStyle.Stroke;
			paint.Color = shadow.GetSKColor();
			paint.StrokeWidth = strokeWidth * 2;
			paint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, blurSigma);
			paint.ImageFilter = null;

			var shape = GetContentShape(state);
			shape.Offset(offsetX, offsetY);
			shape.Inflate(strokeWidth - spread, strokeWidth - spread);

			if (_logger.IsEnabled(LogLevel.Trace))
			{
				_logger.Trace($"[ShadowContainer] DrawInnerShadow => strokeWidth: {paint.StrokeWidth}, x: {offsetX}, y: {offsetY}, width: {shape.Rect.Width}, height: {shape.Rect.Height}");
			}
			canvas.DrawRoundRect(shape, paint);
		}
	}

	private record CornerRadiusRectShadowShapeContext(double Width, double Height, CornerRadius CornerRadius) : RoundRectShadowShapeContext(Width, Height)
	{
		protected override SKRoundRect GetContentShape(ShadowPaintState state)
		{
			var rect = new SKRect(0, 0, (float)Width * state.PixelRatio, (float)Height * state.PixelRatio);
			var radii = new SKPoint[] {
				new SKPoint((float)CornerRadius.TopLeft * state.PixelRatio, (float)CornerRadius.TopLeft * state.PixelRatio),
				new SKPoint((float)CornerRadius.TopRight * state.PixelRatio, (float)CornerRadius.TopRight * state.PixelRatio),
				new SKPoint((float)CornerRadius.BottomRight * state.PixelRatio,(float)CornerRadius.BottomRight * state.PixelRatio),
				new SKPoint((float)CornerRadius.BottomLeft * state.PixelRatio, (float)CornerRadius.BottomLeft * state.PixelRatio)
			};
			var shape = new SKRoundRect();
			shape.SetRectRadii(rect, radii);

			return shape;
		}
	}

	private record RadiusXYRectShadowShapeContext(double Width, double Height, double RadiusX, double RadiusY) : RoundRectShadowShapeContext(Width, Height)
	{
		protected override SKRoundRect GetContentShape(ShadowPaintState state)
		{
			var rect = new SKRect(0, 0, (float)Width * state.PixelRatio, (float)Height * state.PixelRatio);
			var shape = new SKRoundRect(rect, (float)RadiusX * state.PixelRatio, (float)RadiusY * state.PixelRatio);

			return shape;
		}
	}

	/// <summary>
	/// Record of <see cref="Shadow"/> properties at one point in time.
	/// </summary>
	private readonly record struct ShadowInfo(bool Inset, double OffsetX, double OffsetY, double BlurRadius, double Spread, Color Color, double Opacity)
	{
		public static ShadowInfo Snapshot(Shadow x) => new(
			x.IsInner,
			x.OffsetX, x.OffsetY,
			x.BlurRadius, x.Spread,
			x.Color,
			x.Opacity
		);

		public static ShadowInfo[] Snapshot(IEnumerable<Shadow> shadows) => shadows.Safe().Select(Snapshot).ToArray();

		public SKColor GetSKColor() => new SKColor(Color.R, Color.G, Color.B, (byte)(Color.A * Opacity));

		public float GetBlurSigma(float pixelRatio)
		{
			var radius = (float)BlurRadius * pixelRatio;
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
		ShadowShapeContext Shape,
		Color? Background,
		float PixelRatio,
		ShadowInfo[] Shadows)
	{
		public IEnumerable<ShadowInfo> GetDropShadows() => Shadows.Safe().Where(x => !x.Inset);
		public IEnumerable<ShadowInfo> GetInsetShadows() => Shadows.Safe().Where(x => x.Inset);

		// GetHashCode+Equals are needed here for Shadows' sequential equality

		public override int GetHashCode()
		{
			var hash = new HashCode();

			hash.Add(Shape.GetHashCode());
			hash.Add(Background.GetHashCode());
			hash.Add(PixelRatio.GetHashCode());
			foreach (var item in Shadows)
			{
				hash.Add(item);
			}

			return hash.ToHashCode();
		}

		public bool Equals(ShadowPaintState? x) =>
			x is not null &&
			Shape == x.Shape &&
			Background == x.Background &&
			PixelRatio == x.PixelRatio &&
			Shadows.SequenceEqual(x.Shadows);
	}

	public class SurfacePaintCompletedEventArgs : EventArgs
	{
		public bool CreatedNewCanvas { get; }

		public SurfacePaintCompletedEventArgs(bool createdNewCanvas)
		{
			CreatedNewCanvas = createdNewCanvas;
		}
	}
}
