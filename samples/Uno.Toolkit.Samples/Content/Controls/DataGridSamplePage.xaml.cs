using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using KumikoUI.Core.Editing;
using KumikoUI.Core.Models;
using KumikoUI.Core.Rendering;

namespace Uno.Toolkit.Samples.Content.Controls;

[SamplePage(SampleCategory.Controls, "DataGrid", Description = "Skia-rendered data grid powered by KumikoUI — full feature showcase.")]
public sealed partial class DataGridSamplePage : Page
{
	private static readonly string[] Departments = { "Engineering", "Marketing", "Sales", "HR", "Finance", "Design", "Support" };
	private static readonly string[] Cities = { "New York", "San Francisco", "Austin", "Seattle", "Chicago", "Denver", "Boston" };
	private static readonly string[] Levels = { "Junior", "Mid", "Senior", "Staff", "Principal", "Director" };
	private static readonly string[] FirstNames = { "Alice", "Bob", "Charlie", "Diana", "Eve", "Frank", "Grace", "Hank", "Iris", "Jack" };
	private static readonly string[] LastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Wilson", "Moore" };

	private static readonly string[] Categories = { "Electronics", "Books", "Clothing", "Food", "Toys", "Sports", "Health", "Auto" };
	private static readonly string[] Regions = { "North", "South", "East", "West", "Central" };
	private static readonly string[] Suppliers = { "Acme Corp", "GlobalTrade", "MegaSupply", "TechParts", "EcoGoods" };
	private static readonly string[] ProductNames = { "Widget A", "Gadget B", "Gizmo C", "Doohickey D", "Thingamajig E", "Whatchamacallit F" };

	private readonly ObservableCollection<Employee> _employees = new();
	private readonly ObservableCollection<GroupingEmployee> _groupingEmployees = new();
	private readonly Random _random = new(42);

	// Track current edit trigger for cycling
	private int _editTriggerIndex;
	private static readonly EditTrigger[] EditTriggerValues = { EditTrigger.DoubleTap, EditTrigger.SingleTap, EditTrigger.LongPress, EditTrigger.SingleTap | EditTrigger.DoubleTap | EditTrigger.LongPress };
	private static readonly string[] EditTriggerLabels = { "DoubleTap", "SingleTap", "LongPress", "All" };

	public DataGridSamplePage()
	{
		this.InitializeComponent();
		this.Loaded += OnLoaded;
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		// ── Tab switching ──
		TabAllComponents.Checked += OnTabChecked;
		TabGrouping.Checked += OnTabChecked;
		TabLargeData.Checked += OnTabChecked;
		TabTheming.Checked += OnTabChecked;

		SetupAllComponentsTab();
		SetupGroupingTab();
		SetupLargeDataTab();
		SetupThemingTab();
	}

	private void OnTabChecked(object sender, RoutedEventArgs e)
	{
		// Uncheck other tabs
		var tabs = new[] { TabAllComponents, TabGrouping, TabLargeData, TabTheming };
		foreach (var tab in tabs)
		{
			if (tab != sender)
				tab.IsChecked = false;
		}

		AllComponentsPanel.Visibility = TabAllComponents.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
		GroupingPanel.Visibility = TabGrouping.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
		LargeDataPanel.Visibility = TabLargeData.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
		ThemingPanel.Visibility = TabTheming.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
	}

	// ═══════════════════════════════════════════════════════
	//  TAB 1: All Components
	// ═══════════════════════════════════════════════════════

	private void SetupAllComponentsTab()
	{
		AddRowBtn.Click += OnAddRowClicked;
		RemoveLastBtn.Click += OnRemoveLastClicked;
		UpdateSalaryBtn.Click += OnUpdateSalaryClicked;
		RandomizeBtn.Click += OnRandomizeRatingsClicked;
		RowDragBtn.Click += OnToggleRowDragClicked;
		ThemeBtn.Click += OnToggleThemeClicked;
		HandlePosBtn.Click += OnToggleHandlePositionClicked;
		DismissKBBtn.Click += OnToggleDismissKeyboardClicked;
		EditTriggerBtn.Click += OnToggleEditTriggerClicked;
		SelectionModeBtn.Click += OnToggleSelectionModeClicked;

		var columns = new ObservableCollection<DataGridColumn>
		{
			new DataGridColumn { Header = "Id", PropertyName = "Id", Width = 60, ColumnType = DataGridColumnType.Numeric, TextAlignment = GridTextAlignment.Right, IsReadOnly = true, AllowTabStop = false, IsFrozen = true },
			new DataGridColumn { Header = "Name", PropertyName = "Name", Width = 160, IsFrozen = true },
			new DataGridColumn { Header = "Department", PropertyName = "Department", Width = 140, ColumnType = DataGridColumnType.ComboBox, EditorItemsString = "Engineering,Marketing,Sales,HR,Finance,Design,Support" },
			new DataGridColumn { Header = "Salary", PropertyName = "Salary", Width = 120, Format = "C0", ColumnType = DataGridColumnType.Numeric, TextAlignment = GridTextAlignment.Right },
			new DataGridColumn { Header = "Hire Date", PropertyName = "HireDate", Width = 120, Format = "yyyy-MM-dd", ColumnType = DataGridColumnType.Date },
			new DataGridColumn { Header = "Active", PropertyName = "IsActive", Width = 80, ColumnType = DataGridColumnType.Boolean, AllowTabStop = false },
			new DataGridColumn { Header = "Level", PropertyName = "Level", Width = 120, ColumnType = DataGridColumnType.Picker, EditorItemsString = "Junior,Mid,Senior,Staff,Principal,Director" },
			new DataGridColumn
			{
				Header = "Performance", PropertyName = "Performance", Width = 140, ColumnType = DataGridColumnType.Template, IsReadOnly = true,
				CustomCellRenderer = new ProgressBarCellRenderer
				{
					Minimum = 0, Maximum = 100, TrackHeight = 12, CornerRadius = 6, ShowText = true, TextFormat = "N0",
					ColorSelector = (value, min, max) =>
					{
						var pct = (value - min) / (max - min);
						return pct switch { < 0.3 => new GridColor(220, 53, 69), < 0.6 => new GridColor(255, 193, 7), _ => new GridColor(40, 167, 69) };
					},
				},
			},
			new DataGridColumn { Header = "Rating", PropertyName = "Rating", Width = 100, ColumnType = DataGridColumnType.Template, EditorDescriptor = new NumericUpDownEditorDescriptor { Minimum = 1, Maximum = 5, Step = 1, DecimalPlaces = 0 } },
			new DataGridColumn { Header = "City", PropertyName = "City", Width = 130, FreezeMode = ColumnFreezeMode.Right },
		};

		SampleDataGrid.Columns = columns;
		SampleDataGrid.EditTriggers = EditTrigger.DoubleTap | EditTrigger.F2Key | EditTrigger.Typing;
		SampleDataGrid.EditTextSelectionMode = EditTextSelectionMode.SelectAll;

		var style = SampleDataGrid.GridStyle;
		style.ShowRowDragHandle = true;
		SampleDataGrid.GridStyle = style;

		SampleDataGrid.TableSummaryRows = new ObservableCollection<TableSummaryRow>
		{
			new TableSummaryRow
			{
				Name = "Averages", Title = "Averages", Position = SummaryPosition.Top,
				Columns = new List<SummaryColumnDescription>
				{
					new SummaryColumnDescription { PropertyName = "Salary", SummaryType = SummaryType.Average, Format = "C0" },
					new SummaryColumnDescription { PropertyName = "Performance", SummaryType = SummaryType.Average, Format = "N1" },
					new SummaryColumnDescription { PropertyName = "Rating", SummaryType = SummaryType.Average, Format = "N1" },
				},
			},
			new TableSummaryRow
			{
				Name = "Totals", Title = "Totals", Position = SummaryPosition.Bottom,
				Columns = new List<SummaryColumnDescription>
				{
					new SummaryColumnDescription { PropertyName = "Salary", SummaryType = SummaryType.Sum, Format = "C0" },
					new SummaryColumnDescription { PropertyName = "Id", SummaryType = SummaryType.Count, Label = "Rows: " },
					new SummaryColumnDescription { PropertyName = "Rating", SummaryType = SummaryType.Max, Label = "Best: " },
				},
			},
		};

		LoadAllComponentsData();
		SampleDataGrid.ItemsSource = _employees;
	}

	private void LoadAllComponentsData()
	{
		for (var i = 1; i <= 200; i++)
		{
			_employees.Add(new Employee
			{
				Id = i,
				Name = $"{FirstNames[_random.Next(FirstNames.Length)]} {LastNames[_random.Next(LastNames.Length)]}",
				Department = Departments[_random.Next(Departments.Length)],
				Salary = _random.Next(45_000, 180_000),
				HireDate = DateTime.Today.AddDays(-_random.Next(100, 3000)),
				IsActive = _random.NextDouble() > 0.15,
				City = Cities[_random.Next(Cities.Length)],
				Level = Levels[_random.Next(Levels.Length)],
				Performance = Math.Round(_random.NextDouble() * 100, 1),
				Rating = _random.Next(1, 6),
			});
		}
	}

	private void OnAddRowClicked(object sender, RoutedEventArgs e)
	{
		var nextId = _employees.Count + 1;
		_employees.Add(new Employee
		{
			Id = nextId, Name = $"New Employee {nextId}", Department = "Engineering",
			Salary = _random.Next(50_000, 150_000), HireDate = DateTime.Today, IsActive = true,
			City = "Boston", Level = "Junior", Performance = Math.Round(_random.NextDouble() * 100, 1), Rating = _random.Next(1, 6),
		});
	}

	private void OnRemoveLastClicked(object sender, RoutedEventArgs e)
	{
		if (_employees.Count > 0) _employees.RemoveAt(_employees.Count - 1);
	}

	private void OnUpdateSalaryClicked(object sender, RoutedEventArgs e)
	{
		if (_employees.Count > 0) _employees[_random.Next(_employees.Count)].Salary = _random.Next(45_000, 200_000);
	}

	private void OnRandomizeRatingsClicked(object sender, RoutedEventArgs e)
	{
		for (var i = 0; i < 10 && _employees.Count > 0; i++)
		{
			var idx = _random.Next(_employees.Count);
			_employees[idx].Rating = _random.Next(1, 6);
			_employees[idx].Performance = Math.Round(_random.NextDouble() * 100, 1);
		}
	}

	private void OnToggleRowDragClicked(object sender, RoutedEventArgs e)
	{
		var gridStyle = SampleDataGrid.GridStyle;
		if (gridStyle.ShowRowDragHandle)
		{
			gridStyle.ShowRowDragHandle = false;
			gridStyle.AllowRowDragDrop = true;
			RowDragBtn.Content = "Row Drag: Full Row";
		}
		else if (gridStyle.AllowRowDragDrop)
		{
			gridStyle.AllowRowDragDrop = false;
			RowDragBtn.Content = "Row Drag: OFF";
		}
		else
		{
			gridStyle.ShowRowDragHandle = true;
			RowDragBtn.Content = "Row Drag: Handle";
		}
		SampleDataGrid.GridStyle = gridStyle;
	}

	private void OnToggleThemeClicked(object sender, RoutedEventArgs e)
	{
		SampleDataGrid.Theme = SampleDataGrid.Theme switch
		{
			DataGridThemeMode.Light => DataGridThemeMode.Dark,
			DataGridThemeMode.Dark => DataGridThemeMode.HighContrast,
			_ => DataGridThemeMode.Light,
		};
		ThemeBtn.Content = $"Theme: {SampleDataGrid.Theme}";
		var gridStyle = SampleDataGrid.GridStyle;
		gridStyle.ShowRowDragHandle = true;
		SampleDataGrid.GridStyle = gridStyle;
		RowDragBtn.Content = "Row Drag: Handle";
	}

	private void OnToggleHandlePositionClicked(object sender, RoutedEventArgs e)
	{
		var gridStyle = SampleDataGrid.GridStyle;
		if (gridStyle.RowDragHandlePosition == DragHandlePosition.Left)
		{
			gridStyle.RowDragHandlePosition = DragHandlePosition.Right;
			HandlePosBtn.Content = "Handle: Right";
		}
		else
		{
			gridStyle.RowDragHandlePosition = DragHandlePosition.Left;
			HandlePosBtn.Content = "Handle: Left";
		}
		SampleDataGrid.GridStyle = gridStyle;
	}

	private void OnToggleDismissKeyboardClicked(object sender, RoutedEventArgs e)
	{
		SampleDataGrid.DismissKeyboardOnEnter = !SampleDataGrid.DismissKeyboardOnEnter;
		DismissKBBtn.Content = $"Dismiss KB: {(SampleDataGrid.DismissKeyboardOnEnter ? "ON" : "OFF")}";
	}

	private void OnToggleEditTriggerClicked(object sender, RoutedEventArgs e)
	{
		_editTriggerIndex = (_editTriggerIndex + 1) % EditTriggerValues.Length;
		SampleDataGrid.EditTriggers = EditTriggerValues[_editTriggerIndex] | EditTrigger.F2Key | EditTrigger.Typing;
		EditTriggerBtn.Content = $"Edit: {EditTriggerLabels[_editTriggerIndex]}";
	}

	private void OnToggleSelectionModeClicked(object sender, RoutedEventArgs e)
	{
		SampleDataGrid.EditTextSelectionMode = SampleDataGrid.EditTextSelectionMode == EditTextSelectionMode.SelectAll
			? EditTextSelectionMode.CursorAtEnd
			: EditTextSelectionMode.SelectAll;
		SelectionModeBtn.Content = $"Selection: {SampleDataGrid.EditTextSelectionMode}";
	}

	// ═══════════════════════════════════════════════════════
	//  TAB 2: Grouping
	// ═══════════════════════════════════════════════════════

	private void SetupGroupingTab()
	{
		GroupDeptBtn.Click += OnGroupByDeptClicked;
		GroupLevelBtn.Click += OnGroupByLevelClicked;
		GroupBothBtn.Click += OnGroupByBothClicked;
		ClearGroupBtn.Click += OnClearGroupsClicked;
		ExpandAllBtn.Click += OnExpandAllClicked;
		CollapseAllBtn.Click += OnCollapseAllClicked;
		AddGroupSummaryBtn.Click += OnAddGroupSummaryClicked;
		ToggleFilteringBtn.Click += OnToggleFilteringClicked;

		var columns = new ObservableCollection<DataGridColumn>
		{
			new DataGridColumn { Header = "Id", PropertyName = "Id", Width = 60, ColumnType = DataGridColumnType.Numeric, TextAlignment = GridTextAlignment.Right, IsReadOnly = true },
			new DataGridColumn { Header = "Name", PropertyName = "Name", Width = 160 },
			new DataGridColumn { Header = "Department", PropertyName = "Department", Width = 140 },
			new DataGridColumn { Header = "Salary", PropertyName = "Salary", Width = 120, Format = "C0", ColumnType = DataGridColumnType.Numeric, TextAlignment = GridTextAlignment.Right },
			new DataGridColumn { Header = "Level", PropertyName = "Level", Width = 120 },
			new DataGridColumn { Header = "Active", PropertyName = "IsActive", Width = 80, ColumnType = DataGridColumnType.Boolean },
			new DataGridColumn { Header = "City", PropertyName = "City", Width = 130 },
		};

		GroupingDataGrid.Columns = columns;

		GroupingDataGrid.TableSummaryRows = new ObservableCollection<TableSummaryRow>
		{
			new TableSummaryRow
			{
				Name = "Summary", Title = "Summary", Position = SummaryPosition.Bottom,
				Columns = new List<SummaryColumnDescription>
				{
					new SummaryColumnDescription { PropertyName = "Id", SummaryType = SummaryType.Count, Label = "Total: " },
					new SummaryColumnDescription { PropertyName = "Salary", SummaryType = SummaryType.Average, Format = "C0" },
				},
			},
		};

		LoadGroupingData();
		GroupingDataGrid.ItemsSource = _groupingEmployees;
	}

	private void LoadGroupingData()
	{
		for (var i = 1; i <= 300; i++)
		{
			_groupingEmployees.Add(new GroupingEmployee
			{
				Id = i,
				Name = $"{FirstNames[_random.Next(FirstNames.Length)]} {LastNames[_random.Next(LastNames.Length)]}",
				Department = Departments[_random.Next(Departments.Length)],
				Salary = _random.Next(45_000, 180_000),
				Level = Levels[_random.Next(Levels.Length)],
				IsActive = _random.NextDouble() > 0.15,
				City = Cities[_random.Next(Cities.Length)],
			});
		}
	}

	private void OnGroupByDeptClicked(object sender, RoutedEventArgs e)
	{
		GroupingDataGrid.DataSource.ClearGroupDescriptions();
		GroupingDataGrid.DataSource.AddGroupDescription(new GroupDescription("Department"));
	}

	private void OnGroupByLevelClicked(object sender, RoutedEventArgs e)
	{
		GroupingDataGrid.DataSource.ClearGroupDescriptions();
		GroupingDataGrid.DataSource.AddGroupDescription(new GroupDescription("Level"));
	}

	private void OnGroupByBothClicked(object sender, RoutedEventArgs e)
	{
		GroupingDataGrid.DataSource.ClearGroupDescriptions();
		GroupingDataGrid.DataSource.AddGroupDescription(new GroupDescription("Department"));
		GroupingDataGrid.DataSource.AddGroupDescription(new GroupDescription("Level"));
	}

	private void OnClearGroupsClicked(object sender, RoutedEventArgs e)
	{
		GroupingDataGrid.DataSource.ClearGroupDescriptions();
	}

	private void OnExpandAllClicked(object sender, RoutedEventArgs e)
	{
		GroupingDataGrid.DataSource.ExpandAllGroups();
	}

	private void OnCollapseAllClicked(object sender, RoutedEventArgs e)
	{
		GroupingDataGrid.DataSource.CollapseAllGroups();
	}

	private void OnAddGroupSummaryClicked(object sender, RoutedEventArgs e)
	{
		GroupingDataGrid.DataSource.ClearGroupSummaryRows();
		GroupingDataGrid.DataSource.AddGroupSummaryRow(new GroupSummaryRow
		{
			Columns = new List<SummaryColumnDescription>
			{
				new SummaryColumnDescription { PropertyName = "Id", SummaryType = SummaryType.Count, Label = "Count: " },
				new SummaryColumnDescription { PropertyName = "Salary", SummaryType = SummaryType.Average, Format = "C0" },
			},
		});
	}

	private void OnToggleFilteringClicked(object sender, RoutedEventArgs e)
	{
		GroupingDataGrid.AllowFiltering = !GroupingDataGrid.AllowFiltering;
		ToggleFilteringBtn.Content = $"Filtering: {(GroupingDataGrid.AllowFiltering ? "ON" : "OFF")}";
	}

	// ═══════════════════════════════════════════════════════
	//  TAB 3: Large Data
	// ═══════════════════════════════════════════════════════

	private ObservableCollection<LargeDataItem>? _largeDataItems;

	private void SetupLargeDataTab()
	{
		Load10KBtn.Click += (_, _) => LoadLargeData(10_000);
		Load50KBtn.Click += (_, _) => LoadLargeData(50_000);
		Load100KBtn.Click += (_, _) => LoadLargeData(100_000);
		ClearDataBtn.Click += OnClearLargeDataClicked;
		UpdateRandomBtn.Click += OnUpdateRandomClicked;
		LargeThemeBtn.Click += OnToggleLargeThemeClicked;

		var columns = new ObservableCollection<DataGridColumn>
		{
			new DataGridColumn { Header = "#", PropertyName = "RowNumber", Width = 70, ColumnType = DataGridColumnType.Numeric, TextAlignment = GridTextAlignment.Right, IsReadOnly = true },
			new DataGridColumn { Header = "Full Name", PropertyName = "FullName", Width = 180 },
			new DataGridColumn { Header = "Category", PropertyName = "Category", Width = 120 },
			new DataGridColumn { Header = "Value", PropertyName = "Value", Width = 100, Format = "N2", ColumnType = DataGridColumnType.Numeric, TextAlignment = GridTextAlignment.Right },
			new DataGridColumn { Header = "Amount", PropertyName = "Amount", Width = 120, Format = "C2", ColumnType = DataGridColumnType.Numeric, TextAlignment = GridTextAlignment.Right },
			new DataGridColumn { Header = "Created", PropertyName = "CreatedDate", Width = 120, Format = "yyyy-MM-dd", ColumnType = DataGridColumnType.Date },
			new DataGridColumn { Header = "Enabled", PropertyName = "IsEnabled", Width = 80, ColumnType = DataGridColumnType.Boolean },
			new DataGridColumn { Header = "Region", PropertyName = "Region", Width = 100 },
			new DataGridColumn { Header = "Score", PropertyName = "Score", Width = 80, Format = "N1", ColumnType = DataGridColumnType.Numeric, TextAlignment = GridTextAlignment.Right },
			new DataGridColumn { Header = "Notes", PropertyName = "Notes", Width = 200 },
		};

		LargeDataGrid.Columns = columns;
	}

	private void LoadLargeData(int count)
	{
		LargeDataStatus.Text = $"Generating {count:N0} rows...";

		var swGenerate = Stopwatch.StartNew();
		var items = new ObservableCollection<LargeDataItem>();
		var rng = new Random(42);

		for (var i = 1; i <= count; i++)
		{
			items.Add(new LargeDataItem
			{
				RowNumber = i,
				FullName = $"{FirstNames[rng.Next(FirstNames.Length)]} {LastNames[rng.Next(LastNames.Length)]}",
				Category = Categories[rng.Next(Categories.Length)],
				Value = Math.Round(rng.NextDouble() * 10000, 2),
				Amount = Math.Round((decimal)(rng.NextDouble() * 50000), 2),
				CreatedDate = DateTime.Today.AddDays(-rng.Next(0, 3650)),
				IsEnabled = rng.NextDouble() > 0.2,
				Region = Regions[rng.Next(Regions.Length)],
				Score = Math.Round(rng.NextDouble() * 100, 1),
				Notes = $"Record {i}",
			});
		}

		var generateMs = swGenerate.ElapsedMilliseconds;

		var swAssign = Stopwatch.StartNew();
		_largeDataItems = items;
		LargeDataGrid.ItemsSource = _largeDataItems;
		var assignMs = swAssign.ElapsedMilliseconds;

		LargeDataStatus.Text = $"{count:N0} rows | Generate: {generateMs}ms | Assign+Rebuild: {assignMs}ms | Total: {generateMs + assignMs}ms";
	}

	private void OnClearLargeDataClicked(object sender, RoutedEventArgs e)
	{
		_largeDataItems?.Clear();
		LargeDataGrid.ItemsSource = null;
		LargeDataStatus.Text = "Click a button to load data...";
	}

	private void OnUpdateRandomClicked(object sender, RoutedEventArgs e)
	{
		if (_largeDataItems is null || _largeDataItems.Count == 0)
			return;

		var rng = new Random();
		var count = Math.Min(100, _largeDataItems.Count);

		for (var i = 0; i < count; i++)
		{
			var idx = rng.Next(_largeDataItems.Count);
			var item = _largeDataItems[idx];
			item.Value = Math.Round(rng.NextDouble() * 10000, 2);
			item.Amount = Math.Round((decimal)(rng.NextDouble() * 50000), 2);
			item.Score = Math.Round(rng.NextDouble() * 100, 1);
		}

		LargeDataStatus.Text = $"Updated {count} random rows";
	}

	private void OnToggleLargeThemeClicked(object sender, RoutedEventArgs e)
	{
		LargeDataGrid.Theme = LargeDataGrid.Theme switch
		{
			DataGridThemeMode.Light => DataGridThemeMode.Dark,
			DataGridThemeMode.Dark => DataGridThemeMode.HighContrast,
			_ => DataGridThemeMode.Light,
		};
		LargeThemeBtn.Content = $"Theme: {LargeDataGrid.Theme}";
	}

	// ═══════════════════════════════════════════════════════
	//  TAB 4: Theming
	// ═══════════════════════════════════════════════════════

	private void SetupThemingTab()
	{
		ThemeLightBtn.Click += (_, _) => ThemingDataGrid.Theme = DataGridThemeMode.Light;
		ThemeDarkBtn.Click += (_, _) => ThemingDataGrid.Theme = DataGridThemeMode.Dark;
		ThemeHCBtn.Click += (_, _) => ThemingDataGrid.Theme = DataGridThemeMode.HighContrast;
		ThemeOceanBtn.Click += (_, _) => ApplyCustomTheme(CreateOceanBlueTheme());
		ThemeForestBtn.Click += (_, _) => ApplyCustomTheme(CreateForestGreenTheme());
		ThemeSunsetBtn.Click += (_, _) => ApplyCustomTheme(CreateSunsetTheme());

		SliderR.ValueChanged += OnHeaderColorSliderChanged;
		SliderG.ValueChanged += OnHeaderColorSliderChanged;
		SliderB.ValueChanged += OnHeaderColorSliderChanged;

		var columns = new ObservableCollection<DataGridColumn>
		{
			new DataGridColumn { Header = "SKU", PropertyName = "Sku", Width = 80, IsReadOnly = true },
			new DataGridColumn { Header = "Product", PropertyName = "ProductName", Width = 160 },
			new DataGridColumn { Header = "Category", PropertyName = "Category", Width = 120 },
			new DataGridColumn { Header = "Price", PropertyName = "Price", Width = 100, Format = "C2", ColumnType = DataGridColumnType.Numeric, TextAlignment = GridTextAlignment.Right },
			new DataGridColumn { Header = "Stock", PropertyName = "Stock", Width = 80, ColumnType = DataGridColumnType.Numeric, TextAlignment = GridTextAlignment.Right },
			new DataGridColumn { Header = "In Stock", PropertyName = "InStock", Width = 80, ColumnType = DataGridColumnType.Boolean },
			new DataGridColumn { Header = "Rating", PropertyName = "Rating", Width = 80, Format = "N1", ColumnType = DataGridColumnType.Numeric, TextAlignment = GridTextAlignment.Right },
			new DataGridColumn { Header = "Supplier", PropertyName = "Supplier", Width = 140 },
		};

		ThemingDataGrid.Columns = columns;

		var products = new ObservableCollection<ThemingProduct>();
		var rng = new Random(99);
		for (var i = 1; i <= 50; i++)
		{
			products.Add(new ThemingProduct
			{
				Sku = $"SKU-{i:D4}",
				ProductName = ProductNames[rng.Next(ProductNames.Length)],
				Category = Categories[rng.Next(Categories.Length)],
				Price = Math.Round((decimal)(rng.NextDouble() * 500 + 5), 2),
				Stock = rng.Next(0, 500),
				InStock = rng.NextDouble() > 0.2,
				Rating = Math.Round(rng.NextDouble() * 4 + 1, 1),
				Supplier = Suppliers[rng.Next(Suppliers.Length)],
			});
		}

		ThemingDataGrid.ItemsSource = products;
	}

	private void ApplyCustomTheme(DataGridStyle style)
	{
		ThemingDataGrid.GridStyle = style;
	}

	private void OnHeaderColorSliderChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
	{
		var r = (byte)SliderR.Value;
		var g = (byte)SliderG.Value;
		var b = (byte)SliderB.Value;

		var style = ThemingDataGrid.GridStyle;
		style.HeaderBackgroundColor = new GridColor(r, g, b);

		// Auto-contrast text color based on luminance
		var luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255.0;
		style.HeaderTextColor = luminance > 0.5 ? new GridColor(0, 0, 0) : new GridColor(255, 255, 255);

		ThemingDataGrid.GridStyle = style;
	}

	private static DataGridStyle CreateOceanBlueTheme()
	{
		var s = new DataGridStyle();
		s.HeaderBackgroundColor = new GridColor(25, 84, 123);
		s.HeaderTextColor = new GridColor(255, 255, 255);
		s.BackgroundColor = new GridColor(235, 245, 255);
		s.CellTextColor = new GridColor(20, 40, 60);
		s.AlternateRowColor = new GridColor(215, 235, 255);
		s.GridLineColor = new GridColor(180, 210, 240);
		s.SelectionColor = new GridColor(100, 160, 220, 100);
		s.SortIndicatorColor = new GridColor(200, 220, 255);
		return s;
	}

	private static DataGridStyle CreateForestGreenTheme()
	{
		var s = new DataGridStyle();
		s.HeaderBackgroundColor = new GridColor(34, 87, 46);
		s.HeaderTextColor = new GridColor(255, 255, 255);
		s.BackgroundColor = new GridColor(235, 250, 235);
		s.CellTextColor = new GridColor(20, 50, 20);
		s.AlternateRowColor = new GridColor(210, 240, 210);
		s.GridLineColor = new GridColor(180, 220, 180);
		s.SelectionColor = new GridColor(76, 175, 80, 100);
		s.SortIndicatorColor = new GridColor(200, 240, 200);
		return s;
	}

	private static DataGridStyle CreateSunsetTheme()
	{
		var s = new DataGridStyle();
		s.HeaderBackgroundColor = new GridColor(180, 60, 30);
		s.HeaderTextColor = new GridColor(255, 255, 255);
		s.BackgroundColor = new GridColor(255, 245, 235);
		s.CellTextColor = new GridColor(60, 20, 10);
		s.AlternateRowColor = new GridColor(255, 230, 210);
		s.GridLineColor = new GridColor(240, 200, 180);
		s.SelectionColor = new GridColor(255, 120, 60, 100);
		s.SortIndicatorColor = new GridColor(255, 200, 150);
		return s;
	}
}

// ═══════════════════════════════════════════════════════
//  Data Models
// ═══════════════════════════════════════════════════════

public class Employee : INotifyPropertyChanged
{
	private int _id;
	private string _name = string.Empty;
	private string _department = string.Empty;
	private decimal _salary;
	private DateTime _hireDate;
	private bool _isActive;
	private string _city = string.Empty;
	private string _level = string.Empty;
	private double _performance;
	private int _rating;

	public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
	public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
	public string Department { get => _department; set { _department = value; OnPropertyChanged(); } }
	public decimal Salary { get => _salary; set { _salary = value; OnPropertyChanged(); } }
	public DateTime HireDate { get => _hireDate; set { _hireDate = value; OnPropertyChanged(); } }
	public bool IsActive { get => _isActive; set { _isActive = value; OnPropertyChanged(); } }
	public string City { get => _city; set { _city = value; OnPropertyChanged(); } }
	public string Level { get => _level; set { _level = value; OnPropertyChanged(); } }
	public double Performance { get => _performance; set { _performance = value; OnPropertyChanged(); } }
	public int Rating { get => _rating; set { _rating = value; OnPropertyChanged(); } }

	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class GroupingEmployee : INotifyPropertyChanged
{
	private int _id;
	private string _name = string.Empty;
	private string _department = string.Empty;
	private decimal _salary;
	private string _level = string.Empty;
	private bool _isActive;
	private string _city = string.Empty;

	public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
	public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
	public string Department { get => _department; set { _department = value; OnPropertyChanged(); } }
	public decimal Salary { get => _salary; set { _salary = value; OnPropertyChanged(); } }
	public string Level { get => _level; set { _level = value; OnPropertyChanged(); } }
	public bool IsActive { get => _isActive; set { _isActive = value; OnPropertyChanged(); } }
	public string City { get => _city; set { _city = value; OnPropertyChanged(); } }

	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class LargeDataItem : INotifyPropertyChanged
{
	private int _rowNumber;
	private string _fullName = string.Empty;
	private string _category = string.Empty;
	private double _value;
	private decimal _amount;
	private DateTime _createdDate;
	private bool _isEnabled;
	private string _region = string.Empty;
	private double _score;
	private string _notes = string.Empty;

	public int RowNumber { get => _rowNumber; set { _rowNumber = value; OnPropertyChanged(); } }
	public string FullName { get => _fullName; set { _fullName = value; OnPropertyChanged(); } }
	public string Category { get => _category; set { _category = value; OnPropertyChanged(); } }
	public double Value { get => _value; set { _value = value; OnPropertyChanged(); } }
	public decimal Amount { get => _amount; set { _amount = value; OnPropertyChanged(); } }
	public DateTime CreatedDate { get => _createdDate; set { _createdDate = value; OnPropertyChanged(); } }
	public bool IsEnabled { get => _isEnabled; set { _isEnabled = value; OnPropertyChanged(); } }
	public string Region { get => _region; set { _region = value; OnPropertyChanged(); } }
	public double Score { get => _score; set { _score = value; OnPropertyChanged(); } }
	public string Notes { get => _notes; set { _notes = value; OnPropertyChanged(); } }

	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class ThemingProduct : INotifyPropertyChanged
{
	private string _sku = string.Empty;
	private string _productName = string.Empty;
	private string _category = string.Empty;
	private decimal _price;
	private int _stock;
	private bool _inStock;
	private double _rating;
	private string _supplier = string.Empty;

	public string Sku { get => _sku; set { _sku = value; OnPropertyChanged(); } }
	public string ProductName { get => _productName; set { _productName = value; OnPropertyChanged(); } }
	public string Category { get => _category; set { _category = value; OnPropertyChanged(); } }
	public decimal Price { get => _price; set { _price = value; OnPropertyChanged(); } }
	public int Stock { get => _stock; set { _stock = value; OnPropertyChanged(); } }
	public bool InStock { get => _inStock; set { _inStock = value; OnPropertyChanged(); } }
	public double Rating { get => _rating; set { _rating = value; OnPropertyChanged(); } }
	public string Supplier { get => _supplier; set { _supplier = value; OnPropertyChanged(); } }

	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
