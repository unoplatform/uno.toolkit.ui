using System;
using System.Collections;
using System.Linq;
using System.Windows.Input;
using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.Samples.ViewModels;
using Uno.Toolkit.UI;
using Uno.Toolkit.Samples.Entities.Data;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.Samples.Content.Controls;

[SamplePage(SampleCategory.Behaviors, nameof(ItemsRepeaterExtensions), SourceSdk.UnoToolkit, DataType = typeof(ViewModel))]
public sealed partial class ItemsRepeaterExtensionsSamplePage : Page
{
	public ItemsRepeaterExtensionsSamplePage()
	{
		this.InitializeComponent();
	}

	private class ViewModel : ViewModelBase
	{
		public int[] MultiItemsSource { get => GetProperty<int[]>(); set => SetProperty(value); }
		public object[] MultiSelectedItems { get => GetProperty<object[]>(); set => SetProperty(value); }
		public int[] MultiSelectedIndexes { get => GetProperty<int[]>(); set => SetProperty(value); }

		public int[] SingleItemsSource { get => GetProperty<int[]>(); set => SetProperty(value); }
		public int SingleSelectedItem { get => GetProperty<int>(); set => SetProperty(value); }
		public int SingleSelectedIndex { get => GetProperty<int>(); set => SetProperty(value); }

		public ViewModel()
		{
			MultiItemsSource = new int[] { 1, 2, 3, 4, 5};
			MultiSelectedIndexes = new[] { 0, 1, 2 };
			MultiSelectedItems = MultiSelectedIndexes.Select(x => MultiItemsSource[x]).Cast<object>().ToArray();

			SingleItemsSource = new int[] { 1, 2, 3, 4, 5};
			SingleSelectedIndex = 2;
			SingleSelectedItem = SingleItemsSource[SingleSelectedIndex];
		}
	}
}

