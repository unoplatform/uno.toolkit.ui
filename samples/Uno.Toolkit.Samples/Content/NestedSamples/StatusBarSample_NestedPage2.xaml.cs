namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	public sealed partial class StatusBarSample_NestedPage2 : Page
	{
		public StatusBarSample_NestedPage2()
		{
			this.InitializeComponent();
		}

		private void NavigateBack(object sender, RoutedEventArgs e) => Frame.GoBack();
	}
}
