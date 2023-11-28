---
uid: Toolkit.UI.ResponsiveExtension
---
# ResponsiveExtension

## Summary
The `ResponsiveExtension` class is a markup extension in the Uno Toolkit UI library that allows you to adapt the properties of a UI element based on the current window width.
This is particularly useful for creating responsive designs that look good on a variety of screen sizes.

### C#
```csharp
public partial class ResponsiveExtension : MarkupExtension, IResponsiveCallback
```

### XAML
```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<utu:ResponsiveExtension />
```

### Inheritance 
Object &#8594; MarkupExtension &#8594; ResponsiveExtension

### Constructors
| Constructor           | Description                                                    |
|-----------------------|----------------------------------------------------------------|
| ResponsiveExtension() | Initializes a new instance of the `ResponsiveExtension` class. |

### Properties
| Property   | Type             | Description                                                                                         |
| ---------- | ---------------- | --------------------------------------------------------------------------------------------------- |
| Narrowest  | object           | Value to be used when the screen size is at its narrowest.                                          |
| Narrow     | object           | Value to be used when the screen size is narrow.                                                    |
| Normal     | object           | Value to be used when the screen size is normal.                                                    |
| Wide       | object           | Value to be used when the screen size is wide.                                                      |
| Widest     | object           | Value to be used when the screen size is at its widest.                                             |
| Layout     | ResponsiveLayout | Overrides the MaxWidth for each screen size: `Narrowest`, `Narrow`, `Normal`, `Wide`, and `Widest`. |

### Usage
```xml
<!-- Include the following XAML namespace to use the sample below -->
xmlns:utu="using:Uno.Toolkit.UI"
...
<Page.Resources>
	<SolidColorBrush x:Key="NarrowRed">Crimson</SolidColorBrush>
	<SolidColorBrush x:Key="WideBlue">Blue</SolidColorBrush>
</Page.Resources>
...
<Border Width="30"
		Height="30"
		HorizontalAlignment="Left"
		Background="{utu:Responsive Normal={StaticResource NarrowRed},
									Wide={StaticResource WideBlue}}" />
```

## Remarks
**Platform Limitations**: The ability to update property values when the window size changes is only available on targets other than Windows UWP. On UWP targets, where only the parameterless constructor is available, the property value can only be set initially and cannot be updated if the window size changes. This is a limitation of UWP, and the class logs a warning message when the extension is used on UWP.

**Initialization**: The `ResponsiveHelper` needs to be hooked up to the window’s `SizeChanged` event in order for it to receive updates when the window size changes. This is typically done in the `App.xaml.cs` file, where you can get the current window and call the `HookupEvent` method on the `ResponsiveHelper`. It’s important to do this when the app launches, otherwise the `ResponsiveExtension` won’t be able to update the property values when the window size changes.

Here is an example of how this might be achieved:

```cs
#if IS_WINUI
using XamlWindow = Microsoft.UI.Xaml.Window;
#else
using XamlWindow = Windows.UI.Xaml.Window;
#endif
...
public sealed partial class App : Application
{
	private XamlWindow _window;
...
#if NET5_0_OR_GREATER && WINDOWS && !HAS_UNO
	_window = new XamlWindow();
	_window.Activate();
#else
	_window = XamlWindow.Current;
#endif

	var helper = Uno.Toolkit.UI.Helpers.ResponsiveHelper.GetForCurrentView();
	helper.HookupEvent(_window);
}
```

## ResponsiveLayout
Provides the ability to override the default breakpoints (i.e., the window widths at which the value changes) for the screen sizes. This is done using an instance of the ResponsiveLayout class.

### Properties
| Property   | Type             | Description            |
| ---------- | ---------------- | ---------------------- |
| Narrowest  | double           | Default value is 150.  |
| Narrow     | double           | Default value is 300.  |
| Normal     | double           | Default value is 600.  |
| Wide       | double           | Default value is 800.  |
| Widest     | double           | Default value is 1080. |

### Usage

```xml
<!-- Include the following XAML namespace to use the sample below -->
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

