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

		sut.Children.Add(new Border());
		await UnitTestUIContentHelperEx.WaitForIdle();

		// 3rd child: i=2, row=2/2=1, col=2%2=0
		Assert.AreEqual((1, 0), GetPosition(sut.Children[2]));
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

	private static Grid CreateGrid(int rows, int cols, int childCount)
	{
		var grid = new Grid();

		for (var r = 0; r < rows; r++)
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		for (var c = 0; c < cols; c++)
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

		for (var i = 0; i < childCount; i++)
			grid.Children.Add(new Border
			{
				Width = 10,
				Height = 10,
				Background = new SolidColorBrush(UnoColors[i % UnoColors.Length]),
			});

		return grid;
	}

	private static (int row, int col) GetPosition(UIElement child) => (Grid.GetRow(child), Grid.GetColumn(child));
}
