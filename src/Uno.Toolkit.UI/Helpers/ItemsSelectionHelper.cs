using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using ItemsSourceView = Microsoft.UI.Xaml.Controls.ItemsSourceView;
using ItemsRepeater = Microsoft.UI.Xaml.Controls.ItemsRepeater;
#endif

namespace Uno.Toolkit.UI;

public enum ItemsSelectionMode
{
	None = 1,
	SingleOrNone,
	Single,
	Multiple,
}

internal static class ItemsSelectionHelper
{
#if HAS_UNO
	public static int IndexOf(this ItemsSourceView isv, object item)
	{
		return Enumerable.Range(0, isv.Count)
			.Cast<int?>()
			.FirstOrDefault(x => isv.GetAt((int)x!) == item) ?? -1;
	}
#endif

	/// <summary>
	/// Update the selection indexes by toggling the provided index, and then coerced according to the selection mode.
	/// </summary>
	/// <param name="mode">Selection mode</param>
	/// <param name="length">Length of items</param>
	/// <param name="selection">Current selection</param>
	/// <param name="index">Index to toggle</param>
	/// <returns>Updated selection indexes</returns>
	public static int[] ToggleSelectionAtCoerced(ItemsSelectionMode mode, int length, IList<int> selection, int index)
	{
		if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
		if (0 > index || index >= length) throw new ArgumentOutOfRangeException(nameof(index));

		if (mode is ItemsSelectionMode.None)
		{
			return Array.Empty<int>();
		}
		else if (mode is ItemsSelectionMode.Single or ItemsSelectionMode.SingleOrNone)
		{
			var wasSelected = selection.Contains(index);
			if (wasSelected && mode is ItemsSelectionMode.Single) // prevent deselection
			{
				return new int[] { index };
			}

			return wasSelected ? Array.Empty<int>() : new int[] { index };
		}
		else if (mode is ItemsSelectionMode.Multiple)
		{
			var wasSelected = selection.Contains(index);
			var newSelection = wasSelected
				? selection.Where(x => x != index)
				: selection.Append(index);

			return newSelection.ToArray();
		}
		else
		{
			throw new ArgumentOutOfRangeException(nameof(mode));
		}
	}

	public static DependencyObject? FindRootElementOf(this ItemsRepeater ir, DependencyObject node)
	{
		// e.OriginalSource is the top-most element under the cursor.
		// In order to find the materialized element, we have to walk up the visual-tree, to the first element right below IR:
		// ItemsRepeater > (item template root) > (layer0...n) > (tapped element)
		return node.GetAncestors(includeCurrent: true)
			.ZipSkipOne()
			.FirstOrDefault(x => x.Current is ItemsRepeater)
			.Previous;
	}
}
