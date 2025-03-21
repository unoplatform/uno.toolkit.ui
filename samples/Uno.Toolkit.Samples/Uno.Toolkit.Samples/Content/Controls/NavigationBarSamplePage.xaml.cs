namespace Uno.Toolkit.Samples.Content.Controls
{
	[SamplePage(SampleCategory.Controls, nameof(NavigationBar))]
	public sealed partial class NavigationBarSamplePage : Page
	{
		public NavigationBarSamplePage()
		{
			this.InitializeComponent();
		}

		private void ModalFlyout_Opened(object sender, object e)
		{
			var flyoutContent = (sender as Flyout)?.Content;
			var modalFrame = VisualTreeHelperEx.GetFirstDescendant<Frame>(flyoutContent, x => x.Name == "ModalFrame");
			modalFrame?.Navigate(typeof(MaterialNavigationBarSample_ModalPage1));
		}

		private void M3ModalFlyout_Opened(object sender, object e)
		{
			var flyoutContent = (sender as Flyout)?.Content;
			var modalFrameM3 = VisualTreeHelperEx.GetFirstDescendant<Frame>(flyoutContent, x => x.Name == "M3ModalFrame");
			modalFrameM3?.Navigate(typeof(M3MaterialNavigationBarSample_ModalPage1));
		}

		private void LaunchFullScreenMaterialSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<MaterialNavigationBarSample_NestedPage1>(clearStack: true);
		}

		private void LaunchFullScreenMaterialDataContextSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<MaterialNavigationBarSample_DataContext_NestedPage1>(clearStack: true);
		}

		private void LaunchFullScreenM3MaterialSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<M3MaterialNavigationBarSample_NestedPage1>(clearStack: true);
		}

		private void LaunchFullScreenM3MaterialDataContextSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<M3MaterialNavigationBarSample_DataContext_NestedPage1>(clearStack: true);
		}

		private void LaunchFullScreenM3MaterialPrimarySample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<M3MaterialNavigationBarSample_Primary>(clearStack: true);
		}

		private void LaunchFullScreenFluentSample (object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<FluentNavigationBarSampleNestedPage>(clearStack: true);
		}
    }
}
