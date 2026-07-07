# How to react when a tab is tapped again

This feature lets a **selected** `TabBarItem` do something when the user taps it again.
It comes from **Uno.Toolkit.UI**.

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

XAML namespace used in all samples:

```xml
xmlns:utu="using:Uno.Toolkit.UI"
```

---

## How to scroll the current list to the top

**Outcome:** user re-taps the selected tab → current list/scroll area goes back to the top.

**When to use:** tab shows a long list and you want a “jump to top” without adding a button.

```xml
<Grid>
    <!-- Your scrollable content -->
    <ListView x:Name="ItemsList" />

    <TabBar VerticalAlignment="Bottom">
        <utu:TabBarItem
            Icon="Home"
            Header="Home"
            utu:TabBarItemExtensions.OnClickBehaviors="ScrollToTop" />
    </TabBar>
</Grid>
```

**What it does**

* Looks for a `ListView` or `ScrollViewer` in the target content
* Resets its scroll position to top
* Triggered only when the tab was already selected

---

## How to go back in navigation when the tab is tapped again

**Outcome:** user re-taps the selected tab → app navigates back (like clicking the navigation back button).

**When to use:** tab shows a navigation area (`Frame`, `NavigationView`) and you want a “back to previous page” gesture on re-tap.

```xml
<Grid>
    <!-- Host that has navigation / back stack -->
    <Frame x:Name="ContentFrame" />

    <TabBar VerticalAlignment="Bottom">
        <utu:TabBarItem
            Icon="Back"
            Header="Browse"
            utu:TabBarItemExtensions.OnClickBehaviors="BackNavigation" />
    </TabBar>
</Grid>
```

**What it does**

* Finds the first `NavigationView` (or a host with a back stack)
* Calls back navigation if it can
* Triggered only when the tab was already selected
* If no back stack is found, nothing happens

---

## How to enable “do everything” on re-tap

**Outcome:** user re-taps the selected tab → it tries to go back, and also tries to scroll to top.

This is the `Auto` mode.

```xml
<Grid>
    <Frame x:Name="ContentFrame" />
    <ListView x:Name="ItemsList" />

    <TabBar VerticalAlignment="Bottom">
        <utu:TabBarItem
            Icon="Home"
            Header="Home"
            utu:TabBarItemExtensions.OnClickBehaviors="Auto" />
    </TabBar>
</Grid>
```

**What it does (order):**

1. Look for navigation with back stack → go back
2. Look for list / scroll viewer → scroll to top

Use this if you don’t want to decide which behavior to use.

---

## How to target a specific content area

Sometimes the tab bar is not directly above the content, or there are **multiple** lists. Use `OnClickBehaviorsTarget` to tell the extension **where** to look.

**Outcome:** user re-taps the selected tab → behavior runs **inside the element you point to**, not the default parent.

```xml
<Grid>
    <!-- This is the area we want to affect -->
    <Grid x:Name="ContentHost">
        <ScrollViewer>
            <!-- long content -->
        </ScrollViewer>
    </Grid>

    <TabBar VerticalAlignment="Bottom">
        <utu:TabBarItem
            Icon="Home"
            Header="Home"
            utu:TabBarItemExtensions.OnClickBehaviors="ScrollToTop"
            utu:TabBarItemExtensions.OnClickBehaviorsTarget="{Binding ElementName=ContentHost}" />
    </TabBar>
</Grid>
```

**What it does**

* Uses `OnClickBehaviorsTarget` as the starting point
* Searches **inside** it (deep search) for something that supports the behavior
* If you don’t set it, the parent of the `TabBar` is used

---

## How to wire it when TabBar is not in the same control

If the `TabBar` is in a separate control (e.g. a shell, a page template), you can still point to the real content area.

```xml
<Page
    x:Class="MyApp.Shell"
    xmlns:utu="using:Uno.Toolkit.UI">

    <Grid>
        <!-- Shared content host -->
        <Frame x:Name="MainFrame" />

        <TabBar VerticalAlignment="Bottom">
            <utu:TabBarItem
                Header="Home"
                utu:TabBarItemExtensions.OnClickBehaviors="Auto"
                utu:TabBarItemExtensions.OnClickBehaviorsTarget="{Binding ElementName=MainFrame}" />
        </TabBar>
    </Grid>
</Page>
```

This ensures the behavior applies to the frame, **not** to the shell grid.

---

## Reference (RAG-friendly)

**Attached properties**

* `utu:TabBarItemExtensions.OnClickBehaviors`

  * Values: `BackNavigation` | `ScrollToTop` | `Auto`
  * Runs when: tab is already selected and user taps it again

* `utu:TabBarItemExtensions.OnClickBehaviorsTarget`

  * Type: `UIElement`
  * Optional
  * Start point to search for content that supports the selected behavior

**Behavior rules from the original doc** ([Uno Platform][1])

* `BackNavigation`: find first `NavigationView` with back stack → go back
* `ScrollToTop`: find first `ListView` or `ScrollViewer` → scroll to top
* `Auto`: do both
* If target not set → use the parent of the `TabBar` as the target

[1]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/helpers/TabBarItem-extensions.html "TabBarItem Extensions"
