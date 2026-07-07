namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	public sealed partial class M3MaterialNavigationBarSample_Primary : Page
	{
		public M3MaterialNavigationBarSample_Primary()
		{
			this.InitializeComponent();
		}


		private void NavigateBack(object sender, RoutedEventArgs e) => Shell.GetForCurrentView().BackNavigateFromNestedSample();
	}
}
