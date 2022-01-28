using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.Samples.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Uno.Toolkit.Samples.Content.NestedSamples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NavigationBarSample_DataContext_NestedPage1 : Page
    {
        public NavigationBarSample_DataContext_NestedPage1()
        {
            this.InitializeComponent();
			this.DataContext = new Page1VM();
        }

		private void NavigateBack(object sender, RoutedEventArgs e) => Shell.GetForCurrentView().BackNavigateFromNestedSample();
	}

	public class Page1VM
	{
		public string ViewModelName { get; } = nameof(Page1VM);
	}
}
