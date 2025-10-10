using System.Linq;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI;

/// <summary>
/// A container that allows its children to overflow its bounds.
/// </summary>
/// <remarks>This control allows <see cref="ZoomContentControl"/> content to not limited by its viewport.</remarks>
public partial class OverflowContainer : Grid
{
	protected override Size MeasureOverride(Size availableSize)
	{
		if (Children.FirstOrDefault() is { } child)
		{
			child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			return child.DesiredSize;
		}

		return default;
	}
}
