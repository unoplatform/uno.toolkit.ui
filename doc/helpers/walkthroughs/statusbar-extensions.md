# Control status bar colors on iOS/Android

**Goal:** Set status bar foreground/background from XAML.

## Dependencies
- `Uno.Toolkit.UI` (or `Uno.Toolkit.WinUI`)

## Set foreground and background
```xml
<Page xmlns:utu="using:Uno.Toolkit.UI"
      utu:StatusBar.Foreground="Dark"
      utu:StatusBar.Background="SkyBlue" />
```
- Foreground: `None | Light | Dark | Auto | AutoInverse`
- Background: `SolidColorBrush` only

## Notes
- iOS: set `UIViewControllerBasedStatusBarAppearance=false` in `Info.plist` for changes to apply.
- No effect on platforms other than iOS/Android.
