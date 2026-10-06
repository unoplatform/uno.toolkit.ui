using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.Toolkit.UI.Simple;
using Uno.UI.RuntimeTests;

namespace Uno.Toolkit.RuntimeTests.Tests;

/// <summary>
/// Guards how the theme AppBarButton templates render <c>Icon</c> and <c>Content</c> for
/// <see cref="NavigationBar.PrimaryCommands"/>.
///
/// Regression guard for unoplatform/uno.toolkit.ui#1652: Uno's <see cref="AppBarButton"/> looks up the template
/// part named <c>Content</c> and force-sets it to <c>Icon ?? Content</c>. The Simple template used that name for
/// the icon presenter nested in the Viewbox that collapses when no Icon is set, so a <see cref="UIElement"/>
/// assigned to <c>Content</c> was re-parented into that collapsed presenter and never rendered. A string survived
/// only because it can be displayed by two presenters at once.
/// </summary>
partial class NavigationBarTests
{
	[TestMethod]
	public async Task Simple_PrimaryCommands_Render_Icon_And_Content_Variants()
	{
		var theme = new SimpleToolkitTheme();
		var root = new Grid { Width = 800, Height = 200 };
		root.Resources.MergedDictionaries.Add(theme);

		var iconOnly = new SymbolIcon(Symbol.Add);
		var elementMarker = new TextBlock { Text = "Hello" };
		var bothIcon = new SymbolIcon(Symbol.Edit);
		var navigationBar = new NavigationBar
		{
			Content = "Title",
			Style = (Style)theme["SimpleNavigationBarStyle"],
			// Force the XAML-rendered template so the assertions hold on mobile hosts too.
			Template = (ControlTemplate)theme["XamlSimpleNavigationBarTemplate"],
		};
		navigationBar.PrimaryCommands.Add(new AppBarButton { Icon = iconOnly });
		navigationBar.PrimaryCommands.Add(new AppBarButton { Content = "Save" });
		navigationBar.PrimaryCommands.Add(new AppBarButton { Content = new Grid { Children = { elementMarker } } });
		navigationBar.PrimaryCommands.Add(new AppBarButton { Icon = bothIcon, Content = "Edit" });
		root.Children.Add(navigationBar);

		await UnitTestUIContentHelperEx.SetContentAndWait(root);

		var buttons = navigationBar.PrimaryCommands.Cast<AppBarButton>().ToArray();
		AssertRenderedInside(buttons[0], iconOnly, "The icon-only command's Icon");
		AssertRenderedInside(buttons[1], FindVisibleText(buttons[1], "Save"), "The string-content command's Content");
		AssertRenderedInside(buttons[2], elementMarker, "The UIElement-content command's Content");
		AssertRenderedInside(buttons[3], bothIcon, "The icon + content command's Icon");
		AssertRenderedInside(buttons[3], FindVisibleText(buttons[3], "Edit"), "The icon + content command's Content");
	}

	private static void AssertRenderedInside(AppBarButton button, FrameworkElement? element, string what)
	{
		if (element is null)
		{
			Assert.Fail($"{what} should be part of the AppBarButton visual tree.");
			return;
		}

		Assert.AreSame(button, element.GetFirstAncestor<AppBarButton>(), $"{what} should be hosted by the expected AppBarButton.");

		var collapsedAncestor = element.GetAncestors().OfType<FrameworkElement>().FirstOrDefault(x => x.Visibility == Visibility.Collapsed);
		Assert.IsNull(collapsedAncestor,
			$"{what} should not be nested under a collapsed element (found {collapsedAncestor?.GetType().Name}#{collapsedAncestor?.Name}).");
		Assert.IsTrue(element.ActualWidth > 0 && element.ActualHeight > 0,
			$"{what} should be laid out (was {element.ActualWidth}x{element.ActualHeight}).");
	}

	// A string may legitimately be hosted by more than one presenter; the visible one is what matters.
	private static TextBlock? FindVisibleText(AppBarButton button, string text) => button.GetDescendants()
		.OfType<TextBlock>()
		.FirstOrDefault(x => x.Text == text
			&& x.GetAncestors().OfType<FrameworkElement>().All(a => a.Visibility != Visibility.Collapsed));
}
