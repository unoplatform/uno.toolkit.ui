using System.Linq;
using Uno.Themes;
using Windows.UI;
using Microsoft.UI.Xaml.Media;

namespace Uno.Toolkit.Samples.Content.Controls;

[SamplePage(
	SampleCategory.Controls,
	"Seed Color",
	SourceSdk.UnoToolkit,
	Description = "Generate a full color palette from a single seed color using the HCT color space.",
	SupportedDesigns = new[] { Design.Material })]
public sealed partial class SeedColorSamplePage : Page
{
	private static Color _lastSeed = Color.FromArgb(0xFF, 0x59, 0x46, 0xD2);

	public SeedColorSamplePage()
	{
		this.InitializeComponent();
		SeedColorPicker.Color = _lastSeed;
		ApplySeedColor(_lastSeed);

		this.ActualThemeChanged += (s, e) =>
		{
			SetPrimarySeed(null);
			SetPrimarySeed(_lastSeed);
		};
	}

	private void SeedColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
	{
		ApplySeedColor(args.NewColor);
	}

	private void ApplySeedColor(Color seed)
	{
		_lastSeed = seed;
		SetPrimarySeed(seed);

		SeedSwatch.Background = new SolidColorBrush(seed);
		SeedHex.Text = $"#{seed.R:X2}{seed.G:X2}{seed.B:X2}";

#if THEME_SIMPLE
		XamlSnippet.Text =
			$"<SimpleToolkitTheme xmlns=\"using:Uno.Toolkit.UI.Simple\">\n" +
			$"  <SimpleToolkitTheme.Colors>\n" +
			$"    <ThemeColors xmlns=\"using:Uno.Themes\"\n" +
			$"                 PrimarySeed=\"#{seed.R:X2}{seed.G:X2}{seed.B:X2}\" />\n" +
			$"  </SimpleToolkitTheme.Colors>\n" +
			$"</SimpleToolkitTheme>";
#else
		XamlSnippet.Text =
			$"<MaterialToolkitTheme xmlns=\"using:Uno.Toolkit.UI.Material\">\n" +
			$"  <MaterialToolkitTheme.Colors>\n" +
			$"    <ThemeColors xmlns=\"using:Uno.Themes\"\n" +
			$"                 PrimarySeed=\"#{seed.R:X2}{seed.G:X2}{seed.B:X2}\" />\n" +
			$"  </MaterialToolkitTheme.Colors>\n" +
			$"</MaterialToolkitTheme>";
#endif
	}

	/// <summary>
	/// Finds the <see cref="BaseTheme"/> nested inside the toolkit theme wrapper
	/// and sets the primary seed color on it.
	/// </summary>
	private static void SetPrimarySeed(Color? seed)
	{
		var baseTheme = Application.Current?.Resources?.MergedDictionaries
			.SelectMany(rd => rd.MergedDictionaries)
			.OfType<BaseTheme>()
			.FirstOrDefault();

		if (baseTheme is null) return;

		if (baseTheme.Colors is null)
		{
			baseTheme.Colors = new ThemeColors();
		}

		baseTheme.Colors.PrimarySeed = seed;
	}
}
