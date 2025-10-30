---
uid: Toolkit.Controls.ResponsiveView.HowTo
tags: [responsive, breakpoints, adaptive-layout, responsive-design, window-width, template-selector]
---

# Create responsive layouts based on Window width breakpoints

> Dependencies: add **`Uno.Toolkit.UI`**

---

## Show different content by width

```xml
<Page
    xmlns:utu="using:Uno.Toolkit.UI">
    <utu:ResponsiveView>
        <utu:ResponsiveView.NarrowTemplate>
            <DataTemplate>
                <!-- Stack for phones -->
                <StackPanel Spacing="8">
                    <TextBlock Text="Narrow" />
                    <!-- phone-friendly content -->
                </StackPanel>
            </DataTemplate>
        </utu:ResponsiveView.NarrowTemplate>

        <utu:ResponsiveView.WideTemplate>
            <DataTemplate>
                <!-- Grid for tablets/desktop -->
                <Grid ColumnDefinitions="*,*">
                    <TextBlock Text="Wide" Grid.ColumnSpan="2" />
                    <!-- wide layout -->
                </Grid>
            </DataTemplate>
        </utu:ResponsiveView.WideTemplate>
    </utu:ResponsiveView>
</Page>
```

`ResponsiveView` picks the first defined template whose breakpoint is met; if none match, it falls back to the largest defined template. ([Uno Platform][2])

---

## Change the breakpoints (per-control)

```xml
<Page
    xmlns:utu="using:Uno.Toolkit.UI">
    <utu:ResponsiveView>
        <utu:ResponsiveView.ResponsiveLayout>
            <utu:ResponsiveLayout
                Narrow="360"
                Normal="640"
                Wide="1024"
                Widest="1440" />
        </utu:ResponsiveView.ResponsiveLayout>

        <utu:ResponsiveView.NarrowTemplate>
            <DataTemplate> <!-- ... --> </DataTemplate>
        </utu:ResponsiveView.NarrowTemplate>

        <utu:ResponsiveView.WideTemplate>
            <DataTemplate> <!-- ... --> </DataTemplate>
        </utu:ResponsiveView.WideTemplate>
    </utu:ResponsiveView>
</Page>
```

`ResponsiveLayout` lets you override breakpoints for `Narrowest`, `Narrow`, `Normal`, `Wide`, `Widest` (defaults: 150, 300, 600, 800, 1080). ([Uno Platform][2])

---

## Set app-wide default breakpoints

```xml
<Application
    xmlns:utu="using:Uno.Toolkit.UI">
    <Application.Resources>
        <utu:ResponsiveLayout x:Key="DefaultResponsiveLayout"
                              Narrow="360"
                              Normal="640"
                              Wide="1024"
                              Widest="1440" />
    </Application.Resources>
</Application>
```

Precedence for where the layout is taken from:

1. `ResponsiveView.ResponsiveLayout`
2. `DefaultResponsiveLayout` in parent/ancestor resources
3. `DefaultResponsiveLayout` in `Application.Resources`
4. Built-in default `[150/300/600/800/1080]`. ([Uno Platform][2])

---

## Use only two templates (phone vs. desktop)

```xml
<utu:ResponsiveView>
    <utu:ResponsiveView.NarrowTemplate>
        <DataTemplate> <!-- phone layout --> </DataTemplate>
    </utu:ResponsiveView.NarrowTemplate>

    <utu:ResponsiveView.WideTemplate>
        <DataTemplate> <!-- desktop/tablet layout --> </DataTemplate>
    </utu:ResponsiveView.WideTemplate>
</utu:ResponsiveView>
```

You don't need to define all five templates. `ResponsiveView` chooses from what you provide and still resolves correctly. ([Uno Platform][2])

---

## FAQs (quick)

* **What are the default breakpoints?** 150, 300, 600, 800, 1080. ([Uno Platform][2])
* **How is the template picked?** Discard undefined layouts, pick the first whose breakpoint is met by current width; if none, return the first of remaining (effectively the largest defined). ([Uno Platform][2])
* **Do I need all five templates?** No—define the minimum you need. ([Uno Platform][2])
* **Where should I set app-wide breakpoints?** Add `ResponsiveLayout` as `DefaultResponsiveLayout` in resources (page or app), or set it per control. ([Uno Platform][2])
* **What packages do I need?** `Uno.Toolkit.UI` + a design system package (Material or Cupertino). ([NuGet][1])

---

## Copy-paste snippets

### Minimal XAML

```xml
<utu:ResponsiveView xmlns:utu="using:Uno.Toolkit.UI">
    <utu:ResponsiveView.NarrowTemplate>
        <DataTemplate><TextBlock Text="Narrow"/></DataTemplate>
    </utu:ResponsiveView.NarrowTemplate>
    <utu:ResponsiveView.WideTemplate>
        <DataTemplate><TextBlock Text="Wide"/></DataTemplate>
    </utu:ResponsiveView.WideTemplate>
</utu:ResponsiveView>
```

### Customize breakpoints

```xml
<utu:ResponsiveView xmlns:utu="using:Uno.Toolkit.UI">
    <utu:ResponsiveView.ResponsiveLayout>
        <utu:ResponsiveLayout Narrow="360" Wide="1024" />
    </utu:ResponsiveView.ResponsiveLayout>
    <!-- templates -->
</utu:ResponsiveView>
```

---

## Notes for RAG indexers

* Primary concepts: `ResponsiveView`, `ResponsiveLayout`, breakpoints, template selection precedence. ([Uno Platform][2])
* Related: **ResponsiveExtension** for property-level responsiveness (pairs well with `ResponsiveView` when you don't need template swapping). ([Uno Platform][3])

---

*Source page summarized & refactored for outcome-oriented how-tos.* ([Uno Platform][2])

[1]: https://www.nuget.org/packages/Uno.Toolkit.UI
[2]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/controls/ResponsiveView.html
[3]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/helpers/ResponsiveExtension.html
