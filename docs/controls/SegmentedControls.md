# Segmented Controls

## Summary

Represents a set of styles that can be used to customize the appearance of `TabBar` to be a Cupertino-themed segmented control.

## Remarks

There are two segmented control styles that you can use to theme `TabBar` and its set of mutually exclusive items. Depending on your design needs, you can use `CupertinoSlidingSegmentedStyle` to display a sliding thumb over the selected item. Alternatively, you can choose `CupertinoSegmentedStyle` to match the design found before iOS 13.

## Usage

### Sliding Segmented Control

```xml
xmlns:toolkitLib="using:Uno.UI.ToolkitLib"
...
<toolkitLib:TabBar Style="{StaticResource CupertinoSlidingSegmentedStyle}">
	<toolkitLib:TabBar.Items>
		<toolkitLib:TabBarItem Content="Item One" />
		<toolkitLib:TabBarItem Content="Item Two" />
		<toolkitLib:TabBarItem Content="Item Three" />
	</toolkitLib:TabBar.Items>
</toolkitLib:TabBar>
```

### Segmented Control

```xml
xmlns:toolkitLib="using:Uno.UI.ToolkitLib"
...
<toolkitLib:TabBar Style="{StaticResource CupertinoSegmentedStyle}">
	<toolkitLib:TabBar.Items>
		<toolkitLib:TabBarItem Content="Item One" />
		<toolkitLib:TabBarItem Content="Item Two" />
		<toolkitLib:TabBarItem Content="Item Three" />
	</toolkitLib:TabBar.Items>
</toolkitLib:TabBar>
```