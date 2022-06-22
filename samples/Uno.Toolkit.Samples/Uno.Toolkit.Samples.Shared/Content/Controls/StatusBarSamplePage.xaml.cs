using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Uno.Toolkit.Samples.Content.NestedSamples;
using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.UI;

#if IS_WINUI
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI;
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
	[SamplePage(SampleCategory.Behaviors, nameof(Uno.Toolkit.UI.StatusBar), source: SourceSdk.UnoToolkit)]
	public sealed partial class StatusBarSamplePage : Page, IExitNestedSampleHandler
	{
		public StatusBarSamplePage()
		{
			this.InitializeComponent();
		}

		public void OnExitedFromNestedSample(object sender)
		{
			StatusBar.SetForegroundTheme(StatusBarTheme.Light);
			StatusBar.SetBackground(Colors.Gray);
		}

		private void ShowSample(object sender, RoutedEventArgs e)
		{
			Shell.GetForCurrentView().ShowNestedSample<StatusBarSample_NestedPage1>(clearStack: true);
		}
	}
}
