---
uid: Toolkit.Controls.ResponsiveView
---
# ResponsiveView

The `ResponsiveView` provides the ability to display different content based on screen size, making it easy to adapt to various devices.

## Remarks

The `ResponsiveView` control adapts to different screen sizes by dynamically choosing the right template. It looks at the current screen width and the defined templates. Since not all templates need a value, the control ensures a smooth user experience by picking the smallest defined template that satisfies the width requirements. If no match is found, it defaults to the largest defined template.

## Properties

| Property            | Type               | Description                                             |
| ------------------- | ------------------ | ------------------------------------------------------- |
| `NarrowestTemplate` | `DataTemplate`     | Template to be displayed on the narrowest screen size.  |
| `NarrowTemplate`    | `DataTemplate`     | Template to be displayed on a narrow screen size.       |
| `NormalTemplate`    | `DataTemplate`     | Template to be displayed on a normal screen size.       |
| `WideTemplate`      | `DataTemplate`     | Template to be displayed on a wide screen size.         |
| `WidestTemplate`    | `DataTemplate`     | Template to be displayed on the widest screen size.     |
| `ResponsiveLayout`  | `ResponsiveLayout` | Overrides the screen size threshold/breakpoints.        |

### ResponsiveLayout

Provides the ability to override the breakpoint for each screen size: `Narrowest`, `Narrow`, `Normal`, `Wide`, and `Widest`.

#### Properties

| Property     | Type     | Description            |
| ------------ | -------- | ---------------------- |
| `Narrowest`  | `double` | Default value is 150.  |
| `Narrow`     | `double` | Default value is 300.  |
| `Normal`     | `double` | Default value is 600.  |
| `Wide`       | `double` | Default value is 800.  |
| `Widest`     | `double` | Default value is 1080. |

#### Resolution Logics

The layouts whose value(ResponsiveExtension) or template(ResponsiveView) is not provided are first discarded. From the remaining layouts, we look for the first layout whose breakpoint at met by the current screen width. If none are found, the first layout is return regardless of its breakpoint.

Below are the selected layout at different screen width if all layouts are provided:

| Width          | Layout    |
|----------------|-----------|
| 149            | Narrowest |
| 150(Narrowest) | Narrowest |
| 151            | Narrowest |
| 299            | Narrowest |
| 300(Narrow)    | Narrow    |
| 301            | Narrow    |
| 599            | Narrow    |
| 600(Normal)    | Normal    |
| 601            | Normal    |
| 799            | Normal    |
| 800(Wide)      | Wide      |
| 801            | Wide      |
| 1079           | Wide      |
| 1080(Widest)   | Widest    |
| 1081           | Widest    |

Here are the selected layout at different screen width if only `Narrow` and `Wide` are provided:

| Width              | Layout |
|--------------------|--------|
| 149                | Narrow |
| 150(~~Narrowest~~) | Narrow |
| 151                | Narrow |
| 299                | Narrow |
| 300(Narrow)        | Narrow |
| 301                | Narrow |
| 599                | Narrow |
| 600(~~Normal~~)    | Narrow |
| 601                | Narrow |
| 799                | Narrow |
| 800(Wide)          | Wide   |
| 801                | Wide   |
| 1079               | Wide   |
| 1080(~~Widest~~)   | Wide   |
| 1081               | Wide   |

## Usage

If you prefer instructional videos, you can follow our tutorials on how to work with ResponsiveView using XAML or C# markup:

### [**XAML**](#tab/techbite-xaml) <!-- markdownlint-disable-line MD051 -->

> [!Video https://www.youtube-nocookie.com/embed/VgpC79ErNRI]

### [**C# Markup**](#tab/techbite-csmarkup) <!-- markdownlint-disable-line MD051 -->

> [!Video https://www.youtube-nocookie.com/embed/lQueBn6LEyU]

---

> [!TIP]
> It is not necessary to define every template or layout breakpoint: Narrowest, Narrow, Normal, Wide, Widest. You can just define the bare minimum needed.

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<utu:ResponsiveView>
    <!-- note:  -->
    <utu:ResponsiveView.NarrowTemplate>
        <DataTemplate>
            <!-- Narrow content -->
        </DataTemplate>
    </utu:ResponsiveView.NarrowTemplate>
    <utu:ResponsiveView.WideTemplate>
        <DataTemplate>
           <!-- Wide content -->
        </DataTemplate>
    </utu:ResponsiveView.WideTemplate>
</utu:ResponsiveView>
```

Using `ResponsiveLayout` to customize the screen size breakpoints.

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...
<utu:ResponsiveView>
    <utu:ResponsiveView.ResponsiveLayout>
        <utu:ResponsiveLayout>
            <utu:ResponsiveLayout.Narrow>350</utu:ResponsiveLayout.Narrow>
            <utu:ResponsiveLayout.Wide>1200</utu:ResponsiveLayout.Wide>
        </utu:ResponsiveLayout>
    </utu:ResponsiveView.ResponsiveLayout>

    <utu:ResponsiveView.NarrowTemplate>
        <DataTemplate>
            <!-- Narrow content -->
        </DataTemplate>
    </utu:ResponsiveView.NarrowTemplate>
    <utu:ResponsiveView.WideTemplate>
        <DataTemplate>
           <!-- Wide content -->
        </DataTemplate>
    </utu:ResponsiveView.WideTemplate>
</utu:ResponsiveView>
```

> [!NOTE]
> The `ResponsiveLayout` can also be provided from different locations. In order of precedences, they are:
>
> - from the `.ResponsiveLayout` property
> - in `ResponsiveView`'s parent `.Resources` with `x:Key="DefaultResponsiveLayout"`, or its ancestor's...
> - in `Application.Resources` with `x:Key="DefaultResponsiveLayout"`
> - from the hardcoded `ResponsiveHelper.DefaultLayout` which is defined as [150/300/600/800/1080]
