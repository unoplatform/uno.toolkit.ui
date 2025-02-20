namespace ToolkitSampleApp;

public sealed partial class SecondPage : Page
{
    public SecondPage()
    {
        this.InitializeComponent();
    }

    private void GoBack(object sender, RoutedEventArgs e)
    {
        Frame.GoBack();
    }
}
