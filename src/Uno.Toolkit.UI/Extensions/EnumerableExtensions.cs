using System;
using System.Collections.Generic;
using System.Text;

namespace Uno.Toolkit.UI;

internal static class EnumerableExtensions
{
	/// <summary>
	/// Functionally the same as <code>source.Zip(source.Skip(1))</code> without double iterating.
	/// </summary>
	public static IEnumerable<(T Previous, T Current)> ZipSkipOne<T>(this IEnumerable<T> source)
	{
		var etor = source.GetEnumerator();
		etor.MoveNext();

		var previous = etor.Current;
		while (etor.MoveNext())
		{
			yield return (previous, etor.Current);
			previous = etor.Current;
		}
	}
}
