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
- [ ] Resolve remaining desktop ALC reclamation failures and obtain a clean full Release build.

## Review

- Desktop wrapper built successfully. Initial full sample compilation emitted nullable
  warnings; incremental wrapper rebuild was warning-free. Do not equate incremental
  output with a warning-free clean solution build.
- Material Release runtime suite: 361 passed, 0 failed, 11 skipped by existing test gates.
  Log: `/tmp/toolkit-wrapper-runtime-tests.log`; results: `/tmp/toolkit-wrapper-runtime-tests.xml`.
- Final desktop hosting smoke renders all themes but fails ALC reclamation assertions.
  The unresolved retention and investigation are recorded below; assertions remain enabled.
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

### Desktop retention diagnosis

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

These fixes allow the first explicit unload to collect. Cross-theme/final unloads still
fail the strict Release smoke test on macOS, reproduced on .NET 10.0.3 and 10.0.12.
Empty-host heap inspection found a native runtime strong handle to the remaining ALC,
without an ordinary managed guest root or guest code on the stack. This narrows the
investigation but does not establish the remaining root cause. Experimental delays,
extra collection sweeps, diagnostic pauses, and inlining changes were removed.

Constraint: Linux hosting smoke could not be run locally (Docker daemon unavailable).
Impact: desktop unloading can retain guest memory, and desktop verification remains
failing. Mitigation/follow-up: retain the strict Linux CI hosting smoke gate and resolve
the remaining retention before treating this work as fully verified. Browser collection
diagnostics do not prove that memory is reclaimed. No test assertions were weakened.

### Final validation commands and logs

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
- PR preparation: publish as a draft because desktop reclamation and a warning-free
  full Release build remain unresolved. No related issue was supplied; association
  remains pending. Creating the PR will trigger the configured preview workflow.

### PR CI follow-up

- [x] Inspect PR #1654 checks and distinguish current failures from canceled runs.
- [x] Fix MD012 in these notes and run the exact CI Markdown validation command.
- [ ] Fix desktop guest reclamation reproduced by the Linux CI smoke test.
- [ ] Monitor replacement runs and address additional failures without weakening checks.

The Linux smoke on build 236378 reproduces the same reclamation failure observed on
macOS. This is a cross-platform hosting issue, not a macOS-only limitation.

Compared the exact diff of [Uno Themes PR #1729](https://github.com/unoplatform/Uno.Themes/pull/1729):
the Uno 7 cleanup arity/dying-ALC fix, guest-owned application resource lookup, and full
shared asset packaging are already present here. Toolkit samples do not use
ShowMeTheXAML, so its entry-assembly initialization fix does not apply. This parity
check does not establish that the remaining guest reclamation failure is fixed.
