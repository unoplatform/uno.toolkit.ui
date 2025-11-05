---
title: UI Command Actions How-Tos in Uno Platform
description: How-to guides for executing MVVM commands on UI interactions without code-behind, optimized for cross-platform apps.
keywords: uno platform, mvvm commands, enter submit, toggle change, item click, selection change, navigation invoke, item tap, element tap, parameter override, trigger events, troubleshooting
---

# UI Command Actions How-Tos

## Submit Text on Enter

Execute command on Enter key press, dismiss keyboard automatically.

**NuGet:** Uno.Toolkit.UI

**XAML Example:**

```xml
xmlns:utu="using:Uno.Toolkit.UI"

<TextBox utu:CommandExtensions.Command="{Binding SubmitText}" />
```

**Notes:**

- Triggers on Enter key press.
- Default parameter: Text value.
- Keyboard dismisses automatically.

## Submit Password on Enter

Execute command on Enter key press, dismiss keyboard automatically.

**NuGet:** Uno.Toolkit.UI

**XAML Example:**

```xml
xmlns:utu="using:Uno.Toolkit.UI"

<PasswordBox utu:CommandExtensions.Command="{Binding SubmitPassword}" />
```

**Notes:**

- Triggers on Enter key press.
- Default parameter: Password value.
- Keyboard dismisses automatically.

## Toggle Feature on Change

Execute command on state change.

**NuGet:** Uno.Toolkit.UI

**XAML Example:**

```xml
xmlns:utu="using:Uno.Toolkit.UI"

<ToggleSwitch utu:CommandExtensions.Command="{Binding ToggleFeature}" />
```

**Notes:**

- Triggers on state change.
- Default parameter: IsOn value (true/false).

## Handle Item Click

Execute command on item click.

**NuGet:** Uno.Toolkit.UI

**XAML Example:**

```xml
xmlns:utu="using:Uno.Toolkit.UI"

<ListView ItemsSource="{Binding Items}"
          IsItemClickEnabled="True"
          utu:CommandExtensions.Command="{Binding HandleItemClick}" />
```

**Notes:**

- Requires IsItemClickEnabled=True.
- Default parameter: Clicked item.

## Change Selection

Execute command on selection change.

**NuGet:** Uno.Toolkit.UI

**XAML Example:**

```xml
xmlns:utu="using:Uno.Toolkit.UI"

<ComboBox ItemsSource="{Binding Categories}"
          utu:CommandExtensions.Command="{Binding ChangeCategory}" />
```

**Notes:**

- Triggers on SelectionChanged.
- Default parameter: Selected item.

## Navigate on Item Select

Execute command on item select.

**NuGet:** Uno.Toolkit.UI

**XAML Example:**

```xml
xmlns:utu="using:Uno.Toolkit.UI"

<NavigationView utu:CommandExtensions.Command="{Binding NavigateToPage}">
    <NavigationView.MenuItems>
        <NavigationViewItem Content="Home" />
    </NavigationView.MenuItems>
</NavigationView>
```

**Notes:**

- Triggers on ItemInvoked.
- Default parameter: Invoked item.

## Handle Item Tap on ItemsRepeater

Execute command on item tap.

**NuGet:** Uno.Toolkit.UI

**XAML Example:**

```xml
xmlns:utu="using:Uno.Toolkit.UI"
xmlns:muxc="using:Microsoft.UI.Xaml.Controls"

<muxc:ItemsRepeater ItemsSource="{Binding Items}"
                    utu:CommandExtensions.Command="{Binding HandleItemTap}">
    <muxc:ItemsRepeater.ItemTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Name}" />
        </DataTemplate>
    </muxc:ItemsRepeater.ItemTemplate>
</muxc:ItemsRepeater>
```

**Notes:**

- Triggers on tap.
- Default parameter: Item DataContext.

## Handle Element Tap

Execute command on element tap.

**NuGet:** Uno.Toolkit.UI

**XAML Example:**

```xml
xmlns:utu="using:Uno.Toolkit.UI"

<Grid utu:CommandExtensions.Command="{Binding HandleTap}" />
```

**Notes:**

- Triggers on tap.
- Default parameter: Element itself.

## Send Static Value with Command

Override default parameter with static value.

**NuGet:** Uno.Toolkit.UI

**XAML Example:**

```xml
xmlns:utu="using:Uno.Toolkit.UI"

<ToggleSwitch utu:CommandExtensions.Command="{Binding ToggleFeature}"
              utu:CommandExtensions.CommandParameter="custom-value" />
```

**Notes:**

- Replaces default parameter.
- Applies to CanExecute and Execute.

## Send Bound Value with Command

Override default parameter with bound value.

**NuGet:** Uno.Toolkit.UI

**XAML Example:**

```xml
xmlns:utu="using:Uno.Toolkit.UI"

<ListView ItemsSource="{Binding Items}"
          IsItemClickEnabled="True"
          utu:CommandExtensions.Command="{Binding HandleItemClick}">
    <ListView.ItemTemplate>
        <DataTemplate>
            <Grid utu:CommandExtensions.CommandParameter="{Binding Id}" />
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```

**Notes:**

- Replaces default parameter.
- Applies to CanExecute and Execute.

## Navigate to Page on Tap

Navigate directly on item tap.

**NuGet:** Uno.Extensions.Navigation

**XAML Example:**

```xml
xmlns:uen="using:Uno.Extensions.Navigation.UI"
xmlns:muxc="using:Microsoft.UI.Xaml.Controls"

<muxc:ItemsRepeater ItemsSource="{Binding Items}"
                    uen:Navigation.Request="DetailsPage"
                    uen:Navigation.Data="{Binding}" />
```

**Notes:**

- Triggers navigation to registered route.
- Passes data parameter.
- Register routes in app bootstrap.
