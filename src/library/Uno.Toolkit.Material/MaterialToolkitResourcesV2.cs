using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

/// <summary>
/// Material Toolkit resources
/// </summary>
namespace Uno.Toolkit.UI.Material
{
	public sealed class MaterialToolkitResourcesV2 : ResourceDictionary
	{
		private const string StylePrefix = "M3Material";
#if IS_WINUI
		private const string PackageName =
			"Uno.Toolkit.WinUI.Material";
		private const string PackageNameSuffix =
			"winui";
#else
		private const string PackageName =
			"Uno.Toolkit.UI.Material";
		private const string PackageNameSuffix =
			"uwp";
#endif

		public MaterialToolkitResourcesV2()
		{
			Source = new Uri($"ms-appx:///{PackageName}/Generated/mergedpages.{PackageNameSuffix}.v2.xaml");

			MapSharedStyleKeys();
		}

		public bool WithImplicitStyles { set => ExportImplicitStyles(value); }

		private void MapSharedStyleKeys()
		{
			var aliasedResources = new ResourceDictionary();
			foreach (var key in GetSharedStyleKeys())
			{
				var materialStyleKey = StylePrefix + key;
				var style = FindStyle(materialStyleKey);

				aliasedResources.Add(key, style);
			}

			// UWP don't allow for res-dict with Source set to contain resource directly:
			// > Local values are not allowed in resource dictionary with Source set
			// but, we can add them through merged-dict instead.
			this.MergedDictionaries.Add(aliasedResources);
		}

		private void ExportImplicitStyles(bool value)
		{
			if (!value) return; // we don't support teardown

			var implicitResources = new ResourceDictionary();
			foreach (var key in GetImplicitStyles())
			{
				var style = FindStyle(key);

				implicitResources.Add(style.TargetType, style);
			}

			// UWP don't allow for res-dict with Source set to contain resource directly:
			// > Local values are not allowed in resource dictionary with Source set
			// but, we can add them through merged-dict instead.
			this.MergedDictionaries.Add(implicitResources);
		}

		private Style FindStyle(string key)
		{
			if (!this.TryGetValue(key, out var resource) || !(resource is Style style))
			{
				// uwp: If the {key} style is clearly defined in {info.Source}, but we can't find it here.
				// And, that it only happens on uwp, and not other uno platforms.
				// It means that the style references resources that are not directly included.
				// This can usually be fixed by including `<MaterialColors xmlns="using:Uno.Material" />` in the MergedDictionaries of {info.Source}.
				// note: Resources used on Style.Setters need to be directly defined/included, those used in Style.Template dont have to be.
				throw new ArgumentException($"Missing resource: key={key}");
			}
			if (style.TargetType == null)
			{
				throw new InvalidOperationException($"Missing TargetType on style: key={key}");
			}

			return style;
		}

		private string[] GetSharedStyleKeys()
		{
			return new[] {
				"TopTabBarStyle",
				"ModalNavigationBarStyle",
				"NavigationBarStyle"
			};
		}

		private string[] GetImplicitStyles()
		{
			return new[] {
				"TopTabBarStyle",
			};
		}
	}
}
