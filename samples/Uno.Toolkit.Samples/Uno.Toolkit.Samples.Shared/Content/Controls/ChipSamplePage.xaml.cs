using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Uno.Toolkit.Samples.Entities.Data;
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
	[SamplePage(SampleCategory.Controls, "Chip", SourceSdk.UnoMaterial, DataType = typeof(TestCollections))]
	public sealed partial class ChipSamplePage : Page
	{
		public ChipSamplePage()
		{
			this.InitializeComponent();
		}

		private void RemoveChipItem(object sender, ChipItemEventArgs e)
		{
			if (DataContext is Sample sample)
			{
				if (sample.Data is TestCollections test)
				{
					test.RemoveChipItem(e.Item as TestCollections.SelectableData);
				}
			}
		}

		private void ResetChipItems(object sender, RoutedEventArgs e)
		{
			if (DataContext is Sample sample)
			{
				if (sample.Data is TestCollections test)
				{
					test.ResetChipItems();
				}
			}
		}
	}
}
