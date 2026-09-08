---
uid: Toolkit.GettingStarted.Simple
---
# Uno Simple Toolkit Library

**UnoFeatures:** `Toolkit;Simple` (add to `<UnoFeatures>` in your `.csproj`)

The Uno Simple Toolkit provides a set of resources and styles based on the Uno Simple design system for the controls included in the base [Uno Toolkit library](xref:Toolkit.GettingStarted)

## Getting Started

Initialization of the Simple Toolkit resources is handled by the specialized `SimpleToolkitTheme` ResourceDictionary.

### `SimpleToolkitTheme`

> [!NOTE]
> The `SimpleToolkitTheme` class also handles the required initialization of the Uno Simple resources. Therefore, there is no need to initialize `SimpleTheme` within the `App.xaml`

#### Constructors

| Constructor                                                                               | Description                                                                                                |
|-------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------|
| `SimpleToolkitTheme()`                                                                    | Initializes a new instance of the `SimpleToolkitTheme` resource dictionary.                                |
| `SimpleToolkitTheme(ResourceDictionary? colorOverride, ResourceDictionary? fontOverride)` | Initializes a new instance of the `SimpleToolkitTheme` resource dictionary and applies the given overrides |

#### Properties

`SimpleToolkitTheme` inherits its theme properties from Uno Themes' `SimpleTheme` and `BaseTheme`, including `Colors`, font/color overrides, `DefaultFontFamily`, `DefaultSpacing`, `DefaultDensity`, and `DefaultCornerRadius`. See the [Uno Themes property reference](xref:Uno.Themes.DesignTokens#properties-reference) and [Uno Simple customization guide](xref:Uno.Themes.Simple.GetStarted#customization) for their behavior and usage.

## Installation

> [!NOTE]
> Make sure to setup your environment first by [following our instructions](xref:Uno.GetStarted.vs2022).

### Creating a new project with the Uno Simple Toolkit

#### [**Wizard**](#tab/wizard)

1. Follow the steps in the [Getting Started with Visual Studio](xref:Uno.GettingStarted.CreateAnApp.VS2022#create-the-app) instructions to launch the Uno Platform Template Wizard.
2. Select `Simple` under the `Theme` section.
3. Select `Toolkit` under the `Features` section.

#### [**CLI**](#tab/cli)

1. Install the [`dotnet new` CLI templates](xref:Uno.GetStarted.dotnet-new) with:

    ```bash
    dotnet new install Uno.Templates
    ```

2. Create a new application with:

    ```bash
    dotnet new unoapp -o SimpleToolkitApp -toolkit -theme simpletheme
    ```

---

### Installing Uno Simple Toolkit in an existing project

Depending on the type of project template that the Uno Platform application was created with, follow the instructions below to install the Uno Simple Toolkit.

#### [**Single Project Template**](#tab/singleproj)

1. Edit your project file (`PROJECT_NAME.csproj`) and add `Toolkit` and `SimpleTheme` to the list of `UnoFeatures`:

    ```xml
    <UnoFeatures>Toolkit;SimpleTheme</UnoFeatures>
    ```

2. Initialize `SimpleToolkitTheme` in the `App.xaml`:

    ```xml
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>

                <!-- Code omitted of brevity -->

                <SimpleToolkitTheme xmlns="using:Uno.Toolkit.UI.Simple" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
    ```

#### [**Multi-Head Project Template (Legacy)**](#tab/multihead)

1. In the Solution Explorer panel, right-click on your app's **App Code Library** project (`PROJECT_NAME.csproj`) and select `Manage NuGet Packages...`
2. Install the [`Uno.Toolkit.WinUI.Simple`](https://www.nuget.org/packages/Uno.Toolkit.WinUI.Simple)
3. Add the `SimpleToolkitTheme` to `AppResources.xaml`:

    ```xml
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>

            <SimpleToolkitTheme xmlns="using:Uno.Toolkit.UI.Simple" />

        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
    ```

#### [**Shared Project (.shproj) Template (Legacy)**](#tab/shproj)

1. In the Solution Explorer panel, right-click on your solution name and select `Manage NuGet Packages for Solution ...`. Choose either:
     - The [`Uno.Toolkit.UI.Simple`](https://www.nuget.org/packages/Uno.Toolkit.UI.Simple/) package when targetting Xamarin/UWP
     - The [`Uno.Toolkit.WinUI.Simple`](https://www.nuget.org/packages/Uno.Toolkit.WinUI.Simple) package when targetting net6.0+/WinUI

2. Select the following projects for installation:
    - `PROJECT_NAME.Wasm.csproj`
    - `PROJECT_NAME.Mobile.csproj` (or `PROJECT_NAME.iOS.csproj`, `PROJECT_NAME.Droid.csproj`, and `PROJECT_NAME.macOS.csproj` if you have an existing project)
    - `PROJECT_NAME.Skia.Gtk.csproj`
    - `PROJECT_NAME.Skia.WPF.csproj`
    - `PROJECT_NAME.Windows.csproj` (or `PROJECT_NAME.UWP.csproj` for existing projects)
3. Add the following resources inside `App.xaml`:

    ```xml
    <Application ...>
        <Application.Resources>
            <ResourceDictionary>
                <ResourceDictionary.MergedDictionaries>

                    <!-- Load WinUI resources -->
                    <XamlControlsResources xmlns="using:Microsoft.UI.Xaml.Controls" />

                    <SimpleToolkitTheme xmlns="using:Uno.Toolkit.UI.Simple" />

                    <!-- Load custom application resources -->
                    <!-- ... -->

                </ResourceDictionary.MergedDictionaries>
            </ResourceDictionary>
        </Application.Resources>
    </Application>
    ```

---

## Customization

Apply the properties shown in Uno Themes examples directly to `SimpleToolkitTheme`. It initializes the underlying theme and Toolkit resources, so do not add a separate `SimpleTheme`.

### Customize Colors

Follow the [Uno Simple color override guide](xref:Uno.Themes.Simple.GetStarted#customize-color-palette), using `SimpleToolkitTheme` as the theme dictionary.

### Customize Fonts

See [Uno Themes typography customization](xref:Uno.Themes.DesignTokens#typography-font-swap) for `DefaultFontFamily`, override precedence, and runtime refresh behavior, and the [Uno Simple font guide](xref:Uno.Themes.Simple.GetStarted#customize-fonts) for font resources.

Toolkit control-specific aliases such as `NavigationBarFontFamily` and `DividerSubHeaderFontFamily` retain their own override keys; `DefaultFontFamily` does not regenerate these aliases. See [NavigationBar styling](controls/NavigationBar.md) and [Divider styling](controls/Divider.md).

### Customize Default Density

See [Uno Themes spacing and shape customization](xref:Uno.Themes.DesignTokens#via-scalar-properties) and [density modes](xref:Uno.Themes.DesignTokens#density-modes) for `DefaultSpacing` and `DefaultDensity`. Toolkit padding and margins defined as fixed values do not follow these generated scales.

### Customize Default Corner Radius

See [Uno Themes shape customization](xref:Uno.Themes.DesignTokens#via-scalar-properties) for `DefaultCornerRadius`. Toolkit styles follow it where they consume the generated shape tokens.

### Seed Color Customization

Follow the [Uno Themes seed color guide](xref:Uno.Themes.SeedColors) for color generation and runtime customization, applying its examples to the Toolkit theme.
