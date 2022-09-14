#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI
{
	[ContentProperty(Name = nameof(Children))]
	[TemplatePart(Name = PART_RootGrid, Type = typeof(Grid))]
	public partial class AutoLayout : Control
	{
		private Grid? _grid;
		private const string PART_RootGrid = nameof(PART_RootGrid);

		public AutoLayout()
		{
			DefaultStyleKey = typeof(AutoLayout);
			Children = new AutoLayoutChildren();
		}

		protected override void OnApplyTemplate()
		{
			_grid = GetTemplateChild(PART_RootGrid) as Grid;

			base.OnApplyTemplate();

			UpdateAlignments();
		}

		public static readonly DependencyProperty IsReverseZIndexProperty = DependencyProperty.Register(
			"IsReverseZIndex", typeof(bool), typeof(AutoLayout), new PropertyMetadata(default(bool), propertyChangedCallback: UpdateCallback));

		public bool IsReverseZIndex
		{
			get => (bool)GetValue(IsReverseZIndexProperty);
			set => SetValue(IsReverseZIndexProperty, value);         
		}

		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
			"Orientation", typeof(Orientation), typeof(AutoLayout), new PropertyMetadata(default(Orientation), propertyChangedCallback: UpdateCallback));

		public Orientation Orientation
		{
			get => (Orientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(
			"Spacing", typeof(double), typeof(AutoLayout), new PropertyMetadata(default(double), propertyChangedCallback: UpdateCallback));

		public double Spacing
		{
			get => (double)GetValue(SpacingProperty);
			set => SetValue(SpacingProperty, value);
		}

		public static readonly DependencyProperty JustifyProperty = DependencyProperty.Register(
			"Justify", typeof(AutoLayoutJustify), typeof(AutoLayout), new PropertyMetadata(default(AutoLayoutJustify), propertyChangedCallback: UpdateCallback));

		public AutoLayoutJustify Justify
		{
			get => (AutoLayoutJustify)GetValue(JustifyProperty);
			set => SetValue(JustifyProperty, value);
		}

		public static readonly DependencyProperty ChildrenProperty = DependencyProperty.Register(
			"Children", typeof(AutoLayoutChildren), typeof(AutoLayout), new PropertyMetadata(default(AutoLayoutChildren), propertyChangedCallback: UpdateCallback));

		public AutoLayoutChildren Children
		{
			get => (AutoLayoutChildren)GetValue(ChildrenProperty);
			set => SetValue(ChildrenProperty, value);
		}

		public static readonly DependencyProperty PrimaryAlignmentProperty = DependencyProperty.RegisterAttached(
			"PrimaryAlignment",
			typeof(AutoLayoutPrimaryAlignment),
			typeof(AutoLayout),
			new PropertyMetadata(default(AutoLayoutPrimaryAlignment), propertyChangedCallback: UpdateAttachedCallback));

		public static void SetPrimaryAlignment(DependencyObject element, AutoLayoutPrimaryAlignment value)
		{
			element.SetValue(PrimaryAlignmentProperty, value);
		}

		public static AutoLayoutPrimaryAlignment GetPrimaryAlignment(DependencyObject element)
		{
			return (AutoLayoutPrimaryAlignment)element.GetValue(PrimaryAlignmentProperty);
		}

		public static readonly DependencyProperty CounterAlignmentProperty = DependencyProperty.RegisterAttached(
			"CounterAlignment",
			typeof(AutoLayoutAlignment),
			typeof(AutoLayout),
			new PropertyMetadata(AutoLayoutAlignment.Stretch, propertyChangedCallback: UpdateAttachedCallback));

		public static void SetCounterAlignment(DependencyObject element, AutoLayoutAlignment value)
		{
			element.SetValue(CounterAlignmentProperty, value);
		}

		public static AutoLayoutAlignment GetCounterAlignment(DependencyObject element)
		{
			return (AutoLayoutAlignment)element.GetValue(CounterAlignmentProperty);
		}

		public static readonly DependencyProperty PrimaryLengthProperty = DependencyProperty.RegisterAttached(
			"PrimaryLength", typeof(double), typeof(AutoLayout), new PropertyMetadata(default(double), propertyChangedCallback: UpdateAttachedCallback));

		public static void SetPrimaryLength(DependencyObject element, double value)
		{
			element.SetValue(PrimaryLengthProperty, value);
		}

		public static double GetPrimaryLength(DependencyObject element)
		{
			return (double)element.GetValue(PrimaryLengthProperty);
		}

		public static readonly DependencyProperty CounterLengthProperty = DependencyProperty.RegisterAttached(
			"CounterLength", typeof(double), typeof(AutoLayout), new PropertyMetadata(default(double), propertyChangedCallback: UpdateAttachedCallback));

		public static void SetCounterLength(DependencyObject element, double value)
		{
			element.SetValue(CounterLengthProperty, value);
		}

		public static double GetCounterLength(DependencyObject element)
		{
			return (double)element.GetValue(CounterLengthProperty);
		}

		public static readonly DependencyProperty PrimaryAxisAlignmentProperty = DependencyProperty.Register(
			"PrimaryAxisAlignment",
			typeof(AutoLayoutAlignment),
			typeof(AutoLayout),
			new PropertyMetadata(default(AutoLayoutAlignment), propertyChangedCallback: UpdateCallback));

		public AutoLayoutAlignment PrimaryAxisAlignment
		{
			get => (AutoLayoutAlignment)GetValue(PrimaryAxisAlignmentProperty);
			set => SetValue(PrimaryAxisAlignmentProperty, value);
		}

		private static void UpdateCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is AutoLayout al)
			{
				al.UpdateAlignments();
			}
		}

		private static void UpdateAttachedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is FrameworkElement fe)
			{
				if (fe.Parent is AutoLayout al)
				{
					al.UpdateAlignments();
				}
			}
		}

		private void UpdateAlignments()
		{
			if (_grid == null)
			{
				return;
			}

			var isVertical = Orientation is Orientation.Vertical;
			var isSpaceBetween = Justify == AutoLayoutJustify.SpaceBetween;

			var childrenCount = Children.Count;

			if (childrenCount == 0)
			{
				_grid.Children.Clear();
				_grid.RowDefinitions.Clear();
				_grid.ColumnDefinitions.Clear();
				return;
			}

			var gridDefinitionsCount = isSpaceBetween ? (childrenCount * 2) - 1 : childrenCount;

			// Inject & Move elements in the inner grid
			for (var i = 0; i < childrenCount; i++)
			{
				var child = Children[IsReverseZIndex ? childrenCount-i-1 : i];

				if (child.Parent != null)
				{
					// We need to move instead of adding the element
					var currentIndexInGrid = _grid.Children.IndexOf(child);
					if (currentIndexInGrid == i)
					{
						// nothing to do, already at right place
						continue;
					}

					_grid.Children.Move((uint)currentIndexInGrid, (uint)i);
				}
				else
				{
					if (_grid.Children.Count > i)
					{
						_grid.Children.Insert(i, child);
					}
					else
					{
						_grid.Children.Add(child);
					}
				}
			}

			// remove any extra children on grid
			while (Children.Count < _grid.Children.Count)
			{
				_grid.Children.RemoveAt(_grid.Children.Count - 1);
			}

			// Set inter-element spacing
			if (isVertical)
			{
				_grid.RowSpacing = Spacing;
				_grid.ClearValue(Grid.ColumnSpacingProperty);
			}
			else
			{
				_grid.ColumnSpacing = Spacing;
				_grid.ClearValue(Grid.RowSpacingProperty);
			}

			bool atLeastOneChildFillAvailableSpaceInPrimaryAxis = false;

			if (isVertical)
			{
				// Ensure no columns defined (orientation is vertical)
				_grid.ColumnDefinitions.Clear();

				// Ensure definitions is of right size
				while (_grid.RowDefinitions.Count < gridDefinitionsCount)
				{
					_grid.RowDefinitions.Add(new RowDefinition());
				}

				while (_grid.RowDefinitions.Count > gridDefinitionsCount)
				{
					_grid.RowDefinitions.RemoveAt(_grid.RowDefinitions.Count - 1);
				}

				// Process children
				for (var i = 0; i < childrenCount; i++)
				{
					var child = Children[i];
					var gridIndex = isSpaceBetween ? i * 2 : i;

					var gridDefinition = _grid.RowDefinitions[gridIndex];

					Grid.SetRow(child, gridIndex);

					// Get relative alignment or default if not set + set on child element
					var primaryAlignment = GetPrimaryAlignment(child);
					if (primaryAlignment is AutoLayoutPrimaryAlignment.Stretch)
					{
						gridDefinition.Height = new GridLength(1, GridUnitType.Star);
						atLeastOneChildFillAvailableSpaceInPrimaryAxis = true;
					}
					else if (GetPrimaryLength(child) is var height and > 0)
					{
						gridDefinition.Height = new GridLength(height);
					}
					else
					{
						gridDefinition.Height = GridLength.Auto;
					}

					child.VerticalAlignment = VerticalAlignment.Stretch;
					child.HorizontalAlignment = GetCounterAlignment(child).ToHorizontalAlignment();

					if (GetCounterLength(child) is var width and > 0)
					{
						child.Width = width;
					}
				}

				// Process "space between"
				if (isSpaceBetween)
				{
					if (atLeastOneChildFillAvailableSpaceInPrimaryAxis)
					{
						for (var i = 1; i < gridDefinitionsCount; i += 2)
						{
							_grid.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Auto);
						}
					}
					else
					{
						for (var i = 1; i < gridDefinitionsCount; i += 2)
						{
							_grid.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Star);
						}
					}
				}
			}
			else // Horizontal
			{
				// Ensure no rows defined (orientation is horizontal)
				_grid.RowDefinitions.Clear();

				// Ensure definitions is of right size
				while (_grid.ColumnDefinitions.Count < gridDefinitionsCount)
				{
					_grid.ColumnDefinitions.Add(new ColumnDefinition());
				}

				while (_grid.ColumnDefinitions.Count > gridDefinitionsCount)
				{
					_grid.ColumnDefinitions.RemoveAt(_grid.ColumnDefinitions.Count - 1);
				}

				// Process children
				for (var i = 0; i < childrenCount; i++)
				{
					var child = Children[i];
					var gridIndex = isSpaceBetween ? i * 2 : i;

					var gridDefinition = _grid.ColumnDefinitions[gridIndex];

					Grid.SetColumn(child, gridIndex);

					// Get relative alignment or default if not set + set on child element
					var primaryAlignment = GetPrimaryAlignment(child);
					if (primaryAlignment is AutoLayoutPrimaryAlignment.Stretch)
					{
						gridDefinition.Width = new GridLength(1, GridUnitType.Star);
						atLeastOneChildFillAvailableSpaceInPrimaryAxis = true;
					}
					else if (GetPrimaryLength(child) is var width and > 0)
					{
						gridDefinition.Width = new GridLength(width);
					}
					else
					{
						gridDefinition.Width = GridLength.Auto;
					}

					child.HorizontalAlignment = HorizontalAlignment.Stretch;
					child.VerticalAlignment = GetCounterAlignment(child).ToVerticalAlignment();

					if (GetCounterLength(child) is var height and > 0)
					{
						child.Height = height;
					}
				}

				// Process "space between"
				if (isSpaceBetween)
				{
					if (atLeastOneChildFillAvailableSpaceInPrimaryAxis)
					{
						for (var i = 1; i < gridDefinitionsCount; i += 2)
						{
							_grid.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Auto);
						}
					}
					else
					{
						for (var i = 1; i < gridDefinitionsCount; i += 2)
						{
							_grid.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Star);
						}
					}
				}
			}

			// Set container alignments
			if (isVertical)
			{
				if (atLeastOneChildFillAvailableSpaceInPrimaryAxis || isSpaceBetween)
				{
					_grid.ClearValue(VerticalAlignmentProperty);
				}
				else
				{
					_grid.VerticalAlignment = PrimaryAxisAlignment.ToVerticalAlignment();
				}
				_grid.ClearValue(HorizontalAlignmentProperty);
			}
			else
			{
				if (atLeastOneChildFillAvailableSpaceInPrimaryAxis || isSpaceBetween)
				{
					_grid.ClearValue(HorizontalAlignmentProperty);
				}
				else
				{
					_grid.HorizontalAlignment = PrimaryAxisAlignment.ToHorizontalAlignment();
				}
				_grid.ClearValue(VerticalAlignmentProperty);
			}
		}
	}
}
