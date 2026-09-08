# Fluent shared sample integration

## Contract

Replace the standalone Fluent gallery with the same shared sample host architecture used by Simple and Material. Preserve Desktop/WASM targets and runtime-test startup. Set the Fluent design before constructing the shared shell; include all existing pages with `FluentTemplate` and keep design-agnostic samples available under the shared policy. Do not fabricate Fluent templates by relabeling Material examples.

- [x] Compare shared project imports, App resources, navigation, and design filters.
- [x] Add and run a failing host/catalog regression before replacing the gallery.
- [x] Import shared content and resources; initialize the shared App/Shell and Fluent design.
- [x] Integrate Fluent catalog filtering and template selection; retire redundant gallery files.
- [x] Build and run hosted runtime tests on Desktop and WASM; verify existing Material filtering.
- [x] Update documentation and record results and limitations.

## Review and verification

- Red: all three shared-host regressions failed against the standalone Fluent app, confirming the missing shared App/catalog. Results: `/private/tmp/fluent-shared-sample-red.xml`.
- Static catalog audit: 27 eligible Fluent entries (one Fluent-template page, 25 agnostic-template pages, and runtime runner). Material, Cupertino, and Simple visibility is unchanged.
- User approved the matching `Uno.Core.Extensions.Compatibility` reference; added it without changing the centrally managed version.
- Release builds succeed: Fluent Desktop 95 warnings/0 errors, Fluent WASM 100 warnings/0 errors, Material Desktop 100 warnings/0 errors. Warnings come from shared sample sources; no suppression was added.
- Hosted runtime tests: 17/17 pass on Fluent Desktop (`/private/tmp/fluent-shared-desktop-final.xml`), Fluent WASM (`/private/tmp/fluent-shared-wasm-final.xml`), and Material Desktop (`/private/tmp/fluent-shared-material-green.xml`), with no skipped or inconclusive cases. The suite includes all 13 Fluent style tests and four shared-host regressions for the catalog, rendered templates, shell, and landing page.
- The catalog test renders all 26 content pages in Fluent. The runtime-runner entry is checked structurally rather than instantiated inside another runner: a second `UnitTestsControl` redirects the engine's global UI helper. Actual runner startup is exercised by every hosted test invocation. Tests restore the prior UI content and selected design.
- Material regression red/fix/green: Overview rendered blank because the shared version selector always selected M3 even without an M3 template (`/private/tmp/fluent-shared-material-final.xml`: 16/17). The selector now defaults to M2 when M3 is unavailable; all 17 cases pass after the fix. Catalog visibility is unchanged.
- Shared Card resources have been scoped to their actual Material templates, and Fluent no longer initializes the hidden Material-version selector. Existing Fluent nested navigation examples now use semantic Toolkit styles.
- The standalone `MainPage.xaml`/code-behind were removed; they remain recoverable from git. The updated Desktop sample was launched after verification. Documentation and shared agent instructions now describe the shared-head architecture.
- The generated base XAML was restored through `GenerateMergedXaml` in Debug; no generated-source edits are included. Diff whitespace validation passes.

### Remaining limitations

- Release builds are not warning-free: the shared samples report nullable, unsupported-API, and (on WASM) trimming warnings. No warning suppression or unrelated cleanup was introduced. The matching Material head reports the same shared-source warning categories; warning cleanup remains separate from this integration.
- Runtime logs still contain resource-resolution diagnostics and existing shared sample diagnostics (including ResponsiveExtension on non-FrameworkElement targets, and the base splash template's internal `SplashScreenContent` binding). Rendering and template assertions pass; this change does not claim to fix those broader shared-control diagnostics.
- User requested committing and pushing this follow-up correction to `dev/sb/fluent` after verification.
