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
		protected override string SampleName => "TabBarBehavior";

		[Test]
		public void When_Tab_Selected()
		{
			var tab1 = App.MarkedAnywhere("SlideTab1");
			var tab2 = App.MarkedAnywhere("SlideTab2");
			var tab3 = App.MarkedAnywhere("SlideTab3");

			var item1 = App.MarkedAnywhere("SlidePage1");
			var item2 = App.MarkedAnywhere("SlidePage2");
			var item3 = App.MarkedAnywhere("SlidePage3");

			App.FastTap(tab1);

			Assert.IsTrue(tab1.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsTrue(item1.GetDependencyPropertyValue<bool>("IsSelected")); 
			
			App.FastTap(tab2);

			Assert.IsTrue(tab2.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsTrue(item2.GetDependencyPropertyValue<bool>("IsSelected"));

			App.FastTap(tab3);

			Assert.IsTrue(tab3.GetDependencyPropertyValue<bool>("IsSelected"));
			Assert.IsTrue(item3.GetDependencyPropertyValue<bool>("IsSelected"));
		}
	}
}
