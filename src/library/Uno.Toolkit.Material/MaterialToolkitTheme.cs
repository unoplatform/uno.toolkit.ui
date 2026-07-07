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
		public MaterialToolkitTheme() : this(colorOverride: null, fontOverride: null)
		{
		}

		public MaterialToolkitTheme(ResourceDictionary? colorOverride = null, ResourceDictionary? fontOverride = null)
			: base(colorOverride, fontOverride)
		{
		}

		/// <summary>
		/// The combined, static control styles for the Material toolkit theme. This layers the
		/// Material base styles, the toolkit base styles, and the toolkit Material styles into a
		/// single source so they are applied once via <see cref="ResourceDictionary.Source"/>,
		/// rather than re-added on every theme rebuild through the dynamic resource hook.
		/// </summary>
		protected override string DefaultStylesSource => "ms-appx:///Uno.Toolkit.WinUI.Material/Themes/MaterialToolkitStyles.xaml";
	}
}
