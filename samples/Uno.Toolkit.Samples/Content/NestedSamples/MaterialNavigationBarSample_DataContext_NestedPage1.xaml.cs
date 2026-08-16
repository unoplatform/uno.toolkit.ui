// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Uno.Toolkit.Samples.Content.NestedSamples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MaterialNavigationBarSample_DataContext_NestedPage1 : Page
    {
        public MaterialNavigationBarSample_DataContext_NestedPage1()
        {
            this.InitializeComponent();
			this.DataContext = new Page1VM();
        }

		private void NavigateBack(object sender, RoutedEventArgs e) => Shell.GetForCurrentView().BackNavigateFromNestedSample();

		public class Page1VM
		{
			public string ViewModelName { get; } = nameof(Page1VM);
		}
	}
}
