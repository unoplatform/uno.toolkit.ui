# Move focus / dismiss keyboard / set return key text

**Goal:** Improve text input flow on mobile/desktop.

## Dependencies
- `Uno.Toolkit.UI` (or `Uno.Toolkit.WinUI`)

## Move focus to the next control on Enter
```xml
xmlns:utu="using:Uno.Toolkit.UI"
<TextBox x:Name="Input1" utu:InputExtensions.AutoFocusNext="True" />
<TextBox x:Name="Input2" utu:InputExtensions.AutoFocusNext="True" />
```
- Next target is resolved using focus navigation.

## Move focus to a specific control on Enter
```xml
<TextBox x:Name="Username" />
<PasswordBox x:Name="Password"
             utu:InputExtensions.AutoFocusNextElement="{Binding ElementName=SignIn}" />
<Button x:Name="SignIn" Content="Sign in" />
```
- Explicit target wins over `AutoFocusNext`.

## Dismiss the soft keyboard on Enter
```xml
<TextBox utu:InputExtensions.AutoDismiss="True" />
```

## Pick the soft keyboard return type (Android/iOS)
```xml
<TextBox utu:InputExtensions.ReturnType="Search" />
<!-- Options: Default, Done, Go, Next, Search, Send -->
```
