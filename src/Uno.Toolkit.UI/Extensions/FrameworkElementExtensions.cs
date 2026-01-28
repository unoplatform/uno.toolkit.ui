using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Disposables;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif

namespace Uno.Toolkit.UI;

internal static class FrameworkElementExtensions
{
	public static void InheritDataContextFrom(this FrameworkElement fe, FrameworkElement parent)
	{
		fe.SetBinding(FrameworkElement.DataContextProperty, new Binding
		{
			Source = parent,
			Path = new PropertyPath(nameof(parent.DataContext)),
			Mode = BindingMode.OneWay,
		});
	}

	/// <summary>
	/// Subscribes to the Loaded event of nested elements, as defined by the innerSelectors, with a callback on the innermost element loaded.
	/// </summary>
	/// <remarks>Used to wait through multiple layers of template materialization.</remarks>
	/// <param name="root"></param>
	/// <param name="innerSelectors"></param>
	/// <param name="onInnerMostLoaded"></param>
	/// <returns></returns>
	public static IDisposable SubscribeToNestedElements(this FrameworkElement root, Func<FrameworkElement, FrameworkElement?>[] innerSelectors, Action<FrameworkElement> onInnerMostLoaded)
	{
		var subscriptions = new List<(int Depth, FrameworkElement Element, Action Unsubscribe)>();
		var subscriptionsLock = new object();
		var disposed = false;

		Subscribe(root, 0);
		return Disposable.Create(() =>
		{
			disposed = true;
			subscriptions.Clear();
		});

		void Subscribe(FrameworkElement e, int depth)
		{
			if (disposed) return;

			if (subscriptions.Count > depth)
			{
				if ((subscriptions.ElementAt(depth).Element != e))
				{
					// elment register at this depth is no longer the same
					// drop everything from this depth and lower
					lock (subscriptionsLock) subscriptions.RemoveRange(depth, subscriptions.Count - depth);

					// and, push a new stack
					RoutedEventHandler handler = (s, _) => Walk(e, depth);
					e.Loaded += handler;
					lock (subscriptionsLock) subscriptions.Add((depth, e, () => e.Loaded -= handler));
				}
				else
				{
					// same element reloaded, nothing to do
				}
			}
			else
			{
				// new depth reached, just push a new stack
				RoutedEventHandler handler = (s, _) => Walk(e, depth);
				e.Loaded += handler;
				lock (subscriptionsLock) subscriptions.Add((depth, e, () => e.Loaded -= handler));
			}

			if (e.IsLoaded)
			{
				Walk(e, depth);
			}
		}
		void Walk(FrameworkElement e, int depth)
		{
			if (disposed) return;

			if (depth >= innerSelectors.Length)
			{
				onInnerMostLoaded(e);
			}
			else if (innerSelectors[depth](e) is { } nested)
			{
				Subscribe(nested, depth + 1);
			}
		}
	}
}
