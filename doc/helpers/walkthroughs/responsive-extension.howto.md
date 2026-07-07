# How to create responsive UIs and layouts based on window width breakpoints

All examples assume you have **Uno.Toolkit.UI** referenced in your project (the package that ships `ResponsiveExtension`).
Add the correct namespace if needed:

```xml
xmlns:utu="using:Uno.Toolkit.UI"
```

---

## 1. Change a property when the screen gets wider

**Outcome:** When the window is small, a `Grid` has `Padding="12"`. When the window becomes wider, the padding grows gradually up to `48`.

```xml
<Page
    x:Class="MyApp.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:utu="using:Uno.Toolkit.UI">

    <Grid
        Background="{ThemeResource ApplicationPageBackgroundThemeBrush}"
        Padding="{utu:Responsive
            Narrowest=12
            Narrow=16
            Normal=24
            Wide=32
            Widest=48}">
        <!-- page content -->
    </Grid>
</Page>
```

**What’s happening**

* `utu:Responsive` returns a value depending on the current screen size.
* `Narrowest` is the smallest breakpoint, `Widest` the largest.
* Missing ones fall back to the nearest smaller defined breakpoint.

---

## 2. Hide something on phones, show it on tablets/desktops

**Outcome:** a right-side panel is **collapsed on Narrowest/Narrow** screens and **visible on Normal+** screens.

```xml
<StackPanel
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:utu="using:Uno.Toolkit.UI"
    Orientation="Horizontal">

    <!-- main content -->
    <Grid Width="*">
        <!-- ... -->
    </Grid>

    <!-- secondary panel -->
    <Border
        Width="280"
        Background="{ThemeResource CardBackgroundFillColorDefaultBrush}"
        Visibility="{utu:Responsive
            Narrowest=Collapsed
            Narrow=Collapsed
            Normal=Visible}" />
</StackPanel>
```

**Why this works**

* The panel’s `Visibility` switches to `Collapsed` for small screens.
* On `Normal` and above, it becomes `Visible`.

---

## 3. Shrink a control on narrow windows

**Outcome:** a `Button` is large on desktops but smaller on mobile.

```xml
<Button
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:utu="using:Uno.Toolkit.UI"
    Content="Continue"
    FontSize="{utu:Responsive
        Narrowest=14
        Narrow=15
        Normal=16
        Wide=18
        Widest=20}"
    Padding="{utu:Responsive
        Narrowest=8,4
        Normal=12,6
        Wide=16,8}" />
```

**Notes**

* Use different property names on the same element — each one gets its own `Responsive` markup.
* You can skip breakpoints; undefined ones inherit from the last narrower size.

---

## 4. Change layout spacing across breakpoints

**Outcome:** a list has tight spacing on small screens and relaxed spacing on larger ones.

```xml
<StackPanel
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:utu="using:Uno.Toolkit.UI"
    Spacing="{utu:Responsive
        Narrowest=4
        Narrow=6
        Normal=8
        Wide=10
        Widest=12}">

    <TextBlock Text="Title" Style="{StaticResource TitleTextBlockStyle}" />
    <TextBlock Text="Subtitle" />
    <Button Content="Action" />
</StackPanel>
```

**Use this when:** you only need to adjust spacing for readability.

---

## 5. Make a pane width responsive (master/detail, flyout-like UIs)

**Outcome:** side area width grows as screens widen.

```xml
<Grid
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:utu="using:Uno.Toolkit.UI">

    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*" />
        <ColumnDefinition
            Width="{utu:Responsive
                Narrowest=240
                Narrow=280
                Normal=320
                Wide=360
                Widest=420}" />
    </Grid.ColumnDefinitions>

    <!-- content -->
    <Frame Grid.Column="0" />

    <!-- details area -->
    <Border Grid.Column="1" Background="{ThemeResource CardBackgroundFillColorDefaultBrush}">
        <!-- details -->
    </Border>
</Grid>
```

**Benefit:** Adjust layout density without full state triggers.

---

## 6. Use responsive values inside a `DataTemplate`

**Outcome:** thumbnails in a list grow with screen width.

```xml
<DataTemplate x:Key="ProductTemplate"
              xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
              xmlns:utu="using:Uno.Toolkit.UI">
    <StackPanel Orientation="Horizontal" Spacing="8">
        <Image
            Source="{Binding Thumbnail}"
            Width="{utu:Responsive Narrowest=40 Narrow=48 Normal=56 Wide=64 Widest=72}"
            Height="{utu:Responsive Narrowest=40 Narrow=48 Normal=56 Wide=64 Widest=72}" />
        <TextBlock Text="{Binding Name}" VerticalAlignment="Center" />
    </StackPanel>
</DataTemplate>
```

**Tip:** works in any context that accepts a value (including `DataTemplate`).

---

## 7. Override only one breakpoint

**Outcome:** text size increases only on very large displays.

```xml
<TextBlock
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:utu="using:Uno.Toolkit.UI"
    Text="Orders"
    FontSize="{utu:Responsive
        Normal=18
        Widest=24}" />
```

**Why:** You can specify only the sizes you want to override — other values are inherited.

---

## 8. Combine with VisualStateManager (when you must)

**Outcome:** use `ResponsiveExtension` for *fine-tuning* inside broader layout state changes.

```xml
<Grid
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:utu="using:Uno.Toolkit.UI">

    <VisualStateManager.VisualStateGroups>
        <VisualStateGroup x:Name="LayoutStates">
            <VisualState x:Name="Normal" />
            <VisualState x:Name="DetailsVisible">
                <VisualState.Setters>
                    <Setter Target="DetailsPanel.Visibility" Value="Visible" />
                </VisualState.Setters>
            </VisualState>
        </VisualStateGroup>
    </VisualStateManager.VisualStateGroups>

    <Grid Margin="{utu:Responsive Narrowest=8 Normal=12 Wide=16}">
        <!-- ... -->
    </Grid>

    <Border x:Name="DetailsPanel" Visibility="Collapsed">
        <!-- ... -->
    </Border>
</Grid>
```

**Guideline:**

* **VSM** → structure changes.
* **ResponsiveExtension** → property value tuning.

---

## 9. What are the breakpoints?

Uno Toolkit uses **five** logical breakpoints:

| Name          | Typical range (inferred)           | Purpose                               |
| ------------- | ---------------------------------- | ------------------------------------- |
| **Narrowest** | phones and smallest windows        | most compact UI                       |
| **Narrow**    | small tablets or small split views | slightly roomier spacing              |
| **Normal**    | default, mid-size range            | baseline design size                  |
| **Wide**      | tablets and medium desktops        | expanded padding and spacing          |
| **Widest**    | large monitors, ultra-wide         | maximum layout comfort and visibility |
