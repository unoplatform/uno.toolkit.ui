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

		protected override void AddThemeSpecificResources()
		{
			base.AddThemeSpecificResources();

			// Layer the toolkit's own control styles on top of the generated theme.
			// A fresh ResourceDictionary is created per call so hot-reload edits to the
			// underlying XAML propagate, and AddThemeDictionary tracks them so they are
			// removed and re-added on every rebuild instead of accumulating.
			AddThemeDictionary(new ResourceDictionary { Source = new Uri($"ms-appx:///{ToolkitPackageName}/Generated/mergedpages.xaml") });
			AddThemeDictionary(new ResourceDictionary { Source = new Uri($"ms-appx:///{ToolkitMaterialPackageName}/Generated/mergedpages.v2.xaml") });
		}
	}
}
