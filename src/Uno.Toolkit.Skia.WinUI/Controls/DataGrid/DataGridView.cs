using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using KumikoUI.Core;
using KumikoUI.Core.Editing;
using KumikoUI.Core.Input;
using KumikoUI.Core.Layout;
using KumikoUI.Core.Models;
using KumikoUI.Core.Rendering;
using KumikoUI.SkiaSharp;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SkiaSharp;
using Uno.WinUI.Graphics2DSK;
using Windows.Foundation;
using Windows.System;

namespace Uno.Toolkit.UI;

/// <summary>
/// Uno Platform port of KumikoUI DataGrid control.
/// All rendering is done by KumikoUI.Core/SkiaSharp — this class is the thin
/// platform host that wires Uno input events and lifecycle to the engine.
/// </summary>
[Microsoft.UI.Xaml.Markup.ContentProperty(Name = "Columns")]
public partial class DataGridView : Grid
{
	private readonly DataGridCanvas _canvasView;
	private readonly TextBox _keyboardProxy;
	private readonly DataGridRenderer _renderer = new();
	private readonly DataGridSource _dataSource = new();
	private readonly ScrollState _scroll = new();
	private readonly KumikoUI.Core.Models.SelectionModel _selection = new();
	private readonly GridInputController _inputController = new();
	private readonly EditSession _editSession = new();
	private DataGridStyle _style = new();

	// Inertial scroll timer
	private DispatcherTimer? _scrollTimer;

	// Cursor blink timer — drives regular repaints while editing
	private DispatcherTimer? _cursorBlinkTimer;
	private bool _filterPopupActive;

	// Double-tap detection
	private long _lastTapTimeMs;
	private float _lastTapX, _lastTapY;
	private int _pendingClickCount = 1;

	private const long DoubleTapThresholdMs = 400;
	private const float DoubleTapDistanceThreshold = 20f;

	// Long press detection
	private DispatcherTimer? _longPressTimer;
	private float _longPressX, _longPressY;
	private bool _longPressFired;
	private const long LongPressThresholdMs = 500;
	private const float LongPressMoveTolerance = 15f;

	// Track if we are suppressing TextChanged because we set the text programmatically
	private bool _suppressTextChanged;

	// Sentinel character used to detect backspace on TextBox.
	private const string KeyboardSentinel = "\u200B";

	// Canvas logical size cached from last RenderOverride
	private Size _canvasArea;

	// Theme mode backing field
	private DataGridThemeMode _themeMode = DataGridThemeMode.Light;

	public DataGridView()
	{
		// Accessibility
		Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(this, "Data grid");

		// Set up the hidden keyboard proxy (for keyboard input capture)
		_keyboardProxy = new TextBox
		{
			Opacity = 0,
			Height = 0,
			Width = 0,
			IsHitTestVisible = false,
			Text = KeyboardSentinel,
		};

		// HW-accelerated Skia canvas element — uses Uno's internal Skia surface
		_canvasView = new DataGridCanvas(this);
		_canvasView.HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch;
		_canvasView.VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Stretch;

		Children.Add(_canvasView);
		Children.Add(_keyboardProxy);

		// Auto-init collections so XAML can add items directly
		Columns = new ObservableCollection<DataGridColumn>();
		TableSummaryRows = new ObservableCollection<TableSummaryRow>();

		// Wire internal event handlers
		_dataSource.DataChanged += OnDataSourceDataChanged;
		_inputController.NeedsRedraw += OnInputControllerNeedsRedraw;
		_inputController.ColumnReordered += OnColumnReordered;
		_inputController.RowReordered += OnRowReordered;
		_inputController.AutoFitColumnRequested += OnAutoFitColumnRequested;
		_inputController.KeyboardFocusRequested += OnKeyboardFocusRequested;
		_inputController.FilterPopupOpened += OnFilterPopupOpened;
		_inputController.FilterPopupClosed += OnFilterPopupClosed;

		// KumikoUI's default ScrollSettings (WheelScrollMultiplier=3, etc.)
		// are tuned for MAUI where wheel deltas are normalized to 1 per notch.
		// WinUI reports 120 units per notch, so we normalize in the handler.

		// Wire edit session to input controller
		_inputController.EditSession = _editSession;
		_editSession.Style = _style;
		_editSession.NeedsRedraw += OnEditSessionNeedsRedraw;
		_editSession.CellBeginEdit += OnEditSessionCellBeginEdit;
		_editSession.CellEndEdit += OnEditSessionCellEndEdit;

		// Wire pointer events on the canvas
		_canvasView.PointerPressed += OnCanvasPointerPressed;
		_canvasView.PointerMoved += OnCanvasPointerMoved;
		_canvasView.PointerReleased += OnCanvasPointerReleased;
		_canvasView.PointerCanceled += OnCanvasPointerCanceled;
		_canvasView.PointerWheelChanged += OnCanvasPointerWheelChanged;

		// Wire keyboard proxy events
		_keyboardProxy.TextChanged += OnKeyboardProxyTextChanged;
		_keyboardProxy.KeyDown += OnKeyboardProxyKeyDown;

		// Lifecycle
		Loaded += OnLoaded;
		Unloaded += OnUnloaded;
	}

	// ── Public API ──

	/// <summary>Configurable scroll/inertia settings.</summary>
	public ScrollSettings ScrollSettings
	{
		get => _inputController.ScrollSettings;
		set => _inputController.ScrollSettings = value;
	}

	/// <summary>Gets the underlying data source.</summary>
	public DataGridSource DataSource => _dataSource;

	/// <summary>Gets the selection model.</summary>
	public KumikoUI.Core.Models.SelectionModel Selection => _selection;

	/// <summary>Gets the input controller for advanced event wiring.</summary>
	public GridInputController InputController => _inputController;

	/// <summary>Gets the edit session for inline cell editing.</summary>
	public EditSession EditSession => _editSession;

	/// <summary>Gets or sets the grid style / theme.</summary>
	public DataGridStyle GridStyle
	{
		get => _style;
		set
		{
			_style = value;
			_editSession.Style = value;
			InvalidateCanvas();
		}
	}

	/// <summary>
	/// Applies a built-in theme preset.
	/// </summary>
	public DataGridThemeMode Theme
	{
		get => _themeMode;
		set
		{
			if (_themeMode == value) return;
			_themeMode = value;
			_style = DataGridTheme.Create(value);
			_editSession.Style = _style;
			InvalidateCanvas();
		}
	}

	/// <summary>Set the data items.</summary>
	public void SetItemsSource(IEnumerable items)
	{
		_dataSource.SetItems(items);
	}

	/// <summary>Set the column definitions.</summary>
	public void SetColumns(IEnumerable<DataGridColumn> columns)
	{
		_dataSource.SetColumns(columns);
		InvalidateCanvas();
	}

	// ── Events ──

	/// <summary>Fires when a row is tapped.</summary>
	public event EventHandler<RowTappedEventArgs2>? RowTapped
	{
		add => _inputController.RowTapped += value;
		remove => _inputController.RowTapped -= value;
	}

	/// <summary>Fires when a row is double-tapped.</summary>
	public event EventHandler<RowTappedEventArgs2>? RowDoubleTapped
	{
		add => _inputController.RowDoubleTapped += value;
		remove => _inputController.RowDoubleTapped -= value;
	}

	/// <summary>Fires before a cell enters edit mode. Set Cancel=true to prevent.</summary>
	public event EventHandler<CellBeginEditEventArgs>? CellBeginEdit
	{
		add => _editSession.CellBeginEdit += value;
		remove => _editSession.CellBeginEdit -= value;
	}

	/// <summary>Fires after a cell exits edit mode.</summary>
	public event EventHandler<CellEndEditEventArgs>? CellEndEdit
	{
		add => _editSession.CellEndEdit += value;
		remove => _editSession.CellEndEdit -= value;
	}

	/// <summary>Fires when a cell value changes via editing.</summary>
	public event EventHandler<CellValueChangedEventArgs>? CellValueChanged
	{
		add => _editSession.CellValueChanged += value;
		remove => _editSession.CellValueChanged -= value;
	}

	// ── Lifecycle ──

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		// Nothing extra needed — constructor already wired everything.
		// On re-navigation, Loaded fires again and canvas will repaint.
		InvalidateCanvas();
	}

	private void OnUnloaded(object sender, RoutedEventArgs e)
	{
		StopInertialScrollTimer();
		StopCursorBlinkTimer();
		CancelLongPressTimer();
	}

	// ── Rendering ──

	/// <summary>
	/// Called by our inner <see cref="DataGridCanvas"/> when Uno's Skia
	/// pipeline is ready to paint.  The canvas is already scaled to logical
	/// units — no manual DPI compensation is needed.
	/// </summary>
	internal void Render(SKCanvas canvas, Size area)
	{
		_canvasArea = area;

		var bg = _style.BackgroundColor;
		canvas.Clear(new SKColor(bg.R, bg.G, bg.B, bg.A));

		_scroll.ViewportWidth = (float)area.Width;
		_scroll.ViewportHeight = (float)area.Height;

		using var drawingContext = new SkiaDrawingContext(canvas);

		_renderer.Render(drawingContext,
			_dataSource, _scroll, _selection, _style,
			_inputController.DragColumnIndex, _inputController.DragColumnScreenX,
			_inputController.DragRowIndex, _inputController.DragRowScreenY,
			_editSession, _inputController.PopupManager);
	}

	// ── Pointer Input ──

	private (float x, float y) GetScaledPosition(PointerRoutedEventArgs e)
	{
		// SKCanvasElement operates in logical units — no DPI scaling needed.
		var point = e.GetCurrentPoint(_canvasView).Position;
		return ((float)point.X, (float)point.Y);
	}

	private PointerButton MapPointerButton(PointerRoutedEventArgs e)
	{
		var props = e.GetCurrentPoint(_canvasView).Properties;
		if (props.IsRightButtonPressed) return PointerButton.Secondary;
		if (props.IsMiddleButtonPressed) return PointerButton.Middle;
		return PointerButton.Primary;
	}

	private void OnCanvasPointerPressed(object sender, PointerRoutedEventArgs e)
	{
		_canvasView.CapturePointer(e.Pointer);
		var (x, y) = GetScaledPosition(e);

		// Focus keyboard proxy for key events on desktop
		FocusKeyboardInput();

		// Double-tap detection
		int clickCount = 1;
		long nowMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		float dx = x - _lastTapX;
		float dy = y - _lastTapY;
		float dist = MathF.Sqrt(dx * dx + dy * dy);

		if (nowMs - _lastTapTimeMs < DoubleTapThresholdMs &&
			dist < DoubleTapDistanceThreshold)
		{
			clickCount = 2;
			_lastTapTimeMs = 0;
		}
		else
		{
			_lastTapTimeMs = nowMs;
		}

		_lastTapX = x;
		_lastTapY = y;
		_pendingClickCount = clickCount;

		// Start long press detection
		_longPressFired = false;
		_longPressX = x;
		_longPressY = y;
		StartLongPressTimer();

		var gridEvent = new GridPointerEventArgs
		{
			X = x,
			Y = y,
			Action = InputAction.Pressed,
			Button = MapPointerButton(e),
			TimestampMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
			ClickCount = clickCount
		};

		_inputController.HandlePointer(gridEvent, _scroll, _selection, _style, _dataSource);
		e.Handled = gridEvent.Handled;
	}

	private void OnCanvasPointerMoved(object sender, PointerRoutedEventArgs e)
	{
		var point = e.GetCurrentPoint(_canvasView);

		// Only forward moves when a button is pressed (drag).
		// On desktop, PointerMoved fires on hover too — KumikoUI expects
		// touch-style semantics where moves only occur during a press.
		if (!point.IsInContact)
			return;

		var (x, y) = GetScaledPosition(e);

		// Cancel long press if finger moved too far
		float lpDx = x - _longPressX;
		float lpDy = y - _longPressY;
		if (MathF.Sqrt(lpDx * lpDx + lpDy * lpDy) > LongPressMoveTolerance)
			CancelLongPressTimer();

		var gridEvent = new GridPointerEventArgs
		{
			X = x,
			Y = y,
			Action = InputAction.Moved,
			Button = MapPointerButton(e),
			TimestampMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
			ClickCount = 1
		};

		_inputController.HandlePointer(gridEvent, _scroll, _selection, _style, _dataSource);
		e.Handled = gridEvent.Handled;
	}

	private void OnCanvasPointerReleased(object sender, PointerRoutedEventArgs e)
	{
		_canvasView.ReleasePointerCapture(e.Pointer);
		CancelLongPressTimer();

		// If long press already fired, suppress Release to avoid double-triggering
		if (_longPressFired)
		{
			_longPressFired = false;
			e.Handled = true;
			return;
		}

		var (x, y) = GetScaledPosition(e);
		int clickCount = _pendingClickCount;
		_pendingClickCount = 1;

		var gridEvent = new GridPointerEventArgs
		{
			X = x,
			Y = y,
			Action = InputAction.Released,
			Button = MapPointerButton(e),
			TimestampMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
			ClickCount = clickCount
		};

		_inputController.HandlePointer(gridEvent, _scroll, _selection, _style, _dataSource);

		// Start/stop inertial scroll timer
		if (_inputController.IsInertialScrolling)
			StartInertialScrollTimer();

		e.Handled = gridEvent.Handled;
	}

	private void OnCanvasPointerCanceled(object sender, PointerRoutedEventArgs e)
	{
		_canvasView.ReleasePointerCapture(e.Pointer);
		CancelLongPressTimer();

		var (x, y) = GetScaledPosition(e);

		var gridEvent = new GridPointerEventArgs
		{
			X = x,
			Y = y,
			Action = InputAction.Cancelled,
			Button = PointerButton.Primary,
			TimestampMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
			ClickCount = 1
		};

		_inputController.HandlePointer(gridEvent, _scroll, _selection, _style, _dataSource);
		e.Handled = gridEvent.Handled;
	}

	private void OnCanvasPointerWheelChanged(object sender, PointerRoutedEventArgs e)
	{
		var (x, y) = GetScaledPosition(e);
		var point = e.GetCurrentPoint(_canvasView);

		// WinUI reports 120 units per wheel notch; KumikoUI expects
		// normalized deltas (~1 per notch) like MAUI provides.
		int wheelDelta = point.Properties.MouseWheelDelta;
		float normalizedDelta = wheelDelta / 120f;

		var gridEvent = new GridPointerEventArgs
		{
			X = x,
			Y = y,
			Action = InputAction.Scroll,
			Button = PointerButton.Primary,
			ScrollDeltaY = normalizedDelta,
			TimestampMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
			ClickCount = 1
		};

		_inputController.HandlePointer(gridEvent, _scroll, _selection, _style, _dataSource);

		// Start inertial scroll decay if KumikoUI set velocity
		if (_inputController.IsInertialScrolling)
			StartInertialScrollTimer();

		e.Handled = gridEvent.Handled;
	}

	// ── Long press detection ──

	private void StartLongPressTimer()
	{
		CancelLongPressTimer();
		_longPressTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(LongPressThresholdMs) };
		_longPressTimer.Tick += OnLongPressTimerTick;
		_longPressTimer.Start();
	}

	private void CancelLongPressTimer()
	{
		if (_longPressTimer != null)
		{
			_longPressTimer.Stop();
			_longPressTimer.Tick -= OnLongPressTimerTick;
			_longPressTimer = null;
		}
	}

	private void OnLongPressTimerTick(object? sender, object e)
	{
		CancelLongPressTimer();
		_longPressFired = true;

		var gridEvent = new GridPointerEventArgs
		{
			X = _longPressX,
			Y = _longPressY,
			Action = InputAction.LongPress,
			Button = PointerButton.Primary,
			TimestampMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
			ClickCount = 1
		};

		_inputController.HandlePointer(gridEvent, _scroll, _selection, _style, _dataSource);
	}

	// ── Inertial Scroll Timer ──

	private void StartInertialScrollTimer()
	{
		if (_scrollTimer != null) return;

		_scrollTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) }; // ~60fps
		_scrollTimer.Tick += OnScrollTimerTick;
		_scrollTimer.Start();
	}

	private void OnScrollTimerTick(object? sender, object e)
	{
		if (!_inputController.UpdateInertialScroll(_scroll, 16f))
		{
			StopInertialScrollTimer();
		}
	}

	private void StopInertialScrollTimer()
	{
		if (_scrollTimer != null)
		{
			_scrollTimer.Stop();
			_scrollTimer.Tick -= OnScrollTimerTick;
			_scrollTimer = null;
		}
	}

	// ── Cursor Blink Timer ──

	private void StartCursorBlinkTimer()
	{
		if (_cursorBlinkTimer != null) return;

		_cursorBlinkTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(530) };
		_cursorBlinkTimer.Tick += OnCursorBlinkTimerTick;
		_cursorBlinkTimer.Start();
	}

	private void OnCursorBlinkTimerTick(object? sender, object e)
	{
		if (_editSession.IsEditing || _filterPopupActive)
			_canvasView.Invalidate();
		else
			StopCursorBlinkTimer();
	}

	private void StopCursorBlinkTimer()
	{
		if (_cursorBlinkTimer != null)
		{
			_cursorBlinkTimer.Stop();
			_cursorBlinkTimer.Tick -= OnCursorBlinkTimerTick;
			_cursorBlinkTimer = null;
		}
	}

	// ── Named event handlers ──

	private void OnDataSourceDataChanged()
	{
		_canvasView.Invalidate();
		UpdateAccessibilityHint();
	}

	private void OnInputControllerNeedsRedraw() => _canvasView.Invalidate();

	private void OnEditSessionNeedsRedraw() => _canvasView.Invalidate();

	private void OnEditSessionCellBeginEdit(object? sender, CellBeginEditEventArgs e)
	{
		if (!e.Cancel)
		{
			StartCursorBlinkTimer();
			FocusKeyboardInput();
		}
	}

	private void OnKeyboardFocusRequested() => FocusKeyboardInput();

	private void OnFilterPopupOpened()
	{
		_filterPopupActive = true;
		StartCursorBlinkTimer();
	}

	private void OnFilterPopupClosed()
	{
		_filterPopupActive = false;
		StopCursorBlinkTimer();
	}

	private void OnEditSessionCellEndEdit(object? sender, CellEndEditEventArgs e)
	{
		StopCursorBlinkTimer();
		_selection.IsEditing = false;
		UnfocusKeyboardInput();
	}

	private void OnColumnReordered(object? sender, ColumnReorderedEventArgs e)
	{
		_dataSource.ReorderColumn(e.OldIndex, e.NewIndex);
	}

	private void OnRowReordered(object? sender, RowReorderedEventArgs e)
	{
		_dataSource.ReorderRow(e.OldIndex, e.NewIndex);
	}

	private void OnAutoFitColumnRequested(object? sender, AutoFitColumnEventArgs e)
	{
		using var bitmap = new SKBitmap(1, 1);
		using var canvas = new SKCanvas(bitmap);
		var measureCtx = new SkiaDrawingContext(canvas);
		var layoutEngine = new GridLayoutEngine();

		float optimalWidth = layoutEngine.CalculateAutoFitWidth(
			e.Column, _dataSource, measureCtx, _style);

		e.Column.Width = optimalWidth;
		InvalidateCanvas();
	}

	// ── Keyboard Proxy Handlers ──

	private void OnKeyboardProxyTextChanged(object sender, TextChangedEventArgs e)
	{
		if (_suppressTextChanged) return;

		string newText = _keyboardProxy.Text ?? string.Empty;

		// No meaningful change or programmatic reset
		if (newText == KeyboardSentinel) return;

		// Detect backspace: text became empty (user deleted the sentinel)
		if (string.IsNullOrEmpty(newText))
		{
			SendKeyEvent(GridKey.Backspace);
			ResetKeyboardProxy();
			return;
		}

		// Extract new characters — text after the sentinel
		string newInput;
		int sentinelIdx = newText.IndexOf(KeyboardSentinel, StringComparison.Ordinal);
		if (sentinelIdx >= 0)
		{
			newInput = newText.Remove(sentinelIdx, KeyboardSentinel.Length);
		}
		else
		{
			newInput = newText;
		}

		foreach (char ch in newInput)
		{
			SendKeyEvent(GridKey.Character, ch);
		}

		ResetKeyboardProxy();
	}

	private void OnKeyboardProxyKeyDown(object sender, KeyRoutedEventArgs e)
	{
		var modifiers = InputModifiers.None;
		var keyboardModifiers = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift);
		if (keyboardModifiers.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down))
			modifiers |= InputModifiers.Shift;
		var ctrlState = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control);
		if (ctrlState.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down))
			modifiers |= InputModifiers.Control;
		var altState = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Menu);
		if (altState.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down))
			modifiers |= InputModifiers.Alt;

		GridKey? gridKey = e.Key switch
		{
			VirtualKey.Enter => GridKey.Enter,
			VirtualKey.Escape => GridKey.Escape,
			VirtualKey.Tab => GridKey.Tab,
			VirtualKey.Left => GridKey.Left,
			VirtualKey.Right => GridKey.Right,
			VirtualKey.Up => GridKey.Up,
			VirtualKey.Down => GridKey.Down,
			VirtualKey.Home => GridKey.Home,
			VirtualKey.End => GridKey.End,
			VirtualKey.PageUp => GridKey.PageUp,
			VirtualKey.PageDown => GridKey.PageDown,
			VirtualKey.Delete => GridKey.Delete,
			VirtualKey.Back => GridKey.Backspace,
			VirtualKey.Space => GridKey.Space,
			VirtualKey.F2 => GridKey.F2,
			_ => null
		};

		if (gridKey.HasValue)
		{
			SendKeyEvent(gridKey.Value, null, modifiers);
			e.Handled = true;

			// Re-focus if editing continues after Enter
			if (gridKey.Value == GridKey.Enter && _editSession.IsEditing)
				FocusKeyboardInput();
		}
	}

	private void ResetKeyboardProxy()
	{
		_suppressTextChanged = true;
		_keyboardProxy.Text = KeyboardSentinel;
		_suppressTextChanged = false;
	}

	private void FocusKeyboardInput()
	{
		if (_keyboardProxy.FocusState == FocusState.Unfocused)
		{
			ResetKeyboardProxy();
			_keyboardProxy.Focus(FocusState.Programmatic);
		}
	}

	private void UnfocusKeyboardInput()
	{
		// Move focus away from the proxy to dismiss keyboard
		if (_keyboardProxy.FocusState != FocusState.Unfocused)
		{
			this.Focus(FocusState.Programmatic);
		}
	}

	private void SendKeyEvent(GridKey key, char? character = null, InputModifiers modifiers = InputModifiers.None)
	{
		var keyEvent = new GridKeyEventArgs
		{
			Key = key,
			Character = character,
			Modifiers = modifiers
		};

		_inputController.HandleKey(keyEvent, _scroll, _selection, _style, _dataSource);
	}

	// ── ItemsSource Change Handling ──

	private INotifyCollectionChanged? _boundCollectionChangedSource;

	private void OnItemsSourceChanged(IEnumerable? oldValue, IEnumerable? newValue)
	{
		if (_boundCollectionChangedSource != null)
		{
			_boundCollectionChangedSource.CollectionChanged -= OnBoundItemsCollectionChanged;
			_boundCollectionChangedSource = null;
		}

		if (newValue != null)
		{
			_dataSource.SetItems(newValue);

			if (newValue is INotifyCollectionChanged incc)
			{
				_boundCollectionChangedSource = incc;
				_boundCollectionChangedSource.CollectionChanged += OnBoundItemsCollectionChanged;
			}
		}
		else
		{
			_dataSource.SetItems(Array.Empty<object>());
		}
	}

	private void OnBoundItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		DispatcherQueue.TryEnqueue(InvalidateCanvas);
	}

	// ── Columns Change Handling ──

	private void OnColumnsChanged(
		ObservableCollection<DataGridColumn>? oldCollection,
		ObservableCollection<DataGridColumn>? newCollection)
	{
		if (oldCollection != null)
			oldCollection.CollectionChanged -= OnColumnsCollectionChanged;

		if (newCollection != null)
		{
			newCollection.CollectionChanged += OnColumnsCollectionChanged;
			_dataSource.SetColumns(newCollection);
		}
		else
		{
			_dataSource.SetColumns(Array.Empty<DataGridColumn>());
		}

		InvalidateCanvas();
	}

	private void OnColumnsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (Columns != null)
			_dataSource.SetColumns(Columns);
		InvalidateCanvas();
	}

	// ── TableSummaryRows Change Handling ──

	private void OnTableSummaryRowsChanged(
		ObservableCollection<TableSummaryRow>? oldCollection,
		ObservableCollection<TableSummaryRow>? newCollection)
	{
		if (oldCollection != null)
			oldCollection.CollectionChanged -= OnTableSummaryRowsCollectionChanged;

		if (newCollection != null)
		{
			newCollection.CollectionChanged += OnTableSummaryRowsCollectionChanged;
			SyncTableSummaryRows(newCollection);
		}
		else
		{
			_dataSource.ClearTableSummaryRows();
		}
	}

	private void OnTableSummaryRowsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (TableSummaryRows != null)
			SyncTableSummaryRows(TableSummaryRows);
	}

	private void SyncTableSummaryRows(IEnumerable<TableSummaryRow> rows)
	{
		_dataSource.ClearTableSummaryRows();
		foreach (var row in rows)
			_dataSource.AddTableSummaryRow(row);
	}

	// ── Helpers ──

	private void InvalidateCanvas() => _canvasView.Invalidate();

	private void UpdateAccessibilityHint()
	{
		int rows = _dataSource.RowCount;
		int cols = _dataSource.Columns.Count;
		string hint = $"{rows} rows, {cols} columns";

		if (_selection.CurrentCell.IsValid)
			hint += $", current cell row {_selection.CurrentCell.Row + 1} column {_selection.CurrentCell.Column + 1}";

		Microsoft.UI.Xaml.Automation.AutomationProperties.SetHelpText(this, hint);
	}

	/// <summary>
	/// Hardware-accelerated Skia canvas that delegates rendering to the
	/// owning <see cref="DataGridView"/>.  Lives as a private inner class
	/// so it can freely call internal members while staying invisible to
	/// the rest of the toolkit API surface.
	/// </summary>
	private sealed partial class DataGridCanvas : SKCanvasElement
	{
		private readonly DataGridView _owner;

		internal DataGridCanvas(DataGridView owner) => _owner = owner;

		protected override void RenderOverride(SKCanvas canvas, Size area)
			=> _owner.Render(canvas, area);
	}
}
