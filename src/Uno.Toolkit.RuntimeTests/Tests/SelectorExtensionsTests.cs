using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.UI.RuntimeTests;
using Uno.Toolkit.RuntimeTests.Helpers;
using System.Threading.Tasks;
using Uno.Toolkit.UI;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	public class SelectorExtensionsTests
	{
		[TestMethod]
		public async Task TestAddingItemsBeforeLoadedAndAfterSetPipsPager()
		{
			var grid = new Grid();
			var flipView = new FlipView();
			var pipsPager = new PipsPager();
			grid.Children.Add(flipView);
			grid.Children.Add(pipsPager);
			SelectorExtensions.SetPipsPager(flipView, pipsPager);
			flipView.Items.Add("Test1");
			flipView.Items.Add("Test2");
			flipView.Items.Add("Test3");
			await UnitTestUIContentHelperEx.SetContentAndWait(grid);
			Assert.AreEqual(3, pipsPager.NumberOfPages);
		}
	}
}
