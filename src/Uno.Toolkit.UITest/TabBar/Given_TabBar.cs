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
		protected override string SampleName => "TabBar";
		private string[] _sections = new[] { "Home", "Search", "Support", "About" };

		[Test]
		public void When_Top_TabBar()
		{
			const string FlipViewItemTextPrefix = "TopTabBar_FlipView_Item_Text_";
			const string TabBarItemPrefix = "TopTabBar_Item_";

			NavigateToNestedSample("M3MaterialTopBarSampleNestedPage");
			App.WaitForElement("TopTabBar");

			foreach (var section in _sections)
			{
				var currentTabBarItem = App.Marked($"{TabBarItemPrefix}{section}");

				currentTabBarItem.FastTap();
				var c = $"{FlipViewItemTextPrefix}{section}";
				App.WaitForElement(c, timeout: TimeSpan.FromMinutes(5), timeoutMessage: "Why are you here");

				App.WaitForDependencyPropertyValue(currentTabBarItem, "IsSelected", true);
				App.WaitForText($"{FlipViewItemTextPrefix}{section}", section);
			}
		}

		[Test]
		public void When_Bottom_TabBar()
		{
			const string FlipViewItemTextPrefix = "BottomTabBar_FlipView_Item_Text_";
			const string TabBarItemPrefix = "BottomTabBar_Item_";

			NavigateToNestedSample("M3MaterialBottomBarSampleNestedPage");
			App.WaitForElement("BottomTabBar");

			foreach (var section in _sections)
			{
				var currentTabBarItem = App.Marked($"{TabBarItemPrefix}{section}");

				currentTabBarItem.FastTap();
				var c = $"{FlipViewItemTextPrefix}{section}";
				App.WaitForElement(c, timeout: TimeSpan.FromMinutes(5), timeoutMessage: "Why are you here");

				App.WaitForDependencyPropertyValue(currentTabBarItem, "IsSelected", true);
				App.WaitForText($"{FlipViewItemTextPrefix}{section}", section);
			}
		}
	}
}
