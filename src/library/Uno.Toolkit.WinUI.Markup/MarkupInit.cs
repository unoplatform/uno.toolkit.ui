using Microsoft.UI.Xaml;
using Uno.Extensions.Markup;

namespace Uno.Toolkit.UI.Markup;

/// <summary>
/// Application helper extensions for easy initialization of <see cref="ToolkitResources"/>
/// </summary>
public static class MarkupInit
{
	/// <summary>
	/// Initializes and adds the <see cref="ToolkitResources"/> to the MergedDictionaries of <see cref="Application.Resources"/> 
	/// </summary>
	public static ResourceDictionaryBuilder UseToolkit(this ResourceDictionaryBuilder builder)
		=> builder.Merged(new ToolkitResources());
}
