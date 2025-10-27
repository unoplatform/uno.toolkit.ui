---
uid: Toolkit.Helpers.ResponsiveExtension
---

# ResponsiveExtension (Concise Reference)

## Summary

The `ResponsiveExtension` class is a markup extension that enables the customization of `UIElement` properties based on screen size.
This functionality provides a dynamic and responsive user interface experience.

## Properties

| Property    | Type               | Description                                                |
|-------------|--------------------|------------------------------------------------------------|
| `Narrowest` | `object`           | Value to be used when the screen size is at its narrowest. |
| `Narrow`    | `object`           | Value to be used when the screen size is narrow.           |
| `Normal`    | `object`           | Value to be used when the screen size is normal.           |
| `Wide`      | `object`           | Value to be used when the screen size is wide.             |
| `Widest`    | `object`           | Value to be used when the screen size is at its widest.    |
| `Layout`    | `ResponsiveLayout` | Overrides the screen size thresholds/breakpoints.          |
| Property    | Type     | Description            |
|-------------|----------|------------------------|
| `Narrowest` | `double` | Default value is 150.  |
| `Narrow`    | `double` | Default value is 300.  |
| `Normal`    | `double` | Default value is 600.  |
| `Wide`      | `double` | Default value is 800.  |
| `Widest`    | `double` | Default value is 1080. |
| Width          | Layout    |
|----------------|-----------|
| ... | ... | ... |

## Usage Examples

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<TextBlock Background="{utu:Responsive Narrow=Red, Wide=Blue}" Text="Asd" />
```

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<Page.Resources>
    <utu:ResponsiveLayout x:Key="CustomLayout"
                          Narrow="400"
                          Wide="800" />
</Page.Resources>
...

<TextBlock Text="{utu:Responsive Layout={StaticResource CustomLayout}, Narrow=Narrow, Wide=Wide}" />
```

---

**Note**: This is a concise reference. 
For complete documentation, see [responsive-extension.md](responsive-extension.md)