using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions;
using Uno.Toolkit.RuntimeTests.Extensions;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using ChipControl = Uno.Toolkit.UI.Chip; // ios/macos: to avoid collision with `global::Chip` namespace...
using ItemsRepeater = Microsoft.UI.Xaml.Controls.ItemsRepeater;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal class ItemsRepeaterChipTests
{
	// note: the default state of Chip.IsChecked (inherited from ToggleButton) is false, since we don't use IsThreeState

	#region Selection via Toggle

	[TestMethod]
	[DataRow(ItemsSelectionMode.None, new[] { 1 }, null)]
	[DataRow(ItemsSelectionMode.SingleOrNone, new[] { 1 }, 1)]
	[DataRow(ItemsSelectionMode.SingleOrNone, new[] { 1, 1 }, null)] // deselection
	[DataRow(ItemsSelectionMode.SingleOrNone, new[] { 1, 2 }, 2)] // reselection
	[DataRow(ItemsSelectionMode.Single, new int[0], 0)] // selection enforced by 'Single'
	[DataRow(ItemsSelectionMode.Single, new[] { 1 }, 1)]
	[DataRow(ItemsSelectionMode.Single, new[] { 1, 1 }, 1)] // deselection denied
	[DataRow(ItemsSelectionMode.Single, new[] { 1, 2 }, 2)] // reselection
	[DataRow(ItemsSelectionMode.Multiple, new[] { 1 }, new object[] { 1 })]
	[DataRow(ItemsSelectionMode.Multiple, new[] { 1, 2 }, new object[] { 1, 2 })] // multi-select@1,2
	[DataRow(ItemsSelectionMode.Multiple, new[] { 1, 2, 2 }, new object[] { 1 })] // multi-select@1,2, deselection@2
	public async Task VariousMode_TapSelection(ItemsSelectionMode mode, int[] selectionSequence, object expectation)
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = SetupItemsRepeater(source, mode);
		bool?[] expected, actual;

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		expected = mode is ItemsSelectionMode.Single
			? new bool?[] { true, false, false }
			: new bool?[] { false, false, false };
		actual = GetChipsSelectionState(SUT);
		CollectionAssert.AreEqual(expected, actual);

		foreach (var i in selectionSequence)
		{
			FakeTapItemAt(SUT, i);
		}
		expected = (expectation switch
		{
			object[] array => source.Select(x => array.Contains(x)),
			int i => source.Select(x => x == i),
			null => Enumerable.Repeat(false, 3),

			_ => throw new ArgumentOutOfRangeException(nameof(expectation)),
		}).Cast<bool?>().ToArray();
		actual = GetChipsSelectionState(SUT);
		CollectionAssert.AreEqual(expected, actual);
	}

	[TestMethod]
	public async Task SingleMode_Selection()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = SetupItemsRepeater(source, ItemsSelectionMode.SingleOrNone);
		bool?[] expected = new bool?[] { false, true, false }, actual;

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		actual = GetChipsSelectionState(SUT);
		Assert.IsTrue(actual.All(x => x == false));

		FakeTapItemAt(SUT, 1);
		actual = GetChipsSelectionState(SUT);
		CollectionAssert.AreEqual(expected, actual);
	}

	#endregion

	#region Changing SelectionMode

	[TestMethod]
	public async Task SingleOrNoneToSingle_NoneSelected_ShouldAutoSelect()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = SetupItemsRepeater(source, ItemsSelectionMode.SingleOrNone);
		bool?[] expected = new bool?[] { true, false, false }, actual;

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		actual = GetChipsSelectionState(SUT);
		Assert.IsTrue(actual.All(x => x == false));

		ItemsRepeaterExtensions.SetSelectionMode(SUT, ItemsSelectionMode.Single);
		actual = GetChipsSelectionState(SUT);
		CollectionAssert.AreEqual(expected, actual);
	}

	[TestMethod]
	public async Task SingleOrNoneToSingle_Selected_ShouldPreserveSelection()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = SetupItemsRepeater(source, ItemsSelectionMode.SingleOrNone);
		bool?[] expected = new bool?[] { false, false, true }, actual;

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		FakeTapItemAt(SUT, 2);
		actual = GetChipsSelectionState(SUT);
		CollectionAssert.AreEqual(expected, actual);

		ItemsRepeaterExtensions.SetSelectionMode(SUT, ItemsSelectionMode.Single);
		actual = GetChipsSelectionState(SUT);
		CollectionAssert.AreEqual(expected, actual);
	}

	[TestMethod]
	public async Task MultiToSingle_Selected_ShouldPreserveFirstSelection()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = SetupItemsRepeater(source, ItemsSelectionMode.Multiple);
		bool?[] expected = new bool?[] { false, true, true }, actual;

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		FakeTapItemAt(SUT, 1);
		FakeTapItemAt(SUT, 2);
		actual = GetChipsSelectionState(SUT);
		CollectionAssert.AreEqual(expected, actual);

		ItemsRepeaterExtensions.SetSelectionMode(SUT, ItemsSelectionMode.Single);
		expected = new bool?[] { false, true, false };
		actual = GetChipsSelectionState(SUT);
		CollectionAssert.AreEqual(expected, actual);
	}

	#endregion

	private static ItemsRepeater SetupItemsRepeater(object source, ItemsSelectionMode mode)
	{
		var SUT = new ItemsRepeater
		{
			ItemsSource = source,
			ItemTemplate = XamlHelper.LoadXaml<DataTemplate>("""
				<DataTemplate>
					<utu:Chip />
				</DataTemplate>
				"""),
		};
		ItemsRepeaterExtensions.SetSelectionMode(SUT, mode);

		return SUT;
	}

	private static bool? IsChipSelectedAt(ItemsRepeater ir, int index)
	{
		return (ir.TryGetElement(index) as ChipControl)?.IsChecked;
	}

	// since we are not using IsThreeState=True, the values can only be true/false.
	// if any of them is null, that means there is another problem and should be thrown.
	// therefore, only == check should be used in an assertion.
	private static bool?[] GetChipsSelectionState(ItemsRepeater ir)
	{
		return (ir.ItemsSource as IEnumerable)?.Cast<object>()
			.Select((_, i) => (ir.TryGetElement(i) as ChipControl)?.IsChecked)
			.ToArray() ?? new bool?[0];
	}

	private static void FakeTapItemAt(ItemsRepeater ir, int index)
	{
		if (ir.TryGetElement(index) is { } element)
		{
			// Fake local tap handler on ToggleButton level.
			// For SelectorItem, nothing will happen on tap unless nested under a Selector, which isnt the case here.
			(element as ToggleButton)?.Toggle();

			// This is whats called in ItemsRepeater::Tapped handler.
			// Note that the handler will not trigger from a "fake tap" like the line above, so we have to manually invoke here.
			ItemsRepeaterExtensions.ToggleItemSelectionAtCoerced(ir, index);
		}
		else
		{
			throw new InvalidOperationException($"Element at index={index} is not yet materialized or out of range.");
		}
	}
}
