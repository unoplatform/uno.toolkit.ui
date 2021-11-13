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
/// Cupertino resources
/// </summary>
namespace Uno.Toolkit.UI.Cupertino
{
	public sealed class CupertinoToolkitResources : ResourceDictionary
	{
		private const string PackageName =
#if IS_WINUI
			"Uno.Toolkit.WinUI.Cupertino";
#else
			"Uno.Toolkit.UI.Cupertino";
#endif

		public CupertinoToolkitResources()
		{
			this.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{PackageName}/Styles/Controls/BottomTabBar.xaml") });
			this.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{PackageName}/Styles/Controls/SlidingSegmentedControl.xaml") });
			this.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{PackageName}/Styles/Controls/SegmentedControl.xaml") });
		}
	}
}
