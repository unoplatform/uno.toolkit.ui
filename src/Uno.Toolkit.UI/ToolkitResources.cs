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

namespace Uno.UI.ToolkitLib
{
	public sealed class ToolkitResources : ResourceDictionary
	{
		private const string PackageName =
#if IS_WINUI
			"Uno.WinUI.ToolkitLib";
#else
			"Uno.UI.ToolkitLib";
#endif

		public ToolkitResources()
		{
			var dicts = new string[]
			{
				"DrawerControl/DrawerControl.xaml",
				"DrawerControl/DrawerControl.Enhanced.xaml",
				"TabBar/TabBar.xaml",
			};
			foreach (var dict in dicts)
			{
				MergedDictionaries.Add(new ResourceDictionary
				{
					Source = new Uri($"ms-appx:///{PackageName}/{dict}")
				});
			}
		}
	}
}
