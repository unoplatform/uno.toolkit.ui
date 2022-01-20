using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.Toolkit.Samples.Entities;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Uno.Toolkit.Samples.Content.NestedSamples;
#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif


namespace Uno.Toolkit.Samples.Content.Controls
{
    [SamplePage(SampleCategory.Controls, "NavigationBar")]
    public sealed partial class NavigationBarSamplePage : Page
    {
        public NavigationBarSamplePage()
        {
            this.InitializeComponent();
        }

        private void LaunchFullScreenSample(object sender, RoutedEventArgs e)
        {
            Shell.GetForCurrentView().ShowNestedSample<NavigationBarSample_NestedPage1>(clearStack: true);
        }

        private async void LaunchModalSample(object sender, RoutedEventArgs e)
        {
            await Shell.GetForCurrentView().ShowModal<NavigationBarSample_ModalPage1>();
        }
    }
}
