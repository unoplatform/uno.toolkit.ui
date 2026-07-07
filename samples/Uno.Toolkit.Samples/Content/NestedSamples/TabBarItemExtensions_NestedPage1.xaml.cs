namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	public sealed partial class TabBarItemExtensions_NestedPage1 : Page
	{
		public TabBarItemExtensions_NestedPage1()
		{
			this.InitializeComponent();
		}

		private void GotoNestedPage2(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(TabBarItemExtensions_NestedPage2));
		}
	}
}
