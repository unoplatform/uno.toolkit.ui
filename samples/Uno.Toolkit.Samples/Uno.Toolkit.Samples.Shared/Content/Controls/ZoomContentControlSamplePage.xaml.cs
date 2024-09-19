using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Uno.Toolkit.Samples.Content.Controls
{
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
