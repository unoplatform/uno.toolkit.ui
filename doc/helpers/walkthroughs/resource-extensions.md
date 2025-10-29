# Apply a ResourceDictionary directly to a control style

**Goal:** Lightweight-style many controls without repeating setters.

## Dependencies
- `Uno.Toolkit.UI` (or `Uno.Toolkit.WinUI`)

## Example: override colors for filled buttons
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
            <SolidColorBrush x:Key="FilledButtonForeground" Color="DarkGreen" />
            <StaticResource x:Key="FilledButtonBackground" ResourceKey="SystemControlTransparentBrush" />
          </ResourceDictionary>
          <ResourceDictionary x:Key="Light">
            <SolidColorBrush x:Key="FilledButtonForeground" Color="DarkGreen" />
            <StaticResource x:Key="FilledButtonBackground" ResourceKey="SystemControlTransparentBrush" />
          </ResourceDictionary>
        </ResourceDictionary.ThemeDictionaries>
      </ResourceDictionary>
    </Setter.Value>
  </Setter>
</Style>
```
- Reuse the same dictionary across multiple styles.
