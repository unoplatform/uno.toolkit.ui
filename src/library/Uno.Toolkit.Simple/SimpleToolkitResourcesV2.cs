using System;
using Uno.Simple;
using Windows.Foundation.Metadata;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI.Simple
{
	/// <summary>
	/// Simple Design System styles for the controls in the Uno.Toolkit.UI library.
	/// </summary>
	[Deprecated("Resource initialization for the Toolkit Simple theme should now be done using the SimpleToolkitTheme class instead.", DeprecationType.Deprecate, 3)]
	public class SimpleToolkitResourcesV2 : ResourceDictionary
	{
#if IS_WINUI
		private const string PackageName =
			"Uno.Toolkit.WinUI.Simple";
		private const string PackageNameSuffix =
			"WinUI";
#else
		private const string PackageName =
			"Uno.Toolkit.UI.Simple";
		private const string PackageNameSuffix =
			"UWP";
#endif
		public SimpleToolkitResourcesV2()
		{
			Source = new Uri($"ms-appx:///{PackageName}/Generated/mergedpages.{PackageNameSuffix}.xaml");

			MergedDictionaries.Add(new SimpleTheme());
		}
	}
}
