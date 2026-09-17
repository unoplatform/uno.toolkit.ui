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

## Done — mobile port

Split into four slices by file ownership, each built in its own worktree and reviewed
adversarially before integration.

- [x] **NavigationBar** — the XAML template is the only path; native renderers,
      `NativeFramePresenter`, `NativeNavigationBarPresenter`, the native keys and the `IsNativeStyle`
      frame styles are gone. `NavigationBarTests` assert on `NavigationBarPresenter`/`CommandBar`.
      The size and elevation resources stay defined for compatibility.
- [x] **TabBar and helpers** — `TabBarSelectorBehaviorState` follows the FlipView `ScrollViewer`
      everywhere; `ScrollableHelper` scrolls the `ListView`'s `ScrollViewer` on every Uno target
      (new `ScrollableHelperTests`, `TabBarItemExtensionsTests`); the TabBar, Chip, segmented-control
      and FlipView styles use the arm Skia heads already ran.
- [x] **Platform services** — `ExtendedSplashScreen` builds the Skia splash from `UnoSplash.def` on
      Android and iOS. `SafeArea` uses `Bounds`/`VisibleBounds` everywhere; the Android bounds-race
      guard stays because Uno 7 still raises `VisibleBoundsChanged` before updating `Bounds`.
- [x] **Samples, build and CI** — Android heads go through `UnoPlatformHostBuilder`; Hot Design and
      App MCP are disabled in Debug heads (their pinned builds still bind Uno 6); macOS/Catalyst
      plumbing and dead XAML prefixes are gone. The API 24 floor already comes from the SDK.
- [x] `DrawerControl.IsOpen` set off the UI thread is applied on the UI thread; Uno 7 enforces thread
      access in composition like WinUI.
- [x] iOS trim analyzer (enabled by the iOS SDK for every project): `IL2122` fixed by qualifying the
      type name with `Uno.WinRT`; `IL2070`/`IL2075` suppressed with justification on name-based
      dependency-property lookups.
- [x] Markup projects build against C# Markup `7.0.0-dev.33` (`UnoCSharpMarkupVersion`).
- [x] The incremental-loading tests scroll to the current end: Uno 7's `ScrollViewer` keeps a
      `ChangeView` offset as intent (`unoplatform/uno@385b6146a9`), so a fixed 10,000 px offset kept
      loading batches.

### Kept native-only exception

`ExtendedSplashScreen.Init(Activity)` still calls AndroidX `InstallSplashScreen` and keeps the
pre-API-31 `ExtendedSplashScreenTheme`. It drives the OS splash screen, not the view tree.

### Verified

Local, Windows, with the pipeline commands:

| Leg | Result |
|---|---|
| Libraries and runtime tests, `net10.0-android` / `net10.0-ios` | 0 errors, no `UNOB0020` |
| Desktop publish (Material, Cupertino, Simple) | 0 errors |
| WASM publish (Material, Cupertino) | 0 errors |
| Android build (Material, Cupertino) | 0 errors |
| Packages (MSBuild, all TFMs, 8 packages) | 0 errors |
| Desktop runtime tests, Linux (WSLg), before the port | 334 of 341 passed |
| Desktop runtime tests, Linux (WSLg), after integration | 354 of 360 passed; the 6 failures were the incremental-loading tests, fixed and re-run with `Attempts: 1` |

On Windows the same suite reports sub-pixel `AutoLayout` failures (`24.8` vs `25`) caused by the
display scale; use Linux or a 100 % scale.

**Skia Android, API 34 emulator (local, Release, `Attempts: 1`):** 244 of 365 passed. Every one of
the 121 failures is an exact-size assertion broken by layout rounding at the emulator's 2.625
density (`100` vs `99.81`): 109 in `AutoLayoutTest`, 8 in `ShadowContainerTests`, 4 in
`TabBarTests.Verify_Indicator_Transitions`. The port-sensitive tests all pass: the
`ExtendedSplashScreen` smoke and splash tests, the `SafeArea` system-bar and bottom-inset tests, every
`NavigationBar` test, the re-enabled `ChipGroupTests` (#1300), `DrawerTests.IsOpen_FromNonUIThread`,
`ScrollableHelper`, `TabBarItemExtensions` and the incremental-loading tests.

The run needs an app-side autostart: the engine's embedded runner hooks `Console.CancelKeyPress`,
which throws `PlatformNotSupportedException` on Android. The local harness reused Uno.Extensions'
`MobileRuntimeTestsAutostart` (`UITEST_RUNTIME_AUTOSTART_RESULT_FILE` passed as an `am start` extra)
and is not committed.

### Not verified

- Skia iOS runtime tests, and on Android the visual splash handoff and the DEBUG-only leak and
  bounds-transition tests (the emulator run used Release).
- Exact-size runtime assertions fail at fractional scales (Windows display scale, Android density);
  they would need a tolerance before a mobile runtime-test lane is added.
- iOS app heads (need a Mac) and the WinAppSDK sample heads.
- Hot-reload runtime tests.

## Blockers and risks

| Item | State |
|---|---|
| Uno.Themes Uno 7 build | **Resolved** — `9.0.0-dev.15` |
| C# Markup Uno 7 build | **Resolved** — `7.0.0-dev.33`. `Uno.Themes.WinUI.Markup` 9.0 still references `Uno.WinUI.Markup` 5.2; check at runtime |
| SkiaSharp views built for Uno 7 | None exist; avoided by `SKCanvasElement` |
| `CS0407` in generated `mergedpages_*.cs` on mobile | **Resolved** upstream (`unoplatform/uno#24457`, in `7.0.0-dev.697`) |
| Hot Design / App MCP Uno 7 builds | None published; disabled in Debug sample heads |
| Mobile runtime tests | CI runs desktop and WASM only |
| Runtime-test engine retries | `Uno.UI.RuntimeTests.Engine` 2.0.0-dev.79 retries failures three times without reporting; run with `Attempts: 1` when validating |

## Follow-ups

- `uno#7393` is closed; the Material v1 bottom TabBar FAB workaround could return to its
  `RenderTransform` design after a visual check.
- The `NativeFrame` sample now only exercises `Frame` back-stack handling; rename it.
- `NavigationBar.Subtitle` is displayed nowhere since the Android renderer went.

## Public API removals to announce

- `NativeFramePresenter` and `NativeNavigationBarPresenter` (mobile-only types).
- Resource keys `NativeDefaultToolkitFrame`, `NativeNavigationBarTemplate`,
  `MaterialNativeNavigationBarTemplate`, `SimpleNativeNavigationBarTemplate`, and the implicit
  native `Frame` styles.
- `net10.0-maccatalyst`.

Behavioral, no signature change: `NavigationBar` on Android/iOS no longer renders a native
`Toolbar`/`UINavigationBar`, and the mobile assets now match what Skia mobile heads already ran.
