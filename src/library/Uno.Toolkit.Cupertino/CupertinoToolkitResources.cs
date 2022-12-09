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

namespace Uno.Toolkit.UI.Cupertino
{
	/// <summary>
	/// Cupertino styles for the controls in the Uno.Toolkit.UI library.
	/// </summary>
	public sealed class CupertinoToolkitResources : ResourceDictionary
	{
#if IS_WINUI
		private const string PackageName =
			"Uno.Toolkit.WinUI.Cupertino";
		private const string PackageNameSuffix =
			"WinUI";
#else
		private const string PackageName =
			"Uno.Toolkit.UI.Cupertino";
		private const string PackageNameSuffix =
			"UWP";
#endif

		public CupertinoToolkitResources()
		{
			Source = new Uri($"ms-appx:///{PackageName}/Generated/mergedpages.{PackageNameSuffix}.xaml");
		}
	}
}
