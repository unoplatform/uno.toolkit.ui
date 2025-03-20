using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.UI;



namespace Uno.Toolkit.Samples.Content.Controls;

[SamplePage(SampleCategory.Controls, "ZoomContentControl")]
public sealed partial class ZoomContentControlSamplePage : Page
{
	private ZoomContentControl zoomControl;

	public ZoomContentControlSamplePage()
	{
		this.InitializeComponent();
		this.Loaded += (s, e) => SetUpOptions();
	}

	private void SetUpOptions()
	{
		zoomControl = SamplePageLayout.GetSampleChild<ZoomContentControl>(Design.Agnostic, "ZoomContent");
		var zoomInButton = SamplePageLayout.GetSampleChild<Button>(Design.Agnostic, "ZoomInButton");
		var zoomOutButton = SamplePageLayout.GetSampleChild<Button>(Design.Agnostic, "ZoomOutButton");
		var resetButton = SamplePageLayout.GetSampleChild<Button>(Design.Agnostic, "ResetButton");

		zoomInButton.Click += OnZoomInClick;
		zoomOutButton.Click += OnZoomOutClick;
		resetButton.Click += OnResetClick;
	}

	private void OnZoomInClick(object sender, RoutedEventArgs e)
	{
		if (zoomControl.ZoomLevel < zoomControl.MaxZoomLevel)
		{
			zoomControl.ZoomLevel += 0.2;
		}
	}

	private void OnZoomOutClick(object sender, RoutedEventArgs e)
	{
		if (zoomControl.ZoomLevel > zoomControl.MinZoomLevel)
		{
			zoomControl.ZoomLevel -= 0.2;
		}
	}

	private void OnResetClick(object sender, RoutedEventArgs e)
	{
		zoomControl.ResetViewport();
	}
}
