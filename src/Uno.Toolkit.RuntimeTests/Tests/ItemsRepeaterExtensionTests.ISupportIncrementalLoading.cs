using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Windows.Foundation;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Controls;
#else
using ItemsRepeater = Microsoft.UI.Xaml.Controls.ItemsRepeater;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

partial class ItemsRepeaterExtensionTests
{
	[TestMethod]
	[RequiresFullWindow]
	[RunsOnUIThread]
	[DataRow(Orientation.Vertical)]
	[DataRow(Orientation.Horizontal)]
	public async Task When_Incremental_Load(Orientation orientation)
	{
		const int BatchSize = 25;

		var source = new InfiniteSource<int>(async start =>
		{
			await Task.Delay(25);
			return Enumerable.Range(start, BatchSize).ToArray();
		});

		(Grid panel, ItemsRepeater sut, ScrollViewer sv) = BuildSetup(source, orientation);

		panel.Children.Add(sv);

		await UnitTestUIContentHelperEx.SetContentAndWait(panel);
		await Task.Delay(1000);
		var initial = GetCurrenState();

		(double? hOffset, double? vOffset) = orientation switch
		{
			Orientation.Vertical => ((double?)null, 10_000),
			Orientation.Horizontal => (10_000, default),
			_ => throw new NotSupportedException()
		};

		// scroll to bottom
		sv.ChangeView(hOffset, vOffset, null, disableAnimation: true);
		await Task.Delay(500);
		await UnitTestsUIContentHelper.WaitForIdle();
		var firstScroll = GetCurrenState();

		// scroll to bottom
		sv.ChangeView(hOffset, vOffset, null, disableAnimation: true);
		await Task.Delay(500);
		await UnitTestsUIContentHelper.WaitForIdle();
		var secondScroll = GetCurrenState();

		Assert.AreEqual(BatchSize * 1, initial.LastLoaded, "Should start with first batch loaded.");
		Assert.AreEqual(BatchSize * 2, firstScroll.LastLoaded, "Should have 2 batches loaded after first scroll.");
		Assert.IsTrue(initial.LastMaterialized < firstScroll.LastMaterialized, "No extra item materialized after first scroll.");
		Assert.AreEqual(BatchSize * 3, secondScroll.LastLoaded, "Should have 3 batches loaded after second scroll.");
		Assert.IsTrue(firstScroll.LastMaterialized < secondScroll.LastMaterialized, "No extra item materialized after second scroll.");

		(int LastLoaded, int LastMaterialized) GetCurrenState() =>
		(
			source.LastIndex,
			Enumerable.Range(0, source.LastIndex).Reverse().FirstOrDefault(x => sut.TryGetElement(x) != null)
		);
	}

	[TestMethod]
	[RequiresFullWindow]
	[RunsOnUIThread]
	[DataRow(Orientation.Vertical)]
	[DataRow(Orientation.Horizontal)]
	public async Task When_Incremental_Load_ShouldStop(Orientation orientation)
	{
		const int BatchSize = 25;
		var source = new InfiniteSource<int>(async start =>
		{
			await Task.Delay(25);
			return Enumerable.Range(start, BatchSize).ToArray();
		});

		(Grid panel, ItemsRepeater sut, ScrollViewer sv) = BuildSetup(source, orientation);

		panel.Children.Add(sv);

		await UnitTestUIContentHelperEx.SetContentAndWait(panel);
		await Task.Delay(1000);
		var initial = GetCurrenState();

		(double? hOffset, double? vOffset) = orientation switch
		{
			Orientation.Vertical => ((double?)null, 10_000),
			Orientation.Horizontal => (10_000, default),
			_ => throw new NotSupportedException()
		};

		// scroll to bottom
		sv.ChangeView(hOffset, vOffset, null, disableAnimation: true);
		await Task.Delay(500);
		await UnitTestsUIContentHelper.WaitForIdle();
		var firstScroll = GetCurrenState();

		// Has'No'MoreItems
		source.HasMoreItems = false;

		// scroll to bottom
		sv.ChangeView(hOffset, vOffset, null, disableAnimation: true);
		await Task.Delay(500);
		await UnitTestsUIContentHelper.WaitForIdle();
		var secondScroll = GetCurrenState();

		Assert.AreEqual(BatchSize * 1, initial.LastLoaded, "Should start with first batch loaded.");
		Assert.AreEqual(BatchSize * 2, firstScroll.LastLoaded, "Should have 2 batches loaded after first scroll.");
		Assert.IsTrue(initial.LastMaterialized < firstScroll.LastMaterialized, "No extra item materialized after first scroll.");
		Assert.AreEqual(BatchSize * 2, secondScroll.LastLoaded, "Should still have 2 batches loaded after first scroll since HasMoreItems was false.");
		Assert.AreEqual(BatchSize * 2 - 1, secondScroll.LastMaterialized, "Last materialized item should be the last from 2nd batch (50th/index=49).");

		(int LastLoaded, int LastMaterialized) GetCurrenState() =>
		(
			source.LastIndex,
			Enumerable.Range(0, source.LastIndex).Reverse().FirstOrDefault(x => sut.TryGetElement(x) != null)
		);
	}

	[TestMethod]
	[RequiresFullWindow]
	[RunsOnUIThread]
	[DataRow(Orientation.Vertical)]
	[DataRow(Orientation.Horizontal)]
	public async Task When_Incremental_Load_IsLoading(Orientation orientation)
	{
		const int BatchSize = 25;
		var source = new InfiniteSource<int>(async start =>
		{
			await Task.Delay(1000);
			return Enumerable.Range(start, BatchSize).ToArray();
		});

		(Grid panel, ItemsRepeater sut, ScrollViewer sv) = BuildSetup(source, orientation);
		panel.Children.Add(sv);

		await UnitTestUIContentHelperEx.SetContentAndWait(panel);
		await Task.Delay(2000);

		(double? hOffset, double? vOffset) = orientation switch
		{
			Orientation.Vertical => ((double?)null, 10_000),
			Orientation.Horizontal => (10_000, default),
			_ => throw new NotSupportedException()
		};

		// scroll to bottom
		sv.ChangeView(hOffset, vOffset, null, disableAnimation: true);

		await UnitTestUIContentHelperEx.WaitFor(() => ItemsRepeaterExtensions.GetIsLoading(sut), timeoutMS: 2000, message: "IsLoading should become true");
		await UnitTestUIContentHelperEx.WaitFor(() => !ItemsRepeaterExtensions.GetIsLoading(sut), timeoutMS: 2000, message: "IsLoading should become false when done loading more items");
	}

	private static ItemsRepeater SetupIncrementalLoadingItemsRepeater(object source, Orientation orientation)
	{
		var ir = new ItemsRepeater
		{
			Layout = new Microsoft.UI.Xaml.Controls.StackLayout { Orientation = orientation },
			ItemsSource = source,
			ItemTemplate = XamlHelper.LoadXaml<DataTemplate>("""
				<DataTemplate>
					<Border Width="29"
							Height="29"
							Background="LightSeaGreen">
						<TextBlock Text="{Binding}"
								   HorizontalAlignment="Center" />
					</Border>
				</DataTemplate>
				"""),
		};

		ItemsRepeaterExtensions.SetSupportsIncrementalLoading(ir, true);

		return ir;
	}

	private static ScrollViewer SetupScrollViewer(ItemsRepeater ir)
	{
		return new ScrollViewer
		{
			Content = ir,
			HorizontalScrollMode = ScrollMode.Enabled,
			HorizontalScrollBarVisibility = ScrollBarVisibility.Visible,
			VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
			VerticalScrollMode = ScrollMode.Enabled
		};
	}

	private static (Grid panel, ItemsRepeater sut, ScrollViewer vs) BuildSetup(object source, Orientation orientation)
	{
		var panel = new Grid() { Height = 210, Width = 210, VerticalAlignment = VerticalAlignment.Bottom };
		var sut = SetupIncrementalLoadingItemsRepeater(source, orientation);
		var sv = SetupScrollViewer(sut);

		return (panel, sut, sv);
	}
}


public class InfiniteSource<T> : ObservableCollection<T>, ISupportIncrementalLoading
{
	public delegate Task<T[]> AsyncFetch(int start);
	public delegate T[] Fetch(int start);

	private readonly AsyncFetch _fetchAsync;
	private int _start;

	public InfiniteSource(AsyncFetch fetch)
	{
		_fetchAsync = fetch;
		_start = 0;
	}

	public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
	{
		return AsyncInfo.Run(async ct =>
		{
			var items = await _fetchAsync(_start);
			foreach (var item in items)
			{
				Add(item);
			}
			_start += items.Length;

			return new LoadMoreItemsResult { Count = count };
		});
	}

	public bool HasMoreItems { get; set; } = true;

	public int LastIndex => _start;
}
