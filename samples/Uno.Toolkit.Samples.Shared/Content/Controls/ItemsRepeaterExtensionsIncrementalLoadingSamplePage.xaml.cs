using System;
using System.Collections;
using System.Linq;
using System.Windows.Input;
using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.Samples.ViewModels;
using Uno.Toolkit.UI;
using Uno.Toolkit.Samples.Entities.Data;
using System.Threading.Tasks;


#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif
namespace Uno.Toolkit.Samples.Content.Controls;


[SamplePage(SampleCategory.Behaviors, "IR IncrementalLoading", SourceSdk.UnoToolkit, DataType = typeof(IncrementalLoadingViewModel))]

public sealed partial class ItemsRepeaterExtensionsIncrementalLoadingSamplePage : Page
{
	public ItemsRepeaterExtensionsIncrementalLoadingSamplePage()
	{
		this.InitializeComponent();
	}

	private class IncrementalLoadingViewModel : ViewModelBase
	{
		private const int BatchSize = 25;
		public bool IsVerticalLoading { get => GetProperty<bool>(); set => SetProperty(value); }
		public bool IsHorizontalLoading { get => GetProperty<bool>(); set => SetProperty(value); }
		public InfiniteSource<int> VerticalInfiniteItemsSource { get => GetProperty<InfiniteSource<int>>(); set => SetProperty(value); }
		public InfiniteSource<int> HorizontalInfiniteItemsSource { get => GetProperty<InfiniteSource<int>>(); set => SetProperty(value); }

		public IncrementalLoadingViewModel()
		{
			VerticalInfiniteItemsSource = new InfiniteSource<int>(async start =>
			{
				await Task.Delay(2000);
				return Enumerable.Range(start, BatchSize).ToArray();
			});

			HorizontalInfiniteItemsSource = new InfiniteSource<int>(async start =>
			{
				await Task.Delay(2000);
				return Enumerable.Range(start, BatchSize).ToArray();
			});
		}
	}
}
