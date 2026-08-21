# Feature: `AsyncContentView` — async value presenter for MVVM (and MVUX via adapter)

**Area**: `Uno.Toolkit` (contract) + `Uno.Toolkit.UI` — `Controls/AsyncContentView`
**Type**: New control + new core contract
**Branch**: `dev/sb/avp`
**Status**: Design — not yet implemented

## Summary

MVUX has [`FeedView`](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Mvux/FeedView.html): bind an `IFeed<T>` and get loading / error / empty / value rendering for free. Plain-MVVM apps have nothing equivalent in the toolkit — `LoadingView` covers loading/loaded only — so every page hand-rolls a `Grid` with three `Visibility` bindings.

This spec proposes a lightweight control, `AsyncContentView`, that renders the universal four-state shape (loading / error / empty / value) from the MVVM lingua franca (`IsBusy` / `Data` / `Error` / command), plus one small contract, `IAsyncValue`, so that a single `Source` binding can drive it — from a toolkit-provided holder, from a hand-written implementation, or from an MVUX feed through an adapter that lives in `Uno.Extensions.Reactive.UI`.

Design goals, in priority order:

1. Works with "normal" MVVM (CommunityToolkit.Mvvm, hand-rolled INPC) with **zero** toolkit types in the ViewModel.
2. One optional contract (`IAsyncValue`) for the one-binding path; the control knows only that contract.
3. MVUX support is an adapter, not control logic. The toolkit never references `Uno.Extensions`.
4. Composes with what already exists: `ContentControl` for the value, `ILoadable` / `LoadingView` for nesting, orthogonal VSM groups borrowed from `FeedView`.

## What exists today

| Piece | Where | Role |
|---|---|---|
| `ILoadable` (`IsExecuting` + `IsExecutingChanged`) | `src/Uno.Toolkit/ILoadable.cs` (netstandard2.0, no UI deps) | Busy signal VMs can implement without referencing UI |
| `LoadingView` | `src/Uno.Toolkit.UI/Controls/LoadingView/` | `Source : ILoadable`, `Loading`/`Loaded` VSM, `LoadingContent(+Template)` |
| `LoadableSource` / `CompositeLoadableSource` | same folder | DP adapter / aggregate for `ILoadable` |
| `AncestorBinding` | `src/Uno.Toolkit.UI/Markup/AncestorBindingExtension.cs` | Reach the VM from inside a `DataTemplate` |
| `FeedView` | `uno.extensions/src/Uno.Extensions.Reactive.UI/View/FeedView*.cs` | `Source : object` (cast to `ISignal<IMessage>`), `FeedViewState` template DataContext (`Data`/`Error`/`Progress`/`Refresh`/`Parent`), three orthogonal VSM groups (`DataGroup` Undefined/None/Some, `ErrorGroup`, `ProgressGroup`), `RefreshingState` enum, implements `ILoadable` so it nests in `LoadingView` |

## 1. The contract — `Uno.Toolkit` (netstandard2.0)

Lives next to `ILoadable`, for the same reason: ViewModels can implement/consume it without a UI dependency.

```csharp
namespace Uno.Toolkit;

/// <summary>A value produced asynchronously: last known Value, last Error, in-flight flag, optional re-load.</summary>
public interface IAsyncValue : ILoadable           // IsExecuting + IsExecutingChanged come from ILoadable
{
    object? Value { get; }                         // stale value is kept while reloading / after error
    Exception? Error { get; }
    ICommand? Refresh { get; }                     // null ⇒ not refreshable
    event EventHandler? ValueChanged;              // raised when Value or Error change
}

public interface IAsyncValue<T> : IAsyncValue
{
    new T? Value { get; }
}
```

Decisions:

- **Extends `ILoadable`, not `INotifyPropertyChanged`.** Keeps the toolkit's event-per-concern style and makes every `IAsyncValue` droppable into `LoadingView.Source` / `ExtendedSplashScreen` for free. It also deliberately discourages page-VMs from *being* the value — the holder (§3) is the unit.
- `Error` is `Exception?` on the contract (async failures are exceptions; the holder, `Task`, and MVUX all have one). The control's `Error` DP is `object?` (superset) so string-error VMs can bind it directly.
- `Refresh` is an `ICommand?` (netstandard2.0 has `System.Windows.Input.ICommand`) so templates can bind a button to it and get `CanExecute` for free.

## 2. The control — `Uno.Toolkit.UI`

```csharp
public partial class AsyncContentView : ContentControl, ILoadable
{
    // Driver (optional). `object` so subclasses/adapters can accept other shapes.
    public object? Source { get; set; }                       // DP

    // State. Bindable directly (loose MVVM) — or pushed by Source.  Rule: Source wins.
    // Content (from ContentControl) = the value
    public bool IsLoading { get; set; }                       // DP
    public object? Error { get; set; }                        // DP  (object, so string errors work)
    public ICommand? RefreshCommand { get; set; }             // DP

    // Per-state content, LoadingView naming
    public object? LoadingContent; public DataTemplate? LoadingContentTemplate;   // DPs
    public object? EmptyContent;   public DataTemplate? EmptyContentTemplate;     // DPs
    public DataTemplate? ErrorTemplate;                       // DP, DataContext = Error

    /// <summary>Turn an arbitrary Source into the contract. Base: IAsyncValue as-is; anything else is a constant value.</summary>
    protected virtual IAsyncValue? ResolveSource(object? source) => source as IAsyncValue;

    bool ILoadable.IsExecuting => IsLoading;                  // nests inside LoadingView like FeedView does
}
```

Behavior:

- **Lifetime** (same rule as `FeedView`): on `Loaded` / `Source` change → `_resolved = ResolveSource(Source)`, subscribe `ValueChanged` + `IsExecutingChanged`, project `Value→Content`, `Error→Error`, `IsExecuting→IsLoading`, `Refresh→RefreshCommand`. On `Unloaded` → unsubscribe and `(_resolved as IDisposable)?.Dispose()`.
- **Threading**: projections are marshalled through `DispatcherCompat` (as `LoadableSource` does) — adapters may raise events from background threads.
- **Plain values**: `ResolveSource` returns `null` for a non-`IAsyncValue` → `Content = Source`. A bound `List<T>` just works (Empty/Value only).
- **Precedence**: if both `Source` and the loose DPs are set, `Source` wins (the projection is a local `SetValue`, which replaces a one-way binding). Documented, not guarded.

### State derivation

Pure function of the four DPs, re-run on any change. Three **orthogonal** VSM groups (the one genuinely good idea borrowed from `FeedView`) so "stale value + spinner" and "stale value + error banner" fall out for free:

| Group | States | Rule |
|---|---|---|
| `DataStates` | `Empty` / `Value` | `Content is null` or `ICollection { Count: 0 }` → `Empty`, else `Value`. Lazy `IEnumerable` is **not** enumerated. |
| `ErrorStates` | `NoError` / `Error` | `Error is not null` → `Error` |
| `LoadingStates` | `Idle` / `Loading` | `IsLoading` → `Loading` |

The default template resolves the combinations:

| Loading | Data | Error | Renders |
|---|---|---|---|
| ✓ | Empty | — | `LoadingContent` only |
| ✓ | Value | — | value + overlay progress (replaces `FeedView.RefreshingState` entirely) |
| — | Value | ✓ | value + dismissable error banner |
| — | Empty | ✓ | `ErrorTemplate` + Retry |
| — | Empty | — | `EmptyContent` (blank if unset) |
| — | Value | — | value |

The **control template** owns the Retry button (`TemplateBinding RefreshCommand`, collapsed when null), so `ErrorTemplate` stays a pure "render the error" `DataTemplate` (DataContext = `Error`) — no `FeedViewState`-style wrapper object and no `{Binding Data.X}` prefix. `ContentTemplate`'s DataContext is the value, like any `ContentControl`; reach the VM with `utu:AncestorBinding` / `ElementName`.

`[TemplateVisualState]` attributes for all six states; state names centralised in a private `VisualStateNames` class (per `LoadingView`).

## 3. Adapters

### MVVM holder — `Uno.Toolkit` (the `TaskNotifier` / `NotifyTask` role, ~60 lines)

```csharp
public sealed class AsyncValue<T> : IAsyncValue<T>
{
    public AsyncValue(Func<CancellationToken, Task<T>> load, bool lazy = false);   // lazy ⇒ first subscriber triggers LoadAsync (FeedView-like)
    public static AsyncValue<T> FromTask(Task<T> task);                            // one-shot, Refresh = null

    public T? Value { get; }  public Exception? Error { get; }  public bool IsExecuting { get; }
    public ICommand? Refresh { get; }                                               // => LoadAsync(); CanExecute = !IsExecuting
    public Task LoadAsync(CancellationToken ct = default);                         // latest-wins: cancels in-flight, keeps stale Value on error
    public event EventHandler? ValueChanged, IsExecutingChanged;
}
```

```csharp
// VM (CommunityToolkit or anything)
public AsyncValue<List<Customer>> Customers { get; } = new(ct => api.GetCustomersAsync(ct), lazy: true);
```

### MVUX — `Uno.Extensions.Reactive.UI` (already references `Uno.Toolkit` for `ILoadable`)

```csharp
internal sealed class FeedAsyncValue : IAsyncValue, IDisposable
{
    // = today's FeedView.Subscription, minus the VSM: FeedUIHelper.GetSource(feed, ctx) with ctx resolved from
    //   view.DataContext / page DataContext / SourceContext.GetOrCreate(view);
    //   message → Value = Data.SomeOrDefault(), Error = Current.Error, IsExecuting = Current.IsTransient,
    //   Refresh = RequestSource.RequestRefresh() wrapped as ICommand
}

public partial class FeedView : AsyncContentView            // FeedView keeps its tag; gets the toolkit template
{
    protected override IAsyncValue? ResolveSource(object? source)
        => source is ISignal<IMessage> feed ? new FeedAsyncValue(this, feed) : base.ResolveSource(source);
}
```

Alternative (if the bare `<utu:AsyncContentView Source="{Binding SomeFeed}">` tag should work without a subclass): replace the `protected virtual` with a static `AsyncContentView.SourceAdapters` list that `Reactive.UI` registers into from a module initializer. Same adapter, global state instead of a subclass. Not preferred.

Note: `FeedView : AsyncContentView` changes FeedView's template contract (`{Binding Data.X}` → `{Binding X}`, state/template names). That is a `Reactive.UI` major-version conversation. The adapter alone (`FeedAsyncValue` + `feed.AsAsyncValue()`) is additive and can ship first.

## 4. Call sites

```xml
<!-- MVUX — unchanged -->
<reactive:FeedView Source="{Binding Customers}" ContentTemplate="{StaticResource CustomerList}" />

<!-- MVVM, holder -->
<utu:AsyncContentView Source="{Binding Customers}" ContentTemplate="{StaticResource CustomerList}" />

<!-- MVVM, loose props (no toolkit type in the VM) -->
<utu:AsyncContentView Content="{Binding Customers}"
                      IsLoading="{Binding LoadCommand.IsRunning}"
                      Error="{Binding LoadError}"
                      RefreshCommand="{Binding LoadCommand}"
                      ContentTemplate="{StaticResource CustomerList}">
    <utu:AsyncContentView.EmptyContent>
        <TextBlock Text="No customers yet" />
    </utu:AsyncContentView.EmptyContent>
</utu:AsyncContentView>

<!-- plain value — Empty/Value only -->
<utu:AsyncContentView Source="{Binding Customers}" ContentTemplate="{StaticResource CustomerList}" />
```

## 5. Decisions & rejected alternatives

| Decision | Why |
|---|---|
| Loose DPs (`Content`/`IsLoading`/`Error`/`RefreshCommand`) are the primitive; `Source` is sugar | Works with every MVVM framework with zero adapters. `IAsyncValue` is layered on top, exactly like `LoadingView.Source : ILoadable` + `LoadableSource`. |
| No raw `Task<T>` as `Source` | Reading `Result` off a non-generic `Task` needs reflection → trimming/AOT hostile. `IAsyncRelayCommand.IsRunning` / `ExecutionTask` already give MVVM users what they need; `AsyncValue<T>.FromTask` covers the rest. |
| No `FeedViewState` wrapper for templates | `ContentTemplate` DataContext = value, `ErrorTemplate` DataContext = error, Retry lives in the control template. Avoids the `Data.` / `Parent.` prefixes; MVVM users already use `AncestorBinding` / `ElementName`. |
| No `Undefined` state | MVVM has no "never emitted". `Source == null` renders as `Empty` (blank unless `EmptyContent` is set). INPC bindings resolve synchronously on `DataContext` set, so the flash-of-empty is rare. |
| No `RefreshingState` enum | Orthogonal `LoadingStates` × `DataStates` already express "value + spinner" vs "spinner only". |
| Not folded into `LoadingView` | Its `Content` is "the page when done" (DataContext-inherited) and `Source == null` means *loading*. Bolting `Content`-as-value + `IsLoading` onto it puts two mental models in one control. |
| `ContentControl` base, not `Control` + `ValueTemplate` | `Content`/`ContentTemplate`/`ContentTemplateSelector`/alignment for free; renders without a template. Costs the `FeedView` inline-`<DataTemplate>` ergonomics (`ContentProperty = ValueTemplate`) — acceptable. |
| `IAsyncValue : ILoadable` (event-based), not `INotifyPropertyChanged` | Toolkit style consistency; free `LoadingView` composition; discourages page-VMs from implementing it. |
| Name `AsyncContentView` | Templated → `*View`, not `*Presenter` (XAML `*Presenter` implies a non-templated element). `DataView` collides with `System.Data.DataView`. Bikeshed — open. |

## 6. Open questions

- Name (`AsyncContentView` vs `AsyncValueView` vs …).
- Should `IEmpty`-ness also consider `IReadOnlyCollection<T>` (generic → needs reflection or a registered check)? Current answer: non-generic `ICollection` only.
- `AsyncValue<T>.lazy` default — `false` (explicit `LoadAsync`) or `true` (FeedView parity)? Current answer: `false`.
- Does `Reactive.UI` want `FeedView : AsyncContentView` (template-breaking) or just the additive adapter? Toolkit side is unaffected either way.

## 7. Deliverables (see `progress.md`)

- `src/Uno.Toolkit/IAsyncValue.cs`, `AsyncValue.cs`
- `src/Uno.Toolkit.UI/Controls/AsyncContentView/AsyncContentView.cs` + `.xaml` (picked up by the `XamlMergeInput` glob)
- Material / Cupertino styles under `src/library/Uno.Toolkit.{Material,Cupertino}/Styles/`
- Runtime tests: `src/Uno.Toolkit.RuntimeTests/Tests/AsyncContentViewTests.cs` (+ `TestPages/`) covering the state table, `Source` projection/precedence, `Unloaded` teardown (leak guard), `AsyncValue<T>` latest-wins/stale-value semantics
- Sample page under `samples/Uno.Toolkit.Samples/Content/Controls/`
- Docs: `doc/controls/AsyncContentView.md`; `doc/controls-styles.md` + `doc/lightweight-styling.md` for new resource keys; cross-link from `doc/controls/LoadingView.md`
