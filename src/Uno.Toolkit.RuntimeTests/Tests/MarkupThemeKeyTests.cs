using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.UI.Material;
using Uno.Toolkit.UI.Simple;
using Uno.UI.RuntimeTests;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
public class MarkupThemeKeyTests
{
	private const string ResourceKeyDefinitionAttributeName = "ResourceKeyDefinitionAttribute";

	[TestMethod]
	[DataRow("Material")]
	[DataRow("Simple")]
	public void When_ThemeLoaded_Every_Markup_StyleKey_IsDefined(string themeName)
	{
		ResourceDictionary theme = themeName switch
		{
			"Material" => new MaterialToolkitTheme(),
			"Simple" => new SimpleToolkitTheme(),
			_ => throw new ArgumentOutOfRangeException(nameof(themeName), themeName, null),
		};

		var keys = GetMarkupStyleKeys();
		Assert.IsTrue(keys.Count > 0, "No Style keys were found on the C# Markup theme types.");

		// Check that the key is defined rather than its resolved value: in Debug the style files are not
		// merged, so an unprefixed alias in _Common.xaml only resolves once the theme is in the app resources.
		var missing = keys
			.Where(key => !theme.ContainsKey(key))
			.ToArray();

		Assert.AreEqual(0, missing.Length, $"C# Markup Style keys missing from the {themeName} toolkit theme: {string.Join(", ", missing)}");
	}

	// Reads the keys from the [ResourceKeyDefinition(typeof(Style), "<key>", ...)] attributes
	// on Uno.Toolkit.WinUI.Markup, so new Markup keys are covered without updating this test.
	private static IReadOnlyCollection<string> GetMarkupStyleKeys()
	{
		const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		return typeof(Uno.Toolkit.UI.Markup.ToolkitTheme).Assembly
			.GetTypes()
			.SelectMany(type => type.GetProperties(Flags))
			.SelectMany(property => property.GetCustomAttributesData())
			.Where(attribute =>
				attribute.AttributeType.Name == ResourceKeyDefinitionAttributeName &&
				attribute.ConstructorArguments.Count >= 2 &&
				attribute.ConstructorArguments[0].Value is Type resourceType &&
				resourceType == typeof(Style))
			.Select(attribute => attribute.ConstructorArguments[1].Value)
			.OfType<string>()
			.Distinct(StringComparer.Ordinal)
			.OrderBy(key => key, StringComparer.Ordinal)
			.ToArray();
	}
}
