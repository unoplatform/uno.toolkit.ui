---
uid: Toolkit.Helpers.ProgressExtensions
---

# Progress Extensions (Concise Reference)

## Summary

Provides attached properties to easily change the behavior of progress controls (ie. `ProgressBar` and `ProgressRing`), when nested in a complex visual tree.

## Properties

| Property            | Type        | Description                                                                                   |
|---------------------|-------------|-----------------------------------------------------------------------------------------------|
| `IsActive`          | `bool`      | Once applied to a `FrameworkElement`, this property value sets whether its visual descendant progress controls display a loading indicator. |

## Usage Examples

```xml
<!-- Include the following XAML namespace to use the samples below -->
xmlns:utu="using:Uno.Toolkit.UI"
...

<Grid>
    <ContentPresenter 
        x:Name="WeeklyPromotionsPresenter"
        Content="{TemplateBinding WeeklyPromotionsContent}"
        utu:ProgressExtensions.IsActive="True"
...
```

---

**Note**: This is a concise reference. 
For complete documentation, see [progress-extensions.md](progress-extensions.md)