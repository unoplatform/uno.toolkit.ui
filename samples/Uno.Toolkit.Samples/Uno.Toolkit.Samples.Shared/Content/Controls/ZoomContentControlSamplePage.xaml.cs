using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.Toolkit.Samples.Entities;
using Windows.Foundation;
using Windows.Foundation.Collections;

#if IS_WINUI
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif


namespace Uno.Toolkit.Samples.Content.Controls
{
	[SamplePage(SampleCategory.Controls, "ZoomContentControl")]
	public sealed partial class ZoomContentControlSamplePage : Page
	{
		public ZoomContentControlSamplePage()
		{
			this.InitializeComponent();
		}

		private void OnZoomInClick(object sender, RoutedEventArgs e)
		{
			if (ZoomContent.ZoomLevel < ZoomContent.MaxZoomLevel)
			{
				ZoomContent.ZoomLevel += 0.2;
			}
		}

		private void OnZoomOutClick(object sender, RoutedEventArgs e)
		{
			if (ZoomContent.ZoomLevel > ZoomContent.MinZoomLevel)
			{
				ZoomContent.ZoomLevel -= 0.2;
			}
		}

		private void OnResetClick(object sender, RoutedEventArgs e)
		{
			ZoomContent.ResetZoom();
		}
	}
}
