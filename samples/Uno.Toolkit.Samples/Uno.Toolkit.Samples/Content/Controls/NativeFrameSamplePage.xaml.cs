namespace Uno.Toolkit.Samples.Content.Controls
{
	[SamplePage(SampleCategory.Controls, "NativeFrame")]
	public sealed partial class NativeFrameSamplePage : Page
	{
		public NativeFrameSamplePage()
		{
			this.InitializeComponent();
		}

		private void LaunchNestedSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<NativeFrame_MainPage>(clearStack: true);
		}
	}
}
