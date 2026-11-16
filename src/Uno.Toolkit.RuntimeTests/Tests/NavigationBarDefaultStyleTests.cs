using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Windows.Foundation;

namespace Uno.Toolkit.RuntimeTests.Tests;

/// <summary>
/// Covers the default (Fluent) NavigationBar style shipped by <see cref="ToolkitResources"/>,
/// scoped to the test root so the host app's design system does not supply the implicit style.
/// </summary>
[TestClass]
[RunsOnUIThread]
public sealed class NavigationBarDefaultStyleTests
{
	private const string TitleHostName = "ContentControl";
	private const string Title = "Page title";
	private const string BackGlyph = "\uE72B";

	// WinUI metrics: CommandBar compact height, NavigationView back button, Subtitle text.
	private const double BarHeight = 48;
	private const double BackButtonWidth = 40;
	private const double BackButtonHeight = 36;
	private const double TitleFontSize = 20;
	private const double BarWidth = 480;
	private const double CommandBarGutter = 6;

	[TestCleanup]
	public async Task Cleanup()
	{
		UnitTestsUIContentHelper.Content = null;
		await UnitTestsUIContentHelper.WaitForIdle();
	}

	[TestMethod]
	public async Task When_TitleOnly_TitleIsLeftAlignedAndVerticallyCentered()
	{
		var navBar = new NavigationBar { Content = Title };

		await Load(navBar);

		var title = Bounds(TitleText(navBar), navBar);
		Assert.AreEqual(BarHeight, navBar.ActualHeight, 0.5, "Bar height");
		Assert.IsTrue(title.Left < BarWidth / 4, $"The title must start at the left, but starts at {title.Left}.");
		Assert.IsTrue(title.Left >= 12, $"The title needs a left inset, but starts at {title.Left}.");
		Assert.AreEqual(navBar.ActualHeight / 2, title.Top + title.Height / 2, 1.5, "Title vertical centre");
	}

	[TestMethod]
	public async Task When_Templated_TitleUsesSubtitleTypography()
	{
		var navBar = new NavigationBar { Content = Title };

		await Load(navBar);

		var text = TitleText(navBar);
		Assert.AreEqual(TitleFontSize, text.FontSize, "Font size");
		Assert.AreEqual(FontWeights.SemiBold.Weight, text.FontWeight.Weight, "Font weight");
	}

	[TestMethod]
	public async Task When_MainCommandShown_BackButtonHasFluentMetrics()
	{
		var navBar = new NavigationBar { Content = Title, MainCommandMode = MainCommandMode.Action };

		await Load(navBar);

		var button = navBar.MainCommand;
		Assert.IsNotNull(button);
		Assert.AreEqual(Visibility.Visible, button.Visibility);
		var bounds = Bounds(button, navBar);
		Assert.AreEqual(BackButtonWidth, bounds.Width, 0.5, "Width");
		Assert.AreEqual(BackButtonHeight, bounds.Height, 0.5, "Height");
		Assert.AreEqual(navBar.ActualHeight / 2, bounds.Top + bounds.Height / 2, 1, "Back button vertical centre");

		var title = Bounds(TitleText(navBar), navBar);
		Assert.IsTrue(title.Left >= bounds.Right, $"The title ({title.Left}) must start after the back button ({bounds.Right}).");

		var glyph = button.GetDescendants().OfType<TextBlock>().FirstOrDefault(x => x.Text == BackGlyph);
		Assert.IsNotNull(glyph, "Without an Icon, the main command must show the back glyph.");
		Assert.IsTrue(glyph.ActualWidth > 0 && glyph.ActualHeight > 0, "The back glyph must be laid out.");
	}

	[TestMethod]
	public async Task When_MainCommandIsDisabled_ItLooksDisabled()
	{
		var navBar = new NavigationBar { Content = Title, MainCommandMode = MainCommandMode.Action };
		await Load(navBar);
		var enabled = ColorOf(IconPresenter(navBar.MainCommand).Foreground);

		navBar.MainCommand.IsEnabled = false;
		await UnitTestsUIContentHelper.WaitForIdle();
		var disabled = ColorOf(IconPresenter(navBar.MainCommand).Foreground);
		navBar.MainCommand.IsEnabled = true;
		await UnitTestsUIContentHelper.WaitForIdle();

		Assert.AreNotEqual(enabled, disabled, "Disabled");
		Assert.AreEqual(enabled, ColorOf(IconPresenter(navBar.MainCommand).Foreground), "Re-enabled");
	}

	[TestMethod]
	public async Task When_NoOverflowCommands_OverflowButtonIsHidden()
	{
		var navBar = new NavigationBar { Content = Title };
		navBar.PrimaryCommands.Add(new AppBarButton { Icon = new SymbolIcon(Symbol.Add), Label = "Add" });

		await Load(navBar);

		var more = navBar.GetDescendants().OfType<Button>().FirstOrDefault(x => x.Name == "MoreButton");
		Assert.IsNotNull(more, "MoreButton");
		Assert.AreEqual(Visibility.Collapsed, more.Visibility);
	}

	[TestMethod]
	public async Task When_NavigationBarFontSizeIsSet_TitleUsesIt()
	{
		var navBar = new NavigationBar { Content = Title, FontSize = 28 };

		await Load(navBar);

		Assert.AreEqual(28d, TitleText(navBar).FontSize);
	}

	[TestMethod]
	public async Task When_NavigationBarHeightIsOverriddenInScope_BarFollowsAndCommandsStayCentered()
	{
		var command = new AppBarButton { Icon = new SymbolIcon(Symbol.Add) };
		var navBar = new NavigationBar { Content = Title };
		navBar.PrimaryCommands.Add(command);
		var scope = new Grid { Children = { navBar } };
		scope.Resources["NavigationBarHeight"] = 56d;

		await Load(scope);

		Assert.AreEqual(56d, navBar.ActualHeight, 0.5, "Bar height");
		var bounds = Bounds(command, navBar);
		Assert.AreEqual(28d, bounds.Top + bounds.Height / 2, 1, "Command vertical centre");
		var title = Bounds(TitleText(navBar), navBar);
		Assert.AreEqual(28d, title.Top + title.Height / 2, 1.5, "Title vertical centre");
	}

	[TestMethod]
	public async Task When_MainCommandIconIsUIElement_ItIsRendered()
	{
		var icon = new SymbolIcon(Symbol.Cancel);
		var navBar = new NavigationBar
		{
			Content = Title,
			MainCommandMode = MainCommandMode.Action,
			MainCommand = new AppBarButton { Icon = icon },
		};

		await Load(navBar);

		Assert.IsTrue(icon.ActualWidth > 0 && icon.ActualHeight > 0, "The main command icon must be laid out.");
		var bounds = Bounds(icon, navBar.MainCommand);
		Assert.IsTrue(bounds.Left >= 0 && bounds.Right <= navBar.MainCommand.ActualWidth, $"The icon ({bounds}) must sit inside the button.");
	}

	[TestMethod]
	public async Task When_ContentAlignmentIsCenter_TitleIsCenteredInTheBar()
	{
		var navBar = new NavigationBar
		{
			Content = Title,
			MainCommandMode = MainCommandMode.Action,
			HorizontalContentAlignment = HorizontalAlignment.Center,
		};
		navBar.PrimaryCommands.Add(new AppBarButton { Icon = new SymbolIcon(Symbol.Add), Label = "Add" });

		await Load(navBar);

		var title = Bounds(TitleText(navBar), navBar);
		// The CommandBar template keeps a 6px gutter at the right edge, so the title centres in the bar minus that gutter.
		Assert.AreEqual(navBar.ActualWidth / 2, title.Left + title.Width / 2, CommandBarGutter / 2 + 0.5);
	}

	[TestMethod]
	public async Task When_PrimaryCommandsSet_TheyAreVerticallyCentered()
	{
		var command = new AppBarButton { Icon = new SymbolIcon(Symbol.Add), Label = "Add" };
		var navBar = new NavigationBar { Content = Title };
		navBar.PrimaryCommands.Add(command);

		await Load(navBar);

		var bounds = Bounds(command, navBar);
		Assert.IsTrue(bounds.Width > 0, "The primary command must be laid out.");
		Assert.IsTrue(bounds.Left > navBar.ActualWidth / 2, "Primary commands sit at the right.");
		Assert.AreEqual(navBar.ActualHeight / 2, bounds.Top + bounds.Height / 2, 1, "Primary command vertical centre");
	}

	[TestMethod]
	public async Task When_PrimaryCommandHasLabel_BarStaysCompact()
	{
		var command = new AppBarButton { Icon = new SymbolIcon(Symbol.Add), Label = "Add" };
		var navBar = new NavigationBar { Content = Title };
		navBar.PrimaryCommands.Add(command);

		await Load(navBar);

		Assert.AreEqual(BarHeight, navBar.ActualHeight, 0.5, "Bar height");
		Assert.IsTrue(Bounds(command, navBar).Height <= BarHeight + 0.5, "The command must fit the bar.");
	}

	[TestMethod]
	public async Task When_ThemeChanges_TitleColorFollows()
	{
		var navBar = new NavigationBar { Content = Title };
		await Load(navBar, ElementTheme.Light);
		var light = ColorOf(TitleText(navBar).Foreground);

		((FrameworkElement)navBar.Parent).RequestedTheme = ElementTheme.Dark;
		await UnitTestsUIContentHelper.WaitForIdle();

		Assert.AreNotEqual(light, ColorOf(TitleText(navBar).Foreground));
	}

	[TestMethod]
	public async Task When_ResourcesOverriddenInScope_TitleUsesThem()
	{
		var navBar = new NavigationBar { Content = Title };
		var scope = new Grid { Children = { navBar } };
		scope.Resources["NavigationBarFontSize"] = 28d;

		await Load(scope);

		Assert.AreEqual(28d, TitleText(navBar).FontSize);
	}

	[TestMethod]
	[DataRow("Default")]
	[DataRow("Light")]
	[DataRow("HighContrast")]
	public void When_ToolkitResourcesLoaded_NavigationBarBrushesExistPerTheme(string theme)
	{
		var keys = ThemeKeys(new ToolkitResources(), theme).ToHashSet();

		foreach (var key in new[]
		{
			"NavigationBarBackground", "NavigationBarForeground",
			"NavigationBarMainCommandForeground", "NavigationBarMainCommandForegroundPointerOver", "NavigationBarMainCommandForegroundPressed", "NavigationBarMainCommandForegroundDisabled",
			"NavigationBarMainCommandBackground", "NavigationBarMainCommandBackgroundPointerOver", "NavigationBarMainCommandBackgroundPressed", "NavigationBarMainCommandBackgroundDisabled",
		})
		{
			Assert.IsTrue(keys.Contains(key), $"'{key}' is missing from the '{theme}' theme dictionary.");
		}
	}

	private static async Task Load(FrameworkElement content, ElementTheme theme = ElementTheme.Light)
	{
		var root = new Grid { RequestedTheme = theme, Width = BarWidth, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
		root.Resources.MergedDictionaries.Add(new ToolkitResources());
		root.Children.Add(content);

		await UnitTestUIContentHelperEx.SetContentAndWait(root);
	}

	private static TextBlock TitleText(NavigationBar navBar)
	{
		var host = navBar.GetDescendants().OfType<ContentControl>().FirstOrDefault(x => x.Name == TitleHostName);
		Assert.IsNotNull(host, "The title host was not found.");
		var text = host.GetDescendants().OfType<TextBlock>().FirstOrDefault(x => x.Text == Title);
		Assert.IsNotNull(text, "The title text was not found.");
		return text;
	}

	private static ContentPresenter IconPresenter(AppBarButton button)
	{
		var presenter = button.GetDescendants().OfType<ContentPresenter>().FirstOrDefault(x => x.Name == "IconPresenter");
		Assert.IsNotNull(presenter, "The main command icon presenter was not found.");
		return presenter;
	}

	private static Rect Bounds(FrameworkElement element, UIElement relativeTo)
		=> element.TransformToVisual(relativeTo).TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight));

	private static Windows.UI.Color ColorOf(Brush? brush)
	{
		Assert.IsInstanceOfType<SolidColorBrush>(brush, "Expected a solid brush.");
		return ((SolidColorBrush)brush).Color;
	}

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
