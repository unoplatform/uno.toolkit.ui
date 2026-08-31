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

## Notes for future work

- Headless runtime tests locally need **both** `UNO_RUNTIME_TESTS_RUN_TESTS` and `UNO_RUNTIME_TESTS_OUTPUT_PATH` env vars in addition to `--runtime-tests=<path>`; with only the filter var set, the app idles at the runner UI and never starts.
- Placeholder regeneration is triggered by `SizeChanged` of both the ContentPresenter *and* the overlay Canvas — the overlay is collapsed while not loading, so only its own 0→W size change reliably signals "visible and arranged" when loading restarts.
- `LeakTest` is fully `[Ignore]`d (#429), so no SkeletonView row was added there.
