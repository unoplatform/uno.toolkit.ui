using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Uno.Disposables;
using Uno.Extensions;
using Uno.Logging;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using ItemsRepeater = Microsoft.UI.Xaml.Controls.ItemsRepeater;
#endif

namespace Uno.Toolkit.UI;

public static partial class ItemsRepeaterExtensions
{
	private static ILogger _logger { get; } = typeof(CommandExtensions).Log();

	#region DependencyProperty: IsSynchronizingSelection

	private static DependencyProperty IsSynchronizingSelectionProperty { [DynamicDependency(nameof(GetIsSynchronizingSelection))] get; } = DependencyProperty.RegisterAttached(
		"IsSynchronizingSelection",
		typeof(bool),
		typeof(ItemsRepeaterExtensions),
		new PropertyMetadata(default(bool)));

	[DynamicDependency(nameof(SetIsSynchronizingSelection))]
	private static bool GetIsSynchronizingSelection(ItemsRepeater obj) => (bool)obj.GetValue(IsSynchronizingSelectionProperty);
	[DynamicDependency(nameof(GetIsSynchronizingSelection))]
	private static void SetIsSynchronizingSelection(ItemsRepeater obj, bool value) => obj.SetValue(IsSynchronizingSelectionProperty, value);

	#endregion
	#region DependencyProperty: SelectedItem

	public static DependencyProperty SelectedItemProperty { [DynamicDependency(nameof(GetSelectedItem))] get; } = DependencyProperty.RegisterAttached(
		"SelectedItem",
		typeof(object),
		typeof(ItemsRepeaterExtensions),
		new PropertyMetadata(default(object), OnSelectedItemChanged));

	[DynamicDependency(nameof(SetSelectedItem))]
	public static object? GetSelectedItem(ItemsRepeater obj) => obj.GetValue(SelectedItemProperty);
	[DynamicDependency(nameof(GetSelectedItem))]
	public static void SetSelectedItem(ItemsRepeater obj, object? value) => obj.SetValue(SelectedItemProperty, value);

	#endregion
	#region DependencyProperty: SelectedItems

	public static DependencyProperty SelectedItemsProperty { [DynamicDependency(nameof(GetSelectedItems))] get; } = DependencyProperty.RegisterAttached(
		"SelectedItems",
		typeof(IList<object>),
		typeof(ItemsRepeaterExtensions),
		new PropertyMetadata(default(IList<object>), OnSelectedItemsChanged));

	[DynamicDependency(nameof(SetSelectedItems))]
	public static IList<object> GetSelectedItems(ItemsRepeater obj) => (IList<object>)obj.GetValue(SelectedItemsProperty);
	[DynamicDependency(nameof(GetSelectedItems))]
	public static void SetSelectedItems(ItemsRepeater obj, IList<object> value) => obj.SetValue(SelectedItemsProperty, value);

	#endregion
	#region DependencyProperty: SelectedIndex = -1

	public static DependencyProperty SelectedIndexProperty { [DynamicDependency(nameof(GetSelectedIndex))] get; } = DependencyProperty.RegisterAttached(
		"SelectedIndex",
		typeof(int),
		typeof(ItemsRepeaterExtensions),
		new PropertyMetadata(-1, OnSelectedIndexChanged));

	[DynamicDependency(nameof(SetSelectedIndex))]
	public static int GetSelectedIndex(ItemsRepeater obj) => (int)obj.GetValue(SelectedIndexProperty);
	[DynamicDependency(nameof(GetSelectedIndex))]
	public static void SetSelectedIndex(ItemsRepeater obj, int value) => obj.SetValue(SelectedIndexProperty, value);

	#endregion
	#region DependencyProperty: SelectedIndexes

	public static DependencyProperty SelectedIndexesProperty { [DynamicDependency(nameof(GetSelectedIndexes))] get; } = DependencyProperty.RegisterAttached(
		"SelectedIndexes",
		typeof(IList<int>),
		typeof(ItemsRepeaterExtensions),
		new PropertyMetadata(default(IList<int>), OnSelectedIndexesChanged));

	[DynamicDependency(nameof(SetSelectedIndexes))]
	public static IList<int> GetSelectedIndexes(ItemsRepeater obj) => (IList<int>)obj.GetValue(SelectedIndexesProperty);
	[DynamicDependency(nameof(GetSelectedIndexes))]
	public static void SetSelectedIndexes(ItemsRepeater obj, IList<int> value) => obj.SetValue(SelectedIndexesProperty, value);

	#endregion
	#region DependencyProperty: SelectionMode = (ItemsSelectionMode)0

	public static DependencyProperty SelectionModeProperty { [DynamicDependency(nameof(GetSelectionMode))] get; } = DependencyProperty.RegisterAttached(
		"SelectionMode",
		typeof(ItemsSelectionMode),
		typeof(ItemsRepeaterExtensions),
		new PropertyMetadata((ItemsSelectionMode)0, OnSelectionModeChanged));

	[DynamicDependency(nameof(SetSelectionMode))]
	public static ItemsSelectionMode GetSelectionMode(ItemsRepeater obj) => (ItemsSelectionMode)obj.GetValue(SelectionModeProperty);
	[DynamicDependency(nameof(GetSelectionMode))]
	public static void SetSelectionMode(ItemsRepeater obj, ItemsSelectionMode value) => obj.SetValue(SelectionModeProperty, value);

	#endregion
	#region DependencyProperty: SelectionSubscription

	private static DependencyProperty SelectionSubscriptionProperty { [DynamicDependency(nameof(GetSelectionSubscription))] get; } = DependencyProperty.RegisterAttached(
		"SelectionSubscription",
		typeof(IDisposable),
		typeof(ItemsRepeaterExtensions),
		new PropertyMetadata(default(IDisposable)));

	[DynamicDependency(nameof(SetSelectionSubscription))]
	private static IDisposable GetSelectionSubscription(ItemsRepeater obj) => (IDisposable)obj.GetValue(SelectionSubscriptionProperty);
	[DynamicDependency(nameof(GetSelectionSubscription))]
	private static void SetSelectionSubscription(ItemsRepeater obj, IDisposable value) => obj.SetValue(SelectionSubscriptionProperty, value);

	#endregion

	#region ItemCommand Impl
	internal static void OnItemCommandChanged(ItemsRepeater sender, DependencyPropertyChangedEventArgs e)
	{
		if (e.OldValue is ICommand)
		{
			if (e.NewValue is not ICommand) // tear down
			{
				sender.Tapped -= OnItemsRepeaterCommandTapped;
			}
			else
			{
				// When transitioning from one command to another, there is no need to rewire the event.
				// Since the handler is setup to invoke the command in the DP.
			}
		}
		else if (e.NewValue is ICommand command)
		{
			sender.Tapped += OnItemsRepeaterCommandTapped;
		}
	}

	private static void OnItemsRepeaterCommandTapped(object sender, TappedRoutedEventArgs e)
	{
		// ItemsRepeater is more closely related to Panel than ItemsControl, and it cannot be templated.
		// It is safe to assume all direct children of IR are materialized item template,
		// and there can't be header/footer or wrapper (ItemContainer) among them.

		if (sender is not ItemsRepeater ir) return;
		if (e.OriginalSource is ItemsRepeater) return;
		if (e.OriginalSource is DependencyObject source)
		{
			if (ir.FindRootElementOf(source) is FrameworkElement root)
			{
				CommandExtensions.TryInvokeCommand(ir, CommandExtensions.GetCommandParameter(root) ?? root.DataContext);
			}
		}
	}
	#endregion

	// ItemsRepeater is more closely related to Panel than ItemsControl, and it cannot be templated.
	// It is safe to assume all direct children of IR are materialized item template,
	// and there can't be header/footer or wrapper (ItemContainer) among them.

	// ItemsRepeater.ItemsSource can contain UIElement, although it is not possible to define directly in xaml like you can with ListView+LVI.
	// In the above case, the UIElement will be nested directly under the IR, unless an IR.ItemTemplate has been defined.

	// ItemsRepeater's children contains only materialized element; materialization and de-materialization can be track with
	// ElementPrepared and ElementClearing events. Recycled elements are reused based on FIFO-rule, resulting in index desync.
	// Selection state saved on the element (LVI.IsSelect, Chip.IsChecked) will also desync when it happens.
	// !!! So it is important to save the selection state into a dp, and validate against that on element materialization and correct when necessary.

	// Unlike ToggleButton (or Chip which derives from), SelectorItem is not normally selectable on click, unless nested under a Selector.

	private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => OnSelectionStateChanged(sender, e);
	private static void OnSelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => OnSelectionStateChanged(sender, e);
	private static void OnSelectedIndexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => OnSelectionStateChanged(sender, e);
	private static void OnSelectedIndexesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => OnSelectionStateChanged(sender, e);

	private static void OnSelectionModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
	{
		if (sender is not ItemsRepeater ir) return;

		if (e.NewValue is ItemsSelectionMode value && value != 0)
		{
			// Skip events setup, when we are just switching between valid modes.
			if (e.OldValue is ItemsSelectionMode oldValue && oldValue == 0)
			{
				ir.Tapped += OnItemsRepeaterTapped;
				ir.ElementPrepared += OnItemsRepeaterElementPrepared;

				SetSelectionSubscription(ir, new CompositeDisposable(
					Disposable.Create(() =>
					{
						ir.Tapped -= OnItemsRepeaterTapped;
						ir.ElementPrepared -= OnItemsRepeaterElementPrepared;
					}),
					ir.RegisterDisposablePropertyChangedCallback(ItemsRepeater.ItemsSourceProperty, OnItemsRepeaterItemsSourceChanged)
				));
			}

			if (ir.ItemsSourceView is { Count: > 0 })
			{
				try
				{
					SetIsSynchronizingSelection(ir, true);

					TrySynchronizeDefaultSelection(ir);
					SynchronizeMaterializedElementsSelection(ir);
				}
				finally
				{
					SetIsSynchronizingSelection(ir, false);
				}
			}
		}
		else
		{
			GetSelectionSubscription(ir)?.Dispose();
		}
	}
	private static void OnSelectionStateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
	{
		if (sender is not ItemsRepeater ir) return;
		if (GetIsSynchronizingSelection(ir)) return;

		try
		{
			SetIsSynchronizingSelection(ir, true);

			if (ir.ItemsSourceView is { Count: > 0 })
			{
				var indexes = MapStateToIndexes();

				// validate indexes are within bounds
				var count = ir.ItemsSourceView.Count;
				if (indexes.All(x => 0 <= x && x < count))
				{
					// commit selection change
					SetSelectionStates(ir, indexes);
					SynchronizeMaterializedElementsSelection(ir);
				}
				else
				{
					// rollback
					ir.SetValue(e.Property, e.OldValue);
				}
			}
			else
			{
				SetEmptySelection(ir);
			}
		}
		finally
		{
			SetIsSynchronizingSelection(ir, false);
		}

		int[] MapStateToIndexes()
		{
			if (e.Property == SelectedItemProperty || e.Property == SelectedItemsProperty)
			{
				if (e.NewValue == null) return Array.Empty<int>();

				var selectedItems = e.Property == SelectedItemProperty
					? new[] { e.NewValue }
					: (IList<object>)e.NewValue;

#if HAS_UNO || true // ItemsSourceView::IndexOf is not defined
				return Enumerable.Join(
					Enumerable.Range(0, ir.ItemsSourceView.Count), // all valid indexes
					selectedItems,
					ir.ItemsSourceView.GetAt,
					x => x,
					(index, item) => index
				).ToArray();
#else
				return items.Select(ir.ItemsSourceView.IndexOf).ToArray();
#endif
			}
			else if (e.Property == SelectedIndexProperty)
			{
				if (e.NewValue is int index && index != -1)
				{
					return new int[] { index };
				}
				else
				{
					return Array.Empty<int>();
				}
			}
			else if (e.Property == SelectedIndexesProperty)
			{
				// Out-of-bounds values will be handled by the outside block
				return (e.NewValue as IList<int>)?.ToArray() ?? Array.Empty<int>();
			}
			else
			{
				throw new InvalidOperationException($"Unknown selection state property: {e.Property}");
			}
		}
	}

	private static void OnItemsRepeaterElementPrepared(ItemsRepeater sender, Microsoft.UI.Xaml.Controls.ItemsRepeaterElementPreparedEventArgs args)
	{
		// When we reach here, it should be guaranteed that default selection state has been applied,
		// and we can rely on it to synchronize the selection on the view-level.
		var selected = GetSelectedIndexes(sender)?.Contains(args.Index) ?? false;

		SetItemSelection(args.Element, selected);
	}
	private static void OnItemsRepeaterItemsSourceChanged(DependencyObject sender, DependencyProperty dp)
	{
		// When we reach here, ItemsSourceView is already updated.
		if (sender is not ItemsRepeater ir) return;
		if (ir.ItemsSourceView is { Count: > 0 })
		{
			try
			{
				SetIsSynchronizingSelection(ir, true);

				TrySynchronizeDefaultSelection(ir, shouldBaseFromSelectionState: false);
				// Unlike in OnSelectionModeChanged, ir.GetChildren() still holds old materialized elements from previous ItemsSource.
				// There is nothing to do here; We can just let OnItemsRepeaterElementPrepared to handle the rest.
			}
			finally
			{
				SetIsSynchronizingSelection(ir, false);
			}
		}
	}
	private static void OnItemsRepeaterTapped(object sender, TappedRoutedEventArgs e)
	{
		// By the time we are here, ToggleButton.IsChecked would have already been toggled.

		if (sender is not ItemsRepeater ir) return;
		if (e.OriginalSource is ItemsRepeater) return;
		if (e.OriginalSource is DependencyObject source)
		{
			if (ir.FindRootElementOf(source) is { } element)
			{
				ToggleItemSelectionAtCoerced(ir, ir.GetElementIndex(element));
			}
		}
	}

	// note: TryCoerceSelectionFromXyz methods should never accept empty-selection as result, so that the next non-empty property can be evaluated.
	// Even if all 4 properties fail for being empty, TrySetDefaultSelection or SetEmptySelection can pick from there.
	private static bool TrySynchronizeDefaultSelection(ItemsRepeater ir, bool shouldBaseFromSelectionState = true)
	{
		if (ir.ItemsSourceView is not { Count: > 0 }) return false;
		if (GetSelectionMode(ir) is var mode && mode != 0)
		{
			if (mode is ItemsSelectionMode.None)
			{
				return SetEmptySelection(ir);
			}
			else if (mode is ItemsSelectionMode.Multiple)
			{
				if (shouldBaseFromSelectionState)
				{
					return TryCoerceSelectionFromIndexes(ir)
						|| TryCoerceSelectionFromItems(ir)
						|| TryCoerceSelectionFromIndex(ir)
						|| TryCoerceSelectionFromItem(ir)
						|| SetEmptySelection(ir);
				}
				else
				{
					return SetEmptySelection(ir);

				}

			}
			else if (mode is ItemsSelectionMode.Single or ItemsSelectionMode.SingleOrNone)
			{
				if (shouldBaseFromSelectionState)
				{
					return TryCoerceSelectionFromIndex(ir)
						|| TryCoerceSelectionFromItem(ir)
						|| TryCoerceSelectionFromIndexes(ir)
						|| TryCoerceSelectionFromItems(ir)
						|| TrySetDefaultSelection(ir)
						|| SetEmptySelection(ir);
				}
				else
				{
					return TrySetDefaultSelection(ir)
						|| SetEmptySelection(ir);
				}
			}
			else
			{
				throw new ArgumentOutOfRangeException($"ItemsSelectionMode: {mode}");
			}
		}

		return false;
	}
	private static bool TryCoerceSelectionFromIndex(ItemsRepeater ir)
	{
		if (GetSelectedIndex(ir) is { } index &&
			0 <= index && index < ir.ItemsSourceView.Count)
		{
			return SetSelectionStates(ir, index);
		}

		return false;
	}
	private static bool TryCoerceSelectionFromItem(ItemsRepeater ir)
	{
		if (GetSelectedItem(ir) is { } item &&
			ir.ItemsSourceView.IndexOf(item) is { } index && index != -1)
		{
			return SetSelectionStates(ir, index);
		}

		return false;
	}
	private static bool TryCoerceSelectionFromIndexes(ItemsRepeater ir)
	{
		// note: Do not accept partially matched indexes/items.
		if (GetSelectedIndexes(ir) is { Count: > 0 } indexes &&
			indexes.All(x => 0 <= x && x <= ir.ItemsSourceView.Count))
		{
			return SetSelectionStates(ir, indexes.ToArray());
		}

		return false;
	}
	private static bool TryCoerceSelectionFromItems(ItemsRepeater ir)
	{
		// note: Do not accept partially matched indexes/items.
		if (GetSelectedItems(ir) is { Count: > 0 } items &&
			items.Select(ir.ItemsSourceView.IndexOf).ToArray() is { } indexes &&
			indexes.All(x => 0 <= x && x <= ir.ItemsSourceView.Count))
		{
			return SetSelectionStates(ir, indexes.ToArray());
		}

		return false;
	}
	private static bool TrySetDefaultSelection(ItemsRepeater ir)
	{
		if (GetSelectionMode(ir) is ItemsSelectionMode.Single)
		{
			if (ir.ItemsSourceView is { Count: > 0 } isv)
			{
				return SetSelectionStates(ir, indexes: 0);
			}
		}

		return false;
	}
	private static bool SetEmptySelection(ItemsRepeater ir)
	{
		return SetSelectionStates(ir, indexes: null);
	}
	private static bool SetSelectionStates(ItemsRepeater ir, params int[]? indexes)
	{
		if (indexes is { Length: > 0 })
		{
			var items = indexes.Select(ir.ItemsSourceView.GetAt).ToArray();

			SetSelectedIndex(ir, indexes[0]);
			SetSelectedItem(ir, items[0]);
			SetSelectedIndexes(ir, indexes);
			SetSelectedItems(ir, items);
		}
		else
		{
			SetSelectedIndex(ir, -1);
			SetSelectedItem(ir, null);
			SetSelectedIndexes(ir, Array.Empty<int>());
			SetSelectedItems(ir, Array.Empty<object>());
		}

		return true;
	}

	private static void SynchronizeMaterializedElementsSelection(ItemsRepeater ir)
	{
		if (GetSelectedIndexes(ir) is not { } indexes) throw new InvalidOperationException("Selection state is not ready.");

		foreach (var element in ir.GetChildren())
		{
			if (element is UIElement uie &&
				ir.GetElementIndex(uie) is var index && index != -1)
			{
				SetItemSelection(uie, indexes.Contains(index));
			}
		}
	}
	internal static void ToggleItemSelectionAtCoerced(ItemsRepeater ir, int index)
	{
		if (GetIsSynchronizingSelection(ir))
		{
			if (_logger.IsEnabled(LogLevel.Warning))
			{
				_logger.Warn($"ToggleItemSelectionAtCoerced is invoked during selection synchronization.");
			}
			return;
		}

		try
		{
			SetIsSynchronizingSelection(ir, true);

			var selection = GetSelectedIndexes(ir) ?? Array.Empty<int>();
			var updated = ItemsSelectionHelper.ToggleSelectionAtCoerced(
				GetSelectionMode(ir),
				ir.ItemsSourceView.Count,
				selection,
				index
			);
			var diffIndexes = new[] { index } // even if selection at index remains unchanged, the element is out of sync already from ToggleButton impl.
				.Concat(updated.Except(selection))
				.Concat(selection.Except(updated))
				.Distinct()
				.ToArray();

			SetSelectionStates(ir, updated);
			foreach (var diffIndex in diffIndexes)
			{
				if (ir.TryGetElement(diffIndex) is { } materialized)
				{
					SetItemSelection(materialized, updated.Contains(diffIndex));
				}
				else
				{
					// non-materialized element will be handled by OnItemsRepeaterElementPrepared when its is materializing
				}
			}

		}
		finally
		{
			SetIsSynchronizingSelection(ir, false);
		}
	}
	internal static void SetItemSelection(DependencyObject x, bool value)
	{
		if (x is SelectorItem si)
		{
			si.IsSelected = value;
		}
		else if (x is ToggleButton toggle)
		{
			toggle.IsChecked = value;
		}
		else
		{
			// todo: generic item is not supported
		}
	}
}
