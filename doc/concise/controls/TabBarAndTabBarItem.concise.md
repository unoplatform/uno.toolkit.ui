---
uid: Toolkit.Controls.TabBar
---

# TabBar & TabBarItem (Concise Reference)

## Summary

> This guide covers details for `TabBar` and `TabBarItem` specifically. If you are just getting started with the Uno Toolkit Library, please see our [general getting started](../getting-started.md) page to make sure you have the correct setup in place.
>
For more information and detailed walkthroughs on using TabBar and TabBarItem, please refer to the rest of the [video playlist](https://www.youtube.com/playlist?list=PLl_OlDcUya9qONoKVz4uGGsEeDbGuaIo_).

## Constructors

| Constructor    | Description                                           |
|----------------|-------------------------------------------------------|
| `TabBarItem()` | Initializes a new instance of the `TabBarItem` class. |

## Properties

| Property           | Type          | Description                                                                                                                                                                                                                                                                                    |
|--------------------|---------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `BadgeValue`       | `string`      | Gets or sets the value to be displayed in the badge of the `TabBarItem`. If a value is set the large badge will be displayed otherwise it will be the small badge. (Currently only supported by the Material Theme Toolkit Library with `BottomTabBarItemStyle` and `VerticalTabBarItemStyle`) |
| `BadgeVisibility`  | `Visibility`  | Gets or sets the badge visibility of the `TabBarItem`. (Currently only supported by the Material Theme Toolkit Library with `BottomTabBarItemStyle` and `VerticalTabBarItemStyle`)                                                                                                             |
| `Command`          | `ICommand`    | Gets or sets the command to invoke when the `TabBarItem` is pressed.                                                                                                                                                                                                                           |
| `CommandParameter` | `object`      | Gets or sets the parameter to pass to the `Command` property.                                                                                                                                                                                                                                  |
| `Flyout`           | `double`      | Gets or sets the flyout associated with this `TabBarItem`.                                                                                                                                                                                                                                     |
| `Icon`             | `IconElement` | Gets or sets the icon of the `TabBarItem`.                                                                                                                                                                                                                                                     |
| `IsSelectable`     | `bool`        | Gets or sets whether the `TabBarItem` can be selected.                                                                                                                                                                                                                                         |

## Events

| Event   | Type                 | Description                              |
|---------|----------------------|------------------------------------------|
| `Click` | `RoutedEventHandler` | Occurs when the `TabBarItem` is pressed. |

## TabBar

`TabBar` is a specialized `ItemsControl` used to present a collection of `TabBarItem`s.

---

**Note**: This is a concise reference. 
For complete documentation, see [TabBarAndTabBarItem.md](TabBarAndTabBarItem.md)