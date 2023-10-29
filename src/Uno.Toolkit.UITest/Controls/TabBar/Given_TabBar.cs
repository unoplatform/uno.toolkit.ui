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

namespace Uno.Toolkit.UITest.Controls.TabBar
{
	[TestFixture]
	public class Given_TabBar : TestBase
	{
		protected override string SampleName => "TabBar";
		private string[] _sections = new[] { "Home", "Search", "Support", "About" };

		[Test]
		[AutoRetry]
		[Ignore("https://github.com/unoplatform/uno.toolkit.ui/issues/887")]
		public void When_Top_TabBar()
		{
			const string FlipViewItemTextPrefix = "TopTabBar_FlipView_Item_Text_";
			const string TabBarItemPrefix = "TopTabBar_Item_";

			NavigateToNestedSample("M3MaterialTopBarSampleNestedPage");
			App.WaitForElementWithMessage("TopTabBar");

			foreach (var section in _sections)
			{
				App.FastTap($"{TabBarItemPrefix}{section}");
				App.WaitForElementWithMessage($"{FlipViewItemTextPrefix}{section}", timeout: TimeSpan.FromMinutes(5));
			}
		}

		[Test]
		[AutoRetry]
		[Ignore("https://github.com/unoplatform/uno.toolkit.ui/issues/887")]
		public void When_Bottom_TabBar()
		{
			const string FlipViewItemTextPrefix = "BottomTabBar_FlipView_Item_Text_";
			const string TabBarItemPrefix = "BottomTabBar_Item_";

			NavigateToNestedSample("M3MaterialBottomBarSampleNestedPage");
			App.WaitForElementWithMessage("BottomTabBar");

			foreach (var section in _sections)
			{
				App.FastTap($"{TabBarItemPrefix}{section}");
				App.WaitForElementWithMessage($"{FlipViewItemTextPrefix}{section}", timeout: TimeSpan.FromMinutes(5));
			}
		}

		[Test]
		[AutoRetry]
		[Ignore("https://github.com/unoplatform/uno.toolkit.ui/issues/887")]
		public void When_Vertical_TabBar()
		{
			const string FlipViewItemTextPrefix = "VerticalTabBar_FlipView_Item_Text_";
			const string TabBarItemPrefix = "VerticalTabBar_Item_";

			NavigateToNestedSample("M3MaterialVerticalBarSampleNestedPage");
			App.WaitForElementWithMessage("VerticalTabBar");

			foreach (var section in _sections)
			{
				App.FastTap($"{TabBarItemPrefix}{section}");
				App.WaitForElementWithMessage($"{FlipViewItemTextPrefix}{section}", timeout: TimeSpan.FromMinutes(5));
			}
		}
	}
}