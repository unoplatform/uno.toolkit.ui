---
uid: Toolkit.Helpers.CommandExtensions
---

# Command Extensions

Provides Command/CommandParameter attached properties for common scenarios.

## Attached Properties

| Property           | Type       | Description                                                     |
|--------------------|------------|-----------------------------------------------------------------|
| `Command`          | `ICommand` | Sets the command to execute when interacting with the control.\* |
| `CommandParameter` | `object`   | Sets the parameter to pass to the Command property.             |

### Remarks

- `Command` is executed on:
  - `ListViewBase.ItemClick`
  - `NavigationView.ItemInvoked`
  - `ItemsRepeater` item tapped
  - `TextBox` and `PasswordBox` when the Enter key is pressed
  - any `UIElement` tapped
- For Command, the relevant parameter is also provided for the `CanExecute` and `Execute` call:
  > Unless CommandParameter is set, which replaces the following.
  - `TextBox.Text`
  - `PasswordBox.Password`
  - `ItemClickEventArgs.ClickedItem` from `ListView.ItemClick`
  - `NavigationViewItemInvokedEventArgs.InvokedItem` from `NavigationView.ItemInvoked`
  - `ItemsRepeater`'s item root's DataContext
- Command on `TextBox`/`PasswordBox`\*: Having this set will also cause the keyboard to dismiss on enter.
- Command on `ListView`\*: [`IsItemClickEnabled`](https://learn.microsoft.com/uwp/api/windows.ui.xaml.controls.listviewbase.isitemclickenabled) must also be set to true for this to work.

## Usage

```xml
<!-- Include the following XAML namespace to use the samples below -->
xmlns:utu="using:Uno.Toolkit.UI"
...

<!-- Execute command on enter -->
<PasswordBox utu:CommandExtensions.Command="{Binding Login}" />

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
