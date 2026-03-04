namespace Uno.Toolkit.Samples.Content.Controls
{
	[SamplePage(SampleCategory.Controls, "Card", SourceSdk.UnoMaterial, Description = "This control is used to display content and actions about a single item.", DocumentationLink = "https://material.io/components/cards", SupportedDesigns = new[] { Design.Material })]
	public sealed partial class CardSamplePage : Page
	{
		public CardSamplePage()
		{
			this.InitializeComponent();
		}
	}
}
