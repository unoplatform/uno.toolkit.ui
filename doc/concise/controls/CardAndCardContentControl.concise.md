---
uid: Toolkit.Controls.Card
---

# Card & CardContentControl (Concise Reference)

## Summary

> This guide covers details for `Card` and `CardContentControl` specifically. If you are just getting started with the Uno Material Toolkit Library, please see our [general getting started](../getting-started.md) page to make sure you have the correct setup in place.

## Constructors

| Constructor | Description                                     |
|-------------|-------------------------------------------------|
| `Card()`    | Initializes a new instance of the `Card` class. |

## Properties

| Property                    | Type           | Description                                                                                   |
|-----------------------------|----------------|-----------------------------------------------------------------------------------------------|
| `HeaderContent`             | `object`       | Gets or sets the content for the control's header.                                            |
| `HeaderContentTemplate`     | `DataTemplate` | Gets or sets the data template used to display the content of the control's header.           |
| `SubHeaderContent`          | `object`       | Gets or sets the content for the control's subheader.                                         |
| `SubHeaderContentTemplate`  | `DataTemplate` | Gets or sets the data template used to display the content of the control's subheader.        |
| `AvatarContent`             | `object`       | Gets or sets the content for the control's avatar.                                            |
| `AvatarContentTemplate`     | `DataTemplate` | Gets or sets the data template used to display the content of the control's avatar.           |
| `MediaContent`              | `object`       | Gets or sets the content for the control's media area.                                        |
| `MediaContentTemplate`      | `DataTemplate` | Gets or sets the data template used to display the content of the control's media area.       |
| `SupportingContent`         | `object`       | Gets or sets the content for the control's supporting area.                                   |
| `SupportingContentTemplate` | `DataTemplate` | Gets or sets the data template used to display the content of the control's supporting area.  |
| `IconsContent`              | `object`       | Gets or sets the content for the control's icons.                                             |
| `IconsContentTemplate`      | `DataTemplate` | Gets or sets the data template used to display the content of the control's icons.            |
| `Elevation`                 | `double`       | Gets or sets the elevation of the control.                                                    |
| `ShadowColor`               | `Color`        | Gets or sets the color to use for the shadow of the control.                                  |
| `IsClickable`               | `bool`         | Gets or sets a value indicating whether the control will respond to pointer and focus events. |

## Usage Examples

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<!-- ElevatedCardStyle -->
<utu:Card HeaderContent="Elevated card"
          SubHeaderContent="With title and subtitle"
          Style="{StaticResource ElevatedCardStyle}" />

<!-- FilledCardStyle -->
<utu:Card HeaderContent="Filled card"
          SubHeaderContent="With title and subtitle"
          Style="{StaticResource FilledCardStyle}" />

<!-- OutlinedCardStyle -->
<utu:Card HeaderContent="Outlined card"
          SubHeaderContent="With title and subtitle"
          Style="{StaticResource OutlinedCardStyle}" />
```

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<!-- ElevatedCardContentControlStyle -->
<utu:CardContentControl Style="{StaticResource ElevatedCardContentControlStyle}">
    <utu:CardContentControl.ContentTemplate>
        <DataTemplate>
            <Grid>
                <TextBlock Text="Elevated card" MaxLines="1" Style="{StaticResource HeadlineMedium}" />
            </Grid>
        </DataTemplate>
    </utu:CardContentControl.ContentTemplate>
</utu:CardContentControl>

<!-- FilledCardContentControlStyle -->
<utu:CardContentControl Style="{StaticResource FilledCardContentControlStyle}">
    <utu:CardContentControl.ContentTemplate>
        <DataTemplate>
            <Grid>
                <TextBlock Text="Filled card" MaxLines="1" Style="{StaticResource HeadlineMedium}" />
            </Grid>
        </DataTemplate>
    </utu:CardContentControl.ContentTemplate>
</utu:CardContentControl>

<!-- OutlinedCardContentControlStyle -->
<utu:CardContentControl Style="{StaticResource OutlinedCardContentControlStyle}">
    <utu:CardContentControl.ContentTemplate>
        <DataTemplate>
            <Grid>
                <TextBlock Text="Outlined card" MaxLines="1" Style="{StaticResource HeadlineMedium}" />
            </Grid>
        </DataTemplate>
    </utu:CardContentControl.ContentTemplate>
</utu:CardContentControl>
```

---

**Note**: This is a concise reference. 
For complete documentation, see [CardAndCardContentControl.md](CardAndCardContentControl.md)