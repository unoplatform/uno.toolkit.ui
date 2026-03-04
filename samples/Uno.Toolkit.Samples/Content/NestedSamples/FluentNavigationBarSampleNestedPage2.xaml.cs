namespace Uno.Toolkit.Samples.Content.NestedSamples;

public sealed partial class FluentNavigationBarSampleNestedPage2 : Page
{
    public FluentNavigationBarSampleNestedPage2()
    {
        this.InitializeComponent();
    }

	private void NavigateBack(object sender, RoutedEventArgs e) => Frame.GoBack();

	private void NavigateToThird(object sender, RoutedEventArgs e) => Frame.Navigate(typeof(FluentNavigationBarSampleNestedPage3));
}
