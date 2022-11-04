using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
public class FlipViewExtensionsTests
{
	[TestMethod]
	public async Task AssingFlipViewExtensions()
	{
		var flipView = CreateFlipView();

		// Until we port the WindowHelper
		await Task.Delay(2_000);

		var firstItem = flipView.Items[0];
		var firstButton = (Button)((Grid)firstItem).Children[2];

	}


	static FlipView CreateFlipView()
	{
		var flipView = new FlipView();
		AddContentToFlipView(flipView);

		return flipView;
	}


	static void AddContentToFlipView(FlipView flipView, int itemsToAdd = 1)
	{
		for (int i = 0; i < itemsToAdd; i++)
		{
			var grid = new Grid();

			var bt1 = new Button
			{
				Content = "Previous",
				HorizontalAlignment = HorizontalAlignment.Left
			};

			FlipViewExtensions.SetPrevious(bt1, flipView);


			var bt2 = new Button
			{
				Content = "Next",
				HorizontalAlignment = HorizontalAlignment.Right
			};
			FlipViewExtensions.SetNext(bt2, flipView);

			grid.Children.Add(bt1);
			grid.Children.Add(bt2);

			flipView.Items.Add(grid);
		}
	}
}
