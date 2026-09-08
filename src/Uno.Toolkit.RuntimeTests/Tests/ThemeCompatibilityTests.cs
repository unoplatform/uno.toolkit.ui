using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Themes;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.Toolkit.UI.Material;
using Uno.Toolkit.UI.Simple;
using Uno.UI.RuntimeTests;

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	internal sealed class ThemeCompatibilityTests
	{
		[TestCleanup]
		public async Task Cleanup()
		{
			UnitTestsUIContentHelper.Content = null;
			await UnitTestsUIContentHelper.WaitForIdle();
		}

		[TestMethod]
		[DataRow(ElementTheme.Light)]
		[DataRow(ElementTheme.Dark)]
		public async Task When_SimpleStylesLoaded_FontsResolveDefaultFontFamily(ElementTheme requestedTheme)
		{
			var theme = new SimpleToolkitTheme();
			var root = CreateRoot(theme, requestedTheme);
			var card = new Card { HeaderContent = "Card", Style = (Style)theme["SimpleFilledCardStyle"] };
			var contentCard = new CardContentControl { Content = "Content card", Style = (Style)theme["SimpleFilledCardContentControlStyle"] };
			var chip = new Chip { Content = "Chip", Style = (Style)theme["SimpleChipStyle"] };
			var divider = new Divider { SubHeader = "Divider", Style = (Style)theme["SimpleDividerStyle"] };
			var navigationBar = new NavigationBar
			{
				Content = "Navigation",
				Style = (Style)theme["SimpleNavigationBarStyle"],
				// Exercise the shared XAML font aliases on mobile hosts as well.
				Template = (ControlTemplate)theme["XamlSimpleNavigationBarTemplate"]
			};
			var ellipsisButton = new Button { Style = (Style)theme["SimpleEllipsisButton"] };
			root.Children.Add(new StackPanel { Children = { card, contentCard, chip, divider, navigationBar, ellipsisButton } });

			await UnitTestUIContentHelperEx.SetContentAndWait(root);

			var expected = ((FontFamily)theme["DefaultFontFamily"]).Source;
			StringAssert.Contains(expected, "Inter.ttf", "The scoped Simple theme must supply its own default typeface.");
			Assert.AreEqual(expected, card.FontFamily.Source, "Card");
			Assert.AreEqual(expected, contentCard.FontFamily.Source, "CardContentControl");
			Assert.AreEqual(expected, chip.FontFamily.Source, "Chip");
			Assert.AreEqual(expected, Descendants<TextBlock>(divider).Single(x => x.Text == "Divider").FontFamily.Source, "Divider subheader alias");
			Assert.AreEqual(expected, Descendants<ContentControl>(navigationBar).Single(x => x.Name == "ContentControl").FontFamily.Source, "NavigationBar content alias");
			Assert.AreEqual(expected, ellipsisButton.FontFamily.Source, "NavigationBar ellipsis alias");
		}

		[TestMethod]
		[DataRow(ElementTheme.Light)]
		[DataRow(ElementTheme.Dark)]
		public async Task When_SimpleDefaultFontFamilySet_DirectControlStylesUseScopedFont(ElementTheme requestedTheme)
		{
			var expected = new FontFamily("Arial");
			var theme = new SimpleToolkitTheme { DefaultFontFamily = expected };
			var root = CreateRoot(theme, requestedTheme);
			var card = new Card { HeaderContent = "Card", Style = (Style)theme["SimpleFilledCardStyle"] };
			var contentCard = new CardContentControl { Content = "Content card", Style = (Style)theme["SimpleFilledCardContentControlStyle"] };
			var chip = new Chip { Content = "Chip", Style = (Style)theme["SimpleChipStyle"] };
			root.Children.Add(new StackPanel { Children = { card, contentCard, chip } });

			await UnitTestUIContentHelperEx.SetContentAndWait(root);

			Assert.AreEqual(expected.Source, card.FontFamily.Source, "Card");
			Assert.AreEqual(expected.Source, contentCard.FontFamily.Source, "CardContentControl");
			Assert.AreEqual(expected.Source, chip.FontFamily.Source, "Chip");
		}

		[TestMethod]
		[DataRow(Density.Compact, 9d)]
		[DataRow(Density.Regular, 12d)]
		[DataRow(Density.Comfy, 15d)]
		public async Task When_MaterialDefaultSpacingSet_DensityScalesInheritedTokens(Density density, double expected)
		{
			var theme = new MaterialToolkitTheme { DefaultSpacing = 6, DefaultDensity = density };
			var root = CreateRoot(theme, ElementTheme.Light);
			var probe = (TextBlock)XamlReader.Load("""
				<TextBlock xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
				           FontSize="{ThemeResource Space200}" Text="Spacing token" />
				""");
			root.Children.Add(probe);

			await UnitTestUIContentHelperEx.SetContentAndWait(root);

			Assert.AreEqual(expected, probe.FontSize, "Space200 must equal DefaultSpacing × density factor × 2.");
		}

		private static Grid CreateRoot(ResourceDictionary theme, ElementTheme requestedTheme)
		{
			var root = new Grid { Width = 500, Height = 600, RequestedTheme = requestedTheme };
			root.Resources.MergedDictionaries.Add(theme);
			return root;
		}

		private static IEnumerable<T> Descendants<T>(DependencyObject parent) where T : DependencyObject
		{
			for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
			{
				var child = VisualTreeHelper.GetChild(parent, i);
				if (child is T match)
				{
					yield return match;
				}
				foreach (var descendant in Descendants<T>(child))
				{
					yield return descendant;
				}
			}
		}
	}
}
