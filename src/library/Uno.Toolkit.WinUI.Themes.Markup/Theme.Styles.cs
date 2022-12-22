using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.MarkupHelpers;
using Microsoft.UI.Xaml.MarkupHelpers.Internals;

namespace Uno.Toolkit.Themes.Markup
{
	public static partial class Theme
	{
		public static class Styles
		{
			public static class AppBarButton
			{
				[ResourceKeyDefinition(typeof(Style), "MainCommandStyle", TargetType = typeof(AppBarButton))]
				public static Action<IDependencyPropertyBuilder<Style>> Default => StaticResource.Get<Style>("MainCommandStyle");

				[ResourceKeyDefinition(typeof(Style), "MainCommandStyle", TargetType = typeof(AppBarButton))]
				public static Action<IDependencyPropertyBuilder<Style>> Modal => StaticResource.Get<Style>("ModalMainCommandStyle");

				[ResourceKeyDefinition(typeof(Style), "MainCommandStyle", TargetType = typeof(AppBarButton))]
				public static Action<IDependencyPropertyBuilder<Style>> Primary => StaticResource.Get<Style>("PrimaryMainCommandStyle");

				[ResourceKeyDefinition(typeof(Style), "MainCommandStyle", TargetType = typeof(AppBarButton))]
				public static Action<IDependencyPropertyBuilder<Style>> PrimaryModal => StaticResource.Get<Style>("PrimaryModalMainCommandStyle");

				[ResourceKeyDefinition(typeof(Style), "MainCommandStyle", TargetType = typeof(AppBarButton))]
				public static Action<IDependencyPropertyBuilder<Style>> PrimaryAppBar => StaticResource.Get<Style>("PrimaryAppBarButtonStyle");
			}

			public static class Card
			{
				[ResourceKeyDefinition(typeof(Style), "FilledCardStyle", TargetType = typeof(Card))]
				public static Action<IDependencyPropertyBuilder<Style>> Filled => StaticResource.Get<Style>("FilledCardStyle");

				[ResourceKeyDefinition(typeof(Style), "OutlinedCardStyle", TargetType = typeof(Card))]
				public static Action<IDependencyPropertyBuilder<Style>> Outlined => StaticResource.Get<Style>("OutlinedCardStyle");

				[ResourceKeyDefinition(typeof(Style), "ElevatedCardStyle", TargetType = typeof(Card))]
				public static Action<IDependencyPropertyBuilder<Style>> Elevated => StaticResource.Get<Style>("ElevatedCardStyle");

				public static class Avatar
				{
					[ResourceKeyDefinition(typeof(Style), "AvatarFilledCardStyle", TargetType = typeof(Card))]
					public static Action<IDependencyPropertyBuilder<Style>> Filled => StaticResource.Get<Style>("AvatarFilledCardStyle");

					[ResourceKeyDefinition(typeof(Style), "AvatarOutlinedCardStyle", TargetType = typeof(Card))]
					public static Action<IDependencyPropertyBuilder<Style>> Outlined => StaticResource.Get<Style>("AvatarOutlinedCardStyle");

					[ResourceKeyDefinition(typeof(Style), "AvatarElevatedCardStyle", TargetType = typeof(Card))]
					public static Action<IDependencyPropertyBuilder<Style>> Elevated => StaticResource.Get<Style>("AvatarElevatedCardStyle");
				}

				public static class SmallMedia
				{
					[ResourceKeyDefinition(typeof(Style), "SmallMediaFilledCardStyle", TargetType = typeof(Card))]
					public static Action<IDependencyPropertyBuilder<Style>> Filled => StaticResource.Get<Style>("SmallMediaFilledCardStyle");

					[ResourceKeyDefinition(typeof(Style), "SmallMediaOutlinedCardStyle", TargetType = typeof(Card))]
					public static Action<IDependencyPropertyBuilder<Style>> Outlined => StaticResource.Get<Style>("SmallMediaOutlinedCardStyle");

					[ResourceKeyDefinition(typeof(Style), "SmallMediaElevatedCardStyle", TargetType = typeof(Card))]
					public static Action<IDependencyPropertyBuilder<Style>> Default => StaticResource.Get<Style>("SmallMediaElevatedCardStyle");
				}
			}

			public static class CardContentControl
			{
				[ResourceKeyDefinition(typeof(Style), "FilledCardContentControlStyle", TargetType = typeof(CardContentControl))]
				public static Action<IDependencyPropertyBuilder<Style>> Filled => StaticResource.Get<Style>("FilledCardContentControlStyle");

				[ResourceKeyDefinition(typeof(Style), "OutlinedCardContentControlStyle", TargetType = typeof(CardContentControl))]
				public static Action<IDependencyPropertyBuilder<Style>> Outlined => StaticResource.Get<Style>("OutlinedCardContentControlStyle");

				[ResourceKeyDefinition(typeof(Style), "ElevatedCardContentControlStyle", TargetType = typeof(CardContentControl))]
				public static Action<IDependencyPropertyBuilder<Style>> Elevated => StaticResource.Get<Style>("ElevatedCardContentControlStyle");
			}

			public static class Chip
			{
				public static class Assist
				{
					[ResourceKeyDefinition(typeof(Style), "AssistChipStyle", TargetType = typeof(Chip))]
					public static Action<IDependencyPropertyBuilder<Style>> Default => StaticResource.Get<Style>("AssistChipStyle");

					[ResourceKeyDefinition(typeof(Style), "ElevatedAssistChipStyle", TargetType = typeof(Chip))]
					public static Action<IDependencyPropertyBuilder<Style>> Elevated => StaticResource.Get<Style>("ElevatedAssistChipStyle");
				}

				public static class Input
				{
					[ResourceKeyDefinition(typeof(Style), "InputChipStyle", TargetType = typeof(Chip))]
					public static Action<IDependencyPropertyBuilder<Style>> Default => StaticResource.Get<Style>("InputChipStyle");
				}

				public static class Filter
				{
					[ResourceKeyDefinition(typeof(Style), "FilterChipStyle", TargetType = typeof(Chip))]
					public static Action<IDependencyPropertyBuilder<Style>> Default => StaticResource.Get<Style>("FilterChipStyle");

					[ResourceKeyDefinition(typeof(Style), "ElevatedFilterChipStyle", TargetType = typeof(Chip))]
					public static Action<IDependencyPropertyBuilder<Style>> Elevated => StaticResource.Get<Style>("ElevatedFilterChipStyle");
				}

				public static class Suggestion
				{
					[ResourceKeyDefinition(typeof(Style), "SuggestionChipStyle", TargetType = typeof(Chip))]
					public static Action<IDependencyPropertyBuilder<Style>> Default => StaticResource.Get<Style>("SuggestionChipStyle");

					[ResourceKeyDefinition(typeof(Style), "ElevatedSuggestionChipStyle", TargetType = typeof(Chip))]
					public static Action<IDependencyPropertyBuilder<Style>> Elevated => StaticResource.Get<Style>("ElevatedSuggestionChipStyle");
				}
			}

			public static class ChipGroup
			{
				public static class Assist
				{
					[ResourceKeyDefinition(typeof(Style), "AssistChipGroupStyle", TargetType = typeof(ChipGroup))]
					public static Action<IDependencyPropertyBuilder<Style>> Default => StaticResource.Get<Style>("AssistChipGroupStyle");

					[ResourceKeyDefinition(typeof(Style), "ElevatedAssistChipGroupStyle", TargetType = typeof(ChipGroup))]
					public static Action<IDependencyPropertyBuilder<Style>> Elevated => StaticResource.Get<Style>("ElevatedAssistChipGroupStyle");
				}

				public static class Input
				{
					[ResourceKeyDefinition(typeof(Style), "InputChipGroupStyle", TargetType = typeof(ChipGroup))]
					public static Action<IDependencyPropertyBuilder<Style>> Default => StaticResource.Get<Style>("InputChipGroupStyle");
				}

				public static class Filter
				{
					[ResourceKeyDefinition(typeof(Style), "FilterChipGroupStyle", TargetType = typeof(ChipGroup))]
					public static Action<IDependencyPropertyBuilder<Style>> Default => StaticResource.Get<Style>("FilterChipGroupStyle");

					[ResourceKeyDefinition(typeof(Style), "ElevatedFilterChipGroupStyle", TargetType = typeof(ChipGroup))]
					public static Action<IDependencyPropertyBuilder<Style>> Elevated => StaticResource.Get<Style>("ElevatedFilterChipGroupStyle");
				}

				public static class Suggestion
				{
					[ResourceKeyDefinition(typeof(Style), "SuggestionChipGroupStyle", TargetType = typeof(ChipGroup))]
					public static Action<IDependencyPropertyBuilder<Style>> Default => StaticResource.Get<Style>("SuggestionChipGroupStyle");

					[ResourceKeyDefinition(typeof(Style), "ElevatedSuggestionChipGroupStyle", TargetType = typeof(ChipGroup))]
					public static Action<IDependencyPropertyBuilder<Style>> Elevated => StaticResource.Get<Style>("ElevatedSuggestionChipGroupStyle");
				}
			}

			public static class Divider
			{
				[ResourceKeyDefinition(typeof(Style), "DividerStyle", TargetType = typeof(Divider))]
				public static Action<IDependencyPropertyBuilder<Style>> Default => StaticResource.Get<Style>("DividerStyle");
			}

			public static class FlyoutPresenter
			{
				[ResourceKeyDefinition(typeof(Style), "LeftDrawerFlyoutPresenterStyle", TargetType = typeof(FlyoutPresenter))]
				public static Action<IDependencyPropertyBuilder<Style>> LeftDrawer => StaticResource.Get<Style>("LeftDrawerFlyoutPresenterStyle");

				[ResourceKeyDefinition(typeof(Style), "TopDrawerFlyoutPresenterStyle", TargetType = typeof(FlyoutPresenter))]
				public static Action<IDependencyPropertyBuilder<Style>> TopDrawer => StaticResource.Get<Style>("TopDrawerFlyoutPresenterStyle");

				[ResourceKeyDefinition(typeof(Style), "RightDrawerFlyoutPresenterStyle", TargetType = typeof(FlyoutPresenter))]
				public static Action<IDependencyPropertyBuilder<Style>> RightDrawer => StaticResource.Get<Style>("RightDrawerFlyoutPresenterStyle");

				[ResourceKeyDefinition(typeof(Style), "BottomDrawerFlyoutPresenterStyle", TargetType = typeof(FlyoutPresenter))]
				public static Action<IDependencyPropertyBuilder<Style>> BottomDrawer => StaticResource.Get<Style>("BottomDrawerFlyoutPresenterStyle");
			}

			public static class NavigationBar
			{
				[ResourceKeyDefinition(typeof(Style), "NavigationBarStyle", TargetType = typeof(NavigationBar))]
				public static Action<IDependencyPropertyBuilder<Style>> Default => StaticResource.Get<Style>("NavigationBarStyle");

				[ResourceKeyDefinition(typeof(Style), "ModalNavigationBarStyle", TargetType = typeof(NavigationBar))]
				public static Action<IDependencyPropertyBuilder<Style>> Modal => StaticResource.Get<Style>("ModalNavigationBarStyle");

				[ResourceKeyDefinition(typeof(Style), "PrimaryNavigationBarStyle", TargetType = typeof(NavigationBar))]
				public static Action<IDependencyPropertyBuilder<Style>> Primary => StaticResource.Get<Style>("PrimaryNavigationBarStyle");

				[ResourceKeyDefinition(typeof(Style), "PrimaryModalNavigationBarStyle", TargetType = typeof(NavigationBar))]
				public static Action<IDependencyPropertyBuilder<Style>> PrimaryModal => StaticResource.Get<Style>("PrimaryModalNavigationBarStyle");
			}

			public static class TabBar
			{
				[ResourceKeyDefinition(typeof(Style), "BottomTabBarStyle", TargetType = typeof(TabBar))]
				public static Action<IDependencyPropertyBuilder<Style>> Bottom => StaticResource.Get<Style>("BottomTabBarStyle");

				[ResourceKeyDefinition(typeof(Style), "TopTabBarStyle", TargetType = typeof(TabBar))]
				public static Action<IDependencyPropertyBuilder<Style>> Top => StaticResource.Get<Style>("TopTabBarStyle");

				[ResourceKeyDefinition(typeof(Style), "ColoredTopTabBarStyle", TargetType = typeof(TabBar))]
				public static Action<IDependencyPropertyBuilder<Style>> Colored => StaticResource.Get<Style>("ColoredTopTabBarStyle");
			}

			public static class TabBarItem
			{
				[ResourceKeyDefinition(typeof(Style), "BottomFabTabBarItemStyle", TargetType = typeof(TabBarItem))]
				public static Action<IDependencyPropertyBuilder<Style>> BottomFab => StaticResource.Get<Style>("BottomFabTabBarItemStyle");

				[ResourceKeyDefinition(typeof(Style), "BottomTabBarItemStyle", TargetType = typeof(TabBarItem))]
				public static Action<IDependencyPropertyBuilder<Style>> Bottom => StaticResource.Get<Style>("BottomTabBarItemStyle");
			}
		}
	}
}
