---
uid: Toolkit.Controls.NavigationBar
---

# NavigationBar (Concise Reference)

## Summary

> This guide covers details for `NavigationBar` specifically. If you are just getting started with the Uno Material Toolkit Library, please see our [general getting started](../getting-started.md) page to make sure you have the correct setup in place.

## Constructors

| Constructor       | Description                                              |
|-------------------|----------------------------------------------------------|
| `NavigationBar()` | Initializes a new instance of the `NavigationBar` class. |

## Modes

The `NavigationBar` supports 2 different modes:

| Mode    | Style                        |
|---------|------------------------------|
| Windows | `XamlDefaultNavigationBar`   |
| Native  | `NativeDefaultNavigationBar` |

## Properties

| Property                     | Windows | iOS | Android | Comments                                                                                                              |
|------------------------------|:-------:|:---:|:-------:|---------------------------------------------------------------------------------------------------------------------- |
| `Background`                 | x       | x   | x       |                                                                                                                       |
| `Content`                    | x       | x   | x       |                                                                                                                       |
| `Foreground`                 | x       | x   | x       |                                                                                                                       |
| `Height`                     | x       | -   | -       | **iOS** and **Android**: Fixed and can't be changed.                                                                  |
| `HorizontalAlignment`        | x       | -   | x       | **iOS**: Always use `HorizontalAlignment.Stretch`.                                                                    |
| `Opacity`                    | x       | x   | x       |                                                                                                                       |
| `Padding`                    | x       | x   | x       | **iOS** and **Android**: Please refer to the `Padding` section.                                                       |
| `MainCommand`                | x       | x   | x       |                                                                                                                       |
| `PrimaryCommands`            | x       | x   | x       |                                                                                                                       |
| `SecondaryCommands`          | x       | -   | x       | **iOS**: Not supported.                                                                                               |
| `VerticalAlignment`          | x       | -   | x       | **iOS**: Always use `VerticalAlignment.Top`.                                                                          |
| `Visibility`                 | x       | x   | x       |                                                                                                                       |
| `Width`                      | x       | -   | x       | **iOS**: Always use `double.NaN`.                                                                                     |
| `HorizontalContentAlignment` | x       | -   | x       | **Android**: Stretch and Left are supported. **Windows**: Set `IsDynamicOverflowEnabled="False"` for proper behavior. |
| `VerticalContentAlignment`   | x       | -   | -       | Only supported on Windows. **Android**: Alignment needs to be done through the content itself.                        |
| ... | ... | ... |

## Events

| Event     | Windows | iOS | Android | Comments |
|-----------|:-------:|:---:|:-------:|----------|
| `Clicked` | x       | x   | x       |          |

---

**Note**: This is a concise reference. 
For complete documentation, see [NavigationBar.md](NavigationBar.md)