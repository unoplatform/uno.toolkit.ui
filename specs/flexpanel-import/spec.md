# Feature: Import the Yoga-backed `FlexPanel` into `Uno.Toolkit.UI`

**Area**: `Uno.Toolkit.UI` — new `Controls/FlexPanel/` + vendored `Layout/Yoga/`
**Type**: New control + vendored third-party engine
**Upstream**: [`microsoft/microsoft-ui-reactor`](https://github.com/microsoft/microsoft-ui-reactor) @ `v0.1.0-preview.13`, MIT — `src/Reactor/Yoga/*`
**Tracks**: [`uno.toolkit.ui-private#17`](https://github.com/unoplatform/uno.toolkit.ui-private/issues/17) (`AutoLayout -> FlexPanel`)

## Summary

`AutoLayout` implements Figma Auto Layout, not CSS Flexbox. It has no wrap, no `flex-grow`/`flex-shrink` ratios, no `flex-basis`, no `align-content`, no baseline alignment, no min/max constraints, and only two `Justify` modes. Closing that gap by extending `AutoLayout`'s bespoke measure pass means hand-writing the CSS Flexbox §9 algorithm — the part everyone gets subtly wrong (auto-min sizing, wrap line-breaking, shrink with floors, rounding).

Microsoft's Reactor project contains a C# line-port of **Meta's Yoga** engine plus a thin WinUI `Panel` adapter, under MIT. Importing it gives us a spec-correct flexbox implementation *and* Yoga's upstream conformance corpus as our test suite, instead of a bespoke algorithm we have to prove ourselves.

This spec covers importing that code as a **new, additive** `FlexPanel` control. `AutoLayout` is not modified, renamed, or deprecated by this work.

## Why import rather than extend `AutoLayout`

| | Extend `AutoLayout` | Import Yoga + `FlexPanel` |
|---|---|---|
| Algorithm | hand-written CSS §9 | Yoga, already written |
| Conformance evidence | tests we author | 26 upstream generated test files, 590 tests, 17,784 assertions |
| Figma semantics | must keep (hijacked `Padding`, hug/fill roles) | not our problem — separate control |
| Breaking-change risk to existing `AutoLayout` users | high (every behavior change is a regression surface) | zero |
| Ongoing cost | we own the algorithm forever | we own ~1,000 LOC of adapter; engine is a vendored fork |

`AutoLayout` keeps serving Figma-generated layouts, which is what it is good at. `FlexPanel` serves CSS-shaped layouts. Two mental models, two controls, no compromise API that satisfies neither.

## `AutoLayout` coverage audit

D1 keeps `AutoLayout` untouched and FR-9 keeps its tests green, which leaves one question open before P0: **if a consumer moved a layout from `AutoLayout` to `FlexPanel`, what would break?** Operationally, "the existing `AutoLayout` feature set" is the behavior pinned by `src/Uno.Toolkit.RuntimeTests/Tests/AutoLayoutTest.cs` — 20 test methods, ~130 `DataRow` cases, one method `[Ignore]`d for #1203. Every case below was checked by hand against the CSS flexbox result, arithmetically wherever an offset is asserted.

**Result: 2 of the 19 active tests are non-portable, and both fail for the same single reason — `IsIndependentLayout`.** The Figma-vs-CSS divide is real but far narrower than the Summary implies.

### The padding warning is not a gap

`doc/controls/AutoLayoutControl.md` warns that "the anchor points determine which sides of the Padding will be taken into consideration", and `InnerArrange` does gate padding behind `haveStartPadding` / `haveEndPadding`. That gating does **not** produce Figma-anchored offsets, because `PrimaryAxisAlignmentOffsetSize` receives the *raw* padding values and adds them back:

| `PrimaryAxisAlignment` | resulting `currentOffset` | CSS content-box offset |
|---|---|---|
| `Start` | `borderStart + start` | `borderStart + start` |
| `Center` | `borderStart + start + (C − occupied) / 2` | identical |
| `End` | `borderStart + (H − end − occupied)`, which expands to `borderStart + start + (C − occupied)` | identical |

(`H` = full length along the axis, `C = H − start − end`.) `ComputeCounterAlignmentOffset` has the same shape on the counter axis. Verified numerically against the one test built to break symmetry — `When_Axis_Are_Center_With_No_Homogeneous_Padding`, `Padding="150,20,100,50"` on 300×300, two 100×50 children, `Spacing="10"`: the asserted `(80, 140, 125, 125)` and `(110, 110, 70, 180)` are exactly the CSS results.

**`AutoLayout`'s `Padding` is CSS padding.** The gating leaks in exactly one place — the overflow clamp at `AutoLayout.Layouting.Arrange.cs:213` measures against `H − start` instead of the content box, so an already-overflowing child truncates at a different point (G3).

### Verdict per test

| Verdict | Tests (cases) | Count |
|---|---|---|
| ✅ Ports unchanged | `When_Collapsed`, `When_Initially_Collapsed_With_Spacing`, `When_Late_Collapsed_With_Spacing`, `When_Collapsed_With_Spacing_Margin` (16), `When_Measure`, `When_CounterAlignment_stretch` (2), `When_Fixed_Dimensions_Padding_And_SpaceBetween_Horizontal`, `..._Vertical`, `When_Hug_With_Padding`, `When_Hug_With_Padding_CounterAxis`, `When_Axis_Are_Center_With_No_Homogeneous_Padding` (2) | 11 |
| ⚠️ Ports under a mapping rule | `When_Padding` (40) and `When_Padding_CounterAxis` (40) → M1 + M2; `When_SpaceBetween_with_spacing` (2) → M3; `When_Stretched_PrimaryAlignment_In_ScrollViewer` → M4 | 4 |
| 🔶 Ports only if `MaxWidth`/`MaxHeight` ships in v1 | `When_PrimaryAxisAlignment_Centered_And_Filled_Single_Element_With_MaxSize` (3), `..._Filled_Elements_With_MaxSize` (2) | 2 |
| ❌ Non-portable | `When_AbsolutePosition_WithPadding` (9), `When_Space_between_With_AbsolutePosition` (2) | 2 |
| — Already disabled | `When_Hug_With_CounterAlignment` (`[Ignore]`, #1203) | 1 |

### Mapping rules

Not gaps — the behavior is reachable, but only through a non-obvious translation. Anything generating `FlexPanel` XAML from `AutoLayout` XAML must apply all four.

- **M1 — every `AutoLayout` child is `Shrink="0"`.** `AutoLayout` has no shrink concept: `Fixed` children keep their declared length and `Hug` children their desired length, whatever the deficit. `FlexPanel`'s default is `Shrink="1"`. Without `Shrink="0"`, the overflow rows of `When_Padding` (a 350px child in a 400px panel with `Padding="100"`, asserted at `-50`) shrink to fit instead of overflowing; with it, CSS also lands on `-50`.
- **M2 — `PrimaryAlignment="Stretch"` is `Grow="1" Basis="0"`, never `Grow="1"` alone.** `MeasureFilledChildren` divides `remainingSize / filledChildrenCount`, giving equal *final* sizes — that is `flex: 1 1 0`. Bare `flex-grow: 1` distributes only *free* space on top of each child's content size, so unequal children stay unequal.
- **M3 — `Justify="SpaceBetween"` drops `Spacing`.** `MeasureOverride` step 4 skips spacing entirely under `SpaceBetween`, and `ComputeSpaceBetween` recomputes it from the leftover. CSS treats `gap` as a *floor* under `space-between`, so carrying `Spacing="100"` over as `RowGap="100"` turns `When_SpaceBetween_with_spacing`'s expected `10 / 130 / 250` into `10 / 210 / 410`. Translate `Spacing` to `0` whenever `Justify="SpaceBetween"`.
- **M4 — the adapter must re-run Yoga against the arrange `finalSize`.** `When_Stretched_PrimaryAlignment_In_ScrollViewer` depends on `AutoLayout`'s two-pass behavior: under the `ScrollViewer`'s infinite constraint `filledAsHug` demotes `Filled` to `Hug`, then `InnerArrange` re-measures at the real 300px and the child fills 275px. Yoga reaches the same answer only if the adapter recomputes when `finalSize` differs from the measure constraint.

### Genuine gaps

- **G1 — `IsIndependentLayout` is a different positioning model, not a remappable property.** The child is arranged into `new Rect(default, finalSize)` — the whole panel rect, padding ignored — then positioned by its *own* `HorizontalAlignment` / `VerticalAlignment` / `Margin`; `doc/controls/walkthroughs/AutoLayout.howto.md` documents exactly that ("position it with normal alignments"). Yoga's `Position: Absolute` is inset-driven off the *padding box* and never consults WinUI alignment, so `Margin="10,280,0,0"` + `VerticalAlignment="Top"` (asserted at `Y = 280` in `When_Space_between_With_AbsolutePosition`) becomes `Top="270"` — a different number through a different mechanism. Separately, `MeasureIndependentChildren` folds these children into the panel's desired size (`Math.Max` on both axes), where CSS absolutely-positioned children contribute nothing to container intrinsic size. Both non-portable tests fail here.
- **G2 — negative `Spacing` has no container-level equivalent.** CSS `gap` is non-negative. Four of `When_AbsolutePosition_WithPadding`'s nine rows use `spacing: -30` / `-20`, and the overlapping-avatar samples use `Spacing="-10"` / `"-20"` (`samples/Uno.Toolkit.Samples/Content/Controls/AutoLayoutPage.xaml:51,87,163`). The *capability* survives via negative child `Margin` (Yoga supports negative margins); the container property does not. **Answered in P1 — Yoga clamps.** `YogaStyle.ComputeGapForAxis` is the single gap entry point for both `YogaAlgorithm` call sites and ends in `YogaFloat.MaxOrDefined(gap.Resolve(ownerSize), 0)`, i.e. `MathF.Max(x, 0)`. So G2 stands as a genuine gap, exactly as described.
- **G3 — overflow is truncated, not shrunk or overflowed.** A child longer than the remaining space is clamped to it (`Arrange.cs:213`), against a padding box that omits the non-anchored side. Under M1 the *offsets* match CSS but the arranged *size* does not: a 350px child in a 300px slot arranges at 300 under `AutoLayout`, at 350 (overflowing) under `FlexPanel`. No test asserts it — the padding tests check offsets only — but consumers see it.
- **G4 — no frame chrome.** `AutoLayout : RelativePanel` inherits *public* `Padding`, `BorderBrush`, `BorderThickness` and `CornerRadius` (declared in Uno's `RelativePanel.Properties.cs`); `Panel` exposes only the `internal` `PaddingInternal` / `BorderThicknessInternal` / `CornerRadiusInternal` plumbing, which `Uno.Toolkit.UI` cannot reach. `FlexPanel : Panel` therefore declares its own `Padding` (D3) but **cannot draw a border or corner radius at all** — a wrapping `Border` is required. This is broader than the deferred "Yoga-measured `BorderThickness`", which is about measurement; this is about rendering. `AutoLayout` also folds `BorderThickness` into both measure and arrange.
- **G5 — `IsReverseZIndex` needs a justify flip.** Despite the name (and the howto's claim that it "does not change layout order"), the arrange loop iterates children in reverse while `currentOffset` still increases — so it reverses *positional* order and leaves paint order, which follows the `Children` index, untouched. `Direction="RowReverse"` plus the mirrored `JustifyContent` reproduces both. Listed here rather than as a mapping rule because the doc is wrong and should be fixed independently of this work.

### Divergence in the other direction

**`AutoLayout` ignores child `Margin`.** `When_Collapsed_With_Spacing_Margin_Test_Matrix` hard-codes `itemMargin = 0d` with the comment *"AutoLayout just ignores margin on children"* (see also #1279). FR-4 makes `FlexPanel` honor `Margin` as Yoga margin edges — correct, and a superset — but a consumer migrating a layout whose children carry incidental margins will see them start to apply.

### Consequences for this spec

1. **Recommend un-deferring `MaxWidth` / `MaxHeight` into v1.** The only bucket the audit found that is purely a scope decision: two additive attached properties, no engine work, `YogaMinMaxDimensionTest` already in the tier-1 corpus. Both max-size tests were verified *exactly* CSS-equivalent — `AutoLayout`'s `filledMaxSurplus` / `GetChildrenLowerThanAllocateSurplus` machinery is a hand-rolled CSS §9.7 freeze-violations loop, and Yoga's is the correct one. Left in Deferred, the audit's 🔶 row becomes a second pair of non-portable tests for no reason. **P0 decision.**
2. **Tier 2 gains four mapping-rule guards**, one per M1–M4, named after the rule, so a future "unify the two controls" attempt cannot quietly regress them. `When_Grow_InScrollViewer_ThenFillsViewport` (M4) has no analogue in the current tier-2 list.
3. **D1's justification is narrower than the Summary suggests and should be stated as such** in the "`FlexPanel` vs `AutoLayout`: which do I want?" doc section: the answer is G1 (absolute-position semantics), G2 (negative spacing) and G4 (frame chrome) — not padding, and not alignment.

## Scope of the import

Upstream `src/Reactor/Yoga/` — 11 files, 5,449 LOC. Verified dependency split:

| File(s) | LOC | Non-`System` deps | Adaptation |
|---|---|---|---|
| `YogaAlgorithm.cs` | 2,203 | none | namespace only |
| `YogaNode.cs` | 649 | none | namespace only |
| `AlgorithmUtils.cs` | 516 | none | namespace only |
| `YogaStyle.cs` | 402 | none | namespace only |
| `LayoutResults.cs` | 188 | none | namespace only |
| `YogaConfig.cs` | 127 | none | namespace only |
| `YogaEnums.cs` / `FlexEnums.cs` | 189 | none | namespace only |
| `YogaValue.cs` | 116 | none | namespace only |
| `FlexDirectionHelper.cs` | 81 | none | namespace only |
| **subtotal (engine)** | **4,471** | **`System.Threading`, `System.Diagnostics` only** | **mechanical** |
| `FlexPanel.cs` | 978 | `Microsoft.UI.Xaml{,.Controls}`, `Windows.Foundation` | **rewrite as Uno adapter** |

Not imported: `src/Reactor/Elements/FlexExtensions.cs` (the `.Flex(...)` Reactor-DSL modifier — a ~40-line wrapper over the attached DPs; our surface is XAML-first).

Verified absent from all 11 files: any reference to `Microsoft.UI.Reactor.Core`, `Element`, `Component`, hooks, source generators, analyzers, `#if`, `[Experimental]`, partial-method hooks. The single `Reactor.Core` grep hit is the header comment asserting the absence.

## Public API (v1)

Namespace `Uno.Toolkit.UI`. `public partial class FlexPanel : Panel`.

### Container properties

| Property | Type | Default | CSS equivalent |
|---|---|---|---|
| `Direction` | `FlexDirection` (`Row`, `RowReverse`, `Column`, `ColumnReverse`) | `Row` ⚠️ | `flex-direction` |
| `Wrap` | `FlexWrap` (`NoWrap`, `Wrap`, `WrapReverse`) | `NoWrap` | `flex-wrap` |
| `JustifyContent` | `FlexJustify` (`FlexStart`, `Center`, `FlexEnd`, `SpaceBetween`, `SpaceAround`, `SpaceEvenly`) | `FlexStart` | `justify-content` |
| `AlignItems` | `FlexAlign` (`Stretch`, `FlexStart`, `Center`, `FlexEnd`, `Baseline`) | `Stretch` | `align-items` |
| `AlignContent` | `FlexAlign` (+ `SpaceBetween`/`SpaceAround`/`SpaceEvenly`) | `FlexStart` | `align-content` |
| `ColumnGap` | `double` | `0` | `column-gap` |
| `RowGap` | `double` | `0` | `row-gap` |
| `Padding` | `Thickness` | `0` | `padding` |
| `LayoutDirection` | `FlexLayoutDirection` (`Inherit`, `LeftToRight`, `RightToLeft`) | `LeftToRight` | `direction` |

⚠️ `Direction`'s DP metadata default is `Row`, which **diverges from the enum's zero value** — upstream's `FlexEnums.cs` uses Yoga's numbering (`Column = 0, ColumnReverse = 1, Row = 2, RowReverse = 3`). Upstream gets away with it because its `FlexRow()`/`FlexColumn()` factories always set `Direction` explicitly; a bare `<utu:FlexPanel>` in XAML relies entirely on the metadata default. Keep `Row` (it is the CSS default and the useful one) and keep the comment in the DP registration, because a reader of `FlexEnums.cs` will otherwise assume `Column`.

### Attached properties

| Property | Type | Default | CSS equivalent |
|---|---|---|---|
| `FlexPanel.Grow` | `double` | `0` | `flex-grow` |
| `FlexPanel.Shrink` | `double` | `1` | `flex-shrink` |
| `FlexPanel.Basis` | `double` | `NaN` (= `auto`) | `flex-basis` (px only in v1) |
| `FlexPanel.FlexMinWidth` | `double` | `NaN` (= `min-width: auto`) | `min-width` |
| `FlexPanel.FlexMinHeight` | `double` | `NaN` (= `min-height: auto`) | `min-height` |
| `FlexPanel.AlignSelf` | `FlexAlign` | `Auto` | `align-self` |
| `FlexPanel.Position` | `FlexPositionType` (`Static`, `Relative`, `Absolute`) | `Relative` | `position` |
| `FlexPanel.Left` / `.Top` / `.Right` / `.Bottom` | `double` | `NaN` | inset properties |

Child `Margin`, `Width`, `Height`, and `Visibility` participate without any attached property: the adapter syncs `Margin` → Yoga margin edges, `Width`/`Height` → `YogaNode.Width`/`Height` (`NaN` → `auto`), and `Visibility="Collapsed"` → Yoga `Display: None`.

```xml
<utu:FlexPanel Direction="Row"
               Wrap="Wrap"
               AlignItems="Center"
               ColumnGap="8"
               RowGap="8"
               Padding="16,8">
    <TextBlock Text="MyApp" utu:FlexPanel.Shrink="0" />
    <Border utu:FlexPanel.Grow="1" />
    <Button Content="Settings" />
</utu:FlexPanel>
```

### Deferred (engine supports it, v1 does not surface it)

`MaxWidth`/`MaxHeight`, percentage `Basis`, `AspectRatio`, `Overflow`, `BoxSizing`, the `flex` shorthand, Yoga-measured `BorderThickness`, CSS Grid (`JustifyItems`/`JustifySelf`). Each is an additive attached property later — none requires touching the engine. `order` is *not* deferred: Yoga does not implement it, so `FlexPanel` will not have it.

## Design decisions

**D1 — Additive new control; `AutoLayout` untouched.** No shared base class, no shared enums, no "unified" property set. Convergence, if it ever happens, is a separate decision with its own breaking-change budget.

**D2 — Vendored code is physically isolated and marked.**

```
src/Uno.Toolkit.UI/
  Controls/FlexPanel/
    FlexPanel.cs                  # our adapter (rewritten)
    FlexPanel.Properties.cs       # container + attached DPs
  Layout/Yoga/                    # vendored, near-verbatim
    .editorconfig                 # analyzer containment (see D7)
    README.md                     # upstream URL + commit SHA + "do not hand-edit"
    YogaAlgorithm.cs, YogaNode.cs, …
```

Splitting DPs into `FlexPanel.Properties.cs` follows `AutoLayout`'s existing file layout. The vendored folder is *not* under `Controls/` because it is not a control and must not be confused for our code; its `README.md` pins the upstream SHA so the next sync is a diff, not an archaeology exercise.

⚠️ **Namespace correction (P1).** This spec originally called for namespace `Uno.Toolkit.UI.Layout`. **That name is unavailable**: `Uno.Toolkit.UI` already declares `public enum Layout` (`Helpers/ResponsiveHelper.cs:17`, consumed by `ResponsiveHelper`), a namespace cannot share a fully-qualified name with a type in the same assembly, and the enum is public API that cannot be renamed. As imported:

| | namespace |
|---|---|
| `FlexEnums.cs` — the 6 public enums that become `FlexPanel`'s DP types | `Uno.Toolkit.UI` |
| The other 9 engine files, all `internal` | `Uno.Toolkit.UI.Yoga` |

The public enums join the flat toolkit namespace (132 of 133 files there today) so `FlexPanel.SetAlignSelf(el, FlexAlign.Center)` needs no extra `using`; C# enclosing-namespace lookup lets the engine resolve them without one either. Public surface added by the import is **exactly those 6 enum types** — everything else was already `internal` upstream, so no visibility rewrite was needed.

**D3 — `Padding`, not `FlexPadding`.** Upstream needs the `Flex` prefix because Reactor's `.Padding()` DSL modifier already means "`FrameworkElement.Margin`/`Padding`". We have no such conflict: `Panel` exposes only `Background` — **there is no `Panel.Padding`** (verified against `src/Uno.UI/Generated/3.0.0.0/Microsoft.UI.Xaml.Controls/Panel.cs`). So `FlexPanel.Padding` reads exactly like `Border.Padding` and means what a XAML author expects.
*Kept* upstream naming for `FlexMinWidth`/`FlexMinHeight`, despite the `FlexPanel.FlexMinWidth` stutter. Rejected alternative `FlexPanel.MinWidth`: it would sit next to `FrameworkElement.MinWidth` on the same element while meaning something different (the flex floor, not the measure constraint) — the stutter is cheaper than that trap.

**D3b — `FlexLayoutDirection` members are renamed `LTR`/`RTL` → `LeftToRight`/`RightToLeft`; numeric values stay verbatim.** The rename is the one place the v1 surface deliberately diverges from the vendored source: WinUI spells it out (`FlowDirection.LeftToRight`), abbreviations read badly in XAML, and the enum is part of *our* public API. It costs one adapter-side mapping and must be applied in P1, not discovered in P3. Everything else about the vendored enums is untouched — in particular **the numeric values are not renumbered**, including `FlexDirection`'s Yoga ordering. Renumbering for aesthetics would silently change any serialized or interop value, and by D8's logic divergence has to be paid for.

**D4 — Layout rounding is bound to `UseLayoutRounding`, not hardcoded to the DPI.** Upstream sets `YogaConfig.PointScaleFactor = XamlRoot.RasterizationScale` specifically to match *WinUI's* physical-pixel rounding. Uno rounds differently per target, and `RoundValueToPixelGrid` treats `PointScaleFactor <= 0` as "no rounding" (`AlgorithmUtils.cs:318`). So:

- `UseLayoutRounding = true` (default) → `PointScaleFactor = XamlRoot.RasterizationScale`
- `UseLayoutRounding = false` → `PointScaleFactor = 0`, Yoga emits unrounded values and the platform's own rounding is the only one applied

This gives an escape hatch for any target where double-rounding produces drift, expressed through a property WinUI/Uno authors already know. Upstream's lazy `SyncPointScaleLazy()` from `MeasureOverride` (deliberately *not* subscribing to `XamlRoot.Changed`, to avoid pinning every panel through a multicast delegate) is kept verbatim — that reasoning applies to Uno identically.

**D5 — Follow the house `#if IS_WINUI` using-block convention.** 107 files in `Uno.Toolkit.UI` guard their XAML usings this way (`AutoLayout.cs` included). `IS_WINUI` is unconditionally defined by the only library csproj today, so the `#else` branch is dead — but a new file that omits the pattern reads as foreign. Adapter only; the engine files need no guards at all.

**D6 — No Material/Cupertino styles.** `FlexPanel` is a templateless layout panel: no `Style`, no theme resources, no `controls-styles.md` / `lightweight-styling.md` entries.

**D7 — Analyzer containment via a scoped `.editorconfig`, never global `NoWarn`.** `src/Directory.Build.props` sets `TreatWarningsAsErrors=true`, `Nullable=enable`, and `AnalysisModePerformance=AllEnabledByDefault`. 4,471 LOC of C++-shaped port (`float` math, `ref` returns, big `switch`es, unused generated enum members) will not pass that clean, and *editing it to comply destroys the line-for-line correspondence that makes upstream syncs tractable*. Fix: a `.editorconfig` scoped to `src/Uno.Toolkit.UI/Layout/Yoga/` that downgrades the offending rules to `none` for that path only. AGENTS.md forbids expanding global `<NoWarn>`; this satisfies the rule and keeps the blast radius at the folder.

✅ **Measured in P1: no suppressions were needed.** The vendored engine builds **zero-warning** in Release under all three settings — upstream already builds under the same `TreatWarningsAsErrors=true` + `Nullable=enable`. The scoped `.editorconfig` was still added, but it carries only `indent_style = space` / `indent_size = 4` (upstream is 4-space, the repo is tabs, and reformatting would break the line-for-line correspondence) plus a note directing any future suppression *here* rather than to the global `<NoWarn>`.

**D8 — The engine's static generation counter is accepted as-is.** `YogaAlgorithm` is a static class with `private static uint s_currentGenerationCount`, self-documented "thread-unsafe", bumped via `Interlocked.Increment`. Layout runs on the UI thread on every Uno target, so the invariant holds. It is a value type, so it pins nothing and is ALC-safe. Recorded rather than refactored — divergence from upstream must be paid for.

**D9 — Node cache lifetime is already handled; we still guard it.** `SyncYogaTree()` prunes `_nodeCache` / `_attachedCache` against the live `Children` on every layout pass, so removed children are released. The residual window — a panel that never lays out again after a child is removed holds that child alive — is exactly what a `LeakTest` is for (see Tests, tier 3).

## Requirements

- **FR-1** — `FlexPanel` is a `Panel` usable from pure XAML with no C# DSL, no framework opt-in, and no initialization call.
- **FR-2** — Every property in the two API tables above is a `DependencyProperty` with a CLR wrapper; attached properties follow the `Get/Set<Name>(DependencyObject)` convention and each has XML docs naming its CSS equivalent.
- **FR-3** — Changing any container or attached property invalidates measure and produces a new layout pass.
- **FR-4** — Child `Margin`, `Width`, `Height` and `Visibility="Collapsed"` participate in layout without attached properties (margin edges, definite dimensions, `Display: None` respectively).
- **FR-5** — CSS §4.5 automatic minimum size is honored on the main axis: `Basis=auto` + `FlexMinWidth=NaN` floors at min-content; `FlexMinWidth=0` or `Basis=0` opts out; `ScrollViewer`/`ScrollView` children always floor at 0 so a sizing-only pre-measure never realizes virtualized content.
- **FR-6** — `UseLayoutRounding=false` disables Yoga's pixel-grid rounding (`PointScaleFactor=0`); `true` tracks `XamlRoot.RasterizationScale`.
- **FR-7** — `LayoutDirection` is the *only* RTL input `FlexPanel` reads. Verified: the adapter never touches `FlowDirection` — it passes `LayoutDirection` straight into `CalculateLayout` (`FlexPanel.cs:479`, `:580`) and its DP default is `LeftToRight`, not `Inherit`. The two mirroring mechanisms are therefore fully independent, and setting **both** `FlowDirection="RightToLeft"` and `LayoutDirection="RightToLeft"` double-mirrors. v1 contract: `FlexPanel` ignores `FlowDirection`; the doc states this and the sample demonstrates `LayoutDirection`. (Deferred, not silently dropped: resolving `Inherit` from the ambient `FlowDirection` so the WinUI-idiomatic path works — additive, and better decided once we have real RTL feedback.)
- **FR-8** — Layout is identical across the **Skia heads** — desktop/Skia, WASM, and Skia mobile — for the conformance corpus, modulo documented rounding. (Narrowed in P1: Uno 7.0 removes the native `-ios` / `-android` / `-windows` / `-maccatalyst` targets, work already underway in this repo on `dev/mazi/uno7-groundwork`. Library projects have a single non-native TFM, and `-p:TargetFrameworkOverride=desktop` resolves to it alone. Costs nothing here — the engine is pure managed `float` math with no platform API, no `#if`, and no TFM-conditional code.)
- **FR-9** — `AutoLayout`'s public API and behavior are byte-for-byte unchanged; its existing runtime tests pass untouched.

## Non-functional requirements

- **NFR-1** — Zero warnings in Release across all TFMs (D7 is the only sanctioned suppression, scoped to the vendored folder).
- **NFR-2** — No per-layout-pass allocation in the adapter's steady state: the node cache, the child `HashSet`, and the removal list are instance fields reused across passes (upstream already does this; a change that regresses it is a defect). No LINQ in `MeasureOverride`/`ArrangeOverride`.
- **NFR-3** — A `FlexPanel` whose children never change performs at most one `Measure` per child per pass in the `Basis`-definite case; the min-content pre-measure only runs where FR-5 requires it.
- **NFR-4** — No static event subscriptions, no `XamlRoot.Changed` subscription, no strong reference held to a child after it is removed *and* the panel re-lays out.
- **NFR-5** — Every imported file carries a provenance + license header, and `THIRD-PARTY-NOTICES.md` at the repo root names Reactor/Yoga, both MIT texts, and the pinned SHA. (Restated in P1: there was **no upstream MIT header to keep** — the vendored files carry only `// C# port of Meta's Yoga…` comments, with no copyright, license or provenance. Each imported file is therefore prepended with a generated 4-line header giving the upstream repo, tag, commit, that file's own upstream path, the import date, and `SPDX-License-Identifier: MIT` with both copyright holders — Microsoft for the C# port, Facebook/Meta for Yoga. The full license text lives once in the notices file.)

## Tests

All under `src/Uno.Toolkit.RuntimeTests/Tests/`, MSTest, hosted in the sample apps (there is no `dotnet test` entry point).

**Tier 1 — engine conformance (the point of the whole exercise).** Port all 26 files from upstream `tests/Reactor.Tests/YogaGenerated/` into `Tests/Yoga/`. They are pure computation over `YogaNode`/`YogaConfig`/`YogaValue` with zero XAML, so the conversion is mechanical: xUnit `[Fact]` → MSTest `[TestMethod]`, `Assert.Equal(expected, actual)` → `Assert.AreEqual(expected, actual)`. Running them inside the runtime-test host (rather than a new headless unit-test project) is deliberate: it costs no new infrastructure and it validates float behavior on WASM and on AOT targets, which a desktop-only unit test would not.
Coverage inherited: `YogaAbsolutePositionTest`, `YogaAlignContentTest`, `YogaAlignItemsTest`, `YogaAlignSelfTest`, `YogaAspectRatioTest`, `YogaBorderTest`, `YogaBoxSizingTest`, `YogaDimensionTest`, `YogaDisplayTest`, `YogaDisplayContentsTest`, `YogaFlexTest`, `YogaFlexBasisFitContentTest`, `YogaFlexDirectionTest`, `YogaFlexWrapTest`, `YogaGapTest`, `YogaIntrinsicSizeTest`, `YogaJustifyContentTest`, `YogaMarginTest`, `YogaMinMaxDimensionTest`, `YogaPaddingTest`, `YogaPercentageTest`, `YogaRoundingTest`, `YogaSizeOverflowTest`, `YogaStaticPositionTest`, `YogaAndroidNewsFeed`, `YogaAutoTest`.

**Tier 2 — adapter behavior**, `Tests/FlexPanelTests.cs`:

- `When_Grow_ThenRemainderSplitByRatio` — two children `Grow=1`/`Grow=2` split leftover 1:2.
- `When_Shrink_ThenDeficitSplitByRatio` — overflowing row with `Shrink=1`/`Shrink=0` shrinks only the first.
- `When_BasisSet_ThenWidthIgnored` — `Width=200` + `Grow=1` arranges at the grown size, not 200 (encodes the documented CSS/WinUI mismatch).
- `When_Wrap_ThenLinesBreakAndRowGapApplies` — N chips in a constrained width produce 2 lines separated by `RowGap`.
- `When_AlignItemsBaseline_ThenTextBaselinesAlign` — mixed-`FontSize` `TextBlock`s share a baseline.
- `When_AutoMin_ThenChildDoesNotShrinkBelowMinContent` and `When_FlexMinWidthZero_ThenChildShrinksBelowContent` — FR-5 both directions.
- `When_ScrollViewerChild_ThenMinContentIsZero` — FR-5 virtualization guard.
- `When_ChildCollapsed_ThenExcludedFromLayoutAndGap` — no phantom gap.
- `When_ChildMarginSet_ThenNotDoubleCounted` — guards the adapter's margin-compensation path.
- `When_PositionAbsolute_ThenInsetsHonoredAndSiblingsUnaffected`.
- `When_AttachedPropertyChanges_ThenLayoutUpdates` — FR-3 for one attached and one container property.
- `When_UseLayoutRoundingFalse_ThenFractionalArrangePreserved` — FR-6.
- `When_LayoutDirectionRightToLeft_ThenMainAxisMirrored` — FR-7.
- `When_FlowDirectionAndLayoutDirectionBothSet_ThenNoDoubleMirror` — FR-7's hazard, pinned as a test so the "independent mechanisms" contract can't silently regress into double-mirroring on some target.

Plus four mapping-rule guards from the `AutoLayout` coverage audit above, one per rule, so a future convergence attempt cannot quietly regress them:

- `When_ChildHasNoShrink_ThenOverflowsInsteadOfFitting` — M1.
- `When_GrowWithBasisZero_ThenChildrenAreEqualNotProportional` — M2, the `flex: 1` vs `flex: 1 1 0` distinction.
- `When_SpaceBetweenWithGap_ThenGapIsAFloor` — M3, the behavior `AutoLayout` does *not* have.
- `When_Grow_InScrollViewer_ThenFillsViewport` — M4; no analogue in the list above, and the one most likely to break silently per target.

**Tier 3 — leak guard**, `Tests/FlexPanelLeakTests.cs`: children removed from a `FlexPanel` (including via `ItemsRepeater` recycling) are collectable after a subsequent layout pass — D9 / NFR-4.

## Docs and samples

- `doc/controls/FlexPanel.md` — property reference with the CSS-equivalent column, the `Basis`-vs-`Width` caveat, the auto-min table, and an explicit "`FlexPanel` vs `AutoLayout`: which do I want?" section.
- `doc/controls/AutoLayoutControl.md` — one cross-link paragraph pointing CSS-shaped use cases at `FlexPanel`. No behavioral claims changed.
- `samples/Uno.Toolkit.Samples/Content/Controls/FlexPanelSamplePage.xaml{,.cs}` with `[SamplePage(SampleCategory.Controls, "FlexPanel", SupportedDesigns = new[] { Design.Material, Design.Cupertino })]`, covering: direction, justify/align matrix, wrap + gap, grow/shrink/basis (app-shell + equal-columns), absolute child, and a live-resize surface for eyeballing rounding.
- No `controls-styles.md` / `lightweight-styling.md` changes (D6).

## Out of scope

- Any change to `AutoLayout` beyond the doc cross-link — no rename, no `[Obsolete]`, no shared base type, no migration shim.
- The deferred API list above.
- CSS Grid, even though `YogaStyle` carries `JustifyItems`/`JustifySelf`.
- A `Uno.Toolkit.WinUI.Markup` builder for `FlexPanel` (follow-up once the API settles).
- Upstreaming our adapter changes back to Reactor.
- Contributing the engine to `Uno.UI` proper.

## Risks and open questions

1. **Rounding parity across the Skia heads (highest).** D4 gives the escape hatch, but the per-head validation is real work — `YogaRoundingTest` at scale factors 1.0/1.25/1.5/2.0 on desktop/Skia, WASM and Skia mobile is the gate. Expect this to be where the schedule goes. (Scope narrowed with FR-8; the native heads are going away in Uno 7.0.) All 17,784 corpus assertions route through a single `YogaAssert.Equal` shim precisely so a tolerance can be introduced in one place if a head needs it.
2. **Min-content pre-measure semantics.** `ComputeMinContent` calls `Measure(0, ∞)` and then re-measures; `DesiredSize` caching and repeat-measure behavior differ subtly across Uno targets. Tier-1 tests will not catch this — tier 2 `When_AutoMin_*` on WASM/iOS will.
3. **WASM cost.** Two-pass measure over a large `FlexPanel` is more `Measure` traffic than `StackPanel`. Need a sample-page stress case (200 children) profiled on WASM before we recommend `FlexPanel` for list rows in docs.
4. **Fork drift.** Upstream is `0.1.0-preview.13` and moving weekly. Pin the SHA in `Layout/Yoga/README.md`; re-sync deliberately, never opportunistically.
5. **Open — vendor or reference?** This spec assumes vendoring. Referencing `Microsoft.UI.Reactor` is not viable (single `net10.0-windows10.0.22621` TFM, WindowsAppSDK ≥ 2.1 dependency, ships the whole framework). If Microsoft later factors the panel into a layout-only multi-target package, revisit.
6. **Open — is `FlexPanel` the final name?** It matches upstream and the `Panel` suffix convention. `FlexLayout` would echo MAUI and issue #17's title. Recommend `FlexPanel`: it *is* a `Panel`, and `AutoLayout` already burned the "Layout" suffix on a `RelativePanel` subclass.

## Phases

- [x] **P0 — Approval.** Confirm vendoring (risk 5), the name (risk 6), and the v1/deferred API split.
- [x] **P1 — Engine import.** Vendor 10 files, namespace `Uno.Toolkit.UI.Yoga` (+ `Uno.Toolkit.UI` for the public enums — see D2's correction), add `Layout/Yoga/README.md` + scoped `.editorconfig` (D7), add `THIRD-PARTY-NOTICES.md` (NFR-5). Builds clean, no adapter yet. G2 answered: **Yoga clamps negative gap to `0`**.
- [x] **P2 — Tier-1 conformance.** Port the 26 generated test files. **Gate: 100% green on desktop before any adapter work.** ✅ **544 passed / 0 failed / 46 skipped upstream** on desktop/Skia in 6s. A failure here would have been an import bug, 10× cheaper to find at this point than after P3.
- [ ] **P3 — Adapter.** `FlexPanel.cs` + `FlexPanel.Properties.cs` per D3/D4/D5. Tier-2 tests alongside, red/fix/green.
- [ ] **P4 — Cross-platform validation.** Tier 1 + 2 on the Skia heads: desktop, WASM, Skia mobile. Rounding matrix (risk 1). Tier-3 leak test.
- [ ] **P5 — Sample + docs.** Sample page, `doc/controls/FlexPanel.md`, `AutoLayoutControl.md` cross-link. WASM stress profile (risk 3).
- [ ] **P6 — Review.** Release build zero-warning on every Skia TFM; `/review-panel`; PR against #17 with the deferred list as follow-up issues.

Progress tracking lives in [`progress.md`](./progress.md) — P1/P2 results, the amendments recorded above, and the P3 handoff notes.
