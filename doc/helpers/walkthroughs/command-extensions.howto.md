# Command actions on UI interactions

These how-tos show how to run a view-model `ICommand` when common controls are used, without writing code-behind. All samples use the attached properties from **`Uno.Toolkit.UI`**:

```xml
xmlns:utu="using:Uno.Toolkit.UI"
```

NuGet needed for these samples: **`Uno.Toolkit.UI`**.

---

## 1. Run a command when user presses Enter in a text box

**Outcome:** submit the text (or password) on Enter, and close the keyboard.

**XAML**

```xml
<Page
    ...
    xmlns:utu="using:Uno.Toolkit.UI">
    <TextBox
        PlaceholderText="Email"
        utu:CommandExtensions.Command="{Binding SubmitEmail}" />

    <PasswordBox
        utu:CommandExtensions.Command="{Binding Login}" />
</Page>
```

**Notes**

* Command is executed on **Enter**. ([Uno Platform][1])
* If you need to send a custom value (not the text), add `utu:CommandExtensions.CommandParameter`.
* For text boxes, the default parameter is the current **text**. For password boxes, it’s the **password**. ([Uno Platform][1])
* Keyboard is dismissed automatically when the command runs. ([Uno Platform][1])

---

## 2. Run a command when a toggle switch changes

**Outcome:** react to On/Off without handling `Toggled`.

**XAML**

```xml
<ToggleSwitch
    Header="Dark mode"
    utu:CommandExtensions.Command="{Binding SetDarkMode}" />
```

**What the command receives**

* By default, the command parameter is the toggle state: `true` / `false`. ([Uno Platform][1])
* To send your own data, add:

  ```xml
  utu:CommandExtensions.CommandParameter="dark-mode"
  ```

---

## 3. Run a command when a list item is clicked

**Outcome:** get the clicked item in the command.

**XAML**

```xml
<ListView
    ItemsSource="{Binding Items}"
    IsItemClickEnabled="True"
    utu:CommandExtensions.Command="{Binding ItemClicked}" />
```

**Important**

* `IsItemClickEnabled="True"` is **required** for `ListView`. ([Uno Platform][1])
* By default, the command receives the **clicked item** (`ItemClickEventArgs.ClickedItem`). ([Uno Platform][1])
* To override the parameter, set `utu:CommandExtensions.CommandParameter` **on the item container or on the root of the item template**. That value replaces the clicked item. ([Uno Platform][1])

**Item template override example**

```xml
<ListView
    ItemsSource="{Binding Items}"
    IsItemClickEnabled="True"
    utu:CommandExtensions.Command="{Binding ItemClicked}">
    <ListView.ItemTemplate>
        <DataTemplate>
            <Grid utu:CommandExtensions.CommandParameter="{Binding Id}">
                <TextBlock Text="{Binding Name}" />
            </Grid>
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```

---

## 4. Run a command when selection changes (Selector controls)

**Outcome:** react to user choosing another item in controls like `ComboBox` / `ListViewBase` / other `Selector`s.

**XAML (generic)**

```xml
<ComboBox
    ItemsSource="{Binding Categories}"
    utu:CommandExtensions.Command="{Binding CategoryChanged}" />
```

**Behavior**

* Command runs on **SelectionChanged** (for selector-like controls).
* Parameter is the **selected item** by default. ([Uno Platform][1])
* `ListView` is the special case: it prefers **ItemClick** over selection (see previous how-to). ([Uno Platform][1])

---

## 5. Run a command when a NavigationView item is invoked

**Outcome:** navigate without handling `ItemInvoked` in code-behind.

**XAML**

```xml
<NavigationView
    utu:CommandExtensions.Command="{Binding Navigate}">
    <NavigationView.MenuItems>
        <NavigationViewItem Content="Home" Tag="home" />
        <NavigationViewItem Content="Settings" Tag="settings" />
    </NavigationView.MenuItems>
</NavigationView>
```

**What the command gets**

* By default, the parameter is the **invoked item** (`NavigationViewItemInvokedEventArgs.InvokedItem`). ([Uno Platform][1])
* If you want to pass something cleaner (like `Tag`), set `utu:CommandExtensions.CommandParameter` on the item:

  ```xml
  <NavigationViewItem
      Content="Settings"
      Tag="settings"
      utu:CommandExtensions.CommandParameter="settings" />
  ```

---

## 6. Run a command for an ItemsRepeater item

**Outcome:** handle taps on a repeated item without writing handlers.

**XAML**

```xml
<muxc:ItemsRepeater
    ItemsSource="{Binding Items}"
    utu:CommandExtensions.Command="{Binding ItemTapped}">
    <muxc:ItemsRepeater.ItemTemplate>
        <DataTemplate>
            <Grid Padding="8">
                <TextBlock Text="{Binding Name}" />
            </Grid>
        </DataTemplate>
    </muxc:ItemsRepeater.ItemTemplate>
</muxc:ItemsRepeater>
```

**Behavior**

* Command runs when an item is **tapped**.
* Parameter is the **item’s DataContext** (the bound model). ([Uno Platform][1])
* Put `utu:CommandExtensions.CommandParameter="..."` on the root of the template to override. ([Uno Platform][1])

---

## 7. Run a command when any UIElement is tapped

**Outcome:** make any visual element “commandable” with no `Click` event.

**XAML**

```xml
<Grid
    Background="Transparent"
    utu:CommandExtensions.Command="{Binding ShowDetails}"
    utu:CommandExtensions.CommandParameter="{Binding}">
    <TextBlock Text="Tap for details" />
</Grid>
```

**Notes**

* Works on **any `UIElement`**. The default parameter is the element itself. ([Uno Platform][1])
* Use this to turn cards, rows, or icons into tappable command sources.

---

## 8. Override the command parameter

**Outcome:** always send the value you want, regardless of the control’s default.

**XAML examples**

1. **Send static value**

   ```xml
   <ToggleSwitch
       utu:CommandExtensions.Command="{Binding TrackToggle}"
       utu:CommandExtensions.CommandParameter="dark-mode" />
   ```

2. **Send bound object from item template**

   ```xml
   <ListView ...>
       <ListView.ItemTemplate>
           <DataTemplate>
               <StackPanel
                   utu:CommandExtensions.CommandParameter="{Binding Id}">
                   <TextBlock Text="{Binding Name}" />
               </StackPanel>
           </DataTemplate>
       </ListView.ItemTemplate>
   </ListView>
   ```

**Why override?**

* Some controls send event args (clicked item, selected item, text, etc.).
* The extension lets you replace that with **your own** value (an Id, a DTO, a route).
* This value is used for **both** `CanExecute` and `Execute`. ([Uno Platform][1])

---

## 9. What events trigger the command?

The extension hooks into these cases:

* `ListViewBase.ItemClick`
* `Selector.SelectionChanged` (except `ListView`, which uses item click)
* `NavigationView.ItemInvoked`
* `ItemsRepeater` item tapped
* `TextBox`/`PasswordBox` on **Enter**
* `ToggleSwitch` toggled
* **any** `UIElement` tapped
  All of this is built-in; you just set the attached property. ([Uno Platform][1])

---

## 10. Troubleshooting

**Command not firing on ListView**

* Check `IsItemClickEnabled="True"`. This is required. ([Uno Platform][1])

**View-model command doesn’t run**

* Make sure the page’s `DataContext` is set.
* Make sure the binding mode is correct (`{Binding MyCommand}` exists).

**`CanExecute` is not using the value I expect**

* If you didn't set `CommandParameter`, the extension uses the **control's relevant value** (clicked item, selected item, text, etc.) for both `CanExecute` and `Execute`. Set the parameter explicitly to control this. ([Uno Platform][1])

---

[1]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/helpers/command-extensions.html "Command Extensions "
