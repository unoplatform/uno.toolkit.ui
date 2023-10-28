using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.Toolkit.Samples.Entities;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Uno.Toolkit.Samples.Content.NestedSamples;
using Uno.Toolkit.UI;
using Uno.Toolkit.Samples.Helpers;
using UIKit;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
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
	[SamplePage(SampleCategory.Controls, nameof(NavigationBar))]
	public sealed partial class NavigationBarSamplePage : Page
	{
		public NavigationBarSamplePage()
		{
			this.InitializeComponent();
		}

		private void ModalFlyout_Opened(object sender, object e)
		{
			var flyoutContent = (sender as Flyout)?.Content;
			var modalFrame = VisualTreeHelperEx.GetFirstDescendant<Frame>(flyoutContent, x => x.Name == "ModalFrame");
			modalFrame?.Navigate(typeof(MaterialNavigationBarSample_ModalPage1));
		}

		private async void M3ModalFlyout_Opened(object sender, object e)
		{
			if (sender is Flyout flyout)
			{
				var modalFrameM3 = VisualTreeHelperEx.GetFirstDescendant<Grid>(flyout.Content, x => x.Name == "M3ModalFrame");
				Console.WriteLine(modalFrameM3.ShowLocalVisualTree());
			
				await Task.Delay(3000);

				modalFrameM3.AddChild(new Border
					{
						Background = new SolidColorBrush(Microsoft.UI.Colors.Red),
						Width = 200,
						Height = 200
					});

					Console.WriteLine(modalFrameM3.ShowLocalVisualTree());

			}
			//var flyoutContent = (sender as Flyout)?.Content;

			// var modalFrameM3 = VisualTreeHelperEx.GetFirstDescendant<Frame>(flyoutContent, x => x.Name == "M3ModalFrame");
			// modalFrameM3.Navigated += async (s, e) =>
			// {
			// 	//modalFrameM3.BackStack.Clear();
			// 	await Task.Delay(5000);
			// 	var page = modalFrameM3.ShowLocalVisualTree();
			// 	Console.WriteLine(page);
			// };
			// modalFrameM3?.Navigate(typeof(M3MaterialNavigationBarSample_ModalPage1));
		}

		private void LaunchFullScreenMaterialSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<MaterialNavigationBarSample_NestedPage1>(clearStack: true);
		}

		private void LaunchFullScreenMaterialDataContextSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<MaterialNavigationBarSample_DataContext_NestedPage1>(clearStack: true);
		}

		private void LaunchFullScreenM3MaterialSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<M3MaterialNavigationBarSample_NestedPage1>(clearStack: true);
		}

		private void LaunchFullScreenM3MaterialDataContextSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<M3MaterialNavigationBarSample_DataContext_NestedPage1>(clearStack: true);
		}

		private void LaunchFullScreenM3MaterialPrimarySample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<M3MaterialNavigationBarSample_Primary>(clearStack: true);
		}

		private void LaunchFullScreenFluentSample (object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<FluentNavigationBarSampleNestedPage>(clearStack: true);
		}
    }
}
