namespace Uno.Toolkit.Samples.Content
{
	[SamplePage(SampleCategory.None, "RuntimeTest Runner", SourceSdk.UnoToolkit, IconPath = Icons.Tests.RuntimeTest, SupportedDesigns = new[] { Design.Material, Design.Cupertino })]
	public sealed partial class RuntimeTestRunner : Page
	{
		public RuntimeTestRunner()
		{
			this.InitializeComponent();
		}
	}
}
