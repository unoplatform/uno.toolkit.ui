using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.UITest;
using Uno.UITest.Helpers.Queries;
using Uno.UITests.Helpers;
using Uno.Toolkit.UITest.Extensions;
using System.Collections;
using Uno.Toolkit.UITest.Framework;
using OpenQA.Selenium.DevTools;
using Uno.UITest.Helpers;

namespace Uno.Toolkit.UITest.Controls.NativeFrame
{
	[TestFixture]
	public class Given_NativeFrame : TestBase
	{
		protected override string SampleName => "NativeFrame";

		[Test]
		[AutoRetry]
		[ActivePlatforms(Platform.iOS)]
		public void When_BackStack_Changed()
		{
			NavigateToNestedSample("NativeFrame_MainPage");

			App.WaitForElementWithMessage("NativeFrame_MainPage_Title");

			App.FastTap("NativeFrame_MainPage_DeeplinkBtn");

			App.WaitForElementWithMessage("NativeFrame_Page2_Title");

			App.FastTap("NativeFrame_Page2_BackWithBackStackBtn");

			App.WaitForElementWithMessage("NativeFrame_Page1_Title");

			App.FastTap("NativeFrame_Page1_BackBtn");
			
			App.WaitForElementWithMessage("NativeFrame_MainPage_Title");
		}
	}
}
