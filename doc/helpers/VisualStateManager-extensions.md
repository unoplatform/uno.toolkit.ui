---
uid: Toolkit.Helpers.VisualStateManagerExtensions
---

# VisualStateManager Extensions

Provides a way of manipulating the visual states of `Control` with attached property.

## Remarks

`VisualStateManager.GoToState` is typically used with `Control` where you would set `<VisualStateManager.VisualStateGroups>` on the root element of the ControlTemplate. Because this class is implemented using the same method, it means that if you are setting `StatesProperty` on an element, the `VisualStateManager.VisualStateGroups` should not be set on the very same element, but its first child:

```xml
<Page utu:VisualStateManagerExtensions.States="{Binding OnboardingState, Mode=OneWay}">
    <!-- Wrong -->
    <VisualStateManager.VisualStateGroups>...

    <!-- Good -->
    <Grid>
        <VisualStateManager.VisualStateGroups>...
```

This "first child" is more common known as the template root within the context of a `ControlTemplate` where you typically have a top-level `Grid`/`Border`/Panel with the x:Name that contains "Root".

## Attached Properties

| Property | Type     | Description                              |
|----------|----------|------------------------------------------|
| `States` | `string` | Sets the visual states of the control.\* |

States\*: The accepted value can be a space, comma or semi-colon separated list of visual state names. eg:

- "LoggedIn": just a single state
- "LoggedIn, OnMeteredNetwork": two concurrent states from different visual state groups
- "Pressed, Selected, PressedSelected": with a combined state

## Usage

```xml
<Page ...
      xmlns:utu="using:Uno.Toolkit.UI"
      utu:VisualStateManagerExtensions.States="{Binding PageState, Mode=OneWay}">
    <Grid ColumnDefinitions="Auto,*" RowDefinitions="*,*,*">

        <VisualStateManager.VisualStateGroups>
            <VisualStateGroup x:Name="PageStates">
                <VisualStateGroup.Transitions>
                    <VisualTransition From="Red" To="Green">
                        <Storyboard>
                            <ColorAnimation Storyboard.TargetName="BackgroundBorder"
                                            Storyboard.TargetProperty="(Border.Background).(SolidColorBrush.Color)"
                                            From="Red" To="Green" Duration="00:00:00.333" />
                        </Storyboard>
                    </VisualTransition>
                    <!-- repeats for every permutation of [Red,Green,Blue] ... -->
                </VisualStateGroup.Transitions>

                <VisualState x:Name="Red">
                    <VisualState.Setters>
                        <Setter Target="BackgroundBorder.Background" Value="Red" />
                    </VisualState.Setters>
                </VisualState>
                <VisualState x:Name="Green">
                    <VisualState.Setters>
                        <Setter Target="BackgroundBorder.Background" Value="Green" />
                    </VisualState.Setters>
                </VisualState>
                <VisualState x:Name="Blue">
                    <VisualState.Setters>
                        <Setter Target="BackgroundBorder.Background" Value="Blue" />
                    </VisualState.Setters>
                </VisualState>
            </VisualStateGroup>
        </VisualStateManager.VisualStateGroups>

        <Button Grid.Row="0" Grid.Column="0"
                Content="Red"
                Command="{Binding ChangePageStateCommand}"
                CommandParameter="{Binding RelativeSource={RelativeSource Mode=Self}, Path=Content}"
                CornerRadius="0"
                HorizontalAlignment="Stretch" />
        <Button Grid.Row="0" Grid.Column="1"
                Content="Green"
                Command="{Binding ChangePageStateCommand}"
                CommandParameter="{Binding RelativeSource={RelativeSource Mode=Self}, Path=Content}"
                CornerRadius="0"
                HorizontalAlignment="Stretch" />
        <Button Grid.Row="0" Grid.Column="2"
                Content="Blue"
                Command="{Binding ChangePageStateCommand}"
                CommandParameter="{Binding RelativeSource={RelativeSource Mode=Self}, Path=Content}"
                CornerRadius="0"
                HorizontalAlignment="Stretch" />

        <Border x:Name="BackgroundBorder"
                Grid.Row="1" Grid.ColumnSpan="3" />
```

```csharp
public class ViewModel : ViewModelBase
{
    public string PageState { get => GetProperty<string>(); set => SetProperty(value); }

    public ICommand ChangePageStateCommand => new Command(ChangePageState);

    public ViewModel()
    {
        PageState = "Blue";
    }

    private void ChangePageState(object parameter)
    {
        if (parameter is string state)
        {
            PageState = state;
        }
    }
}
```
