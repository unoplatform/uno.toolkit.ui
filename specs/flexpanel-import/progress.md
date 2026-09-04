# Progress: Yoga-backed `FlexPanel` import

Tracking for [`spec.md`](./spec.md). Branch `dev/xygu/20260904/reactor-flexpanel-import`, forked from
`dev/xygu/20260820/reactor-flex-panel-specs`.

| Phase | State |
|---|---|
| P0 — Approval | ✅ vendoring confirmed, name `FlexPanel` kept |
| **P1 — Engine import** | ✅ **done** |
| **P2 — Tier-1 conformance** | ✅ **done — 544/544 green on desktop/Skia** |
| P3 — Adapter (`FlexPanel.cs`) | ⬜ not started (P2's gate deliberately blocks it until now) |
| P4 — Cross-platform validation | ⬜ |
| P5 — Sample + docs | ⬜ |
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

```
result=Passed  total=544  passed=544  failed=0  skipped=46  inconclusive=0
```

Run on desktop/Skia (`MaterialSampleApp` @ `net10.0-desktop`), filter `Tests.Yoga.`, **6 seconds**.

**FR-9 re-verified in the same host**: `AutoLayoutTest` → 119 passed / 0 failed / 1 skipped (the
pre-existing `[Ignore]` for #1203). `git diff` against the fork point shows zero changes under
`src/Uno.Toolkit.UI/Controls/AutoLayout/`.

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
(8.41s → 8.51s). Negligible; no need to gate the corpus behind a build flag.

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

## Handoff to P3

- The adapter is a **rewrite**, not a port. Upstream `FlexPanel.cs` (978 LOC) is the reference for the
  two-pass measure, the `MeasureFunc` bridge and `SyncYogaTree`'s cache pruning, but it targets WinUI
  directly. It is in the pinned clone, not vendored here.
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
