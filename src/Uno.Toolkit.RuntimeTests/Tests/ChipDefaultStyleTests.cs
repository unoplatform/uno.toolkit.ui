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
/// Covers the default (Fluent) Chip and ChipGroup styles shipped by <see cref="ToolkitResources"/>,
/// scoped to the test root so the host app's design system does not supply the implicit style.
/// The host app may still override the WinUI brushes the chip follows, so colors are only compared with each other.
/// </summary>
[TestClass]
[RunsOnUIThread]
public sealed class ChipDefaultStyleTests
{
	private const string RootName = "RootGrid";
	private const string IconPresenterName = "IconPresenter";
	private const string IconHostName = "IconViewbox";
	private const string ContentPresenterName = "ContentPresenter";
	private const string RemoveButtonName = "PART_RemoveButton";
	private const double ExpectedHeight = 32;

	[TestCleanup]
	public async Task Cleanup()
	{
		UnitTestsUIContentHelper.Content = null;
		await UnitTestsUIContentHelper.WaitForIdle();
	}

	[TestMethod]
	public async Task When_NoStyle_DefaultTemplateIsApplied()
	{
		var chip = new Chip { Content = "Chip" };

		await Load(chip);

		Assert.IsNotNull(Find<Button>(chip, RemoveButtonName), "A Chip without a Style must get the toolkit template, not the ToggleButton one.");
		Assert.IsNotNull(Find<ContentPresenter>(chip, ContentPresenterName));
	}

	[TestMethod]
	[DataRow(ElementTheme.Light)]
	[DataRow(ElementTheme.Dark)]
	public async Task When_NoStyle_ChipIsPillShaped(ElementTheme theme)
	{
		var chip = new Chip { Content = "Chip" };

		await Load(chip, theme);

		Assert.AreEqual(ExpectedHeight, chip.ActualHeight, 0.5, "Height");
		Assert.IsTrue(chip.CornerRadius.TopLeft >= chip.ActualHeight / 2, $"Corner radius {chip.CornerRadius.TopLeft} must round the full height.");
	}

	[TestMethod]
	public async Task When_IconIsUIElement_IconIsShown()
	{
		var icon = new SymbolIcon(Symbol.Favorite);
		var chip = new Chip { Content = "Chip", Icon = icon };

		await Load(chip);

		var presenter = Find<ContentPresenter>(chip, IconPresenterName);
		Assert.IsNotNull(presenter);
		Assert.AreEqual(Visibility.Visible, Find<Viewbox>(chip, IconHostName)?.Visibility);
		Assert.IsTrue(icon.ActualWidth > 0 && icon.ActualHeight > 0, "The icon element must be laid out.");
		Assert.IsTrue(presenter.Foreground is SolidColorBrush { Color.A: > 0 }, "The icon foreground must not be transparent.");
	}

	[TestMethod]
	public async Task When_IconIsNull_NoSpaceIsReserved()
	{
		var withoutIcon = new Chip { Content = "Chip" };
		var withIcon = new Chip { Content = "Chip", Icon = new SymbolIcon(Symbol.Favorite) };

		await Load(new StackPanel { Children = { withoutIcon, withIcon } });

		Assert.AreEqual(Visibility.Collapsed, Find<Viewbox>(withoutIcon, IconHostName)?.Visibility);
		Assert.IsTrue(withoutIcon.ActualWidth < withIcon.ActualWidth, "A chip without an icon must be narrower than the same chip with one.");
	}

	[TestMethod]
	public async Task When_IconChanges_VisibilityFollows()
	{
		var chip = new Chip { Content = "Chip" };
		await Load(chip);
		var host = Find<Viewbox>(chip, IconHostName);
		Assert.IsNotNull(host);

		chip.Icon = new SymbolIcon(Symbol.Favorite);
		await UnitTestsUIContentHelper.WaitForIdle();
		Assert.AreEqual(Visibility.Visible, host.Visibility, "After setting Icon");

		chip.Icon = null;
		await UnitTestsUIContentHelper.WaitForIdle();
		Assert.AreEqual(Visibility.Collapsed, host.Visibility, "After clearing Icon");
	}

	[TestMethod]
	public async Task When_CanRemove_RemoveButtonFollows()
	{
		var chip = new Chip { Content = "Chip" };
		await Load(chip);
		var button = Find<Button>(chip, RemoveButtonName);
		Assert.IsNotNull(button);
		Assert.AreEqual(Visibility.Collapsed, button.Visibility, "CanRemove=false");

		chip.CanRemove = true;
		await UnitTestsUIContentHelper.WaitForIdle();

		Assert.AreEqual(Visibility.Visible, button.Visibility, "CanRemove=true");
		Assert.IsTrue(button.ActualWidth > 0, "The remove button must be laid out.");
	}

	[TestMethod]
	[DataRow(ElementTheme.Light)]
	[DataRow(ElementTheme.Dark)]
	public async Task When_Checked_BackgroundAndForegroundChange(ElementTheme theme)
	{
		var rest = new Chip { Content = "Chip" };
		var selected = new Chip { Content = "Chip", IsChecked = true };

		await Load(new StackPanel { Children = { rest, selected } }, theme);

		Assert.AreNotEqual(BackgroundOf(rest), BackgroundOf(selected), "Checked background");
		Assert.AreNotEqual(ForegroundOf(rest), ForegroundOf(selected), "Checked foreground");
	}

	[TestMethod]
	[DataRow(ElementTheme.Light)]
	[DataRow(ElementTheme.Dark)]
	public async Task When_CheckedAndDisabled_StaysDistinctAndReadable(ElementTheme theme)
	{
		var rest = new Chip { Content = "Chip" };
		var disabled = new Chip { Content = "Chip", IsEnabled = false };
		var selectedDisabled = new Chip { Content = "Chip", IsChecked = true, IsEnabled = false };

		await Load(new StackPanel { Children = { rest, disabled, selectedDisabled } }, theme);

		Assert.AreNotEqual(BackgroundOf(rest), BackgroundOf(selectedDisabled), "A disabled checked chip must not look like an enabled unchecked one.");
		Assert.AreNotEqual(BackgroundOf(disabled), BackgroundOf(selectedDisabled), "A disabled checked chip must not look like a disabled unchecked one.");
		Assert.IsTrue(ForegroundOf(selectedDisabled).A > 0, "The text of a disabled checked chip must stay visible.");
	}

	[TestMethod]
	public async Task When_ThemeChanges_ColorsFollow()
	{
		var chip = new Chip { Content = "Chip" };
		await Load(chip, ElementTheme.Light);
		var lightBackground = BackgroundOf(chip);
		var lightForeground = ForegroundOf(chip);

		((FrameworkElement)chip.Parent).RequestedTheme = ElementTheme.Dark;
		await UnitTestsUIContentHelper.WaitForIdle();

		Assert.AreNotEqual(lightBackground, BackgroundOf(chip), "Background");
		Assert.AreNotEqual(lightForeground, ForegroundOf(chip), "Foreground");
	}

	[TestMethod]
	public async Task When_PaddingIsZero_RemoveButtonStaysInside()
	{
		var chip = new Chip { Content = "Chip", CanRemove = true, Padding = new Thickness(0) };

		await Load(chip);

		var button = Find<Button>(chip, RemoveButtonName);
		Assert.IsNotNull(button);
		var right = button.TransformToVisual(chip).TransformPoint(default).X + button.ActualWidth;
		Assert.IsTrue(right <= chip.ActualWidth, $"The remove button ends at {right}, past the chip width {chip.ActualWidth}.");
	}

	[TestMethod]
	public async Task When_ResourcesOverriddenInScope_ChipUsesThem()
	{
		var radius = new CornerRadius(4);
		var background = new SolidColorBrush(Microsoft.UI.Colors.Red);
		var chip = new Chip { Content = "Chip" };
		var scope = new Grid { Children = { chip } };
		scope.Resources["ChipCornerRadius"] = radius;
		scope.Resources["ChipBackground"] = background;

		await Load(scope);

		Assert.AreEqual(radius, chip.CornerRadius, "Metric override");
		Assert.AreEqual(background.Color, BackgroundOf(chip), "Brush override");
	}

	[TestMethod]
	public async Task When_Templated_AllToggleStatesAreDefined()
	{
		var chip = new Chip { Content = "Chip" };
		await Load(chip);

		var root = Find<Grid>(chip, RootName);
		Assert.IsNotNull(root);
		var states = VisualStateManager.GetVisualStateGroups(root).SelectMany(x => x.States).Select(x => x.Name).ToArray();

		// Every state ToggleButton can request: a missing one leaves the previous visuals in place.
		var expected = new[]
		{
			"Normal", "PointerOver", "Pressed", "Disabled",
			"Checked", "CheckedPointerOver", "CheckedPressed", "CheckedDisabled",
			"Indeterminate", "IndeterminatePointerOver", "IndeterminatePressed", "IndeterminateDisabled",
		};
		CollectionAssert.IsSubsetOf(expected, states);
	}

	[TestMethod]
	public async Task When_RightToLeft_IconAndRemoveButtonAreMirrored()
	{
		var chip = new Chip { Content = "Chip", CanRemove = true, Icon = new SymbolIcon(Symbol.Favorite) };

		var root = await Load(chip, flowDirection: FlowDirection.RightToLeft);

		var bounds = ScreenBounds(chip, root);
		var icon = ScreenBounds(Find<ContentPresenter>(chip, IconPresenterName)!, root);
		var content = ScreenBounds(Find<ContentPresenter>(chip, ContentPresenterName)!, root);
		var remove = ScreenBounds(Find<Button>(chip, RemoveButtonName)!, root);
		Assert.IsTrue(icon.Left >= content.Right - 0.5, $"The icon ({icon}) must be at the leading, right side of the text ({content}).");
		Assert.IsTrue(remove.Right <= content.Left + 0.5, $"The remove button ({remove}) must be at the trailing, left side of the text ({content}).");
		Assert.IsTrue(remove.Left >= bounds.Left && icon.Right <= bounds.Right + 0.5, "Icon and remove button must stay inside the chip.");
	}

	[TestMethod]
	public async Task When_RightToLeft_InsetsAreMirrored()
	{
		var ltrChip = new Chip { Content = "Chip", CanRemove = true, Icon = new SymbolIcon(Symbol.Favorite) };
		var ltrRoot = await Load(ltrChip);
		var ltrIconInset = ScreenBounds(Find<ContentPresenter>(ltrChip, IconPresenterName)!, ltrRoot).Left - ScreenBounds(ltrChip, ltrRoot).Left;
		var ltrRemoveInset = ScreenBounds(ltrChip, ltrRoot).Right - ScreenBounds(Find<Button>(ltrChip, RemoveButtonName)!, ltrRoot).Right;

		var rtlChip = new Chip { Content = "Chip", CanRemove = true, Icon = new SymbolIcon(Symbol.Favorite) };
		var rtlRoot = await Load(rtlChip, flowDirection: FlowDirection.RightToLeft);
		var rtlIconInset = ScreenBounds(rtlChip, rtlRoot).Right - ScreenBounds(Find<ContentPresenter>(rtlChip, IconPresenterName)!, rtlRoot).Right;
		var rtlRemoveInset = ScreenBounds(Find<Button>(rtlChip, RemoveButtonName)!, rtlRoot).Left - ScreenBounds(rtlChip, rtlRoot).Left;

		Assert.AreEqual(ltrIconInset, rtlIconInset, 0.5, "Icon inset from the leading edge");
		Assert.AreEqual(ltrRemoveInset, rtlRemoveInset, 0.5, "Remove button inset from the trailing edge");
	}

	[TestMethod]
	public void When_ToolkitResourcesLoaded_HighContrastBrushesResolve()
	{
		var unresolved = ThemeDictionaryHelper.FindUnresolvedBrushes(new ToolkitResources(), "HighContrast", minimumCount: 30, "Chip");

		Assert.AreEqual(0, unresolved.Count, $"These Chip High Contrast keys do not resolve to a brush: {string.Join(", ", unresolved)}");
	}

	[TestMethod]
	public void When_ToolkitResourcesLoaded_ImplicitChipStyleSetsOnlyWhatMaterialV1ChipsSet()
	{
		// On Uno the implicit Chip style sits under every explicit design-system style, so a property set here reaches each
		// Material v1 chip that does not set it (v1 defines no implicit Chip style of its own). v1 chips set the first group
		// and their templates do not read the second (focus visuals only); anything new must go in the template, or be left to the ToggleButton default style as alignment and font size are.
		var setByMaterialV1 = new[]
		{
			Control.ForegroundProperty, Control.BackgroundProperty, Control.BorderBrushProperty, Control.BorderThicknessProperty, Control.CornerRadiusProperty,
			Control.PaddingProperty, Control.FontFamilyProperty, FrameworkElement.VerticalAlignmentProperty, Control.VerticalContentAlignmentProperty, Control.TemplateProperty,
		};
		var ignoredByMaterialV1Templates = new[]
		{
			Control.UseSystemFocusVisualsProperty, Control.FocusVisualMarginProperty,
		};
		var resources = new ToolkitResources();
		Assert.IsTrue(resources.TryGetValue(typeof(Chip), out var value) && value is Style, "ToolkitResources must define an implicit Chip style.");

		for (var style = (Style?)value; style is not null; style = style.BasedOn)
		{
			var added = style.Setters.OfType<Setter>()
				.Where(x => !setByMaterialV1.Contains(x.Property) && !ignoredByMaterialV1Templates.Contains(x.Property))
				.Select(x => x.Property?.ToString() ?? "?")
				.ToArray();
			Assert.AreEqual(0, added.Length, $"The implicit Chip style (or one it is based on) now also sets {string.Join(", ", added)}.");
		}
	}

	[TestMethod]
	public async Task When_ChipHasNoFontSize_ItUsesTheControlContentFontSize()
	{
		// The font size comes from the ToggleButton default style, so the implicit Chip style does not set it.
		var chip = new Chip { Content = "Chip" };

		await Load(chip);

		Assert.AreEqual((double)Application.Current.Resources["ControlContentThemeFontSize"], chip.FontSize);
		Assert.AreEqual(HorizontalAlignment.Left, chip.HorizontalAlignment, "HorizontalAlignment");
		Assert.AreEqual(HorizontalAlignment.Center, chip.HorizontalContentAlignment, "HorizontalContentAlignment");
	}

	[TestMethod]
	public async Task When_ChipHasNoSize_ItIsAtLeastTheChipHeightTall()
	{
		// MinHeight lives in the template (not the implicit style) so it cannot leak into design-system chips.
		var chip = new Chip { Content = "Chip" };

		await Load(chip);

		Assert.AreEqual(ExpectedHeight, chip.ActualHeight, 0.5);
		Assert.AreEqual(0d, chip.MinHeight, "The style must not set MinHeight on the chip itself.");
	}

	[TestMethod]
	public async Task When_ChipGroupHasNoStyle_ChipsAreLaidOutHorizontally()
	{
		var group = new ChipGroup();
		group.Items.Add(new Chip { Content = "One" });
		group.Items.Add(new Chip { Content = "Two" });

		await Load(group);

		var chips = group.GetDescendants().OfType<Chip>().ToArray();
		Assert.AreEqual(2, chips.Length);
		var first = chips[0].TransformToVisual(group).TransformPoint(default);
		var second = chips[1].TransformToVisual(group).TransformPoint(default);
		Assert.AreEqual(first.Y, second.Y, 0.5, "Chips must share a row.");
		Assert.IsTrue(second.X >= first.X + chips[0].ActualWidth, "The second chip must follow the first.");
	}

	[TestMethod]
	[DataRow("Default")]
	[DataRow("Light")]
	[DataRow("HighContrast")]
	public void When_ToolkitResourcesLoaded_ChipBrushesExistPerTheme(string theme)
	{
		var keys = ThemeKeys(new ToolkitResources(), theme).ToHashSet();

		foreach (var key in new[] { "ChipBackground", "ChipBackgroundChecked", "ChipForeground", "ChipForegroundCheckedDisabled", "ChipBorderBrush", "ChipIconForeground" })
		{
			Assert.IsTrue(keys.Contains(key), $"'{key}' is missing from the '{theme}' theme dictionary.");
		}
	}

	[TestMethod]
	[DataRow("Material", "MaterialDefaultChipStyle")]
	[DataRow("Simple", "SimpleDefaultChipStyle")]
	public async Task When_DesignSystemLoaded_ItsImplicitStyleWins(string designSystem, string defaultStyleKey)
	{
		ResourceDictionary theme = designSystem == "Material" ? new MaterialToolkitTheme() : new SimpleToolkitTheme();
		var implicitChip = new Chip { Content = "Chip" };
		var explicitChip = new Chip { Content = "Chip", Style = (Style)theme[defaultStyleKey] };
		var root = new Grid { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
		// The design system is merged after the toolkit defaults, as its own theme dictionary does.
		root.Resources.MergedDictionaries.Add(new ToolkitResources());
		root.Resources.MergedDictionaries.Add(theme);
		root.Children.Add(new StackPanel { Children = { implicitChip, explicitChip } });

		await UnitTestUIContentHelperEx.SetContentAndWait(root);

		Assert.AreEqual(explicitChip.CornerRadius, implicitChip.CornerRadius, "CornerRadius");
		Assert.AreEqual(explicitChip.Padding, implicitChip.Padding, "Padding");
		Assert.AreEqual(explicitChip.ActualHeight, implicitChip.ActualHeight, 0.5, "Height");
		Assert.AreEqual(TemplateShape(explicitChip), TemplateShape(implicitChip), "Template");
	}

	private static async Task<Grid> Load(FrameworkElement content, ElementTheme theme = ElementTheme.Light, FlowDirection flowDirection = FlowDirection.LeftToRight)
	{
		var root = new Grid { RequestedTheme = theme, FlowDirection = flowDirection, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
		root.Resources.MergedDictionaries.Add(new ToolkitResources());
		root.Children.Add(content);

		await UnitTestUIContentHelperEx.SetContentAndWait(root);
		return root;
	}

	/// <summary>Bounds on screen. Measured against the LTR parent of the test root, so that a right-to-left layout is not reported in leading-edge coordinates.</summary>
	private static Rect ScreenBounds(FrameworkElement element, FrameworkElement root)
		=> element.TransformToVisual((UIElement)root.Parent).TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight));

	private static T? Find<T>(FrameworkElement owner, string name) where T : FrameworkElement
		=> owner.GetDescendants().OfType<T>().FirstOrDefault(x => x.Name == name);

	private static string TemplateShape(Chip chip)
		=> string.Join("/", chip.GetDescendants().OfType<FrameworkElement>().Take(4).Select(x => $"{x.GetType().Name}#{x.Name}"));

	private static Windows.UI.Color BackgroundOf(Chip chip)
		=> ColorOf(Find<Grid>(chip, RootName)?.Background, "background");

	private static Windows.UI.Color ForegroundOf(Chip chip)
		=> ColorOf(Find<ContentPresenter>(chip, ContentPresenterName)?.Foreground, "foreground");

	private static Windows.UI.Color ColorOf(Brush? brush, string what)
	{
		Assert.IsInstanceOfType<SolidColorBrush>(brush, $"The chip {what} must be a solid brush.");
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
