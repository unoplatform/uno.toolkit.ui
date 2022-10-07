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
using XamlWindow = Windows.UI.Xaml.Window;
#endif
#if __IOS__
using UIKit;
#endif

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SafeArea_ModalPage : Page
	{
		public SafeArea_ModalPage()
		{
			this.InitializeComponent();

			SafeArea.SetSafeAreaOverride(ContainerGrid, new Thickness(0, 0, 0, 30));
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			LayoutRectangle.Visibility = LayoutRectangle.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
		}

		private void CloseModalClick(object sender, RoutedEventArgs e)
		{
#if __IOS__
			UIApplication.SharedApplication.KeyWindow.RootViewController.DismissModalViewController(animated: false);
#endif
		}
	}
}
