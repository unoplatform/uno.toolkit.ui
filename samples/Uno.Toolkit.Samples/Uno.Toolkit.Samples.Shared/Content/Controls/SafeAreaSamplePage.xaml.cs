using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.Toolkit.Samples.Entities;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Uno.Toolkit.Samples.Content.NestedSamples;
using Uno.Toolkit.UI;
using Uno.Toolkit.Samples.Helpers;
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
	[SamplePage(SampleCategory.Controls, nameof(SafeArea))]
	public sealed partial class SafeAreaSamplePage : Page
	{
		public SafeAreaSamplePage()
		{
			this.InitializeComponent();
		}

		private void LaunchAPSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<SafeAreaSamplePage_NestedPage>(clearStack: true);
		}

		private void LaunchControlSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<SafeArea_Control_NestedPage>(clearStack: true);
		}

		private void LaunchSoftInputSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<SafeArea_SoftInput_NestedPage>(clearStack: true);
		}
	}
}
