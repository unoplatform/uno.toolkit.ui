---
uid: Toolkit.GettingStarted.Cupertino
---
# Uno Toolkit Cupertino Library

The Uno.Toolkit.Cupertino library is available as NuGet packages that can be added to any new or existing Uno solution.

## Installation

> [!NOTE]
> As of [Uno Platform 4.7](https://platform.uno/blog/uno-platform-4-7-new-project-template-performance-improvements-and-more/), the solution template of the Uno app has changed. There is no longer a Shared project (.shproj), it has been replaced with a regular cross-platform library containing all user code files, referred to as the **App Code Library** project. This also implies that package references can be included in a single location without the previous need to include those in all project heads.

1. Open an existing Uno project, or create a new Uno project using the `Multi-Platform App (Uno Platform)` template.
2. In the Solution Explorer panel, right-click on your app's **App Code Library** project (`PROJECT_NAME.csproj`) and select `Manage NuGet Packages...`
3. Install the [**`Uno.Toolkit.WinUI.Cupertino`**](https://www.nuget.org/packages/Uno.Toolkit.WinUI.Cupertino) package
4. Add the resources to `AppResources.xaml`:

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

            <!-- Load custom application resources -->
            <!-- ... -->

        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
    ```

### Installing the Cupertino Toolkit on previous versions of Uno Platform

If your application is based on the older solution template that includes a shared project (.shproj), follow these steps:

1. Open your existing Uno project
2. In the Solution Explorer panel, right-click on your solution name and select `Manage NuGet Packages for Solution ...`. Choose either:
    - The [**`Uno.Toolkit.UI.Cupertino`**](https://www.nuget.org/packages/Uno.Toolkit.UI.Cupertino) package when targetting Xamarin/UWP
    - The [**`Uno.Toolkit.WinUI.Cupertino`**](https://www.nuget.org/packages/Uno.Toolkit.WinUI.Cupertino) package when targetting net6.0+/WinUI

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

## Customization

For instructions on changing the default color palette or the font family, please refer to the [Customization section in Uno.Cupertino guide](https://platform.uno/docs/articles/external/uno.themes/doc/cupertino-getting-started.html#customization).
