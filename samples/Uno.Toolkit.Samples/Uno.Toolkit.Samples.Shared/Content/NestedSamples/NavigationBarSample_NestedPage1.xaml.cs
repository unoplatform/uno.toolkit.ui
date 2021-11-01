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

namespace Uno.Toolkit.Samples.Content.NestedSamples
{
	public sealed partial class NavigationBarSample_NestedPage1 : Page
	{
		public NavigationBarSample_NestedPage1()
		{
			this.InitializeComponent();
			DataContext = new FirstPageViewModel();
		}

		private void NavigateToNextPage(object sender, RoutedEventArgs e) => Frame.Navigate(typeof(NavigationBarSample_NestedPage2));

		private void NavigateBack(object sender, RoutedEventArgs e) => Shell.GetForCurrentView().BackNavigateFromNestedSample();


	}

	public class FirstPageViewModel : ViewModelBase
	{
		public int PrimaryCommand1Count { get => GetProperty<int>(); set => SetProperty(value); }
		public int PrimaryCommand2Count { get => GetProperty<int>(); set => SetProperty(value); }
		public int SecondaryCommand1Count { get => GetProperty<int>(); set => SetProperty(value); }
		public int SecondaryCommand2Count { get => GetProperty<int>(); set => SetProperty(value); }
		public int SecondaryCommand3Count { get => GetProperty<int>(); set => SetProperty(value); }

		public ICommand Primary1CountCommand => new Command(_ => PrimaryCommand1Count++);
		public ICommand Primary2CountCommand => new Command(_ => PrimaryCommand2Count++);
		public ICommand Secondary1CountCommand => new Command(_ => SecondaryCommand1Count++);
		public ICommand Secondary2CountCommand => new Command(_ => SecondaryCommand2Count++);
		public ICommand Secondary3CountCommand => new Command(_ => SecondaryCommand3Count++);
	}
}
