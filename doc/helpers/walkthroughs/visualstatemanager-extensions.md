# Change visual states with a bindable string

**Goal:** Drive `VisualStateManager` with a simple `string` property.

## Dependencies
- `Uno.Toolkit.UI` (or `Uno.Toolkit.WinUI`)

## Define states on the template root
```xml
<Page
  xmlns:utu="using:Uno.Toolkit.UI"
  utu:VisualStateManagerExtensions.States="{Binding PageState, Mode=OneWay}">
  <Grid>
    <VisualStateManager.VisualStateGroups>
      <VisualStateGroup x:Name="PageStates">
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
      </VisualStateGroup>
    </VisualStateManager.VisualStateGroups>

    <Border x:Name="BackgroundBorder" />
  </Grid>
</Page>
```
> Do **not** put `VisualStateGroups` on the *same* element that has `States` attached; put them on its first child (template root).

## ViewModel
```csharp
public string PageState { get; set; } = "Green";
```
- Set multiple states by separating names with space/comma/semicolon.
