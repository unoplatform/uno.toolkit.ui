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

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	internal class TabBarTests
	{
		[TestMethod]
		[DataRow(new int[0], null)]
		[DataRow(new[] { 1 }, 1)]
		[DataRow(new[] { 1, 1 }, 1)]
		[DataRow(new[] { 1, 2 }, 2)]
		public async Task TabBarTapSelection(int[] selectionSequence, object? expectation)
		{
			var source = Enumerable.Range(0, 3).ToArray();
			var SUT = new TabBar
			{
				ItemsSource = source,
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
			Assert.IsNull(SUT.SelectedItem);

			foreach (var i in selectionSequence)
			{
				((TabBarItem)SUT.ContainerFromIndex(i)).ExecuteTap();
				await UnitTestsUIContentHelper.WaitForIdle();
			}

			Assert.AreEqual((int?)expectation, SUT.SelectedItem);
		}

		[TestMethod]
		public async Task SetSelectedItem()
		{
			var source = Enumerable.Range(0, 3).Select(x => new TabBarItem { Content = x }).ToArray();
			var SUT = new TabBar
			{
				ItemsSource = source,
			};

			source[0].IsSelectable = false;

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
			Assert.IsNull(SUT.SelectedItem);

			// should not select when IsSelectable is false
			SUT.SelectedItem = source[0];
			await UnitTestsUIContentHelper.WaitForIdle();
			Assert.AreEqual(-1, SUT.SelectedIndex);
			Assert.IsFalse(source[0].IsSelected);

			// should select
			SUT.SelectedItem = source[1];
			await UnitTestsUIContentHelper.WaitForIdle();
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

			source[0].IsSelectable = false;

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
			Assert.IsNull(SUT.SelectedItem);

			// should not select when IsSelectable is false
			SUT.SelectedIndex = 0;
			await UnitTestsUIContentHelper.WaitForIdle();
			Assert.IsNull(SUT.SelectedItem);
			Assert.IsFalse(source[0].IsSelected);

			// should select
			SUT.SelectedIndex = 1;
			await UnitTestsUIContentHelper.WaitForIdle();
			Assert.AreEqual(source[1], SUT.SelectedItem);
			Assert.AreEqual(1, SUT.SelectedIndex);
			Assert.IsTrue(source[1].IsSelected);
		}

		[TestMethod]
		public async Task Verify_Indicator_Max_Size()
		{
			var source = Enumerable.Range(0, 3).Select(x => new TabBarItem { Content = x }).ToArray();
			var indicator = new Border() { Height = 5, Background = new SolidColorBrush(Colors.Red) };
			var SUT = new TabBar
			{
				ItemsSource = source,
				SelectionIndicatorContent = indicator,
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			source[0].IsSelected = true;
			await UnitTestsUIContentHelper.WaitForIdle();

			var expectedWidth = SUT.ActualWidth / 3;
			var indicatorWidth = indicator.ActualWidth;
			Assert.AreEqual(expectedWidth, indicatorWidth, delta: 1f);

			source[1].Visibility = Visibility.Collapsed;

			await UnitTestsUIContentHelper.WaitForIdle();
			await UnitTestUIContentHelperEx.WaitFor(() => indicator.ActualWidth > indicatorWidth, timeoutMS: 2000);

			expectedWidth = SUT.ActualWidth / 2;
			Assert.AreEqual(expectedWidth, indicator.ActualWidth, delta: 1f);
		}

	}
}
