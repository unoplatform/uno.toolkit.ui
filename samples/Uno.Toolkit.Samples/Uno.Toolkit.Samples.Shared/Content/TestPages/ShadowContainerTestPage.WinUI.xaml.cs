#if IS_WINUI
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.Toolkit.Samples.Entities;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

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
			SelectedElement = ElementState.Border;

			statusText.Text = string.Empty;

			xOffsetText.Text = string.Empty;
			yOffsetText.Text = string.Empty;
			inner.IsChecked = false;
			check_Border.IsChecked = true;

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
#endif
