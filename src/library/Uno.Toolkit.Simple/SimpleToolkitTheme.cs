using System;
using Uno.Simple;
using Microsoft.UI.Xaml;

namespace Uno.Toolkit.UI.Simple
{
	/// <summary>
	/// Simple Design System styles for the controls in the Uno.Toolkit.UI library.
	/// Inherits from <see cref="SimpleTheme"/> so all theme properties
	/// (Colors, DefaultDensity, DefaultCornerRadius, font/color overrides) are
	/// available directly without manual forwarding.
	/// </summary>
	public class SimpleToolkitTheme : SimpleTheme
	{
		private const string ToolkitPackageName = "Uno.Toolkit.WinUI";
		private const string ToolkitSimplePackageName = "Uno.Toolkit.WinUI.Simple";

		public SimpleToolkitTheme() : this(colorOverride: null, fontOverride: null)
		{
		}

		public SimpleToolkitTheme(ResourceDictionary? colorOverride = null, ResourceDictionary? fontOverride = null)
			: base(colorOverride, fontOverride)
		{
		}

		protected override ResourceDictionary GenerateSpecificResources()
		{
			var dict = base.GenerateSpecificResources();
			dict.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{ToolkitPackageName}/Generated/mergedpages.xaml") });
			dict.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{ToolkitSimplePackageName}/Generated/mergedpages.xaml") });
			return dict;
		}
	}
}
