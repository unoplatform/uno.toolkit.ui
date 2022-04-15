﻿using System;
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
	public sealed partial class MaterialNavigationBarSample_NestedPage1 : Page
	{
		public MaterialNavigationBarSample_NestedPage1()
		{
			this.InitializeComponent();
		}

		private void NavigateToNextPage(object sender, RoutedEventArgs e) => Frame.Navigate(typeof(M3MaterialNavigationBarSample_NestedPage2));

		private void NavigateBack(object sender, RoutedEventArgs e) => Shell.GetForCurrentView().BackNavigateFromNestedSample();

	}
}
