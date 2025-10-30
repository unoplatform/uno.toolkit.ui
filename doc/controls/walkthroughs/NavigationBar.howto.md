---
uid: Toolkit.Controls.NavigationBar.HowTo
tags: [navigationbar, appbar, toolbar, back-button, navigation, commandbar, native-navigation]
---

# Display a cross-platform navigation bar with back and actions

> **Packages**
>
> Install **one** base package (depending on your app type), plus a design system package.
>
> * WinAppSDK/WinUI 3: `Uno.Toolkit.WinUI` (+ usually `Uno.Toolkit.WinUI.Material`) ([NuGet][1])

Add the namespace once in your XAML:

```xml
xmlns:utu="using:Uno.Toolkit.UI"
```

---

## Show a back button that navigates back

Use the built-in MainCommand in **Back** mode (default). It appears on the left and calls `Frame.GoBack()` when possible.

```xml
<utu:NavigationBar Content="Details" />
```

* The back icon/text is native on iOS/Android; on desktop you can override a resource (`NavigationBarBackIconData`). ([Uno Platform][4])
* The bar auto-handles system back requests when visible. (WinAppSDK note: `SystemNavigationManager` isn't supported.) ([Uno Platform][4])

---

## Use a hamburger (or any action) instead of back

Set **MainCommandMode** to `Action` and provide a custom `AppBarButton`.

```xml
<utu:NavigationBar Content="Home"
                   MainCommandMode="Action">
    <utu:NavigationBar.MainCommand>
        <AppBarButton Label="Menu" Click="OpenMenu">
            <AppBarButton.Icon>
                <BitmapIcon UriSource="ms-appx:///Assets/menu.png"/>
            </AppBarButton.Icon>
        </AppBarButton>
    </utu:NavigationBar.MainCommand>
</utu:NavigationBar>
```

* Use `Action` when the left command isn't back (e.g., burger, prompt). ([Uno Platform][4])

---

## Add primary actions on the right

Put **AppBarButton** items in `PrimaryCommands`.

```xml
<utu:NavigationBar Content="Photos">
    <utu:NavigationBar.PrimaryCommands>
        <AppBarButton Label="Search">
            <AppBarButton.Icon>
                <BitmapIcon UriSource="ms-appx:///Assets/search.png"/>
            </AppBarButton.Icon>
        </AppBarButton>
        <AppBarButton Label="Share">
            <AppBarButton.Icon>
                <BitmapIcon UriSource="ms-appx:///Assets/share.png"/>
            </AppBarButton.Icon>
        </AppBarButton>
    </utu:NavigationBar.PrimaryCommands>
</utu:NavigationBar>
```

* Supports `AppBarButton` only (no `AppBarToggleButton`/`AppBarSeparator`). ([Uno Platform][4])

---

## Add overflow menu (Android only)

Use `SecondaryCommands` — it shows as an overflow (ellipsis) on Android; iOS doesn't support it.

```xml
<utu:NavigationBar Content="Items">
    <utu:NavigationBar.SecondaryCommands>
        <AppBarButton Label="Settings"/>
        <AppBarButton Label="Help"/>
    </utu:NavigationBar.SecondaryCommands>
</utu:NavigationBar>
```

* Not supported on iOS. ([Uno Platform][4])

---

## Change the title text color (foreground)

Use the bar's `Foreground` (solid color only).

```xml
<utu:NavigationBar Content="Inbox" Foreground="White"/>
```

* Affects the title; not the buttons — set button `Foreground` individually. SolidColorBrush only. ([Uno Platform][4])

---

## Put custom content (e.g., a search box) in the title area

`Content` can be a `FrameworkElement` and uses the available center region.

```xml
<utu:NavigationBar>
    <utu:NavigationBar.Content>
        <TextBox PlaceholderText="Search mail"/>
    </utu:NavigationBar.Content>
    <utu:NavigationBar.PrimaryCommands>
        <AppBarButton Label="Filter"/>
    </utu:NavigationBar.PrimaryCommands>
</utu:NavigationBar>
```

* `Horizontal/VerticalContentAlignment` are ignored; on iOS content centers unless `HorizontalAlignment="Stretch"`. ([Uno Platform][4])

---

## Show iOS back label (text next to the chevron)

Provide a `Label` in `MainCommand` (iOS only).

```xml
<utu:NavigationBar Content="Details">
    <utu:NavigationBar.MainCommand>
        <AppBarButton Label="Back"/>
    </utu:NavigationBar.MainCommand>
</utu:NavigationBar>
```

* iOS uses previous page's title for back text; set `Label=""` in a style to hide it globally. ([Uno Platform][4])

---

## Respect safe areas (notches/punch-holes)

Padding is applied automatically on iOS/Android (`SafeArea.Insets=Top`).
No extra work needed for typical layouts. ([Uno Platform][4])

---

## Use bitmap icons for buttons (cross-platform)

Use `BitmapIcon`/bitmap sources for bar buttons to avoid known mobile issues.
(Design guidance also recommends bitmaps for NavigationBar buttons.) ([Uno Platform][5])

---

### Notes for RAG indexers

* Properties frequently asked: `MainCommandMode`, `PrimaryCommands`, `SecondaryCommands`, `Foreground`, `Content`, platform **heights**, **placement** rules. ([Uno Platform][4])

[1]: https://www.nuget.org/packages/Uno.Toolkit.WinUI?utm_source=chatgpt.com "Uno.Toolkit.WinUI 8.2.4"
[4]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/controls/NavigationBar.html "NavigationBar "
[5]: https://platform.uno/docs/articles/external/figma-docs/components/navigation-bar.html?utm_source=chatgpt.com "Navigation Bar"
