
using Uno.Toolkit.Samples.Entities;

using Microsoft.UI;

namespace Uno.Toolkit.Samples.Content.TestPages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>

	[SamplePage(SampleCategory.Tests, "ShadowContainerRectangleTest")]
	public sealed partial class ShadowContainerRectangleTestPage : Page
	{
		public ShadowContainerRectangleTestPage()
		{
			this.InitializeComponent();
		}

		private void runButton_Click(object sender, RoutedEventArgs e)
		{
			statusText.Text = "Running";
			shadowContainer.Shadows.Clear();

			if (!int.TryParse(xOffsetText.Text, out var xOffset))
			{
				xOffset = 0;
			}

			if (!int.TryParse(yOffsetText.Text, out var yOffset))
			{
				yOffset = 0;
			}

			var isInner = inner.IsChecked ?? false;

			shadowContainer.Shadows.Add(new UI.Shadow
			{
				OffsetX = xOffset,
				OffsetY = yOffset,
				IsInner = isInner,
				Opacity = 1,
				Color = Colors.Red,
			});

			statusText.Text = "Verify";
		}

		private void reset_Click(object sender, RoutedEventArgs e)
		{
			statusText.Text = string.Empty;

			xOffsetText.Text = string.Empty;
			yOffsetText.Text = string.Empty;
			inner.IsChecked = false;

			shadowContainer.Shadows.Clear();
		}
	}
}

