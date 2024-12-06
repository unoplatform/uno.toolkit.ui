using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Toolkit.RuntimeTests.Extensions;

internal static class DictionaryExtensions
{
	/// <summary>
	/// Combine two dictionaries into a new one.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	/// <param name="dict"></param>
	/// <param name="other"></param>
	/// <param name="preferOther"></param>
	/// <param name="comparer"></param>
	/// <returns></returns>
	public static IDictionary<TKey,TValue> Combine<TKey, TValue>(
		this IReadOnlyDictionary<TKey,TValue> dict,
		IReadOnlyDictionary<TKey,TValue>? other,
		bool preferOther = true,
		IEqualityComparer<TKey>? comparer = null
	) where TKey : notnull
	{
		var result = new Dictionary<TKey, TValue>(dict, comparer);
		if (other is { })
		{
			foreach (var kvp in other)
			{
				if (preferOther || !result.ContainsKey(kvp.Key))
				{
					result[kvp.Key] = kvp.Value;
				}
			}
		}

		return result;
	}
}
