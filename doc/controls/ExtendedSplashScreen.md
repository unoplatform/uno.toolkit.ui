---
uid: Toolkit.Controls.ExtendedSplashScreen
---
# ExtendedSplashScreen

Represents a control, derived from [`LoadingView`](xref:Toolkit.Controls.LoadingView) that displays a view that replicates the appearance and behavior of the target platform's native application loading screen.

Refer to [`LoadingView`](xref:Toolkit.Controls.LoadingView) for a list of inherited members.

## Remarks

The term _splash screen_ commonly refers to a view that is displayed when the user launches the application. Note that only a subset of the supported targets defines a native splash screen construct that is displayed by the platform.

A common use case for this control is to display an application loading element for extended durations after transitioning from the native splash screen. This is useful for scenarios where the application is performing a longer-running operation, such as loading data or performing a network request upon startup.

`ExtendedSplashScreen` is a custom implementation, and does not require the presence of a native splash screen. As such, it can serve as an application's standalone loading screen, regardless of whether the platform provides a native equivalent.

> [!TIP]
> Users expect applications to load quickly after they launch them, so it is important to minimize the duration that a splash screen is displayed. `ExtendedSplashScreen` is intended to support a consistent user experience across platforms where scenarios necessitate a longer startup process.

## Properties

| Property | Type | Description | Value |
|----------|------|-------------|---------------|
| **Platforms** | `SplashScreenPlatform` | Gets or sets the platform(s) where extended splash screen should be used. This is a flag enumeration, which allows for combining multiple values eg: `"Android,iOS"` | Default value is **All**. Other possible values include **Android**, **iOS**, **Windows**, **WebAssembly**, **Skia**, and **None**. |
| **SplashIsEnabled** | `bool` | Gets a value representing whether the current environment is to display this splash screen. | **True** if the current platform is included in the **Platforms** property, otherwise **false**. |

## Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| **Init** | `void` | Initializes the splash screen for the provided `Activity` instance. This static method should be invoked from the **OnCreate** override in `MainActivity`.<br/>**Note: This method only needs to be called on Android** |

## Usage

### Specify platforms to display splash screen

The following code snippet will only display the splash screen on Android and iOS by specifying a `SplashScreenPlatform` value for the `Platforms` property.

```xml
<utu:ExtendedSplashScreen x:Name="Splash"
                          Platforms="Android,iOS" />
```

### Set loading state content

The example below demonstrates a typical use of `ExtendedSplashScreen` in XAML. The [**LoadingContentTemplate**](xref:Toolkit.Controls.LoadingView) property below is inherited from `LoadingView`. This property is used to define the content that will be displayed during the loading/waiting state.

```xml
<!-- xmlns:utu="using:Uno.Toolkit.UI" -->

<utu:ExtendedSplashScreen x:Name="Splash"
                          Platforms="Android,iOS">
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
