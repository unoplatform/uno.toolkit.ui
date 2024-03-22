using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml.Data;
#else
using Windows.UI.Xaml.Data;
#endif

namespace Uno.Toolkit.UI
{
	internal static class ItemIndexRangeExtensions
	{
		internal static IEnumerable<ItemIndexRange> ReduceToRange(this IEnumerable<int> indexes)
		{
			int first = int.MinValue;
			uint n = 0;
			foreach (var i in indexes.OrderBy(x => x))
			{
				if (first + n == i)
				{
					n++;
				}
				else
				{
					if (n > 0) yield return new(first, n);

					first = i;
					n = 1;
				}
			}

			if (n > 0)
			{
				yield return new(first, n);
			}
		}

		internal static int[] Expand(this ItemIndexRange range) => Enumerable.Range(range.FirstIndex, (int)range.Length).ToArray();

		internal static int[] Expand(this IEnumerable<ItemIndexRange> ranges) => ranges.SelectMany(Expand).ToArray();
	}
}
