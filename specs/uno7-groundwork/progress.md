# Uno Platform 7.0 groundwork

Tracking branch: `dev/mazi/uno7-groundwork` — PR [#1635](https://github.com/unoplatform/uno.toolkit.ui/pull/1635).

Uno 7.0 removes the native iOS/UIKit and Android view-rendering layers (`UIElement` no longer
derives from `UIView` / `Android.Views.View`), renames `Uno.UI.Toolkit` to `Uno.UI.Extras` and
re-namespaces its types, turns `DependencyObject` into a class, and drops the native style
pipeline. This document tracks what that costs the toolkit.

## Method note

The Uno checkout at `D:\Work\uno` is **stale** relative to the shipped package. `uno.winui.nuspec`
for `7.0.0-dev.679` pins commit `d1710cafb14428664bc3eadd5bd6d4ee49cc7395`. Grepping the working
tree gives wrong answers — for example it still contains `Generic.Native.xaml` and
`IDependencyObjectStoreProvider`, neither of which is in the shipped assemblies. Verify against
`git show d1710ca:<path>` or against `D:\Packages\NuGet\uno.winui\7.0.0-dev.679`.

## Done

- [x] Rebase onto `main` (picks up `d88a0c12`, servicing/* publishing + `10.0-dev`).
- [x] `chore(deps)`: `Uno.Sdk.Private` and `Uno.WinUI.DevServer` → `7.0.0-dev.679`.
      Restores `GetBindingExpression`, clearing 4 CS1061 errors. Introduces no new errors.
- [x] `fix(styles)`: split the `toolkit` xmlns. BC53 turned one namespace into two but both
      halves kept the `toolkit` prefix; `Uno.XamlMerge.Task` requires one prefix → one namespace.
      Now `toolkit` = `Uno.UI.Xaml.Controls`, `unoxaml` = `Uno.UI.Xaml`.
- [x] `fix`: `IDependencyObjectStoreProvider` → `Uno.UI.Helpers.MarkupHelper.SetParent`.
      BC26 removed both `IDependencyObjectStoreProvider` and `DependencyObjectStore`.
- [x] `fix(skia)`: explicit `SkiaSharp.Views.Uno.WinUI` reference — see the caveat below.
- [x] Commit messages: `BREAKING CHANGE:` bodies added so commitsar passes. The BC26 commit was
      demoted from `fix!` to `fix` — widening `where T : class, DependencyObject` to
      `where T : DependencyObject` is source- and binary-compatible, so it is not breaking.

### Verified green

The Material desktop sample **publishes with 0 errors** via the exact CI command
(`dotnet publish -c Release -f net10.0-desktop -p:TargetFrameworkOverride=desktop`), including
`Uno.Toolkit`, `Uno.Toolkit.WinUI`, `Uno.Toolkit.Skia.WinUI`, `Uno.Toolkit.WinUI.Material` and
`Uno.Toolkit.RuntimeTests` — **but only** when `Uno.Themes.WinUI` is a build retargeted to Uno 7.

## Blockers

### 1. Cross-repo dependencies (critical path)

All 20 `UNOB0020` warnings name Uno-6-built *dependencies*, not toolkit code.

| Package | State | Action |
|---|---|---|
| `Uno.Themes.WinUI` / `.Material` / `.Cupertino` / `.Simple` `7.1.0-dev.1` | Uno 6 (`lib/net9.0-android35.0`, binds `Uno.dll`, `Uno.Foundation`, `Uno.UI.Toolkit`) | Retarget PR open: [uno.themes#1722](https://github.com/unoplatform/Uno.Themes/pull/1722) — all four desktop sample heads publish clean locally. Bump `UnoThemesVersion` once it merges and publishes a dev package. |
| `Uno.WinUI.Markup`, `Uno.Extensions.Markup.WinUI` `6.7.0-dev.16` | **No Uno 7 build exists** | Gates `Uno.Toolkit.WinUI.Markup` and `.Material.Markup` entirely. Not a port task — needs the owning repo. Raise now; this may be the real critical path. |

### 2. `ShadowContainer` / SkiaSharp — the committed fix is not sufficient

`SkiaSharp.Views.Uno.WinUI` `4.151.1` (the version the Uno SDK pins) is **itself Uno-6-built** —
it binds `Uno`, `Uno.Foundation` and `Uno.UI.Toolkit`. Referencing it makes the toolkit *compile*,
but the app then dies at startup:

```
System.IO.FileNotFoundException: Could not load file or assembly 'Uno.UI.Toolkit, Version=255.255.255.255'
   at SkiaSharp.Views.Windows.GlobalStaticResources.Initialize()
   at Uno.Toolkit.RuntimeTests.GlobalStaticResources..cctor()
```

Reproduced by running the published desktop sample with `--runtime-tests=`: the runtime-test host
crashes before a single test executes, so the Runtime Tests CI job would still fail.

The Uno 7 route for drawing Skia content is `Uno.WinUI.Graphics2DSK` (`SKCanvasElement`) — which is
what the retargeted `Uno.Themes` already references. **`ShadowContainer` should be ported off
`SKXamlCanvas` onto `SKCanvasElement`.** Until then the `SkiaSharp.Views.Uno.WinUI` reference stays
as a compile-time unblock only.

### 3. Mobile (iOS + Android) — needs a product decision

Every remaining error is on the `net10.0-ios` / `net10.0-android` legs. `net10.0` is clean and
`net10.0-windows` fails only on `UNOB0008`, which is a local `dotnet build` vs `msbuild` artifact,
not a CI failure.

Error accounting (from a clean Release build; the raw counts are misleading):

| Code | Count | Reality |
|---|---|---|
| CS0234 | 112 | Only **14** are `AndroidX.*`. 86 are `Uno.Toolkit.UI.*` cascade from the base library's android leg failing. |
| CS0246 | 22 | `FlipViewSource`, `Toolbar`, `ViewPager`, `SKPaintSurfaceEventArgs` |
| UXAML0001 | 20 | `Style.IsNativeStyle` + cascade |
| CS0115 | 12 | UIKit overrides (`MovedToSuperview`, `Frame`, …) |
| CS0109 | 10 | redundant `new` on `Card`/`Chip`/`TabBarItem`/`ExtendedSplashScreen` members |

Corrections to earlier assumptions, both verified:

- **Mobile TFMs are still first-class in Uno 7.** `Uno.WinUI` ships no mobile `lib/` asset, but it
  *does* ship `buildTransitive/net10.0-android36.0`, `net10.0-ios26.0`, `net10.0-tvos26.0`.
  Dropping mobile TFMs is a choice, not a forced consequence.
- **`Uno.UI.DrawableHelper` moved, it was not removed** — it is now
  `Uno.Helpers.DrawableHelper` in the `Uno.WinRT` assembly.

`net10.0-maccatalyst` should go regardless: Uno 7 ships no MacCatalyst target and the package has
no `maccatalyst` `buildTransitive` folder.

## Plan

Ordered deliberately — deleting sources *before* touching TFMs makes the compiler prove what is
dead, instead of a TFM removal hiding it.

- [ ] **0. Unblock dependencies.** Land [uno.themes#1722](https://github.com/unoplatform/Uno.Themes/pull/1722),
      bump `UnoThemesVersion`. Raise the `Uno.WinUI.Markup` / `Uno.Extensions.Markup` gap.
- [ ] **1. Port `ShadowContainer`** from `SKXamlCanvas` to `Uno.WinUI.Graphics2DSK`'s
      `SKCanvasElement`, then drop the `SkiaSharp.Views.Uno.WinUI` reference.
- [ ] **2. Delete the native rendering code** with the mobile TFMs still in place:
      `Controls/NavigationBar/{AppBarButtonRenderer,NavigationAppBarButtonRenderer,NavigationBarRenderer,NavigationBarNavigationItemRenderer,NavigationBarHelper,NativeNavigationBarPresenter,NativeFramePresenter}.*`
      (including the **shared** `NativeNavigationBarPresenter.cs`) and the forked `Renderer.cs`;
      `Extensions/{ColorExtensions,UIBarButtonItemExtensions}.iOS.cs`;
      `Helpers/{DictionaryExtensions,ImageHelper}.iOS.cs`;
      `Behaviors/TabBarSelectorBehaviorState.{Android,iOS}.cs`.
      Strip the markup they served (`NavigationBar.xaml` `mobile:` arms, `NativeDefaultToolkitFrame`,
      both `IsNativeStyle="True"` styles, and the `mobile:` arms in Material v1/v2 and Simple).
      Delete the CS0109 `#if __ANDROID__ new` sites explicitly rather than letting a TFM drop hide them.
      Collapse `VisualTreeHelperEx.Native` and `ScrollableHelper` — Uno 7 aliases
      `_View = UIElement`, so the "native tree isn't crawlable" premise is dead.
- [ ] **3. Port the runtime tests before touching TFMs.** ≥21 `[TestMethod]`s sit behind
      `__ANDROID__`/`__IOS__` (`NavigationBarTests`, `SafeAreaTests`, the leak tests,
      `ChipGroupTests`, `AutoLayoutTest`, `DrawerTests`, `LeakTest`). Re-express them against the
      managed presenter — AGENTS.md §5 forbids losing coverage to a refactor.
- [ ] **4. Decide the TFM set** — see below.
- [ ] **5. Sweep remaining BCs**: BC38 (`FrameworkElement.Background` → `Control`),
      BC58 (`DataContext` is FE-only), BC65 (`FrameworkElement` is no longer `IEnumerable`),
      BC01 (`Microsoft.UI.Xaml.{NewFrameworkTemplateBuilder,TemplateMaterializationSettings}` →
      `Uno.UI.*`).
- [ ] **6. XAML prefix hygiene.** The conditional vocabulary is now `android`, `ios`, `tvos`,
      `desktop`, `wasm`, `winappsdk`/`win` and `not_` forms. `skia`, `macos`, `netstdref`,
      `xamarin`, `not_mux`, `legacy` are gone. `win`/`not_win` are now conditional on the WinAppSDK
      target, so re-verify the four files that actually use them.
- [ ] **7. Docs** (AGENTS.md §13): `doc/controls/NavigationBar.md` documents native mode across
      ~600 lines; `doc/controls/ExtendedSplashScreen.md:125-131` documents `Init(Activity)`.
- [ ] **8. Re-baseline visuals** on the Skia Android/iOS heads — HarfBuzz text metrics, managed
      virtualization and Skia animation all differ.

### Open decision — TFM set (step 4)

**(a) Platform-neutral** — `net10.0` + `net10.0-windows10.0.19041`, matching `Uno.UI.Extras` and
the retargeted `Uno.Themes`. Cost: `ExtendedSplashScreen.{Android,iOS,macOS}.cs` and the
`AndroidResource` `Resources/values/Styles.xml` disappear, so **`ExtendedSplashScreen.Init(Activity)`
is removed** (public, documented). Android splash falls back to the Skia path. Also the library can
never gate XAML per-runtime again — a `net10.0` build bakes in `not_android`/`not_ios`/… permanently.

**(b) Keep `net10.0-android` / `net10.0-ios` on `Uno.Toolkit.WinUI` only**, purely to host the
platform services, via a per-project override — *not* by editing `src/tfm-common-winui.props`,
which **eight** projects import (including `Uno.Toolkit.RuntimeTests`). Then add the missing
`Xamarin.AndroidX.AppCompat` / `.Core` / `.ViewPager` references (`.Core.SplashScreen` is already
there) and repoint `DrawableHelper` at `Uno.Helpers`.

Either way, drop `net10.0-maccatalyst`.

## Public API removals to announce

- `Uno.Toolkit.UI.NativeFramePresenter` — mobile-only, no shared partial; the type ceases to exist.
- `Uno.Toolkit.UI.NativeNavigationBarPresenter` — internal in the shared partial, raised to public
  only by the platform partials.
- The `NativeDefaultToolkitFrame` resource key and both `IsNativeStyle` Frame styles — a consumer
  doing `BasedOn="{StaticResource NativeDefaultToolkitFrame}"` breaks.
- Under (a): `ExtendedSplashScreen.Init(Android.App.Activity)` and the per-platform `SplashIsEnabled`.
- `net10.0-maccatalyst`.

Behavioural, no signature change: `NavigationBar` on Android/iOS stops rendering as a native
`Toolbar`/`UINavigationBar`. `TabBarSelectorBehaviorState` is internal — no API impact.

## CI environment issues (not code)

- **Packages job never compiles.** It dies at `NETSDK1147: wasm-tools must be installed` on the
  windows-2022 agent; `build/workflow/templates/dotnet-workload-install-windows.yml` does not
  install it. Even a perfect tree fails this job.
- **`dotnet build` cannot build the WinAppSDK leg** (`UNOB0008`) — CI uses `MSBuild@1`, so this is
  a local-only artifact. The `net10.0-windows` leg is therefore unverified locally.

## Not verified

- iOS, Android, macOS and WASM heads were not built locally.
- Runtime tests have never completed — the host crashes at startup (blocker 2). No test result
  has been observed on this branch.
