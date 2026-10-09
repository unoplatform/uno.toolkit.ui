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
- `FeedView.Source` swaps leave the view in the `Some` visual state with cleared data (only changed axes drive visual states). Resolved in the style, not FeedView: the initial skeleton follows the FeedView's `ILoadable` state (true during the initial load and after a Source change) instead of the data visual state

## Phase 5 — documentation and samples

- [x] `doc/controls/SkeletonView.md` rewritten: generation rules, per-type property reference (`SkeletonView`, `SkeletonPresenter`, `Skeleton` attached properties, `SkeletonShape`) with defaults, one section per scenario, lightweight styling, limitations/troubleshooting
- [x] Sample page: one section per `SkeletonView`/`SkeletonPresenter` scenario (live content, ILoadable, overrides incl. `Shape="Rectangle"`, appearance, `SkeletonPresenter`, `SkeletonPresenter` in `LoadingView`)
- [x] uno.extensions `doc/Learn/Mvux/FeedView.md`: "Skeleton loading" section for `SkeletonFeedViewStyle`

## Phase 6 — Ignore keeps elements visible

- [x] While loading, only placeholder-covered elements are hidden (`Opacity` 0, original local value/binding restored once loaded) instead of the whole content presenter; ignored elements and container backgrounds stay visible. Template no longer sets `PART_ContentPresenter.Opacity` (still not hit-testable)
- [x] Tests: `When_Ignore_Set_Element_Stays_Visible`, `When_Loaded_Original_Opacity_Restored`; `When_IsLoading_Toggles` updated to assert per-element hiding (19/19 pass)

## Phase 7 — empty lists inside SkeletonView (stand-in rows)

Goal: `SkeletonPresenter` + a duplicated `DataTemplate` is no longer needed for list skeletons. A `SkeletonView` around any content (including a nested, empty `ListView`) produces placeholder rows for opted-in lists, without touching the app's list data or visual tree.

Design:

- Opt-in: an `ItemsControl`/`ItemsRepeater` in the content with an `ItemTemplate`, **no items**, and a *local* `Skeleton.PlaceholderCount`.
- While loading, a private stand-in list (same kind: `ListView`/`GridView`/`ItemsControl`/`ItemsRepeater`, sharing `ItemTemplate`, `ItemContainerStyle`, `ItemsPanel`/`Layout`, `Style`, `Padding`) filled with `PlaceholderCount` null items is hosted in an invisible canvas inside the overlay, positioned over the real list at its width. Placeholders are collected from the stand-in instead of the real list (single shimmer storyboard), clipped to the real list's bounds.
- Space: when the real list is sized by its content (not stretched into extra space, no explicit `Height`), its `MinHeight` is temporarily raised to the stand-in's height, so surrounding content lays out as if the rows existed. Otherwise (star row, fixed height) the rows fill its bounds and are clipped. `MinHeight` is restored (local value or binding) when loading ends or real items arrive.
- No reparenting, no `ItemsSource` swap, no template change (the stand-in host is created in code, only while needed).
- Limitations: list `Header`/`Footer` are not mirrored (rows start at the list's top); a list with no width (e.g. unconstrained in a horizontal `StackPanel`) gets no stand-in.

Tasks:

- [x] Red tests (`SkeletonListPlaceholderTests`, 7): nested empty `ListView` in a `Grid` → rows + content below pushed down; constrained list → rows clipped; items arriving while loading → real rows mirrored, `MinHeight` released; original `MinHeight` restored once loaded; no `PlaceholderCount` → no rows; `PlaceholderCount` on an ancestor; `ItemsRepeater`. 6 failed before the fix (the opt-out guard passed)
- [x] Implement in `SkeletonView` (`SkeletonView.ListStandIns.cs`); `PlaceholderCount` lookup shared with `SkeletonPresenter` (`Skeleton.TryGetInheritedPlaceholderCount`, local values only, metadata default never applies in a `SkeletonView`)
- [x] Sample section 5 → wrapped empty `ListView` in a `Grid`; section 6 (`LoadingView`) keeps `SkeletonPresenter`. Docs: "Empty lists" section, generation rule 7, `PlaceholderCount` row, troubleshooting
- [x] Release zero-warning (library, runtime tests; the 2 `UXAML0007` warnings come from the `Uno.UI.RuntimeTests.Engine` package); 26/26 skeleton runtime tests pass (Material desktop)

Review:

- Implementation choices: rows are collected from the stand-in instead of the list (one shimmer storyboard). The stand-in host lives in the overlay only while needed and survives `ClearOverlay`. Rows are clipped to the list's layout slot. `MinHeight` is raised only when the list's slot ≈ its desired height, so stretched lists aren't resized.
- Hot path: `GenerateOverlay` now walks the content twice, once to find empty lists and once to collect placeholders, even when there are no lists. It's still O(n), and only runs while loading. Stand-in lists are disabled, so they never take focus.
- While stand-ins exist, the `LayoutUpdated` retry stays hooked rather than subscribing to the lists' collection events, because realized rows or arriving items always trigger a layout pass. The cost is one content walk per layout pass while loading with an empty list; there's no per-list subscription, so nothing to leak.
- `SkeletonFeedViewStyle` (uno.extensions) keeps `InitialSkeleton`. Dropping it would mean showing `SomePresenter` in the `Undefined` state, with `IsLoading` driven both by the FeedView's `ILoadable` and by the `Indeterminate` setter. It would also lose the default of 4 rows: inside a `SkeletonView`, rows are opt-in, and the style can't give `PlaceholderCount` a default without shadowing the FeedView's own value (attached properties can't be template-bound). Apps that set `PlaceholderCount` on the FeedView also get rows when refreshing an empty list.

## Notes for future work

- Headless runtime tests locally need **both** `UNO_RUNTIME_TESTS_RUN_TESTS` and `UNO_RUNTIME_TESTS_OUTPUT_PATH` env vars in addition to `--runtime-tests=<path>`; with only the filter var set, the app idles at the runner UI and never starts.
- Placeholder regeneration is triggered by `SizeChanged` of both the ContentPresenter *and* the overlay Canvas — the overlay is collapsed while not loading, so only its own 0→W size change reliably signals "visible and arranged" when loading restarts.
- `LeakTest` is fully `[Ignore]`d (#429), so no SkeletonView row was added there.
