using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif
using Uno.Toolkit.UI;

[assembly: ElementMetadataUpdateHandlerAttribute(typeof(LoadingView), typeof(LoadingViewElementMetadataUpdateHandler))]
[assembly: ElementMetadataUpdateHandlerAttribute(typeof(ExtendedSplashScreen), typeof(LoadingViewElementMetadataUpdateHandler))]

namespace Uno.Toolkit.UI;
internal static class LoadingViewElementMetadataUpdateHandler
{

	public static void CaptureState(FrameworkElement element, IDictionary<string, object> stateDictionary, Type[]? updatedTypes)
	{
		if (element is LoadingView lv)
		{
			if (lv.Source is { } src)
			{
				stateDictionary["LV.Source"] = src;
			}
			if (lv.Content is { } content)
			{
				stateDictionary["LV.Content"] = content;
			}
		}

	}

	public static Task RestoreState(FrameworkElement element, IDictionary<string, object> state, Type[]? updatedTypes)
	{
		if (element is LoadingView lv)
		{
			if (lv.Source is null && state.TryGetValue("LV.Source", out var savedSrc) && savedSrc is ILoadable src)
			{
				lv.Source = src;
			}

			if (lv.Content is null && state.TryGetValue("LV.Content", out var savedContent) && savedContent is { } content)
			{
				lv.Content = content;
			}
		}

		return Task.CompletedTask;
	}
}
