using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.UI;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.Samples.Content.Controls
{
	[SamplePage(SampleCategory.Controls, nameof(ShadowContainer), Description = "Add many colored shadows to your controls.")]
	public sealed partial class ShadowContainerSamplePage : Page
	{
		public ShadowContainerSamplePage()
		{
			this.InitializeComponent();
		}

		private void CheckBox_Indeterminate(object sender, RoutedEventArgs e)
		{

        }
    }
}
