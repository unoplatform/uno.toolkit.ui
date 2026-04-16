using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Uno.Disposables;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

#if HAS_UNO
using Windows.Foundation.Collections;
#endif

namespace Uno.Toolkit.UI;

public static class AutoGrid
{
	// Disposable subscriptions keyed by Grid instance
	private static readonly Dictionary<Grid, IDisposable> _state = new();

	// -- Mode Attached Property --

	public static DependencyProperty ModeProperty { [DynamicDependency(nameof(GetMode))] get; } = DependencyProperty.RegisterAttached(
		"Mode",
		typeof(AutoGridMode),
		typeof(AutoGrid),
		new PropertyMetadata(AutoGridMode.Disabled, OnModeChanged));

	[DynamicDependency(nameof(SetMode))]
	public static AutoGridMode GetMode(DependencyObject obj) => (AutoGridMode)obj.GetValue(ModeProperty);

	[DynamicDependency(nameof(GetMode))]
	public static void SetMode(DependencyObject obj, AutoGridMode value) => obj.SetValue(ModeProperty, value);

	private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not Grid grid) return;
		Unsubscribe(grid);
		if ((AutoGridMode)e.NewValue != AutoGridMode.Disabled)
			Subscribe(grid);
	}

	private static void Subscribe(Grid grid)
	{
#if HAS_UNO
		VectorChangedEventHandler<UIElement> childrenHandler = (_, _) => UpdateLayout(grid);
		VectorChangedEventHandler<ColumnDefinition> columnsHandler = (_, _) => UpdateLayout(grid);
		VectorChangedEventHandler<RowDefinition> rowsHandler = (_, _) => UpdateLayout(grid);

		grid.Children.VectorChanged += childrenHandler;
		grid.ColumnDefinitions.VectorChanged += columnsHandler;
		grid.RowDefinitions.VectorChanged += rowsHandler;

		_state[grid] = Disposable.Create(() =>
		{
			grid.Children.VectorChanged -= childrenHandler;
			grid.ColumnDefinitions.VectorChanged -= columnsHandler;
			grid.RowDefinitions.VectorChanged -= rowsHandler;
		});
#else
		// WinAppSDK: UIElementCollection/DefinitionCollection don't implement IObservableVector,
		// so fall back to LayoutUpdated which fires after any structural change to the Grid.
		EventHandler<object> layoutHandler = (_, _) => UpdateLayout(grid);
		grid.LayoutUpdated += layoutHandler;
		_state[grid] = Disposable.Create(() => grid.LayoutUpdated -= layoutHandler);
#endif

		UpdateLayout(grid);
	}

	private static void Unsubscribe(Grid grid)
	{
		if (!_state.TryGetValue(grid, out var disposable)) return;
		disposable.Dispose();
		_state.Remove(grid);
	}

	// -- StateHash Attached Property (private, WinAppSDK only) --
	// LayoutUpdated fires on every layout pass (not just structural changes), so we use a hash
	// of the children identity and definition counts to skip updates when nothing relevant changed.
	private static DependencyProperty StateHashProperty { get; } = DependencyProperty.RegisterAttached(
		"StateHash",
		typeof(int),
		typeof(AutoGrid),
		new PropertyMetadata(0));

	private static int GetStateHash(DependencyObject obj) => (int)obj.GetValue(StateHashProperty);
	private static void SetStateHash(DependencyObject obj, int value) => obj.SetValue(StateHashProperty, value);

	private static int ComputeStateHash(Grid grid)
	{
		var hash = new HashCode();
		hash.Add(grid.ColumnDefinitions.Count);
		hash.Add(grid.RowDefinitions.Count);
		foreach (var child in grid.Children)
			hash.Add(child.GetHashCode());
		return hash.ToHashCode();
	}

	private static void UpdateLayout(Grid grid)
	{
#if !HAS_UNO
		var hash = ComputeStateHash(grid);
		if (GetStateHash(grid) == hash) return;
		SetStateHash(grid, hash);
#endif

		var mode = GetMode(grid);
		var children = grid.Children;
		var childCount = children.Count;
		var hasCols = grid.ColumnDefinitions.Count > 0;
		var hasRows = grid.RowDefinitions.Count > 0;

		for (int i = 0; i < childCount; i++)
		{
			int row, col;
			if (!hasCols && !hasRows)
			{
				row = 0;
				col = 0;
			}
			else if (hasCols && !hasRows)
			{
				row = 0;
				col = i % grid.ColumnDefinitions.Count;
			}
			else if (!hasCols && hasRows)
			{
				col = 0;
				row = i % grid.RowDefinitions.Count;
			}
			else
			{
				var cols = grid.ColumnDefinitions.Count;
				var rows = grid.RowDefinitions.Count;
				var cell = i % (cols * rows);
				if (mode == AutoGridMode.Vertical)
				{
					col = cell / rows;
					row = cell % rows;
				}
				else // Horizontal / Enable
				{
					row = cell / cols;
					col = cell % cols;
				}
			}
			Grid.SetRow((UIElement)children[i], row);
			Grid.SetColumn((UIElement)children[i], col);
		}
	}
}
