---
uid: Toolkit.GettingStarted.Fluent
---
# Uno Fluent Toolkit

`Uno.Toolkit.WinUI.Fluent` supplies Fluent styles for Toolkit controls and inherits the semantic colors, typography, and theme settings from `Uno.Fluent.WinUI`.

## Setup

Reference `Uno.Toolkit.WinUI.Fluent`. In this repository, the project references the Fluent library in `ref/Uno.Themes` directly, so that checkout must contain `src/library/Uno.Fluent.WinUI/Uno.Fluent.WinUI.csproj`. The source dependency becomes a dependency on `Uno.Fluent.WinUI` when packing.

Merge the platform resources first, followed by `FluentToolkitTheme`:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <XamlControlsResources xmlns="using:Microsoft.UI.Xaml.Controls" />
            <FluentToolkitTheme xmlns="using:Uno.Toolkit.UI.Fluent" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

`FluentToolkitTheme` includes the Fluent base theme and Toolkit resources. It replaces separate `FluentTheme` and `ToolkitResources` dictionaries. The platform resources must be available when the theme is constructed, including when constructing a theme at page scope.

Use the shared [semantic style keys](controls-styles.md) in portable XAML:

```xml
<StackPanel xmlns:utu="using:Uno.Toolkit.UI" Spacing="16">
    <utu:NavigationBar Content="Projects" Style="{StaticResource NavigationBarStyle}" />
    <utu:Chip Content="Assigned to me" Style="{StaticResource FilterChipStyle}" />
    <utu:Card HeaderContent="Project status"
              SupportingContent="All tasks are up to date."
              Style="{StaticResource OutlinedCardStyle}" />
</StackPanel>
```

## Control coverage

| Controls | Fluent treatment |
|---|---|
| NavigationBar and commands | Shared XAML command-bar template on every target, neutral surface, semantic typography; primary variants emphasize the accent. |
| Chip and ChipGroup | Compact outlined controls, accent selection, pointer/pressed/disabled states, removable input chips. Elevated semantic variants use the same Fluent chip treatment. |
| TabBar and TabBarItem | Top, bottom, and vertical navigation with accent indicators, badges, and nonselectable action items for FAB aliases. |
| Card | Filled, outlined, elevated, brand, avatar, and small-media styles; semantic typography and all six content slots. |
| CardContentControl | Filled, outlined, and elevated containers with the same Fluent shape and interaction states. |
| Divider | Subtle one-pixel rule with an optional semantic caption. |
| DrawerControl and DrawerFlyoutPresenter | Semantic surfaces, system dismiss overlay, and Fluent flyout borders and corners; directional flyout aliases retained. |
| LoadingView and ExtendedSplashScreen | Existing loading behavior with a Fluent progress indicator. |
| ResponsiveView, SafeArea, ZoomContentControl | Existing functional templates with inherited semantic foregrounds. |
| AutoLayout and ShadowContainer | Layout and effect behavior is theme-independent; no additional chrome is imposed. |

The theme includes every semantic style alias in the Simple Toolkit surface, including `DangerChipStyle`, `BrandFilledCardStyle`, and `BrandOutlinedCardStyle`. Concrete styles use the `Fluent` prefix, for example `FluentOutlinedCardStyle`.

Toolkit controls receive implicit defaults: top navigation for tabs, filled cards, standard chips, and the corresponding utility styles. Stock WinUI controls retain the defaults supplied by `XamlControlsResources`; `MainCommandStyle` is applied explicitly to navigation commands.

`DrawerFlyoutPresenterStyle` and its directional variants target the platform `FlyoutPresenter`. `ToolkitDrawerFlyoutPresenterStyle` targets the Toolkit `DrawerFlyoutPresenter` itself.

## Customization

The inherited `Colors`, `DefaultFontFamily`, `DefaultCornerRadius`, `DefaultSpacing`, `DefaultDensity`, and font override properties are available directly on `FluentToolkitTheme`. Color overrides should provide both Light and Default (dark) dictionaries. Semantic colors and brushes use `ThemeResource` so controls follow the active appearance.

Cards use `OverlayCornerRadius` and `Space400Thickness`; chip and tab corners use `ControlCornerRadius`. Card typography uses `TitleMedium` and `BodyMedium` font resources. See [lightweight styling](lightweight-styling.md#fluent-toolkit) for resource customization. Value-type token changes can require control recreation, as documented by the base theme.

Card avatar and media defaults accept image data. When placing a UIElement directly in those slots, clear the corresponding content template. Text slots accept strings or UIElements directly; custom data templates remain available for data objects.

## Sample and runtime tests

The dedicated gallery is `samples/Uno.Toolkit.Samples.Fluent/FluentSampleApp.csproj` and targets Desktop and WebAssembly:

```shell
dotnet run --project samples/Uno.Toolkit.Samples.Fluent/FluentSampleApp.csproj -f net10.0-desktop -p:TargetFrameworkOverride=desktop -p:NugetOverrideVersion=
```

Use `--mode=rt` to open the hosted runtime-test runner. For automated execution, set both `UNO_RUNTIME_TESTS_RUN_TESTS='{"Filter":{"Value":"Given_FluentToolkitTheme"}}'` and `UNO_RUNTIME_TESTS_OUTPUT_PATH=<absolute-results.xml>` before launching the built desktop DLL. The installed engine requires the output environment variable; the `--runtime-tests` argument alone does not configure it.

For WebAssembly, use the repository's `uno-runtimetests-wasm` runner with `--filter Given_FluentToolkitTheme` and `--query-param mode=rt`. Do not add a second `UNO_RUNTIME_TESTS_RUN_TESTS` query parameter: the runner configures that value itself.

The shared test assembly contains the tests and engine UI. `samples/Directory.Build.targets` links the package's embedded runner into each platform head so browser-only startup and configuration paths compile correctly. Keep `UnoRuntimeTestsEngineVersion` in the root build properties synchronized with the package reference through its existing property expression.
