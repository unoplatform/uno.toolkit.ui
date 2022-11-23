using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using ItemsRepeater = Microsoft.UI.Xaml.Controls.ItemsRepeater;
#endif

namespace Uno.Toolkit.UI
{
	public static partial class ItemsRepeaterExtensions
	{
		internal static void OnItemCommandChanged(ItemsRepeater sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue is ICommand)
			{
				if (e.NewValue is not ICommand) // tear down
				{
					sender.Tapped -= OnItemsRepeaterTapped;
				}
				else
				{
					// When transitioning from one command to another, there is no need to rewire the event.
					// Since the handler is setup to invoke the command in the DP.
				}
			}
			else if (e.NewValue is ICommand command)
			{
				sender.Tapped += OnItemsRepeaterTapped;
			}
		}

		private static void OnItemsRepeaterTapped(object sender, TappedRoutedEventArgs e)
		{
			// ItemsRepeater is more closely related to Panel than ItemsControl, and it cannot be templated.
			// It is safe to assume all direct children of IR are materialized item template,
			// and there can't be header/footer or wrapper (ItemContainer) among them.

			if (sender is not ItemsRepeater owner) return;
			if (e.OriginalSource is ItemsRepeater) return;
			if (e.OriginalSource is DependencyObject source)
			{
				// e.OriginalSource is the top-most element under the cursor.
				// In order to find the materialized item template, we have to walk up the visual-tree, to the first element right below IR:
				// ItemsRepeater > (item template root) > (layer0...n) > (tapped element)
				var element = source.GetAncestors(includeCurrent: true)
					.ZipSkipOne()
					.FirstOrDefault(x => x.Current is ItemsRepeater)
					.Previous;
				if (element is FrameworkElement fe)
				{
					CommandExtensions.TryInvokeCommand(owner, CommandExtensions.GetCommandParameter(fe) ?? fe.DataContext);
				}
			}
		}
	}
}
