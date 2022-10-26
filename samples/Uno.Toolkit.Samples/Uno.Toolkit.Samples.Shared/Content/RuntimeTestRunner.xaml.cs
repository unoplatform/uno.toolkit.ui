using Uno.Toolkit.Samples.Entities;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.Samples.Content
{
	[SamplePage(SampleCategory.None, "RuntimeTest Runner", SourceSdk.UnoToolkit)]
	public sealed partial class RuntimeTestRunner : Page
	{
		public RuntimeTestRunner()
		{
			this.InitializeComponent();
		}
	}
}
