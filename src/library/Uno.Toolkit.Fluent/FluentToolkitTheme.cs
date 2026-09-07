using System;
using Microsoft.UI.Xaml;
using Uno.Fluent;

namespace Uno.Toolkit.UI.Fluent;

/// <summary>
/// Fluent styles and semantic resources for Uno Toolkit controls, including the
/// base Fluent theme's colors, typography, and customization properties.
/// </summary>
/// <remarks>
/// Merge <c>XamlControlsResources</c> before this dictionary. This theme includes
/// the base Toolkit and Fluent resources; no additional theme dictionary is needed.
/// </remarks>
public class FluentToolkitTheme : FluentTheme
{
	private static readonly string[] SemanticStyleKeys =
	{
		"DividerStyle", "NavigationBarStyle", "MainCommandStyle", "ChipStyle", "ChipGroupStyle",
		"ModalNavigationBarStyle", "PrimaryNavigationBarStyle", "PrimaryModalNavigationBarStyle",
		"ModalMainCommandStyle", "PrimaryMainCommandStyle", "PrimaryModalMainCommandStyle", "PrimaryAppBarButtonStyle",
		"TabBarStyle", "TabBarItemStyle", "TopTabBarStyle", "ColoredTopTabBarStyle", "TopTabBarItemStyle", "ColoredTopTabBarItemStyle",
		"BottomTabBarStyle", "BottomTabBarItemStyle", "BottomFabTabBarItemStyle",
		"VerticalTabBarStyle", "VerticalTabBarItemStyle", "FabTabBarItemStyle", "NavigationTabBarItemStyle",
		"ElevatedSuggestionChipStyle", "SuggestionChipStyle", "InputChipStyle",
		"ElevatedFilterChipStyle", "FilterChipStyle", "ElevatedAssistChipStyle", "AssistChipStyle", "DangerChipStyle",
		"ElevatedSuggestionChipGroupStyle", "SuggestionChipGroupStyle", "InputChipGroupStyle",
		"ElevatedFilterChipGroupStyle", "FilterChipGroupStyle", "ElevatedAssistChipGroupStyle", "AssistChipGroupStyle",
		"OutlinedCardStyle", "FilledCardStyle", "ElevatedCardStyle",
		"AvatarOutlinedCardStyle", "AvatarFilledCardStyle", "AvatarElevatedCardStyle",
		"SmallMediaOutlinedCardStyle", "SmallMediaFilledCardStyle", "SmallMediaElevatedCardStyle",
		"BrandFilledCardStyle", "BrandOutlinedCardStyle",
		"OutlinedCardContentControlStyle", "FilledCardContentControlStyle", "ElevatedCardContentControlStyle",
		"LoadingViewStyle", "ExtendedSplashScreenStyle", "DrawerControlStyle", "DrawerFlyoutPresenterStyle",
		"ToolkitDrawerFlyoutPresenterStyle", "LeftDrawerFlyoutPresenterStyle", "TopDrawerFlyoutPresenterStyle",
		"RightDrawerFlyoutPresenterStyle", "BottomDrawerFlyoutPresenterStyle",
		"ResponsiveViewStyle", "SafeAreaStyle", "ZoomContentControlStyle",
	};

	private static readonly (Type Type, string Key)[] ImplicitStyles =
	{
		(typeof(Divider), "DividerStyle"),
		(typeof(NavigationBar), "NavigationBarStyle"),
		(typeof(Chip), "ChipStyle"),
		(typeof(ChipGroup), "ChipGroupStyle"),
		(typeof(TabBar), "TopTabBarStyle"),
		(typeof(TabBarItem), "TopTabBarItemStyle"),
		(typeof(Card), "FilledCardStyle"),
		(typeof(CardContentControl), "FilledCardContentControlStyle"),
		(typeof(LoadingView), "LoadingViewStyle"),
		(typeof(ExtendedSplashScreen), "ExtendedSplashScreenStyle"),
		(typeof(DrawerControl), "DrawerControlStyle"),
		(typeof(DrawerFlyoutPresenter), "ToolkitDrawerFlyoutPresenterStyle"),
		(typeof(ResponsiveView), "ResponsiveViewStyle"),
		(typeof(SafeArea), "SafeAreaStyle"),
		(typeof(ZoomContentControl), "ZoomContentControlStyle"),
	};

	// Initialized before BaseTheme invokes the virtual resource hook.
	private readonly ResourceDictionary _toolkitAliases = new();
	private bool _aliasesInitialized;

	/// <summary>
	/// Initializes Fluent Toolkit with the platform's default palette and fonts.
	/// </summary>
	public FluentToolkitTheme() : this(colorOverride: null, fontOverride: null)
	{
	}

	/// <summary>
	/// Initializes Fluent Toolkit with optional semantic palette and font overrides.
	/// </summary>
	/// <param name="colorOverride">Semantic color overrides for Light and Default (dark) themes.</param>
	/// <param name="fontOverride">Overrides for the semantic typography resources.</param>
	public FluentToolkitTheme(ResourceDictionary? colorOverride = null, ResourceDictionary? fontOverride = null)
		: base(colorOverride, fontOverride)
	{
	}

	/// <inheritdoc />
	protected override string DefaultStylesSource => "ms-appx:///Uno.Toolkit.WinUI.Fluent/Themes/FluentToolkitStyles.xaml";

	/// <inheritdoc />
	protected override void AddThemeSpecificResources()
	{
		base.AddThemeSpecificResources();

		if (!_aliasesInitialized)
		{
			// These controls have only an implicit base style. Inherit from the style
			// in our own source bundle before installing the Fluent implicit defaults.
			InheritBaseStyle(typeof(DrawerControl), "FluentDrawerControlStyle");
			InheritBaseStyle(typeof(ResponsiveView), "FluentResponsiveViewStyle");
			InheritBaseStyle(typeof(SafeArea), "FluentSafeAreaStyle");

			foreach (var key in SemanticStyleKeys)
			{
				if (TryGetValue("Fluent" + key, out var value) && value is Style style)
				{
					_toolkitAliases[key] = style;
				}
			}

			foreach (var (type, key) in ImplicitStyles)
			{
				if (_toolkitAliases.TryGetValue(key, out var value) && value is Style style)
				{
					_toolkitAliases[type] = style;
				}
			}

			_aliasesInitialized = true;
		}

		// Late-bound aliases avoid app-scope StaticResource resolution when this
		// theme is constructed inside a page using another theme at app scope.
		AddThemeDictionary(_toolkitAliases);
	}

	private void InheritBaseStyle(Type type, string key)
	{
		if (TryGetValue(type, out var baseValue) && baseValue is Style baseStyle &&
			TryGetValue(key, out var value) && value is Style style)
		{
			style.BasedOn = baseStyle;
		}
	}
}
