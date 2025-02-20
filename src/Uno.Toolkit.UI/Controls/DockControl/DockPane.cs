using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

using Windows.ApplicationModel.DataTransfer;
using Uno.UI.Extensions;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI;

public partial class DockPane : Control
{
	protected DockControl? DockControl => this.FindFirstAncestor<DockControl>();


#if DEBUG
	internal virtual object?[] GetLogicalMembers() => [];
	internal virtual IEnumerable<string> GetDebugDescriptions()
	{
		if (Parent is Grid)
		{
			int c = Grid.GetColumn(this), cspan = Grid.GetColumnSpan(this),
				r = Grid.GetRow(this), rspan = Grid.GetRowSpan(this);

			yield return $"R{FormatRange(r, rspan)}C{FormatRange(c, cspan)}";
			string FormatRange(int x, int span) => span > 1 ? $"{x}-{x + span - 1}" : $"{x}";
		}
	}
#endif
}

public partial class LayoutPane : DockPane
{
	#region DependencyProperty: Orientation

	public static DependencyProperty OrientationProperty { get; } = DependencyProperty.Register(
		nameof(Orientation),
		typeof(Orientation),
		typeof(LayoutPane),
		new PropertyMetadata(default(Orientation)));

	public Orientation Orientation
	{
		get => (Orientation)GetValue(OrientationProperty);
		set => SetValue(OrientationProperty, value);
	}

	#endregion

	#region DependencyProperty: NestedPanes

	public static DependencyProperty NestedPanesProperty { get; } = DependencyProperty.Register(
		nameof(NestedPanes),
		typeof(IList<DockPane>),
		typeof(LayoutPane),
		new PropertyMetadata(default(IList<DockPane>)));

	public IList<DockPane> NestedPanes
	{
		get => (IList<DockPane>)GetValue(NestedPanesProperty);
		set => SetValue(NestedPanesProperty, value);
	}

	#endregion

	private ItemsControl? _nestedPanesHost;
	private Grid? _itemsGrid;

	public LayoutPane()
	{
		var panes = new ObservableCollection<DockPane>();
		panes.CollectionChanged += OnNestedPanesCollectionChanged;

		NestedPanes = panes;
	}

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		_nestedPanesHost = GetTemplateChild("NestedPanesHost") as ItemsControl;
	}

	public void OnItemsPanelPrepared(DockPaneItemsGrid panel)
	{
		_itemsGrid = panel;

		panel.RowDefinitions.Clear();
		panel.ColumnDefinitions.Clear();

		var oneStar = new GridLength(1, GridUnitType.Star);
		foreach (var item in NestedPanes)
		{
			if (Orientation is Orientation.Horizontal)
			{
				panel.ColumnDefinitions.Add(new ColumnDefinition { Width = oneStar });
				Grid.SetColumn(item, panel.ColumnDefinitions.Count - 1);
			}
			else
			{
				panel.RowDefinitions.Add(new RowDefinition { Height = oneStar });
				Grid.SetRow(item, panel.RowDefinitions.Count - 1);
			}
		}
	}

	private void OnNestedPanesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		Debug.WriteLine($"OnNestedPanesCollectionChanged: {e.Action}");

		if (_itemsGrid is null) return;

		void FixTailIndex(int start)
		{
			for (int i = start; i < NestedPanes.Count; i++)
			{
				if (Orientation is Orientation.Horizontal)
				{
					Grid.SetColumn(NestedPanes[i], i);
				}
				else
				{
					Grid.SetRow(NestedPanes[i], i);
				}
			}
		}
		
		var oneStar = new GridLength(1, GridUnitType.Star);
		if (e.Action is NotifyCollectionChangedAction.Add)
		{
			for (int i = 0; i < e.NewItems!.Count; i++)
			{
				if (e.NewItems[i] is not DockPane pane) throw new InvalidOperationException($"Non DockPane child inserted: {e.NewItems[i]?.GetType().Name}");

				if (Orientation is Orientation.Horizontal)
				{
					_itemsGrid.ColumnDefinitions.Insert(e.NewStartingIndex + i, new ColumnDefinition { Width = oneStar });
					Grid.SetColumn(pane, e.NewStartingIndex + i);
				}
				else
				{
					_itemsGrid.RowDefinitions.Insert(e.NewStartingIndex + i, new RowDefinition { Height = oneStar });
					Grid.SetRow(pane, e.NewStartingIndex + i);
				}
			}
			FixTailIndex(e.NewStartingIndex + e.NewItems.Count);
		}
		else if (e.Action is NotifyCollectionChangedAction.Remove)
		{
			for (int i = 0; i < e.OldItems!.Count; i++)
			{
				if (e.OldItems[i] is not DockPane pane) throw new InvalidOperationException($"Non DockPane child inserted: {e.OldItems[i]?.GetType().Name}");

				if (Orientation is Orientation.Horizontal)
				{
					_itemsGrid.ColumnDefinitions.RemoveAt(e.OldStartingIndex + i);
					Grid.SetColumn(pane, 0);
				}
				else
				{
					_itemsGrid.RowDefinitions.RemoveAt(e.OldStartingIndex + i);
					Grid.SetRow(pane, 0);
				}
			}
			FixTailIndex(e.OldStartingIndex); // dont add Count here
		}
		else
		{
			throw new NotImplementedException($"OnNestedPanesCollectionChanged: {e.Action}");
		}
	}

#if DEBUG
	internal override IEnumerable<string> GetDebugDescriptions()
	{
		foreach (var line in base.GetDebugDescriptions())
		{
			yield return line;
		}

		yield return $"Orientation={Orientation}";
		if (_itemsGrid is { } g)
		{
			if (g.ColumnDefinitions is { Count: > 0 } columns)
			{
				yield return $"Columns={string.Join(',', columns.Select(PrettyPrint.FormatGridDefinition))}";
			}
			if (g.RowDefinitions is { Count: > 0 } rows)
			{
				yield return $"Rows={string.Join(',', rows.Select(PrettyPrint.FormatGridDefinition))}";
			}
		}
	}
	internal override object?[] GetLogicalMembers() => NestedPanes.ToArray();
#endif
}

public partial class RootPane : LayoutPane // should we merge this into DockControl?
{
	private AnchoredPane? _leftAnchoredPane;
	private AnchoredPane? _topAnchoredPane;
	private AnchoredPane? _rightAnchoredPane;
	private AnchoredPane? _bottomAnchoredPane;

	private DocumentPane? _documentPane;

	public DocumentPane? DocumentPane => _documentPane;

	public RootPane()
	{
		Orientation = Orientation.Vertical;
	}

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		_leftAnchoredPane = GetTemplateChild("LeftAnchoredPane") as AnchoredPane;
		_topAnchoredPane = GetTemplateChild("TopAnchoredPane") as AnchoredPane;
		_rightAnchoredPane = GetTemplateChild("RightAnchoredPane") as AnchoredPane;
		_bottomAnchoredPane = GetTemplateChild("BottomAnchoredPane") as AnchoredPane;

		PopulateNestedPanes();
	}
	private void PopulateNestedPanes()
	{
		//_nestedPanesHost?.Children.Add(_documentPane = new DocumentPane()
		NestedPanes.Add(_documentPane = new DocumentPane()
		{
#if DEBUG
			new DocumentItem { Header = "asd-1.cs", Content = "content: asdasd-1" },
			new DocumentItem { Header = "asd-2.cs", Content = "content: asdasd-2" },
			new DocumentItem { Header = "asd-3.cs", Content = "content: asdasd-3" },
			new ToolItem { Header = "tool-4", Title = "Tool-4 Window", Content = "content: tool-4" },
#endif
		});
		//_nestedPanesHost?.Children.Add(new ToolPane()
		NestedPanes.Add(new ToolPane()
		{
#if DEBUG
			new ToolItem { Header = "tool-5", Title = "Tool-5 Window", Content = "content: tool-5" },
			new ToolItem { Header = "tool-6", Title = "Tool-6 Window", Content = "content: tool-6" },
#endif
		});
	}

#if DEBUG
	internal override object?[] GetLogicalMembers() => NestedPanes.Concat([_leftAnchoredPane, _topAnchoredPane, _rightAnchoredPane, _bottomAnchoredPane]).ToArray();
#endif
}

public partial class AnchoredPane : DockPane // non-pinned pane, once pinned it should convert to tool pane
{
	#region DependencyProperty: Direction

	public static DependencyProperty DirectionProperty { get; } = DependencyProperty.Register(
		nameof(Direction),
		typeof(DockDirection),
		typeof(AnchoredPane),
		new PropertyMetadata(DockDirection.Left, OnDirectionChanged));

	public DockDirection Direction
	{
		get => (DockDirection)GetValue(DirectionProperty);
		set => SetValue(DirectionProperty, value);
	}

	#endregion

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		OnDirectionChanged();
	}

	private static void OnDirectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as AnchoredPane)?.OnDirectionChanged();
	private void OnDirectionChanged()
	{
		return;
		if (Direction is DockDirection.Top or DockDirection.Bottom)
		{
			Width = double.NaN;
			Height = 5;
		}
		else
		{
			Width = 5;
			Height = double.NaN;
		}
	}
}
public partial class PreviewPane : DockPane { }
public abstract partial class ElementPane : DockPane
{
	private TabView? _tabView;

	private ObservableCollection<DockItem> _items;

	public ElementPane()
	{
		_items = new ObservableCollection<DockItem>();
	}
	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		_tabView = GetTemplateChild("TabView") as TabView;

		AllowDrop = true;

		if (_tabView is { })
		{
			_tabView.TabCloseRequested += (s, e) =>
			{
				if (e.Item is DockItem item)
				{
					_items.Remove(item);
				}
			};
			_tabView.TabDragStarting += (s, e) =>
			{
				e.Data.Properties[DockControl.PropertyKeys.Container] = this;
				e.Data.Properties[DockControl.PropertyKeys.Item] = e.Item;
			};
			_tabView.TabDragCompleted += (s, e) =>
			{
				Debug.WriteLine("TabDragCompleted");

				DockControl?.OnItemDropCompleted(this, e);
			};
			_tabView.TabDroppedOutside += (s, e) =>
			{
				Debug.WriteLine("TabDroppedOutside");
			};

			_tabView.CanReorderTabs = true;
			_tabView.CanDragTabs = true;
			_tabView.TabItemsSource = _items;
		}
	}

	protected override void OnDragEnter(DragEventArgs e)
	{
		base.OnDragEnter(e);

		var item = e.DataView.Properties[DockControl.PropertyKeys.Item];
		if (CanAcceptDrop(item))
		{
			e.AcceptedOperation = DataPackageOperation.Move;
		}
		else
		{
			e.AcceptedOperation = DataPackageOperation.None;
		}

		DockControl?.OnPaneDropEnter(this, e);
	}

	//protected override void OnDragOver(DragEventArgs e)
	//{
	//	base.OnDragOver(e);
	//	DockControl?.OnPaneDropOver(this, e);
	//}

	protected override void OnDragLeave(DragEventArgs e)
	{
		base.OnDragLeave(e);
		DockControl?.OnPaneDropLeave(this, e);

		e.AcceptedOperation = DataPackageOperation.None;
	}
	protected override void OnDrop(DragEventArgs e)
	{
		base.OnDrop(e);
		DockControl?.OnPaneDrop(this, e);
	}

	public void Add(DockItem item)
	{
		_items.Add(item);
		if (_tabView is { })
		{
			_tabView.SelectedItem = item;
		}
	}
	public bool Remove(DockItem item)
	{
		return _items.Remove(item);
	}

	protected internal abstract bool CanAcceptDrop(object data);

#if DEBUG
	internal override object?[] GetLogicalMembers() => _items.ToArray();
#endif
}

public partial class ToolPane : ElementPane
{
	protected internal override bool CanAcceptDrop(object data)
	{
		return data is ToolItem;
	}
}
public partial class DocumentPane : ElementPane
{
	protected internal override bool CanAcceptDrop(object data)
	{
		return data is DocumentItem or ToolItem;
	}
}
