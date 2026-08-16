using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

using _Impl = Microsoft.UI.Dispatching.DispatcherQueue;
using _Handler = Microsoft.UI.Dispatching.DispatcherQueueHandler;
using _Priority = Microsoft.UI.Dispatching.DispatcherQueuePriority;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

using _Impl = Windows.UI.Core.CoreDispatcher;
using _Handler = Windows.UI.Core.DispatchedHandler;
using _Priority = Windows.UI.Core.CoreDispatcherPriority;
#endif

namespace Uno.Toolkit.UI;

//note: there are 3 "dispatcher" classes:
//	- Microsoft.UI.Dispatching.DispatcherQueue // from UIElement::DispatcherQueue [winui-only]
//	- Windows.UI.Core.CoreDispatcher // from UIElement::Dispatcher [uwp-only][winui: signature still exists, but value is always null]
//	- Windows.System.DispatcherQueue // from DispatcherQueue::GetForCurrentThread()
//		^ #406: supposedly obtainable with `GetForCurrentThread()` on ui-thread,
//		^ but on winui-desktop, it returns null during control's ctor...
//		^ hence why we are not using it

internal class DispatcherCompat
{
	public enum Priority { Low = -1, Normal = 0, High = 1 }

	private readonly _Impl _impl;

	public DispatcherCompat(_Impl impl)
	{
		this._impl = impl;
	}

	private static _Priority RemapPriority(Priority priority) => priority switch
	{
		// [uwp] Windows.UI.Core.CoreDispatcherPriority::Idle doesnt have a counterpart, and is thus ignored.

		Priority.Low => _Priority.Low,
		Priority.Normal => _Priority.Normal,
		Priority.High => _Priority.High,

		_ => throw new ArgumentOutOfRangeException($"Invalid value: ({priority:d}){priority}"),
	};

	public bool HasThreadAccess => _impl.HasThreadAccess;

	public void Invoke(_Handler handler) => Invoke(default, handler);
	public void Invoke(Priority priority, _Handler handler)
	{
		if (_impl.HasThreadAccess)
		{
			handler();
		}
		else
		{
			Schedule(priority, handler);
		}
	}

	public Task RunAsync(_Handler handler) => RunAsync(default, handler);
	public Task RunAsync(Priority priority, _Handler handler)
	{
		if (_impl.HasThreadAccess)
		{
			handler();
			return Task.CompletedTask;
		}
		else
		{
			var tcs = new TaskCompletionSource<object>();

			Schedule(priority, () =>
			{
				try
				{
					handler();
					tcs.TrySetResult(default!);
				}
				catch (Exception e)
				{
					tcs.TrySetException(e);
				}
			});

			return tcs.Task;
		}
	}

	public void Schedule(_Handler handler) => Schedule(default, handler);
	public void Schedule(Priority priority, _Handler handler)
	{
#if IS_WINUI
			_impl.TryEnqueue(RemapPriority(priority), handler);
#else
			_ = _impl.RunAsync(RemapPriority(priority), handler);
#endif
	}
}
