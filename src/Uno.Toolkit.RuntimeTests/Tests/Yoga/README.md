# Vendored: Yoga conformance corpus

This folder is **vendored third-party test code**. Do not hand-edit it.

It is Meta's Yoga generated conformance suite, ported to C# by Microsoft in the
[Reactor](https://github.com/microsoft/microsoft-ui-reactor) project and imported here alongside the
engine in [`src/Uno.Toolkit.UI/Layout/Yoga/`](../../../Uno.Toolkit.UI/Layout/Yoga/README.md). MIT; full
text in [`THIRD-PARTY-NOTICES.md`](../../../../THIRD-PARTY-NOTICES.md).

| | |
|---|---|
| Upstream | <https://github.com/microsoft/microsoft-ui-reactor> |
| Tag | `v0.1.0-preview.13` |
| **Commit** | **`c9191b97c40a2e4d6bcbc72df7714184862b4d36`** |
| Upstream path | `tests/Reactor.Tests/YogaGenerated/` |
| Imported | 2026-09-04 |
| Size | 26 files · 590 test methods (544 active + 46 skipped upstream) · 17,784 assertions |

## Why this corpus is the point

The Yoga engine was imported *because* this suite exists. It is the difference between "we believe our
flexbox is spec-correct" and "the upstream conformance corpus says so". Every case traces back to a
`yoga/tests/generated/YG*Test.cpp` file, named in each file's `<summary>` doc comment.

**A red test here is an engine regression, not a test problem.** Do not adjust an expectation to make it
pass.

## Why it runs in the runtime-test host

These are pure computation over `YogaNode` / `YogaConfig` / `YogaValue` — no XAML, no UI thread (hence no
`[RunsOnUIThread]`). A standalone headless unit-test project would have run them too, but hosting them
here costs no new infrastructure *and* exercises the float math on the same Skia heads the product ships
on, which is what the cross-target parity requirement actually needs.

The engine types are `internal`; the corpus reaches them through the existing
`[assembly: InternalsVisibleTo("Uno.Toolkit.RuntimeTests")]` in `Uno.Toolkit.UI/AssemblyInfo.cs`.

## Adaptations applied on import

Scripted and idempotent, by the same [`build/scripts/import-yoga.py`](../../../../build/scripts/import-yoga.py)
that imports the engine:

1. **Provenance header** — 4 lines, offsetting each file by 4 relative to upstream.
2. `using Microsoft.UI.Reactor.Layout;` + `using Xunit;` → `using Microsoft.VisualStudio.TestTools.UnitTesting;`
   + `using Uno.Toolkit.UI;` + `using Uno.Toolkit.UI.Yoga;`
3. `namespace Microsoft.UI.Reactor.Tests.YogaGenerated;` → `namespace Uno.Toolkit.RuntimeTests.Tests.Yoga;`
4. `[Fact]` → `[TestMethod]`, and `[TestClass]` inserted (xUnit has no class attribute).
   `[Fact(Skip = "…")]` → `[TestMethod]` + `[Ignore("…")]` — see "The 46 skipped tests" below.
5. `Assert.Equal(` → `YogaAssert.Equal(` — see [`YogaAssert.cs`](./YogaAssert.cs). All 17,784 assertions
   are the same shape (`float` literal vs a `Layout{X,Y,Width,Height}`), so routing them through one
   method keeps exact-comparison semantics *and* leaves a single place to add tolerance if a Skia head
   ever needs it.
6. `FlexLayoutDirection.LTR`/`.RTL` → `.LeftToRight`/`.RightToLeft`, matching the engine-side rename.

## Deliberately *not* changed

Upstream file names, class names and method names are kept verbatim, so a failure maps straight onto the
upstream file. In particular, method names stay in Yoga's style (`Wrap_Child`) rather than this repo's
`When_X_Then_Y` convention.

Reactor breaks its own `Yoga*` file/class naming in **5 of 26 files**. That inconsistency is upstream's,
and it is preserved intentionally — do not "fix" it, or the fork silently diverges:

| Yoga C++ origin | file | class |
|---|---|---|
| `YGAlignContentTest.cpp` | `YogaAlignContentTest.cs` | `FlexAlignContentTest` |
| `YGAlignItemsTest.cpp` | `YogaAlignItemsTest.cs` | `FlexAlignItemsTest` |
| `YGAlignSelfTest.cpp` | `YogaAlignSelfTest.cs` | `FlexAlignSelfTest` |
| `YGFlexDirectionTest.cpp` | `YogaFlexDirectionTest.cs` | `FlexDirectionTest` |
| `YGJustifyContentTest.cpp` | `YogaJustifyContentTest.cs` | `FlexJustifyContentTest` |

The other 21 files match their file name.

`YogaAssert.cs` is **ours**, not vendored — it is the only hand-written file in this folder. The
scoped `.editorconfig` here keeps the imported files at their upstream 4-space indentation (the repo
default is tabs) so a formatter run cannot silently rewrite 1.7 MB of corpus and destroy the upstream
diff; `YogaAssert.cs` is carved out and follows repo style.

## The 46 skipped tests

46 of the 590 cases arrive as `[Fact(Skip = "Skipped in upstream Yoga (GTEST_SKIP)")]` — they are
skipped in Meta's own C++ suite, so they were never active anywhere. They import as `[TestMethod]` +
`[Ignore(...)]`, which mirrors that upstream state rather than deactivating anything of ours (the
AGENTS.md rule against `[Ignore]` is about the latter). Enabling them would import 46 known-failing
cases. Concentrated in `YogaIntrinsicSizeTest` (28) and `YogaPercentageTest` (5) — mostly `fit-content`
/ `max-content` / `stretch` sizing keywords.

## Running just this corpus

Build a sample head for a Skia target and pass the runtime-test switch, filtered on the namespace:

```bash
dotnet build samples/Uno.Toolkit.Samples.Material/MaterialSampleApp.csproj -c Debug -f net10.0-desktop
cd samples/Uno.Toolkit.Samples.Material/bin/Debug/net10.0-desktop

export UNO_RUNTIME_TESTS_OUTPUT_PATH=/abs/path/to/results.xml   # required - see below
export UNO_RUNTIME_TESTS_RUN_TESTS='{"Filter":{"Value":"Tests.Yoga."}}'
dotnet MaterialSampleApp.dll --runtime-tests=$UNO_RUNTIME_TESTS_OUTPUT_PATH
```

Two things that will otherwise waste your time:

- **`UNO_RUNTIME_TESTS_OUTPUT_PATH` is required**, even though `--runtime-tests=` already names the file.
  Without it the app starts, prints *"Application has not been configured with output destination,
  aborting runtime-test embedded runner"*, and then just sits there.
- **Filter on `Tests.Yoga.`, not `Yoga`.** The filter matches the test class's `FullName`, and 5 classes
  are named `Flex*Test` (see the table above) - the namespace is what they all have in common.

The whole corpus runs in about 6 seconds. `build/workflow/scripts/linux-skia-runtime-tests.sh` is the
same entry point wrapped in `xvfb-run` for CI. Re-syncing is documented in the engine folder's README.
