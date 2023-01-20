using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Text;
using Uno.Toolkit.UI.Material;

namespace Uno.Toolkit.UI.Material
{
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
}
