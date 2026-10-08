# SkeletonView — auto-generated placeholders

## Context

Revival of `dev/sb/skeleton` (originally a `SkeletonView` + manually-composed `Skeleton` shapes). Redesigned so no skeleton shapes are ever declared: `SkeletonView` wraps the real content and derives placeholders from the actual layout of its leaf elements while loading.

## Plan

- [x] Rebase `dev/sb/skeleton` onto latest `main` (dropped unrelated `.vscode` changes)
- [x] Delete manual `Skeleton` control (`Skeleton.cs` / `Skeleton.xaml`)
- [x] Rework `SkeletonView`: walk content visual tree, one placeholder per leaf (`TextBlock`, `Image`, `Shape`, `ButtonBase`), bounds via `TransformToVisual`
- [x] Empty-value fallback: layout slot for width, `FontSize`-derived line height for empty text
- [x] `IsLoading` + `Source` (`ILoadable`, reusing `LoadableExtension.BindIsExecuting`)
- [x] Attached overrides: `SkeletonView.Ignore`, `SkeletonView.Shape` (`Auto`/`Rectangle`/`Circle`)
- [x] Shimmer: per-placeholder gradient bands aligned into one sweep, single code-built Storyboard (`TranslateTransform.X`, no Composition)
- [x] Sample page rework (`SkeletonViewSamplePage`)
- [x] Runtime tests (7) + docs (`doc/controls/SkeletonView.md`, `doc/controls-styles.md`)

## Review

- Runtime tests: 7/7 passed headless on `net10.0-desktop` (Material head, `UNO_RUNTIME_TESTS_RUN_TESTS` filter `SkeletonView`).
- Release builds zero-warning: `Uno.Toolkit.WinUI`, `.Material`, `.Cupertino`, `Uno.Toolkit.RuntimeTests` (desktop override).
- Known repo issues encountered (pre-existing, untouched): `src/Uno.Toolkit.sln` references a non-existent `Uno.Toolkit.UITest` project (full-sln build fails at restore); sample-head Release restore requires `Microsoft.NETCore.App.Runtime.Mono.win-x64 10.0.1` which is not on the configured feeds.
- Not covered by tests: shimmer animation visuals (band structure is asserted; sweep needs an eyeball in the sample app).

## Phase 2 — template-driven skeletons + FeedView injection

- [x] `Skeleton` static class: `Ignore`/`Shape` moved here from `SkeletonView`, plus `IsEnabled`, `PlaceholderTemplate`, `PlaceholderCount`
- [x] `Skeleton.IsEnabled` injects a `SkeletonDefaultProgressTemplate` resource as the target's `ProgressTemplate` (resolved by convention via reflection — no Uno.Extensions dependency; explicit user template never overwritten; disabling removes only what was injected)
- [x] `SkeletonPresenter : SkeletonView` — always-loading, derives its skeleton from a template (own `ContentTemplate` → owner's `Skeleton.PlaceholderTemplate` → owner's `ValueTemplate`); stamps `PlaceholderCount` dummy rows into empty `ItemsControl`/`ItemsRepeater` with an `ItemTemplate` (private instantiation only — never live content)
- [x] `SkeletonView`: `PrepareSkeletonContent` hook + LayoutUpdated retry while a loading pass yields zero placeholders (list containers realize without resizing the presenter/overlay)
- [x] Tests: 13/13 passed headless (`SkeletonViewTests` + `SkeletonInjectionTests` with a `FeedViewStub` mimicking FeedView's DP shape)
- [x] Release zero-warning (library + runtime tests, desktop); reflection helper carries an IL2070 suppression with graceful degradation (warning logged, nothing injected)

## Phase 3 — injection sample (removed)

- A FeedView-shaped `SampleFeedView` stand-in demoed `Skeleton.IsEnabled` in the sample app; removed at the user's request. The injection path stays covered by `SkeletonInjectionTests` (`FeedViewStub`), and FeedView itself is demoed by the uno.extensions Playground (`SkeletonFeedViewStyle`).

## Phase 4 — FeedView style integration fixes

Driven by `SkeletonFeedViewStyle` in uno.extensions (`dev/sb/skeleton-feedview`), tested in its Playground against local Toolkit packages.

- [x] Container chrome skipped: for a `ContentControl`, only its content is walked (`ContentTemplateRoot` → direct `UIElement` content → the template's `ContentPresenter`), so e.g. Material `ListViewItem`'s full-size `BorderBackground` no longer covers the row's placeholders — `When_ContentControl_Chrome_Skipped`, `When_ListView_Items_First_Loading_Pass_Skips_Item_Chrome`
- [x] `SkeletonPresenter` takes `PlaceholderCount` from the nearest ancestor setting it (attached properties can't be template-bound from a control template) — `When_PlaceholderCount_Set_On_Ancestor`
- [x] Unconstrained `SkeletonPresenter` (e.g. in a vertical `StackPanel`): placeholder rows are stamped before the overlay size check, since the empty list is what keeps it at 0 height — `When_Presenter_Has_No_Height_Constraint`
- [x] 17/17 skeleton runtime tests pass headless (Material desktop); Release zero-warning
- Out of scope: swapping `FeedView.Source` leaves the view in the `Some` visual state with cleared data (only changed axes drive visual states), so a source swap shows blank content instead of the initial-load skeleton

## Phase 5 — documentation and samples

- [x] `doc/controls/SkeletonView.md` rewritten: generation rules, per-type property reference (`SkeletonView`, `SkeletonPresenter`, `Skeleton` attached properties, `SkeletonShape`) with defaults, one section per scenario, lightweight styling, limitations/troubleshooting
- [x] Sample page: one section per `SkeletonView`/`SkeletonPresenter` scenario (live content, ILoadable, overrides incl. `Shape="Rectangle"`, appearance, `SkeletonPresenter`, `SkeletonPresenter` in `LoadingView`)
- [x] uno.extensions `doc/Learn/Mvux/FeedView.md`: "Skeleton loading" section for `SkeletonFeedViewStyle`

## Phase 6 — Ignore keeps elements visible

- [x] While loading, only placeholder-covered elements are hidden (`Opacity` 0, original local value/binding restored once loaded) instead of the whole content presenter; ignored elements and container backgrounds stay visible. Template no longer sets `PART_ContentPresenter.Opacity` (still not hit-testable)
- [x] Tests: `When_Ignore_Set_Element_Stays_Visible`, `When_Loaded_Original_Opacity_Restored`; `When_IsLoading_Toggles` updated to assert per-element hiding (19/19 pass)

## Notes for future work

- Headless runtime tests locally need **both** `UNO_RUNTIME_TESTS_RUN_TESTS` and `UNO_RUNTIME_TESTS_OUTPUT_PATH` env vars in addition to `--runtime-tests=<path>`; with only the filter var set, the app idles at the runner UI and never starts.
- Placeholder regeneration is triggered by `SizeChanged` of both the ContentPresenter *and* the overlay Canvas — the overlay is collapsed while not loading, so only its own 0→W size change reliably signals "visible and arranged" when loading restarts.
- `LeakTest` is fully `[Ignore]`d (#429), so no SkeletonView row was added there.
