---
uid: Toolkit.Helpers.ResourceExtensions
---

# Resource Extensions (Concise Reference)

## Summary

This extension facilitates assigning a specific `ResourceDictionary` directly to a control's style. It simplifies [lightweight styling](../lightweight-styling.md) by eliminating the necessity to declare each resource on the page explicitly, enabling the easy creation of diverse visual elements with shared styles but varied attributes. The extension also supports the reuse of resource dictionaries across different control styles, enhancing consistency and efficiency in the UI design process.

## Properties

| Property    | Type                 | Description                                                                |
|-------------|----------------------|----------------------------------------------------------------------------|
| `Resources` | `ResourceDictionary` | Gets or sets the `ResourceDictionary` to be applied to the control's style |

## Usage Examples

```xml
<Style x:Key="OverridenFilledButtonStyle"
       xmlns:utu="using:Uno.Toolkit.UI"
       TargetType="Button"
       BasedOn="{StaticResource MaterialFilledButtonStyle}">
    <Setter Property="utu:ResourceExtensions.Resources">
        <Setter.Value>
            <ResourceDictionary>
                <ResourceDictionary.ThemeDictionaries>
                    <ResourceDictionary x:Key="Default">
                        <SolidColorBrush x:Key="FilledButtonForeground"
                                          Color="DarkGreen" />
                        <StaticResource x:Key="FilledButtonBackground"
                                        ResourceKey="SystemControlTransparentBrush" />
                    </ResourceDictionary>
                    <ResourceDictionary x:Key="Light">
                        <SolidColorBrush x:Key="FilledButtonForeground"
                                          Color="DarkGreen" />
                        <StaticResource x:Key="FilledButtonBackground"
                                        ResourceKey="SystemControlTransparentBrush" />
                    </ResourceDictionary>
                </ResourceDictionary.ThemeDictionaries>
            </ResourceDictionary>
        </Setter.Value>
    </Setter>
</Style>
```

---

**Note**: This is a concise reference. 
For complete documentation, see [resource-extensions.md](resource-extensions.md)