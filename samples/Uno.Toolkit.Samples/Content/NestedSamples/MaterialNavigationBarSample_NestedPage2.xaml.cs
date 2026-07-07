// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Uno.Toolkit.Samples.Content.NestedSamples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MaterialNavigationBarSample_NestedPage2 : Page
    {
        public MaterialNavigationBarSample_NestedPage2()
        {
            this.InitializeComponent();
        }

        private void NavigateBack(object sender, RoutedEventArgs e) => Frame.GoBack();
    }
}
