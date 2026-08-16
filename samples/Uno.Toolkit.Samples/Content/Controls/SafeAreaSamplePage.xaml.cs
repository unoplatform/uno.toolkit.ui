#if __IOS__
using UIKit;
#endif

namespace Uno.Toolkit.Samples.Content.Controls
{
	[SamplePage(SampleCategory.Controls, nameof(SafeArea), SupportedDesigns = new[] { Design.Material, Design.Cupertino })]
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
	}
}
