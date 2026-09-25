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
using static Uno.Toolkit.UI.ItemsSelectionMode;
using Uno.Extensions;
using Uno.Toolkit.RuntimeTests.Extensions;

#if IS_WINUI
using Microsoft.UI.Xaml.Data;
#else
using Windows.UI.Xaml.Data;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	public partial class ItemsRepeaterExtensionsTests
	{
		private const ItemsSelectionMode Single = ItemsSelectionMode.Single;

		[TestMethod]
		[DataRow(SingleOrNone, null, false, null, DisplayName = $"Select: {nameof(SingleOrNone)} none")]
		[DataRow(SingleOrNone, new[] { 0 }, false, new[] { 0 }, DisplayName = $"Select: {nameof(SingleOrNone)} 0")]
		[DataRow(Multiple, new[] { 0, 2 }, false, new[] { 0, 2 }, DisplayName = $"Select: {nameof(Multiple)} (0, 2)")]
		[DataRow(SingleOrNone, null, true, new[] { 0 }, DisplayName = $"Deselect: {nameof(SingleOrNone)} none")]
		[DataRow(SingleOrNone, new[] { 0 }, true, null, DisplayName = $"Deselect: {nameof(SingleOrNone)} 0")]
		[DataRow(Multiple, new[] { 0, 2 }, true, new[] { 1 }, DisplayName = $"Deselect: {nameof(Multiple)} (0, 2)")]
		public async Task When_Tapped_With_ISelectionInfo(ItemsSelectionMode mode, int[]? tapSequence, bool isDeselecting, int[]? expected)
		{
			var selected = isDeselecting ? new HashSet<int>() { 0, 1, 2 } : new HashSet<int>();

			var source = SelectionSource.Create(3, isPreselected: x => isDeselecting);
			source.DeselectRangeOverride = x => DeselectRangeOverride(x, selected);
			source.SelectRangeOverride = x => SelectRangeOverride(x, selected);

			var SUT = SetupItemsRepeater(source, mode);

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			foreach (var i in tapSequence.Safe())
			{
				SUT.FakeTapItemAt(i);
			}

			Assert.IsTrue(AreEqual(expected.Safe(), selected));
		}

		[TestMethod]
		[DataRow(None, Single, null, new[] { 0 }, DisplayName = $"{nameof(None)} to {nameof(Single)} with [] selected")]
		[DataRow(None, SingleOrNone, null, null, DisplayName = $"{nameof(None)} to {nameof(SingleOrNone)} with [] selected")]
		[DataRow(None, Multiple, null, null, DisplayName = $"{nameof(None)} to {nameof(Multiple)} with [] selected")]
		[DataRow(Single, None, new[] { 1 }, null, DisplayName = $"{nameof(Single)} to {nameof(None)} with [1] selected")]
		[DataRow(Single, SingleOrNone, new[] { 1 }, new[] { 1 }, DisplayName = $"{nameof(Single)} to {nameof(SingleOrNone)} with [1] selected")]
		[DataRow(Single, Multiple, new[] { 1 }, new[] { 1 }, DisplayName = $"{nameof(Single)} to {nameof(Multiple)} with [1] selected")]
		[DataRow(SingleOrNone, None, new[] { 1 }, null, DisplayName = $"{nameof(SingleOrNone)} to {nameof(None)} with [1] selected")]
		[DataRow(SingleOrNone, Single, new[] { 1 }, new[] { 1 }, DisplayName = $"{nameof(SingleOrNone)} to {nameof(Single)} with [1] selected")]
		[DataRow(SingleOrNone, Multiple, new[] { 1 }, new[] { 1 }, DisplayName = $"{nameof(SingleOrNone)} to {nameof(Multiple)} with [1] selected")]
		[DataRow(SingleOrNone, None, null, null, DisplayName = $"{nameof(SingleOrNone)} to {nameof(None)} with [] selected")]
		[DataRow(SingleOrNone, Single, null, new[] { 0 }, DisplayName = $"{nameof(SingleOrNone)} to {nameof(Single)} with [] selected")]
		[DataRow(SingleOrNone, Multiple, null, null, DisplayName = $"{nameof(SingleOrNone)} to {nameof(Multiple)} with [] selected")]
		[DataRow(Multiple, None, new[] { 1, 2 }, null, DisplayName = $"{nameof(Multiple)} to {nameof(None)} with [1, 2] selected")]
		[DataRow(Multiple, Single, new[] { 1, 2 }, new[] { 1 }, DisplayName = $"{nameof(Multiple)} to {nameof(Single)} with [1, 2] selected")]
		[DataRow(Multiple, SingleOrNone, new[] { 1, 2 }, new[] { 1 }, DisplayName = $"{nameof(Multiple)} to {nameof(SingleOrNone)} with [1, 2] selected")]
		public async Task When_Mode_Changed_ISelectionInfo(ItemsSelectionMode originalMode, ItemsSelectionMode newMode, int[]? selectedIndexes, int[]? expectedIndexes)
		{
			var selected = new HashSet<int>(selectedIndexes.Safe());

			var source = SelectionSource.Create(3, isPreselected: selected.Contains);
			source.DeselectRangeOverride = x => DeselectRangeOverride(x, selected);
			source.SelectRangeOverride = x => SelectRangeOverride(x, selected);

			var SUT = SetupItemsRepeater(source, originalMode);

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			ItemsRepeaterExtensions.SetSelectionMode(SUT, newMode);

			Assert.IsTrue(AreEqual(expectedIndexes.Safe(), selected));
		}

		[TestMethod]
		public async Task When_Source_Changed_With_ISelectionInfo()
		{
			var evenSource = SelectionSource.Create(4, isPreselected: x => x % 2 == 0);
			var oddSource = SelectionSource.Create(4, isPreselected: x => x % 2 == 1);

			var SUT = SetupItemsRepeater(evenSource, Multiple);

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			Assert.IsTrue(ItemsRepeaterExtensions.GetSelectedIndex(SUT) == 0);
			Assert.IsTrue(AreEqual(ItemsRepeaterExtensions.GetSelectedIndexes(SUT), new[] { 0, 2 } ));

			SUT.ItemsSource = oddSource;

			Assert.IsTrue(ItemsRepeaterExtensions.GetSelectedIndex(SUT) == 1);
			Assert.IsTrue(AreEqual(ItemsRepeaterExtensions.GetSelectedIndexes(SUT), new[] { 1, 3 }));
		}

		[TestMethod]
		public async Task When_Preselected_SelectedItems_With_ISelectionInfo()
		{
			var preselectedSource = SelectionSource.Create(4, isPreselected: x => x == 2);

			var SUT = SetupItemsRepeater(preselectedSource, Multiple);

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			var selectedItems = ItemsRepeaterExtensions.GetSelectedItems(SUT).OfType<SelectionData>().Select(x => x.Value);

			Assert.IsTrue(AreEqual(selectedItems, new[] { 2 }));
		}

		// Checks equality of two lists based on values, ignoring order
		private static bool AreEqual(IEnumerable<int> expected, IEnumerable<int> actual)
		{
			return Enumerable.SequenceEqual(expected.OrderBy(x => x), actual.OrderBy(x => x));
		}

		private static void DeselectRangeOverride(ItemIndexRange range, ICollection<int> selected)
		{
			range.Expand().ForEach(idx => selected.Remove(idx));
		}

		private static void SelectRangeOverride(ItemIndexRange range, ICollection<int> selected)
		{
			selected.AddRange(range.Expand());
		}
	}
}

public class SelectionData
{
	public int Value { get; set; }
	public bool Selected { get; set; }

	public override string ToString() => Value.ToString();
}
public class SelectionSource : List<SelectionData>, ISelectionInfo
{
	public Action<ItemIndexRange> SelectRangeOverride { get; set; }
	public Action<ItemIndexRange> DeselectRangeOverride { get; set; }
	public Func<int, bool> IsSelectedOverride { get; set; }
	public Func<IReadOnlyList<ItemIndexRange>> GetSelectedRangesOverride { get; set; }

	public SelectionSource(IEnumerable<SelectionData> source) : base(source)
	{
		this.SelectRangeOverride = SelectRangeImpl;
		this.DeselectRangeOverride = DeselectRangeImpl;
		this.IsSelectedOverride = IsSelectedImpl;
		this.GetSelectedRangesOverride = GetSelectedRangesImpl;
	}
	public static SelectionSource Create(int count, Func<int, bool> isPreselected) => Create(Enumerable.Range(0, count), isPreselected);
	public static SelectionSource Create(IEnumerable<int> source, Func<int, bool> isPreselected)
	{
		return new(source.Select(x => new SelectionData
		{
			Value = x,
			Selected = isPreselected(x),
		}));
	}

	// ISelectionInfo
	public void SelectRange(ItemIndexRange itemIndexRange) => SelectRangeOverride(itemIndexRange);
	public void DeselectRange(ItemIndexRange itemIndexRange) => DeselectRangeOverride(itemIndexRange);
	public bool IsSelected(int index) => IsSelectedOverride(index);
	public IReadOnlyList<ItemIndexRange> GetSelectedRanges() => GetSelectedRangesOverride();

	// ISelectionInfo impl
	internal void SelectRangeImpl(ItemIndexRange itemIndexRange)
	{
		foreach (var index in ExpandRange(itemIndexRange))
		{
			this[index].Selected = true;
		}
	}
	internal void DeselectRangeImpl(ItemIndexRange itemIndexRange)
	{
		foreach (var index in ExpandRange(itemIndexRange))
		{
			this[index].Selected = false;
		}
	}
	internal bool IsSelectedImpl(int index) => this[index].Selected;
	internal IReadOnlyList<ItemIndexRange> GetSelectedRangesImpl()
	{
		return ReduceToRange(this
			.Select((x, i) => (Index: i, x.Selected))
			.Where(x => x.Selected)
			.Select(x => x.Index)
		).ToArray();
	}

	// helper methods
	internal static IEnumerable<ItemIndexRange> ReduceToRange(IEnumerable<int> indexes)
	{
		int first = int.MinValue;
		uint n = 0;
		foreach (var i in indexes.OrderBy(x => x))
		{
			if (first + n == i)
			{
				n++;
			}
			else
			{
				if (n > 0) yield return new(first, n);

				first = i;
				n = 1;
			}
		}

		if (n > 0)
		{
			yield return new(first, n);
		}
	}
	internal static IEnumerable<int> ExpandRange(ItemIndexRange range) => Enumerable.Range(range.FirstIndex, (int)range.Length);
}

