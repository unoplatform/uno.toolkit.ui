---
uid: Toolkit.Helpers.InputExtensions
---
# InputExtensions Attached Properties
Provides various attached properties for _input controls_, such as `TextBox` and `PasswordBox`.

## Properties
Property|Type|Description
-|-|-
AutoDismiss|bool|Whether the soft-keyboard will be dismissed when the enter key is pressed.
AutoFocusNext|bool|Whether the focus will move to the next focusable element when the enter key is pressed.\*
AutoFocusNextElement|Control|Sets the next control to focus when the enter key is pressed.\*

AutoFocusNext and AutoFocusNextElement\*: Having either or both of the two properties set will enable the focus next behavior. AutoFocusNextElement will take precedences over AutoFocusNext when both are set.

### Remarks
- AutoFocusNext and AutoFocusNextElement have different focus target:
  - AutoFocusNext is determined by `FocusManager.FindNextFocusableElement`
  - AutoFocusNextElement is provided by the value.

## Usage
```xml
<!-- The focus will move in this order when pressing enter repeatedly: 1-2-4-3 -->
<TextBox x:Name="Input1" utu:InputExtensions.AutoFocusNext="True" />
<TextBox x:Name="Input2" utu:InputExtensions.AutoFocusNextElement="{Binding ElementName=Input4}" />
<TextBox x:Name="Input3" utu:InputExtensions.AutoFocusNextElement="{Binding ElementName=Input1}" />
<TextBox x:Name="Input4" utu:InputExtensions.AutoFocusNextElement="{Binding ElementName=Input3}" />

<!-- Dismiss soft-keyboard on enter -->
<TextBox utu:InputExtensions.AutoDismiss="True" />
```
