using System;
using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;

namespace Uno.Toolkit.RuntimeTests.Tests;

/// <summary>
/// Verifies that Simple toolkit control resources reference the correct design tokens.
/// Uses reflection to instantiate SimpleToolkitTheme to avoid compile-time dependency
/// on Uno.Simple.WinUI (which conflicts with Uno.Themes.WinUI Density type).
/// These tests only run when the Simple library is available (i.e., in the Simple sample app).
/// </summary>
[TestClass]
public class Given_SimpleDesignTokens
{
	// ─────────────────────────────────────────────────────────────────────
	// Helpers
	// ─────────────────────────────────────────────────────────────────────

	private static readonly Type? s_simpleThemeType =
		Type.GetType("Uno.Toolkit.UI.Simple.SimpleToolkitTheme, Uno.Toolkit.WinUI.Simple");

	private static Grid CreateThemedContainer()
	{
		if (s_simpleThemeType == null)
		{
			Assert.Inconclusive("SimpleToolkitTheme not available — run in Simple sample app");
		}
		var theme = (ResourceDictionary)Activator.CreateInstance(s_simpleThemeType!)!;
		var container = new Grid();
		container.Resources.MergedDictionaries.Add(theme);
		return container;
	}

	private static Grid CreateThemedContainerWithDensity(int densityValue)
	{
		if (s_simpleThemeType == null)
		{
			Assert.Inconclusive("SimpleToolkitTheme not available — run in Simple sample app");
		}
		var theme = (ResourceDictionary)Activator.CreateInstance(s_simpleThemeType!)!;

		// Set DefaultDensity via DependencyProperty (Density: Compact=3, Regular=4, Comfy=5)
		var dpProp = s_simpleThemeType!.GetProperty(
			"DefaultDensityProperty",
			BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
		Assert.IsNotNull(dpProp, "DefaultDensityProperty not found");
		var dp = (DependencyProperty)dpProp!.GetValue(null)!;

		var densityProp = s_simpleThemeType.GetProperty(
			"DefaultDensity",
			BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
		Assert.IsNotNull(densityProp, "DefaultDensity property not found");
		var densityEnumType = densityProp!.PropertyType;
		var densityVal = Enum.ToObject(densityEnumType, densityValue);

		((DependencyObject)theme).SetValue(dp, densityVal);

		var container = new Grid();
		container.Resources.MergedDictionaries.Add(theme);
		return container;
	}

	private static T GetResource<T>(Grid container, string key)
	{
		if (container.Resources.TryGetValue(key, out var value) && value is T typed)
		{
			return typed;
		}

		Assert.Fail($"Resource '{key}' not found or not of type {typeof(T).Name}");
		return default!;
	}

	// ═══════════════════════════════════════════════════════════════════════
	// 1. CARD — padding and corner radius reference design tokens
	// ═══════════════════════════════════════════════════════════════════════

	[TestMethod]
	[RunsOnUIThread]
	public void When_SimpleCardPadding_Then_MatchesSpace600Thickness()
	{
		var container = CreateThemedContainer();
		var padding = GetResource<Thickness>(container, "CardPadding");
		// Space600Thickness at Regular density (base=4): 4×6=24
		Assert.AreEqual(new Thickness(24), padding,
			"CardPadding should be Space600Thickness (24) at Regular density");
	}

	[TestMethod]
	[RunsOnUIThread]
	public void When_SimpleCardCornerRadius_Then_MatchesRadius200CornerRadius()
	{
		var container = CreateThemedContainer();
		var cr = GetResource<CornerRadius>(container, "CardCornerRadius");
		// Radius200CornerRadius at base=4: 4×2=8
		Assert.AreEqual(new CornerRadius(8), cr,
			"CardCornerRadius should be Radius200CornerRadius (8) at base=4");
	}

	// ═══════════════════════════════════════════════════════════════════════
	// 2. CHIP — height, corner radius, padding reference tokens
	// ═══════════════════════════════════════════════════════════════════════

	[TestMethod]
	[RunsOnUIThread]
	public void When_SimpleChipHeight_Then_MatchesControlHeightSmall()
	{
		var container = CreateThemedContainer();
		var height = GetResource<double>(container, "ChipHeight");
		Assert.AreEqual(32.0, height, 0.001, "ChipHeight should be ControlHeightSmall (32)");
	}

	[TestMethod]
	[RunsOnUIThread]
	public void When_SimpleChipCornerRadius_Then_MatchesRadius200CornerRadius()
	{
		var container = CreateThemedContainer();
		var cr = GetResource<CornerRadius>(container, "ChipCornerRadius");
		// Radius200CornerRadius at base=4: 4×2=8
		Assert.AreEqual(new CornerRadius(8), cr,
			"ChipCornerRadius should be Radius200CornerRadius (8) at base=4");
	}

	[TestMethod]
	[RunsOnUIThread]
	public void When_SimpleChipPadding_Then_MatchesSpace200HorizontalThickness()
	{
		var container = CreateThemedContainer();
		var padding = GetResource<Thickness>(container, "ChipPadding");
		// Space200HorizontalThickness at Regular (base=4): Thickness(8,0,8,0)
		Assert.AreEqual(new Thickness(8, 0, 8, 0), padding,
			"ChipPadding should be Space200HorizontalThickness (8,0,8,0) at Regular density");
	}

	// ═══════════════════════════════════════════════════════════════════════
	// 3. NAVIGATION BAR — padding references token
	// ═══════════════════════════════════════════════════════════════════════

	[TestMethod]
	[RunsOnUIThread]
	public void When_SimpleNavigationBarPadding_Then_MatchesSpace100LeftThickness()
	{
		var container = CreateThemedContainer();
		var padding = GetResource<Thickness>(container, "NavigationBarPadding");
		// Space100LeftThickness at Regular (base=4): Thickness(4,0,0,0)
		Assert.AreEqual(new Thickness(4, 0, 0, 0), padding,
			"NavigationBarPadding should be Space100LeftThickness (4,0,0,0) at Regular density");
	}

	// ═══════════════════════════════════════════════════════════════════════
	// 4. TAB BAR — height references design token
	// ═══════════════════════════════════════════════════════════════════════

	[TestMethod]
	[RunsOnUIThread]
	public void When_SimpleTopTabBarHeight_Then_MatchesControlHeightLarge()
	{
		var container = CreateThemedContainer();
		var height = GetResource<double>(container, "TopTabBarHeight");
		Assert.AreEqual(48.0, height, 0.001, "TopTabBarHeight should be ControlHeightLarge (48)");
	}

	// ═══════════════════════════════════════════════════════════════════════
	// 5. FIXED TOKENS — density-invariant across Simple theme
	// ═══════════════════════════════════════════════════════════════════════

	[TestMethod]
	[RunsOnUIThread]
	[DataRow(3)] // Compact
	[DataRow(4)] // Regular
	[DataRow(5)] // Comfy
	public void When_SimpleDensityChanges_Then_FixedTokensAreConstant(int densityValue)
	{
		var container = CreateThemedContainerWithDensity(densityValue);

		// ChipHeight is ControlHeightSmall = 32 at all densities
		Assert.AreEqual(32.0, GetResource<double>(container, "ChipHeight"), 0.001,
			"ChipHeight (ControlHeightSmall) should be 32 at all densities");

		// TopTabBarHeight is ControlHeightLarge = 48 at all densities
		Assert.AreEqual(48.0, GetResource<double>(container, "TopTabBarHeight"), 0.001,
			"TopTabBarHeight (ControlHeightLarge) should be 48 at all densities");
	}
}
