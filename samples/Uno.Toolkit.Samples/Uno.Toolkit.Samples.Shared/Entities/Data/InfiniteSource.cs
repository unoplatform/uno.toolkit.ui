﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml.Data;
#else
using Windows.UI.Xaml.Data;
#endif

namespace Uno.Toolkit.Samples.Entities.Data;

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
