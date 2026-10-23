#if IS_WINUI
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Uno.Toolkit.UI;

[assembly: ElementMetadataUpdateHandlerAttribute(typeof(SkeletonView), typeof(SkeletonViewElementMetadataUpdateHandler))]

namespace Uno.Toolkit.UI;

// Same as LoadingView: a Source or Content assigned from code is lost when hot reload rebuilds the tree, and a
// SkeletonView without a Source stays in its loading state.
internal static class SkeletonViewElementMetadataUpdateHandler
{
	private const string SourceKey = nameof(SkeletonView) + "." + nameof(SkeletonView.Source);
	private const string ContentKey = nameof(SkeletonView) + "." + nameof(SkeletonView.Content);

	public static void CaptureState(FrameworkElement element, IDictionary<string, object> stateDictionary, Type[]? updatedTypes)
	{
		if (element is SkeletonView view)
		{
			if (view.Source is { } source)
			{
				stateDictionary[SourceKey] = source;
			}
			if (view.Content is { } content)
			{
				stateDictionary[ContentKey] = content;
			}
		}
	}

	public static Task RestoreState(FrameworkElement element, IDictionary<string, object> state, Type[]? updatedTypes)
	{
		if (element is SkeletonView view)
		{
			if (view.Source is null && view.GetBindingExpression(SkeletonView.SourceProperty) is null && state.TryGetValue(SourceKey, out var savedSource) && savedSource is ILoadable source)
			{
				view.Source = source;
			}
			if (view.Content is null && view.GetBindingExpression(SkeletonView.ContentProperty) is null && state.TryGetValue(ContentKey, out var savedContent) && savedContent is { } content)
			{
				view.Content = content;
			}
		}

		return Task.CompletedTask;
	}
}
#endif
