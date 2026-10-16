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
- The list is never moved to another parent, and there is no `ItemsSource` swap, no template change (the stand-in host is created in code, only while needed).
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

## Phase 8 — placeholder rows must not inherit the page's DataContext

Found while building a teaser app: placeholder items are `null`, so their containers inherited the outer DataContext. Rows bound to the page's model (`{Binding}` showed the model's `ToString()`, and `{Binding Name}` would show real data). A horizontal chip row rendered 2 wide bars instead of 4 chips. In `SkeletonPresenter`, the same leak hung the UI thread.

- [x] Red: `When_Empty_Horizontal_ItemsControl_Generates_One_Placeholder_Per_Item` (2 rects instead of 4), `When_Inherited_DataContext_Placeholder_Rows_Stay_Empty` (app hung)
- [x] Fix: the stand-in list (`SkeletonView.ListStandIns.cs`) and the lists `SkeletonPresenter` stamps get `DataContext = null`
- [x] Green: 28/28 skeleton runtime tests (Material desktop); Release library build has zero warnings

## Phase 9 — full review (2026-10-09)

Scope: `origin/main..HEAD` (11 commits, 17 files). Six targeted reviews (quality, skeptic, performance, contract, architecture, operability) plus two headless probes on the Material desktop head. Security review skipped: no I/O, auth or untrusted input in scope (reflection only on the app's own control types). Verdict: **not mergeable as is; no redesign needed** — four small, silent correctness fixes with red/green tests, then approve.

### Blocking (fix + regression test before merge)

1. **Nested `SkeletonView` leaves content permanently invisible** — `SkeletonView.cs:347-351` walks into an inner `SkeletonView` through the `ContentControl` branch, so both views capture/restore the same leaf's `Opacity`; the second capturer records the other's `0`. Executed (`SkeletonNestedProbeTests`, uncommitted): inner finishes first → OK; outer finishes first → text stays at `Opacity 0`. Fix: treat `SkeletonView` as a walk boundary in `CollectPlaceholder` and `FindPlaceholderLists` (`ListStandIns.cs:129`); document "nested skeletons own their subtree". Test: `When_Nested_SkeletonView_Outer_Finishes_First`.
2. **`Content`/`ContentTemplate` swap while loading never regenerates** — no `OnContentChanged`/`OnContentTemplateChanged` override; `SizeChanged` doesn't fire for a same-size swap and `LayoutUpdated` is unhooked once placeholders exist (`:285-292`). Old leaves stay at `Opacity 0` (and rooted in `_hiddenElements`), new content shows under stale placeholders; `Content = null` early-returns at `:264` before `HideCoveredElements`, so stale placeholders persist. Fix: override both → `RestoreHiddenElements`, `RemoveListStandIns`, clear `_lastPlaceholders`, `GenerateOverlay`. Tests: `When_Content_Replaced_Same_Size_While_Loading`, `When_Content_Set_Null_While_Loading`.
3. **Re-template while loading → empty overlay, content hidden, orphaned storyboard** — `OnApplyTemplate` (`:105-136`) removes stand-ins but keeps `_lastPlaceholders`/`_shimmerBands`/`_shimmerStoryboard`; the next pass hits `SequenceEqual` (`:294`) and returns, `StartShimmerIfNeeded` bails on the stale storyboard (`:545`), which keeps animating transforms from the old overlay forever. Fix: `StopShimmer(); ClearOverlay();` before `base.OnApplyTemplate()`. Test: `When_Template_Swapped_While_Loading`.
4. **`Skeleton.IsEnabled` skips injection whenever a `ProgressTemplate` is already in effect** — `Skeleton.cs:196` uses `GetValue`, which sees Style values; FeedView's default style sets `ProgressTemplate` (uno.extensions `FeedView.xaml:11`). Uno applies the default style at end of parse (`FrameworkElement.ApplyStyles`), so a XAML attribute injects, but code, bindings and `BasedOn` styles that take effect later are silently skipped (no log). Never validated on a real FeedView (`FeedViewStub` has no style). Fix: `ReadLocalValue(progressTemplateProperty) != UnsetValue` (matches the doc's "a template you assign yourself"), Debug log on skip. Test: stub with a Style-provided `ProgressTemplate`, then `SetIsEnabled(true)` → injected. **Coupled to decision B below**: only worth fixing if the injection API ships now; then also validate in the Playground on a real FeedView.

### Should-fix before merge (small, low risk)

- `SkeletonView.Source` doesn't `InheritDataContextFrom(this)` (`LoadingView.cs:166` does) → an inline `<utu:SkeletonView.Source><utu:LoadableSource Source="{Binding …}"/>` never binds, although the doc points to `LoadableSource`/`CompositeLoadableSource`. Mirror `LoadingView.OnSourceChanged` (+ `ClearValue` on the old value). Test.
- No `SkeletonView.HotReload.cs` — `LoadingView.HotReload.cs` restores a code-set `Source`/`Content` after a hot-reload tree rebuild; without it a code-set `Source` is lost and `IsLoading` falls back to `true` (permanent skeleton). Mirror the handler.
- `SkeletonPresenter.PrepareSkeletonContent` (`SkeletonPresenter.cs:81-100`) stamps `ItemsSource` into whatever is under the presenter; the "never the app's live content" comment is false when `Content` is set directly instead of `ContentTemplate`. Guard on `ContentTemplate is { }`; also require `Items.Count == 0` (an `ItemsControl` with inline items throws "Items must be empty" from a layout path).
- `ReserveRowsSpace` ignores `MaxHeight` (`ListStandIns.cs:242-258`); `MinHeight` wins over `MaxHeight` in layout, so a `ListView MaxHeight="100"` grows to the rows' height. Clamp with `Math.Min(rowsHeight, list.MaxHeight)`. Test: `When_Empty_List_Has_MaxHeight_Rows_Clipped`.
- Stand-in copies `Style`/`ItemContainerStyle` onto a base `ListView`/`GridView` (`ListStandIns.cs:173-196`); a `ListView` subclass with `TargetType="local:MyListView"` throws on WinUI (TargetType validation) from a layout handler, fine on Skia. Copy only when `style.TargetType.IsAssignableFrom(copy.GetType())`.
- `placeholders.SequenceEqual(_lastPlaceholders)` (`:294`) compares `Rect` doubles exactly; sub-pixel drift → full rebuild and a visible shimmer restart. Compare with `LayoutTolerance`.
- `HookLayoutUpdated` has no `IsLoaded` guard: an `IsLoading`/appearance change on a detached view (Frame back-stack) runs `GenerateOverlay` with stale sizes and re-hooks window-wide `LayoutUpdated` (and can raise `MinHeight`) until `IsLoading` flips. `if (!IsLoaded) return;` (`OnLoaded` regenerates).
- C# Markup: `doc/lightweight-styling.md:54` promises every lightweight key in `Uno.Toolkit.WinUI.Markup`; every control in that list has `Theme/<Control>.cs`, none for SkeletonView. Add `Theme/SkeletonView.cs` (3 resource keys, 2 style keys) mirroring `Divider.cs`.
- API hygiene: `ILoadable Source`, `Brush SkeletonBackground`, `Brush ShimmerBrush` are non-nullable with null defaults (`ShimmerBrush = null` is a documented feature) → annotate `?`; XML docs on the 7 DP identifiers and both ctors; `_overlay!` at `:488` → pass the overlay in.
- `Skeleton.FindDependencyProperty` (`Skeleton.cs:254-260`) re-implements the cached `DependencyObjectExtensions.FindDependencyProperty` (which already carries the IL2070/IL2075 suppressions and is used by `ResponsiveExtension`/`AncestorBindingExtension`). Reuse it; add a no-log flag if the miss warning is the obstacle for `FindValueTemplate` probing.
- `SkeletonPresenter`: XML summary says "always-loading" while the doc says `IsLoading = false` stops the shimmer — reconcile; `ResolveTemplate` warns twice (OnApplyTemplate + Loaded, since `IsLoaded` is set at tree entry on Uno) and never re-resolves a late-bound owner template → warn only from the `Loaded` call.

### Decisions for the author

- **A. `PlaceholderCount` default.** Today: metadata default 4, but inside a `SkeletonView` only a *local* value (self/ancestor) counts, so `GetPlaceholderCount(list)` returns 4 for a list with 0 rows and a Style setter is ignored. Option (b): default 0 (= off) + effective-value ancestor walk, with `SkeletonPresenter` owning its own `4` constant. Gains: honest getter, Style setters work — and `SkeletonFeedViewStyle` could then supply the 4-row default itself (a local value on the FeedView still wins), which changes the Phase 7 trade-off recorded above. Cost: an explicit `0` can no longer veto an ancestor's count.
- **B. Ship `Skeleton.IsEnabled`/`PlaceholderTemplate` now?** Only known target is FeedView, the doc itself prefers `SkeletonFeedViewStyle`, and injection was never exercised on a real FeedView. Attached DPs are a public commitment. Either fix #4 and validate in the Playground, or defer the injection API (and `SkeletonPresenter`'s owner lookup) to a follow-up with a second consumer.
- **C. `Skeleton.Ignore`d elements are visible but dead** while loading (`PART_ContentPresenter.IsHitTestVisible = False`, `SkeletonView.xaml:62`): fine for a header, surprising for a Cancel button. Intended?
- **D. `TextBox` on Skia** hosts a `TextBlock`, which `IsSkeletonLeaf` matches → typed text is hidden and mirrored on Skia but visible on WinUI. Make `TextBox` an explicit leaf (consistent) or document.

### Follow-ups (not blocking)

- Hot-path allocations while `LayoutUpdated` is hooked: `GetChildren()` is `Enumerable.Range().Select()` (iterator + closure per node) in both walks and `FindContentRoot`; per-pass `List`/`HashSet`/`Queue`. Index loops and scratch collections. Measured: 0 passes/2 s during steady-state shimmer (shimmer never invalidates layout), 9 vs 4 passes during initial materialization with/without a stand-in — so this is per genuine layout pass, not per frame.
- Shimmer: one CPU-ticked `DoubleAnimation` per placeholder on Skia; a single shared transform (per-band offset baked into `Margin`) is the cheap alternative if measured to matter.
- Duplicated exclusion predicate (`SkeletonView.cs:330-336` vs `ListStandIns.cs:132`) and four `ItemsControl`/`ItemsRepeater` `ItemsSource` switch sites → shared helpers.
- Plausible, needs a test: collapsed container chrome (e.g. `Expander IsExpanded="False"`) → `FindContentRoot` jumps to never-laid-out content → stray synthesized text placeholder at the parent's origin. `ItemTemplateSelector`/`IElementFactory` on an `ItemsRepeater` receives `null` items. `Unloaded` while loading doesn't restore `Opacity`/`MinHeight` (an element reparented elsewhere stays hidden).
- Test gaps: `Shape="Rectangle"` (sample advertises it), `GridView` stand-in, `Source` teardown on `Unloaded`/reassign, `IsEnabled` on a type without `ProgressTemplateProperty`, `PlaceholderCount = 0`, `ShimmerBrush = null`, `PlaceholderTemplate` clear; `GetOverlay` + `"PART_SkeletonOverlay"` copy-pasted across three test classes → shared helper. Nothing has run off Skia desktop: one Windows run of the three classes would settle `Style.TargetType`, `Binding` reuse and `GetLayoutSlot` semantics.
- Logging/docs: Debug log when `CreatePlaceholderList` returns null for an opted-in but unsupported list kind (`ListStandIns.cs:182`); list the three warnings and logger categories in the troubleshooting section; state the UI-thread contract for `ILoadable.IsExecutingChanged` on `Source` (same gap as `LoadingView`). `TextLineHeightFactor` ignores an explicit `TextBlock.LineHeight`; `StackLayout` is cloned but other `Layout`s are shared by reference.
- Commits `chore: initial` / `chore: skeleton work` → squash into the adjacent `feat(skeleton)` commits (already noted on the PR). PR still has no associated issue.

### Fixes applied (red/green, 40/40 skeleton tests pass on Material desktop, rebased on `92fc3f65`)

- Blocking 1–4: nested `SkeletonView` is a walk boundary (`IsNestedSkeleton`, both walks) — `When_Nested_SkeletonView_Outer_Finishes_First` / `_Inner_Finishes_First`; `OnContentChanged`/`OnContentTemplateChanged` restore, clear and re-hook `LayoutUpdated` — `When_Content_Replaced_Same_Size_While_Loading`, `When_Content_Set_Null_While_Loading`, `When_ContentTemplate_Changed_While_Loading`; `OnApplyTemplate` stops the shimmer and clears the overlay cache — `When_Template_Swapped_While_Loading`; injection checks `ReadLocalValue` (Debug log when a local template is kept) — `When_Style_ProgressTemplate_Replaced_By_Injection`.
- Should-fix: `Source` inherits the DataContext (`OnSourceChanged`) — `When_Source_Is_LoadableSource_DataContext_Inherited`; `SkeletonView.HotReload.cs` mirrors LoadingView's handler; the presenter stamps only its `ContentTemplate` instantiation and only empty lists — `When_Presenter_Direct_Content_Not_Stamped`, `When_Template_List_Has_Items_Not_Stamped`; `MinHeight` capped at `MaxHeight` — `When_Empty_List_Has_MaxHeight_Rows_Clipped`; a `Style` targeting a list subclass is not copied onto the stand-in — `When_ListView_Subclass_With_Own_Style_Gets_Rows` (was also red on Skia: 1 row instead of 2); placeholder comparison uses `LayoutTolerance` (`SameGeometry`); `HookLayoutUpdated` requires `IsLoaded`; `Theme/SkeletonView.cs` added to `Uno.Toolkit.WinUI.Markup`; `Source`/`ShimmerBrush` annotated nullable, XML docs on all DP identifiers and ctors, no `_overlay!`; `Skeleton` reuses `DependencyObjectExtensions.FindDependencyProperty` (new `logMissing` flag) instead of its own reflection + suppression; `SkeletonPresenter` warns once (from `Loaded`) and documents "loading by default"; Debug log for an opted-in list of an unsupported kind.
- Docs: injection semantics (local vs style-provided template), presenter stamping scope, `MaxHeight`, subclass styles, nested skeletons, logged warnings.
- CI *Packages* (C# Markup generator on `Skeleton.PlaceholderTemplate`) and *Docs Validations* failures: fixed by `21d368ed` and `92fc3f65` (Markup csproj workaround, doc indentation, lesson in `specs/lessons.md`); not duplicated here.
- Not applied (decisions A–D, follow-ups): unchanged, see above.

### Verified non-issues

- `Generated/mergedpages.xaml`: the committed +1 line is the Debug reference form main tracks; the working-tree drift seen mid-review was Release merge output (gitignored but tracked) and is gone after a Debug build.
- `TransformToVisual` cannot throw on Uno for non-live/non-ancestor targets (identity/root-inverse); same-tree invariant holds at every call site. `Binding` instance reuse in `PropertyValue.Restore` is legal on WinUI and Uno. `LayoutUpdated` hook/unhook is symmetric. No async code. Logging matches `LoadingView`'s `typeof(X).Log()` pattern. No enum in the library declares an underlying type. `IsLoading` default `true` matches `LoadingView` (`Source?.IsExecuting ?? true`).
- `specs/lessons.md`: no correction this phase, nothing to add.

## Notes for future work

- Headless runtime tests locally need **both** `UNO_RUNTIME_TESTS_RUN_TESTS` and `UNO_RUNTIME_TESTS_OUTPUT_PATH` env vars in addition to `--runtime-tests=<path>`; with only the filter var set, the app idles at the runner UI and never starts.
- Placeholder regeneration is triggered by `SizeChanged` of both the ContentPresenter *and* the overlay Canvas — the overlay is collapsed while not loading, so only its own 0→W size change reliably signals "visible and arranged" when loading restarts.
- `LeakTest` is fully `[Ignore]`d (#429), so no SkeletonView row was added there.
