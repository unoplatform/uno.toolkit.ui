# AsyncContentView — progress

Design: [spec.md](spec.md). Branch: `dev/sb/avp`.

## Plan

### 0. Design
- [x] Survey existing pieces (`ILoadable`, `LoadingView`, `LoadableSource`, `FeedView`)
- [x] Write spec (contract, control surface, state table, adapters, rejected alternatives)
- [ ] Settle name (`AsyncContentView` vs alternatives)
- [ ] Review spec with team

### 1. Contract — `src/Uno.Toolkit/`
- [ ] `IAsyncValue` / `IAsyncValue<T>` (extends `ILoadable`)
- [ ] `AsyncValue<T>` holder (`Func<CancellationToken, Task<T>>` ctor, `FromTask`, `LoadAsync` latest-wins, `Refresh` command, `lazy` opt-in)
- [ ] XML docs on all public members

### 2. Control — `src/Uno.Toolkit.UI/Controls/AsyncContentView/`
- [ ] `AsyncContentView.cs`: DPs (`Source`, `IsLoading`, `Error`, `RefreshCommand`, `LoadingContent(+Template)`, `EmptyContent(+Template)`, `ErrorTemplate`), `ResolveSource` hook, `ILoadable`
- [ ] Source subscription lifetime (Loaded/Unloaded, `IDisposable` adapters, `DispatcherCompat` marshalling)
- [ ] State derivation → three orthogonal VSM groups (`DataStates`, `ErrorStates`, `LoadingStates`), `[TemplateVisualState]`, centralised state names
- [ ] `AsyncContentView.xaml` default style (Retry button in control template bound to `RefreshCommand`, overlay progress for Loading+Value, banner for Error+Value)
- [ ] Material + Cupertino styles under `src/library/Uno.Toolkit.{Material,Cupertino}/Styles/`

### 3. Tests — `src/Uno.Toolkit.RuntimeTests/Tests/`
- [ ] State table: each row of the Loading × Data × Error matrix lands in the expected visual states
- [ ] Empty detection: `null`, empty `ICollection`, non-empty, lazy `IEnumerable` not enumerated
- [ ] `Source` projection: `IAsyncValue` drives `Content`/`IsLoading`/`Error`/`RefreshCommand`; plain object → `Content`
- [ ] `Source` precedence over loose DPs (documented behavior)
- [ ] Unloaded → unsubscribed + adapter disposed (leak guard, per `LeakTest` pattern)
- [ ] `AsyncValue<T>`: latest-wins cancellation, stale `Value` kept on error, `Refresh.CanExecute` while executing, `lazy` triggers on first subscriber
- [ ] Nests in `LoadingView` (`ILoadable` passthrough)

### 4. Samples & docs
- [ ] Sample page under `samples/Uno.Toolkit.Samples/Content/Controls/` (holder, loose-props, plain-value variants)
- [ ] `doc/controls/AsyncContentView.md`
- [ ] `doc/controls-styles.md` + `doc/lightweight-styling.md` (new resource keys)
- [ ] Cross-link from `doc/controls/LoadingView.md`

### 5. Verification
- [ ] Release build `-p:TargetFrameworkOverride=desktop` — zero warnings
- [ ] Runtime tests green on Desktop (Skia) and WASM heads

### 6. Follow-up (outside this repo)
- [ ] `Uno.Extensions.Reactive.UI`: `FeedAsyncValue` adapter (`feed.AsAsyncValue()`), optionally `FeedView : AsyncContentView`

## Review

_(filled in when implementation lands)_
