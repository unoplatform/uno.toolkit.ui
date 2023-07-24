using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.Toolkit.UI;
using Windows.Foundation;
using Windows.Foundation.Collections;
#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Uno.Toolkit.Samples.Content.NestedSamples;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class FluentNavigationBarSampleNestedPage : Page
{
    public FluentNavigationBarSampleNestedPage()
    {
        this.InitializeComponent();
	}

	private void NavigateToNextPage(object sender, RoutedEventArgs e) => Frame.Navigate(typeof(FluentNavigationBarSampleNestedPage2));

	private void NavigateBack(object sender, RoutedEventArgs e) => Shell.GetForCurrentView().BackNavigateFromNestedSample();
}
