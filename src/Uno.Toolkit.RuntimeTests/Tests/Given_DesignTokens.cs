using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.Toolkit.UI.Material;
using Uno.UI.RuntimeTests;

namespace Uno.Toolkit.RuntimeTests.Tests;

/// <summary>
/// Verifies that toolkit control resources reference the correct design tokens
/// via <see cref="MaterialToolkitTheme"/>.
/// Note: Toolkit XAML resources using StaticResource aliases resolve at parse time
/// against app-level resources, so these tests verify Regular density (base=4, corner=4)
/// which matches the app theme configuration.
/// </summary>
[TestClass]
public class Given_DesignTokens
{
	// ─────────────────────────────────────────────────────────────────────
	// Helpers
	// ─────────────────────────────────────────────────────────────────────

	private static (Grid container, MaterialToolkitTheme theme) CreateThemedContainer()
	{
		var theme = new MaterialToolkitTheme();
		var container = new Grid();
		container.Resources.MergedDictionaries.Add(theme);
		return (container, theme);
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
	public void When_CardPadding_Then_MatchesSpace400Thickness()
	{
		var (container, _) = CreateThemedContainer();
		var padding = GetResource<Thickness>(container, "CardPadding");
		// Space400Thickness at Regular density (base=4): 4×4=16
		Assert.AreEqual(new Thickness(16), padding,
			"CardPadding should be Space400Thickness (16) at Regular density");
	}

	[TestMethod]
	[RunsOnUIThread]
	public void When_CardCornerRadius_Then_MatchesRadius300CornerRadius()
	{
		var (container, _) = CreateThemedContainer();
		var cr = GetResource<CornerRadius>(container, "CardCornerRadius");
		// Radius300CornerRadius at base=4: 4×3=12
		Assert.AreEqual(new CornerRadius(12), cr,
			"CardCornerRadius should be Radius300CornerRadius (12) at base=4");
	}

	[TestMethod]
	[RunsOnUIThread]
	public void When_CardElevationMargin_Then_MatchesSpace150Thickness()
	{
		var (container, _) = CreateThemedContainer();
		var margin = GetResource<Thickness>(container, "CardElevationMargin");
		// Space150Thickness at Regular density (base=4): 4×1.5=6
		Assert.AreEqual(new Thickness(6), margin,
			"CardElevationMargin should be Space150Thickness (6) at Regular density");
	}

	// ═══════════════════════════════════════════════════════════════════════
	// 2. CHIP — height, corner radius, padding, margins reference tokens
	// ═══════════════════════════════════════════════════════════════════════

	[TestMethod]
	[RunsOnUIThread]
	public void When_ChipHeight_Then_MatchesControlHeightSmall()
	{
		var (container, _) = CreateThemedContainer();
		var height = GetResource<double>(container, "ChipHeight");
		Assert.AreEqual(32.0, height, 0.001, "ChipHeight should be ControlHeightSmall (32)");
	}

	[TestMethod]
	[RunsOnUIThread]
	public void When_ChipCornerRadius_Then_MatchesRadius200CornerRadius()
	{
		var (container, _) = CreateThemedContainer();
		var cr = GetResource<CornerRadius>(container, "ChipCornerRadius");
		// Radius200CornerRadius at base=4: 4×2=8
		Assert.AreEqual(new CornerRadius(8), cr,
			"ChipCornerRadius should be Radius200CornerRadius (8) at base=4");
	}

	[TestMethod]
	[RunsOnUIThread]
	public void When_ChipPadding_Then_MatchesSpace200HorizontalThickness()
	{
		var (container, _) = CreateThemedContainer();
		var padding = GetResource<Thickness>(container, "ChipPadding");
		// Space200HorizontalThickness at Regular (base=4): Thickness(8,0,8,0)
		Assert.AreEqual(new Thickness(8, 0, 8, 0), padding,
			"ChipPadding should be Space200HorizontalThickness (8,0,8,0) at Regular density");
	}

	[TestMethod]
	[RunsOnUIThread]
	public void When_ChipElevationMargin_Then_MatchesSpace100Thickness()
	{
		var (container, _) = CreateThemedContainer();
		var margin = GetResource<Thickness>(container, "ChipElevationMargin");
		// Space100Thickness at Regular (base=4): Thickness(4)
		Assert.AreEqual(new Thickness(4), margin,
			"ChipElevationMargin should be Space100Thickness (4) at Regular density");
	}

	[TestMethod]
	[RunsOnUIThread]
	public void When_ChipSize_Then_MatchesSpace300()
	{
		var (container, _) = CreateThemedContainer();
		var size = GetResource<double>(container, "ChipSize");
		// Space300 at Regular (base=4): 4×3=12
		Assert.AreEqual(12.0, size, 0.001,
			"ChipSize should be Space300 (12) at Regular density");
	}

	[TestMethod]
	[RunsOnUIThread]
	public void When_ChipContentMinHeight_Then_MatchesSpace500()
	{
		var (container, _) = CreateThemedContainer();
		var minHeight = GetResource<double>(container, "ChipContentMinHeight");
		// Space500 at Regular (base=4): 4×5=20
		Assert.AreEqual(20.0, minHeight, 0.001,
			"ChipContentMinHeight should be Space500 (20) at Regular density");
	}

	// ═══════════════════════════════════════════════════════════════════════
	// 3. NAVIGATION BAR — heights and icon sizes reference tokens
	// ═══════════════════════════════════════════════════════════════════════

	[TestMethod]
	[RunsOnUIThread]
	public void When_NavigationBarHeight_Then_MatchesSpace1600()
	{
		var (container, _) = CreateThemedContainer();
		// Space1600 at Regular (base=4): 4×16=64
		var height = GetResource<double>(container, "MaterialNavigationBarHeight");
		Assert.AreEqual(64.0, height, 0.001,
			"MaterialNavigationBarHeight should be Space1600 (64) at Regular density");

		var xamlHeight = GetResource<double>(container, "MaterialXamlNavigationBarHeight");
		Assert.AreEqual(64.0, xamlHeight, 0.001,
			"MaterialXamlNavigationBarHeight should be Space1600 (64) at Regular density");
	}

	[TestMethod]
	[RunsOnUIThread]
	public void When_NavigationBarContentMargin_Then_MatchesSpace400HorizontalThickness()
	{
		var (container, _) = CreateThemedContainer();
		var margin = GetResource<Thickness>(container, "MaterialNavigationBarContentMargin");
		// Space400HorizontalThickness at Regular (base=4): Thickness(16,0,16,0)
		Assert.AreEqual(new Thickness(16, 0, 16, 0), margin,
			"MaterialNavigationBarContentMargin should be Space400HorizontalThickness (16,0,16,0)");
	}

	[TestMethod]
	[RunsOnUIThread]
	public void When_NavBarIconSizes_Then_MatchFixedTokens()
	{
		var (container, _) = CreateThemedContainer();
		Assert.AreEqual(16.0, GetResource<double>(container, "NavBarAppBarButtonContentHeight"), 0.001,
			"NavBarAppBarButtonContentHeight should be IconSizeSmall (16)");
		Assert.AreEqual(24.0, GetResource<double>(container, "NavBarMainCommandAppBarButtonContentHeight"), 0.001,
			"NavBarMainCommandAppBarButtonContentHeight should be IconSizeMedium (24)");
	}

	[TestMethod]
	[RunsOnUIThread]
	public void When_NavigationBarPadding_Then_MatchesSpace100LeftThickness()
	{
		var (container, _) = CreateThemedContainer();
		var padding = GetResource<Thickness>(container, "NavigationBarPadding");
		// Space100LeftThickness at Regular (base=4): Thickness(4,0,0,0)
		Assert.AreEqual(new Thickness(4, 0, 0, 0), padding,
			"NavigationBarPadding should be Space100LeftThickness (4,0,0,0) at Regular density");
	}

	[TestMethod]
	[RunsOnUIThread]
	public void When_NavBarCompactHeight_Then_MatchesSpace1600()
	{
		var (container, _) = CreateThemedContainer();
		Assert.AreEqual(64.0, GetResource<double>(container, "NavBarAppBarThemeCompactHeight"), 0.001,
			"NavBarAppBarThemeCompactHeight should be Space1600 (64)");
	}

	// ═══════════════════════════════════════════════════════════════════════
	// 4. TAB BAR — heights reference design tokens
	// ═══════════════════════════════════════════════════════════════════════

	[TestMethod]
	[RunsOnUIThread]
	public void When_TopTabBarHeight_Then_MatchesControlHeightLarge()
	{
		var (container, _) = CreateThemedContainer();
		var height = GetResource<double>(container, "TopTabBarHeight");
		Assert.AreEqual(48.0, height, 0.001, "TopTabBarHeight should be ControlHeightLarge (48)");
	}

	[TestMethod]
	[RunsOnUIThread]
	public void When_NavigationTabBarActiveIndicatorWidth_Then_MatchesSpace1600()
	{
		var (container, _) = CreateThemedContainer();
		var width = GetResource<double>(container, "NavigationTabBarItemActiveIndicatorWidth");
		// Space1600 at Regular (base=4): 4×16=64
		Assert.AreEqual(64.0, width, 0.001,
			"NavigationTabBarItemActiveIndicatorWidth should be Space1600 (64) at Regular density");
	}

	[TestMethod]
	[RunsOnUIThread]
	public void When_NavigationTabBarActiveIndicatorHeight_Then_MatchesControlHeightSmall()
	{
		var (container, _) = CreateThemedContainer();
		var height = GetResource<double>(container, "NavigationTabBarItemActiveIndicatorHeight");
		Assert.AreEqual(32.0, height, 0.001,
			"NavigationTabBarItemActiveIndicatorHeight should be ControlHeightSmall (32)");
	}

	[TestMethod]
	[RunsOnUIThread]
	public void When_TabBarCornerRadii_Then_MatchTokens()
	{
		var (container, _) = CreateThemedContainer();
		// Radius400CornerRadius at base=4: 4×4=16
		Assert.AreEqual(new CornerRadius(16),
			GetResource<CornerRadius>(container, "NavigationTabBarItemActiveIndicatorCornerRadius"),
			"NavigationTabBarItemActiveIndicatorCornerRadius should be Radius400CornerRadius (16)");
		Assert.AreEqual(new CornerRadius(16),
			GetResource<CornerRadius>(container, "FabTabBarItemCornerRadius"),
			"FabTabBarItemCornerRadius should be Radius400CornerRadius (16)");
		// Radius200CornerRadius at base=4: 4×2=8
		Assert.AreEqual(new CornerRadius(8),
			GetResource<CornerRadius>(container, "NavigationTabBarItemLargeBadgeCornerRadius"),
			"NavigationTabBarItemLargeBadgeCornerRadius should be Radius200CornerRadius (8)");
	}

	[TestMethod]
	[RunsOnUIThread]
	public void When_TabBarBadgeDimensions_Then_MatchTokens()
	{
		var (container, _) = CreateThemedContainer();
		// Space150 at Regular (base=4): 4×1.5=6
		Assert.AreEqual(6.0, GetResource<double>(container, "NavigationTabBarItemSmallBadgeHeight"), 0.001);
		Assert.AreEqual(6.0, GetResource<double>(container, "NavigationTabBarItemSmallBadgeWidth"), 0.001);
		// Space400 at Regular (base=4): 4×4=16
		Assert.AreEqual(16.0, GetResource<double>(container, "NavigationTabBarItemLargeBadgeHeight"), 0.001);
		Assert.AreEqual(16.0, GetResource<double>(container, "NavigationTabBarItemLargeBadgeMinWidth"), 0.001);
	}

	[TestMethod]
	[RunsOnUIThread]
	public void When_TabBarFabResources_Then_MatchTokens()
	{
		var (container, _) = CreateThemedContainer();
		// FabTabBarItemContentWidthOrHeight = IconSizeSmall = 16
		Assert.AreEqual(16.0, GetResource<double>(container, "FabTabBarItemContentWidthOrHeight"), 0.001);
		// FabTabBarItemIconTextPadding = Space300 at Regular (base=4): 12
		Assert.AreEqual(12.0, GetResource<double>(container, "FabTabBarItemIconTextPadding"), 0.001);
		// FabTabBarItemPadding = Space500Thickness at Regular (base=4): Thickness(20)
		Assert.AreEqual(new Thickness(20), GetResource<Thickness>(container, "FabTabBarItemPadding"));
	}

	// ═══════════════════════════════════════════════════════════════════════
	// 4b. DIVIDER — sub-header margin references design tokens
	// ═══════════════════════════════════════════════════════════════════════

	[TestMethod]
	[RunsOnUIThread]
	public void When_DividerSubHeaderMargin_Then_MatchesSpace100TopThickness()
	{
		var (container, _) = CreateThemedContainer();
		var margin = GetResource<Thickness>(container, "DividerSubHeaderMargin");
		// Space100TopThickness at Regular (base=4): Thickness(0,4,0,0)
		Assert.AreEqual(new Thickness(0, 4, 0, 0), margin,
			"DividerSubHeaderMargin should be Space100TopThickness (0,4,0,0)");
	}

	// ═══════════════════════════════════════════════════════════════════════
	// 5. FIXED TOKENS — control heights and icon sizes are density-invariant
	// ═══════════════════════════════════════════════════════════════════════

	[TestMethod]
	[RunsOnUIThread]
	[DataRow(3)] // Compact
	[DataRow(4)] // Regular
	[DataRow(5)] // Comfy
	public void When_DensityChanges_Then_FixedToolkitTokensAreConstant(int densityValue)
	{
		var theme = new MaterialToolkitTheme { DefaultDensity = (Uno.Themes.Density)densityValue };
		var container = new Grid();
		container.Resources.MergedDictionaries.Add(theme);

		// ChipHeight is ControlHeightSmall = 32 at all densities
		Assert.AreEqual(32.0, GetResource<double>(container, "ChipHeight"), 0.001,
			"ChipHeight (ControlHeightSmall) should be 32 at all densities");

		// TopTabBarHeight is ControlHeightLarge = 48 at all densities
		Assert.AreEqual(48.0, GetResource<double>(container, "TopTabBarHeight"), 0.001,
			"TopTabBarHeight (ControlHeightLarge) should be 48 at all densities");

		// Icon sizes
		Assert.AreEqual(16.0, GetResource<double>(container, "NavBarAppBarButtonContentHeight"), 0.001,
			"NavBarAppBarButtonContentHeight (IconSizeSmall) should be 16 at all densities");
		Assert.AreEqual(24.0, GetResource<double>(container, "NavBarMainCommandAppBarButtonContentHeight"), 0.001,
			"NavBarMainCommandAppBarButtonContentHeight (IconSizeMedium) should be 24 at all densities");
	}

	// ═══════════════════════════════════════════════════════════════════════
	// 6. INTEGRATION — rendered controls use design token values
	// ═══════════════════════════════════════════════════════════════════════

	[TestMethod]
	[RunsOnUIThread]
	public async Task When_CardRendered_Then_PaddingAndCornerRadiusFromTokens()
	{
		var card = XamlHelper.LoadXaml<Card>("""
			<utu:Card Style="{StaticResource MaterialFilledCardStyle}" />
		""");

		await UnitTestUIContentHelperEx.SetContentAndWait(card);

		// CardPadding = Space400Thickness at Regular (base=4): 16
		Assert.AreEqual(new Thickness(16), card.Padding,
			"Rendered Card.Padding should be Space400Thickness (16)");

		// CardCornerRadius = Radius300CornerRadius at base=4: 12
		Assert.AreEqual(new CornerRadius(12), card.CornerRadius,
			"Rendered Card.CornerRadius should be Radius300CornerRadius (12)");
	}

	[TestMethod]
	[RunsOnUIThread]
	public async Task When_ChipRendered_Then_CornerRadiusAndPaddingFromTokens()
	{
		var chip = XamlHelper.LoadXaml<Chip>("""
			<utu:Chip Style="{StaticResource MaterialChipStyle}" Content="Test" />
		""");

		await UnitTestUIContentHelperEx.SetContentAndWait(chip);

		// ChipCornerRadius = Radius200CornerRadius at base=4: 8
		Assert.AreEqual(new CornerRadius(8), chip.CornerRadius,
			"Rendered Chip.CornerRadius should be Radius200CornerRadius (8)");

		// ChipPadding = Space200HorizontalThickness at Regular (base=4): (8,0,8,0)
		Assert.AreEqual(new Thickness(8, 0, 8, 0), chip.Padding,
			"Rendered Chip.Padding should be Space200HorizontalThickness (8,0,8,0)");
	}

	[TestMethod]
	[RunsOnUIThread]
	public async Task When_TopTabBarRendered_Then_MinHeightFromToken()
	{
		var tabBar = XamlHelper.LoadXaml<TabBar>("""
			<utu:TabBar Style="{StaticResource MaterialTopTabBarStyle}">
				<utu:TabBarItem Content="Tab1" />
			</utu:TabBar>
		""");

		await UnitTestUIContentHelperEx.SetContentAndWait(tabBar);

		// TopTabBarHeight = ControlHeightLarge = 48
		Assert.AreEqual(48.0, tabBar.MinHeight, 0.001,
			"Rendered TabBar.MinHeight should be ControlHeightLarge (48)");
	}
}

