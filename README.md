# Uno Toolkit
A set of custom controls for the UWP, WinUI and the Uno Platform not offered out of the box by WinUI, such as Card, TabBar, NavigationBar, etc.

## Getting Started
The steps to bootstrap Uno.Toolkit.UI into your application is very similar to those of [Uno.Material/Uno.Cupertino](https://github.com/unoplatform/Uno.Themes#getting-started).

1. Install the `Uno.Toolkit.UI` NuGet Package
2. Initialize the Toolkit resources. The order in which the different resources are loaded is important. Add this to `App.xaml`
```diff
<Application.Resources>
	<ResourceDictionary>
		<ResourceDictionary.MergedDictionaries>
			<!-- Load WinUI resources -->
			<XamlControlsResources xmlns="using:Microsoft.UI.Xaml.Controls" />
				
+			<!-- Load Toolkit resources -->
+			<ToolkitResources xmlns="using:Uno.Toolkit.UI" />

			<!-- Load remaining application resources -->
		</ResourceDictionary.MergedDictionaries>
	</ResourceDictionary>
</Application.Resources>
```
3. (Optional) The Uno Toolkit also comes packaged with Material and Cupertino styles for many of its controls. If you wish to use the styles in your application, add the following to your `App.xaml`. Again, the order in which the resources are laoded is important:

> ⚠️
>  Please note that you will need to install and initialize the Uno.Material and/or the Uno.Cupertino packages before you can use the Material/Cupertino Toolkit styles.

```diff
<Application.Resources>
	<ResourceDictionary>
		<ResourceDictionary.MergedDictionaries>
			<!--  Load WinUI resources  -->
			<XamlControlsResources xmlns="using:Microsoft.UI.Xaml.Controls" />

			<!--  Load Uno.UI.Toolkit resources  -->
			<ToolkitResources xmlns="using:Uno.Toolkit.UI" />

+			<!--  Load Material resources  -->
+			<MaterialColors xmlns="using:Uno.Material" />
+			<MaterialResources xmlns="using:Uno.Material" />

+			<!--  Load Cupertino resources  -->
+			<CupertinoColors xmlns="using:Uno.Cupertino" />
+			<CupertinoResources xmlns="using:Uno.Cupertino" />

+			<!--  Load Material Toolkit resources  -->
+			<MaterialToolkitColors xmlns="using:Uno.Toolkit.UI.Material" />
+			<MaterialToolkitResources xmlns="using:Uno.Toolkit.UI.Material" />

+			<!--  Load Cupertino Toolkit resources  -->
+			<CupertinoToolkitResources xmlns="using:Uno.Toolkit.UI.Cupertino" />

			<!--  Load remaining application resources  -->
		</ResourceDictionary.MergedDictionaries>
	</ResourceDictionary>
</Application.Resources>
```

4. Start using our controls on your pages!
Here is an example of usage for our TabBar control:

```xml
xmlns:utu="using:Uno.Toolkit.UI"

[...]

<utu:TabBar>
    <utu:TabBar.Items>
        
        <utu:TabBarItem Content="HOME">
            <utu:TabBarItem.Icon>
                <FontIcon Glyph="&#xE80F;" />
            </utu:TabBarItem.Icon>
        </utu:TabBarItem>
        <utu:TabBarItem Content="SUPPORT">
            <utu:TabBarItem.Icon>
                <FontIcon Glyph="&#xE8F2;" />
            </utu:TabBarItem.Icon>
        </utu:TabBarItem>
        <utu:TabBarItem Content="ABOUT">
            <utu:TabBarItem.Icon>
                <FontIcon Glyph="&#xE946;" />
            </utu:TabBarItem.Icon>
        </utu:TabBarItem>

    </utu:TabBar.Items>
</utu:TabBar>
```

### Further Documentation
For further documentation regarding any of the controls within Toolkit, you can find them in the `docs/controls` folder or listed here:
- [CardContentControl](docs/controls/CardContentControl.md)
- [Chip](docs/controls/Chip.md)
- [SegmentedControl](docs/controls/SegmentedControls.md)

### Material Styles for Toolkit controls
| **Controls**              | **StyleNames**                                                                |
|---------------------------|-------------------------------------------------------------------------------|
| BottomTabBar              | MaterialBottomTabBarStyle <br> MaterialBottomFabTabBarItemStyle <br> MaterialBottomTabBarItemStyle  |
| Card                      | MaterialOutlinedCardStyle <br> MaterialElevatedCardStyle <br> MaterialAvatarOutlinedCardStyle <br> MaterialAvatarElevatedCardStyle <br> MaterialSmallMediaOutlinedCardStyle <br> MaterialSmallMediaElevatedCardStyle |
| CardContentControl        | MaterialOutlinedCardContentControlStyle <br> MaterialElevatedCardContentControlStyle |
| Chip                      | MaterialFilledInputChipStyle<br>MaterialFilledChoiceChipStyle<br>MaterialFilledFilterChipStyle<br>MaterialFilledActionChipStyle<br>MaterialOutlinedInputChipStyle<br>MaterialOutlinedChoiceChipStyle<br>MaterialOutlinedFilterChipStyle<br>MaterialOutlinedActionChipStyle |
| ChipGroup                 | MaterialFilledInputChipGroupStyle<br>MaterialFilledChoiceChipGroupStyle<br>MaterialFilledFilterChipGroupStyle<br>MaterialFilledActionChipGroupStyle<br>MaterialOutlinedInputChipGroupStyle<br>MaterialOutlinedChoiceChipGroupStyle<br>MaterialOutlinedFilterChipGroupStyle<br>MaterialOutlinedActionChipGroupStyle |
| Divider                   | MaterialDividerStyle |
| NavigationBar             | MaterialNavigationBarStyle <br> MaterialModalNavigationBarStyle <br> MaterialMainCommandStyle <br> MaterialModalMainCommandStyle |
| TopTabBar                 | MaterialTopTabBarStyle <br> MaterialTopTabBarItemStyle |



### Cupertino Styles for Toolkit controls
| **Controls**              | **StyleNames**                                                                |
|---------------------------|-------------------------------------------------------------------------------|
| BottomTabBar              | CupertinoBottomTabBarStyle <br> CupertinoBottomTabBarItemStyle |
| SegmentedControl          | CupertinoSegmentedStyle <br> CupertinoSegmentedItemStyle |
| SlidingSegmentedControl   | CupertinoSlidingSegmentedStyle <br> CupertinoSlidingSegmentedItemStyle |
