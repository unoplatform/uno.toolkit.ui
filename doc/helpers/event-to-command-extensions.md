---
uid: Toolkit.Helpers.CommandExtensions.EventCommands
---

# Event to Command (EventCommands)

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

Provides the ability to bind multiple events to commands on a single element using a collection-based approach. This is part of `CommandExtensions` and allows you to invoke commands when any events are raised, similar to .NET MAUI's [EventToCommandBehavior](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/behaviors/event-to-command-behavior).

## Attached Properties

### CommandExtensions.EventCommands

| Property                  | Type                       | Description                                                                      |
|---------------------------|----------------------------|----------------------------------------------------------------------------------|
| `EventCommands`           | `EventToCommandCollection` | A collection of `EventToCommand` objects that map events to commands.            |

### EventToCommand Properties

| Property                  | Type            | Description                                                                                      |
|---------------------------|-----------------|--------------------------------------------------------------------------------------------------|
| `Event`                   | `string`        | The name of the event to subscribe to (e.g., "Click", "Tapped", "SelectionChanged").             |
| `Command`                 | `ICommand`      | The command to execute when the event is raised.                                                 |
| `CommandParameter`        | `object`        | An optional parameter to pass to the command. If not set, see `PassEventArgsToCommand`.         |
| `PassEventArgsToCommand`  | `bool`          | When `true`, the event arguments are passed to the command as the parameter.                     |
| `EventArgsConverter`      | `IValueConverter` | An optional converter to transform event arguments before passing to the command.              |

## Remarks

- The `EventCommands` property accepts a collection of `EventToCommand` objects, allowing multiple events to be mapped to multiple commands on a single element.
- Each `EventToCommand` specifies which event to listen to and the command to execute.
- If `CommandParameter` is set, it takes precedence as the command parameter.
- If `CommandParameter` is not set and `PassEventArgsToCommand` is `true`, the event arguments are passed to the command.
- If `EventArgsConverter` is also set, it is applied to the event arguments before passing them to the command.
- The `Command.CanExecute` is checked before executing the command.

## Usage

```xml
<!-- Include the following XAML namespace to use the samples below -->
xmlns:utu="using:Uno.Toolkit.UI"
...

<!-- Single event to command mapping -->
<Button Content="Click Me">
    <utu:CommandExtensions.EventCommands>
        <utu:EventToCommandCollection>
            <utu:EventToCommand Event="Click" Command="{Binding MyClickCommand}" />
        </utu:EventToCommandCollection>
    </utu:CommandExtensions.EventCommands>
</Button>

<!-- Multiple events to multiple commands on the same element -->
<Button Content="Interactive Button">
    <utu:CommandExtensions.EventCommands>
        <utu:EventToCommandCollection>
            <utu:EventToCommand Event="Click" Command="{Binding ClickCommand}" />
            <utu:EventToCommand Event="PointerEntered" Command="{Binding HoverCommand}" />
            <utu:EventToCommand Event="PointerExited" Command="{Binding LeaveCommand}" />
        </utu:EventToCommandCollection>
    </utu:CommandExtensions.EventCommands>
</Button>

<!-- With command parameter -->
<Button Content="Submit">
    <utu:CommandExtensions.EventCommands>
        <utu:EventToCommandCollection>
            <utu:EventToCommand Event="Click" 
                                Command="{Binding SubmitCommand}" 
                                CommandParameter="FormSubmit" />
        </utu:EventToCommandCollection>
    </utu:CommandExtensions.EventCommands>
</Button>

<!-- Passing event args to command with optional converter -->
<ComboBox ItemsSource="{Binding Items}">
    <utu:CommandExtensions.EventCommands>
        <utu:EventToCommandCollection>
            <utu:EventToCommand Event="SelectionChanged" 
                                Command="{Binding SelectionChangedCommand}"
                                PassEventArgsToCommand="True"
                                EventArgsConverter="{StaticResource SelectionArgsConverter}" />
        </utu:EventToCommandCollection>
    </utu:CommandExtensions.EventCommands>
</ComboBox>
```

## Comparison with CommandExtensions.Command

The `EventCommands` property provides more flexibility than the single `Command` attached property by allowing you to:

1. **Bind to any event** - Not just predefined events like `ItemClick` or `SelectionChanged`
2. **Map multiple events** - Handle multiple events on the same element with different commands
3. **Access event arguments** - Pass the original event args to your command
4. **Use converters** - Transform event arguments before they reach your command

Use `CommandExtensions.EventCommands` when you need to:
- Bind to events not covered by `CommandExtensions.Command`
- Handle multiple different events on the same element
- Access the full event arguments in your command
- Apply custom logic to event arguments via converters

Use `CommandExtensions.Command` when you need a simpler, more opinionated approach for common scenarios like:
- List item clicks
- Enter key handling in text boxes
- Navigation view item invocation
