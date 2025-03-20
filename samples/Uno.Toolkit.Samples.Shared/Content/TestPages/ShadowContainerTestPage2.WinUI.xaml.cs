#if IS_WINUI
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.Toolkit.Samples.Entities;
using Windows.Foundation;
using Windows.Foundation.Collections;

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Threading.Tasks;

namespace Uno.Toolkit.Samples.Content.TestPages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>

	[SamplePage(SampleCategory.Tests, "ShadowContainerTest2")]
	public sealed partial class ShadowContainerTestPage2 : Page
	{
		public ShadowContainerTestPage2()
		{
			this.InitializeComponent();
		}

		private void OnReload(object sender, RoutedEventArgs e)
		{
			testStatus.Text = "Running test...";

			try
			{
				sp.Children.Remove(shadowContainer);
				sp.Children.Add(shadowContainer);
				sp.Children.Remove(shadowContainer);
				sp.Children.Add(shadowContainer);
			}
			finally
			{
				testStatus.Text = "Completed";
			}
		}
	}
}
#endif
