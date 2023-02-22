using NUnit.Framework;
using Uno.Toolkit.UITest.Extensions;
using Uno.Toolkit.UITest.Framework;
using Uno.UITest.Helpers.Queries;

namespace Uno.Toolkit.UITest.Behaviors.FlipViewPipsPagerBehavior;

[TestFixture]
internal class Given_FlipViewExtensions : TestBase
{
	protected override string SampleName => "FlipViewExtensions";

	[Test]
	[AutoRetry]
	public void Tapping_On_Next_Button_Navigate_To_Other_Content()
	{
		var flipView = App.MarkedAnywhere("flipView");

		App.WaitForElement(flipView);

		var oldFlipViewSelectedIndex = flipView.GetDependencyPropertyValue<int>("SelectedIndex");
		Assert.AreEqual(0, oldFlipViewSelectedIndex);

		App.Tap("BtnNext1");
		App.Tap("BtnNext2");

		var newFlipViewSelectedIndex = flipView.GetDependencyPropertyValue<int>("SelectedIndex");

		Assert.AreEqual(2, newFlipViewSelectedIndex);
	}

	[Test]
	[AutoRetry]
	public void Tapping_On_Privous_Button_Navigate_Back()
	{
		var flipView = App.MarkedAnywhere("flipView");

		var oldFlipViewSelectedIndex = flipView.GetDependencyPropertyValue<int>("SelectedIndex");
		Assert.AreEqual(0, oldFlipViewSelectedIndex);

		App.Tap("BtnNext1");
		App.Tap("BtnPrevious2");

		var newFlipViewSelectedIndex = flipView.GetDependencyPropertyValue<int>("SelectedIndex");

		Assert.AreEqual(oldFlipViewSelectedIndex, newFlipViewSelectedIndex);
	}
}
