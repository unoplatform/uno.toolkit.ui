---
uid: Toolkit.Helpers.CommandExtensions
---

# Command Extensions (Concise Reference)

## Summary

Provides Command/CommandParameter attached properties for common scenarios.

## Properties

| Property           | Type       | Description                                                      |
|--------------------|------------|------------------------------------------------------------------|
| `Command`          | `ICommand` | Sets the command to execute when interacting with the control.\* |
| `CommandParameter` | `object`   | Sets the parameter to pass to the Command property.              |

## Usage Examples

```xml
<!-- Include the following XAML namespace to use the samples below -->
xmlns:utu="using:Uno.Toolkit.UI"
...

<!-- Execute command on enter -->
<PasswordBox utu:CommandExtensions.Command="{Binding Login}" />

<!-- Execute command on toggle -->
<ToggleSwitch utu:CommandExtensions.Command="{Binding SetDarkMode}" />

<!-- ListView item click-->
<ListView ItemsSource="123"
          IsItemClickEnabled="True"
          utu:CommandExtensions.Command="{Binding UpdateSelection}" />

<!-- NavigationView item invoke -->
<NavigationView utu:CommandExtensions.Command="{Binding Navigate}">
    <NavigationView.MenuItems>
        <NavigationViewItem Content="Apple" />
        <NavigationViewItem Content="Banana" />
        <NavigationViewItem Content="Cactus" />
    </NavigationView.MenuItems>
</NavigationView>

<muxc:ItemsRepeater ItemsSource="123" utu:CommandExtensions.Command="{Binding UpdateSelection}" />
```

---

**Note**: This is a concise reference. 
For complete documentation, see [command-extensions.md](command-extensions.md)