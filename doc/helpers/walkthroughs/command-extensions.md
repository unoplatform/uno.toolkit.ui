# Run commands from controls that don't expose `Command`

**Goal:** Invoke an `ICommand` when users interact with controls like `PasswordBox`, `TextBox`, `ListView`, `NavigationView`, `ItemsRepeater`, `ToggleSwitch`, or any `UIElement`.

## Dependencies
- `Uno.Toolkit.UI` (or `Uno.Toolkit.WinUI`)

## Execute on Enter (TextBox / PasswordBox)
```xml
xmlns:utu="using:Uno.Toolkit.UI"
<PasswordBox utu:CommandExtensions.Command="{Binding Login}" />
<!-- Also dismisses soft keyboard on Enter -->
```

## Execute on toggle (ToggleSwitch)
```xml
<ToggleSwitch utu:CommandExtensions.Command="{Binding SetDarkMode}" />
<!-- CommandParameter defaults to ToggleSwitch.IsOn -->
```

## Execute on list item click (ListView)
```xml
<ListView ItemsSource="{Binding Items}"
          IsItemClickEnabled="True"
          utu:CommandExtensions.Command="{Binding UpdateSelection}" />
<!-- Parameter defaults to clicked item -->
```

## Execute on nav invoke (NavigationView)
```xml
<NavigationView utu:CommandExtensions.Command="{Binding Navigate}">
    <NavigationView.MenuItems>
        <NavigationViewItem Content="Home" />
        <NavigationViewItem Content="Settings" />
    </NavigationView.MenuItems>
</NavigationView>
```

## Execute on item tap (ItemsRepeater)
```xml
<muxc:ItemsRepeater ItemsSource="{Binding Items}"
                    utu:CommandExtensions.Command="{Binding UpdateSelection}" />
```

## Execute on any UIElement tapped
```xml
<Grid utu:CommandExtensions.Command="{Binding DoSomething}" />
```

## Custom parameter
```xml
<!-- Override default parameter -->
<TextBox utu:CommandExtensions.Command="{Binding Search}"
         utu:CommandExtensions.CommandParameter="{Binding Text, RelativeSource={RelativeSource Mode=Self}}" />
```
