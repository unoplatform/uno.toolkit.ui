---
uid: Toolkit.Helpers.StyleExtensions
---

## Resource

This extension facilitates assigning a specific ResourceDictionary directly to a control's style. It simplifies [lightweight styling](../lightweight-styling.md) by allowing you to add a resource dictionary to the control's style. This eliminates the necessity to declare each resource on the page explicitly, enabling the easy creation of diverse visual elements with shared styles but varied attributes. The extension also supports the reuse of resource dictionaries across different control styles, enhancing consistency and efficiency in the UI design process.

Here is an example of how lightweight styling could be applied could be used on a button's style:

 ```xml
<Style x:Key="OverridenFilledButtonStyle"
	   TargetType="Button"
	   BasedOn="{StaticResource MaterialFilledButtonStyle}">
	<Setter Property="utu:StyleExtensions.Resources">
		<Setter.Value>
			<ResourceDictionary>
				<ResourceDictionary.ThemeDictionaries>
					<ResourceDictionary x:Key="Default">
						<SolidColorBrush x:Key="FilledButtonForeground"
										 Color="DarkGreen" />
						<SolidColorBrush x:Key="FilledButtonBackground"
										 Color="LightGreen" />

						<SolidColorBrush x:Key="FilledButtonForegroundPointerOver"
										 Color="DarkRed" />
						<SolidColorBrush x:Key="FilledButtonBackgroundPointerOver"
										 Color="LightPink" />

						<SolidColorBrush x:Key="FilledButtonForegroundPressed"
										 Color="DarkBlue" />
						<SolidColorBrush x:Key="FilledButtonBackgroundPressed"
										 Color="LightSteelBlue" />

						<SolidColorBrush x:Key="FilledButtonForegroundFocused"
										 Color="AntiqueWhite" />
						<SolidColorBrush x:Key="FilledButtonBackgroundFocused"
										 Color="DarkMagenta" />
					</ResourceDictionary>
					<ResourceDictionary x:Key="Light">
						<SolidColorBrush x:Key="FilledButtonForeground"
										 Color="LightGreen" />
						<SolidColorBrush x:Key="FilledButtonBackground"
										 Color="DarkGreen" />

						<SolidColorBrush x:Key="FilledButtonForegroundPointerOver"
										 Color="LightPink" />
						<SolidColorBrush x:Key="FilledButtonBackgroundPointerOver"
										 Color="DarkRed" />

						<SolidColorBrush x:Key="FilledButtonForegroundPressed"
										 Color="LightSteelBlue" />
						<SolidColorBrush x:Key="FilledButtonBackgroundPressed"
										 Color="DarkBlue" />

						<SolidColorBrush x:Key="FilledButtonForegroundFocused"
										 Color="DarkMagenta" />
						<SolidColorBrush x:Key="FilledButtonBackgroundFocused"
										 Color="AntiqueWhite" />
					</ResourceDictionary>
				</ResourceDictionary.ThemeDictionaries>
			</ResourceDictionary>
		</Setter.Value>
	</Setter>
</Style>
```
