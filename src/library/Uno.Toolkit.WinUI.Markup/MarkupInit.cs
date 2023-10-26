using Microsoft.UI.Xaml;

namespace Uno.Toolkit.UI.Markup;

/// <summary>
/// Application helper extensions for easy initialization of <see cref="ToolkitResources"/>
/// </summary>
public static class MarkupInit
{
	/// <summary>
	/// Initializes and adds the <see cref="ToolkitResources"/> to the MergedDictionaries of <see cref="Application.Resources"/> 
	/// </summary>
	public static T UseToolkit<T>(this T app) where T : Application
		=> app.Resources(r => r.Merged(new ToolkitResources()));
}
