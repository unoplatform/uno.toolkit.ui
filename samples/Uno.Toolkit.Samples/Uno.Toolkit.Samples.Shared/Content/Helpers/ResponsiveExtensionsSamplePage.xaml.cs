using Uno.Toolkit.Samples.Entities;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.Samples.Content.Helpers;

[SamplePage(SampleCategory.Helpers, "Responsive Extensions", SourceSdk.UnoToolkit, IconPath = Icons.Helpers.MarkupExtension)]
public sealed partial class ResponsiveExtensionsSamplePage : Page
{
	public ResponsiveExtensionsSamplePage()
	{
		this.InitializeComponent();
	}
}
