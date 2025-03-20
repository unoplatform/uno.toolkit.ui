using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.UI;



using Microsoft.UI;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Uno.Toolkit.Samples.Content.Controls
{
	[SamplePage(SampleCategory.Controls, nameof(DrawerControl))]
	public sealed partial class DrawerControlSamplePage : Page
	{
		public DrawerControlSamplePage()
		{
			this.InitializeComponent();
			this.Loaded += (s, e) => SetupOptions();
		}

		private void SetupOptions()
		{
			var design = Design.Agnostic;

			var sampleDrawerControl = SamplePageLayout.GetSampleChild<DrawerControl>(design, "SampleDrawerControl");
			var optionOpenDirection = SamplePageLayout.GetSampleChild<ComboBox>(design, "OptionOpenDirection");
			var optionDrawerDepthLengthIsNull = SamplePageLayout.GetSampleChild<CheckBox>(design, "OptionDrawerDepthLengthIsNull");
			var optionDrawerDepthValue = SamplePageLayout.GetSampleChild<Slider>(design, "OptionDrawerDepthValue");
			var optionLightDismissOverlayBackground = SamplePageLayout.GetSampleChild<ComboBox>(design, "OptionLightDismissOverlayBackground");
			var optionEdgeSwipeDetectionLengthIsNull = SamplePageLayout.GetSampleChild<CheckBox>(design, "OptionEdgeSwipeDetectionLengthIsNull");
			var optionEdgeSwipeDetectionLengthValue = SamplePageLayout.GetSampleChild<Slider>(design, "OptionEdgeSwipeDetectionLengthValue");

			optionOpenDirection.ItemsSource = typeof(DrawerOpenDirection).GetEnumValues();
			optionOpenDirection.SelectedIndex = 0;
			optionOpenDirection.SelectionChanged += (s, e) =>
			{
				sampleDrawerControl.OpenDirection = (DrawerOpenDirection)optionOpenDirection.SelectedValue;
			};

			optionDrawerDepthLengthIsNull.Click += (s, e) => UpdateDrawerDepthLength();
			optionDrawerDepthValue.ValueChanged += (s, e) => UpdateDrawerDepthLength();
			void UpdateDrawerDepthLength() =>
				sampleDrawerControl.DrawerDepth =
					optionDrawerDepthLengthIsNull.IsChecked == true
						? default(double?)
						: optionDrawerDepthValue.Value;

			optionLightDismissOverlayBackground.ItemsSource = "SystemControlBackgroundChromeMediumLowBrush,Pink,SkyBlue".Split(',');
			optionLightDismissOverlayBackground.SelectedIndex = 0;
			optionLightDismissOverlayBackground.SelectionChanged += (s, e) =>
			{
				sampleDrawerControl.LightDismissOverlayBackground = (string)optionLightDismissOverlayBackground.SelectedValue switch
				{
					"SystemControlBackgroundChromeMediumLowBrush" => (Brush)Resources["SystemControlBackgroundChromeMediumLowBrush"],
					"SkyBlue" => new SolidColorBrush(Colors.SkyBlue),
					"Pink" => new SolidColorBrush(Colors.Pink),

					_ => throw new ArgumentOutOfRangeException(),
				};
			};

			optionEdgeSwipeDetectionLengthIsNull.Click += (s, e) => UpdateEdgeSwipeDetectionLength();
			optionEdgeSwipeDetectionLengthValue.ValueChanged += (s, e) => UpdateEdgeSwipeDetectionLength();
			void UpdateEdgeSwipeDetectionLength() =>
				sampleDrawerControl.EdgeSwipeDetectionLength =
					optionEdgeSwipeDetectionLengthIsNull.IsChecked == true
						? default(double?)
						: optionEdgeSwipeDetectionLengthValue.Value;
		}
	}
}
