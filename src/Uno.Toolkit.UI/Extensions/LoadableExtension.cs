using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Disposables;

namespace Uno.Toolkit.UI
{
	internal static class LoadableExtension
	{
		public static IDisposable BindIsExecuting(this ILoadable source, Action<bool> update, bool propagateInitialValue = true) => source.BindIsExecuting(() => update(source.IsExecuting), propagateInitialValue);

		public static IDisposable BindIsExecuting(this ILoadable source, Action update, bool propagateInitialValue = true)
		{
			if (propagateInitialValue) update();

			source.IsExecutingChanged += OnIsExecutingChanged;
			return Disposable.Create(() => source.IsExecutingChanged -= OnIsExecutingChanged);

			void OnIsExecutingChanged(object? sender, EventArgs e) => update();
		}
	}
}
