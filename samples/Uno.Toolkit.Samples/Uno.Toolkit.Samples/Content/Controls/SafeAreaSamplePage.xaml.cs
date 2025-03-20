using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.Samples.Content.NestedSamples;
using Uno.Toolkit.UI;

#if __IOS__
using UIKit;



using XamlWindow = Microsoft.UI.Xaml.Window;


using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XamlWindow = Windows.UI.Xaml.Window;



namespace Uno.Toolkit.Samples.Content.Controls
{
	[SamplePage(SampleCategory.Controls, nameof(SafeArea))]
	public sealed partial class SafeAreaSamplePage : Page
	{
		public SafeAreaSamplePage()
		{
			this.InitializeComponent();
		}

		private void LaunchAPSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<SafeAreaSamplePage_NestedPage>(clearStack: true);
		}

		private void LaunchControlSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<SafeArea_Control_NestedPage>(clearStack: true);
		}

		private void LaunchSoftInputSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<SafeArea_SoftInput_NestedPage>(clearStack: true);
		}

		private void LaunchSoftInputScrollSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<SafeArea_SoftInput_Scroll>(clearStack: true);
		}

		private void LaunchModalSample(object sender, RoutedEventArgs e)
		{
#if __IOS__
			var vc = new UIViewController { View = new SafeArea_ModalPage() };
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
			{
				// Esnure the behavior of the iPad modal presentation mimics that of the iPhone
				vc.PreferredContentSize = XamlWindow.Current.Bounds.ToCGRect().Size;
				vc.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			}
			UIApplication.SharedApplication.KeyWindow.RootViewController.PresentModalViewController(vc, true);

		}
	}
}
