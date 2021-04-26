using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Uno.UITest.Helpers;
using Uno.UITest.Helpers.Queries;

namespace Uno.Toolkit.UITest.TabBar
{
	public class Given_TabBar : TestBase
	{
		[SetUp]
		public override void SetUpTest()
		{
			base.SetUpTest();
			NavigateToSample("TabBar");
		}

		[Test]
		public void When_Click_Tab_With_Command()
		{
			for (int i = 1; i < 4; i++)
			{
				var tabCount = App.MarkedAnywhere($"Tab{i}_Count");

				App.WaitForText(tabCount, "0");

				TakeScreenshot($"Before Tab {i} Tapped");
				App.WaitThenTap($"TabBarWithCommand_Tab{i}");
				TakeScreenshot($"After Tab {i} Tapped");

				App.WaitForText(tabCount, "1");
			}
		}

		[Test]
		public void When_Custom_SelectionIndicator()
		{
			App.ScrollTo("TabBar_SelectionIndicator");

			var selectionIndicator = App.MarkedAnywhere("TabBar_SelectionIndicator")
				.Descendant()
				.Marked("SelectionIndicatorPresenter");
			
			App.WaitForDependencyPropertyValue<string>(selectionIndicator, "Opacity", "0");
			App.WaitThenTap("TabBar_SelectionIndicator_Tab2");
			App.WaitForDependencyPropertyValue<string>(selectionIndicator, "Opacity", "1");
		}
	}
}
