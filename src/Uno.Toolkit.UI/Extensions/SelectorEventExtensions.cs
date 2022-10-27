using System;
using System.Collections.Generic;
using Windows.Foundation.Collections;
using Uno.Extensions.Specialized;
#if IS_WINUI
using Microsoft.UI.Xaml.Controls.Primitives;

#else
using Windows.UI.Xaml.Controls.Primitives;
#endif

namespace Uno.Toolkit.UI;

static class SelectorEventExtensions
{
	public static WeakReference<Selector>? GetSelector(this IReadOnlyCollection<WeakReference<Selector>> keys, Selector target)
	{
		foreach (var key in keys)
		{
			key.TryGetTarget(out var selector);

			if (selector == target)
				return key;
		}

		return null;
	}

	public static void CleanNullKeyReferences(this Dictionary<WeakReference<Selector>, VectorChangedEventHandler<object>> dictionary)
	{
		using var enumerator = dictionary.Keys.GetEnumerator();

		while (enumerator.MoveNext())
		{
			var key = enumerator.Current;
			if(!key.TryGetTarget(out _))
			{
				dictionary.Remove(key);
			}
		}
	}
}
