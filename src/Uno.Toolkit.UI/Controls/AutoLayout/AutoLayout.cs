using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

			Loaded += OnLoaded;

			this.RegisterDisposablePropertyChangedCallback(HorizontalAlignmentProperty, (snd, e) => UpdateAutoLayout());
			this.RegisterDisposablePropertyChangedCallback(VerticalAlignmentProperty, (snd, e) => UpdateAutoLayout());
			this.RegisterDisposablePropertyChangedCallback(PaddingProperty, (snd, e) => UpdateAutoLayout());
		}

		protected override void OnApplyTemplate()
		{
			_grid = GetTemplateChild(PART_RootGrid) as Grid;

			base.OnApplyTemplate();

			UpdateAutoLayout();
		}

		private static void OnLoaded(object sender, RoutedEventArgs e)
		{
			(sender as AutoLayout)?.UpdateAutoLayout();
		}

		public static readonly DependencyProperty IsReverseZIndexProperty = DependencyProperty.Register(
			"IsReverseZIndex", typeof(bool), typeof(AutoLayout), new PropertyMetadata(default(bool), propertyChangedCallback: UpdateCallback));

		public bool IsReverseZIndex
		{
			get => (bool)GetValue(IsReverseZIndexProperty);
			set => SetValue(IsReverseZIndexProperty, value);         
		}

		private bool IsHorizontalHug => HorizontalAlignment is not HorizontalAlignment.Stretch && Width is double.NaN;

		private bool IsVerticalHug => VerticalAlignment is not VerticalAlignment.Stretch && Height is double.NaN;

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

		[DynamicDependency(nameof(GetPrimaryAlignment))]
		public static readonly DependencyProperty PrimaryAlignmentProperty = DependencyProperty.RegisterAttached(
			"PrimaryAlignment",
			typeof(AutoLayoutPrimaryAlignment),
			typeof(AutoLayout),
			new PropertyMetadata(default(AutoLayoutPrimaryAlignment), propertyChangedCallback: UpdateAttachedCallback));

		[DynamicDependency(nameof(GetPrimaryAlignment))]
		public static void SetPrimaryAlignment(DependencyObject element, AutoLayoutPrimaryAlignment value)
		{ 
			element.SetValue(PrimaryAlignmentProperty, value);
		}

		[DynamicDependency(nameof(GetPrimaryAlignment))]
		public static AutoLayoutPrimaryAlignment GetPrimaryAlignment(DependencyObject element)
		{
			return (AutoLayoutPrimaryAlignment)element.GetValue(PrimaryAlignmentProperty);
		}

		[DynamicDependency(nameof(GetCounterAlignment))]
		public static readonly DependencyProperty CounterAlignmentProperty = DependencyProperty.RegisterAttached(
			"CounterAlignment",
			typeof(AutoLayoutAlignment),
			typeof(AutoLayout),
			new PropertyMetadata(AutoLayoutAlignment.Stretch, propertyChangedCallback: UpdateAttachedCallback));

		[DynamicDependency(nameof(GetCounterAlignment))]
		public static void SetCounterAlignment(DependencyObject element, AutoLayoutAlignment value)
		{
			element.SetValue(CounterAlignmentProperty, value);
		}

		[DynamicDependency(nameof(SetCounterAlignment))]
		public static AutoLayoutAlignment GetCounterAlignment(DependencyObject element)
		{
			return (AutoLayoutAlignment)element.GetValue(CounterAlignmentProperty);
		}

		[DynamicDependency(nameof(GetIsIndependentLayout))]
		public static readonly DependencyProperty IsIndependentLayoutProperty = DependencyProperty.RegisterAttached(
			"IsIndependentLayout", typeof(bool), typeof(AutoLayout), new PropertyMetadata(default(bool), propertyChangedCallback: UpdateAttachedCallback));

		[DynamicDependency(nameof(GetIsIndependentLayout))]
		public static void SetIsIndependentLayout(DependencyObject element, bool value)
		{
			element.SetValue(IsIndependentLayoutProperty, value);
		}

		[DynamicDependency(nameof(SetIsIndependentLayout))]
		public static bool GetIsIndependentLayout(DependencyObject element)
		{
			return (bool)element.GetValue(IsIndependentLayoutProperty);
		}

		[DynamicDependency(nameof(GetPrimaryLength))]
		public static readonly DependencyProperty PrimaryLengthProperty = DependencyProperty.RegisterAttached(
			"PrimaryLength", typeof(double), typeof(AutoLayout), new PropertyMetadata(default(double), propertyChangedCallback: UpdateAttachedCallback));

		[DynamicDependency(nameof(GetPrimaryLength))]
		public static void SetPrimaryLength(DependencyObject element, double value)
		{
			element.SetValue(PrimaryLengthProperty, value);
		}

		[DynamicDependency(nameof(SetPrimaryLength))]
		public static double GetPrimaryLength(DependencyObject element)
		{
			return (double)element.GetValue(PrimaryLengthProperty);
		}

		[DynamicDependency(nameof(GetCounterLength))]
		public static readonly DependencyProperty CounterLengthProperty = DependencyProperty.RegisterAttached(
			"CounterLength", typeof(double), typeof(AutoLayout), new PropertyMetadata(default(double), propertyChangedCallback: UpdateAttachedCallback));

		[DynamicDependency(nameof(GetCounterLength))]
		public static void SetCounterLength(DependencyObject element, double value)
		{
			element.SetValue(CounterLengthProperty, value);
		}

		[DynamicDependency(nameof(SetCounterLength))]
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
				al.UpdateAutoLayout();
			}
		}

		private static void UpdateAttachedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is FrameworkElement fe)
			{
				if (fe.Parent is AutoLayout al)
				{
					al.UpdateAutoLayout();
				}
			}
		}

		private void UpdateAutoLayout()
		{
			if (_grid == null || IsLoaded is false)
			{
				return;
			}

			var padding = Padding;
			var hasPadding = !padding.Equals(default(Thickness));
			var isVertical = Orientation is Orientation.Vertical;
			var spacing = Spacing;
			var hasSpace = Spacing != 0 || Justify == AutoLayoutJustify.SpaceBetween;
			var hasSpaceBetween = Justify == AutoLayoutJustify.SpaceBetween;
			var isSpacing = spacing != 0 && !hasSpaceBetween;

			var children = Children;

			var childrenCount = children.Count;

			if (childrenCount == 0)
			{
				_grid.Children.Clear();
				_grid.RowDefinitions.Clear();
				_grid.ColumnDefinitions.Clear();
				return;
			}

			_grid.Padding = new Thickness(0);

			var independentLayoutCount = children.Where(child => GetIsIndependentLayout(child)).Count();
			var hasIndependentLayout = independentLayoutCount > 0;


			var gridIndexOffSet = 0;

			bool atLeastOneChildFillAvailableSpaceInPrimaryAxis = children.Where(child => GetPrimaryAlignment(child) is AutoLayoutPrimaryAlignment.Stretch).Any();

			var shouldAddInBetweenRow = (hasSpaceBetween && atLeastOneChildFillAvailableSpaceInPrimaryAxis is false) || spacing > 0;
			var gridDefinitionsCount = shouldAddInBetweenRow ? ((childrenCount - independentLayoutCount) * 2) - 1 : (childrenCount - independentLayoutCount);

			var isPrimaryAxisAlignmentCenterOrEnd = hasIndependentLayout && PrimaryAxisAlignment is AutoLayoutAlignment.Center or AutoLayoutAlignment.End;

			// Inject & Move elements in the inner grid
			for (var i = 0; i < childrenCount; i++)
			{
				var child = children[IsReverseZIndex ? childrenCount-i-1 : i];

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
			while (childrenCount < _grid.Children.Count)
			{
				_grid.Children.RemoveAt(_grid.Children.Count - 1);
			}

			// Set inter-element spacing
			if (hasSpace)
			{
				_grid.ClearValue(Grid.ColumnSpacingProperty);
				_grid.ClearValue(Grid.RowSpacingProperty);
			}


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

				var rawChildIndex = 0;

				if (hasPadding)
				{
					_grid.Padding = new Thickness(0);

					var topPaddingSize = spacing < 0 ? padding.Top - spacing : padding.Top;
					var topPadding = new GridLength(topPaddingSize);

					_grid.RowDefinitions.Insert(gridIndexOffSet, new RowDefinition() { Height = topPadding, MaxHeight = topPaddingSize});
					gridIndexOffSet += 1;

					_grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(padding.Left )});
					_grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, IsHorizontalHug ? GridUnitType.Auto : GridUnitType.Star) });
					_grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(padding.Right) });
				}

				if (hasSpaceBetween is false && atLeastOneChildFillAvailableSpaceInPrimaryAxis is false && isPrimaryAxisAlignmentCenterOrEnd)
				{
					_grid.RowDefinitions.Insert(gridIndexOffSet, new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
					gridIndexOffSet += 1;
				}
				// Process children
				for (var i = 0; i < childrenCount; i++)
				{
					var child = children[i];

					if (GetIsIndependentLayout(child))
					{
						Grid.SetRow(child, 0);
						// two extra count for the span. Depending of the PrimaryAxisAlignment we need to add two extra grid
						Grid.SetRowSpan(child, gridDefinitionsCount + 4);
						Grid.SetColumnSpan(child, gridDefinitionsCount + 4);
						continue;
					}

					var gridIndex = shouldAddInBetweenRow ? rawChildIndex * 2 : rawChildIndex;
					//We add a grid. we need to adjust the index.
					var adjustGridIndex = gridIndex + gridIndexOffSet;
					Grid.SetRow(child, adjustGridIndex);
					var gridDefinition = _grid.RowDefinitions[adjustGridIndex];

					// Get relative alignment or default if not set + set on child element
					var primaryAlignment = GetPrimaryAlignment(child);
					if (primaryAlignment is AutoLayoutPrimaryAlignment.Stretch)
					{
						gridDefinition.Height = new GridLength(1, GridUnitType.Star);
						child.VerticalAlignment = VerticalAlignment.Stretch;
					}
					else if (GetPrimaryLength(child) is var height and > 0)
					{
						gridDefinition.Height = new GridLength(height);
						child.VerticalAlignment = VerticalAlignment.Stretch;
					}
					else
					{
						gridDefinition.Height = GridLength.Auto;
						child.VerticalAlignment = VerticalAlignment.Top;
					}

					var counterAlignment = GetCounterAlignment(child).ToHorizontalAlignment();

					child.HorizontalAlignment = counterAlignment;

					if (hasPadding)
					{
						switch (counterAlignment)
						{
							case HorizontalAlignment.Left:
								Grid.SetColumn(child, 1);
								Grid.SetColumnSpan(child, IsHorizontalHug ? 1 : 2);
								break;
							case HorizontalAlignment.Center:
								Grid.SetColumn(child, 1);
								break;
							case HorizontalAlignment.Right:
								Grid.SetColumn(child, IsHorizontalHug ? 1 : 0);
								Grid.SetColumnSpan(child, IsHorizontalHug ? 1 : 2);
								break;
							case HorizontalAlignment.Stretch:
								Grid.SetColumn(child, 1);
								break;
							default:
								break;
						}
					}

					if (GetCounterLength(child) is var width and > 0)
					{
						child.Width = width;
					}
					rawChildIndex++;
				}

				// Process "space between"
				if (hasSpace)
				{
					if (isSpacing)
					{
						if (spacing < 0)
						{
							_grid.RowSpacing = spacing;
							_grid.ClearValue(Grid.ColumnSpacingProperty);
						}
						else
						{
							for (var i = 1 + gridIndexOffSet; i < gridDefinitionsCount + gridIndexOffSet; i += 2)
							{
								_grid.RowDefinitions[i].Height = new GridLength(spacing);
							}
						}
					}
					else if (hasSpaceBetween)
					{
						for (var i = 1 + gridIndexOffSet; i < gridDefinitionsCount + gridIndexOffSet; i += 2)
						{
							_grid.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Star);
						}
					}
				}

				if (hasIndependentLayout && atLeastOneChildFillAvailableSpaceInPrimaryAxis is not true && PrimaryAxisAlignment != AutoLayoutAlignment.End && !hasSpaceBetween)
				{
					//We need to make sure that the independent layout can span all across his parent
					_grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
				}

				var paddingCondition = atLeastOneChildFillAvailableSpaceInPrimaryAxis || PrimaryAxisAlignment is not AutoLayoutAlignment.Start || IsVerticalHug;

				if ( hasSpaceBetween || (hasPadding && paddingCondition))
				{
					var bottomPaddingSize = spacing < 0 ? padding.Bottom - spacing : padding.Bottom;
					_grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(bottomPaddingSize), MaxHeight = bottomPaddingSize });
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

				var rawChildIndex = 0;

				if (hasPadding)
				{
					_grid.Padding = new Thickness(0);

					var firstColumnWidthSize = spacing < 0 ? padding.Left - spacing : padding.Left;
					var firstColumnWidth = new GridLength(firstColumnWidthSize);

					_grid.ColumnDefinitions.Insert(gridIndexOffSet, new ColumnDefinition() { Width = firstColumnWidth, MaxWidth = firstColumnWidthSize });
					gridIndexOffSet += 1;

					_grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(padding.Top) });
					_grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
					_grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(padding.Bottom) }); 
				}

				if (hasSpaceBetween is false && atLeastOneChildFillAvailableSpaceInPrimaryAxis is false && isPrimaryAxisAlignmentCenterOrEnd )
				{
					_grid.ColumnDefinitions.Insert(gridIndexOffSet, new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
					gridIndexOffSet += 1;
				}
				// Process children
				for (var i = 0; i < childrenCount; i++)
				{
					var child = children[i];
					if (GetIsIndependentLayout(child))
					{
						Grid.SetColumn(child, 0);
						// two extra count for the span. Depending of the PrimaryAxisAlignment we need to add two extra grid
						Grid.SetColumnSpan(child, gridDefinitionsCount + 4);
						Grid.SetRowSpan(child, gridDefinitionsCount + 4);
						continue;
					}

					var gridIndex = shouldAddInBetweenRow ? rawChildIndex * 2 : rawChildIndex;
					//We add a grid. we need to adjust the index.
					var adjustGridIndex = gridIndex + gridIndexOffSet;
					var gridDefinition = _grid.ColumnDefinitions[adjustGridIndex];
					Grid.SetColumn(child, adjustGridIndex);

					// Get relative alignment or default if not set + set on child element
					var primaryAlignment = GetPrimaryAlignment(child);
					if (primaryAlignment is AutoLayoutPrimaryAlignment.Stretch)
					{
						gridDefinition.Width = new GridLength(1, GridUnitType.Star);
						child.HorizontalAlignment = HorizontalAlignment.Stretch;
					}
					else if (GetPrimaryLength(child) is var width and > 0)
					{
						gridDefinition.Width = new GridLength(width);
						child.HorizontalAlignment = HorizontalAlignment.Stretch;
					}
					else
					{
						gridDefinition.Width = GridLength.Auto;
						child.HorizontalAlignment = HorizontalAlignment.Left;
					}

					var counterAlignment = GetCounterAlignment(child).ToVerticalAlignment();

					child.VerticalAlignment = counterAlignment;

					if (hasPadding)
					{
						switch (counterAlignment)
						{
							case VerticalAlignment.Top:
								Grid.SetRow(child, 1);
								Grid.SetRowSpan(child, IsVerticalHug ? 1 : 2);
								break;
							case VerticalAlignment.Center:
								Grid.SetRow(child, 1);
								break;
							case VerticalAlignment.Bottom:
								Grid.SetRow(child, IsVerticalHug ? 1 : 0);
								Grid.SetRowSpan(child, 2);
								break;
							case VerticalAlignment.Stretch:
								Grid.SetRow(child, 1);
								break;
							default:
								break;
						}
					}

					if (GetCounterLength(child) is var height and > 0)
					{
						child.Height = height;
					}
					rawChildIndex++;
				}

				// Process "space between"
				if (hasSpace)
				{
					if (isSpacing)
					{

						if (spacing < 0)
						{
							_grid.ColumnSpacing = spacing;
							_grid.ClearValue(Grid.RowSpacingProperty);
						}
						else
						{
							for (var i = 1 + gridIndexOffSet; i < gridDefinitionsCount + gridIndexOffSet; i += 2)
							{
								_grid.ColumnDefinitions[i].Width = new GridLength(spacing);
							}
						}
					}
					else if (hasSpaceBetween)
					{
						for (var i = 1 + gridIndexOffSet; i < gridDefinitionsCount + gridIndexOffSet; i += 2)
						{
							_grid.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Star);
						}
					}
				}

				if (hasIndependentLayout && atLeastOneChildFillAvailableSpaceInPrimaryAxis is not true && PrimaryAxisAlignment != AutoLayoutAlignment.End)
				{
					//We need to make sure that the independent layout can span all across his parent
					_grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
				}

				var paddingCondition = atLeastOneChildFillAvailableSpaceInPrimaryAxis || PrimaryAxisAlignment is not AutoLayoutAlignment.Start || IsHorizontalHug;

				if (hasSpaceBetween || (hasPadding && paddingCondition))
				{
					var rightPaddingsize = spacing < 0 ? padding.Right - spacing : padding.Right;
					var lastColumnWidth = new GridLength(rightPaddingsize);

					_grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = lastColumnWidth, MaxWidth = rightPaddingsize });
				}
			}

			var shouldUsePrimaryAxisAlignment = atLeastOneChildFillAvailableSpaceInPrimaryAxis || hasSpaceBetween || hasIndependentLayout;

			// Set container alignments
			if (isVertical)
			{
				if (shouldUsePrimaryAxisAlignment)
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
				if (shouldUsePrimaryAxisAlignment)
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
