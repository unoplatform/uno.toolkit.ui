namespace Uno.Toolkit.Samples.Content;

[SamplePage(SampleCategory.None, "Overview", IconSymbol = Symbol.Home, SortOrder = 0, SupportedDesigns = new[] { Design.Material, Design.Cupertino })]
public sealed partial class OverviewPage : Page
{
	public OverviewPage()
	{
		this.InitializeComponent();
	}
}
