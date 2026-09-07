using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Themes;
using Uno.Toolkit.RuntimeTests.Extensions;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.Toolkit.UI.Fluent;
using Uno.UI.RuntimeTests;

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
public class Given_FluentToolkitTheme
{
	private static readonly string[] SemanticStyleKeys =
	[
		"DividerStyle", "NavigationBarStyle", "MainCommandStyle", "ChipStyle",
		"ModalNavigationBarStyle", "PrimaryNavigationBarStyle", "PrimaryModalNavigationBarStyle",
		"ModalMainCommandStyle", "PrimaryMainCommandStyle", "PrimaryModalMainCommandStyle", "PrimaryAppBarButtonStyle",
		"TabBarStyle", "TabBarItemStyle", "TopTabBarStyle", "ColoredTopTabBarStyle", "TopTabBarItemStyle", "ColoredTopTabBarItemStyle",
		"BottomTabBarStyle", "BottomTabBarItemStyle", "BottomFabTabBarItemStyle",
		"VerticalTabBarStyle", "VerticalTabBarItemStyle", "FabTabBarItemStyle", "NavigationTabBarItemStyle",
		"ElevatedSuggestionChipStyle", "SuggestionChipStyle", "InputChipStyle", "ElevatedFilterChipStyle", "FilterChipStyle",
		"ElevatedAssistChipStyle", "AssistChipStyle", "DangerChipStyle",
		"ElevatedSuggestionChipGroupStyle", "SuggestionChipGroupStyle", "InputChipGroupStyle",
		"ElevatedFilterChipGroupStyle", "FilterChipGroupStyle", "ElevatedAssistChipGroupStyle", "AssistChipGroupStyle",
		"OutlinedCardStyle", "FilledCardStyle", "ElevatedCardStyle",
		"AvatarOutlinedCardStyle", "AvatarFilledCardStyle", "AvatarElevatedCardStyle",
		"SmallMediaOutlinedCardStyle", "SmallMediaFilledCardStyle", "SmallMediaElevatedCardStyle",
		"BrandFilledCardStyle", "BrandOutlinedCardStyle",
		"OutlinedCardContentControlStyle", "FilledCardContentControlStyle", "ElevatedCardContentControlStyle",
		"ChipGroupStyle", "LoadingViewStyle", "ExtendedSplashScreenStyle", "DrawerControlStyle", "DrawerFlyoutPresenterStyle",
		"ToolkitDrawerFlyoutPresenterStyle", "LeftDrawerFlyoutPresenterStyle", "TopDrawerFlyoutPresenterStyle",
		"RightDrawerFlyoutPresenterStyle", "BottomDrawerFlyoutPresenterStyle", "ResponsiveViewStyle", "SafeAreaStyle", "ZoomContentControlStyle",
	];

	[TestMethod]
	public void When_ThemeIsScoped_AliasesResolveItsOwnStyles()
	{
		var foreign = new Style(typeof(Button));
		var conflictingResources = new ResourceDictionary();
		foreach (var key in SemanticStyleKeys)
		{
			conflictingResources[key] = foreign;
			conflictingResources["Fluent" + key] = foreign;
		}
		Application.Current.Resources.MergedDictionaries.Add(conflictingResources);

		try
		{
			// The theme deliberately remains outside Application.Resources while its aliases initialize.
			var theme = new FluentToolkitTheme();
			foreach (var key in SemanticStyleKeys)
			{
				Assert.IsTrue(theme.TryGetValue(key, out var semantic), $"Missing semantic style {key}.");
				Assert.IsTrue(theme.TryGetValue("Fluent" + key, out var fluent), $"Missing Fluent style for {key}.");
				Assert.AreSame(fluent, semantic, $"{key} must refer to this theme's Fluent style.");
				Assert.AreNotSame(foreign, semantic, $"{key} captured a foreign application style.");
				Assert.AreEqual(ExpectedTargetType(key), ((Style)semantic).TargetType, $"{key} targets the wrong control.");
			}
		}
		finally
		{
			Application.Current.Resources.MergedDictionaries.Remove(conflictingResources);
		}
	}

	[TestMethod]
	[DataRow(ElementTheme.Light)]
	[DataRow(ElementTheme.Dark)]
	public async Task When_SemanticStylesApplied_ControlsRealizeInBothThemes(ElementTheme appearance)
	{
		var theme = new FluentToolkitTheme();
		var root = new StackPanel { RequestedTheme = appearance, Width = 600 };
		root.Resources.MergedDictionaries.Add(theme);
		var controls = new (string Key, Control Control)[]
		{
			("DividerStyle", new Divider()),
			("NavigationBarStyle", new NavigationBar { Content = "Navigation" }),
			("PrimaryNavigationBarStyle", new NavigationBar { Content = "Primary navigation" }),
			("ChipStyle", new Chip { Content = "Chip" }),
			("InputChipStyle", new Chip { Content = "Input" }),
			("FilterChipGroupStyle", new ChipGroup { ItemsSource = new[] { "First", "Second" } }),
			("OutlinedCardStyle", new Card { HeaderContent = "Outlined" }),
			("FilledCardStyle", new Card { HeaderContent = "Filled" }),
			("ElevatedCardStyle", new Card { HeaderContent = "Elevated" }),
			("FilledCardContentControlStyle", new CardContentControl { Content = "Content" }),
			("TopTabBarStyle", new TabBar { Items = { new TabBarItem { Content = "Top tab" } } }),
			("BottomTabBarStyle", new TabBar { Items = { new TabBarItem { Content = "Bottom tab" } } }),
			("VerticalTabBarStyle", new TabBar { Items = { new TabBarItem { Content = "Vertical tab" } } }),
			("TopTabBarItemStyle", new TabBarItem { Content = "Tab item" }),
			("LoadingViewStyle", new LoadingView { LoadingContent = "Loading" }),
			("ResponsiveViewStyle", new ResponsiveView { NormalTemplate = XamlHelper.LoadXaml<DataTemplate>("<DataTemplate><TextBlock Text='Responsive' /></DataTemplate>") }),
			("DrawerControlStyle", new DrawerControl { Content = "Drawer host", DrawerContent = "Drawer content", Height = 80 }),
			("SafeAreaStyle", new SafeArea { Content = "Safe content" }),
			("ZoomContentControlStyle", new ZoomContentControl { Content = new TextBlock { Text = "Zoom content" }, Height = 80 }),
		};

		foreach (var (key, control) in controls)
		{
			control.Style = (Style)theme[key];
			root.Children.Add(control);
		}
		await UnitTestUIContentHelperEx.SetContentAndWait(root);

		foreach (var (key, control) in controls)
		{
			Assert.IsNotNull(control.Template, $"{key} has no control template in {appearance}.");
			Assert.IsTrue(VisualTreeHelper.GetChildrenCount(control) > 0, $"{key} did not realize a visual tree in {appearance}.");
			Assert.AreEqual(appearance, control.ActualTheme, key);
			Assert.IsTrue(control.ActualHeight > 0, $"{key} has no visible height.");
		}
	}

	[TestMethod]
	[DataRow(ElementTheme.Light)]
	[DataRow(ElementTheme.Dark)]
	public async Task When_CardSlotsProvided_EachSlotIsPresented(ElementTheme appearance)
	{
		var theme = new FluentToolkitTheme();
		var slots = new[] { "Header", "Subheader", "Avatar", "Media", "Supporting", "Actions" }
			.Select(text => new TextBlock { Text = text }).ToArray();
		var card = new Card
		{
			Style = (Style)theme["AvatarOutlinedCardStyle"],
			HeaderContent = slots[0],
			SubHeaderContent = slots[1],
			AvatarContent = slots[2],
			MediaContent = slots[3],
			SupportingContent = slots[4],
			IconsContent = slots[5],
		};
		// Image-source defaults must be cleared when presenting UIElements directly.
		card.SetValue(Card.AvatarContentTemplateProperty, null);
		card.SetValue(Card.MediaContentTemplateProperty, null);
		var root = new Grid { RequestedTheme = appearance, Width = 600, Children = { card } };
		root.Resources.MergedDictionaries.Add(theme);

		await UnitTestUIContentHelperEx.SetContentAndWait(root);

		foreach (var slot in slots)
		{
			Assert.IsNotNull(VisualTreeHelper.GetParent(slot), $"The {slot.Text} slot was not attached to the card.");
			Assert.IsTrue(slot.ActualHeight > 0 && slot.ActualWidth > 0, $"The {slot.Text} slot was not laid out.");
		}
	}

	[TestMethod]
	public async Task When_FilterChipToggled_GroupSelectionFollows()
	{
		var theme = new FluentToolkitTheme();
		var group = new ChipGroup
		{
			Style = (Style)theme["FilterChipGroupStyle"],
			SelectionMode = ChipSelectionMode.Multiple,
			ItemsSource = new[] { "First", "Second" },
		};
		group.Resources.MergedDictionaries.Add(theme);
		await UnitTestUIContentHelperEx.SetContentAndWait(group);
		var chip = Require(group.ContainerFromIndex(1) as Chip, "The Fluent group must generate Chip containers.");

		chip.Toggle();

		Assert.AreEqual(true, chip.IsChecked);
		CollectionAssert.AreEqual(new[] { "Second" }, group.SelectedItems);
		chip.Toggle();
		Assert.AreEqual(false, chip.IsChecked);
		Assert.IsNull(group.SelectedItems, "Clearing the last selected chip must clear the group selection.");
	}

	[TestMethod]
	public async Task When_InputChipRemoveInvoked_RemovedIsRaisedAndCanBeCancelled()
	{
		var theme = new FluentToolkitTheme();
		var chip = new Chip { Content = "Removable", Style = (Style)theme["InputChipStyle"] };
		chip.Resources.MergedDictionaries.Add(theme);
		var removedCount = 0;
		var cancel = false;
		chip.Removed += (_, _) => removedCount++;
		chip.Removing += (_, args) => args.Cancel = cancel;
		await UnitTestUIContentHelperEx.SetContentAndWait(chip);
		var removeButton = Require(FindDescendant<Button>(chip, "PART_RemoveButton"), "Input chips require a named remove button.");
		Assert.IsTrue(chip.CanRemove, "InputChipStyle must enable removal.");
		Assert.AreEqual(Visibility.Visible, removeButton.Visibility);
		var peer = FrameworkElementAutomationPeer.CreatePeerForElement(removeButton);
		var invoke = Require(peer?.GetPattern(PatternInterface.Invoke) as IInvokeProvider, "The remove button must be invokable for keyboard and automation users.");

		invoke.Invoke();
		await UnitTestUIContentHelperEx.WaitForIdle();
		Assert.AreEqual(1, removedCount);
		cancel = true;
		invoke.Invoke();
		await UnitTestUIContentHelperEx.WaitForIdle();
		Assert.AreEqual(1, removedCount, "Cancelling Removing must suppress Removed.");
	}

	[TestMethod]
	public void When_ThemeRebuilds_StylesAndOverridesRemainAvailable()
	{
		var theme = new FluentToolkitTheme();
		var original = theme["OutlinedCardStyle"];
		theme.Colors = new ThemeColors
		{
			OverrideDictionary = new ResourceDictionary { ["PrimaryColor"] = Colors.Crimson },
		};

		theme.DefaultSpacing = 6;

		Assert.AreSame(original, theme["OutlinedCardStyle"], "Rebuilding tokens must retain the Toolkit style bundle.");
		Assert.AreEqual(Colors.Crimson, theme["PrimaryColor"], "Toolkit resources must not discard consumer color overrides.");
		foreach (var key in SemanticStyleKeys)
		{
			Assert.AreSame(theme["Fluent" + key], theme[key], $"Rebuilding lost alias {key}.");
		}
	}

	[TestMethod]
	[DataRow(ElementTheme.Light)]
	[DataRow(ElementTheme.Dark)]
	public async Task When_InteractionStatesChange_SemanticBrushesApplyAndRestore(ElementTheme appearance)
	{
		var theme = new FluentToolkitTheme(colorOverride: new ResourceDictionary
		{
			["PrimaryBrush"] = new SolidColorBrush(Colors.Crimson),
			["SurfaceVariantBrush"] = new SolidColorBrush(Colors.Pink),
			["OnSurfaceDisabledBrush"] = new SolidColorBrush(Colors.Gray),
		});
		var chip = new Chip { Content = "Stateful", Style = (Style)theme["ChipStyle"] };
		var tab = new TabBarItem { Content = "Stateful", Style = (Style)theme["TopTabBarItemStyle"] };
		var card = new Card { HeaderContent = "Stateful", Style = (Style)theme["OutlinedCardStyle"], IsClickable = true };
		var host = new StackPanel { RequestedTheme = appearance, Children = { chip, tab, card } };
		host.Resources.MergedDictionaries.Add(theme);
		await UnitTestUIContentHelperEx.SetContentAndWait(host);
		var chipRoot = Require(FindDescendant<Grid>(chip, "Root"), "Chip must realize its root grid.");
		var tabRoot = Require(FindDescendant<Grid>(tab, "Root"), "TabBarItem must realize its root grid.");
		var tabContent = Require(FindDescendant<ContentPresenter>(tab, "ContentPresenter"), "TabBarItem must realize its content presenter.");
		var cardOverlay = Require(FindDescendant<Border>(card, "InteractionOverlay"), "Card must realize its interaction overlay.");
		var cardHeader = Require(FindDescendant<ContentPresenter>(card, "HeaderContentPresenter"), "Card must realize its header presenter.");
		var initialChipFill = BrushColor(chipRoot.Background);
		var initialTabFill = BrushColor(tabRoot.Background);
		var initialHeaderColor = BrushColor(cardHeader.Foreground);

		await ChangeState(chip, "PointerOver");
		Assert.AreEqual(Colors.Pink, BrushColor(chipRoot.Background));
		await ChangeState(chip, "Checked");
		Assert.AreEqual(Colors.Crimson, BrushColor(chipRoot.Background));
		await ChangeState(chip, "CheckedPressed");
		Assert.IsTrue(chipRoot.Opacity < 1, "CheckedPressed must provide visible press feedback.");
		await ChangeState(chip, "Normal");
		Assert.AreEqual(initialChipFill, BrushColor(chipRoot.Background));
		Assert.AreEqual(1d, chipRoot.Opacity);

		await ChangeState(tab, "Selected");
		Assert.AreEqual(Colors.Crimson, BrushColor(tabContent.Foreground));
		Assert.AreEqual(Colors.Pink, BrushColor(tabRoot.Background));
		await ChangeState(tab, "Disabled");
		Assert.AreEqual(Colors.Gray, BrushColor(tabContent.Foreground));
		await ChangeState(tab, "Normal");
		Assert.AreEqual(initialTabFill, BrushColor(tabRoot.Background));

		await ChangeState(card, "PointerOver");
		Assert.IsTrue(cardOverlay.Opacity > 0, "PointerOver must expose the interaction overlay.");
		var hoverOpacity = cardOverlay.Opacity;
		await ChangeState(card, "Pressed");
		Assert.IsTrue(cardOverlay.Opacity > hoverOpacity, "Pressed must provide stronger feedback than hover.");
		await ChangeState(card, "Disabled");
		Assert.AreEqual(Colors.Gray, BrushColor(cardHeader.Foreground));
		await ChangeState(card, "Normal");
		Assert.AreEqual(0d, cardOverlay.Opacity);
		Assert.AreEqual(initialHeaderColor, BrushColor(cardHeader.Foreground));
	}

	[TestMethod]
	public async Task When_DangerChipStatesChange_ErrorSemanticsRemain()
	{
		var theme = new FluentToolkitTheme();
		var chip = new Chip { Content = "Delete", Style = (Style)theme["DangerChipStyle"] };
		chip.Resources.MergedDictionaries.Add(theme);
		await UnitTestUIContentHelperEx.SetContentAndWait(chip);
		var root = Require(FindDescendant<Grid>(chip, "Root"), "Chip must realize its root grid.");
		var label = Require(FindDescendant<ContentPresenter>(chip, "ContentPresenter"), "Chip must realize its label.");
		var error = BrushColor((Brush)theme["ErrorBrush"]);
		var onError = BrushColor((Brush)theme["OnErrorBrush"]);
		await ChangeState(chip, "Pressed");
		Assert.AreEqual(error, BrushColor(label.Foreground), "Pressing a danger chip must preserve its error foreground.");
		foreach (var state in new[] { "Checked", "CheckedPointerOver", "CheckedPressed" })
		{
			await ChangeState(chip, state);
			Assert.AreEqual(error, BrushColor(root.Background), state);
			Assert.AreEqual(onError, BrushColor(label.Foreground), state);
		}
		await ChangeState(chip, "Normal");
		Assert.AreEqual(error, BrushColor(label.Foreground));
	}

	[TestMethod]
	public async Task When_DesignTokensCustomized_CardsUseTheSemanticScale()
	{
		var theme = new FluentToolkitTheme { DefaultCornerRadius = 7, DefaultSpacing = 6 };
		var card = new Card { HeaderContent = "Card", Style = (Style)theme["OutlinedCardStyle"] };
		var contentCard = new CardContentControl { Content = "Content card", Style = (Style)theme["FilledCardContentControlStyle"] };
		var host = new StackPanel { Children = { card, contentCard } };
		host.Resources.MergedDictionaries.Add(theme);

		await UnitTestUIContentHelperEx.SetContentAndWait(host);

		Assert.AreEqual(new CornerRadius(14), card.CornerRadius, "Card surfaces use the Fluent overlay radius (twice the base unit).");
		Assert.AreEqual(new CornerRadius(14), contentCard.CornerRadius);
		Assert.AreEqual(new Thickness(24), card.Padding, "Card padding uses four semantic spacing units.");
		Assert.AreEqual(new Thickness(24), contentCard.Padding);
	}

	[TestMethod]
	public async Task When_LoadingSourceChanges_DefaultProgressRingTracksActivity()
	{
		var theme = new FluentToolkitTheme();
		var source = new LoadableSource { IsExecuting = false };
		var view = new LoadingView
		{
			Style = (Style)theme["LoadingViewStyle"],
			Source = source,
			UseTransitions = false,
			Content = "Loaded content",
		};
		view.Resources.MergedDictionaries.Add(theme);
		await UnitTestUIContentHelperEx.SetContentAndWait(view);
		var ring = Require(FindDescendant<ProgressRing>(view), "The Fluent default loading template must realize a ProgressRing.");
		var content = Require(FindDescendant<ContentPresenter>(view, "ContentPresenter"), "LoadingView must realize its content presenter.");
		Assert.IsFalse(ring.IsActive, "A source that starts loaded must not animate the hidden ring.");
		Assert.AreEqual(1d, content.Opacity);

		source.IsExecuting = true;
		await UnitTestUIContentHelperEx.WaitForIdle();
		Assert.IsTrue(ring.IsActive, "Starting work must activate the existing ring.");
		Assert.AreEqual(0d, content.Opacity);

		source.IsExecuting = false;
		await UnitTestUIContentHelperEx.WaitForIdle();
		Assert.IsFalse(ring.IsActive, "Completing work must stop the existing ring.");
		Assert.AreEqual(1d, content.Opacity);
	}

	private static T Require<T>(T? value, string reason) where T : class
		=> value ?? throw new AssertFailedException(reason);

	private static async Task ChangeState(Control control, string state)
	{
		Assert.IsTrue(VisualStateManager.GoToState(control, state, false), $"{control.GetType().Name} is missing {state}.");
		await UnitTestUIContentHelperEx.WaitForIdle();
	}

	private static Windows.UI.Color BrushColor(Brush brush)
	{
		Assert.IsInstanceOfType(brush, typeof(SolidColorBrush));
		return ((SolidColorBrush)brush).Color;
	}

	private static Type ExpectedTargetType(string key)
	{
		if (key.EndsWith("ChipGroupStyle", StringComparison.Ordinal)) return typeof(ChipGroup);
		if (key.EndsWith("ChipStyle", StringComparison.Ordinal)) return typeof(Chip);
		if (key.EndsWith("CardContentControlStyle", StringComparison.Ordinal)) return typeof(CardContentControl);
		if (key.EndsWith("CardStyle", StringComparison.Ordinal)) return typeof(Card);
		if (key.EndsWith("TabBarItemStyle", StringComparison.Ordinal)) return typeof(TabBarItem);
		if (key.EndsWith("TabBarStyle", StringComparison.Ordinal)) return typeof(TabBar);
		if (key.EndsWith("NavigationBarStyle", StringComparison.Ordinal)) return typeof(NavigationBar);
		if (key.EndsWith("MainCommandStyle", StringComparison.Ordinal) || key == "PrimaryAppBarButtonStyle") return typeof(AppBarButton);
		return key switch
		{
			"DividerStyle" => typeof(Divider),
			"LoadingViewStyle" => typeof(LoadingView),
			"ExtendedSplashScreenStyle" => typeof(ExtendedSplashScreen),
			"DrawerControlStyle" => typeof(DrawerControl),
			"ToolkitDrawerFlyoutPresenterStyle" => typeof(DrawerFlyoutPresenter),
			"ResponsiveViewStyle" => typeof(ResponsiveView),
			"SafeAreaStyle" => typeof(SafeArea),
			"ZoomContentControlStyle" => typeof(ZoomContentControl),
			_ when key.EndsWith("DrawerFlyoutPresenterStyle", StringComparison.Ordinal) => typeof(FlyoutPresenter),
			_ => throw new InvalidOperationException($"Add an expected target type for {key}."),
		};
	}

	private static T? FindDescendant<T>(DependencyObject parent, string? name = null) where T : FrameworkElement
	{
		for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
		{
			var child = VisualTreeHelper.GetChild(parent, i);
			if (child is T element && (name is null || element.Name == name))
			{
				return element;
			}
			if (FindDescendant<T>(child, name) is { } descendant)
			{
				return descendant;
			}
		}
		return null;
	}
}
