---
uid: Toolkit.Controls.SegmentedControls
---

# Segmented Controls

> [!TIP]
> This guide covers details for `Segmented Controls` specifically. If you are just getting started with the Uno Material Toolkit Library, please see our [general getting started](../getting-started.md) page to make sure you have the correct setup in place.

## Summary

Represents a set of styles that can be used to customize the appearance of `TabBar` to be a Cupertino-themed segmented control.

## Remarks

There are two segmented control styles that you can use to theme `TabBar` and its set of mutually exclusive items. Depending on your design needs, you can use `CupertinoSlidingSegmentedStyle` to display a sliding thumb over the selected item. Alternatively, you can choose `CupertinoSegmentedStyle` to match the design found before iOS 13.

## Usage

### Sliding Segmented Control

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...
<utu:TabBar Style="{StaticResource CupertinoSlidingSegmentedStyle}">
    <utu:TabBar.Items>
        <utu:TabBarItem Content="Item One" />
        <utu:TabBarItem Content="Item Two" />
        <utu:TabBarItem Content="Item Three" />
    </utu:TabBar.Items>
</utu:TabBar>
```

### Segmented Control

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...
<utu:TabBar Style="{StaticResource CupertinoSegmentedStyle}">
    <utu:TabBar.Items>
        <utu:TabBarItem Content="Item One" />
        <utu:TabBarItem Content="Item Two" />
        <utu:TabBarItem Content="Item Three" />
    </utu:TabBar.Items>
</utu:TabBar>
```
