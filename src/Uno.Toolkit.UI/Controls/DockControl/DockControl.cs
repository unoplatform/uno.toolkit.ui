using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		public const string Item = nameof(Item);
		public const string Container = nameof(Container);
	}

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
			_rootPane?.DocumentPane?.Add(item);
		}
	}
}
public partial class DockControl // handlers, forwarded handlers
{
	internal void OnPaneDropEnter(ElementPane pane, DragEventArgs e)
	{
		Debug.WriteLine($"@xy DockControl::OnPaneDropEnter");

		var item = e.DataView.Properties[PropertyKeys.Item];
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
		Debug.WriteLine($"@xy DockControl::OnPaneDrop: {pane.GetType().Name}, {_dockingDiamond?.Direction}, {e.AcceptedOperation}");

		_dockingDiamond?.Hide();

		if (e.AcceptedOperation is DataPackageOperation.None) return;
		if (e.DataView.Properties[PropertyKeys.Container] is not ElementPane originPane) return;
		if (e.DataView.Properties[PropertyKeys.Item] is not DockItem item) return;
		if (!pane.CanAcceptDrop(item)) return;

		var direction = _dockingDiamond?.Direction ?? DockDirection.None;
		if (direction is DockDirection.None)
		{
			if (originPane == pane) return;

			if (originPane.Remove(item))
			{
				pane.Add(item);
				TryCloseEmptyPane(originPane);
			}
		}
		else if (item is DocumentItem & pane is DocumentPane)
		{
			// For document pane, if we previously had splited, then only allow split in the same orientation (horizontally/vertically).
			// If not, any direction is okay. If the document panes were to be reduced to a single pane again, remove the orientation restriction.
			throw new NotImplementedException();
		}
		else if (item is ToolItem)
		{
			if (pane.ParentPane is not { } parentPane) return;
			if (!(parentPane.NestedPanes.IndexOf(pane) is var index && index != -1)) return;

			if (originPane.Remove(item))
			{
				// if the direction is parallel to the parent layout, just insert the new pane before/after the target pane
				if (parentPane.Orientation == direction.ToOrientation())
				{
					var newPane = new ToolPane();

					parentPane.NestedPanes.Insert(index + direction is DockDirection.Right or DockDirection.Bottom ? 1 : 0, newPane);

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

				TryCloseEmptyPane(originPane);
			}
		}
		else
		{
			throw new NotImplementedException($"DockControl::OnItemDropCompleted: {direction}");
		}
	}

	//internal void OnItemAdded(ElementPane pane, DockItem item) { }
	//internal void OnItemRemoved(ElementPane pane, DockItem item) { }
	internal void OnItemCloseRequested(ElementPane pane, TabViewTabCloseRequestedEventArgs e)
	{
		Debug.WriteLine($"@xy DockControl::OnItemCloseRequested");

		if (e.Item is DockItem item)
		{
			pane.Remove(item);
			TryCloseEmptyPane(pane);
		}
	}
	internal void OnItemDragStarting(ElementPane pane, TabViewTabDragStartingEventArgs e)
	{
		Debug.WriteLine($"@xy DockControl::OnItemDragStarting");

		e.Data.Properties[PropertyKeys.Container] = pane;
		e.Data.Properties[PropertyKeys.Item] = e.Item;
	}
	internal void OnItemDropCompleted(ElementPane pane, TabViewTabDragCompletedEventArgs e)
	{
		Debug.WriteLine($"@xy DockControl::OnItemDropCompleted");

		_dockingDiamond?.Hide();
	}
	internal void OnItemDroppedOutside(ElementPane pane, TabViewTabDroppedOutsideEventArgs e)
	{
		Debug.WriteLine($"@xy DockControl::OnItemDroppedOutside");
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
