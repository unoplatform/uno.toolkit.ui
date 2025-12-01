---
uid: Toolkit.Controls.LoadingView.HowTo
tags: [loading, async, progress, spinner, iloadable, loading-state, progress-ring]
---

# Display content with a loading state while async operations run

**Packages**

* `Uno.Toolkit.UI` (required) ([Uno Platform][1])

---

## Show a ProgressRing while a command runs

**Goal:** Display custom loading UI while an async command executes.

```xml
<Page
    xmlns:utu="using:Uno.Toolkit.UI">
    <utu:LoadingView Source="{Binding FetchWeather}">
        <Grid RowDefinitions="Auto,*" Padding="16">
            <Button Grid.Row="0" Content="Refresh" Command="{Binding FetchWeather}" />
            <ListView Grid.Row="1" ItemsSource="{Binding Forecasts}" />
        </Grid>

        <utu:LoadingView.LoadingContent>
            <StackPanel Spacing="8" HorizontalAlignment="Center" VerticalAlignment="Center">
                <TextBlock Text="Loading forecasts…" />
                <ProgressRing IsActive="True" />
            </StackPanel>
        </utu:LoadingView.LoadingContent>
    </utu:LoadingView>
</Page>
```

```csharp
public interface ILoadable
{
    bool IsExecuting { get; }
    event EventHandler? IsExecutingChanged;
}

// ViewModel bits
public IReadOnlyList<WeatherItem> Forecasts { get; private set; } = Array.Empty<WeatherItem>();
public AsyncCommand FetchWeather { get; }

public ViewModel()
{
    FetchWeather = new AsyncCommand(async () =>
    {
        var data = await _service.GetForecastsAsync();
        Forecasts = data;
        OnPropertyChanged(nameof(Forecasts));
    });
}
```

`LoadingView.Source` expects an **`ILoadable`**. The sample `AsyncCommand` implements `ILoadable` and toggles `IsExecuting` while it runs. ([Uno Platform][1])

---

## Wrap your async command so it "talks" to `LoadingView`

**Goal:** Minimal `AsyncCommand` that implements `ILoadable`.

```csharp
public sealed class AsyncCommand : ICommand, ILoadable
{
    public event EventHandler? CanExecuteChanged;
    public event EventHandler? IsExecutingChanged;

    private readonly Func<Task> _execute;
    private bool _isExecuting;

    public AsyncCommand(Func<Task> execute) => _execute = execute;

    public bool IsExecuting
    {
        get => _isExecuting;
        private set
        {
            if (_isExecuting == value) return;
            _isExecuting = value;
            IsExecutingChanged?.Invoke(this, EventArgs.Empty);
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool CanExecute(object? parameter) => !IsExecuting;

    public async void Execute(object? parameter)
    {
        try { IsExecuting = true; await _execute(); }
        finally { IsExecuting = false; }
    }
}
```

Bind this command instance to `LoadingView.Source` and to your `Button.Command`. ([Uno Platform][1])

---

## Combine multiple operations into one loading state

**Goal:** Show one loader if **any** of several tasks are running.

```xml
<utu:LoadingView xmlns:utu="using:Uno.Toolkit.UI">
    <utu:LoadingView.Source>
        <utu:CompositeLoadableSource>
            <utu:LoadableSource Source="{Binding LoadUsers}" />
            <utu:LoadableSource Source="{Binding LoadReports}" />
            <utu:LoadableSource Source="{Binding SyncSettings}" />
        </utu:CompositeLoadableSource>
    </utu:LoadingView.Source>

    <StackPanel>
        <ListView ItemsSource="{Binding Users}" />
        <ListView ItemsSource="{Binding Reports}" />
        <!-- … -->
    </StackPanel>

    <utu:LoadingView.LoadingContent>
        <ProgressRing IsActive="True" />
    </utu:LoadingView.LoadingContent>
</utu:LoadingView>
```

`CompositeLoadableSource` is **executing** when **any** nested source is executing. ([Uno Platform][1])

---

## Swap the default loader UI for your own

**Goal:** Use `LoadingContent`/`LoadingContentTemplate` for a branded wait state.

```xml
<utu:LoadingView Source="{Binding DoCheckout}">
    <utu:LoadingView.LoadingContent>
        <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center" Spacing="12">
            <Image Width="64" Height="64" Source="ms-appx:///Assets/brand-spinner.png" />
            <TextBlock Text="Processing payment…" />
        </StackPanel>
    </utu:LoadingView.LoadingContent>
</utu:LoadingView>
```

Prefer `LoadingContentTemplate` if you need data-binding within the template; use `LoadingContentTemplateSelector` to pick templates dynamically. ([Uno Platform][1])

---

## FAQ (quick)

* **What is `ILoadable`?** A tiny contract with `bool IsExecuting` and `event IsExecutingChanged`. `LoadingView` listens to it. ([Uno Platform][1])
* **How do I observe multiple tasks?** Wrap each in `LoadableSource`, then aggregate with `CompositeLoadableSource`. ([Uno Platform][1])

---

## API surface (for skimmers)

* **Properties:** `Source (ILoadable)`, `LoadingContent`, `LoadingContentTemplate`, `LoadingContentTemplateSelector`, `UseTransitions (bool)`.
* **Helpers:** `LoadableSource`, `CompositeLoadableSource`. ([Uno Platform][1])

---

## Related

* **ExtendedSplashScreen** — splash built on `LoadingView`. ([Uno Platform][3])

---

*Source: Official Uno Toolkit docs and samples referenced above.*

[1]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/controls/LoadingView.html "LoadingView "
[3]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/controls/ExtendedSplashScreen.html "ExtendedSplashScreen"
