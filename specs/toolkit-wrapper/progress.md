# Toolkit sample wrapper

## Design

Port the Uno Themes `ThemesSampleApp` secondary-AssemblyLoadContext host to
`samples/Uno.Toolkit.Samples.ThemeWrapper/ToolkitSampleApp.csproj`. Host Material,
Cupertino, and Simple with isolated Toolkit/theme assemblies, shared Uno runtime,
and the same switching, teardown, deep links, and hosting smoke checks. Include
shared sample assets and guest WASM assembly payloads. Preserve standalone heads.
Deploy the wrapper for PR previews and main pushes; build guests before packaging.
No package version changes or new third-party dependencies are intended.

## Plan

- [x] Inspect reference host, repository instructions, and deployment entry points.
- [x] Port host and adapt guest startup/access to each guest's own application.
- [x] Include wrapper in solutions and update CI build/deployment artifacts.
- [x] Build Release desktop and publish Release WASM with all guests.
- [x] Run hosting smoke (switch/reload/unload all guests) and relevant runtime tests.
- [x] Review changes and document commands, results, and remaining limitations.
- [x] Resolve desktop ALC reclamation failures and verify with unchanged strict smoke.
- [ ] Obtain a warning-free full Release build (sample warnings remain).

## Review

- Desktop wrapper built successfully. Initial full sample compilation emitted nullable
  warnings; incremental wrapper rebuild was warning-free. Do not equate incremental
  output with a warning-free clean solution build.
- Material Release runtime suite: 361 passed, 0 failed, 11 skipped by existing test gates.
  Log: `/tmp/toolkit-wrapper-runtime-tests.log`; results: `/tmp/toolkit-wrapper-runtime-tests.xml`.
- Initial desktop hosting smoke failed ALC reclamation. The follow-up fix below now passes
  the unchanged strict smoke twice in fresh processes; assertions remain enabled.
- First WASM guest run hit MSB4166 (build worker exited); retry with `-m:1` built all three
  guests. Final Release WASM guest builds and wrapper publish succeeded. Guest builds
  emitted 103/102/102 warnings (Material/Cupertino/Simple); the wrapper publish emitted
  no warnings. A warning-free full build has not been achieved.
- Port corrections: guest-owned App.Instance; synchronous guest launch; package fonts
  and manifests from guest content; Lottie isolation and case-insensitive assembly lookup.
- The host retains the reference implementation's framework cache cleanup mitigations.
  WASM/Debug ALC collection is diagnostic; Release desktop collection is required.

### Browser verification

Release WASM publish succeeded with the workflow flags plus `-m:1` locally.
Served the published `wwwroot` on localhost and ran `?smoke` in headless Chrome:
`[HOSTING-SMOKE] RESULT: PASS`. All three guest manifests and raw IL payloads exist;
Roboto/Inter fonts and weight manifests are included. `?app=material` renders the
picker and Material NavigationBar sample correctly (screenshot inspected).
The missing-guest smoke request intentionally returns a 404. WASM collection remains
non-authoritative, following the reference host's documented limitation.

Browser smoke repeated against the final publish: passed.
Final browser log: `/tmp/toolkit-wrapper-final-browser.log`.
Preview: `/tmp/toolkit-wrapper-material.png`.

### Initial desktop retention diagnosis

The hosting smoke reproduced failures before adding cleanup. Heap dumps of the empty
host identified two independent strong roots in the pinned Uno runtime:

1. `DependencyObject._dpChangedEventArgsPool` retains `PropertyInternal`, which points
   to guest-owned dependency properties even after their values are cleared. Replacing
   the idle pool during UI-thread teardown preserves its capacity and drops those roots.
2. A platform host caches the application factory. The original factory closure holds
   the guest constructor and session after construction. The factory must release those
   captures after its single invocation.

The pool workaround lives only in the wrapper's existing reflection cleanup layer;
shipping Toolkit libraries are unchanged. Remove it when Uno clears the retained
property on pool return. Desktop and WASM installed assemblies were inspected for the
pool's field layout and rent/return semantics; the untrimmed wrapper retains metadata.

These initial fixes allowed the first explicit unload to collect. Cross-theme/final unloads still
failed the strict Release smoke test on macOS, reproduced on .NET 10.0.3 and 10.0.12.
Empty-host heap inspection found a native runtime strong handle to the remaining ALC,
without an ordinary managed guest root or guest code on the stack. This narrows the
investigation but does not establish the remaining root cause. Experimental delays,
extra collection sweeps, diagnostic pauses, and inlining changes were removed.

Constraint: Linux hosting smoke could not be run locally (Docker daemon unavailable).
Initial impact: desktop unloading retained guest memory and failed verification.
The resolution below now passes locally; the strict Linux CI hosting smoke gate remains
required before treating the cross-platform behavior as fully verified. Browser collection
diagnostics do not prove that memory is reclaimed. No test assertions were weakened.

### Initial validation commands and logs

```bash
dotnet build samples/Uno.Toolkit.Samples.ThemeWrapper/ToolkitSampleApp.csproj -c Release -f net10.0-desktop -p:TargetFrameworkOverride=desktop -p:GeneratePackageOnBuild=false -m:1
dotnet samples/Uno.Toolkit.Samples.ThemeWrapper/bin/Release/net10.0-desktop/ToolkitSampleApp.dll --smoke
bash build/workflow/scripts/build-wasm-guest-heads.sh Release -p:CompressionEnabled=false -m:1
dotnet publish samples/Uno.Toolkit.Samples.ThemeWrapper/ToolkitSampleApp.csproj -c Release -f net10.0-browserwasm -p:TargetFrameworkOverride=browserwasm -p:CompressionEnabled=false -m:1
```

- `/tmp/toolkit-wrapper-final-desktop-build.log`: passed, incremental build, 0 warnings/errors.
- `/tmp/toolkit-wrapper-final-desktop-smoke.log`: failed, ALC collection assertions.
- `/tmp/toolkit-wrapper-final-wasm-guests.log`: passed, sample warnings described above.
- `/tmp/toolkit-wrapper-final-wasm-publish.log`: passed, no warnings emitted.
- Workflow `actionlint`, Azure YAML parsing, and guest script `bash -n`: passed.
- Missing-payload target check: failed as expected with an actionable missing guest error.
- Solutions preserve standalone runtime-test artifacts and exclude the wrapper from
  mobile/package-only build configurations. No package dependencies or version pins added.
- Initially published as a draft with the reclamation failure documented. The subsequent
  fix below relates to [uno#24581](https://github.com/unoplatform/uno/issues/24581);
  the PR preview workflow deploys the combined wrapper.

### PR CI follow-up

- [x] Inspect PR #1654 checks and distinguish current failures from canceled runs.
- [x] Fix MD012 in these notes and run the exact CI Markdown validation command.
- [x] Fix desktop guest reclamation reproduced by the Linux CI smoke test; validate locally.
- [x] Monitor replacement runs and address additional failures without weakening checks.

The Linux smoke on build 236378 reproduces the same reclamation failure observed on
macOS. This is a cross-platform hosting issue, not a macOS-only limitation.

Compared the exact diff of [Uno Themes PR #1729](https://github.com/unoplatform/Uno.Themes/pull/1729):
the Uno 7 cleanup arity/dying-ALC fix, guest-owned application resource lookup, and full
shared asset packaging are already present here. Toolkit samples do not use
ShowMeTheXAML, so its entry-assembly initialization fix does not apply. This parity
check does not establish that the remaining guest reclamation failure is fixed.

Review follow-up: renamed the project directory to `Uno.Toolkit.Samples.ThemeWrapper`
and updated solutions, workflows, and README commands. Narrowed reflection lookup and
cleanup-invocation catches to expected reflection exceptions. Kept the intentional
collection passes and per-guest `App.Instance`; replacing either as suggested by the
code-quality bot would break the host's lifecycle/resource-lookup contract.

The earlier review changes built in Release desktop with 0 warnings/errors. Strict smoke still
failed reclamation (`/tmp/toolkit-ci-review-smoke.log`). Longer waits, extra cleanup passes,
no-inlining, path-based assembly loading, clearing the guest instance, and progressively
simplified guest UIs did not produce a reliable fix. All diagnostic code was removed;
no assertions or production GC settings were weakened. Those early experiments did not
identify the root cause; see the resolution below. CI on `06644975` passed all standalone
desktop/mobile/WASM builds, packages,
Markdown/spelling, hot-reload tests, and CodeQL; wrapper publishing was still running
when these results were recorded.

### ALC reclamation resolution

Explicit enumeration of thread-static fields identified another retained reference:
`Control._isEnabledChangedEventArgs.SourceEvent` held a pooled
`DependencyPropertyChangedEventArgs` whose reused `PropertyInternal` referred to a guest
property. Replacing the shared event-args pool did not release this separate alias.
The wrapper now clears the control cache on its UI thread after teardown; Uno lazily
recreates it for the next IsEnabled change. No shipping Toolkit library was changed.

The remaining intermediate failures came from starting a new guest before the prior
context had finished collection. Uno's binding resolver could then cache the old guest's
property getter in the new app (related to [uno#24581](https://github.com/unoplatform/uno/issues/24581)).
Release desktop now yields out of the teardown continuation and allows up to five
collection/finalization passes before booting another guest. If collection cannot finish,
it leaves the host empty and reports a load error that lets the user retry. Debug/WASM retain diagnostic
collection because their existing runtime limitations prevent this guarantee.

Red/fix/green: `/tmp/toolkit-ci-tls-red.log` fails with the original strict smoke;
`/tmp/toolkit-ci-alc-fixed-smoke1.log` and `...smoke2.log` both pass in fresh processes
with normal runtime settings and unchanged assertions. The final incremental Release
desktop build has 0 warnings/errors (`/tmp/toolkit-ci-alc-fixed-build.log`). No diagnostic
branches, delays in the test, JIT overrides, or dependency changes were retained.

The final Release WASM guest builds and wrapper publish passed, and the published
browser hosting smoke passed (`/tmp/toolkit-ci-fixed-browser.log`). WASM collection
remains diagnostic. The deployed preview also passed at the pre-fix review commit.
Follow the latest
[PR checks](https://github.com/unoplatform/uno.toolkit.ui/pull/1654/checks) for Linux CI
verification and the preview deployment.

### Final CI verification

[Azure build 236394](https://dev.azure.com/uno-platform/Uno%20Platform/_build/results?buildId=236394)
passed on `c3e2179c`, including the strict Linux hosting smoke, all sample platform
builds, packages, documentation checks, and runtime tests. CI validated 372 desktop
runtime test cases and 48 hot-reload cases. GitHub CodeQL and preview deployment also
passed. The final deployed preview hosting smoke passed
(`/tmp/toolkit-pr-final-deployed-browser.log`). The follow-up comment corrections align
the Uno version and Lottie sharing documentation with the existing implementation.

WASM collection remains diagnostic, and full sample builds still emit the warnings
documented above. Those limitations are not hidden by the successful CI result.

## Uno 7.0.0-dev.2285 retarget (2026-10-21)

Uno `7.0.0-dev.1664` removed the `Uno.WinUI.Lottie` package (unoplatform/uno#25117);
`LottieVisualSource` now ships in `Uno.UI`. Uno.Themes moved past it in
unoplatform/Uno.Themes#1760, and Toolkit follows with Uno.Sdk `7.0.0-dev.2285` and
Uno.Themes `9.0.0-dev.56`. Themes `9.0.0-dev.37` still depended on `Uno.WinUI.Lottie`, which
`Uno.Toolkit.WinUI.Material` / `.Cupertino` passed on to consumers.

- No Release guest ships `Uno.UI.Lottie` any more, so the `!Uno.UI.Lottie` rule was removed
  from `GuestSharedAssemblies.txt`.
- Sample heads: Android host base types come from `Uno.UI.Runtime.Android`, and
  `UnoSplashScreen` uses `BackgroundColor` (`Uno.Resizetizer0001`).

### Not fixed (upstream): hosted ProgressRing animations never load

Same as Uno.Themes' `specs/05-alc-wrapper-app/upstream-issues.md` § 6: `Uno.UI` resolves
`embedded://uno.material.winui/...` with `Assembly.Load` from the default ALC, which cannot see
the guest's isolated theme assembly (`FileNotFoundException`, `Failed to load animation`).
This is not a regression: `main` (`7.0.0-dev.1574`, with the `!Uno.UI.Lottie` rule) logs the
same two failures for hosted Material (Win32 Release, `--app=material`):
`Failed to load animation: embedded://uno.material.winui/Uno.Material.WinUI.Assets.MaterialDeterminate.json`
and `...MaterialIndeterminate.json`, each with `FileNotFoundException: Could not load file or
assembly 'uno.material.winui'`. Standalone heads are unaffected.

### Not fixed (upstream): a reloaded Cupertino guest's ALC is not reclaimed (uno-private#2469)

On Win32 Release, the smoke failed at "Reloading Cupertino": the previous Cupertino ALC stayed
alive through all five collection passes. Deterministic: `main` passes 2/2, the branch and the
dependency-bump commit alone fail 2/2. The Linux CI smoke (build 240718) shows the same single
case: Material and Simple are reclaimed within the retries; only the reloaded Cupertino is not.
Uno.Themes sees comparable retention on Linux (unoplatform/uno-private#2469).

What is known about the cause:

- The wrapper's reflection sweeps of private Uno fields (`GuestAppLoader.Sweeps.cs`) still find
  every field on 2285: no "was not reachable" / "were not found" warning in any Win32 or Linux
  smoke log.
- No retention root path was recorded (no heap dump). The bisect cannot separate the Uno change
  from the Themes change, because Themes `9.0.0-dev.37` cannot run on Uno 2285.

Unlike Themes, the Toolkit loader refuses to boot while an unloaded ALC is alive (the uno#24581
guard above), so the wrapper got stuck rather than only failing the reclamation check. Until
uno-private#2469 is fixed (`6e38489e`, narrowed by `f5a04959`):

- The loader tracks every unloaded ALC. Release desktop still throws `GuestAppLoadException`
  unless the only survivor is one Cupertino ALC (`_knownRetainedGuests`, at most
  `_maxKnownRetainedAlcs`). Then it logs one warning naming the retained guest and the uno#24581
  risk, and boots the next guest.
- The smoke's reclamation check stays authoritative on Release desktop. The known retention
  passes and adds `KNOWN-ISSUE uno-private#2469` to the RESULT line. With the set emptied, the
  smoke fails again at "Reloading Cupertino" (checked locally).
- On Win32 the retained Cupertino ALC is collected once the next guest (Simple) is hosted.

- [ ] Once uno-private#2469 is fixed: empty `_knownRetainedGuests` and remove the
  `TODO(uno-private#2469)` code in `GuestAppLoader.cs` and `GuestHostingSmoke.cs` (revert
  `f5a04959` and `6e38489e`), and drop the exception from `README.md`.

### Verification

- Desktop Release solution build: 0 errors, libraries warning-free. Remaining warnings are the
  existing sample nullable warnings and `UXAML0007` from the runtime-test engine.
- Material runtime suite (Skia desktop, Release): 957 passed, 0 failed, the same as `main`.
- Android heads (Debug) and iOS/Windows libraries build. The two `WMC1500` warnings in the
  Windows Material library are also present on `main`.
- Packed `Uno.Toolkit.WinUI.Material` / `.Cupertino` depend on `Uno.Material.WinUI` /
  `Uno.Cupertino.WinUI 9.0.0-dev.56` and `Uno.WinUI 7.0.0-dev.2285`, with no `Uno.WinUI.Lottie`.
- Standalone Material, Cupertino and Simple heads launch without errors.
- Win32 Release hosting smoke passes with the known issue reported. CI build 240718 (before the
  narrowing) passed the hosting smoke, hot-reload, desktop runtime test, WASM, Android and iOS
  jobs.
