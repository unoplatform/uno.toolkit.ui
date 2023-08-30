---
uid: Toolkit.Controls.Divider
---
# Divider
A divider is a thin line that groups content in lists and layouts.

## Properties
Property|Type|Description
-|-|-
SubHeader|String|Gets or sets the text of the text below the Divider.
SubHeaderForeground|Brush|Gets or sets the foreground of the subheader.

### C#
```csharp
public partial class Divider : Control
```

### XAML
```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

content
<utu:Divider/>
content

```

## Lightweight Styling

Key|Type|Value
-|-|-
DividerForeground|SolidColorBrush|OutlineVariantBrush
DividerSubHeaderForeground|SolidColorBrush|OnSurfaceLowBrush
DividerSubHeaderFontFamily|FontFamily|MaterialMediumFontFamily
DividerSubHeaderFontWeight|FontWeight|BodySmallFontWeight
DividerSubHeaderFontSize|FontSize|BodySmallFontSize
DividerSubHeaderCharacterSpacing|CharacterSpacing|BodySmallCharacterSpacing
DividerSubHeaderMargin|Thickness|0,4,0,0
DividerHeight|Double|1
