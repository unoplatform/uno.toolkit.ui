---
uid: Toolkit.Controls.LoadingView
---

# LoadingView

Represents a control that indicates that the UI is waiting on a task to complete.

> [!Video https://www.youtube-nocookie.com/embed/3cpjJ3keBvM]

## Properties

| Property                         | Type                   | Description                                                                                         |
|----------------------------------|------------------------|-----------------------------------------------------------------------------------------------------|
| `Source`                         | `ILoadable`            | Gets and sets the source `ILoadable` associated with this control.                                  |
| `LoadingContent`                 | `object`               | Gets or sets the content to be displayed during loading/waiting.                                    |
| `LoadingContentTemplate`         | `DataTemplate`         | Gets or sets the template to be used to display the LoadingContent during loading/waiting.          |
| `LoadingContentTemplateSelector` | `DataTemplateSelector` | Gets or sets the template selector to be used to display the LoadingContent during loading/waiting. |
| `UseTransitions`                 | `bool`                 | Gets and sets whether transitions will run when going between states.                               |

## ILoadable

Describes if this instance is currently in a busy state and notifies subscribers that said state when has changed.

### Members

| Property             | Type            | Description                                                |
|----------------------|-----------------|------------------------------------------------------------|
| `IsExecuting`        | `bool`          | Indicates whether the instance is doing work.              |
| `IsExecutingChanged` | `EventHandler?` | Event that fires when the `IsExecuting` state has changed. |

### LoadableSource

Represents an `ILoadable` that forwards the `ILoadable.IsExecuting` state of its Source.

### CompositeLoadableSource

Represents an `ILoadable` aggregate that is `ILoadable.IsExecuting` when any of its nested Sources is.

## Usage

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<!-- single source example -->
<utu:LoadingView Source="{Binding FetchWeatherForecasts}">
    <Grid RowDefinitions="Auto,*">
        <Button Grid.Row="0" Content="Refresh" Command="{Binding FetchWeatherForecasts}">

        <ListView Grid.Row="1" ItemsSource="{Binding Forecasts}" />
    </Grid>

    <utu:LoadingView.LoadingContent>
        <StackPanel>
            <TextBlock Text="Scrying..." />
            <Image Source="ms-appx:///Assets/CrystalBall.jpg" />
        </StackPanel>
    </utu:LoadingView.LoadingContent>
<utu:LoadingView>

<!-- multi-sources example -->
<utu:LoadingView>
    <utu:LoadingView.Source>
        <utu:CompositeLoadableSource>
            <utu:LoadableSource Source="{Binding LoadContent0Command}" />
            <utu:LoadableSource Source="{Binding LoadContent1Command}" />
        </utu:CompositeLoadableSource>
    </utu:LoadingView.Source>

    <StackPanel>
        <ListView ItemsSource="{Binding Content0}" />
        <ListView ItemsSource="{Binding Content1}" />
    </StackPanel>

    <utu:LoadingView.LoadingContent>
        <ProgressRing IsActive="True" />
    </utu:LoadingView.LoadingContent>
<utu:LoadingView>
```

```csharp
private class ViewModel : ViewModelBase
{
    public string Content0 { get => GetProperty<string>(); set => SetProperty(value); }
    public IEnumerable<int> Content1 { get => GetProperty<IEnumerable<int>>(); set => SetProperty(value); }

    public AsyncCommand LoadContent0Command { get; }
    public AsyncCommand LoadContent1Command { get; }

    private readonly Random _random = new Random();

    public ViewModel()
    {
        LoadContent0Command = new AsyncCommand(LoadContent0);
        LoadContent1Command = new AsyncCommand(LoadContent1);
    }

    public Task LoadContent0() => SimulateFetchOrWork(500, () => Content0 = DateTime.Now.ToString());
    public Task LoadContent1() => SimulateFetchOrWork(2000, () => Content1 = Enumerable.Range(0, _random.Next(3, 12)));

    private async Task SimulateFetchOrWork(int delay, Action onCompleted)
    {
        await Task.Delay(delay);
        onCompleted();
    }
}
```

AsyncCommand implements ILoadable to notify the LoadingView of ongoing work:

```csharp
public class AsyncCommand : ICommand, ILoadable
{
    public event EventHandler CanExecuteChanged;
    public event EventHandler IsExecutingChanged;

    private Func<Task> _executeAsync;
    private bool _isExecuting;

    public AsyncCommand(Func<Task> executeAsync)
    {
        _executeAsync = executeAsync;
    }

    public bool CanExecute(object parameter) => !IsExecuting;

    public bool IsExecuting
    {
        get => _isExecuting;
        set
        {
            if (_isExecuting != value)
            {
                _isExecuting = value;
                IsExecutingChanged?.Invoke(this, new());
                CanExecuteChanged?.Invoke(this, new());
            }
        }
    }

    public async void Execute(object parameter)
    {
        try
        {
            IsExecuting = true;
            await _executeAsync();
        }
        finally
        {
            IsExecuting = false;
        }
    }
}
```
