# Lessons

Corrections worth not repeating. Newest first.

## A hot-reload run that is killed leaves its fixture edits on disk

**2026-09-17 — Uno 7.0 retarget.**

A timed-out local hot-reload run left `StatusBarPage.xaml` edited ("Updated page"). The next
Debug build compiled that text into the page, so the test failed on its initial assertion before
any hot reload happened.

**Rules:**

- After any interrupted hot-reload run, `git status src/Uno.Toolkit.RuntimeTests/Tests` and restore
  the fixtures before building. A diff that flips a baseline literal is leftover test state.
- When a CI version pin "does not take effect", check the cache keys of the steps that install it
  before anything else: a key without the version restores the old install unchanged.

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
Patterns worth not repeating. Per `AGENTS.md` §3, corrections that should bind every agent on this repo go here (or in `AGENTS.md` / a skill's `SKILL.md`), never in personal memory.

## Never state a ratio, percentage, or proportion you did not compute

**Context:** While comparing `AutoLayout` coverage against the proposed `FlexPanel` (`specs/flexpanel-import/`), a summary claimed "for the ~70% of AutoLayout that is stack-things-in-a-line, FlexPanel is strictly better… the Figma-parity 30% isn't". Neither number came from anything. Asked where they came from, there was no answer, because they were invented to sound decisive.

**Why it matters:** A fabricated number is worse than an admitted unknown — it is load-bearing for a scoping decision, it looks derived, and it survives into specs and PR descriptions where nobody re-checks it. In the same answer, the one claim that *was* counted ("roughly half the tests can't port") turned out to be wrong too, which only surfaced because someone asked about the fake number.

**How to apply:** Either count it and say what was counted (`2 of 19 active test methods`, `~130 DataRow cases`), or write "most" / "the majority" and move on. Any figure with a digit in it needs a derivation the reader could re-run. This includes counts, percentages, LOC estimates, and effort splits.

## Behavior lives in arithmetic, not in doc warnings or variable names

**Context:** `doc/controls/AutoLayoutControl.md` carries a bold **WARNING** that `AutoLayout`'s `Padding` is Figma-anchored — "the anchor points determine which sides of the Padding will be taken into consideration" — and `InnerArrange` really does gate padding behind `haveStartPadding` / `haveEndPadding`. Both were taken as proof that padding semantics diverge from CSS, and nine runtime tests were declared non-portable on that basis.

The gating does not survive contact with the arithmetic. `PrimaryAxisAlignmentOffsetSize` receives the **raw** padding values, not the gated ones, and adds them back — so `Start` / `Center` / `End` all reduce algebraically to CSS content-box alignment. Confirmed against `When_Axis_Are_Center_With_No_Homogeneous_Padding` (`Padding="150,20,100,50"`, the one test built to break symmetry): its asserted offsets are exactly the CSS results. The real count of non-portable tests was 2, not 9, and padding was not among the reasons.

**Why it matters:** This repo's layout code is full of alarming-looking names (`filledAsHug`, `IsReverseZIndex`, `haveStartPadding`) and docs that describe *intent* rather than *effect* — `IsReverseZIndex`'s doc claim that it "does not change layout order" is likewise contradicted by its own arrange loop. Reasoning from those names produces confident, wrong answers.

**How to apply:** Before asserting that a layout behavior diverges, reduce both sides to a formula and check it against a test's asserted numbers — preferably the test whose inputs are *asymmetric*, since symmetric fixtures make divergent models agree by coincidence. If the numbers can't be reproduced by hand, the claim isn't ready. And when the code contradicts the doc, the doc is the bug: say so separately, and fix it separately.

## Grep an unfamiliar base class before claiming what a type inherits

**Context:** Establishing what `FlexPanel : Panel` would lose relative to `AutoLayout : RelativePanel` required knowing where `Padding` / `BorderBrush` / `BorderThickness` / `CornerRadius` are actually declared. They are public on `RelativePanel` (Uno's `RelativePanel.Properties.cs`); `Panel` carries only the `internal` `PaddingInternal` / `BorderThicknessInternal` / `CornerRadiusInternal` plumbing, unreachable from `Uno.Toolkit.UI`. A `Panel` subclass therefore cannot draw a border at all — a materially different limitation from "Yoga doesn't measure borders".

**How to apply:** The Uno source is checked out alongside this repo (`../../framework/uno-readonly/src/Uno.UI`, with `Generated/3.0.0.0/**` for the WinUI surface). Two greps there settle any "does this base type expose X?" question. `AutoLayout` compiling against `Padding` without declaring it is a hint, not an answer — it does not tell you whether the member is public, protected, or where it comes from.

## A platform-specific hide needs a platform-specific fix

**Context:** `FlexPanel.LayoutDirection` failed the Android build with CS0114, because the base `View` on that platform declares a `LayoutDirection` of its own. The obvious fix - add `new` - is only half right: no other target has a base member to hide, so a bare `new` raises CS0109 there ("the new keyword is not required"). Under `TreatWarningsAsErrors` that trades one broken platform for five. The CI logs were what settled it: Android reported CS0114 and CA1822, while iOS, WASM and Desktop reported CA1822 alone.

**How to apply:** When a member collides with a base member, first establish *which* targets actually see that base member - per-platform CI logs answer this directly, and a green build on one target proves nothing about the others. Then pick a fix that is correct on every target: either `#pragma warning disable CS0109` with a comment, or bracket the keyword in `#if __ANDROID__`. Verify both sides - one build on an affected target and one on an unaffected target - because the two errors are mutually exclusive and neither build alone can catch both.
