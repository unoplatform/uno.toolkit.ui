namespace ToolkitSampleApp;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(SecondPage));
    }
}
