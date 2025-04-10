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
