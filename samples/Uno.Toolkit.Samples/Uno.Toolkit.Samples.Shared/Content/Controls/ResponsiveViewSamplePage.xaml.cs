using Uno.Toolkit.Samples.Entities;
#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.Samples.Content.Controls
{
	[SamplePage(SampleCategory.Controls, "ResponsiveView")]
	public sealed partial class ResponsiveViewSamplePage : Page
	{
		public ResponsiveViewSamplePage()
		{
			this.InitializeComponent();
		}
	}
}
