using NUnit.Framework;
using Uno.Toolkit.UITest.Extensions;
using Uno.Toolkit.UITest.Framework;
using Uno.UITest.Helpers.Queries;

namespace Uno.Toolkit.UITest.Behaviors.FlipViewPipsPagerBehavior;

[TestFixture]
public class Given_PipsPagerExtensions : TestBase
{
	protected override string SampleName => "FlipViewExtensions";

	[Test]
	[AutoRetry]
	public void Number_Of_Items_Should_Be_The_Same()
	{
		var (flipViewCounter, pipsPager) = GetPipsPagerAndLabelElements();

		var pipsPagerItemsCount = pipsPager.GetDependencyPropertyValue<int>("NumberOfPages");
		var flipViewCount = flipViewCounter.GetDependencyPropertyValue<string>("Text");

		Assert.AreEqual(pipsPagerItemsCount, int.Parse(flipViewCount));
	}

	[Test]
	[AutoRetry]
	public void Number_Of_Items_Should_Be_The_Same_AfterAddingMoreItems()
	{
		var (flipViewCounter, pipsPager) = GetPipsPagerAndLabelElements();

		var oldPipsPagerItemsCount = pipsPager.GetDependencyPropertyValue<int>("NumberOfPages");
		var oldFlipViewCount = flipViewCounter.GetDependencyPropertyValue<string>("Text");

		Assert.AreEqual(oldPipsPagerItemsCount, int.Parse(oldFlipViewCount));

		App.Tap("AddNewPageButton");

		var newPipsPagerItemsCount = pipsPager.GetDependencyPropertyValue<int>("NumberOfPages");
		var newFlipViewCount = flipViewCounter.GetDependencyPropertyValue<string>("Text");

		Assert.AreEqual(newPipsPagerItemsCount, int.Parse(newFlipViewCount));
		Assert.AreNotEqual(newPipsPagerItemsCount, oldPipsPagerItemsCount);
	}

	[Test]
	[AutoRetry]
	public void Selected_Item_Index_Should_Be_Equal()
	{
		var (flipView, pipsPager) = GetPipsPagerAndFlipViewElements();


		var oldPipsPagerSelectedIndex = pipsPager.GetDependencyPropertyValue<int>("SelectedPageIndex");
		var oldFlipViewSelectedIndex = flipView.GetDependencyPropertyValue<int>("SelectedIndex");

		Assert.AreEqual(oldPipsPagerSelectedIndex, oldFlipViewSelectedIndex);

		App.Tap("BtnNext1");
		App.Tap("BtnNext2");

		var newPipsPagerSelectedIndex = pipsPager.GetDependencyPropertyValue<int>("SelectedPageIndex");
		var newFlipViewSelectedIndex = flipView.GetDependencyPropertyValue<int>("SelectedIndex");

		Assert.AreEqual(newPipsPagerSelectedIndex, newFlipViewSelectedIndex);
		Assert.AreNotEqual(newPipsPagerSelectedIndex, oldPipsPagerSelectedIndex);
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

		return (flipView, pipsPager);
	}
}
