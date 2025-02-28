#if HAS_UNO
#define MANIPULATION_ABSOLUTE_COORD_ISSUE // https://github.com/unoplatform/uno/issues/6964
#endif

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Uno.UI.Extensions;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Input;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Core;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI;

// todo@xy: split into multiple files

public partial class DockPane : Control
{
	protected DockControl? DockControl => this.FindFirstAncestor<DockControl>();

	internal LayoutPane? ParentPane { get; set; }

#if DEBUG
	internal virtual object?[] GetLogicalMembers() => [];
	internal virtual IEnumerable<string> GetDebugDescriptions()
	{
		if (this is not RootPane && Parent is Grid)
		{
			int c = Grid.GetColumn(this), cspan = Grid.GetColumnSpan(this),
				r = Grid.GetRow(this), rspan = Grid.GetRowSpan(this);

			yield return $"R{FormatRange(r, rspan)}C{FormatRange(c, cspan)}";
			string FormatRange(int x, int span) => span > 1 ? $"{x}-{x + span - 1}" : $"{x}";
		}

		yield return $"ParentPane={ParentPane?.GetType().Name}";
	}
#endif
}

public partial class LayoutPane : DockPane, IEnumerable
{
	private record class CapturedManipulation(
		int Lane, // Index of left/top-side pane.
		Rect Rect, // Offset and Size for the handle area
		Point StartingPosition // Starting position for this manipulation.
	);

	#region DependencyProperty: Orientation

	public static DependencyProperty OrientationProperty { get; } = DependencyProperty.Register(
		nameof(Orientation),
		typeof(Orientation),
		typeof(LayoutPane),
		new PropertyMetadata(default(Orientation), OnOrientationChanged));

	public Orientation Orientation
	{
		get => (Orientation)GetValue(OrientationProperty);
		set => SetValue(OrientationProperty, value);
	}

	private static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as LayoutPane)?.OnOrientationChanged(e);

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
	private Border? _resizePreview;
	private DockPaneItemsGrid? _itemsPanel;

	private CapturedManipulation? _capturedManipulation;
	private Point? _lastPointerPosition;

	public LayoutPane()
	{
		var panes = new ObservableCollection<DockPane>();
		panes.CollectionChanged += OnNestedPanesCollectionChanged;

		NestedPanes = panes;
	}
	public LayoutPane(Orientation orientation) : this()
	{
		Orientation = orientation;
	}

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		_nestedPanesHost = GetTemplateChild("NestedPanesHost") as ItemsControl;
		_resizePreview = GetTemplateChild("ResizePreview") as Border;

		if (_resizePreview is { })
		{
			_resizePreview.IsHitTestVisible = false;
		}
	}

	public void OnItemsPanelPrepared(DockPaneItemsGrid panel)
	{
		Debug.WriteLine($"OnItemsPanelPrepared // {GetType().Name}");
		if (_itemsPanel == panel)
		{
			// OnItemsPanelPrepared will be called twice from .Loaded.
			// This avoid an unnecessary call, and duplicated event registration.
			return;
		}

		_itemsPanel = panel;
		if (_itemsPanel is { })
		{
			if (_itemsPanel.Background is null)
			{
				// ensure there is a background, so we can receive the manipulation events.
				_itemsPanel.Background = new SolidColorBrush(Colors.Transparent);
			}

			_itemsPanel.PointerPressed += OnItemsPanelPointerPressed;
			_itemsPanel.ManipulationStarted += OnItemsPanelManipulationStarted;
			_itemsPanel.ManipulationDelta += OnItemsPanelManipulationDelta;
			_itemsPanel.ManipulationCompleted += OnItemsPanelManipulationCompleted;
		}

		UpdateManipulationMode();
		UpdateChildrenLayout();
	}

	private void OnNestedPanesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		Debug.WriteLine($"OnNestedPanesCollectionChanged: {e.Action}");

		UpdateParentPane();
		UpdateInpactedChildrenLayout();

		void UpdateParentPane()
		{
			if (e.NewItems is { })
			{
				foreach (var item in e.NewItems)
				{
					if (item is DockPane pane)
					{
						if (pane.ParentPane is not null)
						{
							throw new InvalidOperationException("DockPane is added under another LayoutPane, before being removed from previous parent.");
						}
						pane.ParentPane = this;
					}
				}
			}
			if (e.OldItems is { })
			{
				foreach (var item in e.OldItems)
				{
					if (item is DockPane pane)
					{
						pane.ParentPane = null;
					}
				}
			}
		}
		void UpdateInpactedChildrenLayout()
		{
			if (_itemsPanel is null) return;

			var oneStar = new GridLength(1, GridUnitType.Star);
			if (e.Action is NotifyCollectionChangedAction.Add)
			{
				for (int i = 0; i < e.NewItems!.Count; i++)
				{
					if (e.NewItems[i] is not DockPane pane) throw new InvalidOperationException($"Non DockPane child inserted: {e.NewItems[i]?.GetType().Name}");

					if (Orientation is Orientation.Horizontal)
					{
						_itemsPanel.ColumnDefinitions.Insert(e.NewStartingIndex + i, new ColumnDefinition { Width = oneStar });
						Grid.SetColumn(pane, e.NewStartingIndex + i);
					}
					else
					{
						_itemsPanel.RowDefinitions.Insert(e.NewStartingIndex + i, new RowDefinition { Height = oneStar });
						Grid.SetRow(pane, e.NewStartingIndex + i);
					}
				}
				RepairTailIndex(e.NewStartingIndex + e.NewItems.Count);
			}
			else if (e.Action is NotifyCollectionChangedAction.Remove)
			{
				for (int i = 0; i < e.OldItems!.Count; i++)
				{
					if (e.OldItems[i] is not DockPane pane) throw new InvalidOperationException($"Non DockPane child inserted: {e.OldItems[i]?.GetType().Name}");

					if (Orientation is Orientation.Horizontal)
					{
						_itemsPanel.ColumnDefinitions.RemoveAt(e.OldStartingIndex + i);
						Grid.SetColumn(pane, 0);
					}
					else
					{
						_itemsPanel.RowDefinitions.RemoveAt(e.OldStartingIndex + i);
						Grid.SetRow(pane, 0);
					}
				}
				RepairTailIndex(e.OldStartingIndex); // dont add Count here
			}
			else
			{
				throw new NotImplementedException($"OnNestedPanesCollectionChanged: {e.Action}");
			}
		}
		void RepairTailIndex(int start)
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
	}

	private void OnOrientationChanged(DependencyPropertyChangedEventArgs e)
	{
		UpdateManipulationMode();
		UpdateChildrenLayout();
	}


	private void OnItemsPanelPointerPressed(object sender, PointerRoutedEventArgs e)
	{
		_lastPointerPosition = e.GetCurrentPoint(this).Position;
	}
	protected void OnItemsPanelManipulationStarted(object? sender, ManipulationStartedRoutedEventArgs e)
	{
		//Debug.WriteLine($"OnManipulationStarted: hash={e.GetHashCode():X8}, Container={e.Container.GetType().Name}, Source={e.OriginalSource.GetType().Name} // {GetType().Name}");
		_capturedManipulation = null;

		if (_itemsPanel == null) return;
		if (sender is not DockPaneItemsGrid panel) return;

		// 1. ManipulationStarted: Block bubbling with `e.Handled = true`. This is so that, in step-2, we don't complete LayoutPane-B event from LayoutPane-A.
		e.Handled = true;
		if (panel != _itemsPanel) return;

		var p = CoreWindow.GetForCurrentThread()?.PointerPosition;

		// for all manipulation events: // tested for Manipulation(Started|Delta|Completed)
		// - uno: Source=[top level UIElement CURRENTLY under cursor)], Container=[always ItemsPanel]
		//		^ live, based from cursor placement when the event is raised, not locked to starting location
		// - win: Source=[always ItemsPanel], Container=**ERRATIC**[based on STARTING click: ItemsPanel if clicked on UIElement above ItemsPanel, otherwise null]
		//		^ for the entire event chain, the properties stay the same from where the starting click happens.
		// ^ It would be easier to extract from the pointer position... Plus, we will need to calculate rect of "GridSplitter" anyways.

		// There are some descrepencies between with the Manipulate events' e.Position:
		// - win: cursor position in sender's coordinate space
		//		^ alternate between sender-relative coords and app-absolute coords somehow, despite everything...
		// - uno: cursor position in the app's coordinate space (same as skia, except app always sits at 0.0)
		// - uno-skia: cursor position in the combined screens' coordinate space (absolute of absolute)
		// So, we are using the last recorded pointer position from PointerPressed, since that one is consistent AND always in local coords.

		var position = _lastPointerPosition ?? e.Position;
		var capture = CaptureManipulation(position);
		Debug.WriteLine($"OnManipulationStarted: position={position}, capture={capture} // {GetType().Name}");
		if (capture is { })
		{
			_capturedManipulation = capture;

			if (_resizePreview is { })
			{
				_resizePreview.Margin = new Thickness(
					_capturedManipulation.Rect.X,
					_capturedManipulation.Rect.Y,
					0,
					0
				);
				_resizePreview.Width = capture.Rect.Width;
				_resizePreview.Height = capture.Rect.Height;
				_resizePreview.Visibility = Visibility.Visible;
			}
		}
		else
		{
			// 2. ManipulationStarted: Prematurely `e.Complete()` events that doesn't start in the Grid row/column-spacing.
			// On Uno, OriginalSource and Container are always re-calculated from current cursor location.
			// Unlike, on windows, where they are locked to the starting click.
			// Because of this, ManipulationStarted is the only place where we can determine the starting point...
			e.Complete();
		}
		//Debug.WriteLine(capture is { } ? "accepted" : "refused");
	}
	protected void OnItemsPanelManipulationDelta(object? sender, ManipulationDeltaRoutedEventArgs e)
	{
		if (_itemsPanel == null) return;

		e.Handled = true;
		if (sender as DockPaneItemsGrid != _itemsPanel) return;
		if (_capturedManipulation is null) return;

		//Debug.WriteLine($"OnManipulationDelta: Container={e.Container?.GetType().Name}, Source={e.OriginalSource?.GetType().Name} // {GetType().Name}");
		//Debug.WriteLine($"OnManipulationDelta: p0={e.Position.Subtract(e.Cumulative.Translation)}, et={e.Cumulative.Translation}, dt={e.Delta.Translation} // {GetType().Name}");
		//Debug.WriteLine($"OnManipulationDelta: et={e.Cumulative.Translation}, p={CorrectManipulationPosition(e.Position)} // {GetType().Name}");

		if (_resizePreview is { })
		{
			_resizePreview.Margin = new Thickness(
				_capturedManipulation.Rect.X + e.Cumulative.Translation.X,
				_capturedManipulation.Rect.Y + e.Cumulative.Translation.Y,
				0,
				0
			);
			_resizePreview.Visibility = Visibility.Visible;
		}
	}
	protected void OnItemsPanelManipulationCompleted(object? sender, ManipulationCompletedRoutedEventArgs e)
	{
		if (_itemsPanel == null) return;

		e.Handled = true;
		if (sender as DockPaneItemsGrid != _itemsPanel) return;
		if (_capturedManipulation is null) return;

		//Debug.WriteLine($"OnItemsPanelManipulationCompleted: Container={e.Container?.GetType().Name}, Source={e.OriginalSource?.GetType().Name} // {GetType().Name}");
		//Debug.WriteLine($"OnManipulationCompleted: et={e.Cumulative.Translation}, p={CorrectManipulationPosition(e.Position)} // {GetType().Name}");

		// todo@xy: add safeguard against column mutation since ManipulationStarted

		if (Orientation is Orientation.Horizontal)
		{
			var left = _itemsPanel.ColumnDefinitions[_capturedManipulation.Lane];
			var right = _itemsPanel.ColumnDefinitions[_capturedManipulation.Lane + 1];

			if (left.Width.IsAuto || left.Width.IsAbsolute) throw new NotImplementedException();
			if (right.Width.IsAuto || right.Width.IsAbsolute) throw new NotImplementedException();

			var targetLeftWidth = Math.Clamp(left.ActualWidth + e.Cumulative.Translation.X, 0, left.ActualWidth + right.ActualWidth);
			var totalWidth = left.ActualWidth + right.ActualWidth;
			var totalStarValue = left.Width.Value + right.Width.Value;

			var targetLeftStarValue = targetLeftWidth / totalWidth * totalStarValue;
			var targetRightStarValue = totalStarValue - targetLeftStarValue;

			left.Width = new GridLength(targetLeftStarValue, GridUnitType.Star);
			right.Width = new GridLength(targetRightStarValue, GridUnitType.Star);
		}
		else
		{
			var up = _itemsPanel.RowDefinitions[_capturedManipulation.Lane];
			var down = _itemsPanel.RowDefinitions[_capturedManipulation.Lane + 1];

			if (up.Height.IsAuto || up.Height.IsAbsolute) throw new NotImplementedException();
			if (down.Height.IsAuto || down.Height.IsAbsolute) throw new NotImplementedException();

			var targetLeftHeight = Math.Clamp(up.ActualHeight + e.Cumulative.Translation.Y, 0, up.ActualHeight + down.ActualHeight);
			var totalHeight = up.ActualHeight + down.ActualHeight;
			var totalStarValue = up.Height.Value + down.Height.Value;

			var targetUpStarValue = targetLeftHeight / totalHeight * totalStarValue;
			var targetDownStarValue = totalStarValue - targetUpStarValue;

			up.Height = new GridLength(targetUpStarValue, GridUnitType.Star);
			down.Height = new GridLength(targetDownStarValue, GridUnitType.Star);
		}

		if (_resizePreview is { })
		{
			_resizePreview.Margin = default;
			_resizePreview.Width = 0;
			_resizePreview.Height = 0;
			_resizePreview.Visibility = Visibility.Collapsed;
		}
	}

	private void UpdateChildrenLayout()
	{
		if (_itemsPanel is null) return;

		_itemsPanel.RowDefinitions.Clear();
		_itemsPanel.ColumnDefinitions.Clear();

		var oneStar = new GridLength(1, GridUnitType.Star);
		foreach (var item in NestedPanes)
		{
			if (Orientation is Orientation.Horizontal)
			{
				_itemsPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = oneStar });
				Grid.SetColumn(item, _itemsPanel.ColumnDefinitions.Count - 1);
			}
			else
			{
				_itemsPanel.RowDefinitions.Add(new RowDefinition { Height = oneStar });
				Grid.SetRow(item, _itemsPanel.RowDefinitions.Count - 1);
			}
		}
	}
	private void UpdateManipulationMode()
	{
		if (_itemsPanel is null) return;

		_itemsPanel.ManipulationMode = Orientation == Orientation.Horizontal
			? ManipulationModes.TranslateX | ManipulationModes.TranslateRailsX
			: ManipulationModes.TranslateY | ManipulationModes.TranslateRailsY;
	}

	public void Add(DockPane pane)
	{
		NestedPanes.Add(pane);
	}

	private CapturedManipulation? CaptureManipulation(Point position)
	{
		if (_itemsPanel is null) return null;
		if (NestedPanes.Count < 1) return null;

		var lanes = NestedPanes
			.ZipSkipOne((a, b) => Orientation == Orientation.Horizontal
				? RectExtensions.FromLTRB(
					a.ActualOffset.X + a.ActualWidth,
					a.ActualOffset.Y,
					b.ActualOffset.X,
					b.ActualOffset.Y + b.ActualHeight)
				: RectExtensions.FromLTRB(
					a.ActualOffset.X,
					a.ActualOffset.Y + a.ActualHeight,
					b.ActualOffset.X + b.ActualWidth,
					b.ActualOffset.Y)
			)
			.Select((x, i) => new { Index = i, Rect = x, IsMatch = x.Contains(position) });
		if (lanes.FirstOrDefault(x => x.IsMatch) is not { } matched) return null;

		return new CapturedManipulation(matched.Index, matched.Rect, position);
	}

	IEnumerator IEnumerable.GetEnumerator() => NestedPanes.GetEnumerator();

#if DEBUG
	internal override IEnumerable<string> GetDebugDescriptions()
	{
		foreach (var line in base.GetDebugDescriptions())
		{
			yield return line;
		}

		yield return $"Orientation={Orientation}";
		if (_itemsPanel is { } g)
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
public partial class RootPane : LayoutPane // todo@xy: should we merge this into DockControl?
{
	private AnchoredPane? _leftAnchoredPane;
	private AnchoredPane? _topAnchoredPane;
	private AnchoredPane? _rightAnchoredPane;
	private AnchoredPane? _bottomAnchoredPane;

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
		NestedPanes.Add(new EditorPane()
		{
			new DocumentPane()
			{
#if DEBUG
				new DocumentItem { Header = "asd-1.cs", Content = "content: asdasd-1" },
				new DocumentItem { Header = "asd-2.cs", Content = "content: asdasd-2" },
				new DocumentItem { Header = "asd-3.cs", Content = "content: asdasd-3" },
				new ToolItem { Header = "tool-4", Title = "Tool-4 Window", Content = "content: tool-4" },
#endif
			}
		});
#if DEBUG
		NestedPanes.Add(new LayoutPane(Orientation.Opposite())
		{
			new ToolPane()
			{
				new ToolItem { Header = "tool-5", Title = "Tool-5 Window", Content = "content: tool-5" },
			},
			new ToolPane()
			{
				new ToolItem { Header = "tool-6", Title = "Tool-6 Window", Content = "content: tool-6" },
			}
		});
#endif
	}

#if DEBUG
	internal override object?[] GetLogicalMembers() => NestedPanes.Concat([_leftAnchoredPane, _topAnchoredPane, _rightAnchoredPane, _bottomAnchoredPane]).ToArray();
#endif
}
public partial class EditorPane : LayoutPane
{
	public bool IsOrientationLocked => NestedPanes.Count > 1;
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
public abstract partial class ElementPane : DockPane, IEnumerable
{
	public int ElementCount => _items.Count;

	private TabView? _tabView;

	private ObservableCollection<DockItem> _items;

	public ElementPane()
	{
		_items = new ObservableCollection<DockItem>();

		AllowDrop = true;
	}
	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		_tabView = GetTemplateChild("TabView") as TabView;
		if (_tabView is { })
		{
			_tabView.TabCloseRequested += (s, e) => DockControl?.OnItemCloseRequested(this, e);
			_tabView.TabDragStarting += (s, e) => DockControl?.OnItemDragStarting(this, e);
			_tabView.TabDragCompleted += (s, e) => DockControl?.OnItemDropCompleted(this, e);
			_tabView.TabDroppedOutside += (s, e) => DockControl?.OnItemDroppedOutside(this, e);

			_tabView.CanReorderTabs = true;
			_tabView.CanDragTabs = true;
			_tabView.TabItemsSource = _items;
		}
	}

	protected override void OnDragEnter(DragEventArgs e)
	{
		base.OnDragEnter(e);
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

	internal void RepairTabView()
	{
		if (_tabView is null) return;

		var index = _tabView.SelectedIndex;

		_tabView.TabItemsSource = null;
		_tabView.TabItemsSource = _items;

		_tabView.SelectedIndex = index;
	}
	
	IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

#if DEBUG
	internal override object?[] GetLogicalMembers() => _items.ToArray();
#endif
}

public partial class DocumentPane : ElementPane
{
	protected internal override bool CanAcceptDrop(object data)
	{
		return data is DocumentItem or ToolItem;
	}
}
public partial class ToolPane : ElementPane
{
	protected internal override bool CanAcceptDrop(object data)
	{
		return data is ToolItem;
	}
}
