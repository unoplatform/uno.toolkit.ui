---
uid: Toolkit.Helpers.TabBarItemExtensions
---

# TabBarItem Extensions (Concise Reference)

## Summary

Provides additional features for `TabBarItem`.

## Properties

Property|Type|Description
-|-|-
`OnClickBehaviors`|`TBIOnClickBehaviors`\*|Backing property for the `TabBarItem` on-click behaviors when already selected.
`OnClickBehaviorsTarget`|UIElement|Optional. Backing property for the target of `OnClickBehaviors`.\*

## Usage Examples

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

---

**Note**: This is a concise reference. 
For complete documentation, see [TabBarItem-extensions.md](TabBarItem-extensions.md)