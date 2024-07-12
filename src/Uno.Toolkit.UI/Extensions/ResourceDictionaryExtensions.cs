using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI;

internal static class ResourceDictionaryExtensions
{
	private static readonly ILogger _logger = typeof(ResourceDictionaryExtensions).Log();

	/// <summary>
	/// Creates a deep clone of the given <see cref="ResourceDictionary"/>.
	/// </summary>
	/// <param name="rd">The resource dictionary to clone.</param>
	/// <returns>A deep clone of the resource dictionary.</returns>
	/// <remarks>Only the resource dictionary, and its nesting theme and merged dictionaries are deep cloned. Not their values.</remarks>
	public static ResourceDictionary DeepClone(this ResourceDictionary rd)
	{
		try
		{
			if (rd.Source is not null) return new ResourceDictionary() { Source = rd.Source };

			var result = new ResourceDictionary();

			if (rd.ThemeDictionaries is { })
			{
				foreach (var (key, value) in rd.ThemeDictionaries)
				{
					result.ThemeDictionaries[key] = (value as ResourceDictionary)?.DeepClone() ?? value;
				}
			}
			if (rd.MergedDictionaries is { })
			{
				foreach (var md in rd.MergedDictionaries)
				{
					result.MergedDictionaries.Add(md.DeepClone());
				}
			}
			foreach (var (key, value) in rd)
			{
				result[key] = value;
			}

			return result;
		}
		catch (Exception e)
		{
			_logger.Error("Failed to clone the resource-dictionary", e);
			throw;
		}
	}
}
