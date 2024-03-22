using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemsRepeater = Microsoft.UI.Xaml.Controls.ItemsRepeater;
using Uno.Toolkit.UI;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif

namespace Uno.Toolkit.RuntimeTests.Extensions
{
	internal static class ItemsRepeaterTestExtensions
	{
		public static void FakeTapItemAt(this ItemsRepeater ir, int index)
		{
			if (ir.TryGetElement(index) is { } element)
			{
				// Fake local tap handler on ToggleButton level.
				// For SelectorItem, nothing will happen on tap unless nested under a Selector, which isnt the case here.
				(element as ToggleButton)?.Toggle();

				// This is what's called in ItemsRepeater::Tapped handler.
				// Note that the handler will not trigger from a "fake tap" like the line above, so we have to manually invoke here.
				ItemsRepeaterExtensions.ToggleItemSelectionAtCoerced(ir, index);
			}
			else
			{
				throw new InvalidOperationException($"Element at index={index} is not yet materialized or out of range.");
			}
		}
	}
}
