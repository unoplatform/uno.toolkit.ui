using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Uno.Toolkit.UITest.Extensions;
using Uno.Toolkit.UITest.Framework;
using Uno.UITest.Helpers;
using Uno.UITest.Helpers.Queries;

namespace Uno.Toolkit.UITest.Behaviors.TabBarBehavior
{
	[TestFixture]
	public class Given_TabBarBehavior : TestBase
	{
		protected override string SampleName => "TabBarBehavior";

		[Test]
		[AutoRetry]
		public void When_Tab_Selected()
		{
			var tab1 = App.MarkedAnywhere("SlideTab1");
			var tab2 = App.MarkedAnywhere("SlideTab2");
			var tab3 = App.MarkedAnywhere("SlideTab3");

			var tabBar = App.MarkedAnywhere("SlideTabBar");
			var flipView = App.MarkedAnywhere("SlideFlipView");

			App.FastTap(tab1);

			App.WaitForDependencyPropertyValue(tabBar, "SelectedIndex", 0);
			App.WaitForDependencyPropertyValue(flipView, "SelectedIndex", 0);

			App.FastTap(tab2);

			App.WaitForDependencyPropertyValue(tabBar, "SelectedIndex", 1);
			App.WaitForDependencyPropertyValue(flipView, "SelectedIndex", 1);

			App.FastTap(tab3);

			App.WaitForDependencyPropertyValue(tabBar, "SelectedIndex", 2);
			App.WaitForDependencyPropertyValue(flipView, "SelectedIndex", 2);
		}
	}
}
