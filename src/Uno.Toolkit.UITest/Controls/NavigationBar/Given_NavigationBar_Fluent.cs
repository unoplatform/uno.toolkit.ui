using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Uno.Toolkit.UITest.Extensions;
using Uno.Toolkit.UITest.Framework;
using Uno.UITest;
using Uno.UITest.Helpers;
using Uno.UITest.Helpers.Queries;
using Uno.UITests.Helpers;

namespace Uno.Toolkit.UITest.Controls.NavigationBar
{
	[ActivePlatforms(Platform.Android, Platform.iOS)]
	[TestFixture]
	public class Given_NavigationBar_Fluent : TestBase
	{
		protected override string SampleName => "NavigationBar-Fluent";

		[Test]
		[AutoRetry]
		public void NavBar_AppBarButton_With_Icon_Click()
		{
			NavigateToNestedSample("FluentNavigationBarSampleNestedPage");

			PlatformHelpers.On(
				iOS: () => App.Tap("FluentPage1NavBarPrimaryCommand3"),
				Android: () => App.Tap("FluentPage1NavBarPrimaryCommand3")
			);

			App.WaitForElement("FluentPage2NavBar", "Timed out waiting for Page 2 Nav Bar");
		}

		[Test]
		[AutoRetry]
		public void NavBar_Can_Close_Flyout_With_MainCommand()
		{
			NavigateToNestedSample("FluentNavigationBarSampleNestedPage");
			App.Tap("OpenPage2FlyoutButton");
			App.WaitForElement("FluentPage2NavBar", "Timed out waiting for Page 2 Nav Bar");
			App.Tap("NavigateToThirdButton");
			App.WaitForElement("FluentPage3NavBar", "Timed out waiting for Page 3 Nav Bar");

			PlatformHelpers.On(
				iOS: () => App.Tap("FluentPage3NavBarMainCommand"),
				Android: () => App.Tap(q => q.Marked("FluentPage3NavBar").Descendant("AppCompatImageButton"))
			);

			App.WaitForElement("FluentPage2NavBar", "Timed out waiting for Page 2 Nav Bar");

			PlatformHelpers.On(
				iOS: () => App.Tap("FluentPage2NavBarMainCommand"),
				Android: () => App.Tap(q => q.Marked("FluentPage2NavBar").Descendant("AppCompatImageButton"))
			);

			App.WaitForElement("FluentPage1NavBar", "Timed out waiting for Page 1 Nav Bar");
		}
	}
}
