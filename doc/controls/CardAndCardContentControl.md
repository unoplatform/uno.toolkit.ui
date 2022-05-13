# Card & CardContentControl

## Summary

`Card` and `CardContentControl` represent controls identifiable as a single, contained unit used to visually group related child content and actions that relate information about a subject.
A card's layout and dimensions depend on its contents.

## Remarks

Currently, there are three [Material](https://m3.material.io/components/cards/) styles for `Card` and `CardContentControl` that you can use. 
Depending on the amount of user attention you want to draw to the content you can use:
- `ElevatedCardStyle` or `ElevatedCardContentControlStyle` to add a subtle z-axis elevation.
- `FilledCardStyle` or `FilledCardContentControlStyle` to display a simple background color without any elevation or border for the card.
- `OutlinedCardStyle` or `OutlinedCardContentControlStyle` to display a simple solid stroke along the border of the card.

## Card
The Card control comes with all the built-in properties of a `Control`  and also a few additional properties listed below that let you tailor the content to be displayed:

| Property                  | Type              |
|---------------------------|-------------------|
| HeaderContent             | object            |
| HeaderContentTemplate     | DataTemplate      |
| SubHeaderContent          | object            |
| SubHeaderContentTemplate  | DataTemplate      |
| AvatarContent             | object            |
| AvatarContentTemplate     | DataTemplate      |
| MediaContent              | object            |
| MediaContentTemplate      | DataTemplate      |
| SupportingContent         | object            |
| SupportingContentTemplate | DataTemplate      |
| IconsContent              | object            |
| IconsContentTemplate      | DataTemplate      |
| Elevation                 | double            |
| ShadowColor               | Windows.UI.Color  |

Consider using [CardContentControl](#cardcontentcontrol) if you need full control over the content layout.

### Usage

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

## CardContentControl

The `CardContentControl` is based on `ContentControl` and allows you to customize the entire content through `DataTemplate` to fit your needs.

### Usage

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