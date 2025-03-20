using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.Toolkit.UI;
using Uno.UI.Toolkit;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using XamlWindow = Microsoft.UI.Xaml.Window;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static Uno.Toolkit.UI.SafeArea;
using XamlWindow = Windows.UI.Xaml.Window;
#endif

namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	public sealed partial class SafeArea_SoftInput_NestedPage : Page
	{
		public SafeArea_SoftInput_NestedPage()
		{
			this.InitializeComponent();
		}


		private void NavigateBack(object sender, RoutedEventArgs e)
		{
			// Normally we would've just called `Frame.GoBack();` if we only have a single frame.
			// However, a nested frame is used to show-case fullscreen sample, so we need some
			// custom handling to hide the nested frame on back navigation when the stack is empty.
			Shell.GetForCurrentView()?.BackNavigateFromNestedSample();
		}
	}
}
