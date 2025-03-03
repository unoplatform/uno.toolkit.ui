using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI;

public partial class DockControl : Control
{
	internal class PropertyKeys
	{
		public const string Container = nameof(Container);
		public const string Item = nameof(Item);
	}

	#region DependencyProperty: PaneClosingBehavior = CloseItem

	public static DependencyProperty PaneClosingBehaviorProperty { get; } = DependencyProperty.Register(
		nameof(PaneClosingBehavior),
		typeof(DockPaneClosingBehavior),
		typeof(DockControl),
		new PropertyMetadata(DockPaneClosingBehavior.CloseItem));

	public DockPaneClosingBehavior PaneClosingBehavior
	{
		get => (DockPaneClosingBehavior)GetValue(PaneClosingBehaviorProperty);
		set => SetValue(PaneClosingBehaviorProperty, value);
	}

	#endregion

	private RootPane? _rootPane;
	private DockingDiamond? _dockingDiamond;

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		_rootPane = GetTemplateChild("RootPane") as RootPane;
		_dockingDiamond = GetTemplateChild("DockingDiamond") as DockingDiamond;
	}

	public void AddItem(DockItem item) => AddItem(item, null, null);
	public void AddItem(DockItem item, ElementPane? pane, DockDirection? direction)
	{
		if (pane is { })
		{
			//if (!IsNested(pane)) throw;

			pane.Add(item);
		}
		else
		{
			FindPane<DocumentPane>()?.Add(item);
		}
	}

	public TPane? FindPane<TPane>() where TPane : DockPane => FindPane<TPane>(x => true);
	public TPane? FindPane<TPane>(Func<TPane, bool> predicate) where TPane : DockPane
	{
		if (_rootPane is null) return null;

		TPane? Walk(DockPane pane)
		{
			if (pane is TPane tpane && predicate(tpane)) return tpane;

			foreach (var nested in (pane as LayoutPane)?.NestedPanes ?? Enumerable.Empty<DockPane>())
			{
				if (Walk(nested) is { } result) return result;
			}

			return null;
		}

		return Walk(_rootPane);
	}
}
public partial class DockControl // handlers, forwarded handlers
{
	internal void OnPaneCloseRequested(ElementPane pane, RoutedEventArgs e)
	{
		Debug.WriteLine($"@xy DockControl::OnPaneCloseRequested");

		if (pane is DocumentPane) return; // n/a

		if (pane is ToolPane)
		{
			if (PaneClosingBehavior is DockPaneClosingBehavior.CloseItem)
			{
				if ((pane.SelectedItem ?? pane.Items.LastOrDefault()) is { } item)
				{
					pane.Remove(item);
				}
			}
			else
			{
				pane.ClearItems();
			}
			TryCloseEmptyPane(pane);
		}
	}
	internal void OnPaneDropEnter(ElementPane pane, DragEventArgs e)
	{
		Debug.WriteLine($"@xy DockControl::OnPaneDropEnter");

		var (container, item) = ExtractDragInfo(e.DataView);
		if (container is null) return;

		if (item is { })
		{
			if (pane.CanAcceptDrop(item))
			{
				e.AcceptedOperation = DataPackageOperation.Move;
				_dockingDiamond?.ShowAt(pane);
			}
			else
			{
				e.AcceptedOperation = DataPackageOperation.None;
				_dockingDiamond?.Hide();
			}
		}
		else
		{
			if (container != pane)
			{
				e.AcceptedOperation = DataPackageOperation.Move;
				_dockingDiamond?.ShowAt(pane);
			}
			else
			{
				e.AcceptedOperation = DataPackageOperation.None;
				_dockingDiamond?.Hide();
			}
		}
	}
	//internal void OnPaneDropOver(ElementPane pane, DragEventArgs e)
	//{
	//	//Debug.WriteLine($"@xy DockControl::OnPaneDropOver");
	//}
	internal void OnPaneDropLeave(ElementPane pane, DragEventArgs e)
	{
		Debug.WriteLine($"@xy DockControl::OnPaneDropLeave");

		e.AcceptedOperation = DataPackageOperation.None;
		_dockingDiamond?.Hide();
	}
	internal void OnPaneDrop(ElementPane pane, DragEventArgs e)
	{
		Debug.WriteLine($"@xy DockControl::OnPaneDrop: {pane.GetType().Name}, {_dockingDiamond?.Direction}");

		_dockingDiamond?.Hide();

		var (container, item) = ExtractDragInfo(e.DataView);
		if (container is null) return;

		// fixme@xy: sometimes we fail to reduce all unnecessary LayoutPane, repro?
		// todo@xy: merge both branches below

		if (item is { }) // dragging single DockItem
		{
			if (!pane.CanAcceptDrop(item)) return;

			var direction = _dockingDiamond?.Direction ?? DockDirection.None;
			if (direction is DockDirection.None)
			{
				if (container == pane) return;

				if (container.Remove(item))
				{
					pane.Add(item);
					pane.RepairTabView();

					TryCloseEmptyPane(container);
				}
			}
			else if (pane is DocumentPane { ParentPane: EditorPane editorPane })
			{
				if (pane.ParentPane is not { } parentPane) return;
				if (!(parentPane.NestedPanes.IndexOf(pane) is var index && index != -1)) return;

				// For document pane, if we previously had splited, then only allow split in the same orientation (horizontally/vertically).
				// If not, any direction is okay. If the document panes were to be reduced back to a single one again, remove the orientation restriction.
				if (!editorPane.IsOrientationLocked && direction.ToOrientation() is { } orientation)
				{
					editorPane.Orientation = orientation;
				}

				if (editorPane.Orientation == direction.ToOrientation() &&
					container.Remove(item))
				{
					var newPane = new DocumentPane();

					parentPane.NestedPanes.Insert(index + direction is DockDirection.Left or DockDirection.Top ? 0 : 1, newPane);

					newPane.Add(item);

					TryCloseEmptyPane(container);
				}
				else
				{
					// This case should be prevented by the DockingDiamond logics.
					throw new InvalidOperationException();
				}
			}
			else if (item is ToolItem)
			{
				if (pane.ParentPane is not { } parentPane) return;
				if (!(parentPane.NestedPanes.IndexOf(pane) is var index && index != -1)) return;

				if (container.Remove(item))
				{
					// if the direction is parallel to the parent layout, just insert the new pane before/after the target pane
					if (parentPane.Orientation == direction.ToOrientation())
					{
						var newPane = new ToolPane();

						parentPane.NestedPanes.Insert(index + direction is DockDirection.Left or DockDirection.Top ? 0 : 1, newPane);

						newPane.Add(item);
					}
					// if the direction is perpendicular to the parent layout,
					// remove the target pane from its parent, insert a new layout pane at its place,
					// add the target pane and the new pane.
					else
					{
						var newLayoutPane = new LayoutPane { Orientation = pane.ParentPane.Orientation.Opposite() };
						var newPane = new ToolPane();

						parentPane.NestedPanes.Remove(pane);
						if (direction is DockDirection.Left or DockDirection.Top)
						{
							newLayoutPane.NestedPanes.Add(newPane);
							newLayoutPane.NestedPanes.Add(pane);
						}
						else
						{
							newLayoutPane.NestedPanes.Add(pane);
							newLayoutPane.NestedPanes.Add(newPane);
						}
						parentPane.NestedPanes.Insert(index, newLayoutPane);

						pane.RepairTabView();

						newPane.Add(item);
					}

					TryCloseEmptyPane(container);
				}
			}
			else
			{
				throw new NotImplementedException($"DockControl::OnItemDropCompleted: {direction}");
			}
		}
		else // dragging entire DockPane
		{
			if (container is not ToolPane) return;

			var direction = _dockingDiamond?.Direction ?? DockDirection.None;
			if (direction is DockDirection.None)
			{
				if (container == pane) return;

				var items = container.Items.ToArray();

				container.ClearItems();
				foreach (var item2 in items)
				{
					pane.Add(item2);
				}
				TryCloseEmptyPane(container);
			}
			else if (pane is DocumentPane { ParentPane: EditorPane editorPane })
			{
				if (pane.ParentPane is not { } parentPane) return;
				if (!(parentPane.NestedPanes.IndexOf(pane) is var index && index != -1)) return;

				// For document pane, if we previously had splited, then only allow split in the same orientation (horizontally/vertically).
				// If not, any direction is okay. If the document panes were to be reduced back to a single one again, remove the orientation restriction.
				if (!editorPane.IsOrientationLocked && direction.ToOrientation() is { } orientation)
				{
					editorPane.Orientation = orientation;
				}

				if (editorPane.Orientation == direction.ToOrientation())
				{
					var newPane = new DocumentPane();

					parentPane.NestedPanes.Insert(index + direction is DockDirection.Left or DockDirection.Top ? 0 : 1, newPane);

					var items = container.Items.ToArray();
					container.ClearItems();
					foreach (var item2 in items)
					{
						newPane.Add(item2);
					}

					TryCloseEmptyPane(container);
				}
				else
				{
					// This case should be prevented by the DockingDiamond logics.
					throw new InvalidOperationException();
				}
			}
			else if (container is ToolPane)
			{
				if (container == pane) return; // can't move relative to itself
				if (pane.ParentPane is not { } parentPane) return;
				if (!(parentPane.NestedPanes.IndexOf(pane) is var index && index != -1)) return;

				var items = container.Items.ToArray();
				container.ClearItems();

				// if the direction is parallel to the parent layout, just insert the new pane before/after the target pane
				if (parentPane.Orientation == direction.ToOrientation())
				{
					var newPane = new ToolPane();

					parentPane.NestedPanes.Insert(index + direction is DockDirection.Left or DockDirection.Top ? 0 : 1, newPane);

					newPane.AddRange(items);
				}
				// if the direction is perpendicular to the parent layout,
				// remove the target pane from its parent, insert a new layout pane at its place,
				// add the target pane and the new pane.
				else
				{
					var newLayoutPane = new LayoutPane { Orientation = pane.ParentPane.Orientation.Opposite() };
					var newPane = new ToolPane();

					parentPane.NestedPanes.Remove(pane);
					if (direction is DockDirection.Left or DockDirection.Top)
					{
						newLayoutPane.NestedPanes.Add(newPane);
						newLayoutPane.NestedPanes.Add(pane);
					}
					else
					{
						newLayoutPane.NestedPanes.Add(pane);
						newLayoutPane.NestedPanes.Add(newPane);
					}
					parentPane.NestedPanes.Insert(index, newLayoutPane);

					pane.RepairTabView();
					
					newPane.AddRange(items);
				}

				TryCloseEmptyPane(container);
			}
			else
			{
				throw new NotImplementedException($"DockControl::OnItemDropCompleted: {direction}");
			}
		}
	}

	internal void OnPaneDragStarting(ElementPane pane, DragStartingEventArgs e)
	{
		Debug.WriteLine($"@xy DockPanel::OnPaneDragStarting");

		e.Data.Properties[PropertyKeys.Container] = pane;
	}
	internal void OnPaneDropCompleted(ElementPane pane, DropCompletedEventArgs e)
	{
		Debug.WriteLine($"@xy DockPanel::OnPaneDragStarting");
	}

	internal void OnItemCloseRequested(ElementPane pane, TabViewTabCloseRequestedEventArgs e)
	{
		Debug.WriteLine($"@xy DockControl::OnItemCloseRequested");

		if (e.Item is DockItem item)
		{
			pane.Remove(item);

			if (pane is ToolPane)
			{
				TryCloseEmptyPane(pane);
			}
			else if (pane is DocumentPane { ParentPane: EditorPane { NestedPanes.Count: > 1 } })
			{
				// we shouldn't close down the EditorPane's last nested pane
				TryCloseEmptyPane(pane);
			}
		}
	}
	internal void OnItemDragStarting(ElementPane pane, TabViewTabDragStartingEventArgs e)
	{
		Debug.WriteLine($"@xy DockControl::OnItemDragStarting");

		e.Data.Properties[PropertyKeys.Container] = pane;
		e.Data.Properties[PropertyKeys.Item] = e.Item;

		e.Data.RequestedOperation = DataPackageOperation.Move;
	}
	internal void OnItemDropCompleted(ElementPane pane, TabViewTabDragCompletedEventArgs e)
	{
		Debug.WriteLine($"@xy DockControl::OnItemDropCompleted");

		_dockingDiamond?.Hide();
	}
	internal void OnItemDroppedOutside(ElementPane pane, TabViewTabDroppedOutsideEventArgs e)
	{
		//Debug.WriteLine($"@xy DockControl::OnItemDroppedOutside");
	}
}
public partial class DockControl // helpers
{
	private void TryCloseEmptyPane(ElementPane pane)
	{
		if (pane is { ParentPane: { } parent, ElementCount: 0 })
		{
			if (parent.NestedPanes.Remove(pane))
			{
				ReduceEmptyPaneRecursively(parent);
			}
		}
	}
	private void ReduceEmptyPaneRecursively(LayoutPane? pane)
	{
		if (pane?.NestedPanes.Count == 0 &&
			pane.ParentPane?.NestedPanes is { } parentNestedPanes &&
			parentNestedPanes.Remove(pane) &&
			parentNestedPanes.Count == 0)
		{
			ReduceEmptyPaneRecursively(pane);
		}
	}
	private (ElementPane? Container, DockItem? Item) ExtractDragInfo(DataPackageView view)
	{
		if (ExtractValue<ElementPane>(view.Properties, PropertyKeys.Container, out var container))
		{
			if (ExtractValue<DockItem>(view.Properties, PropertyKeys.Item, out var item))
			{
				return (container, item);
			}
			return (container, null);
		}
		return (null, null);

		static bool ExtractValue<T>(IReadOnlyDictionary<string, object> dict, string key, out T? value) where T : class
		{
			value = dict.TryGetValue(key, out var result)
				? result as T
				: null;
			return value is { };
		}
	}
}
public partial class DockControl // debug
{
#if DEBUG
	internal string? LogicalTreeGraph() => LogicalTreeGraph(this);

	internal static string? LogicalTreeGraph(object? x)
	{
		return (x as DependencyObject)?.TreeGraph(Describe, GetMembers);

		IEnumerable<string> Describe(object x)
		{
			if (x is DockControl dc) return dc.GetDebugDescriptions();
			if (x is DockPane pane) return pane.GetDebugDescriptions();
			if (x is DockItem item) return [$"Header={item.Header}"];

			return [];
		}
		IEnumerable<object> GetMembers(object x, IEnumerable<object> members)
		{
			if (x is DockControl dc) return dc.GetLogicalMembers().TrimNull();
			if (x is DockPane pane) return pane.GetLogicalMembers().TrimNull();

			return [];
		}
	}

	internal object?[] GetLogicalMembers() => [_rootPane, _dockingDiamond];

	internal IEnumerable<string> GetDebugDescriptions() => [];
#endif
}
