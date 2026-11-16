# Uno Themes compatibility

Adapt Toolkit's existing 10.0 development line to the latest breaking changes in
`../uno.themes`. Keep Toolkit's version unchanged and add no new dependencies.

## Plan

- [x] Compare upstream breaking changes and audit Toolkit APIs/resources.
- [x] Verify the published package contains the latest functional changes and update existing Themes version pins.
- [x] Add and run regression coverage before replacing removed Simple font resources.
- [x] Migrate affected styles and document DefaultFontFamily, DefaultSpacing, and density semantics.
- [x] Build affected libraries and sample heads in Release for Desktop and WebAssembly; run relevant runtime tests.
- [x] Review the final diff and prepare the requested PR against main.

## Scope

- Upstream #1701 separates DefaultSpacing from the Density mode.
- Upstream #1710 replaces SimpleFontFamily and root typeface tokens with DefaultFontFamily.
- Upstream #1707/#1716 adds runtime font overrides and theme-aware font reads.
- Upstream #1715 makes generated spacing/shape reads dynamic; Toolkit has no direct Space*/Radius* reads to migrate.
- Preserve Toolkit lightweight styling keys and existing public API.

## Documentation follow-up

- [x] Remove exact prerelease package versions from public migration guidance.
- [x] Link shared BaseTheme API usage to Uno Themes; retain Toolkit setup, migration impact, and control-specific caveats.
- [x] Record the documentation rule in shared agent guidance and lessons, verify links, and prepare the PR update.

Follow-up validation: all 13 Uno Themes cross-reference targets/anchors and local Markdown links in the three revised guides resolve against the checked-out sources. `git diff --check` passes. This follow-up changes documentation only; the previous build/runtime results remain applicable and were not rerun.

## Review and validation

Validated on 2026-09-08 with .NET SDK 10.0.400.

The latest published package, `8.0.0-dev.15`, records upstream commit
`47d42c185764907a10d0dd5511eb1f0eddd8b6f1` in its nuspec. Comparing that commit
with upstream HEAD `b592b87c` shows only the 9.0 version.json change. Both existing
Themes pins now use that published package; Toolkit's `10.0-dev.{height}` is unchanged.

### Runtime tests

The new `ThemeCompatibilityTests` ran against the upgraded dependency before
the XAML fix: 4 Simple font cases failed while resolving the removed resource,
and 3 spacing cases passed. After the fix, all 7 passed.

| Requirement | Evidence |
| --- | --- |
| Simple defaults and Divider/NavigationBar aliases in Light/Dark | `When_SimpleStylesLoaded_FontsResolveDefaultFontFamily` (2 passed) |
| Scoped font override on Card/CardContentControl/Chip | `When_SimpleDefaultFontFamilySet_DirectControlStylesUseScopedFont` (2 passed) |
| Spacing base scaled by each density mode | `When_MaterialDefaultSpacingSet_DensityScalesInheritedTokens` (3 passed) |
| Existing theme/control regressions | `ThemeInitTests` (2), `CardContentControlTests` (3), `ChipGroupTests` (21), `NavigationBarTests` (8), all passed |

Total: **Passed: 41, Failed: 0, Skipped: 0** on the Material Desktop runtime host.
Each class was run separately with
`UNO_RUNTIME_TESTS_RUN_TESTS='{"Filter":{"Value":"<class>"},"Attempts":1}'`
and `MaterialSampleApp.exe --runtime-tests=<results.xml>`. A combined filter did
not select the intended suite in this runner, so its duplicate 7 cases are not
included in the total. Logs and XML results are in ignored
`artifacts/themes-compatibility/`.

### Builds and limits

- Release Material Desktop and WebAssembly builds passed; Simple and Cupertino Desktop builds passed.
- Standalone Release Material Markup (including base Markup/Material), Simple, and Cupertino library builds: zero warnings/errors.
- Sample warnings: Material Desktop 100; Material WebAssembly 113; Simple/Cupertino Desktop 99 each. All warnings match clean `origin/main` (`d88a0c12`) builds in an isolated worktree, including message text and locations. The repository-wide zero-warning sample requirement remains unmet on main; no warning suppressions or unrelated fixes were added here.
- Builds used `dotnet build <project> -c Release -p:TargetFrameworkOverride=<desktop|browserwasm>` and explicit `-f net10.0-<desktop|browserwasm>` for samples. Sample builds also used `-p:DisableMobileTargets=true`.
- WebAssembly was build-validated; runtime execution was on Desktop. Mobile/WinUI and Debug hot-reload runs were not performed.
- XAML Styler was run on all five changed files. Its unrelated whole-file formatting changes were removed to preserve the minimal nine-reference diff.
- Generated XAML build output was excluded from the change; final `git diff --check` passed.
- Independent source/documentation/test review found no blocking issues. Toolkit-specific font aliases retain their override keys; their runtime regeneration limitation is documented.

## Full-branch documentation review

- [x] Review all public-documentation and XML-summary changes against origin/main.
- [x] Remove inherited-member catalogs from getting-started pages and XML summaries; keep upstream links, Toolkit setup, migration actions, and control caveats.
- [x] Verify all 13 upstream cross-reference targets/anchors and local Markdown links; prepare the documentation follow-up for publication.

`git diff --check` passes. Changes are limited to documentation and XML comments; no executable code changed, so builds and runtime tests were not rerun.

## PR #1636 review follow-up

- [x] Inventory all review threads, including resolved feedback and release coordination.
- [x] Compare Toolkit spacing generation with plain MaterialTheme for identical inputs, using Border.Padding probes.
- [x] Replace the local descendant iterator with the existing Toolkit extension.
- [x] Build Release Desktop and WebAssembly and run ThemeCompatibilityTests.
- [x] Audit CI notices and issue-link feedback; record remaining publication steps and validation results.

Keep the PR targeting main. Merge, servicing/10.0 backport publication, and notifying
Martin after merge remain release coordination steps requiring explicit authorization.

### Review follow-up validation (2026-09-09)

- `ThemeCompatibilityTests`: **Passed: 7, Failed: 0, Skipped: 0** on macOS Desktop, including all three density cases compared against plain MaterialTheme and all four existing font cases. The reference padding must also be positive so unresolved tokens cannot produce a vacuous comparison.
- Standalone Release runtime-test project build passed with **0 warnings, 0 errors**.
- Release Material sample builds passed for Desktop and WebAssembly (.NET SDK 10.0.103): 100 and 113 warnings respectively, matching the counts in the prior baseline audit above; zero errors. No warning suppressions were added. WebAssembly runtime tests were not rerun.
- Launch from `samples/Uno.Toolkit.Samples.Material/bin/Release/net10.0-desktop`:

  ```sh
  UNO_RUNTIME_TESTS_RUN_TESTS='{"Filter":{"Value":"ThemeCompatibilityTests"},"Attempts":1}' UNO_RUNTIME_TESTS_OUTPUT_PATH="$PWD/results.xml" dotnet MaterialSampleApp.dll --runtime-tests="$PWD/results.xml"
  ```

  The initial launches without `UNO_RUNTIME_TESTS_OUTPUT_PATH` produced no results; setting the output environment variable required by the embedded runner enabled execution. Actual logs/XML are in ignored `artifacts/themes-review/`.
- Review changes refactor existing tests; no product behavior changed and no test cases were removed.
- Latest Azure CI run 232693 passes, superseding the older failure notices. Preview deployment is blocked by Azure's staging-environment quota. Copilot setup fails on a missing `Uno.Toolkit.UITest.csproj` solution entry also present on `origin/main`; neither failure calls for changing theme compatibility code.
- The PR template permits GitHub or internal issues and does not require a Toolkit-repository issue. Verified related upstream issues: [Uno.Themes#1709](https://github.com/unoplatform/Uno.Themes/issues/1709) and [Uno.Themes#1688](https://github.com/unoplatform/Uno.Themes/issues/1688). An updated PR body is prepared in ignored `artifacts/themes-review/pr-body.md`.
- Publication, review-thread updates, merge/backport, and post-merge notification have not been performed.
