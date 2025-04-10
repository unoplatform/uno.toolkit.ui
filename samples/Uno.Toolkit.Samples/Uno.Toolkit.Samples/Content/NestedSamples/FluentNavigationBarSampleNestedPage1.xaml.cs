// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Uno.Toolkit.Samples.Content.NestedSamples;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class FluentNavigationBarSampleNestedPage : Page
{
    public FluentNavigationBarSampleNestedPage()
    {
        this.InitializeComponent();
	}

	private void NavigateToNextPage(object sender, RoutedEventArgs e) => Frame.Navigate(typeof(FluentNavigationBarSampleNestedPage2));

	private void NavigateBack(object sender, RoutedEventArgs e) => Shell.GetForCurrentView().BackNavigateFromNestedSample();

	private void OpenPage2Flyout(object sender, RoutedEventArgs e) => Page2FlyoutFrame.Navigate(typeof(FluentNavigationBarSampleNestedPage2));
	
	private void OpenPage3Flyout(object sender, RoutedEventArgs e) => Page3FlyoutFrame.Navigate(typeof(FluentNavigationBarSampleNestedPage3));
}
