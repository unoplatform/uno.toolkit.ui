using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Toolkit.UI;

internal static class Hierarchy
{
	public static IEnumerable<T> Walk<T>(T? current, Func<T, T?> walk)
	{
		for (; current != null; current = walk(current))
			yield return current;
	}
	public static IEnumerable<T> Walk<T>(T? current, Func<T, IEnumerable<T>?> walk)
	{
		if (current == null) yield break;

		yield return current;
		foreach (var child in walk(current) ?? Enumerable.Empty<T>())
		{
			foreach (var item in Walk(child, walk))
			{
				yield return item;
			}
		}
	}

	public static T? Find<T>(T? current, Func<T, T?> walk, Func<T, bool> predicate)
	{
		return Walk(current, walk).FirstOrDefault(predicate);
	}
}
