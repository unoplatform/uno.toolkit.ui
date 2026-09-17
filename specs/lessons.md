# Lessons

Corrections worth not repeating. Newest first.

## A Skia mobile head did not run your mobile asset before Uno 7 — now it does

**2026-09-17 — Uno 7.0 retarget.**

The first plan for the mobile port kept the native `ExtendedSplashScreen.Android.cs`/`.iOS.cs`
implementations alive and set out to make them work under Skia. The maintainer pointed out that
Skia Android should keep using the plain Skia path the toolkit already used. The reason is in the
Uno SDK, not in this repo: in Uno 6 a Skia Android/iOS head replaced every Uno-referencing
library's `-android`/`-ios` asset with its plain `netX.0` build (`RuntimeAssetsSelectorTask`). Every
Skia mobile user of the toolkit has therefore always run the plain build — `not_mobile` XAML, no
`__ANDROID__`/`__IOS__` code. Uno 7 commit `bc6254848a` removed that swap, so the mobile assets now
run for the first time on Skia.

**Rules:**

- When deciding what a mobile TFM build should do on Uno 7, start from what the plain build did:
  that is the behavior Skia mobile users already have. Keep a native-only path only as a justified
  OS-level exception, and list it.
- Before reasoning about which asset of a package an app loads, read the SDK task that selects
  assets for the Uno version in use. NuGet's `project.assets.json` is not the final answer on Uno 6.

## Verify with the command CI runs, not an equivalent-looking one

**2026-09-08 — Uno 7.0 retarget.**

A `uno.themes` retarget built with **0 errors** via
`dotnet build Uno.Themes.sln -c Release -p:TargetFrameworkOverride=desktop`, then failed
immediately in CI with `CS0413` and, behind it, 29 XAML errors.

Cause: MSBuild resolves `msbuild-sdks` from the `global.json` nearest the **entry point**.
A solution build at the repo root reads the root `global.json`; CI's
`dotnet publish src/samples/<head>/<head>.csproj` reads `src/samples/global.json`. That repo has
both, and only the root one had been bumped — so the libraries compiled against Uno 7 and the
sample heads against Uno 6. Same source, same flags, opposite results.

The `CS0413` was a real portability bug the wrong-SDK build happened to expose:
`where T : class, DependencyObject` is illegal once `DependencyObject` is a class (CS0450), but
plain `where T : DependencyObject` breaks `child as T` while it is still an interface (CS0413).
A type pattern (`child is T typed`) needs no class constraint and compiles either way.

**Rules:**

- Before claiming a build is green, run the *literal* command from the pipeline YAML. For this
  repo that is `dotnet publish samples/<head>/<head>.csproj -c Release -f net10.0-desktop
  -p:TargetFrameworkOverride=desktop` for the sample jobs, and the `Uno.Toolkit-packages.slnf`
  build for the Packages job — not a convenient solution-wide build.
- `grep -rn "global.json"` before an SDK bump. Nested `global.json` files are invisible to a
  solution build and authoritative for a direct project build.
- When a local build and CI disagree, dump `obj/**/project.assets.json` and compare resolved
  package versions before theorising. It named the exact culprit in one step here.

## Read the shipped package, not the source checkout

**2026-09-08 — Uno 7.0 retarget.**

`D:\Work\uno` was ~5 weeks behind the build the toolkit consumes. `uno.winui.nuspec` for
`7.0.0-dev.679` pins commit `d1710cafb14428664bc3eadd5bd6d4ee49cc7395`. Greps of the working tree
returned types that are **not** in the shipped assemblies (`IDependencyObjectStoreProvider`,
`Generic.Native.xaml`, `Style.IsNativeStyle`), which is exactly backwards from what the build
errors said.

**Rule:** for "was this API removed?", read the assembly in `D:\Packages\NuGet\<pkg>\<version>\`
or `git show <nuspec-commit>:<path>`. A working-tree grep is a hypothesis, not evidence.

## Compiling is not the same as working

**2026-09-08 — Uno 7.0 retarget.**

Two separate bugs in one day that a green build would have hidden:

- `xmlns:toolkit="using:Uno.UI.Extras"` compiled fine — the namespace exists, holding only a
  generated `GlobalStaticResources` — while the XAML generator emitted references to
  `Uno.UI.Extras.ElevatedView`, a type that does not exist. It would have failed at runtime, in
  every affected template.
- Adding `SkiaSharp.Views.Uno.WinUI` made `ShadowContainer` compile, but that package is itself
  Uno-6-built and binds `Uno.UI.Toolkit`, so the app died at startup with a `FileNotFoundException`
  before a single runtime test ran.

**Rules:**

- After a namespace/xmlns migration, disassemble the output and check the emitted type references
  actually resolve — a `TypeReference` to a non-existent type is not a build error.
- Check what a dependency binds to, not just that it restores:
  list its `AssemblyReferences` and look for the assemblies the new major version removed.
- Launch the app (`--runtime-tests=`) before claiming a port works. A clean build proved nothing
  here.

## Documentation ownership

- Public migration guidance should describe release-level behavior without pinning the explanation to an exact prerelease NuGet build. Package provenance belongs in PR and validation notes.
- Inherited APIs do not need a second usage guide in Toolkit. Keep Toolkit setup and control-specific caveats locally, and link shared `BaseTheme` behavior to Uno Themes so changes have one documentation owner.
- Apply documentation ownership decisions to the entire branch diff, including XML summaries. Avoid maintaining lists of inherited members in Toolkit; link to their owner and retain only local integration details.

## Compatibility test ownership

- Test Toolkit's inheritance of upstream token generation against the upstream theme with identical inputs. Keep arithmetic tables and default asset names in the owning Themes tests.
- Reuse Toolkit's existing visual-tree helpers in runtime tests instead of introducing a second traversal implementation.
