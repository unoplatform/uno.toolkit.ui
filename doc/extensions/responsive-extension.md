---
uid: Toolkit.Helpers.ResponsiveExtension
---
# ResponsiveExtension

The `ResponsiveExtension` class is a markup extension that enables the customization of `UIElement` properties based on screen size.
This functionality provides a dynamic and responsive user interface experience.

### Inheritance
Object &#8594; MarkupExtension &#8594; ResponsiveExtension

## Properties
| Property   | Type             | Description                                                |
| ---------- | ---------------- | ---------------------------------------------------------- |
| Narrowest  | object           | Value to be used when the screen size is at its narrowest. |
| Narrow     | object           | Value to be used when the screen size is narrow.           |
| Normal     | object           | Value to be used when the screen size is normal.           |
| Wide       | object           | Value to be used when the screen size is wide.             |
| Widest     | object           | Value to be used when the screen size is at its widest.    |
| Layout     | ResponsiveLayout | Overrides the screen size thresholds/breakpoints.          |

### ResponsiveLayout
Provides the ability to override the default breakpoints (i.e., the window widths at which the value changes) for the screen sizes.
This is done using an instance of the `ResponsiveLayout` class.

#### Properties
| Property   | Type             | Description            |
| ---------- | ---------------- | ---------------------- |
| Narrowest  | double           | Default value is 150.  |
| Narrow     | double           | Default value is 300.  |
| Normal     | double           | Default value is 600.  |
| Wide       | double           | Default value is 800.  |
| Widest     | double           | Default value is 1080. |

## Remarks
**Initialization**: The `ResponsiveHelper` needs to be hooked up to the window's `SizeChanged` event in order for this markup to receive updates when the window size changes.
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
## Platform limitation (UWP-desktop)
`ResponsiveExtension` relies on `MarkupExtension.ProvideValue(IXamlServiceProvider)` to find the target control and property for continuous value updates, and to obtain the property type to apply automatic type conversion, as its value properties are parsed as string by the XAML engine. Since this overload is a recent addition exclusive to WinUI, UWP projects targeting Windows won't have access to these features. Uno UWP projects targeting non-Windows platforms do not face this limitation. However, the Windows app may crash or present unexpected behavior if you attempt to use this markup on a non-string property.
```xml
<Border Background="{utu:Responsive Narrow=Red, Wide=Blue}"
        Tag="This will not work on Uwp for windows" />
```
You can workaround this by declaring the values as resources and using {StaticResource} to refer to them:
```xml
<Page.Resources>
    <SolidColorBrush x:Key="RedBrush">Red</SolidColorBrush>
    <SolidColorBrush x:Key="BlueBrush">Blue</SolidColorBrush>
...

<Border Background="{utu:Responsive Narrow={StaticResource RedBrush},
                                    Wide={StaticResource BlueBrush}}" />
```
## Usage

> [!TIP]
> It is not necessary to define every template or layout breakpoint: Narrowest, Narrow, Normal, Wide, Widest. You can just define the bare minimum needed.

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<TextBlock Background="{utu:Responsive Narrow=Red, Wide=Blue}" Text="Asd" />
```

### Custom thresholds
```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<Page.Resources>
	<utu:ResponsiveLayout x:Key="CustomLayout" Narrow="400" Wide="800" />
</Page.Resources>
...

<TextBlock Text="{utu:Responsive Layout={StaticResource CustomLayout}, Narrow=Narrow, Wide=Wide}" />
```

> [!NOTE]
> This `ResponsiveLayout` can also be provided from different locations. In the order of precedences, they are:
> - from the `Layout` property
> - in the property owner's parent `.Resources` with `x:Key="DefaultResponsiveLayout"`, or the property owner's parent's parent's...
> - in `Application.Resources` with `x:Key="DefaultResponsiveLayout"`
> - from the hardcoded `ResponsiveHelper.Layout`

