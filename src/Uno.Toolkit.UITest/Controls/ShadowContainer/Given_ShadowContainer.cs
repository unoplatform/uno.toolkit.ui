#if IS_WINUI
using NUnit.Framework;
using System;
using System.Drawing;
using Uno.Toolkit.UITest.Extensions;
using Uno.Toolkit.UITest.Framework;
using Uno.UITest;
using Uno.UITest.Helpers;
using Uno.UITest.Helpers.Queries;

namespace Uno.Toolkit.UITest.Controls.ShadowContainer
{
	[TestFixture]
	public class Given_ShadowContainer : TestBase
	{
		protected override string SampleName => "ShadowContainerTest";

		const string Red = "#FF0000";
		const string Blue = "#0000FF";
		const string Green = "#008000";

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

			var shadowContainer = App.WaitForElementWithMessage("shadowContainer");
			var runButton = App.MarkedAnywhere("runButton");
			var resetButton = App.MarkedAnywhere("resetButton");
			

			var statusText = App.MarkedAnywhere("statusText");
			var shadowRect = App.GetPhysicalRect("shadowContainer");
			App.MarkedAnywhere("xOffsetText").ClearText().EnterTextAndDismiss(xOffset.ToString());
			App.MarkedAnywhere("yOffsetText").ClearText().EnterTextAndDismiss(yOffset.ToString());

			App.Tap("check_Border");
			var innerCheck = App.MarkedAnywhere("inner");
			innerCheck.SetDependencyPropertyValue("IsChecked", inner.ToString());
			App.WaitForDependencyPropertyValue(innerCheck, "IsChecked", inner);

			runButton.FastTap();

			App.WaitForDependencyPropertyValue<string>(statusText, "Text", "Verify");
			var outerBorderRect = App.GetPhysicalRect("outerBorder");
			var elementRect = App.GetPhysicalRect("border");

			Check_Assert(outerBorderRect, elementRect, xOffset, yOffset, inner);

			resetButton.FastTap();

		}


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
		public void When_RectangleShadows(int xOffset, int yOffset, bool inner)
		{

			var shadowContainer = App.WaitForElementWithMessage("shadowContainer");
			App.Tap("check_Rectangle");
			shadowContainer = App.WaitForElementWithMessage("shadowContainerRectangle");
			var runButton = App.MarkedAnywhere("runButton");
			var resetButton = App.MarkedAnywhere("resetButton");

			var statusText = App.MarkedAnywhere("statusText");
			var shadowRect = App.GetPhysicalRect("shadowContainerRectangle");
			App.MarkedAnywhere("xOffsetText").ClearText().EnterTextAndDismiss(xOffset.ToString());
			App.MarkedAnywhere("yOffsetText").ClearText().EnterTextAndDismiss(yOffset.ToString());

			var innerCheck = App.MarkedAnywhere("inner");
			innerCheck.SetDependencyPropertyValue("IsChecked", inner.ToString());
			App.WaitForDependencyPropertyValue(innerCheck, "IsChecked", inner);

			runButton.FastTap();

			App.WaitForDependencyPropertyValue<string>(statusText, "Text", "Verify");
			var outerBorderRect = App.GetPhysicalRect("outerBorderRetangle");
			var elementRect = App.GetPhysicalRect("rectangle");

			Check_Assert(outerBorderRect, elementRect, xOffset, yOffset, inner);
			resetButton.FastTap();

		}


		public void Check_Assert(IAppRect outerBorderRect, IAppRect borderRect, int xOffset, int yOffset, bool inner)
		{

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
				var leftOuterTestPoint = new Point((int)outerBorderRect.X + currentX, (int)outerBorderRect.CenterY);
				var topOuterTestPoint = new Point((int)outerBorderRect.CenterX, (int)outerBorderRect.Y + currentY);
				var rightOuterTestPoint = new Point((int)outerBorderRect.Right - currentX, (int)outerBorderRect.CenterY);
				var bottomOuterTestPoint = new Point((int)outerBorderRect.CenterX, (int)outerBorderRect.Bottom - currentY);

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
#endif
