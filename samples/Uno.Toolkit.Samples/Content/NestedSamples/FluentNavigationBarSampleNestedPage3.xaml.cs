namespace Uno.Toolkit.Samples.Content.NestedSamples;

public sealed partial class FluentNavigationBarSampleNestedPage3 : Page
{
    public FluentNavigationBarSampleNestedPage3()
    {
        this.InitializeComponent();
    }

	private void NavigateBack(object sender, RoutedEventArgs e) => Frame.GoBack();
}
