using NUnit.Framework;
using OpenQA.Selenium.DevTools.V85.CSS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Toolkit.UITest.Extensions;
using Uno.Toolkit.UITest.Framework;
using Uno.UITest.Helpers;
using Uno.UITest.Helpers.Queries;

namespace Uno.Toolkit.UITest.Controls.ShadowContainer
{
	[TestFixture]
	public class Given_ShadowContainer : TestBase
	{
		protected override string SampleName => "ShadowContainerTest";

		[Test]
		[TestCase(10, 10, false)]
		[TestCase(10, 10, false)]
		[TestCase(10, 10, true)]
		[TestCase(-10, -10, false)]
		[TestCase(-10, -10, true)]
		[TestCase(-10, 10, true)]
		[TestCase(10, -10, true)]
		[TestCase(-10, 10, false)]
		[TestCase(10, -10, false)]
		public void When_Shadows(int xOffset, int yOffset, bool inner)
		{
			const string Red = "#FF0000";
			const string Blue = "#0000FF";
			const string Green = "#008000";

			var shadowContainer = App.WaitForElementWithMessage("shadowContainer");
			var runButton = App.MarkedAnywhere("runButton");
			var statusText = App.MarkedAnywhere("statusText");


			App.MarkedAnywhere("xOffsetText").ClearText().EnterTextAndDismiss(xOffset.ToString());
			App.MarkedAnywhere("yOffsetText").ClearText().EnterTextAndDismiss(yOffset.ToString());
			var innerCheck = App.MarkedAnywhere("inner");

			innerCheck.SetDependencyPropertyValue("IsChecked", inner.ToString());
			App.WaitForDependencyPropertyValue(innerCheck, "IsChecked", inner);

			runButton.FastTap();

			App.WaitForDependencyPropertyValue<string>(statusText, "Text", "Verify");
			var outerBorderRect = App.GetPhysicalRect("outerBorder");
			var borderRect = App.GetPhysicalRect("border");
			var shadowRect = App.GetPhysicalRect("shadowContainer");

			var caseName = $"Shadow_x{xOffset}_y{yOffset}{(inner ? "_Inner" : "_Outer")}";

			using var screenshot = TakeScreenshot(caseName);

			

			var xStart = xOffset < 0 ? (int)outerBorderRect.X : (int)outerBorderRect.Right;
			var yStart = yOffset < 0 ? (int)outerBorderRect.Y : (int)outerBorderRect.Bottom;

			int currentX = 1;
			int currentY = 1;

			int absXOffset = Math.Abs(xOffset);
			int absYOffset = Math.Abs(yOffset);

			while (currentX < absXOffset || currentY < absYOffset)
			{
				var outerTestRect = outerBorderRect;

				var leftOuterTestPoint = new Point((int)outerTestRect.X + currentX, (int)outerTestRect.CenterY);
				var topOuterTestPoint = new Point((int)outerTestRect.CenterX, (int)outerTestRect.Y + currentY);
				var rightOuterTestPoint = new Point((int)outerTestRect.Right - currentX, (int)outerTestRect.CenterY);
				var bottomOuterTestPoint = new Point((int)outerTestRect.CenterX, (int)outerTestRect.Bottom - currentY);

				var leftInnerTestPoint = new Point((int)borderRect.X + currentX, (int)borderRect.CenterY);
				var topInnerTestPoint = new Point((int)borderRect.CenterX, (int)borderRect.Y + currentY);
				var rightInnerTestPoint = new Point((int)borderRect.Right - currentX, (int)borderRect.CenterY);
				var bottomInnerTestPoint = new Point((int)borderRect.CenterX, (int)borderRect.Bottom - currentY);

				var outerDefault = inner ? Blue : Red;
				var innerDefault = inner ? Red : Green;

				AssertExpectations(new[]
				{
					(leftOuterTestPoint, xOffset < 0 ? outerDefault : Blue),
					(topOuterTestPoint, yOffset < 0 ? outerDefault : Blue), 
					(rightOuterTestPoint, xOffset < 0 ? Blue : outerDefault), 
					(bottomOuterTestPoint, yOffset < 0 ? Blue : outerDefault),
				});

				AssertExpectations(new[]
				{
					(leftInnerTestPoint, xOffset < 0 ? Green : innerDefault),
					(topInnerTestPoint, yOffset < 0 ? Green : innerDefault), 
					(rightInnerTestPoint, xOffset < 0 ? innerDefault : Green), 
					(bottomInnerTestPoint, yOffset < 0 ? innerDefault : Green),
				});

				void AssertExpectations((Point TestPoint, string Color)[] expectations)
				{
					foreach (var expectation in expectations)
					{
						ImageAssert.HasPixels(
							screenshot,
							ExpectedPixels
								.At(expectation.TestPoint)
								.Named($"{caseName}_at_{expectation.TestPoint.X}_{expectation.TestPoint.Y}_offset_{currentX}_{currentY}")
								.Pixel(expectation.Color)
						);
					}
				}

				currentX = Math.Min(++currentX, absXOffset);
				currentY = Math.Min(++currentY, absYOffset);
			}
		}

		private Rectangle GetRectangle(string marked)
		{
			var rect = App.Marked(marked).FirstResult()?.Rect;
			return rect != null ? App.ToPhysicalRect(rect).ToRectangle() : Rectangle.Empty;
		}
	}
}
