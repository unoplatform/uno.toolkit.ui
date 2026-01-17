---
uid: Toolkit.Helpers.EventToCommandExtensions
---

# Event to Command Extensions

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

Provides attached properties to bind events to commands in XAML. This allows you to invoke a command when any event is raised, similar to .NET MAUI's [EventToCommandBehavior](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/behaviors/event-to-command-behavior).

## Attached Properties

| Property                  | Type            | Description                                                                                      |
|---------------------------|-----------------|--------------------------------------------------------------------------------------------------|
| `Event`                   | `string`        | The name of the event to subscribe to (e.g., "Click", "Tapped", "SelectionChanged").             |
| `Command`                 | `ICommand`      | The command to execute when the event is raised.                                                 |
| `CommandParameter`        | `object`        | An optional parameter to pass to the command. If not set, see `PassEventArgsToCommand`.         |
| `PassEventArgsToCommand`  | `bool`          | When `true`, the event arguments are passed to the command as the parameter.                     |
| `EventArgsConverter`      | `IValueConverter` | An optional converter to transform event arguments before passing to the command.              |

## Remarks

- The `Event` property specifies which event to listen to on the target element.
- When the specified event is raised, the `Command` is executed.
- If `CommandParameter` is set, it takes precedence as the command parameter.
- If `CommandParameter` is not set and `PassEventArgsToCommand` is `true`, the event arguments are passed to the command.
- If `EventArgsConverter` is also set, it is applied to the event arguments before passing them to the command.
- The `Command.CanExecute` is checked before executing the command.

## Usage

```xml
<!-- Include the following XAML namespace to use the samples below -->
xmlns:utu="using:Uno.Toolkit.UI"
...

<!-- Execute command on button click -->
<Button Content="Click Me"
        utu:EventToCommandExtensions.Event="Click"
        utu:EventToCommandExtensions.Command="{Binding MyClickCommand}" />

<!-- Execute command on selection changed with event args -->
<ComboBox ItemsSource="{Binding Items}"
          utu:EventToCommandExtensions.Event="SelectionChanged"
          utu:EventToCommandExtensions.Command="{Binding SelectionChangedCommand}"
          utu:EventToCommandExtensions.PassEventArgsToCommand="True" />

<!-- Execute command with a custom parameter -->
<Button Content="Submit"
        utu:EventToCommandExtensions.Event="Click"
        utu:EventToCommandExtensions.Command="{Binding SubmitCommand}"
        utu:EventToCommandExtensions.CommandParameter="FormSubmit" />

<!-- Execute command on TextBox text changed with converter -->
<TextBox utu:EventToCommandExtensions.Event="TextChanged"
         utu:EventToCommandExtensions.Command="{Binding TextChangedCommand}"
         utu:EventToCommandExtensions.PassEventArgsToCommand="True"
         utu:EventToCommandExtensions.EventArgsConverter="{StaticResource TextChangedArgsConverter}" />

<!-- Execute command on any UIElement event -->
<Border Background="LightGray"
        utu:EventToCommandExtensions.Event="PointerEntered"
        utu:EventToCommandExtensions.Command="{Binding HoverCommand}">
    <TextBlock Text="Hover over me" />
</Border>
```

## Comparison with CommandExtensions

The `EventToCommandExtensions` provides more flexibility than `CommandExtensions` by allowing you to:

1. **Bind to any event** - Not just predefined events like `ItemClick` or `SelectionChanged`
2. **Access event arguments** - Pass the original event args to your command
3. **Use converters** - Transform event arguments before they reach your command

Use `EventToCommandExtensions` when you need to:
- Bind to events not covered by `CommandExtensions`
- Access the full event arguments in your command
- Apply custom logic to event arguments via converters

Use `CommandExtensions` when you need a simpler, more opinionated approach for common scenarios like:
- List item clicks
- Enter key handling in text boxes
- Navigation view item invocation
