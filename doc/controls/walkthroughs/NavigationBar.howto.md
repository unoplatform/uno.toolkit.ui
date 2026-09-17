---
uid: Toolkit.Controls.NavigationBar.HowTo
tags: [navigationbar, appbar, toolbar, back-button, navigation, commandbar, safe-area]
---

# Display a cross-platform navigation bar with back and actions

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`).
Add your design system packages (e.g., `Material` or `Cupertino`) if you use those styles.

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

> [!IMPORTANT]
> For a simple text title, set `Content` directly as a string — **do not wrap it inside any `FrameworkElement`**.

* The back icon comes from the style; with the Material and Simple styles you can override a resource (`NavigationBarBackIconData`). ([Uno Platform][1])
* The bar auto-handles system back requests when visible. (WinAppSDK note: `SystemNavigationManager` isn't supported.) ([Uno Platform][1])

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

* Use `Action` when the left command isn't back (e.g., burger, prompt). ([Uno Platform][1])

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

* Accepts the same items as `CommandBar.PrimaryCommands`. ([Uno Platform][1])

---

## Add an overflow menu

Use `SecondaryCommands` — they show in the overflow (ellipsis) menu.

```xml
<utu:NavigationBar Content="Items">
    <utu:NavigationBar.SecondaryCommands>
        <AppBarButton Label="Settings"/>
        <AppBarButton Label="Help"/>
    </utu:NavigationBar.SecondaryCommands>
</utu:NavigationBar>
```

---

## Change the title text color (foreground)

Use the bar's `Foreground`.

```xml
<utu:NavigationBar Content="Inbox" Foreground="White"/>
```

* Affects the title; not the buttons — set button `Foreground` individually. ([Uno Platform][1])

---

## Put custom content (e.g., a search box) in the title area

> [!NOTE]
> Only use `FrameworkElement` content when you need **interactive controls** like search boxes or custom layouts.
> For simple text, always use `Content="Text"` directly.

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

* `HorizontalContentAlignment="Center"` centers the content across the bar; other values place it next to the back button. ([Uno Platform][1])

---

## Respect safe areas (notches/punch-holes)

The default style applies `SafeArea.Insets="Top"` to the bar.
With the Material and Simple styles, reserve the status bar area on the page instead. ([Uno Platform][1])

```xml
<Grid utu:SafeArea.Insets="VisibleBounds">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*"/>
    </Grid.RowDefinitions>
    <utu:NavigationBar Content="Details"/>
</Grid>
```

---

## Use icons for buttons

Any `IconElement` works (`BitmapIcon`, `PathIcon`, `FontIcon`, `SymbolIcon`).
(Design guidance recommends bitmaps for NavigationBar buttons.) ([Uno Platform][2])

---

### Notes for RAG indexers

* Properties frequently asked: `MainCommandMode`, `PrimaryCommands`, `SecondaryCommands`, `Foreground`, `Content`, **safe area** handling. ([Uno Platform][1])

[1]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/controls/NavigationBar.html "NavigationBar "
[2]: https://platform.uno/docs/articles/external/figma-docs/components/navigation-bar.html "Navigation Bar"
