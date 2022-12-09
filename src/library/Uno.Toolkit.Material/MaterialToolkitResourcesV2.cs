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

namespace Uno.Toolkit.UI.Material
{
	/// <summary>
	/// Material (Material Design 3) styles for the controls in the Uno.Toolkit.UI library.
	/// </summary>
	public class MaterialToolkitResourcesV2 : ResourceDictionary
	{
#if IS_WINUI
		private const string PackageName =
			"Uno.Toolkit.WinUI.Material";
		private const string PackageNameSuffix =
			"WinUI";
#else
		private const string PackageName =
			"Uno.Toolkit.UI.Material";
		private const string PackageNameSuffix =
			"UWP";
#endif
		public MaterialToolkitResourcesV2()
		{
			Source = new Uri($"ms-appx:///{PackageName}/Generated/mergedpages.{PackageNameSuffix}.v2.xaml");
		}
	}
}
