---
title: UI Command Actions on UI interactions How-Tos in Uno Platform
description: How-to guides for executing MVVM commands on UI interactions without code-behind, optimized for cross-platform apps.
keywords: uno platform, mvvm commands, enter submit, toggle change, item click, selection change, navigation invoke, item tap, element tap, trigger events
---

## Handle Text on Enter key press

Execute command on Enter key press, dismiss keyboard automatically.

UnoFeatures: **Toolkit**

**XAML Example:**

```xml
xmlns:utu="using:Uno.Toolkit.UI"

<TextBox utu:CommandExtensions.Command="{Binding SubmitText}" />
```

**Notes:**

- Triggers on Enter key press.
- Default parameter: Text value.
- Keyboard dismisses automatically.

## Handle PasswordBox on Enter key press

Execute command on Enter key press, dismiss keyboard automatically.

UnoFeatures: **Toolkit**

**XAML Example:**

```xml
xmlns:utu="using:Uno.Toolkit.UI"

<PasswordBox utu:CommandExtensions.Command="{Binding SubmitPassword}" />
```

**Notes:**

- Triggers on Enter key press.
- Default parameter: Password value.
- Keyboard dismisses automatically.

## Handle Feature on Change

Execute command on state change.

UnoFeatures: **Toolkit**

**XAML Example:**

```xml
xmlns:utu="using:Uno.Toolkit.UI"

<ToggleSwitch utu:CommandExtensions.Command="{Binding ToggleFeature}" />
```

**Notes:**

- Triggers on state change.
- Default parameter: IsOn value (true/false).

## Handle List Item Click

Execute command on a `ListView` item click

UnoFeatures: **Toolkit**

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

## Handle ComboBox Selection

Execute command on selection change.

UnoFeatures: **Toolkit**

**XAML Example:**

```xml
xmlns:utu="using:Uno.Toolkit.UI"

<ComboBox ItemsSource="{Binding Categories}"
          utu:CommandExtensions.Command="{Binding ChangeCategory}" />
```

**Notes:**

- Triggers on SelectionChanged.
- Default parameter: Selected item.

## Handle NavigationView Item Selection

Execute command on item select.

UnoFeatures: **Toolkit**

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

## Handle ItemsRepeater Item Tap

Execute command on item tap.

UnoFeatures: **Toolkit**

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

UnoFeatures: **Toolkit**

**XAML Example:**

```xml
xmlns:utu="using:Uno.Toolkit.UI"

<Grid utu:CommandExtensions.Command="{Binding HandleTap}" />
```

**Notes:**

- Triggers on tap.
- Default parameter: Element itself.

## Send Static Value as Custom CommandParameter

Override default parameter with static value.

UnoFeatures: **Toolkit**

**XAML Example:**

```xml
xmlns:utu="using:Uno.Toolkit.UI"

<ToggleSwitch utu:CommandExtensions.Command="{Binding ToggleFeature}"
              utu:CommandExtensions.CommandParameter="custom-value" />
```

**Notes:**

- Replaces default parameter.
- Applies to CanExecute and Execute.

## Send Bound Value as Custom CommandParameter

Override default parameter with bound value.

UnoFeatures: **Toolkit**

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

UnoFeatures: **Toolkit**

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
