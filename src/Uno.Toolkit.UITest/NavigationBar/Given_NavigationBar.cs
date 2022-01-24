using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Uno.UITest.Helpers;
using Uno.UITest.Helpers.Queries;

namespace Uno.Toolkit.UITest.NavigationBar
{
	public class Given_NavigationBar : TestBase
	{
		[SetUp]
		public override void SetUpTest()
		{
			base.SetUpTest();
			NavigateToSample("NavigationBar");
		}

		[Test]
		public void NavBar_Has_Size()
		{
			App.WaitThenTap("NavigationBar_Launch_Sample_Button");

			var toolbar = App.Marked("Page1NavBar").Descendant("Toolbar").FirstResult();

			Assert.NotZero(toolbar.Rect.Height);
			Assert.NotZero(toolbar.Rect.Width);
		}

		[Test]
		public void NavBar_Has_Title()
		{
			App.WaitThenTap("NavigationBar_Launch_Sample_Button");
			
			var title = App.Marked("Page1NavBar").Descendant("AppCompatTextView").FirstResult();

			Assert.AreEqual("First Page", title.Text);
		}

		[Test]
		public void NavBar_Can_Close_From_First_Page()
		{
			App.WaitThenTap("NavigationBar_Launch_Sample_Button");

			App.WaitForElement("Page1NavBar", "Timed out waiting for no Nav Bar");

			App.Tap(q => q.Marked("Page1NavBar").Descendant("AppCompatImageButton"));

			App.WaitForNoElement("Page1NavBar", "Timed out waiting for Nav Bar");
		}

		[Test]
		public void NavBar_Page2_NavBar_Exists()
		{
			App.WaitThenTap("NavigationBar_Launch_Sample_Button");

			App.WaitForNoElement("Page2NavBar", "Timed out waiting for no Page 2 Nav Bar");

			App.Tap("Page1_Navigate_To_Page2");

			App.WaitForElement("Page2NavBar", "Timed out waiting for Page 2 Nav Bar");
			
			var toolbar = App.Query(q => q.Marked("Page2NavBar").Descendant("Toolbar")).Single();

			Assert.NotZero(toolbar.Rect.Height);
			Assert.NotZero(toolbar.Rect.Width);
		}

		[Test]
		public void NavBar_Page_Can_Go_Back()
		{
			App.WaitThenTap("NavigationBar_Launch_Sample_Button");

			App.WaitForNoElement("Page2NavBar", "Timed out waiting for no Page 2 Nav Bar");

			App.Tap("Page1_Navigate_To_Page2");

			App.WaitForElement("Page2NavBar", "Timed out waiting for Page 2 Nav Bar");


			App.Tap(q => q.Marked("Page2NavBar").Descendant("AppCompatImageButton"));
			
			App.WaitForNoElement("Page2NavBar", "Timed out waiting for no Page 2 Nav Bar");
		}
	}
}
