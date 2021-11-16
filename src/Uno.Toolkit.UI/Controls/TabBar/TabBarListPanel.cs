using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Uno.Toolkit.UI.Extensions;

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
			var count = Children.Count(IsVisible);
			if (count < 1)
			{
				return availableSize.FiniteOrDefault(default);
			}

			Size cellSize = new Size(availableSize.Width / count, availableSize.Height);
			foreach (var child in Children)
			{
				child.Measure(cellSize);
			}

			var maxHeight = Children.Where(IsVisible).Max(x => x.DesiredSize.Height);

			var size = new Size(availableSize.Width, maxHeight);
			return size.FiniteOrDefault(default);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			var count = Children.Count(IsVisible);
			if (count < 1)
			{
				return finalSize;
			}

			Size cellSize = new Size(finalSize.Width / count, finalSize.Height);
			int col = 0;

			foreach (var child in Children.Where(IsVisible))
			{
				child.Arrange(new Rect(new Point(cellSize.Width * col, 0), cellSize));
				col++;
			}

			return finalSize;
		}

		private bool IsVisible(UIElement x) => x.Visibility == Visibility.Visible;
	}
}
