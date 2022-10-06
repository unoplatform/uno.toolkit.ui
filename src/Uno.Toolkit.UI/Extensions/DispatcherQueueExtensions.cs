using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;

namespace Uno.Toolkit.UI
{
	internal static class DispatcherQueueExtensions
	{
		public static Task ExecuteAsync(
			this DispatcherQueue dispatcher,
			Func<CancellationToken, Task> actionWithResult,
			CancellationToken cancellation)
		{
			return dispatcher.ExecuteAsync<object?>(async ct =>
			{
				await actionWithResult(ct);
				return default;
			}, cancellation);
		}


		public static async Task<TResult> ExecuteAsync<TResult>(
			this DispatcherQueue dispatcher,
			Func<CancellationToken, Task<TResult>> actionWithResult,
			CancellationToken cancellation)
		{
			if (dispatcher.HasThreadAccess)
			{
				return await actionWithResult(cancellation);
			}

			var completion = new TaskCompletionSource<TResult>();
			dispatcher.TryEnqueue(async () =>
			{
				try
				{
					var result = await actionWithResult(cancellation);
					completion.SetResult(result);
				}
				catch (Exception ex)
				{
					completion.SetException(ex);
				}
			});
			return await completion.Task;
		}
	}

}
