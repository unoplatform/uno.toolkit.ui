using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Material;
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
			Assert.IsFalse(string.IsNullOrWhiteSpace(expected), "Simple theme DefaultFontFamily must resolve to a non-empty FontFamily source.");
			Assert.AreEqual(expected, card.FontFamily.Source, "Card");
			Assert.AreEqual(expected, contentCard.FontFamily.Source, "CardContentControl");
			Assert.AreEqual(expected, chip.FontFamily.Source, "Chip");
			Assert.AreEqual(expected, divider.GetDescendants().OfType<TextBlock>().Single(x => x.Text == "Divider").FontFamily.Source, "Divider subheader alias");
			Assert.AreEqual(expected, navigationBar.GetDescendants().OfType<ContentControl>().Single(x => x.Name == "ContentControl").FontFamily.Source, "NavigationBar content alias");
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
		[DataRow(Density.Compact)]
		[DataRow(Density.Regular)]
		[DataRow(Density.Comfy)]
		public async Task When_MaterialDefaultSpacingSet_InheritsMaterialThemeTokens(Density density)
		{
			var theme = new MaterialToolkitTheme { DefaultSpacing = 6, DefaultDensity = density };
			var referenceTheme = new MaterialTheme { DefaultSpacing = 6, DefaultDensity = density };
			var root = CreateRoot(theme, ElementTheme.Light);
			var referenceRoot = CreateRoot(referenceTheme, ElementTheme.Light);
			const string probeXaml = """
				<Border xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
				        Padding="{ThemeResource Space200}" />
				""";
			var probe = (Border)XamlReader.Load(probeXaml);
			var referenceProbe = (Border)XamlReader.Load(probeXaml);
			root.Children.Add(probe);
			referenceRoot.Children.Add(referenceProbe);

			await UnitTestUIContentHelperEx.SetContentAndWait(new Grid { Children = { root, referenceRoot } });

			Assert.IsTrue(referenceProbe.Padding.Left > 0, "The reference spacing token must resolve.");
			Assert.AreEqual(referenceProbe.Padding, probe.Padding, "Toolkit must inherit MaterialTheme spacing generation for the same inputs.");
		}

		private static Grid CreateRoot(ResourceDictionary theme, ElementTheme requestedTheme)
		{
			var root = new Grid { Width = 500, Height = 600, RequestedTheme = requestedTheme };
			root.Resources.MergedDictionaries.Add(theme);
			return root;
		}
	}
}
