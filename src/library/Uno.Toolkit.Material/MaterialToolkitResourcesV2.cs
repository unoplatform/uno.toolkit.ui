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

		private const string StylePrefix = "M3Material";
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
			
			Add("M3MaterialDividerStyle", isImplicit: true);
			Add("M3MaterialNavigationBarStyle", isImplicit: true);
			Add("M3MaterialModalNavigationBarStyle");
			Add("M3MaterialMainCommandStyle", isImplicit: true);
			Add("M3MaterialModalMainCommandStyle");
			Add("M3MaterialTopTabBarStyle");
			Add("M3MaterialColoredTopTabBarStyle");
			Add("M3MaterialElevatedSuggestionChipStyle");
			Add("M3MaterialSuggestionChipStyle");
			Add("M3MaterialInputChipStyle");
			Add("M3MaterialElevatedFilterChipStyle");
			Add("M3MaterialFilterChipStyle");
			Add("M3MaterialElevatedAssistChipStyle");
			Add("M3MaterialAssistChipStyle");
			Add("M3MaterialElevatedSuggestionChipGroupStyle");
			Add("M3MaterialSuggestionChipGroupStyle");
			Add("M3MaterialInputChipGroupStyle");
			Add("M3MaterialElevatedFilterChipGroupStyle");
			Add("M3MaterialFilterChipGroupStyle");
			Add("M3MaterialElevatedAssistChipGroupStyle");
			Add("M3MaterialAssistChipGroupStyle");
			Add("M3MaterialBottomTabBarStyle");
			Add("M3MaterialBottomTabBarItemStyle");
			Add("M3MaterialBottomFabTabBarItemStyle");
			
			Add("M3MaterialOutlinedCardStyle");
			Add("M3MaterialFilledCardStyle");
			Add("M3MaterialElevatedCardStyle");
			Add("M3MaterialAvatarOutlinedCardStyle");
			Add("M3MaterialAvatarFilledCardStyle");
			Add("M3MaterialAvatarElevatedCardStyle");
			Add("M3MaterialSmallMediaOutlinedCardStyle");
			Add("M3MaterialSmallMediaFilledCardStyle");
			Add("M3MaterialSmallMediaElevatedCardStyle");
			Add("M3MaterialOutlinedCardContentControlStyle");
			Add("M3MaterialFilledCardContentControlStyle");
			Add("M3MaterialElevatedCardContentControlStyle");
			return result;

			void Add(string key, string? alias = null, bool isImplicit = false) =>
				result.Add((key, alias ?? key.Substring(StylePrefix.Length), isImplicit));
		}
	}
}
