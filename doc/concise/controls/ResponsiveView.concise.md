---
uid: Toolkit.Controls.ResponsiveView
---

# ResponsiveView (Concise Reference)

## Summary

The `ResponsiveView` provides the ability to display different content based on screen size, making it easy to adapt to various devices.

## Properties

| Property            | Type               | Description                                             |
| ------------------- | ------------------ | ------------------------------------------------------- |
| `NarrowestTemplate` | `DataTemplate`     | Template to be displayed on the narrowest screen size.  |
| `NarrowTemplate`    | `DataTemplate`     | Template to be displayed on a narrow screen size.       |
| `NormalTemplate`    | `DataTemplate`     | Template to be displayed on a normal screen size.       |
| `WideTemplate`      | `DataTemplate`     | Template to be displayed on a wide screen size.         |
| `WidestTemplate`    | `DataTemplate`     | Template to be displayed on the widest screen size.     |
| `ResponsiveLayout`  | `ResponsiveLayout` | Overrides the screen size threshold/breakpoints.        |
| Property     | Type     | Description            |
| ------------ | -------- | ---------------------- |
| `Narrowest`  | `double` | Default value is 150.  |
| `Narrow`     | `double` | Default value is 300.  |
| `Normal`     | `double` | Default value is 600.  |
| `Wide`       | `double` | Default value is 800.  |
| `Widest`     | `double` | Default value is 1080. |
| Width          | Layout    |
|----------------|-----------|
| ... | ... | ... |

## Usage Examples

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<utu:ResponsiveView>
    <!-- note:  -->
    <utu:ResponsiveView.NarrowTemplate>
        <DataTemplate>
            <!-- Narrow content -->
        </DataTemplate>
    </utu:ResponsiveView.NarrowTemplate>
    <utu:ResponsiveView.WideTemplate>
        <DataTemplate>
           <!-- Wide content -->
        </DataTemplate>
    </utu:ResponsiveView.WideTemplate>
</utu:ResponsiveView>
```

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...
<utu:ResponsiveView>
    <utu:ResponsiveView.ResponsiveLayout>
        <utu:ResponsiveLayout>
            <utu:ResponsiveLayout.Narrow>350</utu:ResponsiveLayout.Narrow>
            <utu:ResponsiveLayout.Wide>1200</utu:ResponsiveLayout.Wide>
        </utu:ResponsiveLayout>
    </utu:ResponsiveView.ResponsiveLayout>

    <utu:ResponsiveView.NarrowTemplate>
        <DataTemplate>
            <!-- Narrow content -->
        </DataTemplate>
    </utu:ResponsiveView.NarrowTemplate>
    <utu:ResponsiveView.WideTemplate>
        <DataTemplate>
           <!-- Wide content -->
        </DataTemplate>
    </utu:ResponsiveView.WideTemplate>
</utu:ResponsiveView>
```

---

**Note**: This is a concise reference. 
For complete documentation, see [ResponsiveView.md](ResponsiveView.md)