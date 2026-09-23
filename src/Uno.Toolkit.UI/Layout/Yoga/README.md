# Vendored: Yoga flexbox layout engine

This folder is **vendored third-party code**. Do not hand-edit it.

It is a C# line-port of [Meta's Yoga](https://github.com/facebook/yoga) flexbox engine, authored by
Microsoft as part of the [Reactor](https://github.com/microsoft/microsoft-ui-reactor) project. Both
grants are MIT; the full text of both is in [`THIRD-PARTY-NOTICES.md`](../../../../THIRD-PARTY-NOTICES.md)
at the repo root.

| | |
|---|---|
| Upstream | <https://github.com/microsoft/microsoft-ui-reactor> |
| Tag | `v0.1.0-preview.13` |
| **Commit** | **`c9191b97c40a2e4d6bcbc72df7714184862b4d36`** |
| Upstream path | `src/Reactor/Yoga/` |
| Imported | 2026-09-04 |
| Files | 10 of the 11 in that folder (`FlexPanel.cs`, the WinUI adapter, is not vendored — we write our own) |

## Why it is here rather than referenced

`Microsoft.UI.Reactor` ships as a single `net10.0-windows10.0.22621` package with a WindowsAppSDK
dependency and the whole Reactor framework attached, so a `PackageReference` is not viable for a
multi-target Uno library. See `specs/flexpanel-import/spec.md` (risk 5).

## Why it is not under `Controls/`

It is not a control, and it must not be mistaken for our own code. The `Uno.Toolkit.UI` XAML merge
(`xamlmerge-toolkit.props`) only globs `Controls/**` and `Behaviors/**`, so `Layout/` is inert there and
no csproj change was needed — the implicit `Compile` glob picks these files up.

## Adaptations applied on import

Every one is **mechanical and scripted**, by [`build/scripts/import-yoga.py`](../../../../build/scripts/import-yoga.py).
Re-running it against a newer upstream reproduces this folder exactly.

1. **Provenance header.** Four lines prepended to each file: upstream repo, tag, commit, that file's
   original upstream path, import date, and SPDX license with both copyright holders. This offsets every
   file by **4 lines** when diffing against upstream — account for it.
2. **Namespace.**
   - `FlexEnums.cs` → `Uno.Toolkit.UI`. It holds the six public, user-facing enums (`FlexAlign`,
     `FlexDirection`, `FlexJustify`, `FlexWrap`, `FlexPositionType`, `FlexLayoutDirection`) that are the
     DP types of `FlexPanel`, so they belong in the flat toolkit namespace where consumers already look.
   - Everything else → `Uno.Toolkit.UI.Yoga`. All of it is `internal`, so none of it is public API.
     C# enclosing-namespace lookup resolves the enums from here with no `using`.

   > ⚠️ `Uno.Toolkit.UI.Layout` — which `specs/flexpanel-import/spec.md` originally proposed — **cannot be
   > used**. It collides with the existing `public enum Layout` in `Uno.Toolkit.UI`
   > (`Helpers/ResponsiveHelper.cs`), and that enum is public API that cannot be renamed.

3. **Explicit `using` directives added.** Upstream builds with `ImplicitUsings=enable`;
   `Uno.Toolkit.WinUI.csproj` does not. `System`, `System.Collections.Generic`,
   `System.Runtime.CompilerServices`, `System.Diagnostics` and `System.Threading` are added per file
   as needed. The script skips any a file already declares.
4. **`FlexLayoutDirection.LTR`/`.RTL` renamed to `.LeftToRight`/`.RightToLeft`** (spec D3b). This enum is
   part of *our* public API and WinUI spells it out (`FlowDirection.LeftToRight`); abbreviations read
   badly in XAML. **Numeric values are not renumbered** — not here, and not for `FlexDirection`, whose
   Yoga ordering (`Column = 0, ColumnReverse = 1, Row = 2, RowReverse = 3`) is deliberately preserved.
5. **Nothing else.** No visibility changes (upstream is already `internal` everywhere but `FlexEnums.cs`),
   no reformatting, no `#nullable` pragmas, no analyzer suppressions — none turned out to be necessary.

## Accepted as-is

- `YogaAlgorithm` is a static class holding `private static uint s_currentGenerationCount`, documented
  upstream as thread-unsafe and bumped via `Interlocked.Increment`. Layout runs on the UI thread on every
  Uno target, so the invariant holds; it is a value type, so it pins nothing and is ALC-safe. Recorded
  rather than refactored — divergence from upstream has to be paid for (spec D8).
- CSS Grid properties present in C++ Yoga are absent from this port. `YogaStyle` still carries
  `JustifyItems`/`JustifySelf`, but grid layout is not implemented and is out of scope.

## Conformance tests

The upstream generated corpus is ported alongside the engine to
`src/Uno.Toolkit.RuntimeTests/Tests/Yoga/` — 26 files, 590 test methods (544 active, 46 skipped
upstream), 17,784 assertions. It is the
reason this engine was imported rather than hand-written, so **keep it green**. See that folder's
`README.md`.

## Re-syncing

1. Clone `microsoft/microsoft-ui-reactor` at the new tag.
2. Bump `TAG`, `COMMIT` and `IMPORTED` in `build/scripts/import-yoga.py`, then run it:

   ```bash
   REACTOR_SRC=/path/to/reactor python build/scripts/import-yoga.py all
   ```

   It rewrites both the engine and the test corpus, is idempotent, and self-verifies — it fails on any
   leftover upstream namespace, `Xunit` reference, `[Fact]`, un-renamed `LTR`/`RTL`, or missing
   provenance header.
3. Build, then run the tier-1 corpus. A conformance failure after a sync is an engine regression, not a
   test problem.

Re-sync **deliberately, never opportunistically** — upstream is a `0.1.0-preview` and moves weekly.
