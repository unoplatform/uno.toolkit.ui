namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	public sealed partial class MaterialNavigationBarSample_NestedPage1 : Page
	{
		public MaterialNavigationBarSample_NestedPage1()
		{
			this.InitializeComponent();
		}

		private void NavigateToNextPage(object sender, RoutedEventArgs e) => Frame.Navigate(typeof(M3MaterialNavigationBarSample_NestedPage2));

		private void NavigateBack(object sender, RoutedEventArgs e) => Shell.GetForCurrentView().BackNavigateFromNestedSample();

	}
}
