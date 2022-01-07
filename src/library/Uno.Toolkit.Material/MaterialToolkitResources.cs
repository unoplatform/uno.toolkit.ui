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

/// <summary>
/// Material Toolkit resources
/// </summary>
namespace Uno.Toolkit.UI.Material
{
	public sealed class MaterialToolkitResources : ResourceDictionary
	{
#if IS_WINUI
		private const string PackageName =
			"Uno.Toolkit.WinUI.Material";
		private const string PackageNameSuffix =
			"winui";
#else
		private const string PackageName =
			"Uno.Toolkit.UI.Material";
		private const string PackageNameSuffix =
			"uwp";
#endif

		public MaterialToolkitResources()
		{
			Source = new Uri($"ms-appx:///{PackageName}/Generated/mergedpages.{PackageNameSuffix}.xaml");
		}
	}
}
