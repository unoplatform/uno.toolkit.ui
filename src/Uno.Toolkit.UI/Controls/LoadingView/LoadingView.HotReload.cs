#if IS_WINUI
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Uno.Toolkit.UI;

[assembly: ElementMetadataUpdateHandlerAttribute(typeof(LoadingView), typeof(LoadingViewElementMetadataUpdateHandler))]

namespace Uno.Toolkit.UI;

internal static class LoadingViewElementMetadataUpdateHandler
{
	private const string SourceKey = nameof(LoadingView) + "." + nameof(LoadingView.Source);
	private const string ContentKey = nameof(LoadingView) + "." + nameof(LoadingView.Content);

	public static void CaptureState(FrameworkElement element, IDictionary<string, object> stateDictionary, Type[]? updatedTypes)
	{
		if (element is LoadingView lv)
		{
			stateDictionary[SourceKey] = lv.Source;
			stateDictionary[ContentKey] = lv.Content;
		}
	}

	public static Task RestoreState(FrameworkElement element, IDictionary<string, object> state, Type[]? updatedTypes)
	{
		if (element is LoadingView lv)
		{
			if (lv.Source is null && lv.GetBindingExpression(LoadingView.SourceProperty) is null && state.TryGetValue(SourceKey, out var savedSrc) && savedSrc is ILoadable src)
			{
				lv.Source = src;
			}
			if (lv.Content is null && lv.GetBindingExpression(LoadingView.ContentProperty) is null && state.TryGetValue(ContentKey, out var savedContent) && savedContent is { } content)
			{
				lv.Content = content;
			}
		}

		return Task.CompletedTask;
	}
}
#endif
