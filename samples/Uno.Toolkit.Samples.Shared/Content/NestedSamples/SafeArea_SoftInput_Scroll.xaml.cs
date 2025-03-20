using static Uno.Toolkit.UI.SafeArea;
using System;
using Uno.Toolkit.UI;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;
using XamlWindow = Microsoft.UI.Xaml.Window;
using Microsoft.UI;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;
using XamlWindow = Windows.UI.Xaml.Window;
#endif

namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	public sealed partial class SafeArea_SoftInput_Scroll : Page
	{
		private double _constraintHeight = 0d;
		public SafeArea_SoftInput_Scroll()
		{
			this.InitializeComponent();
			SpacerBorder.SizeChanged += OnSizeChanged;
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (Spacer.ActualHeight > 0)
			{
				SpacerBorder.SizeChanged -= OnSizeChanged;
				_constraintHeight = Spacer.ActualHeight;
			}
		}

		private void MarginChecked(object sender, RoutedEventArgs e)
		{
			SafeArea.SetMode(SafeAreaControl, InsetMode.Margin);
		}

		private void PaddingChecked(object sender, RoutedEventArgs e)
		{
			SafeArea.SetMode(SafeAreaControl, InsetMode.Padding);
		}

		private void SoftChecked(object sender, RoutedEventArgs e)
		{
			SpacerBorder.Background = new SolidColorBrush(Colors.Green);
			SpacerBorder.MinHeight = 0d;
		}

		private void HardChecked(object sender, RoutedEventArgs e)
		{
			SpacerBorder.Background = new SolidColorBrush(Colors.Red);
			SpacerBorder.MinHeight = _constraintHeight;
		}

		private void NavigateBack(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView()?.BackNavigateFromNestedSample();
		}
	}
}
