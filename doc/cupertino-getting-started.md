---
uid: Toolkit.GettingStarted.Cupertino
---
# Uno Cupertino Toolkit Library

The Uno Cupertino Toolkit library is available as NuGet packages that can be added to any new or existing Uno solution.

## Installation

> [!NOTE]
> Make sure to setup your environment first by [following our instructions](xref:Uno.GetStarted.vs2022).

### Creating a new project with the Uno Material Toolkit

1. Install the [`dotnet new` CLI templates](xref:Uno.GetStarted.dotnet-new) with:

    ```bash
    dotnet new install Uno.Templates
    ```

2. Create a new application with:

    ```bash
    dotnet new unoapp -o CupertinoToolkitApp -toolkit -theme cupertino
    ```

---

### Installing Uno Cupertino Toolkit in an existing project

Depending on the type of project template that the Uno Platform application was created with, follow the instructions below to install the Uno Cupertino Toolkit.

#### [**Single Project Template**](#tab/singleproj)

1. Edit your project file (`PROJECT_NAME.csproj`) and add `Toolkit` and `Cupertino` to the list of `UnoFeatures`:

    ```xml
    <UnoFeatures>Toolkit;Cupertino</UnoFeatures>
    ```

2. Initialize the resources in the `App.xaml`:

    ```xml
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>

                <!-- Code omitted of brevity -->

                <!-- Load Cupertino resources -->
                <CupertinoColors xmlns="using:Uno.Cupertino"  />
                <CupertinoFonts xmlns="using:Uno.Cupertino"  />
                <CupertinoResources xmlns="using:Uno.Cupertino" />

                <!-- Load Cupertino Toolkit resources -->
                <ToolkitResources xmlns="using:Uno.Toolkit.UI" />
                <CupertinoToolkitResources xmlns="using:Uno.Toolkit.UI.Cupertino" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
    ```

#### [**Multi-Head Project Template (Legacy)**](#tab/multihead)

1. In the Solution Explorer panel, right-click on your app's **App Code Library** project (`PROJECT_NAME.csproj`) and select `Manage NuGet Packages...`
2. Install the [`Uno.Toolkit.WinUI.Cupertino`](https://www.nuget.org/packages/Uno.Toolkit.WinUI.Cupertino)
3. Add the resources to `AppResources.xaml`:

    ```xml
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>

            <!-- Load Cupertino resources -->
            <CupertinoColors xmlns="using:Uno.Cupertino"  />
            <CupertinoFonts xmlns="using:Uno.Cupertino"  />
            <CupertinoResources xmlns="using:Uno.Cupertino" />

            <!-- Load Cupertino Toolkit resources -->
            <ToolkitResources xmlns="using:Uno.Toolkit.UI" />
            <CupertinoToolkitResources xmlns="using:Uno.Toolkit.UI.Cupertino" />

        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
    ```

#### [**Shared Project (.shproj) Template (Legacy)**](#tab/shproj)

1. In the Solution Explorer panel, right-click on your solution name and select `Manage NuGet Packages for Solution ...`. Choose either:
     - The [`Uno.Toolkit.UI.Cupertino`](https://www.nuget.org/packages/Uno.Toolkit.UI.Cupertino/) package when targetting Xamarin/UWP
     - The [`Uno.Toolkit.WinUI.Cupertino`](https://www.nuget.org/packages/Uno.Toolkit.WinUI.Cupertino) package when targetting net6.0+/WinUI

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

                    <!-- Load Cupertino resources -->
                    <CupertinoColors xmlns="using:Uno.Cupertino"  />
                    <CupertinoFonts xmlns="using:Uno.Cupertino"  />
                    <CupertinoResources xmlns="using:Uno.Cupertino" />

                    <!-- Load Cupertino Toolkit resources -->
                    <ToolkitResources xmlns="using:Uno.Toolkit.UI" />
                    <CupertinoToolkitResources xmlns="using:Uno.Toolkit.UI.Cupertino" />

                    <!-- Load custom application resources -->
                    <!-- ... -->

                </ResourceDictionary.MergedDictionaries>
            </ResourceDictionary>
        </Application.Resources>
    </Application>
    ```

---

## Customization

For instructions on changing the default color palette or the font family, please refer to the [Uno Cupertino Customization guide](xref:Uno.Themes.Cupertino.GetStarted#customization).
