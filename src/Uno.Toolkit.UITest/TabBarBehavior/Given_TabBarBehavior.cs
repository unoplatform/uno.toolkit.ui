using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Uno.UITest.Helpers;
using Uno.UITest.Helpers.Queries;

namespace Uno.Toolkit.UITest.TabBarBehavior
{
	public class Given_TabBarBehavior : TestBase
	{
		[SetUp]
		public override void SetUpTest()
		{
			base.SetUpTest();

			NavigateToSample("TabBarBehavior");
		}

		[Test]
		public void When_Tab_Selected()
		{
			App.ScrollTo("ListView");

			var tab1 = App.MarkedAnywhere("ListViewTab1");
			var tab2 = App.MarkedAnywhere("ListViewTab2");
			var tab3 = App.MarkedAnywhere("ListViewTab3");
			var tab4 = App.MarkedAnywhere("ListViewTab4");

			var item1 = App.MarkedAnywhere("ListViewItem1");
			var item2 = App.MarkedAnywhere("ListViewItem2");
			var item3 = App.MarkedAnywhere("ListViewItem3");

			App.Tap(tab1);

			Assert.IsTrue(tab1.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsTrue(item1.GetDependencyPropertyValue<bool>("IsSelected")); 
			
			App.Tap(tab2);

			Assert.IsTrue(tab2.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsTrue(item2.GetDependencyPropertyValue<bool>("IsSelected"));

			App.Tap(tab3);

			Assert.IsTrue(tab3.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsTrue(item3.GetDependencyPropertyValue<bool>("IsSelected"));

			App.Tap(tab4);

			Assert.IsTrue(tab4.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsTrue(item3.GetDependencyPropertyValue<bool>("IsSelected"));
		}

		[Test]
		public void When_Selector_Selected()
		{
			App.ScrollTo("ListView");

			var tab1 = App.MarkedAnywhere("ListViewTab1");
			var tab2 = App.MarkedAnywhere("ListViewTab2");
			var tab3 = App.MarkedAnywhere("ListViewTab3");
			var tab4 = App.MarkedAnywhere("ListViewTab4");

			var item1 = App.MarkedAnywhere("ListViewItem1");
			var item2 = App.MarkedAnywhere("ListViewItem2");
			var item3 = App.MarkedAnywhere("ListViewItem3");

			App.Tap(item1);

			Assert.IsTrue(tab1.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsTrue(item1.GetDependencyPropertyValue<bool>("IsSelected"));

			App.Tap(item2);

			Assert.IsTrue(tab2.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsTrue(item2.GetDependencyPropertyValue<bool>("IsSelected"));

			App.Tap(item3);

			Assert.IsTrue(tab3.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsTrue(item3.GetDependencyPropertyValue<bool>("IsSelected"));
		}


		[Test]
		public void When_Tab_Selected_With_Second_Tab_Unselectable()
		{
			App.ScrollTo("SkipListView");

			var tab1 = App.MarkedAnywhere("ListViewSkipTab1");
			var tab2 = App.MarkedAnywhere("ListViewSkipTab2");
			var tab3 = App.MarkedAnywhere("ListViewSkipTab3");
			var tab4 = App.MarkedAnywhere("ListViewSkipTab4");

			var item1 = App.MarkedAnywhere("ListViewSkipItem1");
			var item2 = App.MarkedAnywhere("ListViewSkipItem2");
			var item3 = App.MarkedAnywhere("ListViewSkipItem3");

			App.Tap(tab1);

			Assert.IsTrue(tab1.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsTrue(item1.GetDependencyPropertyValue<bool>("IsSelected"));

			App.Tap(tab2);

			Assert.IsFalse(tab2.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsFalse(item2.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsTrue(tab1.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsTrue(item1.GetDependencyPropertyValue<bool>("IsSelected"));

			App.Tap(tab3);

			Assert.IsTrue(tab3.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsTrue(item2.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsFalse(item3.GetDependencyPropertyValue<bool>("IsSelected"));

			App.Tap(tab4);

			Assert.IsTrue(tab4.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsTrue(item3.GetDependencyPropertyValue<bool>("IsSelected"));
		}

		[Test]
		public void When_Selector_Selected_With_Second_Tab_Unselectable()
		{
			App.ScrollTo("SkipListView");

			var tab1 = App.MarkedAnywhere("ListViewSkipTab1");
			var tab2 = App.MarkedAnywhere("ListViewSkipTab2");
			var tab3 = App.MarkedAnywhere("ListViewSkipTab3");
			var tab4 = App.MarkedAnywhere("ListViewSkipTab4");

			var item1 = App.MarkedAnywhere("ListViewSkipItem1");
			var item2 = App.MarkedAnywhere("ListViewSkipItem2");
			var item3 = App.MarkedAnywhere("ListViewSkipItem3");

			App.Tap(item1);

			Assert.IsTrue(tab1.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsTrue(item1.GetDependencyPropertyValue<bool>("IsSelected"));

			App.Tap(item2);

			Assert.IsFalse(tab2.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsTrue(item2.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsTrue(tab3.GetDependencyPropertyValue<bool>("IsSelected"));

			App.Tap(item3);

			Assert.IsTrue(tab4.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsTrue(item3.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsFalse(tab3.GetDependencyPropertyValue<bool>("IsSelected"));
		}
	}
}
