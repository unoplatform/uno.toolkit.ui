﻿using System;
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

		public ToolkitResources()
		{
			var dicts = new string[]
			{
				"DrawerControl/DrawerControl.xaml",
				"DrawerControl/DrawerControl.Enhanced.xaml",
				"TabBar/TabBar.xaml",
				"AutoLayout/AutoLayout.xaml",
#if __IOS__ || __ANDROID__
				"NavigationBar/NavigationBar.Native.xaml",
#else
				"NavigationBar/NavigationBar.xaml",
#endif
			};
			foreach (var dict in dicts)
			{
				MergedDictionaries.Add(new ResourceDictionary
				{
					Source = new Uri($"ms-appx:///{PackageName}/Controls/{dict}")
				});
			}
		}
	}
}
