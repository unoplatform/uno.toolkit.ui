# Fluent Toolkit

## Contract and plan

Create `Uno.Toolkit.WinUI.Fluent` backed by the `Uno.Fluent.WinUI` implementation in `ref/Uno.Themes`. Inherit its semantic palette, typography, design tokens, override and rebuild behavior. Supply Fluent Toolkit styles for the complete shared semantic surface, including the additional Simple variants, and appropriate defaults for all visual Toolkit controls. Preserve existing control behavior and templates where they already implement Fluent interaction.

- [x] Inspect theme integration, resource keys, control templates, and available validation tools.
- [x] Add the Fluent package and composed `FluentToolkitTheme`, referencing the local Fluent library.
- [x] Implement navigation, tab, chip, card, divider, and utility-control Fluent styles and semantic aliases.
- [x] Add a Fluent sample host and runtime coverage for style resolution, templates, states, and theme updates.
- [x] Document setup, semantic mappings, and lightweight customization.
- [x] Build Desktop Release and run the Fluent runtime tests.
- [x] Build WebAssembly Release and validate local NuGet packaging.
- [x] Resolve WebAssembly test-engine startup failure and SDK dependency advisories, then meet the warning-free Fluent cross-platform verification gate.
- [x] Review the diff and record exact verification results and limitations.

## Design decisions

### Approved verification follow-up

On September 7, the user approved fixing shared WASM test startup and updating the vulnerable dependency.

- [x] Use a browser-safe hosted test engine without changing test selection or suppressing failures.
- [x] Update the SDK-injected compatibility dependency to a patched .NET 10 servicing version.
- [x] Rebuild and run the Fluent tests on WASM and Desktop; record results and remaining gaps.

- The requested new package, source dependency, and sample host are within the requested Fluent implementation; no unrelated dependencies or framework changes are planned.
- Compose the Fluent base dictionary, Toolkit base dictionary, and Fluent Toolkit dictionary through `DefaultStylesSource`.
- Resolve semantic aliases against bundle-owned styles in code to avoid scoped XAML aliases binding to a different app theme.
- Use semantic `ThemeResource` brushes, Fluent geometry and interaction patterns, and distinct card content layouts.
- Keep behavioral controls and platform-specific implementations intact. Layout-only components need no artificial theme chrome.

## Review and verification

- Initial Release library build succeeded. Its referenced Themes source emitted NU1507 (multiple feeds without mapping) and CS0618 (legacy color override properties).
- Large pinned SDK package downloads required retries on the configured feeds. Sample restore has now completed without SDK or target-framework changes.
- First sample compilation identified nullable-flow errors because this repository uses MSTest 2; explicit assertion guards now satisfy nullable analysis without suppressions.
- Desktop Release: **0 warnings, 0 errors** using the single public restore feed and `NugetOverrideVersion=`. Runtime tests: **13 passed, 0 failed, 0 skipped** (`Given_FluentToolkitTheme`). Results: `/private/tmp/fluent-toolkit-desktop-results.xml`.
- Red/fix/green coverage exposed and fixed card UIElement presentation, initially-loaded progress-ring activity, and danger-chip error colors during pressed/selected states. Light/dark realization, semantic alias scoping, all card slots, chip selection/removal, interaction states, token overrides, and theme rebuilding are covered.
- Navigation uses the shared XAML template with an explicit Back icon so native mobile templates do not substitute platform-specific typography. Mobile and Windows targets have not been built or run on this host.
- Initial WebAssembly Release had **8 NU1903 warnings** from the SDK-injected compatibility bundle. The approved follow-up sets `WindowsCompatibilityVersion=10.0.11` in sample build properties, updating its `System.Security.Cryptography.Xml` dependency without adding warning suppressions. [Microsoft's advisory](https://github.com/advisories/GHSA-23rf-6693-g89p) lists the .NET 10 fix starting at 10.0.10.
- WebAssembly Release after the follow-up: **0 warnings, 0 errors; 13 tests passed, 0 failed, 0 skipped**. Results: `/private/tmp/fluent-toolkit-wasm-results.xml`; logs: `/private/tmp/fluent-wasm-build.log` and `/private/tmp/fluent-wasm-tests.log`.
- Desktop Release after runner relocation: **0 warnings, 0 errors; 13 tests passed, 0 failed, 0 skipped**. The source-link condition uses sample project names because Uno finalizes Desktop `OutputType` after the directory-target import.
- All four sample heads compile in the Desktop samples solution with `UnoDisableHotDesign=true`: **0 errors, 300 warnings** in shared sample code and referenced Themes source. This broader build is not warning-free; unrelated nullable/obsolete/API warnings were not suppressed or refactored. The first attempt was canceled during the HotDesign download, then retried with the source-build switch. Log: `/private/tmp/fluent-all-samples-desktop-build.log`.
- Cross-host regression: the Material Desktop host also runs all **13 Fluent tests successfully**, confirming the relocated runner and scoped Fluent theme work in an existing sample head. Results: `/private/tmp/fluent-material-host-results.xml`.
- The original WebAssembly run failed before execution because the embedded runner's browser conditionals were compiled in the shared net9.0 assembly. The fix removes only that source file from the shared assembly and links the original package source into each sample head. Sample friend assemblies can access the existing engine internals; test discovery and hot-reload source metadata stay in the test assembly. No package source or NuGet cache was patched. Use runner `2.0.0-dev.42` with `--filter Given_FluentToolkitTheme`, not a duplicate RUN_TESTS query parameter.
- Local package validation succeeded: the generated nuspec contains `Uno.Fluent.WinUI` and `Uno.Toolkit.WinUI` dependencies, the Fluent assembly, and build-transitive targets. The local reference checkout evaluates to version `1.0.0`; publishing must supply coordinated release versions. Package: `/private/tmp/fluent-toolkit-packages/Uno.Toolkit.WinUI.Fluent.255.255.255.255.nupkg`.
- XAML was formatted with repository settings. No changes were made to vendored Themes source or the user's cross-target override. Interactive visual review could not run because computer-use permission is unavailable; runtime visual-tree/state assertions passed on Desktop.
