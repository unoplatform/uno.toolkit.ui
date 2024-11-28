using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endif

namespace Uno.Toolkit.UI
{
	internal static class ItemsControlExtensions
	{
		/// <summary>
		/// Get the items.
		/// </summary>
		public static IEnumerable GetItems(this ItemsControl itemsControl) =>
			itemsControl.ItemsSource as IEnumerable ??
			(itemsControl.ItemsSource as CollectionViewSource)?.View ??
			itemsControl.Items ??
			Enumerable.Empty<object>();

		/// <summary>
		/// Gets the container for the given item.
		/// </summary>
		/// <remarks>The item itself maybe its own container.</remarks>
		public static T? FindContainer<T>(this ItemsControl itemsControl, object? item)
			where T : class?
		{
			if (item == null) return null;

			// For some obscure reason, ContainerFromItem returns null when item is an enum.
			// Note however that it works fine for other value types such as int.
			// Because of this, we retrieve the container using the index instead.
			if (item is Enum)
			{
				var index = itemsControl.GetItems().OfType<object>().ToList().IndexOf(item);
				if (index != -1)
				{
					return itemsControl.ContainerFromIndex(index) as T;
				}
			}

			return
				item as T ??
				itemsControl.ContainerFromItem(item) as T;
		}

		/// <summary>
		/// Get the item containers.
		/// </summary>
		/// <remarks>An empty enumerable will returned if the <see cref="ItemsControl.ItemsPanelRoot"/> and the containers have not been materialized.</remarks>
		public static IEnumerable<T> GetItemContainers<T>(this ItemsControl itemsControl) =>
			itemsControl.ItemsPanelRoot?.Children.OfType<T>() ??
			Enumerable.Empty<T>();

		/// <summary>
		/// Gets the container for the given index.
		/// </summary>
		public static T? ContainerFromIndexSafe<T>(this ItemsControl itemsControl, int index) where T : class?
		{
			if (index >= 0 && index < itemsControl.Items.Count)
			{
				return itemsControl.ContainerFromIndex(index) as T;
			}

			return null;
		}
	}
}
