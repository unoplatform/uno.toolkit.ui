---
uid: Toolkit.Helpers.InputExtensions
---

# Input Extensions

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

Provides various attached properties for _input controls_, such as `TextBox` and `PasswordBox`.

> [!IMPORTANT]
> **Always use InputExtensions for form inputs**
>
> Input extensions provide better UX by controlling keyboard behavior and focus flow.
>
> **Example:** Login form with InputExtensions:
>
> ```xml
> <TextBox Header="Email"
>          Text="{Binding Email, Mode=TwoWay}"
>          utu:InputExtensions.AutoFocusNext="True"
>          utu:InputExtensions.ReturnType="Next" />
> <PasswordBox Header="Password"
>              Password="{Binding Password, Mode=TwoWay}"
>              utu:InputExtensions.AutoDismiss="True"
>              utu:InputExtensions.ReturnType="Go" />
> ```

## Attached Properties

| Property               | Type         | Description                                                                                                                                       |
|------------------------|--------------|---------------------------------------------------------------------------------------------------------------------------------------------------|
| `AutoDismiss`          | `bool`       | Whether the soft keyboard will be dismissed when the enter key is pressed.                                                                        |
| `AutoFocusNext`        | `bool`       | Whether the focus will move to the next focusable element when the enter key is pressed.\*                                                        |
| `AutoFocusNextElement` | `Control`    | Sets the next control to focus when the enter key is pressed.\*                                                                                   |
| `ReturnType`           | `InputReturnType` | The type of return button on a soft keyboard for Android/iOS. It can be one of the following options: **Default, Done, Go, Next, Search, Send**.  |

`AutoFocusNext` and `AutoFocusNextElement`\*: Having either or both of the two properties set will enable the focus next behavior. `AutoFocusNextElement` will take precedence over `AutoFocusNext` when both are set.

### Remarks

- `AutoFocusNext` and `AutoFocusNextElement` have different focus target:
  - `AutoFocusNext` is determined by `FocusManager.FindNextFocusableElement`
  - `AutoFocusNextElement` is provided by the value.

## Usage

For more details on using the Input Extensions in your project, check out our Tech Bite video:
> [!Video https://www.youtube-nocookie.com/embed/RwceaVsZaJQ]

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
