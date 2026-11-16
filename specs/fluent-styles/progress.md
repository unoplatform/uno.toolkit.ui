# Fluent default styles: NavigationBar, TabBar, Chip

Umbrella #1325, scoped for 7.1 to #1326 (NavigationBar), #1327 (TabBar), #1328 (Chip).
CardContentControl (#1329) is out of scope.

Goal: the default styles (no design-system package, plain WinUI Fluent theme) look and behave
like WinUI 3 Fluent controls. Style-only: no new control features, no public C# API changes.

## Verified facts (2026-11-16)

Issue text was checked against code on `main` (`b587c557`) and against
`microsoft/microsoft-ui-xaml` `main`, `CommunityToolkit/Windows` `main`, and `unoplatform/uno`
`release/stable/7.0`.

| Claim in the issues | Verdict | Evidence |
| --- | --- | --- |
| NavigationBar has native renderers on Android/iOS | Stale | Removed in `1f421f67`; no `__ANDROID__`/`__IOS__` path left. One managed CommandBar template on every target. |
| "Default styles depend heavily on Material" | Partly | Chip default is a copy of the Material v2 template without ripple. TabBar uses legacy UWP `SystemControl*` brushes. NavigationBar already uses Fluent brushes but has gaps. None reference Material keys. |
| "WinUI now ships a segmented-control equivalent" | Partly | No `Segmented` type in WinUI. The Segmented spec shipped as `SelectorBar` (WinAppSDK 1.5): underline pill, not a boxed control. The boxed look exists only in Community Toolkit `Segmented`. |
| There is a Pill Button to copy | Figma only | No chip/pill control or style in WinUI source (#8564 closed). The reference is the Pill in the Windows UI Kit Figma file (image in #1328). Its fills and text measure out to the `ToggleButton` tokens per state; its border is a flat `ControlStrokeColorDefault` stroke, absent when selected. |
| Fluent resources are available in Uno | Yes, one gap | Key sets match WinUI except `AppBarButtonLabelFontSize`, missing in Uno. Do not reference it. |
| Docs list the lightweight-styling keys | Wrong for defaults | All three doc tables show Material values. NavigationBar's table also disagrees with the Material code (e.g. height 48 vs 64). |

Other findings that shape the work:

- Chip: `DefaultChipStyle` is not implicit and is referenced nowhere, so a bare `utu:Chip` gets
  the `ToggleButton` template. It uses `NullToCollapsedConverter` / `FalseToCollapsedConverter`,
  which the base toolkit does not define (Uno.Themes does). To confirm at runtime: the style
  cannot resolve in a Fluent-only app.
- TabBar: `DefaultTabBarStyle` is the base of the Material, Simple, and Cupertino sliding-segmented
  styles, so setter changes cascade. 14 `TabBarItem*` keys are exposed through the Markup API.
- NavigationBar: local two-way bindings on the inner CommandBar beat its style setters, so setters
  belong on the `NavigationBar` style.
- No sample head runs without a design-system package. A Fluent gallery section renders inside a
  Material/Simple/Cupertino app, where same-named keys override the defaults.

## Design

Every colour goes through a per-control lightweight key with `Default`, `Light`, and
`HighContrast` dictionaries, as WinUI does. Existing key names stay; new keys are additive.

### Chip (#1328), model: `DefaultToggleButtonStyle`, pill-shaped

- Brushes per state from the `ToggleButton*` mapping (accent fill when checked), including
  CheckedDisabled, Indeterminate, and focus visuals.
- Metrics: height 32, full corner radius, 1px border, 14px text, 16px icon, remove button with
  hover/pressed states. No `ElevatedView`, no state overlay, no character spacing.
- Visibility without external converters.
- Keep `PART_RemoveButton`, `ContentPresenter`, and the `Icon`/`IconTemplate`/`CanRemove` bindings.
- ChipGroup default: horizontal panel, spacing 8.

### TabBar (#1327)

- Top: model `SelectorBar` (transparent items, text states, 3px accent pill indicator under the
  selected item). Uses the existing selection-indicator feature.
- Bottom and Vertical: model NavigationView items (subtle fill on hover/pressed, accent indicator,
  icon above label for bottom).
- New keyed styles; `Base*` styles stay neutral so Material/Simple/Cupertino are unaffected.
- Replace legacy `SystemControl*` values with Fluent tokens under the same 14 keys.
- Keep `TabBarListPanel`, both indicator presenters, all state names, `TabBarHeightOrWidth`.

### NavigationBar (#1326), model: `DefaultCommandBarStyle` + `NavigationBackButtonNormalStyle`

- Height 48, transparent background, title in `SubtitleTextBlockStyle` metrics (20 SemiBold).
- Back button 40x36 with glyph E72B and Subtle fill states; a real icon on WinAppSDK too.
- Primary commands use the stock `DefaultAppBarButtonStyle` metrics; overflow uses the stock flyout look.
- HighContrast dictionary; remove dead setters and stale comments.
- Keep `NavigationBarPresenter`, `XamlNavigationBarCommandBar`, `ContentControl`, the CommandBar
  parts, and the column order `UpdateHeaderContentAlignment` relies on.

### Shared

- Samples: a `FluentTemplate` section on the Chip, TabBar, and NavigationBar pages showing the
  default styles; list `Design.Fluent` in `SupportedDesigns`.
- Docs: split each control page's styling and lightweight-styling sections into Default (Fluent)
  and Material/Simple; add default keys to `controls-styles.md`.

## QA at each step

For each control, in order:

1. Baseline: screenshot the current default style, Light and Dark, in a Fluent-only host (a small
   scratch app with only `XamlControlsResources` + `ToolkitResources`, not committed).
2. Red: add runtime tests that fail on `main` (template applies without a design package, parts
   present, key brushes resolve per theme, key metrics).
3. Implement, then green. Run the control's existing test classes and hot-reload tests.
4. Visual check against the WinUI model in Light, Dark, HighContrast; rest, hover, pressed,
   selected, disabled, focus. Compare with the baseline.
5. Regression: Material, Simple, and Cupertino sample pages for that control look unchanged.
6. Release build of the changed libraries with zero warnings; restore `Generated/`.
7. Reviewer pass (quality, contract, skeptic) on the diff before moving to the next control.

## Plan

- [x] Read the issues, audit the three default styles, verify WinUI and Uno sources.
- [x] Plan check-in (decisions below).
- [x] Fluent-only QA host and baseline screenshots.
- [ ] Chip: tests, style, sample, docs, QA (implemented; see Chip status).
- [ ] TabBar: tests, style, sample, docs, QA.
- [ ] NavigationBar: tests, style, sample, docs, QA.
- [ ] Final cross-control review and PR(s).

## Decisions (2026-11-16)

- Chip and ChipGroup default styles become implicit.
- One PR per control, in the order Chip, TabBar, NavigationBar.
- Bugs found outside styling get their own issues and PRs.
- Top TabBar: build a visual comparison of the SelectorBar look and the boxed segmented look; possibly ship one as default and the other as an alternate style.

## Chip status

Done:

- `DefaultChipStyle` rewritten as a pill-shaped WinUI `ToggleButton`: fills and text alias the `ToggleButton*` resources in Default, Light, and HighContrast; borders are a flat stroke, absent when checked, as in the Windows UI Kit Pill; all Checked and Indeterminate states; system focus visual.
- Icon visibility moved to an `IconStates` visual-state group set by `Chip` (the old converter never resolved without Uno.Themes, so icons never showed).
- Implicit `Chip` and `ChipGroup` styles; Material and Simple implicit Chip styles still win when merged after the toolkit defaults.
- `ChipDefaultStyleTests`: 10 of the first 16 cases failed before the change; 21 pass after. `ChipGroupTests` (21), `ItemsRepeaterChipTests` (15), `ThemeCompatibilityTests` (7) pass on the Material desktop head.
- Release desktop build of the base, Material, and Simple libraries: zero warnings.
- Fluent section on the Chip sample page; docs split into Default (Fluent) and Material.
- Contract and skeptic reviews run; findings applied (ThemeResource metrics, no negative margin, stronger tests).

Open:

- Material apps: Material has no implicit `ChipGroup` style, so a bare `ChipGroup` there now gets the toolkit's horizontal panel instead of the vertical `ItemsControl` default. Needs a decision: keep, or add a Material implicit ChipGroup style.
- Visual check done in a Fluent-only host, Light and Dark, against the Windows UI Kit Pill. The keyboard focus ring does not appear in captured screenshots, so focus appearance is unverified.
- Not run: hot-reload tests (skipped locally), WebAssembly, Android, iOS, WinAppSDK builds.
- PR must list the removed default keys (`Chip*Focused`, `ChipStateOverlay*`, `ChipElevation*`, `ChipContentMinHeight`, `ChipSize`), the re-valued metrics, and that `Elevation` has no effect in the default style.
- The remove button has no automation name here; the accessibility branch sets it in code.

## Out of scope, to file separately

- NavigationBar `ClosedDisplayMode`, `OverflowButtonVisibility`, `DefaultLabelPosition` never
  reach the inner CommandBar (`NavigationBarPresenter.SetBindings`).
- TabBarItem `Disabled` state sits in its own group and the code never returns to `Enabled`;
  the disabled look may persist after re-enabling (not yet reproduced).
- TabBarItem badge has no default-template support.
- Dead code: `NavigationBarTemplateSettings`, `TabBarTemplateSettings.SelectionIndicatorMaxSize`.
