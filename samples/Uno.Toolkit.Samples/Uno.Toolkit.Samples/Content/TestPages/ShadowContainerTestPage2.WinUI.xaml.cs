
using Uno.Toolkit.Samples.Entities;

namespace Uno.Toolkit.Samples.Content.TestPages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>

	[SamplePage(SampleCategory.Tests, "ShadowContainerTest2")]
	public sealed partial class ShadowContainerTestPage2 : Page
	{
		public ShadowContainerTestPage2()
		{
			this.InitializeComponent();
		}

		private void OnReload(object sender, RoutedEventArgs e)
		{
			testStatus.Text = "Running test...";

			try
			{
				sp.Children.Remove(shadowContainer);
				sp.Children.Add(shadowContainer);
				sp.Children.Remove(shadowContainer);
				sp.Children.Add(shadowContainer);
			}
			finally
			{
				testStatus.Text = "Completed";
			}
		}
	}
}

