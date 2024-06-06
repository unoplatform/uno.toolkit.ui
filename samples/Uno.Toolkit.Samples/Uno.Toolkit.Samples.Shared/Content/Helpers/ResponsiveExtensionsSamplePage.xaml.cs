using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.Samples.ViewModels;


#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.Samples.Content.Helpers;

[SamplePage(SampleCategory.Helpers, "Responsive Extensions", SourceSdk.UnoToolkit, IconPath = Icons.Helpers.MarkupExtension, DataType = typeof(ViewModel))]
public sealed partial class ResponsiveExtensionsSamplePage : Page
{
	public ResponsiveExtensionsSamplePage()
	{
		this.InitializeComponent();
	}

	private class ViewModel : ViewModelBase
	{
		public int[] ItemsSource { get => GetProperty<int[]>(); set => SetProperty(value); }

		public ViewModel()
		{
			ItemsSource = new int[] { 1, 2, 3 };
		}
	}
}
