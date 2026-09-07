using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Uno.Toolkit.Samples.Fluent;

public sealed partial class MainPage : Page
{
	public MainPage()
	{
		InitializeComponent();
	}

	private void OnSwitchTheme(object sender, RoutedEventArgs args)
	{
		RequestedTheme = ActualTheme == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark;
	}

	private void OnChipRemoved(object sender, RoutedEventArgs args)
	{
		if (sender is FrameworkElement chip)
		{
			chip.Visibility = Visibility.Collapsed;
		}
	}
}
