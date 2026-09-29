namespace Uno.Toolkit.Samples.Content.Controls;

[SamplePage(SampleCategory.Controls,
	"FlexPanel",
	SourceSdk.UnoToolkit,
	Description = "A panel that arranges its children using CSS Flexbox semantics, backed by the Yoga layout engine.",
	SupportedDesigns = new[] { Design.Agnostic })]
public sealed partial class FlexPanelSamplePage : Page
{
	public FlexPanelSamplePage()
	{
		this.InitializeComponent();
	}

	private void LaunchPlayground(object sender, RoutedEventArgs e)
	{
		Shell.GetForCurrentView()?.ShowNestedSample<FlexPanelPlaygroundNestedPage>(clearStack: true);
	}
}
