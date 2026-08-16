# SafeArea Bounds-Transition Guard — R&D Recap

## Summary

Three PRs form a progressive fix chain for a race condition in `SafeArea.UpdateInsets()` where `ApplicationView.VisibleBounds` updates before `Window.Bounds` during system-bar transitions, causing downstream UI corruption and dispatcher starvation.

| PR | Issue | Platform | Problem | Fix |
|----|-------|----------|---------|-----|
| [#1554](https://github.com/unoplatform/uno.toolkit.ui/pull/1554) | [`dispatchscience-private#74`](https://github.com/unoplatform/dispatchscience-private/issues/74) | Android | Inflated bottom inset during `StatusBar` translucency transition | Added bounds-transition deferral guard + `Math.Max(0,…)` clamping |
| [#1572](https://github.com/unoplatform/uno.toolkit.ui/pull/1572) | [`kahua-private#458`](https://github.com/unoplatform/kahua-private/issues/458) | iOS (iPad) | Infinite dispatcher loop after `FormSheet` modal dismiss | Restricted guard to `OperatingSystem.IsAndroid()` |
| [#1593](https://github.com/unoplatform/uno.toolkit.ui/pull/1593) | [`kahua-private#460`](https://github.com/unoplatform/kahua-private/issues/460) | Android | Dispatcher starvation on lock/unlock (Low-priority queue starved) | Converted infinite re-deferral to one-shot accept-and-proceed |

---

## Problem Chain

### Original Problem ([`dispatchscience-private#74`](https://github.com/unoplatform/dispatchscience-private/issues/74))

**Scenario:** Android app with `SafeArea.Insets="VisibleBounds"` on a layout containing a `TabBar` in an Auto-height Grid row. User sets `StatusBar.Background` to a translucent color.

**Root cause:** During the `StatusBar` translucency transition, `ApplicationView.VisibleBounds` updates to reflect the new translucent-bar coordinates *before* `Window.Bounds` updates. `GetWindowInsets()` computes `Bounds.Bottom - VisibleBounds.Bottom` against the stale (smaller) `Bounds`, producing an inflated bottom inset (~75.8px vs expected ~24px). This inflated value is applied as padding to the TabBar's Auto-sized row, permanently stretching it.

**Fix ([PR #1554](https://github.com/unoplatform/uno.toolkit.ui/pull/1554)):**

1. Added `Math.Max(0, …)` clamping to all four sides in `GetWindowInsets()` — prevents negative insets during transition.
2. Added a bounds-transition guard in `UpdateInsets()` with three branches:
   - **Branch 2:** `lastKnownBounds != default && visibleBoundsChanged && !boundsChanged` → Set `s_boundsTransitionPending = true`, defer via `Schedule()`, return.
   - **Branch 1:** `s_boundsTransitionPending && !boundsChanged` → Re-defer via `Schedule()`, return.
   - **Branch 3:** Else → Clear pending, update caches, proceed.
3. Added parent `InvalidateMeasure()` in `OnInsetsApplied` for Android.
4. The guard was **not platform-restricted** — it ran on all platforms.

### First Regression ([`kahua-private#458`](https://github.com/unoplatform/kahua-private/issues/458))

**Scenario:** iPad app with `SafeArea.Insets="VisibleBounds"`. User opens a `UIImagePickerController` presented as `FormSheet`/`OverFullScreen`. After dismissing the modal, taps, buttons, and back-navigation stop responding. Scroll (`UIKit` gesture recognizers) continues to work.

**Root cause:** On iPad, dismissing a `FormSheet` modal causes `VisibleBounds` to update while `Window.Bounds` stays stable — the host view was never detached, so Bounds has no reason to change. [PR #1554](https://github.com/unoplatform/uno.toolkit.ui/pull/1554)'s guard enters Branch 2 (defer), then Branch 1 fires on the deferred callback and **re-defers indefinitely** because `boundsChanged` is perpetually `false`. This infinite `Normal`-priority dispatch loop live-locks the managed `DispatcherQueue`, starving all input handling.

**Fix ([PR #1572](https://github.com/unoplatform/uno.toolkit.ui/pull/1572)):**

Changed the guard condition from `if (!HasSoftInput())` to `if (!HasSoftInput() && OperatingSystem.IsAndroid())`:

- The entire bounds-transition block is now **skipped on non-Android platforms**.
- Used `OperatingSystem.IsAndroid()` instead of `#if __ANDROID__` so the guard remains active on Skia Android builds (which use the `net9.0` TFM without the `__ANDROID__` define).
- Added test hooks (`TestHook_LastKnownBounds`, `TestHook_LastKnownVisibleBounds`, `TestHook_BoundsTransitionPending`, `TestHook_InvokeUpdateInsets`) under `#if DEBUG`.
- Added test `BoundsTransitionGuard_NotActive_OnNonAndroid`.
- Added sample page `CameraSafeAreaTestPage` for iPad manual repro.

### Second Regression ([`kahua-private#460`](https://github.com/unoplatform/kahua-private/issues/460))

**Scenario:** Android app with `SafeArea.Insets="VisibleBounds"`. User locks and unlocks the device. After unlock, low-priority `DispatcherQueue` items (e.g., timer-based updates, deferred navigation) stop executing.

**Root cause:** On some Android devices/OEM skins, locking/unlocking causes `VisibleBounds` to shift while `Window.Bounds` genuinely never changes. [PR #1554](https://github.com/unoplatform/uno.toolkit.ui/pull/1554)'s Branch 1 still re-defers indefinitely because `boundsChanged` is perpetually `false`. While [PR #1572](https://github.com/unoplatform/uno.toolkit.ui/pull/1572) restricted the guard to Android, it did not fix the infinite re-deferral loop *on Android itself*. The continuous `Normal`-priority `Schedule()` calls permanently occupy the dispatcher. Additionally, `LayoutUpdated` keeps firing from the re-layouts, piling up more `Normal`-priority dispatches. `Low`-priority `DispatcherQueue` items are permanently starved — `TryEnqueue` returns `true` (misleading callers into thinking the work was accepted) but the enqueued actions never execute because the `Normal`-priority queue is never empty.

**Fix ([PR #1593](https://github.com/unoplatform/uno.toolkit.ui/pull/1593)):**

Changed Branch 1 behavior from "re-defer" to "accept current values":

```csharp
// BEFORE (PR #1554, still present after PR #1572):
if (s_boundsTransitionPending && !boundsChanged)
{
    Owner?.GetDispatcherCompat().Schedule(() => UpdateInsets(forceUpdate: true));
    return; // ← infinite loop if Bounds never catches up
}

// AFTER (PR #1593):
if (s_boundsTransitionPending && !boundsChanged)
{
    // Accept current values — no infinite re-deferral.
    s_boundsTransitionPending = false;
    s_lastKnownBounds = currentBounds;
    s_lastKnownVisibleBounds = currentVB;
    // Falls through to UpdateSafeAreaOverride()
}
```

This preserves [PR #1554](https://github.com/unoplatform/uno.toolkit.ui/pull/1554)'s one-deferral opportunity for `Window.Bounds` to catch up (via Branch 2 → defer once) while guaranteeing termination: if Bounds doesn't catch up after one dispatch cycle, the guard accepts current values and proceeds.

#### Device Dependency

The bug is device-dependent because it requires two conditions to hold simultaneously:

1. `VisibleBounds` changes during lock/unlock (most Android devices).
2. `Window.Bounds` does **not** change during the same transition (varies by OEM skin, navigation mode, display cutout, and API level).

When both conditions hold, `s_boundsTransitionPending` is set but never cleared, creating the infinite loop. On some devices the timing gap is so short that by the next dispatch tick both values have caught up — those devices never hit the bug because Branch 3 fires and clears the flag normally.

---

## Regression Safety Analysis

### [PR #1593](https://github.com/unoplatform/uno.toolkit.ui/pull/1593) vs [PR #1572](https://github.com/unoplatform/uno.toolkit.ui/pull/1572) (iPad live-lock)

**No regression.** [PR #1572](https://github.com/unoplatform/uno.toolkit.ui/pull/1572) gates the entire bounds-transition block with `OperatingSystem.IsAndroid()`. [PR #1593](https://github.com/unoplatform/uno.toolkit.ui/pull/1593)'s changes are *entirely inside* that gate. On iOS/iPad/WASM/Windows/macOS, the gate skips the block — [PR #1593](https://github.com/unoplatform/uno.toolkit.ui/pull/1593) has zero effect on non-Android platforms.

### [PR #1593](https://github.com/unoplatform/uno.toolkit.ui/pull/1593) vs [PR #1554](https://github.com/unoplatform/uno.toolkit.ui/pull/1554) (inflated bottom inset)

**No regression.** The `StatusBar` translucency transition scenario works as follows:

1. VB changes → Branch 2 fires → defers once (unchanged by [PR #1593](https://github.com/unoplatform/uno.toolkit.ui/pull/1593)).
2. On the deferred callback, either:
   - **Bounds caught up** → Branch 3 fires → normal update with correct bounds. ✅
   - **Bounds didn't catch up** → Branch 1 fires → accepts current values ([PR #1593](https://github.com/unoplatform/uno.toolkit.ui/pull/1593)'s change).

In the translucency transition, `Window.Bounds` *does* catch up within one dispatch cycle, so the happy path (Branch 3) fires. Even in the theoretical case where it doesn't, the `Math.Max(0, …)` clamping from [PR #1554](https://github.com/unoplatform/uno.toolkit.ui/pull/1554) prevents negative insets, and subsequent `LayoutUpdated` / `SizeChanged` events will trigger `UpdateInsets` again with the final correct values.

---

## Guard Logic (Final State)

```text
UpdateInsets() called
│
├── HasSoftInput()? ──yes──► Skip guard entirely (keyboard is up)
│
├── OperatingSystem.IsAndroid()? ──no──► Skip guard entirely (PR #1572)
│
└── Android, no soft input:
    │
    ├── Branch 1: pending && !boundsChanged
    │   → Accept current values, clear pending, proceed (PR #1593)
    │
    ├── Branch 2: lastKnownBounds != default && VB changed && !boundsChanged
    │   → Set pending, defer once via Schedule(), return
    │
    └── Branch 3: else (bounds caught up or no transition)
        → Clear pending, update caches, proceed
```

**Invariant:** At most one deferral per VB-change event. Infinite loops are structurally impossible because Branch 1 always terminates.

---

## Test Coverage

### Non-Android Tests (`#if DEBUG && !__ANDROID__ && !WINDOWS_WINUI`)

| Test | Covers |
|------|--------|
| `BoundsTransitionGuard_NotActive_OnNonAndroid` | Guard doesn't engage when VB changes ahead of Bounds (iPad live-lock regression guard) |
| `BoundsTransitionGuard_IgnoredForAllStates_OnNonAndroid` | Stale pending flag stays unchanged (guard block entirely skipped) |

### Android Tests (`#if DEBUG && __ANDROID__`)

| Test | Covers |
|------|--------|
| `BoundsTransition_DefersOnce_WhenVisibleBoundsChangeAheadOfWindowBounds` | Branch 2: VB changed, Bounds didn't → pending set |
| `BoundsTransition_AcceptsCurrent_AfterSingleDeferral` | Branch 1: pending + Bounds unchanged → accept, clear, update all caches |
| `BoundsTransition_ClearsGuard_WhenBoundsCatchUp` | Branch 3: Bounds caught up → pending cleared |
| `BoundsTransition_NoDeferral_WhenLastKnownBoundsIsDefault` | Edge case: first-ever call with default bounds → no deferral |
| `BoundsTransition_FullCycle_DefersOnceThenAccepts` | Integration: Branch 2 → Branch 1 (Bounds never catch up — [PR #1593](https://github.com/unoplatform/uno.toolkit.ui/pull/1593) fix) |
| `BoundsTransition_FullCycle_DefersOnceThenBoundsCatchUp` | Integration: Branch 2 → Branch 3 (Bounds catch up — [PR #1554](https://github.com/unoplatform/uno.toolkit.ui/pull/1554) happy path) |

### Android UI Tests (`#if __ANDROID__`)

| Test | Covers |
|------|--------|
| `Translucent_SystemBars` | Static translucent bars: correct inset padding applied |
| `Translucent_SystemBars_Dynamic` | Dynamic opaque→translucent transition: insets update correctly |
| `BottomInset_NotInflated_WhenTransitioningToTranslucentBars` | [PR #1554](https://github.com/unoplatform/uno.toolkit.ui/pull/1554) regression guard: TabBar height doesn't inflate during transition |

### Manual Repro Page

`CameraSafeAreaTestPage` (added by [PR #1572](https://github.com/unoplatform/uno.toolkit.ui/pull/1572)): iPad-only page with a `UIImagePickerController` `FormSheet` to manually verify tap counter remains responsive after modal dismiss.

---

## Key Design Decisions

1. **`OperatingSystem.IsAndroid()` vs `#if __ANDROID__`**: Runtime check is required because Skia Android builds use the `net9.0` TFM without the `__ANDROID__` compiler define. The guard must be active on Skia Android.

2. **One-shot deferral vs zero deferral**: Zero deferral (removing the guard entirely on Android) would re-expose the [PR #1554](https://github.com/unoplatform/uno.toolkit.ui/pull/1554) bug. One-shot deferral gives `Window.Bounds` exactly one dispatch cycle to catch up — sufficient for the StatusBar translucency transition — while preventing infinite loops.

3. **Accept-and-proceed vs accept-and-return**: Branch 1 falls through to `UpdateSafeAreaOverride()` instead of returning. This ensures insets are computed with the best-available values even when Bounds didn't catch up.

4. **Static fields for bounds tracking**: `s_lastKnownBounds`, `s_lastKnownVisibleBounds`, and `s_boundsTransitionPending` are static because `Window.Bounds` and `VisibleBounds` are app-global — there's one window, one set of system bars. All `SafeAreaDetails` instances observe the same transition.

---

## File References

- Implementation: `src/Uno.Toolkit.UI/Controls/SafeArea/SafeArea.cs` (lines ~470–530)
- Tests: `src/Uno.Toolkit.RuntimeTests/Tests/SafeAreaTests.cs`
- iPad repro page: `samples/Uno.Toolkit.Samples/Content/TestPages/CameraSafeAreaTestPage.WinUI.xaml`
