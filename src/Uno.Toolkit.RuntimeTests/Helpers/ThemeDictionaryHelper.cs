using System;
using System.Collections.Generic;
using System.Linq;

namespace Uno.Toolkit.RuntimeTests.Helpers;

internal static class ThemeDictionaryHelper
{
	/// <summary>
	/// Returns the keys of the given theme that start with one of <paramref name="prefixes"/> and do not resolve to a brush.
	/// Aliases are resolved while a key is read, so a key that names a missing resource shows up here.
	/// </summary>
	public static IReadOnlyList<string> FindUnresolvedBrushes(ResourceDictionary root, string theme, int minimumCount, params string[] prefixes)
	{
		var unresolved = new List<string>();
		var count = 0;
		foreach (var dictionary in FindThemeDictionaries(root, theme))
		{
			foreach (var key in dictionary.Keys.OfType<string>().Where(k => prefixes.Any(p => k.StartsWith(p, StringComparison.Ordinal))))
			{
				count++;
				object? value;
				try
				{
					value = dictionary[key];
				}
				catch (Exception)
				{
					value = null;
				}

				if (value is not Brush)
				{
					unresolved.Add(key);
				}
			}
		}

		if (count < minimumCount)
		{
			unresolved.Add($"(only {count} keys found with prefixes {string.Join(", ", prefixes)}; expected at least {minimumCount})");
		}

		return unresolved;
	}

	private static IEnumerable<ResourceDictionary> FindThemeDictionaries(ResourceDictionary dictionary, string theme)
	{
		if (dictionary.ThemeDictionaries.TryGetValue(theme, out var themed) && themed is ResourceDictionary themeDictionary)
		{
			yield return themeDictionary;
		}

		foreach (var merged in dictionary.MergedDictionaries)
		{
			foreach (var found in FindThemeDictionaries(merged, theme))
			{
				yield return found;
			}
		}
	}
}
