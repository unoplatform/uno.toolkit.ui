using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.Toolkit.UI;
using Uno.UI.Toolkit;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using static Uno.Toolkit.UI.SafeArea;
using System.Threading.Tasks;
using System.Runtime.InteropServices.ComTypes;
using Uno.UI;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using XamlWindow = Microsoft.UI.Xaml.Window;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XamlWindow = Windows.UI.Xaml.Window;
#endif

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SafeArea_Control_NestedPage : Page
	{
		public SafeArea_Control_NestedPage()
		{
			this.InitializeComponent();

			//MyRed.Loaded += MyRedCanvas_Loaded;
			FrameworkElementHelper.SetUseArrangePathDisabled(this, false);
			FrameworkElementHelper.SetUseMeasurePathDisabled(this, false);

			SafeArea.SetInsets(MyRed, SafeArea.InsetMask.VisibleBounds);
		}

		private async void MyRedCanvas_Loaded(object sender, RoutedEventArgs e)
		{
			//await Task.Delay(1000);
			FrameworkElementHelper.SetUseArrangePathDisabled(this, true);
			FrameworkElementHelper.SetUseMeasurePathDisabled(this, true);

			SafeArea.SetInsets(MyRed, SafeArea.InsetMask.VisibleBounds);
			//VisibleBoundsPadding.SetPaddingMask(MyRed, VisibleBoundsPadding.PaddingMask.All);
			//MyRed.Padding = new Thickness(0, 24, 0, 24);
		}

		private void OnClick(object sender, RoutedEventArgs e)
		{
			//MyBlueGrid.UpdateLayout();
		}
	}
}
