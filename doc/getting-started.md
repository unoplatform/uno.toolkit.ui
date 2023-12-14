---
uid: Toolkit.GettingStarted
---
# Getting Started with Uno Toolkit

## Uno Toolkit Library

The Uno Toolkit library is available as NuGet packages that can be added to any new or existing Uno solution.

This article is a guide for installing the base Uno.Toolkit library, additional steps are needed when installing the Toolkit support libraries for Uno.Material and Uno.Cupertino. Most controls within the base Toolkit library are not designed to be used without an underlying design system installed. After following the installation steps below, refer to the following guides for Material/Cupertino support:

- [Getting Started with Material for Toolkit](xref:Toolkit.GettingStarted.Material)
- [Getting Started with Cupertino for Toolkit](xref:Toolkit.GettingStarted.Cupertino)

### Installation

1. Open an existing Uno project, or create a new Uno project using the `Multi-Platform App (Uno Platform)` template.
2. In the Solution Explorer panel, right-click on your app's **App Code Library** project (`PROJECT_NAME.csproj`) and select `Manage NuGet Packages...`
3. Install the [**`Uno.Toolkit.WinUI`**](https://www.nuget.org/packages/Uno.Toolkit.WinUI) package
4. Add the resources to `AppResources.xaml`:

    ```xml
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>

            <!-- Load Uno.Toolkit.UI resources -->
            <ToolkitResources xmlns="using:Uno.Toolkit.UI" />

            <!-- Load custom application resources -->
            <!-- ... -->

        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
    ```

#### Previous Installation Method

If your application is based on the older solution template that includes a shared project (.shproj), follow these steps:

1. Open your existing Uno project
2. In the Solution Explorer panel, right-click on your solution name and select `Manage NuGet Packages for Solution ...`. Choose either:
    - The [`Uno.Toolkit.UI`](https://www.nuget.org/packages/Uno.Toolkit.UI/) package when targetting Xamarin/UWP
    - The [`Uno.Toolkit.WinUI`](https://www.nuget.org/packages/Uno.Toolkit.WinUI) package when targetting net6.0+/WinUI

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

                    <!-- Load Uno.Toolkit.UI resources -->
                    <ToolkitResources xmlns="using:Uno.Toolkit.UI" />

                    <!-- Load custom application resources -->
                    <!-- ... -->

                </ResourceDictionary.MergedDictionaries>
            </ResourceDictionary>
        </Application.Resources>
    </Application>
    ```

## Using C# Markup

The Toolkit library also has support for C# Markup through a [Uno.Toolkit.WinUI.Markup](https://www.nuget.org/packages/Uno.Toolkit.WinUI.Markup) NuGet Package.

To get started with Toolkit in your C# Markup application, add the `Uno.Toolkit.WinUI.Markup` NuGet package to your **App Code Library** project and your platform heads.
Then, add the following code to your `AppResources.cs`:

```csharp
using Uno.Toolkit.UI.Markup;

this.Build(r => r.UseToolkit());
```

> [!NOTE]
> If you are using the [Uno.Toolkit.WinUI.Material.Markup](https://www.nuget.org/packages/Uno.Toolkit.WinUI.Material.Markup) NuGet package, follow the steps in the [Using C# Markup for the Material Toolkit](xref:Toolkit.GettingStarted.Material#using-c-markup) guide instead as it includes the Toolkit library.
