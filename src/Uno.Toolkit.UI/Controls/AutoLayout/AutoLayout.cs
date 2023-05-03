using System;
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

namespace Uno.Toolkit.UI;

public sealed partial class AutoLayout : RelativePanel
{
	// private void UpdateAutoLayout()
	// {
	// 	if (_grid == null || IsLoaded is false)
	// 	{
	// 		return;
	// 	}
	//
	// 	var children = Children;
	// 	var childrenCount = children.Count;
	//
	// 	if (childrenCount == 0)
	// 	{
	// 		_grid.Children.Clear();
	// 		_grid.RowDefinitions.Clear();
	// 		_grid.ColumnDefinitions.Clear();
	// 		return;
	// 	}
	//
	// 	var padding = Padding;
	// 	var isVertical = Orientation is Orientation.Vertical;
	// 	var spacing = Spacing;
	// 	var hasSpace = Spacing != 0 || Justify == AutoLayoutJustify.SpaceBetween;
	// 	var hasSpaceBetween = Justify == AutoLayoutJustify.SpaceBetween;
	// 	var isSpacing = spacing != 0 && !hasSpaceBetween;
	//
	// 	var childrenCounterALignment = children.Any() ? GetCounterAlignment(children.First()) : AutoLayoutAlignment.Start;
	//
	// 	bool atLeastOneChildFillAvailableSpaceInPrimaryAxis = children.Where(child => GetPrimaryAlignment(child) is AutoLayoutPrimaryAlignment.Stretch).Any();
	// 	bool atLeastOneChildFillAvailableSpaceInCounterAxis = children.Where(child => GetCounterAlignment(child) is AutoLayoutAlignment.Stretch).Any();
	//
	// 	var childrenVerticallAlignment = Orientation == Orientation.Vertical ?
	// 		atLeastOneChildFillAvailableSpaceInPrimaryAxis ? VerticalAlignment.Stretch : PrimaryAxisAlignment.ToVerticalAlignment() :
	// 		atLeastOneChildFillAvailableSpaceInCounterAxis ? VerticalAlignment.Stretch : childrenCounterALignment.ToVerticalAlignment();
	// 	var childrenHorizontalAlignment = Orientation == Orientation.Vertical ?
	// 		atLeastOneChildFillAvailableSpaceInCounterAxis ? HorizontalAlignment.Stretch : childrenCounterALignment.ToHorizontalAlignment() :
	// 		atLeastOneChildFillAvailableSpaceInPrimaryAxis ? HorizontalAlignment.Stretch : PrimaryAxisAlignment.ToHorizontalAlignment();
	//
	// 	var independentLayoutCount = children.Where(child => GetIsIndependentLayout(child)).Count();
	// 	var hasIndependentLayout = independentLayoutCount > 0;
	//
	// 	var hasPadding = !padding.Equals(default(Thickness));
	//
	// 	var gridRowIndexOffSet = 0;
	// 	var gridColumnIndexOffSet = 0;
	//
	// 	var shouldAddInBetweenRow = (hasSpaceBetween && atLeastOneChildFillAvailableSpaceInPrimaryAxis is false) || spacing > 0;
	// 	var gridDefinitionsCount = shouldAddInBetweenRow ? ((childrenCount - independentLayoutCount) * 2) - 1 : (childrenCount - independentLayoutCount);
	//
	// 	var isPrimaryAxisAlignmentCenterOrEnd = hasIndependentLayout && PrimaryAxisAlignment is AutoLayoutAlignment.Center or AutoLayoutAlignment.End;
	//
	// 	// Inject & Move elements in the inner grid
	// 	for (var i = 0; i < childrenCount; i++)
	// 	{
	// 		var child = children[IsReverseZIndex ? childrenCount-i-1 : i];
	//
	// 		if (child.Parent != null)
	// 		{
	// 			// We need to move instead of adding the element
	// 			var currentIndexInGrid = _grid.Children.IndexOf(child);
	// 			if (currentIndexInGrid == i)
	// 			{
	// 				// nothing to do, already at right place
	// 				continue;
	// 			}
	//
	// 			_grid.Children.Move((uint)currentIndexInGrid, (uint)i);
	// 		}
	// 		else
	// 		{
	// 			if (_grid.Children.Count > i)
	// 			{
	// 				_grid.Children.Insert(i, child);
	// 			}
	// 			else
	// 			{
	// 				_grid.Children.Add(child);
	// 			}
	// 		}
	// 	}
	//
	// 	// remove any extra children on grid
	// 	while (childrenCount < _grid.Children.Count)
	// 	{
	// 		_grid.Children.RemoveAt(_grid.Children.Count - 1);
	// 	}
	//
	// 	// Set inter-element spacing
	// 	if (hasSpace)
	// 	{
	// 		_grid.ClearValue(Grid.ColumnSpacingProperty);
	// 		_grid.ClearValue(Grid.RowSpacingProperty);
	// 	}
	//
	// 	if (isVertical)
	// 	{
	// 		_grid.ColumnDefinitions.Clear();
	// 		_grid.RowDefinitions.Clear();
	// 		// Ensure definitions is of right size
	// 		while (_grid.RowDefinitions.Count < gridDefinitionsCount)
	// 		{
	// 			_grid.RowDefinitions.Add(new RowDefinition());
	// 		}
	// 	}
	// 	else
	// 	{
	// 		_grid.RowDefinitions.Clear();
	// 		_grid.ColumnDefinitions.Clear();
	//
	// 		// Ensure definitions is of right size
	// 		while (_grid.ColumnDefinitions.Count < gridDefinitionsCount)
	// 		{
	// 			_grid.ColumnDefinitions.Add(new ColumnDefinition());
	// 		}
	// 	}
	//
	// 	if (hasPadding)
	// 	{
	// 		_grid.Padding = new Thickness(0);
	//
	// 		if (IsVerticalHug || childrenVerticallAlignment is VerticalAlignment.Top or VerticalAlignment.Stretch || hasSpaceBetween)
	// 		{
	// 			var topPaddingSize = spacing < 0 ? padding.Top - spacing : padding.Top;
	// 			_grid.RowDefinitions.Insert(0, new RowDefinition() { Height = new GridLength(topPaddingSize), MaxHeight = topPaddingSize });
	// 			gridRowIndexOffSet += 1;
	// 		}
	//
	// 		if (IsHorizontalHug || childrenHorizontalAlignment is HorizontalAlignment.Left or HorizontalAlignment.Stretch || hasSpaceBetween)
	// 		{
	// 			var firstColumnWidthSize = spacing < 0 ? padding.Left - spacing : padding.Left;
	// 			_grid.ColumnDefinitions.Insert(0, new ColumnDefinition() { Width = new GridLength(firstColumnWidthSize), MaxWidth = firstColumnWidthSize });
	// 			gridColumnIndexOffSet += 1;
	// 		}
	// 	}
	//
	// 	if (isVertical)
	// 	{
	//
	// 		if (hasPadding)
	// 		{
	// 			_grid.ColumnDefinitions.Insert(gridColumnIndexOffSet, new ColumnDefinition() { Width = new GridLength(1, IsHorizontalHug ? GridUnitType.Auto : GridUnitType.Star) });
	// 		}
	//
	// 		var rawChildIndex = 0;
	//
	// 		if (hasSpaceBetween is false && atLeastOneChildFillAvailableSpaceInPrimaryAxis is false && isPrimaryAxisAlignmentCenterOrEnd)
	// 		{
	// 			_grid.RowDefinitions.Insert(gridRowIndexOffSet, new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
	// 			gridRowIndexOffSet += 1;
	// 		}
	// 		// Process children
	// 		for (var i = 0; i < childrenCount; i++)
	// 		{
	// 			var child = children[i];
	//
	// 			if (GetIsIndependentLayout(child))
	// 			{
	// 				Grid.SetRow(child, 0);
	// 				// two extra count for the span. Depending of the PrimaryAxisAlignment we need to add two extra grid
	// 				Grid.SetRowSpan(child, gridDefinitionsCount + 4);
	// 				Grid.SetColumnSpan(child, gridDefinitionsCount + 4);
	// 				continue;
	// 			}
	//
	// 			var gridIndex = shouldAddInBetweenRow ? rawChildIndex * 2 : rawChildIndex;
	// 			//We add a grid. we need to adjust the index.
	// 			var adjustGridIndex = gridIndex + gridRowIndexOffSet;
	// 			Grid.SetRow(child, adjustGridIndex);
	// 			var gridDefinition = _grid.RowDefinitions[adjustGridIndex];
	//
	// 			// Get relative alignment or default if not set + set on child element
	// 			var primaryAlignment = GetPrimaryAlignment(child);
	// 			if (primaryAlignment is AutoLayoutPrimaryAlignment.Stretch)
	// 			{
	// 				gridDefinition.Height = new GridLength(1, GridUnitType.Star);
	// 				child.VerticalAlignment = VerticalAlignment.Stretch;
	// 			}
	// 			else if (GetPrimaryLength(child) is var height and > 0)
	// 			{
	// 				gridDefinition.Height = new GridLength(height);
	// 				child.VerticalAlignment = VerticalAlignment.Stretch;
	// 			}
	// 			else
	// 			{
	// 				gridDefinition.Height = GridLength.Auto;
	// 				child.VerticalAlignment = VerticalAlignment.Top;
	// 			}
	//
	// 			var counterAlignment = GetCounterAlignment(child).ToHorizontalAlignment();
	//
	// 			child.HorizontalAlignment = counterAlignment;
	//
	// 			if (gridColumnIndexOffSet > 0)
	// 			{
	// 				Grid.SetColumn(child, gridColumnIndexOffSet);
	// 			}
	//
	// 			if (GetCounterLength(child) is var width and > 0)
	// 			{
	// 				child.Width = width;
	// 			}
	// 			rawChildIndex++;
	// 		}
	//
	// 		// Process "space between"
	// 		if (hasSpace)
	// 		{
	// 			if (isSpacing)
	// 			{
	// 				if (spacing < 0)
	// 				{
	// 					_grid.RowSpacing = spacing;
	// 					_grid.ClearValue(Grid.ColumnSpacingProperty);
	// 				}
	// 				else
	// 				{
	// 					for (var i = 1 + gridRowIndexOffSet; i < gridDefinitionsCount + gridRowIndexOffSet; i += 2)
	// 					{
	// 						_grid.RowDefinitions[i].Height = new GridLength(spacing);
	// 					}
	// 				}
	// 			}
	// 			else if (hasSpaceBetween)
	// 			{
	// 				for (var i = 1 + gridRowIndexOffSet; i < gridDefinitionsCount + gridRowIndexOffSet; i += 2)
	// 				{
	// 					_grid.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Star);
	// 				}
	// 			}
	// 		}
	//
	// 		if (hasIndependentLayout && atLeastOneChildFillAvailableSpaceInPrimaryAxis is not true && PrimaryAxisAlignment != AutoLayoutAlignment.End && !hasSpaceBetween)
	// 		{
	// 			//We need to make sure that the independent layout can span all across his parent
	// 			_grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
	// 		}
	//
	// 		var paddingCondition = atLeastOneChildFillAvailableSpaceInPrimaryAxis || PrimaryAxisAlignment is not AutoLayoutAlignment.Start || IsVerticalHug;
	//
	// 	}
	// 	else // Horizontal
	// 	{
	// 		if (hasPadding)
	// 		{
	// 			_grid.RowDefinitions.Insert(gridRowIndexOffSet ,new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
	// 		}
	//
	// 		var rawChildIndex = 0;
	//
	// 		if (hasSpaceBetween is false && atLeastOneChildFillAvailableSpaceInPrimaryAxis is false && isPrimaryAxisAlignmentCenterOrEnd )
	// 		{
	// 			_grid.ColumnDefinitions.Insert(gridColumnIndexOffSet, new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
	// 			gridColumnIndexOffSet += 1;
	// 		}
	//
	// 		// Process children
	// 		for (var i = 0; i < childrenCount; i++)
	// 		{
	// 			var child = children[i];
	// 			if (GetIsIndependentLayout(child))
	// 			{
	// 				Grid.SetColumn(child, 0);
	// 				// two extra count for the span. Depending of the PrimaryAxisAlignment we need to add two extra grid
	// 				Grid.SetColumnSpan(child, gridDefinitionsCount + 4);
	// 				Grid.SetRowSpan(child, gridDefinitionsCount + 4);
	// 				continue;
	// 			}
	//
	// 			var gridIndex = shouldAddInBetweenRow ? rawChildIndex * 2 : rawChildIndex;
	// 			//We add a grid. we need to adjust the index.
	// 			var adjustGridIndex = gridIndex + gridColumnIndexOffSet;
	// 			var gridDefinition = _grid.ColumnDefinitions[adjustGridIndex];
	// 			Grid.SetColumn(child, adjustGridIndex);
	//
	// 			// Get relative alignment or default if not set + set on child element
	// 			var primaryAlignment = GetPrimaryAlignment(child);
	// 			if (primaryAlignment is AutoLayoutPrimaryAlignment.Stretch)
	// 			{
	// 				gridDefinition.Width = new GridLength(1, GridUnitType.Star);
	// 				child.HorizontalAlignment = HorizontalAlignment.Stretch;
	// 			}
	// 			else if (GetPrimaryLength(child) is var width and > 0)
	// 			{
	// 				gridDefinition.Width = new GridLength(width);
	// 				child.HorizontalAlignment = HorizontalAlignment.Stretch;
	// 			}
	// 			else
	// 			{
	// 				gridDefinition.Width = GridLength.Auto;
	// 				child.HorizontalAlignment = HorizontalAlignment.Left;
	// 			}
	//
	// 			var counterAlignment = GetCounterAlignment(child).ToVerticalAlignment();
	//
	// 			child.VerticalAlignment = counterAlignment;
	//
	//
	// 			if (gridRowIndexOffSet > 0)
	// 			{
	// 				Grid.SetRow(child, 1);
	// 			}
	//
	// 			if (GetCounterLength(child) is var height and > 0)
	// 			{
	// 				child.Height = height;
	// 			}
	// 			rawChildIndex++;
	// 		}
	//
	// 		// Process "space between"
	// 		if (hasSpace)
	// 		{
	// 			if (isSpacing)
	// 			{
	//
	// 				if (spacing < 0)
	// 				{
	// 					_grid.ColumnSpacing = spacing;
	// 					_grid.ClearValue(Grid.RowSpacingProperty);
	// 				}
	// 				else
	// 				{
	// 					for (var i = 1 + gridColumnIndexOffSet; i < gridDefinitionsCount + gridColumnIndexOffSet; i += 2)
	// 					{
	// 						_grid.ColumnDefinitions[i].Width = new GridLength(spacing);
	// 					}
	// 				}
	// 			}
	// 			else if (hasSpaceBetween)
	// 			{
	// 				for (var i = 1 + gridColumnIndexOffSet; i < gridDefinitionsCount + gridColumnIndexOffSet; i += 2)
	// 				{
	// 					_grid.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Star);
	// 				}
	// 			}
	// 		}
	//
	// 		if (hasIndependentLayout && atLeastOneChildFillAvailableSpaceInPrimaryAxis is not true && PrimaryAxisAlignment != AutoLayoutAlignment.End && !hasSpaceBetween)
	// 		{
	// 			//We need to make sure that the independent layout can span all across his parent
	// 			_grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
	// 		}
	//
	// 		var paddingCondition = atLeastOneChildFillAvailableSpaceInPrimaryAxis || PrimaryAxisAlignment is not AutoLayoutAlignment.Start || IsHorizontalHug;
	// 	}
	//
	// 	if (hasPadding)
	// 	{
	// 		if (IsVerticalHug || childrenVerticallAlignment is VerticalAlignment.Bottom or VerticalAlignment.Stretch || hasSpaceBetween)
	// 		{
	// 			var bottomPaddingSize = spacing < 0 ? padding.Bottom - spacing : padding.Bottom;
	// 			_grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(bottomPaddingSize), MaxHeight = bottomPaddingSize });
	// 		}
	//
	// 		if (IsHorizontalHug || childrenHorizontalAlignment is HorizontalAlignment.Right or HorizontalAlignment.Stretch || hasSpaceBetween)
	// 		{
	// 			var rightPaddingsize = spacing < 0 ? padding.Right - spacing : padding.Right;
	// 			_grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(rightPaddingsize), MaxWidth = rightPaddingsize });
	// 		}
	// 	}
	//
	// 	var shouldUsePrimaryAxisAlignment = atLeastOneChildFillAvailableSpaceInPrimaryAxis || hasSpaceBetween || hasIndependentLayout;
	//
	// 	// Set container alignments
	// 	if (isVertical)
	// 	{
	// 		if (shouldUsePrimaryAxisAlignment)
	// 		{
	// 			_grid.ClearValue(VerticalAlignmentProperty);
	// 		}
	// 		else
	// 		{
	// 			_grid.VerticalAlignment = PrimaryAxisAlignment.ToVerticalAlignment();
	// 		}
	// 		_grid.ClearValue(HorizontalAlignmentProperty);
	// 	}
	// 	else
	// 	{
	// 		if (shouldUsePrimaryAxisAlignment)
	// 		{
	// 			_grid.ClearValue(HorizontalAlignmentProperty);
	// 		}
	// 		else
	// 		{
	// 			_grid.HorizontalAlignment = PrimaryAxisAlignment.ToHorizontalAlignment();
	// 		}
	// 		_grid.ClearValue(VerticalAlignmentProperty);
	// 	}
	//}
}
