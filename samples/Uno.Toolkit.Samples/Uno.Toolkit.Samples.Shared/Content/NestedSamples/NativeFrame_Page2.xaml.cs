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
	public sealed partial class NativeFrame_Page2 : Page
	{
		public NativeFrame_Page2()
		{
			this.InitializeComponent();
		}

		private void BackClick(object sender, RoutedEventArgs e)
		{
			this.Frame.GoBack();
		}

		private void ChangeBackStackClick(object sender, RoutedEventArgs e)
		{
			var entry = this.Frame.BackStack.Last();
			var newEntry = new PageStackEntry(entry.SourcePageType, new Dictionary<string, object>(), entry.NavigationTransitionInfo);
			this.Frame.BackStack.Remove(entry);
			this.Frame.BackStack.Add(newEntry);
			this.Frame.GoBack();
		}
	}
}
