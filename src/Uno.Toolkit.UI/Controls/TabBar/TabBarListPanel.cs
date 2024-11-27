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
		#region Orientation
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
		#endregion

		public TabBarListPanel()
		{
			this.Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			var owner = this.FindFirstParent<TabBar>();

			// workaround for #1287 ItemsPanelRoot resolution timing related issue
			owner?.OnItemsPanelConnected(this);
		}

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
			var widthOfWidest = 0d;
			var heightOfTallest = 0d;

			var visibleChildren = Children.Where(IsVisible).ToArray();

			var count = visibleChildren.Length;
			if (count < 1)
			{
				return availableSize.FiniteOrDefault(default);
			}

			Size cellSize = new Size();
			if (Orientation == Orientation.Vertical)
			{
				cellSize = new Size(availableSize.Width, availableSize.Height / count);
			}
			else
			{
				cellSize = new Size(availableSize.Width / count, availableSize.Height);
			}


			foreach (var child in visibleChildren)
			{
				child.Measure(cellSize);

				var childDesiredSize = child.DesiredSize.FiniteOrDefault(default);

				if (childDesiredSize.Width > widthOfWidest)
				{
					widthOfWidest = childDesiredSize.Width;
				}

				if (childDesiredSize.Height > heightOfTallest)
				{
					heightOfTallest = childDesiredSize.Height;
				}
			}

			if (Orientation == Orientation.Vertical)
			{
				desiredSize.Width = widthOfWidest;
				desiredSize.Height = availableSize.Height;
			}
			else
			{
				desiredSize.Width = availableSize.Width;
				desiredSize.Height = heightOfTallest;
			}

			return desiredSize.FiniteOrDefault(default);;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			var visibleChildren = Children.Where(IsVisible).ToArray();
			var count = visibleChildren.Length;
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
