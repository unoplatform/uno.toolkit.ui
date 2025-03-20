using Uno.Toolkit.Samples.Content.NestedSamples;
using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.UI;


using Microsoft.UI;


namespace Uno.Toolkit.Samples.Content.Controls
{
	[SamplePage(SampleCategory.Behaviors, nameof(Uno.Toolkit.UI.StatusBar), source: SourceSdk.UnoToolkit)]
	public sealed partial class StatusBarSamplePage : Page, IExitNestedSampleHandler
	{
		public StatusBarSamplePage()
		{
			this.InitializeComponent();
		}

		public void OnExitedFromNestedSample(object sender)
		{
			StatusBar.SetForeground(StatusBarForegroundTheme.Light);
			StatusBar.SetBackground(Colors.Gray);
		}

		private void ShowSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<StatusBarSample_NestedPage1>(clearStack: true);
		}
	}
}
