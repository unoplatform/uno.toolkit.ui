# StatusBar (attached properties)
Provides two attached properties on `Page` to controls the visual of the status bar on mobile platforms.

## Remarks
The attached properties does nothing on platforms other than iOS and Android.
For iOS, `UIViewControllerBasedStatusBarAppearance` should set to false in `info.plist`.

## Properties
Property|Type|Description
-|-|-
ForegroundTheme|StatusBarTheme\*|Sets the foreground color for the text and icons on the status bar. Possible values are: `None, Light, Dark, Auto or AutoInverse`.
Background|Brush|Sets the background color for the status bar. <br/> note: Due to platform limitations, only `SolidColorBrush` are accepted.

StatusBarTheme\*: `Auto` and `AutoInverse` will set the foreground accordingly to the theme, and update itself when the os theme or the app theme changes.

## Usage
```xml
<Page ...
      xmlns:utu="using:Uno.Toolkit.UI"
      utu:StatusBar.ForegroundTheme="Dark"
      utu:StatusBar.Background="SkyBlue" />
```
