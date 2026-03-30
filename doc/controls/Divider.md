---
uid: Toolkit.Controls.Divider
---
# Divider

A divider is a thin line that groups content in lists and layouts.

## Properties

| Property              | Type     | Description                                          |
|-----------------------|----------|------------------------------------------------------|
| `SubHeader`           | `String` | Gets or sets the text of the text below the Divider. |
| `SubHeaderForeground` | `Brush`  | Gets or sets the foreground of the subheader.        |

## Lightweight Styling
Both Material and Simple themes share the same semantic (unprefixed) resource keys. Override these keys to customize control appearance regardless of which theme your app uses. For more information, see [Lightweight Styling](../lightweight-styling.md).

### Material Theme
| Key                              | Type                 | Value                       |
|------------------------------------|--------------------|-----------------------------|
| `DividerForeground`                | `SolidColorBrush`  | `OutlineVariantBrush`       |
| `DividerSubHeaderForeground`       | `SolidColorBrush`  | `OnSurfaceLowBrush`         |
| `DividerSubHeaderFontFamily`       | `FontFamily`       | `BodySmallFontFamily`       |
| `DividerSubHeaderFontWeight`       | `FontWeight`       | `BodySmallFontWeight`       |
| `DividerSubHeaderFontSize`         | `FontSize`         | `BodySmallFontSize`         |
| `DividerSubHeaderCharacterSpacing` | `CharacterSpacing` | `BodySmallCharacterSpacing` |
| `DividerSubHeaderMargin`           | `Thickness`        | `0,4,0,0`                   |
| `DividerHeight`                    | `Double`           | `1`                         |

### Simple Theme

The Divider uses the same semantic resource keys in the Simple theme:

| Key                              | Type               | Value                                  |
|----------------------------------|--------------------|----------------------------------------|
| `DividerForeground`              | `SolidColorBrush`  | `OutlineBrush`      |
| `DividerSubHeaderForeground`     | `SolidColorBrush`  | `OnSurfaceBrush`        |
| `DividerSubHeaderFontFamily`     | `FontFamily`       | `SimpleFontFamily`                     |
| `DividerSubHeaderFontWeight`     | `FontWeight`       | `Normal`                               |
| `DividerSubHeaderFontSize`       | `FontSize`         | `14`                                   |
| `DividerSubHeaderCharacterSpacing` | `CharacterSpacing` | `0`                                  |
| `DividerSubHeaderMargin`         | `Thickness`        | `0,4,0,0`                              |
| `DividerHeight`                  | `Double`           | `1`                                    |

## Usage

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<TextBlock Text="Asd" />
<utu:Divider />
<TextBlock Text="Asd" />
<utu:Divider Foreground="Gray"
             SubHeader="Separator"
             SubHeaderForeground="Black" />
<TextBlock Text="Asd" />
```
