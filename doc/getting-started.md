---
uid: Toolkit.GettingStarted
---
# Getting Started with Uno Toolkit

## Uno Toolkit Library

The Uno.Toolkit library is available as NuGet packages that can be added to any new or existing Uno solution.

> [!NOTE]
> As of [Uno 4.7](https://platform.uno/blog/uno-platform-4-7-new-project-template-performance-improvements-and-more/), the solution template of an Uno app has changed. There is no longer a Shared project (.shproj), it has been replaced with a regular cross-platform library containing all user code files, referred to as the **App Code Library** project. This also implies that package references can be included in a single location without the previous need to include those in all project heads.

This article is a guide for installing the base Uno.Toolkit library, additional steps are needed when installing the Toolkit support libraries for Uno.Material and Uno.Cupertino. Most controls within the base Toolkit library are not designed to be used without an underlying design system installed. After following the installation steps below, refer to the following guides for Material/Cupertino support:

- [Getting Started with Material for Toolkit](./material-getting-started.md)
- [Getting Started with Cupertino for Toolkit](./cupertino-getting-started.md)

## Installation

The Uno Toolkit library can be included in your application by using the [Visual Studio Wizard](xref:Uno.GettingStarted.CreateAnApp.VS2022) and/or the [`dotnet new` templates](xref:Uno.GetStarted.dotnet-new)

### [**Wizard**](#tab/wizard)



1. Open an existing Uno project, or create a new Uno project using the `Multi-Platform App (Uno Platform)` template.
2. In the Solution Explorer panel, right-click on your app's **App Code Library** project (`PROJECT_NAME.csproj`) and select `Manage NuGet Packages...`
3. Install the [**`Uno.Toolkit.WinUI`**](https://www.nuget.org/packages/Uno.Toolkit.WinUI.Material) package
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
### [**dotnet new**](#tab/dotnet-new)

1. When creating a new project using the `dotnet new` templates, make sure to include the [`-toolkit` option](xref:Uno.GettingStarted.UsingWizard.Features#toolkit) in the `dotnet new unoapp` command.     
    
    `dotnet new unoapp -preset blank -o MyApp -toolkit` 

2. To incorporate the Uno Toolkit into an existing application, you must add the [Uno.Toolkit.WinUI NuGet package](https://www.nuget.org/packages/Uno.Toolkit.WinUI) to the **App Code Library** project as well as the platform heads. An easy way to do this is through the command line. Run the following command within the directory of each `.csproj`:       
    
    `dotnet add package Uno.Toolkit.WinUI --version [latest version number]`. Make sure to replace `[latest version number] `with the actual latest NuGet version number.



***


#### Previous Installation Method

If your application is based on the older solution template that includes a shared project (.shproj), follow these steps:

1. Open your existing Uno project
2. In the Solution Explorer panel, right-click on your solution name and select `Manage NuGet Packages for Solution ...`. Choose either:
    - The [`Uno.Toolkit.UI`](https://www.nuget.org/packages/Uno.Material/) package when targetting Xamarin/UWP
    - The [`Uno.Toolkit.WinUI`](https://www.nuget.org/packages/Uno.Material.WinUI) package when targetting net6.0+/WinUI

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

> [!NOTE]: If you are using the [Uno.Toolkit.WinUI.Material.Markup](https://www.nuget.org/packages/Uno.Toolkit.WinUI.Material.Markup) NuGet package, follow the steps in the [Using C# Markup for the Material Toolkit](./material-getting-started.md#using-c-markup) guide instead as it includes the Toolkit library.
