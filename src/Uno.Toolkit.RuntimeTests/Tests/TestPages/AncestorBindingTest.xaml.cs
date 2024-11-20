using System;
using System.Linq;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests.TestPages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AncestorBindingTest : Page
{
	public AncestorBindingTest()
	{
		this.InitializeComponent();
		this.DataContext = new
		{
			Items = Enumerable.Range(0, 5).Select(x => $"Item {x}").ToArray(),
		};
	}
}

public class AncestorBoolToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language) =>
		value as bool? == true
			? Visibility.Visible
			: Visibility.Collapsed;

	public object ConvertBack(object value, Type targetType, object parameter, string language) =>
		throw new NotSupportedException("One-way only");
}
