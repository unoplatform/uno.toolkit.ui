namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	public sealed partial class StatusBarSample_NestedPage1 : Page
	{
		public StatusBarSample_NestedPage1()
		{
			this.InitializeComponent();
		}

		private void NavigateBack(object sender, RoutedEventArgs e) => Shell.GetForCurrentView().BackNavigateFromNestedSample();

		private void NavigateToNextPage(object sender, RoutedEventArgs e) => Frame.Navigate(typeof(StatusBarSample_NestedPage2));
	}
}
