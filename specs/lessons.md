# Lessons

Corrections worth not repeating. Newest first.

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
