#if HAS_UNO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;


#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif

namespace Uno.Toolkit.UI
{
	internal static class ColorHelper
	{
		public static bool TryGetColorWithOpacity(Brush? brush, out Color color)
		{
			switch (brush)
			{
				case SolidColorBrush scb:
					color = GetColorWithOpacity(scb, scb.Color);
					return true;
				case GradientBrush gb:
					color = GetColorWithOpacity(gb, gb.FallbackColor);
					return true;
				case XamlCompositionBrushBase ab:
					color = GetColorWithOpacity(ab, ab.FallbackColor);
					return true;
				default:
					color = default;
					return false;
			}
		}

		public static Color GetColorWithOpacity(Brush brush, Color referenceColor)
		{
			return Color.FromArgb((byte)(brush.Opacity * referenceColor.A), referenceColor.R, referenceColor.G, referenceColor.B);
		}

		public static bool IsTransparent(Color color) => color.A == 0;
	}
}
#endif