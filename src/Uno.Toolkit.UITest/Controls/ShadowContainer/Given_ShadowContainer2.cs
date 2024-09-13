#if IS_WINUI
using NUnit.Framework;
using System;
using System.Drawing;
using System.Reflection;
using Uno.Toolkit.UITest.Extensions;
using Uno.Toolkit.UITest.Framework;
using Uno.UITest;
using Uno.UITest.Helpers;
using Uno.UITest.Helpers.Queries;

namespace Uno.Toolkit.UITest.Controls.ShadowContainer
{
	[TestFixture]
	public class Given_ShadowContainer2 : TestBase
	{
		protected override string SampleName => "ShadowContainerTest2";

		[Test]
		public void When_Unloaded_Then_Loaded()
		{
			App.WaitForElement("shadowContainer");
			using var screenshot1 = TakeScreenshot("initial");

			var testStatus = App.MarkedAnywhere("testStatus");
			var loadButton = App.MarkedAnywhere("reloadButton").FastTap();

			App.WaitForDependencyPropertyValue(testStatus, "Text", "Completed");

			using var screenshot2 = TakeScreenshot("reloaded");

			var rect = GetRectangle("shadowContainer");
			ImageAssert.AreAlmostEqual(screenshot1, screenshot2, rect);
		}

		private Rectangle GetRectangle(string marked)
		{
			var rect = App.Marked(marked).FirstResult()?.Rect;
			return rect != null ? App.ToPhysicalRect(rect).ToRectangle() : Rectangle.Empty;
		}
	}
}
#endif
