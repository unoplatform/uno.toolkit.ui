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

		protected override void AddThemeSpecificResources()
		{
			base.AddThemeSpecificResources();

			// Layer the toolkit's own control styles on top of the generated theme.
			// A fresh ResourceDictionary is created per call so hot-reload edits to the
			// underlying XAML propagate, and AddThemeDictionary tracks them so they are
			// removed and re-added on every rebuild instead of accumulating.
			AddThemeDictionary(new ResourceDictionary { Source = new Uri($"ms-appx:///{ToolkitPackageName}/Generated/mergedpages.xaml") });
			AddThemeDictionary(new ResourceDictionary { Source = new Uri($"ms-appx:///{ToolkitSimplePackageName}/Generated/mergedpages.xaml") });
		}
	}
}
