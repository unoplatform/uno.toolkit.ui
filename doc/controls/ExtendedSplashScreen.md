---
uid: Toolkit.Controls.ExtendedSplashScreen
---
# ExtendedSplashScreen

Represents a control, derived from [`LoadingView`](xref:Toolkit.Controls.LoadingView) that displays a view which replicates the appearance and behavior of the target platform's native application loading screen.

Refer to [`LoadingView`](xref:Toolkit.Controls.LoadingView) for a list of inherited members.

## Remarks

The term _splash screen_ commonly refers to a view that is displayed when the user launches the application. Note that only a subset of the supported targets define a native splash screen construct that is displayed by the platform.

A common use case for this control is to display an application loading element for extended durations after transitioning from the native splash screen. This is useful for scenarios where the application is performing a longer-running operation, such as loading data or performing a network request upon startup.

`ExtendedSplashScreen` is a custom implementation, and does not require the presence of a native splash screen. As such, it can serve as an application's standalone loading screen, regardless of whether the platform provides a native equivalent.

> [!TIP]
> Users expect applications to load quickly after they launch them, so it is important to minimize the duration that a splash screen is displayed. `ExtendedSplashScreen` is intended to support a consistent user experience across platforms where scenarios necessitate a longer startup process.

## Properties

| Property | Type     | Description |
|----------|----------|-------------|
| `Platforms` | `SplashScreenPlatform` | Gets or sets a value that indicates which platforms where a splash screen view (`All` by default). This is a flag enumeration, which allows for combining multiple values. (eg `Android,iOS` will show native splash screen for only Android and iOS) |
| `SplashScreen` | `SplashScreen` | Gets or sets the `SplashScreen` entity that's available for UWP that defines where the splash screen image is positioned. |
| `Window` | `Window` | Gets or sets the `Window` for the application that is used to size the splash screen. |
| `SplashIsEnabled` | `bool` | Gets a value that determines whether native splash is to be displayed for the current platform at runtime. |

> [!NOTE]
> On versions greater than Android 11, the native splash screen is always disabled irrespective of the `Platforms` value.

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

## Setup on Android

To use the `ExtendedSplashScreen` on Android, you need to add the following to your `MainActivity`:

```csharp
 protected override void OnCreate(Bundle bundle)
{ 
    // Handle the splash screen transition.
    Uno.Toolkit.UI.ExtendedSplashScreen.Init(this);

    base.OnCreate(bundle);
}
```
