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

namespace Uno.Toolkit.UITest.Controls.SafeArea
{
	[TestFixture]
	public class Given_SafeArea : TestBase
	{
		protected override string SampleName => "SafeArea";

		private const string CheckboxSuffix = "Mask";
		private const string Left = "Left";
		private const string Top = "Top";
		private const string Right = "Right";
		private const string Bottom = "Bottom";
		private readonly List<string> _availableMasks = new List<string> { Left, Top, Right, Bottom };

		[SetUp]
		public override void SetUpTest()
		{
			base.SetUpTest();
			NavigateToSample("SafeArea");
		}

		[Test]
		[ActivePlatforms(Platform.Android, Platform.iOS)]
		public void When_SoftInput()
		{
			const string NamePrefix = "SafeArea_SoftInput_";
			const int HalfBorderThickness = 5;
			const string Red = "#FF0000";
			const string Black = "#000000";
			const string Blue = "#0000FF";
			const string Purple = "#800080";

			NavigateToNestedSample("SafeArea_SoftInput_NestedPage");

			App.WaitForElementWithMessage($"{NamePrefix}Border_Padding");

			var paddingtestRect = App.GetPhysicalRect($"{NamePrefix}Border_Padding");
			var margintestRect = App.GetPhysicalRect($"{NamePrefix}Border_Margin");
			var nonetestRect = App.GetPhysicalRect($"{NamePrefix}Border_None");
			var textBox = App.MarkedAnywhere($"{NamePrefix}TextBox");

			using var initial_screenshot = TakeScreenshot($"SafeArea_SoftInput_No_Keyboard");

			AssertBorderColors(initial_screenshot, paddingtestRect, new[] { Red, Red, Red, Red }, name: nameof(paddingtestRect));
			AssertBorderColors(initial_screenshot, margintestRect, new[] { Blue, Blue, Blue, Blue }, name: nameof(margintestRect));
			AssertBorderColors(initial_screenshot, nonetestRect, new[] { Black, Black, Black, Black }, name: nameof(nonetestRect));

			textBox.FastTap();
			; App.Wait(TimeSpan.FromSeconds(1)); // allow keyboard to fully open

			using var keyboard_open_screenshot = TakeScreenshot($"SafeArea_SoftInput_Open_Keyboard");

			var adjustedPaddingtestRect = App.GetPhysicalRect($"{NamePrefix}Border_Padding");
			var adjustedMargintestRect = App.GetPhysicalRect($"{NamePrefix}Border_Margin");
			var adjustedNonetestRect = App.GetPhysicalRect($"{NamePrefix}Border_None");

			AssertBorderColors(keyboard_open_screenshot, new AppRect(adjustedPaddingtestRect.X, adjustedPaddingtestRect.Y, adjustedPaddingtestRect.Width, paddingtestRect.Height), new[] { Red, Red, Red, Purple }, name: nameof(adjustedPaddingtestRect));
			AssertBorderColors(keyboard_open_screenshot, adjustedMargintestRect, new[] { Blue, Blue, Blue, Blue }, name: nameof(adjustedMargintestRect));
			Assert.AreEqual(nonetestRect.Y, adjustedNonetestRect.Y, message: $"{nameof(adjustedNonetestRect)} is not equal to {nameof(nonetestRect)}");

			App.DismissKeyboard();
			App.Wait(TimeSpan.FromSeconds(1)); // allow keyboard to fully close

			using var after_keyboard_close_screenshot = TakeScreenshot($"SafeArea_SoftInput_Closed_Keyboard");

			AssertBorderColors(after_keyboard_close_screenshot, paddingtestRect, new[] { Red, Red, Red, Red }, name: nameof(paddingtestRect));
			AssertBorderColors(after_keyboard_close_screenshot, margintestRect, new[] { Blue, Blue, Blue, Blue }, name: nameof(margintestRect));
			AssertBorderColors(after_keyboard_close_screenshot, nonetestRect, new[] { Black, Black, Black, Black }, name: nameof(nonetestRect));

			void AssertBorderColors(ScreenshotInfo screenshot, IAppRect rect, string[] colors, string? name = null)
			{
				name = name == null ? string.Empty : name + " ";

				var bitmap = screenshot.GetBitmap();
				var leftpixel = bitmap.GetPixel((int)rect.X + HalfBorderThickness, (int)rect.CenterY);
				var toppixel = bitmap.GetPixel((int)rect.CenterX, (int)rect.Y + HalfBorderThickness);
				var rightpixel = bitmap.GetPixel((int)rect.Right - HalfBorderThickness, (int)rect.CenterY);
				var bottompixel = bitmap.GetPixel((int)rect.CenterX, (int)rect.Bottom - HalfBorderThickness);

				ImageAssert.HasPixels(
					screenshot,
					ExpectedPixels.At(rect.X + HalfBorderThickness, rect.CenterY).Named($"{name}Left Border").Pixel(colors[0]).WithColorTolerance(5),
					ExpectedPixels.At(rect.CenterX, rect.Y + HalfBorderThickness).Named($"{name}Top Border").Pixel(colors[1]).WithColorTolerance(5),
					ExpectedPixels.At(rect.Right - HalfBorderThickness, rect.CenterY).Named($"{name}Right Border").Pixel(colors[2]).WithColorTolerance(5),
					ExpectedPixels.At(rect.CenterX, rect.Bottom - HalfBorderThickness).Named($"{name}Bottom Border").Pixel(colors[3]).WithColorTolerance(5)
				);
			}
		}

		[TestCase("Padding", "Attached", TestName = "When_Override_Insets_Attached_Padding")]
		[TestCase("Padding", "Control", TestName = "When_Override_Insets_Control_Padding")]
		[TestCase("Margin", "Attached", TestName = "When_Override_Insets_Attached_Margin")]
		[TestCase("Margin", "Control", TestName = "When_Override_Insets_Control_Margin")]
		[Test]
		public void When_Override_Insets(string mode, string safeAreaType)
		{
			const int InsetThickness = 20;
			const int HalfBorderThickness = InsetThickness / 2;

			(var mainElement, var sampleName) = safeAreaType.ToLower() switch
			{
				"control" => ("MySafeArea", "SafeArea_Control_NestedPage"),
				"attached" => ("MyBorder", "SafeAreaSamplePage_NestedPage"),
				_ => throw new InvalidOperationException($"{nameof(safeAreaType)} does not support a value of: {safeAreaType}")
			};

			NavigateToNestedSample(sampleName);
			App.WaitForElementWithMessage(mainElement);

			var isMargin = string.Equals(mode, "Margin", StringComparison.OrdinalIgnoreCase);

			App.FastTap($"{mode}Mode");
			App.FastTap($"SetAllTwenty");


			foreach (List<string> maskCombo in Subsets(_availableMasks))
			{
				ClearMasks();

				foreach (var mask in maskCombo)
				{
					App.FastTap($"{mask}{CheckboxSuffix}");
				}

				var testRect = App.GetPhysicalRect(isMargin ? "WrappingGrid" : mainElement);
				using var screenshot = TakeScreenshot($"Inset{string.Join("_", maskCombo)}_Override_{InsetThickness}");

				if (HasMask(Left))
					ImageAssert.HasPixels(
						screenshot,
						ExpectedPixels
							.At(testRect.X + HalfBorderThickness, testRect.Height / 2)
							.Named($"Inset{string.Join("_", maskCombo)}_Override_{InsetThickness}_Left_Inset")
							.Pixel(GetExpectedColor(Left))
							.WithColorTolerance(5)
					);
				if (HasMask(Top))
					ImageAssert.HasPixels(
						screenshot,
						ExpectedPixels
							.At(testRect.Width / 2, testRect.Y + HalfBorderThickness)
							.Named($"Inset{string.Join("_", maskCombo)}_Override_{InsetThickness}_Top_Inset")
							.Pixel(GetExpectedColor(Top))
							.WithColorTolerance(5)
					);
				if (HasMask(Right))
					ImageAssert.HasPixels(
						screenshot,
						ExpectedPixels
							.At(testRect.Width - HalfBorderThickness, testRect.Height / 2)
							.Named($"Inset{string.Join("_", maskCombo)}_Override_{InsetThickness}_Right_Inset")
							.Pixel(GetExpectedColor(Right))
							.WithColorTolerance(5)
					);
				if (HasMask(Bottom))
					ImageAssert.HasPixels(
						screenshot,
						ExpectedPixels
							.At(testRect.Width / 2, testRect.Bottom - HalfBorderThickness)
							.Named($"Inset{string.Join("_", maskCombo)}_Override_{InsetThickness}_Bottom_Inset")
							.Pixel(GetExpectedColor(Bottom))
							.WithColorTolerance(5)
					);

				bool HasMask(string mask) =>
					maskCombo.Contains("All", StringComparer.OrdinalIgnoreCase) ||
					(!mask.Equals("SoftInput", StringComparison.OrdinalIgnoreCase) && maskCombo.Contains("VisibleBounds", StringComparer.OrdinalIgnoreCase)) ||
					maskCombo.Contains(mask, StringComparer.OrdinalIgnoreCase);

				Color GetExpectedColor(string mask) => HasMask(mask) ? GetInsetColor() : Color.Red;

				Color GetInsetColor()
				{
					return string.Equals(mode, "Padding", StringComparison.OrdinalIgnoreCase) ? Color.Yellow : Color.Green;
				}
			}
		}

		private void ClearMasks()
		{
			App.FastTap("ClearMasks");
		}

		private struct ExpectedColors
		{
			//Expected colors at quadrants (left, top, right, bottom)
			public string[] Colors { get; set; }
		}

		public static IEnumerable<IEnumerable<string>> Subsets(IEnumerable<string> source)
		{
			var items = source.ToArray();
			var flags = items
				.Select((x, i) => new { Item = x, Flag = 1 << i })
				.ToArray();
			var max = (int)Math.Pow(2, items.Length);
			for (int count = 0; count < max; count++)
			{
				yield return flags
					.Where(x => ((count & x.Flag) == x.Flag))
					.Select(x => x.Item)
					.ToList();
			}
		}

		private void WaitForIsChecked(QueryEx element, bool expected) =>
			App.WaitForDependencyPropertyValue(element, "IsChecked", expected);
	}
}
