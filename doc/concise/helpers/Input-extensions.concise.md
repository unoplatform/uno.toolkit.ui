---
uid: Toolkit.Helpers.InputExtensions
---

# Input Extensions (Concise Reference)

## Summary

Provides various attached properties for _input controls_, such as `TextBox` and `PasswordBox`.

## Properties

| Property               | Type         | Description                                                                                                                                       |
|------------------------|--------------|---------------------------------------------------------------------------------------------------------------------------------------------------|
| `AutoDismiss`          | `bool`       | Whether the soft keyboard will be dismissed when the enter key is pressed.                                                                        |
| `AutoFocusNext`        | `bool`       | Whether the focus will move to the next focusable element when the enter key is pressed.\*                                                        |
| `AutoFocusNextElement` | `Control`    | Sets the next control to focus when the enter key is pressed.\*                                                                                   |
| `ReturnType`           | `InputReturnType` | The type of return button on a soft keyboard for Android/iOS. It can be one of the following options: __Default, Done, Go, Next, Search, Send__.  |

## Usage Examples

```xml
<!-- Include the following XAML namespace to use the samples below -->
xmlns:utu="using:Uno.Toolkit.UI"
...

<!-- The focus will move in this order when pressing enter repeatedly: 1-2-4-3 -->
<TextBox x:Name="Input1" utu:InputExtensions.AutoFocusNext="True" />
<TextBox x:Name="Input2" utu:InputExtensions.AutoFocusNextElement="{Binding ElementName=Input4}" />
<TextBox x:Name="Input3" utu:InputExtensions.AutoFocusNextElement="{Binding ElementName=Input1}" />
<TextBox x:Name="Input4" utu:InputExtensions.AutoFocusNextElement="{Binding ElementName=Input3}" />

<!-- Dismiss soft keyboard on enter -->
<TextBox utu:InputExtensions.AutoDismiss="True" />

<!-- Soft keyboard with send as return button -->
<TextBox utu:InputExtensions.ReturnType="Send" />
```

---

**Note**: This is a concise reference. 
For complete documentation, see [Input-extensions.md](Input-extensions.md)