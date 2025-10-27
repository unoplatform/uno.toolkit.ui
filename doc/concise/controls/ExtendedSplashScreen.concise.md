---
uid: Toolkit.Controls.ExtendedSplashScreen
---

# ExtendedSplashScreen (Concise Reference)

## Summary

Represents a control, derived from [`LoadingView`](xref:Toolkit.Controls.LoadingView) that displays a view that replicates the appearance and behavior of the target platform's native application loading screen.
Refer to [`LoadingView`](xref:Toolkit.Controls.LoadingView) for a list of inherited members.

## Properties

| Property | Type | Description | Value |
|----------|------|-------------|---------------|
| **Platforms** | `SplashScreenPlatform` | Gets or sets the platform(s) where extended splash screen should be used. This is a flag enumeration, which allows for combining multiple values eg: `"Android,iOS"` | Default value is **All**. Other possible values include **Android**, **iOS**, **Windows**, **WebAssembly**, **Skia**, and **None**. |
| **SplashIsEnabled** | `bool` | Gets a value representing whether the current environment is to display this splash screen. | **True** if the current platform is included in the **Platforms** property, otherwise **false**. |

## Usage Examples

```xml
<utu:ExtendedSplashScreen x:Name="Splash"
                          Platforms="Android,iOS" />
```

```xml
<UserControl x:Class="SplashScreenApp.Shell"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:SplashScreenApp"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:utu="using:Uno.Toolkit.UI"
             mc:Ignorable="d">

    <utu:ExtendedSplashScreen x:Name="Splash"
                              HorizontalAlignment="Stretch"
                              VerticalAlignment="Stretch"
                              HorizontalContentAlignment="Stretch"
                              VerticalContentAlignment="Stretch">
        <utu:ExtendedSplashScreen.LoadingContent>
            <Grid>
                <Grid.RowDefinitions>
                    <RowDefinition Height="2*" />
                    <RowDefinition />
                </Grid.RowDefinitions>

                <ProgressRing IsActive="True"
                              Grid.Row="1"
                              VerticalAlignment="Center"
                              HorizontalAlignment="Center"
                              Height="100"
                              Width="100" />
            </Grid>
        </utu:ExtendedSplashScreen.LoadingContent>
        <utu:ExtendedSplashScreen.Content>
            <Frame x:Name="ShellFrame" />
        </utu:ExtendedSplashScreen.Content>
    </utu:ExtendedSplashScreen>
</UserControl>
```

---

**Note**: This is a concise reference. 
For complete documentation, see [ExtendedSplashScreen.md](ExtendedSplashScreen.md)