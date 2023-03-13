# Uno Toolkit Material Library

<p align="center">
  <img src="assets/material-toolkit-design-system.png">
</p>

The Uno.Toolkit.Material library is available as NuGet packages that can be added to any new or existing Uno solution.

## Getting Started

Initialization of the Material Toolkit resources is handled by the specialized `MaterialToolkitTheme` ResourceDictionary.

### `MaterialToolkitTheme`

> [!NOTE]
> The `MaterialToolkitTheme` class also handles the required initialization of the Uno.Material resources. Therefore, there is no need to initialize `MaterialTheme` within the `AppResources.xaml`

#### Constructors

| Constructor    | Description                                           |
|----------------|-------------------------------------------------------|
| MaterialToolkitTheme()         | Initializes a new instance of the `MaterialToolkitTheme` resource dictionary.       |
| MaterialToolkitTheme(ResourceDictionary? colorOverride, ResourceDictionary? fontOverride)         | Initializes a new instance of the `MaterialToolkitTheme` resource dictionary and applies the given overrides       |

#### Properties

| Property                  | Type              | Description                                                                                   |
|---------------------------|-------------------|-----------------------------------------------------------------------------------------------|
| ColorOverrideSource             | string            | (Optional) Gets or sets a Uniform Resource Identifier that provides the source location of a ResourceDictionary containing overrides for the default Uno.Material Color resources                                            |
| FontOverrideSource     | string      | (Optional) Gets or sets a Uniform Resource Identifier that provides the source location of a ResourceDictionary containing overrides for the default Uno.Material FontFamily resources            |

## Installation

> [!NOTE]
> As of [Uno Platform 4.7](https://platform.uno/blog/uno-platform-4-7-new-project-template-performance-improvements-and-more/), the solution template of the Uno app has changed. There is no longer a Shared project (.shproj), it has been replaced with a regular cross-platform library containing all user code files, referred to as the **App Code Library** project. This also implies that package references can be included in a single location without the previous need to include those in all project heads.

1. Open an existing Uno project, or create a new Uno project using the `Multi-Platform App (Uno Platform)` template.
2. In the Solution Explorer panel, right-click on your app's **App Code Library** project (`PROJECT_NAME.csproj`) and select `Manage NuGet Packages...`
3. Install the [**`Uno.Toolkit.WinUI.Material`**](https://www.nuget.org/packages/Uno.Toolkit.WinUI.Material) package
4. Add the resources to `AppResources.xaml`:

    ```xml
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>

            <!-- Load Material Toolkit resources (also loads the base Uno.Material resources) -->
            <MaterialToolkitTheme xmlns="using:Uno.Toolkit.UI.Material" />

            <!-- Load custom application resources -->
            <!-- ... -->

        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
    ```

### Installing the Material Toolkit on previous versions of Uno Platform

If your application is based on the older solution template that includes a shared project (.shproj), follow these steps:

1. Open your existing Uno project
2. In the Solution Explorer panel, right-click on your solution name and select `Manage NuGet Packages for Solution ...`. Choose either:
    - The [**`Uno.Toolkit.UI.Material`**](https://www.nuget.org/packages/Uno.Toolkit.UI.Material) package when targetting Xamarin/UWP
    - The [**`Uno.Toolkit.WinUI.Material`**](https://www.nuget.org/packages/Uno.Toolkit.WinUI.Material) package when targetting net6.0+/WinUI

3. Select the following projects for installation:
    - `PROJECT_NAME.Wasm.csproj`
    - `PROJECT_NAME.Mobile.csproj` (or `PROJECT_NAME.iOS.csproj`, `PROJECT_NAME.Droid.csproj`, `PROJECT_NAME.macOS.csproj` if you have an existing project)
    - `PROJECT_NAME.Skia.Gtk.csproj`
    - `PROJECT_NAME.Skia.WPF.csproj`
    - `PROJECT_NAME.Windows.csproj` (or `PROJECT_NAME.UWP.csproj` for existing projects)
4. Add the resources to `App.xaml`:

    ```xml
    <Application ...>
        <Application.Resources>
            <ResourceDictionary>
                <ResourceDictionary.MergedDictionaries>

                    <!-- Load WinUI resources -->
                    <XamlControlsResources xmlns="using:Microsoft.UI.Xaml.Controls" />
                    
                    <!-- Load Material Toolkit resources (also loads the base Uno.Material resources) -->
                    <MaterialToolkitTheme xmlns="using:Uno.Toolkit.UI" />

                    <!-- Load custom application resources -->
                    <!-- ... -->

                </ResourceDictionary.MergedDictionaries>
            </ResourceDictionary>
        </Application.Resources>
    </Application>
    ```

## Customization

With `MaterialToolkitTheme`, you do not need to explicitly initialize `MaterialTheme`, `MaterialColors`, or `MaterialFonts`. This means that all resource overrides should go through `MaterialToolkitTheme` itself. There are two properties on `MaterialToolkitTheme` that you can set within your `AppResources.xaml`.

### Customize Colors

Follow the steps [here](https://platform.uno/docs/articles/external/uno.themes/doc/material-getting-started.html#customize-color-palette) to create a `ColorPaletteOverride.xaml` file and add it to your **App Code Library** project (`PROJECT_NAME.csproj`)

In `AppResources.xaml`, instead of initializing `MaterialColors`, you would use the `ColorOverrideSource` property on `MaterialToolkitTheme`:

```xml
<MaterialToolkitTheme xmlns="using:Uno.Toolkit.UI.Material"
                      ColorOverrideSource="ms-appx:///PROJECT_NAME/Style/Application/ColorPaletteOverride.xaml" />
```

### Customize Fonts

Follow the steps [here](https://platform.uno/docs/articles/external/uno.themes/doc/material-getting-started.html#change-default-font) to create a `FontOverride.xaml` file and add it to your **App Code Library** project (`PROJECT_NAME.csproj`)

In `AppResources.xaml`, instead of initializing `MaterialFonts`, you would use the `FontOverrideSource` property on `MaterialToolkitTheme`:

```xml
<MaterialToolkitTheme xmlns="using:Uno.Toolkit.UI.Material"
                      FontOverrideSource="ms-appx:///PROJECT_NAME/Style/Application/FontOverride.xaml" />
```

## Additional Resources

- [Uno Platform Material Toolkit Sample App](https://github.com/unoplatform/Uno.Samples/tree/master/UI/UnoMaterialToolkitSample)
- [Uno Platform Material Toolkit Figma File](https://www.figma.com/community/file/1110792522046146058)
- [Official Material Design 3 Guidelines](https://m3.material.io/components)
- [Official Material Design 3 Theme Builder](https://m3.material.io/theme-builder)
