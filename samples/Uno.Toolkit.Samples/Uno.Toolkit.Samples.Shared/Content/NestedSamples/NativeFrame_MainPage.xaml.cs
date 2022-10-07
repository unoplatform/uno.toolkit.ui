using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
#endif


namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	public sealed partial class NativeFrame_MainPage : Page
	{
		public NativeFrame_MainPage()
		{
			this.InitializeComponent();
		}

		private void ExitNestedSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView()?.BackNavigateFromNestedSample();
		}

		private void NextClick(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(NativeFrame_Page1));
		}

		private void DeeplinkToPage2(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(NativeFrame_Page2));
			this.Frame.BackStack.Clear();
			this.Frame.BackStack.Add(new PageStackEntry(typeof(NativeFrame_MainPage), null, null));
			this.Frame.BackStack.Add(new PageStackEntry(typeof(NativeFrame_Page1), null, null));
		}
	}
}
