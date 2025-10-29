# Add useful clicks to an already-selected TabBarItem

**Goal:** When the current tab is tapped again, do things like scroll-to-top or go back.

## Dependencies
- `Uno.Toolkit.UI` (or `Uno.Toolkit.WinUI`)

## Auto behaviors (best default)
```xml
<TabBar>
  <utu:TabBarItem Content="Feed"
                  utu:TabBarItemExtensions.OnClickBehaviors="Auto" />
</TabBar>
```
- Includes: `BackNavigation` and `ScrollToTop`

## Pick behaviors explicitly
```xml
<Grid>
  <ListView />
  <Frame />

  <TabBar>
    <utu:TabBarItem Content="Feed"
                    utu:TabBarItemExtensions.OnClickBehaviors="ScrollToTop"
                    utu:TabBarItemExtensions.OnClickBehaviorsTarget="{Binding ElementName=Root}" />
  </TabBar>
</Grid>
```
- Target: parent of `TabBar` by default; override with `OnClickBehaviorsTarget`.
