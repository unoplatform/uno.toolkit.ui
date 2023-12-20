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
	public class Given_ShadowContainer : TestBase
	{
		protected override string SampleName => "ShadowContainerTest";

		const string Red = "#FF0000";
		const string Blue = "#0000FF";
		const string Green = "#008000";

		[Test]
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

			var caseName = $"Shadow_x{xOffset}_y{yOffset}{(inner ? "_Inner" : "_Outer")}";
			Check_Assert(outerBorderRect, elementRect, xOffset, yOffset, inner, caseName);

			resetButton.FastTap();

		}


		[Test]
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

			var caseName = $"ShadowRectangle_x{xOffset}_y{yOffset}{(inner ? "_Inner" : "_Outer")}";
			Check_Assert(outerBorderRect, elementRect, xOffset, yOffset, inner, caseName);
			resetButton.FastTap();

		}


		[Test]
		[TestCase(10, 10, false)]
		[TestCase(-10, -10, false)]
		public void When_AsymmetricShadowsCorner(int xOffset, int yOffset, bool inner)
		{
			var shadowContainer = App.WaitForElementWithMessage("shadowContainer");
			var resetButton = App.MarkedAnywhere("resetButton");
			resetButton.FastTap();

			App.Tap("check_IrregularCorner");
			shadowContainer = App.WaitForElementWithMessage("shadowContainerIrregularCorner");
			var runButton = App.MarkedAnywhere("runButton");
			var statusText = App.MarkedAnywhere("statusText");


			App.MarkedAnywhere("xOffsetText").ClearText().EnterTextAndDismiss(xOffset.ToString());
			App.MarkedAnywhere("yOffsetText").ClearText().EnterTextAndDismiss(yOffset.ToString());
			var innerCheck = App.MarkedAnywhere("inner");

			innerCheck.SetDependencyPropertyValue("IsChecked", inner.ToString());
			App.WaitForDependencyPropertyValue(innerCheck, "IsChecked", inner);

			runButton.FastTap();

			App.WaitForDependencyPropertyValue<string>(statusText, "Text", "Verify");
			var outerTestRect = App.GetPhysicalRect("outerBorderIrregularCorner");

			var caseName = $"ShadowIrregular_x{xOffset}_y{yOffset}{(inner ? "_Inner" : "_Outer")}";

			using var screenshot = TakeScreenshot(caseName);



			int currentX = 3;
			int currentY = 3;

			int absXOffset = Math.Abs(xOffset);
			int absYOffset = Math.Abs(yOffset);

			while (currentX < absXOffset || currentY < absYOffset)
			{

				var beginX = (int)outerTestRect.X;
				var beginY = (int)outerTestRect.Y;
				var centerX = (int)outerTestRect.CenterX;
				var centerY = (int)outerTestRect.CenterY;
				var endX = (int)outerTestRect.Right;
				var endY = (int)outerTestRect.Bottom;

				//CornerRadius="0,100,0,100"
				/*
				 * Points for test
				 	 _______________________
					|	1		7		4	|
					|		A		C		|
					|	2		8		5	|
					|		B		D		|
					|	3		9		6	|
					 _______________________
				*/

				//1
				var leftTopOuterTestPoint = new Point(beginX + currentX, beginY + currentY);
				//2
				var leftMiddleOuterTestPoint = new Point(beginX + currentX, centerY);
				//3
				var leftBottomOuterTestPoint = new Point(beginX + currentX, endY - currentY);

				//4
				var rightTopOuterTestPoint = new Point(endX - currentX, beginY + currentY);
				//5
				var rightMiddleOuterTestPoint = new Point(endX - currentX, centerY);
				//6
				var rightBottomOuterTestPoint = new Point(endX - currentX, endY - currentY);

				//7
				var centerTopOuterTestPoint = new Point(centerX - currentX, beginY + currentY);
				//8
				var centerMiddleOuterTestPoint = new Point(centerX - currentX, centerY);
				//9
				var centerBottomOuterTestPoint = new Point(centerX - currentX, endY - currentY);

				//A
				var cornerTopLeftOuterTestPoint = new Point(beginX + (endX - beginX) / 5 * 2 + currentX, beginY + (endY - beginY) / 5 * 2 + currentY);
				//B
				var cornerBottomLeftOuterTestPoint = new Point(beginX + (endX - beginX) / 5 * 4 + currentX, beginY + (endY - beginY) / 5 * 2 - currentY);
				//C
				var cornerTopRightOuterTestPoint = new Point(beginX + (endX - beginX) / 5 * 2 - currentX, beginY + (endY - beginY) / 5 * 4 + currentY);
				//D
				var cornerBottomRightOuterTestPoint = new Point(beginX + (endX - beginX) / 5 * 4 - currentX, beginY + (endY - beginY) / 5 * 4 - currentY);

				var outerDefault = inner ? Blue : Red;

				AssertExpectations(new[] {
					(leftTopOuterTestPoint, xOffset < 0 ? outerDefault : Blue),
					(leftMiddleOuterTestPoint, Blue),
					(leftBottomOuterTestPoint, Blue),

					(rightTopOuterTestPoint, Blue),
					(rightMiddleOuterTestPoint, Blue),
					(rightBottomOuterTestPoint, xOffset < 0 ? Blue : outerDefault),

					(centerTopOuterTestPoint, Blue),
					(centerMiddleOuterTestPoint, Green),
					(centerBottomOuterTestPoint, Blue ),

					(cornerTopLeftOuterTestPoint, Green),
					(cornerBottomLeftOuterTestPoint, Blue),
					(cornerTopRightOuterTestPoint, Blue),
					(cornerBottomRightOuterTestPoint, Green)
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
			//resetButton.FastTap();
		}

		public void Check_Assert(IAppRect outerBorderRect, IAppRect borderRect, int xOffset, int yOffset, bool inner, string caseName)
		{

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
