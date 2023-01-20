# Getting Started with Uno Toolkit

## Uno Toolkit Library

The Uno.Toolkit library is available as NuGet packages that can be added to any new or existing Uno solution.

> [!NOTE]: As of [Uno 4.7](https://platform.uno/blog/uno-platform-4-7-new-project-template-performance-improvements-and-more/), the solution template of an Uno app has changed. There is no longer a Shared project (.shproj), it has been replaced with a regular cross-platform library containing all user code files, referred to as the **App Code Library** project. This also implies that package references can be included in a single location without the previous need to include those in all project heads.

This article is a guide for installing the base Uno.Toolkit library, additional steps are needed when installing the Toolkit support libraries for Uno.Material and Uno.Cupertino. Most controls within the base Toolkit library are not designed to be used without an underlying design system installed. After following the installation steps below, refer to the following guides for Material/Cupertino support:

- [Getting Started with Material for Toolkit](./material-getting-started.md)
- [Getting Started with Cupertino for Toolkit](./cupertino-getting-started.md)

### Installation

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
3. Add the resources to `App.xaml`:
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