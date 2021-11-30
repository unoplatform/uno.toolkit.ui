# CardContentControl

## Summary

Represents a control used to visually group related child elements and information.

## Remarks

Currently, there are two [Material](https://material.io/components/cards)-themed `CardContentControl` styles that you can use to customize the border type. Depending on the amount of user attention you want to draw to the content, you can use `MaterialElevatedCardContentControlStyle` to add a subtle z-axis elevation. Conversely, `MaterialOutlinedCardContentControlStyle` displays a simple solid stroke along the border of the card. 

## Usage

### MaterialElevatedCardContentControlStyle

```xml
xmlns:toolkit="using:Uno.Toolkit.UI.Controls"
...
<toolkit:CardContentControl Style="{StaticResource MaterialElevatedCardContentControlStyle}">
	<toolkit:CardContentControl.ContentTemplate>
		<DataTemplate>
			<Grid>
			    <TextBlock Text="Elevated card" MaxLines="1" Style="{StaticResource MaterialHeadline6}" />
			</Grid>
		</DataTemplate>
	</toolkit:CardContentControl.ContentTemplate>
</toolkit:CardContentControl>
```

### MaterialOutlinedCardContentControlStyle

```xml
xmlns:toolkit="using:Uno.Toolkit.UI.Controls"
...
<toolkit:CardContentControl Style="{StaticResource MaterialOutlinedCardContentControlStyle}">
	<toolkit:CardContentControl.ContentTemplate>
		<DataTemplate>
			<Grid>
			    <TextBlock Text="Outlined card" MaxLines="1" Style="{StaticResource MaterialHeadline6}" />
			</Grid>
		</DataTemplate>
	</toolkit:CardContentControl.ContentTemplate>
</toolkit:CardContentControl>
```