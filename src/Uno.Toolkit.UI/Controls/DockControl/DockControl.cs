using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Uno.Extensions;
using Uno.Extensions.Specialized;
using Uno.UI.Extensions;

#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

using MUXC = Microsoft.UI.Xaml.Controls;

namespace Uno.Toolkit.UI;

public partial class DockControl : Control
{
	internal class PropertyKeys
	{
		public const string Item = nameof(Item);
		public const string Container = nameof(Container);
	}

	//private ItemsControl? _nestedPanesHost;
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

#if DEBUG
	internal object?[] GetLogicalMembers() => [_rootPane, _dockingDiamond];

	internal IEnumerable<string> GetDebugDescriptions() => [];
#endif

	private class DockControlElementFactory : MUXC.IElementFactoryShim
	{
		public UIElement GetElement(MUXC.ElementFactoryGetArgs args)
		{
			if (args.Data is DockPane pane)
			{
				return pane;
			}

			return new DockPane()
			{
				DataContext = args.Data,
			};
		}

		public void RecycleElement(MUXC.ElementFactoryRecycleArgs args)
		{
		}
	}
}
public partial class DockControl : Control // drag-and-drop
{
	internal void OnPaneDropEnter(ElementPane pane, DragEventArgs e)
	{
		Debug.WriteLine($"@xy DockControl::OnPaneDropEnter");
		
		var item = e.DataView.Properties[PropertyKeys.Item];
		if (pane.CanAcceptDrop(item))
		{
			_dockingDiamond?.ShowAt(pane);
		}
		else
		{
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

		_dockingDiamond?.Hide();
	}
	internal void OnPaneDrop(ElementPane targetPane, DragEventArgs e)
	{
		Debug.WriteLine($"@xy DockControl::OnPaneDrop: {_dockingDiamond?.Direction}");

		_dockingDiamond?.Hide();

		if (e.DataView.Properties[PropertyKeys.Container] is not ElementPane originPane) return;
		if (e.DataView.Properties[PropertyKeys.Item] is not DockItem item) return;

		var direction = _dockingDiamond?.Direction ?? DockDirection.None;
		if (direction is DockDirection.None)
		{
			if (originPane == targetPane) return;

			if (targetPane.CanAcceptDrop(item) && originPane.Remove(item))
			{
				targetPane.Add(item);
			}
		}
		else if (item is ToolItem)
		{
			if (targetPane.FindFirstAncestor<LayoutPane>() is not { } parentLayoutPane) return;
			if (!(parentLayoutPane.NestedPanes.IndexOf(targetPane) is var index && index != -1)) return;

			if (originPane.Remove(item))
			{
				ElementPane GetDestinationPane()
				{
					// if the direction is parallel to the parent layout, just insert the new pane before/after the target pane
					if (parentLayoutPane.Orientation == direction.ToOrientation())
					{
						var newPane = new ToolPane();
						
						parentLayoutPane.NestedPanes.Insert(index + direction is DockDirection.Right or DockDirection.Bottom ? 1 : 0, newPane);

						return newPane;
					}
					// if the direction is perpendicular to the parent layout,
					// remove the target pane from its parent, insert a new layout pane at its place,
					// add the target pane and the new pane.
					else
					{
						var newLayoutPane = new LayoutPane { Orientation = parentLayoutPane.Orientation.Opposite() };
						var newPane = new ToolPane();

						if (direction is DockDirection.Left or DockDirection.Top)
						{
							newLayoutPane.NestedPanes.Add(newPane);
							newLayoutPane.NestedPanes.Add(targetPane);
						}
						else
						{
							newLayoutPane.NestedPanes.Add(targetPane);
							newLayoutPane.NestedPanes.Add(newPane);
						}
						parentLayoutPane.NestedPanes.Remove(targetPane);
						parentLayoutPane.NestedPanes.Insert(index, newLayoutPane);

						return newPane;
					}
				}
				var destinationPane = GetDestinationPane();
				destinationPane.Add(item);
			}
		}
		
		// throw new NotImplementedException($"DockControl::OnItemDropCompleted: {direction}");
	}
	internal void OnItemDropCompleted(ElementPane pane, TabViewTabDragCompletedEventArgs e)
	{
		Debug.WriteLine($"@xy DockControl::OnItemDropCompleted");

		_dockingDiamond?.Hide();
	}
}