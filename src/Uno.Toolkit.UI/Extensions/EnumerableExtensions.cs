using System;
using System.Collections.Generic;
using System.Linq;
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

	/// <summary>
	/// Functionally the same as <seealso cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource})"/>,
	/// but allow no match result to be null instead of default(struct).
	/// </summary>
	public static T? FirstOrNull<T>(this IEnumerable<T> source) where T : struct
	{
		foreach (var element in source)
		{
			return element;
		}

		return null;
	}
	public static T? FirstOrNull<T>(this IEnumerable<T> source, Func<T, bool> predicate) where T : struct
	{
		foreach (var element in source)
		{
			if (predicate(element)) return element;
		}

		return null;
	}

	/// <summary>
	/// Functionally the same as <seealso cref="Enumerable.LastOrDefault{TSource}(IEnumerable{TSource})"/>,
	/// but allow no match result to be null instead of default(struct).
	/// </summary>
	public static T? LastOrNull<T>(this IEnumerable<T> source) where T : struct
	{
		var result = default(T?);
		foreach (var element in source)
		{
			result = element;
		}

		return result;
	}
	public static T? LastOrNull<T>(this IEnumerable<T> source, Func<T, bool> predicate) where T : struct
	{
		var result = default(T?);
		foreach (var element in source)
		{
			if (predicate(element)) result = element;
		}

		return result;
	}

	/// <summary>
	/// ToDictionary that allows for duplicated keys where later ones override previous.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	/// <param name="source"></param>
	/// <param name="keySelector"></param>
	/// <param name="valueSelector"></param>
	/// <returns></returns>
	public static Dictionary<TKey, TValue> ToDictionarySafe<T, TKey, TValue>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<T, TValue> valueSelector)
		where TKey : notnull
	{
		var dict = new Dictionary<TKey, TValue>();
		foreach (var element in source)
		{
			dict[keySelector(element)] = valueSelector(element);
		}

		return dict;
	}

	public static IEnumerable<T> TrimNull<T>(this IEnumerable<T?> source) where T : notnull
	{
		return source.OfType<T>();
	}
}
