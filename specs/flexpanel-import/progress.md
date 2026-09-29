# Progress: Yoga-backed `FlexPanel` import

Tracking for [`spec.md`](./spec.md). Branch `dev/xygu/20260904/reactor-flexpanel-import`, forked from
`dev/xygu/20260820/reactor-flex-panel-specs`.

| Phase | State |
|---|---|
| P0 — Approval | ✅ vendoring confirmed, name `FlexPanel` kept |
| **P1 — Engine import** | ✅ **done** |
| **P2 — Tier-1 conformance** | ✅ **done — 544/544 green on desktop/Skia** |
| **P3 — Adapter (`FlexPanel.cs`)** | ✅ **done — 24/24 tier-2 + tier-3 green on desktop/Skia** |
| P4 — Cross-platform validation | ⬜ (deliberately deferred — see below) |
| **P5 — Sample + docs** | ✅ **done** |
| P6 — Review | ⬜ |

## Pinned upstream

| | |
|---|---|
| Repo | <https://github.com/microsoft/microsoft-ui-reactor> |
| Tag | `v0.1.0-preview.13` |
| Commit | `c9191b97c40a2e4d6bcbc72df7714184862b4d36` |
| Imported | 2026-09-04 |

The import is performed by a scripted, idempotent, self-verifying transform. Re-running it against a
newer upstream reproduces both folders exactly; it fails loudly on any leftover upstream namespace,
`Xunit` reference, `[Fact]`, un-renamed `LTR`/`RTL`, or missing provenance header. Procedure in
[`src/Uno.Toolkit.UI/Layout/Yoga/README.md`](../../src/Uno.Toolkit.UI/Layout/Yoga/README.md).

## P1 — Engine import ✅

10 files into `src/Uno.Toolkit.UI/Layout/Yoga/` (upstream's 11 minus `FlexPanel.cs`, the WinUI adapter,
which is P3 and gets rewritten rather than vendored). Plus `README.md`, a scoped `.editorconfig`, and
`THIRD-PARTY-NOTICES.md` at the repo root.

**Release build: 0 warnings, 0 errors** on the Skia TFM.

## P2 — Tier-1 conformance ✅

26 files into `src/Uno.Toolkit.RuntimeTests/Tests/Yoga/`, plus our own `YogaAssert.cs` and a `README.md`.

```text
result=Passed  total=544  passed=544  failed=0  skipped=46  inconclusive=0
```

Run on desktop/Skia (`MaterialSampleApp` @ `net10.0-desktop`), filter `Tests.Yoga.`, **6 seconds**.

**FR-9 re-verified in the same host**: `AutoLayoutTest` → 119 passed / 0 failed / 1 skipped (the
pre-existing `[Ignore]` for #1203). `git diff --stat` against the fork point is empty for both
`src/Uno.Toolkit.UI/Controls/AutoLayout/` and `src/Uno.Toolkit.RuntimeTests/Tests/AutoLayoutTest.cs`.

### Verified in the configuration CI actually uses

CI runs the suite **unfiltered** (`UNO_RUNTIME_TESTS_RUN_TESTS: '{}'` in
`build/workflow/stage-runtime-tests.yml`), so the filtered runs above are not sufficient evidence on
their own. Unfiltered run, Release sample head, desktop/Skia:

```text
result=Passed  total=874  passed=874  failed=0  skipped=57   (931 cases)
```

Cross-checking every test-method name in the imported corpus against that result:
**590 / 590 present — 544 passed, 46 skipped, 0 missing, 0 failed.** The remaining 341 cases are the
pre-existing suite, unperturbed. Whole run: 38s.

Release was used because the `Tests/HotReload/` folder is `#if DEBUG`-only, and those tests need a
dev-server that is not available in this environment — a pre-existing local limitation, untouched by
this work (the import adds no XAML and no hot-reload surface). They run in their own CI stage.

### Corpus size — correcting the spec

The spec estimates "~1,300 assertions". The measured corpus is an order of magnitude larger:

| | count |
|---|---|
| Files | 26 |
| Test methods discovered | **590** |
| — active | **544** |
| — skipped by upstream Yoga (`GTEST_SKIP`) | **46** |
| Assertions | **17,784** |

All 17,784 have the identical shape `Assert.Equal(<float literal>f, <node>.Layout{X,Y,Width,Height})`,
which is what made the xUnit → MSTest conversion purely mechanical.

### The 46 skipped tests

Upstream ships them as `[Fact(Skip = "Skipped in upstream Yoga (GTEST_SKIP)")]` — they are skipped in
Meta's own C++ suite, so they arrive already disabled. They are imported as
`[TestMethod] [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]`.

This is **not** a violation of the AGENTS.md rule against `[Ignore]`: that rule forbids deactivating
*our* tests. These were never active anywhere. Mirroring upstream's state keeps the corpus complete
and easy to re-sync; enabling them would import 46 known-failing cases. Concentrated in
`YogaIntrinsicSizeTest` (28) and `YogaPercentageTest` (5) — mostly `fit-content` / `max-content` /
`stretch` sizing keywords.

### Compile cost

Adding 1.7 MB / 17,784 statements to `Uno.Toolkit.RuntimeTests` costs **~0.1s** on a full `-t:Rebuild`
(8.41s → 8.51s). At run time the corpus adds about **6s** to a 38s suite. Negligible on both counts;
no need to gate it behind a build flag.

## Findings that amend the spec

Recorded here; `spec.md` has been updated to match.

### 1. 🔴 `Uno.Toolkit.UI.Layout` is unusable — D2 corrected

`Uno.Toolkit.UI` already contains `public enum Layout` (`Helpers/ResponsiveHelper.cs:17`, consumed by
`ResponsiveHelper`). A namespace of the same fully-qualified name cannot coexist with it, and the enum is
public API that cannot be renamed. Resolution:

| | namespace |
|---|---|
| `FlexEnums.cs` — the 6 public enums that become `FlexPanel`'s DP types | `Uno.Toolkit.UI` |
| The other 9 engine files, all `internal` | `Uno.Toolkit.UI.Yoga` |

C# enclosing-namespace lookup means the engine resolves the enums with no `using`, and the public
surface stays exactly 6 enum types.

### 2. ✅ G2 answered — Yoga clamps negative gap to 0

`YogaStyle.ComputeGapForAxis` is the single gap entry point for both call sites in `YogaAlgorithm`:

```csharp
return YogaFloat.MaxOrDefined(gap.Resolve(ownerSize), 0);   // MathF.Max(x, 0)
```

So **G2 stands as a genuine gap**: `AutoLayout`'s negative `Spacing` (the overlapping-avatar samples use
`Spacing="-10"` / `"-20"`) has no `FlexPanel` container-level equivalent. The capability survives only
through negative child `Margin`, which Yoga does support.

### 3. Visibility needed no work

Every vendored top-level type is already `internal` upstream except the 6 public enums, so the engine
never entered the NuGet public surface. The corpus reaches it through the pre-existing
`[assembly: InternalsVisibleTo("Uno.Toolkit.RuntimeTests")]`.

### 4. D7's analyzer containment turned out to be unnecessary

The spec anticipated needing a scoped `.editorconfig` full of suppressions for 4,471 LOC of C++-shaped
port under `TreatWarningsAsErrors=true` + `AnalysisModePerformance=AllEnabledByDefault`. Measured: **zero
warnings**. Upstream already builds under the same `TreatWarningsAsErrors` + `Nullable=enable` settings.
The scoped `.editorconfig` was still added, but contains only `indent_style = space` (upstream is
4-space, the repo is tabs — reformatting would make diffing against upstream painful) and a note on
where to put suppressions if a future sync does need them.

### 5. NFR-5 — there were no MIT headers to preserve

Vendored files carry only `// C# port of Meta's Yoga…` comments; no copyright, no license, no
provenance. Each imported file now gets a generated 4-line header naming the repo, tag, commit, its own
upstream path, the import date, and SPDX MIT with both copyright holders (Microsoft for the port, Meta
for Yoga). Full license text lives once in `THIRD-PARTY-NOTICES.md`.

### 6. Target scope narrowed to Skia

Native targets (`-ios`, `-android`, `-windows`, `-maccatalyst`) are being removed in Uno 7.0; that work
is underway in the toolkit on `dev/mazi/uno7-groundwork`. FR-8 and the P4 matrix have been narrowed to
the Skia heads accordingly. This costs nothing here — the engine is pure managed `float` math with no
platform API, no `#if`, and no TFM-conditional code.

## P3 — Adapter ✅

`src/Uno.Toolkit.UI/Controls/FlexPanel/{FlexPanel.cs, FlexPanel.Properties.cs}`, **adapted** from
upstream's 978-LOC `FlexPanel.cs`. The two-pass measure, the `MeasureFunc` bridge and `SyncYogaTree`'s
cache pruning follow it closely; the D3/D4/D5 adaptations and the `UseLayoutRounding` binding are ours.

> ⚠️ Originally described here as "a rewrite, not a port" — **that was wrong**, and it is why the two
> files initially shipped with no attribution. Measured afterwards: 52 of our 58 identifiers match
> upstream, including private helpers no independent implementation would land on by coincidence
> (`SyncPointScaleLazy`, `IsScrollLikeContainer`, `ComputeMinContent`, `ResolveMinDimension`,
> `SetRootConstraints`), and bodies match closely — `SyncPointScaleLazy` down to the `0.0001f` epsilon
> and the `_yogaConfig` / `_rootNode` field names. It is a derivative work of MIT code, so MIT
> attribution is **required**, not courtesy. Both files now carry a provenance header and are listed
> in `THIRD-PARTY-NOTICES.md`.

**Release build: 0 warnings, 0 errors** on the Skia TFM for `Uno.Toolkit.WinUI` and
`Uno.Toolkit.RuntimeTests`.

### Deviations from upstream, and why

| | |
|---|---|
| `FlexPadding` → `Padding` | D3. `Panel` has no `Padding`, so there is no conflict to dodge. Re-verified against the resolved `Uno.UI.dll` (Uno.WinUI 6.6.166, via `Uno.Sdk.Private`): `Microsoft.UI.Xaml.Controls.Panel` carries only an `internal`, non-virtual `PaddingInternal` auto-property that `Grid`/`StackPanel`/`RelativePanel`/`LayoutPanel` assign from their own DP — `FlexPanel` never writes it, so it stays `default(Thickness)` and the engine is the single consumer. Pinned by `When_Padding_ThenContentBoxIsInsetOnce` / `When_PaddingAndContentSized_ThenDesiredSizeGrowsByPaddingOnce` / `When_PaddingAndArrangeRelayout_ThenInsetStillAppliedOnce`. |
| `GetMinWidth`/`SetMinWidth` → `GetFlexMinWidth`/`SetFlexMinWidth` | **Upstream bug.** It registers the DP as `"FlexMinWidth"` but names the accessors `Get/SetMinWidth`. XAML resolves an attached property by the accessor name, so `utu:FlexPanel.FlexMinWidth="0"` would not bind against upstream's naming. Ours is XAML-first, so the accessors must match the registered name. |
| `PointScaleFactor` bound to `UseLayoutRounding` | D4 / FR-6, rather than upstream's unconditional `RasterizationScale`. |
| Scratch collections cleared at end of pass | See the leak note below. |

### Tier 2 + tier 3 — 27/27 green

`Tests/FlexPanelTests.cs` (24) and `Tests/FlexPanelLeakTests.cs` (3). All 18 tier-2 cases the spec
names, plus six not in the spec:

- `When_DirectionNotSet_ThenDefaultsToRow` — pins the `FlexDirection.Column = 0` vs `Row`-default
  hazard flagged in the P3 handoff notes.
- `When_UseLayoutRoundingTrue_ThenArrangeSnapsToPixelGrid` — the counterpart that makes the FR-6
  toggle an observable change rather than an untested claim.
- `When_PanelUnloaded_ThenChildrenAreCollectable` — the `Unloaded` half of D9.
- `When_Padding_ThenContentBoxIsInsetOnce` and `When_PaddingAndContentSized_ThenDesiredSizeGrowsByPaddingOnce`
  — the D3 rename left `Padding` looking like a framework property, so these pin the seam in both
  directions: a 200-wide box with `Padding="20"` leaves 160 of content (a double inset would leave 120),
  and a content-sized panel around a 50×50 child reports 90×90 (a double inset would report 130×130).
- `When_PaddingAndArrangeRelayout_ThenInsetStillAppliedOnce` — the same seam on the other arrange
  path. The two above measure and arrange at the same size, so `ArrangeOverride` reuses the cached
  child rects; this one measures at content size and arranges larger, forcing the `sizeChanged`
  branch to re-run `CalculateLayout` — the one place padding reaches the engine twice in a single
  layout cycle.

### 🔴 A real leak the tier-3 tests caught

The first run was 20/23: two leak tests failed. `SyncYogaTree` correctly evicted removed children
from `_nodeCache` / `_attachedCache`, but the **scratch collections themselves were left populated**
— `_syncToRemove` still held every element it had just evicted, and `_measuredThisPass` held the
children measured that pass. Both are instance fields reused across passes (NFR-2), so a panel that
never laid out again pinned those children indefinitely. Fixed by clearing `_syncToRemove` /
`_syncCurrentChildren` at the end of `SyncYogaTree` and `_measuredThisPass` at the end of
`MeasureOverride`. Allocation-free, so NFR-2 is unaffected.

This is exactly what D9 predicted tier 3 was for, and it would not have been visible from any
tier-1 or tier-2 assertion.

**Hardened past what the tests can see.** Clearing at the end of a *successful* pass is still the
"released only on the next pass" shape that caused the bug: a pass that throws part-way, or a
panel unloaded mid-pass, would keep holding. The clears now sit in `finally` blocks
(`MeasureOverride` delegates to `MeasureCore`, `SyncYogaTree` to `SyncYogaTreeCore`) and are
repeated in `OnUnloaded`, so the release is unconditional. The leak tests pass either way --
they always trigger a clean subsequent pass -- so this one is reasoned, not test-driven.

### The third failure was the test, not the adapter

`When_UseLayoutRoundingFalse_ThenFractionalArrangePreserved` expected `33.333` and got `33.0`. Yoga
was in fact not rounding — `PixelGridHelper` skips when `PointScaleFactor == 0`, verified in
`AlgorithmUtils.cs`. The remaining rounding was the **framework's own**, applied per element, which
is precisely what FR-6 says survives ("the platform's own rounding is the only one applied"). The
test now clears `UseLayoutRounding` on the children too.

### Verified in the configuration CI uses

Unfiltered, Release sample head, desktop/Skia:

```text
result=Passed  total=898  passed=898  failed=0  skipped=57
```

874 → 898 is exactly the 24 added; `skipped` is unchanged at 57, so nothing in the pre-existing suite
moved. All 24 new method names confirmed present in that unfiltered run.

**FR-9 re-verified**: `git diff` against the fork point is empty for
`src/Uno.Toolkit.UI/Controls/AutoLayout/` and `src/Uno.Toolkit.RuntimeTests/Tests/AutoLayoutTest.cs`,
and the AutoLayout tests pass inside the 898.

## P5 — Sample + docs ✅

- `samples/…/Content/Controls/FlexPanelSamplePage.xaml{,.cs}` — the gallery, `SampleCategory.Controls`,
  `IsDesignAgnostic`. Eleven captioned sections covering direction, the justify/align matrices,
  wrap + gaps, align-content, grow/shrink/basis, absolute positioning, RTL, and padding/auto-min.
  Section 2 is a bare `FlexPanel` whose only job is to show the `Row` default holding.
  Section 8 puts `Grow="1"` next to `Grow="1" Basis="0"` so the M2 trap is visible rather than
  described.
- `samples/…/Content/NestedSamples/FlexPanelPlaygroundNestedPage.xaml{,.cs}` — the playground, opened
  full-screen from the gallery via `Shell.ShowNestedSample`. Every container and per-child property
  live, plus the `UseLayoutRounding` toggle, a `FlowDirection` toggle that demonstrates the FR-7
  double-mirror, a 200-child stress toggle, and an arranged-rect readout.
  A **nested** page rather than `SampleCategory.Tests`: the Tests category is dropped from the
  navigation under `#if !DEBUG`, so a playground there would not ship. Nested pages have no nav entry
  by design and behave identically in Debug and Release.
- `doc/controls/FlexPanel.md`, an `AutoLayoutControl.md` cross-link, and a `doc/toc.yml` entry.
- No `lightweight-styling.md` changes, and no row in the **Control Styles** table of
  `controls-styles.md` (D6 — templateless panel, no theme resources).

### P5b — walkthrough + discovery (added after the P5 doc-gap review)

`FlexPanel` shipped with a reference page but no `*.howto.md`, leaving it one of three controls in
`doc/toc.yml` without one (the others are `Divider` and `DrawerControl`, both still missing one since the
walkthrough batch in #1492). The reference page is also not reachable from anywhere but the nav tree.

- `doc/controls/walkthroughs/FlexPanel.howto.md` — 13 outcome-first recipes in the house `.howto.md`
  shape (`uid` + `tags` front matter, `UnoFeatures` line, per-recipe XAML + bullets, quick reference,
  gotchas). Ordered to lead with the four places CSS/WinUI intuition misleads — `Basis` vs `Width`,
  `Grow="1"` vs `flex: 1 1 0`, `Shrink="0"` overflow, and the auto-min escape hatch — rather than
  burying them after the generic direction/gap/justify recipes.
- **Every recipe is traced to an assertion in `FlexPanelTests.cs`**, not extrapolated from the property
  tables. This is deliberate: `specs/lessons.md` records that `AutoLayout.howto.md` teaches two
  behaviors the code does not implement, and that failure mode is exactly what a table-derived recipe
  reproduces. `Position="Absolute"` and `AlignItems="Baseline"` were written only because
  `When_PositionAbsolute_ThenInsetsHonoredAndSiblingsUnaffected` and
  `When_AlignItemsBaseline_ThenTextBaselinesAlign` pin them; `AlignContent`, `WrapReverse` and the
  `*Reverse` directions are named in the quick reference but given **no** behavioral recipe, because
  nothing at the control level asserts them.
- `doc/toc.yml` — walkthrough entry, alphabetical between `ExtendedSplashScreen` and `LoadingView`.
- Cross-links from `FlexPanel.md` **and** `AutoLayoutControl.md` to their walkthroughs. Neither
  reference page previously linked to its own how-to, so the walkthroughs were reachable only via the
  nav tree or site search.

**Amendment to D6.** `controls-styles.md` now lists `FlexPanel` in the *control index* at the top of
the page — the plain "the library adds the following controls" list, which every other shipped control
appears in. D6 is unchanged as written: it rules out `Style`s, theme resources, a **Control Styles**
table row and `lightweight-styling.md` keys, all of which still apply to a templateless panel. Omitting
the control from the index as well was a side effect of that decision, not part of it, and it left
`FlexPanel` absent from the one page that enumerates what the library ships.

**Not addressed — distribution.** `platform.uno/llms.txt` indexes only `unoplatform/uno`; it carries no
`uno.toolkit.ui` URL at all, so neither this walkthrough nor any existing one reaches that channel.
Fixing it is a change to a different repo and is not in this branch's scope.

**Spec amendment**: the sample attribute uses `SourceSdk.UnoToolkit`, which the spec's snippet omits.
It renders the correct "SOURCE" line. Note that `SupportedDesigns` does **not** filter this page —
`App.xaml.Navigation.cs` early-returns for `WinUI`/`Uno`/`UnoToolkit` sources — so the gallery renders
in the Material, Cupertino *and* Simple heads and uses only theme-neutral resource keys.

## Still open

- **P4 is untouched and deliberately out of scope here.** The rounding matrix across the Skia heads
  (WASM, Skia mobile, scale factors 1.0/1.25/1.5/2.0) remains spec risk 1, and everything above was
  verified on desktop/Skia only.
- **Spec risk 3 is not closed.** The playground ships the 200-child stress toggle, but the WASM
  profile it calls for has not been run; that defers with P4.

## Handoff to P3 (historical — P3 is done; kept for the reasoning)

- The adapter is **adapted from** upstream's `FlexPanel.cs` (978 LOC) — see the correction under P3;
  the original "rewrite, not a port" framing was inaccurate. It is the reference for the two-pass
  measure, the `MeasureFunc` bridge and `SyncYogaTree`'s cache pruning, but it targets WinUI directly.
  It lives in the pinned clone, not vendored here.
- The 6 public enums are already in `Uno.Toolkit.UI`, so `FlexPanel.cs` needs no extra `using` for them;
  it needs `using Uno.Toolkit.UI.Yoga;` to reach `YogaNode` / `YogaConfig` / `YogaValue`.
- D3b is done: `FlexLayoutDirection.LeftToRight` / `.RightToLeft`. Numeric values were **not** renumbered
  anywhere, `FlexDirection`'s Yoga ordering (`Column = 0`) included — so `Direction`'s DP metadata
  default must be set to `Row` explicitly (spec's ⚠️ note).
- A tier-1 failure after any future engine change is an **engine regression**. Do not adjust
  expectations.

## Open

- Spec risk 1 (rounding parity across Skia heads at scale factors 1.0/1.25/1.5/2.0) is untouched — P4.
- Spec risk 3 (WASM cost of the two-pass measure over a large panel) is untouched — P5.
- `YogaAlgorithm`'s static `s_currentGenerationCount` (D8) is safe while the runtime-test runner is
  sequential. Worth revisiting if it ever runs tests in parallel.

## Review pass — PR #1639 (kazo0, 2026-09-28) ✅

Spec amendments R1–R6 and the declined item are recorded in [`spec.md`](./spec.md#review-amendments-pr-1639).
Everything here was measured on desktop/Skia (Debug head, `UNO_RUNTIME_TESTS_RUN_TESTS` filter).

- [x] `Generated/mergedpages.xaml` restored to the Debug version, folded into the commit that had
  swept in the Release output.
- [x] `specs/lessons.md`: preamble placement, local path, and two new lessons (Release builds
  rewrite `Generated/`; fold a fixup against the target commit's parent, not `main`).
- [x] R2 `LayoutDirection` removed. `When_FlowDirectionRightToLeft_ThenMirroredVisually` confirms
  the platform mirrors the panel (first child drawn at x = 200 in a 300px panel).
- [x] R3 public enums owned by FlexPanel; `import-yoga.py` makes the vendored ones internal and
  its self-check rejects public top-level engine types. Re-import is clean; corpus 544 passed,
  46 skipped upstream.
- [x] R6 attached-property cache removed.
- [x] R1 min-content probe removed, after measuring it:
  - `TextBlock("Unbreakable").Measure(0, ∞)` → `0×300`; `Border(Width=300).Measure(0, ∞)` → `0×20`.
    The probe always returned 0.
  - 50px row, 92px word: slot 26px. The §4.5 floor never applied.
  - The old `When_AutoMin_*` child (92px) sat in a 100px panel and never overflowed.
  - Per-level `MeasureOverride` counts, outer → inner, before: `[1,2,2,2,2]` with or without
    overflow; after: `[1,1,1,1,1]` (`When_NestedFiveDeep_ThenEachLevelMeasuredOnce`, red before,
    green after). No exponential growth either way, contrary to the review's estimate.
  - A 500-item `ListView` measured at `(0, ∞)` reports its full 21000px extent but realizes only
    8 items on Uno, so the virtualization concern did not reproduce.
- [x] R4 parent height mode scoped to direct FlexPanel children
  (`When_InnerFlexPanelUnderGridRow_ThenFillsDefiniteSlot`: DesiredSize 20 → 300).
- [x] R5 child `Min*` / `Max*` honored (three tests, all red before: 200 → 100, 50 → 80, 100 → 40).
- [x] XML docs on every `*Property` field (19).
- Runtime tests: FlexPanel 34/34 passed.
- Release, desktop: 0 warnings from the library, runtime tests or FlexPanel (the sample app's
  pre-existing nullable warnings are unrelated). `src/Uno.Toolkit.sln` itself does not load at
  this base — it still lists the deleted `Uno.Toolkit.UITest` project, fixed on `main` — so the
  heads were built directly. WASM is left to CI: `crosstargeting_override.props` pins this
  checkout to desktop.
