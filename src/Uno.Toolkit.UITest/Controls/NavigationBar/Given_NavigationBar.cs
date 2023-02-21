﻿using System;
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
	public class Given_NavigationBar : TestBase
	{
		protected override string SampleName => "NavigationBar";

		[Test]
		[AutoRetry]
		public void NavBar_Has_Size()
		{
			NavigateToNestedSample("M3MaterialNavigationBarSample_NestedPage1");

			var nativeBar = PlatformHelpers.On<IAppResult>(
				iOS: () => App.CreateQuery(x => x.WithClass("navigationBar")).FirstResult(),
				Android: () => App.Marked("M3Page1NavBar").Descendant("Toolbar").FirstResult()
			);

			Assert.NotZero(nativeBar.Rect.Height);
			Assert.NotZero(nativeBar.Rect.Width);
		}

		[Test]
		[AutoRetry]
		public void NavBar_Has_Title()
		{
			NavigateToNestedSample("M3MaterialNavigationBarSample_NestedPage1");

			var title = PlatformHelpers.On<IAppResult>(
				iOS: () => App.CreateQuery(x => x.WithClass("navigationBar").Descendant("label")).FirstResult(),
				Android: () => App.Marked("M3Page1NavBar").Descendant("AppCompatTextView").FirstResult()
			);

			Assert.AreEqual("First Page", title.Text);
		}

		[Test]
		[AutoRetry]
		public void NavBar_Can_Close_From_First_Page()
		{
			NavigateToNestedSample("M3MaterialNavigationBarSample_NestedPage1");

			App.WaitForElementWithMessage("M3Page1NavBar");

			PlatformHelpers.On(
				iOS: () => App.FastTap("CloseIcon"),
				Android: () => App.FastTap(q => q.Marked("M3Page1NavBar").Descendant("AppCompatImageButton"))
			);
			;

			PlatformHelpers.On(
				iOS: () => App.WaitForNoElement(q => q.Class("navigationBar"), "Timed out waiting for Nav Bar"),
				Android: () => App.WaitForNoElement("M3Page1NavBar", "Timed out waiting for Nav Bar")
			);
		}

		[Test]
		[AutoRetry]
		public void NavBar_Page2_NavBar_Exists()
		{
			NavigateToNestedSample("M3MaterialNavigationBarSample_NestedPage1");

			App.WaitForNoElement("M3Page2NavBar", "Timed out waiting for no Page 2 Nav Bar");

			App.FastTap("M3_Page1_Navigate_To_Page2");

			App.WaitForElementWithMessage("M3Page2NavBar");
			var nativeBar = PlatformHelpers.On<IAppResult>(
				iOS: () => App.CreateQuery(x => x.WithClass("navigationBar")).FirstResult(),
				Android: () => App.Marked("M3Page2NavBar").Descendant("Toolbar").FirstResult()
			);

			Assert.NotZero(nativeBar.Rect.Height);
			Assert.NotZero(nativeBar.Rect.Width);
		}

		[Test]
		[AutoRetry]
		public void NavBar_Page_Can_Go_Back()
		{
			NavigateToNestedSample("M3MaterialNavigationBarSample_NestedPage1");

			App.WaitForNoElement("M3Page2NavBar", "Timed out waiting for no Page 2 Nav Bar");

			App.FastTap("M3_Page1_Navigate_To_Page2");

			App.WaitForElementWithMessage("M3Page2NavBar");

			PlatformHelpers.On(
				iOS: () => App.FastTap("BackButton"),
				Android: () => App.FastTap(q => q.Marked("M3Page2NavBar").Descendant("AppCompatImageButton"))
			);

			App.WaitForNoElement("M3Page2NavBar", "Timed out waiting for no Page 2 Nav Bar");
		}
	}
}
