---
uid: Toolkit.Helpers.ResponsiveExtension
---
# ResponsiveExtension

## Summary
The `ResponsiveExtension` class is a markup extension that enables the customization of `UIElement` properties based on screen size.
This functionality provides a dynamic and responsive user interface experience.

### Inheritance
Object &#8594; MarkupExtension &#8594; ResponsiveExtension

### Constructors
| Constructor           | Description                                                    |
|-----------------------|----------------------------------------------------------------|
| ResponsiveExtension() | Initializes a new instance of the `ResponsiveExtension` class. |

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
**Platform limitation**: The ability to update property values when the window size changes is only available on targets other than Windows UWP.
Due to a limitation of the UWP API (Windows target only), the `MarkupExtension.ProvideValue(IXamlServiceProvider)` overload is unavailable, which is required to continuously update the value.
Because of this, the markup extension will only provide the initial value, and will not respond to window size changes.

**Initialization**: The `ResponsiveHelper` needs to be hooked up to the window's `SizeChanged` event in order for it to receive updates when the window size changes.
This is typically done in the `OnLaunched` method in the `App` class, where you can get the current window and call the `HookupEvent` method on the `ResponsiveHelper`.
It is important to do this when the app launches, otherwise the `ResponsiveExtension` won't be able to update the property values when the window size changes.

Here is an example of how this might be achieved:

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

**Property type limitation**: Content property values other than strings must be appropriately typed for the markup extension to interpret them correctly.
In the basic usage example below, the values `NarrowRed` and `WideBlue` are properly typed as they refer to the `StaticResource` keys defined in `Page.Resources`.
For instance, using `Background={utu:Responsive Narrow=SkyBlue, Wide=Pink}` would be incorrect, while the string literal usage under `<TextBlock Text="{utu:Responsive Narrow='Narrow format', Wide='Wide format'}" />` is accepted.

## Usage

### Basic example
```xml
xmlns:utu="using:Uno.Toolkit.UI"
...
<Page.Resources>
	<SolidColorBrush x:Key="NarrowRed">Crimson</SolidColorBrush>
	<SolidColorBrush x:Key="WideBlue">Blue</SolidColorBrush>
</Page.Resources>
...
<TextBlock Text="Asd"
		   Background="{utu:Responsive Narrow={StaticResource NarrowRed}, Wide={StaticResource WideBlue}}" />
```

### Custom thresholds
```xml
xmlns:utu="using:Uno.Toolkit.UI"
xmlns:hlp="using:Uno.Toolkit.UI.Helpers"
...
<Page.Resources>
	<hlp:ResponsiveLayout x:Key="CustomLayout"
							Narrowest="200"
							Narrow="350"
							Normal="800"
							Wide="1200"
							Widest="1500" />
</Page.Resources>
...
<TextBlock>
	<TextBlock.Text>
		<utu:ResponsiveExtension Layout="{StaticResource CustomLayout}"
								 Narrowest="This is the narrowest screen size."
								 Narrow="This is a narrow screen size."
								 Normal="This is a normal screen size."
								 Wide="This is a wide screen size."
								 Widest="This is the widest screen size." />
	</TextBlock.Text>
</TextBlock>
```

