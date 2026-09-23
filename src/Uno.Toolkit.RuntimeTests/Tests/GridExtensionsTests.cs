using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Windows.UI;

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal partial class GridExtensionsTests;

partial class GridExtensionsTests
{
	// Auto toggling

	[TestMethod]
	public async Task When_Auto_False_Positions_Are_Not_Modified()
	{
		var sut = CreateGrid(rows: 2, cols: 3, childCount: 4);
		GridExtensions.SetAuto(sut, false);

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		// All children should stay at default (0, 0)
		foreach (var child in sut.Children)
			Assert.AreEqual((0, 0), GetPosition(child));
	}

	[TestMethod]
	public async Task When_Auto_Enabled_Then_Disabled_Positions_Are_Not_Reset()
	{
		var sut = CreateGrid(rows: 2, cols: 3, childCount: 4);
		GridExtensions.SetAuto(sut, true);

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		// Positions should be assigned while Auto is true
		Assert.AreEqual((0, 1), GetPosition(sut.Children[1]));

		// Disabling Auto should not reset positions
		GridExtensions.SetAuto(sut, false);
		await UnitTestUIContentHelperEx.WaitForIdle();

		Assert.AreEqual((0, 1), GetPosition(sut.Children[1]));

		// ...and should stop placing new children, proving the subscription was torn down
		sut.Children.Add(CreateChild(4));
		await UnitTestUIContentHelperEx.WaitForIdle();

		Assert.AreEqual((0, 0), GetPosition(sut.Children[4]), "expecting no placement once Auto is disabled");
	}

	[TestMethod]
	public async Task When_Assigns_Correct_Positions()
	{
		// 2 rows x 3 cols, 6 children
		var sut = CreateGrid(rows: 2, cols: 3, childCount: 6);
		GridExtensions.SetAuto(sut, true);

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		// Row-major order: (row = i/cols, col = i%cols)
		Assert.AreEqual((0, 0), GetPosition(sut.Children[0]));
		Assert.AreEqual((0, 1), GetPosition(sut.Children[1]));
		Assert.AreEqual((0, 2), GetPosition(sut.Children[2]));
		Assert.AreEqual((1, 0), GetPosition(sut.Children[3]));
		Assert.AreEqual((1, 1), GetPosition(sut.Children[4]));
		Assert.AreEqual((1, 2), GetPosition(sut.Children[5]));
	}

	[TestMethod]
	public async Task When_Overflow_Wraps()
	{
		// 2 rows x 2 cols, 5 children — 5th wraps back to (0,0)
		var sut = CreateGrid(rows: 2, cols: 2, childCount: 5);
		GridExtensions.SetAuto(sut, true);

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		Assert.AreEqual((0, 0), GetPosition(sut.Children[4]));
	}

	// Dynamic children

	[TestMethod]
	public async Task When_Child_Added_Positions_Are_Updated()
	{
		var sut = CreateGrid(rows: 2, cols: 2, childCount: 2);
		GridExtensions.SetAuto(sut, true);

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		sut.Children.Add(CreateChild(2));
		await UnitTestUIContentHelperEx.WaitForIdle();

		// 3rd child: i=2, row=2/2=1, col=2%2=0
		Assert.AreEqual((1, 0), GetPosition(sut.Children[2]));
	}

	[TestMethod]
	public async Task When_Definitions_Changed_Positions_Are_Updated()
	{
		var sut = CreateGrid(rows: 2, cols: 3, childCount: 4);
		GridExtensions.SetAuto(sut, true);

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		Assert.AreEqual((1, 0), GetPosition(sut.Children[3]));

		// narrowing to 2 columns re-flows the 4th child: i=3, row=3/2%2=1, col=3%2=1
		sut.ColumnDefinitions.RemoveAt(2);
		await UnitTestUIContentHelperEx.WaitForIdle();

		Assert.AreEqual((1, 1), GetPosition(sut.Children[3]));
	}

	[TestMethod]
	public async Task When_Detached_And_Reattached_Positions_Are_Still_Updated()
	{
		var sut = CreateGrid(rows: 2, cols: 2, childCount: 2);
		var host = new Border { Child = sut };
		GridExtensions.SetAuto(sut, true);

		await UnitTestUIContentHelperEx.SetContentAndWait(host);

		// The LayoutUpdated subscription is scoped to Loaded/Unloaded, so a round-trip
		// through the tree must re-subscribe. (A "placement stops while detached" assert
		// would prove nothing: an unrooted element gets no layout pass either way.)
		host.Child = null;
		await UnitTestUIContentHelperEx.WaitForIdle();

		host.Child = sut;
		await UnitTestUIContentHelperEx.WaitForIdle();

		sut.Children.Add(CreateChild(2));
		await UnitTestUIContentHelperEx.WaitForIdle();

		// 3rd child: i=2, row=2/2=1, col=2%2=0
		Assert.AreEqual((1, 0), GetPosition(sut.Children[2]), "expecting placement to resume after the grid is re-attached");
	}

	[TestMethod]
	public async Task When_Child_Removed_Positions_Are_Updated()
	{
		var sut = CreateGrid(rows: 2, cols: 2, childCount: 4);
		GridExtensions.SetAuto(sut, true);

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		// Initially: child[2] → (1, 0), child[3] → (1, 1)
		Assert.AreEqual((1, 0), GetPosition(sut.Children[2]));

		sut.Children.RemoveAt(0);
		await UnitTestUIContentHelperEx.WaitForIdle();

		// After removal: what was child[2] is now child[1] → (0, 1)
		Assert.AreEqual((0, 1), GetPosition(sut.Children[1]));
	}

	// Layout outcome

	[TestMethod]
	public async Task When_Auto_Children_Are_Laid_Out_In_Their_Cells()
	{
		// 2 rows x 2 cols of equal stars over a 200x100 grid => 100x50 cells
		var sut = CreateGrid(rows: 2, cols: 2, childCount: 4, cellLength: new GridLength(1, GridUnitType.Star));
		sut.Width = 200;
		sut.Height = 100;

		// let the children fill their cell, so that their offset is the cell origin
		for (var i = 0; i < sut.Children.Count; i++)
		{
			var child = (FrameworkElement)sut.Children[i];
			child.Width = double.NaN;
			child.Height = double.NaN;
		}

		GridExtensions.SetAuto(sut, true);

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		// beyond the attached-property values, the children should actually land in those cells
		AssertOffset(sut.Children[0], 0, 0);
		AssertOffset(sut.Children[1], 100, 0);
		AssertOffset(sut.Children[2], 0, 50);
		AssertOffset(sut.Children[3], 100, 50);

		void AssertOffset(UIElement child, double x, double y)
		{
			var offset = child.TransformToVisual(sut).TransformPoint(default);

			Assert.AreEqual(x, offset.X, 1, $"unexpected x-offset for child at {GetPosition(child)}");
			Assert.AreEqual(y, offset.Y, 1, $"unexpected y-offset for child at {GetPosition(child)}");
		}
	}

	// Documented limitations

	[TestMethod]
	public async Task When_Child_Has_Preset_Position_It_Is_Overwritten()
	{
		var sut = CreateGrid(rows: 2, cols: 3, childCount: 4);
		Grid.SetRow(sut.Children[1], 1);
		Grid.SetColumn(sut.Children[1], 2);

		GridExtensions.SetAuto(sut, true);

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		// by design: author-set positions are not preserved while Auto is enabled
		Assert.AreEqual((0, 1), GetPosition(sut.Children[1]));
	}

	[TestMethod]
	public async Task When_Child_Has_Span_It_Is_Ignored()
	{
		var sut = CreateGrid(rows: 2, cols: 2, childCount: 3);
		Grid.SetColumnSpan(sut.Children[0], 2);

		GridExtensions.SetAuto(sut, true);

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		// by design: spans are not accounted for and children are placed as 1x1, so the
		// 2nd child lands in the cell already covered by the 1st child's span.
		Assert.AreEqual((0, 0), GetPosition(sut.Children[0]));
		Assert.AreEqual((0, 1), GetPosition(sut.Children[1]));
		Assert.AreEqual((1, 0), GetPosition(sut.Children[2]));

		Assert.AreEqual(2, Grid.GetColumnSpan(sut.Children[0]), "the span itself should be left untouched");
	}

	// Edge cases

	[TestMethod]
	public async Task When_No_Definitions_All_Children_At_Origin()
	{
		var sut = CreateGrid(0, 0, 2);
		GridExtensions.SetAuto(sut, true);

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		foreach (var child in sut.Children)
			Assert.AreEqual((0, 0), GetPosition(child));
	}

	[TestMethod]
	public async Task When_Only_Columns_Defined_Children_Fill_Single_Row()
	{
		var sut = CreateGrid(rows: 0, cols: 3, childCount: 3);
		GridExtensions.SetAuto(sut, true);

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		Assert.AreEqual((0, 0), GetPosition(sut.Children[0]));
		Assert.AreEqual((0, 1), GetPosition(sut.Children[1]));
		Assert.AreEqual((0, 2), GetPosition(sut.Children[2]));
	}

	[TestMethod]
	public async Task When_Only_Rows_Defined_Children_Fill_Single_Column()
	{
		var sut = CreateGrid(rows: 3, cols: 0, childCount: 3);
		GridExtensions.SetAuto(sut, true);

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		Assert.AreEqual((0, 0), GetPosition(sut.Children[0]));
		Assert.AreEqual((1, 0), GetPosition(sut.Children[1]));
		Assert.AreEqual((2, 0), GetPosition(sut.Children[2]));
	}
}
partial class GridExtensionsTests // helpers methods
{
	private static readonly Color[] UnoColors =
	[
		Color.FromArgb(0xFF, 0x22, 0x9D, 0xFC), // #FF229DFC UnoBlue
		Color.FromArgb(0xFF, 0x7A, 0x69, 0xF5), // #FF7A69F5 UnoPurple
		Color.FromArgb(0xFF, 0x6C, 0xE5, 0xAE), // #FF6CE5AE UnoGreen
		Color.FromArgb(0xFF, 0xF6, 0x56, 0x78), // #FFF65678 UnoRed
	];

	private static Grid CreateGrid(int rows, int cols, int childCount, GridLength? cellLength = null)
	{
		var length = cellLength ?? GridLength.Auto;
		var grid = new Grid();

		for (var r = 0; r < rows; r++)
			grid.RowDefinitions.Add(new RowDefinition { Height = length });
		for (var c = 0; c < cols; c++)
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = length });

		for (var i = 0; i < childCount; i++)
			grid.Children.Add(CreateChild(i));

		return grid;
	}

	private static Border CreateChild(int index) => new()
	{
		Width = 10,
		Height = 10,
		Background = new SolidColorBrush(UnoColors[index % UnoColors.Length]),
	};

	private static (int row, int col) GetPosition(UIElement child) => (Grid.GetRow(child), Grid.GetColumn(child));
}
