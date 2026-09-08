---
uid: Toolkit.GettingStarted.Material
---
# Uno Material Toolkit Library

**UnoFeatures:** `Toolkit;Material` (add to `<UnoFeatures>` in your `.csproj`)

<p align="center">
  <img src="assets/material-toolkit-design-system.png" alt="Material Toolkit Design System">
</p>

The Uno Material Toolkit provides a set of resources and styles based on [Material Design guidelines](https://m3.material.io/) for the controls included in the base [Uno Toolkit library](xref:Toolkit.GettingStarted)

## Getting Started

Initialization of the Material Toolkit resources is handled by the specialized `MaterialToolkitTheme` ResourceDictionary.

### `MaterialToolkitTheme`

> [!NOTE]
> The `MaterialToolkitTheme` class also handles the required initialization of the Uno Material resources. Therefore, there is no need to initialize `MaterialTheme` within the `App.xaml`

#### Constructors

| Constructor                                                                                 | Description                                                                                                  |
|---------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------|
| `MaterialToolkitTheme()`                                                                    | Initializes a new instance of the `MaterialToolkitTheme` resource dictionary.                                |
| `MaterialToolkitTheme(ResourceDictionary? colorOverride, ResourceDictionary? fontOverride)` | Initializes a new instance of the `MaterialToolkitTheme` resource dictionary and applies the given overrides |

#### Properties

`MaterialToolkitTheme` inherits its theme properties from Uno Themes' `MaterialTheme` and `BaseTheme`, including `Colors`, font/color overrides, `DefaultFontFamily`, `DefaultSpacing`, `DefaultDensity`, and `DefaultCornerRadius`. See the [Uno Themes property reference](xref:Uno.Themes.DesignTokens#properties-reference) and [Uno Material customization guide](xref:Uno.Themes.Material.GetStarted#customization) for their behavior and usage.

## Installation

> [!NOTE]
> Make sure to setup your environment first by [following our instructions](xref:Uno.GetStarted.vs2022).

### Creating a new project with the Uno Material Toolkit

#### [**Wizard**](#tab/wizard)

1. Follow the steps in the [Getting Started with Visual Studio](xref:Uno.GettingStarted.CreateAnApp.VS2022#create-the-app) instructions to launch the Uno Platform Template Wizard.
2. Select `Material` under the `Theme` section.

    ![Material selection in the Uno Platform Template Wizard](assets/material-toolkit-wizard-theme.png)

3. Select `Toolkit` under the `Features` section.

    ![Toolkit selection in the Uno Platform Template Wizard](assets/material-toolkit-wizard-feature.png)

#### [**CLI**](#tab/cli)

1. Install the [`dotnet new` CLI templates](xref:Uno.GetStarted.dotnet-new) with:

    ```bash
    dotnet new install Uno.Templates
    ```

2. Create a new application with:

    ```bash
    dotnet new unoapp -o MaterialToolkitApp -toolkit -theme material
    ```

---

### Installing Uno Material Toolkit in an existing project

Depending on the type of project template that the Uno Platform application was created with, follow the instructions below to install the Uno Material Toolkit.

#### [**Single Project Template**](#tab/singleproj)

1. Edit your project file (`PROJECT_NAME.csproj`) and add `Toolkit` and `Material` to the list of `UnoFeatures`:

    ```xml
    <UnoFeatures>Toolkit;Material</UnoFeatures>
    ```

2. Initialize `MaterialToolkitTheme` in the `App.xaml`:

    ```xml
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>

                <!-- Code omitted of brevity -->

                <MaterialToolkitTheme xmlns="using:Uno.Toolkit.UI.Material" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
    ```

#### [**Multi-Head Project Template (Legacy)**](#tab/multihead)

1. In the Solution Explorer panel, right-click on your app's **App Code Library** project (`PROJECT_NAME.csproj`) and select `Manage NuGet Packages...`
2. Install the [`Uno.Toolkit.WinUI.Material`](https://www.nuget.org/packages/Uno.Toolkit.WinUI.Material)
3. Add the `MaterialToolkitTheme` to `AppResources.xaml`:

    ```xml
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>

            <MaterialToolkitTheme xmlns="using:Uno.Toolkit.UI.Material" />

        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
    ```

#### [**Shared Project (.shproj) Template (Legacy)**](#tab/shproj)

1. In the Solution Explorer panel, right-click on your solution name and select `Manage NuGet Packages for Solution ...`. Choose either:
     - The [`Uno.Toolkit.UI.Material`](https://www.nuget.org/packages/Uno.Toolkit.UI.Material/) package when targetting Xamarin/UWP
     - The [`Uno.Toolkit.WinUI.Material`](https://www.nuget.org/packages/Uno.Toolkit.WinUI.Material) package when targetting net6.0+/WinUI

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

                    <MaterialToolkitTheme xmlns="using:Uno.Toolkit.UI.Material" />

                    <!-- Load custom application resources -->
                    <!-- ... -->

                </ResourceDictionary.MergedDictionaries>
            </ResourceDictionary>
        </Application.Resources>
    </Application>
    ```

---

## Customization

Apply the properties shown in Uno Themes examples directly to `MaterialToolkitTheme`. It initializes the underlying theme and Toolkit resources, so do not add a separate `MaterialTheme`.

### Customize Colors

Follow the [Uno Material color override guide](xref:Uno.Themes.Material.GetStarted#manual-color-overrides), using `MaterialToolkitTheme` as the theme dictionary.

### Customize Fonts

See [Uno Themes typography customization](xref:Uno.Themes.DesignTokens#typography-font-swap) for `DefaultFontFamily`, override precedence, and runtime refresh behavior, and the [Uno Material font guide](xref:Uno.Themes.Material.GetStarted#change-default-font) for font resources.

Toolkit control-specific aliases such as `NavigationBarFontFamily` and `DividerSubHeaderFontFamily` retain their own override keys; `DefaultFontFamily` does not regenerate these aliases. See [NavigationBar styling](controls/NavigationBar.md) and [Divider styling](controls/Divider.md).

### Customize Spacing and Density

See [Uno Themes spacing and shape customization](xref:Uno.Themes.DesignTokens#via-scalar-properties) and [density modes](xref:Uno.Themes.DesignTokens#density-modes) for `DefaultSpacing` and `DefaultDensity`. Toolkit padding and margins defined as fixed values do not follow these generated scales.

### Seed Color Customization

Follow the [Uno Themes seed color guide](xref:Uno.Themes.SeedColors) for color generation and runtime customization, applying its examples to the Toolkit theme.

## Using C# Markup

The Uno Material Toolkit library also has support for C# Markup through a [Uno.Toolkit.WinUI.Material.Markup](https://www.nuget.org/packages/Uno.Toolkit.WinUI.Material.Markup) NuGet Package.

To get started with the Uno Material Toolkit in your C# Markup application, add the `Uno.Toolkit.WinUI.Material.Markup` NuGet package to your application project.
Then, add the following code to your `App.xaml.cs`:

```csharp
using Uno.Toolkit.UI.Material.Markup;

this.Build(r => r.UseMaterialToolkit(
    //optional
    new Styles.ColorPaletteOverride(),
    //optional
    new Styles.MaterialFontsOverride()
));
```

> [!NOTE]
> The [Uno.Toolkit.WinUI.Material.Markup](https://www.nuget.org/packages/Uno.Toolkit.WinUI.Material.Markup) NuGet package includes the base [Uno Toolkit Markup package](https://www.nuget.org/packages/Uno.Toolkit.WinUI.Markup) as a dependency. Therefore, there is no need to add the `Uno.Toolkit.WinUI.Markup` package separately. Furthermore, the `UseMaterialToolkit` extension method also initializes the base Uno Toolkit library, so there is no need to call the `UseToolkit` extension method in your `App.xaml.cs`.

## Additional Resources

- [Uno Platform Material Toolkit Sample App](https://aka.platform.uno/unomaterialtoolkit-sampleapp)
- [Uno Platform Material Toolkit Figma File](https://aka.platform.uno/uno-figma-material-toolkit)
- [Official Material Design 3 Guidelines](https://m3.material.io/components)
- [Official Material Design 3 Theme Builder](https://m3.material.io/theme-builder)
