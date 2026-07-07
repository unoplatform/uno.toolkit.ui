namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	public sealed partial class TabBarItemExtensions_NestedPage2 : Page
	{
		public TabBarItemExtensions_NestedPage2()
		{
			this.InitializeComponent();
		}

		private void GoBack(object sender, RoutedEventArgs e)
		{
			Frame.GoBack();
		}
	}
}
