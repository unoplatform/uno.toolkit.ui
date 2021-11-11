using System;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.UI.ToolkitLib
{
	internal static class AutoLayoutExtensions
	{
		internal static HorizontalAlignment ToHorizontalAlignment(this AutoLayoutRelativeAlignment relativeAlignment)
		{
			return relativeAlignment switch
			{
				AutoLayoutRelativeAlignment.Start => HorizontalAlignment.Left,
				AutoLayoutRelativeAlignment.Center => HorizontalAlignment.Center,
				AutoLayoutRelativeAlignment.End => HorizontalAlignment.Right,
				AutoLayoutRelativeAlignment.Stretch => HorizontalAlignment.Stretch,
				_ => throw new ArgumentException($"Unknown {nameof(AutoLayoutRelativeAlignment)}: {relativeAlignment}", nameof(relativeAlignment)),
			};
		}

		internal static VerticalAlignment ToVerticalAlignment(this AutoLayoutRelativeAlignment relativeAlignment)
		{
			return relativeAlignment switch
			{
				AutoLayoutRelativeAlignment.Start => VerticalAlignment.Top,
				AutoLayoutRelativeAlignment.Center => VerticalAlignment.Center,
				AutoLayoutRelativeAlignment.End => VerticalAlignment.Bottom,
				AutoLayoutRelativeAlignment.Stretch => VerticalAlignment.Stretch,
				_ => throw new ArgumentException($"Unknown {nameof(AutoLayoutRelativeAlignment)}: {relativeAlignment}", nameof(relativeAlignment)),
			};
		}

		internal static HorizontalAlignment ToHorizontalAlignment(this AutoLayoutAlignment layoutAlignment)
		{
			return layoutAlignment switch
			{
				AutoLayoutAlignment.Start => HorizontalAlignment.Left,
				AutoLayoutAlignment.Center => HorizontalAlignment.Center,
				AutoLayoutAlignment.End => HorizontalAlignment.Right,
				AutoLayoutAlignment.Stretch => HorizontalAlignment.Stretch,
				_ => throw new ArgumentException($"Unknown {nameof(AutoLayoutAlignment)}: {layoutAlignment}", nameof(layoutAlignment)),
			};
		}

		internal static VerticalAlignment ToVerticalAlignment(this AutoLayoutAlignment layoutAlignment)
		{
			return layoutAlignment switch
			{
				AutoLayoutAlignment.Start => VerticalAlignment.Top,
				AutoLayoutAlignment.Center => VerticalAlignment.Center,
				AutoLayoutAlignment.End => VerticalAlignment.Bottom,
				AutoLayoutAlignment.Stretch => VerticalAlignment.Stretch,
				_ => throw new ArgumentException($"Unknown {nameof(AutoLayoutAlignment)}: {layoutAlignment}", nameof(layoutAlignment)),
			};
		}
	}
}
