namespace Uno.Toolkit.Samples.Content.TestPages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>

	[SamplePage(SampleCategory.Tests, "ShadowContainerTest")]
	public sealed partial class ShadowContainerTestPage : Page
	{
		private enum ElementState
		{
			Border,
			Rectangle,
			BorderIrregular
		}
		private ElementState SelectedElement;
		public ShadowContainerTestPage()
		{
			this.InitializeComponent();
		}

		private void runButton_Click(object sender, RoutedEventArgs e)
		{
			UpdateVisibilityElement();
			statusText.Text = "Running";

			shadowContainer.Shadows.Clear();
			shadowContainerRectangle.Shadows.Clear();
			shadowContainerIrregularCorner.Shadows.Clear();

			if (!int.TryParse(xOffsetText.Text, out var xOffset))
			{
				xOffset = 0;
			}

			if (!int.TryParse(yOffsetText.Text, out var yOffset))
			{
				yOffset = 0;
			}

			var isInner = inner.IsChecked ?? false;
			var shadow = new UI.Shadow
			{
				OffsetX = xOffset,
				OffsetY = yOffset,
				IsInner = isInner,
				Opacity = 1,
				Color = Colors.Red,
			};
			switch (SelectedElement)
			{
				case ElementState.Border:
					shadowContainer.Shadows.Add(shadow);
					break;
				case ElementState.Rectangle:
					shadowContainerRectangle.Shadows.Add(shadow);
					break;
				case ElementState.BorderIrregular:
					shadowContainerIrregularCorner.Shadows.Add(shadow);
					break;

			}

			statusText.Text = "Verify";
		}

		private void reset_Click(object sender, RoutedEventArgs e)
		{

			statusText.Text = string.Empty;

			xOffsetText.Text = string.Empty;
			yOffsetText.Text = string.Empty;
			inner.IsChecked = false;
			check_Border.IsChecked = true;
			SelectedElement = ElementState.Border;

			shadowContainer.Shadows.Clear();
			shadowContainerRectangle.Shadows.Clear();
			shadowContainerIrregularCorner.Shadows.Clear();

			UpdateVisibilityElement();

		}

		private void Border_ClickElement(object sender, RoutedEventArgs e)
		{
			SelectedElement = ElementState.Border;
			UpdateVisibilityElement();
		}
		private void Rectangle_ClickElement(object sender, RoutedEventArgs e)
		{
			SelectedElement = ElementState.Rectangle;
			UpdateVisibilityElement();
		}
		private void IrregularCorner_ClickElement(object sender, RoutedEventArgs e)
		{
			SelectedElement = ElementState.BorderIrregular;
			UpdateVisibilityElement();
		}

		private void UpdateVisibilityElement()
		{
			statusText.Text = string.Empty;
			containerIrregularCorner.Visibility = Visibility.Collapsed;
			containerRectangle.Visibility = Visibility.Collapsed;
			containerBorder.Visibility = Visibility.Collapsed;
			switch (SelectedElement)
			{
				case ElementState.Border:
					containerBorder.Visibility = Visibility.Visible;
					break;
				case ElementState.Rectangle:
					containerRectangle.Visibility = Visibility.Visible;
					break;
				case ElementState.BorderIrregular:
					containerIrregularCorner.Visibility = Visibility.Visible;
					break;

			}
		}
	}
}

