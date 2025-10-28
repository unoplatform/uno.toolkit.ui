### Customize focus and soft keyboard behavior for input controls with InputExtensions

Summary: InputExtensions provides attached properties to enhance TextBox, PasswordBox, and other input controls with Enter-key behaviors and soft-keyboard return types.

---

### Attached properties

| Property | Type | Description |
|---|---:|---|
| AutoDismiss | bool | Dismisses the soft keyboard when the Enter key is pressed |
| AutoFocusNext | bool | Moves focus to the next focusable element when Enter is pressed |
| AutoFocusNextElement | Control | Explicitly sets the next control to receive focus when Enter is pressed |
| ReturnType | InputReturnType | Sets the soft-keyboard return button type: Default; Done; Go; Next; Search; Send |

### How to use InputExtensions

#### Dismiss the soft keyboard when Enter is pressed

```csharp
<TextBox utu:InputExtensions.AutoDismiss="True" />
```

This causes the soft keyboard to be dismissed on Enter.

#### Automatically move focus to the next focusable element

```csharp
<TextBox x:Name="InputA" utu:InputExtensions.AutoFocusNext="True" />
<TextBox x:Name="InputB" utu:InputExtensions.AutoFocusNext="True" />
```

AutoFocusNext uses the platform focus manager to find the next element.

#### Explicitly set which input control should receive the focus after pressing Enter

```csharp
<TextBox x:Name="Input1" utu:InputExtensions.AutoFocusNext="True" />
<TextBox x:Name="Input2" utu:InputExtensions.AutoFocusNextElement="{Binding ElementName=Input4}" />
<TextBox x:Name="Input3" utu:InputExtensions.AutoFocusNextElement="{Binding ElementName=Input1}" />
<TextBox x:Name="Input4" utu:InputExtensions.AutoFocusNextElement="{Binding ElementName=Input3}" />
```

#### Set the soft-keyboard return button type using InputExtensions.ReturnType

```csharp
<TextBox utu:InputExtensions.ReturnType="Send" />
<TextBox utu:InputExtensions.ReturnType="Search" />
```

ReturnType values influence the soft-keyboard return action and label on Android/iOS.
