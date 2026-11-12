using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.UI.Material;
using Uno.UI.RuntimeTests;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

// These tests cover the [Deprecated] dictionaries on purpose: they still ship and must keep loading.
#pragma warning disable CS0618

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
public class MaterialToolkitResourcesTests
{
	// Keys that only exist in the v1 (Material Design 2) or v2 (Material Design 3) style set.
	private const string V1OnlyStyleKey = "MaterialFilledActionChipStyle";
	private const string V2OnlyStyleKey = "MaterialAssistChipStyle";

	[TestMethod]
	public void MaterialToolkitResourcesV1_Loads_V1_Styles()
	{
		var resources = new MaterialToolkitResourcesV1();

		AssertHasStyle(resources, V1OnlyStyleKey);
	}

	[TestMethod]
	public void MaterialToolkitResourcesV2_Loads_V2_Styles()
	{
		var resources = new MaterialToolkitResourcesV2();

		AssertHasStyle(resources, V2OnlyStyleKey);
	}

	[TestMethod]
	public void MaterialToolkitResources_Loads_V2_Styles()
	{
		var resources = new MaterialToolkitResources();

		AssertHasStyle(resources, V2OnlyStyleKey);
	}

	private static void AssertHasStyle(ResourceDictionary resources, string key)
	{
		Assert.IsTrue(resources.TryGetValue(key, out var value), $"'{key}' was not found.");
		Assert.IsInstanceOfType(value, typeof(Style));
	}
}
