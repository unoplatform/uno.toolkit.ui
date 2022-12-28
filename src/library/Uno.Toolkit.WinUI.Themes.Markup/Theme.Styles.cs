using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.MarkupHelpers.Internals;
using Uno.Themes.Markup;

namespace Uno.Toolkit.Themes.Markup
{
	public static partial class Theme
	{
		public static class Styles
		{
			public static class AppBarButton
			{
				public static ResourceValue<Style> Default => Main;

				[ResourceKeyDefinition(typeof(Style), "MainCommandStyle", TargetType = typeof(AppBarButton))]
				public static ResourceValue<Style> Main => new("MainCommandStyle");

				[ResourceKeyDefinition(typeof(Style), "MainCommandStyle", TargetType = typeof(AppBarButton))]
				public static ResourceValue<Style> Modal => new("ModalMainCommandStyle");

				[ResourceKeyDefinition(typeof(Style), "MainCommandStyle", TargetType = typeof(AppBarButton))]
				public static ResourceValue<Style> Primary => new("PrimaryMainCommandStyle");

				[ResourceKeyDefinition(typeof(Style), "MainCommandStyle", TargetType = typeof(AppBarButton))]
				public static ResourceValue<Style> PrimaryModal => new("PrimaryModalMainCommandStyle");

				[ResourceKeyDefinition(typeof(Style), "MainCommandStyle", TargetType = typeof(AppBarButton))]
				public static ResourceValue<Style> PrimaryAppBar => new("PrimaryAppBarButtonStyle");
			}

			public static class Card
			{
				[ResourceKeyDefinition(typeof(Style), "FilledCardStyle", TargetType = typeof(Card))]
				public static ResourceValue<Style> Filled => new("FilledCardStyle");

				[ResourceKeyDefinition(typeof(Style), "OutlinedCardStyle", TargetType = typeof(Card))]
				public static ResourceValue<Style> Outlined => new("OutlinedCardStyle");

				[ResourceKeyDefinition(typeof(Style), "ElevatedCardStyle", TargetType = typeof(Card))]
				public static ResourceValue<Style> Elevated => new("ElevatedCardStyle");

				public static class Avatar
				{
					[ResourceKeyDefinition(typeof(Style), "AvatarFilledCardStyle", TargetType = typeof(Card))]
					public static ResourceValue<Style> Filled => new("AvatarFilledCardStyle");

					[ResourceKeyDefinition(typeof(Style), "AvatarOutlinedCardStyle", TargetType = typeof(Card))]
					public static ResourceValue<Style> Outlined => new("AvatarOutlinedCardStyle");

					[ResourceKeyDefinition(typeof(Style), "AvatarElevatedCardStyle", TargetType = typeof(Card))]
					public static ResourceValue<Style> Elevated => new("AvatarElevatedCardStyle");
				}

				public static class SmallMedia
				{
					[ResourceKeyDefinition(typeof(Style), "SmallMediaFilledCardStyle", TargetType = typeof(Card))]
					public static ResourceValue<Style> Filled => new("SmallMediaFilledCardStyle");

					[ResourceKeyDefinition(typeof(Style), "SmallMediaOutlinedCardStyle", TargetType = typeof(Card))]
					public static ResourceValue<Style> Outlined => new("SmallMediaOutlinedCardStyle");

					[ResourceKeyDefinition(typeof(Style), "SmallMediaElevatedCardStyle", TargetType = typeof(Card))]
					public static ResourceValue<Style> Elevated => new("SmallMediaElevatedCardStyle");
				}
			}

			public static class CardContentControl
			{
				[ResourceKeyDefinition(typeof(Style), "FilledCardContentControlStyle", TargetType = typeof(CardContentControl))]
				public static ResourceValue<Style> Filled => new("FilledCardContentControlStyle");

				[ResourceKeyDefinition(typeof(Style), "OutlinedCardContentControlStyle", TargetType = typeof(CardContentControl))]
				public static ResourceValue<Style> Outlined => new("OutlinedCardContentControlStyle");

				[ResourceKeyDefinition(typeof(Style), "ElevatedCardContentControlStyle", TargetType = typeof(CardContentControl))]
				public static ResourceValue<Style> Elevated => new("ElevatedCardContentControlStyle");
			}

			public static class Chip
			{
				public static class Assist
				{
					[ResourceKeyDefinition(typeof(Style), "AssistChipStyle", TargetType = typeof(Chip))]
					public static ResourceValue<Style> Default => new("AssistChipStyle");

					[ResourceKeyDefinition(typeof(Style), "ElevatedAssistChipStyle", TargetType = typeof(Chip))]
					public static ResourceValue<Style> Elevated => new("ElevatedAssistChipStyle");
				}

				public static class Input
				{
					[ResourceKeyDefinition(typeof(Style), "InputChipStyle", TargetType = typeof(Chip))]
					public static ResourceValue<Style> Default => new("InputChipStyle");
				}

				public static class Filter
				{
					[ResourceKeyDefinition(typeof(Style), "FilterChipStyle", TargetType = typeof(Chip))]
					public static ResourceValue<Style> Default => new("FilterChipStyle");

					[ResourceKeyDefinition(typeof(Style), "ElevatedFilterChipStyle", TargetType = typeof(Chip))]
					public static ResourceValue<Style> Elevated => new("ElevatedFilterChipStyle");
				}

				public static class Suggestion
				{
					[ResourceKeyDefinition(typeof(Style), "SuggestionChipStyle", TargetType = typeof(Chip))]
					public static ResourceValue<Style> Default => new("SuggestionChipStyle");

					[ResourceKeyDefinition(typeof(Style), "ElevatedSuggestionChipStyle", TargetType = typeof(Chip))]
					public static ResourceValue<Style> Elevated => new("ElevatedSuggestionChipStyle");
				}
			}

			public static class ChipGroup
			{
				public static class Assist
				{
					[ResourceKeyDefinition(typeof(Style), "AssistChipGroupStyle", TargetType = typeof(ChipGroup))]
					public static ResourceValue<Style> Default => new("AssistChipGroupStyle");

					[ResourceKeyDefinition(typeof(Style), "ElevatedAssistChipGroupStyle", TargetType = typeof(ChipGroup))]
					public static ResourceValue<Style> Elevated => new("ElevatedAssistChipGroupStyle");
				}

				public static class Input
				{
					[ResourceKeyDefinition(typeof(Style), "InputChipGroupStyle", TargetType = typeof(ChipGroup))]
					public static ResourceValue<Style> Default => new("InputChipGroupStyle");
				}

				public static class Filter
				{
					[ResourceKeyDefinition(typeof(Style), "FilterChipGroupStyle", TargetType = typeof(ChipGroup))]
					public static ResourceValue<Style> Default => new("FilterChipGroupStyle");

					[ResourceKeyDefinition(typeof(Style), "ElevatedFilterChipGroupStyle", TargetType = typeof(ChipGroup))]
					public static ResourceValue<Style> Elevated => new("ElevatedFilterChipGroupStyle");
				}

				public static class Suggestion
				{
					[ResourceKeyDefinition(typeof(Style), "SuggestionChipGroupStyle", TargetType = typeof(ChipGroup))]
					public static ResourceValue<Style> Default => new("SuggestionChipGroupStyle");

					[ResourceKeyDefinition(typeof(Style), "ElevatedSuggestionChipGroupStyle", TargetType = typeof(ChipGroup))]
					public static ResourceValue<Style> Elevated => new("ElevatedSuggestionChipGroupStyle");
				}
			}

			public static class Divider
			{
				[ResourceKeyDefinition(typeof(Style), "DividerStyle", TargetType = typeof(Divider))]
				public static ResourceValue<Style> Default => new("DividerStyle");
			}

			public static class FlyoutPresenter
			{
				[ResourceKeyDefinition(typeof(Style), "LeftDrawerFlyoutPresenterStyle", TargetType = typeof(FlyoutPresenter))]
				public static ResourceValue<Style> LeftDrawer => new("LeftDrawerFlyoutPresenterStyle");

				[ResourceKeyDefinition(typeof(Style), "TopDrawerFlyoutPresenterStyle", TargetType = typeof(FlyoutPresenter))]
				public static ResourceValue<Style> TopDrawer => new("TopDrawerFlyoutPresenterStyle");

				[ResourceKeyDefinition(typeof(Style), "RightDrawerFlyoutPresenterStyle", TargetType = typeof(FlyoutPresenter))]
				public static ResourceValue<Style> RightDrawer => new("RightDrawerFlyoutPresenterStyle");

				[ResourceKeyDefinition(typeof(Style), "BottomDrawerFlyoutPresenterStyle", TargetType = typeof(FlyoutPresenter))]
				public static ResourceValue<Style> BottomDrawer => new("BottomDrawerFlyoutPresenterStyle");
			}

			public static class NavigationBar
			{
				[ResourceKeyDefinition(typeof(Style), "NavigationBarStyle", TargetType = typeof(NavigationBar))]
				public static ResourceValue<Style> Default => new("NavigationBarStyle");

				[ResourceKeyDefinition(typeof(Style), "ModalNavigationBarStyle", TargetType = typeof(NavigationBar))]
				public static ResourceValue<Style> Modal => new("ModalNavigationBarStyle");

				[ResourceKeyDefinition(typeof(Style), "PrimaryNavigationBarStyle", TargetType = typeof(NavigationBar))]
				public static ResourceValue<Style> Primary => new("PrimaryNavigationBarStyle");

				[ResourceKeyDefinition(typeof(Style), "PrimaryModalNavigationBarStyle", TargetType = typeof(NavigationBar))]
				public static ResourceValue<Style> PrimaryModal => new("PrimaryModalNavigationBarStyle");
			}

			public static class TabBar
			{
				[ResourceKeyDefinition(typeof(Style), "BottomTabBarStyle", TargetType = typeof(TabBar))]
				public static ResourceValue<Style> Bottom => new("BottomTabBarStyle");

				[ResourceKeyDefinition(typeof(Style), "TopTabBarStyle", TargetType = typeof(TabBar))]
				public static ResourceValue<Style> Top => new("TopTabBarStyle");

				[ResourceKeyDefinition(typeof(Style), "ColoredTopTabBarStyle", TargetType = typeof(TabBar))]
				public static ResourceValue<Style> Colored => new("ColoredTopTabBarStyle");
			}

			public static class TabBarItem
			{
				[ResourceKeyDefinition(typeof(Style), "BottomFabTabBarItemStyle", TargetType = typeof(TabBarItem))]
				public static ResourceValue<Style> BottomFab => new("BottomFabTabBarItemStyle");

				[ResourceKeyDefinition(typeof(Style), "BottomTabBarItemStyle", TargetType = typeof(TabBarItem))]
				public static ResourceValue<Style> Bottom => new("BottomTabBarItemStyle");
			}
		}
	}
}
