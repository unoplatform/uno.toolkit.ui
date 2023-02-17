using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Extensions;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Windows.UI;
#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
#endif
using AutoLayoutControl = Uno.Toolkit.UI.AutoLayout;

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal class AutoLayoutTest
{
	[TestMethod]
	[DataRow(Orientation.Vertical, 10, 130, 250)]
	[DataRow(Orientation.Horizontal, 10, 70, 130)]
	public async Task When_SpaceBetween_with_spacing(Orientation orientation, double expectedResult1, double expectedResult2, double expectedResult3)
	{
		var SUT = new AutoLayout()
		{
			Orientation = orientation,
			Justify = AutoLayoutJustify.SpaceBetween,
			Padding = new Thickness(10),
			Spacing = 100,
			Width = 190,
			Height = 360,
			Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0))
		};

		var border1 = new Border() { Width = 50, Height = 100, Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255)) };
		var border2 = new Border() { Width = 50, Height = 100, Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255)) };
		var border3 = new Border() { Width = 50, Height = 100, Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255)) };

		AutoLayout.SetCounterAlignment(border1, AutoLayoutAlignment.Start);
		AutoLayout.SetCounterAlignment(border2, AutoLayoutAlignment.Start);
		AutoLayout.SetCounterAlignment(border3, AutoLayoutAlignment.Start);

		SUT.Children.Add(border1);
		SUT.Children.Add(border2);
		SUT.Children.Add(border3);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		var layoutRect0Actual = LayoutInformation.GetLayoutSlot(SUT.Children[0] as FrameworkElement);
		var layoutRect1Actual = LayoutInformation.GetLayoutSlot(SUT.Children[1] as FrameworkElement);
		var layoutRect2Actual = LayoutInformation.GetLayoutSlot(SUT.Children[2] as FrameworkElement);

		if (orientation is Orientation.Vertical)
		{
			Assert.AreEqual(layoutRect0Actual.Y, expectedResult1);
			Assert.AreEqual(layoutRect1Actual.Y, expectedResult2);
			Assert.AreEqual(layoutRect2Actual.Y, expectedResult3);
		}
		else
		{
			Assert.AreEqual(layoutRect0Actual.X, expectedResult1);
			Assert.AreEqual(layoutRect1Actual.X, expectedResult2);
			Assert.AreEqual(layoutRect2Actual.X, expectedResult3);
		}
	}

	[TestMethod]
	[DataRow(true, Orientation.Vertical, VerticalAlignment.Bottom, HorizontalAlignment.Left, new[] { 10, 10, 10, 10 }, new[] { 108, 0, 0, 10 }, 10, 298, 110, 12, 185)]
	[DataRow(true, Orientation.Vertical, VerticalAlignment.Top, HorizontalAlignment.Left, new[] { 10, 10, 10, 10 }, new[] { 108, 10, 0, 0 }, 10, 12, 110, 12, 185)]
	[DataRow(true, Orientation.Vertical, VerticalAlignment.Top, HorizontalAlignment.Left, new[] { 10, 10, 10, 10 }, new[] { 108, 10, 0, 0 }, -30, 12, 110, 12, 165)]
	[DataRow(true, Orientation.Horizontal, VerticalAlignment.Top, HorizontalAlignment.Left, new[] { 10, 10, 10, 10 }, new[] { 10, 10, 0, 0 }, 10, 12, 12, 12, 105)]
	[DataRow(true, Orientation.Horizontal, VerticalAlignment.Top, HorizontalAlignment.Left, new[] { 10, 10, 10, 10 }, new[] { 10, 10, 0, 0 }, 10, 12, 12, 12, 105)]
	[DataRow(true, Orientation.Horizontal, VerticalAlignment.Top, HorizontalAlignment.Left, new[] { 10, 10, 10, 10 }, new[] { 10, 10, 0, 0 }, -30, 12, 12, 12, 85)]
	[DataRow(false, Orientation.Vertical, VerticalAlignment.Top, HorizontalAlignment.Left, new[] { 10, 10, 10, 10 }, new[] { 108, 10, 0, 0 }, 10, 12, 110, 138, 248)]
	[DataRow(false, Orientation.Horizontal, VerticalAlignment.Top, HorizontalAlignment.Left, new[] { 10, 10, 10, 10 }, new[] { 108, 10, 0, 0 }, 10, 12, 110, 78, 138)]
	[DataRow(false, Orientation.Vertical, VerticalAlignment.Top, HorizontalAlignment.Left, new[] { 10, 10, 10, 10 }, new[] { 108, 10, 0, 0 }, -20, 12, 110, 168, 248)]
	[DataRow(false, Orientation.Horizontal, VerticalAlignment.Top, HorizontalAlignment.Left, new[] { 10, 10, 10, 10 }, new[] { 108, 10, 0, 0 }, -20, 12, 110, 108, 138)]
	public async Task When_AbsolutePosition_WithPadding(bool isStretch, Orientation orientation, VerticalAlignment vAlign, HorizontalAlignment hAlign, int[] padding, int[] margin, int spacing, double expectedY, double expectedX, double rec1expected, double rec2expected)
	{
		var SUT = new AutoLayout()
		{
			Orientation = orientation,
			PrimaryAxisAlignment = AutoLayoutAlignment.End,
			BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
			BorderThickness = new Thickness(2),
			Padding = new Thickness(padding[0], padding[1], padding[2], padding[3]),
			Spacing = spacing,
			Width = 200,
			Height = 360,
		};
		var border1 = new Border()
		{
			//Height = 100,
			Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255)),
		};

		var border2 = new Border()
		{
			//Height = 100,
			Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)),
		};

		var border3 = new Border()
		{
			Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
			Height = 50,
			Width = 50,
			Margin = new Thickness(margin[0], margin[1], margin[2], margin[3]),
			VerticalAlignment = vAlign,
			HorizontalAlignment = hAlign,
		};

		AutoLayout.SetIsIndependentLayout(border3, true);

		if (isStretch)
		{
			AutoLayout.SetPrimaryAlignment(border1, AutoLayoutPrimaryAlignment.Stretch);
			AutoLayout.SetPrimaryAlignment(border2, AutoLayoutPrimaryAlignment.Stretch);
		}
		else
		{
			if (orientation is Orientation.Vertical)
			{
				border1!.Height = 100;
				border2!.Height = 100;
			}
			else
			{
				border1!.Width = 50;
				border2!.Width = 50;
			}
		}

		SUT.Children.Add(border1);
		SUT.Children.Add(border2);
		SUT.Children.Add(border3);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		var border1Transform = (MatrixTransform)border1.TransformToVisual(SUT);
		var border2Transform = (MatrixTransform)border2.TransformToVisual(SUT);
		var border3Transform = (MatrixTransform)border3.TransformToVisual(SUT);

		if (orientation is Orientation.Vertical)
		{
			Assert.AreEqual(border1Transform!.Matrix.OffsetY!, rec1expected);
			Assert.AreEqual(border2Transform!.Matrix.OffsetY!, rec2expected);
		}
		else
		{
			Assert.AreEqual(border1Transform!.Matrix.OffsetX!, rec1expected);
			Assert.AreEqual(border2Transform!.Matrix.OffsetX!, rec2expected);
		}

		Assert.AreEqual(border3Transform!.Matrix.OffsetY!, expectedY);
		Assert.AreEqual(border3Transform!.Matrix.OffsetX!, expectedX);
	}

	[TestMethod]
	[DataRow(true, Orientation.Horizontal, new[] { 10, 10, 10, 10 }, 10, 298, 110, 10, 205)]
	[DataRow(true, Orientation.Vertical, new[] { 10, 10, 10, 10 }, 10, 298, 110, 10, 205)]
	[DataRow(false, Orientation.Vertical, new[] { 10, 10, 10, 10 }, 10, 298, 110, 10, 220)]
	[DataRow(false, Orientation.Horizontal, new[] { 10, 10, 10, 10 }, 10, 298, 110, 10, 220)]
	public async Task When_Padding(bool isStretch, Orientation orientation, int[] padding, int spacing, double expectedY, double expectedX, double rec1expected, double rec2expected)
	{
		var SUT = new AutoLayout()
		{
			Orientation = orientation,
			Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
			Padding = new Thickness(padding[0], padding[1], padding[2], padding[3]),
			Spacing = spacing,
			Width = 400,
			Height = 400,
		};
		var border1 = new Border()
		{
			Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255)),
		};

		var border2 = new Border()
		{
			Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)),
		};

		if (isStretch)
		{
			AutoLayout.SetPrimaryAlignment(border1, AutoLayoutPrimaryAlignment.Stretch);
			AutoLayout.SetPrimaryAlignment(border2, AutoLayoutPrimaryAlignment.Stretch);
		}
		else
		{
			border1!.Width = 200;
			border2!.Width = 200;
			border1!.Height = 200;
			border2!.Height = 200;
		}

		if (orientation is Orientation.Horizontal)
		{
			AutoLayout.SetCounterAlignment(border1, AutoLayoutAlignment.Start);
			AutoLayout.SetCounterAlignment(border2, AutoLayoutAlignment.Start);
		}

		SUT.Children.Add(border1);
		SUT.Children.Add(border2);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		var border1Transform = (MatrixTransform)border1.TransformToVisual(SUT);
		var border2Transform = (MatrixTransform)border2.TransformToVisual(SUT);

		if (orientation is Orientation.Vertical)
		{
			Assert.AreEqual(border1Transform!.Matrix.OffsetY!, rec1expected);
			Assert.AreEqual(border2Transform!.Matrix.OffsetY!, rec2expected);
		}
		else
		{
			Assert.AreEqual(border1Transform!.Matrix.OffsetX!, rec1expected);
			Assert.AreEqual(border2Transform!.Matrix.OffsetX!, rec2expected);
		}
	}

}
