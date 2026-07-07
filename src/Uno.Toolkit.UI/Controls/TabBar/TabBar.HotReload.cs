#if IS_WINUI
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

[assembly: ElementMetadataUpdateHandlerAttribute(typeof(Uno.Toolkit.UI.TabBar), typeof(Uno.Toolkit.UI.TabBarElementMetadataUpdateHandler))]

namespace Uno.Toolkit.UI;
internal static class TabBarElementMetadataUpdateHandler
{
	private const string SelectedIndexKey = nameof(TabBar) + "." + nameof(TabBar.SelectedIndex);

	public static void CaptureState(FrameworkElement element, IDictionary<string, object> stateDictionary, Type[]? updatedTypes)
	{
		if (element is TabBar tabBar)
		{
			stateDictionary[SelectedIndexKey] = tabBar.SelectedIndex;
		}
	}

	public static Task RestoreState(FrameworkElement element, IDictionary<string, object> stateDictionary, Type[]? updatedTypes)
	{
		if (element is TabBar tabBar &&
		    stateDictionary.TryGetValue(SelectedIndexKey, out var savedIndex) &&
		    savedIndex is int index and >= 0 &&
		    index < tabBar.Items.Count &&
		    tabBar.GetBindingExpression(TabBar.SelectedIndexProperty) is null)
		{
			tabBar.SelectedIndex = index;
		}

		return Task.CompletedTask;
	}
}
#endif
