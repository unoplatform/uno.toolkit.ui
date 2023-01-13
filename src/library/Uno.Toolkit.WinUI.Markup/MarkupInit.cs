using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Text;

namespace Uno.Toolkit.UI
{
	public static class MarkupInit
	{
		public static T UseToolkit<T>(this T app) where T : Application
			=> app.Resources(r => r.Merged(new ToolkitResources()));
	}
}
