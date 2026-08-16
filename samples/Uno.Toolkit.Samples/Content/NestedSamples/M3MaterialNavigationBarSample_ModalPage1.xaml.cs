// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class M3MaterialNavigationBarSample_ModalPage1 : Page
	{
		public M3MaterialNavigationBarSample_ModalPage1()
		{
			this.InitializeComponent();
		}

		private void NavigateToNextPage(object sender, RoutedEventArgs e) => Frame.Navigate(typeof(M3MaterialNavigationBarSample_ModalPage2));

		private void NavigateBack(object sender, RoutedEventArgs e) => Shell.GetForCurrentView().BackNavigateFromNestedSample();
	}
}
