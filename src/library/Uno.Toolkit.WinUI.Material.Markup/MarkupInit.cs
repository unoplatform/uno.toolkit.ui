using Microsoft.UI.Xaml;

namespace Uno.Toolkit.UI.Material.Markup;

/// <summary>
/// Application helper extensions for easy initialization of <see cref="MaterialToolkitTheme"/>
/// </summary>
public static class MarkupInit
{
	/// <summary>
	/// Initializes and adds the <see cref="MaterialToolkitTheme"/> to the MergedDictionaries of <see cref="Application.Resources"/> 
	/// </summary>
	public static T UseMaterialToolkit<T>(this T app, ResourceDictionary? colorOverride = null, ResourceDictionary? fontOverride = null) where T : Application
		=> app.Resources(r => r.Merged(new MaterialToolkitTheme(colorOverride, fontOverride)));
}
