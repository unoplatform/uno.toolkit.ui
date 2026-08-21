# Lessons

Patterns worth not repeating. Per `AGENTS.md` §3, corrections that should bind every agent on this repo go here (or in `AGENTS.md` / a skill's `SKILL.md`), never in personal memory.

## L1 — Never state a ratio, percentage, or proportion you did not compute

**Context:** While comparing `AutoLayout` coverage against the proposed `FlexPanel` (`specs/flexpanel-import/`), a summary claimed "for the ~70% of AutoLayout that is stack-things-in-a-line, FlexPanel is strictly better… the Figma-parity 30% isn't". Neither number came from anything. Asked where they came from, there was no answer, because they were invented to sound decisive.

**Why it matters:** A fabricated number is worse than an admitted unknown — it is load-bearing for a scoping decision, it looks derived, and it survives into specs and PR descriptions where nobody re-checks it. In the same answer, the one claim that *was* counted ("roughly half the tests can't port") turned out to be wrong too, which only surfaced because someone asked about the fake number.

**How to apply:** Either count it and say what was counted (`2 of 19 active test methods`, `~130 DataRow cases`), or write "most" / "the majority" and move on. Any figure with a digit in it needs a derivation the reader could re-run. This includes counts, percentages, LOC estimates, and effort splits.

## L2 — Behavior lives in arithmetic, not in doc warnings or variable names

**Context:** `doc/controls/AutoLayoutControl.md` carries a bold **WARNING** that `AutoLayout`'s `Padding` is Figma-anchored — "the anchor points determine which sides of the Padding will be taken into consideration" — and `InnerArrange` really does gate padding behind `haveStartPadding` / `haveEndPadding`. Both were taken as proof that padding semantics diverge from CSS, and nine runtime tests were declared unportable on that basis.

The gating does not survive contact with the arithmetic. `PrimaryAxisAlignmentOffsetSize` receives the **raw** padding values, not the gated ones, and adds them back — so `Start` / `Center` / `End` all reduce algebraically to CSS content-box alignment. Confirmed against `When_Axis_Are_Center_With_No_Homogeneous_Padding` (`Padding="150,20,100,50"`, the one test built to break symmetry): its asserted offsets are exactly the CSS results. The real count of unportable tests was 2, not 9, and padding was not among the reasons.

**Why it matters:** This repo's layout code is full of alarming-looking names (`filledAsHug`, `IsReverseZIndex`, `haveStartPadding`) and docs that describe *intent* rather than *effect* — `IsReverseZIndex`'s doc claim that it "does not change layout order" is likewise contradicted by its own arrange loop. Reasoning from those names produces confident, wrong answers.

**How to apply:** Before asserting that a layout behavior diverges, reduce both sides to a formula and check it against a test's asserted numbers — preferably the test whose inputs are *asymmetric*, since symmetric fixtures make divergent models agree by coincidence. If the numbers can't be reproduced by hand, the claim isn't ready. And when the code contradicts the doc, the doc is the bug: say so separately, and fix it separately.

## L3 — Grep an unfamiliar base class before claiming what a type inherits

**Context:** Establishing what `FlexPanel : Panel` would lose relative to `AutoLayout : RelativePanel` required knowing where `Padding` / `BorderBrush` / `BorderThickness` / `CornerRadius` are actually declared. They are public on `RelativePanel` (Uno's `RelativePanel.Properties.cs`); `Panel` carries only the `internal` `PaddingInternal` / `BorderThicknessInternal` / `CornerRadiusInternal` plumbing, unreachable from `Uno.Toolkit.UI`. A `Panel` subclass therefore cannot draw a border at all — a materially different limitation from "Yoga doesn't measure borders".

**How to apply:** The Uno source is checked out alongside this repo (`../../framework/uno-readonly/src/Uno.UI`, with `Generated/3.0.0.0/**` for the WinUI surface). Two greps there settle any "does this base type expose X?" question. `AutoLayout` compiling against `Padding` without declaring it is a hint, not an answer — it does not tell you whether the member is public, protected, or where it comes from.
