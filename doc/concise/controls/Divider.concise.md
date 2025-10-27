---
uid: Toolkit.Controls.Divider
---

# Divider (Concise Reference)

## Summary

A divider is a thin line that groups content in lists and layouts.

## Properties

| Property              | Type     | Description                                          |
|-----------------------|----------|------------------------------------------------------|
| `SubHeader`           | `String` | Gets or sets the text of the text below the Divider. |
| `SubHeaderForeground` | `Brush`  | Gets or sets the foreground of the subheader.        |

## Usage Examples

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

---

**Note**: This is a concise reference. 
For complete documentation, see [Divider.md](Divider.md)