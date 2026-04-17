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

public static class GridExtensions
{
	#region DependencyProperty: Auto

	public static DependencyProperty AutoProperty { [DynamicDependency(nameof(GetAuto))] get; } = DependencyProperty.RegisterAttached(
		"Auto",
		typeof(bool),
		typeof(GridExtensions),
		new PropertyMetadata(false, OnAutoChanged));

	[DynamicDependency(nameof(SetAuto))]
	public static bool GetAuto(DependencyObject obj) => (bool)obj.GetValue(AutoProperty);

	[DynamicDependency(nameof(GetAuto))]
	public static void SetAuto(DependencyObject obj, bool value) => obj.SetValue(AutoProperty, value);

	#endregion
	#region DependencyProperty: Direction

	public static DependencyProperty DirectionProperty { [DynamicDependency(nameof(GetDirection))] get; } = DependencyProperty.RegisterAttached(
		"Direction",
		typeof(Orientation),
		typeof(GridExtensions),
		new PropertyMetadata(Orientation.Horizontal, OnDirectionChanged));

	[DynamicDependency(nameof(SetDirection))]
	public static Orientation GetDirection(DependencyObject obj) => (Orientation)obj.GetValue(DirectionProperty);

	[DynamicDependency(nameof(GetDirection))]
	public static void SetDirection(DependencyObject obj, Orientation value) => obj.SetValue(DirectionProperty, value);

	#endregion
	#region DependencyProperty: StateHash (private)

	private static DependencyProperty StateHashProperty { get; } = DependencyProperty.RegisterAttached(
		"StateHash",
		typeof(int),
		typeof(GridExtensions),
		new PropertyMetadata(0));

	private static int GetStateHash(DependencyObject obj) => (int)obj.GetValue(StateHashProperty);
	private static void SetStateHash(DependencyObject obj, int value) => obj.SetValue(StateHashProperty, value);

	#endregion
	#region DependencyProperty: Subscription (private)

	private static DependencyProperty SubscriptionProperty { get; } = DependencyProperty.RegisterAttached(
		"Subscription",
		typeof(IDisposable),
		typeof(GridExtensions),
		new PropertyMetadata(null));

	private static IDisposable? GetSubscription(DependencyObject obj) => (IDisposable?)obj.GetValue(SubscriptionProperty);
	private static void SetSubscription(DependencyObject obj, IDisposable? value) => obj.SetValue(SubscriptionProperty, value);

	#endregion

	private static void OnAutoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not Grid grid) return;

		if (e.NewValue is true)
		{
			if (GetSubscription(grid) is null)
			{
				// WinAppSDK: UIElementCollection/DefinitionCollection don't implement IObservableVector,
				// so fall back to LayoutUpdated which fires after any structural change to the Grid.
				// Uno: There is UIElementCollection.CollectionChanged, but still missing relevant hooks for row/column-definitions.
				EventHandler<object> handler = (_, _) => UpdateLayout(grid);
				grid.LayoutUpdated += handler;
				SetSubscription(grid, Disposable.Create(() => grid.LayoutUpdated -= handler));
			}

			UpdateLayout(grid);
		}
		else
		{
			GetSubscription(grid)?.Dispose();
			SetSubscription(grid, null);
			SetStateHash(grid, 0);
		}
	}

	private static void OnDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is Grid grid && GetAuto(grid))
			UpdateLayout(grid);
	}

	private static int ComputeStateHash(Grid grid)
	{
		if (!GetAuto(grid)) return 0;

		// if there is no more than 1 cells, there is nothing to arrange
		if (grid.RowDefinitions.Count is { } rowCount &&
			grid.ColumnDefinitions.Count is { } columnCount &&
			rowCount <= 1 && columnCount <= 1) return 0;

		var hash = new HashCode();

		hash.Add(GetDirection(grid));
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

		if (!GetAuto(grid)) return;

		// if there is no more than 1 cells, there is nothing to arrange
		var rowCount = Math.Max(grid.RowDefinitions.Count, 1);
		var columnCount = Math.Max(grid.ColumnDefinitions.Count, 1);
		if (rowCount == 1 && columnCount == 1) return;

		var childCount = grid.Children.Count;
		var fillByColumnsFirst = GetDirection(grid) is Orientation.Horizontal;

		for (var i = 0; i < childCount; i++)
		{
			var (row, column) = fillByColumnsFirst
				? (i / columnCount % rowCount, i % columnCount)
				: (i % rowCount, i / rowCount % columnCount);

			if (grid.Children[i] is FrameworkElement childAsFE)
			{
				Grid.SetRow(childAsFE, row);
				Grid.SetColumn(childAsFE, column);
			}
		}
	}
}
