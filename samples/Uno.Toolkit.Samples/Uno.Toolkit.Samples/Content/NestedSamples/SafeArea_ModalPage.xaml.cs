using Uno.Toolkit.UI;

#if __IOS__
using UIKit;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SafeArea_ModalPage : Page
	{
		public SafeArea_ModalPage()
		{
			this.InitializeComponent();

			SafeArea.SetSafeAreaOverride(ContainerGrid, new Thickness(0, 0, 0, 30));
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			LayoutRectangle.Visibility = LayoutRectangle.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
		}

		private void CloseModalClick(object sender, RoutedEventArgs e)
		{
#if __IOS__
			UIApplication.SharedApplication.KeyWindow.RootViewController.DismissModalViewController(animated: false);

		}
	}
}
