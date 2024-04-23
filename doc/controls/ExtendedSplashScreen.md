---
uid: Toolkit.Controls.ExtendedSplashScreen
---
# ExtendedSplashScreen

Represents a control, derived from [`LoadingView`](xref:Toolkit.Controls.LoadingView) that continues to display the native splash screen for an extended amount of time, allowing the application to fully load the UI. See [`LoadingView`](xref:Toolkit.Controls.LoadingView) for Properties and other Members that are inherited.

<div style="position: relative; width: 100%; padding-bottom: 56.25%;">
    <iframe
        src="https://www.youtube-nocookie.com/embed/jMI4E2e9gYE"
        title="YouTube video player"
        frameborder="0"
        allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
        allowfullscreen
        style="position: absolute; top: 0; left: 0; width: 100%; height: 100%;">
    </iframe>
</div>

## Properties

| Property          | Type                   | Description                                                                                                                                                                                                                                          |
|-------------------|------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Platforms`       | `SplashScreenPlatform` | Gets or sets which platforms load the native splash screen (`All` by default). This is an flag enumeration, meaning you can specify multiple values that will be combined (eg `Android,iOS` will show native splash screen for only Android and iOS) |
| `SplashScreen`    | `SplashScreen`         | Gets or sets the `SplashScreen` entity that's available for UWP that defines where the splash screen image is positioned.                                                                                                                            |
| `Window`          | `Window`               | Gets or sets the `Window` for the application that is used to size the splash screen.                                                                                                                                                                |
| `SplashIsEnabled` | `bool`                 | Gets a value that determines whether native splash is to be displayed for the current platform at runtime. Note for Android 12+ the native splash screen is always disabled irrespective of the Platforms value.                                     |

## Usage

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<!-- example -->
<utu:ExtendedSplashScreen x:Name="Splash"
                            HorizontalAlignment="Stretch"
                            VerticalAlignment="Stretch"
                            HorizontalContentAlignment="Stretch"
                            VerticalContentAlignment="Stretch"
                            Platforms="Android,iOS,Windows">
    <utu:ExtendedSplashScreen.LoadingContentTemplate>
        <DataTemplate>
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
        </DataTemplate>
    </utu:ExtendedSplashScreen.LoadingContentTemplate>
</utu:ExtendedSplashScreen>
```
