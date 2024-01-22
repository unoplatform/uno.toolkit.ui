---
uid: Toolkit.Helpers.TabBarItemExtensions
---

# TabBarItem Extensions

Provides additional features for `TabBarItem`.

## Attached Properties

Property|Type|Description
-|-|-
`OnClickBehaviors`|`TBIOnClickBehaviors`\*|Backing property for the `TabBarItem` on-click behaviors when already selected.
`OnClickBehaviorsTarget`|UIElement|Optional. Backing property for the target of `OnClickBehaviors`.\*

`TBIOnClickBehaviors`\*: Specifies the on-click behaviors of `TabBarItem`:

- `BackNavigation`: Find the first `NavigationView` with back stack to back navigate.
- `ScrollToTop`: Find the first `ListView` or `ScrollViewer` to reset scroll position.
- `Auto`: All of above.

`OnClickBehaviorsTarget`\*: The content host which the on-click behavior is applied is either the target itself or one of its descendent (via deep first search) suitable for the behavior. When omitted, the parent of `TabBar` will serve as the target.

## Usage

```xml
<!-- Include the following XAML namespace to use the samples below -->
xmlns:utu="using:Uno.Toolkit.UI"
...

<Grid>
    <ListView Grid.Row="0" ... />
    <ScrollViewer Grid.Row="0" ... />
    <Frame Grid.Row="0" ... />

    <TabBar Grid.Row="1">
        <utu:TabBarItem ... utu:TabBarItemExtensions.OnClickBehaviors="Auto" />
```
