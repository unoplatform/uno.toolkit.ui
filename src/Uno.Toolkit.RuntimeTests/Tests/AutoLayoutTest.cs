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
#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif
using AutoLayoutControl = Uno.Toolkit.UI.AutoLayout;

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal class AutoLayoutTest
{
	[TestMethod]
	public async Task When_SpaceBetween_with_spacing()
	{
		var SUT = new AutoLayout()
		{
			Justify = AutoLayoutJustify.SpaceBetween,
			Padding = new Thickness(10),
			Spacing = 100,
			Width = 190,
			Height = 360,
		};

		var border1 = new Border() { Width = 150, Height = 100 };
		var border2 = new Border() { Width = 150, Height = 100 };
		var border3 = new Border() { Width = 150, Height = 100 };

		AutoLayout.SetCounterAlignment(border1, AutoLayoutAlignment.Start);
		AutoLayout.SetCounterAlignment(border2, AutoLayoutAlignment.Start);
		AutoLayout.SetCounterAlignment(border3, AutoLayoutAlignment.Start);

		SUT.Children.Add(border1);
		SUT.Children.Add(border2);
		SUT.Children.Add(border3);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		var layoutRect0Actual = LayoutInformation.GetLayoutSlot(SUT.Children[0] as FrameworkElement);
		Assert.AreEqual(layoutRect0Actual.Y, 10);
		var layoutRect1Actual = LayoutInformation.GetLayoutSlot(SUT.Children[1] as FrameworkElement);
		Assert.AreEqual(layoutRect1Actual.Y, 135);
		var layoutRect2Actual = LayoutInformation.GetLayoutSlot(SUT.Children[2] as FrameworkElement);
		Assert.AreEqual(layoutRect2Actual.Y, 260);
	}
}

