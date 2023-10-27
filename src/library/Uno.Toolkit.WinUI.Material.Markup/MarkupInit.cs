using Microsoft.UI.Xaml;
using Uno.Extensions.Markup;
using Uno.Material;

namespace Uno.Toolkit.UI.Material.Markup;

/// <summary>
/// Application helper extensions for easy initialization of <see cref="MaterialToolkitTheme"/>
/// </summary>
public static class MarkupInit
{
	/// <summary>
	/// Initializes and adds the <see cref="MaterialToolkitTheme"/> to the MergedDictionaries of <see cref="Application.Resources"/> 
	/// </summary>
	public static ResourceDictionaryBuilder UseMaterialToolkit(
		this ResourceDictionaryBuilder builder,
		ResourceDictionary? colorOverride = null,
		ResourceDictionary? fontOverride = null)
		=> builder.Merged(new MaterialToolkitTheme(colorOverride, fontOverride));
}
