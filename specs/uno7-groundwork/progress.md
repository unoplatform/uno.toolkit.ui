# Uno Platform 7.0 groundwork

Tracking branch: `dev/mazi/uno7-groundwork` — PR [#1635](https://github.com/unoplatform/uno.toolkit.ui/pull/1635).

Uno 7.0 removes the native iOS/UIKit and Android view-rendering layers (`UIElement` no longer
derives from `UIView` / `Android.Views.View`), renames `Uno.UI.Toolkit` to `Uno.UI.Extras` and
re-namespaces its types, turns `DependencyObject` into a class, and drops the native style
pipeline. This document tracks what that costs the toolkit.

## Method note

The Uno checkout at `D:\Work\uno` is **stale** relative to the shipped package. Read the shipped
assemblies under `D:\Packages\NuGet\uno.winui\<version>`, or the source at the commit the package
pins: `uno.winui.nuspec` for `7.0.0-dev.701` pins `0a4053a40b7bc7fe5f7a0ef5aaa890c5cd5e7a6d`
(use `git show <sha>:<path>`, never a working-tree grep).

## Decisions

- **Target frameworks: keep `net10.0-android` and `net10.0-ios`** on every library, alongside
  `net10.0` and `net10.0-windows10.0.19041`, matching Uno.Themes 9.0. Skia Android and Skia iOS
  are first-class. `net10.0-maccatalyst` is gone.
- **The mobile builds behave like the plain Skia build.** In Uno 6, a Skia Android/iOS head did not
  load a library's `-android`/`-ios` asset: the SDK's `RuntimeAssetsSelectorTask` swapped in the
  plain `netX.0` build. Toolkit users on Skia mobile have therefore always run the plain build —
  `not_mobile` XAML, no `__ANDROID__`/`__IOS__` code, the Skia `ExtendedSplashScreen` path. Uno 7
  (`bc6254848a`, "Keep platform-specific assets on Skia mobile") stops the swap, so the mobile
  assets now run. Native-renderer-era code is deleted and the plain path becomes what the mobile
  targets compile. A native-only behavior survives only as a justified OS-level exception, and
  each one is listed below.
- `ExtendedSplashScreen` on Skia Android and Skia iOS uses the Skia (`.cross.cs`) implementation.
  `ExtendedSplashScreen.Init(Activity)` keeps its signature.

## Done

- [x] Rebase onto `main` (picks up `cab35f0b` 11.0-dev versioning, `572ffc3f` Themes typography).
- [x] `chore(deps)`: `Uno.Sdk.Private` and `Uno.WinUI.DevServer` → `7.0.0-dev.701`;
      `Uno.Themes` → `9.0.0-dev.15`, the first Themes line built against Uno 7. Clears every
      `CS0012` (`CoreDispatcher`, `Color` in `Uno`) on the desktop and WASM sample heads.
- [x] `fix(ci)`: pin the .NET SDK to `10.0.101`, the band `uno.check` 1.34.1 provisions
      `wasm-tools` for. On `10.0.102` the Packages job failed `NETSDK1147` on every
      `net10.0-ios` project (same fix as Uno.Themes `87bfb588`).
- [x] `fix(skia)`: `ShadowContainer` renders through `SKCanvasElement` on every Uno target and keeps
      `SKXamlCanvas` for WinAppSDK only. `SkiaSharp.Views.Uno.WinUI` (4.151.1 and 4.152.0) is
      compiled against Uno 5 and binds `Uno.dll`/`Uno.UI.Toolkit`, so the reference is gone.
- [x] `fix(styles)`: split the `toolkit` xmlns (`toolkit` = `Uno.UI.Xaml.Controls`,
      `unoxaml` = `Uno.UI.Xaml`).
- [x] `fix`: `IDependencyObjectStoreProvider` → `Uno.UI.Helpers.MarkupHelper.SetParent` (BC26).
- [x] Commit messages carry `BREAKING CHANGE:` bodies so commitsar passes.

### Verified

- Material desktop sample publishes with **0 errors** via the CI command.
- Every published assembly binds only Uno 7 assemblies (checked with a metadata reader).
- Desktop runtime-test host starts; all 20 `ShadowContainerTests` pass on Skia desktop (Windows).
- Full desktop suite on Windows: 255 passed, 86 failed, 11 skipped of 341. Most failures are
  sub-pixel `AutoLayout` assertions (`24.8` vs `25`) consistent with a non-100 % display scale;
  CI runs under `xvfb` at scale 1, so re-baseline on Linux before reading them as regressions.

## In progress — mobile port

Split into four slices by file ownership, each in its own worktree, reviewed adversarially before
integration:

- [ ] **NavigationBar** — delete native renderers, `NativeFramePresenter`,
      `NativeNavigationBarPresenter`, the native templates/keys and the `IsNativeStyle` frame styles
      (UI, Material v1/v2, Simple); port `NavigationBarTests`; update `doc/controls/NavigationBar.md`.
- [ ] **TabBar and helpers** — managed `TabBarSelectorBehaviorState` everywhere, managed
      `ScrollableHelper` scroll-to-top (the mobile branch threw `NotImplementedException`),
      collapse `VisualTreeHelperEx.Native`, drop the `CS0109` `new` members, unwrap the TabBar, Chip,
      segmented-control and FlipView XAML forks.
- [ ] **Platform services** — `ExtendedSplashScreen` Skia path on mobile, `SafeArea` without
      `IsStatusBarTranslucent`, remaining `ShadowContainer` mobile gates, their tests and docs.
- [ ] **Samples, build and CI** — Android host through `UnoPlatformHostBuilder`, Android API 24 floor
      for app heads, dead native sample code and XAML prefixes, remaining test gates, the upstream
      `CS0407` generator issue, CI stage review.
- [ ] Integrate, then build every leg with the CI commands (Packages via MSBuild, desktop, WASM,
      Android, iOS) and run runtime tests on desktop, WASM and a local Android emulator.

## Blockers and risks

| Item | State |
|---|---|
| Uno.Themes Uno 7 build | **Resolved** — `9.0.0-dev.15` |
| `Uno.WinUI.Markup` Uno 7 build | Exists (`7.0.0-dev.33`); the SDK still pins `6.7.0-dev.16`, so the Markup projects need `UnoCSharpMarkupVersion`. `Uno.Themes.WinUI.Markup` 9.0 still references `Uno.WinUI.Markup` 5.2 — check at runtime |
| SkiaSharp views built for Uno 7 | None exist; avoided by `SKCanvasElement` |
| `CS0407` in generated `mergedpages_*.cs` on mobile (seen in Uno.Themes for Simple) | Under investigation |
| Mobile runtime tests | CI runs desktop and WASM only; Android is verified locally |

## Public API removals to announce

- `NativeFramePresenter` and `NativeNavigationBarPresenter` (mobile-only types).
- Resource keys `NativeDefaultToolkitFrame`, `NativeNavigationBarTemplate`,
  `MaterialNativeNavigationBarTemplate`, `SimpleNativeNavigationBarTemplate`, and the implicit
  native `Frame` styles.
- `net10.0-maccatalyst`.

Behavioral, no signature change: `NavigationBar` on Android/iOS no longer renders a native
`Toolbar`/`UINavigationBar`, and the mobile assets now match what Skia mobile heads already ran.
