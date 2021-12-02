# CardContentControl

## Summary

Represents a control used to visually group related child elements and information.

## Remarks

Currently, there are two [Material](https://material.io/components/cards)-themed `CardContentControl` styles that you can use to customize the border type. Depending on the amount of user attention you want to draw to the content, you can use `MaterialElevatedCardContentControlStyle` to add a subtle z-axis elevation. Conversely, `MaterialOutlinedCardContentControlStyle` displays a simple solid stroke along the border of the card. 

## Usage

### MaterialElevatedCardContentControlStyle

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...
<utu:CardContentControl Style="{StaticResource MaterialElevatedCardContentControlStyle}">
	<utu:CardContentControl.ContentTemplate>
		<DataTemplate>
			<Grid>
			    <TextBlock Text="Elevated card" MaxLines="1" Style="{StaticResource MaterialHeadline6}" />
			</Grid>
		</DataTemplate>
	</utu:CardContentControl.ContentTemplate>
</utu:CardContentControl>
```

### MaterialOutlinedCardContentControlStyle

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...
<utu:CardContentControl Style="{StaticResource MaterialOutlinedCardContentControlStyle}">
	<utu:CardContentControl.ContentTemplate>
		<DataTemplate>
			<Grid>
			    <TextBlock Text="Outlined card" MaxLines="1" Style="{StaticResource MaterialHeadline6}" />
			</Grid>
		</DataTemplate>
	</utu:CardContentControl.ContentTemplate>
</utu:CardContentControl>
```