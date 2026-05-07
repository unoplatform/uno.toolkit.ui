using System;
using Uno.Material;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI.Material
{
	/// <summary>
	/// Material (Material Design 3) styles for the controls in the Uno.Toolkit.UI library.
	/// Inherits from <see cref="MaterialTheme"/> so all theme properties
	/// (Colors, DefaultDensity, DefaultCornerRadius, font/color overrides) are
	/// available directly without manual forwarding.
	/// </summary>
	public class MaterialToolkitTheme : MaterialTheme
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

		protected override ResourceDictionary GenerateSpecificResources()
		{
			var dict = base.GenerateSpecificResources();
			dict.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{ToolkitPackageName}/Generated/mergedpages.xaml") });
			dict.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{ToolkitMaterialPackageName}/Generated/mergedpages.v2.xaml") });
			return dict;
		}
	}
}
