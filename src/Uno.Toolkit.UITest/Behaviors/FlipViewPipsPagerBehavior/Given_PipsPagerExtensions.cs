using NUnit.Framework;
using Uno.Toolkit.UITest.Extensions;
using Uno.UITest.Helpers.Queries;

namespace Uno.Toolkit.UITest.Behaviors.FlipViewPipsPagerBehavior;

public class Given_PipsPagerExtensions : TestBase
{
	protected override string SampleName => "FlipViewExtensions";

	[Test]
	public void Number_Of_Items_Should_Be_The_Same()
	{
		var (flipViewCounter, pipsPager) = GetPipsPagerAndLabelElements();

		var pipsPagerItemsCount = pipsPager.GetDependencyPropertyValue<int>("NumberOfPages");
		var flipViewCount = flipViewCounter.GetDependencyPropertyValue<string>("Text");

		Assert.AreEqual(pipsPagerItemsCount, int.Parse(flipViewCount));
	}

	[Test]
	public void Number_Of_Items_Should_Be_The_Same_AfterAddingMoreItems()
	{
		var (flipViewCounter, pipsPager) = GetPipsPagerAndLabelElements();

		App.Tap("AddNewPageButton");

		var pipsPagerItemsCount = pipsPager.GetDependencyPropertyValue<int>("NumberOfPages");
		var flipViewCount = flipViewCounter.GetDependencyPropertyValue<string>("Text");

		Assert.AreEqual(pipsPagerItemsCount, int.Parse(flipViewCount));
	}

	[Test]
	public void Selected_Item_Index_Should_Be_Equal()
	{
		var (flipView, pipsPager) = GetPipsPagerAndFlipViewElements();

		App.Tap("BtnNext1");
		App.Tap("BtnNext2");

		var pipsPagerSelectedIndex = pipsPager.GetDependencyPropertyValue<int>("SelectedPageIndex");
		var flipViewSelectedIndex = flipView.GetDependencyPropertyValue<int>("SelectedIndex");

		Assert.AreEqual(pipsPagerSelectedIndex, flipViewSelectedIndex);
	}

	(QueryEx flipViewCounter, QueryEx pipsPager) GetPipsPagerAndLabelElements()
	{
		var flipViewCounter = App.MarkedAnywhere("flipViewItems");
		var pipsPager = App.MarkedAnywhere("pipsPager");

		Assert.IsNotNull(flipViewCounter);
		Assert.IsNotNull(pipsPager);

		return (flipViewCounter, pipsPager);
	}


	(QueryEx flipView, QueryEx pipsPager) GetPipsPagerAndFlipViewElements()
	{
		var flipView = App.MarkedAnywhere("flipView");
		var pipsPager = App.MarkedAnywhere("pipsPager");

		Assert.IsNotNull(flipView);
		Assert.IsNotNull(pipsPager);

		return (flipView, pipsPager);
	}
}
