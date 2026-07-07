using static Uno.Toolkit.UI.SafeArea;


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
