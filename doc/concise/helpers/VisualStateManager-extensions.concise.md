---
uid: Toolkit.Helpers.VisualStateManagerExtensions
---

# VisualStateManager Extensions (Concise Reference)

## Summary

Provides a way of manipulating the visual states of `Control` with attached property.

## Properties

| Property | Type     | Description                              |
|----------|----------|------------------------------------------|
| `States` | `string` | Sets the visual states of the control.\* |

## Usage Examples

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

---

**Note**: This is a concise reference. 
For complete documentation, see [VisualStateManager-extensions.md](VisualStateManager-extensions.md)