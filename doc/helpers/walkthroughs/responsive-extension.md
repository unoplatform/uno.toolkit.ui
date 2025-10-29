# Change values by screen size

**Goal:** Bind different values at different widths; optionally change breakpoints.

## Dependencies
- `Uno.Toolkit.UI` (or `Uno.Toolkit.WinUI`)

## Quick usage
```xml
xmlns:utu="using:Uno.Toolkit.UI"
<TextBlock Background="{utu:Responsive Narrow=Red, Wide=Blue}" />
```

> If the target type is not a FrameworkElement (e.g., `ColumnDefinition`), define values as resources and reference with `{StaticResource ...}`.

## Custom breakpoints
```xml
<Page.Resources>
  <utu:ResponsiveLayout x:Key="CustomLayout"
                        Narrow="400" Wide="800" />
</Page.Resources>

<TextBlock Text="{utu:Responsive Layout={StaticResource CustomLayout}, Narrow=Narrow, Wide=Wide}" />
```
- Precedence for layout source: explicitly set `Layout` → parent `Resources` (`DefaultResponsiveLayout`) → `Application.Resources` (`DefaultResponsiveLayout`) → built-in defaults `[150,300,600,800,1080]`.

## Notes
- You don't have to define every size (Narrowest/Narrow/Normal/Wide/Widest); define only what you need.
