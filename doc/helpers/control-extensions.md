---
uid: Toolkit.Helpers.ContorlExtensions
---
# CommandExtensions Attached Properties
Provides Command/CommandParameter attached properties for common scenarios.

## Properties
Property|Type|Description
-|-|-
Command|ICommand|Sets the command to execute when `TextBox`/`PasswordBox` enter key is pressed, `ListViewBase.ItemClick`, `NavigationView.ItemInvoked`, and `ItemsRepeater` item tapped.
CommandParameter|ICommand|Sets the parameter to pass to the Command property.

Command on `TextBox`/`PasswordBox`\*: Having this set will also cause the keyboard dismiss on enter.
Command on `ListView`\*: [`IsItemClickEnabled`](https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.controls.listviewbase.isitemclickenabled) must also be set to true for this to work.

### Remarks
- For Command, the relevant parameter is also provided for the `CanExecute` and `Execute` call:
  > Unless CommandParameter is set, which replaces the following.
  - `TextBox.Text`
  - `PasswordBox.Password`
  - `ItemClickEventArgs.ClickedItem` from `ListView.ItemClick`
  - `NavigationViewItemInvokedEventArgs.InvokedItem` from `NavigationView.ItemInvoked`
  - `ItemsRepeater`'s item root's DataContext

## Usage
```xml
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
