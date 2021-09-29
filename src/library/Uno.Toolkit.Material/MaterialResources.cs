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
/// Material resources
/// </summary>
namespace Uno.Toolkit.Material
{
	public sealed class MaterialResources : ResourceDictionary
	{
		private const string PackageName = "Uno.UI.Toolkit.Material";

		public MaterialResources()
		{
			//this.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{PackageName}/Styles/Controls/BottomTabBar.xaml") });
			//this.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{PackageName}/Styles/Controls/StandardTabBar.xaml") });
			this.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("ms-appx:///Uno.UI.Toolkit.Material/Styles/Controls/BottomTabBar.xaml") });
			this.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("ms-appx:///Uno.UI.Toolkit.Material/Styles/Controls/StandardTabBar.xaml") });
		}
	}
}
