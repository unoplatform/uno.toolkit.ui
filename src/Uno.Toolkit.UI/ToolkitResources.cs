using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Default styles for the controls in the Uno.Toolkit.UI library.
	/// </summary>
	public sealed class ToolkitResources : ResourceDictionary
	{
		private const string PackageName =
#if IS_WINUI
			"Uno.Toolkit.WinUI";
#else
			"Uno.Toolkit.UI";
#endif
		private const string PackageNameSuffix =
#if IS_WINUI
			"WinUI";
#else
			"UWP";
#endif


		public ToolkitResources()
		{
			Source = new Uri($"ms-appx:///{PackageName}/Generated/mergedpages.{PackageNameSuffix}.xaml");
		}
	}
}
