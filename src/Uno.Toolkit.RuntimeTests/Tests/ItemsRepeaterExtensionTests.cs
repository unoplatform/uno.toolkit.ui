using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using ItemsRepeater = Microsoft.UI.Xaml.Controls.ItemsRepeater;
using static Uno.Toolkit.RuntimeTests.Tests.ItemsRepeaterChipTests; // to borrow helper methods

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal class ItemsRepeaterExtensionTests
{
	[TestMethod]
	[DataRow(nameof(ItemsRepeaterExtensions.SelectedItemProperty))]
	[DataRow(nameof(ItemsRepeaterExtensions.SelectedItemsProperty))]
	[DataRow(nameof(ItemsRepeaterExtensions.SelectedIndexProperty))]
	[DataRow(nameof(ItemsRepeaterExtensions.SelectedIndexesProperty))]
	public async Task When_Selection_Property_Changed(string property)
	{
		var source = Enumerable.Range(0, 3).ToList();
		var SUT = SetupItemsRepeater(source, ItemsSelectionMode.SingleOrNone);
		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		(property switch
		{
			nameof(ItemsRepeaterExtensions.SelectedItemProperty) => () => ItemsRepeaterExtensions.SetSelectedItem(SUT, source.ElementAt(1)),
			nameof(ItemsRepeaterExtensions.SelectedItemsProperty) => () => ItemsRepeaterExtensions.SetSelectedItems(SUT, new object[] { 1 }),
			nameof(ItemsRepeaterExtensions.SelectedIndexProperty) => () => ItemsRepeaterExtensions.SetSelectedIndex(SUT, 1),
			nameof(ItemsRepeaterExtensions.SelectedIndexesProperty) => () => ItemsRepeaterExtensions.SetSelectedIndexes(SUT, new int[] { 1 }),

			_ => default(Action) ?? throw new ArgumentOutOfRangeException(property),
		})();
		Assert.AreEqual(true, IsChipSelectedAt(SUT, 1));
	}
}
