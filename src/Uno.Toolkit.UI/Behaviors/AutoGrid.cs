using System;
using System.Diagnostics.CodeAnalysis;
using Uno.Disposables;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI;

public static class AutoGrid
{
	#region DependencyProperty: Mode

	public static DependencyProperty ModeProperty { [DynamicDependency(nameof(GetMode))] get; } = DependencyProperty.RegisterAttached(
		"Mode",
		typeof(AutoGridMode),
		typeof(AutoGrid),
		new PropertyMetadata(AutoGridMode.None, OnModeChanged));

	[DynamicDependency(nameof(SetMode))]
	public static AutoGridMode GetMode(DependencyObject obj) => (AutoGridMode)obj.GetValue(ModeProperty);

	[DynamicDependency(nameof(GetMode))]
	public static void SetMode(DependencyObject obj, AutoGridMode value) => obj.SetValue(ModeProperty, value);

	#endregion
	#region DependencyProperty: StateHash (private)

	private static DependencyProperty StateHashProperty { get; } = DependencyProperty.RegisterAttached(
		"StateHash",
		typeof(int),
		typeof(AutoGrid),
		new PropertyMetadata(0));

	private static int GetStateHash(DependencyObject obj) => (int)obj.GetValue(StateHashProperty);
	private static void SetStateHash(DependencyObject obj, int value) => obj.SetValue(StateHashProperty, value);

	#endregion
	#region DependencyProperty: Subscription (private)

	private static DependencyProperty SubscriptionProperty { get; } = DependencyProperty.RegisterAttached(
		"Subscription",
		typeof(IDisposable),
		typeof(AutoGrid),
		new PropertyMetadata(null));

	private static IDisposable? GetSubscription(DependencyObject obj) => (IDisposable?)obj.GetValue(SubscriptionProperty);
	private static void SetSubscription(DependencyObject obj, IDisposable? value) => obj.SetValue(SubscriptionProperty, value);

	#endregion

	private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not Grid grid) return;

		if (e.NewValue is not AutoGridMode.None)
		{
			if (GetSubscription(grid) is null)
			{
				// WinAppSDK: UIElementCollection/DefinitionCollection don't implement IObservableVector,
				// so fall back to LayoutUpdated which fires after any structural change to the Grid.
				// Uno: There is UIElementCollection.CollectionChanged, but still missing relevant hooks for row/column-definitions.
				grid.LayoutUpdated += OnGridLayoutUpdated;
				SetSubscription(grid, Disposable.Create(() => grid.LayoutUpdated -= OnGridLayoutUpdated));
			}

			UpdateLayout(grid);
		}
		else
		{
			GetSubscription(grid)?.Dispose();
			SetSubscription(grid, null);
		}
	}

	private static void OnGridLayoutUpdated(object? sender, object e)
	{
		if (sender is not Grid grid) return;

		UpdateLayout(grid);
	}

	private static int ComputeStateHash(Grid grid)
	{
		if (GetMode(grid) is { } mode && mode is AutoGridMode.None) return 0;
		if (grid.RowDefinitions.Count is { } rowCount &&
			grid.ColumnDefinitions.Count is { } columnCount &&
			rowCount == 0 && columnCount == 0) return 0;

		var hash = new HashCode();

		hash.Add(mode);
		hash.Add(rowCount);
		hash.Add(columnCount);

		foreach (var child in grid.Children)
			hash.Add(child.GetHashCode());

		return hash.ToHashCode();
	}

	private static void UpdateLayout(Grid grid)
	{
		// LayoutUpdated fires on every layout pass (not just structural changes),
		// so we use a state-hash to skip updates when nothing relevant changed.
		var hash = ComputeStateHash(grid);
		var oldHash = GetStateHash(grid);
		if (hash == oldHash) return;
		SetStateHash(grid, hash);
		
		if (GetMode(grid) is { } mode && mode is AutoGridMode.None) return;
		if (grid.RowDefinitions.Count is { } rowCount &&
			grid.ColumnDefinitions.Count is { } columnCount &&
			rowCount == 0 && columnCount == 0) return;

		var childCount = grid.Children.Count;
		var fillByColumnsFirst = mode is AutoGridMode.Column;

		for (var i = 0; i < childCount; i++)
		{
			var (row, column) = fillByColumnsFirst
				? (i / columnCount % rowCount, i % columnCount)
				: (i % rowCount, i / rowCount % columnCount);

			Grid.SetRow(grid.Children[i], row);
			Grid.SetColumn(grid.Children[i], column);
		}
	}
}
