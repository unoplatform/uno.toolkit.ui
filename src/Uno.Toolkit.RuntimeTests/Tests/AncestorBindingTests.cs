using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.UI.RuntimeTests;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.Extensions;
using Uno.Toolkit.RuntimeTests.Tests.TestPages;


#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal class AncestorBindingTests
{
	[TestMethod]
	public async Task Ancestor_TopLevel_PageBinding()
	{
		var setup = new AncestorBindingTest();
		await UnitTestUIContentHelperEx.SetContentAndWait(setup);

		var sut = setup.GetFirstDescendant<TextBlock>(x => x.Name == "TopLevelTextBlock") ?? throw new Exception("Failed to find TopLevelTextBlock");
		Assert.AreEqual(sut.Text, setup.Tag);
	}

	[TestMethod]
	public async Task Ancestor_Nested_PageBinding()
	{
		var setup = new AncestorBindingTest();
		await UnitTestUIContentHelperEx.SetContentAndWait(setup);

		var lv = setup.GetFirstDescendant<ListView>(x => x.Name == "TopLevelListView") ?? throw new Exception("Failed to find TopLevelListView");
		var container = lv.ContainerFromIndex(0);
		var sut = (container as FrameworkElement)?.GetFirstDescendant<TextBlock>(x => x.Name == "NestedLvTextBlock1") ?? throw new Exception("Failed to find NestedLvTextBlock1");

		Assert.AreEqual(sut.Text, setup.Tag);
	}

	[TestMethod]
	public async Task Ancestor_Nested_LvBinding()
	{
		var setup = new AncestorBindingTest();
		await UnitTestUIContentHelperEx.SetContentAndWait(setup);

		var lv = setup.GetFirstDescendant<ListView>(x => x.Name == "TopLevelListView") ?? throw new Exception("Failed to find TopLevelListView");
		var container = lv.ContainerFromIndex(0);
		var sut = (container as FrameworkElement)?.GetFirstDescendant<TextBlock>(x => x.Name == "NestedLvTextBlock2") ?? throw new Exception("Failed to find NestedLvTextBlock2");
		Assert.AreEqual(sut.Text, lv.Tag);
	}

	[TestMethod]
	public async Task Ancestor_Converter_InitialValue()
	{
		var setup = new AncestorBindingTest();
		await UnitTestUIContentHelperEx.SetContentAndWait(setup);

		var host = setup.GetFirstDescendantOrThrow<CheckBox>("ConverterTestHost");
		var sut = setup.GetFirstDescendantOrThrow<Border>("ConverterTestInnerBorder");

		Assert.AreEqual(false, host.IsChecked);
		Assert.AreEqual(Visibility.Collapsed, sut.Visibility);
	}

	[TestMethod]
	public async Task Ancestor_Converter_UpdatedValue()
	{
		var setup = new AncestorBindingTest();
		await UnitTestUIContentHelperEx.SetContentAndWait(setup);

		var host = setup.GetFirstDescendantOrThrow<CheckBox>("ConverterTestHost");
		var sut = setup.GetFirstDescendantOrThrow<Border>("ConverterTestInnerBorder");

		Assert.AreEqual(false, host.IsChecked);
		Assert.AreEqual(Visibility.Collapsed, sut.Visibility);

		host.IsChecked = true;
		await UnitTestsUIContentHelper.WaitForIdle();

		Assert.AreEqual(true, host.IsChecked);
		Assert.AreEqual(Visibility.Visible, sut.Visibility);
	}
}
