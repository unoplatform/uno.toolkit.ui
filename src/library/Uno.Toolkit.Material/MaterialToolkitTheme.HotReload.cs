#if IS_WINUI
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Uno.Toolkit.UI.Material;

[assembly: ElementMetadataUpdateHandlerAttribute(typeof(MaterialToolkitThemeElementMetadataUpdateHandler))]

namespace Uno.Toolkit.UI.Material;

internal static class MaterialToolkitThemeElementMetadataUpdateHandler
{
	/// <summary>
	/// Called after the visual tree has been updated during Hot Reload.
	/// Forces all controls to re-apply their templates to pick up updated theme resources.
	/// </summary>
	public static void AfterVisualTreeUpdate(Type[]? updatedTypes)
	{
		// When resource dictionaries are hot reloaded, we need to force controls
		// to re-apply their templates so they pick up the new brush values.
		// This is especially important for Material-themed controls that use
		// StaticResource in their theme dictionaries.
		
		if (Application.Current?.Resources is not null)
		{
			// Trigger resource update for hot reload
			Application.Current.UpdateResourceBindingsForHotReload();
		}
	}
}
#endif
