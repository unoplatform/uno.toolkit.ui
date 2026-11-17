using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.Toolkit.UI.Material;
using Uno.Toolkit.UI.Simple;
using Uno.UI.RuntimeTests;
using Windows.Foundation;

namespace Uno.Toolkit.RuntimeTests.Tests;

/// <summary>
/// Covers the default (Fluent) TabBar styles shipped by <see cref="ToolkitResources"/>,
/// scoped to the test root so the host app's design system does not supply the implicit styles.
/// The host app may still override the WinUI brushes these styles use, so colors are only compared with each other.
/// </summary>
[TestClass]
[RunsOnUIThread]
public sealed class TabBarDefaultStyleTests
{
	private const string TopStyle = "DefaultTopTabBarStyle";
	private const string SegmentedStyle = "DefaultSegmentedTabBarStyle";
	private const string BottomStyle = "DefaultBottomTabBarStyle";
	private const string VerticalStyle = "DefaultVerticalTabBarStyle";
	private const string IconPresenterName = "Icon";
	private const string ContentPresenterName = "ContentPresenter";
	private const string BaseDefaultStyle = "DefaultTabBarStyle";
	private const string SelectionIndicatorName = "SelectionIndicator";

	// The height TabBar had before the Fluent top styles; they must be more compact than a bottom bar.
	private const double BottomBarHeight = 64;

	[TestCleanup]
	public async Task Cleanup()
	{
		UnitTestsUIContentHelper.Content = null;
		await UnitTestsUIContentHelper.WaitForIdle();
	}

	[TestMethod]
	[DataRow(null)]
	[DataRow(BaseDefaultStyle)]
	[DataRow(TopStyle)]
	[DataRow(SegmentedStyle)]
	[DataRow(BottomStyle)]
	[DataRow(VerticalStyle)]
	public async Task When_ItemSelected_OnlyThatItemIsMarked(string? styleKey)
	{
		var tabBar = CreateTabBar(selectedIndex: 1);

		await Load(tabBar, styleKey);

		for (var i = 0; i < 3; i++)
		{
			var item = Item(tabBar, i);
			var pill = Find<Border>(item, SelectionIndicatorName);
			Assert.AreEqual(i == 1 ? 1d : 0d, pill.Opacity, $"Selection indicator of item {i}");
			var bounds = Bounds(pill, item);
			Assert.IsTrue(bounds.Left >= 0 && bounds.Right <= item.ActualWidth && bounds.Top >= 0 && bounds.Bottom <= item.ActualHeight, $"The indicator ({bounds}) must sit inside item {i}.");
		}

		tabBar.SelectedIndex = 2;
		await UnitTestsUIContentHelper.WaitForIdle();

		Assert.AreEqual(0d, Find<Border>(Item(tabBar, 1), SelectionIndicatorName).Opacity, "Deselected item");
		Assert.AreEqual(1d, Find<Border>(Item(tabBar, 2), SelectionIndicatorName).Opacity, "Newly selected item");
	}

	[TestMethod]
	public async Task When_PaddingIsSet_IndicatorStaysCenteredOnItem()
	{
		var tabBar = CreateTabBar(selectedIndex: 2);
		tabBar.Padding = new Thickness(24, 0, 0, 0);

		await Load(tabBar, TopStyle);

		var item = Item(tabBar, 2);
		var pill = Bounds(Find<Border>(item, SelectionIndicatorName), tabBar);
		var cell = Bounds(item, tabBar);
		Assert.AreEqual(cell.Left + cell.Width / 2, pill.Left + pill.Width / 2, 0.5);
	}

	[TestMethod]
	public async Task When_SelectionIndicatorContentIsSet_ItIsDisplayed()
	{
		var custom = new Border { Background = new SolidColorBrush(Microsoft.UI.Colors.Purple) };
		var tabBar = CreateTabBar(selectedIndex: 0);
		tabBar.SelectionIndicatorContent = custom;

		await Load(tabBar, styleKey: null);

		Assert.IsTrue(tabBar.GetDescendants().Contains(custom), "The consumer's indicator must be in the visual tree.");
		Assert.IsTrue(custom.ActualWidth > 0 && custom.ActualHeight > 0, "The consumer's indicator must be laid out.");
	}

	[TestMethod]
	[DataRow(TopStyle)]
	[DataRow(SegmentedStyle)]
	public async Task When_TopStyle_IconSitsBesideLabel(string styleKey)
	{
		var tabBar = CreateTabBar();

		await Load(tabBar, styleKey);

		var item = Item(tabBar, 0);
		var icon = Bounds(Find<ContentPresenter>(item, IconPresenterName), item);
		var label = Bounds(Find<ContentPresenter>(item, ContentPresenterName), item);
		Assert.IsTrue(icon.Right <= label.Left, $"Icon ({icon}) must be left of the label ({label}).");
		Assert.IsTrue(icon.Top < label.Bottom && label.Top < icon.Bottom, "Icon and label must share a row.");
		Assert.IsTrue(tabBar.ActualHeight < BottomBarHeight, $"A top bar must be more compact than {BottomBarHeight}, but is {tabBar.ActualHeight}.");
	}

	[TestMethod]
	[DataRow(null)]
	[DataRow(BottomStyle)]
	[DataRow(VerticalStyle)]
	public async Task When_NavigationStyle_IconSitsAboveLabel(string? styleKey)
	{
		var tabBar = CreateTabBar();

		await Load(tabBar, styleKey);

		var item = Item(tabBar, 0);
		var icon = Bounds(Find<ContentPresenter>(item, IconPresenterName), item);
		var label = Bounds(Find<ContentPresenter>(item, ContentPresenterName), item);
		Assert.IsTrue(icon.Bottom <= label.Top, $"Icon ({icon}) must be above the label ({label}).");
	}

	[TestMethod]
	[DataRow(null, Orientation.Horizontal)]
	[DataRow(TopStyle, Orientation.Horizontal)]
	[DataRow(SegmentedStyle, Orientation.Horizontal)]
	[DataRow(BottomStyle, Orientation.Horizontal)]
	[DataRow(VerticalStyle, Orientation.Vertical)]
	public async Task When_Styled_ItemsFollowOrientation(string? styleKey, Orientation orientation)
	{
		var tabBar = CreateTabBar();

		await Load(tabBar, styleKey);

		var first = Bounds(Item(tabBar, 0), tabBar);
		var second = Bounds(Item(tabBar, 1), tabBar);
		Assert.AreEqual(orientation, tabBar.Orientation);
		if (orientation == Orientation.Horizontal)
		{
			Assert.IsTrue(second.Left >= first.Right - 0.5, "Items must follow each other horizontally.");
		}
		else
		{
			Assert.IsTrue(second.Top >= first.Bottom - 0.5, "Items must follow each other vertically.");
		}
	}

	[TestMethod]
	[DataRow(null)]
	[DataRow(TopStyle)]
	[DataRow(SegmentedStyle)]
	public async Task When_ItemIsReEnabled_DisabledLookIsCleared(string? styleKey)
	{
		var tabBar = CreateTabBar(selectedIndex: 0);
		await Load(tabBar, styleKey);
		var item = Item(tabBar, 1);
		var enabled = ForegroundOf(item);

		item.IsEnabled = false;
		await UnitTestsUIContentHelper.WaitForIdle();
		var disabled = ForegroundOf(item);
		item.IsEnabled = true;
		await UnitTestsUIContentHelper.WaitForIdle();

		Assert.AreNotEqual(enabled, disabled, "A disabled item must look different.");
		Assert.AreEqual(enabled, ForegroundOf(item), "Re-enabling must restore the enabled look.");
	}

	[TestMethod]
	public async Task When_SelectedItemIsReEnabled_SelectedLookReturns()
	{
		var selectedBrush = new SolidColorBrush(Microsoft.UI.Colors.Red);
		var tabBar = CreateTabBar(selectedIndex: 1);
		var scope = new Grid { Children = { tabBar } };
		scope.Resources["TabBarItemForegroundSelected"] = selectedBrush;
		await Load(scope, tabBar, styleKey: null, ElementTheme.Light);
		var item = Item(tabBar, 1);
		Assert.AreEqual(selectedBrush.Color, ForegroundOf(item), "Selected");

		item.IsEnabled = false;
		await UnitTestsUIContentHelper.WaitForIdle();
		Assert.AreNotEqual(selectedBrush.Color, ForegroundOf(item), "Disabled");
		item.IsEnabled = true;
		await UnitTestsUIContentHelper.WaitForIdle();

		Assert.AreEqual(selectedBrush.Color, ForegroundOf(item), "Re-enabled");
		Assert.AreEqual(1d, Find<Border>(item, SelectionIndicatorName).Opacity, "The item must still be marked as selected.");
	}

	[TestMethod]
	[DataRow(null)]
	[DataRow(TopStyle)]
	[DataRow(SegmentedStyle)]
	public async Task When_ThemeChanges_ColorsFollow(string? styleKey)
	{
		var tabBar = CreateTabBar();
		await Load(tabBar, styleKey, ElementTheme.Light);
		var light = ForegroundOf(Item(tabBar, 0));

		((FrameworkElement)tabBar.Parent).RequestedTheme = ElementTheme.Dark;
		await UnitTestsUIContentHelper.WaitForIdle();

		Assert.AreNotEqual(light, ForegroundOf(Item(tabBar, 0)));
	}

	[TestMethod]
	[DataRow(null)]
	[DataRow(TopStyle)]
	[DataRow(SegmentedStyle)]
	[DataRow(BottomStyle)]
	public async Task When_RightToLeft_ItemsAndIndicatorAreMirrored(string? styleKey)
	{
		var tabBar = CreateTabBar(selectedIndex: 0);

		var root = await Load(tabBar, styleKey, flowDirection: FlowDirection.RightToLeft);

		var first = ScreenBounds(Item(tabBar, 0), root);
		var second = ScreenBounds(Item(tabBar, 1), root);
		Assert.IsTrue(first.Left >= second.Right - 0.5, $"The first item ({first}) must be at the leading, right side of the second ({second}).");
		var pill = ScreenBounds(Find<Border>(Item(tabBar, 0), SelectionIndicatorName), root);
		Assert.AreEqual(first.Left + first.Width / 2, pill.Left + pill.Width / 2, 0.5, "The selection indicator must stay centered on the selected item.");
	}

	[TestMethod]
	[DataRow(TopStyle)]
	[DataRow(SegmentedStyle)]
	public async Task When_RightToLeft_IconLeadsTheLabel(string styleKey)
	{
		var tabBar = CreateTabBar();

		var root = await Load(tabBar, styleKey, flowDirection: FlowDirection.RightToLeft);

		var item = Item(tabBar, 0);
		var icon = ScreenBounds(Find<ContentPresenter>(item, IconPresenterName), root);
		var label = ScreenBounds(Find<ContentPresenter>(item, ContentPresenterName), root);
		Assert.IsTrue(icon.Left >= label.Right - 0.5, $"The icon ({icon}) must be at the leading, right side of the label ({label}).");
	}

	[TestMethod]
	public async Task When_RightToLeft_VerticalIndicatorIsOnTheLeadingEdge()
	{
		var ltrBar = CreateTabBar(selectedIndex: 0);
		var ltrRoot = await Load(ltrBar, VerticalStyle);
		var ltrItem = ScreenBounds(Item(ltrBar, 0), ltrRoot);
		var ltrPill = ScreenBounds(Find<Border>(Item(ltrBar, 0), SelectionIndicatorName), ltrRoot);

		var rtlBar = CreateTabBar(selectedIndex: 0);
		var rtlRoot = await Load(rtlBar, VerticalStyle, flowDirection: FlowDirection.RightToLeft);
		var rtlItem = ScreenBounds(Item(rtlBar, 0), rtlRoot);
		var rtlPill = ScreenBounds(Find<Border>(Item(rtlBar, 0), SelectionIndicatorName), rtlRoot);

		Assert.IsTrue(ltrPill.Left < ltrItem.Left + ltrItem.Width / 2, "In LTR the indicator is at the left edge of the item.");
		Assert.IsTrue(rtlPill.Left > rtlItem.Left + rtlItem.Width / 2, "In RTL the indicator is at the right edge of the item.");
	}

	[TestMethod]
	public void When_ToolkitResourcesLoaded_HighContrastBrushesResolve()
	{
		var unresolved = ThemeDictionaryHelper.FindUnresolvedBrushes(new ToolkitResources(), "HighContrast", minimumCount: 30, "TabBar", "DefaultTopTabBar", "DefaultSegmentedTabBar");

		Assert.AreEqual(0, unresolved.Count, $"These TabBar High Contrast keys do not resolve to a brush: {string.Join(", ", unresolved)}");
	}

	[TestMethod]
	public async Task When_SegmentedStyle_BarIsBoxed()
	{
		var tabBar = CreateTabBar();

		await Load(tabBar, SegmentedStyle);

		Assert.IsTrue(tabBar.CornerRadius.TopLeft > 0, "Corner radius");
		Assert.IsTrue(tabBar.BorderThickness.Left > 0, "Border thickness");
		Assert.IsInstanceOfType<SolidColorBrush>(tabBar.BorderBrush, "Border brush");
		Assert.AreEqual(1d, Find<Border>(Item(tabBar, 0), "SelectedPlate").Opacity, "The selected item is raised.");
		Assert.AreEqual(0d, Find<Border>(Item(tabBar, 1), "SelectedPlate").Opacity, "Other items are flat.");
	}

	[TestMethod]
	public async Task When_ResourcesOverriddenInScope_IndicatorUsesThem()
	{
		var fill = new SolidColorBrush(Microsoft.UI.Colors.Red);
		var tabBar = CreateTabBar(selectedIndex: 0);
		var scope = new Grid { Children = { tabBar } };
		scope.Resources["TabBarSelectionIndicatorFill"] = fill;

		await Load(scope, tabBar, TopStyle, ElementTheme.Light);

		var pill = Find<Border>(Item(tabBar, 0), SelectionIndicatorName);
		Assert.AreEqual(fill.Color, (pill.Background as SolidColorBrush)?.Color);
	}

	[TestMethod]
	[DataRow("Default")]
	[DataRow("Light")]
	[DataRow("HighContrast")]
	public void When_ToolkitResourcesLoaded_TabBarBrushesExistPerTheme(string theme)
	{
		var keys = ThemeKeys(new ToolkitResources(), theme).ToHashSet();

		foreach (var key in new[]
		{
			"TabBarItemBackground", "TabBarItemBackgroundPointerOver", "TabBarItemForeground", "TabBarItemForegroundSelected", "TabBarItemForegroundDisabled",
			"TabBarSelectionIndicatorFill", "DefaultTopTabBarItemForeground", "DefaultSegmentedTabBarBackground",
		})
		{
			Assert.IsTrue(keys.Contains(key), $"'{key}' is missing from the '{theme}' theme dictionary.");
		}
	}

	[TestMethod]
	[DataRow("Material", "TopTabBarStyle")]
	[DataRow("Material", "BottomTabBarStyle")]
	[DataRow("Simple", "TopTabBarStyle")]
	public async Task When_DesignSystemLoaded_ItsStylesAreUnchanged(string designSystem, string styleKey)
	{
		ResourceDictionary NewTheme() => designSystem == "Material" ? new MaterialToolkitTheme() : new SimpleToolkitTheme();
		var themeOnly = NewTheme();
		var layered = NewTheme();
		var reference = CreateTabBar(selectedIndex: 1);
		var subject = CreateTabBar(selectedIndex: 1);
		reference.Style = (Style)themeOnly[styleKey];
		subject.Style = (Style)layered[styleKey];
		var referenceRoot = new Grid { Width = 400, Children = { reference } };
		referenceRoot.Resources.MergedDictionaries.Add(themeOnly);
		var subjectRoot = new Grid { Width = 400, Children = { subject } };
		// The design system is merged after the toolkit defaults, as its own theme dictionary does.
		subjectRoot.Resources.MergedDictionaries.Add(new ToolkitResources());
		subjectRoot.Resources.MergedDictionaries.Add(layered);

		await UnitTestUIContentHelperEx.SetContentAndWait(new StackPanel { Children = { referenceRoot, subjectRoot } });

		Assert.AreEqual(reference.ActualHeight, subject.ActualHeight, 0.5, "Height");
		Assert.AreEqual(Shape(Item(reference, 0)), Shape(Item(subject, 0)), "Item template");
		Assert.AreEqual(ForegroundOf(Item(reference, 1)), ForegroundOf(Item(subject, 1)), "Selected item foreground");
	}

	private static TabBar CreateTabBar(int selectedIndex = 0)
	{
		var tabBar = new TabBar();
		tabBar.Items.Add(new TabBarItem { Content = "Home", Icon = new SymbolIcon(Symbol.Home) });
		tabBar.Items.Add(new TabBarItem { Content = "Search", Icon = new SymbolIcon(Symbol.Find) });
		tabBar.Items.Add(new TabBarItem { Content = "Settings", Icon = new SymbolIcon(Symbol.Setting) });
		tabBar.SelectedIndex = selectedIndex;
		return tabBar;
	}

	private static Task<Grid> Load(TabBar tabBar, string? styleKey, ElementTheme theme = ElementTheme.Light, FlowDirection flowDirection = FlowDirection.LeftToRight)
		=> Load(tabBar, tabBar, styleKey, theme, flowDirection);

	private static async Task<Grid> Load(FrameworkElement content, TabBar tabBar, string? styleKey, ElementTheme theme, FlowDirection flowDirection = FlowDirection.LeftToRight)
	{
		var resources = new ToolkitResources();
		var root = new Grid { RequestedTheme = theme, FlowDirection = flowDirection, Width = 400, Height = 320, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
		root.Resources.MergedDictionaries.Add(resources);
		if (styleKey is not null)
		{
			Assert.IsTrue(resources.TryGetValue(styleKey, out var style), $"'{styleKey}' is not defined by ToolkitResources.");
			tabBar.Style = (Style)style;
		}
		tabBar.VerticalAlignment = VerticalAlignment.Top;
		root.Children.Add(content);

		await UnitTestUIContentHelperEx.SetContentAndWait(root);
		return root;
	}

	/// <summary>Bounds on screen. Measured against the LTR parent of the test root, so that a right-to-left layout is not reported in leading-edge coordinates.</summary>
	private static Rect ScreenBounds(FrameworkElement element, FrameworkElement root)
		=> Bounds(element, (UIElement)root.Parent);

	private static TabBarItem Item(TabBar tabBar, int index)
		=> tabBar.GetDescendants().OfType<TabBarItem>().ElementAt(index);

	private static T Find<T>(FrameworkElement owner, string name) where T : FrameworkElement
	{
		var element = owner.GetDescendants().OfType<T>().FirstOrDefault(x => x.Name == name);
		Assert.IsNotNull(element, $"'{name}' was not found in {owner.GetType().Name}.");
		return element;
	}

	private static Rect Bounds(FrameworkElement element, UIElement relativeTo)
		=> element.TransformToVisual(relativeTo).TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight));

	private static Windows.UI.Color ForegroundOf(TabBarItem item)
	{
		var brush = Find<ContentPresenter>(item, ContentPresenterName).Foreground;
		Assert.IsInstanceOfType<SolidColorBrush>(brush, "The item foreground must be a solid brush.");
		return ((SolidColorBrush)brush).Color;
	}

	private static string Shape(TabBarItem item)
		=> string.Join("/", item.GetDescendants().OfType<FrameworkElement>().Take(6).Select(x => $"{x.GetType().Name}#{x.Name}"));

	private static IEnumerable<string> ThemeKeys(ResourceDictionary dictionary, string theme)
	{
		if (dictionary.ThemeDictionaries.TryGetValue(theme, out var themed) && themed is ResourceDictionary themeDictionary)
		{
			foreach (var key in themeDictionary.Keys.OfType<string>())
			{
				yield return key;
			}
		}

		foreach (var merged in dictionary.MergedDictionaries)
		{
			foreach (var key in ThemeKeys(merged, theme))
			{
				yield return key;
			}
		}
	}
}
