namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	public sealed partial class NativeFrame_MainPage : Page
	{
		public NativeFrame_MainPage()
		{
			this.InitializeComponent();
		}

		private void ExitNestedSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView()?.BackNavigateFromNestedSample();
		}

		private void NextClick(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(NativeFrame_Page1));
		}

		private void DeeplinkToPage2(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(NativeFrame_Page2));
			this.Frame.BackStack.Clear();
			this.Frame.BackStack.Add(new PageStackEntry(typeof(NativeFrame_MainPage), null, null));
			this.Frame.BackStack.Add(new PageStackEntry(typeof(NativeFrame_Page1), null, null));
		}
	}
}
