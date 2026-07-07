namespace Uno.Toolkit.Samples.Content.Helpers;

[SamplePage(SampleCategory.Helpers, "Responsive Extensions", SourceSdk.UnoToolkit, IconPath = Icons.Helpers.MarkupExtension, SupportedDesigns = new[] { Design.Material, Design.Cupertino })]
public sealed partial class ResponsiveExtensionsSamplePage : Page
{
	public ResponsiveExtensionsSamplePage()
	{
		this.InitializeComponent();
	}
}
