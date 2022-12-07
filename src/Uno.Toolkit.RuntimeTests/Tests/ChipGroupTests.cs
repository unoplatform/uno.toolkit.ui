using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Extensions;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using ChipControl = Uno.Toolkit.UI.Chip; // ios/macos: to avoid collision with `global::Chip` namespace...

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal class ChipGroupTests
{
	/*	Test Plan
	 *		- tap triggered selection in various SelectionMode
	 *		- SelectedItem(s) in various SelectionMode
	 *		- pre-assertions & post-assertions when changing SelectionMode
	 */

	#region Selection via Toggle

	[TestMethod]
	[DataRow(ChipSelectionMode.None, new[] { 1 }, null)]
	[DataRow(ChipSelectionMode.SingleOrNone, new[] { 1 }, 1)]
	[DataRow(ChipSelectionMode.SingleOrNone, new[] { 1, 1 }, null)] // deselection
	[DataRow(ChipSelectionMode.SingleOrNone, new[] { 1, 2 }, 2)] // reselection
	[DataRow(ChipSelectionMode.Single, new int[0], 0)] // selection enforced by 'Single'
	[DataRow(ChipSelectionMode.Single, new[] { 1 }, 1)]
	[DataRow(ChipSelectionMode.Single, new[] { 1, 1 }, 1)] // deselection denied
	[DataRow(ChipSelectionMode.Single, new[] { 1, 2 }, 2)] // reselection
	[DataRow(ChipSelectionMode.Multiple, new[] { 1 }, new object[] { 1 })]
	[DataRow(ChipSelectionMode.Multiple, new[] { 1, 2 }, new object[] { 1, 2 })] // multi-select@1,2
	[DataRow(ChipSelectionMode.Multiple, new[] { 1, 2, 2 }, new object[] { 1 })] // multi-select@1,2, deselection@2
	public async Task VariousMode_TapSelection(ChipSelectionMode mode, int[] selectionSequence, object expectation)
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = new ChipGroup
		{
			SelectionMode = mode,
			ItemsSource = source,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		Assert.AreEqual(mode is ChipSelectionMode.Single ? source[0] : null, SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		foreach (var i in selectionSequence)
		{
			((ChipControl)SUT.ContainerFromIndex(i)).Toggle();
		}
		if (mode is ChipSelectionMode.SingleOrNone or ChipSelectionMode.Single)
		{
			Assert.AreEqual((int?)expectation, SUT.SelectedItem);
			Assert.IsNull(SUT.SelectedItems);
		}
		else
		{
			Assert.IsNull(SUT.SelectedItem);
			CollectionAssert.AreEqual((object[]?)expectation, SUT.SelectedItems);
		}
	}

	[TestMethod]
	public async Task SingleMode_Selection()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.SingleOrNone,
			ItemsSource = source,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		Assert.IsNull(SUT.SelectedItem);

		((ChipControl)SUT.ContainerFromIndex(1)).Toggle();
		Assert.AreEqual(source[1], SUT.SelectedItem);
	}

	#endregion

	#region Selection via SelectedItem & SelectItems

	[TestMethod]
	public async Task None_SetSelection()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.None,
			ItemsSource = source,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		Assert.IsNull(SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		// invalid assignment for current selection mode will reset the selection (clear, and then coerced)
		SUT.SelectedItem = source[1];
		Assert.IsNull(SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		// invalid assignment for current selection mode will reset the selection (clear, and then coerced)
		SUT.SelectedItems = source.Take(2).ToArray();
		Assert.IsNull(SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);
	}

	[TestMethod]
	public async Task Single_SetSelection()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.Single,
			ItemsSource = source,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		Assert.AreEqual(source[0], SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		SUT.SelectedItem = source[1];
		Assert.AreEqual(source[1], SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		// invalid assignment for current selection mode will reset the selection (clear, and then coerced)
		SUT.SelectedItems = source.Skip(1).Take(2).ToArray();
		Assert.AreEqual(source[0], SUT.SelectedItem); // coerced from Single
		Assert.IsNull(SUT.SelectedItems);
	}

	[TestMethod]
	public async Task SingleOrNone_SetSelection()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.SingleOrNone,
			ItemsSource = source,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		Assert.IsNull(SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		SUT.SelectedItem = source[1];
		Assert.AreEqual(source[1], SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		// invalid assignment for current selection mode will reset the selection (clear, and then coerced)
		SUT.SelectedItems = source.Take(2).ToArray();
		Assert.IsNull(SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);
	}

	[TestMethod]
	public async Task Multiple_SetSelection()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.Multiple,
			ItemsSource = source,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		Assert.IsNull(SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		// invalid assignment for current selection mode will reset the selection (clear, and then coerced)
		SUT.SelectedItem = source[1];
		Assert.IsNull(SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		var newSelection = source.Skip(1).Take(2).ToArray();
		SUT.SelectedItems = newSelection;
		Assert.IsNull(SUT.SelectedItem);
		CollectionAssert.AreEqual(newSelection, SUT.SelectedItems);
	}

	[TestMethod]
	public async Task Multiple_ReassignSelectedItems_ShouldNotStack()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.Multiple,
			ItemsSource = source,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		Assert.IsNull(SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		// Changing SelectedItems should not create union of old & new values
		foreach (var selection in Enumerable.Range(0, 1).Select(x => source.Skip(x).Take(2).ToArray()))
		{
			SUT.SelectedItems = selection;
			Assert.IsNull(SUT.SelectedItem);
			CollectionAssert.AreEqual(selection, SUT.SelectedItems);
		}
	}
	#endregion

	#region Changing SelectionMode

	[TestMethod]
	public async Task SingleOrNoneToSingle_NoneSelected_ShouldAutoSelect()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.SingleOrNone,
			ItemsSource = source,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		Assert.IsNull(SUT.SelectedItem);

		SUT.SelectionMode = ChipSelectionMode.Single;
		Assert.AreEqual(source[0], SUT.SelectedItem);
	}

	[TestMethod]
	public async Task SingleOrNoneToSingle_Selected_ShouldPreserveSelection()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var selected = source.Last();
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.SingleOrNone,
			ItemsSource = source,
			SelectedItem = selected,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		Assert.AreEqual(selected, SUT.SelectedItem);

		SUT.SelectionMode = ChipSelectionMode.Single;
		Assert.AreEqual(selected, SUT.SelectedItem);
	}

	[TestMethod]
	public async Task MultiToSingle_Selected_ShouldPreserveFirstSelection()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var selected = source.Skip(1).Take(2).ToArray();
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.Multiple,
			ItemsSource = source,
			SelectedItems = selected,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		CollectionAssert.AreEqual(selected, SUT.SelectedItems);

		SUT.SelectionMode = ChipSelectionMode.Single;
		Assert.AreEqual(selected.First(), SUT.SelectedItem);
	}

	#endregion
}
