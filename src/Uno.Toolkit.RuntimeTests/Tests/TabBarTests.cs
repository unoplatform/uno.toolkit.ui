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
using Windows.UI.Xaml.Controls;

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	internal class TabBarTests
	{
		[TestMethod]
		[DataRow(new int[0], 0)] // selection enforced by 'Single'
		[DataRow(new[] { 1 }, 1)]
		[DataRow(new[] { 1, 1 }, 1)] // deselection denied
		[DataRow(new[] { 1, 2 }, 2)] // reselection
		[Ignore("Test ignored until we can simulate clicks")]
		public async Task TapSelection(ChipSelectionMode mode, int[] selectionSequence, object expectation)
		{
			var source = Enumerable.Range(0, 3).ToArray();
			var SUT = new TabBar
			{
				ItemsSource = source,
			};
			var grid = new Grid();
			grid.Children.Add(SUT);

			await UnitTestUIContentHelperEx.SetContentAndWait(grid);
			Assert.IsNull(SUT.SelectedItem);

			foreach (var i in selectionSequence)
			{
				//((TabBarItem)SUT.ContainerFromIndex(i)).RaiseClick();
			}

			Assert.AreEqual((int?)expectation, SUT.SelectedItem);
			Assert.IsNull(SUT.SelectedItem);
		}

		[TestMethod]
		public async Task SetSelectedItem()
		{
			var source = Enumerable.Range(0, 3).Select(x => new TabBarItem { Content = x }).ToArray();
			var SUT = new TabBar
			{
				ItemsSource = source,
			};

			var grid = new Grid();
			grid.Children.Add(SUT);

			source[0].IsSelectable = false;			

			await UnitTestUIContentHelperEx.SetContentAndWait(grid);
			Assert.IsNull(SUT.SelectedItem);

			// should not select when IsSelectable is false
			SUT.SelectedItem = source[0];
			Assert.AreEqual(-1, SUT.SelectedIndex);
			Assert.IsFalse(source[0].IsSelected);

			// should select
			SUT.SelectedItem = source[1];
			Assert.AreEqual(source[1], SUT.SelectedItem);
			Assert.AreEqual(1, SUT.SelectedIndex);
			Assert.IsTrue(source[1].IsSelected);
		}

		
		[TestMethod]
		public async Task SetSelectedIndex()
		{
			var source = Enumerable.Range(0, 3).Select(x => new TabBarItem { Content = x }).ToArray();
			var SUT = new TabBar
			{
				ItemsSource = source,
			};

			var grid = new Grid();
			grid.Children.Add(SUT);

			source[0].IsSelectable = false;

			await UnitTestUIContentHelperEx.SetContentAndWait(grid);
			Assert.IsNull(SUT.SelectedItem);

			// should not select when IsSelectable is false
			SUT.SelectedIndex = 0;
			Assert.IsNull(SUT.SelectedItem);
			Assert.IsFalse(source[0].IsSelected);

			// should select
			SUT.SelectedIndex = 1;
			Assert.AreEqual(source[1], SUT.SelectedItem);
			Assert.AreEqual(1, SUT.SelectedIndex);
			Assert.IsTrue(source[1].IsSelected);
		}
	}
}
