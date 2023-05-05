using System.Linq;

using SkiaSharp;

using Windows.UI;

#if IS_WINUI
using SkiaSharp.Views.Windows;
#else
using SkiaSharp.Views.UWP;
#endif

namespace Uno.Toolkit.UI;

public partial class ShadowContainer
{
    private record ShadowInfos(double Width, double Height, double BlurRadius, double Spread, double X, double Y, Color color)
    {
        public static readonly ShadowInfos Empty = new ShadowInfos(0, 0, 0, 0, 0, 0, new Color());
    }

    private ShadowInfos[] _shadowInfoArray = new[] { ShadowInfos.Empty };
    private float _currentPixelRatio;

    private static ShadowInfos ToShadowInfos(Shadow shadow, double width, double height)
    {
        return new ShadowInfos(
            width,
            height,
            shadow.BlurRadius,
            shadow.Spread,
            shadow.OffsetX,
            shadow.OffsetY,
            Color.FromArgb((byte)(shadow.Color.A * shadow.Opacity), shadow.Color.R, shadow.Color.G, shadow.Color.B));
    }

    private bool NeedsPaint(double width, double height, float pixelRatio, out bool pixelRatioChanged)
    {
        var shadows = Shadows ?? new ShadowCollection();
        var newShadowInfos = shadows.Select(s => ToShadowInfos(s, width, height)).ToArray();
        bool needsPaint = false;
		pixelRatioChanged = false;

        if (newShadowInfos.Length != _shadowInfoArray.Length)
        {
            _shadowInfoArray = newShadowInfos;
            needsPaint = true;
        }
        else
        {
            int index = 0;
            foreach (var newInfo in newShadowInfos)
            {
                var currentInfo = _shadowInfoArray[index++];
                if (currentInfo != newInfo)
                {
                    _shadowInfoArray = newShadowInfos;
                    needsPaint = true;
                    break;
                }
            }
        }

        if (pixelRatio != _currentPixelRatio)
        {
            _currentPixelRatio = pixelRatio;
            pixelRatioChanged = needsPaint = true;
        }

        return needsPaint;
    }

#if false // ANDROID
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

        var shadows = Shadows ?? new ShadowCollection();

        if (!NeedsPaint(width, height, pixelRatio, out bool pixelRatioChanged))
        {
            return;
        }

        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        canvas.Save();

        if (shadows.Count == 0) 
        {
            return;
        }

        string shadowsKey = shadows.ToKey(width, height);
        if (Cache.TryGet(shadowsKey, out var shadowsImage))
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

        // System.Diagnostics.Debug.WriteLine($">>>>>> Pixel Ratio: {pixelRatio}");

        float childWidth = (float)width * pixelRatio;
        float childHeight = (float)height * pixelRatio;

        float diffWidthSurfaceChild = surfaceWidth - childWidth;
        float diffHeightSurfaceChild = surfaceHeight - childHeight;
        canvas.Translate(diffWidthSurfaceChild / 2, diffHeightSurfaceChild / 2);

        using var paint = new SKPaint();
        paint.IsAntialias = true;
        paint.Style = SKPaintStyle.Fill;

        float cornerRadius = (float)_cornerRadius.BottomRight * pixelRatio;

        foreach (var shadow in shadows)
        {
            float offsetX = (float)shadow.OffsetX * pixelRatio;
            float offsetY = (float)shadow.OffsetY * pixelRatio;
            float spread = (float)shadow.Spread * pixelRatio;

            float blurRadius = (float)shadow.BlurRadius * pixelRatio;
            // Blur sigma conversion taken from flutter source code
            float blurSigma = blurRadius > 0 ? blurRadius * 0.57735f + 0.5f : 0f;

            var color = shadow.Color.ToSKColor();
            color = color.WithAlpha((byte)(color.Alpha * shadow.Opacity));

            // System.Diagnostics.Debug.WriteLine($">>>>>> Blur sigma: {pixelBlurSigma}");
            DrawShadow(canvas, paint, blurSigma, offsetX, offsetY, childWidth, childHeight, color, cornerRadius, spread);
        }

        canvas.Restore();

        if (!_shadowPropertyChanged)
        {
            // If a property has changed dynamically, we don't want to cache the updated shadows
            Cache.AddOrUpdate(shadowsKey, surface.Snapshot());
        }

        _shadowPropertyChanged = false;
    }

    private static void DrawShadow(
		SKCanvas canvas,
		SKPaint paint,
		float blurSigma,
		float x,
		float y,
		float width,
		float height,
		SKColor color,
		float cornerRadius,
		float spread)
    {
        //x = 0;
        //y = 0;
        //paint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Inner, blurSigma);

        paint.Color = color;
        paint.ImageFilter = SKImageFilter.CreateBlur(blurSigma, blurSigma);

        //paint.ImageFilter = SKImageFilter.CreateDropShadowOnly(
        //    0,
        //    0,
        //    blurSigma,
        //    blurSigma,
        //    color);

        var shadowShape = new SKRoundRect(new SKRect(x, y, x + width, y + height), cornerRadius);
        shadowShape.Inflate(spread, spread);
        canvas.DrawRoundRect(shadowShape, paint);

        System.Diagnostics.Debug.WriteLine($"ShadowContainer => x: {x}, y: {y}, width: {width}, height: {height}");
    }
}
