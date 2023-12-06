---
uid: Toolkit.Controls.ResponsiveView
---
# ResponsiveView

The `ResponsiveView` provides the ability to display different content based on screen size, making it easy to adapt to various devices.

## Remarks

The `ResponsiveView` control adapts to different screen sizes by dynamically choosing the right template. It looks at the current screen width and the defined templates. Since not all templates need a value, the control ensures a smooth user experience by picking the smallest defined template that satisfies the width requirements. If no match is found, it defaults to the largest defined template.

**Initialization**: The `ResponsiveHelper` needs to be hooked up to the window's `SizeChanged` event in order for this control to receive updates when the window size changes.
This is typically done in the `OnLaunched` method in the `App` class, where you can get the current `Window` instance for `ResponsiveHelper.HookupEvent`:
```cs
protected override void OnLaunched(LaunchActivatedEventArgs args)
{
#if NET6_0_OR_GREATER && WINDOWS && !HAS_UNO
	MainWindow = new Window();
#else
	MainWindow = Microsoft.UI.Xaml.Window.Current;
#endif

	// ...
	var helper = Uno.Toolkit.UI.ResponsiveHelper.GetForCurrentView();
	helper.HookupEvent(MainWindow);
}
```

## Inheritance
Object &#8594; DependencyObject &#8594; UIElement &#8594; FrameworkElement &#8594; Control &#8594; ContentControl

## Properties
| Property          | Type             | Description                                             |
| ----------------- | ---------------- | ------------------------------------------------------- |
| NarrowestTemplate | DataTemplate     | Template to be displayed on the narrowest screen size.  |
| NarrowTemplate    | DataTemplate     | Template to be displayed on a narrow screen size.       |
| NormalTemplate    | DataTemplate     | Template to be displayed on a normal screen size.       |
| WideTemplate      | DataTemplate     | Template to be displayed on a wide screen size.         |
| WidestTemplate    | DataTemplate     | Template to be displayed on the widest screen size.     |
| ResponsiveLayout  | ResponsiveLayout | Overrides the screen size threshold/breakpoints.        |

### ResponsiveLayout
Provides the ability to override the breakpoint for each screen size: `Narrowest`, `Narrow`, `Normal`, `Wide`, and `Widest`.

### Properties
| Property   | Type   | Description            |
| ---------- | ------ | ---------------------- |
| Narrowest  | double | Default value is 150.  |
| Narrow     | double | Default value is 300.  |
| Normal     | double | Default value is 600.  |
| Wide       | double | Default value is 800.  |
| Widest     | double | Default value is 1080. |

## Usage

> [!TIP]
> It is not necessary to define every template or layout breakpoints: Narrowest,Narrow,Normal,Wide,Widest. You can just define the bare minimum needed.

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

Using `ResponsiveLayout` to customize the screen size breakpoints.

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

> [!NOTE]
> This `ResponsiveLayout` can also be provided from different locations. In the order of precedences, they are:
> - from the `.ResponsiveLayout` property
> - in `ResponsiveView`'s parent `.Resources` with `x:Key="DefaultResponsiveLayout"`, or its ancestor's...
> - in `Application.Resources` with `x:Key="DefaultResponsiveLayout"`
> - from the hardcoded `ResponsiveHelper.Layout`
