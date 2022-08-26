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

namespace Uno.Toolkit.UI.Material
{
	/// <summary>
	/// Material (Material Design 3) styles for the controls in the Uno.Toolkit.UI library.
	/// </summary>
	public class MaterialToolkitResourcesV2 : ResourceDictionary
	{
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

		private const string StylePrefix = "Material";
		private ResourceDictionary _implicitResources = new ResourceDictionary();

		public MaterialToolkitResourcesV2()
		{
			Source = new Uri($"ms-appx:///{PackageName}/Generated/mergedpages.{PackageNameSuffix}.v2.xaml");

			MapStyleInfo();
		}

		public bool WithImplicitStyles { set => ExportImplicitStyles(value); }

		private void MapStyleInfo()
		{
			var aliasedResources = new ResourceDictionary();
			foreach (var (resKey, sharedKey, isDefaultStyle) in GetStyleInfos())
			{
				var style = GetStyle(resKey);

				if (isDefaultStyle)
				{
					_implicitResources.Add(style.TargetType, style);
				}

				aliasedResources.Add(sharedKey, style);
			}

			// UWP don't allow for res-dict with Source set to contain resource directly:
			// > Local values are not allowed in resource dictionary with Source set
			// but, we can add them through merged-dict instead.
			this.MergedDictionaries.Add(aliasedResources);
		}

		private void ExportImplicitStyles(bool value)
		{
			if (!value) return; // we don't support teardown

			this.MergedDictionaries.Add(_implicitResources);
		}

		private Style GetStyle(string key)
		{
			if (!this.TryGetValue(key, out var resource) || !(resource is Style style))
			{
				// uwp: If the {key} style is clearly defined, but we can't find it here.
				// And, that it only happens on uwp, and not other uno platforms.
				// It means that the style references resources that are not directly included.
				// note: Resources used on Style.Setters need to be directly defined/included, those used in Style.Template dont have to be.
				throw new ArgumentException($"Missing resource: key={key}");
			}
			if (style.TargetType == null)
			{
				throw new InvalidOperationException($"Missing TargetType on style: key={key}");
			}

			return style;
		}

		private IEnumerable<(string ResourceKey, string SharedKey, bool IsDefaultStyle)> GetStyleInfos()
		{
			var result = new List<(string ResourceKey, string SharedKey, bool IsDefaultStyle)>();
			
			Add("MaterialDividerStyle", isImplicit: true);
			Add("MaterialNavigationBarStyle", isImplicit: true);
			Add("MaterialModalNavigationBarStyle");
			Add("MaterialPrimaryNavigationBarStyle");
			Add("MaterialPrimaryModalNavigationBarStyle");
			Add("MaterialMainCommandStyle", isImplicit: true);
			Add("MaterialModalMainCommandStyle");
			Add("MaterialPrimaryMainCommandStyle");
			Add("MaterialPrimaryModalMainCommandStyle");
			Add("MaterialPrimaryAppBarButtonStyle");
			Add("MaterialTopTabBarStyle");
			Add("MaterialColoredTopTabBarStyle");
			Add("MaterialElevatedSuggestionChipStyle");
			Add("MaterialSuggestionChipStyle");
			Add("MaterialInputChipStyle");
			Add("MaterialElevatedFilterChipStyle");
			Add("MaterialFilterChipStyle");
			Add("MaterialElevatedAssistChipStyle");
			Add("MaterialAssistChipStyle");
			Add("MaterialElevatedSuggestionChipGroupStyle");
			Add("MaterialSuggestionChipGroupStyle");
			Add("MaterialInputChipGroupStyle");
			Add("MaterialElevatedFilterChipGroupStyle");
			Add("MaterialFilterChipGroupStyle");
			Add("MaterialElevatedAssistChipGroupStyle");
			Add("MaterialAssistChipGroupStyle");
			Add("MaterialBottomTabBarStyle");
			Add("MaterialBottomTabBarItemStyle");
			Add("MaterialBottomFabTabBarItemStyle");
			Add("MaterialOutlinedCardStyle");
			Add("MaterialFilledCardStyle");
			Add("MaterialElevatedCardStyle");
			Add("MaterialAvatarOutlinedCardStyle");
			Add("MaterialAvatarFilledCardStyle");
			Add("MaterialAvatarElevatedCardStyle");
			Add("MaterialSmallMediaOutlinedCardStyle");
			Add("MaterialSmallMediaFilledCardStyle");
			Add("MaterialSmallMediaElevatedCardStyle");
			Add("MaterialOutlinedCardContentControlStyle");
			Add("MaterialFilledCardContentControlStyle");
			Add("MaterialElevatedCardContentControlStyle");
			Add("MaterialSmallInfoBadgeBottomTabBarItemStyle");
			Add("MaterialLargeInfoBadgeBottomTabBarItemStyle");
			return result;

			void Add(string key, string? alias = null, bool isImplicit = false) =>
				result.Add((key, alias ?? key.Substring(StylePrefix.Length), isImplicit));
		}
	}
}
