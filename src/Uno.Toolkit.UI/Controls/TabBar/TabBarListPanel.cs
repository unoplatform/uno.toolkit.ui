using System;
using System.Collections.Generic;
using System.Text;
using Uno.Toolkit.UI.Extensions;
using Windows.Foundation;
using System.Linq;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI.Controls
{
    public partial class TabBarListPanel : Panel
    {
		protected override Size MeasureOverride(Size availableSize)
		{
			if (Children.Count < 1)
			{
				return availableSize.FiniteOrDefault(default);
			}

			Size cellSize = new Size(availableSize.Width / Children.Count, availableSize.Height);
			foreach (var child in Children)
			{
				child.Measure(cellSize);
			}

			var maxHeight = Children.Max(x => x.DesiredSize.Height);

			var size = new Size(availableSize.Width, maxHeight);
			return size.FiniteOrDefault(default);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			if (Children.Count < 1)
			{
				return finalSize;
			}

			Size cellSize = new Size(finalSize.Width / Children.Count, finalSize.Height);
			int col = 0;

			foreach (var child in Children)
			{
				child.Arrange(new Rect(new Point(cellSize.Width * col, 0), cellSize));
				col++;
			}

			return finalSize;
		}
	}
}
