using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.UI;
using Windows.Foundation;
using Windows.Foundation.Collections;

#if IS_WINUI
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif


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
