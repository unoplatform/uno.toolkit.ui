namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	public sealed partial class NativeFrame_Page1 : Page
	{
		public NativeFrame_Page1()
		{
			this.InitializeComponent();
		}

		private void NextClick(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(NativeFrame_Page2));
		}
		private void BackClick(object sender, RoutedEventArgs e)
		{
			this.Frame.GoBack();
		}
	}
}
