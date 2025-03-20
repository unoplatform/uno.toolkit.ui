using Uno.Toolkit.Samples.Entities.Data;
using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.UI;


namespace Uno.Toolkit.Samples.Content.Controls
{
	[SamplePage(SampleCategory.Controls, "Chip", SourceSdk.UnoMaterial, DataType = typeof(TestCollections))]
	public sealed partial class ChipSamplePage : Page
	{
		public ChipSamplePage()
		{
			this.InitializeComponent();
		}

		private void RemoveChipItem(object sender, ChipItemEventArgs e)
		{
			if (DataContext is Sample sample)
			{
				if (sample.Data is TestCollections test)
				{
					test.RemoveChipItem(e.Item as TestCollections.SelectableData);
				}
			}
		}

		private void ResetChipItems(object sender, RoutedEventArgs e)
		{
			if (DataContext is Sample sample)
			{
				if (sample.Data is TestCollections test)
				{
					test.ResetChipItems();
				}
			}
		}
	}
}
