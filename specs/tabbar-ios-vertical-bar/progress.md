# TabBar in the iOS vertical bar (iPhone Duo cover screen)

## Context

iOS 27.1 adds a *vertical bar* on some devices: a strip along one side of the screen that holds the status bar. On the iPhone Duo cover screen (simulator `iPhone Duo`, Xcode 27.1) it is 84 pt wide, on the right in portrait and on the left in landscape. Native tab bars and toolbar buttons move into it: a stock `UITabBarController` draws its tab bar as an icon-only vertical pill at the bottom of the strip, and Safari moves its back, bookmarks, new tab and tabs buttons there.

The strip is reported as a left or right safe-area inset. Measured on the cover screen with a Toolkit app (Uno.Sdk 7.1, Toolkit 11.0.1, Material, Skia renderer):

| Orientation | Window (pt) | Safe area L T R B | Size class H / V | `verticalBarEdge` | `{utu:Responsive}` layout |
|---|---|---|---|---|---|
| Portrait | 466 × 678 | 0 0 84 34 | Compact / Regular | Trailing | Narrow |
| Landscape | 678 × 466 | 84 0 0 34 | Compact / Compact | Leading | Normal |

The documented responsive TabBar recipe chooses by width only, so it shows the bottom TabBar in both orientations. `SafeArea` keeps the items out of the strip, but the strip stays empty and the bar takes 112 pt of the height.

Relevant iOS 27.1 APIs (bound in .NET for iOS 27.1):

- `UITraitCollection.VerticalBarEdge` (`Unspecified` / `Leading` / `Trailing`), plus `SystemTraitsAffectingVerticalBarEdge` for change registration.
- `UIViewController.PreferredVerticalBarBehavior` (`Automatic` / `Disabled`) to opt out. Uno's internal `RootViewController` doesn't override it, so apps can't opt out today. An experiment that injected the override at runtime restored the horizontal status bar and removed the side inset.

Building with the iOS 27.1 bindings defines `IOS27_1_OR_GREATER`. CI builds the iOS head with Xcode 26.1.1, where the symbol is not defined, so recipe code guarded by it compiles out there.

## Plan

- [x] How-to section in `doc/controls/walkthroughs/TabBar-and-TabBarItem.howto.md`: icon-only vertical `TabBar` inside the strip, driven by `VerticalBarEdge` and `SafeArea.WindowInsets`, plus a FAQ entry
- [x] Cross-link from the "Responsive TabBar Layout" section of `doc/controls/TabBarAndTabBarItem.md`
- [x] Validate the doc snippet unchanged in a fresh `dotnet new unoapp` on the iPhone Duo simulator (portrait, landscape, launch transition)
- [x] Sample: `M3MaterialIOSVerticalBarSampleNestedPage`, opened from the Material TabBar sample page ("Show iOS Vertical Bar Sample")
- [x] Build the Material sample head for iOS (27.1 bindings) and desktop
- [x] Run the sample on the iPhone Duo simulator

## Review

- Doc snippet, extracted from the markdown and pasted into a fresh `dotnet new unoapp` (Uno.Sdk 7.1, Toolkit 11.0.1, Material) with only `x:Class`, a namespace and a title added: builds with no warnings against the iOS 27.1 bindings. On the iPhone Duo cover screen the TabBar sits in the right strip in portrait and the left strip in landscape. A bottom `SelectedIndex="2"` shows the third tab selected in the strip. iPhone 18 Pro on iOS 27.0 keeps the bottom TabBar.
- No polling needed: `VisibleBoundsChanged` covers both the rotation and the launch transition, where the window first reports a top inset (`T=82 R=0`) and then the strip (`T=0 R=84`) without a size change.
- CA1416: `OperatingSystem.IsIOSVersionAtLeast(27, 1)` does not satisfy the analyzer for `UITraitCollection.VerticalBarEdge`, even combined with `!OperatingSystem.IsMacCatalyst()`, because `IsIOSVersionAtLeast` is also a Mac Catalyst guard and the API is annotated iOS-only. `OperatingSystem.IsOSPlatformVersionAtLeast("ios", 27, 1)` compiles clean, so the doc and sample use it.
- Material sample head builds for `net10.0-desktop` (vertical bar code compiled out) and `net10.0-ios` iossimulator-arm64 with the 27.1 bindings (code compiled in), no warnings from the changed files. Ran on the iPhone Duo simulator through a temporary, uncommitted launch hook: matches the doc in portrait and landscape, and the `NavigationBar` with `Left,Top,Right` insets stays out of the strip.
- XAML formatted with `XamlStyler.Console -c Settings.XamlStyler --indent-tabs true` (the root settings default to spaces, which would re-indent the existing tab-indented sample files).
- Not run: cspell and markdownlint (no Node on the validation machine); checked by hand for blank lines around lists and fences, trailing spaces and the final newline.

## Open questions

- Inner (unfolded) screen, 669 × 951 pt: not tested. The simulator can't be folded from the command line.
- Regular iPhone on iOS 27.1 (expected `Unspecified`): not tested. The iOS 27.1 simulator runtime only accepts the iPhone Duo device type.
- Whether apps linked against an older iOS SDK get the vertical bar at all.
- Opt-out: needs an Uno change so `RootViewController` can return `UIVerticalBarBehavior.Disabled`.
- A Toolkit helper (for example a responsive condition or attached property for the vertical bar) would remove the iOS-specific code from apps.
