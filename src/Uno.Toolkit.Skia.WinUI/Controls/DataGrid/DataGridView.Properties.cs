using System.Collections;
using System.Collections.ObjectModel;
using KumikoUI.Core.Editing;
using KumikoUI.Core.Models;
using Microsoft.UI.Xaml;

namespace Uno.Toolkit.UI;

public partial class DataGridView
{
	#region DependencyProperty: ItemsSource

	public static DependencyProperty ItemsSourceProperty { get; } = DependencyProperty.Register(
		nameof(ItemsSource),
		typeof(IEnumerable),
		typeof(DataGridView),
		new PropertyMetadata(default(IEnumerable), (d, e) =>
			((DataGridView)d).OnItemsSourceChanged((IEnumerable?)e.OldValue, (IEnumerable?)e.NewValue)));

	public IEnumerable? ItemsSource
	{
		get => (IEnumerable?)GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}

	#endregion

	#region DependencyProperty: Columns

	public static DependencyProperty ColumnsProperty { get; } = DependencyProperty.Register(
		nameof(Columns),
		typeof(ObservableCollection<DataGridColumn>),
		typeof(DataGridView),
		new PropertyMetadata(default(ObservableCollection<DataGridColumn>), (d, e) =>
			((DataGridView)d).OnColumnsChanged(
				e.OldValue as ObservableCollection<DataGridColumn>,
				e.NewValue as ObservableCollection<DataGridColumn>)));

	public ObservableCollection<DataGridColumn>? Columns
	{
		get => (ObservableCollection<DataGridColumn>?)GetValue(ColumnsProperty);
		set => SetValue(ColumnsProperty, value);
	}

	#endregion

	#region DependencyProperty: TableSummaryRows

	public static DependencyProperty TableSummaryRowsProperty { get; } = DependencyProperty.Register(
		nameof(TableSummaryRows),
		typeof(ObservableCollection<TableSummaryRow>),
		typeof(DataGridView),
		new PropertyMetadata(default(ObservableCollection<TableSummaryRow>), (d, e) =>
			((DataGridView)d).OnTableSummaryRowsChanged(
				e.OldValue as ObservableCollection<TableSummaryRow>,
				e.NewValue as ObservableCollection<TableSummaryRow>)));

	public ObservableCollection<TableSummaryRow>? TableSummaryRows
	{
		get => (ObservableCollection<TableSummaryRow>?)GetValue(TableSummaryRowsProperty);
		set => SetValue(TableSummaryRowsProperty, value);
	}

	#endregion

	#region DependencyProperty: FrozenRowCount

	public static DependencyProperty FrozenRowCountProperty { get; } = DependencyProperty.Register(
		nameof(FrozenRowCount),
		typeof(int),
		typeof(DataGridView),
		new PropertyMetadata(0, (d, e) =>
		{
			var view = (DataGridView)d;
			view._dataSource.FrozenRowCount = (int)e.NewValue;
		}));

	/// <summary>Number of top data rows to freeze when scrolling vertically.</summary>
	public int FrozenRowCount
	{
		get => (int)GetValue(FrozenRowCountProperty);
		set => SetValue(FrozenRowCountProperty, value);
	}

	#endregion

	#region DependencyProperty: GridSelectionMode

	public static DependencyProperty GridSelectionModeProperty { get; } = DependencyProperty.Register(
		nameof(GridSelectionMode),
		typeof(KumikoUI.Core.Models.SelectionMode),
		typeof(DataGridView),
		new PropertyMetadata(KumikoUI.Core.Models.SelectionMode.Extended, (d, e) =>
		{
			var view = (DataGridView)d;
			view.Selection.Mode = (KumikoUI.Core.Models.SelectionMode)e.NewValue;
			view.InvalidateCanvas();
		}));

	/// <summary>Selection mode: None, Single, Multiple, or Extended.</summary>
	public KumikoUI.Core.Models.SelectionMode GridSelectionMode
	{
		get => (KumikoUI.Core.Models.SelectionMode)GetValue(GridSelectionModeProperty);
		set => SetValue(GridSelectionModeProperty, value);
	}

	#endregion

	#region DependencyProperty: IsReadOnly

	public static DependencyProperty IsReadOnlyProperty { get; } = DependencyProperty.Register(
		nameof(IsReadOnly),
		typeof(bool),
		typeof(DataGridView),
		new PropertyMetadata(false));

	/// <summary>When true, cells cannot be edited.</summary>
	public bool IsReadOnly
	{
		get => (bool)GetValue(IsReadOnlyProperty);
		set => SetValue(IsReadOnlyProperty, value);
	}

	#endregion

	#region DependencyProperty: DismissKeyboardOnEnter

	public static DependencyProperty DismissKeyboardOnEnterProperty { get; } = DependencyProperty.Register(
		nameof(DismissKeyboardOnEnter),
		typeof(bool),
		typeof(DataGridView),
		new PropertyMetadata(true, (d, e) =>
			((DataGridView)d)._editSession.DismissKeyboardOnEnter = (bool)e.NewValue));

	/// <summary>
	/// When true (default), pressing Enter while editing commits the edit,
	/// dismisses the keyboard, and ends editing. When false, Enter commits
	/// the current cell and automatically begins editing the cell below.
	/// </summary>
	public bool DismissKeyboardOnEnter
	{
		get => (bool)GetValue(DismissKeyboardOnEnterProperty);
		set => SetValue(DismissKeyboardOnEnterProperty, value);
	}

	#endregion

	#region DependencyProperty: EditTriggers

	public static DependencyProperty EditTriggersProperty { get; } = DependencyProperty.Register(
		nameof(EditTriggers),
		typeof(EditTrigger),
		typeof(DataGridView),
		new PropertyMetadata(EditTrigger.Default, (d, e) =>
			((DataGridView)d)._editSession.EditTriggers = (EditTrigger)e.NewValue));

	/// <summary>
	/// Configures which user actions trigger cell editing.
	/// Default: DoubleTap | F2Key | Typing.
	/// </summary>
	public EditTrigger EditTriggers
	{
		get => (EditTrigger)GetValue(EditTriggersProperty);
		set => SetValue(EditTriggersProperty, value);
	}

	#endregion

	#region DependencyProperty: EditTextSelectionMode

	public static DependencyProperty EditTextSelectionModeProperty { get; } = DependencyProperty.Register(
		nameof(EditTextSelectionMode),
		typeof(EditTextSelectionMode),
		typeof(DataGridView),
		new PropertyMetadata(EditTextSelectionMode.SelectAll, (d, e) =>
			((DataGridView)d)._editSession.TextSelectionMode = (EditTextSelectionMode)e.NewValue));

	/// <summary>
	/// Controls how text is selected when a cell enters edit mode.
	/// SelectAll (default): all text is selected.
	/// CursorAtEnd: cursor is placed at the end with no selection.
	/// </summary>
	public EditTextSelectionMode EditTextSelectionMode
	{
		get => (EditTextSelectionMode)GetValue(EditTextSelectionModeProperty);
		set => SetValue(EditTextSelectionModeProperty, value);
	}

	#endregion

	#region DependencyProperty: AllowSorting

	public static DependencyProperty AllowSortingProperty { get; } = DependencyProperty.Register(
		nameof(AllowSorting),
		typeof(bool),
		typeof(DataGridView),
		new PropertyMetadata(true));

	/// <summary>When false, column header taps do not sort.</summary>
	public bool AllowSorting
	{
		get => (bool)GetValue(AllowSortingProperty);
		set => SetValue(AllowSortingProperty, value);
	}

	#endregion

	#region DependencyProperty: AllowFiltering

	public static DependencyProperty AllowFilteringProperty { get; } = DependencyProperty.Register(
		nameof(AllowFiltering),
		typeof(bool),
		typeof(DataGridView),
		new PropertyMetadata(true));

	/// <summary>When false, filter icons are not shown in column headers.</summary>
	public bool AllowFiltering
	{
		get => (bool)GetValue(AllowFilteringProperty);
		set => SetValue(AllowFilteringProperty, value);
	}

	#endregion

	#region DependencyProperty: HeaderHeight

	public static DependencyProperty HeaderHeightProperty { get; } = DependencyProperty.Register(
		nameof(HeaderHeight),
		typeof(float),
		typeof(DataGridView),
		new PropertyMetadata(40f, (d, e) =>
		{
			var view = (DataGridView)d;
			view._style.HeaderHeight = (float)e.NewValue;
			view.InvalidateCanvas();
		}));

	/// <summary>Height of the column header row in pixels.</summary>
	public float HeaderHeight
	{
		get => (float)GetValue(HeaderHeightProperty);
		set => SetValue(HeaderHeightProperty, value);
	}

	#endregion

	#region DependencyProperty: RowHeight

	public static DependencyProperty RowHeightProperty { get; } = DependencyProperty.Register(
		nameof(RowHeight),
		typeof(float),
		typeof(DataGridView),
		new PropertyMetadata(36f, (d, e) =>
		{
			var view = (DataGridView)d;
			view._style.RowHeight = (float)e.NewValue;
			view.InvalidateCanvas();
		}));

	/// <summary>Height of each data row in pixels.</summary>
	public float RowHeight
	{
		get => (float)GetValue(RowHeightProperty);
		set => SetValue(RowHeightProperty, value);
	}

	#endregion

	#region DependencyProperty: GridDescription

	public static DependencyProperty GridDescriptionProperty { get; } = DependencyProperty.Register(
		nameof(GridDescription),
		typeof(string),
		typeof(DataGridView),
		new PropertyMetadata("Data grid", (d, e) =>
		{
			var view = (DataGridView)d;
			Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(view, (string)e.NewValue);
		}));

	/// <summary>Accessible description for screen readers.</summary>
	public string GridDescription
	{
		get => (string)GetValue(GridDescriptionProperty);
		set => SetValue(GridDescriptionProperty, value);
	}

	#endregion
}
