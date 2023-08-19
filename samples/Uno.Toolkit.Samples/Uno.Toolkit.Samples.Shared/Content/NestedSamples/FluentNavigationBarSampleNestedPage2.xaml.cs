using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.Toolkit.UI;
using Windows.Foundation;
using Windows.Foundation.Collections;
#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	public sealed partial class FluentNavigationBarSampleNestedPage2 : Page
	{
		public FluentNavigationBarSampleNestedPage2()
		{
			this.InitializeComponent();
		}

		private void NavigateBack(object sender, RoutedEventArgs e) => Frame.GoBack();
	}
}
