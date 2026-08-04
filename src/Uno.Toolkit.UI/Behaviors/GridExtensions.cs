using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Uno.Collections;
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

	/// <summary>
	/// Internal per-grid state, kept out of the XAML-resolvable attached-property space.
	/// </summary>
	private class AutoState
	{
		public int StateHash { get; set; }
		public IDisposable? Subscription { get; set; }
		public bool IsUpdating { get; set; }
	}

	private static readonly WeakAttachedDictionary<Grid, string> _states = new();
	private const string AutoStateKey = "AutoState";

	private static AutoState GetState(Grid grid) => _states.GetValue(grid, AutoStateKey, () => new AutoState());

	private static void OnAutoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not Grid grid) return;

		// -= before += so that a redundant Auto=true doesn't double-subscribe
		grid.Loaded -= OnGridLoaded;
		grid.Unloaded -= OnGridUnloaded;

		if (e.NewValue is true)
		{
			grid.Loaded += OnGridLoaded;
			grid.Unloaded += OnGridUnloaded;

			if (grid.IsLoaded)
			{
				Subscribe(grid);
			}

			UpdateLayout(grid);
		}
		else
		{
			Unsubscribe(grid);
			GetState(grid).StateHash = 0;
		}
	}

	private static void OnGridLoaded(object sender, RoutedEventArgs e)
	{
		if (sender is not Grid grid) return;

		Subscribe(grid);

		// Loaded and the first LayoutUpdated of the same pass have no guaranteed ordering, so arrange
		// eagerly here rather than waiting on an event that may not fire again until layout is invalidated.
		UpdateLayout(grid);
	}

	private static void OnGridUnloaded(object sender, RoutedEventArgs e)
	{
		if (sender is not Grid grid) return;

		Unsubscribe(grid);

		// Children/definitions can change while detached, so force a recompute on the next load.
		GetState(grid).StateHash = 0;
	}

	private static void Subscribe(Grid grid)
	{
		var state = GetState(grid);
		if (state.Subscription is not null) return;

		// WinAppSDK: UIElementCollection/DefinitionCollection don't implement IObservableVector,
		// so fall back to LayoutUpdated which fires after any structural change to the Grid.
		// Uno: There is UIElementCollection.CollectionChanged, but still missing relevant hooks for row/column-definitions.
		EventHandler<object> handler = (_, _) => UpdateLayout(grid);
		grid.LayoutUpdated += handler;
		state.Subscription = Disposable.Create(() => grid.LayoutUpdated -= handler);
	}

	private static void Unsubscribe(Grid grid)
	{
		var state = GetState(grid);

		state.Subscription?.Dispose();
		state.Subscription = null;
	}

	private static int ComputeStateHash(Grid grid, int rowCount, int columnCount)
	{
		var hash = new HashCode();

		hash.Add(rowCount);
		hash.Add(columnCount);

		var children = grid.Children;
		for (var i = 0; i < children.Count; i++)
		{
			// identity hash: a consumer overriding GetHashCode with an unstable implementation
			// would otherwise produce a fresh hash on every layout pass.
			hash.Add(RuntimeHelpers.GetHashCode(children[i]));
		}

		return hash.ToHashCode();
	}

	private static void UpdateLayout(Grid grid)
	{
		if (!GetAuto(grid)) return;

		// if there is no more than 1 cells, there is nothing to arrange
		var rowCount = Math.Max(grid.RowDefinitions.Count, 1);
		var columnCount = Math.Max(grid.ColumnDefinitions.Count, 1);
		if (rowCount == 1 && columnCount == 1) return;

		var state = GetState(grid);

		// setting Grid.Row/Grid.Column below invalidates layout, which can re-raise LayoutUpdated
		if (state.IsUpdating) return;

		// LayoutUpdated fires on every layout pass (not just structural changes),
		// so we use a state-hash to skip updates when nothing relevant changed.
		var hash = ComputeStateHash(grid, rowCount, columnCount);
		if (hash == state.StateHash) return;
		state.StateHash = hash;

		state.IsUpdating = true;
		try
		{
			var children = grid.Children;
			for (var i = 0; i < children.Count; i++)
			{
				var (row, column) = (i / columnCount % rowCount, i % columnCount);

				// note: Grid.RowSpan/Grid.ColumnSpan are not accounted for; children are placed as 1x1.
				if (children[i] is FrameworkElement childAsFE)
				{
					Grid.SetRow(childAsFE, row);
					Grid.SetColumn(childAsFE, column);
				}
			}
		}
		finally
		{
			state.IsUpdating = false;
		}
	}
}
