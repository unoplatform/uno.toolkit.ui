using System;
using Uno.Material;
using Uno.Themes;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI.Material
{
	/// <summary>
	/// Material (Material Design 3) styles for the controls in the Uno.Toolkit.UI library.
	/// </summary>
	public class MaterialToolkitTheme : BaseToolkitTheme
	{
		private const string ToolkitPackageName = "Uno.Toolkit.WinUI";
		private const string ToolkitMaterialPackageName = "Uno.Toolkit.WinUI.Material";

		public MaterialToolkitTheme() : this(colorOverride: null, fontOverride: null)
		{
		}

		public MaterialToolkitTheme(ResourceDictionary? colorOverride = null, ResourceDictionary? fontOverride = null)
			: base(colorOverride, fontOverride)
		{
		}

		protected override void UpdateSource()
		{
#if !HAS_UNO
			Source = null;
#endif
			ThemeDictionaries.Clear();
			MergedDictionaries.Clear();
			this.Clear();

			var materialTheme = new MaterialTheme(fontOverride: FontOverrideDictionary);

			// Route color overrides through ThemeColors.OverrideDictionary which has
			// highest precedence in BaseTheme (above the seed palette). This ensures
			// user color overrides aren't stomped by DefaultPrimarySeed.
			if (Colors is { } colors)
			{
				materialTheme.Colors = colors;
			}

			if (ColorOverrideDictionary is { } colorOverride)
			{
				var tc = materialTheme.Colors ?? new ThemeColors();
				tc.OverrideDictionary ??= colorOverride;
				materialTheme.Colors ??= tc;
			}

			MergedDictionaries.Add(materialTheme);
			MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{ToolkitPackageName}/Generated/mergedpages.xaml") });
			MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{ToolkitMaterialPackageName}/Generated/mergedpages.v2.xaml") });
		}
	}
}
