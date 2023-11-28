---
uid: Toolkit.Controls.ResponsiveView
---
# ResponsiveView

## Summary

The `ResponsiveView` provides the ability to display different content based on screen size, making it easy to adapt to various devices.

### C#
```csharp
public partial class ResponsiveView : ContentControl
```

### XAML
```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<utu:ResponsiveView>
     <!-- Contents -->
</utu:ResponsiveView>
```

### Inheritance 
Object &#8594; DependencyObject &#8594; UIElement &#8594; FrameworkElement &#8594; Control &#8594; ContentControl

### Constructors
| Constructor      | Description                                               |
|------------------|-----------------------------------------------------------|
| ResponsiveView() | Initializes a new instance of the `ResponsiveView` class. |

### Properties
| Property          | Type             | Description                                                                                         |
| ----------------- | ---------------- | --------------------------------------------------------------------------------------------------- |
| NarrowestContent  | DataTemplate     | Content to be displayed on the narrowest screen size.                                               |
| NarrowContent     | DataTemplate     | Content to be displayed on a narrow screen size.                                                    |
| NormalContent     | DataTemplate     | Content to be displayed on a normal screen size.                                                    |
| WideContent       | DataTemplate     | Content to be displayed on a wide screen size.                                                      |
| WidestContent     | DataTemplate     | Content to be displayed on the widest screen size.                                                  |
| ResponsiveLayout  | ResponsiveLayout | Overrides the MaxWidth for each screen size: `Narrowest`, `Narrow`, `Normal`, `Wide`, and `Widest`. |

### Usage

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...
<utu:ResponsiveView>
    <utu:ResponsiveView.NarrowestContent>
        <DataTemplate>
            <!-- Narrowest content -->
        </DataTemplate>
    </utu:ResponsiveView.NarrowestContent>
    <utu:ResponsiveView.NarrowContent>
        <DataTemplate>
            <!-- Narrow content -->
        </DataTemplate>
    </utu:ResponsiveView.NarrowContent>
    <utu:ResponsiveView.NormalContent>
        <DataTemplate>
            <!-- Normal content -->
        </DataTemplate>
    </utu:ResponsiveView.NormalContent>
    <utu:ResponsiveView.WideContent>
        <DataTemplate>
           <!-- Wide content -->
        </DataTemplate>
    </utu:ResponsiveView.WideContent>
    <utu:ResponsiveView.WidestContent>
        <DataTemplate>
           <!-- Widest content -->
        </DataTemplate>
    </utu:ResponsiveView.WidestContent>
</utu:ResponsiveView>
```

## ResponsiveLayout
Provides the ability to override the MaxWidth for each screen size: `Narrowest`, `Narrow`, `Normal`, `Wide`, and `Widest`.

### Properties
| Property   | Type   | Description            |
| ---------- | ------ | ---------------------- |
| Narrowest  | double | Default value is 150.  |
| Narrow     | double | Default value is 300.  |
| Normal     | double | Default value is 600.  |
| Wide       | double | Default value is 800.  |
| Widest     | double | Default value is 1080. |

### Usage

```xml
xmlns:utu="using:Uno.Toolkit.UI"
xmlns:hlp="using:Uno.Toolkit.UI.Helpers"
...
<utu:ResponsiveView>
    <utu:ResponsiveView.ResponsiveLayout>
        <hlp:ResponsiveLayout>
            <hlp:ResponsiveLayout.Narrowest>200</hlp:ResponsiveLayout.Narrowest>
            <hlp:ResponsiveLayout.Narrow>350</hlp:ResponsiveLayout.Narrow>
            <hlp:ResponsiveLayout.Normal>800</hlp:ResponsiveLayout.Normal>
            <hlp:ResponsiveLayout.Wide>1200</hlp:ResponsiveLayout.Wide>
            <hlp:ResponsiveLayout.Widest>1500</hlp:ResponsiveLayout.Widest>
        </hlp:ResponsiveLayout>
    </utu:ResponsiveView.ResponsiveLayout>
    <utu:ResponsiveView.NarrowestContent>
        <DataTemplate>
            <!-- Narrowest content -->
        </DataTemplate>
    </utu:ResponsiveView.NarrowestContent>
    <utu:ResponsiveView.NarrowContent>
        <DataTemplate>
            <!-- Narrow content -->
        </DataTemplate>
    </utu:ResponsiveView.NarrowContent>
    <utu:ResponsiveView.NormalContent>
        <DataTemplate>
            <!-- Normal content -->
        </DataTemplate>
    </utu:ResponsiveView.NormalContent>
    <utu:ResponsiveView.WideContent>
        <DataTemplate>
           <!-- Wide content -->
        </DataTemplate>
    </utu:ResponsiveView.WideContent>
    <utu:ResponsiveView.WidestContent>
        <DataTemplate>
           <!-- Widest content -->
        </DataTemplate>
    </utu:ResponsiveView.WidestContent>
</utu:ResponsiveView>
```