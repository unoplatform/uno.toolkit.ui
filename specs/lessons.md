# Lessons

Corrections worth not repeating. Newest first. Per `AGENTS.md` §3, corrections that should bind
every agent on this repo go here (or in `AGENTS.md` / a skill's `SKILL.md`), never in personal memory.

## Never name an `AppBarButton` template part `Content` unless it must show `Icon ?? Content`

**2026-09-29 — Simple NavigationBar primary commands (#1652).**

An `AppBarButton` in `NavigationBar.PrimaryCommands` whose `Content` was a `UIElement` rendered an
empty slot under the Simple theme, while a string `Content` and an `Icon` rendered fine. Uno's
`AppBarButton` (`AppBarButton.uno.cs`, `SetupContentUpdate`) looks up the template part named
`Content` and force-sets it to `Icon ?? Content` on every change of either property, so WinUI's
icon-only template still shows something when only `Content` is set. The toolkit's own
`SimpleAppBarButtonStyle` in `Uno.Toolkit.Simple/Styles/Controls/NavigationBar.xaml` used that name
for the icon presenter inside the `Viewbox` that collapses when `Icon` is null; with no `Icon`,
Uno moved the user's `Content` into that hidden presenter, and a `UIElement` can only have one
visual parent, so it vanished from the visible `ContentPresenter`. Strings passed every sample and
test because a string can be shown by two presenters at once. Same root cause as
unoplatform/Uno.Themes#1735.

**Rules:**

- In an `AppBarButton` template, the part named `Content` is owned by Uno. Name the icon presenter
  `IconPresenter` (and the content presenter `ContentPresenter`); only an icon-only template that
  deliberately wants the `Icon ?? Content` fallback may keep the name `Content`.
- A template that splits a control's properties across several presenters needs a runtime test that
  puts a `UIElement`, not a string, into each of them — strings hide re-parenting bugs.
- Before naming a part in a re-templated framework control, grep the Uno implementation for
  `GetTemplateChild("...")`; Uno adds lookups WinUI does not have.
- The toolkit ships its own `SimpleAppBarButtonStyle`, same key as `Uno.Simple.WinUI`'s. Fixes to
  the Uno.Themes copy do not reach the NavigationBar, which resolves the toolkit copy at parse time.

## A Release build rewrites `Generated/mergedpages.xaml`

**2026-09-29 — FlexPanel review (#1639).**

**Context:** `f0eeef12` committed the Release-merged `src/Uno.Toolkit.UI/Generated/mergedpages.xaml` (+2,019 lines) alongside an unrelated test change. On `main` that file is the Debug version — a small dictionary pointing at each control's XAML so hot reload works — and `AGENTS.md` forbids editing anything under `Generated/`. Nobody wrote it by hand; a Release build did, and `git add` of a directory swept it in.

**How to apply:** After any Release build, run `git status` and restore `Generated/` (`git checkout <base> -- src/Uno.Toolkit.UI/Generated/`) before staging. Stage by explicit path, not by directory.

## Fold a fix into an old commit against that commit's parent, never against `main`

**2026-09-29 — FlexPanel review (#1639).**

**Context:** Folding a revert into an earlier branch commit was done with `git rebase -i --autosquash main`. `main` had moved ~100 commits past the branch's base, so the "squash" silently rebased the whole branch onto the unrelated Uno 7 groundwork, which then had to be undone by hand.

**How to apply:** `git commit --fixup=<sha>` then `GIT_SEQUENCE_EDITOR=: git rebase -i --autosquash <sha>^`. The base is the target commit's parent, which keeps every earlier SHA and the branch's base untouched. Confirm afterwards with `git merge-base HEAD main` (unchanged) and `git diff <old-tip> HEAD` (only the intended change).

## Documentation ownership

- Public migration guidance should describe release-level behavior without pinning the explanation to an exact prerelease NuGet build. Package provenance belongs in PR and validation notes.
- Inherited APIs do not need a second usage guide in Toolkit. Keep Toolkit setup and control-specific caveats locally, and link shared `BaseTheme` behavior to Uno Themes so changes have one documentation owner.
- Apply documentation ownership decisions to the entire branch diff, including XML summaries. Avoid maintaining lists of inherited members in Toolkit; link to their owner and retain only local integration details.

## Compatibility test ownership

- Test Toolkit's inheritance of upstream token generation against the upstream theme with identical inputs. Keep arithmetic tables and default asset names in the owning Themes tests.
- Reuse Toolkit's existing visual-tree helpers in runtime tests instead of introducing a second traversal implementation.

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

**How to apply:** Use a local clone of [unoplatform/uno](https://github.com/unoplatform/uno) (`src/Uno.UI`, with `Generated/3.0.0.0/**` for the WinUI surface). Two greps there settle any "does this base type expose X?" question. `AutoLayout` compiling against `Padding` without declaring it is a hint, not an answer — it does not tell you whether the member is public, protected, or where it comes from.

## A platform-specific hide needs a platform-specific fix

**Context:** `FlexPanel.LayoutDirection` failed the Android build with CS0114, because the base `View` on that platform declares a `LayoutDirection` of its own. The obvious fix - add `new` - is only half right: no other target has a base member to hide, so a bare `new` raises CS0109 there ("the new keyword is not required"). Under `TreatWarningsAsErrors` that trades one broken platform for five. The CI logs were what settled it: Android reported CS0114 and CA1822, while iOS, WASM and Desktop reported CA1822 alone.

**How to apply:** When a member collides with a base member, first establish *which* targets actually see that base member - per-platform CI logs answer this directly, and a green build on one target proves nothing about the others. Then pick a fix that is correct on every target: either `#pragma warning disable CS0109` with a comment, or bracket the keyword in `#if __ANDROID__`. Verify both sides - one build on an affected target and one on an unaffected target - because the two errors are mutually exclusive and neither build alone can catch both.
