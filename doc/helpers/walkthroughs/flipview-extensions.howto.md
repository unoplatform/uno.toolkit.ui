# FlipView: move items from XAML

This group of how-tos shows how to control a `FlipView` from **other buttons** using the attached properties from **Uno.Toolkit.UI**.

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

---

## How to go to the next item

**Goal:** tap a button → `FlipView` advances by 1.

```xml
<Page
    ...
    xmlns:utu="using:Uno.Toolkit.UI">

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>

        <!-- The target FlipView -->
        <FlipView x:Name="MyFlipView">
            <FlipView.Items>
                <Grid Background="Tomato">
                    <TextBlock Text="Item 1"
                               HorizontalAlignment="Center"
                               VerticalAlignment="Center" />
                </Grid>
                <Grid Background="CornflowerBlue">
                    <TextBlock Text="Item 2"
                               HorizontalAlignment="Center"
                               VerticalAlignment="Center" />
                </Grid>
                <Grid Background="Goldenrod">
                    <TextBlock Text="Item 3"
                               HorizontalAlignment="Center"
                               VerticalAlignment="Center" />
                </Grid>
            </FlipView.Items>
        </FlipView>

        <!-- The command button -->
        <Button Grid.Row="1"
                Content="Next"
                utu:FlipViewExtensions.Next="{Binding ElementName=MyFlipView}" />
    </Grid>
</Page>
```

**Key points**

* Attach **`utu:FlipViewExtensions.Next`** to **any `ButtonBase`** (ex: `Button`, `AppBarButton`).
* The value is a binding to the **target `FlipView`**.
* No code-behind required.
* If the `FlipView` is already on the last item, it will stay there (same as native behavior). ([Uno Platform][1])

---

## How to go to the previous item

**Goal:** tap a button → `FlipView` moves back by 1.

```xml
<Page
    ...
    xmlns:utu="using:Uno.Toolkit.UI">

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>

        <FlipView x:Name="MyFlipView">
            <!-- items -->
        </FlipView>

        <Button Grid.Row="1"
                Content="Previous"
                utu:FlipViewExtensions.Previous="{Binding ElementName=MyFlipView}" />
    </Grid>
</Page>
```

**Key points**

* Use **`utu:FlipViewExtensions.Previous`** for backward navigation.
* Works with any control derived from `ButtonBase`. ([Uno Platform][1])

---

## How to add both Previous/Next buttons

**Goal:** add 2 buttons under the `FlipView`: **Previous** on the left, **Next** on the right.

```xml
<Page
    ...
    xmlns:utu="using:Uno.Toolkit.UI">

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>

        <FlipView x:Name="MyFlipView">
            <!-- items -->
        </FlipView>

        <StackPanel Grid.Row="1"
                    Orientation="Horizontal"
                    HorizontalAlignment="Center"
                    Spacing="16">
            <Button Content="Previous"
                    utu:FlipViewExtensions.Previous="{Binding ElementName=MyFlipView}" />

            <Button Content="Next"
                    utu:FlipViewExtensions.Next="{Binding ElementName=MyFlipView}" />
        </StackPanel>
    </Grid>
</Page>
```

**Why this matters for designers / RAG**

* You can put navigation **anywhere** (header, footer, overlay) and still drive the same `FlipView`.
* No need to expose the `SelectedIndex` in a view model.
* Great for “carousel with custom buttons” scenarios. ([Uno Platform][1])

---

## How to hide desktop arrow buttons

**Goal:** you want to keep the `FlipView`, but **remove the built-in arrows** shown on desktop targets.

```xml
<Page
    ...
    xmlns:utu="using:Uno.Toolkit.UI">

    <Grid>
        <FlipView Style="{StaticResource NoArrowsFlipViewStyle}">
            <!-- items -->
        </FlipView>
    </Grid>
</Page>
```

**Notes**

* `NoArrowsFlipViewStyle` is provided by **Uno Toolkit**.
* Use this when you supply **your own navigation buttons** (like in the sections above) to avoid double navigation UI. ([Uno Platform][1])

---

## How to trigger a FlipView from an AppBarButton

**Goal:** use a command-like button in a `CommandBar` / `NavigationBar` to move the `FlipView`.

```xml
<CommandBar>
    <AppBarButton Icon="Back"
                  Label="Prev"
                  utu:FlipViewExtensions.Previous="{Binding ElementName=MyFlipView}" />
    <AppBarButton Icon="Forward"
                  Label="Next"
                  utu:FlipViewExtensions.Next="{Binding ElementName=MyFlipView}" />
</CommandBar>

<FlipView x:Name="MyFlipView">
    <!-- items -->
</FlipView>
```

**Why separate how-to?**

* Same attached properties.
* Different location in the visual tree (command area vs content).
* Helps RAG give the right sample for “toolbar” queries.

---

## FAQ

**Q: Do I need code-behind to move the `FlipView`?**
A: No. The whole point of `FlipViewExtensions.Next/Previous` is to let XAML buttons move the `FlipView` without code. ([Uno Platform][1])

**Q: Can I bind to another page’s FlipView?**
A: Bindings must resolve in the current visual tree. Usually you bind by `ElementName`. If the `FlipView` is not in scope, expose it (named element) or move the button nearer.

**Q: Can I style the buttons differently per platform?**
A: Yes. The extension only handles **what** to navigate. You still style the **button itself** with normal XAML styles.

**Q: What happens at the last item or first item?**
A: It behaves like a normal `FlipView`—it won’t loop.

---

All of them depend on **`xmlns:utu="using:Uno.Toolkit.UI"`** and the `Toolkit` entry in `<UnoFeatures>`.

[1]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/helpers/FlipView-extensions.html?utm_source=chatgpt.com "FlipView Extensions"
