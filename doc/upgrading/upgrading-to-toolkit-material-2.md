---
uid: Toolkit.Material.Upgrading2
---

# Updating to Toolkit Material v2

Starting with version 2.0.0 of the [Uno.Toolkit.UI.Material](https://www.nuget.org/packages/Uno.Toolkit.UI.Material/2.0.0) and [Uno.Toolkit.WinUI.Material](https://www.nuget.org/packages/Uno.Toolkit.WinUI.Material/2.0.0) packages, users can now take advantage of the new [Material Design 3](https://m3.material.io/) design system from Google.
Along with the new Material Design 3 styles, our Material Toolkit NuGet packages will continue to support the previous Material Design 2 styles. In order to achieve this backward compatibility, we have had to make some changes to the way the Material toolkit is initialized within your `App.xaml`. 

> [!NOTE]
> In order to avoid any confusion going forward in this article, we will be referring to the new collection of Material Design 3 compatible styles as the "v2" styles and the previous collection of Material Design 2 styles will be referred to as the "v1" styles.

 There are two available paths once you have updated to the new Material Toolkit v2 package: 

- Continue to keep using the v1 styles; or,
- Migrate app to reference the new v2 styles


## Continue Using v1 Styles
> [!WARNING]
> In order to continue using the v1 styles, some changes are required in your `App.xaml`.

Since the Material Toolkit has a dependency on the Uno Material library, it is required to first follow the steps in the **_Continue Using v1 Styles_** section of the [Uno Material v2 migration documentation](xref:Uno.Themes.Material.Migration).

The Material Toolkit v2 NuGet package contains both sets of v1 and v2 styles. Within your `App.xaml`, you will need to replace the reference to `MaterialToolkitResources` with `MaterialToolkitResourcesV1`.

## Migrating to v2 Styles
`MaterialToolkitResources` will now always initialize the latest version of the Material-compatible Toolkit styles. It is also possible to directly reference the `MaterialToolkitResourcesV2` ResourceDictionary if needed.
