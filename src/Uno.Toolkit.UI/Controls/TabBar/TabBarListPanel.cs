using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uno.Extensions.Specialized;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI
{
	public partial class TabBarListPanel : Panel
	{
		public Orientation Orientation
		{
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}

		public static DependencyProperty OrientationProperty { get; } = DependencyProperty.Register(
			nameof(Orientation),
			typeof(Orientation),
			typeof(TabBarListPanel),
			new PropertyMetadata(Orientation.Horizontal, (s, e) => ((TabBarListPanel)s).OnPropertyChanged(e)));

		private void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
		{
			if (args.Property == OrientationProperty)
			{
				InvalidateMeasure();
			}
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			var desiredSize = new Size();

			var totalWidth = 0d;
			var totalHeight = 0d;
			var widthOfWidest = 0d;
			var heightOfTallest = 0d;

			var visibleChildren = Children.Where(IsVisible);
			var count = visibleChildren.Count();

			foreach (var child in visibleChildren)
			{
				child.Measure(availableSize);

				var childDesiredSize = child.DesiredSize.FiniteOrDefault(default);

				// Calculate width of all items as if they were laid out horizontally
				totalWidth += childDesiredSize.Width;
				if (childDesiredSize.Width > widthOfWidest)
				{
					widthOfWidest = childDesiredSize.Width;
				}

				// Calculate height of all items if they were laid out vertically
				totalHeight += childDesiredSize.Height;
				if (childDesiredSize.Height > heightOfTallest)
				{
					heightOfTallest = childDesiredSize.Height;
				}
			}

			if (Orientation == Orientation.Vertical)
			{
				desiredSize.Width = widthOfWidest;
				desiredSize.Height = totalHeight;
			}
			else
			{
				desiredSize.Width = totalWidth;
				desiredSize.Height = heightOfTallest;
			}

			return desiredSize;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			var visibleChildren = Children.Where(IsVisible);
			var count = visibleChildren.Count();
			if (count < 1)
			{
				return finalSize;
			}

			Size cellSize;
			if (Orientation == Orientation.Vertical)
			{
				cellSize = new Size(finalSize.Width, finalSize.Height / count);
			}
			else
			{
				cellSize = new Size(finalSize.Width / count, finalSize.Height);
			}

			int pos = 0;
			foreach (var child in visibleChildren)
			{
				Point arrangePoint;
				if (Orientation == Orientation.Vertical)
				{
					arrangePoint = new Point(0, cellSize.Height * pos);
				}
				else
				{
					arrangePoint = new Point(cellSize.Width * pos, 0);
				}

				child.Arrange(new Rect(arrangePoint, cellSize));
				pos++;
			}

			return finalSize;
		}

		private bool IsVisible(UIElement x) => x.Visibility == Visibility.Visible;
	}
}
