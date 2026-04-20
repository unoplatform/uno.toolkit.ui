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

| Property              | Type                 | Description                                                                                                                                                                       |
|-----------------------|----------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `ColorOverrideSource` | `string`             | (Optional) Gets or sets a Uniform Resource Identifier that provides the source location of a ResourceDictionary containing overrides for the default Simple Color resources       |
| `FontOverrideSource`  | `string`             | (Optional) Gets or sets a Uniform Resource Identifier that provides the source location of a ResourceDictionary containing overrides for the default Simple FontFamily resources  |
| `DefaultSize`         | `SimpleControlSize`  | (Optional) Gets or sets the default size variant for control styles. The default is `Small`. Accepted values are `Small` (compact, 32 px height for buttons) and `Medium` (standard, 40 px height for buttons) |

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

With `SimpleToolkitTheme`, you do not need to explicitly initialize `ToolkitResources`, `SimpleTheme`, or `SimpleColors`. This means that all resource overrides should go through `SimpleToolkitTheme` itself.

### Customize Colors

Follow the [Uno Simple Customization guide](xref:Uno.Themes.Simple.GetStarted#customization) to create a `ColorPaletteOverride.xaml` file and add it to your application project.

In `App.xaml`, use the `ColorOverrideSource` property on `SimpleToolkitTheme`:

```xml
<SimpleToolkitTheme xmlns="using:Uno.Toolkit.UI.Simple"
                    ColorOverrideSource="ms-appx:///Style/Application/ColorPaletteOverride.xaml" />
```

### Customize Fonts

Follow the [Uno Simple Customization guide](xref:Uno.Themes.Simple.GetStarted#customization) to create a `FontOverride.xaml` file and add it to your application project.

In `App.xaml`, use the `FontOverrideSource` property on `SimpleToolkitTheme`:

```xml
<SimpleToolkitTheme xmlns="using:Uno.Toolkit.UI.Simple"
                    FontOverrideSource="ms-appx:///Style/Application/FontOverride.xaml" />
```

### Customize Default Control Size

The `SimpleToolkitTheme` supports two size variants for control styles through the `DefaultSize` property:

- `Small` (default) - Compact sizing (32 px height for buttons)
- `Medium` - Standard sizing (40 px height for buttons)

In `App.xaml`, use the `DefaultSize` property on `SimpleToolkitTheme`:

```xml
<SimpleToolkitTheme xmlns="using:Uno.Toolkit.UI.Simple"
                    DefaultSize="Medium" />
```

### Seed Color Customization

`SimpleToolkitTheme` supports seed-based color generation. A single seed color is used to derive tonal palettes for both Light and Dark themes.

`SimpleToolkitTheme` uses seed color generation by default with a neutral gray seed (`#808080`). You can override it by setting the `Colors` property:

```xml
<SimpleToolkitTheme xmlns="using:Uno.Toolkit.UI.Simple"
                    xmlns:ut="using:Uno.Themes">
    <SimpleToolkitTheme.Colors>
        <ut:ThemeColors PrimarySeed="#2196F3" />
    </SimpleToolkitTheme.Colors>
</SimpleToolkitTheme>
```

You can also change the seed color at runtime:

```csharp
using Uno.Themes;
using Windows.UI;

SemanticThemeHelper.PrimarySeed = Color.FromArgb(0xFF, 0x21, 0x96, 0xF3);
```

For more details, see the [Seed Color Palette documentation](xref:Uno.Themes.SeedColors).
