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
		public SimpleToolkitTheme() : this(colorOverride: null, fontOverride: null)
		{
		}

		public SimpleToolkitTheme(ResourceDictionary? colorOverride = null, ResourceDictionary? fontOverride = null)
			: base(colorOverride, fontOverride)
		{
		}

		/// <summary>
		/// The combined, static control styles for the Simple toolkit theme. This layers the
		/// Simple base styles, the toolkit base styles, and the toolkit Simple styles into a
		/// single source so they are applied once via <see cref="ResourceDictionary.Source"/>,
		/// rather than re-added on every theme rebuild through the dynamic resource hook.
		/// </summary>
		protected override string DefaultStylesSource => "ms-appx:///Uno.Toolkit.WinUI.Simple/Themes/SimpleToolkitStyles.xaml";
	}
}
