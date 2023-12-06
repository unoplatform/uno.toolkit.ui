#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI;

internal static class ResourceHelper
{
	public static T? ResolveLocalResource<T>(this FrameworkElement? resourceProvider, object key) where T : class
		=> resourceProvider.ResolveLocalResource(key) as T;

	public static object? ResolveLocalResource(this FrameworkElement? resourceProvider, object key)
	{
		while (resourceProvider != null)
		{
			if (resourceProvider.Resources.TryGetValue(key, out var resource)) return resource;

			resourceProvider = resourceProvider.Parent as FrameworkElement;
		}

		return null;
	}

	public static T? ResolveLocalResource<T>(this Application resourceProvider, object key) where T : class
		=> resourceProvider.ResolveLocalResource(key) as T;

	public static object? ResolveLocalResource(this Application resourceProvider, object key)
	{
		if (resourceProvider.Resources.TryGetValue(key, out var resource)) return resource;

		return null;
	}
}
