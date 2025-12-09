---
uid: Toolkit.Controls.TabBar.HowTo
tags: [tabbar, tabs, navigation, bottom-navigation, segmented-control, badge, fab]
---

# Display a list of selectable tabs vertically or horizontally

> Packages used below (reference per how-to):
>
> * `Uno.Toolkit.UI` (controls)
> * `Uno.Toolkit.UI.Material` or `Uno.Toolkit.UI.Cupertino` (prebuilt styles, badges, segmented looks)
> * `Uno.Extensions.Navigation` + `Uno.Extensions.Navigation.Toolkit.WinUI` (if you want tab selection to navigate views)

## Show a bottom tab bar (icons only)

**When to use:** primary, bottom navigation with icons.

**Packages:** `Uno.Toolkit.UI` + `Uno.Toolkit.UI.Material`

```xml
<Page
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:utu="using:Uno.Toolkit.UI">
    <utu:TabBar SelectedIndex="0" Style="{StaticResource BottomTabBarStyle}">
        <utu:TabBarItem>
            <utu:TabBarItem.Icon><SymbolIcon Symbol="Home"/></utu:TabBarItem.Icon>
        </utu:TabBarItem>
        <utu:TabBarItem>
            <utu:TabBarItem.Icon><SymbolIcon Symbol="Find"/></utu:TabBarItem.Icon>
        </utu:TabBarItem>
        <utu:TabBarItem>
            <utu:TabBarItem.Icon><SymbolIcon Symbol="Help"/></utu:TabBarItem.Icon>
        </utu:TabBarItem>
    </utu:TabBar>
</Page>
```

Notes: `SelectedIndex` sets the starting tab. Use Material or Cupertino theme libraries for built-in styles. ([aka.platform.uno][1])

---

## Switch views when a tab is selected

**Goal:** clicking a tab navigates to a view region.

**Packages:** `Uno.Extensions.Navigation` + `Uno.Extensions.Navigation.Toolkit.WinUI`

**App startup (C#):**

```csharp
var builder = this.CreateBuilder(args)
    .UseNavigation()
    .UseToolkitNavigation(); // enables TabBar to drive regions
```

**XAML:**

```xml
<Page
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:utu="using:Uno.Toolkit.UI"
    xmlns:uen="using:Uno.Extensions.Navigation.UI">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- Where views appear -->
        <ContentControl uen:Region.Name="MainRegion"/>

        <!-- Tabs drive the MainRegion -->
        <utu:TabBar Grid.Row="1" Style="{StaticResource BottomTabBarStyle}">
            <utu:TabBarItem Content="Home"  uen:Navigation.Request="Home"/>
            <utu:TabBarItem Content="Search" uen:Navigation.Request="Search"/>
            <utu:TabBarItem Content="About" uen:Navigation.Request="About"/>
        </utu:TabBar>
    </Grid>
</Page>
```

This wires TabBar selection to a named region for navigation. ([Uno Platform][2])

---

## Add a selection indicator (underline / slider)

**Goal:** show an indicator above or below the selected item.

**Packages:** `Uno.Toolkit.UI`

```xml
<utu:TabBar SelectedIndex="0">
    <utu:TabBar.SelectionIndicatorContent>
        <Border Height="2" Background="Red" VerticalAlignment="Bottom"/>
    </utu:TabBar.SelectionIndicatorContent>

    <!-- Above content (default). Use Below to not cover content -->
    <utu:TabBar SelectionIndicatorPlacement="Below"/>

    <utu:TabBar.Items>
        <utu:TabBarItem Content="HOME">
            <utu:TabBarItem.Icon><SymbolIcon Symbol="Home"/></utu:TabBarItem.Icon>
        </utu:TabBarItem>
        <utu:TabBarItem Content="SUPPORT">
            <utu:TabBarItem.Icon><FontIcon Glyph="&#xE8F2;"/></utu:TabBarItem.Icon>
        </utu:TabBarItem>
    </utu:TabBar.Items>
</utu:TabBar>
```

You can switch transition behavior with `SelectionIndicatorTransitionMode="Slide|Snap"`. ([aka.platform.uno][1])

---

## Show a small badge (dot) on a tab

**Goal:** indicate a status change without a number.

**Packages:** `Uno.Toolkit.UI` + `Uno.Toolkit.UI.Material`

```xml
<utu:TabBar Style="{StaticResource BottomTabBarStyle}">
    <utu:TabBarItem Content="Favorites"
                    BadgeVisibility="Visible">
        <utu:TabBarItem.Icon><FontIcon Glyph="&#xE113;"/></utu:TabBarItem.Icon>
    </utu:TabBarItem>
</utu:TabBar>
```

> [!IMPORTANT]
> Apply the style to the `TabBar` container (`BottomTabBarStyle` or `VerticalTabBarStyle`), not to individual `TabBarItem` elements. The TabBar's style automatically styles its children. Do not use `Style="{StaticResource BottomTabBarItemStyle}"` on TabBarItem—this style does not exist in Material Toolkit v2.

Badges are supported when using Material `BottomTabBarStyle` and `VerticalTabBarStyle`. ([aka.platform.uno][1])

---

## Show a numbered badge

**Goal:** show counts (e.g., unread messages).

**Packages:** `Uno.Toolkit.UI` + `Uno.Toolkit.UI.Material`

```xml
<utu:TabBar Style="{StaticResource BottomTabBarStyle}">
    <utu:TabBarItem Content="Mail"
                    BadgeVisibility="Visible"
                    BadgeValue="8">
        <utu:TabBarItem.Icon><FontIcon Glyph="&#xE119;"/></utu:TabBarItem.Icon>
    </utu:TabBarItem>
</utu:TabBar>
```

Setting `BadgeValue` switches to the large (numbered) badge. The TabBar's `BottomTabBarStyle` automatically applies the correct styling to child items. ([aka.platform.uno][1])

---

## Add a center FAB tab with a menu

**Goal:** floating action in the tab row with a flyout.

**Packages:** `Uno.Toolkit.UI` + `Uno.Toolkit.UI.Material`

```xml
<utu:TabBar Style="{StaticResource BottomTabBarStyle}">
    <!-- ...left tab(s)... -->
    <utu:TabBarItem Style="{StaticResource BottomFabTabBarItemStyle}">
        <utu:TabBarItem.Flyout>
            <MenuFlyout Placement="Top">
                <MenuFlyoutItem Text="Like">
                    <MenuFlyoutItem.Icon><SymbolIcon Symbol="Like"/></MenuFlyoutItem.Icon>
                </MenuFlyoutItem>
                <MenuFlyoutItem Text="Dislike">
                    <MenuFlyoutItem.Icon><SymbolIcon Symbol="Dislike"/></MenuFlyoutItem.Icon>
                </MenuFlyoutItem>
            </MenuFlyout>
        </utu:TabBarItem.Flyout>
        <utu:TabBarItem.Icon><SymbolIcon Symbol="Add"/></utu:TabBarItem.Icon>
    </utu:TabBarItem>
    <!-- ...right tab(s)... -->
</utu:TabBar>
```

Use `BottomFabTabBarItemStyle` to embed a FAB within the TabBar. ([aka.platform.uno][1])

---

## Make a segmented switch (Cupertino look)

**Goal:** iOS-style segmented control behavior and visuals.

**Packages:** `Uno.Toolkit.UI` + `Uno.Toolkit.UI.Cupertino`

```xml
<utu:TabBar Style="{StaticResource SegmentedStyle}">
    <utu:TabBar.Items>
        <utu:TabBarItem Content="ORANGE"/>
        <utu:TabBarItem Content="PURPLE"/>
        <utu:TabBarItem Content="BLUE"/>
    </utu:TabBar.Items>
</utu:TabBar>

<!-- Sliding thumb style -->
<utu:TabBar Style="{StaticResource SlidingSegmentedStyle}">
    <utu:TabBar.Items>
        <utu:TabBarItem Content="ORANGE"/>
        <utu:TabBarItem Content="PURPLE"/>
        <utu:TabBarItem Content="BLUE"/>
    </utu:TabBar.Items>
</utu:TabBar>
```

Use `SegmentedStyle` or `SlidingSegmentedStyle` from the Cupertino theme. ([aka.platform.uno][1])

---

## Disable a tab

**Goal:** make a tab not selectable.

**Packages:** `Uno.Toolkit.UI`

```xml
<utu:TabBarItem Content="Admin" IsSelectable="False">
    <utu:TabBarItem.Icon><SymbolIcon Symbol="Contact"/></utu:TabBarItem.Icon>
</utu:TabBarItem>
```

`IsSelectable="False"` prevents selection; `IsEnabled="False"` also disables interaction. ([aka.platform.uno][1])

---

## Run a command when a tab is clicked

**Goal:** execute an `ICommand` on press (e.g., analytics).

**Packages:** `Uno.Toolkit.UI`

```xml
<utu:TabBarItem Content="Search"
                Command="{Binding TabPressed}"
                CommandParameter="Search">
    <utu:TabBarItem.Icon><SymbolIcon Symbol="Find"/></utu:TabBarItem.Icon>
</utu:TabBarItem>
```

`Command` and `CommandParameter` fire on click/tap. ([aka.platform.uno][1])

---

## Scroll to top when re-tapping the selected tab

**Goal:** common mobile UX—reselecting the active tab scrolls its list to top.

**Packages:** `Uno.Toolkit.UI` (TabBarItem extensions)

```xml
<Grid
    xmlns:utu="using:Uno.Toolkit.UI">
    <utu:TabBar>
        <utu:TabBarItem Content="Feed"
            utu:TabBarItemExtensions.OnClickBehaviors="ScrollToTop"
            utu:TabBarItemExtensions.OnClickBehaviorsTarget="{Binding ElementName=ContentHost}"/>
        <!-- other items -->
    </utu:TabBar>

    <!-- content region containing a ListView/ScrollViewer named ContentHost -->
</Grid>
```

Use `OnClickBehaviors="ScrollToTop"` and point `OnClickBehaviorsTarget` to the content host. ([Uno Platform][3])

---

## Back-navigate when re-tapping the selected tab

**Goal:** reselecting an active tab pops the current page within that tab's stack.

**Packages:** `Uno.Toolkit.UI` (TabBarItem extensions)

```xml
<utu:TabBarItem Content="Home"
    utu:TabBarItemExtensions.OnClickBehaviors="BackNavigation"/>
```

`BackNavigation` finds the nearest `NavigationView` with a back stack and navigates back. ([Uno Platform][3])

---

## Listen for selection changes (code-behind)

**Goal:** react when the selected tab changes.

**Packages:** `Uno.Toolkit.UI`

```xml
<utu:TabBar SelectionChanged="OnTabChanged">
    <!-- items -->
</utu:TabBar>
```

```csharp
private void OnTabChanged(utu:TabBar sender, TabBarSelectionChangedEventArgs e)
{
    var oldItem = e.OldItem; // previously selected TabBarItem
    var newItem = e.NewItem; // newly selected TabBarItem
}
```

`SelectionChanged` provides `OldItem` and `NewItem`. ([aka.platform.uno][1])

---

## Place the indicator below content so it doesn't cover text

**Goal:** avoid overlaying the item content.

**Packages:** `Uno.Toolkit.UI`

```xml
<utu:TabBar SelectionIndicatorPlacement="Below">
    <!-- items + indicator template/content -->
</utu:TabBar>
```

"Above" is the default; set `Below` to avoid obscuring content. ([aka.platform.uno][1])

---

## Make a vertical navigation rail

**Goal:** left-side vertical tabs for top-level areas.

**Packages:** `Uno.Toolkit.UI` + `Uno.Toolkit.UI.Material`

```xml
<utu:TabBar Style="{StaticResource VerticalTabBarStyle}">
    <utu:TabBarItem><utu:TabBarItem.Icon><SymbolIcon Symbol="Home"/></utu:TabBarItem.Icon></utu:TabBarItem>
    <utu:TabBarItem><utu:TabBarItem.Icon><SymbolIcon Symbol="Find"/></utu:TabBarItem.Icon></utu:TabBarItem>
    <utu:TabBarItem><utu:TabBarItem.Icon><SymbolIcon Symbol="Help"/></utu:TabBarItem.Icon></utu:TabBarItem>
</utu:TabBar>
```

Material provides `VerticalTabBarStyle`. ([aka.platform.uno][1])

---

## Lightweight style knobs (quick theming)

**Goal:** tweak common sizes/paddings via resource keys.

**Packages:** `Uno.Toolkit.UI.Material`

Examples:
`TopTabBarHeight`, `NavigationTabBarItemIconHeight`, `FabTabBarItemCornerRadius`, badge sizes/margins, etc. Override the keys in your resource dictionaries:

```xml
<Page.Resources>
    <x:Double x:Key="TopTabBarHeight">56</x:Double>
    <Thickness x:Key="NavigationTabBarItemPadding">0,10,0,14</Thickness>
</Page.Resources>
```

See key list in the control docs. ([aka.platform.uno][1])

---

### Related

* Extra TabBar/TabBarItem styles and scenarios (Material & Cupertino). ([aka.platform.uno][1])
* Segmented control looks for TabBar (Cupertino). ([Uno Platform][4])
* Driving navigation with TabBar + Regions. ([Uno Platform][2])

---

### FAQ

**Q: Which theme packages do I need for the ready-made looks?**

Use `Uno.Toolkit.UI.Material` for Bottom/Top/Vertical/FAB + badges. Use `Uno.Toolkit.UI.Cupertino` for segmented looks. ([aka.platform.uno][1])

**Q: Can I animate the selection indicator?**

Yes—set `SelectionIndicatorTransitionMode="Slide"`, or style `SelectionIndicatorPresenterStyle` for deeper customization. ([aka.platform.uno][1])

**Q: How do I wire this to actual navigation?**

Add `UseToolkitNavigation()` and place a `uen:Region.Name` host (e.g., `ContentControl`) your TabBar items target via `uen:Navigation.Request`. ([Uno Platform][2])

**Q: Are badges supported on Cupertino styles?**

Badges are documented for Material (`BottomTabBarItemStyle`, `VerticalTabBarItemStyle`). ([aka.platform.uno][1])

---

[1]: https://aka.platform.uno/toolkit-doc-tabbar-tabbaritem "TabBar & TabBarItem "
[2]: https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Tutorials/Navigation/Advanced/HowTo-UseTabBar.html "How-To: Use a TabBar to Switch Views"
[3]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/helpers/TabBarItem-extensions.html "TabBarItem Extensions"
[4]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/controls/SegmentedControls.html "Segmented Controls"
