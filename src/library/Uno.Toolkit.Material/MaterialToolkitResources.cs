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
		private const string PackageName =
#if IS_WINUI
			"Uno.Toolkit.WinUI.Material";
#else
			"Uno.Toolkit.UI.Material";
#endif

		public MaterialToolkitResources()
		{
			this.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{PackageName}/Styles/Controls/BottomTabBar.xaml") });
			this.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{PackageName}/Styles/Controls/TopTabBar.xaml") });
			this.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{PackageName}/Styles/Controls/Divider.xaml") });
			this.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{PackageName}/Styles/Controls/NavigationBar.xaml") });
		}
	}
}
