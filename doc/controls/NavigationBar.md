---
uid: Toolkit.Controls.NavigationBar
---

# NavigationBar

> [!TIP]
> This guide covers details for `NavigationBar` specifically. If you are just getting started with the Uno Material Toolkit Library, please see our [general getting started](../getting-started.md) page to make sure you have the correct setup in place.

## Summary

The `NavigationBar` represents a specialized app bar that provides the layout for `AppBarButton` and navigation logic.

> [!IMPORTANT]
> Starting with Uno Platform 7, `NavigationBar` is rendered from its XAML template on every platform, including iOS and Android. The native mode, which mapped the control to a `UINavigationBar` on iOS and a `Toolbar` on Android, has been removed. If your app relied on it, see [Migrating from the native mode](#migrating-from-the-native-mode).

For a quick introduction to `NavigationBar`, you can check out our introductory video below:

> [!Video https://www.youtube-nocookie.com/embed/4-Q0hy2BnMI]

### C\#

```csharp
public partial class NavigationBar : ContentControl
```

### XAML

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<utu:NavigationBar .../>
```

### Inheritance

`Object` &#8594; `DependencyObject` &#8594; `UIElement` &#8594; `FrameworkElement` &#8594; `Control` &#8594; `ContentControl` &#8594; `NavigationBar`

### Constructors

| Constructor       | Description                                              |
|-------------------|----------------------------------------------------------|
| `NavigationBar()` | Initializes a new instance of the `NavigationBar` class. |

## Template

Under the hood, `NavigationBar` uses a custom-styled `CommandBar`. It is templatable and supports a template that's almost identical to the default WinUI `CommandBar`, except for the addition of a leading `AppBarButton` named `MainCommand`. The same template, visual states, and `AppBarButton` styles are used on every platform.

![NavigationBar - First Page](../assets/navbar-windows-page1.png)

![NavigationBar - Second Page](../assets/navbar-windows-page2.png)

### Usage Example

```xml
<Style TargetType="utu:NavigationBar" BasedOn="{StaticResource NavigationBarStyle}" />
```

`NavigationBarStyle` is provided by the Material and Simple style libraries. Without a design system, use `DefaultNavigationBar`.

## Properties

| Property                     | Comments                                                                                                                            |
|------------------------------|-------------------------------------------------------------------------------------------------------------------------------------|
| `Background`                 |                                                                                                                                     |
| `Content`                    | Please refer to the `Content` section.                                                                                              |
| `Foreground`                 |                                                                                                                                     |
| `Height`                     |                                                                                                                                     |
| `HorizontalAlignment`        |                                                                                                                                     |
| `HorizontalContentAlignment` | `Center` centers the `Content` across the whole bar, while other values place it next to the `MainCommand`. Set `IsDynamicOverflowEnabled="False"` for proper behavior. |
| `IsDynamicOverflowEnabled`   |                                                                                                                                     |
| `MainCommand`                | Please refer to the `MainCommand` section.                                                                                          |
| `MainCommandMode`            | Please refer to the `MainCommandMode` section.                                                                                      |
| `MainCommandStyle`           |                                                                                                                                     |
| `Opacity`                    |                                                                                                                                     |
| `Padding`                    |                                                                                                                                     |
| `PrimaryCommands`            |                                                                                                                                     |
| `SecondaryCommands`          |                                                                                                                                     |
| `Subtitle`                   | Not displayed by the built-in templates.                                                                                            |
| `VerticalAlignment`          |                                                                                                                                     |
| `VerticalContentAlignment`   |                                                                                                                                     |
| `Visibility`                 |                                                                                                                                     |
| `Width`                      |                                                                                                                                     |

### Background

Gets or sets a `Brush` that describes the background of a control.

### Content

Gets or sets the content of a `ContentControl`.

> [!IMPORTANT]
> For simple text titles, always set `Content` directly as a string: `<utu:NavigationBar Content="Title" />`
> Only use `FrameworkElement` content (like `TextBox`, `SearchBox`, etc.) when you need interactive or complex UI in the title area.

#### String Content (Recommended for Simple Titles)

When `Content` is a `string`, it's displayed using the `ContentTemplate` of the style in use, which applies the typography of the design system.

```xml
<utu:NavigationBar Content="Title">
  <utu:NavigationBar.PrimaryCommands>
    <AppBarButton Label="Share">
      <AppBarButton.Icon>
        <BitmapIcon UriSource="ms-appx:///Assets/Share.png" />
      </AppBarButton.Icon>
    </AppBarButton>
  </utu:NavigationBar.PrimaryCommands>
</utu:NavigationBar>
```

When `Content` is a `FrameworkElement`, it's displayed within the available area:

```xml
<utu:NavigationBar>
  <utu:NavigationBar.Content>
    <TextBox />
  </utu:NavigationBar.Content>
  <utu:NavigationBar.PrimaryCommands>
    <AppBarButton Label="Share">
      <AppBarButton.Icon>
        <BitmapIcon UriSource="ms-appx:///Assets/Share.png" />
      </AppBarButton.Icon>
    </AppBarButton>
  </utu:NavigationBar.PrimaryCommands>
</utu:NavigationBar>
```

### Foreground

Gets or sets a `Brush` that describes the foreground color.

```xml
<utu:NavigationBar Content="Title"
                   Foreground="Red">
                   ...
</utu:NavigationBar>
```

Remarks:

* This is typically used to change the text color of the `Content`.
* The `AppBarButton`s take their colors from their own styles. To change them, set `Foreground` on each `AppBarButton`, or use `MainCommandStyle` for the `MainCommand`.

### PrimaryCommands

Gets the collection of primary command elements for the `NavigationBar`.

```xml
<utu:NavigationBar Content="Title">
  <utu:NavigationBar.PrimaryCommands>
    <AppBarButton Label="Search">
      <AppBarButton.Icon>
        <BitmapIcon UriSource="ms-appx:///Assets/Search.png" />
     </AppBarButton.Icon>
   </AppBarButton>
  </utu:NavigationBar.PrimaryCommands>
</utu:NavigationBar>
```

Remarks:

* Accepts the same `ICommandBarElement` items as `CommandBar.PrimaryCommands`.

### SecondaryCommands

Gets the collection of secondary command elements for the `NavigationBar`. They are displayed in the overflow menu of the underlying `CommandBar`.

```xml
<utu:NavigationBar Content="Title">
  <utu:NavigationBar.SecondaryCommands>
    <AppBarButton Label="Item 1" />
    <AppBarButton Label="Item 2" />
    <AppBarButton Label="Item 3" />
  </utu:NavigationBar.SecondaryCommands>
</utu:NavigationBar>
```

### Height

Gets or sets the suggested height of a `FrameworkElement`.

Remarks:

The height is defined by the style in use rather than by the platform or the device. Please refer to the **Lightweight Styling** section for the resources involved.

### MainCommandMode

The `NavigationBar` has a property named `MainCommandMode` that can be set to either:

1. `MainCommandMode.Back` (default)
1. `MainCommandMode.Action`

`MainCommandMode` should be set to `Action` when the `MainCommand` is being used for anything other than backward navigation, such as displaying a burger menu or displaying a prompt to the user before the navigation occurs.

### MainCommand

Gets or sets the `AppBarButton` that the `NavigationBar` will use for displaying a back arrow or custom icon.

Remarks:

Unlike the `PrimaryCommands` or `SecondaryCommands`, which appear to the right of the `NavigationBar`, the `MainCommand` is a special `AppBarButton` that appears to the left of the `NavigationBar`.

Whenever the `NavigationBar` is part of a `Page` whose `Frame` has a non-empty back stack, the back button will be displayed as long as the `NavigationBar` has its `MainCommandMode` set to `Back`. It is also displayed when the `NavigationBar` is hosted in a `Popup`, so that the `Popup` can be closed.

`MainCommand` is typically used for customizing the back button, displaying a different icon, and/or invoking some type of custom action other than back navigation when clicked.

> [!NOTE]
> With the Material and Simple styles, the default back button icon can be customized by overriding the `NavigationBarBackIconData` resource in your resource dictionary:
>
>```xml
><Application.Resources>
>    <x:String x:Key="NavigationBarBackIconData">YOUR_CUSTOM_PATH_DATA</x:String>
></Application.Resources>
>```

### MainCommand Properties

#### Label

Gets or sets the text label of the `MainCommand`.

Remarks:

The `Label` behaves like the `Label` of any other `AppBarButton`. It is not replaced by the title of the previous page. It is highly recommended to set and localize `Label` on all `AppBarButton`s, if only for accessibility.

#### Foreground

Gets or sets the back button foreground for the `MainCommand`.

```xml
<utu:NavigationBar Content="Title">
  <utu:NavigationBar.MainCommand>
    <AppBarButton Foreground="Red" />
  </utu:NavigationBar.MainCommand>
</utu:NavigationBar>
```

OR

```xml
<Style x:Key="MyCustomAppBarButtonStyle" TargetType="AppBarButton">
  <Setter Property="Foreground"
            Value="Red" />
</Style>

  <Style TargetType="utu:NavigationBar">
      <Setter Property="MainCommandStyle"
              Value="{StaticResource MyCustomAppBarButtonStyle}" />
  </Style>
```

#### Icon

Gets or sets the back button icon for the `MainCommand`.

```xml
<utu:NavigationBar Content="Title">
  <utu:NavigationBar.MainCommand>
    <AppBarButton>
      <AppBarButton.Icon>
        <BitmapIcon UriSource="ms-appx:///Assets/Close.png" />
      </AppBarButton.Icon>
    </AppBarButton>
  </utu:NavigationBar.MainCommand>
</utu:NavigationBar>
```

Remarks:

Any `IconElement` can be used.

## Lightweight Styling

| Key                                                                    | Type              | Value                                   |
|------------------------------------------------------------------------|-------------------|-----------------------------------------|
| `NavigationBarCommandBarEllipsisIconForegroundDisabled`                | `SolidColorBrush` | TextFillColorDisabledBrush              |
| `NavigationBarCommandBarBackgroundCompactOpenUp`                       | `SolidColorBrush` | SurfaceBrush                            |
| `NavigationBarCommandBarBackgroundCompactOpenDown`                     | `SolidColorBrush` | SurfaceBrush                            |
| `NavigationBarMainCommandForeground`                                   | `SolidColorBrush` | OnSurfaceBrush                          |
| `NavigationBarForeground`                                              | `SolidColorBrush` | OnSurfaceBrush                          |
| `NavigationBarBackground`                                              | `SolidColorBrush` | SurfaceBrush                            |
| `NavigationBarPadding`                                                 | `Thickness`       | 4,0,0,0                                 |
| `NavigationBarFontFamily`                                              | `FontFamily`      | TitleLargeFontFamily                    |
| `NavigationBarFontWeight`                                              | `String`          | TitleLargeFontWeight                    |
| `NavigationBarFontSize`                                                | `Double`          | TitleLargeFontSize                      |
| `NavigationBarBackIconData`                                            | `String`          | NavigationBarBackIconData               |
| `MaterialModalNavigationBarMainCommandForeground`                      | `SolidColorBrush` | OnSurfaceBrush                          |
| `MaterialModalNavigationBarForeground`                                 | `SolidColorBrush` | OnSurfaceBrush                          |
| `MaterialModalNavigationBarBackground`                                 | `SolidColorBrush` | SurfaceBrush                            |
| `MaterialPrimaryNavigationBarCommandBarEllipsisIconForegroundDisabled` | `SolidColorBrush` | TextFillColorDisabledBrush              |
| `MaterialPrimaryNavigationBarCommandBarBackgroundCompactOpenUp`        | `SolidColorBrush` | PrimaryBrush                            |
| `MaterialPrimaryNavigationBarCommandBarBackgroundCompactOpenDown`      | `SolidColorBrush` | PrimaryBrush                            |
| `MaterialPrimaryNavigationBarMainCommandForeground`                    | `SolidColorBrush` | OnPrimaryBrush                          |
| `MaterialPrimaryNavigationBarForeground`                               | `SolidColorBrush` | OnPrimaryBrush                          |
| `MaterialPrimaryNavigationBarBackground`                               | `SolidColorBrush` | PrimaryBrush                            |
| `MaterialPrimaryAppBarButtonForeground`                                | `SolidColorBrush` | OnPrimaryBrush                          |
| `MaterialPrimaryModalNavigationBarMainCommandForeground`               | `SolidColorBrush` | OnPrimaryBrush                          |
| `MaterialPrimaryModalNavigationBarForeground`                          | `SolidColorBrush` | OnPrimaryBrush                          |
| `MaterialPrimaryModalNavigationBarBackground`                          | `SolidColorBrush` | PrimaryBrush                            |
| `NavigationBarOverflowAppBarButtonForeground`                          | `SolidColorBrush` | OnPrimaryBrush                          |
| `NavigationBarOverflowAppBarButtonBackground`                          | `SolidColorBrush` | SolidColorBrush { Color = Transparent } |
| `NavigationBarEllipsisButtonForeground`                                | `SolidColorBrush` | OnSurfaceBrush                          |
| `NavigationBarEllipsisButtonBackground`                                | `SolidColorBrush` | SolidColorBrush { Color = Transparent } |
| `MaterialXamlNavigationBarHeight`                                      | `Double`          | 64                                      |
| `MaterialNavigationBarHeight`                                          | `Double`          | 48                                      |
| `MaterialNavigationBarContentMargin`                                   | `Thickness`       | 16,0,0,0                                |
| `MaterialAppBarEllipsisButtonInnerBorderMargin`                        | `Thickness`       | 2,6,6,6                                 |
| `NavigationBarMaterialEllipsisButtonFontFamily`                        | `FontFamily`      | MaterialRegularFontFamily               |
| `NavigationBarMaterialEllipsisButtonFontWeight`                        | `FontWeight`      | SemiBold                                |
| `NavigationBarMaterialEllipsisButtonFontSize`                          | `Double`          | ControlContentThemeFontSize             |
| `NavigationBarMaterialEllipsisButtonWidth`                             | `Double`          | AppBarExpandButtonThemeWidth            |
| `NavBarAppBarButtonContentHeight`                                      | `Double`          | 24                                      |
| `NavBarMainCommandAppBarButtonContentHeight`                           | `Double`          | 16                                      |
| `NavBarAppBarThemeCompactHeight`                                       | `Double`          | 56                                      |
| `NavBarAppBarButtonPadding`                                            | `Thickness`       | 12,16                                   |
| `NavBarAppBarButtonHasFlyoutChevronVisibility`                         | `Visibility`      | Collapsed                               |

`MaterialNavigationBarHeight` is only defined by the Material v1 styles.

## Navigation

The `NavigationBar` automatically hooks itself up to the [`SystemNavigationManager.BackRequested` event](https://learn.microsoft.com/uwp/api/windows.ui.core.systemnavigationmanager.backrequested) and will attempt to navigate back by calling `Frame.GoBack()` as long as all of the following conditions are met:

* `MainCommandMode` for the `NavigationBar` is set to `MainCommandMode.Back`
* The `Frame` that contains the `NavigationBar` is currently visible
* The current `Page` of the `Frame` is equal to the parent `Page` of the `NavigationBar`

NOTE: `SystemNavigationManager` is not supported for WinAppSDK

The `NavigationBar` is also aware of its parent `Page` possibly being hosted in a `Popup` (for things like modal pages) and will close the `Popup` when attempting to navigate backward within the `Popup` while `Frame.BackStack` is empty.

## Status Bar and Safe Area

On iOS and Android, the app draws behind the status bar, so a `NavigationBar` placed at the top of a page needs the unsafe area to be reserved:

* The default style (`DefaultNavigationBar`) sets `utu:SafeArea.Insets="Top"` on the `NavigationBar` itself.
* The Material and Simple styles do not, and behave the same on every platform. Apply [`SafeArea`](xref:Toolkit.Controls.SafeArea) to the page instead, for example on its root panel:

```xml
<Grid utu:SafeArea.Insets="VisibleBounds">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
    </Grid.RowDefinitions>

    <utu:NavigationBar Content="Title" />
</Grid>
```

## Migrating from the native mode

Before Uno Platform 7, `NavigationBar` defaulted to a native mode on iOS and Android. That mode no longer exists, and the XAML template described above is used everywhere:

* `NativeFramePresenter` and `NativeNavigationBarPresenter` have been removed. `Frame` no longer needs a custom style: remove any `Style` based on `NativeDefaultToolkitFrame`.
* The `NativeDefaultToolkitFrame`, `NativeNavigationBarTemplate`, `MaterialNativeNavigationBarTemplate`, and `SimpleNativeNavigationBarTemplate` resources have been removed, along with `MaterialNavigationBarElevation`, the Material v2 `MaterialNavigationBarHeight`, and the Simple `NavigationBarElevation` and `NavigationBarHeight` resources.
* The bar is part of the `Page`, so there is no shared `UINavigationBar` or navigation transition across pages. It can be placed anywhere, including inside a `ScrollViewer`, and a page can contain several `NavigationBar`s.
* The back button no longer shows the title of the previous page on iOS, and `SecondaryCommands` use the `CommandBar` overflow menu instead of the Android `Toolbar` menu.
* The `AppBarButton` templates and visual states apply. Platform resources such as the Android `actionMenuTextColor` or `colorControlHighlight` styles no longer affect the bar: use the **Lightweight Styling** resources instead.
* The Material and Simple styles no longer pad the bar for the status bar on iOS and Android. Please refer to the **Status Bar and Safe Area** section.

## FAQ: NavigationBar

### How can I change the back button icon/arrow/chevron in my app?

  ```xml
  xmlns:utu="using:Uno.Toolkit.UI"
  ...
  <utu:NavigationBar Content="Page Title">
    <utu:NavigationBar.MainCommand>
      <AppBarButton>
        <AppBarButton.Icon>
          <BitmapIcon UriSource="ms-appx:///Assets/Back.png" />
        </AppBarButton.Icon>
      </AppBarButton>
    </utu:NavigationBar.MainCommand>
  </utu:NavigationBar>
  ```

### How can I change the color of the back button?

  ```xml
  xmlns:utu="using:Uno.Toolkit.UI"
  ...
  <utu:NavigationBar Content="Page Title">
    <utu:NavigationBar.MainCommand>
      <AppBarButton Foreground="Red"
                    ShowAsMonochrome="False" />
    </utu:NavigationBar.MainCommand>
  </utu:NavigationBar>
  ```

### Why doesn't my NavigationBar show a back button?

The back button is displayed when `MainCommandMode` is `Back` and the `Frame` of the `Page` containing the `NavigationBar` can go back, or when the `NavigationBar` is hosted in a `Popup`. The `NavigationBar` finds its `Page` when it is loaded, so make sure it's part of the page's visual tree.

### How can I add a badge to an AppBarButton?

You can implement your own badge by setting a custom content on `AppBarButton`:

```xml
<AppBarButton>
    <AppBarButton.Content>
        <Grid Height="48"
            Width="48">
            <Image Source="ms-appx:///Assets/Icons/cart.png"
                    VerticalAlignment="Center"
                    HorizontalAlignment="Center" />
            <Border x:Name="Badge"
                    VerticalAlignment="Top"
                    HorizontalAlignment="Right"
                    Background="Red"
                    Margin="8,4"
                    Padding="4,0"
                    MinWidth="16"
                    Height="16"
                    CornerRadius="8">
                <TextBlock x:Name="Count"
                            HorizontalAlignment="Center"
                            VerticalAlignment="Center"
                            Foreground="White"
                            FontSize="8"
                            Text="0" />
            </Border>
        </Grid>
    </AppBarButton.Content>
</AppBarButton>
```

### How can I set custom content to an AppBarButton?

You can set a custom content to an `AppBarButton` like this:

```xml
<AppBarButton>
    <AppBarButton.Content>
        <!-- Custom content goes here -->
        <TextBlock Text="asd" />
    </AppBarButton.Content>
</AppBarButton>
```

### How can I change the height of my NavigationBar?

The height is defined by the style in use. Override the related resources listed in the **Lightweight Styling** section, or provide your own style.

### How can I add a burger menu to the left of my NavigationBar?

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...
<utu:NavigationBar MainCommandMode="Action">
    <utu:NavigationBar.MainCommand>
    <AppBarButton Command="{Binding ToggleMenu}">
        <AppBarButton.Icon>
            <BitmapIcon UriSource="ms-appx:///Assets/Icons/menu.png" />
        </AppBarButton.Icon>
    </AppBarButton>
    </utu:NavigationBar.MainCommand>
</utu:NavigationBar>
```

### How can I customize the font of the NavigationBar title/content?

To customize the font of the `NavigationBar`'s title, you can set a custom `FrameworkElement` as the `Content` of your `NavigationBar`:

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...
<utu:NavigationBar>
    <utu:NavigationBar.Content>
        <TextBlock Text="Title"
                    FontFamily="{StaticResource CustomFontFamily}" />
    </utu:NavigationBar.Content>
</utu:NavigationBar>
```

### How can I show an image under my NavigationBar?

You can show an image under a `NavigationBar` by making its background transparent and superposing it over an `Image`:

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...
<Grid>
    <Image Source="http://www.example.com/image.png">
    <utu:NavigationBar Background="Transparent"
                       VerticalAlignment="Top" />
</Grid>
```
