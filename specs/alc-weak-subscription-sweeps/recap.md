# ALC Weak-Subscription / Pin Sweeps — Recap

## Context

A downstream host that loads previewed apps into their own collectible
`AssemblyLoadContext`s needs those ALCs to be fully collectible after the previewed
app is unloaded. Several Toolkit types subscribe to **process-global event sources**
or hold **process-lifetime static references** with either no teardown, or a teardown
path that only runs on an event that may never fire in a fixed-size preview canvas.

In an ordinary app these references are harmless because the process, the Toolkit
assembly, the window, and the app all share one lifetime. Across a collectible-ALC
boundary they become **hard pins**: the global source (or a Toolkit static) strongly
roots an object graph that belongs to the previewed app's ALC, so the ALc's
`LoaderAllocator` is never released and the ALC leaks for the process lifetime.

This spec tracks a batch of low-risk sweeps that make those subscriptions weak /
self-detaching, or add explicit purge hooks, without changing behavior for normal apps.

Backing issue: **#1607**.

The weak self-detaching handler pattern reuses the template established for the
`SafeArea` process-global `InputPane` / `ApplicationView` subscriptions
(`CreateWeakHandler`): a `WeakReference<T>` to the target plus **static, non-capturing**
`onEvent` / `detach` delegates so the global source holds only a weak reference, and the
wrapper detaches itself once the target is collected.

## Findings

| # | Type | Problem | Fix |
|---|------|---------|-----|
| 1 | `ResponsiveHelper.InitializeIfNeeded` | Strong `XamlRoot.Changed` lambda captures the process-lifetime static `_defaultSizeProvider`; no teardown, no reset. Host `XamlRoot` roots the Toolkit static chain for the process lifetime. | Weak self-detaching `XamlRoot.Changed` handler + internal `Reset()` purge hook. |
| 2 | `DependencyObjectExtensions._dependencyPropertyReflectionCache` | `Type`-keyed static dictionary never releases `Type` keys → a `Type` from a collectible ALC keeps that ALC alive. | Release entries whose `Type` belongs to an unloaded/collected ALC. |
| 3 | `NavigationBar` → `SystemNavigationManager.BackRequested` | Strong sub; detached on `Unloaded`, but `Unloaded` does not fire on abrupt ALC teardown. | Weak self-detaching handler; keep the fast `Unloaded` path. |
| 4 | `ResponsiveExtension` | `HardSelfReferences` + static `WindowSizeChanged` sub + `TrackedInstances` accumulate per cycle; cleanup only runs from `OnWindowSizeChanged`, which never fires in a fixed preview canvas. | Unloaded-hooked removal + dead-host sweep on `Connect` + weak handler; prune dead weak-refs in `TrackedInstances`. |
| 5 | `FrameworkElementExtensions.SubscribeToNestedElements` | Returned disposable clears its subscription list **without invoking** the recorded unsubscribe actions → `Loaded` handlers stay attached. | Invoke each recorded `Unsubscribe()` on dispose. |
| 6 | `ExtendedSplashScreen` | Static `Instance` + retained splash bitmap never released. | Clear on `Unloaded`; drop `SplashScreenContent` after the loaded transition. |

### Out of scope (follow-ups only)

- `ShadowsCache` dispose-on-evict
- `StatusBar` teardown
- `ResponsiveView` slot cache

## Finding 1 — ResponsiveHelper (landed)

**Before**

```csharp
provider.Changed += (s, e) => _defaultSizeProvider.Size = s.Size;
```

The lambda captures the static `_defaultSizeProvider`. The host `XamlRoot` (window-owned,
outlives previewed apps) holds this delegate forever, so the Toolkit static chain — and
transitively the Toolkit ALC — is pinned for the process lifetime. There was no teardown
path at all.

**After**

- A `CreateWeakHandler<XamlRoot, XamlRootChangedEventArgs>` wrapper holds a
  `WeakReference<ResponsiveSizeProvider>` and static non-capturing `onEvent`/`detach`
  delegates; the `XamlRoot` now holds only a weak reference back and the wrapper
  self-detaches once the provider is collected.
- The subscription is also recorded in a `SerialDisposable` (`_defaultSizeProviderDisposable`).
- New internal `Reset()` tears down the default and override providers and their
  subscriptions and clears `WindowSize`, so a host can fully reset the singleton between
  app loads.

**Tests** (`ResponsiveHelperLeakTests`, `#if DEBUG`, runtime):

- `Reset_TearsDown_DefaultProvider` — deterministic: `InitializeIfNeeded` initializes,
  `Reset` tears down (`TestHook_IsDefaultProviderInitialized == false`), and a subsequent
  `InitializeIfNeeded` re-initializes cleanly.
- `DefaultProvider_IsCollectible_WhileXamlRootAlive` — GC proof: while the host `XamlRoot`
  is kept strongly alive and subscribed, the default provider becomes collectible after
  `Reset()`. On the pre-fix strong-lambda code the `XamlRoot.Changed` closure would keep
  the provider alive (red); with the weak handler it is collected (green).

Test hooks added under `#if DEBUG`: `TestHook_IsDefaultProviderInitialized`,
`TestHook_GetDefaultProvider()`.

## Progress

- [x] Finding 1 — ResponsiveHelper weak handler + Reset
- [x] Finding 2 — DependencyObjectExtensions reflection-cache keyed weakly by Type (ConditionalWeakTable)
- [x] Finding 3 — NavigationBar BackRequested weak self-detaching handler (keeps Unloaded fast path)
- [x] Finding 4 — ResponsiveExtension weak WindowSizeChanged sub + Unloaded teardown + dead-instance sweep
- [x] Finding 5 — SubscribeToNestedElements dispose invokes recorded Unsubscribe actions
- [x] Finding 6 — ExtendedSplashScreen releases static Instance + splash content (Android: recycles bitmap) on Unloaded

All fixes + red/green tests compile clean on `net9.0` (Toolkit.UI + RuntimeTests, 0 warnings).

## Tests (all runtime, `#if DEBUG`)

| Finding | Test class | Tests |
|---------|-----------|-------|
| 1 | `ResponsiveHelperLeakTests` | `Reset_TearsDown_DefaultProvider`, `DefaultProvider_IsCollectible_WhileXamlRootAlive` |
| 2 | `DependencyObjectExtensionsLeakTests` | `ReflectionCache_DoesNotRoot_CollectibleTypeKey` (RunAndCollect dynamic assembly) |
| 3 | `NavigationBarLeakTests` | `BackRequested_Subscription_IsWeak_WhenUnloadedNeverFires` |
| 4 | `ResponsiveExtensionsLeakTests` | `SweepDeadInstances_Prunes_CollectedTrackedInstances`, `Connected_Extension_IsCollectible_WithoutResizeOrExplicitUninstall` |
| 5 | `FrameworkElementExtensionsLeakTests` | `SubscribeToNestedElements_Dispose_DetachesLoadedHandlers` |
| 6 | `ExtendedSplashScreenLeakTests` | `Instance_IsReleased_OnUnloaded`, `SplashScreen_IsCollectible_AfterUnloaded` |
