using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Text;
using Uno.Toolkit.UI.Material;

namespace Uno.Toolkit.UI.Material
{
	public static class MarkupInit
	{
		public static T UseMaterialToolkit<T>(this T app) where T : Application
			=> app
			.UseToolkit()
			.Resources(r => r.Merged(new MaterialToolkitResources()));
	}
}
