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
	/// Material (Material Design 2) styles for the controls in the Uno.Toolkit.UI library.
	/// </summary>
	public class MaterialToolkitResourcesV1 : ResourceDictionary
	{
		private const string PackageName = "Uno.Toolkit.WinUI.Material";

		public MaterialToolkitResourcesV1()
		{
			Source = new Uri($"ms-appx:///{PackageName}/Generated/mergedpages.v1.xaml");
		}
	}
}
